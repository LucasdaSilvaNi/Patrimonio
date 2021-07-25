using Microsoft.Reporting.WebForms;
using PagedList;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;





namespace SAM.Web.Controllers
{
    public static class DataRowExtensions
    {
        internal static string ObterValor(this DataRow drLinhaTabela, string nomeColuna)
        {
            string strValorRetorno = null;

            strValorRetorno = ((!drLinhaTabela.HasErrors && !Convert.IsDBNull(drLinhaTabela[nomeColuna])) ? drLinhaTabela[nomeColuna].ToString() : null);


            return strValorRetorno;
        }
    }

    public class InventariosController : BaseController
    {
        const string fmtMsgEstimuloInventarioWs = "{{ \"ResponsavelId\":\"{0}\", \"UaId\":\"{1}\", \"UgeId\":\"{2}\", \"Usuario\":\"{3}\", \"OrigemDados\":\"{4}\", \"TipoInventario\":\"{5}\" }}";
        const string fmtMsgEstimuloItemInventarioWs = "{{ \"Code\":\"{0}\", \"Estado\":\"{1}\", \"Item\":\"{2}\", \"InitialName\":\"{3}\", \"InventarioId\":\"{4}\", \"AssetId\":\"{5}\" }}";


        private SAMContext db = new SAMContext();
        private StructuresController structures = new StructuresController();
        //private RelationshipUserProfile rup = new RelationshipUserProfile();
        private Hierarquia hierarquia = new Hierarquia();
        private FunctionsCommon common = new FunctionsCommon();

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;
        private string _login;


        // GET: Inventarios
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                getHierarquiaPerfil();

                IQueryable<Inventario> lstRetorno;

                if (ContemValor(_sectionId))
                    lstRetorno = (from s in db.Inventarios where s.DivisaoId == _sectionId select s).AsNoTracking();
                else if (ContemValor(_administrativeUnitId))
                    lstRetorno = (from s in db.Inventarios where s.UaId == _administrativeUnitId select s).AsNoTracking();
                else if (ContemValor(_managerUnitId))
                    lstRetorno = (from s in db.Inventarios where s.RelatedAdministrativeUnit.RelatedManagerUnit.Id == _managerUnitId select s).AsNoTracking();
                else if (ContemValor(_budgetUnitId))
                    lstRetorno = (from s in db.Inventarios where s.RelatedAdministrativeUnit.RelatedManagerUnit.BudgetUnitId == _budgetUnitId select s).AsNoTracking();
                else if (ContemValor(_institutionId))
                    lstRetorno = (from s in db.Inventarios where s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId == _institutionId select s).AsNoTracking();
                else
                    lstRetorno = (from s in db.Inventarios select s).AsNoTracking();

                ViewBag.CurrentFilter = searchString;
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var lstFiltro = SearchByFilter(searchString, lstRetorno);

                var result = lstFiltro.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);

