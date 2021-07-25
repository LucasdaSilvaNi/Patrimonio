using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.Relatorios;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class RelatoriosController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;
        public string caminhoDosArquivosExcel;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;

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
            }
            else
            {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
            }
        }

        #region Relatório de Itens Inventario
        [HttpGet]
        public ActionResult Inventario()
        {
            try
            {
                SetCarregaHierarquiaResumoInventario();
                ViewBag.Agrupamento = new SelectList(Agrupamento(), "Id", "Description");


                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem { Value = "0", Text = "Selecione" });
                ViewBag.MesRef = new SelectList(data, "Value", "Text", "0");

                var retorno = new RelatorioInventarioFisicoViewModel();
                retorno.checkRelatorio = true;
                retorno.Excel = false;

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult Inventario(RelatorioInventarioFisicoViewModel objViewModel)
        {
            try
            {
                if (objViewModel.InstitutionId != 0 && objViewModel.BudgetUnitId != 0 && objViewModel.ManagerUnitId != 0)
                {
                    objViewModel.AdministrativeUnitId = objViewModel.AdministrativeUnitId == 0 ? null : objViewModel.AdministrativeUnitId;
                    objViewModel.SectionId = objViewModel.SectionId == 0 ? null : objViewModel.SectionId;

                    var dados = new BuscaDadosInventarioFisico(objViewModel);

                    if (objViewModel.Excel)
                        GeraInventarioFisicoEmExcel(objViewModel, dados.dtRetornoDados);
                    else 
                        GeraInventarioFisicoEmPDF(objViewModel, dados.dtRetornoDados);
                }

                SetCarregaHierarquiaResumoInventario(objViewModel.InstitutionId, objViewModel.BudgetUnitId, objViewModel.ManagerUnitId, objViewModel.AdministrativeUnitId);
                ViewBag.Agrupamento = new SelectList(Agrupamento(), "Id", "Description");

                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem { Value = "0", Text = "Selecione" });
                ViewBag.MesRef = new SelectList(data, "Value", "Text", "0");

                objViewModel.checkRelatorio = true;
                objViewModel.Excel = false;

                return View(objViewModel);

            }
            catch (HttpException ex)
            {
                //Código caso usuário feche a página antes de recebe o download
                switch (ex.ErrorCode)
                {
                    case -2147023901: //0x800703E3
                    case -2147024832: //0x80070040
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

        #endregion

        #region Resumo Do Inventário

        [HttpGet]
        public ActionResult ReportResumoInventario()
        {
            try
            {
                var retorno = new ReportResumoInventarioViewModel();
                retorno.ResumoConsolidado = true;
                retorno.Acervos = true;
                retorno.Terceiros = true;
                SetCarregaHierarquiaResumoConsolidadoInventario();

                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem { Value = "0", Text = "Selecione" });
                ViewBag.MesRef = new SelectList(data, "Value", "Text", "0");

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        [HttpPost]
        public ActionResult ReportResumoInventario(ReportResumoInventarioViewModel objReport)
        {
            try
            {
                bool valida = true;

                if (!objReport.Acervos && !objReport.AquisicoesCorrentes && !objReport.BaixasCorrentes
                    && !objReport.BPTotalDepreciados && !objReport.ResumoConsolidado && !objReport.Terceiros)
                {
                    ModelState.AddModelError("ResumoConsolidado", "Por favor, escolha um conteúdo do Resumo");
                    valida = false;
                }

                if (objReport.MesRef == "0")
                {
                    ModelState.AddModelError("MesRef", "Por favor, informe o mês de referência.");
                    valida = false;
                }

                if (objReport.InstitutionId == 0 && objReport.BudgetUnitId == 0 && objReport.ManagerUnitId == 0)
                    valida = false;

                if (valida)
                {
                    using (db = new SAMContext())
                    {
                        DataTable[] dtDadosRelatorio = new BuscaDadosResumoDoInventario(objReport).dtRetornoDados;
                        if (objReport.Excel)
                        {
                            GeraResumoDoInventarioEmExcel(objReport, dtDadosRelatorio);
                        }
                        else
                        {
                            GeraResumoDoInventarioEmPDF(objReport, dtDadosRelatorio);
                        }
                    }

                    return null;
                }

                SetCarregaHierarquiaResumoConsolidadoInventario(objReport.InstitutionId, objReport.BudgetUnitId, objReport.ManagerUnitId);

                using (db = new SAMContext())
                {
                    ViewBag.MesRef = CarregaListaComPeriodosDaUGE(objReport.BudgetUnitId, objReport.ManagerUnitId, objReport.MesRef);
                }

                return View(objReport);
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

        #endregion

        #region Relatório do fechamento

        [HttpGet]
        public ActionResult Fechamento()
        {
            try
            {
                var retorno = new RelatorioViewModel();
                getHierarquiaPerfil();
                CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);

                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem { Value = "0", Text = "Selecione" });
                ViewBag.MesRef = new SelectList(data, "Value", "Text", "0");

                return View(retorno);
            }
            catch (Exception ex)
            {
                //base.GravaLogErro(ex);
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult Fechamento(RelatorioViewModel closingViewModel)
        {
            try
            {
                ModelState.Remove("AdministrativeUnitId");

                if (closingViewModel.MesRef == string.Empty || string.IsNullOrWhiteSpace(closingViewModel.MesRef))
                {
                    ModelState.AddModelError("MesRef", "Favor informar o mês-referencia");
                }
                else
                {
                    int paraValidar;
                    if (closingViewModel.MesRef == "0" || !int.TryParse(closingViewModel.MesRef, out paraValidar))
                    {
                        ModelState.AddModelError("MesRef", "Favor informar o mês-referencia");
                    }
                }

                if (ModelState.IsValid)
                {
                    var dados = new BuscaDadosRelatorioFechamento(closingViewModel).resultado;

                    if (closingViewModel.Excel)
                    {
                        GeraRelatorioFechamentoEmExcel(closingViewModel, dados);
                    }
                    else
                    {
                        GeraRelatorioFechamentoEmPDF(closingViewModel, dados);
                    }
                }

                getHierarquiaPerfil();
                CarregaHierarquiaFiltro(closingViewModel.InstitutionId, closingViewModel.BudgetUnitId, closingViewModel.ManagerUnitId);
                CarregaComboMesReferencia(closingViewModel.BudgetUnitId, closingViewModel.ManagerUnitId, closingViewModel.MesRef);

                return View(closingViewModel);
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
        #endregion

        #region Termo de Responsabilidade
        [HttpGet]
        public ActionResult TermoDeResponsabilidade()
        {
            SetCarregaHierarquiaResumoInventario();
            CarregaResponsaveis(_administrativeUnitId);

            return View();
        }

        [HttpPost]
        public ActionResult TermoDeResponsabilidade(RelatorioTermoDeResponsabilidadeViewModel objViewModel)
        {
            try
            {
                using (db = new SAMContext())
                {
                    if (ModelState.IsValid)
                    {
                        var parametros = new MontaParametroTermoDeResponsabilidade(objViewModel);
                        parametros.BuscaDados(objViewModel);
                        parametros.caminhoRelatorio = PathCompletoDoRelatorio("TermoResp.rdlc");

                        var pdf = new MontaPDFTermoDeResponsabilidade(parametros);

                        Response.Buffer = true;
                        Response.Clear();
                        Response.ContentType = pdf.mimeType;
                        Response.AddHeader("content-disposition", "attachment; filename=RelatorioResponsavel" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                        Response.BinaryWrite(pdf.renderedBytes); 
                        Response.Flush();
                    }

                    SetCarregaHierarquiaResumoInventario(objViewModel.InstitutionId, objViewModel.BudgetUnitId, objViewModel.ManagerUnitId, objViewModel.AdministrativeUnitId, objViewModel.SectionId);

                    CarregaResponsaveis(objViewModel.AdministrativeUnitId);

                    return View(objViewModel);
                }
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
        #endregion    

        #region Relatorio Saldo Contábil Orgão
        [HttpGet]
        public ActionResult SaldoContabilOrgao()
        {
            try
            {
                if ((int)HttpContext.Items["perfilId"] != (int)EnumProfile.AdministradordeOrgao && !PerfilAdmGeral())
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);

                var retorno = new RelatorioViewModel();
                CarregaSelectOrgao();
                retorno.MesRef = "0";
                retorno.Excel = true;

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult SaldoContabilOrgao(RelatorioViewModel objViewModel)
        {
            try
            {
                getHierarquiaPerfil();

                ModelState.Remove("BudgetUnitId");
                ModelState.Remove("ManagerUnitId");
                ModelState.Remove("AdministrativeUnitId");

                if (objViewModel.InstitutionId == 0)
                    ModelState.AddModelError("InstitutionId", "Por favor, informe o Orgão");
                else
                {
                    if (!PerfilAdmGeral() && objViewModel.InstitutionId != _institutionId)
                    {
                        ModelState.AddModelError("InstitutionId", "Ação inválida");
                    }
                }

                if(objViewModel.MesRef == "0")
                    ModelState.AddModelError("MesRef", "Selecione um Mês de Referência");

                if (ModelState.IsValid)
                {
                    var dados = new BuscaDadosSaldoContabilOrgao(objViewModel).resultado;

                    GeraRelatorioSaldoContabilOrgaoEmExcel(dados);
                }

                CarregaSelectOrgao(objViewModel.InstitutionId);
                objViewModel.Excel = true;
                return View(objViewModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Carrega ViewBags
        private void SetCarregaHierarquiaResumoInventario(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int? modelAdministrativeUnitId = 0, int? modelSectionId = 0)
        {

            hierarquia = new Hierarquia();

            if (PerfilAdmGeral() == true)
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

                if (modelSectionId != 0 && modelSectionId.HasValue)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(0), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
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

                if (modelSectionId != 0 && modelSectionId.HasValue)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);

            }
        }

        public List<ItemGenericViewModel> Agrupamento()
        {
            List<ItemGenericViewModel> lstAgrupamento = new List<ItemGenericViewModel>();
            lstAgrupamento.Add(new ItemGenericViewModel() { Id = 0, Description = "Sem Agrupamento" });
            lstAgrupamento.Add(new ItemGenericViewModel() { Id = 1, Description = "Grupo Material" });
            lstAgrupamento.Add(new ItemGenericViewModel() { Id = 2, Description = "Conta Contábil" });

            return lstAgrupamento;
        }

        private void SetCarregaHierarquiaResumoConsolidadoInventario(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int? modelAdministrativeUnitId = 0)
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

                if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(null), "Id", "Description");
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
            }
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

        private void CarregaComboMesReferencia(int modelBudgetUnitId = 0, int modelManagerUnitId = 0, string mesRef = null) {
            List<SelectListItem> data = new List<SelectListItem>();

            data.Add(new SelectListItem { Value = "0", Text = "Selecione" });

            if (modelBudgetUnitId != 0 && modelManagerUnitId != 0)
            {
                ManagerUnit managerUnit;
                using (db = new SAMContext())
                {
                    managerUnit = (from m in db.ManagerUnits
                                   where m.BudgetUnitId == modelBudgetUnitId &&
                                         m.Id == modelManagerUnitId
                                   select new
                                   {
                                        MesRefInicial = m.ManagmentUnit_YearMonthStart,
                                        mesRefAtual = m.ManagmentUnit_YearMonthReference
                                   }).AsNoTracking().ToList()
                                   .Select(m => new ManagerUnit
                                   {
                                       ManagmentUnit_YearMonthStart = m.MesRefInicial,
                                       ManagmentUnit_YearMonthReference = m.mesRefAtual
                                   }).FirstOrDefault();
                }

                if (managerUnit != null)
                {
                    string anoIni = managerUnit.ManagmentUnit_YearMonthStart.Substring(0, 4);
                    string mesIni = managerUnit.ManagmentUnit_YearMonthStart.Substring(4, 2);

                    string anoRefUGE = managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4);
                    string mesRefUGE = managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2);

                    var DataInicialUGE = new DateTime(int.Parse(anoIni), int.Parse(mesIni), 1);
                    var DataRefUGE = new DateTime(int.Parse(anoRefUGE), int.Parse(mesRefUGE), 1);


                    string AnoMesId = managerUnit.ManagmentUnit_YearMonthStart;
                    string AnoMes = string.Concat(mesIni.PadLeft(2, '0'), "/", anoIni);

                    while (DataInicialUGE < DataRefUGE)
                    {
                        AnoMesId = string.Concat(DataInicialUGE.Year.ToString(), DataInicialUGE.Month.ToString().PadLeft(2, '0'));
                        AnoMes = string.Concat(DataInicialUGE.Month.ToString().PadLeft(2, '0'), "/", DataInicialUGE.Year.ToString());
                        data.Add(new SelectListItem { Value = AnoMesId, Text = AnoMes });

                        DataInicialUGE = DataInicialUGE.AddDays(36);
                        DataInicialUGE = new DateTime(DataInicialUGE.Year, DataInicialUGE.Month, 1);
                    }
                }
            }

            ViewBag.MesRef = new SelectList(data, "Value", "Text", mesRef);
        }

        private void CarregaSelectOrgao(int? modelInstitutionId = null)
        {
            hierarquia = new Hierarquia();

            if (modelInstitutionId != null && modelInstitutionId != 0)
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
            }
            else
            {
                getHierarquiaPerfil();
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", _institutionId);
            }
        }

        private void CarregaResponsaveis(int? UA = null) {
            if (UA == null || UA == 0)
            {
                ViewBag.User = new SelectList(new List<Responsible>(), "Id", "Name");
                return;
            }

            using (db = new SAMContext())
            {
                var vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == UA select r).AsNoTracking().ToList();

                if ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.Responsavel)
                {
                    string cpf = HttpContext.Items["CPF"].ToString();
                    vResponsible = vResponsible.Where(r => r.CPF.Equals(cpf)).ToList();
                }

                vResponsible = vResponsible.Distinct().OrderBy(a => a.Name).ToList();

                ViewBag.User = new SelectList(vResponsible, "Id", "Name");
            }
        }
        #endregion

        #region Métodos privados

        private string PathCompletoDoRelatorio(string nomeRelatorio)
        {
            return Path.Combine(Server.MapPath("~/Report"), nomeRelatorio);
        }

        #region Métodos do Inventario Fisico
        public void GeraInventarioFisicoEmExcel(RelatorioInventarioFisicoViewModel objViewModel, DataTable dadosRelatorio) {

            string nomeArquivo = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "Inventario_", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Millisecond.ToString(), ".xlsx");

            ManagerUnit managerUnit;
            using (db = new SAMContext())
            {
                managerUnit = (from m in db.ManagerUnits
                               where m.Id == objViewModel.ManagerUnitId
                               select new { mesRef = m.ManagmentUnit_YearMonthReference, Code = m.Code, Description = m.Description })
                                       .AsNoTracking()
                                       .ToList()
                                       .Select(m => new ManagerUnit { ManagmentUnit_YearMonthReference = m.mesRef, Code = m.Code, Description = m.Description })
                                       .FirstOrDefault();
            }

            string mesRef;
            if (objViewModel.MesRef == "0")
                mesRef = managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2).PadLeft(2, '0') + "/" + managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
            else
                mesRef = objViewModel.MesRef.Substring(4, 2).PadLeft(2, '0') + "/" + objViewModel.MesRef.Substring(0, 4).PadLeft(4, '0');

            string UGE = string.Format("{0} - {1}", managerUnit.Code, managerUnit.Description);

            var excel = new MontaExcelInventarioFisico(objViewModel.Agrupamento, dadosRelatorio, UGE, mesRef);

            Response.Clear();
            byte[] dt = excel.fs.ToArray();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", nomeArquivo));
            Response.BinaryWrite(dt);
            Response.End();
        }

        public void GeraInventarioFisicoEmPDF(RelatorioInventarioFisicoViewModel objViewModel, DataTable dt) {

            string mesRefAtualDaUGE, mesRef;

            if (objViewModel.MesRef == "0")
            {
                using (db = new SAMContext())
                {
                    mesRefAtualDaUGE = db.ManagerUnits.Find(objViewModel.ManagerUnitId).ManagmentUnit_YearMonthReference;
                    mesRef = mesRefAtualDaUGE.Substring(4, 2).PadLeft(2, '0') + "/" + mesRefAtualDaUGE.Substring(0, 4).PadLeft(4, '0');
                }
            }
            else
                mesRef = objViewModel.MesRef.Substring(4, 2).PadLeft(2, '0') + "/" + objViewModel.MesRef.Substring(0, 4).PadLeft(4, '0');

            string path = "";
            switch (objViewModel.Agrupamento)
            {
                case "0":
                    path = PathCompletoDoRelatorio("InventarioBemMovelSemAgrup.rdlc");
                    break;
                case "1":
                    path = PathCompletoDoRelatorio("InventarioBemMovelGrupoMaterial.rdlc");
                    break;
                case "2":
                    path = PathCompletoDoRelatorio("InventarioBemMovelContaAux.rdlc");
                    break;
            }


            string data = DateTime.Now.ToString("yyyyMMddHHmmss");

            var pdf = new MontaPDFInventarioFisico(objViewModel, dt, mesRef, path);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = pdf.mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=RelatorioInventario" + data + ".pdf");
            Response.BinaryWrite(pdf.renderedBytes);
            Response.Flush();
        }

        #endregion

        #region Métodos do Resumo do Inventário

        private void GeraResumoDoInventarioEmPDF(ReportResumoInventarioViewModel objReport, DataTable[] dtDadosRelatorio) {

            string path = PathCompletoDoRelatorio("ResumoConsolidadoInventarioBP.rdlc");

            var managerUnit = db.ManagerUnits
                                 .Where(m => m.Id == objReport.ManagerUnitId)
                                 .Select(m => new {
                                     ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference,
                                     Code = m.Code,
                                     Description = m.Description
                                 })
                                 .ToList()
                                 .Select(m =>
                                 new ManagerUnit {
                                     ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference,
                                     Code = m.Code,
                                     Description = m.Description
                                 }).FirstOrDefault();

             string mesRef;

             if (objReport.MesRef == "0")
                 mesRef =  managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2).PadLeft(2, '0') + "/" + managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
             else
                mesRef = objReport.MesRef.Substring(4, 2).PadLeft(2, '0') + "/" + objReport.MesRef.Substring(0, 4).PadLeft(4, '0');

            string data = DateTime.Now.ToString("yyyyMMddHHmmss");

            var pdf = new MontaPDFResumoDoInventario(objReport, dtDadosRelatorio, string.Format("{0} - {1}", managerUnit.Code, managerUnit.Description), mesRef, path);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = pdf.mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=RelatorioResumoInventario" + data + ".pdf");
            Response.BinaryWrite(pdf.renderedBytes);
            Response.Flush();
        }

        private void GeraResumoDoInventarioEmExcel(ReportResumoInventarioViewModel objReport, DataTable[] dtDadosRelatorio) {

            string nomeArquivo = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "ResumoConsolidadoInventarioBP_", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Millisecond.ToString(), ".xlsx");

            var managerUnit = db.ManagerUnits
                                 .Where(m => m.Id == objReport.ManagerUnitId)
                                 .Select(m => new {
                                     ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference,
                                     Code = m.Code,
                                     Description = m.Description,
                                 })
                                 .ToList()
                                 .Select(m =>
                                 new ManagerUnit
                                 {
                                     ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference,
                                     Code = m.Code,
                                     Description = m.Description,
                                 }).FirstOrDefault();

            string mesRef;
            if (objReport.MesRef == "0")
                mesRef = managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2).PadLeft(2, '0') + "/" + managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
            else
                mesRef = objReport.MesRef.Substring(4, 2).PadLeft(2, '0') + "/" + objReport.MesRef.Substring(0, 4).PadLeft(4, '0');


            string UGE = string.Format("{0} - {1}", managerUnit.Code, managerUnit.Description);
            
            var excel = new MontaExcelResumoDoInventario(objReport, dtDadosRelatorio, UGE, mesRef);

            Response.Clear();
            byte[] dt = excel.fs.ToArray();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", nomeArquivo));
            Response.BinaryWrite(dt);
            Response.End();
        }

        private SelectList CarregaListaComPeriodosDaUGE(int budgetUnitId, int? managerUnitId, string mesRef = null)
        {
            List<SelectListItem> data = new List<SelectListItem>();

            data.Add(new SelectListItem() { Value = "0", Text = "Selecione" });

            if (managerUnitId != null)
            {
                var managerUnit = (from m in db.ManagerUnits
                                   where m.BudgetUnitId == budgetUnitId &&
                                         m.Id == managerUnitId
                                   select m).AsNoTracking().FirstOrDefault();

                if (managerUnit != null)
                {
                    string anoIni = managerUnit.ManagmentUnit_YearMonthStart.Substring(0, 4);
                    string mesIni = managerUnit.ManagmentUnit_YearMonthStart.Substring(4, 2);

                    string anoRefUGE = managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4);
                    string mesRefUGE = managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2);

                    var DataInicialUGE = new DateTime(int.Parse(anoIni), int.Parse(mesIni), 1);
                    var DataRefUGE = new DateTime(int.Parse(anoRefUGE), int.Parse(mesRefUGE), 1);


                    string AnoMesId = managerUnit.ManagmentUnit_YearMonthStart;
                    string AnoMes = string.Concat(mesIni.PadLeft(2, '0'), "/", anoIni);

                    while (DataInicialUGE < DataRefUGE)
                    {
                        AnoMesId = string.Concat(DataInicialUGE.Year.ToString(), DataInicialUGE.Month.ToString().PadLeft(2, '0'));
                        AnoMes = string.Concat(DataInicialUGE.Month.ToString().PadLeft(2, '0'), "/", DataInicialUGE.Year.ToString());
                        data.Add(new SelectListItem() { Value = AnoMesId, Text = AnoMes });

                        DataInicialUGE = DataInicialUGE.AddDays(36);
                        DataInicialUGE = new DateTime(DataInicialUGE.Year, DataInicialUGE.Month, 1);
                    }
                }

            }

            if (!string.IsNullOrEmpty(mesRef) && !string.IsNullOrWhiteSpace(mesRef))
                return new SelectList(data, "Value", "Text", mesRef);
            else
                return new SelectList(data, "Value", "Text", "0");
        }
        #endregion

        #region Métodos do Relatório de Fechamento

        public void GeraRelatorioFechamentoEmExcel(RelatorioViewModel objViewModel, DataTable dadosRelatorio)
        {
            string nomeArquivo = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "RelatoriofechamentoMensal_", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Millisecond.ToString(), ".xlsx");

            ManagerUnit managerUnit;
            using (db = new SAMContext())
            {
                managerUnit = (from a in db.ManagerUnits
                               where a.Id == objViewModel.ManagerUnitId
                               select new { Codigo = a.Code, Descricao = a.Description })
                               .AsNoTracking()
                               .ToList()
                               .Select(m => new ManagerUnit { Code = m.Codigo, Description = m.Descricao })
                               .FirstOrDefault();
            }

            string UGE = string.Format("{0} - {1}", managerUnit.Code, managerUnit.Description);
            string mesRef = string.Format("{0}/{1}", objViewModel.MesRef.Substring(4, 2).PadLeft(2, '0'), objViewModel.MesRef.Substring(0, 4).PadLeft(4, '0'));
            DateTime data = DateTime.Now;

            var excel = new MontaExcelRelatorioFechamento(dadosRelatorio, UGE, mesRef, data);

            Response.Clear();
            byte[] dt = excel.fs.ToArray();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", nomeArquivo));
            Response.BinaryWrite(dt);
            Response.End();
        }

        public void GeraRelatorioFechamentoEmPDF(RelatorioViewModel objViewModel, DataTable dt) {
            string path = PathCompletoDoRelatorio("Fechamento.rdlc");

            ManagerUnit managerUnit;

            using (db = new SAMContext())
            {
                managerUnit = (from a in db.ManagerUnits
                               where a.Id == objViewModel.ManagerUnitId
                               select new { Codigo = a.Code, Descricao = a.Description })
                               .AsNoTracking()
                               .ToList()
                               .Select(m => new ManagerUnit { Code = m.Codigo, Description = m.Descricao })
                               .FirstOrDefault();
            }

            string UGE = string.Format("{0} - {1}", managerUnit.Code, managerUnit.Description);
            string mesRef = string.Format("{0}/{1}", objViewModel.MesRef.Substring(4, 2).PadLeft(2, '0'), objViewModel.MesRef.Substring(0, 4).PadLeft(4, '0'));
            DateTime data = DateTime.Now;

            var pdf = new MontaPDFRelatorioFechamento(dt, UGE, mesRef, path, data);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = pdf.mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=RelatoriofechamentoMensal_" + data + ".pdf");
            Response.BinaryWrite(pdf.renderedBytes);
            Response.Flush();
        }
        #endregion

        #region Saldo Contábil Orgão
        public void GeraRelatorioSaldoContabilOrgaoEmExcel(DataTable dados) {
            var excel = new MontaExcelSaldoContabilOrgao(dados);

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment;filename=BensSaldoContabilOrgao_{0}.xls", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            Response.Output.Write(excel.sw.ToString());
            Response.Flush();
            Response.End();
        }
        #endregion

        #endregion
    }
}