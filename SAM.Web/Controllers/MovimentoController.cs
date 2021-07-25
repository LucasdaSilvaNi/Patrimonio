using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LinqKit;
using PagedList;
using PatrimonioBusiness.excel;
using PatrimonioBusiness.excel.abstrcts;
using PatrimonioBusiness.visaogeral;
using PatrimonioBusiness.visaogeral.abstracts;
using PatrimonioBusiness.visaogeral.entidades;
using PatrimonioBusiness.visaogeral.interfaces;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Context;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using SAM.Web.Legado;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using Newtonsoft.Json;
using System.Globalization;


namespace SAM.Web.Controllers
{
    public class MovimentoController : BaseController
    {

        private Hierarquia hierarquia;
        private MovimentoContext db;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;
        private string _login;
        private int _perfil;
        private string caminhoDosArquivosExcel;

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
                _perfil = perflLogado.RelatedRelationshipUserProfile.ProfileId;
            }
            else {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
                _login = (string)HttpContext.Items["CPF"];
                _perfil = (int)HttpContext.Items["perfilId"];
            }
        }

        public ActionResult Index(MovimentoIndexViewModel viewModel)
        {
            try
            {
                viewModel.perfilOperador = ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE ||
                                            (int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUO ||
                                            PerfilAdmGeral());

                ViewBag.AlertComSucesso = "";

                if (TempData["msgSucesso"] != null)
                {
                    ViewBag.AlertComSucesso = (string)TempData["msgSucesso"];
                    viewModel.cbFiltros = "22";
                    viewModel.searchString = (string)TempData["numeroDocumento"];
                }

                CarregaComboStatus(viewModel.cbStatus);
                CarregaComboFiltros(viewModel.cbFiltros);

                viewModel.perfilOperadorUGE = ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult()
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string cbFiltros = Request.Form["cbFiltros"].ToString();
            string cbStatus = Request.Form["cbStatus"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int[] IdsHieraquia;
            _perfil = 0;

            int totalRegistros = 0;
            IList<VisaoGeral> result = null;

            byte? parameterStatus = null;
            //int? parameterInstituionId = null;
            try
            {
                try
                {
                    _perfil = (int)HttpContext.Items["perfilId"];

                    if (hierarquiaLogin.Contains(','))
                    {
                        IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                        _login = (string)HttpContext.Items["CPF"];
                    }
                    else
                    {
                        getHierarquiaPerfil();
                        IdsHieraquia = new int[4] { _institutionId,
                                            _budgetUnitId == null ? 0 : (int)_budgetUnitId,
                                            _managerUnitId == null ? 0 : (int)_managerUnitId,
                                            _administrativeUnitId == null ? 0 : (int)_administrativeUnitId};
                    }
                }
                catch (Exception e) {
                    getHierarquiaPerfil();
                    IdsHieraquia = new int[4] { _institutionId,
                                            _budgetUnitId == null ? 0 : (int)_budgetUnitId,
                                            _managerUnitId == null ? 0 : (int)_managerUnitId,
                                            _administrativeUnitId == null ? 0 : (int)_administrativeUnitId};
                }

                if (cbStatus != null)
                {
                    if (Enum.IsDefined(typeof(EnumTelaVisaoGeral.FiltroVisaoGeral), int.Parse(cbStatus)))
                    {
                        parameterStatus = byte.Parse(cbStatus);
                    }
                }

                //if (cbStatus == null || int.Parse(cbStatus) == (int)EnumTelaVisaoGeral.FiltroVisaoGeral.Ativo)
                //    parameterStatus = 'A';
                //else if (cbStatus == null || int.Parse(cbStatus) == (int)EnumTelaVisaoGeral.FiltroVisaoGeral.Baixados)
                //    parameterStatus = 'I';


                if (string.IsNullOrWhiteSpace(currentFilter))
                    currentFilter = null;
                //if (IdsHieraquia[0] > 0)
                //    parameterInstituionId = IdsHieraquia[0];
                if (string.IsNullOrWhiteSpace(cbFiltros))
                    cbFiltros = null;

                
                #region Pesquisa
                if (PerfilAdmGeral() == true)
                {

                    VisaoGeralAbstract visaoGeral = VisaoGeralFactory.GetInstancia().CreateVisaoGeral(System.Data.IsolationLevel.ReadUncommitted);
                    IParametro parametro = Parametro.GetInstancia();
                    parametro.OrderDirecao = orderDir;
                    parametro.CampoOrder = order;

                    parametro.InstitutionId = null;
                    parametro.BudgetUnitId = null;
                    parametro.ManagerUnitId = null;
                    parametro.AdministrativeUnitId = null;

                    parametro.Estado = parameterStatus;
                    parametro.Campo = cbFiltros;
                    parametro.Filtro = currentFilter;
                    parametro.Size = length;
                    parametro.PageNumber = startRec;

                    var resultado = await visaoGeral.Gets(parametro);
                    if (cbStatus != "0")
                        foreach (var item in resultado)
                        {
                            if (item.MovementTypeId == 11 || item.MovementTypeId == 12 || item.MovementTypeId == 47 || item.MovementTypeId == 56 || item.MovementTypeId == 57)
                                item.EntreOsTiposDeMovimento = true;

                            if (item.IdAReclassificar != null && item.IdAReclassificar != 0)
                                item.AReclassificar = true;
                        }

                    result = VisaoGeral.Ordenar(order, orderDir, resultado);
                    totalRegistros = visaoGeral.TotalRegistros;
                    visaoGeral = null;


                    TempData["parametro"] = "";
                    TempData["parametro"] = parametro;
                }
                else
                {
                    VisaoGeralAbstract visaoGeral = VisaoGeralFactory.GetInstancia().CreateVisaoGeral(System.Data.IsolationLevel.ReadUncommitted);
                    IParametro parametro = Parametro.GetInstancia();
                    parametro.OrderDirecao = orderDir;
                    parametro.CampoOrder = order;
                    parametro.InstitutionId = IdsHieraquia[0];

                    if (IdsHieraquia[1] > 0)
                        parametro.BudgetUnitId = IdsHieraquia[1];
                    if (IdsHieraquia[2] > 0)
                        parametro.ManagerUnitId = IdsHieraquia[2];
                    if (IdsHieraquia[3] > 0)
                        parametro.AdministrativeUnitId = IdsHieraquia[3];

                    if (_perfil == (int)EnumProfile.Responsavel) {
                        parametro.CPF = _login;
                    }

                    parametro.Estado = parameterStatus;
                    parametro.Campo = cbFiltros;
                    parametro.Filtro = currentFilter;
                    parametro.Size = length;
                    parametro.PageNumber = startRec;

                    var resultado = await visaoGeral.Gets(parametro);
                    if (cbStatus != "0")
                        foreach (var item in resultado)
                        {
                            if (item.MovementTypeId == 11 || item.MovementTypeId == 12 || item.MovementTypeId == 47 || item.MovementTypeId == 56 || item.MovementTypeId == 57)
                                item.EntreOsTiposDeMovimento = true;

                            if (item.IdAReclassificar != null && item.IdAReclassificar != 0)
                                item.AReclassificar = true;
                        }

                    result = VisaoGeral.Ordenar(order, orderDir, resultado);
                    totalRegistros = visaoGeral.TotalRegistros;
                    visaoGeral = null;


                    TempData["parametro"] = "";
                    TempData["parametro"] = parametro;
                }
                #endregion

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> NumerosNotificacao(string parametros)
        {

            try
            {
                int[] IdsHieraquia;

                if (parametros.Contains(','))
                {
                    IdsHieraquia = Array.ConvertAll<string, int>(parametros.Split(','), int.Parse);
                }
                else
                {
                    getHierarquiaPerfil();
                    IdsHieraquia = new int[4] { _institutionId,
                                            _budgetUnitId == null ? 0 : (int)_budgetUnitId,
                                            _managerUnitId == null ? 0 : (int)_managerUnitId,
                                            _administrativeUnitId == null ? 0 : (int)_administrativeUnitId};
                }

                int[] resultado;
                if (IdsHieraquia[2] != 0)
                {
                    VisaoGeralAbstract visaoGeral = VisaoGeralFactory.GetInstancia().CreateVisaoGeral(System.Data.IsolationLevel.ReadUncommitted);
                    IParametro parametro = Parametro.GetInstancia();

                    parametro.InstitutionId = IdsHieraquia[0];
                    parametro.BudgetUnitId = IdsHieraquia[1];
                    parametro.ManagerUnitId = IdsHieraquia[2];

                    resultado = await visaoGeral.GetsNumeroNotification(parametro);
                }
                else
                {
                    resultado = new int[3];
                }

                return Json(new { qtdItensBolsaOrgao = resultado[0], qtdItensBolsaEstadual = resultado[1], qtdItensPendentesIncorporacao = resultado[2] }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        private void createCaminhoDosArquivosExcel(String nome)
        {

            caminhoDosArquivosExcel = HttpContext.Server.MapPath(Request.Path.Replace("Movimento", "ArquivoVisaoGeral"));
            if (!Directory.Exists(caminhoDosArquivosExcel))
            {
                Directory.CreateDirectory(caminhoDosArquivosExcel);
            }

        }
        private string GetCaminhoVirtualParaDownload(string nomeArquivo)
        {
            string caminhoRelativo = Request.Path.Replace("Movimento", "ArquivoVisaoGeral") + "/" + nomeArquivo;
            return caminhoRelativo;
        }
        public async Task<JsonResult> ExportarExcel()
        {
            string mensagem = "";
            try
            {
                string nomeArquivo = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "VisaoGeral_", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Millisecond.ToString(), ".xlsx");
                createCaminhoDosArquivosExcel(nomeArquivo);
                Server.ScriptTimeout = 600;

                ExportarAbstract exportarAbstract = ExcelFactory.CreateVisaoGeral();
                IParametro parametro = (IParametro)TempData["parametro"];

                VisaoGeralAbstract visaoGeral = VisaoGeralFactory.GetInstancia().CreateVisaoGeralAdo(System.Data.IsolationLevel.ReadUncommitted);
                parametro.Size = null;
                parametro.PageNumber = null;
                //var visaoGerais = await visaoGeral.Gets(parametro);
                var dstExcel = await visaoGeral.GetDataTable(parametro);

                var mensagensDeRetorno = new List<RetornoImportacao>();
                mensagensDeRetorno.Add(new RetornoImportacao { caminhoDoArquivoDownload = GetCaminhoVirtualParaDownload(nomeArquivo), mensagemImportacao = "Planinha:" + nomeArquivo, quantidadeDeRegistros = dstExcel.Rows.Count });


                IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();
                resultado.Add("Visao Geral", mensagensDeRetorno);

                mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);


                exportarAbstract.ExportExcel(dstExcel, caminhoDosArquivosExcel + "\\" + nomeArquivo);
                TempData["parametro"] = parametro;
            }
            catch (Exception ex)
            {
                var mensagensDeRetorno = new List<RetornoImportacao>();
                mensagensDeRetorno.Add(new RetornoImportacao
                {
                    caminhoDoArquivoDownload = "",
                    mensagemImportacao = ex.Message,
                    quantidadeDeRegistros = 0
                });
            }

            return Json(mensagem, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ProcessaTipoAcaoInventarioMovimentacaoInterna(string siglaChapaInventario, string numeroChapaInventario, int codigoUA, int tipoAcaoInventario)
        //{
        //    MovimentoViewModel _movimentacaoBP = null;
        //    //AssetViewModel _asset = null;
        //    SelectList comboUOs = null;
        //    SelectList comboUAs = null;
        //    SelectList comboUGEs = null;
        //    hierarquia = new Hierarquia();

        //    try
        //    {
        //        getHierarquiaPerfil();
        //        //LoadSelectElements(null);

        //        if (numeroChapaInventario != null)
        //        {
        //            using (db = new MovimentoContext())
        //            {
        //                var registroSiglaBP = obterDadosBemPatrimonialParaInventario(siglaChapaInventario, numeroChapaInventario, codigoUA.ToString());
        //                int tipoMovimentoID = -1;
        //                int orgaoID = -1;
        //                int uoID = -1;
        //                int uoDestinoID = -1;
        //                int ugeID = -1;
        //                int ugeDestinoID = -1;
        //                int? uaID = -1;


        //                getHierarquiaPerfil();
        //                //CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
        //                CarregaHierarquiaDestino();
        //                //CarregaComboResponsavel();
        //                CarregaComboTipoMovimento();
        //                ////CarregaComboTipoDeDocumentoSaida();


        //                if (registroSiglaBP.IsNotNull())
        //                {
        //                    switch (tipoAcaoInventario)
        //                    {
        //                        case 2:
        //                            {
        //                                tipoMovimentoID = EnumMovimentType.MovimentacaoInterna.GetHashCode();
        //                                orgaoID = registroSiglaBP.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
        //                                uoID = registroSiglaBP.RelatedManagerUnit.BudgetUnitId;
        //                                ugeID = registroSiglaBP.ManagerUnitId;
        //                                ugeDestinoID = registroSiglaBP.ManagerUnitId;
        //                                uaID = (registroSiglaBP.AssetMovements.LastOrDefault().IsNotNull() ? registroSiglaBP.AssetMovements.LastOrDefault().AdministrativeUnitId : 0);


        //                                comboUOs = new SelectList(hierarquia.GetUos(uoID), "Id", "Description", uoID);
        //                                comboUGEs = new SelectList(hierarquia.GetUges(ugeID), "Id", "Description", ugeID);
        //                                comboUAs = new SelectList(hierarquia.GetUasPorUgeId(ugeID), "Id", "Description", uaID);
        //                                CarregaComboResponsavel(uaID.Value);
        //                                ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(uaID), "Id", "Description");
        //                            }
        //                            break;

        //                        case 4:
        //                            {
        //                                tipoMovimentoID = EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado.GetHashCode();
        //                                orgaoID = registroSiglaBP.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
        //                                uoID = registroSiglaBP.RelatedManagerUnit.BudgetUnitId;
        //                                uoDestinoID = registroSiglaBP.RelatedManagerUnit.BudgetUnitId;
        //                                ugeID = registroSiglaBP.ManagerUnitId;
        //                                ugeDestinoID = registroSiglaBP.ManagerUnitId;
        //                                uaID = registroSiglaBP.AssetMovements.Last().AdministrativeUnitId;
        //                                comboUOs = new SelectList(hierarquia.GetUosPorOrgaoId(orgaoID), "Id", "Description");
        //                                comboUGEs = new SelectList(hierarquia.GetUgesPorUoId(uoID), "Id", "Description");
        //                                //comboUAs = new SelectList(hierarquia.GetUas(uaID), "Id", "Description", uaID);
        //                                //numeroChapaInventario = null;
        //                            }
        //                            break;
        //                    };


        //                    var dadosMovimentacaoPorInventario = String.Format("MOVIMENTACAO BEM PATRIMONAL VIA TELA DE INVENTARIO.\nSIGLA:{0}, CHAPA:{1}\n\nORIGEM:\nUO:{2}\nUGE:{3}\nUA:{4}\n\nDESTINO:\nUO:{5}\nUGE:{6}\nUA:{7}", registroSiglaBP.InitialName
        //                                                                                                                                                                                , registroSiglaBP.NumberIdentification
        //                                                                                                                                                                                , registroSiglaBP.RelatedManagerUnit.RelatedBudgetUnit.Code
        //                                                                                                                                                                                , registroSiglaBP.RelatedManagerUnit.Code
        //                                                                                                                                                                                , registroSiglaBP.AssetMovements.LastOrDefault().RelatedAdministrativeUnit.Code
        //                                                                                                                                                                                , (hierarquia.GetUos(this._budgetUnitId).FirstOrDefault().IsNotNull() ? hierarquia.GetUos(this._budgetUnitId).FirstOrDefault().Code : "")
        //                                                                                                                                                                                , (hierarquia.GetUges(this._managerUnitId).FirstOrDefault().IsNotNull() ? hierarquia.GetUges(this._managerUnitId).FirstOrDefault().Code : "")
        //                                                                                                                                                                                , codigoUA);

        //                    _movimentacaoBP = new MovimentoViewModel()
        //                    {
        //                        InstituationId = orgaoID,
        //                        InstituationIdDestino = orgaoID,
        //                        BudgetUnitId = uoID,
        //                        BudgetUnitIdDestino = uoDestinoID,
        //                        ManagerUnitId = ugeID,
        //                        ManagerUnitIdDestino = ugeDestinoID,
        //                        AdministrativeUnitId = uaID.GetValueOrDefault(),
        //                        AdministrativeUnitIdDestino = uaID.Value,
        //                        AssetId = registroSiglaBP.Id,
        //                        MovementTypeId = tipoMovimentoID,
        //                        SectionIdDestino = 0,
        //                        ResponsibleId = 0,
        //                        NumberProcess = "",
        //                        MovimentoDate = RecuperaAnoMesReferenciaFechamento(ugeDestinoID).RetornarMesAnoReferenciaDateTime().ToString("dd/MM/yyyy"),
        //                        Observation = dadosMovimentacaoPorInventario,
        //                        RepairValue = "",
        //                        SearchString = numeroChapaInventario,
        //                        CPFCNPJ = "",
        //                        ListAssetsParaMovimento = String.Format("[{0}]", registroSiglaBP.Id),
        //                        listaAssetEAssetViewModel = new StaticPagedList<AssetEAssetMovimentViewModel>((new List<AssetEAssetMovimentViewModel>() { new AssetEAssetMovimentViewModel() { Asset = registroSiglaBP, AssetMoviment = registroSiglaBP.AssetMovements.LastOrDefault() } }).ToList(), 1, 1, 1),
        //                        LoginSiafem = "",
        //                        SenhaSiafem = ""
        //                    };



        //                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(orgaoID), "Id", "Description", orgaoID);
        //                    ViewBag.BudgetUnitsDestino = comboUOs;
        //                    ViewBag.ManagerUnitsDestino = comboUGEs;
        //                    ViewBag.AdministrativeUnitsDestino = comboUAs;

        //                    if (tipoAcaoInventario == 2)
        //                    {
        //                        CarregaHierarquiaFiltro(_institutionId, uoID, ugeID, uaID.Value);
        //                        //CarregaPatrimonios(orgaoID, uoID, ugeID, uaID, null, numeroChapaInventario, null, null, tipoMovimentoID, true);
        //                        CarregaPatrimonios(orgaoID, uoID, ugeID, uaID, null, numeroChapaInventario, null, null, tipoMovimentoID, true);
        //                    }
        //                    //FALTA CARREGAR O BP PARA TRANSFERENCIA NA LISTA DE BPS PESQUISADOS E NA LISTA 'PARA MOVIMENTO'
        //                    else if (tipoAcaoInventario == 4)
        //                    {
        //                        //CarregaPatrimonios(orgaoID, uoDestinoID, ugeDestinoID, null, null, numeroChapaInventario, String.Format("[{0}]", registroSiglaBP.Id), null, tipoMovimentoID, true);
        //                        CarregaHierarquiaFiltro(_institutionId, uoDestinoID, ugeDestinoID);
        //                        //CarregaPatrimonios(orgaoID, uoID, ugeID, uaID, null, numeroChapaInventario, null, null, tipoMovimentoID, true);
        //                        //CarregaPatrimoniosParaMovimento(orgaoID, uoID, ugeID, uaID, null, null, String.Format("[{0}]", registroSiglaBP.Id));
        //                    }
        //                }

        //                //return View(_movimentacaoBP);
        //                ViewBag.MovimentacaoInterna = true;
        //                return View("Create", _movimentacaoBP);
        //            }
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        return MensagemErro(CommonMensagens.PadraoException, ex);
        //    }
        //}
        //public ActionResult ProcessaTipoAcaoInventario(string siglaChapaInventario, string numeroChapaInventario, int codigoUA, int tipoAcaoInventario)
        //{
        //    MovimentoViewModel _movimentacaoBP = null;
        //    SelectList comboUOs = null;
        //    SelectList comboUAs = null;
        //    SelectList comboUGEs = null;
        //    hierarquia = new Hierarquia();

        //    try
        //    {
        //        getHierarquiaPerfil();
        //        //LoadSelectElements(null);

        //        if (numeroChapaInventario != null)
        //        {
        //            var registroSiglaBP = obterDadosBemPatrimonialParaInventario(siglaChapaInventario, numeroChapaInventario, codigoUA.ToString());
        //            int tipoMovimentoID = -1;
        //            int orgaoID = -1;
        //            int uoID = -1;
        //            int uoDestinoID = -1;
        //            int ugeID = -1;
        //            int ugeDestinoID = -1;
        //            int? uaID = -1;


        //            getHierarquiaPerfil();
        //            //CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
        //            CarregaHierarquiaDestino();
        //            CarregaComboResponsavel();
        //            CarregaComboTipoMovimento();
        //            ////CarregaComboTipoDeDocumentoSaida();


        //            if (registroSiglaBP.IsNotNull())
        //            {
        //                switch (tipoAcaoInventario)
        //                {
        //                    case 2:
        //                        {
        //                            tipoMovimentoID = EnumMovimentType.MovimentacaoInterna.GetHashCode();
        //                            orgaoID = registroSiglaBP.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
        //                            uoID = registroSiglaBP.RelatedManagerUnit.BudgetUnitId;
        //                            ugeID = registroSiglaBP.ManagerUnitId;
        //                            ugeDestinoID = registroSiglaBP.ManagerUnitId;
        //                            uaID = (registroSiglaBP.AssetMovements.LastOrDefault().IsNotNull() ? registroSiglaBP.AssetMovements.LastOrDefault().AdministrativeUnitId : 0);

        //                            comboUOs = new SelectList(hierarquia.GetUos(uoID), "Id", "Description", uoID);
        //                            comboUGEs = new SelectList(hierarquia.GetUges(ugeID), "Id", "Description", ugeID);
        //                            comboUAs = new SelectList(hierarquia.GetUas(uaID), "Id", "Description", uaID);
        //                        }
        //                        break;

        //                    case 4:
        //                        {
        //                            tipoMovimentoID = EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado.GetHashCode();
        //                            orgaoID = registroSiglaBP.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
        //                            uoID = registroSiglaBP.RelatedManagerUnit.BudgetUnitId;
        //                            uoDestinoID = registroSiglaBP.RelatedManagerUnit.BudgetUnitId;
        //                            ugeID = registroSiglaBP.ManagerUnitId;
        //                            ugeDestinoID = registroSiglaBP.ManagerUnitId;
        //                            uaID = registroSiglaBP.AssetMovements.Last().AdministrativeUnitId;
        //                            comboUOs = new SelectList(hierarquia.GetUosPorOrgaoId(orgaoID), "Id", "Description");
        //                            comboUGEs = new SelectList(hierarquia.GetUgesPorUoId(uoID), "Id", "Description");
        //                            //comboUAs = new SelectList(hierarquia.GetUas(uaID), "Id", "Description", uaID);
        //                            //numeroChapaInventario = null;
        //                        }
        //                        break;
        //                };

        //                var dadosMovimentacaoPorInventario = String.Format("MOVIMENTACAO BEM PATRIMONAL VIA TELA DE INVENTARIO.\nSIGLA:{0}, CHAPA:{1}\n\nORIGEM:\nUO:{2}\nUGE:{3}\nUA:{4}\n\nDESTINO:\nUO:{5}\nUGE:{6}\nUA:{7}", registroSiglaBP.InitialName
        //                                                                                                                                                                                                    , registroSiglaBP.NumberIdentification
        //                                                                                                                                                                                                    , registroSiglaBP.RelatedManagerUnit.RelatedBudgetUnit.Code
        //                                                                                                                                                                                                    , registroSiglaBP.RelatedManagerUnit.Code
        //                                                                                                                                                                                                    , registroSiglaBP.AssetMovements.LastOrDefault().RelatedAdministrativeUnit.Code
        //                                                                                                                                                                                                    , (hierarquia.GetUos(this._budgetUnitId).FirstOrDefault().IsNotNull() ? hierarquia.GetUos(this._budgetUnitId).FirstOrDefault().Code : "")
        //                                                                                                                                                                                                    , (hierarquia.GetUges(this._managerUnitId).FirstOrDefault().IsNotNull() ? hierarquia.GetUges(this._managerUnitId).FirstOrDefault().Code : "")
        //                                                                                                                                                                                                    , codigoUA);
        //                _movimentacaoBP = new MovimentoViewModel()
        //                {
        //                    InstituationId = orgaoID,
        //                    InstituationIdDestino = orgaoID,
        //                    BudgetUnitId = uoID,
        //                    BudgetUnitIdDestino = uoDestinoID,
        //                    ManagerUnitId = ugeID,
        //                    ManagerUnitIdDestino = ugeDestinoID,
        //                    AdministrativeUnitId = uaID.GetValueOrDefault(),
        //                    AssetId = registroSiglaBP.Id,
        //                    MovementTypeId = tipoMovimentoID,
        //                    AdministrativeUnitIdDestino = 0,
        //                    SectionIdDestino = 0,
        //                    ResponsibleId = 0,
        //                    NumberProcess = "",
        //                    MovimentoDate = RecuperaAnoMesReferenciaFechamento(ugeDestinoID).RetornarMesAnoReferenciaDateTime().ToString("dd/MM/yyyy"),
        //                    Observation = dadosMovimentacaoPorInventario,
        //                    RepairValue = "",
        //                    SearchString = numeroChapaInventario,
        //                    CPFCNPJ = "",
        //                    ListAssetsParaMovimento = String.Format("[{0}]", registroSiglaBP.Id),
        //                    listaAssetEAssetViewModel = new StaticPagedList<AssetEAssetMovimentViewModel>((new List<AssetEAssetMovimentViewModel>() { new AssetEAssetMovimentViewModel() { Asset = registroSiglaBP, AssetMoviment = registroSiglaBP.AssetMovements.LastOrDefault() } }).ToList(), 1, 1, 1),
        //                    LoginSiafem = "",
        //                    SenhaSiafem = ""
        //                };



        //                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(orgaoID), "Id", "Description", orgaoID);
        //                ViewBag.BudgetUnitsDestino = comboUOs;
        //                ViewBag.ManagerUnitsDestino = comboUGEs;
        //                ViewBag.AdministrativeUnitsDestino = comboUAs;

        //                if (tipoAcaoInventario == 2)
        //                {
        //                    CarregaHierarquiaFiltro(_institutionId, uoID, ugeID);
        //                    //CarregaPatrimonios(orgaoID, uoID, ugeID, uaID, null, numeroChapaInventario, null, null, tipoMovimentoID, true);
        //                    CarregaPatrimonios(orgaoID, uoID, ugeID, uaID, null, numeroChapaInventario, null, null, tipoMovimentoID, true);
        //                }
        //                //FALTA CARREGAR O BP PARA TRANSFERENCIA NA LISTA DE BPS PESQUISADOS E NA LISTA 'PARA MOVIMENTO'
        //                else if (tipoAcaoInventario == 4)
        //                {
        //                    //CarregaPatrimonios(orgaoID, uoDestinoID, ugeDestinoID, null, null, numeroChapaInventario, String.Format("[{0}]", registroSiglaBP.Id), null, tipoMovimentoID, true);
        //                    CarregaHierarquiaFiltro(_institutionId, uoDestinoID, ugeDestinoID);
        //                    //CarregaPatrimonios(orgaoID, uoID, ugeID, uaID, null, numeroChapaInventario, null, null, tipoMovimentoID, true);
        //                    //CarregaPatrimoniosParaMovimento(orgaoID, uoID, ugeID, uaID, null, null, String.Format("[{0}]", registroSiglaBP.Id));
        //                }
        //            }

        //            //return View(_movimentacaoBP);
        //            ViewBag.TransferirInventario = true;
        //            return View("Create", _movimentacaoBP);
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        return MensagemErro(CommonMensagens.PadraoException, ex);
        //    }
        //}

        //private Asset obterDadosBemPatrimonialParaInventario(string siglaBP, string chapaBP, string codigoUA)
        //{
        //    Asset dadosBemPatrimonial = null;
        //    string _chapaBP = null;
        //    string _siglaBP = null;
        //    int _codigoUA = -1;
        //    Expression<Func<Asset, bool>> expWhereConsulta = null;


        //    _chapaBP = chapaBP;
        //    _siglaBP = siglaBP;
        //    if (Int32.TryParse(codigoUA, out _codigoUA))
        //    {
        //        var dadosUaConsulta = db.AdministrativeUnits.Where(uaSIAFEM => uaSIAFEM.Code == _codigoUA).FirstOrDefault();
        //        if (dadosUaConsulta.IsNotNull())
        //        {
        //            expWhereConsulta = (bemPatrimonial => bemPatrimonial.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code == dadosUaConsulta.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code
        //                                               && bemPatrimonial.InitialName == _siglaBP
        //                                               && bemPatrimonial.NumberIdentification == _chapaBP
        //                                               && bemPatrimonial.Status == true);

        //            dadosBemPatrimonial = db.Assets.Where(expWhereConsulta).FirstOrDefault();
        //        }
        //    }


        //    return dadosBemPatrimonial;
        //}

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            try
            {
                getHierarquiaPerfil();

                if (UGEEstaComPendenciaSIAFEMNoFechamento(_managerUnitId))
                    return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                CarregaHierarquiaDestino();

                using (db = new MovimentoContext())
                {
                    CarregaComboTipoMovimento();
                    CarregaComboResponsavel();
                    CarregaComboContaContabil();

                    var movimentoViewModel = new MovimentoViewModel();
                    movimentoViewModel.AssetId = id;
                    movimentoViewModel.UGESiafem = _managerUnitId == null ? string.Empty : db.ManagerUnits.Find((int)_managerUnitId).Code;
                    //movimentoViewModel.listaAssetEAssetViewModel = ListaHistoricoDeMovimentacoes(id);

                    return View(movimentoViewModel);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public ActionResult CarregaPartialViewTipoMovimento(int movimentTypeId, string hierarquia)
        {
            try
            {
                if (!string.IsNullOrEmpty(hierarquia) && !string.IsNullOrWhiteSpace(hierarquia)) {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquia.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _budgetUnitId = IdsHieraquia[1];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                    getHierarquiaPerfil();

                MovimentoViewModel movimento;

                switch (movimentTypeId)
                {
                    case (int)EnumMovimentType.MovimentacaoInterna:
                        CarregaHierarquiaDestinoComUAEDivisao(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        using (db = new MovimentoContext())
                        {
                            CarregaComboResponsavel();
                        }
                        return PartialView("Movimentacao/_10partialMovimentacaoInterna", new MovimentoViewModel());

                    case (int)EnumMovimentType.VoltaConserto:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_13partialVoltaConserto", new MovimentoViewModel());

                    case (int)EnumMovimentType.SaidaConserto:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_14partialSaidaConserto", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovInservivelNaUGE:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_19partialArrolamento", new MovimentoViewModel());

                    case (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);

                        return PartialView("Movimentacao/_20partialDisponibilizadoParaBolsaSecretaria", new MovimentoViewModel());
                    case (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_21partialDisponibilizadoParaBolsaEstadual", new MovimentoViewModel());

                    case (int)EnumMovimentType.RetiradaDaBolsa:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_22partialRetiradaDaBolsa", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                        movimento = new MovimentoViewModel();
                        using (db = new MovimentoContext())
                        {
                            if (!MesmaGestaoDoFundoSocial(_institutionId))
                                CarregaFundoSocial();
                            else
                                movimento.Proibido = true;
                        }
                        return PartialView("Movimentacao/_42partialSaidaUGEDoacao", movimento);

                    case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                        movimento = new MovimentoViewModel();
                        using (db = new MovimentoContext())
                        {
                            if (MesmaGestaoDoFundoSocial(_institutionId))
                                CarregaFundoSocial();
                            else
                                movimento.Proibido = true;
                        }
                        return PartialView("Movimentacao/_43partialSaidaUGETransferencia", movimento);

                    case (int)EnumMovimentType.MovComodatoConcedidoBensMoveis:
                        CarregaHierarquiaDestinoMesmaGestao();
                        return PartialView("Movimentacao/_44partialComodatoConcedidoBensMoveis", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_45partialComodatoTerceirosRecebidos", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovDoacaoConsolidacao:
                        return PartialView("Movimentacao/_46partialDoacaoConsolidacao", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                        CarregaHierarquiaDestinoOutrasGestoes();
                        return PartialView("Movimentacao/_47partialDoacaoIntraNoEstado", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovDoacaoMunicipio:
                        return PartialView("Movimentacao/_48partialDoacaoMunicipio", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovDoacaoOutrosEstados:
                        return PartialView("Movimentacao/_49partialDoacaoOutrosEstados", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovDoacaoUniao:
                        return PartialView("Movimentacao/_50partialDoacaoUniao", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_51partialExtravioFurtoRouboBensMoveis", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovMorteAnimalPatrimoniado:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_52partialMorteAnimalPatrimoniado", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_54partialMudancaCategoriaDesvalorizacao", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovSementesPlantasInsumosArvores:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_55partialSementesPlantasInsumosArvores", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                        CarregaHierarquiaDestinoMesmaGestao(podeMesmaGestao: false);
                        return PartialView("Movimentacao/_56partialTransferenciaOutroOrgaoPatrimoniado", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                        CarregaHierarquiaDestinoMesmoOrgao();
                        //CarregaComboTipoDeDocumentoSaida();
                        return PartialView("Movimentacao/_57partialTransferenciaMesmoOrgaoPatrimoniado", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_58partialPerdaInvoluntariaBensMoveis", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_59partialPerdaInvoluntariaInservivelBensMoveis", new MovimentoViewModel());

                    case (int)EnumMovimentType.MovVendaLeilaoSemoventes:
                        CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        return PartialView("Movimentacao/_95partialVendaLeilaoSemovente", new MovimentoViewModel());
                }

                return PartialView("_vazio");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAnotacaoMultiplosBotoes(Name = "action", Argument = "Create")]
        public ActionResult Create([Bind(Include = "MovementTypeId, InstituationId, ContaContabilId, BudgetUnitId, ManagerUnitId, InstituationIdDestino, BudgetUnitIdDestino, ManagerUnitIdDestino, AdministrativeUnitIdDestino,  SectionIdDestino, ResponsibleId, NumberProcess, MovimentoDate, Observation, RepairValue, SearchString, CPFCNPJ, ListAssetsParaMovimento, ListAssets, listaAssetEAssetViewModel, LoginSiafem, SenhaSiafem, UGESiafem, Proibido")] MovimentoViewModel movimentoViewModel)
        {
            IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote = null;

            try
            {
                getHierarquiaPerfil();

                var assetsSelected = movimentoViewModel.ListAssets != null ?
                                        Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(movimentoViewModel.ListAssets) :
                                        new List<int>();

                db = new MovimentoContext();
                //Valida se pelo menos um bem foi selecionado
                if (!assetsSelected.Any())
                {
                    ModelState.AddModelError("QtdBensSelecionados", "Por favor, selecione pelo menos um patrimônio para a movimentação");
                }
                else
                {
                    assetsSelected = assetsSelected.Distinct().ToList();

                    if (assetsSelected.Count > 0)
                    {
                        if (BPsSemPendenciaDeNL(assetsSelected, movimentoViewModel.ManagerUnitId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação não pode ser realizada pois possui BPs com pendência(s) de NL(s). Por favor, verifique a tela 'Notas de Lançamentos Pendentes SIAFEM'");
                        }

                        if (AlgumBPPrecisaRealizarReclassificacaoContabil(assetsSelected))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Alguns BPs não podem ser movimentados pois não estão com a conta contábil atualizado a novas normas. Por gentileza, atualize a conta contábil desses BPs na tela Reclassificação Contábil.");
                        }
                    }

                    if (assetsSelected.Count > 1)
                    {
                        if (Convert.ToInt32(EnumMovimentType.MovimentacaoInterna) != movimentoViewModel.MovementTypeId)
                        {
                            if (!MovimentacaoEmLoteComUnicaContaContabil(assetsSelected))
                            {
                                ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação em lote permitido somente com todos os BPs na mesma Conta Contábil");
                            }
                        }
                    }
                    if (assetsSelected.Count > 0)//trava para nao movimentar BP para si msm 
                    {
                    
                        if (Convert.ToInt32(EnumMovimentType.MovimentacaoInterna) == movimentoViewModel.MovementTypeId)
                        {
                            var resultbps = BPSIndiponiveisParaMovimentar(assetsSelected);
                            foreach (var item in resultbps)
                            {
                                if (movimentoViewModel.AdministrativeUnitIdDestino == item.AdministrativeUnitId)
                                {
                                    if (movimentoViewModel.ResponsibleId == item.ResponsibleId && movimentoViewModel.SectionIdDestino == (item.SectionId == null ? 0 : item.SectionId))
                                    {
                                        ModelState.AddModelError("MsgMovimentacaoEmLote", "Não permitida a movimentação de BPs de um responsável para ele mesmo.");
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    var mensagemErro = ValidaDataDeTranferencia(movimentoViewModel.MovimentoDate, assetsSelected, movimentoViewModel.ManagerUnitId);
                    if (mensagemErro != string.Empty)
                    {
                        ModelState.AddModelError("MovimentoDate", mensagemErro);
                    }
                    else
                    {
                        bool orgaoEhImplantado = (from i in db.Institutions
                                                  where i.Id == movimentoViewModel.InstituationIdDestino
                                                  select i.flagImplantado).FirstOrDefault();

                        if (orgaoEhImplantado == true)
                        {
                            //Valida se a data de transferencia segue as normais padrões
                            mensagemErro = ValidaMesRefUGEOrigemIgualAUGEDestino(movimentoViewModel);
                            if (mensagemErro != string.Empty)
                            {
                                ModelState.AddModelError("MovimentoDate", mensagemErro);
                            }
                        }
                    }
                }

                switch (movimentoViewModel.MovementTypeId) {
                    // Tarefa tira a obrigatoriedade da divisao - 14-09-2018
                    case (int)EnumMovimentType.MovimentacaoInterna:
                        ModelState.Remove("SectionIdDestino");
                        break;
                    case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                        if (movimentoViewModel.ManagerUnitId == movimentoViewModel.ManagerUnitIdDestino)
                        {
                            ModelState.AddModelError("ManagerUnitIdDestino", "Transferência(s) de BP(s) para mesma UGE serão realizadas via Movimentação Interna");
                        }
                        break;
                    //alterar para travar no groupcode 88
                    case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:
                    case (int)EnumMovimentType.MovVendaLeilaoSemoventes:
                        if (BPTravado(assetsSelected))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Somente BPs do grupo 88 são aceitos para essa movimentação"); // diferente de 88 nao movimenta, 88 movimenta
                        }
                        break;
                    case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                        if (!BPsSaoTerceiros(assetsSelected))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "BPs para este tipo de movimentação precisam ser do tipo Terceiro");
                        }
                        break;
                    case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                        if (!MesmaGestaoDoFundoSocial(movimentoViewModel.InstituationId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "UGE somente pode realizar saída de inservível como SAÍDA INSERVÍVEL DA UGE - DOAÇÃO");
                        }
                        else {
                            if (movimentoViewModel.ManagerUnitId == movimentoViewModel.ManagerUnitIdDestino)
                            {
                                ModelState.AddModelError("ManagerUnitIdDestino", "Saída de Inservível precisa que a UGE de destino seja diferente da origem");
                            }

                            if (!BPsNaContaContabilInservivel(assetsSelected)) {
                                ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação permitida somente com inservíveis (BPs na conta contábil 123110805)");
                            }
                        }
                        break;
                    case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                        if (MesmaGestaoDoFundoSocial(movimentoViewModel.InstituationId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "UGE somente pode realizar saída de inservível como SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA");
                        }
                        else {
                            if (!BPsNaContaContabilInservivel(assetsSelected))
                            {
                                ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação permitida somente com inservíveis (BPs na conta contábil 123110805)");
                            }
                        }
                        break;
                    case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                        if (!BPsNaContaContabilInservivel(assetsSelected))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação permitida somente para inservíveis (BPs na conta contábil 123110805)");
                        }
                        break;
                }

                ModelState.Remove("ContaContabilId");

                if (ModelState.IsValid)
                {
                    if (UGEEstaComPendenciaSIAFEMNoFechamento(movimentoViewModel.ManagerUnitId))
                        return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                    var tipoMovPatrimonial = (from m in db.MovementTypes
                                              where m.Id == movimentoViewModel.MovementTypeId
                                              select m).FirstOrDefault();

                    base.initDadosSIAFEM();

                    var dataInicioIntegracao = new DateTime(2020, 06, 01);
                    bool MovimentacaoIntegradoAoSIAFEM = tipoMovPatrimonial.PossuiContraPartidaContabil() && base.ugeIntegradaSiafem && Convert.ToDateTime(movimentoViewModel.MovimentoDate) >= dataInicioIntegracao;

                    //Numero de documento gerado para a AssetMovements, ele é unico para todos os itens selecionados.
                    string NumberDocGerado = "";
                    listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>();

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        foreach (var assetId in assetsSelected)
                        {
                            var assetEAssetMovimentBD = (from a in db.Assets
                                                         join am in db.AssetMovements
                                                         on a.Id equals am.AssetId
                                                         where am.Status == true &&
                                                               am.AssetId == assetId &&
                                                               !a.flagVerificado.HasValue &&
                                                               a.flagDepreciaAcumulada == 1
                                                         select new AssetEAssetMovimentViewModel
                                                         {
                                                             Asset = a,
                                                             AssetMoviment = am
                                                         }).FirstOrDefault();

                            //Desativa a movimentação anterior ---------------------------------------------------------------------------------------------------------------------------------------------
                            var oldsAssetMoviment = (from am in db.AssetMovements
                                                     where am.Status == true &&
                                                           am.AssetId == assetEAssetMovimentBD.Asset.Id
                                                     select am).ToList();

                            int? ContaContabilAntesContaInservivel = null;

                            foreach (var oldAssetMovimentDB in oldsAssetMoviment)
                            {
                                if (oldAssetMovimentDB.Status == true)
                                {
                                    ContaContabilAntesContaInservivel = oldAssetMovimentDB.AuxiliaryAccountId;
                                    oldAssetMovimentDB.Status = false;

                                    db.Entry(oldAssetMovimentDB).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                            //Entidade para ser salva no banco, com os campos comuns preenchidos
                            var assetMoviment = new AssetMovements();
                            assetMoviment.AssetId = assetId;
                            assetMoviment.MovimentDate = Convert.ToDateTime(movimentoViewModel.MovimentoDate);
                            assetMoviment.MovementTypeId = movimentoViewModel.MovementTypeId;
                            assetMoviment.StateConservationId = assetEAssetMovimentBD.AssetMoviment.StateConservationId;
                            assetMoviment.NumberPurchaseProcess = movimentoViewModel.NumberProcess;
                            assetMoviment.InstitutionId = _institutionId;
                            assetMoviment.BudgetUnitId = movimentoViewModel.BudgetUnitId;
                            assetMoviment.ManagerUnitId = movimentoViewModel.ManagerUnitId;
                            assetMoviment.AssetTransferenciaId = null;
                            assetMoviment.Observation = movimentoViewModel.Observation;
                            assetMoviment.Login = _login;
                            assetMoviment.DataLogin = DateTime.Now;

                            if (movimentoViewModel.CPFCNPJ != null && movimentoViewModel.CPFCNPJ != "")
                                assetMoviment.CPFCNPJ = movimentoViewModel.CPFCNPJ.RetirarCaracteresEspeciaisCpfCnpj();

                            // Gera Numero de Documento (ANO + COD_UGE + SEQUENCIA)
                            if (NumberDocGerado == "")
                            {
                                string Sequencia = BuscaUltimoNumeroDocumentoSaida(assetEAssetMovimentBD.AssetMoviment.ManagerUnitId);

                                //string Sequencia = (from am in db.AssetMovements where am.ManagerUnitId == assetEAssetMovimentBD.AssetMoviment.ManagerUnitId select am.NumberDoc).Max();

                                if (Sequencia.Equals("0"))
                                    assetMoviment.NumberDoc = DateTime.Now.Year + assetEAssetMovimentBD.AssetMoviment.RelatedManagerUnit.Code + "0001";
                                else
                                {
                                    if (Sequencia.Length > 14) {
                                        int contador = Convert.ToInt32(Sequencia.Substring(10, Sequencia.Length - 10));

                                        if (contador < 9999)
                                        {
                                            assetMoviment.NumberDoc = Sequencia.Substring(0, 10) + (contador + 1).ToString().PadLeft(4, '0');
                                        }
                                        else {
                                            assetMoviment.NumberDoc = (long.Parse(Sequencia) + 1).ToString();
                                        }
                                    }
                                    else
                                        assetMoviment.NumberDoc = (long.Parse(Sequencia) + 1).ToString();
                                }

                                NumberDocGerado = assetMoviment.NumberDoc;
                            }
                            else
                                assetMoviment.NumberDoc = NumberDocGerado;
                            // Fim

                            if ((from ram in db.RelationshipAuxiliaryAccountMovementTypes
                                 where ram.MovementTypeId == movimentoViewModel.MovementTypeId
                                 select ram
                                  ).Any())
                            {
                                assetMoviment.AuxiliaryAccountId =
                                    (from ram in db.RelationshipAuxiliaryAccountMovementTypes
                                     where ram.MovementTypeId == movimentoViewModel.MovementTypeId
                                     select ram.AuxiliaryAccountId
                                     ).FirstOrDefault();
                            }
                            else
                            {
                                assetMoviment.AuxiliaryAccountId = assetEAssetMovimentBD.AssetMoviment.AuxiliaryAccountId;
                            }


                            switch (movimentoViewModel.MovementTypeId)
                            {

                                #region MOVIMENTACAO INTERNA

                                case (int)EnumMovimentType.MovimentacaoInterna:

                                    assetMoviment.Status = true;
                                    assetMoviment.NumberPurchaseProcess = assetEAssetMovimentBD.AssetMoviment.NumberPurchaseProcess;
                                    assetMoviment.BudgetUnitId = _budgetUnitId ?? movimentoViewModel.BudgetUnitIdDestino; //Caso o perfil não seja a nível de UO, pegar a UO do destino
                                    assetMoviment.ManagerUnitId = _managerUnitId ?? movimentoViewModel.ManagerUnitIdDestino; //Caso o perfil não seja a nível de UGE, pegar a UGE do destino
                                    assetMoviment.AdministrativeUnitId = movimentoViewModel.AdministrativeUnitIdDestino;
                                    if (movimentoViewModel.SectionIdDestino == 0)
                                        assetMoviment.SectionId = null;
                                    else
                                        assetMoviment.SectionId = movimentoViewModel.SectionIdDestino;
                                    assetMoviment.ResponsibleId = movimentoViewModel.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region INSERVIVEL NA UGE

                                case (int)EnumMovimentType.MovInservivelNaUGE:

                                    //Insere o registro de arrolamento no movimento
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = ContaContabilAntesContaInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region DISPONIBILIZADO PARA BOLSA SECRETARIA

                                case (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria:

                                    //Insere o registro de disponibilização do item para a bolsa da secretária no movimento
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region DISPONIBILIZADO PARA BOLSA ESTADUAL

                                case (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual:

                                    //Insere o registro de disponibilização do item para a bolsa do estado no movimento
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region RETIRADA DA BOLSA

                                case (int)EnumMovimentType.RetiradaDaBolsa:

                                    //Remove o patrimônio da bolsa
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;



                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region SAÍDA INSERVIVEL DA UGE - TRANSFERÊNCIA

                                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = IdUGEFundoSocial();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetTransferidoDB = (from a in db.Assets
                                                              where a.Id == assetId

                                                              select a).FirstOrDefault();

                                    assetTransferidoDB.Status = false;
                                    db.Entry(assetTransferidoDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;

                                #endregion

                                #region SAÍDA INSERVIVEL DA UGE - DOAÇÃO

                                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = IdUGEFundoSocial();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoDB = (from a in db.Assets
                                                         where a.Id == assetId

                                                         select a).FirstOrDefault();

                                    assetDoacaoDB.Status = false;
                                    db.Entry(assetDoacaoDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;

                                #endregion

                                #region COMODATO - CONCEDIDOS - BENS MÓVEIS

                                case (int)EnumMovimentType.MovComodatoConcedidoBensMoveis:

                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region COMODATO/DE TERCEIROS - RECEBIDOS

                                case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetComodato = (from a in db.Assets
                                                         where a.Id == assetId
                                                         select a).FirstOrDefault();

                                    assetComodato.Status = false;
                                    db.Entry(assetComodato).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region DOAÇÃO CONSOLIDAÇÃO

                                case (int)EnumMovimentType.MovDoacaoConsolidacao:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.CPFCNPJ = movimentoViewModel.CPFCNPJ.RetirarCaracteresEspeciaisCpfCnpj();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoConsolidacao = (from a in db.Assets
                                                                   where a.Id == assetId
                                                                   select a).FirstOrDefault();

                                    assetDoacaoConsolidacao.Status = false;
                                    db.Entry(assetDoacaoConsolidacao).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                                #region DOAÇÃO INTRA - NO ESTADO

                                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:

                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;
                                #endregion

                                #region DOAÇÃO MUNICÍPIO

                                case (int)EnumMovimentType.MovDoacaoMunicipio:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.CPFCNPJ = movimentoViewModel.CPFCNPJ.RetirarCaracteresEspeciaisCpfCnpj();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoMunicipio = (from a in db.Assets
                                                                where a.Id == assetId
                                                                select a).FirstOrDefault();

                                    assetDoacaoMunicipio.Status = false;
                                    db.Entry(assetDoacaoMunicipio).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                                #region DOAÇÃO OUTROS ESTADOS

                                case (int)EnumMovimentType.MovDoacaoOutrosEstados:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.CPFCNPJ = movimentoViewModel.CPFCNPJ.RetirarCaracteresEspeciaisCpfCnpj();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoOutrosEstados = (from a in db.Assets
                                                                    where a.Id == assetId
                                                                    select a).FirstOrDefault();

                                    assetDoacaoOutrosEstados.Status = false;
                                    db.Entry(assetDoacaoOutrosEstados).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                                #region DOAÇÃO UNIÃO

                                case (int)EnumMovimentType.MovDoacaoUniao:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.CPFCNPJ = movimentoViewModel.CPFCNPJ.RetirarCaracteresEspeciaisCpfCnpj();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoUniao = (from a in db.Assets
                                                            where a.Id == assetId
                                                            select a).FirstOrDefault();

                                    assetDoacaoUniao.Status = false;
                                    db.Entry(assetDoacaoUniao).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                                #region VOLTA CONSERTO

                                case (int)EnumMovimentType.VoltaConserto:

                                    assetMoviment.Status = true;
                                    assetMoviment.NumberPurchaseProcess = assetEAssetMovimentBD.AssetMoviment.NumberPurchaseProcess;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.RepairValue = !string.IsNullOrEmpty(movimentoViewModel.RepairValue) ? (decimal?)decimal.Parse(movimentoViewModel.RepairValue) : null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;
                                #endregion

                                #region SAIDA CONSERTO

                                case (int)EnumMovimentType.SaidaConserto:

                                    assetMoviment.Status = true;
                                    assetMoviment.NumberPurchaseProcess = assetEAssetMovimentBD.AssetMoviment.NumberPurchaseProcess;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.RepairValue = !string.IsNullOrEmpty(movimentoViewModel.RepairValue) ? (decimal?)decimal.Parse(movimentoViewModel.RepairValue) : null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region EXTRAVIO, FURTO, ROUBO - BENS MOVEIS

                                case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:

                                    //Inativa o bem
                                    var assetExtraviadoDB = (from a in db.Assets
                                                             where a.Id == assetId

                                                             select a).FirstOrDefault();

                                    assetExtraviadoDB.Status = false;
                                    db.Entry(assetExtraviadoDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Insere o registro de extravio no movimento
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region PERDA INVOLUNTÁRIA - BENS MOVEIS

                                case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:

                                    //Inativa o bem
                                    var assetObsoletoDB = (from a in db.Assets
                                                           where a.Id == assetId

                                                           select a).FirstOrDefault();

                                    assetObsoletoDB.Status = false;
                                    db.Entry(assetObsoletoDB).State = EntityState.Modified;
                                    db.SaveChanges();


                                    //Insere o registro de obsoleto no movimento
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region PERDA INVOLUNTÁRIA INSERVÍVEL - BENS MOVEIS

                                case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:

                                    //Inativa o bem
                                    var assetDanificadoDB = (from a in db.Assets
                                                             where a.Id == assetId

                                                             select a).FirstOrDefault();

                                    assetDanificadoDB.Status = false;
                                    db.Entry(assetDanificadoDB).State = EntityState.Modified;
                                    db.SaveChanges();


                                    //Insere o registro de Danificado no movimento
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region MORTE ANIMAL - PATRIMONIADO

                                case (int)EnumMovimentType.MovMorteAnimalPatrimoniado:

                                    //Inativa o bem
                                    var assetMorteAnimal = (from a in db.Assets
                                                            where a.Id == assetId

                                                            select a).FirstOrDefault();

                                    assetMorteAnimal.Status = false;
                                    db.Entry(assetMorteAnimal).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Insere o registro de obsoleto no sucata
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                                #region MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO

                                case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:

                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;
                                #endregion

                                #region SEMENTES, PLANTAS, INSUMOS E ARVORES

                                case (int)EnumMovimentType.MovSementesPlantasInsumosArvores:

                                    var assetMovSementesDB = (from a in db.Assets
                                                              where a.Id == assetId
                                                              select a).FirstOrDefault();

                                    assetMovSementesDB.Status = false;
                                    db.Entry(assetMovSementesDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;
                                #endregion

                                #region TRANSFERÊNCIA OUTRO ORGÃO - PATRIMÔNIADO

                                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:

                                    bool? orgaoDestinoTransferenciaImplantadoOutroOrgao = (from m in db.Institutions
                                                                                           where m.Id == movimentoViewModel.InstituationIdDestino
                                                                                           select m.flagImplantado).FirstOrDefault();

                                    if (orgaoDestinoTransferenciaImplantadoOutroOrgao == true)
                                    {
                                        assetMoviment.Status = true;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        assetMoviment.Status = false;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;
                                        assetMoviment.flagUGENaoUtilizada = true;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();

                                        var assetTransferidoDB3 = (from a in db.Assets
                                                                   where a.Id == assetId

                                                                   select a).FirstOrDefault();

                                        assetTransferidoDB3.Status = false;
                                        db.Entry(assetTransferidoDB3).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;

                                #endregion

                                #region TRANSFERÊNCIA MESMO ORGÃO - PATRIMÔNIADO

                                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:

                                    bool? orgaoDestinoTransferenciaImplantadoMesmoOrgao = (from m in db.Institutions
                                                                                           where m.Id == movimentoViewModel.InstituationIdDestino
                                                                                           select m.flagImplantado).FirstOrDefault();

                                    if (orgaoDestinoTransferenciaImplantadoMesmoOrgao == true)
                                    {
                                        assetMoviment.Status = true;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        assetMoviment.Status = false;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;
                                        assetMoviment.flagUGENaoUtilizada = true;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();

                                        var assetTransferidoDB2 = (from a in db.Assets
                                                                   where a.Id == assetId

                                                                   select a).FirstOrDefault();

                                        assetTransferidoDB2.Status = false;
                                        db.Entry(assetTransferidoDB2).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    verificaImplantacaoUGEAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;

                                #endregion

                                #region VENDA/LEILÃO - SEMOVENTES

                                case (int)EnumMovimentType.MovVendaLeilaoSemoventes:

                                    //Inativa o bem
                                    var assetAnimalLeiloado = (from a in db.Assets
                                                            where a.Id == assetId

                                                            select a).FirstOrDefault();

                                    assetAnimalLeiloado.Status = false;
                                    db.Entry(assetAnimalLeiloado).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Insere o registro de obsoleto no sucata
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                    #endregion
                            }



                            listaMovimentacoesPatrimoniaisEmLote.Add(assetMoviment);
                        }

                        var objParaAlteracaoContabil = listaMovimentacoesPatrimoniaisEmLote.FirstOrDefault();
                        if (movimentoViewModel.MovementTypeId != (int)EnumMovimentType.MovimentacaoInterna)
                        {
                            ComputaAlteracaoContabil(objParaAlteracaoContabil.InstitutionId, objParaAlteracaoContabil.BudgetUnitId,
                                                     objParaAlteracaoContabil.ManagerUnitId, (int)objParaAlteracaoContabil.AuxiliaryAccountId);

                            if (movimentoViewModel.MovementTypeId == (int)EnumMovimentType.MovInservivelNaUGE) {
                                ComputaAlteracaoContabil(objParaAlteracaoContabil.InstitutionId, objParaAlteracaoContabil.BudgetUnitId,
                                                     objParaAlteracaoContabil.ManagerUnitId, (int)objParaAlteracaoContabil.ContaContabilAntesDeVirarInservivel);
                            }
                        }

                        transaction.Complete();
                    }

                    if (MovimentacaoIntegradoAoSIAFEM)
                    {
                        var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                        var tuplas = svcIntegracaoSIAFEM.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovimentacoesPatrimoniaisEmLote, true);

                        DadosPopupContabiliza objPopUp = new DadosPopupContabiliza();

                        objPopUp.MsgSIAFEM = new List<string>();
                        objPopUp.ListaIdAuditoria = string.Empty;
                        objPopUp.LoginSiafem = movimentoViewModel.LoginSiafem;
                        objPopUp.SenhaSiafem = movimentoViewModel.SenhaSiafem;

                        foreach (var item in tuplas) {
                            objPopUp.MsgSIAFEM.Add(item.Item1);
                            objPopUp.ListaIdAuditoria += item.Item3.ToString() + ",";
                        }

                        objPopUp.ListaIdAuditoria += "0";

                        return View("_VazioBaseParaPopupsSIAFEM", objPopUp);
                    }
                    else {
                        var loginUsuarioSIAFEM = movimentoViewModel.LoginSiafem;
                        var senhaUsuarioSIAFEM = movimentoViewModel.SenhaSiafem;
                        string msgInformeAoUsuario = null;
                        msgInformeAoUsuario = base.novoMetodoProcessamentoMovimentacaoPatrimonialNoContabilizaSP(movimentoViewModel.MovementTypeId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, NumberDocGerado, listaMovimentacoesPatrimoniaisEmLote);
                        msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();
                        TempData["msgSucesso"] = msgInformeAoUsuario;
                        TempData["numeroDocumento"] = NumberDocGerado;
                        TempData.Keep();

                        return RedirectToAction("Index");
                    }
                }

                //Recarrega os combos da tela                        
                CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                CarregaComboTipoMovimento();
                CarregaComboContaContabil();

                CarregaViewBagsPorTipoMovimento(movimentoViewModel);

                return View(movimentoViewModel);


            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        private void CarregaViewBagsPorTipoMovimento(MovimentoViewModel movimentoViewModel)
        {
            //Recarrega todos os combos de acordo com o tipo de movimento
            switch (movimentoViewModel.MovementTypeId)
            {
                case (int)EnumMovimentType.MovimentacaoInterna:
                    CarregaHierarquiaDestinoComUAEDivisao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino,
                                                          movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.SectionIdDestino);
                    CarregaComboResponsavel(movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.ResponsibleId);
                    break;

                case (int)EnumMovimentType.VoltaConserto:
                case (int)EnumMovimentType.SaidaConserto:
                case (int)EnumMovimentType.MovInservivelNaUGE:
                case (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria:
                case (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual:
                case (int)EnumMovimentType.RetiradaDaBolsa:
                case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:
                case (int)EnumMovimentType.MovMorteAnimalPatrimoniado:
                case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:
                case (int)EnumMovimentType.MovSementesPlantasInsumosArvores:
                case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:
                case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                case (int)EnumMovimentType.MovVendaLeilaoSemoventes:
                    CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                    using (db = new MovimentoContext())
                    {
                        if (!MesmaGestaoDoFundoSocial(_institutionId))
                            CarregaFundoSocial();
                        else
                            movimentoViewModel.Proibido = true;
                    }
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                    using (db = new MovimentoContext())
                    {
                        if (MesmaGestaoDoFundoSocial(_institutionId))
                            CarregaFundoSocial();
                        else
                            movimentoViewModel.Proibido = true;
                    }
                    break;
                case (int)EnumMovimentType.MovComodatoConcedidoBensMoveis:
                    CarregaHierarquiaDestinoMesmaGestao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino);
                    break;
                case (int)SAM.Web.Common.Enum.EnumMovimentType.MovDoacaoIntraNoEstado:
                    CarregaHierarquiaDestinoOutrasGestoes(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino);
                    CarregaComboResponsavel(movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.ResponsibleId);
                    break;
                case (int)SAM.Web.Common.Enum.EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    CarregaHierarquiaDestinoMesmaGestao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino, false);
                    CarregaComboResponsavel(movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.ResponsibleId);
                    break;
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                    CarregaHierarquiaDestinoMesmoOrgao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino);
                    break;
            }
        }

        private string BuscaUltimoNumeroDocumentoSaida(int IdUGE) {

            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA] @ManagerUnitId = " + IdUGE.ToString());
            builder.Append(",@Ano = " + DateTime.Now.Year.ToString());

            return db.Database.SqlQuery<string>(builder.ToString()).FirstOrDefault();
        }

        [HttpGet]
        public ActionResult CarregaPartialViewHistorico(int assetId, bool somenteAtivos)
        {

            try
            {
                db = new MovimentoContext();
                return PartialView("_historicoDeMovimentacao", ListaHistoricoDeMovimentacoes(assetId, somenteAtivos));
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        public ActionResult CarregaPatrimonios(int? institutionId, int? contaContabilId, int? budgetUnitId, int? managerUnitId, int? administrativeUnitId, int? sectionId, string searchString, string listAssets, int? page, int? movimentTypeId, bool pesquisa = false)
        {
            try
            {

                var movimentoViewModel = new MovimentoViewModel();

                List<int> listAssertsSelected = string.IsNullOrEmpty(listAssets) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(listAssets);

                if ((listAssertsSelected != null && listAssertsSelected.Count() > 0) || (pesquisa == true && movimentTypeId != null && movimentTypeId > 0))
                {
                    IQueryable<AssetEAssetMovimentViewModel> lstDadosBD = null;
                    //IQueryable<AssetEAssetMovimentViewModel> listAssetsDB = null;

                    using (db = new MovimentoContext())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ValidateOnSaveEnabled = false;

                        lstDadosBD = (from a in db.Assets.Include("RelatedInitial")
                                      join am in db.AssetMovements.Include("RelatedMovementType")
                                      on a.Id equals am.AssetId
                                      where
                                            am.Status == true &&
                                            am.AuxiliaryAccountId == ((contaContabilId.HasValue && contaContabilId != 0) ? contaContabilId : am.AuxiliaryAccountId) &&                                            
                                            am.InstitutionId == institutionId &&
                                            am.BudgetUnitId == budgetUnitId &&
                                            !a.flagVerificado.HasValue &&
                                            a.flagDepreciaAcumulada == 1 &&
                                            am.ManagerUnitId == managerUnitId &&
                                            am.AdministrativeUnitId == ((administrativeUnitId.HasValue && administrativeUnitId != 0) ? administrativeUnitId : am.AdministrativeUnitId) &&
                                            am.SectionId == ((sectionId.HasValue && sectionId != 0) ? sectionId : am.SectionId)
                                      select new AssetEAssetMovimentViewModel
                                      {
                                          Id = a.Id,
                                          Sigla = a.RelatedInitial == null ? a.InitialName : a.RelatedInitial.Name,
                                          Chapa = a.NumberIdentification,
                                          ChapaCompleta = a.ChapaCompleta,
                                          CodigoMaterial = a.MaterialItemCode,
                                          DescricaoMaterial = a.MaterialItemDescription,
                                          GrupoMaterial = a.MaterialGroupCode,
                                          DescricaoTipodeMovimento = am.RelatedMovementType == null ? string.Empty : am.RelatedMovementType.Description,
                                          Responsavel = am.RelatedResponsible == null ? string.Empty : am.RelatedResponsible.Name,
                                          CodigoUGE = am.RelatedManagerUnit.Code,
                                          CodigoUA = am.RelatedAdministrativeUnit == null ? string.Empty : am.RelatedAdministrativeUnit.Code.ToString(),
                                          DescricaoDivisao = am.RelatedSection == null ? string.Empty : am.RelatedSection.Description,
                                          ValorAtual = a.ValueUpdate,
                                          DataAquisicao = a.AcquisitionDate,
                                          MovementTypeId = am.MovementTypeId,
                                          UGEId = am.ManagerUnitId,
                                          Acervo = (a.flagAcervo == true),
                                          Terceiro = (a.flagTerceiro == true),
                                          IdContaContabil = am.AuxiliaryAccountId
                                      }).AsNoTracking();

                        switch (movimentTypeId)
                        {
                            case (int)EnumMovimentType.MovimentacaoInterna:
                            case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:
                                lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual);
                                break;
                            case (int)EnumMovimentType.SaidaConserto:
                                lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                                 x.IdContaContabil != 212508);
                                break;
                            case (int)EnumMovimentType.VoltaConserto:
                                lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId == (int)EnumMovimentType.SaidaConserto);
                                break;
                            case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                            case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                            case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                                lstDadosBD = lstDadosBD.Where(x => (
                                                                    x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                                    x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                                    x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                                    x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                                    x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                                    x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                    x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                                    x.IdContaContabil != 212508 &&
                                                                    x.Terceiro == false
                                                                 )
                                                            );
                                break;
                            case (int)EnumMovimentType.RetiradaDaBolsa:
                                lstDadosBD = lstDadosBD.Where(x => (
                                                                    x.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria ||
                                                                    x.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual
                                                                 ));
                                break;
                            case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                            case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                            case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                                //Id da conta contábil 123110805 - "INSERVÍVEL NA UGE - BENS MÓVEIS" em 22/06/2020
                                lstDadosBD = lstDadosBD.Where(x => (
                                                                    x.MovementTypeId == (int)EnumMovimentType.MovInservivelNaUGE ||
                                                                    x.IdContaContabil == 212508
                                                                 ));
                                break;
                            case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                                lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                   x.Terceiro == true);
                                break;
                            case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:
                                lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                                 x.IdContaContabil != 212508 &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual);
                                break;
                            case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:
                            case (int)EnumMovimentType.MovVendaLeilaoSemoventes:
                                lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                                 x.IdContaContabil != 212508 &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                                 x.Terceiro == false &&
                                                                 x.GrupoMaterial == 88);
                                break;
                            default:
                                lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                                 x.IdContaContabil != 212508 &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                                 x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                                 x.Terceiro == false);
                                break;
                        }

                        if (searchString != null && searchString != string.Empty)
                        {
                            lstDadosBD = (from i in lstDadosBD
                                          where
                                              i.Sigla.Contains(searchString) ||
                                              i.ChapaCompleta.Contains(searchString) ||
                                              i.CodigoMaterial.ToString().Contains(searchString) ||
                                              i.DescricaoMaterial.Contains(searchString) ||
                                              i.DescricaoTipodeMovimento.Contains(searchString) ||
                                              i.Responsavel.Contains(searchString) ||
                                              i.CodigoUGE.ToString().Contains(searchString) ||
                                              i.CodigoUA.ToString().Contains(searchString) ||
                                              i.DataAquisicao.ToString().Contains(searchString)
                                          select i
                                            );
                        }

                        var ListaIdBPsASeremReclassificados = (from l in lstDadosBD
                                                               join rc in db.BPsARealizaremReclassificacaoContabeis on l.Id equals rc.AssetId 
                                                               select l.Id);

                        lstDadosBD = lstDadosBD.Where(l => !ListaIdBPsASeremReclassificados.Contains(l.Id));

                        int pageSize = 5;
                        int pageNumber = (page ?? 1);

                        int _count = lstDadosBD.Count();
                        ViewBag.QtdBps = _count;
                        var countSelected = listAssertsSelected == null ? 0 : listAssertsSelected.Count();

                        if (countSelected == _count || countSelected >= 100)
                            ViewBag.HabilitaTodos = false;
                        else
                            ViewBag.HabilitaTodos = true;

                        long paraValidacao = 0;

                        var result = lstDadosBD.AsEnumerable().OrderBy(s => long.TryParse(s.Chapa, out paraValidacao) ? long.Parse(s.Chapa) : -1).ThenBy(s => s.Sigla).Skip(((pageNumber) - 1) * pageSize).Take(pageSize).ToList();

                        if (listAssertsSelected != null)
                            result.Where(x => listAssertsSelected.Contains(x.Id)).ToList().ForEach(x => x.Selecionado = true);

                        movimentoViewModel.listaAssetEAssetViewModel = new StaticPagedList<AssetEAssetMovimentViewModel>(result, pageNumber, pageSize, _count);

                        foreach (AssetEAssetMovimentViewModel viewModel in movimentoViewModel.listaAssetEAssetViewModel)
                        {
                            if (!viewModel.Acervo && !viewModel.Terceiro)
                            {
                                decimal? valorAtualTabelaDepreciacaoMensal = (from m in db.MonthlyDepreciations
                                                                              join a in db.Assets on m.AssetStartId equals a.AssetStartId
                                                                              where a.Id == viewModel.Id &&
                                                                              m.ManagerUnitId == viewModel.UGEId &&
                                                                              m.MaterialItemCode == viewModel.CodigoMaterial
                                                                              group m by new { m.AssetStartId } into g
                                                                              select g.Min(p => p.CurrentValue)).FirstOrDefault();

                                if (valorAtualTabelaDepreciacaoMensal != null && valorAtualTabelaDepreciacaoMensal != 0)
                                    viewModel.ValorAtual = valorAtualTabelaDepreciacaoMensal;
                            }

                            viewModel.DataAquisicaoFormatada = viewModel.DataAquisicao.ToString("dd/MM/yyyy");

                            if (viewModel.IdContaContabil != null)
                                viewModel.ContaContabil = (from a in db.AuxiliaryAccounts
                                                           where a.Id == viewModel.IdContaContabil
                                                           select a.ContaContabilApresentacao).FirstOrDefault();
                        }
                    }
                }

                return PartialView("_gridAssetsDisponiveis", movimentoViewModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        public ActionResult CarregaPatrimoniosParaMovimento(int? institutionId, int? budgetUnitId, int? managerUnitId, int? administrativeUnitId, int? sectionId, int? page, string listAssets)
        {
            List<int> listAssertsSelected = string.IsNullOrEmpty(listAssets) ? new List<int>() : Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(listAssets);

            var movimentoViewModel = new MovimentoViewModel();
            ViewBag.Count = 0;

            if (listAssertsSelected.Count() > 0)
            {
                int pageSize = 5;
                int pageNumber = (page ?? 1);

                using (db = new MovimentoContext())
                {
                    var listAssetsDB = (from a in db.Assets
                                        join am in db.AssetMovements
                                        on a.Id equals am.AssetId
                                        where am.Status == true &&
                                              listAssertsSelected.Contains(am.AssetId) &&
                                              !a.flagVerificado.HasValue &&
                                              a.flagDepreciaAcumulada == 1
                                        select new AssetEAssetMovimentViewModel
                                        {
                                            Id = a.Id,
                                            Sigla = a.RelatedInitial == null ? a.InitialName : a.RelatedInitial.Name,
                                            Chapa = a.NumberIdentification,
                                            ChapaCompleta = a.ChapaCompleta,
                                            CodigoMaterial = a.MaterialItemCode,
                                            DescricaoMaterial = a.MaterialItemDescription,
                                            DescricaoTipodeMovimento = am.RelatedMovementType == null ? string.Empty : am.RelatedMovementType.Description,
                                            Responsavel = am.RelatedResponsible == null ? string.Empty : am.RelatedResponsible.Name,
                                            CodigoUGE = am.RelatedManagerUnit.Code,
                                            CodigoUA = am.RelatedAdministrativeUnit == null ? string.Empty : am.RelatedAdministrativeUnit.Code.ToString(),
                                            DescricaoDivisao = am.RelatedSection == null ? string.Empty : am.RelatedSection.Description,
                                            ValorAtual = a.ValueUpdate,
                                            DataAquisicao = a.AcquisitionDate,
                                            MovementTypeId = a.MovementTypeId,
                                            UGEId = am.ManagerUnitId,
                                            Acervo = (a.flagAcervo == true),
                                            Terceiro = (a.flagTerceiro == true),
                                            IdContaContabil = am.AuxiliaryAccountId
                                        }).AsNoTracking();

                    ViewBag.Count = listAssetsDB.Count();

                    List<AssetEAssetMovimentViewModel> result;

                    long paraValidacao;

                    if (listAssertsSelected.Any())
                        result = listAssetsDB.AsEnumerable().OrderBy(s => long.TryParse(s.Chapa, out paraValidacao) ? long.Parse(s.Chapa) : -1).ThenBy(s => s.Sigla).Skip(((pageNumber) - 1) * pageSize).Take(pageSize).ToList();
                    else
                        //Não deve passar por aqui nunca. Somente para o compilador não reclamar na próximo instrução.
                        result = new List<AssetEAssetMovimentViewModel>();

                    movimentoViewModel.listaAssetEAssetViewModel = new StaticPagedList<AssetEAssetMovimentViewModel>(result, pageNumber, pageSize, listAssetsDB.Count());

                    foreach (AssetEAssetMovimentViewModel viewModel in movimentoViewModel.listaAssetEAssetViewModel)
                    {
                        if (!viewModel.Acervo && !viewModel.Terceiro)
                        {
                            decimal? valorAtualTabelaDepreciacaoMensal = (from m in db.MonthlyDepreciations
                                                                          join a in db.Assets on m.AssetStartId equals a.AssetStartId
                                                                          where a.Id == viewModel.Id &&
                                                                          m.ManagerUnitId == viewModel.UGEId &&
                                                                          m.MaterialItemCode == viewModel.CodigoMaterial
                                                                          group m by new { m.AssetStartId } into g
                                                                          select g.Min(p => p.CurrentValue)).FirstOrDefault();

                            if (valorAtualTabelaDepreciacaoMensal != null && valorAtualTabelaDepreciacaoMensal != 0)
                                viewModel.ValorAtual = valorAtualTabelaDepreciacaoMensal;
                        }

                        viewModel.DataAquisicaoFormatada = viewModel.DataAquisicao.ToString("dd/MM/yyyy");

                        if (viewModel.IdContaContabil != null)
                            viewModel.ContaContabil = (from a in db.AuxiliaryAccounts
                                                       where a.Id == viewModel.IdContaContabil
                                                       select a.ContaContabilApresentacao).FirstOrDefault();
                    }
                }
            }

            return PartialView("_gridMovimento", movimentoViewModel);
        }

        [HttpGet]
        public JsonResult IncluirTodosBps(int? institutionId, int? budgetUnitId, int? managerUnitId, int? administrativeUnitId, int? sectionId, string searchString, string listAssets, int? movimentTypeId, int? page)
        {
            try
            {
                IQueryable<AssetEAssetMovimentViewModel> lstDadosBD = null;

                using (db = new MovimentoContext())
                {
                    lstDadosBD = (from a in db.Assets
                                  join am in db.AssetMovements
                                  on a.Id equals am.AssetId
                                  where
                                        am.Status == true &&
                                        am.InstitutionId == institutionId &&
                                        am.BudgetUnitId == budgetUnitId &&
                                        !a.flagVerificado.HasValue &&
                                        a.flagDepreciaAcumulada == 1 &&
                                        am.ManagerUnitId == managerUnitId &&
                                        am.AdministrativeUnitId == ((administrativeUnitId.HasValue && administrativeUnitId != 0) ? administrativeUnitId : am.AdministrativeUnitId) &&
                                        am.SectionId == ((sectionId.HasValue && sectionId != 0) ? sectionId : am.SectionId)
                                  select new AssetEAssetMovimentViewModel
                                  {
                                      Id = a.Id,
                                      Sigla = a.RelatedInitial == null ? a.InitialName : a.RelatedInitial.Name,
                                      Chapa = a.NumberIdentification,
                                      CodigoMaterial = a.MaterialItemCode,
                                      DescricaoMaterial = a.MaterialItemDescription,
                                      GrupoMaterial = a.MaterialGroupCode,
                                      DescricaoTipodeMovimento = am.RelatedMovementType == null ? string.Empty : am.RelatedMovementType.Description,
                                      Responsavel = am.RelatedResponsible == null ? string.Empty : am.RelatedResponsible.Name,
                                      CodigoUGE = am.RelatedManagerUnit.Code,
                                      CodigoUA = am.RelatedAdministrativeUnit == null ? string.Empty : am.RelatedAdministrativeUnit.Code.ToString(),
                                      DescricaoDivisao = am.RelatedSection == null ? string.Empty : am.RelatedSection.Description,
                                      ValorAtual = a.ValueUpdate,
                                      DataAquisicao = a.AcquisitionDate,
                                      MovementTypeId = am.MovementTypeId,
                                      UGEId = am.ManagerUnitId,
                                      IdContaContabil = am.AuxiliaryAccountId,
                                      Terceiro = (a.flagTerceiro == true)
                                  }).AsNoTracking();



                    switch (movimentTypeId)
                    {
                        case (int)EnumMovimentType.MovimentacaoInterna:
                        case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:
                            lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                             x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual);
                            break;
                        case (int)EnumMovimentType.SaidaConserto:
                            lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                             x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                             x.IdContaContabil != 212508);
                            break;
                        case (int)EnumMovimentType.VoltaConserto:
                            lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId == (int)EnumMovimentType.SaidaConserto);
                            break;
                        case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                        case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                        case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                            lstDadosBD = lstDadosBD.Where(x => (
                                                                x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                                x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                                x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                                x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                                x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                                x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                                x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                                x.IdContaContabil != 212508 &&
                                                                x.Terceiro == false
                                                             )
                                                        );
                            break;
                        case (int)EnumMovimentType.RetiradaDaBolsa:
                            lstDadosBD = lstDadosBD.Where(x => (
                                                                x.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria ||
                                                                x.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual
                                                             ));
                            break;
                        case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                        case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                        case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                            //Id da conta contábil 123110805 - "INSERVÍVEL NA UGE - BENS MÓVEIS" em 22/06/2020
                            lstDadosBD = lstDadosBD.Where(x => (
                                                                x.MovementTypeId == (int)EnumMovimentType.MovInservivelNaUGE ||
                                                                x.IdContaContabil == 212508
                                                             ));
                            break;
                        case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                            lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                               x.Terceiro == true);
                            break;
                        case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:
                            lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                             x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                             x.IdContaContabil != 212508 &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual);
                            break;
                        case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:
                        case (int)EnumMovimentType.MovVendaLeilaoSemoventes:
                            lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                             x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                             x.IdContaContabil != 212508 &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                             x.Terceiro == false &&
                                                             x.GrupoMaterial == 88);
                            break;
                        default:
                            lstDadosBD = lstDadosBD.Where(x => x.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                                                             x.MovementTypeId != (int)EnumMovimentType.Doacao &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                                                             x.MovementTypeId != (int)EnumMovimentType.SaidaConserto &&
                                                             x.MovementTypeId != (int)EnumMovimentType.MovInservivelNaUGE &&
                                                             x.IdContaContabil != 212508 &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                             x.MovementTypeId != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                             x.Terceiro == false);
                            break;
                    }

                    if (searchString != null && searchString != string.Empty)
                    {
                        lstDadosBD = (from i in lstDadosBD
                                      where
                                          i.Sigla.Contains(searchString) ||
                                          i.Chapa.Contains(searchString) ||
                                          i.CodigoMaterial.ToString().Contains(searchString) ||
                                          i.DescricaoMaterial.Contains(searchString) ||
                                          i.DescricaoTipodeMovimento.Contains(searchString) ||
                                          i.Responsavel.Contains(searchString) ||
                                          i.CodigoUGE.ToString().Contains(searchString) ||
                                          i.CodigoUA.ToString().Contains(searchString) ||
                                          i.DataAquisicao.ToString().Contains(searchString)
                                      select i
                                        ).AsQueryable().AsNoTracking();
                    }

                    int pageNumber = (page ?? 1);

                    long paraValidacao;

                    var lst = lstDadosBD.AsEnumerable().OrderBy(s => long.TryParse(s.Chapa, out paraValidacao) ? long.Parse(s.Chapa) : -1).ThenBy(s => s.Sigla).Select(a => a.Id).ToList();

                    return Json(new { lstDadosBD = lst.Skip(((pageNumber) - 1) * 5).Take(100) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { trava = "Favor verificar !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RecuperaMesAnoReferenciaPorUGE(int? managerUnitId)
        {
            try
            {
                string _mes;
                string _ano;

                if (managerUnitId != null && managerUnitId != 0)
                {
                    using (db = new MovimentoContext())
                    {
                        string _anoMesReferenciaFechamentoDaUGE = RecuperaAnoMesReferenciaFechamento((int)managerUnitId);

                        _mes = _anoMesReferenciaFechamentoDaUGE.Substring(4).PadLeft(2, '0');
                        _ano = _anoMesReferenciaFechamentoDaUGE.Substring(0, 4).PadLeft(4, '0');
                    }
                }
                else
                {
                    _mes = DateTime.Now.Month.ToString().PadLeft(2, '0');
                    _ano = DateTime.Now.Year.ToString().PadLeft(4, '0');
                }

                return Json(new
                {
                    mes = _mes,
                    ano = _ano
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public JsonResult ValidaOrgaoImplantado(int institutionIdDestino, int managerUnitIdDestino)
        {
            try
            {
                string mensagem = string.Empty;

                using (db = new MovimentoContext())
                {
                    bool? orgaoImplantado = (from m in db.Institutions
                                             where m.Id == institutionIdDestino
                                             select m.flagImplantado).FirstOrDefault();

                    if (orgaoImplantado == true)
                    {
                        bool? UGEComoOrgao = (from m in db.ManagerUnits
                                              where m.Id == managerUnitIdDestino
                                              select m.FlagTratarComoOrgao).FirstOrDefault();

                        if (UGEComoOrgao == true)
                        {
                            mensagem = "A UGE de Destino não precisa dar o \"Aceite\" por que não utiliza o SAM.";
                        }
                        else
                        {
                            mensagem = "A UGE de Destino precisa dar o \"Aceite\" para finalizar a Transferência/Doação.";
                        }
                    }
                    else
                        mensagem = "A UGE de Destino não precisa dar o \"Aceite\" por que não utiliza o SAM.";
                }

                return Json(new
                {
                    Texto = mensagem
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        private void ComputaAlteracaoContabil(int IdOrgao, int IdUO, int IdUGE, int IdContaContabil)
        {
            var implantado = UGEIntegradaAoSIAFEM(IdUGE);

            if (implantado)
            {
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

        #region Integracao Contabiliza-SP
        private string ProcessamentoEstornoMovimentacaoNoContabilizaSP(string loginUsuarioSIAFEM, string senhaUsuarioSIAFEM, AssetMovements movimentacaoPatrimonialParaEstorno)
        {
            string msgInformeAoUsuario;
            IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote = null;
            int tipoMovimentacaoPatrimonialId = 0;
            string numeroDocumentoSAM = null;

            //EXIBICAO DE POP-UP COM AS INFORMACOES REFERENTES A MOVIMENTACAO PATRIMONIAL RECEM-EFETIVADA (MAIS INFORMACOES ENVIADAS/RETORNADAS PARA/DE SISTEMA CONTABILIZA-SP)
            //POPULANDO CAMPOS DE ESTORNO
            movimentacaoPatrimonialParaEstorno.FlagEstorno = true;
            movimentacaoPatrimonialParaEstorno.LoginEstorno = 0;
            movimentacaoPatrimonialParaEstorno.DataEstorno = DateTime.Now;
            movimentacaoPatrimonialParaEstorno.Status = false;

            listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>() { movimentacaoPatrimonialParaEstorno };
            tipoMovimentacaoPatrimonialId = movimentacaoPatrimonialParaEstorno.MovementTypeId;
            numeroDocumentoSAM = movimentacaoPatrimonialParaEstorno.NumberDoc;
            msgInformeAoUsuario = base.processamentoMovimentacaoPatrimonialNoContabilizaSP(tipoMovimentacaoPatrimonialId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, numeroDocumentoSAM, listaMovimentacoesPatrimoniaisEmLote);
            msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();
            TempData["msgSucesso"] = msgInformeAoUsuario;
            TempData["numeroDocumento"] = numeroDocumentoSAM;
            TempData.Keep();

            //if (movimentacaoPatrimonialPrecisaAutenticacaoSIAFEM(assetId, assetMovementId))
            //{
            //    movPatrimonialSendoEstornda = db.AssetMovements.Where(movPatrimonial => movPatrimonial.AssetId == assetId
            //                                                                         && movPatrimonial.Id == assetMovementId
            //                                                                         && movPatrimonial.MovementTypeId == movimentTypeId)
            //                                                   .FirstOrDefault();
            //    if (movPatrimonialSendoEstornda.IsNotNull())
            //    {
            //        listaMovimentacoesPatrimoniais = new List<AssetMovements>();
            //        listaMovimentacoesPatrimoniais.Add(movPatrimonialSendoEstornda);
            //        //EXIBICAO DE POP-UP COM AS INFORMACOES REFERENTES A MOVIMENTACAO PATRIMONIAL RECEM-EFETIVADA (MAIS INFORMACOES ENVIADAS/RETORNADAS PARA/DE SISTEMA CONTABILIZA-SP)
            //        var loginUsuarioSIAFEM = "USUARIO_LOGIN_TESTE";// String.Empty;// assetViewModel.LoginSiafem;
            //        var senhaUsuarioSIAFEM = "USUARIO_SENHA_TESTE";// String.Empty;// assetViewModel.SenhaSiafem;
            //        msgInformeAoUsuario = base.processamentoMovimentacaoPatrimonialNoContabilizaSP(movPatrimonialSendoEstornda.MovementTypeId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, movPatrimonialSendoEstornda.NumberDoc, listaMovimentacoesPatrimoniais);
            //        msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();
            //    }
            //}
            return msgInformeAoUsuario;
        }

        private bool ExisteSohUmaNotaLancamentoPendenteVinculada(int bemPatrimonialId, int movimentacaoPatrimonialId, int tipoAgrupamentoMovimentacaoId = 0, bool ehEstorno = false)
        {
            bool existeSohUmaNLPendente = false;
            int[] tiposAgrupamentoMovimentacaoIds = null;
            AssetMovements movimentacaoPatrimonial = null;
            Expression<Func<AssetMovements, bool>> expWhere = null;
            Expression<Func<AssetMovements, bool>> expWhereExisteNLs = null;
            Expression<Func<AssetMovements, bool>> expWhereFitroEstorno = null;
            Expression<Func<AssetMovements, bool>> expWhereFiltroAgrupamento = null;
            SAMContext contextoCamadaDados = null;


            try
            {
                tiposAgrupamentoMovimentacaoIds = new int[] { EnumGroupMoviment.Incorporacao.GetHashCode(), EnumGroupMoviment.Movimentacao.GetHashCode() };


                //Filtro Basico
                expWhere = (movPatrimonial => movPatrimonial.AssetId == bemPatrimonialId
                                           && movPatrimonial.Id == movimentacaoPatrimonialId
                                           && movPatrimonial.NotaLancamentoPendenteSIAFEMId.HasValue);

                //Filtro Estorno
                if (ehEstorno)
                {
                    expWhereFitroEstorno = (movPatrimonial => movPatrimonial.Status == false
                                                           && movPatrimonial.FlagEstorno == true
                                                           && movPatrimonial.DataEstorno.HasValue
                                                           && movPatrimonial.LoginEstorno.HasValue);

                    expWhere = expWhere.And(expWhereFitroEstorno);
                }

                if (tiposAgrupamentoMovimentacaoIds.Contains(tipoAgrupamentoMovimentacaoId))
                {
                    expWhereFiltroAgrupamento = (movPatrimonial => movPatrimonial.RelatedMovementType.GroupMovimentId == tipoAgrupamentoMovimentacaoId);

                    if (!ehEstorno)
                    {
                        expWhereExisteNLs = (movPatrimonial => (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamento))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoDepreciacao))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoReclassificacao)));
                    }
                    else
                    {
                        expWhereExisteNLs = (movPatrimonial => (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoEstorno))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoDepreciacaoEstorno))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoReclassificacaoEstorno)));
                    }

                    expWhere = expWhere.And(expWhereExisteNLs);
                }

                using (contextoCamadaDados = new SAMContext())
                {
                    InitDisconnectContext(ref contextoCamadaDados);

                    int numeroMovPatrimonialVinculadas = 0;

                    numeroMovPatrimonialVinculadas = contextoCamadaDados.AssetMovements.Count(movPatrimonial => movPatrimonial.NotaLancamentoPendenteSIAFEMId == movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId);
                    movimentacaoPatrimonial = contextoCamadaDados.AssetMovements.AsExpandable().Where(expWhere).FirstOrDefault();
                    existeSohUmaNLPendente = (movimentacaoPatrimonial.IsNotNull() && (numeroMovPatrimonialVinculadas == 1));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



            return existeSohUmaNLPendente;
        }

        private bool ExcluiNotaLancamentoPendenteSeSohUmaVinculada(int bemPatrimonialId, int movimentacaoPatrimonialId, int tipoAgrupamentoMovimentacaoId = 0, bool ehEstorno = false)
        {
            bool existeNLPendente = false;
            int[] tiposAgrupamentoMovimentacaoIds = null;
            AssetMovements movimentacaoPatrimonial = null;
            Expression<Func<AssetMovements, bool>> expWhere = null;
            Expression<Func<AssetMovements, bool>> expWhereExisteNLs = null;
            Expression<Func<AssetMovements, bool>> expWhereFitroEstorno = null;
            //Expression<Func<AssetMovements, bool>> expWhereReclassifica = null;
            Expression<Func<AssetMovements, bool>> expWhereFiltroAgrupamento = null;
            SAMContext contextoCamadaDados = null;
            int nlPendenteId = 0;

            try
            {
                tiposAgrupamentoMovimentacaoIds = new int[] { EnumGroupMoviment.Incorporacao.GetHashCode(), EnumGroupMoviment.Movimentacao.GetHashCode() };


                //Filtro Basico
                expWhere = (movPatrimonial => movPatrimonial.AssetId == bemPatrimonialId
                                           && movPatrimonial.Id == movimentacaoPatrimonialId
                                           && movPatrimonial.NotaLancamentoPendenteSIAFEMId.HasValue);

                //Filtro Estorno
                if (ehEstorno)
                {
                    expWhereFitroEstorno = (movPatrimonial => movPatrimonial.Status == false
                                                           && movPatrimonial.FlagEstorno == true
                                                           && movPatrimonial.DataEstorno.HasValue
                                                           && movPatrimonial.LoginEstorno.HasValue);

                    expWhere = expWhere.And(expWhereFitroEstorno);
                }

                if (tiposAgrupamentoMovimentacaoIds.Contains(tipoAgrupamentoMovimentacaoId))
                {
                    expWhereFiltroAgrupamento = (movPatrimonial => movPatrimonial.RelatedMovementType.GroupMovimentId == tipoAgrupamentoMovimentacaoId);

                    if (!ehEstorno)
                    {
                        expWhereExisteNLs = (movPatrimonial => (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamento))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoDepreciacao))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoReclassificacao)));
                    }
                    else
                    {
                        expWhereExisteNLs = (movPatrimonial => (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoEstorno))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoDepreciacaoEstorno))
                                                            || (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoReclassificacaoEstorno)));
                    }

                    expWhere = expWhere.And(expWhereExisteNLs);
                }

                using (contextoCamadaDados = new SAMContext())
                {
                    InitDisconnectContext(ref contextoCamadaDados);


                    movimentacaoPatrimonial = contextoCamadaDados.AssetMovements
                                                                 .Where(expWhere)
                                                                 .FirstOrDefault();
                    if (movimentacaoPatrimonial.IsNotNull())
                    {
                        nlPendenteId = movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId ?? 0;
                        if (nlPendenteId > 0)
                        {
                            int numeroBPsVinculadosNlPendente = contextoCamadaDados.AssetMovements
                                                                                   .Where(movPatrimonial => movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId == nlPendenteId)
                                                                                   .Count();

                            if (numeroBPsVinculadosNlPendente == 1)
                            {
                                var nlPendenteParaExclusao = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                                .Where(nlPendente => nlPendente.Id == nlPendenteId)
                                                                                .FirstOrDefault();

                                movimentacaoPatrimonial = contextoCamadaDados.AssetMovements
                                                                             .Where(movPatrimonial => movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId == nlPendenteId)
                                                                             .First();

                                movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId = null;
                                contextoCamadaDados.AssetMovements.Add(movimentacaoPatrimonial);
                                contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Remove(nlPendenteParaExclusao);

                                contextoCamadaDados.SaveChanges();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



            return existeNLPendente;
        }

        private bool existeNotaLancamentoEstornoPendenteVinculada(int assetId, int assetMovementId)
        {
            bool existeNLPendente = false;
            AssetMovements movimentacaoPatrimonial = null;
            Expression<Func<AssetMovements, bool>> expWhere = null;
            SAMContext contextoCamadaDados = null;

            using (contextoCamadaDados = new SAMContext())
            {
                InitDisconnectContext(ref contextoCamadaDados);

                expWhere = (movPatrimonial => movPatrimonial.AssetId == assetId
                                           && movPatrimonial.Id == assetMovementId
                                           && movPatrimonial.Status == true
                                           && ((!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoEstorno)) ||
                                               (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoDepreciacaoEstorno)) ||
                                               (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoReclassificacaoEstorno)))
                                           && movPatrimonial.NotaLancamentoPendenteSIAFEMId.HasValue);

                movimentacaoPatrimonial = contextoCamadaDados.AssetMovements.Where(expWhere).FirstOrDefault();
                existeNLPendente = movimentacaoPatrimonial.IsNotNull();
            }


            return existeNLPendente;
        }

        private bool ExisteNotaLancamentoPendenteVinculada(int assetId, int assetMovementId, bool notaDeEstorno = false)
        {
            bool existeNLPendente = false;
            AssetMovements movimentacaoPatrimonial = null;
            Expression<Func<AssetMovements, bool>> expWhere = null;
            Expression<Func<AssetMovements, bool>> expWhereEstornoOuNao = null;
            SAMContext contextoCamadaDados = null;





            if (notaDeEstorno)
            {
                expWhereEstornoOuNao = (movPatrimonial => !((movPatrimonial.NotaLancamentoEstorno == null) && (movPatrimonial.NotaLancamentoEstorno.Trim() == string.Empty))
                                                       || !((movPatrimonial.NotaLancamentoDepreciacaoEstorno == null) && (movPatrimonial.NotaLancamentoDepreciacaoEstorno.Trim() == string.Empty))
                                                       || !((movPatrimonial.NotaLancamentoReclassificacaoEstorno == null) && (movPatrimonial.NotaLancamentoReclassificacaoEstorno.Trim() == string.Empty)));
            }
            else
            {
                expWhereEstornoOuNao = (movPatrimonial => !((movPatrimonial.NotaLancamento == null) && (movPatrimonial.NotaLancamentoEstorno.Trim() == string.Empty))
                                                       || !((movPatrimonial.NotaLancamentoDepreciacao == null) && (movPatrimonial.NotaLancamentoDepreciacao.Trim() == string.Empty))
                                                       || !((movPatrimonial.NotaLancamentoReclassificacao == null) && (movPatrimonial.NotaLancamentoReclassificacao.Trim() == string.Empty)));
            }

            using (contextoCamadaDados = new SAMContext())
            {
                //InitDisconnectContext(ref contextoCamadaDados);
                expWhere = (movPatrimonial => movPatrimonial.AssetId == assetId
                                           && movPatrimonial.Id == assetMovementId
                                           && movPatrimonial.Status == true
                                           && movPatrimonial.NotaLancamentoPendenteSIAFEMId.HasValue);

                expWhere = expWhere.And(expWhereEstornoOuNao);

                movimentacaoPatrimonial = contextoCamadaDados.AssetMovements.AsExpandable().Where(expWhere).FirstOrDefault();
                existeNLPendente = movimentacaoPatrimonial.IsNotNull();
            }


            return existeNLPendente;
        }

        private AssetMovements obterMovimentacaoParaProcessamento(int assetId, int assetMovementId, int movimentTypeId, int groupMovimentId)
        {
            AssetMovements rowTabela = null;
            SAMContext contextoCamadaDados = null;



            using (contextoCamadaDados = new SAMContext())
            {
                //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                rowTabela = db.AssetMovements.AsNoTracking()
                                             .Where(movPatrimonial => movPatrimonial.AssetId == assetId
                                                                   && movPatrimonial.Id == assetMovementId
                                                                   && movPatrimonial.MovementTypeId == movimentTypeId)
                                             .FirstOrDefault();
            }


            return rowTabela;
        }

        #endregion Integracao Contabiliza-SP

        #region Métodos privates

        #region Carrega
        private void CarregaHierarquiaFiltro(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null)
        {
            hierarquia = new Hierarquia();
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
            else
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            if (modelAdministrativeUnitId != 0)
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
            else
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            if (modelSectionId != 0)
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
            else
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);

        }
        private void CarregaHierarquiaDestino(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null, bool? mesmoOrgao = null)
        {
            hierarquia = new Hierarquia();
            if (mesmoOrgao != null)
            {
                if (mesmoOrgao == true)
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                {
                    List<Institution> lstInstitution;

                    if (managerCode == null)
                    {
                        lstInstitution = hierarquia.GetOrgaos(null);
                        ViewBag.InstitutionsDestino = new SelectList(lstInstitution.Where(i => i.Id != modelInstitutionId), "Id", "Description");
                    }
                    else
                    {
                        if (mesmaGestao)
                        {
                            lstInstitution = hierarquia.GetOrgaosMesmaGestao(managerCode, null);
                            ViewBag.InstitutionsDestino = new SelectList(lstInstitution.Where(i => i.Id != modelInstitutionId), "Id", "Description", modelInstitutionId);
                        }
                        else
                        {
                            lstInstitution = hierarquia.GetOrgaosGestaoDiferente(managerCode, null);
                            ViewBag.InstitutionsDestino = new SelectList(lstInstitution.Where(i => i.Id != modelInstitutionId), "Id", "Description", modelInstitutionId);
                        }
                    }
                }

            }
            else if (managerCode == null)
            {
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
            }


            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if (modelManagerUnitId != 0)
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            if (modelAdministrativeUnitId != 0)
                ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
            else
                ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            if (modelSectionId != 0)
                ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
            else
                ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);

        }
        private void CarregaHierarquiaDestinoUAeSection(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null)
        {
            hierarquia = new Hierarquia();
            if (managerCode == null)
            {
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, null), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, null), "Id", "Description", modelInstitutionId);
            }

            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
        }

        private void CarregaHierarquiaDestinoComUAEDivisao(int modelInstitutionId, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0) {
            hierarquia = new Hierarquia();
            if (modelInstitutionId == 0)
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", _institutionId);
            else
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);

            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE)
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);

        }

        private void CarregaHierarquiaDestinoOutrasGestoes(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0) {
            if (hierarquia == null)
                hierarquia = new Hierarquia();

            ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosGestaoDiferente(RecuperaGestao(), null), "Id", "Description", modelInstitutionId);
            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
        }

        private void CarregaHierarquiaDestinoMesmaGestao(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, bool podeMesmaGestao = true)
        {
            if (hierarquia == null)
                hierarquia = new Hierarquia();

            if (podeMesmaGestao)
            {
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestao(RecuperaGestao(), null), "Id", "Description", modelInstitutionId);
            }
            else {
                List<Institution> listaOrgaos = hierarquia.GetOrgaosMesmaGestao(RecuperaGestao(), null);
                ViewBag.InstitutionsDestino = new SelectList(listaOrgaos.Where(i => i.Id != _institutionId), "Id", "Description", modelInstitutionId);
            }

            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
        }

        private void CarregaHierarquiaDestinoMesmoOrgao(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0)
        {
            if (hierarquia == null)
                hierarquia = new Hierarquia();

            ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);
            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
        }

        private void CarregaFundoSocial() {
            if (db == null)
                db = new MovimentoContext();
            //Orgao 51004 - UO 5100
            var listaOrgao = db.Institutions.Where(i => i.Code == "51004").ToList();
            listaOrgao.ForEach(l => l.Description = l.Code + " - " + l.Description);

            ViewBag.InstitutionsDestino = new SelectList(listaOrgao, "Id", "Description");

            var listaUO = db.BudgetUnits.Where(i => i.Code == "51004").ToList();
            listaUO.ForEach(l => l.Description = l.Code + " - " + l.Description);

            ViewBag.BudgetUnitIdDestino = new SelectList(listaUO, "Id", "Description");

            var listaUGE = db.ManagerUnits.Where(m => m.Code == "510032" && m.Status == true).ToList();
            listaUGE.ForEach(m => m.Description = m.Code + " - " + m.Description);
            ViewBag.ManagerUnitIdDestino = new SelectList(listaUGE, "Id", "Description");
        }

        private void CarregaComboTipoMovimento()
        {
            var MovimentoTypes = (from mt in db.MovementTypes
                                  where mt.Status == true &&
                                        mt.GroupMovimentId == (int)EnumGroupMoviment.Movimentacao
                                  select mt).OrderBy(t => t.Description).ToList();

            ViewBag.MovimentType = new SelectList(MovimentoTypes, "Id", "Description");
        }

        private void CarregaComboContaContabil()
        {
            /*var ContaContabil = (from mt in db.AuxiliaryAccounts
                                  where mt.Status == true 
                                      //  mt.ContaContabilApresentacao !=""
                                  select mt).OrderBy(t => t.Description).ToList();*/

            var ListaContaContabil = db.AuxiliaryAccounts.Where(m => m.ContaContabilApresentacao != "" && m.Status == true).OrderBy(t => t.Description).ToList();
            ListaContaContabil.ForEach(m => m.Description = m.ContaContabilApresentacao + " - " + m.Description);

            ViewBag.ContaContabil = new SelectList(ListaContaContabil, "Id", "Description");
        }


        // Recomendado não apagar até uma decisão definitiva do destino dos tipos de documentos
        //private void CarregaComboTipoDeDocumentoSaida()
        //{
        //    var TypeDocumentOuts = (from td in db.TypeDocumentOuts
        //                            where td.Status == true
        //                            select td).ToList();

        //    ViewBag.TypeDocumentOut = new SelectList(TypeDocumentOuts, "Id", "Description");
        //}
        private void CarregaComboResponsavel(int administrativeUnitId = 0, int userId = 0)
        {
            var vResponsible = new List<Responsible>();

            if (administrativeUnitId != 0)
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == administrativeUnitId select r).ToList();

            if (userId != 0)
                ViewBag.User = new SelectList(vResponsible.ToList(), "Id", "Name", userId);
            else
                ViewBag.User = new SelectList(vResponsible.ToList(), "Id", "Name");
        }
        private void CarregaComboStatus(string codStatus)
        {
            var lista = new List<ItemGenericViewModel>();

            var itemGeneric = new ItemGenericViewModel
            {
                Id = 1,
                Description = "Ativos",
                Ordem = 5
            };

            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel
            {
                Id = 0,
                Description = "Baixados",
                Ordem = 10
            };

            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel
            {
                Id = 2,
                Description = "Transferências/Doações Pendentes",
                Ordem = 15
            };

            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel
            {
                Id = 3,
                Description = "A serem reclassificados",
                Ordem = 20
            };

            lista.Add(itemGeneric);

            if (codStatus != null && codStatus != "")
                ViewBag.Status = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description", int.Parse(codStatus));
            else
                ViewBag.Status = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");
        }
        private void CarregaComboFiltros(string codFiltro)
        {
            var lista = new List<ItemGenericViewModel>();
            var itemGeneric = new ItemGenericViewModel();

            itemGeneric = new ItemGenericViewModel { Id = 0, Description = "Todos os filtros...", Ordem = 0 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 1, Description = "Sigla", Ordem = 10 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 2, Description = "Chapa", Ordem = 20 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 3, Description = "Grupo do Material", Ordem = 30 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 4, Description = "Item", Ordem = 40 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 5, Description = "Descrição do Item", Ordem = 50 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 6, Description = "Órgão", Ordem = 60 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 7, Description = "UO", Ordem = 70 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 8, Description = "UGE", Ordem = 80 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 9, Description = "UA", Ordem = 90 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 10, Description = "Descrição da Divisão", Ordem = 100 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 11, Description = "Responsável", Ordem = 110 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 12, Description = "Valor de Aquisição", Ordem = 120 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 13, Description = "Depreciação Acumulada", Ordem = 130 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 14, Description = "Valor Atual", Ordem = 140 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 15, Description = "Depreciação", Ordem = 150 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 16, Description = "Vida Útil (Meses)", Ordem = 160 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 17, Description = "Conta Contábil", Ordem = 170 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 18, Description = "Desc. Conta Contábil", Ordem = 180 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 19, Description = "Data de Aquisição", Ordem = 190 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 20, Description = "Data de Incorporação", Ordem = 200 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 21, Description = "Empenho", Ordem = 210 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 22, Description = "Número de documento", Ordem = 220 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 23, Description = "Último Histórico", Ordem = 230 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 24, Description = "Data Último Histórico", Ordem = 240 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 25, Description = "Tipo", Ordem = 250 };
            lista.Add(itemGeneric);

            if (codFiltro != null && codFiltro != "")
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description", int.Parse(codFiltro));
            else
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");
        }
        #endregion


        #region Estorna
        private void EstornaMovimento(int assetId, int assetMovementId, int userID)
        {
            var assetMovimentDB = (from am in db.AssetMovements
                                   where am.Id == assetMovementId
                                   select am).FirstOrDefault();

            assetMovimentDB.Status = false;
            assetMovimentDB.FlagEstorno = true;
            assetMovimentDB.DataEstorno = DateTime.Now;
            assetMovimentDB.LoginEstorno = userID;

            db.Entry(assetMovimentDB).State = EntityState.Modified;
            db.SaveChanges();


            var assetsMovementSemUltimoMovimento = (from am in db.AssetMovements
                                                    where am.AssetId == assetId &&
                                                          am.FlagEstorno == null &&
                                                          am.Id != assetMovementId
                                                    select am.Id).OrderByDescending(am => am).FirstOrDefault();


            if (assetsMovementSemUltimoMovimento != 0)
            {
                var assetMovementPenultimoDB = (from a in db.AssetMovements
                                                where a.Id == assetsMovementSemUltimoMovimento
                                                select a).FirstOrDefault();

                assetMovementPenultimoDB.Status = true;

                db.Entry(assetMovementPenultimoDB).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void RegistraBPExcluido(int assetId, int assetMovementId, int usuario)
        {

            Asset BP = (from a in db.Assets where a.Id == assetId select a).FirstOrDefault();

            AssetMovements incorporacao = (from am in db.AssetMovements where am.Id == assetMovementId select am).FirstOrDefault();

            BPsExcluidos bpExcluido = new BPsExcluidos();
            bpExcluido.InitialId = BP.InitialId;
            bpExcluido.SiglaInicial = BP.InitialName;
            bpExcluido.Chapa = BP.ChapaCompleta;
            bpExcluido.ItemMaterial = BP.MaterialItemCode;
            bpExcluido.GrupoMaterial = BP.MaterialGroupCode;
            bpExcluido.ValorAquisicao = BP.ValueAcquisition;
            bpExcluido.DataAquisicao = BP.AcquisitionDate;
            bpExcluido.DataIncorporacao = BP.MovimentDate;
            bpExcluido.flagAcervo = BP.flagAcervo == null ? false : Convert.ToBoolean(BP.flagAcervo);
            bpExcluido.flagTerceiro = BP.flagTerceiro == null ? false : Convert.ToBoolean(BP.flagTerceiro);
            bpExcluido.flagDecretoSEFAZ = BP.flagDecreto == null ? false : Convert.ToBoolean(BP.flagDecreto);
            bpExcluido.TipoIncorporacao = incorporacao.MovementTypeId;
            bpExcluido.StateConservationId = incorporacao.StateConservationId;
            bpExcluido.ManagerUnitId = incorporacao.ManagerUnitId;
            bpExcluido.AdministrativeUnitId = incorporacao.AdministrativeUnitId;
            bpExcluido.ResponsibleId = incorporacao.ResponsibleId;
            bpExcluido.Processo = incorporacao.NumberPurchaseProcess;
            bpExcluido.NotaLancamento = incorporacao.NotaLancamento;
            bpExcluido.NotaLancamentoEstorno = incorporacao.NotaLancamentoEstorno;
            bpExcluido.NotaLancamentoDepreciacao = incorporacao.NotaLancamentoDepreciacao;
            bpExcluido.NotaLancamentoDepreciacaoEstorno = incorporacao.NotaLancamentoDepreciacaoEstorno;
            bpExcluido.NotaLancamentoReclassificacao = incorporacao.NotaLancamentoReclassificacao;
            bpExcluido.NotaLancamentoReclassificacaoEstorno = incorporacao.NotaLancamentoReclassificacaoEstorno;
            bpExcluido.NumeroDocumento = incorporacao.NumberDoc ?? BP.NumberDoc;
            bpExcluido.Observacoes = null;
            bpExcluido.DataAcao = DateTime.Now;
            bpExcluido.LoginAcao = usuario;

            db.BPsExcluidos.Add(bpExcluido);
            db.SaveChanges();

            //remove registros da tabela AssetMovements e suas depêndencias
            var historicos = (from am in db.AssetMovements where am.AssetId == assetId select am.Id.ToString());

            foreach (string historicoId in historicos)
            {
                //db.NotaLancamentoPendenteSIAFEMs.RemoveRange(db.NotaLancamentoPendenteSIAFEMs.Where(n => n.AssetMovementId == historico.Id));
                db.Database.ExecuteSqlCommand("delete from [dbo].[Closing] where AssetMovementsId = " + historicoId);
                db.Database.ExecuteSqlCommand("delete from [dbo].[LogAlteracaoDadosBP] where AssetMovementId = " + historicoId);
                //db.Closings.RemoveRange(db.Closings.Where(c => c.AssetMovementsId == historico.Id));
                db.Database.ExecuteSqlCommand("delete from [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] where AssetMovementsId = " + historicoId);
                db.Database.ExecuteSqlCommand("delete from [dbo].[AssetMovements] where Id = " + historicoId);
            }

            db.SaveChanges();

            string assetIdString = assetId.ToString();
            //remove registros relacionados ao registro da tabela Asset
            db.Database.ExecuteSqlCommand("delete from [dbo].[AssetNumberIdentification] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Repair] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[ItemInventario] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Exchange] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Closing] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[LogAlteracaoDadosBP] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[HistoricoValoresDecreto] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] where AssetId = " + assetIdString);

            if (incorporacao.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.Doacao &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorporacaoPorTransferencia &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorporacaoPorDoacao &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpDoacaoIntraNoEstado &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia &&
                BP.AssetStartId != null)
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MonthlyDepreciation] where AssetStartId = " + BP.AssetStartId.ToString());
            }

            //remove o registro da Asset
            db.Database.ExecuteSqlCommand("delete from [dbo].[Asset] where Id = " + assetIdString);

            db.SaveChanges();
        }
        private void ReativarMovimentoEAssetDeOutraUGE(int assetId)
        {
            /* Atribui o status ativo novamente para o movimento e limpa o AssetTransferenciaId para que 
            fique novamente pendente para o cliente que transferiu o item anteriormente */
            var _user = UserCommon.CurrentUser();

            var assetMovimentoDB = (from am in db.AssetMovements
                                    where am.AssetTransferenciaId == assetId
                                    select am).FirstOrDefault();

            assetMovimentoDB.Status = true;
            assetMovimentoDB.AssetTransferenciaId = null;

            db.Entry(assetMovimentoDB).State = EntityState.Modified;
            db.SaveChanges();
            //---------------------------------------------------------------------------------------

            //Reativa o último assetsMovimento antigo -----------------------------------------------
            var assetsMovimentoIdDB = (from am in db.AssetMovements
                                       where am.AssetId == assetMovimentoDB.AssetId &&
                                             am.FlagEstorno == null
                                       select am);

            var assetsMovimentoDB = (from a in db.AssetMovements
                                     where a.Id == assetsMovimentoIdDB.Max(x => x.Id)
                                     select a).FirstOrDefault();

            assetsMovimentoDB.Status = true;

            db.Entry(assetsMovimentoDB).State = EntityState.Modified;
            db.SaveChanges();

            //---------------------------------------------------------------------------------------

            //Reativa o asset -----------------------------------------------------------------------
            ReativarAsset(assetMovimentoDB.AssetId);
            //---------------------------------------------------------------------------------------
        }

        private void RetiraReferenciaNaOrigem(int assetId) {
            var _user = UserCommon.CurrentUser();

            var assetMovimentoDB = (from am in db.AssetMovements
                                    where am.AssetTransferenciaId == assetId
                                    select am).FirstOrDefault();

            assetMovimentoDB.AssetTransferenciaId = null;

            db.Entry(assetMovimentoDB).State = EntityState.Modified;
            db.SaveChanges();
        }
        private void EstornaAceiteAutomatico(int assetId, int assetMovementsId, int usuario) {
            int assetIdAposAceite = (int)db.AssetMovements.Find(assetMovementsId).AssetTransferenciaId;
            int assetMovementIdAposAceite = db.AssetMovements.Where(x => x.AssetId == assetIdAposAceite).Select(x => x.Id).FirstOrDefault();
            RegistraBPExcluido(assetIdAposAceite, assetMovementIdAposAceite, usuario);
            EstornaMovimento(assetId, assetMovementsId, usuario);
            Asset asset = (from a in db.Assets where a.Id == assetId select a).AsNoTracking().FirstOrDefault();
            asset.Status = true;
            db.Entry(asset).State = EntityState.Modified;
            db.SaveChanges();
        }
        private void ReativarAsset(int assetId)
        {
            var assetDB = (from a in db.Assets
                           where a.Id == assetId
                           select a).FirstOrDefault();

            assetDB.Status = true;

            db.Entry(assetDB).State = EntityState.Modified;
            db.SaveChanges();
        }

        /* 25/06/2020: A movimentação COMODATO/DE TERCEIROS - RECEBIDOS foi alterada para baixar
         * os BPs. Porém, os usuários fizeram essas movimentação duas vezes para o mesmo BP.
         * Então, caso o usuário estorne a última movimentação, esse método recoloca o BP
         * como baixado novamente. Será retirado assim q não tiver mais essa situação no banco */
        private void ValidaReativacaoAssetEstornoComodatoTerceiro(int assetId) {
            var tipoDeMovimentacaoAtiva = (from am in db.AssetMovements
                                           where am.AssetId == assetId
                                           && am.Status == true
                                           select am).FirstOrDefault();

            if (tipoDeMovimentacaoAtiva.MovementTypeId == (int)EnumMovimentType.MovComodatoTerceirosRecebidos)
            {

                tipoDeMovimentacaoAtiva.Status = false;
                db.Entry(tipoDeMovimentacaoAtiva).State = EntityState.Modified;

                var assetDB = (from a in db.Assets
                               where a.Id == assetId
                               select a).FirstOrDefault();

                assetDB.Status = false;

                db.Entry(assetDB).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        #endregion

        private bool movimentacaoPatrimonialPrecisaAutenticacaoSIAFEM(int bemPatrimonialId, int movimentacoPatrimonialId)
        {
            SAMContext contextoBancoDeDados = null;
            bool precisaAutenticacaoSIAFEM = false;
            bool integracaoSIAFEMAtivada = false;
            bool forcaNaoProcessamentoNoContabilizaSP = false;


            if ((bemPatrimonialId > 0) && (movimentacoPatrimonialId > 0))
            {
                this.initDadosSIAFEM();
                integracaoSIAFEMAtivada = this.ugeIntegradaSiafem;
                InitDisconnectContext(ref contextoBancoDeDados);

                var movPatrimonialParaConsulta = db.AssetMovements.Where(movPatrimonial => movPatrimonial.AssetId == bemPatrimonialId
                                                                                        && movPatrimonial.Id == movimentacoPatrimonialId)
                                                                  .FirstOrDefault();

                if (movPatrimonialParaConsulta.IsNotNull())
                {
                    var tipoMovPatrimonial = contextoBancoDeDados.MovementTypes.Where(tiposMovPatrimonial => tiposMovPatrimonial.Id == movPatrimonialParaConsulta.MovementTypeId).FirstOrDefault();
                    var ugeMovimentacaoId = movPatrimonialParaConsulta.ManagerUnitId;
                    var ugeMovimentacaoPatrimonial = ((ugeMovimentacaoId > 0) ? contextoBancoDeDados.ManagerUnits.Where(ugeMovPatrimonial => ugeMovPatrimonial.Id == ugeMovimentacaoId).FirstOrDefault() : new ManagerUnit());
                    var codigoUGEMovimentacao = (ugeMovimentacaoPatrimonial.IsNotNull() ? ugeMovimentacaoPatrimonial.Code : "0");
                    string nomeTipoMovimentacaoPatrimonial = ((tipoMovPatrimonial.IsNotNull()) ? tipoMovPatrimonial.Description : "DESCONHECIDO");




                    int mesReferenciaMovimentacao = -1;
                    int mesReferenciaUGE = -1;
                    string strMesReferenciaUGE = ugeMovimentacaoPatrimonial.ManagmentUnit_YearMonthReference;
                    string strMesReferenciaMovimentacao = movPatrimonialParaConsulta.MovimentDate.ToString("yyyyMM");
                    Int32.TryParse(strMesReferenciaMovimentacao, out mesReferenciaMovimentacao);
                    Int32.TryParse(strMesReferenciaUGE, out mesReferenciaUGE);
                    forcaNaoProcessamentoNoContabilizaSP = (((mesReferenciaUGE > 0) || (mesReferenciaMovimentacao > 0))
                                                            && ((mesReferenciaUGE <= 201911) || (mesReferenciaMovimentacao <= 201911)));


                    precisaAutenticacaoSIAFEM = ((tipoMovPatrimonial.IsNotNull() && tipoMovPatrimonial.PossuiContraPartidaContabil())
                                              && ugeIntegradaSiafem
                                              && !forcaNaoProcessamentoNoContabilizaSP);
                }
            }


            return precisaAutenticacaoSIAFEM;
        }

        private string RecuperaGestao()
        {
            if (db == null)
                using (db = new MovimentoContext()) {
                    return (from i in db.Institutions
                            where i.Id == _institutionId
                            select i.ManagerCode).FirstOrDefault();
                }
            else
                return (from i in db.Institutions
                        where i.Id == _institutionId
                        select i.ManagerCode).FirstOrDefault();
        }
        private List<AssetEAssetMovimentViewModel> ListaHistoricoDeMovimentacoes(int AssetId, bool somenteAtivos = true)
        {
            List<AssetEAssetMovimentViewModel> listAssetsEAssetsMoviments = (from a in db.Assets.Include("RelatedManagerUnit")
                                                                             join am in db.AssetMovements
                                                                             on a.Id equals am.AssetId
                                                                             where a.Id == AssetId
                                                                             && am.FlagEstorno == null
                                                                             select new AssetEAssetMovimentViewModel
                                                                             {
                                                                                 Asset = a,
                                                                                 AssetMoviment = am,
                                                                                 PodeSerEstornadaPorEstarAtiva = a.Status,
                                                                                 vindoComoTransferencia = false,
                                                                                 transferenciaFeita = false
                                                                             }).AsNoTracking().ToList();

            int IdUge = listAssetsEAssetsMoviments.First().Asset.ManagerUnitId;
            string mesReferenciaAtualDaUgeBanco = listAssetsEAssetsMoviments.First().Asset.RelatedManagerUnit.ManagmentUnit_YearMonthReference;

            bool BPASerReclassificado = db.BPsARealizaremReclassificacaoContabeis.Where(bp => bp.AssetId == AssetId).Any();

            if (listAssetsEAssetsMoviments.Last().AssetMoviment.MovementTypeId != (int)EnumMovimentType.ReclassificacaoContaContabil && !BPASerReclassificado)
            {
                int dataIncorporacao = Convert.ToInt32(listAssetsEAssetsMoviments.Min(x => x.AssetMoviment.MovimentDate).ToString("yyyyMM"));
                int dataUltimaMovimentacao = Convert.ToInt32(listAssetsEAssetsMoviments.Max(x => x.AssetMoviment.MovimentDate).ToString("yyyyMM"));
                
                string mesReferenciaIncialDaUgeBanco = listAssetsEAssetsMoviments.First().Asset.RelatedManagerUnit.ManagmentUnit_YearMonthStart;

                int mesReferenciaAtualDaUge = Convert.ToInt32(mesReferenciaAtualDaUgeBanco);
                int mesReferenciaInicialDaUge = Convert.ToInt32(mesReferenciaIncialDaUgeBanco);

                if ((mesReferenciaAtualDaUge >= dataIncorporacao
                        && mesReferenciaAtualDaUge <= dataUltimaMovimentacao)
                        || dataUltimaMovimentacao < mesReferenciaInicialDaUge
                    )
                {
                    int tipoUltimaMovimentacao = listAssetsEAssetsMoviments.Last().AssetMoviment.MovementTypeId;

                    if (tipoUltimaMovimentacao == (int)EnumMovimentType.MovDoacaoIntraNoEstado
                        || tipoUltimaMovimentacao == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado
                        || tipoUltimaMovimentacao == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado
                        || tipoUltimaMovimentacao == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao
                        || tipoUltimaMovimentacao == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia
                       )
                    {
                        int IdUGEDestino = (int)listAssetsEAssetsMoviments.Last().AssetMoviment.SourceDestiny_ManagerUnitId;

                        if (verificaNaoImplantacao(IdUGEDestino))
                        {
                            listAssetsEAssetsMoviments.Last().PodeSerEstornadaPorSerAceiteAutomatico = true;
                        }
                        else
                        {
                            if (listAssetsEAssetsMoviments.Last().AssetMoviment.AssetTransferenciaId == null)
                            {
                                listAssetsEAssetsMoviments.Last().PodeSerEstornadaPorNaoDependerDeAceite = true;
                            }
                            else
                            {
                                listAssetsEAssetsMoviments.Last().PodeSerEstornadaPorNaoDependerDeAceite = false;
                            }


                        }

                    }
                    else
                    {
                        listAssetsEAssetsMoviments.Last().PodeSerEstornadaPorNaoDependerDeAceite = true;
                    }

                }

                if (dataUltimaMovimentacao == mesReferenciaAtualDaUge
                    || dataUltimaMovimentacao < mesReferenciaInicialDaUge)
                {
                    if (listAssetsEAssetsMoviments.First().PodeSerEstornadaPorEstarAtiva
                       || listAssetsEAssetsMoviments.Last().PodeSerEstornadaPorSerAceiteAutomatico
                       || listAssetsEAssetsMoviments.Last().PodeSerEstornadaPorNaoDependerDeAceite
                      )
                        listAssetsEAssetsMoviments.Last().PodeEstornaMovimento = true;
                }
            }

            if (listAssetsEAssetsMoviments.First().Asset.flagAcervo == true
             || listAssetsEAssetsMoviments.First().Asset.flagTerceiro == true)
            {
                listAssetsEAssetsMoviments.ForEach(l => l.Asset.RateDepreciationMonthly = 0);
                listAssetsEAssetsMoviments.ForEach(l => l.Asset.LifeCycle = 0);
                listAssetsEAssetsMoviments.ForEach(l => l.Asset.ValueUpdate = l.Asset.ValueAcquisition);
            }
            else
            {
                int? AssetStartId = listAssetsEAssetsMoviments.First().Asset.AssetStartId;


                if (AssetStartId == null)
                {
                    listAssetsEAssetsMoviments.ForEach(l => l.Asset.RateDepreciationMonthly = 0);
                    listAssetsEAssetsMoviments.ForEach(l => l.Asset.LifeCycle = 0);

                    if (listAssetsEAssetsMoviments.First().Asset.flagDecreto != true)
                        listAssetsEAssetsMoviments.ForEach(l => l.Asset.ValueUpdate = l.Asset.ValueAcquisition);
                }
                else
                {
                    int CodigoItemMaterialBP = listAssetsEAssetsMoviments.First().Asset.MaterialItemCode;
                    int ano = Convert.ToInt32(mesReferenciaAtualDaUgeBanco.Substring(0, 4));
                    int mes = Convert.ToInt32(mesReferenciaAtualDaUgeBanco.Substring(4, 2));

                    var dadoDepreciacaoMovimento = (from m in db.MonthlyDepreciations
                                                    where m.AssetStartId == AssetStartId &&
                                                    m.ManagerUnitId == (IdUge) &&
                                                    m.MaterialItemCode == CodigoItemMaterialBP &&
                                                    m.CurrentValue <= m.ValueAcquisition &&
                                                    (m.CurrentDate.Year < ano
                                                    || m.CurrentDate.Year == ano
                                                    && m.CurrentDate.Month <= mes)
                                                    select m).AsNoTracking().OrderByDescending(m => m.Id).FirstOrDefault();

                    if (dadoDepreciacaoMovimento != null)
                    {
                        listAssetsEAssetsMoviments.ForEach(l => l.Asset.RateDepreciationMonthly = dadoDepreciacaoMovimento.RateDepreciationMonthly);
                        listAssetsEAssetsMoviments.ForEach(l => l.Asset.ValueUpdate = dadoDepreciacaoMovimento.CurrentValue);
                        listAssetsEAssetsMoviments.ForEach(l => l.Asset.LifeCycle = dadoDepreciacaoMovimento.LifeCycle);
                        //para tirar objeto da memória. Procurar fazer a função do Dispose
                        dadoDepreciacaoMovimento = null;
                    }
                    else
                    {
                        listAssetsEAssetsMoviments.ForEach(l => l.Asset.RateDepreciationMonthly = 0);
                        listAssetsEAssetsMoviments.ForEach(l => l.Asset.LifeCycle = 0);

                        if (listAssetsEAssetsMoviments.First().Asset.flagDecreto != true)
                            listAssetsEAssetsMoviments.ForEach(l => l.Asset.ValueUpdate = l.Asset.ValueAcquisition);
                    }
                }
            }

            var historicoTransferenciaOrigem = db.AssetMovements.Where(i => i.AssetTransferenciaId == AssetId).FirstOrDefault();
            if (historicoTransferenciaOrigem != null)
            {
                listAssetsEAssetsMoviments.First().vindoComoTransferencia = true;
                listAssetsEAssetsMoviments.First().movimentacaoComOrigem = true;
                listAssetsEAssetsMoviments.First().codigoUGEOrigem = historicoTransferenciaOrigem.RelatedManagerUnit.Code;
                listAssetsEAssetsMoviments.First().transferenciaFeita = true;
                listAssetsEAssetsMoviments.First().siglaChapaOutroBP = string.Format("{0}-{1}", historicoTransferenciaOrigem.RelatedAssets.InitialName, historicoTransferenciaOrigem.RelatedAssets.NumberIdentification);
            }
            else {
                int incorporacao = listAssetsEAssetsMoviments.First().Asset.MovementTypeId;
                //se for incorporação manual de transf. outro orgao ou doação intra no estado
                if ((
                      incorporacao == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                      incorporacao == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                      incorporacao == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                      incorporacao == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia
                     ) && listAssetsEAssetsMoviments.First().AssetMoviment.SourceDestiny_ManagerUnitId != null
                    )
                {
                    listAssetsEAssetsMoviments.First().vindoComoTransferencia = true;
                    listAssetsEAssetsMoviments.First().movimentacaoComOrigem = true;
                    listAssetsEAssetsMoviments.First().codigoUGEOrigem = listAssetsEAssetsMoviments.First().AssetMoviment.RelatedSourceDestinyManagerUnit.Code;
                }
            }

            var foiTransferido = listAssetsEAssetsMoviments
                                 .Where(item =>
                                        item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                                        item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                        item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado ||
                                        item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao ||
                                        item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia)
                                 .FirstOrDefault();

            if (foiTransferido != null) {
                listAssetsEAssetsMoviments.First().transferido = true;
                listAssetsEAssetsMoviments
                         .Where(item =>
                                item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                                item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado ||
                                item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao ||
                                item.AssetMoviment.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia)
                          .FirstOrDefault().movimentacaoComDestino = true;
            }

            listAssetsEAssetsMoviments.FirstOrDefault().possuiTipoMovimentoIntegradoAoSIAFEM = HistoricoDoBPTemTipoMovimentoIntegradoAoSIAFEM(listAssetsEAssetsMoviments);
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.NotaLancamento = viewModel.AssetMoviment.NotaLancamento);
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.NotaLancamentoEstorno = viewModel.AssetMoviment.NotaLancamentoEstorno);
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.NotaLancamentoDepreciacao = viewModel.AssetMoviment.NotaLancamentoDepreciacao);
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.NotaLancamentoDepreciacaoEstorno = viewModel.AssetMoviment.NotaLancamentoDepreciacaoEstorno);
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.NotaLancamentoReclassificacao = viewModel.AssetMoviment.NotaLancamentoReclassificacao);
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.NotaLancamentoReclassificacaoEstorno = viewModel.AssetMoviment.NotaLancamentoReclassificacaoEstorno);
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.possuiPendenciaNLAtivaVinculada = (viewModel.AssetMoviment.NotaLancamentoPendenteSIAFEMId.GetValueOrDefault() > 0));
            listAssetsEAssetsMoviments.ForEach(viewModel => viewModel.descricaoPossuiPendenciaNLAtivaVinculada = (viewModel.AssetMoviment.NotaLancamentoPendenteSIAFEMId.HasValue ? "Sim" : "Não"));

            foreach (var historico in listAssetsEAssetsMoviments) {
                if(historico.AssetMoviment.AuxiliaryAccountId != null)
                    historico.ContaContabil = (from a in db.AuxiliaryAccounts
                                               where a.Id == historico.AssetMoviment.AuxiliaryAccountId
                                               select a.ContaContabilApresentacao).FirstOrDefault();
            }


            return listAssetsEAssetsMoviments;

        }

        private bool MovimentacaoTemTodasAsNLsDeEstorno(int _assetMovementId, int _movimentTypeId)
        {
            bool temNLParaEstornar = false;
            bool tipoMovimentacaoPatrimonialDepreciaOuReclassifica = false;
            TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.Desconhecido;
            MovementType tipoMovimentacaoPatrimonial = null;
            AssetMovements movimentacaoPatrimonial = null;


            using (var contextoCamadaDados = new SAMContext())
            {
                tipoMovimentacaoPatrimonial = contextoCamadaDados.MovementTypes.AsNoTracking()
                                                                 .Where(movPatrimonial => movPatrimonial.Id == _movimentTypeId)
                                                                 .FirstOrDefault();

                tipoMovimentacaoPatrimonialDepreciaOuReclassifica = tipoMovimentacaoPatrimonial.ContraPartidaContabilDepreciaOuReclassifica();
                movimentacaoPatrimonial = contextoCamadaDados.AssetMovements.AsNoTracking()
                                                             .Where(movPatrimonial => movPatrimonial.Id == _assetMovementId
                                                                                   && movPatrimonial.MovementTypeId == _movimentTypeId)
                                                             .FirstOrDefault();
            }

            var dadosNLsMovimentacao = movimentacaoPatrimonial.ObterNLsMovimentacao(tipoNotaSIAFEM, true);
            if (dadosNLsMovimentacao.HasElements())
            {
                tipoNotaSIAFEM = TipoNotaSIAFEM.NL_Liquidacao;
                var infoNLMovimentacaoPatrimonial = dadosNLsMovimentacao.Where(detalheDadosNLMovimentacao => detalheDadosNLMovimentacao.Item1 == EnumGeral.GetEnumDescription(tipoNotaSIAFEM)
                                                                                                          && detalheDadosNLMovimentacao.Item2.HasElements())
                                                                        .FirstOrDefault();

                if (infoNLMovimentacaoPatrimonial.IsNotNull())
                    temNLParaEstornar |= infoNLMovimentacaoPatrimonial.Item2.HasElements();
            }

            if (tipoMovimentacaoPatrimonialDepreciaOuReclassifica)
            {
                if (tipoMovimentacaoPatrimonial.ContraPartidaContabilDeprecia())
                {
                    tipoNotaSIAFEM = TipoNotaSIAFEM.NL_Depreciacao;
                    dadosNLsMovimentacao = movimentacaoPatrimonial.ObterNLsMovimentacao(tipoNotaSIAFEM, true);
                    if (dadosNLsMovimentacao.HasElements())
                    {
                        var infoNLMovimentacaoPatrimonial = dadosNLsMovimentacao.Where(detalheDadosNLMovimentacao => detalheDadosNLMovimentacao.Item1 == EnumGeral.GetEnumDescription(tipoNotaSIAFEM)
                                                                                                                  && detalheDadosNLMovimentacao.Item2.HasElements())
                                                                                .FirstOrDefault();

                        if (infoNLMovimentacaoPatrimonial.IsNotNull())
                            temNLParaEstornar &= infoNLMovimentacaoPatrimonial.Item2.HasElements();
                    }
                }

                if (tipoMovimentacaoPatrimonial.ContraPartidaContabilReclassifica())
                {
                    tipoNotaSIAFEM = TipoNotaSIAFEM.NL_Reclassificacao;
                    dadosNLsMovimentacao = movimentacaoPatrimonial.ObterNLsMovimentacao(tipoNotaSIAFEM, true);
                    if (dadosNLsMovimentacao.HasElements())
                    {
                        var infoNLMovimentacaoPatrimonial = dadosNLsMovimentacao.Where(detalheDadosNLMovimentacao => detalheDadosNLMovimentacao.Item1 == EnumGeral.GetEnumDescription(tipoNotaSIAFEM)
                                                                                                                  && detalheDadosNLMovimentacao.Item2.HasElements())
                                                                                .FirstOrDefault();

                        if (infoNLMovimentacaoPatrimonial.IsNotNull())
                            temNLParaEstornar &= infoNLMovimentacaoPatrimonial.Item2.HasElements();
                    }
                }
            }

            return temNLParaEstornar;
        }

        private bool MovimentacaoPossuiNLParaSerEstornada(int _assetMovementId, int _movimentTypeId)
        {
            bool temNLParaEstornar = false;
            bool tipoMovimentacaoPatrimonialDepreciaOuReclassifica = false;
            TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.Desconhecido;
            MovementType tipoMovimentacaoPatrimonial = null;
            AssetMovements movimentacaoPatrimonial = null;


            using (var contextoCamadaDados = new SAMContext())
            {
                tipoMovimentacaoPatrimonial = contextoCamadaDados.MovementTypes.AsNoTracking()
                                                                 .Where(movPatrimonial => movPatrimonial.Id == _movimentTypeId)
                                                                 .FirstOrDefault();

                tipoMovimentacaoPatrimonialDepreciaOuReclassifica = tipoMovimentacaoPatrimonial.ContraPartidaContabilDepreciaOuReclassifica();
                movimentacaoPatrimonial = contextoCamadaDados.AssetMovements.AsNoTracking()
                                                             .Where(movPatrimonial => movPatrimonial.Id == _assetMovementId
                                                                                   && movPatrimonial.MovementTypeId == _movimentTypeId)
                                                             .FirstOrDefault();
            }

            var dadosNLsMovimentacao = movimentacaoPatrimonial.ObterNLsMovimentacao(tipoNotaSIAFEM, false);
            if (dadosNLsMovimentacao.HasElements())
            {
                tipoNotaSIAFEM = TipoNotaSIAFEM.NL_Liquidacao;
                var infoNLMovimentacaoPatrimonial = dadosNLsMovimentacao.Where(detalheDadosNLMovimentacao => detalheDadosNLMovimentacao.Item1 == EnumGeral.GetEnumDescription(tipoNotaSIAFEM)
                                                                                                          && detalheDadosNLMovimentacao.Item2.HasElements())
                                                                        .FirstOrDefault();

                if (infoNLMovimentacaoPatrimonial.IsNotNull())
                    temNLParaEstornar |= infoNLMovimentacaoPatrimonial.Item2.HasElements();
            }

            if (tipoMovimentacaoPatrimonialDepreciaOuReclassifica)
            {
                if (tipoMovimentacaoPatrimonial.ContraPartidaContabilDeprecia())
                {
                    tipoNotaSIAFEM = TipoNotaSIAFEM.NL_Depreciacao;
                    dadosNLsMovimentacao = movimentacaoPatrimonial.ObterNLsMovimentacao(tipoNotaSIAFEM, false);
                    if (dadosNLsMovimentacao.HasElements())
                    {
                        var infoNLMovimentacaoPatrimonial = dadosNLsMovimentacao.Where(detalheDadosNLMovimentacao => detalheDadosNLMovimentacao.Item1 == EnumGeral.GetEnumDescription(tipoNotaSIAFEM)
                                                                                                                  && detalheDadosNLMovimentacao.Item2.HasElements())
                                                                                .FirstOrDefault();

                        if (infoNLMovimentacaoPatrimonial.IsNotNull())
                            temNLParaEstornar |= infoNLMovimentacaoPatrimonial.Item2.HasElements();
                    }
                }

                if (tipoMovimentacaoPatrimonial.ContraPartidaContabilReclassifica())
                {
                    tipoNotaSIAFEM = TipoNotaSIAFEM.NL_Reclassificacao;
                    dadosNLsMovimentacao = movimentacaoPatrimonial.ObterNLsMovimentacao(tipoNotaSIAFEM, false);
                    if (dadosNLsMovimentacao.HasElements())
                    {
                        var infoNLMovimentacaoPatrimonial = dadosNLsMovimentacao.Where(detalheDadosNLMovimentacao => detalheDadosNLMovimentacao.Item1 == EnumGeral.GetEnumDescription(tipoNotaSIAFEM)
                                                                                                                  && detalheDadosNLMovimentacao.Item2.HasElements())
                                                                                .FirstOrDefault();

                        if (infoNLMovimentacaoPatrimonial.IsNotNull())
                            temNLParaEstornar |= infoNLMovimentacaoPatrimonial.Item2.HasElements();
                    }
                }
            }

            return temNLParaEstornar;
        }

        private bool MovimentacaoPossuiPendenciaNLAtiva(int _assetMovementId, int _movimentTypeId)
        {
            bool existePendenciaNLAtivaVinculada = false;
            int ugeId = 0;
            IList<NotaLancamentoPendenteSIAFEM> listaPendenciasNL_Ativas_UGE = null;
            IList<NotaLancamentoPendenteSIAFEM> listaPendenciasNL_Movimentacao_AssetMovementId = null;
            AssetMovements movimentacaoPatrimonial = null;



            using (var contextoCamadaDados = new SAMContext())
            {
                movimentacaoPatrimonial = contextoCamadaDados.AssetMovements
                                           .AsNoTracking()
                                           .Where(movPatrimonial => movPatrimonial.Id == _assetMovementId
                                                                 && movPatrimonial.MovementTypeId == _movimentTypeId)
                                           .FirstOrDefault();

                if (movimentacaoPatrimonial.IsNotNull())
                {
                    ugeId = movimentacaoPatrimonial.ManagerUnitId;

                    //obter todas as pendencias ativas da uge da movimentacao
                    //coluna AssetMovementIds possui todos os movimentos que integram a 'Pendencia NL' separados por espaço
                    //entao por performance eh mais rapido trazer a lista e fazer o campo item a item do que fazer a quebra 'in memory' desse campo e procurar o id em tudo ao mesmo tempo
                    listaPendenciasNL_Ativas_UGE = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                      .Where(nlPendente => nlPendente.ManagerUnitId == ugeId
                                                                                        && nlPendente.StatusPendencia == 1)
                                                                      .ToList();

                    listaPendenciasNL_Movimentacao_AssetMovementId = listaPendenciasNL_Ativas_UGE.Where(pendenciaNL => pendenciaNL.AssetMovementIds
                                                                                                                                  .BreakLine()
                                                                                                                                  .Contains(_assetMovementId.ToString()))
                                                                                                 .ToList();
                    existePendenciaNLAtivaVinculada = listaPendenciasNL_Movimentacao_AssetMovementId.HasElements();
                }
            }

            return existePendenciaNLAtivaVinculada;
        }

        private bool HistoricoDoBPTemTipoMovimentoIntegradoAoSIAFEM(List<AssetEAssetMovimentViewModel> listaHistorico) {
            int[] IdsMovimentoIntegradoAoSiafem = new int[] {
                (int)EnumMovimentType.MovSaidaInservivelUGETransferencia,
                (int)EnumMovimentType.IncorpAnimaisPesquisaSememPeixe,
                (int)EnumMovimentType.IncorpComodatoConcedidoBensMoveis,
                (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos,
                (int)EnumMovimentType.IncorpConfiscoBensMoveis,
                (int)EnumMovimentType.IncorpDoacaoConsolidacao,
                (int)EnumMovimentType.IncorpDoacaoIntraNoEstado,
                (int)EnumMovimentType.IncorpDoacaoMunicipio,
                (int)EnumMovimentType.IncorpDoacaoOutrosEstados,
                (int)EnumMovimentType.IncorpDoacaoUniao,
                (int)EnumMovimentType.IncorpVegetal,
                (int)EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao,
                (int)EnumMovimentType.IncorpNascimentoDeAnimais,
                (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao,
                (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia,
                (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado,
                (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado,
                (int)EnumMovimentType.MovSaidaInservivelUGEDoacao,
                (int)EnumMovimentType.MovSaidaInservivelUGETransferencia,
                (int)EnumMovimentType.MovComodatoConcedidoBensMoveis,
                (int)EnumMovimentType.MovComodatoTerceirosRecebidos,
                (int)EnumMovimentType.MovDoacaoConsolidacao,
                (int)EnumMovimentType.MovDoacaoIntraNoEstado,
                (int)EnumMovimentType.MovDoacaoMunicipio,
                (int)EnumMovimentType.MovDoacaoOutrosEstados,
                (int)EnumMovimentType.MovDoacaoUniao,
                (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis,
                (int)EnumMovimentType.MovMorteAnimalPatrimoniado,
                (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao,
                (int)EnumMovimentType.MovSementesPlantasInsumosArvores,
                (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado,
                (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado,
                (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis,
                (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis,
                (int)EnumMovimentType.MovInservivelNaUGE,
                (int)EnumMovimentType.MovVendaLeilaoSemoventes
            };

            foreach (AssetEAssetMovimentViewModel assetMovements in listaHistorico) {
                if (IdsMovimentoIntegradoAoSiafem.Contains(assetMovements.AssetMoviment.MovementTypeId))
                    return true;
            }

            return false;
        }
        private bool AssetEhAtivo(int assetId)
        {
            return (from a in db.Assets
                    where a.Id == assetId
                    select a.Status).FirstOrDefault();
        }

        public string ValidaDataDeTranferencia(string strDataDeTransferencia, List<int> listaAssetsId, int managerUnitId = 0)
        {
            if (strDataDeTransferencia == null || strDataDeTransferencia.Trim() == string.Empty)
            {
                return "O campo Data de Movimento é obrigatório";
            }

            string _mensagemRetorno = "";

            DateTime dataDeTransferencia = Convert.ToDateTime(strDataDeTransferencia);
            DateTime dataMaisAtualMovimentacaoPorAssets = RecuperaDataMaisAtualMovimentacaoPorAssets(listaAssetsId);

            string anoMesTransferencia = dataDeTransferencia.Year.ToString().PadLeft(4, '0') + dataDeTransferencia.Month.ToString().PadLeft(2, '0');
            string anoMesReferenciaFechamentoDaUGE = RecuperaAnoMesReferenciaFechamento(managerUnitId);

            if (dataDeTransferencia.Date > DateTime.Now.Date)
            {
                return "Por favor, informe uma data de transferência igual ou inferior a data atual.";
            }

            if (anoMesReferenciaFechamentoDaUGE != anoMesTransferencia)
            {
                _mensagemRetorno = "Por favor, informe uma data de transferência que corresponda ao mês/ano de fechamento " + anoMesReferenciaFechamentoDaUGE.Substring(4).PadLeft(2, '0') + "/" + anoMesReferenciaFechamentoDaUGE.Substring(0, 4).PadLeft(4, '0') + ".";
                return _mensagemRetorno;
            }

            if (dataMaisAtualMovimentacaoPorAssets.Date > dataDeTransferencia)
            {
                _mensagemRetorno = "Existem itens para a movimentação cuja última movimentação foi em " + dataMaisAtualMovimentacaoPorAssets.Day + "/" + dataMaisAtualMovimentacaoPorAssets.Month + "/" + dataMaisAtualMovimentacaoPorAssets.Year + ". Por favor, informe uma data de transferência igual ou superior.";
                return _mensagemRetorno;
            }

            return _mensagemRetorno;
        }

        public string ValidaMesRefUGEOrigemIgualAUGEDestino(MovimentoViewModel movimentoViewModel)
        {
            string _mensagemRetorno = "";

            //if temporário, pois o Fundo Social (o único a receber inservível, não está implantado)
            if ((movimentoViewModel.MovementTypeId != (int)EnumMovimentType.MovSaidaInservivelUGEDoacao) && (movimentoViewModel.MovementTypeId != (int)EnumMovimentType.MovSaidaInservivelUGETransferencia))
            {
                if (ValidaMesAnoReferenciaDiferenteDaUGEDestinataria(movimentoViewModel.ManagerUnitId, movimentoViewModel.ManagerUnitIdDestino))
                {
                    var managerUnitRemetente = RetornaUGE(movimentoViewModel.ManagerUnitId);
                    var anoRemetente = managerUnitRemetente.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
                    var mesRemetente = managerUnitRemetente.ManagmentUnit_YearMonthReference.Substring(4).PadLeft(2, '0');

                    var managerUnitDestinatario = RetornaUGE(movimentoViewModel.ManagerUnitIdDestino);
                    var anoDestinatario = managerUnitDestinatario.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
                    var mesDestinatario = managerUnitDestinatario.ManagmentUnit_YearMonthReference.Substring(4).PadLeft(2, '0');

                    _mensagemRetorno = $"O mês de referência da UGE \"{managerUnitRemetente.Description})\" - ({mesRemetente + "/" + anoRemetente}) se encontra maior que o mês de referência da UGE de destino \"{managerUnitDestinatario.Description}\" - ({mesDestinatario + "/" + anoDestinatario }), por favor, altere a data de movimento.";

                    return _mensagemRetorno;
                }
            }

            return _mensagemRetorno;
        }
        private string RecuperaAnoMesReferenciaFechamento(int _managerUnitId)
        {
            return (from m in db.ManagerUnits
                    where m.Id == _managerUnitId
                    select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
        }

        private bool InvalidaSemLoginSiafem(int TipoMoviemento, string LoginSiafem, string SenhaSiafem, int IdUGE)
        {
            bool NaoDigitouLoginSiafem = false;
            bool UGEIntegradaComSiafem = false;

            int[] movimentoComLoginSiafem = new int[] {
                (int)EnumMovimentType.IncorpAnimaisPesquisaSememPeixe,
                (int)EnumMovimentType.IncorpComodatoConcedidoBensMoveis,
                (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos,
                (int)EnumMovimentType.IncorpConfiscoBensMoveis,
                (int)EnumMovimentType.IncorpDoacaoConsolidacao,
                (int)EnumMovimentType.IncorpDoacaoIntraNoEstado,
                (int)EnumMovimentType.IncorpDoacaoMunicipio,
                (int)EnumMovimentType.IncorpDoacaoOutrosEstados,
                (int)EnumMovimentType.IncorpDoacaoUniao,
                (int)EnumMovimentType.IncorpVegetal,
                (int)EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao,
                (int)EnumMovimentType.IncorpNascimentoDeAnimais,
                (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao,
                (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia,
                (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado,
                (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado,
                (int)EnumMovimentType.MovSaidaInservivelUGEDoacao,
                (int)EnumMovimentType.MovSaidaInservivelUGETransferencia,
                (int)EnumMovimentType.MovComodatoConcedidoBensMoveis,
                (int)EnumMovimentType.MovComodatoTerceirosRecebidos,
                (int)EnumMovimentType.MovDoacaoConsolidacao,
                (int)EnumMovimentType.MovDoacaoIntraNoEstado,
                (int)EnumMovimentType.MovDoacaoMunicipio,
                (int)EnumMovimentType.MovDoacaoOutrosEstados,
                (int)EnumMovimentType.MovDoacaoUniao,
                (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis,
                (int)EnumMovimentType.MovMorteAnimalPatrimoniado,
                (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao,
                (int)EnumMovimentType.MovSementesPlantasInsumosArvores,
                (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado,
                (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado,
                (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis,
                (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis,
                (int)EnumMovimentType.MovInservivelNaUGE,
                (int)EnumMovimentType.MovVendaLeilaoSemoventes,
            };

            if (movimentoComLoginSiafem.Contains(TipoMoviemento))
            {

                if (string.IsNullOrEmpty(LoginSiafem) ||
                   string.IsNullOrWhiteSpace(LoginSiafem) ||
                   string.IsNullOrEmpty(SenhaSiafem) ||
                   string.IsNullOrWhiteSpace(SenhaSiafem))
                {
                    NaoDigitouLoginSiafem = true;
                }
                else
                {
                    NaoDigitouLoginSiafem = LoginSiafem.Length != 11;
                }

                var UGE = (from m in db.ManagerUnits
                           where m.Id == IdUGE
                           select m).FirstOrDefault();

                if (UGE == null)
                {
                    UGEIntegradaComSiafem = false;
                    NaoDigitouLoginSiafem = false;
                }
                else
                {
                    var UGEIntegrada = (from i in db.Institutions
                                        join b in db.BudgetUnits on i.Id equals b.InstitutionId
                                        join m in db.ManagerUnits on b.Id equals m.BudgetUnitId
                                        where m.Id == IdUGE
                                        select i.flagIntegracaoSiafem || m.FlagIntegracaoSiafem).FirstOrDefault();

                    if (UGEIntegrada)
                    {
                        UGEIntegradaComSiafem = true;
                    }
                    else {
                        UGEIntegradaComSiafem = false;
                        NaoDigitouLoginSiafem = false;
                    }

                }
            }
            else
            {
                UGEIntegradaComSiafem = false;
                NaoDigitouLoginSiafem = false;
            }

            return UGEIntegradaComSiafem && NaoDigitouLoginSiafem;
        }

        private bool MovimentacaoEmLoteComUnicaContaContabil(List<int> idsDosBPs) {
            var listaIdsContasContabeis = (from a in db.Assets
                                           join am in db.AssetMovements
                                           on a.Id equals am.AssetId
                                           where idsDosBPs.Contains(a.Id)
                                           && am.Status == true
                                           select am.AuxiliaryAccountId).Distinct();

            var listaBookAccountContasContabeis = (from a in db.AuxiliaryAccounts
                                                   where listaIdsContasContabeis.Contains(a.Id)
                                                   select a.BookAccount).Distinct();

            return listaBookAccountContasContabeis.Count() == 1;
        }

        private bool AlgumBPPrecisaRealizarReclassificacaoContabil(List<int> idsDosBPs) {
            return (from rc in db.BPsARealizaremReclassificacaoContabeis
                    where idsDosBPs.Contains(rc.AssetId)
                    select rc.Id).Any();
        }

        private bool MesmaGestaoDoFundoSocial(int institutionId) {
            string gestao = (from i in db.Institutions
                             where i.Id == institutionId
                             select i.ManagerCode).FirstOrDefault();

            string gestaoFundoSocial = (from i in db.Institutions
                                        where i.Code == "51004"
                                        select i.ManagerCode).FirstOrDefault();

            return gestao == gestaoFundoSocial;
        }

        private bool BPsNaContaContabilInservivel(List<int> idsDosBPs) {
            var listaIdsContasContabeis = (from a in db.Assets
                                           join am in db.AssetMovements
                                           on a.Id equals am.AssetId
                                           where idsDosBPs.Contains(a.Id)
                                           && am.Status == true
                                           select am.AuxiliaryAccountId).Distinct();

            var listaOutrasContasContabeis = (from a in db.AuxiliaryAccounts
                                              where listaIdsContasContabeis.Contains(a.Id)
                                              && a.BookAccount != 123110805
                                              select a.BookAccount).Distinct();

            return listaOutrasContasContabeis.Count() == 0;
        }

        private int IdUGEFundoSocial() {
            return (from i in db.Institutions
                    join b in db.BudgetUnits on i.Id equals b.InstitutionId
                    join m in db.ManagerUnits on b.Id equals m.BudgetUnitId
                    where i.Code == "51004" && m.Code == "510032" && m.Status == true
                    select m.Id).FirstOrDefault();
        }

        private bool BPsSaoTerceiros(List<int> idsDosBPs) {
            return (from a in db.Assets
                    where idsDosBPs.Contains(a.Id)
                    && a.flagTerceiro != true
                    select a.Id).Count() == 0;
        }

        private bool BPTravado(List<int> idsDosBPs)
        {
            return (from a in db.Assets
                    where idsDosBPs.Contains(a.Id)
                       && a.MaterialGroupCode != 88
                    select a.Id).Count() > 0;
        }

        private List<AssetMovements> BPSIndiponiveisParaMovimentar(List<int> idsDosBPs)
        {
            return (from a in db.AssetMovements
                    where idsDosBPs.Contains(a.AssetId)
                    && a.Status == true
                    select a).ToList();
        }



        private DateTime RecuperaDataMaisAtualMovimentacaoPorAssets(List<int> listAssetsId)
        {
            var lstDadosBD = (from a in db.Assets
                              join am in db.AssetMovements
                              on a.Id equals am.AssetId
                              where am.Status == true &&
                                    listAssetsId.Contains(am.AssetId) &&
                                    !a.flagVerificado.HasValue &&
                                    a.flagDepreciaAcumulada == 1
                              select am).AsNoTracking();

            return lstDadosBD.OrderByDescending(y => y.MovimentDate).FirstOrDefault().MovimentDate;
        }
        private bool ValidaMesAnoReferenciaDiferenteDaUGEDestinataria(int managerUnitId, int managerUnitIdDestino)
        {
            if (managerUnitId == managerUnitIdDestino) return false;

            var dataRemetente = RetornaDataUGE(managerUnitId);
            var dataDestinatario = RetornaDataUGE(managerUnitIdDestino);
            return (dataRemetente > dataDestinatario || dataRemetente < dataDestinatario);
        }

        private DateTime RetornaDataUGE(int managerUnitId)
        {
            var UGERemetente = (from m in db.ManagerUnits
                                where m.Id == managerUnitId
                                select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

            var anoRemetente = int.Parse(UGERemetente.Substring(0, 4).PadLeft(4, '0'));
            var mesRemetente = int.Parse(UGERemetente.Substring(4).PadLeft(2, '0'));

            var dataRemetente = new DateTime(anoRemetente, mesRemetente, 1);

            return dataRemetente;
        }
        private ManagerUnit RetornaUGE(int managerUnitId)
        {
            return (from m in db.ManagerUnits
                    where m.Id == managerUnitId
                    select m).FirstOrDefault();
        }
        private bool MovimentoPossuiFlagUGENaoUtilizada(int assetMovementId)
        {
            bool? flagUGENaoUtilizada = (from am in db.AssetMovements
                                         where am.Id == assetMovementId
                                         select am.flagUGENaoUtilizada).FirstOrDefault();

            return flagUGENaoUtilizada == true ? true : false;
        }

        private void verificaImplantacaoUGEAceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {
            bool UGEComoOrgaoNaoImplantado = (from m in db.ManagerUnits
                                              where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                              select m.FlagTratarComoOrgao).FirstOrDefault();

            if (UGEComoOrgaoNaoImplantado)
                aceiteAutomatico(assetMoviment, movimentoViewModel);

        }

        public bool verificaNaoImplantacao(int IdUGEDestino) {
            bool naoImplantado = (from i in db.Institutions
                                  join b in db.BudgetUnits
                                  on i.Id equals b.InstitutionId
                                  join m in db.ManagerUnits
                                  on b.Id equals m.BudgetUnitId
                                  where m.Id == IdUGEDestino
                                  select (!i.flagImplantado || m.FlagTratarComoOrgao)).FirstOrDefault();

            return (naoImplantado);
        }

        private void verificaImplantacaoAceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {
            if (verificaNaoImplantacao((int)assetMoviment.SourceDestiny_ManagerUnitId))
                aceiteAutomatico(assetMoviment, movimentoViewModel);

        }

        private void aceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {

            assetMoviment.Status = false;
            db.Entry(assetMoviment).State = EntityState.Modified;

            Asset _assetAntesDaMovimentacao = db.Assets.Find(assetMoviment.AssetId);
            _assetAntesDaMovimentacao.Status = false;
            db.Entry(_assetAntesDaMovimentacao).State = EntityState.Modified;

            Asset assetAposAMovimentacao = assetParaAceiteAutomatico(_assetAntesDaMovimentacao, assetMoviment);

            db.Entry(assetAposAMovimentacao).State = EntityState.Added;

            db.SaveChanges();

            AssetMovements assetMovimentNaNovaUGE = assetMovementsParaAceiteAutomatico(assetAposAMovimentacao, assetMoviment);

            db.Entry(assetMovimentNaNovaUGE).State = EntityState.Added;

            assetMoviment.AssetTransferenciaId = assetAposAMovimentacao.Id;
            db.Entry(assetMoviment).State = EntityState.Modified;
            db.SaveChanges();


            CopiaValorAquisicaoDeDecreto(_assetAntesDaMovimentacao, assetAposAMovimentacao);
        }

        private Asset assetParaAceiteAutomatico(Asset _assetAntesDaMovimentacao, AssetMovements assetMoviment)
        {
            Asset assetAposAMovimentacao = new Asset();

            assetAposAMovimentacao.Status = true;

            assetAposAMovimentacao.InitialId = _assetAntesDaMovimentacao.InitialId;
            assetAposAMovimentacao.InitialName = _assetAntesDaMovimentacao.InitialName;
            assetAposAMovimentacao.NumberIdentification = _assetAntesDaMovimentacao.NumberIdentification;
            assetAposAMovimentacao.DiferenciacaoChapa = _assetAntesDaMovimentacao.DiferenciacaoChapa;
            assetAposAMovimentacao.AcquisitionDate = _assetAntesDaMovimentacao.AcquisitionDate;
            assetAposAMovimentacao.ValueAcquisition = _assetAntesDaMovimentacao.ValueAcquisition;
            assetAposAMovimentacao.MaterialItemCode = _assetAntesDaMovimentacao.MaterialItemCode;
            assetAposAMovimentacao.MaterialItemDescription = _assetAntesDaMovimentacao.MaterialItemDescription;
            assetAposAMovimentacao.MaterialGroupCode = _assetAntesDaMovimentacao.MaterialGroupCode;
            assetAposAMovimentacao.LifeCycle = _assetAntesDaMovimentacao.LifeCycle;


            assetAposAMovimentacao.AceleratedDepreciation = false;
            assetAposAMovimentacao.Empenho = null;
            assetAposAMovimentacao.ShortDescriptionItemId = _assetAntesDaMovimentacao.ShortDescriptionItemId;
            assetAposAMovimentacao.OldNumberIdentification = _assetAntesDaMovimentacao.NumberIdentification;
            assetAposAMovimentacao.DiferenciacaoChapaAntiga = _assetAntesDaMovimentacao.DiferenciacaoChapa;
            assetAposAMovimentacao.NumberDoc = _assetAntesDaMovimentacao.NumberDoc;
            assetAposAMovimentacao.MovimentDate = _assetAntesDaMovimentacao.MovimentDate;
            assetAposAMovimentacao.flagDepreciaAcumulada = 1;
            assetAposAMovimentacao.ResidualValueCalc = _assetAntesDaMovimentacao.ResidualValueCalc;
            assetAposAMovimentacao.flagAcervo = _assetAntesDaMovimentacao.flagAcervo;
            assetAposAMovimentacao.flagDepreciationCompleted = false;
            assetAposAMovimentacao.flagTerceiro = _assetAntesDaMovimentacao.flagTerceiro;
            assetAposAMovimentacao.flagAnimalNaoServico = _assetAntesDaMovimentacao.flagAnimalNaoServico;
            assetAposAMovimentacao.flagVindoDoEstoque = false;
            assetAposAMovimentacao.IndicadorOrigemInventarioInicial = (int)EnumOrigemInventarioInicial.NaoEhInventarioInicial;
            assetAposAMovimentacao.ValorDesdobramento = _assetAntesDaMovimentacao.ValorDesdobramento;

            //Campos que são usados da tabela MontlhyDepreciation na Visão Geral
            assetAposAMovimentacao.ValueUpdate = _assetAntesDaMovimentacao.ValueUpdate;
            assetAposAMovimentacao.MonthUsed = _assetAntesDaMovimentacao.MonthUsed;
            assetAposAMovimentacao.DepreciationByMonth = _assetAntesDaMovimentacao.DepreciationByMonth;
            assetAposAMovimentacao.DepreciationAccumulated = _assetAntesDaMovimentacao.DepreciationAccumulated;
            assetAposAMovimentacao.RateDepreciationMonthly = _assetAntesDaMovimentacao.RateDepreciationMonthly;
            assetAposAMovimentacao.ResidualValue = _assetAntesDaMovimentacao.ResidualValue;

            //Campo usado para consulta na tabela MontlhyDepreciation
            assetAposAMovimentacao.AssetStartId = _assetAntesDaMovimentacao.AssetStartId;

            //Campo com valores pedos no registro de AssetMovement
            assetAposAMovimentacao.ManagerUnitId = Convert.ToInt32(assetMoviment.SourceDestiny_ManagerUnitId);
            assetAposAMovimentacao.Login = assetMoviment.Login;
            assetAposAMovimentacao.DataLogin = assetMoviment.DataLogin;

            switch (assetMoviment.MovementTypeId)
            {
                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpDoacaoIntraNoEstado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia;
                    break;
            }

            return assetAposAMovimentacao;
        }

        private AssetMovements assetMovementsParaAceiteAutomatico(Asset assetAposAMovimentacao, AssetMovements assetMoviment)
        {
            var UOTransferido = (from b in db.BudgetUnits
                                 join m in db.ManagerUnits
                                 on b.Id equals m.BudgetUnitId
                                 where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                 select b).FirstOrDefault();

            AssetMovements assetMovimentNaNovaUGE = new AssetMovements();

            assetMovimentNaNovaUGE.AssetId = assetAposAMovimentacao.Id;
            assetMovimentNaNovaUGE.Status = true;
            assetMovimentNaNovaUGE.MovimentDate = assetMoviment.MovimentDate;
            assetMovimentNaNovaUGE.StateConservationId = assetMoviment.StateConservationId;
            assetMovimentNaNovaUGE.InstitutionId = UOTransferido.InstitutionId;
            assetMovimentNaNovaUGE.BudgetUnitId = UOTransferido.Id;
            assetMovimentNaNovaUGE.ManagerUnitId = Convert.ToInt32(assetMoviment.SourceDestiny_ManagerUnitId);
            assetMovimentNaNovaUGE.ResponsibleId = null;
            assetMovimentNaNovaUGE.AuxiliaryAccountId = assetMoviment.AuxiliaryAccountId;
            assetMovimentNaNovaUGE.Login = assetMoviment.Login;
            assetMovimentNaNovaUGE.DataLogin = assetMoviment.DataLogin;

            switch (assetMoviment.MovementTypeId)
            {
                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpDoacaoIntraNoEstado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia;
                    break;
            }

            return assetMovimentNaNovaUGE;
        }

        private void CopiaValorAquisicaoDeDecreto(Asset antigo, Asset novo)
        {
            if (antigo.flagDecreto == true)
            {

                var lista = db.HistoricoValoresDecretos
                              .Where(h => h.AssetId == antigo.Id)
                              .ToList();

                HistoricoValoresDecreto salvar;
                foreach (HistoricoValoresDecreto historico in lista)
                {
                    salvar = new HistoricoValoresDecreto();
                    salvar.AssetId = novo.Id;
                    salvar.ValorAquisicao = historico.ValorAquisicao;
                    salvar.ValorRevalorizacao = historico.ValorRevalorizacao;
                    salvar.DataAlteracao = historico.DataAlteracao;
                    salvar.LoginId = UserCommon.CurrentUser().Id;
                    db.Entry(salvar).State = EntityState.Added;
                }
                db.SaveChanges();
            }

        }

        private bool BPsSemPendenciaDeNL(List<int> idsDosBPs, int IdDaUGE) {
            int MesRefInicioIntegracao = db.ManagerUnits.Find(IdDaUGE).MesRefInicioIntegracaoSIAFEM;

            int ano = Convert.ToInt32(MesRefInicioIntegracao.ToString().Substring(0,4));
            int mes = Convert.ToInt32(MesRefInicioIntegracao.ToString().Substring(4, 2));

            DateTime inicioIntegracao = new DateTime(ano, mes, 1);

            var listaIdsDasUltimasMovimentacoes = (from a in db.Assets
                                                   join am in db.AssetMovements
                                                   on a.Id equals am.AssetId
                                                   where idsDosBPs.Contains(a.Id)
                                                   && am.Status == true
                                                   && am.DataLogin >= inicioIntegracao
                                                   select am.Id).Distinct().ToList();

            var listaIdsAuditoria = (from r in db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                     where listaIdsDasUltimasMovimentacoes.Contains(r.AssetMovementsId)
                                     select r.AuditoriaIntegracaoId).Distinct().ToList();

            if (listaIdsAuditoria.Count == 0)
            {
                return false;
            }
            else {
                var possuiPendencias = (from n in db.NotaLancamentoPendenteSIAFEMs
                                        where listaIdsAuditoria.Contains(n.AuditoriaIntegracaoId)
                                        && n.StatusPendencia == 1
                                        select n).Count() > 0;

                return possuiPendencias;
            }
        }

        #endregion

        #region Dados SIAFEM

        [HttpPost]
        public ActionResult Prosseguir(string auditorias, string LoginSiafem, string SenhaSiafem)
        {
            string msgInformeAoUsuario = "Retorno vazio";
            using (db = new MovimentoContext())
            {
                MovementType tipoMovPatrimonial = null;
                int numeroNLs = 1;
                string nlLiquidacao = null;
                string nlDepreciacao = null;
                string nlReclassificacao = null;
                IList<Tuple<string, string>> listaNLsGeradas = null;
                string msgErroSIAFEM = null;
                string msgNLGerada = null;
                Tuple<string, string, string, string, bool> envioComandoGeracaoNL = null;
                TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.Desconhecido;
                string cpfUsuarioSessaoLogada = null;
                bool exibeBotaoAbortar = false;
                bool exibeBotaoGerarPendencia = false;
                int contadorNL = 0;
                bool ehEstorno = false;
                AuditoriaIntegracao registroAuditoriaIntegracao = null;



                List<int> IdsAuditorias = null;
                if (auditorias[auditorias.Length - 1] == ',')
                    IdsAuditorias = auditorias.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
                else
                    IdsAuditorias = auditorias.Split(',').Select(Int32.Parse).ToList();


                IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();
                var ListaMovimentacoes = (from r in db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                          join am in db.AssetMovements on r.AssetMovementsId equals am.Id
                                          where IdsAuditorias.Contains(r.AuditoriaIntegracaoId)
                                          select am).Distinct().ToList();

                var PrimeiraMovimentacao = ListaMovimentacoes.FirstOrDefault();
                var movementTypeId = PrimeiraMovimentacao.MovementTypeId;
                var NumberDoc = PrimeiraMovimentacao.NumberDoc;
                tipoMovPatrimonial = db.MovementTypes.Where(movPatrimonial => movPatrimonial.Id == movementTypeId).FirstOrDefault();
                if (IdsAuditorias.HasElements())
                {
                    var usuarioLogado = UserCommon.CurrentUser();
                    var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                    cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);
                    numeroNLs = IdsAuditorias.Count();
                    listaNLsGeradas = new List<Tuple<string, string>>();


                    foreach (var auditoriaIntegracaoId in IdsAuditorias)
                    {
                        //tipoNotaSIAFEM = ((tipoNotaSIAFEM != TipoNotaSIAF.Desconhecido) ? tipoNotaSIAFEM : obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId));
                        tipoNotaSIAFEM = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);
                        registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                        ehEstorno = (registroAuditoriaIntegracao.NLEstorno.ToUpperInvariant() == "S");


                        #region NL Liquidacao
                        //NL Liquidacao
                        if (tipoMovPatrimonial.ContraPartidaContabilLiquida() && tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Liquidacao)
                        {
                            ++contadorNL;
                            envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, LoginSiafem, SenhaSiafem, ehEstorno, tipoNotaSIAFEM);
                            if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                            {
                                nlLiquidacao = envioComandoGeracaoNL.Item3; //captura a NL
                                listaNLsGeradas.Add(Tuple.Create("(liquidação)", nlLiquidacao));
                                this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                                continue; //segue o loop
                            }
                            else
                            {
                                msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                break; //interrompe o loop
                            }
                        }
                        #endregion NL Liquidacao


                        #region NL Depreciacao
                        //NL Depreciacao
                        ++contadorNL;
                        var tipoMovimentacaoNaoFazLiquidacao = (String.IsNullOrWhiteSpace(nlLiquidacao) && !tipoMovPatrimonial.ContraPartidaContabilLiquida());
                        var temNL_Liquidacao = !String.IsNullOrWhiteSpace(nlLiquidacao);

                        //if ((((numeroNLs > 1) && temNL_Liquidacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                        //    tipoMovPatrimonial.ContraPartidaContabilDeprecia()) //...e deprecia?
                       if (((numeroNLs > 1) && tipoMovPatrimonial.ContraPartidaContabilDeprecia()) && tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Depreciacao)
                        {
                            envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, LoginSiafem, SenhaSiafem, ehEstorno, tipoNotaSIAFEM);
                            if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                            {
                                nlDepreciacao = envioComandoGeracaoNL.Item3; //captura a NL
                                listaNLsGeradas.Add(Tuple.Create("(depreciação)", nlDepreciacao));
                                this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                                continue; //segue o loop
                            }
                            else
                            {
                                msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                break; //interrompe o loop
                            }
                        }
                        #endregion NL Depreciacao


                        #region NL Reclassificacao
                        //NL Reclassificacao
                        ++contadorNL;
                        var temNL_Depreciacao = !String.IsNullOrWhiteSpace(nlDepreciacao);
                        //if ((((numeroNLs > 1) && temNL_Depreciacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                        //    tipoMovPatrimonial.ContraPartidaContabilReclassifica()) //...e reclassifica?
                        if (((numeroNLs > 1) && tipoMovPatrimonial.ContraPartidaContabilReclassifica()) && tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Reclassificacao)
                        {
                            envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, LoginSiafem, SenhaSiafem, ehEstorno, tipoNotaSIAFEM);
                            if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                            {
                                nlReclassificacao = envioComandoGeracaoNL.Item3; //captura a NL
                                listaNLsGeradas.Add(Tuple.Create("(reclassificação)", nlReclassificacao));
                                this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                                continue; //segue o loop
                            }
                            else
                            {
                                msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                break; //interrompe o loop
                            }
                        }
                        #endregion NL Reclassificacao
                    }
                }

                //formatacao mensagem de erro
                msgErroSIAFEM = ((!String.IsNullOrWhiteSpace(msgErroSIAFEM)) ? String.Format("Erro retornado pela integração SAM/Contabiliza-SP: {0}", msgErroSIAFEM) : null);

                //formatacao mensagem de exibicao de NL's
                if (listaNLsGeradas.Count() == 1)
                {
                    msgNLGerada = String.Format("NL retornada pela integração SAM/Contabiliza-SP: {0} {1}", listaNLsGeradas[0].Item2, listaNLsGeradas[0].Item1);
                }
                else if (listaNLsGeradas.Count() > 1)
                {
                    bool jahTemNL = false;
                    foreach (var nLGerada in listaNLsGeradas)
                    {
                        jahTemNL = !String.IsNullOrWhiteSpace(msgNLGerada);
                        msgNLGerada += String.Format("{0}{1} {2}", (jahTemNL ? " e " : null), nLGerada.Item2, nLGerada.Item1);
                    }

                    msgNLGerada = String.Format("NLs retornadas pela integração SAM/Contabiliza-SP: {0}.", msgNLGerada);
                }

                DadosSIAFEMValidacaoMovimentacoes praTela = new DadosSIAFEMValidacaoMovimentacoes();

                //sucesso OU erro?
                msgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgNLGerada) ? msgNLGerada : msgErroSIAFEM);
                //sucesso E erro?
                if (!String.IsNullOrWhiteSpace(msgNLGerada) && !String.IsNullOrWhiteSpace(msgErroSIAFEM))
                    msgInformeAoUsuario = String.Format("Retorno de processamento: {0}) {1}", msgNLGerada, msgErroSIAFEM);

                //praTela.mensagem = (!String.IsNullOrWhiteSpace(msgNLGerada) ? msgNLGerada : msgErroSIAFEM);
                praTela.mensagem = msgInformeAoUsuario;
                praTela.mensagemDeSuccesso = (!String.IsNullOrWhiteSpace(msgNLGerada) ? true : false);

                //AQUI
                exibeBotaoAbortar = (contadorNL < numeroNLs); //se na primeira jah parar (isso tendo gerado dois XMLs)
                exibeBotaoGerarPendencia = (contadorNL == numeroNLs); //se a ultima NL das possiveis nao for gerada
                praTela.primeiraNLGeraPendecia = exibeBotaoAbortar;

                return PartialView("_RetornoMovimentoSIAFEM", praTela);
            }
        }


        [HttpPost]
        public void Abortar(string auditorias)
        {
            using (db = new MovimentoContext())
            {

                List<int> IdsAuditorias = null;

                if (auditorias[auditorias.Length - 1] == ',')
                    IdsAuditorias = auditorias.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
                else
                    IdsAuditorias = auditorias.Split(',').Select(Int32.Parse).ToList();

                IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();

                var ListaMovimentacoes = (from r in db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                          join am in db.AssetMovements on r.AssetMovementsId equals am.Id
                                          where IdsAuditorias.Contains(r.AuditoriaIntegracaoId)
                                          select am).Distinct().ToList();

                var movimentTypeId = ListaMovimentacoes.FirstOrDefault().MovementTypeId;
                var _user = UserCommon.CurrentUser();

                foreach (var movimentacao in ListaMovimentacoes)
                {
                    int assetId = movimentacao.AssetId;
                    int assetMovementId = movimentacao.Id;

                    if (AssetEhAtivo(assetId) == true)
                    {
                        EstornaMovimento(assetId, assetMovementId, _user.Id);
                    }
                    else
                    {
                        if (movimentTypeId == (int)EnumMovimentType.VoltaConserto ||
                            movimentTypeId == (int)EnumMovimentType.SaidaConserto ||
                            movimentTypeId == (int)EnumMovimentType.Extravio ||
                            movimentTypeId == (int)EnumMovimentType.Obsoleto ||
                            movimentTypeId == (int)EnumMovimentType.Danificado ||
                            movimentTypeId == (int)EnumMovimentType.Sucata ||
                            movimentTypeId == (int)EnumMovimentType.MovComodatoTerceirosRecebidos ||
                            movimentTypeId == (int)EnumMovimentType.MovDoacaoConsolidacao ||
                            movimentTypeId == (int)EnumMovimentType.MovDoacaoMunicipio ||
                            movimentTypeId == (int)EnumMovimentType.MovDoacaoOutrosEstados ||
                            movimentTypeId == (int)EnumMovimentType.MovDoacaoUniao ||
                            movimentTypeId == (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis ||
                            movimentTypeId == (int)EnumMovimentType.MovMorteAnimalPatrimoniado ||
                            movimentTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis ||
                            movimentTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis ||
                            movimentTypeId == (int)EnumMovimentType.MovSementesPlantasInsumosArvores ||
                            movimentTypeId == (int)EnumMovimentType.MovVendaLeilaoSemoventes)
                        {
                            EstornaMovimento(assetId, assetMovementId, _user.Id);
                            ReativarAsset(assetId);

                            if (movimentTypeId == (int)EnumMovimentType.MovComodatoTerceirosRecebidos)
                            {
                                ValidaReativacaoAssetEstornoComodatoTerceiro(assetId);
                            }

                        }

                        else if (movimentTypeId == (int)EnumMovimentType.Transferencia ||
                                 movimentTypeId == (int)EnumMovimentType.Doacao ||
                                 movimentTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                 movimentTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                                 movimentTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado)
                        {
                            //BPs que estejam inativos pelo fato de serem transferidos para UGEs inativas, podem ser estornados.
                            bool flagUGENaoUtilizada = movimentacao.flagUGENaoUtilizada == true ? true : false;

                            if (flagUGENaoUtilizada == true)
                            {
                                EstornaMovimento(assetId, assetMovementId, _user.Id);
                                ReativarAsset(assetId);
                            }
                            else
                            {
                                int IdUGEDestino = (int)movimentacao.SourceDestiny_ManagerUnitId;
                                EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);

                                if (verificaNaoImplantacao(IdUGEDestino))
                                {
                                    EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);
                                }
                            }
                        }
                        else if (movimentTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
                                 movimentTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao)
                        {
                            //BPs que estejam inativos pelo fato de serem transferidos para UGEs inativas, podem ser estornados.
                            bool flagUGENaoUtilizada = MovimentoPossuiFlagUGENaoUtilizada(assetMovementId);

                            if (flagUGENaoUtilizada == true)
                            {

                                EstornaMovimento(assetId, assetMovementId, _user.Id);
                                ReativarAsset(assetId);
                            }
                            else
                            {
                                int IdUGEDestino = (int)movimentacao.SourceDestiny_ManagerUnitId;
                                if (verificaNaoImplantacao(IdUGEDestino))
                                {
                                    EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);
                                }
                                else
                                {
                                    EstornaMovimento(assetId, assetMovementId, _user.Id);
                                    ReativarAsset(assetId);
                                }
                            }
                        }
                    }
                }

                if (IdsAuditorias.HasElements())
                {
                    foreach (var auditoriaIntegracaoId in IdsAuditorias)
                    {
                        this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                        this.excluiAuditoriaIntegracao(auditoriaIntegracaoId);
                    }
                }
            }
        }

        [HttpPost]
        public void AbortarIncorporacao(string auditorias)
        {
            List<int> IdsAuditorias = null;


            if (auditorias[auditorias.Length - 1] == ',')
                IdsAuditorias = auditorias.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
            else
                IdsAuditorias = auditorias.Split(',').Select(Int32.Parse).ToList();

            IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();
            db = new MovimentoContext();

            var ListaMovimentacoes = (from r in db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                      join am in db.AssetMovements on r.AssetMovementsId equals am.Id
                                      where IdsAuditorias.Contains(r.AuditoriaIntegracaoId)
                                      select am).ToList();

            var movimentTypeId = ListaMovimentacoes.FirstOrDefault().MovementTypeId;
            var _user = UserCommon.CurrentUser();

                
            foreach (var movimentacao in ListaMovimentacoes)
            {
            int assetId = movimentacao.AssetId;
            int assetMovementId = movimentacao.Id;

            bool EhManual = (movimentTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                             movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
                             movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado
                             ) && movimentacao.SourceDestiny_ManagerUnitId != null;

                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                        {
                            if (AssetEhAtivo(assetId) == true)
                            {
                                if (movimentTypeId == (int)EnumMovimentType.Transferencia ||
                                    movimentTypeId == (int)EnumMovimentType.Doacao ||
                                    movimentTypeId == (int)EnumMovimentType.IncorporacaoPorTransferencia ||
                                    movimentTypeId == (int)EnumMovimentType.IncorporacaoPorDoacao ||
                                    movimentTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                                    movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
                                    movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado)
                                {
                                    //Só o modo da incorporação não for manual (ou não tiver), ele reativa o BP antigo
                                    if (!EhManual)
                                    {
                                        ReativarMovimentoEAssetDeOutraUGE(assetId);
                                    }
                                }
                                else if (movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                                         movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
                                {
                                    if (!EhManual)
                                    {
                                        RetiraReferenciaNaOrigem(assetId);
                                    }
                                }

                                int? IdUGEDestino = movimentacao.SourceDestiny_ManagerUnitId;

                                if (IdUGEDestino == null || EhManual)
                                {
                                    RegistraBPExcluido(assetId, assetMovementId, _user.Id);
                                }
                                else
                                {
                                    if (verificaNaoImplantacao((int)IdUGEDestino))
                                    {
                                        EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);
                                    }
                                    else
                                    {
                                        RegistraBPExcluido(assetId, assetMovementId, _user.Id);
                                    }
                                }

                            }
                            transaction.Complete();
                        }
                }

            if (IdsAuditorias.HasElements())
            {
                foreach (var auditoriaIntegracaoId in IdsAuditorias)
                {
                    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                    this.excluiAuditoriaIntegracao(auditoriaIntegracaoId);
                }
            }
        }

        [HttpPost]
        public void GerarPendenciaSIAFEM(string auditorias, string LoginSiafem, string SenhaSiafem)
        {
            List<int> IdsAuditorias = null;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;



            if (auditorias[auditorias.Length - 1] == ',')
                IdsAuditorias = auditorias.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
            else
                IdsAuditorias = auditorias.Split(',').Select(Int32.Parse).ToList();

            IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracaoId => auditoriaIntegracaoId != 0).ToList();
            foreach (var auditoriaIntegracaoId in IdsAuditorias)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                if (String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.NotaLancamento) && (!String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.MsgErro)))
                {
                    if (!this.existeNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId))
                        this.geraNotaLancamentoPendencia(auditoriaIntegracaoId);
                    else
                        this.atualizaMensagemErro_NotaLancamentoPendencia(auditoriaIntegracaoId, registroAuditoriaIntegracao.MsgErro);
                }
            }
        }


        #region Outros Metodos SIAFEM
        /// <summary>
        /// Método para excluir registro AuditoriaIntegracao e as amarracoes existentes na tabela 'Relacionamento__Asset_AssetMovements_AuditoriaIntegracao'
        /// </summary>
        /// <param name="auditoriaIntegracaoId"></param>
        /// <returns></returns>
        private bool excluiAuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            bool auditoriaIntegracaoFoiExcluida = false;
            int numeroRegistrosManipulados = 0;
            AuditoriaIntegracao registroAuditoriaIntegracaoTemporaria = null;



            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    var registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                              .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                              .ToList();


                    registroAuditoriaIntegracaoTemporaria = contextoCamadaDados.AuditoriaIntegracoes
                                                                               .Where(_registroAuditoriaIntegracao => _registroAuditoriaIntegracao.Id == auditoriaIntegracaoId)
                                                                               .FirstOrDefault();

                    if (registrosDeAmarracao.HasElements())
                    {
                        contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.RemoveRange(registrosDeAmarracao);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();

                        if (registroAuditoriaIntegracaoTemporaria.IsNotNull())
                        {
                            contextoCamadaDados.AuditoriaIntegracoes.Remove(registroAuditoriaIntegracaoTemporaria);
                            numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                        }
                    }
                }
            }


            auditoriaIntegracaoFoiExcluida = (numeroRegistrosManipulados > 0);
            return auditoriaIntegracaoFoiExcluida;
        }
        private bool excluiNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId, bool efetuaDesvinculacaoMovimentacaoPatrimonial = true)
        {
            bool nlFoiExcluida = false;
            int numeroRegistrosManipulados = 0;
            NotaLancamentoPendenteSIAFEM pendenciaNL = null;
            IList<int> listaMovimentacaoPatrimonialId = null;


            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                     .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                     .FirstOrDefault();


                    if (pendenciaNL.IsNotNull())
                    {
                        contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Remove(pendenciaNL);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                    }

                    if (efetuaDesvinculacaoMovimentacaoPatrimonial)
                    {
                        listaMovimentacaoPatrimonialId = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                            .Where(registroDeAmarracao => registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                            .Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId)
                                                                            .ToList();
                        if (listaMovimentacaoPatrimonialId.HasElements())
                        {
                            var listaMovimentacaoPatrimonial = contextoCamadaDados.AssetMovements
                                                                                  .Where(_movPatrimonial => listaMovimentacaoPatrimonialId.Contains(_movPatrimonial.Id))
                                                                                  .ToList();
                            if (listaMovimentacaoPatrimonial.HasElements())
                            {
                                foreach (var movimentacaoPatrimonial in listaMovimentacaoPatrimonial)
                                    movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId = null;

                                numeroRegistrosManipulados += contextoCamadaDados.SaveChanges();
                            }
                        }
                    }
                }
            }


            nlFoiExcluida = (numeroRegistrosManipulados > 0);
            return nlFoiExcluida;
        }
        private bool excluiNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId, out string mensagemParaUsuario)
        {
            bool nlFoiInativada = false;
            int numeroRegistrosManipulados = 0;
            IList<int> assetMovementsIds = null;
            string numeroDocumentoSAM = null;
            string siglaBP = null;
            string chapaBP = null;
            decimal valorPendenciaNL = 0.00m;
            NotaLancamentoPendenteSIAFEM pendenciaNL = null;



            mensagemParaUsuario = null;
            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    var registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                  .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                  .ToList();

                    if (registrosDeAmarracao.HasElements() && registrosDeAmarracao.Count() == 1)
                    {
                        assetMovementsIds = registrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).ToList();
                        pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                         .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                         .FirstOrDefault();

                        var movPatrimoniais = contextoCamadaDados.AssetMovements.Where(movPatrimonial => assetMovementsIds.Contains(movPatrimonial.Id)).ToList();
                        movPatrimoniais.ForEach(movPatrimonial => movPatrimonial.NotaLancamentoPendenteSIAFEMId = null);

                        contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.RemoveRange(registrosDeAmarracao);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();

                        contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Remove(pendenciaNL);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                    }
                }

                if (assetMovementsIds.HasElements())
                {
                    if (assetMovementsIds.Count() == 1)
                    {
                        var cultureInfo = new CultureInfo("pt-BR");
                        var movPatrimonial = obterEntidadeAssetMovements(assetMovementsIds.FirstOrDefault());
                        var auditoriaIntegracaoVinculada = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);

                        numeroDocumentoSAM = movPatrimonial.NumberDoc;
                        siglaBP = movPatrimonial.RelatedAssets.InitialName;
                        chapaBP = movPatrimonial.RelatedAssets.NumberIdentification;
                        valorPendenciaNL = auditoriaIntegracaoVinculada.ValorTotal.GetValueOrDefault();
                        mensagemParaUsuario = String.Format(cultureInfo, "A 'Pendencia NL SIAFEM' de documento SAM '{0} de valor {1:C} foi excluída devido ao estorno do BP '{2}'-'{3}'.", numeroDocumentoSAM, valorPendenciaNL, siglaBP, chapaBP);
                    }
                }
            }


            nlFoiInativada = (numeroRegistrosManipulados > 0);
            return nlFoiInativada;
        }
        private AssetMovements obterEntidadeAssetMovements(int assetMovementsId)
        {
            AssetMovements objEntidade = null;


            if (assetMovementsId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AssetMovements.Include("RelatedAssets")
                                                     .Where(movPatrimonial => movPatrimonial.Id == assetMovementsId)
                                                     .FirstOrDefault();
                }

            return objEntidade;
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

        private bool existeNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            bool existeNotaLancamentoPendente = false;

            if (auditoriaIntegracaoId > 0)
            {
                using (_contextoCamadaDados = new SAMContext())
                {
                    existeNotaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                       .Where(pendenciaNL => pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                       .FirstOrDefault()
                                                                       .IsNotNull();
                }
            }


            return existeNotaLancamentoPendente;
        }

        public bool geraNotaLancamentoPendencia(int auditoriaIntegracaoId)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracao = null;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> listadeRegistrosDeAmarracao = null;
            TipoNotaSIAFEM tipoAgrupamentoNotaLancamento = TipoNotaSIAFEM.Desconhecido;
            IList<AssetMovements> listaMovimentacoesPatrimoniais = null;
            string numeroDocumentoSAM = null;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;

            int[] idsMovPatrimoniais = null;

            if (auditoriaIntegracaoId > 0)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);

                if (registroAuditoriaIntegracao.IsNotNull())
                {
                    listadeRegistrosDeAmarracao = _contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                      .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                      .ToList();
                    if (listadeRegistrosDeAmarracao.HasElements())
                        idsMovPatrimoniais = listadeRegistrosDeAmarracao.Select(_registroDeAmarracao => _registroDeAmarracao.AssetMovementsId).ToArray();

                    tipoAgrupamentoNotaLancamento = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);
                    listaMovimentacoesPatrimoniais = obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria(auditoriaIntegracaoId);
                    if (listaMovimentacoesPatrimoniais.HasElements())
                    {
                        registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                        numeroDocumentoSAM = listaMovimentacoesPatrimoniais.FirstOrDefault().NumberDoc;
                        pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
                                                                                        {
                                                                                            AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id,
                                                                                            DataHoraEnvioMsgWS = registroAuditoriaIntegracao.DataEnvio,
                                                                                            ManagerUnitId = registroAuditoriaIntegracao.ManagerUnitId,
                                                                                            TipoNotaPendencia = (short)tipoAgrupamentoNotaLancamento,
                                                                                            NumeroDocumentoSAM = numeroDocumentoSAM,
                                                                                            AssetMovementIds = registroAuditoriaIntegracao.AssetMovementIds,
                                                                                            StatusPendencia = 1,
                                                                                            ErroProcessamentoMsgWS = registroAuditoriaIntegracao.MsgErro,
                                                                                        };


                        idsMovPatrimoniais = ((idsMovPatrimoniais.HasElements()) ? idsMovPatrimoniais : listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray());
                        using (_contextoCamadaDados = new SAMContext())
                        {
                            var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                            var _registroAuditoriaIntegracao = _contextoCamadaDados.AuditoriaIntegracoes.Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id).FirstOrDefault();

                            _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                            numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();

                            _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial =>
                            {
                                movPatrimonial.NotaLancamentoPendenteSIAFEMId = pendenciaNLContabilizaSP.Id;
                                pendenciaNLContabilizaSP.AuditoriaIntegracaoId = _registroAuditoriaIntegracao.Id;

                                if (!listadeRegistrosDeAmarracao.HasElements())
                                {
                                    registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                                    registroDeAmarracao.AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id;
                                    registroDeAmarracao.AssetMovementsId = movPatrimonial.Id;
                                    registroDeAmarracao.AssetId = movPatrimonial.AssetId;

                                    _contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Add(registroDeAmarracao);
                                }
                            });
                            //TODO DCBATISTA ACOMPANHAR ELIMINACAO
                            pendenciaNLContabilizaSP.AssetMovementIds = _registroAuditoriaIntegracao.AssetMovementIds;
                            numeroRegistrosManipulados = +_contextoCamadaDados.SaveChanges();
                        }
                    }
                }
            }


            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }

        private bool atualizaMensagemErro_NotaLancamentoPendencia(int auditoriaIntegracaoId, string msgErro)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM notaLancamentoPendente = null;
            int numeroRegistrosManipulados = 0;
            bool notaLancamentoPendenteAtualizada = false;

            if (auditoriaIntegracaoId > 0)
            {
                using (_contextoCamadaDados = new SAMContext())
                {
                    notaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                 .Where(pendenciaNL => pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                 .FirstOrDefault();

                    if (notaLancamentoPendente.IsNotNull())
                    {
                        notaLancamentoPendente.ErroProcessamentoMsgWS = msgErro;
                        numeroRegistrosManipulados += _contextoCamadaDados.SaveChanges();
                    }
                }
            }

            notaLancamentoPendenteAtualizada = (numeroRegistrosManipulados > 0);
            return notaLancamentoPendenteAtualizada;
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

        private IList<AssetMovements> obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria(int auditoriaIntegracaoId)
        {
            IList<AssetMovements> listaMovPatrimoniais = null;
            AssetMovements movPatrimonial = null;


            try
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    listaMovPatrimoniais = new List<AssetMovements>();
                    var registrosDeAmarracao = new List<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao>();
                    registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                             .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                             .ToList();

                    if (registrosDeAmarracao.HasElements())
                    {
                        foreach (var registroDeAmarracao in registrosDeAmarracao)
                        {
                            movPatrimonial = contextoCamadaDados.AssetMovements
                                                                .Where(_movPatrimonial => _movPatrimonial.Id == registroDeAmarracao.AssetMovementsId)
                                                                .FirstOrDefault();

                            if (movPatrimonial.IsNotNull())
                                listaMovPatrimoniais.Add(movPatrimonial);
                        }

                    }
                }
            }
            catch (Exception excErroRuntime)
            {
                string messageErro = excErroRuntime.Message;
                string stackTrace = excErroRuntime.StackTrace;
                string name = "MovimentoController.obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria";
                GravaLogErro(messageErro, stackTrace, name);
                throw excErroRuntime;
            }

            return listaMovPatrimoniais;
        }
        #endregion



        #endregion
    }
}