                var retorno = new StaticPagedList<Inventario>(result, pageNumber, pageSize, lstFiltro.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Assets/Create
        public ActionResult Create()
        {
            try
            {
                SetCarregaHierarquia();
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventarioViewModel inventarioViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Inventario _inventario = new Inventario();

                    _inventario.Descricao = inventarioViewModel.Descricao;
                    _inventario.DataInventario = DateTime.Now;
                    _inventario.UaId = inventarioViewModel.UaId;
                    _inventario.DivisaoId = inventarioViewModel.DivisaoId;
                    _inventario.UserId = inventarioViewModel.UserId;
                    _inventario.Usuario = UserCommon.CurrentUser().CPF;
                    //_inventario.Status = "0";
                    _inventario.Status = EnumStatusInventario.Pendente.GetHashCode().ToString();

                    db.Inventarios.Add(_inventario);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                SetCarregaHierarquia(inventarioViewModel.InstitutionId, inventarioViewModel.BudgetUnitId, inventarioViewModel.ManagerUnitId, inventarioViewModel.UaId, inventarioViewModel.DivisaoId);
                return View(inventarioViewModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Inventario _inventario = db.Inventarios.Find(id);
                if (_inventario == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(_inventario);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Inventario _inventario = db.Inventarios.Find(id);
                List<ItemInventario> lstItemInventario = (from i in db.ItemInventarios where i.InventarioId == id select i).ToList();

                if (lstItemInventario.Any())
                {
                    using (var _transacation = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.ItemInventarios.RemoveRange(lstItemInventario);
                            db.SaveChanges();

                            db.Inventarios.Remove(_inventario);
                            db.SaveChanges();

                            _transacation.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception ex)
                        {
                            _transacation.Rollback();
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    db.Inventarios.Remove(_inventario);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        public void getHierarquiaPerfil()
        {
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
                _login = u.CPF;
            }
            else {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
                _login = (string)HttpContext.Items["CPF"];
            }
        }

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int? modelSectionId = 0, int modelResponsibleId = 0)
        {
            

            var vUser = new List<User>();
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

                if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(null), "Id", "Description");

                if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(null), "Id", "Description");
            }
            else
            {
                getHierarquiaPerfil();

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

                if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(_administrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
                else if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(_managerUnitId), "Id", "Description", modelAdministrativeUnitId);

                if (_sectionId.HasValue && _sectionId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(_sectionId), "Id", "Description", modelSectionId);
                else if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(_administrativeUnitId), "Id", "Description", modelSectionId);
            }

            #region Combo Responsavel

            if (ContemValor(_administrativeUnitId))
            {
                vUser = (from r in db.Users
                         join u in db.RelationshipUserProfiles on r.Id equals u.UserId
                         join p in db.RelationshipUserProfileInstitutions on u.Id equals p.RelationshipUserProfileId
                         where r.Status == true && u.FlagResponsavel == true && p.AdministrativeUnitId == _administrativeUnitId
                         select r).ToList();
                ViewBag.User = new SelectList(vUser.ToList(), "Id", "Name");
            }
            else if (modelAdministrativeUnitId != 0)
            {
                vUser = (from r in db.Users
                         join u in db.RelationshipUserProfiles on r.Id equals u.UserId
                         join p in db.RelationshipUserProfileInstitutions on u.Id equals p.RelationshipUserProfileId
                         where r.Status == true && u.FlagResponsavel == true && p.AdministrativeUnitId == modelAdministrativeUnitId
                         select r).ToList();
                ViewBag.User = new SelectList(vUser.ToList(), "Id", "Name", modelResponsibleId);
            }
            else
                ViewBag.User = new SelectList(vUser.ToList(), "Id", "Name");

            #endregion
        }

        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null)
                retorno = true;
            return retorno;
        }

        public JsonResult VerificarInventario(int InventarioId, int? page)
        {
            int?[] statusPossiveisParaFechamentoInventarioManual = new int?[] {   EnumSituacaoFisicaBP.OK.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Incorporado.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Movimentado.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Devolvido.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Transferido.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Inventariado_Manualmente.GetHashCode()
                                                                            };

            MensagemModel mensagem = new MensagemModel();
            mensagem.Id = 0;
            mensagem.Mensagem = "";

            List<MensagemModel> mensagens = new List<MensagemModel>();

            var countItens = (from s in db.ItemInventarios where s.InventarioId == InventarioId select s).Count();
            if (countItens < 1)
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "Não existe nenhum item para este inventário, favor verificar";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            //var result = (from s in db.ItemInventarios where s.InventarioId == InventarioId && s.Estado != 1 select s).ToList();
            var result = db.ItemInventarios
                           .Where(itemInventario => itemInventario.InventarioId == InventarioId && (itemInventario.Estado.HasValue 
                                                 && !statusPossiveisParaFechamentoInventarioManual.Contains(itemInventario.Estado.Value)))
                           .ToList();
            if (result.Count == 0)
            {
                mensagem.Id = 1;
                mensagem.Mensagem = "Deseja Finalizar o Inventário?";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            else
            {
                mensagem.Id = 2;
                mensagem.Mensagem = "Existem Itens pendentes para este inventário, favor verificar nos detalhes antes de finalizar o Inventário!";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FinalizarInventario(int InventarioId)
        {
            Inventario _inventario = (from a in db.Inventarios where a.Id == InventarioId select a).ToList().FirstOrDefault();

            //_inventario.Status = "1";
            _inventario.Status = EnumStatusInventario.Finalizado.GetHashCode().ToString(); 
            db.Entry(_inventario).State = EntityState.Modified;
            db.SaveChanges();

            return Json("Inventário finalizado com sucesso!", JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Inventario> SearchByFilter(string searchString, IQueryable<Inventario> result)
        {
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrWhiteSpace(searchString))
            {
                string pendente = "pendente", finalizado = "finalizado";
                searchString = searchString.Trim();
                result = result.Where(s => s.Descricao.Contains(searchString) ||
                                           s.RelatedAdministrativeUnit.RelatedManagerUnit.Code.ToString().Contains(searchString) ||
                                           s.RelatedAdministrativeUnit.Code.ToString().Contains(searchString) ||
                                           s.RelatedAdministrativeUnit.Description.Contains(searchString) ||
                                           s.RelatedResponsible.Name.Contains(searchString) ||
                                           (s.Status == "0" ? pendente.Contains(searchString) : finalizado.Contains(searchString)));
            }
            return result;
        }

        #region Relatorio
        [HttpGet]
        public ActionResult ReportInventarioManual()
        {

            InventarioViewModel retorno = new InventarioViewModel();
            var vResponsible = new List<Responsible>();

            SetCarregaHierarquia();

            #region Combo Responsavel


            vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == _administrativeUnitId select r).ToList();

            vResponsible = vResponsible.Distinct().OrderBy(a => a.Name).ToList();

            ViewBag.User = new SelectList(vResponsible, "Id", "Name");


            #endregion


            return View(retorno);
        }

        [HttpGet]
        public JsonResult SetReportInventarioManual(int orgaoId, int uoId, int ugeId, int uaId, int responsavelId, int divisaoId)
        {
            JsonResult acaoRetorno = null;
            string _mensagemParaUsuario = null;
            DataTable dtDadosRelatorio = null;


            try
            {
                //if (objViewPagina.InstitutionId != 0 && objViewPagina.BudgetUnitId != 0 && objViewPagina.ManagerUnitId != 0 && objViewPagina.AdministrativeUnitId != 0 && objViewPagina.idResponsable != 0 && objViewPagina.idResponsable != null)
                if (orgaoId != 0 && uoId != 0 && ugeId != 0 && uaId != 0 && responsavelId != 0)
                {
                    //if (ExisteInventarioManualPendenteParaResponsavel(objViewPagina.idResponsable.Value))
                    if (ExisteInventariosPendentesParaResponsavel(responsavelId))
                    {
                        //string termoInicioCorreto = null;
                        //string termoCorreto = null;
                        int numeroBPsInventariados = -1;
                        int numeroTotalBPsInventario = -1;

                        numeroBPsInventariados = ObterNumeroBPsVerificadosEmInventarioAtualPendente(responsavelId);
                        numeroTotalBPsInventario = ObterNumeroTotalBPsInventarioAtualPendente(responsavelId);

                        //termoInicioCorreto = ((numeroBPsInventariados == 1) ? "Existe" : "Existem");

                        _mensagemParaUsuario = String.Format("Inventário do tipo 'Inventário Manual' já existente para este Responsavel.<br>Existe(m) {0:#,#00} BP(s) verificado(s) de um total de {1:#,#00} existente(s) neste inventário.<br><br>Para iniciar um novo inventário, o inventário atual em andamento deverá ser excluído!",
                        //    termoInicioCorreto, numeroBPsInventariados, numeroTotalBPsInventario);
                            numeroBPsInventariados, numeroTotalBPsInventario);
                        //return MensagemErroJson(_mensagemParaUsuario);
                        acaoRetorno = MensagemErroJson(_mensagemParaUsuario);
                    }
                    else
                    {
                        List<ReportParameter> parametros = new List<ReportParameter>();

                        string path = PathDoTipoDeRelatorio();
                        LocalReport lr = SetaPathRelatorio(path);


                        dtDadosRelatorio = RetornaDadosRelatorio(orgaoId, uoId, ugeId, uaId, responsavelId, divisaoId);
                        //var institution = (from a in db.Institutions where a.Id == orgaoId select a).FirstOrDefault();
                        //var budgetUnit = (from a in db.BudgetUnits where a.Id == uoId select a).FirstOrDefault();
                        //var managerUnit = (from a in db.ManagerUnits where a.Id == ugeId select a).FirstOrDefault();
                        //var administrativeUnit = (from a in db.AdministrativeUnits where a.Id == uaId select a).FirstOrDefault();
                        //var section = (from a in db.Sections where a.Id == divisaoId select a).FirstOrDefault();
                        //var responsable = (from a in db.Responsibles where a.Id == responsavelId select a).FirstOrDefault();





                        if (dtDadosRelatorio.Rows.Count > 0)//|| !dadosInventarioManualGravados)
                        {
                            #region Inventario Manual
                            IList<KeyValuePair<string, string>> dadosUsuarioRelatorio = obterDadosUsuarioLogado(orgaoId, ugeId, uaId, responsavelId, divisaoId);
                            var dadosInventarioManualGravados = ProcessaSaidaRelatorioInventario(dadosUsuarioRelatorio, dtDadosRelatorio);
                            #endregion


                            string termoCorreto = ((dtDadosRelatorio.Rows.Count == 1) ? "Bem Patrimonial vinculado" : "Bens Patrimoniais vinculados");
                            _mensagemParaUsuario = String.Format("Criado Inventário do tipo 'Inventário Manual' na tela 'Resultado do Inventário' com {0:#,#00} {1} com status 'pendente'.", dtDadosRelatorio.Rows.Count, termoCorreto);
                            //return Json(new { _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                            acaoRetorno = Json(new MensagemModel() { Mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    acaoRetorno = MensagemErroJson(CommonMensagens.PadraoException, new Exception("Não há dados para este responsável!"));
                }

                //_mensagemParaUsuario = String.Format("Criado Inventário do tipo 'Inventário Manual' na tela 'Resultado do Inventário' com {0} Bens Patrimoniais vinculados com status 'pendente'.", dtDadosRelatorio.Rows.Count);
                //return Json(new { _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                //return Json(acaoRetorno, JsonRequestBehavior.AllowGet);
                return acaoRetorno;
            }
            catch (Exception ex)
            {
                //return MensagemErro(CommonMensagens.PadraoException, ex);
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        //public JsonResult ReportInventarioManual(InventarioViewModel objViewPagina)
        public ActionResult GeraRelatorioInventarioManual(int orgaoID, int uoID, int ugeID, int uaID, int responsavelID, int divisaoID)
        {
            string _mensagemParaUsuario = null;
            DataTable dtDadosRelatorio = null;

            try
            {
                if (orgaoID != 0 && uoID != 0 && ugeID != 0 && uaID != 0 && responsavelID != 0)
                {
                    if (ExisteInventariosPendentesParaResponsavel(responsavelID))
                    {
                        //    _mensagemParaUsuario = "Inventário do tipo 'Inventário Manual' já existente para este Responsavel.\nPara gerar novo inventário, o inventário atual em andamento deverá ser excluído!";
                        //    return MensagemErroJson(_mensagemParaUsuario);
                        //}

                        List<ReportParameter> parametros = new List<ReportParameter>();

                        string path = PathDoTipoDeRelatorio();
                        LocalReport lr = SetaPathRelatorio(path);

                        //dtDadosRelatorio = RetornaDadosRelatorio(objViewPagina);
                        dtDadosRelatorio = RetornaDadosRelatorio(orgaoID, uoID, ugeID, uaID, responsavelID, divisaoID);

                        string DescricaoOrgao = (from a in db.Institutions where a.Id == orgaoID select a.Description).FirstOrDefault();
                        string DescricaoUO = (from a in db.BudgetUnits where a.Id == uoID select a.Description).FirstOrDefault();
                        string DescricaoUGE = (from a in db.ManagerUnits where a.Id == ugeID select a.Description).FirstOrDefault();
                        string DescricaoUA = (from a in db.AdministrativeUnits where a.Id == uaID select a.Description).FirstOrDefault();
                        string DescricaoDivisao = (from a in db.Sections where a.Id == divisaoID select a.Description).FirstOrDefault();
                        string NomeResponsavel = (from a in db.Responsibles where a.Id == responsavelID select a.Name).FirstOrDefault();

                        #region Inventario Manual
                        //IList<KeyValuePair<string, string>> dadosUsuarioRelatorio = obterDadosUsuarioLogado(objViewPagina);
                        //IList<KeyValuePair<string, string>> dadosUsuarioRelatorio = obterDadosUsuarioLogado(orgaoID, ugeID, uaID, responsavelID, divisaoID);
                        //var dadosInventarioManualGravados = ProcessaSaidaRelatorioInventario(dadosUsuarioRelatorio, dtDadosRelatorio);
                        var dadosInventarioManualGravados = dtDadosRelatorio.Rows.Count > 0;
                        #endregion

                        if (dadosInventarioManualGravados)
                        {
                            parametros.Add(new ReportParameter("NomeResponsavel", NomeResponsavel));
                            parametros.Add(new ReportParameter("Desc_Orgao", DescricaoOrgao));
                            parametros.Add(new ReportParameter("Desc_UO", DescricaoUO));
                            parametros.Add(new ReportParameter("Desc_UGE", DescricaoUGE));
                            parametros.Add(new ReportParameter("Desc_UA", DescricaoUA));
                            parametros.Add(new ReportParameter("Desc_Divisao", DescricaoDivisao));

                            lr.SetParameters(parametros);

                            ReportDataSource rdBalance = new ReportDataSource("DataSource", dtDadosRelatorio);
                            lr.DataSources.Add(rdBalance);

                            string data = DateTime.Now.ToString("yyyyMMddHHmmss");

                            string reportType = "PDF";
                            string mimeType;
                            string encoding;
                            string fileNameExtension;

                            string deviceInfo =
                                "<DeviceInfo>" +
                                    "  <OutputFormat>" + "PDF" + "</OutputFormat>" +
                                "</DeviceInfo>";

                            Warning[] warnings;
                            string[] streams;
                            byte[] renderedBytes;


                            //Render the report
                            renderedBytes = lr.Render(
                                reportType,
                                deviceInfo,
                                out mimeType,
                                out encoding,
                                out fileNameExtension,
                                out streams,
                                out warnings);
                            Response.Buffer = true;
                            Response.Clear();
                            Response.ContentType = mimeType;
                            Response.AddHeader("content-disposition", "attachment; filename=InventarioManual" + data + ".pdf");
                            Response.BinaryWrite(renderedBytes); // create the file
                            Response.Flush(); // send it to the client to download
                        }
                    }
                    else
                    {
                        //if (objViewPagina.InstitutionId == 0)
                        //    ModelState.AddModelError("InstitutionId", "O Campo Orgão é obrigatório");

                        //if (objViewPagina.BudgetUnitId == 0)
                        //    ModelState.AddModelError("BudgetUnitId", "O Campo UO é obrigatório");

                        //if (objViewPagina.ManagerUnitId == 0)
                        //    ModelState.AddModelError("ManagerUnitId", "O Campo UGE é obrigatório");

                        //if (objViewPagina.AdministrativeUnitId == 0)
                        //    ModelState.AddModelError("AdministrativeUnitId", "O Campo UA é obrigatório");

                        //if (objViewPagina.idResponsable == null || objViewPagina.idResponsable == 0)
                        //    ModelState.AddModelError("idResponsable", "O Campo Responsável é obrigatório");
                        return MensagemErroJson("Não há Inventário do tipo 'Inventário Manual' com status 'pendente' para este responsável!");
                    }
                }

                //SetCarregaHierarquia(objViewPagina.InstitutionId, objViewPagina.BudgetUnitId, objViewPagina.ManagerUnitId, objViewPagina.AdministrativeUnitId, objViewPagina.SectionId);
                SetCarregaHierarquia(orgaoID, uoID, ugeID, uaID, divisaoID);

                #region Combo Responsavel


                var vResponsible = new List<Responsible>();
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == uaID select r).ToList();

                vResponsible = vResponsible.Distinct().OrderBy(a => a.Name).ToList();

                ViewBag.User = new SelectList(vResponsible, "Id", "Name");

                #endregion


                //return View(objViewPagina);
                _mensagemParaUsuario = String.Format("Criado Inventário do tipo 'Inventário Manual' na tela 'Resultado do Inventário' com {0} Bens Patrimoniais vinculados com status 'pendente'.", dtDadosRelatorio.Rows.Count);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //return MensagemErro(CommonMensagens.PadraoException, ex);
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportInventarioManual(InventarioViewModel objViewPagina)
        {
            DataTable dtDadosRelatorio = null;

            try
            {
                if (objViewPagina.InstitutionId != 0 && objViewPagina.BudgetUnitId != 0 && objViewPagina.ManagerUnitId != 0 && objViewPagina.AdministrativeUnitId != 0 && objViewPagina.idResponsable != 0 && objViewPagina.idResponsable != null)
                {

                    List<ReportParameter> parametros = new List<ReportParameter>();

                    if (VerificaSePossuiRegistros(objViewPagina))
                    {

                        string path = PathDoTipoDeRelatorio();
                        LocalReport lr = SetaPathRelatorio(path);

                        dtDadosRelatorio = RetornaDadosGeracaoRelatorioInventarioManual(objViewPagina);

                        string DescricaoOrgao = (from a in db.Institutions where a.Id == objViewPagina.InstitutionId select a.Description).FirstOrDefault();
                        string DescricaoUO = (from a in db.BudgetUnits where a.Id == objViewPagina.BudgetUnitId select a.Description).FirstOrDefault();
                        string DescricaoUGE = (from a in db.ManagerUnits where a.Id == objViewPagina.ManagerUnitId select a.Description).FirstOrDefault();
                        string DescricaoUA = (from a in db.AdministrativeUnits where a.Id == objViewPagina.AdministrativeUnitId select a.Description).FirstOrDefault();
                        string DescricaoDivisao = (from a in db.Sections where a.Id == objViewPagina.SectionId select a.Description).FirstOrDefault();
                        string NomeResponsavel = (from a in db.Responsibles where a.Id == objViewPagina.idResponsable select a.Name).FirstOrDefault();

                        parametros.Add(new ReportParameter("NomeResponsavel", NomeResponsavel));
                        parametros.Add(new ReportParameter("Desc_Orgao", DescricaoOrgao));
                        parametros.Add(new ReportParameter("Desc_UO", DescricaoUO));
                        parametros.Add(new ReportParameter("Desc_UGE", DescricaoUGE));
                        parametros.Add(new ReportParameter("Desc_UA", DescricaoUA));
                        parametros.Add(new ReportParameter("Desc_Divisao", DescricaoDivisao));

                        lr.SetParameters(parametros);

                        ReportDataSource rdBalance = new ReportDataSource("DataSource", dtDadosRelatorio);
                        lr.DataSources.Add(rdBalance);

                        string data = DateTime.Now.ToString("yyyyMMddHHmmss");

                        string reportType = "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;

                        string deviceInfo =
                            "<DeviceInfo>" +
                                "  <OutputFormat>" + "PDF" + "</OutputFormat>" +
                            "</DeviceInfo>";

                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;


                        //Render the report
                        renderedBytes = lr.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);
                        Response.Buffer = true;
                        Response.Clear();
                        Response.ContentType = mimeType;
                        Response.AddHeader("content-disposition", "attachment; filename=InventarioManual" + data + ".pdf");
                        Response.BinaryWrite(renderedBytes); // create the file
                        Response.Flush(); // send it to the client to download
                    }
                    else {
                        ModelState.AddModelError("idResponsable", "Não há dados para este responsável!");
                    }
                }
                else
                {
                    if (objViewPagina.InstitutionId == 0)
                        ModelState.AddModelError("InstitutionId", "O Campo Orgão é obrigatório");

                    if (objViewPagina.BudgetUnitId == 0)
                        ModelState.AddModelError("BudgetUnitId", "O Campo UO é obrigatório");

                    if (objViewPagina.ManagerUnitId == 0)
                        ModelState.AddModelError("ManagerUnitId", "O Campo UGE é obrigatório");

                    if (objViewPagina.AdministrativeUnitId == 0)
                        ModelState.AddModelError("AdministrativeUnitId", "O Campo UA é obrigatório");

                    if (objViewPagina.idResponsable == null || objViewPagina.idResponsable == 0)
                        ModelState.AddModelError("idResponsable", "O Campo Responsável é obrigatório");
                }

                SetCarregaHierarquia(objViewPagina.InstitutionId, objViewPagina.BudgetUnitId, objViewPagina.ManagerUnitId, objViewPagina.AdministrativeUnitId, objViewPagina.SectionId);

                #region Combo Responsavel


                var vResponsible = new List<Responsible>();
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == objViewPagina.AdministrativeUnitId select r).ToList();

                vResponsible = vResponsible.Distinct().OrderBy(a => a.Name).ToList();

                ViewBag.User = new SelectList(vResponsible, "Id", "Name");

                #endregion


                return View(objViewPagina);
            }
            catch (HttpException ex)
            {
                //Código caso usuário feche a página antes de recebe o download
                switch (ex.ErrorCode)
                {
                    case -2147023901:
                    case -2147024832:
                        return HttpNotFound();
                    default:
                        return MensagemErro(CommonMensagens.PadraoException, ex);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }


        private bool VerificaSePossuiRegistros(InventarioViewModel model) {
            if (!ExisteInventariosPendentesParaResponsavel((int)model.idResponsable))
            {
                List<ReportParameter> parametros = new List<ReportParameter>();

                DataTable dtDadosRelatorio = RetornaDadosRelatorio(model.InstitutionId, model.BudgetUnitId, model.ManagerUnitId, model.AdministrativeUnitId, (int)model.idResponsable, model.SectionId);
                if (dtDadosRelatorio.Rows.Count > 0)
                {
                    IList<KeyValuePair<string, string>> dadosUsuarioRelatorio = obterDadosUsuarioLogado(model.InstitutionId, model.ManagerUnitId, model.AdministrativeUnitId, (int)model.idResponsable, model.SectionId);
                    ProcessaSaidaRelatorioInventario(dadosUsuarioRelatorio, dtDadosRelatorio);
                    return true;
                }
                else {
                    return false;
                }

            }else
                return true;
        }

        private static Exception GetFirstRealException(Exception exception)
        {
            Exception realException = exception;
            var aggregateException = realException as AggregateException;

            if (aggregateException != null)
            {
                realException = aggregateException.Flatten().InnerException; // take first real exception

                while (realException != null && realException.InnerException != null)
                {
                    realException = realException.InnerException;
                }
            }

            return realException ?? exception;
        }
        private string processaChamadaWsPatrimonio(string msgEstimuloWS)
        {
            HttpClient httpClient = null;
            Task<string> retornoWsSamPatrimonio = null;
            string retornoWS = null;
            string baseAdress = null;
            string urlConsulta = null;
            string caminhoUrlWS = null;


            caminhoUrlWS = (msgEstimuloWS.Contains("\"InventarioId\"") ? "/Mobiles/FinalizaInventarioItem?estimulo=" : "/Mobiles/FinalizaInventario?estimulo=");
            baseAdress = String.Format(@"{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, HttpContext.Request.ApplicationPath);


            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAdress);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            urlConsulta = String.Format("{0}{1}{2}", baseAdress, caminhoUrlWS, msgEstimuloWS);

            try
            {
                retornoWsSamPatrimonio = httpClient.GetStringAsync(urlConsulta);
                retornoWS = retornoWsSamPatrimonio.Result;
            }
            catch (AggregateException aggregateException)
            {
                var testeEXC = GetFirstRealException(aggregateException);
                throw new Exception("Erro ao importar inventário de coletor COMPEX");
            }


            return retornoWS;
        }
        private string geradorEstimuloInventarioWS(IList<KeyValuePair<string, string>> dadosUsuarioRelatorioInventario)
        {
            string msgEstimuloParaInventario = null;
            string descricaoDispositivoInventariante = null;


            int tipoDispositivoInventarianteID = (ViewBag.TipoDispositivoInventarianteID ?? 0);
            descricaoDispositivoInventariante = "Inventário Manual";

            if (dadosUsuarioRelatorioInventario.HasElements() && dadosUsuarioRelatorioInventario.HasElements())
                msgEstimuloParaInventario = String.Format(fmtMsgEstimuloInventarioWs, dadosUsuarioRelatorioInventario[0].Value, dadosUsuarioRelatorioInventario[1].Value, dadosUsuarioRelatorioInventario[2].Value, dadosUsuarioRelatorioInventario[3].Value, descricaoDispositivoInventariante, EnumTipoInventario.InventarioManual.GetHashCode().ToString());


            return msgEstimuloParaInventario;
        }
        //private IList<KeyValuePair<string, string>> obterDadosUsuarioLogado(InventarioViewModel objViewPagina)
        //{
        //    IList<KeyValuePair<string, string>> dadosUsuarioGeradorRelatorio = null;
        //    User currentUser = UserCommon.CurrentUser();

        //    var cpfUsuarioLogado = currentUser.CPF;
        //    dadosUsuarioGeradorRelatorio = new List<KeyValuePair<string, string>>();
        //    dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("ResponsavelId", objViewPagina.idResponsable.ToString()));
        //    dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("UaId", objViewPagina.AdministrativeUnitId.ToString()));
        //    dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("UgeId", objViewPagina.ManagerUnitId.ToString()));
        //    dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("Usuario", cpfUsuarioLogado));
        //    dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("DivisaoId", objViewPagina.SectionId.ToString()));

        //    return dadosUsuarioGeradorRelatorio;
        //}
        private IList<KeyValuePair<string, string>> obterDadosUsuarioLogado(int orgaoID, int ugeID, int uaID, int responsavelID, int divisaoID)
        {
            IList<KeyValuePair<string, string>> dadosUsuarioGeradorRelatorio = null;
            User currentUser = UserCommon.CurrentUser();

            var cpfUsuarioLogado = currentUser.CPF;
            dadosUsuarioGeradorRelatorio = new List<KeyValuePair<string, string>>();
            dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("ResponsavelId", responsavelID.ToString()));
            dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("UaId", uaID.ToString()));
            dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("UgeId", ugeID.ToString()));
            dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("Usuario", cpfUsuarioLogado));
            dadosUsuarioGeradorRelatorio.Add(new KeyValuePair<string, string>("DivisaoId", divisaoID.ToString()));

            return dadosUsuarioGeradorRelatorio;
        }
        private bool ProcessaSaidaRelatorioInventario(IList<KeyValuePair<string, string>> dadosUsuarioGeradorRelatorio, DataTable dadosRelatorio)
        {
            bool retornoProcessamento = false;
            IList<string> listagemMsgEstimuloItemInventarioWs = null;
            string msgEstimuloItemInventarioWs = null;
            string msgEstimuloInventarioWs = null;
            string chapaBP = null;
            string siglaBP = null;
            string codigoItemMaterial = null;
            string statusInventarialBP = null;
            string inventarioID = null;
            string assetID = null;



            //statusInventarialBP = SituacaoFisicaBP.OK.GetHashCode().ToString();
            statusInventarialBP = EnumSituacaoFisicaBP.Desconhecido.GetHashCode().ToString();
            var rowsInventarioManual = dadosRelatorio.Rows;

            if (rowsInventarioManual.IsNotNull() && rowsInventarioManual.Cast<DataRow>().HasElements())
            {
                msgEstimuloInventarioWs = geradorEstimuloInventarioWS(dadosUsuarioGeradorRelatorio);
                inventarioID = processaChamadaWsPatrimonio(msgEstimuloInventarioWs);

                listagemMsgEstimuloItemInventarioWs = new List<string>();
                if (!String.IsNullOrWhiteSpace(inventarioID))
                {
                    foreach (DataRow drLinhaInventarioManual in rowsInventarioManual)
                    {
                        var dadosSiglaChapaBP = drLinhaInventarioManual.ObterValor("Sigla_Chapa").BreakLine("-");
                        var dadosItemMaterialBP = drLinhaInventarioManual.ObterValor("Dados_BP").BreakLine("-");

                        siglaBP = dadosSiglaChapaBP[0];
                        chapaBP = dadosSiglaChapaBP[1];
                        codigoItemMaterial = dadosItemMaterialBP[0];
                        assetID = drLinhaInventarioManual.ObterValor("AssetID");

                        //msgEstimuloItemInventarioWs = String.Format(fmtMsgEstimuloItemInventarioWs, dadosBPProcessado[5].Value, dadosBPProcessado[6].Value, dadosBPProcessado[7].Value, dadosBPProcessado[8].Value, inventarioID, dadosBPProcessado[9].Value);
                        msgEstimuloItemInventarioWs = String.Format(fmtMsgEstimuloItemInventarioWs, chapaBP, statusInventarialBP, codigoItemMaterial, siglaBP, inventarioID, assetID);
                        listagemMsgEstimuloItemInventarioWs.Add(msgEstimuloItemInventarioWs);
                    }

                    var lotesMsgEstimuloParaProcessamento = listagemMsgEstimuloItemInventarioWs.ParticionadorLista(10);
                    string BPsConsolidados = null;
                    foreach (var loteMsgEstimuloParaProcessamento in lotesMsgEstimuloParaProcessamento)
                    {
                        BPsConsolidados = String.Join(",", loteMsgEstimuloParaProcessamento);
                        BPsConsolidados = String.Format("[{0}]", BPsConsolidados);
                        processaChamadaWsPatrimonio(BPsConsolidados);
                    }
                }
            }

            retornoProcessamento = (listagemMsgEstimuloItemInventarioWs.Count() > 0);
            return retornoProcessamento;
        }

        private string PathDoTipoDeRelatorio()
        {
            return Path.Combine(Server.MapPath("~/Report"), "InventarioManual.rdlc");
        }
        private LocalReport SetaPathRelatorio(string path)
        {
            LocalReport lr = new LocalReport();
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }

            return lr;
        }
        private DataTable RetornaDadosRelatorio(InventarioViewModel objViewPagina)
        {

            string spName = "SAM_PATRIMONIO_GERA_DADOS_INVENTARIO_MANUAL";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "idOrgao", valor = objViewPagina.InstitutionId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUo", valor = objViewPagina.BudgetUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUge", valor = objViewPagina.ManagerUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUa", valor = objViewPagina.AdministrativeUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idResponsavel", valor = objViewPagina.idResponsable });

            DataTable dtDadosInventarioManual = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
            return dtDadosInventarioManual;
        }
        private DataTable RetornaDadosGeracaoRelatorioInventarioManual(InventarioViewModel objViewPagina)
        {

            string spName = "SAM_PATRIMONIO_CONSULTA_INVENTARIO_MANUAL";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "idOrgao", valor = objViewPagina.InstitutionId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUo", valor = objViewPagina.BudgetUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUge", valor = objViewPagina.ManagerUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUa", valor = objViewPagina.AdministrativeUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idResponsavel", valor = objViewPagina.idResponsable });

            if (objViewPagina.SectionId > 0)
                listaParam.Add(new ListaParamentros { nomeParametro = "idDivisao", valor = objViewPagina.SectionId });


            DataTable dtDadosInventarioManual = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
            return dtDadosInventarioManual;
        }
        private DataTable RetornaDadosRelatorio(int orgaoID, int uoID, int ugeID, int uaID, int responsavelID, int divisaoID)
        {

            string spName = "SAM_PATRIMONIO_GERA_DADOS_INVENTARIO_MANUAL";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "idOrgao", valor = orgaoID });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUo", valor = uoID });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUge", valor = ugeID });
            listaParam.Add(new ListaParamentros { nomeParametro = "idUa", valor = uaID });
            listaParam.Add(new ListaParamentros { nomeParametro = "idResponsavel", valor = responsavelID });

            DataTable dtDadosInventarioManual = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
            return dtDadosInventarioManual;
        }
        #endregion

        #region Funcionalidades Operacao com Coletor COMPEX CPX-8000
        [HttpGet]
        public ActionResult Gravacao()
        {
            try
            {
                getHierarquiaPerfil();

                CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0, _administrativeUnitId ?? 0, _sectionId ?? 0);

                OperacoesComColetorViewModel operacoesViewModel = new OperacoesComColetorViewModel();
                operacoesViewModel.GeraTabela_ItemMaterial = true;
                operacoesViewModel.GeraTabela_Responsavel = true;
                operacoesViewModel.GeraTabela_Terceiros = true;
                operacoesViewModel.GeraTabela_Sigla = true;
                operacoesViewModel.GeraTabela_BemPatrimonial = true;
                return View(operacoesViewModel);
            }
            catch (Exception ex) {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public ActionResult Leitura()
        {
            OperacoesComColetorController objControler = null;


            objControler = new OperacoesComColetorController();
            return objControler.Leitura();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Leitura(int tipoDispositivoInventariante = 0)
        {
            OperacoesComColetorController objControler = null;


            objControler = new OperacoesComColetorController();
            return objControler.Leitura(tipoDispositivoInventariante);
        }
        #endregion

        /// <summary>
        /// Esta função sempre retornarah o numero total de BPs inclusos em qualquer inventario do tipo 'Inventario Manual' que esteja com o status 'pendente' para o ResponsavelID indicado
        /// </summary>
        /// <param name="ResponsavelID"></param>
        /// <returns></returns>
        public int ObterNumeroTotalBPsInventarioAtualPendente(int ResponsavelID)
        {
            int numeroTotalBPsInventarioAtualPendente = -1;
            var statusInventario = EnumStatusInventario.Pendente.GetHashCode().ToString();
            var tipoInventario = EnumTipoInventario.InventarioManual.GetHashCode();
            numeroTotalBPsInventarioAtualPendente = db.ItemInventarios
                                                      .Where(itemInventarioConsultado => itemInventarioConsultado.RelatedInventario.ResponsavelId == ResponsavelID
                                                                                      && itemInventarioConsultado.RelatedInventario.Status == statusInventario
                                                                                      && itemInventarioConsultado.RelatedInventario.TipoInventario == tipoInventario)
                                                      .Count();

            return numeroTotalBPsInventarioAtualPendente;
        }

        /// <summary>
        /// Esta função sempre retornarah o numero de BPs que estejam com status 'ok' ou 'nao encontrado fisicamente' de qqr. inclusos em qualquer inventario do tipo 'Inventario Manual' com status 'pendente' para o ResponsavelID indicado
        /// </summary>
        /// <param name="ResponsavelID"></param>
        /// <returns></returns>
        public int ObterNumeroBPsVerificadosEmInventarioAtualPendente(int ResponsavelID)
        {
            int numeroBPsVerificadosInventarioAtualPendente = -1;
            var statusInventario = EnumStatusInventario.Pendente.GetHashCode().ToString();
            var tipoInventario = EnumTipoInventario.InventarioManual.GetHashCode();

            int?[] statusPossiveisParaFechamentoInventarioManual = new int?[] {   EnumSituacaoFisicaBP.OK.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Incorporado.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Movimentado.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Devolvido.GetHashCode()
                                                                                , EnumSituacaoFisicaBP.Transferido.GetHashCode()
                                                                            };
            numeroBPsVerificadosInventarioAtualPendente = db.ItemInventarios
                                                            .Where(itemInventarioConsultado => itemInventarioConsultado.RelatedInventario.ResponsavelId == ResponsavelID
                                                                                            && itemInventarioConsultado.RelatedInventario.Status == statusInventario
                                                                                            && itemInventarioConsultado.RelatedInventario.TipoInventario == tipoInventario
                                                                                            && (statusPossiveisParaFechamentoInventarioManual.Contains(itemInventarioConsultado.Estado)))
                                                            .Count();

            return numeroBPsVerificadosInventarioAtualPendente;
        }


        public bool ExisteInventariosPendentesParaResponsavel(int ResponsavelID)
        {
            var statusInventario = EnumStatusInventario.Pendente.GetHashCode().ToString();
            int[] tiposInventario = new int[] {   EnumTipoInventario.Android.GetHashCode()
                                                , EnumTipoInventario.ColetorDados.GetHashCode()
                                                , EnumTipoInventario.InventarioManual.GetHashCode() };
            var _inventarioManualPendente = db.Inventarios
                                              .Where(inventarioConsultado => inventarioConsultado.ResponsavelId == ResponsavelID
                                                                          && inventarioConsultado.Status == statusInventario
                                                                          && (inventarioConsultado.TipoInventario.HasValue && tiposInventario.Contains(inventarioConsultado.TipoInventario.Value)))
                                              .Count() >= 1;
            return _inventarioManualPendente;
        }

        public async Task<JsonResult> ConsultarBemPatrimoniaisPorAdministrativeUnit(int administrativeUnitId)
        {
            try
            {
                int contadorResponsavelDaUA = await db.Responsibles.Where(responsavelAssociadoUA => responsavelAssociadoUA.AdministrativeUnitId == administrativeUnitId
                                                                                  && responsavelAssociadoUA.Status == true).CountAsync();

                if (contadorResponsavelDaUA == 0) {
                    return Json(new { Mensagem = "Não é possível a geração de arquivos de carga para UA's que não possui (ao menos um) Responsável associado", classeCss = "none" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { classeCss = "block" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                GravaLogErroSemRetorno(ex);
                return Json(new { Mensagem = "Ocorreu algo inesperado no sistema que pode não gerar o arquivo corretamente. Por gentileza, tente realizar novamente essa operação mais tarde.", classeCss = "none" }, JsonRequestBehavior.AllowGet);
            }            
        }

        public bool ExisteItensInventarioPendentesDePreenchimento(int inventarioID)
        {
            IList<int> estadosQuePossibilitamFecharInventario = new List<int>() {  EnumSituacaoFisicaBP.OK.GetHashCode()
                                                                                 , EnumSituacaoFisicaBP.NaoEncontrado.GetHashCode()
                                                                                 , EnumSituacaoFisicaBP.Incorporado.GetHashCode()
                                                                                 , EnumSituacaoFisicaBP.Movimentado.GetHashCode()
                                                                                 , EnumSituacaoFisicaBP.Transferido.GetHashCode()
                                                                                };
            bool statusInventarioPendente = false;
            bool itensInventarioNaoOK = false;

            statusInventarioPendente = (db.ItemInventarios
                                          .Count(itemInventario => itemInventario.InventarioId == inventarioID && !itemInventario.Estado.HasValue) >= 1);

            itensInventarioNaoOK = db.ItemInventarios
                                      .Count(itemInventario => (itemInventario.InventarioId == inventarioID) 
                                                            && (!estadosQuePossibilitamFecharInventario.Contains(itemInventario.Estado.Value))) >= 1;


            return (statusInventarioPendente || itensInventarioNaoOK);
        }

        public Tuple<bool, string, int> ExisteInventarioEmAndamentoParaUA(int UaId)
        {
            Tuple<bool, string, int> dadosRetornoProcessamento = null;
            IQueryable<Inventario> qryInventariosPendentes = null;
            bool statusInventarioPendente = false;
            int inventarioId = -1;
            int numeroInventariosPendentes = -1;
            string msgRetornoProcessamento = null;


            qryInventariosPendentes = db.Inventarios
                                        .AsNoTracking()
                                        .Where(inventario => (inventario.RelatedAdministrativeUnit.Id == UaId && inventario.Status == ((int)EnumStatusInventario.Pendente).ToString()))
                                        .AsQueryable();

            numeroInventariosPendentes = qryInventariosPendentes.Count();

            if (numeroInventariosPendentes == 1)
            {
                inventarioId = qryInventariosPendentes.FirstOrDefault().Id.GetValueOrDefault();
            }
            else if (numeroInventariosPendentes > 1)
            {
                var contextoLocal = new SAMContext();
                contextoLocal.Configuration.AutoDetectChangesEnabled = false;
                contextoLocal.Configuration.ProxyCreationEnabled = false;
                contextoLocal.Configuration.LazyLoadingEnabled = false;


                var dadosUA = contextoLocal.AdministrativeUnits
                                           .Include("ManagerUnit")
                                           .AsNoTracking()
                                           .Where(uaConsultada => uaConsultada.Id == UaId)
                                           .Select(uaConsultada => new { codigoUA = uaConsultada.Code, codigoUGE = uaConsultada.RelatedManagerUnit.Code })
                                           .FirstOrDefault();

                msgRetornoProcessamento = String.Format("Erro de sistema!\nNumero Inválido de Inventários Pendentes para a UA {0} (UGE {1})", dadosUA.codigoUA, dadosUA.codigoUGE);
            }


            statusInventarioPendente = (numeroInventariosPendentes == 1);
            dadosRetornoProcessamento = Tuple.Create<bool, string, int>(statusInventarioPendente, msgRetornoProcessamento, inventarioId);
            return dadosRetornoProcessamento;
        }

        #region CarregaViewBags
        private void CarregaHierarquiaFiltro(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null)
        {
            if (managerCode == null)
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.Institutions = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.Institutions = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
            }


            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if (modelManagerUnitId != 0)
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else if (modelBudgetUnitId != 0)
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
            else
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            if (modelAdministrativeUnitId != 0)
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
            else if (modelManagerUnitId != 0)
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
            else
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            if (modelSectionId != 0)
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
            else if (modelAdministrativeUnitId != 0)
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
            else
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
        }
        #endregion
    }
}
