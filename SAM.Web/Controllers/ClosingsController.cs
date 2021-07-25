using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SAM.Web.Models;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.ViewModels;
using System.IO;
using System.Data.SqlClient;
using PatrimonioBusiness.fechamento;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text;
using Sam.Integracao.SIAF.Core;
using Sam.Common;
using PatrimonioBusiness.contabiliza.entidades;
using Sam.Common.Util;
using System.Globalization;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using System.Transactions;
using System.Net.Mail;
using System.Collections;



namespace SAM.Web.Controllers
{
    public class ClosingsController : BaseController
    {
        private SAMContext db = new SAMContext();
        private Hierarquia hierarquia;
        private FunctionsCommon common;


        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _login;

        private const string CST_CAMPO_OBSERVACAO = "'{0}FECHAMENTO MENSAL INTEGRADO':'UGE {1}'; 'DocumentoSAM':'{2}'; 'Mes-Referencia':'{3}'; 'Conta Contabil Depreciação':'{4}'; 'Token SAM-Patrimonio':'{5}'";
        private readonly string cstEMailParaEnvioSuporteSAM = EnumVariaveisWebConfig.eMailParaEnvioSuporteSam;


        // GET: Fechamento
        public ActionResult Index()
        {
            try
            {
                getHierarquiaPerfil();
                CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);

                var managerUnit = (from m in db.ManagerUnits
                                   where m.BudgetUnitId == _budgetUnitId &&
                                         m.Id == _managerUnitId
                                   select new ClosingViewModel()
                                   {
                                       InstitutionId = _institutionId,
                                       BudgetUnitId = m.BudgetUnitId,
                                       ManagerUnitId = m.Id
                                   }).AsNoTracking().FirstOrDefault();

                if (managerUnit != null)
                {
                    if (managerUnit.ManagerUnitId != null && managerUnit.ManagerUnitId > 0)
                    {
                        managerUnit.IntegradoSIAFEM = UGEEstaIntegrada((int)managerUnit.ManagerUnitId);
                    }
                }

                return View(managerUnit);
            }
            catch (Exception ex)
            {
                GravaLogErroSemRetorno(ex);
                return MensagemErro(CommonMensagens.SistemaInstavel);
            }
        }

        [HttpGet]
        public async Task<JsonResult> UGEEmFechamento(int managerUnitId)
        {

            var UGE = (from m in db.ManagerUnits
                       where m.Id == managerUnitId
                       select m).FirstOrDefault();

            if (!UGE.FlagIntegracaoSiafem)
            {
                return Json(new { emProcesso = false }, JsonRequestBehavior.AllowGet);
            }

            int ano = Convert.ToInt32(UGE.ManagmentUnit_YearMonthReference.Substring(0, 4));
            int mes = Convert.ToInt32(UGE.ManagmentUnit_YearMonthReference.Substring(4, 2));

            string mesReferenciaAnterior = string.Empty;

            if (mes == 1)
            {
                ano = ano - 1;
                mesReferenciaAnterior = ano.ToString() + "12";
            }
            else
            {
                mes = mes - 1;
                mesReferenciaAnterior = ano.ToString() + mes.ToString().PadLeft(2, '0');
            }

            if (Convert.ToInt32(mesReferenciaAnterior) <= UGE.MesRefInicioIntegracaoSIAFEM)
            {
                return Json(new { emProcesso = false }, JsonRequestBehavior.AllowGet);
            }

            int codigoUGE = Convert.ToInt32(UGE.Code);


            //Colocar AuditoriaIntegracaoId == null ou GeneratedNL == null aqui
            //não trazia os registros
            var registros = (from q in this.db.AccountingClosings
                             where q.ManagerUnitCode == codigoUGE
                                && q.ReferenceMonth == mesReferenciaAnterior
                                && q.DepreciationMonth > 0
                                //&& (q.GeneratedNL == null || q.GeneratedNL == string.Empty)
                                && !(q.DepreciationAccount != null && q.ClosingId != null)
                             select q).ToList();

            bool nenhumRegistroComNL = registros.Where(r => r.GeneratedNL != null && !string.IsNullOrEmpty(r.GeneratedNL)).Count() == 0;
            bool aindaEmFechamento = registros.Count() > 0 && nenhumRegistroComNL;

            return Json(new { emProcesso = aindaEmFechamento, mesRef = mesReferenciaAnterior }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> DetalhesDepreciacao(int assetId)
        {
            getHierarquiaPerfil();
            ViewBag.assetId = assetId;

            var asset = await this.db.Assets.Where(x => x.Id == assetId && x.ManagerUnitId == this._managerUnitId).FirstOrDefaultAsync();

            var managerUnit = await (from m in db.ManagerUnits
                                     where m.Id == this._managerUnitId
                                     select m).AsNoTracking().FirstOrDefaultAsync();

            int ano = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
            int mes = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));
            DateTime dataFinal = new DateTime(ano, mes, 1);
            FechamentoFactory factory = FechamentoFactory.GetInstancia();

            var Depreciacao = factory.CreateDepreciacao(System.Data.IsolationLevel.ReadUncommitted);
            var depreciacoes = await Depreciacao.SimularDepreciacao(asset.AssetStartId.Value, this._managerUnitId.Value, asset.MaterialItemCode, dataFinal);

            return View(depreciacoes);
        }

        [HttpPost]
        public async Task<JsonResult> ConfirmarDataAquisicao(int assetId, DateTime? dataAquisicao, DateTime? dataIncorporacao)
        {
            try
            {

                var asset = await (from bem in this.db.Assets where bem.Id == assetId select bem).FirstOrDefaultAsync();
                if (dataAquisicao.HasValue)
                {
                    asset.AcquisitionDate = dataAquisicao.Value;
                }

                if (dataIncorporacao.HasValue)
                {
                    asset.MovimentDate = dataIncorporacao.Value;
                }

                if (!dataAquisicao.HasValue && !dataIncorporacao.HasValue)
                {
                    return Json(new { data = false, Mensagem = "Datas de aquisição/incorporação não podem ser vazias." });

                }

                this.db.Entry(asset).State = EntityState.Modified;

                await this.db.SaveChangesAsync();
                return Json(new { data = true, Mensagem = "Alteração data de aquisição realizada com sucesso!" });


            }
            catch (Exception ex)
            {
                base.GravaLogErro(ex);
                return Json(new { data = false, Mensagem = "Erro ao alterar data de aquisição, por favor entrar em contato com suporte SAM." });

            }
        }

