using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class ReclassificacaoContabilController : BaseController
    {
        private SAMContext db;
        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;

        // GET: ReclassificacaoContabil
        public ActionResult Index()
        {
            getHierarquiaPerfil();
            CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);

            ReclassificacaoContabilViewModel managerUnit;
            using (db = new SAMContext())
            {
                managerUnit = (from m in db.ManagerUnits
                               where m.BudgetUnitId == _budgetUnitId &&
                                     m.Id == _managerUnitId
                               select new ReclassificacaoContabilViewModel()
                               {
                                   InstitutionId = _institutionId,
                                   BudgetUnitId = m.BudgetUnitId,
                                   ManagerUnitId = m.Id
                               }).AsNoTracking().FirstOrDefault();
            }

            return View(managerUnit);
        }

        private List<AuxiliaryAccount> ListaAsContaContabil(int grupoMaterial) {
            return (from r in db.RelationshipAuxiliaryAccountItemGroups
                    join a in db.AuxiliaryAccounts on r.AuxiliaryAccountId equals a.Id
                    join g in db.MaterialGroups on r.MaterialGroupId equals g.Id
                    where g.Code == grupoMaterial
                    select a).ToList();
        }

        #region Métodos privados
        private void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
        }

        private void CarregaHierarquiaFiltro(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, bool report = false, string mesRef = null)
        {
            Hierarquia hierarquia = new Hierarquia();
            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");

                if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");
            }
            else
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);

                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);

                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId), "Id", "Description", modelManagerUnitId);
                else if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId), "Id", "Description", modelManagerUnitId);
            }
        }

        public AssetMovements MovimentacaoMaisRecenteDosBPs(List<int> AssetIds) {
            return (from am in db.AssetMovements
                    where AssetIds.Contains(am.AssetId)
                    select am).OrderByDescending(am => am.MovimentDate).FirstOrDefault();
        }

        public bool MovimentacaoMaisRecenteAntesDoMesRefDaUGE(DateTime UltimoMovimento, int IdUGE)
        {
            if (UltimoMovimento == null || IdUGE <= 0) return false;

            DateTime UltimoDiaMesRef = MesRefDaUGE(IdUGE);
            UltimoDiaMesRef = UltimoDiaMesRef.AddMonths(1).AddDays(-1);
            return UltimoMovimento <= UltimoDiaMesRef;
        }

        private DateTime MesRefDaUGE(int UGE)
        {
            var uge = (from m in db.ManagerUnits where m.Id == UGE select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

            if (uge == null) return new DateTime(1900, 1, 1);

            //DateTime com o último dia do MesRef UGE
            string ano = uge.Substring(0, 4);
            string mes = uge.Substring(4, 2);

            return new DateTime(Convert.ToInt32(ano), Convert.ToInt32(mes), 1);
        }

        private DateTime DataParaMovimento(int UGE)
        {
            var uge = (from m in db.ManagerUnits where m.Id == UGE select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

            //DateTime com o último dia do MesRef UGE
            string ano = uge.Substring(0, 4);
            string mes = uge.Substring(4, 2);

            DateTime _DataParaMovimento = DateTime.Now;

            // Caso o MesRef seja igual ao mes corrente, grava o movimento com a data atual, caso seja menor grava com o ultimo dia do MesRef
            if (!(DateTime.Now.Month == int.Parse(mes) && DateTime.Now.Year == int.Parse(ano)))
                _DataParaMovimento = new DateTime(int.Parse(ano), int.Parse(mes), DateTime.DaysInMonth(int.Parse(ano), int.Parse(mes)));

            return _DataParaMovimento;
        }

        private AssetMovements CriaObjAssetMovementsReclassificacao(AssetMovements am)
        {
            AssetMovements reclassificacao = new AssetMovements();
            reclassificacao.AssetId = am.AssetId;
            reclassificacao.MovementTypeId = (int)EnumMovimentType.ReclassificacaoContaContabil;
            reclassificacao.StateConservationId = am.StateConservationId;
            reclassificacao.NumberPurchaseProcess = am.NumberPurchaseProcess;
            reclassificacao.InstitutionId = am.InstitutionId ;
            reclassificacao.BudgetUnitId = am.BudgetUnitId;
            reclassificacao.ManagerUnitId = am.ManagerUnitId;
            reclassificacao.AdministrativeUnitId = am.AdministrativeUnitId;
            reclassificacao.SectionId = am.SectionId;
            reclassificacao.ResponsibleId = am.ResponsibleId;
            reclassificacao.SourceDestiny_ManagerUnitId = null;
            reclassificacao.ExchangeId = null;
            reclassificacao.ExchangeDate = null;
            reclassificacao.ExchangeUserId = null;
            reclassificacao.TypeDocumentOutId = null;
            reclassificacao.ContaContabilAntesDeVirarInservivel = am.ContaContabilAntesDeVirarInservivel;
            reclassificacao.AssetTransferenciaId = null;
            reclassificacao.Observation = null;
            reclassificacao.Status = true;
            reclassificacao.CPFCNPJ = am.CPFCNPJ;
            reclassificacao.DataLogin = DateTime.Now;

            return reclassificacao;
        }

        private string BuscaNumeroDocumento(int IdUGE) {
            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA] @ManagerUnitId = " + IdUGE.ToString());
            builder.Append(",@Ano = " + DateTime.Now.Year.ToString());

            string Sequencia = db.Database.SqlQuery<string>(builder.ToString()).FirstOrDefault();
            string retorno;

            if (Sequencia.Equals("0"))
            {
                string codigoUGE = (from m in db.ManagerUnits where m.Id == IdUGE select m.Code).FirstOrDefault();
                retorno = DateTime.Now.Year + codigoUGE + "0001";
            }
            else
            {
                if (Sequencia.Length > 14)
                {
                    int contador = Convert.ToInt32(Sequencia.Substring(10, Sequencia.Length - 10));

                    if (contador < 9999)
                    {
                        retorno = Sequencia.Substring(0, 10) + (contador + 1).ToString().PadLeft(4, '0');
                    }
                    else
                    {
                        retorno = (long.Parse(Sequencia) + 1).ToString();
                    }
                }
                else
                    retorno = (long.Parse(Sequencia) + 1).ToString();
            }

            return retorno;
        }

        private void ComputaAlteracaoContabil(int IdUGE, List<int?> IdsContaContabil) {

            foreach (int IdContaContabil in IdsContaContabil)
            {
                 var IdUO = db.ManagerUnits.Where(m => m.Id == IdUGE).FirstOrDefault().BudgetUnitId;
                 var IdOrgao = db.BudgetUnits.Where(m => m.Id == IdUO).FirstOrDefault().InstitutionId;

                 var registro = db.HouveAlteracaoContabeis
                                      .Where(h => h.IdOrgao == IdOrgao &&
                                                  h.IdUO == IdUO &&
                                                  h.IdUGE == IdUGE &&
                                                  h.IdContaContabil == IdContaContabil)
                                       .FirstOrDefault();

                 if (registro != null)
                 {
                     registro.HouveAlteracao = true;
                     db.Entry(registro).State = EntityState.Modified;
                 }
                 else
                 {
                     registro = new HouveAlteracaoContabil();
                     registro.IdOrgao = IdOrgao;
                     registro.IdUO = IdUO;
                     registro.IdUGE = IdUGE;
                     registro.IdContaContabil = IdContaContabil;
                     registro.HouveAlteracao = true;
                     db.Entry(registro).State = EntityState.Added;
                 }

                 db.SaveChanges();
            }

        }
        #endregion

        [HttpPost]
        public JsonResult CarregaCampoGrupo(int numeroUGE) {
            List<MaterialGroup> listaGrupoMaterial;

            using (db = new SAMContext())
            {
                var ListaGrupoMateriaisDosBP = db.BPsARealizaremReclassificacaoContabeis
                                                 .Where(bp => bp.IdUGE == numeroUGE)
                                                 .Select(bp => bp.GrupoMaterial).Distinct().ToList();
                if(ListaGrupoMateriaisDosBP.Count == 0)
                    return Json(new { semGrupos = true}, JsonRequestBehavior.AllowGet);

                listaGrupoMaterial = db.MaterialGroups
                                           .Where(g => ListaGrupoMateriaisDosBP.Contains(g.Code))
                                           .ToList();

                return Json(listaGrupoMaterial.Select(s => new { Codigo = s.Code, Descricao = s.Description })
                            , JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult BuscaBPS() {
            try
            {
                string draw = Request.Form["draw"].ToString();
                int startRec = Convert.ToInt32(Request.Form["start"].ToString());

                int length = 10;

                int grupo = Convert.ToInt32(Request.Form["grupoMaterial"].ToString());
                int IdUGE = Convert.ToInt32(Request.Form["numeroUGE"].ToString());
                string pesquisa = Request.Form["pesquisa"].ToString();

                List<ReclassificacaoContabilViewModel> lista;
                int totalRegistros = 0;

                using (db = new SAMContext())
                {
                    lista = (from bp in db.BPsARealizaremReclassificacaoContabeis
                             join a in db.Assets on bp.AssetId equals a.Id
                             join i in db.Initials on a.InitialId equals i.Id
                             join s in db.ShortDescriptionItens on a.ShortDescriptionItemId equals s.Id
                             join am in db.AssetMovements on a.Id equals am.AssetId
                             join conta in db.AuxiliaryAccounts on am.AuxiliaryAccountId equals conta.Id
                             where bp.IdUGE == IdUGE && bp.GrupoMaterial == grupo && am.Status
                             select new ReclassificacaoContabilViewModel
                             {
                                 Id = a.Id,
                                 Sigla = i.Name,
                                 Chapa = a.NumberIdentification,
                                 Descricao = s.Description,
                                 ContaContabilAtual = conta.ContaContabilApresentacao
                             }).ToList();
                }

                if (!string.IsNullOrEmpty(pesquisa) && !string.IsNullOrWhiteSpace(pesquisa))
                {
                    lista = lista.Where(l => l.Chapa.Contains(pesquisa) ||
                                             l.Sigla.Contains(pesquisa) ||
                                             l.Descricao.Contains(pesquisa) ||
                                             l.ContaContabilAtual.Contains(pesquisa)).ToList();
                }

                totalRegistros = lista.Count();

                var result = lista.OrderByDescending(s => s.Id).Skip(startRec).Take(length).ToList();

                return Json(new { draw = draw, recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                base.GravaLogErroSemRetorno(e);
                return Json(new { draw = 0, recordsTotal = 0, recordsFiltered = 0, data = new List<ReclassificacaoContabilViewModel>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EscolheContaContabilBPsEscolhidos(List<int> lista) {
            List<ReclassificaoEscolherContaViewModel> listaBPs;

            try {
                using (db = new SAMContext())
                {
                    listaBPs = (from bp in db.BPsARealizaremReclassificacaoContabeis
                                join a in db.Assets on bp.AssetId equals a.Id
                                join i in db.Initials on a.InitialId equals i.Id
                                join am in db.AssetMovements on a.Id equals am.AssetId
                                where lista.Contains(a.Id) && am.Status
                                select new ReclassificaoEscolherContaViewModel
                                {
                                    Sigla = i.Name,
                                    Chapa = a.NumberIdentification,
                                    DataMovimentacao = am.MovimentDate,
                                    IdUGE = am.ManagerUnitId,
                                    GrupoMaterial = a.MaterialGroupCode,
                                    ValorDeAquisicao = a.ValueAcquisition,
                                    AssetStartId = a.AssetStartId,
                                    MaterialItemCode = a.MaterialItemCode
                                }).ToList();

                    if (listaBPs.Count != 0)
                    {
                        var BPComUltimaMovimentacao = MovimentacaoMaisRecenteDosBPs(lista);

                        if (MovimentacaoMaisRecenteAntesDoMesRefDaUGE(BPComUltimaMovimentacao.MovimentDate, BPComUltimaMovimentacao.ManagerUnitId))
                        {
                            listaBPs.FirstOrDefault().DataMovimentacao = DataParaMovimento(BPComUltimaMovimentacao.ManagerUnitId);
                            var listaContas = ListaAsContaContabil(listaBPs.FirstOrDefault().GrupoMaterial);

                            listaBPs.FirstOrDefault().GrupoPossuiMaisDeUmaConta = (listaContas.Count > 1);
                            if (listaContas.Count > 1)
                            {
                                listaContas.ForEach(l => l.Description = l.ContaContabilApresentacao + " - " + l.Description);
                                ViewBag.AuxiliaryAccounts = new SelectList(listaContas, "Id", "Description");
                            }
                            else
                            {
                                listaBPs.FirstOrDefault().IdContaContabil = listaContas.FirstOrDefault().Id;
                                listaBPs.FirstOrDefault().DescricaoContaContabil = listaContas.FirstOrDefault().ContaContabilApresentacao + " - " + listaContas.FirstOrDefault().Description;
                            }

                            var ano = DateTime.Now.Year;
                            var mes = DateTime.Now.Year;

                            decimal totalValorAquisicao = 0;
                            decimal totalDepreciacaoAcumulada = 0;
                            List<decimal> listaDepreciacoesAcumuladas;

                            listaBPs.ForEach(item =>
                            {
                                if (item.AssetStartId != null)
                                {

                                    listaDepreciacoesAcumuladas = (from m in db.MonthlyDepreciations
                                                                   where m.AssetStartId == item.AssetStartId &&
                                                                   m.ManagerUnitId == item.IdUGE &&
                                                                   m.MaterialItemCode == item.MaterialItemCode &&
                                                                   m.CurrentValue <= m.ValueAcquisition &&
                                                                   (m.CurrentDate.Year < ano
                                                                   || m.CurrentDate.Year == ano
                                                                   && m.CurrentDate.Month <= mes)
                                                                   select m.AccumulatedDepreciation).ToList();

                                    if (listaDepreciacoesAcumuladas == null || listaDepreciacoesAcumuladas.Count == 0)
                                        item.DepreciacaoAcmumulada = 0;
                                    else
                                        item.DepreciacaoAcmumulada = listaDepreciacoesAcumuladas.Max();

                                    totalValorAquisicao += item.ValorDeAquisicao;
                                    totalDepreciacaoAcumulada += item.DepreciacaoAcmumulada;
                                }
                            });

                            listaBPs.FirstOrDefault().totalValorAquisicao = totalValorAquisicao;
                            listaBPs.FirstOrDefault().totalDepreciacaoAcumulada = totalDepreciacaoAcumulada;


                        }
                        else {
                            string mes = BPComUltimaMovimentacao.MovimentDate.Month.ToString();
                            string ano = BPComUltimaMovimentacao.MovimentDate.Year.ToString();

                            return PartialView("_Mensagem", model: "Não será possível fazer a reclassificação dos BPs escolhidos, pois a data do último histórico relacionados desse BP foi realizado no mês de " + mes + "/" + ano + ". Recomendamos que a UGE esteja nesse mês de referência para fazer essa reclassificação");
                        }
                    }
                }

                if (listaBPs.Count != 0)
                {
                    return PartialView("_EscolherContaContabil", listaBPs);
                }
                else
                {
                    return Json(new { naoEncontrado = true }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                base.GravaLogErroSemRetorno(e);
                return Json(new { erro = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EscolheContaContabilBPsTodos(int grupoMaterial, int numeroUGE)
        {
            List<ReclassificaoEscolherContaViewModel> listaBPs;
            try
            {
                using (db = new SAMContext())
                {
                    listaBPs = (from bp in db.BPsARealizaremReclassificacaoContabeis
                                join a in db.Assets on bp.AssetId equals a.Id
                                join i in db.Initials on a.InitialId equals i.Id
                                join am in db.AssetMovements on a.Id equals am.AssetId
                                where am.ManagerUnitId == numeroUGE
                                && a.MaterialGroupCode == grupoMaterial
                                && am.Status
                                select new ReclassificaoEscolherContaViewModel
                                {
                                    Sigla = i.Name,
                                    Chapa = a.NumberIdentification,
                                    DataMovimentacao = am.MovimentDate,
                                    IdUGE = am.ManagerUnitId,
                                    GrupoMaterial = a.MaterialGroupCode,
                                    ValorDeAquisicao = a.ValueAcquisition,
                                    AssetStartId = a.AssetStartId,
                                    MaterialItemCode = a.MaterialItemCode,
                                    AssetId = a.Id
                                }).ToList();

                    if (listaBPs.Count != 0)
                    {
                        var BPComUltimaMovimentacao = MovimentacaoMaisRecenteDosBPs(listaBPs.Select(l => l.AssetId).ToList());
                        if (MovimentacaoMaisRecenteAntesDoMesRefDaUGE(BPComUltimaMovimentacao.MovimentDate, BPComUltimaMovimentacao.ManagerUnitId))
                        {
                            listaBPs.FirstOrDefault().DataMovimentacao = DataParaMovimento(BPComUltimaMovimentacao.ManagerUnitId);
                            var lista = ListaAsContaContabil(grupoMaterial);

                            listaBPs.FirstOrDefault().GrupoPossuiMaisDeUmaConta = (lista.Count > 1);
                            if (lista.Count > 1)
                            {
                                lista.ForEach(l => l.Description = l.ContaContabilApresentacao + " - " + l.Description);
                                ViewBag.AuxiliaryAccounts = new SelectList(lista, "Id", "Description");
                            }
                            else
                            {
                                listaBPs.FirstOrDefault().IdContaContabil = lista.FirstOrDefault().Id;
                                listaBPs.FirstOrDefault().DescricaoContaContabil = lista.FirstOrDefault().ContaContabilApresentacao + " - " + lista.FirstOrDefault().Description;
                            }

                            var ano = DateTime.Now.Year;
                            var mes = DateTime.Now.Year;

                            decimal totalValorAquisicao = 0;
                            decimal totalDepreciacaoAcumulada = 0;
                            List<decimal> listaDepreciacoesAcumuladas;

                            listaBPs.ForEach(item =>
                            {
                                if (item.AssetStartId != null)
                                {
                                    listaDepreciacoesAcumuladas = (from m in db.MonthlyDepreciations
                                                                   where m.AssetStartId == item.AssetStartId &&
                                                                   m.ManagerUnitId == item.IdUGE &&
                                                                   m.MaterialItemCode == item.MaterialItemCode &&
                                                                   m.CurrentValue <= m.ValueAcquisition &&
                                                                   (m.CurrentDate.Year < ano
                                                                   || m.CurrentDate.Year == ano
                                                                   && m.CurrentDate.Month <= mes)
                                                                   select m.AccumulatedDepreciation).ToList();

                                    if (listaDepreciacoesAcumuladas == null || listaDepreciacoesAcumuladas.Count == 0)
                                        item.DepreciacaoAcmumulada = 0;
                                    else
                                        item.DepreciacaoAcmumulada = listaDepreciacoesAcumuladas.Max();

                                    totalValorAquisicao += item.ValorDeAquisicao;
                                    totalDepreciacaoAcumulada += item.DepreciacaoAcmumulada;
                                }
                            });

                            listaBPs.FirstOrDefault().totalValorAquisicao = totalValorAquisicao;
                            listaBPs.FirstOrDefault().totalDepreciacaoAcumulada = totalDepreciacaoAcumulada;
                        }
                        else
                        {

                            string mes = BPComUltimaMovimentacao.MovimentDate.Month.ToString();
                            string ano = BPComUltimaMovimentacao.MovimentDate.Year.ToString();

                            return PartialView("_Mensagem", model: "Não será possível fazer a reclassificação dos BPs escolhidos, pois a data do último histórico relacionados desse BP foi realizado no mês de " + mes + "/" + ano + ". Recomendamos que a UGE esteja nesse mês de referência para fazer essa reclassificação");
                        }
                    }
                }

                if (listaBPs.Count != 0)
                {
                    return PartialView("_EscolherContaContabil", listaBPs);
                }
                else
                {
                    return Json(new { naoEncontrado = true }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e) {
                base.GravaLogErroSemRetorno(e);
                return Json(new { erro = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RealizaReclassificacao(List<int> lista, int NovaConta)
        {
            try
            {
                using (db = new SAMContext())
                {
                    var listaHistoricoAtivosDosBPs = (from r in db.BPsARealizaremReclassificacaoContabeis
                                                      join a in db.Assets on r.AssetId equals a.Id
                                                      join am in db.AssetMovements on a.Id equals am.AssetId
                                                      where lista.Contains(a.Id) && am.Status == true
                                                      select am).ToList();

                    if (listaHistoricoAtivosDosBPs.Count == 0)
                        return Json(new { msg = "Os Bps não foram encontrados ativos nessa UGE. A página será recarregada" });

                    var listaContaContabeis = listaHistoricoAtivosDosBPs.Select(m => m.AuxiliaryAccountId == null ? 0 : m.AuxiliaryAccountId).Distinct().ToList();
                    listaContaContabeis.Add(NovaConta);

                    int IdUGE = listaHistoricoAtivosDosBPs.FirstOrDefault().ManagerUnitId;
                    ComputaAlteracaoContabil(IdUGE, listaContaContabeis);

                    AssetMovements novoHistorico;
                    string numeroDocumento = BuscaNumeroDocumento(IdUGE);
                    DateTime DataMovimentacao = DataParaMovimento(IdUGE);

                    User u = UserCommon.CurrentUser();

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        foreach (AssetMovements historico in listaHistoricoAtivosDosBPs)
                        {
                            historico.Status = false;
                            db.Entry(historico).State = EntityState.Modified;
                            db.SaveChanges();

                            novoHistorico = CriaObjAssetMovementsReclassificacao(historico);
                            novoHistorico.NumberDoc = numeroDocumento;
                            novoHistorico.AuxiliaryAccountId = NovaConta;
                            novoHistorico.MovimentDate = DataMovimentacao;
                            novoHistorico.Login = u.CPF;

                            db.Entry(novoHistorico).State = EntityState.Added;
                            db.SaveChanges();

                            db.Database.ExecuteSqlCommand("delete from [dbo].[BPsARealizaremReclassificacaoContabil] where AssetId = " + historico.AssetId.ToString());
                        }

                        transaction.Complete();
                    }
                }

                return Json(new { msg = "Reclassificação realizada com sucesso somente no Patrimonio. (Recomendamos que o mesmo processo seja realizado no SIAFEM)" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                base.GravaLogErroSemRetorno(e);
                return Json(new { msg = "Não foi possível fazer a reclassificação do BPs no momento. A página será recarregada. Por gentileza, tente novamente mais tarde." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RealizaReclassificacaoTodos(int NovaConta, int numeroUGE, int grupoMaterial)
        {
            try
            {
                using (db = new SAMContext())
                {
                    var listaHistoricoAtivosDosBPs = (from r in db.BPsARealizaremReclassificacaoContabeis
                                                      join a in db.Assets on r.AssetId equals a.Id
                                                      join am in db.AssetMovements on a.Id equals am.AssetId
                                                      where r.IdUGE == numeroUGE && r.GrupoMaterial == grupoMaterial && am.Status
                                                      select am).ToList();

                    if (listaHistoricoAtivosDosBPs.Count == 0)
                        return Json(new { msg = "Os Bps não foram encontrados ativos nessa UGE. A página será recarregada" });

                    bool relacaoValida = (from r in db.RelationshipAuxiliaryAccountItemGroups
                                          join g in db.MaterialGroups on r.MaterialGroupId equals g.Id
                                          where g.Code == grupoMaterial && r.AuxiliaryAccountId == NovaConta
                                          select r.Id).Count() > 0;

                    if (!relacaoValida)
                        return Json(new { msg = "Relação inválida. A página será recarregada." }, JsonRequestBehavior.AllowGet);

                    var listaContaContabeis = listaHistoricoAtivosDosBPs.Select(m => m.AuxiliaryAccountId == null ? 0 : m.AuxiliaryAccountId).Distinct().ToList();
                    listaContaContabeis.Add(NovaConta);

                    ComputaAlteracaoContabil(numeroUGE, listaContaContabeis);

                    AssetMovements novoHistorico;
                    string numeroDocumento = BuscaNumeroDocumento(numeroUGE);
                    DateTime DataMovimentacao = DataParaMovimento(numeroUGE);

                    User u = UserCommon.CurrentUser();

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        foreach (AssetMovements historico in listaHistoricoAtivosDosBPs)
                        {
                            historico.Status = false;
                            db.Entry(historico).State = EntityState.Modified;
                            db.SaveChanges();

                            novoHistorico = CriaObjAssetMovementsReclassificacao(historico);
                            novoHistorico.NumberDoc = numeroDocumento;
                            novoHistorico.AuxiliaryAccountId = NovaConta;
                            novoHistorico.MovimentDate = DataMovimentacao;
                            novoHistorico.Login = u.CPF;

                            db.Entry(novoHistorico).State = EntityState.Added;
                            db.SaveChanges();

                            db.Database.ExecuteSqlCommand("delete from [dbo].[BPsARealizaremReclassificacaoContabil] where AssetId = " + historico.AssetId.ToString());
                        }

                        transaction.Complete();
                    }
                }

                return Json(new { msg = "Reclassificação realizada com sucesso somente no Patrimonio. (Recomendamos que o mesmo processo seja realizado no SIAFEM)" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                base.GravaLogErroSemRetorno(e);
                return Json(new { msg = "Não foi possível fazer a reclassificação do BPs no momento. A página será recarregada. Por gentileza, tente novamente mais tarde." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}