using AutoMapper;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using Sam.Domain.Entity;
using Sam.Integracao.SIAF.Core;
using Sam.Common;
using Sam.Common.Util;




namespace SAM.Web.Controllers
{
    public class ExchangeController : BaseController
    {
        private Hierarquia hierarquia;
        private SAMContext db = new SAMContext();

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;

        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
        }

        #region Index

        #region BolsaOrgao
        public ActionResult BolsaOrgao(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.RetiradaDaBolsa = (int)SAM.Web.Common.Enum.EnumMovimentType.RetiradaDaBolsa;
                ViewBag.CurrentFilter = searchString;
                ViewBag.AlertComSucesso = "";

                if (TempData["msgSucesso"] != null)
                {
                    ViewBag.AlertComSucesso = (string)TempData["msgSucesso"];
                }

                #region Perfil Administrador Geral
                if (PerfilAdmGeral())
                {


                    ViewBag.Permissao = true;

                    ViewBag.NomeOrgao = "";

                    ViewBag.InstitutionId = 0;
                    ViewBag.BudgetUnitId = 0;
                    ViewBag.ManagerUnitId = 0;

                    return View();
                }
                #endregion
                

                ViewBag.Permissao = ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE);

                getHierarquiaPerfil();
                ViewBag.NomeOrgao = (from i in db.Institutions where i.Id == _institutionId select i.Code + " - " + i.Description).FirstOrDefault();