        [HttpPost]
        public async Task<JsonResult> Depreciar(int assetId)
        {
            getHierarquiaPerfil();

            var asset = await this.db.Assets.Where(x => x.Id == assetId && x.ManagerUnitId == this._managerUnitId).FirstOrDefaultAsync();

            var managerUnit = await (from m in db.ManagerUnits
                                     where m.Id == this._managerUnitId
                                     select m).AsNoTracking().FirstOrDefaultAsync();

            int ano = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
            int mes = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));
            DateTime dataFinal = new DateTime(ano, mes, 1);
            FechamentoFactory factory = FechamentoFactory.GetInstancia();

            var Depreciacao = factory.CreateDepreciacao(System.Data.IsolationLevel.ReadUncommitted);
            var resultado = await Depreciacao.Depreciar(asset.AssetStartId.Value, this._managerUnitId.Value, asset.MaterialItemCode, dataFinal);
            if (resultado)
                return Json(new { data = true, Mensagem = "Depreciação realizada com sucesso!" });
            else
                return Json(new { data = false, Mensagem = "Erro ao realizar a depreciação, por favor entrar em contato com suporte SAM." });

        }

        private async Task DepreciarFechamento(int assetStartId, int materialItemCode, int ManagerUnitId, DateTime dataFinal)
        {
            FechamentoFactory factory = FechamentoFactory.GetInstancia();

            var Depreciacao = factory.CreateDepreciacao(System.Data.IsolationLevel.ReadUncommitted);
            var resultado = await Depreciacao.Depreciar(assetStartId, ManagerUnitId, materialItemCode, dataFinal);
        }

        private async Task<string> Depreciar(int assetStartId, int materialItemCode, DateTime dataFinal)
        {
            getHierarquiaPerfil();

            var managerUnit = await (from m in db.ManagerUnits
                                     where m.Id == this._managerUnitId
                                     select m).AsNoTracking().FirstOrDefaultAsync();


            FechamentoFactory factory = FechamentoFactory.GetInstancia();

            var Depreciacao = factory.CreateDepreciacao(System.Data.IsolationLevel.ReadUncommitted);
            var resultado = await Depreciacao.Depreciar(assetStartId, this._managerUnitId.Value, materialItemCode, dataFinal);

            if (resultado)
                return Json(new { data = true, Mensagem = "Depreciação realizada com sucesso!" }).ToString();
            else
                return Json(new { data = false, Mensagem = "Erro ao realizar a depreciação, por favor entrar em contato com suporte SAM." }).ToString();

        }
        private async Task CreateRelatorioContabil(int managerUnitId, string mesReferencia)
        {
            FechamentoFactory factory = FechamentoFactory.GetInstancia();
            var managerUnitCode = await db.ManagerUnits.Where(x => x.Id == managerUnitId).Select(x => x.Code).FirstOrDefaultAsync();

            IQueryable<AccountingClosing> query = (from q in this.db.AccountingClosings
                                                   where q.ManagerCode == managerUnitCode
                                                      && q.ReferenceMonth == mesReferencia
                                                   select q
                                                   );
            var accouting = await query.FirstOrDefaultAsync();
            if (accouting == null)
            {
                var Depreciacao = factory.CreateDepreciacao(System.Data.IsolationLevel.ReadUncommitted);
                var resultado = Depreciacao.CreateRelatorioContabil(managerUnitId, mesReferencia);
            }
        }
        private async Task<bool> GetInservivel(int materialItemCode, int auxiliaryAccountId)
        {
            IQueryable<int> query = (from bp in db.Assets
                                     join mov in db.AssetMovements on bp.Id equals mov.AssetId
                                     where mov.AuxiliaryAccountId == auxiliaryAccountId //212508
                                        && bp.MaterialItemCode == materialItemCode
                                     select bp.Id
                                    );

            var resultado = await query.FirstOrDefaultAsync();
            return resultado > 0 ? true : false;
        }
        private async Task<List<Asset>> GetInserviveis(int managerUnitId, int auxiliaryAccountId)
        {
            IQueryable<Asset> query = (from bp in db.Assets
                                       join mov in db.AssetMovements on bp.Id equals mov.AssetId
                                       where mov.ManagerUnitId == managerUnitId
                                          && mov.AuxiliaryAccountId == auxiliaryAccountId //212508
                                          && mov.FlagEstorno != true
                                          && bp.Status == true
                                       select bp
                                    ).Distinct();

            var resultados = await query.ToListAsync();
            return resultados;
        }
        private string getCaminhoDosArquivoXml(String baseOrigem)
        {
            string caminhoRelativo = Request.Path.Replace("Closing", "Arquivos") + "/" + baseOrigem;
            var fileName = HttpContext.Server.MapPath(caminhoRelativo);
            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }

            return fileName;

        }
        public async Task<JsonResult> EfetuaFechamento(int managerUnitId)
        {
            string _mensagemParaUsuario = string.Empty;
            ManagerUnit managerUnit = null;
            bool integradaSIAFEM = false;

            int ano;
            int mes;
            DateTime mesAnoReferencia;

            try
            {
                managerUnit = (from m in db.ManagerUnits
                               where m.Id == managerUnitId
                               select m).AsNoTracking().FirstOrDefault();

                if (!ValidaFechamento(managerUnit.ManagmentUnit_YearMonthReference))
                {
                    _mensagemParaUsuario = "Fechamento Inválido.";

                    string messageErro = "Fechamento Inválido.";
                    string stackTrace = "Método EfetuaFechamento, Controller Closings";
                    string name = "Rotina de FECHAMENTO";

                    GravaLogErro(messageErro, stackTrace, name);
                    return Json(new { data = "", mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }

                if (managerUnit.FlagIntegracaoSiafem)
                {
                    integradaSIAFEM = true;
                    if (UGEComPendenciaSIAFEMDoFechamento(managerUnit.Code, managerUnit.ManagmentUnit_YearMonthReference))
                    {
                        return Json(new { data = "", mensagem = CommonMensagens.OperacaoInvalidaIntegracaoFechamento }, JsonRequestBehavior.AllowGet);
                    }

                    if (UGETemBPsComPendenciaDeDados(managerUnit.Id))
                    {
                        return Json(new { data = "", mensagem = "Existem BPs com pendência de dados a serem corrigidos. Por gentileza, consulte as telas Bens Patrimoniais Pendentes  e Bens Patrimoniais Pendentes(Integração) e faça as devidas correções. " }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (OrgaoIntegradoAoSIAFEMDeUGENaoIntegrada(managerUnit.BudgetUnitId))
                    {
                        integradaSIAFEM = true;

                        if (UGEComPendenciaSIAFEMDoFechamento(managerUnit.Code, managerUnit.ManagmentUnit_YearMonthReference))
                        {
                            return Json(new { data = "", mensagem = CommonMensagens.OperacaoInvalidaIntegracaoFechamento }, JsonRequestBehavior.AllowGet);
                        }

                        if (UGETemBPsComPendenciaDeDados(managerUnit.Id))
                        {
                            return Json(new { data = "", mensagem = "Existem BPs com pendência de dados a serem corrigidos. Por gentileza, consulte as telas Bens Patrimoniais Pendentes  e Bens Patrimoniais Pendentes(Integração) e faça as devidas correções. " }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                ano = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
                mes = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));

                mesAnoReferencia = new DateTime(ano, mes, 1);

                if (
                    UGEComBpsASeremReclassificadosDoGrupoSetentaENova(mesAnoReferencia, managerUnit.Id) &&
                    UGEComBpsASeremReclassificadosDiferenteDoGrupoSetentaENova(mesAnoReferencia, managerUnit.Id)
                   )
                {
                    return Json(new { data = "", mensagem = "Existem BPs que precisam de reclassificar a conta contábil. Por gentileza, vá na tela Reclassificação Contábil para efetuar essa reclassificação" }, JsonRequestBehavior.AllowGet);
                }

                if (ValidaPendenciasDeTransferenciaOuDoacao(managerUnit.Id, managerUnit.ManagmentUnit_YearMonthReference))
                {
                    _mensagemParaUsuario = "Existem Transferência/Doação pendentes de serem recebidas, entre em contato com a UGE de Destino para solução do problema.";

                    return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }

                if (ValidaBPsASeremIncorporados(managerUnit.Id, managerUnit.ManagmentUnit_YearMonthReference))
                {
                    _mensagemParaUsuario = "Existem BPs a serem incorporados até a data do mês de referência atual.";

                    return Json(new { data = "", mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }


                if (ValidaPendenciasDeSaidaInservivelFechamento(managerUnit.Id, managerUnit.ManagmentUnit_YearMonthReference))
                {
                    _mensagemParaUsuario = "Existem Saída de Inservíveis ou Recebimento de Inservíveis a serem feitos até a data do mês de referência atual.";

                    return Json(new { data = "", mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }

                if (ValidaTerPendenciaDeNL(managerUnit.Id))
                {
                    _mensagemParaUsuario = "UGE possui pendências de Nota Lançamento para serem resolvidas.";

                    return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                GravaLogErroSemRetorno(e);
                return Json(new { mensagem = "Devido algumas instabilidades encontradas, o sistema não iniciou o processo de fechamento dessa UGE. Por gentileza, tente novamente mais tarde." }, JsonRequestBehavior.AllowGet);
            }


            try
            {
                var primeiraDepreciacao = await (from d in db.MonthlyDepreciations where d.ManagerUnitId == managerUnitId select d.Id).FirstOrDefaultAsync();
                if (primeiraDepreciacao > 0)
                {
                    FechamentoFactory factory = FechamentoFactory.GetInstancia();


                    var materialDepreciacao = factory.CreateMaterialItemDepreciacao(System.Data.IsolationLevel.ReadUncommitted);
                    var materiaisNaoDepreciados = materialDepreciacao.GetNaoDepreciados(managerUnitId, mesAnoReferencia).ToList();

                    if (materiaisNaoDepreciados.Count > 0)
                    {
                        return Json(new { data = materiaisNaoDepreciados, mensagem = "NaoDepreciados" }, JsonRequestBehavior.AllowGet);
                    }

                }



                _mensagemParaUsuario = "Fechamento realizado com sucesso";
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                IQueryable<FechamentoViewModel> query = (from a in db.Assets
                                                         join m in db.AssetMovements on a.Id equals m.AssetId
                                                         where a.MaterialGroupCode != 0
                                                            && a.ManagerUnitId == managerUnitId
                                                            && a.flagVerificado == null
                                                            && a.flagDepreciaAcumulada == 1
                                                            && a.Status == true
                                                            && (a.flagAcervo == null || a.flagAcervo == false)
                                                            && (a.flagTerceiro == null || a.flagTerceiro == false)
                                                            && (a.flagAnimalNaoServico == null || a.flagAnimalNaoServico == false)
                                                            && (m.FlagEstorno == null || m.FlagEstorno == false)
                                                            && (m.Status == true)
                                                         select new FechamentoViewModel
                                                         {
                                                             MaterialItemCode = a.MaterialItemCode,
                                                             AssetStartId = a.AssetStartId,
                                                             AssetId = a.Id
                                                         }).Distinct();

                var fechamentos = await query.ToListAsync();
                decimal RateDepreciationMonthly = decimal.Zero;
                decimal CurrentValue = decimal.Zero;
                decimal AccumulatedDepreciation = decimal.Zero;
                decimal UnfoldingValue = decimal.Zero;
                var dataFechamento = mesAnoReferencia.AddMonths(1);
                var inserviveis = await GetInserviveis(managerUnitId, 212508);

                int contador = 0;
                foreach (var item in fechamentos)
                {
                    ++contador;
                    IQueryable<MonthlyDepreciation> _query = null;

                    var _transferencia = await (from t in db.AssetMovements where t.AssetTransferenciaId == item.AssetId select t).FirstOrDefaultAsync();

                    if (_transferencia != null)
                        _query = (db.MonthlyDepreciations.Where(x => x.AssetStartId == item.AssetStartId).OrderByDescending(x => x.Id)).Take(1);
                    else
                        _query = (db.MonthlyDepreciations.Where(x => x.AssetStartId == item.AssetStartId && x.ManagerUnitId == managerUnitId).OrderByDescending(x => x.Id)).Take(1);

                    var depreciacao = await _query.FirstOrDefaultAsync();

                    var bpInservivel = inserviveis.Where(x => x.Id == item.AssetId).FirstOrDefault();

                    if (depreciacao != null)
                    {
                        if (depreciacao.CurrentDate >= mesAnoReferencia
                           && depreciacao.CurrentMonth != 0
                           && depreciacao.LifeCycle >= depreciacao.CurrentMonth)
                        {
                            depreciacao = null;
                            continue;
                        }

                        if (depreciacao.LifeCycle > depreciacao.CurrentMonth)
                        {
                            if (bpInservivel == null)
                            {

                                RateDepreciationMonthly = depreciacao.RateDepreciationMonthly;
                                AccumulatedDepreciation = depreciacao.AccumulatedDepreciation + RateDepreciationMonthly;
                                CurrentValue = depreciacao.CurrentValue - RateDepreciationMonthly;

                                MonthlyDepreciation _depreciacao = depreciacao.Clone();
                                _depreciacao.AccumulatedDepreciation = AccumulatedDepreciation;
                                _depreciacao.CurrentValue = CurrentValue;
                                _depreciacao.QtdLinhaRepetida = 0;
                                _depreciacao.CurrentDate = _depreciacao.CurrentDate.AddMonths(1);
                                _depreciacao.CurrentMonth += 1;

                                if (_transferencia != null)
                                    _depreciacao.ManagerUnitId = managerUnitId;

                                if (_depreciacao.CurrentMonth == _depreciacao.LifeCycle
                                    && _depreciacao.CurrentValue != _depreciacao.ResidualValue)
                                {
                                    if (_depreciacao.CurrentValue > _depreciacao.ResidualValue)
                                    {
                                        UnfoldingValue = _depreciacao.CurrentValue - _depreciacao.ResidualValue;
                                        _depreciacao.AccumulatedDepreciation += UnfoldingValue;
                                        _depreciacao.RateDepreciationMonthly += UnfoldingValue;
                                        _depreciacao.CurrentValue -= UnfoldingValue;
                                        _depreciacao.UnfoldingValue = UnfoldingValue;
                                    } else {
                                        UnfoldingValue = _depreciacao.ResidualValue - _depreciacao.CurrentValue;
                                        _depreciacao.AccumulatedDepreciation -= UnfoldingValue;
                                        _depreciacao.RateDepreciationMonthly -= UnfoldingValue;
                                        _depreciacao.CurrentValue += UnfoldingValue;
                                        _depreciacao.UnfoldingValue = UnfoldingValue;
                                    }
                                }

                                db.MonthlyDepreciations.Add(_depreciacao);
                                var retorno = await db.SaveChangesAsync();
                                _depreciacao = null;
                                depreciacao = null;
                            }
                        }
                        else if (depreciacao.LifeCycle < depreciacao.CurrentMonth)
                        {
                            if (bpInservivel == null && depreciacao.ManagerUnitId != managerUnitId)
                            {
                                MonthlyDepreciation _depreciacao = depreciacao.Clone();

                                //Caso depreciação total já tenha a linha completa nessa UGE, altera o valor do campo QtdLinhaRepetida
                                //Exemplo: BP depreciou todo na UGE "A" -> manda para a UGE "B" -> no mês seguinte a UGE "B" manda de volta para UGE "A"
                                int cont = (from m in db.MonthlyDepreciations
                                            where m.ManagerUnitId == managerUnitId
                                            && m.MaterialItemCode == _depreciacao.MaterialItemCode
                                            && m.AssetStartId == depreciacao.AssetStartId
                                            select m.Id).Count();

                                _depreciacao.ManagerUnitId = managerUnitId;
                                _depreciacao.QtdLinhaRepetida = cont;
                                db.MonthlyDepreciations.Add(_depreciacao);
                                var retorno = await db.SaveChangesAsync();
                                _depreciacao = null;
                                depreciacao = null;
                            }
                        }
                        else if (depreciacao.LifeCycle == depreciacao.CurrentMonth) {
                            if (bpInservivel == null) {
                                MonthlyDepreciation _depreciacao = depreciacao.Clone();
                                _depreciacao.RateDepreciationMonthly = 0;
                                _depreciacao.CurrentDate = _depreciacao.CurrentDate.AddMonths(1);
                                _depreciacao.CurrentMonth += 1;
                                _depreciacao.QtdLinhaRepetida = 0;

                                if (_transferencia != null)
                                    _depreciacao.ManagerUnitId = managerUnitId;

                                db.MonthlyDepreciations.Add(_depreciacao);
                                var retorno = await db.SaveChangesAsync();
                                _depreciacao = null;
                                depreciacao = null;
                            }
                        }

                    }
                    else if (depreciacao == null)
                    {
                        if (bpInservivel == null)
                        {
                            int _assetStartId = 0;
                            var transferencia = await (from t in db.AssetMovements where t.AssetTransferenciaId == item.AssetId select t).FirstOrDefaultAsync();
                            if (!item.AssetStartId.HasValue)
                            {

                                if (transferencia != null)
                                {
                                    _assetStartId = transferencia.AssetId;
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + item.AssetId.ToString());
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + item.AssetId.ToString());
                                }
                                else
                                {
                                    _assetStartId = item.AssetId;
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + item.AssetId.ToString());
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + item.AssetId.ToString());

                                }
                            }
                            else
                            {
                                _assetStartId = item.AssetStartId.Value;
                            }

                            await DepreciarFechamento(_assetStartId, item.MaterialItemCode, managerUnitId, mesAnoReferencia);
                        }
                    }
                }

                var dataMesXml = mesAnoReferencia.Year.ToString() + mesAnoReferencia.Month.ToString().PadLeft(2, '0');

                if (integradaSIAFEM)
                {
                    await CreateRelatorioContabil(managerUnitId, dataMesXml);
                }

                var dataReferencia = dataFechamento.Year.ToString() + dataFechamento.Month.ToString().PadLeft(2, '0');
                managerUnit.ManagmentUnit_YearMonthReference = dataReferencia;

                db.Entry(managerUnit).State = EntityState.Modified;
                db.SaveChanges();

                //if (managerUnit.Code.StartsWith("380"))
                //{
                //    int codigoUGE = Int32.Parse(managerUnit.Code);
                //    string tipoEventoFechamento = "fechamento";
                //    await enviaEMailParaComite(codigoUGE, integradaSIAFEM, dataFechamento, tipoEventoFechamento, null);
                //}

                if (integradaSIAFEM)
                {
                    ConstarAlteracoesContabeisDaUGE(managerUnit.Id);
                }

                return Json(new { data = "", mensagem = _mensagemParaUsuario, integrado = integradaSIAFEM, numeroUGE = managerUnit.Id, mesref = dataMesXml }, JsonRequestBehavior.AllowGet);
            // Comentado, para verificação de uso futuramente
            //if (retornoDepreciacoes.Where(x => x.Erro == true).FirstOrDefault() == null)
            //{

            //}
            //else
            //{
            //    var retorno = retornoDepreciacoes.OrderBy(x => x.Erro).ToList();

            //    //Não faz o fluxo da integração SIAFEM pq tem alguma pendencia na depreciação
            //    return Json(new { data = retorno, mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
            //}

            }
            catch (Exception ex)
            {
                GravaLogErroSemRetorno(ex);
                return Json(new { mensagem = "Ocorreu algo inesperado durante o processo de fechamento dessa UGE, o que pode ter afetado a depreciação mensal ou não. Por gentileza, abra um chamado no suporte alertando sobre o ocorrido e aguarde nossas orientações."}, JsonRequestBehavior.AllowGet);
            }
            //finally
            //{
            //    retornoDepreciacoes = null;
            //}
        }

        #region Envio Email Comite

        private string[] obtemRelacaoEMailsParaInformes(int OrgaoId)
        {
            string[] relacaoEmails = null;

            //relacaoEmails = new string[] { "suportesam@sp.gov.br" };
            relacaoEmails = new string[] { "ugos-administrativos@sp.gov.br" };
            return relacaoEmails;
        }

        private async Task enviaEMailParaComite(int codigoUGE, bool integradaAoContabilizaSP, DateTime dataFechamentoMensal, string tipoEventoFechamento, string chaveUsuarioSIAFEM)
        {
            try
            {
                string[] relacaoEMails = null;
                if ((codigoUGE > 0) && !String.IsNullOrWhiteSpace(tipoEventoFechamento))
                {
                    relacaoEMails = obtemRelacaoEMailsParaInformes(0);
                    var assuntoMensagem = montarAssuntoMensagem(codigoUGE, dataFechamentoMensal, tipoEventoFechamento);
                    var corpoMensagem = montarCorpoMensagem(codigoUGE, integradaAoContabilizaSP, dataFechamentoMensal, tipoEventoFechamento, chaveUsuarioSIAFEM);
                    var listagemEMails = gerarRelacaoEMails(relacaoEMails);
                    var despachanteEMail = new DespachanteEMail();


                    despachanteEMail.MontarMensagem(listagemEMails["De"] as string,
                                                    listagemEMails["Para"] as string[],
                                                    listagemEMails["CC"] as string[],
                                                    null,
                                                    assuntoMensagem,
                                                    corpoMensagem);

                    despachanteEMail.EnviarMensagem();
                }
            }
            catch (SmtpException excErroEnvioEmail)
            {
                base.GravaLogErro(excErroEnvioEmail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string montarCorpoMensagem(int codigoUGE, bool integradaAoContabilizaSP, DateTime dataFechamentoMensal, string tipoEventoFechamento, string chaveUsuarioSIAFEM)
        {
            string fmtTextoMensagem = null;
            string textoMensagem = null;
            string nomeUsuarioSAMLogado;
            string cpfUsuarioSAMLogado = null;
            string mesReferenciaFechado = null;
            string _tipoEventoFechamento = null;
            string complementoDadosSIAFEM = null;
            string rodapeInformativoEMailAutomatico = null;
            string mensagemSaudacaoEMailAutomatico = null;
            User dadosUsuarioLogado = null;






            try
            {
                _tipoEventoFechamento = ((tipoEventoFechamento.ToLowerInvariant().Contains("reabertura")) ? "reabertura " : null);
                mesReferenciaFechado = String.Format("{0:D2}/{1}", dataFechamentoMensal.Month.ToString(), dataFechamentoMensal.Year);
                dadosUsuarioLogado = (new User() { Name = "TESTE UNITARIO FECHAMENTO", CPF = "00000000000"}); //UserCommon.CurrentUser();
                cpfUsuarioSAMLogado = (dadosUsuarioLogado.IsNotNull() ? dadosUsuarioLogado.CPF : null);
                nomeUsuarioSAMLogado = (dadosUsuarioLogado.IsNotNull() ? dadosUsuarioLogado.Name : null);
                mensagemSaudacaoEMailAutomatico = geraMensagemSaudacao();
                rodapeInformativoEMailAutomatico = "\n\n\nObs.: E-mail enviado automaticamente via sistema. Favor ignorar.";
                complementoDadosSIAFEM = (integradaAoContabilizaSP && !String.IsNullOrWhiteSpace(chaveUsuarioSIAFEM) ? String.Format(" e com utilização de chave SIAFEM '{0}'", chaveUsuarioSIAFEM) : null);
                fmtTextoMensagem = "{0}Informamos a execução do procedimento de {1}fechamento mensal do mês-referência {2} da UGE {3:D6} por meio da chave SAM '{4}' ('{5}'){6}, em {7}.\n\n\n{8}At.te.,";
                textoMensagem = String.Format(fmtTextoMensagem, mensagemSaudacaoEMailAutomatico, _tipoEventoFechamento, mesReferenciaFechado, codigoUGE, cpfUsuarioSAMLogado, nomeUsuarioSAMLogado, complementoDadosSIAFEM, DateTime.Now, rodapeInformativoEMailAutomatico);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return textoMensagem;
        }

        private string geraMensagemSaudacao()
        {
            string mensagemSaudacao = null;
            string periodoDia = null;
            DateTime horarioEnvio = DateTime.Now;

            if (horarioEnvio.Hour >= 0 && horarioEnvio.Hour < 12)
                periodoDia = "Bom dia";
            else if (horarioEnvio.Hour >= 12 && horarioEnvio.Hour < 19)
                periodoDia = "Boa tarde";
            else if (horarioEnvio.Hour >= 19 && horarioEnvio.Hour <= 23)
                periodoDia = "Boa noite";


            mensagemSaudacao = String.Format("Prezado(a)(s),\n{0}.\n\n\n", periodoDia);


            return mensagemSaudacao;
        }

        private string montarAssuntoMensagem(int codigoUGE, DateTime dataFechamentoMensal, string tipoEventoFechamento)
        {
            try
            {
                string _tipoEventoFechamento = null;
                string fmtAssuntoMensagem = null;
                string assuntoMensagem = null;
                string mesReferenciaFechado = null;


                fmtAssuntoMensagem = "Sistema SAM - Módulo Patrimônio - {0}Fechamento Mensal {1} - UGE {2:D6}";
                if ((codigoUGE > 0) && !String.IsNullOrWhiteSpace(tipoEventoFechamento) && (dataFechamentoMensal != DateTime.MinValue))
                {
                    _tipoEventoFechamento = ((tipoEventoFechamento.ToLowerInvariant().Contains("reabertura")) ? "Reabertura " : null);
                    mesReferenciaFechado = String.Format("{0}/{1}", dataFechamentoMensal.Month.ToString("D2"), dataFechamentoMensal.Year);
                    assuntoMensagem = String.Format(fmtAssuntoMensagem, _tipoEventoFechamento, mesReferenciaFechado, codigoUGE);
                }

                return assuntoMensagem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SortedList gerarRelacaoEMails(string[] relacaoEMailsDestinatarios)
        {
            try
            {
                string[] enderecosDestinatarios = null;
                string[] enderecosEMailEmCopia = null;

                SortedList listagemEMails = new SortedList(StringComparer.InvariantCultureIgnoreCase);

                enderecosDestinatarios = new string[] { String.Join("; ", relacaoEMailsDestinatarios) };
                enderecosEMailEmCopia = new string[] { cstEMailParaEnvioSuporteSAM };

                listagemEMails.InserirValor("De", cstEMailParaEnvioSuporteSAM);
                listagemEMails.InserirValor("Para", enderecosDestinatarios);
                listagemEMails.InserirValor("CC", enderecosEMailEmCopia);

                return listagemEMails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private void ConstarAlteracoesContabeisDaUGE(int IdUGE)
        {
            try
            {
                db.Database.ExecuteSqlCommand("EXEC [dbo].[CONSTAR_ALTERACOES_CONTABEIS_POR_UGE_CONTA_CONTABIL_NO_FECHAMENTO] @IdUge = " + IdUGE.ToString());
            }
            catch (Exception e)
            {
                GravaLogErroSemRetorno(e);
            }
        }

        private bool UGEEstaIntegrada(int ugeId)
        {
            return (from i in db.Institutions
                    join b in db.BudgetUnits on i.Id equals b.InstitutionId
                    join m in db.ManagerUnits on b.Id equals m.BudgetUnitId
                    where m.Id == ugeId
                    select i.flagIntegracaoSiafem || m.FlagIntegracaoSiafem).FirstOrDefault();
        }

        private async Task<List<SIAFDOC>> GetXmlSIAFEM(int managerUnitCode, DateTime dataFechamento, bool ehEstorno = false)
        {
            string tokenAuditoriaIntegracao = null;
            string chaveSIAFMonitora = null;
            string acaoPagamentoNotaLancamento = null;
            IList<AccountingClosing> resultados = null;
            List<SIAFDOC> retornos = new List<SIAFDOC>();
            var dataReferencia = dataFechamento.Year.ToString() + dataFechamento.Month.ToString().PadLeft(2, '0');

            try
            {
                IQueryable<AccountingClosing> query = (from q in this.db.AccountingClosings
                                                       where q.ManagerUnitCode == managerUnitCode
                                                          && q.ReferenceMonth == dataReferencia
                                                          && q.DepreciationAccount != null
                                                          && q.ClosingId == null
                                                          && q.DepreciationMonth > 0
                                                       select q
                                                    ).AsNoTracking();

                resultados = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                base.GravaLogErro(ex);
            }

            acaoPagamentoNotaLancamento = (ehEstorno ? "S" : "N");
            var data = new DateTime(dataFechamento.Year, dataFechamento.Month, 30);
            foreach (var item in resultados)
            {
                tokenAuditoriaIntegracao = Guid.NewGuid().ToString();
                chaveSIAFMonitora = String.Format("F{0}_tokenSAMP:{1}", acaoPagamentoNotaLancamento, tokenAuditoriaIntegracao);
                SIAFDOC sIAFDOC = new SIAFDOC();
                sIAFDOC.CdMsg = "SIAFNlPatrimonial";

                SiafemDocNlPatrimonial siafemDocNlPatrimonial = new SiafemDocNlPatrimonial();

                NotaFiscal notaFiscal = new NotaFiscal();

                Repeticao repeticao = new Repeticao();
                string finalDaConta = (item.DepreciationAccount == null ? "0000" : item.DepreciationAccount.ToString().Substring(5, 4));
                string mes = item.ReferenceMonth.Substring(4, 2);
                string ano = item.ReferenceMonth.Substring(2, 2);
                NF nF = new NF()
                {
                    NotaFiscal = string.Format("{0}{1}{2}{3}", managerUnitCode, finalDaConta, mes, ano) //"UGE + conta + mesref exemplo: 380185123110303062020"
                };
                IM iM = new IM()
                {
                    ItemMaterial = item.ItemAccounts.Value.ToString()
                };
                repeticao.IM = iM;
                repeticao.NF = nF;

                ItemMaterial itemMaterial = new ItemMaterial()
                {
                    Repeticao = repeticao
                };
                notaFiscal.Repeticao = repeticao;
                siafemDocNlPatrimonial.NotaFiscal = notaFiscal;
                siafemDocNlPatrimonial.ItemMaterial = itemMaterial;

                XmlDoc xmlDoc = new XmlDoc()
                {
                    Id = chaveSIAFMonitora,
                    TipoMovimento = "DEPRECIAÇÃO",
                    Data = data.ToString("dd/MM/yyyy"),
                    Gestao = item.ManagerCode.Replace(" ", "").PadLeft(5, '0'),
                    Tipo_Entrada_Saida_Reclassificacao_Depreciacao = "DEPRECIAÇÃO",
                    CpfCnpjUgeFavorecida = "",
                    GestaoFavorecida = "",
                    Item = item.ItemAccounts.Value.ToString(),
                    TipoEstoque = "PERMANENTE",
                    Estoque = "",
                    EstoqueDestino = "DESPESA DE DEPRECIAÇÃO DE BENS MÓVEIS (VARIAÇÃO)",
                    EstoqueOrigem = item.AccountName,
                    TipoMovimentacao = "",
                    ValorTotal = item.DepreciationMonth.ToString().Replace(",", "."),
                    ControleEspecifico = "",
                    ControleEspecificoEntrada = "",
                    ControleEspecificoSaida = "",
                    FonteRecurso = "",
                    NLEstorno = acaoPagamentoNotaLancamento,
                    Empenho = "",
                    Observacao = tokenAuditoriaIntegracao,
                    UgeOrigem = managerUnitCode.ToString()

                };


                siafemDocNlPatrimonial.Documento = xmlDoc;
                sIAFDOC.SiafemDocNlPatrimonial = siafemDocNlPatrimonial;

                retornos.Add(sIAFDOC);
                sIAFDOC = null;

            }

            return retornos;
        }

        private async Task<List<SIAFDOC>> CriaPendencias(int IdUGE, DateTime dataFechamento, bool ehEstorno = false)
        {
            string tokenAuditoriaIntegracao = null;
            string chaveSIAFMonitora = null;
            string acaoPagamentoNotaLancamento = null;
            IList<AccountingClosing> resultados = null;
            List<SIAFDOC> retornos = new List<SIAFDOC>();
            var dataReferencia = dataFechamento.Year.ToString() + dataFechamento.Month.ToString().PadLeft(2, '0');
            ManagerUnit UGE = new ManagerUnit();
            AuditoriaIntegracao auditoria;
            int managerUnitCode = 0;

            try
            {
                UGE = (from m in db.ManagerUnits
                       where m.Id == IdUGE
                       select m).FirstOrDefault();

                managerUnitCode = Convert.ToInt32(UGE.Code);

                IQueryable<AccountingClosing> query = (from q in this.db.AccountingClosings
                                                       where q.ManagerUnitCode == managerUnitCode
                                                          && q.ReferenceMonth == dataReferencia
                                                          && q.DepreciationAccount != null
                                                          && q.ClosingId == null
                                                          && q.DepreciationMonth > 0
                                                       select q
                                                    ).AsNoTracking();

                resultados = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                base.GravaLogErro(ex);
            }

            acaoPagamentoNotaLancamento = (ehEstorno ? "S" : "N");
            var data = dataFechamento.AddMonths(1);
            data = data.AddDays(-1);

            string mes = string.Empty;
            string ano = string.Empty;

            foreach (var item in resultados)
            {
                bool gera = false;

                if (item.AuditoriaIntegracaoId == null)
                {
                    gera = true;
                }
                else if ( db.NotaLancamentoPendenteSIAFEMs
                            .Where(n => n.AuditoriaIntegracaoId == item.AuditoriaIntegracaoId).Count() == 0
                         )
                {
                    gera = true;
                }

                if (gera)
                {
                    tokenAuditoriaIntegracao = Guid.NewGuid().ToString();
                    chaveSIAFMonitora = String.Format("F{0}_tokenSAMP:{1}", acaoPagamentoNotaLancamento, tokenAuditoriaIntegracao);

                    Repeticao repeticao = new Repeticao();
                    string finalDaConta = (item.DepreciationAccount == null ? "0000" : item.DepreciationAccount.ToString().Substring(5, 4));

                    mes = item.ReferenceMonth.Substring(4, 2);
                    ano = item.ReferenceMonth.Substring(2, 2);
                    NF nF = new NF()
                    {
                        NotaFiscal = string.Format("{0}{1}{2}{3}", managerUnitCode, finalDaConta, mes, ano) //"UGE + conta + mes + últimos dígitos do ano. Exemplo: 380185 123110303 06 2020 -> 38018503030620"
                    };
                    IM iM = new IM()
                    {
                        ItemMaterial = item.ItemAccounts.Value.ToString()
                    };

                    repeticao.IM = iM;
                    repeticao.NF = nF;

                    ItemMaterial itemMaterial = new ItemMaterial()
                    {
                        Repeticao = repeticao
                    };

                    NotaFiscal notaFiscal = new NotaFiscal();
                    notaFiscal.Repeticao = repeticao;

                    XmlDoc xmlDoc = new XmlDoc()
                    {
                        Id = chaveSIAFMonitora,
                        TipoMovimento = "DEPRECIAÇÃO",
                        Data = data.ToString("dd/MM/yyyy"),
                        Gestao = item.ManagerCode.Replace(" ", "").PadLeft(5, '0'),
                        Tipo_Entrada_Saida_Reclassificacao_Depreciacao = "DEPRECIAÇÃO",
                        CpfCnpjUgeFavorecida = "",
                        GestaoFavorecida = "",
                        Item = item.ItemAccounts.Value.ToString(),
                        TipoEstoque = "PERMANENTE",
                        Estoque = "",
                        EstoqueDestino = "DESPESA DE DEPRECIAÇÃO DE BENS MÓVEIS (VARIAÇÃO)",
                        EstoqueOrigem = item.AccountName,
                        TipoMovimentacao = "",
                        ValorTotal = item.DepreciationMonth.ToString().Replace(",", "."),
                        ControleEspecifico = "",
                        ControleEspecificoEntrada = "",
                        ControleEspecificoSaida = "",
                        FonteRecurso = "",
                        NLEstorno = acaoPagamentoNotaLancamento,
                        Empenho = "",
                        Observacao = tokenAuditoriaIntegracao,
                        UgeOrigem = managerUnitCode.ToString()
                    };

                    SIAFDOC sIAFDOC = new SIAFDOC();
                    sIAFDOC.CdMsg = "SIAFNlPatrimonial";
                    sIAFDOC.SiafemDocNlPatrimonial = new SiafemDocNlPatrimonial();
                    sIAFDOC.SiafemDocNlPatrimonial.NotaFiscal = notaFiscal;
                    sIAFDOC.SiafemDocNlPatrimonial.ItemMaterial = itemMaterial;
                    sIAFDOC.SiafemDocNlPatrimonial.Documento = xmlDoc;

                    auditoria = geraLinhasAuditoriaIntegracaoNovo(sIAFDOC, item, IdUGE, ehEstorno);
                    geraNotaLancamentoPendenciaNovo(auditoria);

                    item.AuditoriaIntegracaoId = auditoria.Id;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    retornos.Add(sIAFDOC);
                    sIAFDOC = null;
                }
            }

            return retornos;
        }

        private AuditoriaIntegracao geraLinhasAuditoriaIntegracaoNovo(SIAFDOC listaObjXML, AccountingClosing objAccountingClosing, int IdUGE, bool ehEstorno = false)
        {
            IList<AuditoriaIntegracao> linhasAuditoriaIntegracaoCriadasNoBancoDeDados = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoParaBD = null;
            string xmlParaWebserviceModoNovo = null;

            linhasAuditoriaIntegracaoCriadasNoBancoDeDados = new List<AuditoriaIntegracao>();

            registroAuditoriaIntegracaoParaBD = this.criaRegistroAuditoriaNovo(listaObjXML, objAccountingClosing, IdUGE, ehEstorno);
            xmlParaWebserviceModoNovo = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(registroAuditoriaIntegracaoParaBD, ehEstorno);

            registroAuditoriaIntegracaoParaBD.MsgEstimuloWS = xmlParaWebserviceModoNovo;

            var usuarioLogado = UserCommon.CurrentUser();
            var cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);
            registroAuditoriaIntegracaoParaBD.UsuarioSAM = cpfUsuarioSessaoLogada;

            db.AuditoriaIntegracoes.Add(registroAuditoriaIntegracaoParaBD);
            db.SaveChanges();

            return registroAuditoriaIntegracaoParaBD;
        }

        private AuditoriaIntegracao criaRegistroAuditoriaNovo(SIAFDOC objSIAFDOC, AccountingClosing objAccountingClosing, int IdUGE, bool ehEstorno = false)
        {
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            string chaveSIAFMONITORA = null;
            string campoObservacao = null;
            string campoNotaFiscal = null;


            if (objSIAFDOC.IsNotNull())
            {
                chaveSIAFMONITORA = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Id.Substring(13, 36);
                CultureInfo cultureInfo = new CultureInfo("pt-BR");
                registroAuditoriaIntegracao = new AuditoriaIntegracao();

                registroAuditoriaIntegracao.DocumentoId = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Id;
                registroAuditoriaIntegracao.TipoMovimento = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoMovimento;
                registroAuditoriaIntegracao.Data = DateTime.ParseExact(objSIAFDOC.SiafemDocNlPatrimonial.Documento.Data, "dd/MM/yyyy", cultureInfo);
                registroAuditoriaIntegracao.UgeOrigem = objSIAFDOC.SiafemDocNlPatrimonial.Documento.UgeOrigem;
                registroAuditoriaIntegracao.Gestao = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Gestao;
                registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                registroAuditoriaIntegracao.CpfCnpjUgeFavorecida = objSIAFDOC.SiafemDocNlPatrimonial.Documento.CpfCnpjUgeFavorecida;
                registroAuditoriaIntegracao.GestaoFavorecida = objSIAFDOC.SiafemDocNlPatrimonial.Documento.GestaoFavorecida;
                registroAuditoriaIntegracao.Item = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Item;
                registroAuditoriaIntegracao.TipoEstoque = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoEstoque;
                registroAuditoriaIntegracao.Estoque = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Estoque;
                registroAuditoriaIntegracao.EstoqueDestino = objSIAFDOC.SiafemDocNlPatrimonial.Documento.EstoqueDestino;
                registroAuditoriaIntegracao.EstoqueOrigem = objSIAFDOC.SiafemDocNlPatrimonial.Documento.EstoqueOrigem;
                registroAuditoriaIntegracao.TipoMovimentacao = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoMovimentacao;
                registroAuditoriaIntegracao.ValorTotal = Decimal.Parse(objSIAFDOC.SiafemDocNlPatrimonial.Documento.ValorTotal.Replace(".", ","), cultureInfo);
                registroAuditoriaIntegracao.ControleEspecifico = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecifico;
                registroAuditoriaIntegracao.ControleEspecificoEntrada = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecificoEntrada;
                registroAuditoriaIntegracao.ControleEspecificoSaida = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecificoSaida;
                registroAuditoriaIntegracao.FonteRecurso = objSIAFDOC.SiafemDocNlPatrimonial.Documento.FonteRecurso;
                registroAuditoriaIntegracao.NLEstorno = objSIAFDOC.SiafemDocNlPatrimonial.Documento.NLEstorno;
                registroAuditoriaIntegracao.Empenho = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Empenho;
                //registroAuditoriaIntegracao.Observacao                                     = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Observacao;
                //registroAuditoriaIntegracao.NotaFiscal                                     = objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14);

                campoObservacao = montaCampoObservacao(objSIAFDOC);
                campoNotaFiscal = ((objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Length > 14) ? objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14) : objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal);
                registroAuditoriaIntegracao.Observacao = campoObservacao;
                registroAuditoriaIntegracao.NotaFiscal = campoNotaFiscal;
                registroAuditoriaIntegracao.ItemMaterial = objSIAFDOC.SiafemDocNlPatrimonial.ItemMaterial.Repeticao.IM.ItemMaterial;
                registroAuditoriaIntegracao.TokenAuditoriaIntegracao = Guid.Parse(chaveSIAFMONITORA);

                registroAuditoriaIntegracao.ManagerUnitId = IdUGE;
                registroAuditoriaIntegracao.DataEnvio = DateTime.Now;
            }

            return registroAuditoriaIntegracao;
        }

        private bool geraNotaLancamentoPendenciaNovo(AuditoriaIntegracao objAuditoria)
        {
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;

            if (objAuditoria.IsNotNull())
            {
                NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
                {
                    AuditoriaIntegracaoId = objAuditoria.Id,
                    DataHoraEnvioMsgWS = objAuditoria.DataEnvio,
                    ManagerUnitId = objAuditoria.ManagerUnitId,
                    TipoNotaPendencia = (short)TipoNotaSIAFEM.NL_Depreciacao,
                    NumeroDocumentoSAM = objAuditoria.NotaFiscal,
                    StatusPendencia = 1,
                    ErroProcessamentoMsgWS = "(O processo de fechamento ainda não foi finalizado.Clique em reenviar, ou vá na tela de fechamento e clique no botão prosseguir, para gerar as NLs.)",
                };

                db.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                numeroRegistrosManipulados = db.SaveChanges();
            }

            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }

        private AuditoriaIntegracao criaRegistroAuditoria(SIAFDOC objSIAFDOC, bool ehEstorno = false)
        {
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            string chaveSIAFMONITORA = null;
            string campoObservacao = null;
            string campoNotaFiscal = null;


            if (objSIAFDOC.IsNotNull())
            {
                chaveSIAFMONITORA = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Id.Substring(13, 36);
                CultureInfo cultureInfo = new CultureInfo("pt-BR");
                registroAuditoriaIntegracao = new AuditoriaIntegracao();

                registroAuditoriaIntegracao.DocumentoId = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Id;
                registroAuditoriaIntegracao.TipoMovimento = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoMovimento;
                registroAuditoriaIntegracao.Data = DateTime.ParseExact(objSIAFDOC.SiafemDocNlPatrimonial.Documento.Data, "dd/MM/yyyy", cultureInfo);
                registroAuditoriaIntegracao.UgeOrigem = objSIAFDOC.SiafemDocNlPatrimonial.Documento.UgeOrigem;
                registroAuditoriaIntegracao.Gestao = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Gestao;
                registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                registroAuditoriaIntegracao.CpfCnpjUgeFavorecida = objSIAFDOC.SiafemDocNlPatrimonial.Documento.CpfCnpjUgeFavorecida;
                registroAuditoriaIntegracao.GestaoFavorecida = objSIAFDOC.SiafemDocNlPatrimonial.Documento.GestaoFavorecida;
                registroAuditoriaIntegracao.Item = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Item;
                registroAuditoriaIntegracao.TipoEstoque = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoEstoque;
                registroAuditoriaIntegracao.Estoque = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Estoque;
                registroAuditoriaIntegracao.EstoqueDestino = objSIAFDOC.SiafemDocNlPatrimonial.Documento.EstoqueDestino;
                registroAuditoriaIntegracao.EstoqueOrigem = objSIAFDOC.SiafemDocNlPatrimonial.Documento.EstoqueOrigem;
                registroAuditoriaIntegracao.TipoMovimentacao = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoMovimentacao;
                registroAuditoriaIntegracao.ValorTotal = Decimal.Parse(objSIAFDOC.SiafemDocNlPatrimonial.Documento.ValorTotal.Replace(".", ","), cultureInfo);
                registroAuditoriaIntegracao.ControleEspecifico = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecifico;
                registroAuditoriaIntegracao.ControleEspecificoEntrada = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecificoEntrada;
                registroAuditoriaIntegracao.ControleEspecificoSaida = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecificoSaida;
                registroAuditoriaIntegracao.FonteRecurso = objSIAFDOC.SiafemDocNlPatrimonial.Documento.FonteRecurso;
                registroAuditoriaIntegracao.NLEstorno = objSIAFDOC.SiafemDocNlPatrimonial.Documento.NLEstorno;
                registroAuditoriaIntegracao.Empenho = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Empenho;
                //registroAuditoriaIntegracao.Observacao                                     = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Observacao;
                //registroAuditoriaIntegracao.NotaFiscal                                     = objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14);

                campoObservacao = montaCampoObservacao(objSIAFDOC);
                campoNotaFiscal = ((objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Length > 14) ? objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14) : objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal);
                registroAuditoriaIntegracao.Observacao = campoObservacao;
                registroAuditoriaIntegracao.NotaFiscal = campoNotaFiscal;
                registroAuditoriaIntegracao.ItemMaterial = objSIAFDOC.SiafemDocNlPatrimonial.ItemMaterial.Repeticao.IM.ItemMaterial;
                registroAuditoriaIntegracao.TokenAuditoriaIntegracao = Guid.Parse(chaveSIAFMONITORA);


                registroAuditoriaIntegracao.ManagerUnitId = obterUgeId(Int32.Parse(objSIAFDOC.SiafemDocNlPatrimonial.Documento.UgeOrigem));
                registroAuditoriaIntegracao.DataEnvio = DateTime.Now;
            }



            return registroAuditoriaIntegracao;
        }

        private string montaCampoObservacao(SIAFDOC objXML)
        {
            string campoObservacao = null;
            string campoNotaFiscal = null;
            string contaContabilDepreciacao = null;
            string codigoUGE = null;
            string numeroDocumentoSAM = null;
            string mesReferencia = null;
            string mesReferenciaFechamento = null;
            string anoMesReferencia = null;
            string tokenSAMPatrimonio = null;
            string indicativoEstorno = null;
            bool ehEstorno = false;

            /*
               NotaFiscal = string.Format("{0}{1}{2}{3}", managerUnitCode, finalDaConta, mes, ano) "380128 0109 07 20"
               'FECHAMENTO MENSAL INTEGRADO':'UGE 380xxx'; 
               'DocumentoSAM':'380xxx01010720'; 
               'Mes-Referencia':'07/2020'; 
               'Conta Contabil Depreciação':'12380101'; 
               'Token SAM-Patrimonio': '00000000-0000-0000-0000-000000000000'.
            */

            if (objXML.IsNotNull())
            {
                numeroDocumentoSAM = objXML.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14);
                campoNotaFiscal = numeroDocumentoSAM;

                ehEstorno = (objXML.SiafemDocNlPatrimonial.Documento.NLEstorno.ToUpperInvariant() == "S");
                indicativoEstorno = (ehEstorno ? "ESTORNO DE " : null);
                codigoUGE = campoNotaFiscal.Substring(0, 6);
                mesReferencia = campoNotaFiscal.Substring(10, 2);
                anoMesReferencia = campoNotaFiscal.Substring(12, 2);
                mesReferenciaFechamento = String.Format("{0}/20{1}", mesReferencia, anoMesReferencia);
                contaContabilDepreciacao = obtemNumeroContaContabilDepreciacao(campoNotaFiscal);
                tokenSAMPatrimonio = objXML.SiafemDocNlPatrimonial.Documento.Id.Substring(13, 36);
                campoObservacao = String.Format(CST_CAMPO_OBSERVACAO, indicativoEstorno, codigoUGE, numeroDocumentoSAM, mesReferenciaFechamento, contaContabilDepreciacao, tokenSAMPatrimonio);
            }

            return campoObservacao;
        }

        private int obterUgeId(int codigoUGE)
        {
            int UGEId = 0;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (codigoUGE > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Code == codigoUGE.ToString()
                                                                            && ugeSIAFEM.Status == true)
                                                           .FirstOrDefault();
                if (dadosUGE.IsNotNull())
                    UGEId = dadosUGE.Id;
            }

            return UGEId;
        }
        private string GetXML(int managerUnitCode, SIAFDOC xmlsIAFDOC, string path)
        {
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                //retira o namespace
                ns.Add("", "");
                XmlSerializer serializer = new XmlSerializer(typeof(SIAFDOC));
                string documentoNome = string.Format("{0}/{1}_{2}{3}{4}{5}.xml", path, managerUnitCode, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Second, DateTime.Now.Minute, DateTime.Now.Millisecond);

                /* Gerar o arquivo fisico para teste.
                //TextWriter writer = new StreamWriter(documentoNome);
                //writer.Close();
                */
                StringWriter stringWriter = new Utf8StringWriter();
                serializer.Serialize(stringWriter, xmlsIAFDOC, ns);

                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        // Área comentada por não ser backup. Não apagar
        //[HttpPost]
        //public async Task<JsonResult> EfetuaFechamento_Backup(int managerUnitId)
        //{
        //    getHierarquiaPerfil();



        //    string _mensagemParaUsuario = string.Empty;
        //    IList<DepreciacaoMensagemViewModel> retornoDepreciacoes = new List<DepreciacaoMensagemViewModel>();
        //    try
        //    {
        //        var managerUnit = (from m in db.ManagerUnits
        //                           where m.Id == managerUnitId
        //                           select m).AsNoTracking().FirstOrDefault();

        //        if (!ValidaFechamento(managerUnit.ManagmentUnit_YearMonthReference))
        //        {
        //            _mensagemParaUsuario = "Fechamento Inválido.";

        //            string messageErro = "Fechamento Inválido.";
        //            string stackTrace = "Método EfetuaFechamento, Controller Closings";
        //            string name = "Rotina de FECHAMENTO";

        //            GravaLogErro(messageErro, stackTrace, name);
        //            return Json(new { data = "", mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
        //        }

        //        if (ValidaPendenciasDeTransferenciaOuDoacao(managerUnit.Id, managerUnit.ManagmentUnit_YearMonthReference))
        //        {
        //            _mensagemParaUsuario = "Existem Transferência/Doação pendentes de serem recebidas, entre em contato com a UGE de Destino para solução do problema.";

        //            return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
        //        }

        //        if (ValidaBPsASeremIncorporados(managerUnit.Id, managerUnit.ManagmentUnit_YearMonthReference))
        //        {
        //            _mensagemParaUsuario = "Existem BPs a serem incorporados/recebidos até a data do mês de referência atual.";

        //            return Json(new { data = "", mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
        //        }

        //        //string spName = "FECHAMENTO";

        //        //List<ListaParamentros> listaParam = new List<ListaParamentros>();
        //        //listaParam.Add(new ListaParamentros { nomeParametro = "managerUnitId", valor = managerUnitId });
        //        //listaParam.Add(new ListaParamentros { nomeParametro = "loginID", valor = _login });



        //        //DataTable dt = common.ReturnDataFromStoredProcedureReport(listaParam, spName);

        //        int ano = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
        //        int mes = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));



        //        DateTime mesAnoReferencia = new DateTime(ano, mes, 1);
        //        DateTime mesAnoReferenciaVerificar = mesAnoReferencia.AddMonths(-1);

        //        var primeiraDepreciacao = await (from d in db.MonthlyDepreciations where d.ManagerUnitId == managerUnitId select d).Take(1).FirstOrDefaultAsync();
        //        if (primeiraDepreciacao != null)
        //        {
        //            FechamentoFactory factory = FechamentoFactory.GetInstancia();


        //            var materialDepreciacao = factory.CreateMaterialItemDepreciacao(System.Data.IsolationLevel.ReadUncommitted);
        //            var materiaisNaoDepreciados = materialDepreciacao.GetNaoDepreciados(managerUnitId, mesAnoReferencia).ToList();

        //            if (materiaisNaoDepreciados.Count > 0)
        //            {
        //                return Json(new { data = materiaisNaoDepreciados, mensagem = "NaoDepreciados" }, JsonRequestBehavior.AllowGet);
        //            }

        //        }



        //        _mensagemParaUsuario = "Fechamento realizado com sucesso";
        //        db.Configuration.AutoDetectChangesEnabled = false;
        //        db.Configuration.LazyLoadingEnabled = false;
        //        db.Configuration.ProxyCreationEnabled = false;

        //        var _query = (from a in db.Assets
        //                      join m in db.AssetMovements on a.Id equals m.AssetId
        //                      where a.MaterialGroupCode != 0
        //                         && a.ManagerUnitId == managerUnitId
        //                         && a.flagVerificado == null
        //                         && a.flagDepreciaAcumulada == 1
        //                         && (a.flagAcervo == null || a.flagAcervo == false)
        //                         && (a.flagTerceiro == null || a.flagTerceiro == false)
        //                         && (a.flagAnimalNaoServico == null || a.flagAnimalNaoServico == false)
        //                         && (m.FlagEstorno == null || m.FlagEstorno == false)
        //                         && (m.AssetTransferenciaId == null)
        //                      select new
        //                      {
        //                          MaterialItemCode = a.MaterialItemCode,
        //                          AssetStartId = a.AssetStartId,
        //                          AssetId = a.Id
        //                      }).Distinct();

        //        var depreciacoes = _query.ToList();
        //        var dataFechamento = mesAnoReferencia.AddMonths(1);

        //        foreach (var depreciacao in depreciacoes)
        //        {
        //            int _assetStartId = 0;
        //            var transferencia = await (from t in db.AssetMovements where t.AssetTransferenciaId == depreciacao.AssetId select t).FirstOrDefaultAsync();
        //            if (!depreciacao.AssetStartId.HasValue)
        //            {

        //                if (transferencia != null)
        //                {
        //                    _assetStartId = transferencia.AssetId;
        //                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + depreciacao.AssetId.ToString());
        //                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + depreciacao.AssetId.ToString());
        //                }
        //                else
        //                {
        //                    _assetStartId = depreciacao.AssetId;
        //                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + depreciacao.AssetId.ToString());
        //                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + depreciacao.AssetId.ToString());

        //                }
        //            }
        //            else
        //            {
        //                _assetStartId = depreciacao.AssetStartId.Value;
        //            }


        //            if (transferencia != null)
        //            {
        //                SqlParameter[] parametrosTransferencia =
        //             {
        //                    new SqlParameter("@ManagerUnitId", transferencia.ManagerUnitId),
        //                    new SqlParameter("@MaterialItemCode", depreciacao.MaterialItemCode),
        //                    new SqlParameter("@AssetStartId", _assetStartId),
        //                    new SqlParameter("@DataFinal", mesAnoReferencia),
        //                    new SqlParameter("@Fechamento", true),
        //                    new SqlParameter("@Retorno",System.Data.SqlDbType.VarChar,1000) { Direction= System.Data.ParameterDirection.Output },
        //                    new SqlParameter("@Erro",System.Data.SqlDbType.Bit) { Direction= System.Data.ParameterDirection.Output }

        //                };
        //                var registro = this.db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[MonthlyDepreciation] WHERE [MaterialItemCode] = " + depreciacao.MaterialItemCode.ToString() + "  AND [AssetStartId] = " + _assetStartId.ToString());
        //                var resultadoTransferencia = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT, @Erro OUT", parametrosTransferencia);
        //            }


        //            SqlParameter[] parametros =
        //            {
        //                 new SqlParameter("@ManagerUnitId",managerUnitId ),
        //                                 new SqlParameter("@MaterialItemCode", depreciacao.MaterialItemCode),
        //                                 new SqlParameter("@AssetStartId", _assetStartId),
        //                                 new SqlParameter("@DataFinal", mesAnoReferencia),
        //                                 new SqlParameter("@Fechamento", false),
        //                                 new SqlParameter("@Retorno",System.Data.SqlDbType.VarChar,1000) { Direction= System.Data.ParameterDirection.Output },
        //                                 new SqlParameter("@Erro",System.Data.SqlDbType.Bit) { Direction= System.Data.ParameterDirection.Output }

        //            };

        //            var resultado = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT,@Erro OUT", parametros);
        //            if (bool.Parse(parametros[6].Value.ToString()) == true)
        //            {
        //                var depreciacaoMensagem = new DepreciacaoMensagemViewModel();

        //                depreciacaoMensagem.AssetId = depreciacao.AssetId;
        //                depreciacaoMensagem.AssetStartId = _assetStartId;
        //                depreciacaoMensagem.MaterialItemCode = depreciacao.MaterialItemCode;
        //                depreciacaoMensagem.Mensagem = parametros[5].Value.ToString();
        //                depreciacaoMensagem.NumberIdentification = (from ben in db.Assets where ben.Id == depreciacao.AssetId select ben.NumberIdentification).FirstOrDefault();
        //                depreciacaoMensagem.AcquisitionDate = (from ben in db.Assets where ben.Id == depreciacao.AssetId select ben.AcquisitionDate).FirstOrDefault().ToString("dd/MM/yyyy");
        //                depreciacaoMensagem.Erro = bool.Parse(parametros[6].Value.ToString());
        //                depreciacaoMensagem.MesReferencia = dataFechamento.Month.ToString().PadLeft(2, '0') + "/" + dataFechamento.Year.ToString();

        //                retornoDepreciacoes.Add(depreciacaoMensagem);
        //            }



        //        }

        //        managerUnit.ManagmentUnit_YearMonthReference = dataFechamento.Year.ToString() + dataFechamento.Month.ToString().PadLeft(2, '0');
        //        db.Entry(managerUnit).State = EntityState.Modified;
        //        db.SaveChanges();
        //        if (retornoDepreciacoes.Where(x => x.Erro == true).FirstOrDefault() == null)
        //        {

        //            return Json(new { data = "", mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var retorno = retornoDepreciacoes.OrderBy(x => x.Erro).ToList();

        //            return Json(new { data = retorno, mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return MensagemErroJson(CommonMensagens.PadraoException, ex);
        //    }


        //}

        [HttpPost]
        public async Task<JsonResult> EfetuaReabertura(int managerUnitId)
        {
            string _mensagemParaUsuario = string.Empty;
            ManagerUnit managerUnit = null;
            int ano;
            int mes;

            DateTime data;

            try
            {
                managerUnit = (from m in db.ManagerUnits
                               where m.Id == managerUnitId
                               select m).AsNoTracking().FirstOrDefault();

                if (!ValidaReabertura(managerUnit))
                {
                    _mensagemParaUsuario = "Reabertura Inválida.";

                    string messageErro = "Reabertura Inválida.";
                    string stackTrace = "Método EfetuaReabertura, Controller Closings";
                    string name = "Rotina de REABERTURA";

                    GravaLogErro(messageErro, stackTrace, name);
                    return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }

                if (managerUnit.FlagIntegracaoSiafem)
                {
                    if (UGETemBPsComPendenciaDeDados(managerUnit.Id))
                    {
                        return Json(new { data = "", mensagem = "Existem BPs com pendência de dados a serem corrigidos. Por gentileza, consulte as telas Bens Patrimoniais Pendentes  e Bens Patrimoniais Pendentes(Integração) e faça as devidas correções. " }, JsonRequestBehavior.AllowGet);
                    }

                    if (UGEComPendenciaSIAFEMDoFechamento(managerUnit.Code, managerUnit.ManagmentUnit_YearMonthReference))
                    {
                        return Json(new { data = "", mensagem = CommonMensagens.OperacaoInvalidaIntegracaoFechamento }, JsonRequestBehavior.AllowGet);
                    }

                    User u = UserCommon.CurrentUser();
                    var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                    var relacaoUsuarioPerfil = BuscaPerfilLogado(u.Id);

                    if (relacaoUsuarioPerfil.ProfileId == (int)EnumProfile.AdministradordeUO)
                    {
                        if (!ValidaAteDataFechamentoDoMesRefSIAFEM(managerUnit.ManagmentUnit_YearMonthReference))
                        {
                            _mensagemParaUsuario = "Mês não pode ser reaberto, pois o fechamento SIAFEM já foi realizado. Em caso de dúvidas, entre em contato com a Secretaria da Fazenda.";

                            return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        if (!ValidaAteDataFechamentoDoMesRefSIAFEMParaOperador(managerUnit.ManagmentUnit_YearMonthReference))
                        {
                            _mensagemParaUsuario = "Mês não pode ser reaberto por operadores. Por gentileza, entre em contato um superior (que possua perfil de Administrador de UO no sistema) para realizar a reabertura.";

                            return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                        }

                    }

                    if (ValidaTerPendenciaDeNL(managerUnit.Id))
                    {
                        _mensagemParaUsuario = "UGE possui pendências de Nota Lançamento para serem resolvidas.";

                        return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                    }
                }
                else {
                    if (OrgaoIntegradoAoSIAFEMDeUGENaoIntegrada(managerUnit.BudgetUnitId))
                    {

                        if (UGEComPendenciaSIAFEMDoFechamento(managerUnit.Code, managerUnit.ManagmentUnit_YearMonthReference))
                        {
                            return Json(new { data = "", mensagem = CommonMensagens.OperacaoInvalidaIntegracaoFechamento }, JsonRequestBehavior.AllowGet);
                        }

                        if (ValidaTerPendenciaDeNL(managerUnit.Id))
                        {
                            _mensagemParaUsuario = "UGE possui pendências de Nota Lançamento para serem resolvidas.";

                            return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                        }

                        if (UGEComPendenciaSIAFEMDoFechamento(managerUnit.Code, managerUnit.ManagmentUnit_YearMonthReference))
                        {
                            return Json(new { data = "", mensagem = CommonMensagens.OperacaoInvalidaIntegracaoFechamento }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                ano = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
                mes = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));

                data = new DateTime(ano, mes, 1);

                if (
                    UGEComBpsASeremReclassificadosDoGrupoSetentaENova(data, managerUnit.Id) &&
                    UGEComBpsASeremReclassificadosDiferenteDoGrupoSetentaENova(data, managerUnit.Id)
                   )
                {
                    return Json(new { data = "", mensagem = "Existem BPs que precisam de reclassificar a conta contábil. Por gentileza, vá na tela Reclassificação Contábil para efetuar essa reclassificação" }, JsonRequestBehavior.AllowGet);
                }

                if (ValidaPendenciasDeTransferenciaOuDoacao(managerUnit.Id, managerUnit.ManagmentUnit_YearMonthReference))
                {
                    _mensagemParaUsuario = "Existem Transferência/Doação pendentes de serem recebidas, entre em contato com a UGE de Destino para solução do problema.";

                    return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }

                if (ValidaPendenciasDeSaidaInservivelReabertura(managerUnit.Id, managerUnit.ManagmentUnit_YearMonthReference))
                {
                    _mensagemParaUsuario = "Existem Saída de Inservíveis ou Recebimento de Inservíveis a serem feitos até a data do mês de referência atual.";

                    return Json(new { data = "", mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                GravaLogErroSemRetorno(ex);
                return Json(new { data = "", mensagem = "Devido algumas instabilidades encontradas, o sistema não iniciou o processo de reabertura dessa UGE. Por gentileza, tente novamente mais tarde." }, JsonRequestBehavior.AllowGet);
            }

            DbContextTransaction transaction = null;
            try
            {
                DateTime dataAtual = data.AddMonths(-1);

                string mesAReabrir = dataAtual.Year.ToString() + dataAtual.Month.ToString().PadLeft(2, '0');

                if (managerUnit.FlagIntegracaoSiafem)
                {
                    return Json(new { integrada = true, numeroUGE = managerUnit.Id, mesAnterior = mesAReabrir }, JsonRequestBehavior.AllowGet);
                }

                managerUnit.ManagmentUnit_YearMonthReference = dataAtual.Year.ToString() + dataAtual.Month.ToString().PadLeft(2, '0');

                SqlParameter[] parametros =
                {
                         new SqlParameter("@ManagerUnitId", managerUnit.Id)
                };
                
                transaction = this.db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                db.Entry(managerUnit).State = EntityState.Modified;

                await db.SaveChangesAsync();

                var resultadoTransferencia = await this.db.Database.ExecuteSqlCommandAsync("EXEC [dbo].[SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA] @ManagerUnitId", parametros);

                transaction.Commit();


                //AGUARDANDO ATUALIZACAO DE DADOS DE EMAIL POR PARTE DE USUARIOS
                //if (managerUnit.Code.StartsWith("380"))
                //{
                //    bool integradaAoContabilizaSP = managerUnit.FlagIntegracaoSiafem;
                //    int codigoUGE = Int32.Parse(managerUnit.Code);
                //    string tipoEventoFechamento = "reabertura";
                //    await enviaEMailParaComite(codigoUGE, integradaAoContabilizaSP, dataAtual, tipoEventoFechamento, null);
                //}

                _mensagemParaUsuario = "Reabertura realizada com sucesso";

                return Json(new { mensagem = _mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                if (transaction != null)
                    transaction.Rollback();

                GravaLogErroSemRetorno(e);
                return Json(new { data = "", mensagem = "Ocorreu algum problema e o processo de reabertura foi abortado. Não houve alteração nos valores dos BPs por essa ocorrência. Por gentileza, tente novamente mais tarde." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult VerificaIntegracaoSIAFEM(int managerUnitId)
        {
            bool integradaSIAFEM = UGEEstaIntegrada(managerUnitId);
            return Json(integradaSIAFEM, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RecuperaMesAnoFechamentoAtual(int budgetUnitId, int managerUnitId)
        {
            try
            {
                var mesAnoReferenciaAtual = (from m in db.ManagerUnits
                                             where m.BudgetUnitId == budgetUnitId &&
                                                   m.Id == managerUnitId
                                             select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

                string _mesAnoReferenciaAtual = mesAnoReferenciaAtual.Substring(4, 2).PadLeft(2, '0') + "/" + mesAnoReferenciaAtual.Substring(0, 4).PadLeft(4, '0');

                return Json(new { mesAnoReferenciaAtual = _mesAnoReferenciaAtual }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //base.GravaLogErro(ex);
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public JsonResult RecuperaMesAnoParaReabertura(int budgetUnitId, int managerUnitId)
        {
            try
            {
                var mesRefAtual = (from m in db.ManagerUnits
                                   where m.BudgetUnitId == budgetUnitId &&
                                         m.Id == managerUnitId
                                   select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

                int anoRef = int.Parse(mesRefAtual.Substring(0, 4));
                int mesRef = int.Parse(mesRefAtual.Substring(4, 2));

                var dataReferenciaAtual = new DateTime(anoRef, mesRef, 1);
                var dataParaReferenciaDaReabertura = dataReferenciaAtual.AddMonths(-1);

                string _mesAnoParaReabertura = dataParaReferenciaDaReabertura.Month.ToString().PadLeft(2, '0') + "/" + dataParaReferenciaDaReabertura.Year.ToString().PadLeft(4, '0');

                return Json(new { mesAnoParaReabertura = _mesAnoParaReabertura }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public JsonResult ValidaFechamentoOuReabertura(int budgetUnitId, int managerUnitId)
        {
            try
            {
                var managerUnit = (from m in db.ManagerUnits
                                   where m.BudgetUnitId == budgetUnitId &&
                                         m.Id == managerUnitId
                                   select m).FirstOrDefault();

                string _mesAnoReferenciaAtual = managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2).PadLeft(2, '0') + "/" + managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');

                bool integradoAoSiafem = managerUnit.FlagIntegracaoSiafem || OrgaoIntegradoAoSIAFEMDeUGENaoIntegrada(managerUnit.BudgetUnitId);

                if (integradoAoSiafem)
                {
                    if (UGEComPendenciaSIAFEMDoFechamento(managerUnit.Code, managerUnit.ManagmentUnit_YearMonthReference))
                    {
                        return Json(new { validaFechamento = false, validaReabertura = false, mesAnoReferenciaAtual = _mesAnoReferenciaAtual, integradoSIAFEM = 1 }, JsonRequestBehavior.AllowGet);
                    }
                }

                var _validaFechamento = ValidaFechamento(managerUnit.ManagmentUnit_YearMonthReference);

                var _validaReabertura = ValidaReabertura(managerUnit);

                if (managerUnit.FlagIntegracaoSiafem)
                {
                    User u = UserCommon.CurrentUser();
                    var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                    var relacaoUsuarioPerfil = BuscaPerfilLogado(u.Id);

                    if (relacaoUsuarioPerfil.ProfileId == (int)EnumProfile.AdministradordeUO)
                    {
                        _validaReabertura = ValidaAteDataFechamentoDoMesRefSIAFEM(managerUnit.ManagmentUnit_YearMonthReference);
                    }
                    else
                    {
                        _validaReabertura = ValidaAteDataFechamentoDoMesRefSIAFEMParaOperador(managerUnit.ManagmentUnit_YearMonthReference);
                    }

                    if (_validaFechamento)
                    {
                        _validaFechamento = !ValidaTerPendenciaDeNL(managerUnit.Id);
                    }
                }

                return Json(new { validaFechamento = _validaFechamento, validaReabertura = _validaReabertura, mesAnoReferenciaAtual = _mesAnoReferenciaAtual, integradoSIAFEM = managerUnit.FlagIntegracaoSiafem }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GravaLogErroSemRetorno(ex);

                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        #region Métodos privados
        private void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _login = u.Id;
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
        }
        private void CarregaHierarquiaFiltro(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0)
        {
            hierarquia = new Hierarquia();
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
        private bool ValidaFechamento(string mesRefAtualUGE)
        {
            int anoRef = int.Parse(mesRefAtualUGE.Substring(0, 4));
            int mesRef = int.Parse(mesRefAtualUGE.Substring(4, 2));

            var DataAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var DataRefUGE = new DateTime(anoRef, mesRef, 1);

            if (DataRefUGE >= DataAtual)
                return false;
            else
                return true;
        }
        private DateTime GetMesAnoFechamento(string mesRefAtualUGE)
        {
            int ano = int.Parse(mesRefAtualUGE.Substring(0, 4));
            int mes = int.Parse(mesRefAtualUGE.Substring(4, 2));

            var AnoMesReferencia = new DateTime(ano, mes, 1);

            return AnoMesReferencia;
        }

        private bool ValidaReabertura(ManagerUnit managerUnit)
        {
            int anoIni = int.Parse(managerUnit.ManagmentUnit_YearMonthStart.Substring(0, 4));
            int mesIni = int.Parse(managerUnit.ManagmentUnit_YearMonthStart.Substring(4, 2));

            int anoRef = int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
            int mesRef = int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));

            var DataInicialUGE = new DateTime(anoIni, mesIni, 1);
            var DataRefUGE = new DateTime(anoRef, mesRef, 1);

            if (DataInicialUGE >= DataRefUGE)
                return false;
            else
            {
                return true;
            }
        }
        private bool ValidaPendenciasDeTransferenciaOuDoacao(int managerUnitId, string mesReferenciaAtual)
        {
            bool existemAssetsPendentesDeTransferenciasOuDoacao = (from a in db.Assets
                                                                   join am in db.AssetMovements on a.Id equals am.AssetId
                                                                   where am.ManagerUnitId == managerUnitId &&
                                                                         am.Status == true &&
                                                                         !a.flagVerificado.HasValue &&
                                                                         a.flagDepreciaAcumulada == 1 &&
                                                                         (
                                                                            am.MovementTypeId == (int)EnumMovimentType.Transferencia ||
                                                                            am.MovementTypeId == (int)EnumMovimentType.Doacao ||
                                                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                                                            am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                                                                            am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado
                                                                         ) &&
                                                                         mesReferenciaAtual.Equals(am.MovimentDate.Year.ToString() +
                                                                                                  (am.MovimentDate.Month < 10 ? "0" + am.MovimentDate.Month.ToString() : am.MovimentDate.Month.ToString()))
                                                                   select a.Id).Any();

            //bool existemInserviveisPendentesDeRecebimento = (from a in db.Assets
            //                                                 join am in db.AssetMovements on a.Id equals am.AssetId
            //                                                 where am.ManagerUnitId == managerUnitId &&
            //                                                       am.Status == false &&
            //                                                       am.FlagEstorno != true &&
            //                                                       am.AssetTransferenciaId == null &&
            //                                                       !a.flagVerificado.HasValue &&
            //                                                       a.flagDepreciaAcumulada == 1 &&
            //                                                       (
            //                                                          am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
            //                                                          am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao
            //                                                       ) &&
            //                                                       mesReferenciaAtual.Equals(am.MovimentDate.Year.ToString() +
            //                                                                                (am.MovimentDate.Month < 10 ? "0" + am.MovimentDate.Month.ToString() : am.MovimentDate.Month.ToString()))
            //                                                 select a.Id).Any();

            return existemAssetsPendentesDeTransferenciasOuDoacao;
        }

        private bool ValidaPendenciasDeSaidaInservivelReabertura(int managerUnitId, string mesReferenciaAtual)
        {
            var listaDatasMovimentacoes = (from a in db.Assets
                                           join am in db.AssetMovements on a.Id equals am.AssetId
                                           where (am.ManagerUnitId == managerUnitId || am.SourceDestiny_ManagerUnitId == managerUnitId) &&
                                                 am.Status == false &&
                                                 am.FlagEstorno != true &&
                                                 am.AssetTransferenciaId == null &&
                                                 !a.flagVerificado.HasValue &&
                                                 a.flagDepreciaAcumulada == 1 &&
                                                 (
                                                    am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
                                                    am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao
                                                 )
                                           select am.MovimentDate).ToList();

            string mesRefMovimento = string.Empty;

            if (listaDatasMovimentacoes != null && listaDatasMovimentacoes.Count > 0)
            {
                if (listaDatasMovimentacoes.Count == 1)
                {
                    DateTime registroUnico = listaDatasMovimentacoes.First();
                    mesRefMovimento = registroUnico.Year.ToString() + (registroUnico.Month < 10 ? "0" + registroUnico.Month.ToString() : registroUnico.Month.ToString());
                }
                else
                {
                    DateTime registroMin = listaDatasMovimentacoes.Min();
                    mesRefMovimento = registroMin.Year.ToString() + (registroMin.Month < 10 ? "0" + registroMin.Month.ToString() : registroMin.Month.ToString());
                }

                return Convert.ToInt32(mesReferenciaAtual) <= Convert.ToInt32(mesRefMovimento);
            }
            else
            {
                return false;
            }
        }

        private bool ValidaPendenciasDeSaidaInservivelFechamento(int managerUnitId, string mesReferenciaAtual)
        {
            var listaDatasMovimentacoes = (from a in db.Assets
                                           join am in db.AssetMovements on a.Id equals am.AssetId
                                           where (am.ManagerUnitId == managerUnitId || am.SourceDestiny_ManagerUnitId == managerUnitId) &&
                                                 am.Status == false &&
                                                 am.FlagEstorno != true &&
                                                 am.AssetTransferenciaId == null &&
                                                 !a.flagVerificado.HasValue &&
                                                 a.flagDepreciaAcumulada == 1 &&
                                                 (
                                                    am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
                                                    am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao
                                                 )
                                           select am.MovimentDate).Distinct().ToList();

            string mesRefMovimento = string.Empty;

            if (listaDatasMovimentacoes != null && listaDatasMovimentacoes.Count > 0)
            {
                if (listaDatasMovimentacoes.Count == 1)
                {
                    DateTime registroUnico = listaDatasMovimentacoes.First();
                    mesRefMovimento = registroUnico.Year.ToString() + (registroUnico.Month < 10 ? "0" + registroUnico.Month.ToString() : registroUnico.Month.ToString());
                }
                else
                {
                    DateTime registroMax = listaDatasMovimentacoes.Max();
                    mesRefMovimento = registroMax.Year.ToString() + (registroMax.Month < 10 ? "0" + registroMax.Month.ToString() : registroMax.Month.ToString());
                }

                return Convert.ToInt32(mesReferenciaAtual) >= Convert.ToInt32(mesRefMovimento);
            }
            else
            {
                return false;
            }
        }

        private bool ValidaBPsASeremIncorporados(int managerUnitId, string mesReferencia)
        {
            int ano = Convert.ToInt32(mesReferencia.Substring(0, 4));
            int mes = Convert.ToInt32(mesReferencia.Substring(4, 2));

            bool existemAssetsASeremIncorporados = (from am in db.AssetMovements
                                                    where am.Status == true
                                                    && am.SourceDestiny_ManagerUnitId == managerUnitId
                                                    &&
                                                    (
                                                       am.MovementTypeId == (int)EnumMovimentType.Transferencia ||
                                                       am.MovementTypeId == (int)EnumMovimentType.Doacao ||
                                                       am.MovementTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                                       am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado ||
                                                       am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado
                                                    ) &&
                                                    (am.MovimentDate.Year < ano || (am.MovimentDate.Year == ano && am.MovimentDate.Month <= mes))
                                                    select am.Id).Any();

            //bool existemAssetsInserviveisASeremIncorporados = (from am in db.AssetMovements
            //                                                   where am.Status == false
            //                                                   && am.SourceDestiny_ManagerUnitId == managerUnitId
            //                                                   && am.FlagEstorno != true
            //                                                   && am.AssetTransferenciaId == null
            //                                                   &&
            //                                                   (
            //                                                      am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
            //                                                      am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao
            //                                                   ) &&
            //                                                   (am.MovimentDate.Year < ano || (am.MovimentDate.Year == ano && am.MovimentDate.Month <= mes))
            //                                                   select am.Id).Any();

            return existemAssetsASeremIncorporados;
        }

        private bool UGETemBPsComPendenciaDeDados(int managerUnitId)
        {
            bool existemBPPendentes = (from a in db.Assets
                                       join am in db.AssetMovements
                                       on a.Id equals am.AssetId
                                       where a.Status == true &&
                                             a.flagVerificado.HasValue &&
                                             a.flagDepreciaAcumulada != 1 &&
                                             am.ManagerUnitId == managerUnitId &&
                                             am.Status == true &&
                                             am.FlagEstorno != true
                                       select a).Any();

            return existemBPPendentes;
        }

        private bool UGEComBpsASeremReclassificados(int managerUnitId)
        {
            bool existemBPASeremReclassificados = (from rc in db.BPsARealizaremReclassificacaoContabeis
                                                   where rc.IdUGE == managerUnitId
                                                   select rc).Any();

            return existemBPASeremReclassificados;
        }

        private bool UGEComBpsASeremReclassificadosDiferenteDoGrupoSetentaENova(DateTime mesAnoReferencia, int managerUnitId)
        {
            if (mesAnoReferencia < new DateTime(2020, 11, 1))
                return false;

            bool existemBPASeremReclassificados = (from rc in db.BPsARealizaremReclassificacaoContabeis
                                                   where rc.IdUGE == managerUnitId
                                                   && rc.GrupoMaterial != 79
                                                   select rc).Any();

            return existemBPASeremReclassificados;
        }

        private bool UGEComBpsASeremReclassificadosDoGrupoSetentaENova(DateTime mesAnoReferencia, int managerUnitId)
        {
            if (mesAnoReferencia < new DateTime(2021, 02, 1))
                return false;

            bool existemBPASeremReclassificados = (from rc in db.BPsARealizaremReclassificacaoContabeis
                                                   where rc.IdUGE == managerUnitId
                                                   && rc.GrupoMaterial == 79
                                                   select rc).Any();

            return existemBPASeremReclassificados;
        }

        private bool ValidaTerPendenciaDeNL(int managerUnitId)
        {
            var cont = (from q in db.NotaLancamentoPendenteSIAFEMs
                        where q.ManagerUnitId == managerUnitId
                           && q.StatusPendencia == 1
                        select q).Count();

            return cont != 0;
        }

        private bool ValidaAteDataFechamentoDoMesRefSIAFEM(string mesRef)
        {
            int ano = Convert.ToInt32(mesRef.Substring(0, 4));
            int mes = Convert.ToInt32(mesRef.Substring(4, 2));
            string auxiliar = string.Empty;

            if (mesRef.Substring(4, 2) == "01")
            {
                auxiliar = (ano - 1).ToString() + "12";
            }
            else
            {
                auxiliar = ano.ToString() + (mes - 1).ToString().PadLeft(2, '0');
            }

            int mesRefComoInteiro = Convert.ToInt32(auxiliar);

            var dataFechamento = (from s in db.SiafemCalendars
                                  where s.ReferenceMonth == mesRefComoInteiro && s.Status
                                  select s.DateClosing).FirstOrDefault();

            if (dataFechamento == null)
                return false;
            else
                return DateTime.Now.Date <= dataFechamento.Value.Date;
        }

        private bool ValidaAteDataFechamentoDoMesRefSIAFEMParaOperador(string mesRef)
        {
            int ano = Convert.ToInt32(mesRef.Substring(0, 4));
            int mes = Convert.ToInt32(mesRef.Substring(4, 2));
            string auxiliar = string.Empty;

            if (mesRef.Substring(4, 2) == "01")
            {
                auxiliar = (ano - 1).ToString() + "12";
            }
            else
            {
                auxiliar = ano.ToString() + (mes - 1).ToString().PadLeft(2, '0');
            }

            int mesRefComoInteiro = Convert.ToInt32(auxiliar);

            var dataFechamento = (from s in db.SiafemCalendars
                                  where s.ReferenceMonth == mesRefComoInteiro && s.Status
                                  select s.DataParaOperadores).FirstOrDefault();

            if (dataFechamento == null)
                return false;
            else
                return DateTime.Now.Date <= dataFechamento.Value.Date;
        }

        private int GetItematerialXML(int depreciationAccount)
        {
            IDictionary<int, int> keyValues = new Dictionary<int, int>();

            keyValues.Add(123810105, 3407489);
            keyValues.Add(123810110, 5034280);
            keyValues.Add(123810101, 5026660);
            keyValues.Add(123810103, 2306255);
            keyValues.Add(123810109, 3310566);
            keyValues.Add(123810104, 4989406);
            keyValues.Add(123810102, 5084784);
            keyValues.Add(123810111, 4624777);
            keyValues.Add(123810199, 3552675);

            return keyValues[depreciationAccount];
        }
        #endregion

        #region DadosSIAFEM

        [HttpPost]
        public async Task<ActionResult> DadosSIAFEMParaValidar(int uge, string mesref) //Codigo da UGE e mes-ref do mes fechado
        {
            try
            {
                DadosSIAFEMValidacaoFechamentoViewModel dados = new DadosSIAFEMValidacaoFechamentoViewModel();
                using (db = new SAMContext())
                {
                    var codigoUGE = (from m in db.ManagerUnits
                                     where m.Id == uge
                                     select m.Code).FirstOrDefault();

                    var codigoInteiro = Convert.ToInt32(codigoUGE);

                    int ano = Convert.ToInt32(mesref.Substring(0, 4));
                    int mes = Convert.ToInt32(mesref.Substring(4, 2));
                    var mesAnoReferencia = new DateTime(ano, mes, 1);
                    var documentos = await this.CriaPendencias(uge, mesAnoReferencia);

                    dados.dadosPorContaContabil = (from d in db.DepreciationAccountingClosings
                                                   where d.ManagerUnitCode == codigoInteiro
                                                   && d.ReferenceMonth == mesref
                                                   select new DadosSiafemTabelaViewModel
                                                   {
                                                       NumeroConta = d.BookAccount.ToString(),
                                                       Valor = d.AccountingValue
                                                   }).ToList();

                    dados.dadosPorContaDepreciacao = (from a in db.AccountingClosings
                                                      where a.ManagerUnitCode == codigoInteiro
                                                      && a.ReferenceMonth == mesref
                                                      && a.DepreciationAccount != null
                                                      && a.ClosingId == null
                                                      && a.DepreciationMonth > 0
                                                      && a.GeneratedNL == null
                                                      select new DadosSiafemTabelaViewModel
                                                      {
                                                          NumeroConta = a.DepreciationAccount.ToString(),
                                                          Valor = a.DepreciationMonth
                                                      }).ToList();
                }

                return PartialView("_DadosSIAFEMParaValidar", dados);
            }
            catch (Exception e)
            {
                return PartialView("_DadosSIAFEMParaValidar");
            }
        }

        [HttpPost]
        public ActionResult DadosSIAFEMParaValidarReabertura(int uge, string mesref) //Codigo da UGE e mes-ref do mes fechado
        {
            try
            {
                DadosSIAFEMValidacaoFechamentoViewModel dados = new DadosSIAFEMValidacaoFechamentoViewModel();
                using (db = new SAMContext())
                {
                    var codigoUGE = (from m in db.ManagerUnits
                                     where m.Id == uge
                                     select m.Code).FirstOrDefault();

                    var codigoInteiro = Convert.ToInt32(codigoUGE);

                    dados.dadosPorContaDepreciacao = (from a in db.AccountingClosings
                                                      where a.ManagerUnitCode == codigoInteiro
                                                      && a.ReferenceMonth == mesref
                                                      && a.DepreciationAccount != null
                                                      && a.ClosingId == null
                                                      && a.DepreciationMonth > 0
                                                      select new DadosSiafemTabelaViewModel
                                                      {
                                                          NumeroConta = a.DepreciationAccount.ToString(),
                                                          Valor = a.DepreciationMonth,
                                                          NL = a.GeneratedNL
                                                      }).ToList();
                }

                return PartialView("_DadosSIAFEMParaEstornarNls", dados);
            }
            catch (Exception e)
            {
                return PartialView("_DadosSIAFEMParaEstornarNls");
            }
        }

        [HttpPost]
        public async Task Abortar(int uge, string mesref)
        {
            int codigoUGE = 0;
            IList<int> idsAuditoriaIntegracao = null;
            DbContextTransaction transaction = null;

            try
            {
                string spName = "SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA";

                List<ListaParamentros> listaParam = new List<ListaParamentros>();
                listaParam.Add(new ListaParamentros { nomeParametro = "managerUnitId", valor = uge });

                common = new FunctionsCommon();
                DataTable dt = common.ReturnDataFromStoredProcedureReport(listaParam, spName);

                SqlParameter[] parametros =
                {
                         new SqlParameter("@ManagerUnitId", uge)
                };

                ManagerUnit UGE = (from m in db.ManagerUnits
                                   where m.Id == uge
                                   select m).FirstOrDefault();

                UGE.ManagmentUnit_YearMonthReference = mesref;

                //Registros da tabela 'AccoutingClosings' que apontem para 'AuditoriaIntegracao' precisam ser excluidos antes da limpeza.
                Int32.TryParse(UGE.Code, out codigoUGE);
                idsAuditoriaIntegracao = this.db.AccountingClosings
                                                .Where(fechamentoMensalContaContabil => fechamentoMensalContaContabil.ManagerUnitCode == codigoUGE
                                                                                     && fechamentoMensalContaContabil.ReferenceMonth == mesref
                                                                                     && (fechamentoMensalContaContabil.AuditoriaIntegracaoId.HasValue && fechamentoMensalContaContabil.AuditoriaIntegracaoId.Value > 0))
                                                .Select(fechamentoMensalContaContabil => fechamentoMensalContaContabil.AuditoriaIntegracaoId.Value)
                                                .ToList();
                transaction = this.db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                db.Entry(UGE).State = EntityState.Modified;
                db.SaveChanges();

                var resultadoTransferencia = await this.db.Database.ExecuteSqlCommandAsync("EXEC [dbo].[SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA] @ManagerUnitId", parametros);
                string query = string.Format("DELETE FROM [dbo].[DepreciationAccountingClosing] WHERE [ManagerUnitCode] = {0} AND [ReferenceMonth] = {1}", UGE.Code, mesref);
                await this.db.Database.ExecuteSqlCommandAsync(query);
                query = string.Format("DELETE FROM [dbo].[AccountingClosing] WHERE [ManagerUnitCode] = {0} AND [ReferenceMonth] = {1}", UGE.Code, mesref);
                await this.db.Database.ExecuteSqlCommandAsync(query);

                transaction.Commit();


                if (idsAuditoriaIntegracao.HasElements())
                {
                    foreach (var auditoriaIntegracaoId in idsAuditoriaIntegracao)
                        this.excluiNotaLancamentoPendencia(auditoriaIntegracaoId);

                    foreach (var auditoriaIntegracaoId in idsAuditoriaIntegracao)
                        this.excluiAuditoriaIntegracaoFechamentoMensalIntegrado(auditoriaIntegracaoId);
                }
            }
            catch (Exception ex)
            {
                base.GravaLogErro(ex);
                if (transaction != null)
                    transaction.Rollback();

                throw ex;
            }
            finally
            {
                transaction.Dispose();
                transaction = null;

            }
        }

        [HttpPost]
        public async Task<ActionResult> Prosseguir(int uge, string mesref, string loginSiafem, string senhaSiafem)
        {
            try
            {
                bool ErroNaPrimeiraNL = true;
                bool GerouPendencia = false;

                Tuple<string, string, string, string, bool> envioComandoGeracaoNL = null;
                AuditoriaIntegracao registroAuditoriaIntegracao = null;
                int contadorNL = 0;
                string msgErroSIAFEM = null;
                IList<string> listaMsgErrosSIAFEM = null;

                var managerUnit = (from q in db.ManagerUnits where q.Id == uge select q).FirstOrDefault();

                var CodigoUGE = managerUnit.Code;
                int ano = Convert.ToInt32(mesref.Substring(0, 4));
                int mes = Convert.ToInt32(mesref.Substring(4, 2));

                var usuarioLogado = UserCommon.CurrentUser();
                var cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);

                string contaContabilDepreciacao = null;

                var codigoComoInteiro = Convert.ToInt32(CodigoUGE);

                var listaAccountingClosing = (from a in db.AccountingClosings
                                                  where a.ManagerUnitCode == codigoComoInteiro
                                                  && a.ReferenceMonth == mesref
                                                  && a.DepreciationAccount != null
                                                  && a.ClosingId == null
                                                  && a.AuditoriaIntegracaoId != null
                                                  && a.DepreciationMonth > 0
                                                  select a).ToList();

                bool JaTemRegistro = listaAccountingClosing.Where(l => l.GeneratedNL != null).Count() > 0;
                var listaAMandarParaContabiliza = listaAccountingClosing.Where(l => l.GeneratedNL == null).ToList();

                listaMsgErrosSIAFEM = new List<string>();
                var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();

                foreach (AccountingClosing objAccountingClosing in listaAMandarParaContabiliza)
                {
                    registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao((int)objAccountingClosing.AuditoriaIntegracaoId);
                    contaContabilDepreciacao = objAccountingClosing.DepreciationAccount.ToString();

                    registroAuditoriaIntegracao.DataEnvio = DateTime.Now;

                    envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoFechamentoMensalIntegradoNoSIAF((int)objAccountingClosing.AuditoriaIntegracaoId, cpfUsuarioSessaoLogada, loginSiafem, senhaSiafem, false, TipoNotaSIAFEM.NL_Depreciacao);
                    ++contadorNL;

                    if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                    {
                        ErroNaPrimeiraNL = false;
                        AtualizaDadosCasoHouveRetorno(registroAuditoriaIntegracao);
                        continue; //segue o loop
                    }
                    else
                    {
                        ErroNaPrimeiraNL = (contadorNL == 1);
                        msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)

                        if (ErroNaPrimeiraNL && !JaTemRegistro) //se deu 'erro SIAFEM' no primeiro XML
                        {
                            break; //interrompe o loop
                        }
                        else //caso nao seja o primeiro XML enviado, que retornou 'erro SIAFEM'
                        {
                            GerouPendencia = true;
                            listaMsgErrosSIAFEM.Add(msgErroSIAFEM);
                            AtualizaPendenciaCasoNaoHouverRetorno(registroAuditoriaIntegracao); //gerar 'Pendencia NL SIAFEM' e continuar o envio dos  XML's
                            continue;
                        }
                    }

                }

                DadosSIAFEMValidacaoFechamentoViewModel dados = new DadosSIAFEMValidacaoFechamentoViewModel();

                if (ErroNaPrimeiraNL)
                {
                    //NAO EH POSSIVEL LIMPAR OS REGISTROS GERADOS ENQUANTO AS REFERENCIAS NA TABELA 'AccountingClosings' nao forem limpas
                    //foreach (var linhaAuditoriaIntegracao in linhasAuditoriaIntegracao)
                    //    this.excluiNotaLancamentoPendencia(linhaAuditoriaIntegracao.Id);

                    //foreach (var linhaAuditoriaIntegracao in linhasAuditoriaIntegracao)
                    //    this.excluiAuditoriaIntegracaoFechamentoMensalIntegrado(linhaAuditoriaIntegracao.Id);

                    //AQUI
                    dados.MensagemDeErro = msgErroSIAFEM;
                    AbortarNaoAsync(managerUnit, mesref);
                    dados.primeiraNLComErro = true;
                    return PartialView("_ResultadoDasNLs", dados);
                }

                dados.gerouPendencia = GerouPendencia;

                //AQUI
                if (GerouPendencia)
                {
                    dados.LstMsgsPendencias = new List<string>();
                    dados.LstMsgsPendencias = listaMsgErrosSIAFEM.ToList();
                }

                dados.dadosPorContaDepreciacao = (from a in db.AccountingClosings
                                                  where a.ManagerUnitCode == codigoComoInteiro
                                                  && a.ReferenceMonth == mesref
                                                  && a.DepreciationAccount != null
                                                  && a.ClosingId == null
                                                  && a.GeneratedNL != null
                                                  select new DadosSiafemTabelaViewModel
                                                  {
                                                      NumeroConta = a.DepreciationAccount.ToString(),
                                                      Valor = a.DepreciationMonth,
                                                      NL = a.GeneratedNL
                                                  }).ToList();

                return PartialView("_ResultadoDasNLs", dados);
            }
            catch (Exception e)
            {
                base.GravaLogErro(e);
                return PartialView("_ResultadoDasNLs");
            }
        }

        private void AtualizaDadosCasoHouveRetorno(AuditoriaIntegracao auditoria) {
            var pendencia = db.NotaLancamentoPendenteSIAFEMs
                              .Where(p => p.AuditoriaIntegracaoId == auditoria.Id)
                              .FirstOrDefault();

            pendencia.DataHoraEnvioMsgWS = auditoria.DataEnvio;
            pendencia.StatusPendencia = 0;

            db.Entry(pendencia).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void AtualizaPendenciaCasoNaoHouverRetorno(AuditoriaIntegracao auditoria) {
            var pendencia = db.NotaLancamentoPendenteSIAFEMs
                              .Where(p => p.AuditoriaIntegracaoId == auditoria.Id)
                              .FirstOrDefault();

            pendencia.DataHoraEnvioMsgWS = auditoria.DataEnvio;
            pendencia.ErroProcessamentoMsgWS = auditoria.MsgErro;
 
            db.Entry(pendencia).State = EntityState.Modified;
            db.SaveChanges();
        }

        private bool geraNotaLancamentoPendencia(int auditoriaIntegracaoId)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            TipoNotaSIAFEM tipoAgrupamentoNotaLancamento = TipoNotaSIAFEM.Desconhecido;
            string numeroDocumentoSAM = null;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;




            if (auditoriaIntegracaoId > 0)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);

                if (registroAuditoriaIntegracao.IsNotNull())
                {
                    tipoAgrupamentoNotaLancamento = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);
                    //listaMovimentacoesPatrimoniais = obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria(auditoriaIntegracaoId);
                    //if (listaMovimentacoesPatrimoniais.HasElements())
                    {
                        numeroDocumentoSAM = registroAuditoriaIntegracao.NotaFiscal;
                        pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
                        {
                            AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id,
                            DataHoraEnvioMsgWS = registroAuditoriaIntegracao.DataEnvio,
                            ManagerUnitId = registroAuditoriaIntegracao.ManagerUnitId,
                            TipoNotaPendencia = (short)tipoAgrupamentoNotaLancamento,
                            NumeroDocumentoSAM = numeroDocumentoSAM,
                            StatusPendencia = 1,
                            ErroProcessamentoMsgWS = registroAuditoriaIntegracao.MsgErro,
                        };
                        using (_contextoCamadaDados = new SAMContext())
                        {
                            var _registroAuditoriaIntegracao = _contextoCamadaDados.AuditoriaIntegracoes
                                                                                   .Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id)
                                                                                   .FirstOrDefault();
                            _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                            numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                        }
                    }
                }
            }


            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }
        private bool atualizaAccountingClosingExcluidos(AccountingClosingExcluidos linhaFechamentoMensalIntegradaParaEstorno)
        {
            int numeroRegistrosManipulados = 0;

            try
            {
                if (linhaFechamentoMensalIntegradaParaEstorno.Id > 0)
                    //using (var contextoCamadaDados = new SAMContext())
                    //{
                    //contextoCamadaDados.AccountingClosingExcluidos.Attach(linhaFechamentoMensalIntegradaParaEstorno);
                    db.Entry(linhaFechamentoMensalIntegradaParaEstorno).State = EntityState.Modified;


                numeroRegistrosManipulados = db.SaveChanges();
                //}
            }
            catch (Exception excErroRuntime)
            {
                throw excErroRuntime;
            }

            return (numeroRegistrosManipulados > 0);
        }
        private bool insereRegistroAuditoriaParcialNaBaseDeDados(AuditoriaIntegracao entidadeAuditoria)
        {
            int numeroRegistrosManipulados = 0;
            SAMContext contextoCamadaDados = null;
            try
            {
                using (contextoCamadaDados = new SAMContext())
                {
                    contextoCamadaDados.AuditoriaIntegracoes.Add(entidadeAuditoria);
                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";
                ErroLog rowTabelaLOG = new ErroLog()
                {
                    DataOcorrido = DateTime.Now,
                    Usuario = entidadeAuditoria.UsuarioSAM,
                    exMessage = msgErro,
                    exStackTrace = excErroOperacaoBancoDados.StackTrace,
                    exTargetSite = excErroOperacaoBancoDados.TargetSite.ToString()
                };

                contextoCamadaDados.ErroLogs.Add(rowTabelaLOG);
            }

            return (numeroRegistrosManipulados > 0);
        }
        private bool excluiAuditoriaIntegracaoFechamentoMensalIntegrado(int auditoriaIntegracaoId)
        {
            bool auditoriaIntegracaoFoiExcluida = false;
            int numeroRegistrosManipulados = 0;
            AuditoriaIntegracao registroAuditoriaIntegracaoFechamentoMensalIntegrado = null;



            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    registroAuditoriaIntegracaoFechamentoMensalIntegrado = contextoCamadaDados.AuditoriaIntegracoes
                                                                               .Where(_registroAuditoriaIntegracao => _registroAuditoriaIntegracao.Id == auditoriaIntegracaoId)
                                                                               .FirstOrDefault();

                    if (registroAuditoriaIntegracaoFechamentoMensalIntegrado.IsNotNull())
                    {
                        contextoCamadaDados.AuditoriaIntegracoes.Remove(registroAuditoriaIntegracaoFechamentoMensalIntegrado);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                    }
                }
            }


            auditoriaIntegracaoFoiExcluida = (numeroRegistrosManipulados > 0);
            return auditoriaIntegracaoFoiExcluida;
        }
        private bool excluiNotaLancamentoPendencia(int auditoriaIntegracaoId)
        {
            bool pendenciaNLFoiExcluida = false;
            int numeroRegistrosManipulados = 0;


            if (auditoriaIntegracaoId > 0)
            {
                NotaLancamentoPendenteSIAFEM pendenciaNL = null;
                using (var contextoCamadaDados = new SAMContext())
                {
                    pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                     .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                     .FirstOrDefault();

                    if (pendenciaNL.IsNotNull())
                    {
                        contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Remove(pendenciaNL);
                        numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    }
                }
            }


            pendenciaNLFoiExcluida = (numeroRegistrosManipulados > 0);
            return pendenciaNLFoiExcluida;
        }
        private AuditoriaIntegracao obterEntidadeAuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            AuditoriaIntegracao objEntidade = null;


            if (auditoriaIntegracaoId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AuditoriaIntegracoes
                                                     .Where(auditoriaIntegracao => auditoriaIntegracao.Id == auditoriaIntegracaoId)
                                                     .FirstOrDefault();
                }

            return objEntidade;
        }
        private TipoNotaSIAFEM obterTipoNotaSIAFEM__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            TipoNotaSIAFEM tipoNotaSIAF = TipoNotaSIAFEM.Desconhecido;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;


            if (auditoriaIntegracaoId > 0)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                switch (registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant())
                {
                    case "ENTRADA":
                    case "SAIDA":
                    case "SAÍDA": { tipoNotaSIAF = TipoNotaSIAFEM.NL_Liquidacao; } break;
                    case "DEPRECIACAO":
                    case "DEPRECIAÇÃO": { tipoNotaSIAF = TipoNotaSIAFEM.NL_Depreciacao; } break;
                    case "RECLASSIFICACAO":
                    case "RECLASSIFICAÇÃO": { tipoNotaSIAF = TipoNotaSIAFEM.NL_Reclassificacao; } break;
                }
            }

            return tipoNotaSIAF;
        }
        private IList<AuditoriaIntegracao> geraLinhasAuditoriaIntegracao(IList<SIAFDOC> listaObjXML)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            IList<AuditoriaIntegracao> linhasAuditoriaIntegracaoCriadasNoBancoDeDados = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoParaBD = null;
            AccountingClosing registroAccountingClosingParaAtualizacao = null;
            string xmlParaWebserviceModoNovo = null;
            string chaveParaPesquisa = null;
            bool ehEstorno = false;
            int numeroRegistrosManipulados = 0;



            if (listaObjXML.HasElements())
            {
                var usuarioLogado = UserCommon.CurrentUser();
                var cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);
                linhasAuditoriaIntegracaoCriadasNoBancoDeDados = new List<AuditoriaIntegracao>();
                foreach (var objSIAFDoc in listaObjXML)
                {
                    ehEstorno = objSIAFDoc.SiafemDocNlPatrimonial.Documento.NLEstorno.ToUpperInvariant() == "S";
                    /*
                    string finalDaConta = (item.DepreciationAccount == null ? "0000" : item.DepreciationAccount.ToString().Substring(5, 4));
                    string mes = item.ReferenceMonth.Substring(4, 2);
                    string ano = item.ReferenceMonth.Substring(2, 2);
                    NotaFiscal = string.Format("{0}{1}{2}{3}", managerUnitCode, finalDaConta, mes, ano) //"UGE + conta + mesref exemplo: 380185123110303062020"
                     */
                    chaveParaPesquisa = objSIAFDoc.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal;
                    registroAccountingClosingParaAtualizacao = contextoCamadaDados.AccountingClosings.Where(rowTabela => (rowTabela.ManagerUnitCode.ToString() + //codigoUGE
                                                                                                                          (rowTabela.DepreciationAccount == null ? "0000" : rowTabela.DepreciationAccount.ToString().Substring(5, 4)) + //final de conta contabil de depreciacao
                                                                                                                          (rowTabela.ReferenceMonth.Substring(4, 2)) + //ano de referencia
                                                                                                                          (rowTabela.ReferenceMonth.Substring(2, 2)) //mes de referencia
                                                                                                                         ) == chaveParaPesquisa)
                                                                                                     .FirstOrDefault();
                    registroAuditoriaIntegracaoParaBD = this.criaRegistroAuditoria(objSIAFDoc, ehEstorno);
                    xmlParaWebserviceModoNovo = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(registroAuditoriaIntegracaoParaBD, ehEstorno);
                    registroAuditoriaIntegracaoParaBD.MsgEstimuloWS = xmlParaWebserviceModoNovo;
                    registroAuditoriaIntegracaoParaBD.UsuarioSAM = cpfUsuarioSessaoLogada;
                    insereRegistroAuditoriaParcialNaBaseDeDados(registroAuditoriaIntegracaoParaBD);

                    if (registroAccountingClosingParaAtualizacao.IsNotNull())
                    {
                        registroAccountingClosingParaAtualizacao.AuditoriaIntegracaoId = registroAuditoriaIntegracaoParaBD.Id;
                        contextoCamadaDados.Entry(registroAccountingClosingParaAtualizacao).State = EntityState.Modified;
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                    }


                    linhasAuditoriaIntegracaoCriadasNoBancoDeDados.Add(registroAuditoriaIntegracaoParaBD);
                }
            }


            return linhasAuditoriaIntegracaoCriadasNoBancoDeDados;
        }

        private void GravaNLGerada(string NotaFiscal, string NLGerada)
        {
            string UGE = NotaFiscal.Substring(0, 6);
            string ContaDepreciacao = NotaFiscal.Substring(6, 4);

            string mes = NotaFiscal.Substring(10, 2);
            string ano = NotaFiscal.Substring(12, 2);

            if (ContaDepreciacao == "0000")
            {
                ContaDepreciacao = null;
            }
            else
            {
                if (ContaDepreciacao == "9999")
                    ContaDepreciacao = "99999" + ContaDepreciacao;
                else
                    ContaDepreciacao = "12381" + ContaDepreciacao;
            }

            string mesRef = "20" + ano + mes;

            string query = string.Format("UPDATE [dbo].[AccountingClosing] SET GeneratedNL = '{0}' where ManagerUnitCode = {1} AND DepreciationAccount = {2} AND ReferenceMonth = {3}",
            NLGerada, UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.db.Database.ExecuteSqlCommand(query);
        }

        private void GravaNLEstornadaGerada(string NotaFiscal, string NLGerada)
        {
            string UGE = NotaFiscal.Substring(0, 6);
            string ContaDepreciacao = NotaFiscal.Substring(6, 4);

            string mes = NotaFiscal.Substring(10, 2);
            string ano = NotaFiscal.Substring(12, 2);

            if (ContaDepreciacao == "0000")
            {
                ContaDepreciacao = null;
            }
            else
            {
                if (ContaDepreciacao == "9999")
                    ContaDepreciacao = "99999" + ContaDepreciacao;
                else
                    ContaDepreciacao = "12381" + ContaDepreciacao;
            }

            string mesRef = "20" + ano + mes;

            string query = string.Format("UPDATE [dbo].[AccountingClosingExcluidos] SET NLEstorno = '{0}' where ManagerUnitCode = {1} AND DepreciationAccount = {2} AND ReferenceMonth = {3}",
            NLGerada, UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.db.Database.ExecuteSqlCommand(query);
        }

        private void DeleteRegistroNaAccountingClosings(string NotaFiscal, string NLGerada)
        {
            string UGE = NotaFiscal.Substring(0, 6);
            string ContaDepreciacao = NotaFiscal.Substring(6, 4);

            string mes = NotaFiscal.Substring(10, 2);
            string ano = NotaFiscal.Substring(12, 2);

            if (ContaDepreciacao == "0000")
            {
                ContaDepreciacao = null;
            }
            else
            {
                if (ContaDepreciacao == "9999")
                    ContaDepreciacao = "99999" + ContaDepreciacao;
                else
                    ContaDepreciacao = "12381" + ContaDepreciacao;
            }

            string mesRef = "20" + ano + mes;

            string tiraDepreciation = string.Format("DELETE from [dbo].[DepreciationAccountingClosing] where [AccountingClosingId] IN (select Id from [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND DepreciationAccount = {1} AND ReferenceMonth = {2})",
            UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.db.Database.ExecuteSqlCommand(tiraDepreciation);

            string tiraDaAccountingClosing = string.Format("DELETE [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND DepreciationAccount = {1} AND ReferenceMonth = {2}",
            UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.db.Database.ExecuteSqlCommand(tiraDaAccountingClosing);

            string tiraDepreciationValorZero = string.Format("DELETE from [dbo].[DepreciationAccountingClosing] where [AccountingClosingId] IN (select Id from [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND ReferenceMonth = {1} AND DepreciationMonth = 0)",
            UGE, mesRef);

            this.db.Database.ExecuteSqlCommand(tiraDepreciationValorZero);

            string tiraDaAccountingClosingValorZero = string.Format("DELETE [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND ReferenceMonth = {1} AND DepreciationMonth = 0",
            UGE, mesRef);

            this.db.Database.ExecuteSqlCommand(tiraDaAccountingClosingValorZero);
        }

        private void DeleteRegistroNaAccountingClosingsExcluidosReabertura(ManagerUnit UGE, string mesref)
        {
            try
            {
                string query = string.Format("DELETE FROM [dbo].[AccountingClosingExcluidos] WHERE [ManagerUnitCode] = {0} AND [ReferenceMonth] = {1} AND NLEstorno IS NULL AND DepreciationMonth > 0", UGE.Code, mesref);
                this.db.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                base.GravaLogErro(ex);
                throw ex;
            }
        }

        private void TerminaReabertura(ManagerUnit UGE, string mesref) {
            try
            {
                UGE.ManagmentUnit_YearMonthReference = mesref;

                db.Entry(UGE).State = EntityState.Modified;
                db.SaveChanges();

                string spName = "SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA";

                List<ListaParamentros> listaParam = new List<ListaParamentros>();
                listaParam.Add(new ListaParamentros { nomeParametro = "managerUnitId", valor = UGE.Id });

                common = new FunctionsCommon();
                DataTable dt = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
            }
            catch (Exception ex) {
                base.GravaLogErro(ex);
                throw ex;
            }
        }

        public async Task<DadosSIAFEMValidacaoFechamentoViewModel> ProsseguirTeste(int uge, string mesref, string loginSiafem, string senhaSiafem)
        {

            string retornoXml = "";
            var dataMesXml = mesref;

           // var fileName = getCaminhoDosArquivoXml("XML");

            var managerUnit = (from q in db.ManagerUnits where q.Id == uge select q).FirstOrDefault();

            var CodigoUGE = managerUnit.Code;

            int ano = Convert.ToInt32(mesref.Substring(0, 4));
            int mes = Convert.ToInt32(mesref.Substring(4, 2));

            var mesAnoReferencia = new DateTime(ano, mes, 1);

            var documentos = await this.GetXmlSIAFEM(int.Parse(CodigoUGE), mesAnoReferencia);
            var procWsSiafem = new ProcessadorServicoSIAF();
            foreach (var item in documentos)
            {
                var xml = this.GetXML(int.Parse(CodigoUGE), item, "");
                //CHAMADA AO SISTEMA CONTABILIZA-SP/SIAFEM
                var resultado = procWsSiafem.ConsumirWS(loginSiafem, senhaSiafem, dataMesXml, CodigoUGE, xml.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", ""), false, true);
                retornoXml = string.Format("{0}{1}", retornoXml, resultado);
            }

            return null;
        }

        #endregion

        private void AbortarNaoAsync(ManagerUnit UGE, string mesref)
        {
            int codigoUGE = 0;
            IList<int> idsAuditoriaIntegracao = null;

            try
            {
                string spName = "SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA";

                List<ListaParamentros> listaParam = new List<ListaParamentros>();
                listaParam.Add(new ListaParamentros { nomeParametro = "managerUnitId", valor = UGE.Id });

                common = new FunctionsCommon();
                DataTable dt = common.ReturnDataFromStoredProcedureReport(listaParam, spName);

                SqlParameter[] parametros =
                   {
                         new SqlParameter("@ManagerUnitId", UGE.Id)
                    };


                UGE.ManagmentUnit_YearMonthReference = mesref;

                Int32.TryParse(UGE.Code, out codigoUGE);
                idsAuditoriaIntegracao = this.db.AccountingClosings
                                                .Where(fechamentoMensalContaContabil => fechamentoMensalContaContabil.ManagerUnitCode == codigoUGE
                                                                                     && fechamentoMensalContaContabil.ReferenceMonth == mesref
                                                                                     && (fechamentoMensalContaContabil.AuditoriaIntegracaoId.HasValue && fechamentoMensalContaContabil.AuditoriaIntegracaoId.Value > 0))
                                                .Select(fechamentoMensalContaContabil => fechamentoMensalContaContabil.AuditoriaIntegracaoId.Value)
                                                .ToList();

                db.Entry(UGE).State = EntityState.Modified;
                db.SaveChanges();

                var resultadoTransferencia = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA] @ManagerUnitId", parametros);
                string query = string.Format("DELETE FROM [dbo].[DepreciationAccountingClosing] WHERE [ManagerUnitCode] = {0} AND [ReferenceMonth] = {1}", UGE.Code, mesref);
                this.db.Database.ExecuteSqlCommand(query);
                query = string.Format("DELETE FROM [dbo].[AccountingClosing] WHERE [ManagerUnitCode] = {0} AND [ReferenceMonth] = {1}", UGE.Code, mesref);
                this.db.Database.ExecuteSqlCommand(query);

                if (idsAuditoriaIntegracao.HasElements())
                {
                    foreach (var auditoriaIntegracaoId in idsAuditoriaIntegracao)
                        this.excluiNotaLancamentoPendencia(auditoriaIntegracaoId);

                    foreach (var auditoriaIntegracaoId in idsAuditoriaIntegracao)
                        this.excluiAuditoriaIntegracaoFechamentoMensalIntegrado(auditoriaIntegracaoId);
                }
            }
            catch (Exception ex)
            {
                base.GravaLogErro(ex);
                throw ex;
            }
        }

        #region Validacao NL de fechamento

        public bool UGEComPendenciaSIAFEMDoFechamento(string CodigoUGEParam, string mesRefAtual)
        {
            int CodigoUGE = Convert.ToInt32(CodigoUGEParam);

            var teste = (from a in db.AccountingClosings
                         where a.ManagerUnitCode == CodigoUGE
                         && (a.GeneratedNL == null || a.GeneratedNL == string.Empty)
                         && a.DepreciationMonth > 0
                         select a).ToList();

            teste = teste.Where(t => Convert.ToInt32(t.ReferenceMonth) > 202006).ToList();

            var listaSemNlFechamento = (from a in db.AccountingClosings
                                        where a.ManagerUnitCode == CodigoUGE
                                        && (a.GeneratedNL == null || a.GeneratedNL == string.Empty)
                                        && a.DepreciationMonth > 0
                                        select a.ReferenceMonth).ToList();

            var listaSemNlFechamentoAposeJunhoDoisMilEVinte = listaSemNlFechamento.Where(t => Convert.ToInt32(t) > 202006).Any();

            if (listaSemNlFechamentoAposeJunhoDoisMilEVinte)
                return true;
            else
            {
                return (from a in db.AccountingClosingExcluidos
                        where a.ManagerUnitCode == CodigoUGE
                        && a.DepreciationMonth > 0
                        && a.ReferenceMonth == mesRefAtual
                        && a.NLEstorno == null
                        select a).Count() > 0;
            }

        }

        public bool OrgaoIntegradoAoSIAFEMDeUGENaoIntegrada(int idDaUO)
        {
            return (from i in db.Institutions
                    join b in db.BudgetUnits on i.Id equals b.InstitutionId
                    where b.Id == idDaUO
                    select i.flagIntegracaoSiafem).FirstOrDefault();
        }

        #endregion

        #region Criacao Auditoria

        #endregion

        #region Prosseguir Reabertura
        [HttpPost]
        public async Task<ActionResult> ProsseguirEstorno(int uge, string mesref, string loginSiafem, string senhaSiafem)
        {
            int codigoUGE = 0;
            IList<AuditoriaIntegracao> linhasAuditoriaIntegracaoParaEstorno = null;
            Tuple<string, string, string, string, bool> envioComandoGeracaoNL = null;
            IntegracaoContabilizaSPController svcIntegracaoSIAFEM = null;
            int auditoriaIntegracaoId = 0;
            TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.NL_Depreciacao;
            bool ehEstorno = false;
            bool ErroNaPrimeiraNL = false;
            bool GerouPendencia = false;
            int contadorNL = 0;
            string msgErroSIAFEM = null;
            string nlDepreciacao = null;
            IList<string> listaNLsGeradas = null;
            string contaContabilDepreciacao = null;
            IList<Tuple<string, string>> listaContaContabil_RetornoSIAFEM = null;
            Tuple<string, string> chaveContaContabil_RetornoSIAFEM = null;
            IList<string> listaMsgErrosSIAFEM = null;
            string cpfUsuarioSessaoLogada = null;
            DadosSIAFEMValidacaoFechamentoViewModel dados = null;



            try
            {
                var managerUnit = (from q in db.ManagerUnits where q.Id == uge select q).FirstOrDefault();
                var strCodigoUGE = managerUnit.Code;

                Int32.TryParse(strCodigoUGE, out codigoUGE);
                svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                linhasAuditoriaIntegracaoParaEstorno = geraDadosParaProcessamentoParaEstornoFechamentoMensalIntegrado(codigoUGE, mesref);
                if (linhasAuditoriaIntegracaoParaEstorno.HasElements())
                {
                    var usuarioLogado = UserCommon.CurrentUser();
                    cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);
                    listaNLsGeradas = new List<string>();
                    listaMsgErrosSIAFEM = new List<string>();
                    listaContaContabil_RetornoSIAFEM = new List<Tuple<string, string>>();
                    foreach (var auditoriaIntegracaoParaEstorno in linhasAuditoriaIntegracaoParaEstorno)
                    {
                        auditoriaIntegracaoId = auditoriaIntegracaoParaEstorno.Id;
                        contaContabilDepreciacao = obtemNumeroContaContabilDepreciacao(auditoriaIntegracaoParaEstorno.NotaFiscal);
                        if (auditoriaIntegracaoId > 0)
                        {
                            //NL Depreciacao
                            tipoNotaSIAFEM = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);
                            ehEstorno = (auditoriaIntegracaoParaEstorno.NLEstorno.ToUpperInvariant() == "S");
                            ++contadorNL;
                            {
                                //CHAMADA AO SISTEMA CONTABILIZA-SP/SIAFEM
                                envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoFechamentoMensalIntegradoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, loginSiafem, senhaSiafem, ehEstorno, tipoNotaSIAFEM);
                                if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                {
                                    nlDepreciacao = envioComandoGeracaoNL.Item3; //captura a NL
                                    //GravaNLEstornadaGerada(auditoriaIntegracaoParaEstorno.NotaFiscal, nlDepreciacao);
                                    DeleteRegistroNaAccountingClosings(auditoriaIntegracaoParaEstorno.NotaFiscal, nlDepreciacao);
                                    listaNLsGeradas.Add(nlDepreciacao);

                                    ErroNaPrimeiraNL = false;
                                    chaveContaContabil_RetornoSIAFEM = Tuple.Create(contaContabilDepreciacao, nlDepreciacao);
                                    listaContaContabil_RetornoSIAFEM.Add(chaveContaContabil_RetornoSIAFEM);
                                    continue; //segue o loop
                                }
                                else
                                {
                                    ErroNaPrimeiraNL = (contadorNL == 1);
                                    msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)

                                    if (ErroNaPrimeiraNL) //se deu 'erro SIAFEM' no primeiro XML
                                    {
                                        break; //interrompe o loop
                                    }
                                    else //caso nao seja o primeiro XML enviado, que retornou 'erro SIAFEM'
                                    {
                                        GerouPendencia = true;
                                        listaMsgErrosSIAFEM.Add(msgErroSIAFEM);
                                        geraNotaLancamentoPendencia(auditoriaIntegracaoId); //gerar 'Pendencia NL SIAFEM' e continuar o envio dos  XML's
                                        chaveContaContabil_RetornoSIAFEM = Tuple.Create(contaContabilDepreciacao, msgErroSIAFEM);
                                        listaContaContabil_RetornoSIAFEM.Add(chaveContaContabil_RetornoSIAFEM);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }


                dados = new DadosSIAFEMValidacaoFechamentoViewModel();
                if (ErroNaPrimeiraNL)
                {
                    //AQUI
                    dados.MensagemDeErro = msgErroSIAFEM;
                    DeleteRegistroNaAccountingClosingsExcluidosReabertura(managerUnit, mesref);
                    dados.primeiraNLComErro = true;
                    return PartialView("_ResultadoNLsEstorno", dados);
                }

                TerminaReabertura(managerUnit, mesref);

                dados.gerouPendencia = GerouPendencia;

                //AQUI
                if (GerouPendencia)
                {
                    dados.LstMsgsPendencias = new List<string>();
                    dados.LstMsgsPendencias = listaMsgErrosSIAFEM.ToList();
                }

                dados.dadosPorContaDepreciacao = (from a in db.AccountingClosingExcluidos
                                                  where a.ManagerUnitCode == codigoUGE
                                                  && a.ReferenceMonth == mesref
                                                  && a.DepreciationAccount != null
                                                  && a.ClosingId == null
                                                  && a.NLEstorno != null
                                                  select new DadosSiafemTabelaViewModel
                                                  {
                                                      NumeroConta = a.DepreciationAccount.ToString(),
                                                      Valor = a.DepreciationMonth,
                                                      NL = a.NLEstorno,
                                                      AuditoriaIdEstorno = (a.AuditoriaIntegracaoIdEstorno == null ? 0 : (int)a.AuditoriaIntegracaoIdEstorno)
                                                  }).ToList();

                var listaIdAuditoria = linhasAuditoriaIntegracaoParaEstorno.Select(x => x.Id).ToList();

                dados.dadosPorContaDepreciacao = dados.dadosPorContaDepreciacao.Where(x => listaIdAuditoria.Contains((x.AuditoriaIdEstorno))).ToList();

                ConstarAlteracoesContabeisDaUGE(uge);

                return PartialView("_ResultadoNLsEstorno", dados);
            }
            catch (Exception e) {
                base.GravaLogErro(e);
                return PartialView("_ResultadoNLsEstorno");
            }
        }

        private string obtemNumeroContaContabilDepreciacao(string notaFiscal)
        {
            string contaContabilDepreciacao = null;

            if (!String.IsNullOrWhiteSpace(notaFiscal) && (notaFiscal.Length >= 10))
            {
                contaContabilDepreciacao = notaFiscal.Substring(6, 4);
                if (contaContabilDepreciacao == "0000")
                {
                    contaContabilDepreciacao = null;
                }
                else
                {
                    if (contaContabilDepreciacao == "9999")
                        contaContabilDepreciacao = "99999" + contaContabilDepreciacao;
                    else
                        contaContabilDepreciacao = "12381" + contaContabilDepreciacao;
                }
            }


            return contaContabilDepreciacao;
        }

        private IList<AuditoriaIntegracao> geraDadosParaProcessamentoParaEstornoFechamentoMensalIntegrado(int codigoUGE, string anoMesReferencia)
        {
            IList<AccountingClosingExcluidos> linhasFechamentoMensalIntegradoParaEstorno = null;
            IList<AuditoriaIntegracao> linhasAuditoriaIntegracaoParaEstorno = null;
            AuditoriaIntegracao auditoriaIntegracaoFechamentoMensalIntegrado = null;
            AuditoriaIntegracao auditoriaIntegracaoParaEstorno = null;
            int auditoriaIntegracaoOriginalId = 0;
            string xmlParaWebserviceModoNovo = null;




            if (codigoUGE > 0 && !String.IsNullOrWhiteSpace(anoMesReferencia))
            {
                if (geraLinhasEstornoFechamentoMesReferenciaAnterior(codigoUGE, anoMesReferencia))
                {
                    linhasFechamentoMensalIntegradoParaEstorno = obtemRegistrosFechamentoMensalIntegradoParaEstorno(codigoUGE, anoMesReferencia);
                    if (linhasFechamentoMensalIntegradoParaEstorno.HasElements())
                    {
                        linhasAuditoriaIntegracaoParaEstorno = new List<AuditoriaIntegracao>();
                        foreach (var linhaFechamentoMensalIntegradoParaEstorno in linhasFechamentoMensalIntegradoParaEstorno)
                        {
                            auditoriaIntegracaoOriginalId = linhaFechamentoMensalIntegradoParaEstorno.AuditoriaIntegracaoId.GetValueOrDefault();
                            auditoriaIntegracaoFechamentoMensalIntegrado = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoOriginalId);

                            auditoriaIntegracaoParaEstorno = geraRegistroAuditoriaIntegracaoParaEstorno(auditoriaIntegracaoFechamentoMensalIntegrado);
                            xmlParaWebserviceModoNovo = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(auditoriaIntegracaoParaEstorno);

                            auditoriaIntegracaoParaEstorno.MsgEstimuloWS = xmlParaWebserviceModoNovo;
                            insereRegistroAuditoriaParcialNaBaseDeDados(auditoriaIntegracaoParaEstorno);

                            linhaFechamentoMensalIntegradoParaEstorno.AuditoriaIntegracaoIdEstorno = auditoriaIntegracaoParaEstorno.Id;
                            atualizaAccountingClosingExcluidos(linhaFechamentoMensalIntegradoParaEstorno);


                            linhasAuditoriaIntegracaoParaEstorno.Add(auditoriaIntegracaoParaEstorno);
                        }
                    }
                }
            }


            return linhasAuditoriaIntegracaoParaEstorno;
        }

        private IList<AccountingClosingExcluidos> obtemRegistrosFechamentoMensalIntegradoParaEstorno(int codigoUGE, string anoMesReferencia)
        {
            IList<AccountingClosingExcluidos> linhasFechamentoMensalIntegradoParaEstorno = null;
            //using (var contextoCamadaDados = new SAMContext())
            //{
            linhasFechamentoMensalIntegradoParaEstorno = db.AccountingClosingExcluidos
                                                            .Where(fechamentoMensalIntegradoParaEstorno => fechamentoMensalIntegradoParaEstorno.ReferenceMonth == anoMesReferencia
                                                                                                        && fechamentoMensalIntegradoParaEstorno.ManagerUnitCode == codigoUGE

                                                            && fechamentoMensalIntegradoParaEstorno.NLEstorno == null
                                                            && fechamentoMensalIntegradoParaEstorno.DepreciationMonth > 0
                                                            && !(fechamentoMensalIntegradoParaEstorno.DepreciationAccount != null && fechamentoMensalIntegradoParaEstorno.ClosingId != null))
                                                            .AsNoTracking().ToList();
            //}

            return linhasFechamentoMensalIntegradoParaEstorno;
        }

        public bool geraLinhasEstornoFechamentoMesReferenciaAnterior(int codigoUGE, string mesReferenciaASerReaberto)
        {
            bool spExecutadaComSucesso = false;
            string spName = "PREENCHE_TABELA_DE_NL_ESTORNADAS";
            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "ManagerUnitCode", valor = codigoUGE });
            listaParam.Add(new ListaParamentros { nomeParametro = "MesRef", valor = mesReferenciaASerReaberto });

            FunctionsCommon fnCommon = new FunctionsCommon();
            int numeroLinhasGeradasParaEstorno = fnCommon.ExecuteNonQueryStoredProcedure(listaParam, spName);


            spExecutadaComSucesso = (numeroLinhasGeradasParaEstorno > 0);
            return spExecutadaComSucesso;
        }

        #endregion

        private AuditoriaIntegracao geraRegistroAuditoriaIntegracaoParaEstorno(AuditoriaIntegracao objetoEntidadeASerClonadoParaEstorno)
        {
            AuditoriaIntegracao auditoriaIntegracaoClonada = null;
            string chavePesquisaSIAFMonitoraOriginal = null;
            string chavePesquisaSIAFMonitoraInversa = null;


            auditoriaIntegracaoClonada = new AuditoriaIntegracao();
            auditoriaIntegracaoClonada.NomeSistema = objetoEntidadeASerClonadoParaEstorno.NomeSistema;
            auditoriaIntegracaoClonada.ManagerUnitId = objetoEntidadeASerClonadoParaEstorno.ManagerUnitId;
            auditoriaIntegracaoClonada.chaveSIAFMonitora = objetoEntidadeASerClonadoParaEstorno.chaveSIAFMonitora;
            auditoriaIntegracaoClonada.TokenAuditoriaIntegracao = objetoEntidadeASerClonadoParaEstorno.TokenAuditoriaIntegracao;


            chavePesquisaSIAFMonitoraOriginal = objetoEntidadeASerClonadoParaEstorno.DocumentoId.Substring(0, 3);
            chavePesquisaSIAFMonitoraInversa = chavePesquisaSIAFMonitoraOriginal.Replace("N", "E");
            chavePesquisaSIAFMonitoraInversa = objetoEntidadeASerClonadoParaEstorno.DocumentoId.Replace(chavePesquisaSIAFMonitoraOriginal, chavePesquisaSIAFMonitoraInversa);
            /* Campos do XML Envio */
            auditoriaIntegracaoClonada.DocumentoId = chavePesquisaSIAFMonitoraInversa;
            auditoriaIntegracaoClonada.TipoMovimento = objetoEntidadeASerClonadoParaEstorno.TipoMovimento;
            auditoriaIntegracaoClonada.Data = objetoEntidadeASerClonadoParaEstorno.Data;
            auditoriaIntegracaoClonada.UgeOrigem = objetoEntidadeASerClonadoParaEstorno.UgeOrigem;
            auditoriaIntegracaoClonada.Gestao = objetoEntidadeASerClonadoParaEstorno.Gestao;
            auditoriaIntegracaoClonada.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = objetoEntidadeASerClonadoParaEstorno.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
            auditoriaIntegracaoClonada.CpfCnpjUgeFavorecida = objetoEntidadeASerClonadoParaEstorno.CpfCnpjUgeFavorecida;
            auditoriaIntegracaoClonada.GestaoFavorecida = objetoEntidadeASerClonadoParaEstorno.GestaoFavorecida;
            auditoriaIntegracaoClonada.Item = objetoEntidadeASerClonadoParaEstorno.Item;
            auditoriaIntegracaoClonada.TipoEstoque = objetoEntidadeASerClonadoParaEstorno.TipoEstoque;
            auditoriaIntegracaoClonada.Estoque = objetoEntidadeASerClonadoParaEstorno.Estoque;
            auditoriaIntegracaoClonada.EstoqueDestino = objetoEntidadeASerClonadoParaEstorno.EstoqueDestino;
            auditoriaIntegracaoClonada.EstoqueOrigem = objetoEntidadeASerClonadoParaEstorno.EstoqueOrigem;
            auditoriaIntegracaoClonada.TipoMovimentacao = objetoEntidadeASerClonadoParaEstorno.TipoMovimentacao;
            auditoriaIntegracaoClonada.ValorTotal = objetoEntidadeASerClonadoParaEstorno.ValorTotal;
            auditoriaIntegracaoClonada.ControleEspecifico = objetoEntidadeASerClonadoParaEstorno.ControleEspecifico;
            auditoriaIntegracaoClonada.ControleEspecificoEntrada = objetoEntidadeASerClonadoParaEstorno.ControleEspecificoEntrada;
            auditoriaIntegracaoClonada.ControleEspecificoSaida = objetoEntidadeASerClonadoParaEstorno.ControleEspecificoSaida;
            auditoriaIntegracaoClonada.FonteRecurso = objetoEntidadeASerClonadoParaEstorno.FonteRecurso;
            auditoriaIntegracaoClonada.NLEstorno = "S";
            auditoriaIntegracaoClonada.Empenho = objetoEntidadeASerClonadoParaEstorno.Empenho;
            auditoriaIntegracaoClonada.Observacao = objetoEntidadeASerClonadoParaEstorno.Observacao;
            auditoriaIntegracaoClonada.NotaFiscal = objetoEntidadeASerClonadoParaEstorno.NotaFiscal;
            auditoriaIntegracaoClonada.ItemMaterial = objetoEntidadeASerClonadoParaEstorno.ItemMaterial;

            auditoriaIntegracaoClonada.DataEnvio = DateTime.Now;
            return auditoriaIntegracaoClonada;
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}