                ViewBag.InstitutionId = _institutionId;
                ViewBag.BudgetUnitId = _budgetUnitId;
                ViewBag.ManagerUnitId = _managerUnitId;

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResultBolsaOrgao(AssetEAssetMovimentViewModel assetEAssetMoviment)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;
            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                IQueryable<BolsaOrgaoViewModel> lstAssetEAssetsMovement;
                #region Perfil Administrador Geral
                if (PerfilAdmGeral() == true)
                {

                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                        lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                                   join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                                   where am.Status == true &&
                                                         !a.flagVerificado.HasValue &&
                                                         a.flagDepreciaAcumulada == 1 &&
                                                         am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                         (a.RelatedInitial.Name.Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemCode.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemDescription.Contains(currentFilter) ||
                                                          am.RelatedInstitution.NameManagerReduced.Contains(currentFilter) ||
                                                          am.RelatedBudgetUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedManagerUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Description.Contains(currentFilter) ||
                                                          am.RelatedResponsible.Name.Contains(currentFilter) ||
                                                          a.RateDepreciationMonthly.ToString().Contains(currentFilter) ||
                                                          a.ValueUpdate.ToString().Contains(currentFilter) ||
                                                          a.LifeCycle.ToString().Contains(currentFilter)
                                                        )
                                                   select new BolsaOrgaoViewModel
                                                   {
                                                       Id = a.Id,
                                                       Sigla = a.InitialName,
                                                       Chapa = a.NumberIdentification,
                                                       Item = a.MaterialItemCode,
                                                       DescricaoDoItem = a.MaterialItemDescription,
                                                       Orgao = am.RelatedInstitution.Code,
                                                       Gestor = am.RelatedInstitution.NameManagerReduced,
                                                       UO = am.RelatedBudgetUnit.Code,
                                                       UGE = am.RelatedManagerUnit.Code,
                                                       InstitutionId = am.RelatedInstitution.Id,
                                                       BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                       ManagerUnitId = am.RelatedManagerUnit.Id
                                                   }).AsNoTracking();
                    else
                        lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                                   join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                                   where am.Status == true &&
                                                         !a.flagVerificado.HasValue &&
                                                         a.flagDepreciaAcumulada == 1 &&
                                                         am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria
                                                   select new BolsaOrgaoViewModel
                                                   {
                                                       Id = a.Id,
                                                       Sigla = a.InitialName,
                                                       Chapa = a.NumberIdentification,
                                                       Item = a.MaterialItemCode,
                                                       DescricaoDoItem = a.MaterialItemDescription,
                                                       Orgao = am.RelatedInstitution.Code,
                                                       Gestor = am.RelatedInstitution.NameManagerReduced,
                                                       UO = am.RelatedBudgetUnit.Code,
                                                       UGE = am.RelatedManagerUnit.Code,
                                                       InstitutionId = am.RelatedInstitution.Id,
                                                       BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                       ManagerUnitId = am.RelatedManagerUnit.Id
                                                   }).AsNoTracking();

                    totalRegistros = await lstAssetEAssetsMovement.CountAsync();

                    var resultAdmin = await lstAssetEAssetsMovement.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultAdmin }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                               join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                               where am.Status == true &&
                                                     am.InstitutionId == _institutionId &&
                                                     am.ManagerUnitId == _managerUnitId &&
                                                     !a.flagVerificado.HasValue &&
                                                     a.flagDepreciaAcumulada == 1 &&
                                                     am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                                         (a.RelatedInitial.Name.Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemCode.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemDescription.Contains(currentFilter) ||
                                                          am.RelatedInstitution.NameManagerReduced.Contains(currentFilter) ||
                                                          am.RelatedBudgetUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedManagerUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Description.Contains(currentFilter) ||
                                                          am.RelatedResponsible.Name.Contains(currentFilter) ||
                                                          a.RateDepreciationMonthly.ToString().Contains(currentFilter) ||
                                                          a.ValueUpdate.ToString().Contains(currentFilter) ||
                                                          a.LifeCycle.ToString().Contains(currentFilter)
                                                        )
                                               select new BolsaOrgaoViewModel
                                               {
                                                   Id = a.Id,
                                                   Sigla = a.InitialName,
                                                   Chapa = a.NumberIdentification,
                                                   Item = a.MaterialItemCode,
                                                   DescricaoDoItem = a.MaterialItemDescription,
                                                   Orgao = am.RelatedInstitution.Code,
                                                   Gestor = am.RelatedInstitution.NameManagerReduced,
                                                   UO = am.RelatedBudgetUnit.Code,
                                                   UGE = am.RelatedManagerUnit.Code,
                                                   InstitutionId = am.RelatedInstitution.Id,
                                                   BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                   ManagerUnitId = am.RelatedManagerUnit.Id
                                               }).AsNoTracking();
                else
                    lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                               join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                               where am.Status == true &&
                                                     am.InstitutionId == _institutionId &&
                                                     am.ManagerUnitId == _managerUnitId &&
                                                     !a.flagVerificado.HasValue &&
                                                     a.flagDepreciaAcumulada == 1 &&
                                                     am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria
                                               select new BolsaOrgaoViewModel
                                               {
                                                   Id = a.Id,
                                                   Sigla = a.InitialName,
                                                   Chapa = a.NumberIdentification,
                                                   Item = a.MaterialItemCode,
                                                   DescricaoDoItem = a.MaterialItemDescription,
                                                   Orgao = am.RelatedInstitution.Code,
                                                   Gestor = am.RelatedInstitution.NameManagerReduced,
                                                   UO = am.RelatedBudgetUnit.Code,
                                                   UGE = am.RelatedManagerUnit.Code,
                                                   InstitutionId = am.RelatedInstitution.Id,
                                                   BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                   ManagerUnitId = am.RelatedManagerUnit.Id
                                               }).AsNoTracking();

                totalRegistros = await lstAssetEAssetsMovement.CountAsync();

                var result = await lstAssetEAssetsMovement.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region BolsaOrgaoRequisitar

        public ActionResult BolsaOrgaoRequisitar(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                #region Perfil Administrador Geral
                if (PerfilAdmGeral())
                {
                    ViewBag.NomeOrgao = "";

                    ViewBag.InstitutionId = "0";
                    ViewBag.BudgetUnitId = "0";
                    ViewBag.ManagerUnitId = "0";

                    return View();
                }
                #endregion

                getHierarquiaPerfil();
                //Guarda o filtro
                ViewBag.NomeOrgao = (from i in db.Institutions where i.Id == _institutionId select i.Code + " - " + i.Description).FirstOrDefault();

                ViewBag.InstitutionId = _institutionId.ToString();
                ViewBag.BudgetUnitId = _budgetUnitId.ToString();
                ViewBag.ManagerUnitId = _managerUnitId.ToString();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResultBolsaOrgaoRequisitar(AssetEAssetMovimentViewModel assetEAssetMoviment)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;

            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                IQueryable<BolsaOrgaoViewModel> lstRetorno;

                #region Perfil Administrador Geral
                if (PerfilAdmGeral())
                {
                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                        lstRetorno = (from a in db.Assets
                                      join am in db.AssetMovements on a.Id equals am.AssetId
                                      where am.Status == true &&
                                       !a.flagVerificado.HasValue &&
                                       a.flagDepreciaAcumulada == 1 &&
                                       am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                       (a.RelatedInitial.Name.Contains(currentFilter)
                                         || a.NumberIdentification.ToString().Contains(currentFilter)
                                         || a.MaterialItemCode.ToString().Contains(currentFilter)
                                         || a.MaterialItemDescription.Contains(currentFilter)
                                         || am.RelatedInstitution.NameManagerReduced.Contains(currentFilter)
                                         || am.RelatedBudgetUnit.Code.Contains(currentFilter)
                                         || am.RelatedManagerUnit.Code.Contains(currentFilter)
                                         || am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter)
                                         || am.RelatedSection.Code.ToString().Contains(currentFilter)
                                         || am.RelatedSection.Description.Contains(currentFilter)
                                         || am.RelatedResponsible.Name.Contains(currentFilter)
                                         || a.RateDepreciationMonthly.ToString().Contains(currentFilter)
                                         || a.ValueUpdate.ToString().Contains(currentFilter)
                                         || a.LifeCycle.ToString().Contains(currentFilter))
                                      select new BolsaOrgaoViewModel
                                      {
                                          Id = a.Id,
                                          Sigla = a.InitialName,
                                          Chapa = a.NumberIdentification,
                                          Item = a.MaterialItemCode,
                                          DescricaoDoItem = a.MaterialItemDescription,
                                          Orgao = am.RelatedInstitution.Code,
                                          Gestor = am.RelatedInstitution.NameManagerReduced,
                                          UO = am.RelatedBudgetUnit.Code,
                                          UGE = am.RelatedManagerUnit.Code,
                                          DescricaoUGE = am.RelatedManagerUnit.Description,
                                          InstitutionId = am.RelatedInstitution.Id,
                                          BudgetUnitId = am.RelatedBudgetUnit.Id,
                                          ManagerUnitId = am.RelatedManagerUnit.Id,
                                          Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                      }).AsNoTracking();
                    else
                        lstRetorno = (from a in db.Assets
                                      join am in db.AssetMovements on a.Id equals am.AssetId
                                      where am.Status == true &&
                                       !a.flagVerificado.HasValue &&
                                       a.flagDepreciaAcumulada == 1 &&
                                       am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria
                                      select new BolsaOrgaoViewModel
                                      {
                                          Id = a.Id,
                                          Sigla = a.InitialName,
                                          Chapa = a.NumberIdentification,
                                          Item = a.MaterialItemCode,
                                          DescricaoDoItem = a.MaterialItemDescription,
                                          Orgao = am.RelatedInstitution.Code,
                                          Gestor = am.RelatedInstitution.NameManagerReduced,
                                          UO = am.RelatedBudgetUnit.Code,
                                          UGE = am.RelatedManagerUnit.Code,
                                          DescricaoUGE = am.RelatedManagerUnit.Description,
                                          InstitutionId = am.RelatedInstitution.Id,
                                          BudgetUnitId = am.RelatedBudgetUnit.Id,
                                          ManagerUnitId = am.RelatedManagerUnit.Id,
                                          Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                      }).AsNoTracking();

                    totalRegistros = await lstRetorno.CountAsync();

                    var resultAdmin = await lstRetorno.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultAdmin }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    lstRetorno = (from a in db.Assets
                                  join am in db.AssetMovements on a.Id equals am.AssetId
                                  where am.Status == true &&
                                        !a.flagVerificado.HasValue &&
                                        a.flagDepreciaAcumulada == 1 &&
                                        am.InstitutionId == _institutionId &&
                                        am.ManagerUnitId != _managerUnitId &&
                                        am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                                        (a.RelatedInitial.Name.Contains(currentFilter)
                                              || a.NumberIdentification.ToString().Contains(currentFilter)
                                              || a.MaterialItemCode.ToString().Contains(currentFilter)
                                              || a.MaterialItemDescription.Contains(currentFilter)
                                              || am.RelatedInstitution.NameManagerReduced.Contains(currentFilter)
                                              || am.RelatedBudgetUnit.Code.Contains(currentFilter)
                                              || am.RelatedManagerUnit.Code.Contains(currentFilter)
                                              || am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter)
                                              || am.RelatedSection.Code.ToString().Contains(currentFilter)
                                              || am.RelatedSection.Description.Contains(currentFilter)
                                              || am.RelatedResponsible.Name.Contains(currentFilter)
                                              || a.RateDepreciationMonthly.ToString().Contains(currentFilter)
                                              || a.ValueUpdate.ToString().Contains(currentFilter)
                                              || a.LifeCycle.ToString().Contains(currentFilter))
                                  select new BolsaOrgaoViewModel
                                  {
                                      Id = a.Id,
                                      Sigla = a.InitialName,
                                      Chapa = a.NumberIdentification,
                                      Item = a.MaterialItemCode,
                                      DescricaoDoItem = a.MaterialItemDescription,
                                      Orgao = am.RelatedInstitution.Code,
                                      Gestor = am.RelatedInstitution.NameManagerReduced,
                                      UO = am.RelatedBudgetUnit.Code,
                                      UGE = am.RelatedManagerUnit.Code,
                                      DescricaoUGE = am.RelatedManagerUnit.Description,
                                      InstitutionId = am.RelatedInstitution.Id,
                                      BudgetUnitId = am.RelatedBudgetUnit.Id,
                                      ManagerUnitId = am.RelatedManagerUnit.Id,
                                      Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                  }).AsNoTracking();
                else
                    lstRetorno = (from a in db.Assets
                                  join am in db.AssetMovements on a.Id equals am.AssetId
                                  where am.Status == true &&
                                        !a.flagVerificado.HasValue &&
                                        a.flagDepreciaAcumulada == 1 &&
                                        am.InstitutionId == _institutionId &&
                                        am.ManagerUnitId != _managerUnitId &&
                                        am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria
                                  select new BolsaOrgaoViewModel
                                  {
                                      Id = a.Id,
                                      Sigla = a.InitialName,
                                      Chapa = a.NumberIdentification,
                                      Item = a.MaterialItemCode,
                                      DescricaoDoItem = a.MaterialItemDescription,
                                      Orgao = am.RelatedInstitution.Code,
                                      Gestor = am.RelatedInstitution.NameManagerReduced,
                                      UO = am.RelatedBudgetUnit.Code,
                                      UGE = am.RelatedManagerUnit.Code,
                                      DescricaoUGE = am.RelatedManagerUnit.Description,
                                      InstitutionId = am.RelatedInstitution.Id,
                                      BudgetUnitId = am.RelatedBudgetUnit.Id,
                                      ManagerUnitId = am.RelatedManagerUnit.Id,
                                      Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                  }).AsNoTracking();

                totalRegistros = await lstRetorno.CountAsync();

                var result = await lstRetorno.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region BolsaEstadual
        public ActionResult BolsaEstadual(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.RetiradaDaBolsa = (int)SAM.Web.Common.Enum.EnumMovimentType.RetiradaDaBolsa;
                ViewBag.CurrentFilter = searchString;
                ViewBag.AlertComSucesso = "";

                if (TempData["msgSucesso"] != null)
                {
                    ViewBag.AlertComSucesso = (string)TempData["msgSucesso"];
                }

                #region Perfil Administrador Geral
                if (PerfilAdmGeral())
                {
                    ViewBag.NomeEstadual = "";

                    ViewBag.InstitutionId = 0;
                    ViewBag.BudgetUnitId = 0;
                    ViewBag.ManagerUnitId = 0;

                    return View();
                }
                #endregion

                getHierarquiaPerfil();
                //Guarda o filtro
                ViewBag.NomeEstadual = (from i in db.Institutions where i.Id == _institutionId select i.Code + " - " + i.Description).FirstOrDefault();

                ViewBag.InstitutionId = _institutionId;
                ViewBag.BudgetUnitId = _budgetUnitId;
                ViewBag.ManagerUnitId = _managerUnitId;

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResultBolsaEstadual(AssetEAssetMovimentViewModel assetEAssetMoviment)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;
            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                IQueryable<BolsaEstadualViewModel> lstAssetEAssetsMovement;
                #region Perfil Administrador Geral
                if (PerfilAdmGeral() == true)
                {

                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                        lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                                   join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                                   where am.Status == true &&
                                                         !a.flagVerificado.HasValue &&
                                                         a.flagDepreciaAcumulada == 1 &&
                                                         am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                         (a.RelatedInitial.Name.Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemCode.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemDescription.Contains(currentFilter) ||
                                                          am.RelatedInstitution.NameManagerReduced.Contains(currentFilter) ||
                                                          am.RelatedBudgetUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedManagerUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Description.Contains(currentFilter) ||
                                                          am.RelatedResponsible.Name.Contains(currentFilter) ||
                                                          a.RateDepreciationMonthly.ToString().Contains(currentFilter) ||
                                                          a.ValueUpdate.ToString().Contains(currentFilter) ||
                                                          a.LifeCycle.ToString().Contains(currentFilter)
                                                        )
                                                   select new BolsaEstadualViewModel
                                                   {
                                                       Id = a.Id,
                                                       Sigla = a.InitialName,
                                                       Chapa = a.NumberIdentification,
                                                       Item = a.MaterialItemCode,
                                                       DescricaoDoItem = a.MaterialItemDescription,
                                                       Orgao = am.RelatedInstitution.Code,
                                                       Gestor = am.RelatedInstitution.NameManagerReduced,
                                                       UO = am.RelatedBudgetUnit.Code,
                                                       UGE = am.RelatedManagerUnit.Code,
                                                       InstitutionId = am.RelatedInstitution.Id,
                                                       BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                       ManagerUnitId = am.RelatedManagerUnit.Id
                                                   }).AsNoTracking();
                    else
                        lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                                   join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                                   where am.Status == true &&
                                                         !a.flagVerificado.HasValue &&
                                                         a.flagDepreciaAcumulada == 1 &&
                                                         am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual
                                                   select new BolsaEstadualViewModel
                                                   {
                                                       Id = a.Id,
                                                       Sigla = a.InitialName,
                                                       Chapa = a.NumberIdentification,
                                                       Item = a.MaterialItemCode,
                                                       DescricaoDoItem = a.MaterialItemDescription,
                                                       Orgao = am.RelatedInstitution.Code,
                                                       Gestor = am.RelatedInstitution.NameManagerReduced,
                                                       UO = am.RelatedBudgetUnit.Code,
                                                       UGE = am.RelatedManagerUnit.Code,
                                                       InstitutionId = am.RelatedInstitution.Id,
                                                       BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                       ManagerUnitId = am.RelatedManagerUnit.Id
                                                   }).AsNoTracking();

                    totalRegistros = await lstAssetEAssetsMovement.CountAsync();

                    var resultAdmin = await lstAssetEAssetsMovement.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultAdmin }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                               join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                               where am.Status == true &&
                                                     am.InstitutionId == _institutionId &&
                                                     am.ManagerUnitId == _managerUnitId &&
                                                     !a.flagVerificado.HasValue &&
                                                     a.flagDepreciaAcumulada == 1 &&
                                                     am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                                         (a.RelatedInitial.Name.Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.NumberIdentification.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemCode.ToString().Contains(currentFilter) ||
                                                          a.MaterialItemDescription.Contains(currentFilter) ||
                                                          am.RelatedInstitution.NameManagerReduced.Contains(currentFilter) ||
                                                          am.RelatedBudgetUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedManagerUnit.Code.Contains(currentFilter) ||
                                                          am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Code.ToString().Contains(currentFilter) ||
                                                          am.RelatedSection.Description.Contains(currentFilter) ||
                                                          am.RelatedResponsible.Name.Contains(currentFilter) ||
                                                          a.RateDepreciationMonthly.ToString().Contains(currentFilter) ||
                                                          a.ValueUpdate.ToString().Contains(currentFilter) ||
                                                          a.LifeCycle.ToString().Contains(currentFilter)
                                                        )
                                               select new BolsaEstadualViewModel
                                               {
                                                   Id = a.Id,
                                                   Sigla = a.InitialName,
                                                   Chapa = a.NumberIdentification,
                                                   Item = a.MaterialItemCode,
                                                   DescricaoDoItem = a.MaterialItemDescription,
                                                   Orgao = am.RelatedInstitution.Code,
                                                   Gestor = am.RelatedInstitution.NameManagerReduced,
                                                   UO = am.RelatedBudgetUnit.Code,
                                                   UGE = am.RelatedManagerUnit.Code,
                                                   InstitutionId = am.RelatedInstitution.Id,
                                                   BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                   ManagerUnitId = am.RelatedManagerUnit.Id
                                               }).AsNoTracking();
                else
                    lstAssetEAssetsMovement = (from a in db.Assets.Include("RelatedInitial")
                                               join am in db.AssetMovements.Include("RelatedInstitution").Include("RelatedManagerUnit").Include("RelatedBudgetUnit") on a.Id equals am.AssetId
                                               where am.Status == true &&
                                                     am.InstitutionId == _institutionId &&
                                                     am.ManagerUnitId == _managerUnitId &&
                                                     !a.flagVerificado.HasValue &&
                                                     a.flagDepreciaAcumulada == 1 &&
                                                     am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual
                                               select new BolsaEstadualViewModel
                                               {
                                                   Id = a.Id,
                                                   Sigla = a.InitialName,
                                                   Chapa = a.NumberIdentification,
                                                   Item = a.MaterialItemCode,
                                                   DescricaoDoItem = a.MaterialItemDescription,
                                                   Orgao = am.RelatedInstitution.Code,
                                                   Gestor = am.RelatedInstitution.NameManagerReduced,
                                                   UO = am.RelatedBudgetUnit.Code,
                                                   UGE = am.RelatedManagerUnit.Code,
                                                   InstitutionId = am.RelatedInstitution.Id,
                                                   BudgetUnitId = am.RelatedBudgetUnit.Id,
                                                   ManagerUnitId = am.RelatedManagerUnit.Id
                                               }).AsNoTracking();

                totalRegistros = await lstAssetEAssetsMovement.CountAsync();

                var result = await lstAssetEAssetsMovement.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region BolsaEstadualRequisitar
        public ActionResult BolsaEstadualRequisitar(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                #region Perfil Administrador Geral
                if (PerfilAdmGeral())
                {
                    ViewBag.NomeEstadual = "";

                    ViewBag.InstitutionId = "0";
                    ViewBag.BudgetUnitId = "0";
                    ViewBag.ManagerUnitId = "0";

                    return View();
                }
                #endregion

                getHierarquiaPerfil();
                ViewBag.NomeOrgao = (from i in db.Institutions where i.Id == _institutionId select i.Code + " - " + i.Description).FirstOrDefault();

                ViewBag.InstitutionId = _institutionId.ToString();
                ViewBag.BudgetUnitId = _budgetUnitId.ToString();
                ViewBag.ManagerUnitId = _managerUnitId.ToString();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResultBolsaEstadualRequisitar(AssetEAssetMovimentViewModel assetEAssetMoviment)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;

            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                IQueryable<BolsaEstadualViewModel> lstRetorno;

                #region Perfil Administrador Geral
                if (PerfilAdmGeral())
                {
                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                        lstRetorno = (from a in db.Assets
                                      join am in db.AssetMovements on a.Id equals am.AssetId
                                      where am.Status == true &&
                                       !a.flagVerificado.HasValue &&
                                       a.flagDepreciaAcumulada == 1 &&
                                       am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                       (a.RelatedInitial.Name.Contains(currentFilter)
                                         || a.NumberIdentification.ToString().Contains(currentFilter)
                                         || a.MaterialItemCode.ToString().Contains(currentFilter)
                                         || a.MaterialItemDescription.Contains(currentFilter)
                                         || am.RelatedInstitution.NameManagerReduced.Contains(currentFilter)
                                         || am.RelatedBudgetUnit.Code.Contains(currentFilter)
                                         || am.RelatedManagerUnit.Code.Contains(currentFilter)
                                         || am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter)
                                         || am.RelatedSection.Code.ToString().Contains(currentFilter)
                                         || am.RelatedSection.Description.Contains(currentFilter)
                                         || am.RelatedResponsible.Name.Contains(currentFilter)
                                         || a.RateDepreciationMonthly.ToString().Contains(currentFilter)
                                         || a.ValueUpdate.ToString().Contains(currentFilter)
                                         || a.LifeCycle.ToString().Contains(currentFilter))
                                      select new BolsaEstadualViewModel
                                      {
                                          Id = a.Id,
                                          Sigla = a.InitialName,
                                          Chapa = a.NumberIdentification,
                                          Item = a.MaterialItemCode,
                                          DescricaoDoItem = a.MaterialItemDescription,
                                          Orgao = am.RelatedInstitution.Code,
                                          Gestor = am.RelatedInstitution.NameManagerReduced,
                                          UO = am.RelatedBudgetUnit.Code,
                                          UGE = am.RelatedManagerUnit.Code,
                                          DescricaoUGE = am.RelatedManagerUnit.Description,
                                          InstitutionId = am.RelatedInstitution.Id,
                                          BudgetUnitId = am.RelatedBudgetUnit.Id,
                                          ManagerUnitId = am.RelatedManagerUnit.Id,
                                          Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                      }).AsNoTracking();
                    else
                        lstRetorno = (from a in db.Assets
                                      join am in db.AssetMovements on a.Id equals am.AssetId
                                      where am.Status == true &&
                                       !a.flagVerificado.HasValue &&
                                       a.flagDepreciaAcumulada == 1 &&
                                       am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual
                                      select new BolsaEstadualViewModel
                                      {
                                          Id = a.Id,
                                          Sigla = a.InitialName,
                                          Chapa = a.NumberIdentification,
                                          Item = a.MaterialItemCode,
                                          DescricaoDoItem = a.MaterialItemDescription,
                                          Orgao = am.RelatedInstitution.Code,
                                          Gestor = am.RelatedInstitution.NameManagerReduced,
                                          UO = am.RelatedBudgetUnit.Code,
                                          UGE = am.RelatedManagerUnit.Code,
                                          DescricaoUGE = am.RelatedManagerUnit.Description,
                                          InstitutionId = am.RelatedInstitution.Id,
                                          BudgetUnitId = am.RelatedBudgetUnit.Id,
                                          ManagerUnitId = am.RelatedManagerUnit.Id,
                                          Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                      }).AsNoTracking();

                    totalRegistros = await lstRetorno.CountAsync();

                    var resultAdmin = await lstRetorno.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultAdmin }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    lstRetorno = (from a in db.Assets
                                  join am in db.AssetMovements on a.Id equals am.AssetId
                                  where am.Status == true &&
                                        !a.flagVerificado.HasValue &&
                                        a.flagDepreciaAcumulada == 1 &&
                                        am.ManagerUnitId != _managerUnitId &&
                                        am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                                        (a.RelatedInitial.Name.Contains(currentFilter)
                                              || a.NumberIdentification.ToString().Contains(currentFilter)
                                              || a.MaterialItemCode.ToString().Contains(currentFilter)
                                              || a.MaterialItemDescription.Contains(currentFilter)
                                              || am.RelatedInstitution.NameManagerReduced.Contains(currentFilter)
                                              || am.RelatedBudgetUnit.Code.Contains(currentFilter)
                                              || am.RelatedManagerUnit.Code.Contains(currentFilter)
                                              || am.RelatedAdministrativeUnit.Code.ToString().Contains(currentFilter)
                                              || am.RelatedSection.Code.ToString().Contains(currentFilter)
                                              || am.RelatedSection.Description.Contains(currentFilter)
                                              || am.RelatedResponsible.Name.Contains(currentFilter)
                                              || a.RateDepreciationMonthly.ToString().Contains(currentFilter)
                                              || a.ValueUpdate.ToString().Contains(currentFilter)
                                              || a.LifeCycle.ToString().Contains(currentFilter))
                                  select new BolsaEstadualViewModel
                                  {
                                      Id = a.Id,
                                      Sigla = a.InitialName,
                                      Chapa = a.NumberIdentification,
                                      Item = a.MaterialItemCode,
                                      DescricaoDoItem = a.MaterialItemDescription,
                                      Orgao = am.RelatedInstitution.Code,
                                      Gestor = am.RelatedInstitution.NameManagerReduced,
                                      UO = am.RelatedBudgetUnit.Code,
                                      UGE = am.RelatedManagerUnit.Code,
                                      DescricaoUGE = am.RelatedManagerUnit.Description,
                                      InstitutionId = am.RelatedInstitution.Id,
                                      BudgetUnitId = am.RelatedBudgetUnit.Id,
                                      ManagerUnitId = am.RelatedManagerUnit.Id,
                                      Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                  }).AsNoTracking();
                else
                    lstRetorno = (from a in db.Assets
                                  join am in db.AssetMovements on a.Id equals am.AssetId
                                  where am.Status == true &&
                                        !a.flagVerificado.HasValue &&
                                        a.flagDepreciaAcumulada == 1 &&
                                        am.ManagerUnitId != _managerUnitId &&
                                        am.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual
                                  select new BolsaEstadualViewModel
                                  {
                                      Id = a.Id,
                                      Sigla = a.InitialName,
                                      Chapa = a.NumberIdentification,
                                      Item = a.MaterialItemCode,
                                      DescricaoDoItem = a.MaterialItemDescription,
                                      Orgao = am.RelatedInstitution.Code,
                                      Gestor = am.RelatedInstitution.NameManagerReduced,
                                      UO = am.RelatedBudgetUnit.Code,
                                      UGE = am.RelatedManagerUnit.Code,
                                      DescricaoUGE = am.RelatedManagerUnit.Description,
                                      InstitutionId = am.RelatedInstitution.Id,
                                      BudgetUnitId = am.RelatedBudgetUnit.Id,
                                      ManagerUnitId = am.RelatedManagerUnit.Id,
                                      Selecionado = (from e in db.Exchange where e.ManagerUnitId == _managerUnitId && e.Status == true && e.AssetId == a.Id select e.Id).Count() > 0
                                  }).AsNoTracking();

                totalRegistros = await lstRetorno.CountAsync();

                var result = await lstRetorno.OrderBy(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion

        [HttpGet]
        public JsonResult RequisitarItemNaBolsa(int _asseId, int _institution, int _budgetUnit, int _managerUnit)
        {
            try
            {
                Exchange _exchange = new Exchange();

                _exchange.AssetId = _asseId;
                _exchange.InstitutionId = _institution;
                _exchange.BudgetUnitId = _budgetUnit;
                _exchange.ManagerUnitId = _managerUnit;
                _exchange.DateRequisition = DateTime.Now;
                _exchange.DateService = DateTime.Now;
                _exchange.Login = UserCommon.CurrentUser().CPF;
                _exchange.Status = true;

                db.Exchange.Add(_exchange);
                db.SaveChanges();

                var retorno = Json(_exchange.Id, JsonRequestBehavior.AllowGet);

                return retorno;
            }
            catch (Exception ex)
            {
                MensagemModel mensagem = new MensagemModel();
                mensagem.Mensagem = "Não foi possivel requisitar o item da Bolsa";

                return Json(mensagem, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ExcluirItemRequisicao(int _asseId, int _institution, int _budgetUnit, int _managerUnit)
        {
            try
            {
                Exchange _exchange = (from e in db.Exchange
                                      where e.AssetId == _asseId
                                      && e.InstitutionId == _institution
                                      && e.BudgetUnitId == _budgetUnit
                                      && e.ManagerUnitId == _managerUnit
                                      && e.Status == true
                                      select e).AsNoTracking().FirstOrDefault();

                _exchange.Status = false;

                db.Entry(_exchange).State = EntityState.Modified;
                db.SaveChanges();

                var retorno = Json(_exchange.Id, JsonRequestBehavior.AllowGet);

                return retorno;
            }
            catch (Exception ex)
            {
                MensagemModel mensagem = new MensagemModel();
                mensagem.Mensagem = "Não foi possivel excluir a requisição do item!";

                return Json(mensagem, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarItemNaBolsa(int assetId, string tela)
        {
            try
            {
                getHierarquiaPerfil();

                ViewBag.InstitutionId = _institutionId;
                ViewBag.Ug = (from o in db.Institutions where o.Id == _institutionId && o.Status == true select o.ManagerCode).FirstOrDefault();

                ViewBag.Tela = tela;

                var lstExchangeView = (from a in db.Exchange
                                       join i in db.Institutions
                                       on a.InstitutionId equals i.Id
                                       join b in db.BudgetUnits
                                       on a.BudgetUnitId equals b.Id
                                       join m in db.ManagerUnits
                                       on a.ManagerUnitId equals m.Id
                                       where a.Status == true
                                       && a.AssetId == assetId
                                       select new ExchangeViewModel
                                       {
                                           Id = a.Id,
                                           AssetId = a.AssetId,
                                           InstitutionId = a.InstitutionId,
                                           InstitutionCode = i.Code,
                                           NameManagerReduced = i.NameManagerReduced,
                                           BudgetUnitId = a.BudgetUnitId,
                                           BudgetUnitCode = b.Code,
                                           ManagerUnitId = a.ManagerUnitId,
                                           ManagerUnitCode = m.Code,
                                           ManagerUnitDescription = m.Description,
                                           DateRequisition = a.DateRequisition,
                                           Login = a.Login,
                                           NomeRequisitante = (from u in db.Users where u.CPF == a.Login select u.Name).FirstOrDefault(),
                                           ManagerCodeRequisitante = i.ManagerCode
                                       }).AsNoTracking();

                return PartialView("_listaItensRequisitados", lstExchangeView);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #region MovimentoBolsa

        public ActionResult MovimentoBolsa(int AssetId, int tipoMovimento, int institutionIdDestino, int budgetUnitIdDestino, int managerUnitIdDestino, string searchString)
        {
            if (tipoMovimento != (int)EnumMovimentType.RetiradaDaBolsa)
            {
                var exchange = (from e in db.Exchange
                                where e.AssetId == AssetId
                                && e.InstitutionId == institutionIdDestino
                                && e.BudgetUnitId == budgetUnitIdDestino
                                && e.ManagerUnitId == managerUnitIdDestino
                                && e.Status == true
                                select e.Id).FirstOrDefault();

                if (exchange == 0)
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }

            var assetmovement = (from a in db.Assets
                                 join am in db.AssetMovements
                                 on a.Id equals am.AssetId
                                 where a.Id == AssetId &&
                                       am.Status == true
                                 select new AssetEAssetMovimentViewModel
                                 {
                                     Asset = a,
                                     AssetMoviment = am
                                 }).FirstOrDefault();

            getHierarquiaPerfil();

            if (assetmovement.AssetMoviment.ManagerUnitId != _managerUnitId)
                return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);

            if (UGEEstaComPendenciaSIAFEMNoFechamento(assetmovement.AssetMoviment.ManagerUnitId))
                return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

            var tipos = new[] {(int)EnumMovimentType.RetiradaDaBolsa, (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado,
                               (int)EnumMovimentType.MovDoacaoIntraNoEstado, (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado};

            if (!tipos.Contains(tipoMovimento))
                return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);

            MovimentoViewModel movimento = new MovimentoViewModel();
            movimento.AssetId = AssetId;
            movimento.ManagerUnitId = (int)_managerUnitId;
            movimento.SearchString = searchString;
            var MovimentoTypes = (from mt in db.MovementTypes
                                  where mt.Status == true &&
                                        mt.Code == tipoMovimento
                                  select mt).ToList();

            ViewBag.MovimentType = new SelectList(MovimentoTypes, "Id", "Description");
            hierarquia = new Hierarquia();
            ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(institutionIdDestino), "Id", "Description", institutionIdDestino);
            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUos(budgetUnitIdDestino), "Id", "Description", budgetUnitIdDestino);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUges(managerUnitIdDestino), "Id", "Description", managerUnitIdDestino);
            ViewBag.Transferencia = (tipoMovimento == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado
                                     || tipoMovimento == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado);
            ViewBag.RetiradaBolsa = (tipoMovimento == (int)EnumMovimentType.RetiradaDaBolsa);
            ViewBag.DoacaoIntra = (tipoMovimento == (int)EnumMovimentType.MovDoacaoIntraNoEstado);
            ViewBag.flagIntegracaoSiafem = ImplantadoSIAFEM();
            ViewBag.MsgBotaoSalvar = (tipoMovimento == (int)EnumMovimentType.RetiradaDaBolsa) ?
                                      "Todas as requisições à essa bolsa serão excluídas após a retirada" :
                                      MensagemBaixa(movimento.ManagerUnitIdDestino);

            return View(movimento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MovimentoBolsa(MovimentoViewModel movimento)
        {
            try
            {
                getHierarquiaPerfil();

                var mensagemErro = ValidaDataDeTranferencia(movimento.MovimentoDate, new List<int>() { movimento.AssetId }, movimento, movimento.ManagerUnitId);

                if (mensagemErro != string.Empty)
                    ModelState.AddModelError("MovimentoDate", mensagemErro);


                if (ModelState.IsValid)
                {
                    string tipoMovimento = (from m in db.MovementTypes where m.Id == movimento.MovementTypeId select m.Description).FirstOrDefault();

                    //Geração do NumberDoc da AssetMovements
                    string NumberDocGerado = "";
                    string Sequencia = BuscaUltimoNumeroDocumentoSaida(movimento.ManagerUnitId);

                    //string Sequencia = (from am in db.AssetMovements where am.ManagerUnitId == assetEAssetMovimentBD.AssetMoviment.ManagerUnitId select am.NumberDoc).Max();

                    if (Sequencia.Equals("0"))
                    {
                        string Codigo = (from m in db.ManagerUnits where m.Id == movimento.ManagerUnitId select m.Code).FirstOrDefault();
                        NumberDocGerado = DateTime.Now.Year + Codigo + "0001";
                    }
                    else
                    {
                        if (Sequencia.Length > 14)
                        {
                            int contador = Convert.ToInt32(Sequencia.Substring(10, Sequencia.Length - 10));

                            if (contador < 9999)
                            {
                                NumberDocGerado = Sequencia.Substring(0, 10) + (contador + 1).ToString().PadLeft(4,'0');
                            }
                            else
                            {
                                NumberDocGerado = (long.Parse(Sequencia) + 1).ToString();
                            }
                        }
                        else
                            NumberDocGerado = (long.Parse(Sequencia) + 1).ToString();
                    }

                    //Fim da Geração

                    AssetMovements assetMoviment = new AssetMovements();
                    assetMoviment.MovimentDate = Convert.ToDateTime(movimento.MovimentoDate);
                    assetMoviment.MovementTypeId = movimento.MovementTypeId;
                    assetMoviment.InstitutionId = _institutionId;
                    assetMoviment.BudgetUnitId = _budgetUnitId ?? 0;
                    assetMoviment.ManagerUnitId = _managerUnitId ?? 0;
                    assetMoviment.AssetTransferenciaId = null;
                    assetMoviment.ExchangeId = null;
                    assetMoviment.ExchangeDate = null;
                    assetMoviment.ExchangeUserId = null;
                    assetMoviment.Observation = movimento.Observation;
                    assetMoviment.Login = UserCommon.CurrentUser().CPF;
                    assetMoviment.DataLogin = DateTime.Now;

                    var assetEAssetMovimentBD = (from a in db.Assets
                                                 join am in db.AssetMovements
                                                 on a.Id equals am.AssetId
                                                 where a.Id == movimento.AssetId &&
                                                       am.Status == true
                                                 select new AssetEAssetMovimentViewModel
                                                 {
                                                     Asset = a,
                                                     AssetMoviment = am
                                                 }).FirstOrDefault();

                    assetMoviment.AssetId = assetEAssetMovimentBD.AssetMoviment.AssetId;
                    assetMoviment.StateConservationId = assetEAssetMovimentBD.AssetMoviment.StateConservationId;
                    assetMoviment.AuxiliaryAccountId = assetEAssetMovimentBD.AssetMoviment.AuxiliaryAccountId;
                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                    string actionSucesso = "BolsaOrgao";
                    db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
                    switch (movimento.MovementTypeId)
                    {
                        #region DOAÇÃO INTRA - NO ESTADO

                        case (int)EnumMovimentType.MovDoacaoIntraNoEstado:

                            assetEAssetMovimentBD.AssetMoviment.Status = false;
                            db.Entry(assetMoviment).State = EntityState.Modified;
                            assetMoviment.Status = true;
                            assetMoviment.NumberPurchaseProcess = movimento.NumberProcess;
                            assetMoviment.AdministrativeUnitId = null;
                            assetMoviment.SectionId = null;
                            assetMoviment.ResponsibleId = null;
                            assetMoviment.SourceDestiny_ManagerUnitId = movimento.ManagerUnitIdDestino;
                            assetMoviment.TypeDocumentOutId = movimento.TypeDocumentOutId;

                            db.Entry(assetMoviment).State = EntityState.Added;
                            db.SaveChanges();

                            // --->>> INICIO - SERVICO CONTABILIZA
                            //TODO DCBATISTA INSERIR CHAMADA INTEGRACAO CONTABILIZA-SP
                            // --->>> FIM - SERVICO CONTABILIZA

                            verificaImplantacaoAceiteAutomatico(assetMoviment, movimento);

                            actionSucesso = "BolsaEstadual";
                            TempData["msgSucesso"] = String.Format("Movimentação do tipo {0} salva com sucesso!", tipoMovimento);
                            TempData.Keep();
                            break;
                        #endregion
                        #region TRANSFERÊNCIA OUTRO ORGÃO - PATRIMÔNIADO

                        case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:

                            assetEAssetMovimentBD.AssetMoviment.Status = false;
                            db.Entry(assetMoviment).State = EntityState.Modified;

                            bool? orgaoDestinoTransferenciaImplantadoOutroOrgao = (from m in db.Institutions
                                                                                   where m.Id == movimento.InstituationIdDestino
                                                                                   select m.flagImplantado).FirstOrDefault();

                            if (orgaoDestinoTransferenciaImplantadoOutroOrgao == true)
                            {
                                assetMoviment.Status = true;

                                assetMoviment.NumberPurchaseProcess = movimento.NumberProcess;
                                assetMoviment.AdministrativeUnitId = null;
                                assetMoviment.SectionId = null;
                                assetMoviment.ResponsibleId = null;
                                assetMoviment.SourceDestiny_ManagerUnitId = movimento.ManagerUnitIdDestino;
                                assetMoviment.TypeDocumentOutId = null;

                                db.Entry(assetMoviment).State = EntityState.Added;
                                db.SaveChanges();
                            }
                            else
                            {
                                assetMoviment.Status = false;
                                assetMoviment.NumberPurchaseProcess = movimento.NumberProcess;
                                assetMoviment.AdministrativeUnitId = null;
                                assetMoviment.SectionId = null;
                                assetMoviment.ResponsibleId = null;
                                assetMoviment.SourceDestiny_ManagerUnitId = movimento.ManagerUnitIdDestino;
                                assetMoviment.TypeDocumentOutId = null;
                                assetMoviment.flagUGENaoUtilizada = true;

                                db.Entry(assetMoviment).State = EntityState.Added;
                                db.SaveChanges();

                                var assetTransferidoDB3 = (from a in db.Assets
                                                           where a.Id == movimento.AssetId

                                                           select a).FirstOrDefault();

                                assetTransferidoDB3.Status = false;
                                db.Entry(assetTransferidoDB3).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            // --->>> INICIO - SERVICO CONTABILIZA
                            //TODO DCBATISTA INSERIR CHAMADA INTEGRACAO CONTABILIZA-SP
                            // --->>> FIM - SERVICO CONTABILIZA

                            verificaImplantacaoAceiteAutomatico(assetMoviment, movimento);

                            actionSucesso = "BolsaEstadual";

                            TempData["msgSucesso"] = String.Format("Movimentação do tipo {0} salva com sucesso!", tipoMovimento);
                            TempData.Keep();

                            break;

                        #endregion
                        #region TRANSFERÊNCIA MESMO ORGÃO - PATRIMÔNIADO
                        case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:

                            assetEAssetMovimentBD.AssetMoviment.Status = false;
                            db.Entry(assetMoviment).State = EntityState.Modified;

                            bool? orgaoDestinoTransferenciaImplantadoMesmoOrgao = (from m in db.Institutions
                                                                                   where m.Id == movimento.InstituationIdDestino
                                                                                   select m.flagImplantado).FirstOrDefault();

                            if (orgaoDestinoTransferenciaImplantadoMesmoOrgao == true)
                            {
                                assetMoviment.Status = true;
                                assetMoviment.NumberPurchaseProcess = movimento.NumberProcess;
                                assetMoviment.AdministrativeUnitId = null;
                                assetMoviment.SectionId = null;
                                assetMoviment.ResponsibleId = null;
                                assetMoviment.SourceDestiny_ManagerUnitId = movimento.ManagerUnitIdDestino;
                                assetMoviment.TypeDocumentOutId = null;

                                db.Entry(assetMoviment).State = EntityState.Added;
                                db.SaveChanges();
                            }
                            else
                            {
                                assetMoviment.Status = false;
                                assetMoviment.NumberPurchaseProcess = movimento.NumberProcess;
                                assetMoviment.AdministrativeUnitId = null;
                                assetMoviment.SectionId = null;
                                assetMoviment.ResponsibleId = null;
                                assetMoviment.SourceDestiny_ManagerUnitId = movimento.ManagerUnitIdDestino;
                                assetMoviment.TypeDocumentOutId = null;
                                assetMoviment.flagUGENaoUtilizada = true;

                                db.Entry(assetMoviment).State = EntityState.Added;
                                db.SaveChanges();

                                var assetTransferidoDB2 = (from a in db.Assets
                                                           where a.Id == movimento.AssetId

                                                           select a).FirstOrDefault();

                                assetTransferidoDB2.Status = false;
                                db.Entry(assetTransferidoDB2).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            // --->>> INICIO - SERVICO CONTABILIZA
                            //TODO DCBATISTA INSERIR CHAMADA INTEGRACAO CONTABILIZA-SP
                            // --->>> FIM - SERVICO CONTABILIZA

                            verificaImplantacaoUGEAceiteAutomatico(assetMoviment, movimento);

                            TempData["msgSucesso"] = String.Format("Movimentação do tipo {0} salva com sucesso!", tipoMovimento);
                            TempData.Keep();

                            if (assetEAssetMovimentBD.AssetMoviment.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria)
                                actionSucesso = "BolsaOrgao";
                            else
                                actionSucesso = "BolsaEstadual";

                            break;
                        #endregion
                        #region RetiradaBolsa
                        case (int)EnumMovimentType.RetiradaDaBolsa:

                            assetEAssetMovimentBD.AssetMoviment.Status = false;
                            db.Entry(assetMoviment).State = EntityState.Modified;

                            AssetMovements assetMovimentAnterior = (from am in db.AssetMovements
                                                                    where am.AssetId == assetEAssetMovimentBD.AssetMoviment.AssetId
                                                                    && am.Id < assetEAssetMovimentBD.AssetMoviment.Id
                                                                    orderby am.Id descending
                                                                    select am).FirstOrDefault();

                            //assetMovimentAnterior.Status = true;
                            //db.Entry(assetMovimentAnterior).State = EntityState.Modified;


                            assetMoviment.Status = true;
                            assetMoviment.NumberDoc = NumberDocGerado;
                            assetMoviment.NumberPurchaseProcess = movimento.NumberProcess;
                            assetMoviment.AdministrativeUnitId = assetMovimentAnterior.AdministrativeUnitId;
                            assetMoviment.SectionId = assetMovimentAnterior.SectionId;
                            assetMoviment.ResponsibleId = assetMovimentAnterior.ResponsibleId;
                            assetMoviment.SourceDestiny_ManagerUnitId = null;

                            assetMoviment.TypeDocumentOutId = movimento.TypeDocumentOutId;


                            var lstRequisicoesDoacao = (from am in db.Exchange where am.AssetId == assetMoviment.AssetId select am).ToList();
                            foreach (var item in lstRequisicoesDoacao)
                            {
                                item.Status = false;
                                db.Entry(item).State = EntityState.Modified;
                            }

                            db.Entry(assetMoviment).State = EntityState.Added;
                            db.SaveChanges();

                            if (assetEAssetMovimentBD.AssetMoviment.MovementTypeId == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria)
                                actionSucesso = "BolsaOrgao";
                            else
                                actionSucesso = "BolsaEstadual";
                            break;
                    }
                    #endregion
                    db.Database.CurrentTransaction.Commit();

                    ComputaAlteracaoContabil(assetMoviment.InstitutionId, assetMoviment.BudgetUnitId, assetMoviment.ManagerUnitId, (int)assetMoviment.AuxiliaryAccountId);

                    return RedirectToAction(actionSucesso, "Exchange");
                }

                var MovimentoTypes = (from mt in db.MovementTypes
                                      where mt.Status == true &&
                                            mt.Code == movimento.MovementTypeId
                                      select mt).AsNoTracking().ToList();

                ViewBag.MovimentType = new SelectList(MovimentoTypes, "Id", "Description");
                hierarquia = new Hierarquia();
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(movimento.InstituationIdDestino), "Id", "Description", movimento.InstituationIdDestino);
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUos(movimento.BudgetUnitIdDestino), "Id", "Description", movimento.BudgetUnitIdDestino);
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUges(movimento.ManagerUnitIdDestino), "Id", "Description", movimento.ManagerUnitIdDestino);
                ViewBag.Transferencia = (movimento.MovementTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado
                                         || movimento.MovementTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado);
                ViewBag.DoacaoIntra = (movimento.MovementTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado);
                ViewBag.RetiradaBolsa = (movimento.MovementTypeId == (int)EnumMovimentType.RetiradaDaBolsa);
                ViewBag.flagIntegracaoSiafem = ImplantadoSIAFEM();
                ViewBag.MsgBotaoSalvar = (movimento.MovementTypeId == (int)EnumMovimentType.RetiradaDaBolsa) ?
                                      "Todas as requisições à essa bolsa serão excluídas após a retirada." :
                                      MensagemBaixa(movimento.ManagerUnitIdDestino);

                return View(movimento);
            }
            catch (Exception ex)
            {
                if (db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();

                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Copiado da MovimentoController
        public string ValidaDataDeTranferencia(string strDataDeTransferencia, List<int> listaAssetsId, MovimentoViewModel movimentoViewModel, int managerUnitId = 0)
        {
            string _mensagemRetorno = "";

            DateTime dataDeTransferencia = Convert.ToDateTime(strDataDeTransferencia);
            DateTime dataMaisAtualMovimentacaoPorAssets = RecuperaDataMaisAtualMovimentacaoPorAssets(listaAssetsId);

            string anoMesTransferencia = dataDeTransferencia.Year.ToString().PadLeft(4, '0') + dataDeTransferencia.Month.ToString().PadLeft(2, '0');
            string anoMesReferenciaFechamentoDaUGE = RecuperaAnoMesReferenciaFechamento(managerUnitId);

            if (strDataDeTransferencia == null || strDataDeTransferencia.Trim() == string.Empty)
            {
                _mensagemRetorno = "O campo Data de Movimento é obrigatório";
                return _mensagemRetorno;
            }

            if (dataDeTransferencia.Date > DateTime.Now.Date)
            {
                _mensagemRetorno = "Por favor, informe uma data de transferência igual ou inferior a data atual.";
                return _mensagemRetorno;
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

        private string RecuperaAnoMesReferenciaFechamento(int _managerUnitId)
        {
            return (from m in db.ManagerUnits
                    where m.Id == _managerUnitId
                    select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
        }

        private string BuscaUltimoNumeroDocumentoSaida(int IdUGE)
        {

            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA] @ManagerUnitId = " + IdUGE.ToString());
            builder.Append(",@Ano = " + DateTime.Now.Year.ToString());

            return db.Database.SqlQuery<string>(builder.ToString()).FirstOrDefault();
        }

        #endregion

        #region SIAFEM

        private string ImplantadoSIAFEM()
        {
            int flagSiafem = 0;

            flagSiafem = (from i in db.Institutions
                          where i.Id == _institutionId
                             && i.flagIntegracaoSiafem == true
                          select i.flagIntegracaoSiafem).Count();
            if (flagSiafem < 1)
            {
                flagSiafem = (from m in db.ManagerUnits
                              where m.Id == _managerUnitId
                                 && m.FlagIntegracaoSiafem == true
                              select m.FlagIntegracaoSiafem).Count();
            }

            return flagSiafem.ToString();
        }

        #endregion SIAFEM

        private string MensagemBaixa(int IdUGEDestino)
        {
            bool orgaoEhImplantado = (from i in db.Institutions
                                      join b in db.BudgetUnits
                                      on i.Id equals b.InstitutionId
                                      join m in db.ManagerUnits
                                      on b.Id equals m.BudgetUnitId
                                      where m.Id == IdUGEDestino
                                      select i.flagImplantado).FirstOrDefault();

            bool UGEComoOrgaoNaoImplantado = (from m in db.ManagerUnits
                                              where m.Id == IdUGEDestino
                                              select m.FlagTratarComoOrgao).FirstOrDefault();

            if (orgaoEhImplantado || UGEComoOrgaoNaoImplantado)
                return "A UGE de Destino precisa dar o \"Aceite\" para finalizar a Transferência/Doação.";
            else
                return "A UGE de Destino não precisa dar o \"Aceite\" por que não utiliza o SAM.";
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

        #region Aceite Automatico
        private void verificaImplantacaoUGEAceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {
            bool UGEComoOrgaoNaoImplantado = (from m in db.ManagerUnits
                                              where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                              select m.FlagTratarComoOrgao).FirstOrDefault();

            if (UGEComoOrgaoNaoImplantado)
                aceiteAutomatico(assetMoviment, movimentoViewModel);

        }

        private void verificaImplantacaoAceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {
            bool orgaoEhImplantado = (from i in db.Institutions
                                      join b in db.BudgetUnits
                                      on i.Id equals b.InstitutionId
                                      join m in db.ManagerUnits
                                      on b.Id equals m.BudgetUnitId
                                      where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                      select i.flagImplantado).FirstOrDefault();

            bool UGEComoOrgaoNaoImplantado = (from m in db.ManagerUnits
                                              where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                              select m.FlagTratarComoOrgao).FirstOrDefault();

            if (!orgaoEhImplantado || UGEComoOrgaoNaoImplantado)
                aceiteAutomatico(assetMoviment, movimentoViewModel);

        }

        private void aceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {
            IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote = null;

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

            // --->>> INICIO - SERVICO CONTABILIZA PARA NOVA MOVIMENTAÇÃO
            //TODO DCBATISTA INSERIR CHAMADA INTEGRACAO CONTABILIZA-SP
            string msgInformeAoUsuario = null;
            var loginUsuarioSIAFEM = movimentoViewModel.LoginSiafem;
            var senhaUsuarioSIAFEM = movimentoViewModel.SenhaSiafem;
            listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>() { assetMovimentNaNovaUGE };
            msgInformeAoUsuario = base.processamentoMovimentacaoPatrimonialNoContabilizaSP(movimentoViewModel.MovementTypeId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, assetMovimentNaNovaUGE.NumberDoc, listaMovimentacoesPatrimoniaisEmLote);
            msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();
            TempData["msgSucesso"] = msgInformeAoUsuario;
            TempData["numeroDocumento"] = assetMovimentNaNovaUGE.NumberDoc;
            TempData.Keep();
            // --->>> FIM - SERVICO CONTABILIZA PARA NOVA MOVIMENTAÇÃO
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
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado;
                    break;
            }

            return assetMovimentNaNovaUGE;
        }

        #endregion Aceite Automatico

    }
}