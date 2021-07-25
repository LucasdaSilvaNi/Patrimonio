using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using SAM.Web.Incorporacao;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAM.Web.Controllers
{
    public class AssetPendingController : BaseController
    {
        private SAMContext db = new SAMContext();

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;

        private int _lifeCycleMaterialGroup;
        private decimal _rateDepreciationMonthlyMaterialGroup;
        private decimal _residualValueMaterialGroup;
        private readonly ApiMovimentoIntegracaoController apiMovimentoIntegracaoController = new ApiMovimentoIntegracaoController();

        public void getHierarquiaPerfil()
        {
            RelationshipUserProfileInstitution perflLogado;
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            }
            else
            {
                perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
            }
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _administrativeUnitId = perflLogado.AdministrativeUnitId;
            _sectionId = perflLogado.SectionId;
        }

        #region Index
        // GET: AssetPending
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page, string cbStatus, string cbFiltros)
        {
            try
            {
                if ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.AdministradordeOrgao || (int)HttpContext.Items["perfilId"] == (int)EnumProfile.AdministradordeUO)
                    ViewBag.NaoDesabilitaExcel = true;
                else
                    ViewBag.NaoDesabilitaExcel = false;

                //Guarda o filtro
                ViewBag.CurrentFilter = searchString;
                ViewBag.CurrentFilterCbFiltros = cbFiltros;

                //Carrega combo de filtros
                CarregaComboFiltros(cbFiltros, true);

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(UserViewModel user)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"] == null ? null : Request.Form["currentFilter"].ToString();
            string cbFiltro = Request.Form["cbFiltro"] == null ? null : Request.Form["cbFiltro"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;

            try
            {

                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _budgetUnitId = IdsHieraquia[1];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                    _budgetUnitId = _budgetUnitId == null ? 0 : _budgetUnitId;
                    _managerUnitId = _managerUnitId == null ? 0 : _managerUnitId;
                }




                IQueryable<AssetPendingViewModel> result = null;

                if (PerfilAdmGeral())
                {
                    result = (from a in db.Assets.Include("RelatedInitial").Include("RelatedManagerUnit")
                              join am in db.AssetMovements.Include("RelatedAdministrativeUnit")
                              on a.Id equals am.AssetId
                              where a.Status == true &&
                                    a.flagVerificado.HasValue &&
                                    a.flagDepreciaAcumulada != 1 &&
                                    a.flagVindoDoEstoque == false
                              select new AssetPendingViewModel
                              {
                                  Id = a.Id,
                                  Sigla = a.RelatedInitial == null ? string.Empty : a.RelatedInitial.Name,
                                  Chapa = a.ChapaCompleta,
                                  CodigoMaterial = a.MaterialItemCode,
                                  DescricaoMaterial = a.MaterialItemDescription,
                                  UGE = am.RelatedManagerUnit == null ? string.Empty : am.RelatedManagerUnit.Code,
                                  UA = am.RelatedAdministrativeUnit == null ? 0 : am.RelatedAdministrativeUnit.Code,
                                  ValorAquisicao = a.ValueAcquisition,
                                  TaxaDepreciacaoMensal = a.RateDepreciationMonthly.ToString()
                              }).AsNoTracking();

                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrEmpty(currentFilter))
                        result = FiltraPesquisa(result, currentFilter == null ? currentFilter : currentFilter.Trim(), cbFiltro);

                    totalRegistros = await result.CountAsync();

                    result = result.OrderBy(ap => ap.Id).Skip(startRec).Take(length).AsNoTracking();
                    result = Ordena(result, Convert.ToInt32(order), orderDir);

                    TempData["ListaVisao"] = result;
                    TempData.Keep("ListaVisao");

                    var resultAdmin = await result.ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultAdmin }, JsonRequestBehavior.AllowGet);

                }
                result = (from a in db.Assets
                          join am in db.AssetMovements
                          on a.Id equals am.AssetId
                          where am.Status == true &&
                                a.flagVerificado.HasValue &&
                                a.flagDepreciaAcumulada != 1 &&
                                a.flagVindoDoEstoque == false &&
                                am.InstitutionId == _institutionId &&
                                am.BudgetUnitId == (_budgetUnitId != 0 ? _budgetUnitId : am.BudgetUnitId) &&
                                am.ManagerUnitId == (_managerUnitId != 0 ? _managerUnitId : am.ManagerUnitId)
                          select new AssetPendingViewModel
                          {
                              Id = a.Id,
                              Sigla = a.RelatedInitial == null ? string.Empty : a.RelatedInitial.Name,
                              Chapa = a.ChapaCompleta,
                              CodigoMaterial = a.MaterialItemCode,
                              DescricaoMaterial = a.MaterialItemDescription,
                              UGE = am.RelatedManagerUnit == null ? string.Empty : am.RelatedManagerUnit.Code,
                              UA = am.RelatedAdministrativeUnit == null ? 0 : am.RelatedAdministrativeUnit.Code,
                              ValorAquisicao = a.ValueAcquisition,
                              TaxaDepreciacaoMensal = a.RateDepreciationMonthly.ToString()
                          }).AsNoTracking();


                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrEmpty(currentFilter))
                    result = FiltraPesquisa(result, currentFilter == null ? currentFilter : currentFilter.Trim(), cbFiltro);

                TempData["ListaVisao"] = result;
                TempData.Keep("ListaVisao");

                totalRegistros = await result.CountAsync();

                result = result.OrderBy(ap => ap.Chapa).ThenBy(ap => ap.Sigla).Skip(startRec).Take(length).AsNoTracking();
                result = Ordena(result, Convert.ToInt32(order), orderDir);
                var resultado = await result.ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Index

        #region Integração
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult EditIntegracao()
        {

            if (Request.Params["Id"] == null)
                return MensagemErro(CommonMensagens.IdentificadorNulo);

            var integracaoAssetIdViewModels = JsonConvert.DeserializeObject<List<IntegracaoAssetIdViewModel>>(Request.Params["Id"].ToString());
            try
            {
                if (integracaoAssetIdViewModels == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                int id = integracaoAssetIdViewModels[0].AssetId;

                AssetViewModel BPASerEditado = (from a in db.Assets
                                                join am in db.AssetMovements
                                                on a.Id equals am.AssetId
                                                where a.flagVerificado.HasValue &&
                                                      a.flagDepreciaAcumulada != 1 &&
                                                      a.Id == id
                                                select new AssetViewModel
                                                {
                                                    Id = a.Id,
                                                    InitialId = a.InitialId,
                                                    InitialDescription = a.RelatedInitial == null ? null : a.RelatedInitial.Description,
                                                    NumberIdentification = a.NumberIdentification,
                                                    AcquisitionDate = a.AcquisitionDate,
                                                    //t.MovimentDate = a.MovimentDate,
                                                    ValueAcquisition = a.ValueAcquisition,
                                                    MaterialItemCode = a.MaterialItemCode,
                                                    MaterialItemDescription = a.MaterialItemDescription,
                                                    MaterialGroupCode = a.MaterialGroupCode,
                                                    MaterialGroupDescription = string.Empty,
                                                    ShortDescription = a.RelatedShortDescriptionItem == null ? null : a.RelatedShortDescriptionItem.Description,
                                                    LifeCycle = 0,
                                                    RateDepreciationMonthly = 0,
                                                    ResidualValue = 0,
                                                    ShortDescriptionItemId = a.ShortDescriptionItemId,
                                                    OldInitial = a.OldInitial,
                                                    OldNumberIdentification = a.OldNumberIdentification,
                                                    DiferenciacaoChapaAntiga = a.DiferenciacaoChapaAntiga,
                                                    NumberDoc = a.NumberDoc,
                                                    checkFlagAcervo = a.flagAcervo == null ? false : (bool)a.flagAcervo,
                                                    checkFlagTerceiro = a.flagTerceiro == null ? false : (bool)a.flagTerceiro,
                                                    checkFlagDecretoSefaz = a.flagDecreto == null ? false : (bool)a.flagDecreto,
                                                    SerialNumber = a.SerialNumber,
                                                    DateGuarantee = a.DateGuarantee,
                                                    Brand = a.Brand,
                                                    NumberPlate = a.NumberPlate,
                                                    ManufactureDate = a.ManufactureDate,
                                                    ChassiNumber = a.ChassiNumber,
                                                    Model = a.Model,
                                                    AdditionalDescription = a.AdditionalDescription,
                                                    MovementTypeId = a.MovementTypeId,
                                                    AuxiliaryMovementTypeId = am.AuxiliaryMovementTypeId
                                                }).FirstOrDefault();

                if (BPASerEditado == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                AssetMovements HistoricoDoBP = (from am in db.AssetMovements where am.AssetId == id && am.Status == true select am).AsNoTracking().FirstOrDefault();

                if (ValidarRequisicao(HistoricoDoBP.InstitutionId, null, null, null, null))
                {
                    ViewBag.StateConservationDescription = (from s in db.StateConservations
                                                            where s.Id == HistoricoDoBP.StateConservationId
                                                            select s.Description).FirstOrDefault();

                    BPASerEditado.ValueAcquisitionModel = BPASerEditado.ValueAcquisition.ToString();
                    BPASerEditado.ManagerUnitId = HistoricoDoBP.ManagerUnitId;
                    BPASerEditado.AssetMovementsId = HistoricoDoBP.Id;
                    BPASerEditado.StateConservationId = HistoricoDoBP.StateConservationId;
                    BPASerEditado.NumberPurchaseProcess = HistoricoDoBP.NumberPurchaseProcess;
                    BPASerEditado.InstitutionId = HistoricoDoBP.InstitutionId;
                    BPASerEditado.BudgetUnitId = HistoricoDoBP.BudgetUnitId;
                    BPASerEditado.BudgetUnitCode = HistoricoDoBP.RelatedBudgetUnit.Code;
                    BPASerEditado.ManagerUnitId = HistoricoDoBP.ManagerUnitId;
                    BPASerEditado.ManagerUnitCode = HistoricoDoBP.RelatedManagerUnit.Code;
                    BPASerEditado.AdministrativeUnitId = HistoricoDoBP.AdministrativeUnitId;
                    BPASerEditado.AdministrativeUnitCode = HistoricoDoBP.RelatedAdministrativeUnit == null ? 0 : HistoricoDoBP.RelatedAdministrativeUnit.Code;
                    BPASerEditado.SectionId = HistoricoDoBP.SectionId;
                    BPASerEditado.SectionCode = HistoricoDoBP.RelatedSection == null ? 0 : HistoricoDoBP.RelatedSection.Code;
                    BPASerEditado.AuxiliaryAccountId = HistoricoDoBP.AuxiliaryAccountId;
                    BPASerEditado.AuxiliaryAccountCode = HistoricoDoBP.RelatedAuxiliaryAccount == null ? 0 : HistoricoDoBP.RelatedAuxiliaryAccount.Code;
                    BPASerEditado.ResponsibleId = HistoricoDoBP.ResponsibleId;
                    BPASerEditado.ResponsibleName = HistoricoDoBP.RelatedResponsible == null ? null : HistoricoDoBP.RelatedResponsible.Name;
                    BPASerEditado.MetodoGet = true;
                    BPASerEditado.NumberIdentification = null;
                    BPASerEditado.MovementTypeId = HistoricoDoBP.MovementTypeId;
                    //BP VINDO DO SAM-ESTOQUE
                    BPASerEditado.AuxiliaryMovementTypeId = HistoricoDoBP.AuxiliaryMovementTypeId;

                    BuscaValorDepreciacaoPorGrupo(BPASerEditado);
                    CarregaViewBags(BPASerEditado);

                    ModelState.AddModelError("Chapa", "O campo Chapa é obrigatório.");

                    StringBuilder builder = new StringBuilder();
                    var _ids = integracaoAssetIdViewModels.Select(x => x.AssetId).ToArray();
                    foreach (var _id in _ids)
                    {
                        if (Array.IndexOf(_ids, _id) == _ids.Length - 1)
                            builder.Append(_id.ToString());
                        else
                        {
                            builder.Append(_id.ToString());
                            builder.Append(";");
                        }
                    }
                    BPASerEditado.assetIdsIntegracao = builder.ToString();

                    return View(BPASerEditado);
                }
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditIntegracao(AssetViewModel asset)
        {
            IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>();

            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                
                asset.MetodoGet = false;
                asset.DiferenciacaoChapa = asset.DiferenciacaoChapa ?? "";
                asset.DiferenciacaoChapaAntiga = (asset.DiferenciacaoChapaAntiga ?? "");

                if (string.IsNullOrWhiteSpace(asset.assetIdsIntegracao))
                {
                    ModelState.AddModelError("MovementTypeId", "Ação Inválida!");
                    asset.assetIdsIntegracao = asset.Id.ToString();
                    CarregaViewBags(asset);
                    return View("Edit", asset);
                }

                // Este Campo somente é obrigatorio quando for realizada a baixa do Bem Pendente.
                ModelState.Remove("MovementTypeId");

                if (string.IsNullOrEmpty(asset.ShortDescription))
                {
                    ModelState.AddModelError("ShortDescription", "Por favor, informe a Descrição Resumida do Bem");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if (!string.IsNullOrEmpty(asset.NumberDoc) && !string.IsNullOrWhiteSpace(asset.NumberDoc))
                {
                    if (asset.NumberDoc.Length > 14)
                    {
                        ModelState.AddModelError("NumberDoc", "Digite no máximo de 14 caracteres");
                        CarregaViewBags(asset);
                        return View(asset);
                    }
                }

                if (asset.AuxiliaryAccountId == null)
                {
                    ModelState.AddModelError("AuxiliaryAccountId", "Por favor, informe a Conta Contábil do Bem");
                    CarregaViewBags(asset);
                    return View(asset);
                }
                else
                {

                    int tipo = 0;
                    BuscaContasContabeis buscaContas = new BuscaContasContabeis();

                    tipo = asset.checkFlagAcervo == true ? 1 : (asset.checkFlagTerceiro == true ? 2 : 0);

                    if (tipo == 0)
                    {
                        if (!(from racmg in db.RelationshipAuxiliaryAccountItemGroups
                              join mg in db.MaterialGroups on racmg.MaterialGroupId equals mg.Id
                              where mg.Code == asset.MaterialGroupCode
                                    && racmg.AuxiliaryAccountId == asset.AuxiliaryAccountId
                              select racmg).Any()
                        )
                        {
                            ModelState.AddModelError("AuxiliaryAccountId", "Conta Contábil inválida.Por favor, informe a Conta Contábil do Bem");
                            CarregaViewBags(asset);
                            return View(asset);
                        }
                    }
                    else
                    {
                        if (!(from ac in db.AuxiliaryAccounts
                              where ac.RelacionadoBP == tipo
                                 && ac.Id == asset.AuxiliaryAccountId
                              select ac).Any())
                        {
                            ModelState.AddModelError("AuxiliaryAccountId", "Conta Contábil inválida.Por favor, informe a Conta Contábil do Bem");
                            CarregaViewBags(asset);
                            return View(asset);
                        }
                    }

                }

                if (asset.InitialId == 0 || (from b in db.Initials where b.Id == asset.InitialId && b.Description == "SIGLA_GENERICA" select b).Any())
                {
                    ModelState.AddModelError("Sigla", "Favor informar a Sigla correta!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if (asset.NumberIdentification == "CHAPA_GENERICA")
                {
                    ModelState.AddModelError("Chapa", "Favor informar a Chapa correta!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                int paraValidarChapa;
                if (!int.TryParse(asset.NumberIdentification, out paraValidarChapa))
                {
                    ModelState.AddModelError("Chapa", "Favor informar uma chapa numérica!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if ((from b in db.BudgetUnits where b.Id == asset.BudgetUnitId && b.Code == "99999" select b).Any())
                {
                    ModelState.AddModelError("UO", "Favor informar a UO correta!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if ((from b in db.ManagerUnits where b.Id == asset.ManagerUnitId && b.Code == "999999" select b).Any())
                {
                    ModelState.AddModelError("UGE", "Favor informar a UGE correta!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if ((from b in db.AdministrativeUnits where b.Id == asset.AdministrativeUnitId && b.Code == 99999999 select b).Any())
                {
                    ModelState.AddModelError("UA", "Favor informar a UA correta!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if ((from b in db.Sections where b.Id == asset.SectionId && b.Code == 999 select b).Any())
                {
                    ModelState.AddModelError("Divisao", "Favor informar a Divisão correta!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if ((from b in db.Responsibles where b.Id == asset.ResponsibleId && b.Name.Contains("RESPONSAVEL_") select b).Any())
                {
                    ModelState.AddModelError("Responsavel", "Favor informar o Responsavel correto!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if (asset.AcquisitionDate.ToString().Contains("31/12/9999"))
                {
                    ModelState.AddModelError("DataAquisicao", "Favor informar a Data Aquisição correta!");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if (asset.AcquisitionDate.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("DataAquisicao", "Por favor, informe uma data de Aquisição igual ou inferior a data atual.");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                var mensagemErro = ValidaDataDeIncorporacao(asset.MovimentDate, asset.ManagerUnitId, asset.AcquisitionDate.Date);
                if (mensagemErro != string.Empty)
                {
                    ModelState.AddModelError("MovimentDate", mensagemErro);
                    CarregaViewBags(asset);
                    return View(asset);
                }

                Asset _assetBase = BPAtivoComMesmaSiglaChapa(asset);

                if (_assetBase != null)
                {
                    int id = _assetBase.Id;
                    var sigla = (from i in db.Initials
                                 join a in db.Assets on i.Id equals a.InitialId
                                 where a.Id == id
                                 select i).FirstOrDefault();

                    var ugeParaMsg = (from m in db.ManagerUnits
                                      join a in db.Assets on m.Id equals a.ManagerUnitId
                                      where a.Id == id
                                      select m).FirstOrDefault();

                    ModelState.AddModelError("Chapa", "A Chapa: " + asset.NumberIdentification + asset.DiferenciacaoChapa + " e a Sigla: " + sigla.Name + " - " + sigla.Description + " já estão cadastrados na UGE: " + ugeParaMsg.Code + " - " + ugeParaMsg.Description);
                    CarregaViewBags(asset);
                    return View(asset);
                }

                if (string.IsNullOrEmpty(asset.ValueAcquisitionModel) || string.IsNullOrWhiteSpace(asset.ValueAcquisitionModel))
                {
                    ModelState.AddModelError("ValorAquisicao", "Valor de Aquisição é obrigatório");
                    CarregaViewBags(asset);
                    return View(asset);
                }

                // Tratamento para ACERVO E TERCEIRO
                if (asset.checkFlagAcervo == true || asset.checkFlagTerceiro == true)
                {
                    if ((string.IsNullOrEmpty(asset.ValueUpdateModel)) || (asset.ValueUpdateModel != null && Convert.ToDecimal(asset.ValueUpdateModel) == 0))
                    {
                        ModelState.AddModelError("ValueUpdateModel", "Por favor, informe o valor atual.");
                        CarregaViewBags(asset);
                        return View(asset);
                    }

                    // 11/10/2018
                    //if ((from b in db.AuxiliaryAccounts where b.Id == asset.AuxiliaryAccountId && b.Code == 0 select b).Any())
                    //{
                    //    ModelState.AddModelError("ContaAuxiliar", "Favor informar a Conta Contábil correta!");
                    //    CarregaViewBags(asset);
                    //    return View(asset);
                    //}

                    if (double.Parse(asset.ValueAcquisitionModel.ToString()) < 1)
                    {
                        ModelState.AddModelError("ValorAquisicao", "Valor não pode ser menor que 1,00, favor verificar!");
                        CarregaViewBags(asset);
                        return View(asset);
                    }

                    if (string.IsNullOrEmpty(asset.ShortDescription))
                    {
                        ModelState.AddModelError("ShortDescription", "Por favor, informe a Descrição Resumida do Bem");
                        CarregaViewBags(asset);
                        return View(asset);
                    }
                    ModelState.Remove("ShortDescriptionItemId");
                    ModelState.Remove("MaterialItemCode");
                    ModelState.Remove("MaterialItemDescription");
                }
                else
                {
                    if (!(from m in db.MaterialGroups where m.Code == asset.MaterialGroupCode select m.Id).Any())
                    {
                        ModelState.AddModelError("ItemMaterial", "Item material ou Grupo material não foram encontrados, favor verificar!");
                        CarregaViewBags(asset);
                        return View(asset);
                    }
                    if (asset.MaterialItemCode == 999999999)
                    {
                        ModelState.AddModelError("ItemMaterial", "Favor realizar a pesquisa do item material atualizado!");
                        CarregaViewBags(asset);
                        return View(asset);
                    }

                    //Dados somente para acervo ou terceiro
                    if (asset.MaterialItemCode == 5628121 ||
                        asset.MaterialItemCode == 5628156)
                    {
                        ModelState.AddModelError("ItemMaterial", "Favor realizar uma pesquisa com item material atualizado!");
                        CarregaViewBags(asset);
                        return View(asset);
                    }

                    if (!string.IsNullOrEmpty(asset.MaterialItemDescription) &&
                        !string.IsNullOrWhiteSpace(asset.MaterialItemDescription))
                    {
                        if (asset.MaterialItemDescription.Trim() == "Acervos" ||
                            asset.MaterialItemDescription.Trim() == "Bens de Terceiros")
                        {
                            ModelState.AddModelError("ItemMaterial", "Favor realizar uma pesquisa com item material atualizado!");
                            CarregaViewBags(asset);
                            return View(asset);
                        }
                    }


                    //VALIDACAO FLAG DECRETO
                    if (asset.checkFlagDecretoSefaz)
                    {

                        if (string.IsNullOrEmpty(asset.ValueUpdateModel) || string.IsNullOrWhiteSpace(asset.ValueUpdateModel))
                        {
                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado é obrigatório");
                            CarregaViewBags(asset);
                            return View(asset);
                        }

                        if (Convert.ToDecimal(asset.ValueUpdateModel) < Convert.ToDecimal("10,00"))
                        {
                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado não pode ser inferior a R$10,00");
                            CarregaViewBags(asset);
                            return View(asset);
                        }

                        if (Convert.ToDecimal(asset.ValueUpdateModel) <= Convert.ToDecimal(asset.ValueAcquisitionModel))
                        {
                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado de um decreto deve ser superior ao Valor de Aquisição");
                            CarregaViewBags(asset);
                            return View(asset);
                        }


                    }
                    else
                    {
                        if (double.Parse(asset.ValueAcquisitionModel.ToString()) < 10)
                        {
                            ModelState.AddModelError("ValorAquisicao", "Valor não pode ser menor que 10,00, favor verificar!");
                            CarregaViewBags(asset);
                            return View(asset);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
                    if (asset.checkFlagAcervo)
                    {
                        asset.MaterialItemCode = 5628156;
                        asset.MaterialItemDescription = "Acervos";
                        asset.MaterialGroupCode = 99;
                        asset.LifeCycle = 0;
                        asset.RateDepreciationMonthly = 0;
                        asset.ResidualValue = 0;
                    }

                    var _ShortDescription = (from a in db.ShortDescriptionItens where a.Description == asset.ShortDescription select a).FirstOrDefault();

                    if (_ShortDescription.IsNotNull())
                    {
                        asset.ShortDescriptionItemId = _ShortDescription.Id;
                        asset.MaterialItemDescription = _ShortDescription.Description;
                    }
                    else
                    {
                        this.db.Configuration.AutoDetectChangesEnabled = false;
                        this.db.Configuration.LazyLoadingEnabled = false;

                        ShortDescriptionItem _shortDescriptionItem = new ShortDescriptionItem();
                        _shortDescriptionItem.Description = asset.ShortDescription;
                        db.Entry(_shortDescriptionItem).State = EntityState.Added;
                        db.SaveChanges();

                        asset.ShortDescriptionItemId = _shortDescriptionItem.Id;
                        asset.MaterialItemDescription = _shortDescriptionItem.Description;

                        this.db.Configuration.AutoDetectChangesEnabled = true;
                        this.db.Configuration.LazyLoadingEnabled = true;
                    }

                    VerificaGrupoMaterial(asset.MaterialGroupCode);

                    Asset _asset = db.Assets.Find(asset.Id);
                    AssetMovements _assetMovements = db.AssetMovements.Find(asset.AssetMovementsId);
                    _asset = ConverterAssetViewModeParaAsset(asset, ref _asset);
                    _assetMovements = ConverterAssetViewModelParaAssetMovements(asset, ref _assetMovements);

                    _asset.Status = true;
                    _asset.Login = UserCommon.CurrentUser().CPF;
                    _asset.DataLogin = DateTime.Now;
                    _asset.InitialId = asset.InitialId;
                    _asset.InitialName = (from i in db.Initials where i.Id == asset.InitialId select i.Name).FirstOrDefault();
                    _asset.NumberIdentification = asset.NumberIdentification;
                    _asset.DiferenciacaoChapa = asset.DiferenciacaoChapa;
                    _asset.AcquisitionDate = asset.AcquisitionDate;
                    _asset.MovimentDate = asset.MovimentDate;
                    _asset.ValueAcquisition = Convert.ToDecimal(asset.ValueAcquisitionModel);
                    _asset.MaterialItemCode = asset.MaterialItemCode;
                    _asset.flagAcervo = asset.checkFlagAcervo;
                    _asset.flagTerceiro = asset.checkFlagTerceiro;
                    _asset.flagDepreciaAcumulada = 1;
                    _asset.flagVerificado = null;
                    _asset.flagVindoDoEstoque = true;
                    _asset.IndicadorOrigemInventarioInicial = (byte)EnumOrigemInventarioInicial.IntegracaoComEstoque;
                    if (_asset.MaterialGroupCode != 88)
                        _asset.flagAnimalNaoServico = false;

                    //INCLUSAO DA DATA DE AQUISICAO NA MOVIMENTACAO DO BP
                    _assetMovements.MovimentDate = asset.MovimentDate;

                    db.Entry(_asset).State = EntityState.Modified;
                    db.Entry(_assetMovements).State = EntityState.Modified;
                    db.SaveChanges();

                    DepreciacaoNaIncorporacao objDepreciacao = new DepreciacaoNaIncorporacao(db);
                    int PrimeiroAssetStartId = 0;

                    var arrayIds = asset.assetIdsIntegracao.Split(';');

                    for (int i = 0; i <= arrayIds.Length - 1; i++)
                    {
                        int _id = int.Parse(arrayIds[i]);
                        _asset = ConverterAssetViewModeParaAsset(asset, ref _asset);

                        db.Entry(_asset).State = EntityState.Detached;
                        _asset.NumberIdentification = (int.Parse(asset.NumberIdentification) + i).ToString();
                        _asset.Id = _id;
                        _asset.flagDepreciaAcumulada = 1;
                        _asset.flagVerificado = null;
                        _asset.flagVindoDoEstoque = true;
                        _asset.IndicadorOrigemInventarioInicial = (byte)EnumOrigemInventarioInicial.IntegracaoComEstoque;
                        if (_asset.MaterialGroupCode != 88)
                            _asset.flagAnimalNaoServico = false;

                        db.Entry(_asset).State = EntityState.Modified;
                        db.SaveChanges();

                        IQueryable<int> query = (from q in db.AssetMovements where q.AssetId == _id select q.Id);
                        var assetMovementsId = query.ToList();
                        foreach (var assetMovementId in assetMovementsId)
                        {
                            db.Entry(_assetMovements).State = EntityState.Detached;
                            _assetMovements = ConverterAssetViewModelParaAssetMovements(asset, ref _assetMovements);
                            _assetMovements.Id = assetMovementId;
                            _assetMovements.AssetId = _id;

                            db.Entry(_assetMovements).State = EntityState.Modified;
                            db.SaveChanges();

                            //INCLUSAO PARA CAPTURA DE MOVIMENTACAO PATRIMONIAL EM LOTE
                            listaMovimentacoesPatrimoniaisEmLote.Add(_assetMovements);
                        }

                        if (PrimeiroAssetStartId == 0)
                        {
                            objDepreciacao.Deprecia(_asset, _assetMovements);
                            PrimeiroAssetStartId = (int)_asset.AssetStartId;
                        }
                        else {
                            objDepreciacao.CopiaDepreciacaoNaIncorporacaoEmLote(_asset, _assetMovements, PrimeiroAssetStartId);
                        }
                    }

                    ComputaAlteracaoContabil(_assetMovements.InstitutionId, _assetMovements.BudgetUnitId,
                                             _assetMovements.ManagerUnitId, (int)_assetMovements.AuxiliaryAccountId);

                    db.Database.CurrentTransaction.Commit();

                    //return RedirectToAction("Integrating");
                    base.initDadosSIAFEM();
                    var tipoIncorporacao = db.MovementTypes.AsNoTracking().Where(tipoMovimentacao => tipoMovimentacao.Id == _assetMovements.MovementTypeId).FirstOrDefault();
                    int mesReferenciaMovimentacao = -1;
                    string strMesReferenciaMovimentacao = _assetMovements.MovimentDate.ToString("yyyyMM");
                    Int32.TryParse(strMesReferenciaMovimentacao, out mesReferenciaMovimentacao);
                    var tipoIncorporacao_Origem__SAM_Estoque__MesRef_Maior_202102 = ((tipoIncorporacao.Id == EnumMovimentType.IncorpIntegracaoSAMEstoque_SAMPatrimonio.GetHashCode()) && (mesReferenciaMovimentacao >= 202102));
                    var dataInicioIntegracao = new DateTime(2020, 06, 01);
                    bool MovimentacaoIntegradoAoSIAFEM = (tipoIncorporacao.IsNotNull() && tipoIncorporacao.PossuiContraPartidaContabil() && base.ugeIntegradaSiafem && asset.MovimentDate >= dataInicioIntegracao);
                    //if (MovimentacaoIntegradoAoSIAFEM)
                    if (MovimentacaoIntegradoAoSIAFEM && tipoIncorporacao_Origem__SAM_Estoque__MesRef_Maior_202102)
                    {
                        var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                        var tuplas = svcIntegracaoSIAFEM.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovimentacoesPatrimoniaisEmLote, true);

                        DadosPopupContabiliza objPopUp = new DadosPopupContabiliza();

                        objPopUp.MsgSIAFEM = new List<string>();
                        objPopUp.ListaIdAuditoria = string.Empty;
                        objPopUp.LoginSiafem = asset.LoginSiafem;
                        objPopUp.SenhaSiafem = asset.SenhaSiafem;

                        foreach (var item in tuplas)
                        {
                            objPopUp.MsgSIAFEM.Add(item.Item1);
                            objPopUp.ListaIdAuditoria += item.Item3.ToString() + ",";
                        }

                        objPopUp.ListaIdAuditoria += "0";

                        return View("_VazioBaseParaPopupsSIAFEM", objPopUp);
                    }
                    else
                    {
                        string msgInformeAoUsuario = null;
                        //EXIBICAO DE POP-UP COM AS INFORMACOES REFERENTES A MOVIMENTACAO PATRIMONIAL RECEM-EFETIVADA (MAIS INFORMACOES ENVIADAS/RETORNADAS PARA/DE SISTEMA CONTABILIZA-SP)
                        var loginUsuarioSIAFEM = asset.LoginSiafem;
                        var senhaUsuarioSIAFEM = asset.SenhaSiafem;
                        msgInformeAoUsuario = base.processamentoMovimentacaoPatrimonialNoContabilizaSP(asset.MovementTypeId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, asset.NumberDoc, listaMovimentacoesPatrimoniaisEmLote);
                        msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();
                        TempData["msgSucesso"] = msgInformeAoUsuario;
                        TempData["numeroDocumento"] = asset.NumberDoc;
                        TempData.Keep();

                        return RedirectToAction("Integrating");
                    }
                }
                CarregaViewBags(asset);
                return View(asset);
            }
            catch (Exception ex)
            {
                if (db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
            finally
            {
                if (db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();
            }
        }

        [HttpGet]
        public async Task<JsonResult> ImportarIntegracao()
        {
            getHierarquiaPerfil();
            if (_managerUnitId.HasValue)
            {
                var ManagerUnit = db.ManagerUnits.Where(x => x.Id == _managerUnitId).AsNoTracking().FirstOrDefault();
                if (ManagerUnit != null)
                {
                    int? managerUnitCode = int.Parse(ManagerUnit.Code);
                    var retorno = await apiMovimentoIntegracaoController.CreateIntegracao(User.Identity.Name, managerUnitCode, ManagerUnit.Code);
                    return Json(retorno, JsonRequestBehavior.AllowGet);
                }

            }
            return Json("", JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public async Task<JsonResult> ReverseIntegrating(String assetIds, string numeroDocumento)
        {

            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            db.Database.CommandTimeout = 4000;
            try
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                IList<IntegracaoAssetIdViewModel> _assetIds = json_serializer.Deserialize<IList<IntegracaoAssetIdViewModel>>(assetIds);

                foreach (var integracao in _assetIds)
                {
                    Asset asset = db.Assets.Where(x => x.Id == integracao.AssetId).AsNoTracking().FirstOrDefault();
                    AssetMovements assetMovements = db.AssetMovements.Where(x => x.AssetId == integracao.AssetId).AsNoTracking().FirstOrDefault();
                    assetMovements.MovementTypeId = (int)EnumMovimentType.BemIncorporadoViaIntegracaoEstoque;
                    RegistraBPExcluido(asset, assetMovements, numeroDocumento);

                }

                return await Task.Run(() => Json(new { status = "Sucesso", Mensagem = "Estorno realizado com sucesso!", MensagemErro = "" }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception ex)
            {

                return await Task.Run(() => Json(new { status = "Erro", Mensagem = "Por favor, contacte suporte!", MensagemErro = ex.Message }, JsonRequestBehavior.AllowGet));
            }
        }
        public async Task<ActionResult> Integrating(string sortOrder, string searchString, string currentFilter, int? page, string cbStatus, string cbFiltros)
        {
            try
            {
                getHierarquiaPerfil();

                if (this._managerUnitId.HasValue)
                {
                    string codigoUge = db.Database.SqlQuery<string>("SELECT TOP 1 [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = " + this._managerUnitId.Value.ToString()).FirstOrDefault();
                    var retorno = await apiMovimentoIntegracaoController.CreateIntegracao(User.Identity.Name, int.Parse(codigoUge), codigoUge);
                }

                IQueryable<AssetEAssetMovimentViewModel> query = null;
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
                if (PerfilAdmGeral() == true)
                {

                    query = (from a in db.Assets
                             join am in db.AssetMovements
                             on a.Id equals am.AssetId
                             where a.Status == true &&
                                   a.flagVerificado.HasValue &&
                                   a.flagDepreciaAcumulada != 1
                                   && a.flagVindoDoEstoque == true
                             select new AssetEAssetMovimentViewModel
                             {
                                 Asset = a,
                                 AssetMoviment = am,
                                 codigo_descricao_uge_emissor_transferencia = a.RelatedManagerUnit.Code,
                                 codigo_descricao_ua_emissor_transferencia = am.RelatedAdministrativeUnit.Code.ToString(),
                                 codigo_uge_almox_emissor_transferencia = (am.RelatedSourceDestinyManagerUnit != null && am.RelatedSourceDestinyManagerUnit.Code != null ? am.RelatedSourceDestinyManagerUnit.Code : "")
                             }).AsNoTracking();

                }
                else
                {
                    query = (from a in db.Assets
                             join am in db.AssetMovements
                             on a.Id equals am.AssetId
                             where am.Status == true &&
                                   a.flagVerificado.HasValue &&
                                   a.flagDepreciaAcumulada != 1 &&
                                   a.flagVindoDoEstoque == true &&
                                   am.InstitutionId == _institutionId &&
                                   am.ManagerUnitId == (_managerUnitId.HasValue ? _managerUnitId : am.ManagerUnitId) &&
                                   am.BudgetUnitId == (_budgetUnitId.HasValue ? _budgetUnitId : am.BudgetUnitId)
                             select new AssetEAssetMovimentViewModel
                             {
                                 Asset = a,
                                 AssetMoviment = am,
                                 codigo_descricao_uge_emissor_transferencia = a.RelatedManagerUnit.Code,
                                 codigo_descricao_ua_emissor_transferencia = am.RelatedAdministrativeUnit.Code.ToString(),
                                 codigo_uge_almox_emissor_transferencia = (am.RelatedSourceDestinyManagerUnit != null && am.RelatedSourceDestinyManagerUnit.Code != null ? am.RelatedSourceDestinyManagerUnit.Code : "")
                             }).AsNoTracking();
                }


                //Guarda o filtro
                ViewBag.CurrentFilter = searchString;
                ViewBag.CurrentFilterCbFiltros = cbFiltros;

                //Filter
                query = SearchByFilter(searchString == null ? searchString : searchString.Trim(), query, cbFiltros);

                //Carrega combo de filtros
                CarregaComboFiltros(cbFiltros);

                //Pagination
                int pageSize2 = 10;
                int pageNumber2 = (page ?? 1);

                var result2 = query.OrderBy(s => s.Asset.Id).Skip(((pageNumber2) - 1) * pageSize2).Take(pageSize2).AsNoTracking();
                var resultado = result2.ToList();
                var listaparaCount2 = (from a in query select a.Asset.Id);
                int contador2 = listaparaCount2.Count();

                var retorno2 = new StaticPagedList<AssetEAssetMovimentViewModel>(resultado, pageNumber2, pageSize2, contador2);
                db.Database.CurrentTransaction.Commit();
                return View(retorno2);

            }
            catch (Exception ex)
            {
                if (db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();

                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion Integração

        #region Details

        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                var asset = (from a in db.Assets
                             join am in db.AssetMovements
                             on a.Id equals am.AssetId
                             where am.Status == true &&
                                   a.flagVerificado.HasValue &&
                                   a.flagDepreciaAcumulada != 1 &&
                                   am.AssetId == id
                             select new AssetEAssetMovimentViewModel
                             {
                                 Asset = a,
                                 AssetMoviment = am
                             }).FirstOrDefault();

                if (asset == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (ValidarRequisicao(asset.AssetMoviment.InstitutionId, null, null, null, null))
                    return View(asset);
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                AssetViewModel BPASerEditado = (from a in db.Assets
                                                join am in db.AssetMovements
                                                on a.Id equals am.AssetId
                                                where a.flagVerificado.HasValue &&
                                                      a.flagDepreciaAcumulada != 1 &&
                                                      a.Id == id
                                                select new AssetViewModel
                                                {
                                                    Id = a.Id,
                                                    InitialId = a.InitialId,
                                                    InitialDescription = a.RelatedInitial == null ? null : a.RelatedInitial.Description,
                                                    NumberIdentification = a.NumberIdentification,
                                                    AcquisitionDate = a.AcquisitionDate,
                                                    //t.MovimentDate = a.MovimentDate,
                                                    ValueAcquisition = a.ValueAcquisition,
                                                    MaterialItemCode = a.MaterialItemCode,
                                                    MaterialItemDescription = a.MaterialItemDescription,
                                                    MaterialGroupCode = a.MaterialGroupCode,
                                                    MaterialGroupDescription = (from m in db.MaterialGroups where m.Code == a.MaterialGroupCode select m.Description).FirstOrDefault(),
                                                    ShortDescription = a.RelatedShortDescriptionItem == null ? null : a.RelatedShortDescriptionItem.Description,
                                                    LifeCycle = a.LifeCycle,
                                                    RateDepreciationMonthly = a.RateDepreciationMonthly,
                                                    ResidualValue = a.ResidualValue,
                                                    ShortDescriptionItemId = a.ShortDescriptionItemId,
                                                    OldInitial = a.OldInitial,
                                                    OldNumberIdentification = a.OldNumberIdentification,
                                                    DiferenciacaoChapaAntiga = a.DiferenciacaoChapaAntiga,
                                                    NumberDoc = a.NumberDoc,
                                                    checkFlagAcervo = a.flagAcervo == null ? false : (bool)a.flagAcervo,
                                                    checkFlagTerceiro = a.flagTerceiro == null ? false : (bool)a.flagTerceiro,
                                                    checkFlagDecretoSefaz = a.flagDecreto == null ? false : (bool)a.flagDecreto,
                                                    checkflagAnimalAServico = a.flagAnimalNaoServico == null ? true : !(bool)a.flagAnimalNaoServico,
                                                    SerialNumber = a.SerialNumber,
                                                    DateGuarantee = a.DateGuarantee,
                                                    Brand = a.Brand,
                                                    NumberPlate = a.NumberPlate,
                                                    ManufactureDate = a.ManufactureDate,
                                                    ChassiNumber = a.ChassiNumber,
                                                    Model = a.Model,
                                                    AdditionalDescription = a.AdditionalDescription,
                                                    flagVerificado = a.flagVerificado,
                                                    ValueUpdateModel = a.ValueUpdate == null ? "0" : a.ValueUpdate.ToString()
                                                }).FirstOrDefault();

                if (BPASerEditado == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                AssetMovements HistoricoDoBP = (from am in db.AssetMovements where am.AssetId == id && am.Status == true select am).AsNoTracking().FirstOrDefault();

                if (ValidarRequisicao(HistoricoDoBP.InstitutionId, null, null, null, null))
                {
                    ViewBag.StateConservationDescription = (from s in db.StateConservations
                                                            where s.Id == HistoricoDoBP.StateConservationId
                                                            select s.Description).FirstOrDefault();

                    BPASerEditado.ValueAcquisitionModel = BPASerEditado.ValueAcquisition.ToString();
                    BPASerEditado.ManagerUnitId = HistoricoDoBP.ManagerUnitId;
                    BPASerEditado.AssetMovementsId = HistoricoDoBP.Id;
                    BPASerEditado.StateConservationId = HistoricoDoBP.StateConservationId;
                    BPASerEditado.NumberPurchaseProcess = HistoricoDoBP.NumberPurchaseProcess;
                    BPASerEditado.InstitutionId = HistoricoDoBP.InstitutionId;
                    BPASerEditado.BudgetUnitId = HistoricoDoBP.BudgetUnitId;
                    BPASerEditado.BudgetUnitCode = HistoricoDoBP.RelatedBudgetUnit.Code;
                    BPASerEditado.ManagerUnitId = HistoricoDoBP.ManagerUnitId;
                    BPASerEditado.ManagerUnitCode = HistoricoDoBP.RelatedManagerUnit.Code;
                    BPASerEditado.AdministrativeUnitId = HistoricoDoBP.AdministrativeUnitId;
                    BPASerEditado.AdministrativeUnitCode = HistoricoDoBP.RelatedAdministrativeUnit == null ? 0 : HistoricoDoBP.RelatedAdministrativeUnit.Code;
                    BPASerEditado.SectionId = HistoricoDoBP.SectionId;
                    BPASerEditado.SectionCode = HistoricoDoBP.RelatedSection == null ? 0 : HistoricoDoBP.RelatedSection.Code;
                    BPASerEditado.AuxiliaryAccountId = HistoricoDoBP.AuxiliaryAccountId;
                    BPASerEditado.AuxiliaryAccountCode = HistoricoDoBP.RelatedAuxiliaryAccount == null ? 0 : HistoricoDoBP.RelatedAuxiliaryAccount.Code;
                    BPASerEditado.ResponsibleId = HistoricoDoBP.ResponsibleId;
                    BPASerEditado.ResponsibleName = HistoricoDoBP.RelatedResponsible == null ? null : HistoricoDoBP.RelatedResponsible.Name;
                    BPASerEditado.MetodoGet = true;

                    if (!BPASerEditado.checkFlagAcervo && !BPASerEditado.checkFlagTerceiro && !BPASerEditado.checkFlagDecretoSefaz)
                        BPASerEditado.ValueUpdateModel = null;

                    CarregaViewBags(BPASerEditado);

                    return View(BPASerEditado);
                }
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);

            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AssetViewModel asset)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                asset.MetodoGet = false;
                asset.DiferenciacaoChapa = asset.DiferenciacaoChapa ?? "";
                asset.DiferenciacaoChapaAntiga = asset.DiferenciacaoChapaAntiga ?? "";

                if (asset.checkDepreciacao)
                {
                    if (asset.MovementTypeId != 0)
                    {
                        Asset BPASerExcluido = db.Assets.Find(asset.Id);
                        AssetMovements HistoricoASerExcluido = db.AssetMovements.Find(asset.AssetMovementsId);

                        BPASerExcluido.Status = false;
                        BPASerExcluido.Login = UserCommon.CurrentUser().CPF;
                        BPASerExcluido.DataLogin = DateTime.Now;

                        HistoricoASerExcluido.Status = false;
                        HistoricoASerExcluido.MovementTypeId = asset.MovementTypeId;
                        HistoricoASerExcluido.Login = UserCommon.CurrentUser().CPF;
                        HistoricoASerExcluido.DataLogin = DateTime.Now;

                        if (BPASerExcluido.MaterialItemDescription.IsNullOrEmpty())
                        {
                            BPASerExcluido.MaterialItemDescription = "SEM DESCRICAO";
                        }

                        db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
                        db.Entry(BPASerExcluido).State = EntityState.Modified;
                        db.Entry(HistoricoASerExcluido).State = EntityState.Modified;
                        db.SaveChanges();

                        RegistraBPExcluido(BPASerExcluido, HistoricoASerExcluido);

                        db.Database.CurrentTransaction.Commit();

                        return RedirectToAction("Index");
                    }

                    CarregaViewBags(asset);
                    return View(asset);
                }

                // Este Campo somente é obrigatorio quando for realizada a baixa do Bem Pendente.
                ModelState.Clear();

                asset.flagVerificado = 0;

                if (string.IsNullOrEmpty(asset.ShortDescription))
                {
                    ModelState.AddModelError("ShortDescription", "Por favor, informe a Descrição Resumida do Bem");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteDescricao;
                }

                if (!string.IsNullOrEmpty(asset.NumberDoc) && !string.IsNullOrWhiteSpace(asset.NumberDoc))
                {
                    if (asset.NumberDoc.Length > 14)
                    {
                        ModelState.AddModelError("NumberDoc", "Digite no máximo de 14 caracteres");
                    }
                }

                if (asset.AuxiliaryAccountId == null)
                {
                    ModelState.AddModelError("AuxiliaryAccountId", "Por favor, informe a Conta Contábil do Bem");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteContaContabil;
                }
                else
                {

                    int tipo = 0;
                    BuscaContasContabeis buscaContas = new BuscaContasContabeis();

                    tipo = asset.checkFlagAcervo == true ? 1 : (asset.checkFlagTerceiro == true ? 2 : 0);

                    if (tipo == 0)
                    {
                        if (!(from racmg in db.RelationshipAuxiliaryAccountItemGroups
                              join mg in db.MaterialGroups on racmg.MaterialGroupId equals mg.Id
                              where mg.Code == asset.MaterialGroupCode
                                    && racmg.AuxiliaryAccountId == asset.AuxiliaryAccountId
                              select racmg).Any()
                        )
                        {
                            ModelState.AddModelError("AuxiliaryAccountId", "Conta Contábil inválida.Por favor, informe a Conta Contábil do Bem");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.ContaContabilInvalida;
                        }
                    }
                    else
                    {
                        if (!(from ac in db.AuxiliaryAccounts
                              where ac.RelacionadoBP == tipo
                                 && ac.Id == asset.AuxiliaryAccountId
                              select ac).Any())
                        {
                            ModelState.AddModelError("AuxiliaryAccountId", "Conta Contábil inválida.Por favor, informe a Conta Contábil do Bem");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.ContaContabilInvalida;
                        }
                    }

                }

                if (asset.InitialId == 0 || (from b in db.Initials where b.Id == asset.InitialId && b.Description == "SIGLA_GENERICA" select b).Any())
                {
                    ModelState.AddModelError("Sigla", "Favor informar a Sigla correta!");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteSigla;
                }

                if (string.IsNullOrEmpty(asset.NumberIdentification) || string.IsNullOrWhiteSpace(asset.NumberIdentification))
                {
                    ModelState.AddModelError("Chapa", "Favor informar a Chapa!");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteChapa;
                }
                else {
                    if (asset.NumberIdentification == "CHAPA_GENERICA")
                    {
                        ModelState.AddModelError("Chapa", "Favor informar a Chapa correta!");
                        asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteChapa;
                    }

                    int paraValidarChapa;
                    if (!int.TryParse(asset.NumberIdentification, out paraValidarChapa)) {
                        ModelState.AddModelError("Chapa", "Favor informar uma chapa numérica!");
                        asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteChapa;
                    }
                }

                if ((from b in db.BudgetUnits where b.Id == asset.BudgetUnitId && b.Code == "99999" select b).Any())
                {
                    ModelState.AddModelError("UO", "Favor informar a UO correta!");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteUO;
                }

                //Se não for escolihda a UGE, o sistema só apresenta as mensagens na tela
                // (não salva do banco devido chaves estrangeiras)
                if (asset.ManagerUnitId == 0)
                {
                    ModelState.AddModelError("UGE", "Favor informar a UGE correta!");
                    ModelState.AddModelError("UA", "Favor informar a UA correta!");
                    ModelState.AddModelError("Responsavel", "Favor informar o Responsavel correto!");
                }
                else
                {
                    //Se não for escolihda a UA, o sistema só apresenta as mensagens na tela
                    // (não salva do banco devido chaves estrangeiras)
                    if (asset.AdministrativeUnitId == 0)
                    {
                        ModelState.AddModelError("UA", "Favor informar a UA correta!");
                        ModelState.AddModelError("Responsavel", "Favor informar o Responsavel correto!");
                    }
                    else
                    {

                        if ((from b in db.ManagerUnits where b.Id == asset.ManagerUnitId && b.Code == "999999" select b).Any())
                        {
                            ModelState.AddModelError("UGE", "Favor informar a UGE correta!");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteUGE;
                        }

                        if ((from b in db.AdministrativeUnits where b.Id == asset.AdministrativeUnitId && b.Code == 99999999 select b).Any())
                        {
                            ModelState.AddModelError("UA", "Favor informar a UA correta!");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteUA;
                        }

                        if ((from b in db.Sections where b.Id == asset.SectionId && b.Code == 999 select b).Any())
                        {
                            ModelState.AddModelError("Divisao", "Favor informar a Divisão correta!");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteDivisao;
                        }

                        if (asset.ResponsibleId == null || (from b in db.Responsibles where b.Id == asset.ResponsibleId && b.Name.Contains("RESPONSAVEL_") select b).Any())
                        {
                            ModelState.AddModelError("Responsavel", "Favor informar o Responsavel correto!");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteResponsavel;
                        }

                    }

                    //Validação da data de movimento, somente feita quando UGE for selecionada
                    var mensagemErro = ValidaDataDeIncorporacao(asset.MovimentDate, asset.ManagerUnitId, asset.AcquisitionDate);
                    if (mensagemErro != string.Empty)
                    {
                        ModelState.AddModelError("MovimentDate", mensagemErro);
                    }
                }

                if (asset.AcquisitionDate.ToString().Contains("31/12/9999"))
                {
                    ModelState.AddModelError("DataAquisicao", "Favor informar a Data Aquisição correta!");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.DataAquisicaoInvalida;
                }

                if (asset.AcquisitionDate.Date > DateTime.Now.Date || DateTime.Compare(asset.AcquisitionDate.Date, new DateTime(1900, 1, 1)) <= 0)
                {
                    ModelState.AddModelError("DataAquisicao", "Por favor, informe uma data de Aquisição igual ou inferior a data atual.");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteDataAquisicao;
                }

                if (DateTime.Compare(asset.AcquisitionDate, new DateTime(1900, 1, 1)) <= 0) {
                    ModelState.AddModelError("DataAquisicao", "Favor informar a Data Aquisição correta!");
                }


                Asset _assetBase = BPAtivoComMesmaSiglaChapa(asset);
                
                if (_assetBase != null)
                {
                    int id = _assetBase.Id;
                    var sigla = (from i in db.Initials
                                 join a in db.Assets on i.Id equals a.InitialId
                                 where a.Id == id
                                 select i).FirstOrDefault();

                    var ugeParaMsg = (from m in db.ManagerUnits
                                      join a in db.Assets on m.Id equals a.ManagerUnitId
                                      where a.Id == id
                                      select m).FirstOrDefault();

                    ModelState.AddModelError("Chapa", "A Chapa: " + asset.NumberIdentification + asset.DiferenciacaoChapa + " e a Sigla: " + sigla.Name + " - " + sigla.Description + " já estão cadastrados na UGE: " + ugeParaMsg.Code + " - " + ugeParaMsg.Description);
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.ChapaRepetidaNoOrgao;
                }

                if (string.IsNullOrEmpty(asset.ValueAcquisitionModel) || string.IsNullOrWhiteSpace(asset.ValueAcquisitionModel))
                {
                    asset.ValueAcquisitionModel = "0";
                    ModelState.AddModelError("ValorAquisicao", "Valor de Aquisição é obrigatório");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteValorAquisicao;
                }

                if (asset.StateConservationId == 0) {
                    ModelState.AddModelError("StateConservationId", "Estado de Conservação é obrigatório");
                    asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteEstadoConservacao;
                }

                // Tratamento para ACERVO E TERCEIRO
                if (asset.checkFlagAcervo == true || asset.checkFlagTerceiro == true)
                {
                    if ((string.IsNullOrEmpty(asset.ValueUpdateModel)) || (asset.ValueUpdateModel != null && Convert.ToDecimal(asset.ValueUpdateModel) == 0))
                    {
                        ModelState.AddModelError("ValueUpdateModel", "Por favor, informe o valor atual.");
                        asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteValorAtual;
                    }
                    else
                    {

                        // 11/10/2018
                        //if ((from b in db.AuxiliaryAccounts where b.Id == asset.AuxiliaryAccountId && b.Code == 0 select b).Any())
                        //{
                        //    ModelState.AddModelError("ContaAuxiliar", "Favor informar a Conta Contábil correta!");
                        //    CarregaViewBags(asset);
                        //    return View(asset);
                        //}

                        if (double.Parse(asset.ValueAcquisitionModel.ToString()) < 1)
                        {
                            ModelState.AddModelError("ValorAquisicao", "Valor não pode ser menor que 1,00, favor verificar!");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteValorAquisicao;
                        }

                        if (string.IsNullOrEmpty(asset.ShortDescription) || string.IsNullOrWhiteSpace(asset.ShortDescription))
                        {
                            ModelState.AddModelError("ShortDescription", "Por favor, informe a Descrição Resumida do Bem");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteDescricao;
                        }
                    }

                    asset.LifeCycle = 0;
                    asset.RateDepreciationMonthly = 0;
                    asset.ResidualValue = 0;
                    asset.MaterialGroupCode = 99;

                    if (asset.checkFlagAcervo)
                    {
                        asset.MaterialItemCode = 5628156;
                        asset.MaterialItemDescription = "Acervos";
                    }
                    else if (asset.checkFlagTerceiro)
                    {
                        asset.MaterialItemCode = 5628121;
                        asset.MaterialItemDescription = "Bens de Terceiros";
                    }
                }
                else
                {
                    if (!(from m in db.MaterialGroups where m.Code == asset.MaterialGroupCode select m.Id).Any())
                    {
                        ModelState.AddModelError("ItemMaterial", "Item material ou Grupo material não foram encontrados, favor verificar!");
                        asset.flagVerificado = (int)EnumPendenciasDadosBP.CodigoMaterialOuGrupoMaterialInvalido;
                    }
                    if (asset.MaterialItemCode == 999999999)
                    {
                        ModelState.AddModelError("ItemMaterial", "Favor realizar a pesquisa do item material atualizado!");
                        asset.flagVerificado = (int)EnumPendenciasDadosBP.CodigoMaterialOuGrupoMaterialInvalido;
                    }
                    //Dados somente para acervo ou terceiro
                    if (asset.MaterialItemCode == 5628121 ||
                        asset.MaterialItemCode == 5628156)
                    {
                        ModelState.AddModelError("ItemMaterial", "Favor realizar a pesquisa do item material atualizado!");
                        asset.flagVerificado = (int)EnumPendenciasDadosBP.CodigoMaterialOuGrupoMaterialInvalido;
                    }

                    if (!string.IsNullOrEmpty(asset.MaterialItemDescription) &&
                        !string.IsNullOrWhiteSpace(asset.MaterialItemDescription))
                    {
                        if (asset.MaterialItemDescription.Trim() == "Acervos" ||
                            asset.MaterialItemDescription.Trim() == "Bens de Terceiros")
                        {
                            ModelState.AddModelError("ItemMaterial", "Favor realizar a pesquisa do item material atualizado!");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.CodigoMaterialOuGrupoMaterialInvalido;
                        }
                    }

                    //VALIDACAO FLAG DECRETO
                    if (asset.checkFlagDecretoSefaz)
                    {
                        if (string.IsNullOrEmpty(asset.ValueUpdateModel) || string.IsNullOrWhiteSpace(asset.ValueUpdateModel))
                        {
                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado é obrigatório");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteValorAquisicao;
                        }

                        if (Convert.ToDecimal(asset.ValueUpdateModel) < Convert.ToDecimal("10,00"))
                        {
                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado não pode ser inferior a R$10,00");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteValorAtual;
                        }

                        if (Convert.ToDecimal(asset.ValueUpdateModel) <= Convert.ToDecimal(asset.ValueAcquisitionModel))
                        {
                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado de um decreto deve ser superior ao Valor de Aquisição");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteValorAtual;
                        }

                    }
                    else
                    {
                        if (double.Parse(asset.ValueAcquisitionModel.ToString()) < 10)
                        {
                            ModelState.AddModelError("ValorAquisicao", "Valor não pode ser menor que 10,00, favor verificar!");
                            asset.flagVerificado = (int)EnumPendenciasDadosBP.PendenteValorAquisicao;
                        }
                    }
                }

                //if (ModelState.IsValid)

                VerificaGrupoMaterial(asset.MaterialGroupCode);

                Asset _asset = db.Assets.Find(asset.Id);
                AssetMovements _assetMovements = db.AssetMovements.Find(asset.AssetMovementsId);
                _asset = ConverterAssetViewModeParaAsset(asset, ref _asset);
                _assetMovements = ConverterAssetViewModelParaAssetMovements(asset, ref _assetMovements);

                _asset.Status = true;
                _asset.Login = UserCommon.CurrentUser().CPF;
                _asset.DataLogin = DateTime.Now;
                _asset.ValueAcquisition = Convert.ToDecimal(asset.ValueAcquisitionModel);
                _asset.MaterialItemCode = asset.MaterialItemCode;
                _asset.flagAcervo = asset.checkFlagAcervo;
                _asset.flagTerceiro = asset.checkFlagTerceiro;
                _asset.flagVindoDoEstoque = false;

                //INCLUSAO DA DATA DE INCORPORACAO NA MOVIMENTACAO DO BP
                _assetMovements.MovimentDate = _asset.MovimentDate;

                db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

                db.Entry(_assetMovements).State = EntityState.Modified;

                if (ModelState.IsValid)
                {
                    _asset.flagDepreciaAcumulada = 1;
                    _asset.flagVerificado = null;

                    if (asset.checkFlagAcervo)
                    {
                        _asset.MaterialItemCode = 5628156;
                        _asset.MaterialItemDescription = "Acervos";
                        asset.MaterialGroupCode = 99;
                        _asset.MaterialGroupCode = 99;
                        _asset.LifeCycle = 0;
                        _asset.RateDepreciationMonthly = 0;
                        _asset.ResidualValue = 0;
                    }
                    else if (asset.checkFlagTerceiro)
                    {
                        _asset.MaterialItemCode = 5628121;
                        _asset.MaterialItemDescription = "Bens de Terceiros";
                        asset.MaterialGroupCode = 99;
                        _asset.MaterialGroupCode = 99;
                        _asset.LifeCycle = 0;
                        _asset.RateDepreciationMonthly = 0;
                        _asset.ResidualValue = 0;
                    }
                    else {
                        PreencheValorAquisicaoDeDecreto(_asset);
                        DepreciacaoNaIncorporacao objDepreciacao = new DepreciacaoNaIncorporacao(db);
                        objDepreciacao.Deprecia(_asset, _assetMovements);
                    }

                    var _ShortDescription = (from a in db.ShortDescriptionItens where a.Description == asset.ShortDescription select a).FirstOrDefault();

                    if (_ShortDescription.IsNotNull())
                    {
                        _asset.ShortDescriptionItemId = _ShortDescription.Id;
                        _asset.MaterialItemDescription = _ShortDescription.Description;
                    }
                    else
                    {
                        this.db.Configuration.AutoDetectChangesEnabled = false;
                        this.db.Configuration.LazyLoadingEnabled = false;

                        ShortDescriptionItem _shortDescriptionItem = new ShortDescriptionItem();
                        _shortDescriptionItem.Description = asset.ShortDescription;
                        db.Entry(_shortDescriptionItem).State = EntityState.Added;
                        db.SaveChanges();

                        _asset.ShortDescriptionItemId = _shortDescriptionItem.Id;
                        _asset.MaterialItemDescription = _shortDescriptionItem.Description;

                        this.db.Configuration.AutoDetectChangesEnabled = true;
                        this.db.Configuration.LazyLoadingEnabled = true;
                    }

                    PreencheFlagAnimalAServico(asset);
                    _asset.flagAnimalNaoServico = asset.flagAnimalNaoServico;
                    db.Entry(_asset).State = EntityState.Modified;
                    ComputaAlteracaoContabil(_assetMovements.InstitutionId, _assetMovements.BudgetUnitId,
                                             _assetMovements.ManagerUnitId, (int)_assetMovements.AuxiliaryAccountId);

                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();

                    return RedirectToAction("Index");
                }

                _asset.flagVerificado = asset.flagVerificado;
                _asset.flagDepreciaAcumulada = null;
                db.Entry(_asset).State = EntityState.Modified;
                db.SaveChanges();
                db.Database.CurrentTransaction.Commit();

                CarregaViewBags(asset);
                return View(asset);
            }
            catch (Exception ex)
            {
                if (db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
            finally
            {
                if (db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();
            }
        }

        //private AssetViewModel InformacoesAsset(AssetEAssetMovimentViewModel assetEAssetMoviment)
        //{
        //    AssetViewModel asset = new AssetViewModel();

        //    asset.InitialId = assetEAssetMoviment.Asset.InitialId;
        //    asset.InitialDescription = assetEAssetMoviment.Asset.RelatedInitial == null ? null : assetEAssetMoviment.Asset.RelatedInitial.Description;
        //    asset.NumberIdentification = assetEAssetMoviment.Asset.NumberIdentification;
        //    asset.AcquisitionDate = assetEAssetMoviment.Asset.AcquisitionDate;
        //    //asset.MovimentDate = assetEAssetMoviment.Asset.MovimentDate;
        //    asset.ValueAcquisition = assetEAssetMoviment.Asset.ValueAcquisition;
        //    asset.ValueAcquisitionModel = assetEAssetMoviment.Asset.ValueAcquisition.ToString();
        //    asset.MaterialItemCode = assetEAssetMoviment.Asset.MaterialItemCode;
        //    asset.MaterialItemDescription = assetEAssetMoviment.Asset.MaterialItemDescription;
        //    asset.MaterialGroupCode = assetEAssetMoviment.Asset.MaterialGroupCode;
        //    asset.MaterialGroupDescription = (from m in db.MaterialGroups where m.Code == assetEAssetMoviment.Asset.MaterialGroupCode select m.Description).FirstOrDefault();
        //    asset.ShortDescription = assetEAssetMoviment.Asset.RelatedShortDescriptionItem == null ? null : assetEAssetMoviment.Asset.RelatedShortDescriptionItem.Description;
        //    asset.LifeCycle = assetEAssetMoviment.Asset.LifeCycle;
        //    asset.RateDepreciationMonthly = assetEAssetMoviment.Asset.RateDepreciationMonthly;
        //    asset.ResidualValue = assetEAssetMoviment.Asset.ResidualValue;
        //    asset.ShortDescriptionItemId = assetEAssetMoviment.Asset.ShortDescriptionItemId;
        //    asset.OldInitial = assetEAssetMoviment.Asset.OldInitial;
        //    asset.OldNumberIdentification = assetEAssetMoviment.Asset.OldNumberIdentification;
        //    asset.NumberDoc = assetEAssetMoviment.Asset.NumberDoc;
        //    asset.ManagerUnitId = assetEAssetMoviment.AssetMoviment.ManagerUnitId;
        //    //asset.ValueUpdateModel = assetEAssetMoviment.Asset.ValueUpdate.ToString();
        //    asset.checkFlagAcervo = assetEAssetMoviment.Asset.flagAcervo == null ? false : (bool)assetEAssetMoviment.Asset.flagAcervo;

        //    asset.AssetMovementsId = assetEAssetMoviment.AssetMoviment.Id;
        //    asset.StateConservationId = assetEAssetMoviment.AssetMoviment.StateConservationId;
        //    asset.NumberPurchaseProcess = assetEAssetMoviment.AssetMoviment.NumberPurchaseProcess;
        //    asset.InstitutionId = assetEAssetMoviment.AssetMoviment.InstitutionId;
        //    asset.BudgetUnitId = assetEAssetMoviment.AssetMoviment.BudgetUnitId;
        //    asset.BudgetUnitCode = assetEAssetMoviment.AssetMoviment.RelatedBudgetUnit.Code;
        //    asset.ManagerUnitId = assetEAssetMoviment.AssetMoviment.ManagerUnitId;
        //    asset.ManagerUnitCode = assetEAssetMoviment.AssetMoviment.RelatedManagerUnit.Code;
        //    asset.AdministrativeUnitId = assetEAssetMoviment.AssetMoviment.AdministrativeUnitId;
        //    asset.AdministrativeUnitCode = assetEAssetMoviment.AssetMoviment.RelatedAdministrativeUnit == null ? 0 : assetEAssetMoviment.AssetMoviment.RelatedAdministrativeUnit.Code;
        //    asset.SectionId = assetEAssetMoviment.AssetMoviment.SectionId;
        //    asset.SectionCode = assetEAssetMoviment.AssetMoviment.RelatedSection == null ? 0 : assetEAssetMoviment.AssetMoviment.RelatedSection.Code;
        //    asset.AuxiliaryAccountId = assetEAssetMoviment.AssetMoviment.AuxiliaryAccountId;
        //    asset.AuxiliaryAccountCode = assetEAssetMoviment.AssetMoviment.RelatedAuxiliaryAccount == null ? 0 : assetEAssetMoviment.AssetMoviment.RelatedAuxiliaryAccount.Code;
        //    asset.ResponsibleId = assetEAssetMoviment.AssetMoviment.ResponsibleId;
        //    asset.ResponsibleName = assetEAssetMoviment.AssetMoviment.RelatedResponsible == null ? null : assetEAssetMoviment.AssetMoviment.RelatedResponsible.Name;

        //    // Dados Complementares
        //    asset.SerialNumber = assetEAssetMoviment.Asset.SerialNumber;
        //    asset.DateGuarantee = assetEAssetMoviment.Asset.DateGuarantee;
        //    asset.Brand = assetEAssetMoviment.Asset.Brand;
        //    asset.NumberPlate = assetEAssetMoviment.Asset.NumberPlate;
        //    asset.ManufactureDate = assetEAssetMoviment.Asset.ManufactureDate;
        //    asset.ChassiNumber = assetEAssetMoviment.Asset.ChassiNumber;
        //    asset.Model = assetEAssetMoviment.Asset.Model;
        //    asset.AdditionalDescription = assetEAssetMoviment.Asset.AdditionalDescription;

        //    CarregaViewBags(asset);
        //    return asset;
        //}
        private void BuscaValorDepreciacaoPorGrupo(AssetViewModel asset) {
            var grupoMaterial = (from mg in db.MaterialGroups
                                 where mg.Code == asset.MaterialGroupCode
                                 select mg).ToList().FirstOrDefault();

            if (grupoMaterial != null) {
                asset.MaterialGroupDescription = grupoMaterial.Description;
                asset.LifeCycle = grupoMaterial.LifeCycle;
                asset.RateDepreciationMonthly = grupoMaterial.RateDepreciationMonthly;
                asset.ResidualValue = grupoMaterial.ResidualValue;
            }
        }

        private void VerificaGrupoMaterial(int MaterialGroupCode)
        {
            var materialGroup = (from mg in db.MaterialGroups
                                 where mg.Code == MaterialGroupCode
                                 select mg).ToList().FirstOrDefault();

            if (materialGroup != null)
            {
                _lifeCycleMaterialGroup = materialGroup.LifeCycle;
                _rateDepreciationMonthlyMaterialGroup = materialGroup.RateDepreciationMonthly;
                _residualValueMaterialGroup = materialGroup.ResidualValue;
            }
            else
            {
                _lifeCycleMaterialGroup = 0;
                _rateDepreciationMonthlyMaterialGroup = 0;
                _residualValueMaterialGroup = 0;
            }
        }
        public string ValidaDataDeIncorporacao(DateTime DataDeIncorporacao, int ManagerUnitIdInformado, DateTime DataDeAquisicao)
        {
            string _mensagemRetorno = "";
            string anoMesIncorporacao = DataDeIncorporacao.Year.ToString().PadLeft(4, '0') + DataDeIncorporacao.Month.ToString().PadLeft(2, '0');
            string anoMesReferenciaFechamentoDaUGE = RecuperaAnoMesReferenciaFechamento((int)ManagerUnitIdInformado);
            string anoMesStartUGE = RecuperaAnoMesStartUGE((int)ManagerUnitIdInformado);

            if (DateTime.Compare(DataDeIncorporacao, new DateTime(1900, 1, 1)) <= 0) {
                _mensagemRetorno = "Por favor, informe uma Data Incorporação que corresponda ao mês/ano de referência " + anoMesReferenciaFechamentoDaUGE.Substring(4) + "/" + anoMesReferenciaFechamentoDaUGE.Substring(0, 4) + ".";
                return _mensagemRetorno;
            }

            if (DateTime.Compare(DataDeAquisicao, new DateTime(1900, 1, 1)) > 0)
            {
                if (DataDeIncorporacao < DataDeAquisicao) {
                    _mensagemRetorno = "A Data de incorporação deve ser posterior a data de aquisição";
                    return _mensagemRetorno;
                }
            }

            if (DataDeIncorporacao.Date > DateTime.Now.Date)
            {
                _mensagemRetorno = "A Data de incorporação deve ser igual ou inferior a data atual";
                return _mensagemRetorno;
            }

            if (int.Parse(anoMesReferenciaFechamentoDaUGE) != int.Parse(anoMesStartUGE))
            {
                if (anoMesReferenciaFechamentoDaUGE != anoMesIncorporacao)
                {
                    _mensagemRetorno = "Por favor, informe uma Data Incorporação que corresponda ao mês/ano de referência " + anoMesReferenciaFechamentoDaUGE.Substring(4) + "/" + anoMesReferenciaFechamentoDaUGE.Substring(0, 4) + ".";
                    return _mensagemRetorno;
                }
            }
            else
            {
                if (int.Parse(anoMesIncorporacao) > int.Parse(anoMesReferenciaFechamentoDaUGE))
                {
                    _mensagemRetorno = "Por favor, informe uma data cujo mês/ano sejam iguais ou inferiores ao mês de referência " + anoMesReferenciaFechamentoDaUGE.Substring(4) + "/" + anoMesReferenciaFechamentoDaUGE.Substring(0, 4) + ".";
                    return _mensagemRetorno;
                }
            }

            return _mensagemRetorno;
        }
        private string RecuperaAnoMesReferenciaFechamento(int ManagerUnitIdInformado)
        {
            return (from m in db.ManagerUnits
                    where m.Id == ManagerUnitIdInformado
                    select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
        }
        private string RecuperaAnoMesStartUGE(int ManagerUnitIdInformado)
        {
            return (from m in db.ManagerUnits
                    where m.Id == ManagerUnitIdInformado
                    select m.ManagmentUnit_YearMonthStart).FirstOrDefault();
        }
        private AssetMovements ConverterAssetViewModelParaAssetMovements(AssetViewModel model, ref AssetMovements _assetMovements)
        {

            _assetMovements.Status = true;
            //if(model.StateConservationId > 0)
            _assetMovements.StateConservationId = model.StateConservationId;

            _assetMovements.NumberPurchaseProcess = model.NumberPurchaseProcess;
            _assetMovements.InstitutionId = model.InstitutionId;
            if(model.BudgetUnitId > 0)
                _assetMovements.BudgetUnitId = model.BudgetUnitId;
            if (model.ManagerUnitId > 0)
                _assetMovements.ManagerUnitId = model.ManagerUnitId;

            _assetMovements.AdministrativeUnitId = model.AdministrativeUnitId == 0 ? null : model.AdministrativeUnitId;
            _assetMovements.SectionId = model.SectionId == 0 ? null : model.SectionId;
            _assetMovements.AuxiliaryAccountId = model.AuxiliaryAccountId;
            _assetMovements.ResponsibleId = model.ResponsibleId;
            _assetMovements.Login = UserCommon.CurrentUser().CPF;
            _assetMovements.DataLogin = DateTime.Now;

            return _assetMovements;

        }
        private Asset ConverterAssetViewModeParaAsset(AssetViewModel model, ref Asset _asset)
        {
            _asset.AceleratedDepreciation = false;
            // Se houver alguma alteração em um dos campos de depreciação que sai do padrão do grupo de material o BP será considerado com depreciação acelerada
            if (model.LifeCycle != _lifeCycleMaterialGroup || model.RateDepreciationMonthly != _rateDepreciationMonthlyMaterialGroup || model.ResidualValue != _residualValueMaterialGroup)
                _asset.AceleratedDepreciation = true;

            //Dados obrigatórios no banco de dados

            _asset.Status = true;
            _asset.Login = UserCommon.CurrentUser().CPF;
            _asset.DataLogin = DateTime.Now;
            if(model.InitialId > 0)
                _asset.InitialId = model.InitialId;
            _asset.LifeCycle = model.LifeCycle;
            _asset.RateDepreciationMonthly = model.RateDepreciationMonthly;
            _asset.ResidualValue = model.ResidualValue;


            if (!string.IsNullOrEmpty(model.NumberIdentification) && !string.IsNullOrWhiteSpace(model.NumberIdentification))
            {
                _asset.NumberIdentification = model.NumberIdentification;
                _asset.DiferenciacaoChapa = model.DiferenciacaoChapa;
            }

            if (DateTime.Compare(model.AcquisitionDate, new DateTime(1900, 1, 1)) > 0)
                _asset.AcquisitionDate = model.AcquisitionDate;
            if (DateTime.Compare(model.MovimentDate, new DateTime(1900, 1, 1)) > 0)
                _asset.MovimentDate = model.MovimentDate;

            _asset.ValueAcquisition = Convert.ToDecimal(model.ValueAcquisitionModel);
            if (!string.IsNullOrEmpty(model.MaterialItemDescription) && !string.IsNullOrWhiteSpace(model.MaterialItemDescription))
            {
                _asset.MaterialItemCode = model.MaterialItemCode;
                _asset.MaterialItemDescription = model.MaterialItemDescription;
                _asset.MaterialGroupCode = model.MaterialGroupCode;
            }

            if (!string.IsNullOrEmpty(model.NumberDoc) && !string.IsNullOrWhiteSpace(model.NumberDoc))
                _asset.NumberDoc = model.NumberDoc;

            // Criar registro da Descrição Resumida na sua tabela e relacionar o Id ao BP
            if (!string.IsNullOrEmpty(model.ShortDescription) && !string.IsNullOrWhiteSpace(model.ShortDescription))
            {
                int IdDescricao = (from s in db.ShortDescriptionItens where s.Description.Trim() == model.ShortDescription.Trim() select s.Id).FirstOrDefault();
                if (IdDescricao > 0)
                    _asset.ShortDescriptionItemId = IdDescricao;
                else
                {
                    this.db.Configuration.AutoDetectChangesEnabled = false;
                    this.db.Configuration.LazyLoadingEnabled = false;

                    ShortDescriptionItem shortdesc = new ShortDescriptionItem();
                    shortdesc.Description = model.ShortDescription.Trim();

                    db.ShortDescriptionItens.Add(shortdesc);
                    db.SaveChanges();

                    _asset.ShortDescriptionItemId = (from s in db.ShortDescriptionItens where s.Description.Trim() == shortdesc.Description.Trim() select s.Id).FirstOrDefault();

                    this.db.Configuration.AutoDetectChangesEnabled = true;
                    this.db.Configuration.LazyLoadingEnabled = true;
                }
            }

            //Fim Dados obrigatórios no banco de dados

            int IdSiglaASerSalvo = _asset.InitialId;

            _asset.InitialName = (from i in db.Initials where i.Id == IdSiglaASerSalvo select i.Name).FirstOrDefault();
            _asset.flagAcervo = model.checkFlagAcervo;
            _asset.flagTerceiro = model.checkFlagTerceiro;
            _asset.flagAnimalNaoServico = !model.checkflagAnimalAServico;

            if (model.checkFlagAcervo || model.checkFlagTerceiro)
                _asset.ValueUpdate = Convert.ToDecimal(model.ValueUpdateModel);

            // Tratamento de acordo com o Decreto da Fazenda
            if (model.checkFlagDecretoSefaz)
            {
                _asset.flagDecreto = true;
                _asset.flagVerificado = null;
                _asset.flagDepreciaAcumulada = 1;
                _asset.ValueUpdate = Convert.ToDecimal(model.ValueUpdateModel);
                _asset.ResidualValueCalc = Math.Round((Convert.ToDecimal(_asset.ValueUpdate) * (model.ResidualValue / 100)), 2);

                // Diferença absoluta entre meses da data de aquisição a data atual.
                //int MesesUtilizados = Math.Abs(((_asset.AcquisitionDate.Month - DateTime.Now.Month) + 12) * (_asset.AcquisitionDate.Year - DateTime.Now.Year));

                string anoMesReferenciaFechamentoDaUGE = RecuperaAnoMesReferenciaFechamento(_asset.ManagerUnitId);
                int MesesUtilizados = _asset.AcquisitionDate.MonthDiff(anoMesReferenciaFechamentoDaUGE.RetornarMesAnoReferenciaDateTimeBPPendente());
                _asset.MonthUsed = MesesUtilizados > model.LifeCycle ? model.LifeCycle : MesesUtilizados;

                _asset.DepreciationByMonth = (MesesUtilizados >= model.LifeCycle ? 0 : (_asset.ValueUpdate - _asset.ResidualValueCalc) / (model.LifeCycle - MesesUtilizados));
                _asset.DepreciationAccumulated = 0;
                _asset.DepreciationByMonth = Common.TratamentoDados.truncarDuasCasas(_asset.DepreciationByMonth);

                if (_asset.MonthUsed == model.LifeCycle)
                {
                    _asset.ValorDesdobramento = (_asset.ValueUpdate - _asset.ResidualValueCalc) - _asset.DepreciationByMonth;
                    _asset.flagDepreciationCompleted = true;
                }
                else
                    _asset.flagDepreciationCompleted = false;


            }

            _asset.OldInitial = model.OldInitial;
            _asset.OldNumberIdentification = model.OldNumberIdentification;
            _asset.DiferenciacaoChapaAntiga = model.DiferenciacaoChapaAntiga;
            
            if (model.ManagerUnitId > 0)
                _asset.ManagerUnitId = model.ManagerUnitId;

            //Dados complementares
            _asset.SerialNumber = model.SerialNumber;
            _asset.DateGuarantee = model.DateGuarantee;
            _asset.Brand = model.Brand;
            _asset.NumberPlate = model.NumberPlate;
            _asset.ManufactureDate = model.ManufactureDate;
            _asset.ChassiNumber = model.ChassiNumber;
            _asset.Model = model.Model;
            _asset.AdditionalDescription = model.AdditionalDescription;

            return _asset;
        }

        private void PreencheValorAquisicaoDeDecreto(Asset asset)
        {
            if (asset.flagDecreto == true)
            {
                HistoricoValoresDecreto historico = new HistoricoValoresDecreto();
                historico.AssetId = asset.Id;
                historico.ValorAquisicao = asset.ValueAcquisition;
                historico.ValorRevalorizacao = Convert.ToDecimal(asset.ValueUpdate);
                historico.DataAlteracao = asset.MovimentDate;
                historico.LoginId = UserCommon.CurrentUser().Id;
                
                db.Entry(historico).State = EntityState.Added;
                db.SaveChanges();

                asset.ValueAcquisition = Convert.ToDecimal(asset.ValueUpdate);
            }

        }

        private void PreencheFlagAnimalAServico(AssetViewModel asset)
        {
            if (asset.MaterialGroupCode == 88)
            {
                asset.flagAnimalNaoServico = !asset.checkflagAnimalAServico;
            }
            else {
                asset.flagAnimalNaoServico = false;
            }

        }
        private void RegistraBPExcluido(Asset BP, AssetMovements incorporacao, string Observacoes = "")
        {
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
            //para baixas de BPs pendentes, os campos referentes ao estornos não são preenchidos
            bpExcluido.DataAcao = Convert.ToDateTime(incorporacao.DataLogin);
            bpExcluido.LoginAcao = UserCommon.CurrentUser().Id;

            if (!string.IsNullOrEmpty(Observacoes) && !string.IsNullOrWhiteSpace(Observacoes))
            {
                bpExcluido.Observacoes = "Numero Documento=" + Observacoes;

                if (Observacoes.Length > 20)
                    bpExcluido.NumeroDocumento = Observacoes.Substring(0, 20);
                else
                    bpExcluido.NumeroDocumento = Observacoes;
            }

            db.BPsExcluidos.Add(bpExcluido);
            db.SaveChanges();

            //exclusão de registros
            //db.NotaLancamentoPendenteSIAFEMs.RemoveRange(db.NotaLancamentoPendenteSIAFEMs.Where(n => n.AssetMovementId == incorporacao.Id));
            db.Database.ExecuteSqlCommand("delete from [dbo].[Closing] where AssetMovementsId = " + incorporacao.Id.ToString());
            db.Entry(incorporacao).State = EntityState.Deleted;
            db.AssetMovements.Remove(incorporacao);

            db.Database.ExecuteSqlCommand("delete from [dbo].[AssetNumberIdentification] where AssetId = " + BP.Id.ToString());
            db.Database.ExecuteSqlCommand("delete from [dbo].[Closing] where AssetId = " + BP.Id.ToString());

            if (BP.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                BP.MovementTypeId != (int)EnumMovimentType.Doacao &&
                BP.MovementTypeId != (int)EnumMovimentType.IncorporacaoPorTransferencia &&
                BP.MovementTypeId != (int)EnumMovimentType.IncorporacaoPorDoacao &&
                BP.MovementTypeId != (int)EnumMovimentType.IncorpDoacaoIntraNoEstado &&
                BP.MovementTypeId != (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado &&
                BP.MovementTypeId != (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado &&
                BP.AssetStartId != null)
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MonthlyDepreciation] where AssetStartId = " + BP.AssetStartId.ToString() + ";");
            }

            db.Entry(BP).State = EntityState.Deleted;
            db.Assets.Remove(BP);

            db.SaveChanges();

        }

        private Asset BPAtivoComMesmaSiglaChapa(AssetViewModel viewmodel)
        {
            return (from a in db.Assets
                    join m in db.AssetMovements on a.Id equals m.AssetId
                    join ma in db.ManagerUnits on a.ManagerUnitId equals ma.Id
                    where a.NumberIdentification == viewmodel.NumberIdentification
                       && a.DiferenciacaoChapa == viewmodel.DiferenciacaoChapa
                       && a.InitialId == viewmodel.InitialId
                       && m.InstitutionId == viewmodel.InstitutionId
                       && !a.flagVerificado.HasValue
                       && a.flagDepreciaAcumulada == 1
                       && a.Status == true
                       && a.Id != viewmodel.Id
                    select a).FirstOrDefault();
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
        #endregion Edit

        #region Excel
        public void GerarExcel()
        {
            if (TempData["ListaVisao"] != null)
                TempData["resulVisao"] = TempData["ListaVisao"];

            TempData.Keep("resulVisao");

            GridView gv = new GridView();
            var listaPendentes = ((IQueryable<AssetPendingViewModel>)TempData["resulVisao"]).ToList();

            IList<ExportarExcelViewModelBPPendentes> Excels = new List<ExportarExcelViewModelBPPendentes>();

            foreach (var pendente in listaPendentes)
            {
                ExportarExcelViewModelBPPendentes excel = new ExportarExcelViewModelBPPendentes();

                excel.SIGLA = pendente.Sigla;
                excel.CHAPA = pendente.Chapa.ToString();
                excel.CODIGO_ITEM = pendente.CodigoMaterial;
                excel.DESCRICAO_ITEM = pendente.DescricaoMaterial;
                excel.UGE = pendente.UGE;
                excel.UA = pendente.UA;
                excel.VALOR_AQUISICAO = "R$" + pendente.ValorAquisicao.ToString();
                excel.DEPRECIACAO_MENSAL = String.Format("{0:n2}", pendente.TaxaDepreciacaoMensal.ToString().Replace(".", ","));
                Excels.Add(excel);
                excel = null;
            }

            gv.AutoGenerateColumns = false;
            gv.Columns.Add(new BoundField { HeaderText = "Sigla", DataField = "SIGLA" });
            gv.Columns.Add(new BoundField { HeaderText = "Chapa", DataField = "CHAPA" });
            gv.Columns.Add(new BoundField { HeaderText = "Código do Item Material", DataField = "CODIGO_ITEM" });
            gv.Columns.Add(new BoundField { HeaderText = "Descrição do Item Material", DataField = "DESCRICAO_ITEM" });
            gv.Columns.Add(new BoundField { HeaderText = "UGE", DataField = "UGE" });
            gv.Columns.Add(new BoundField { HeaderText = "UA", DataField = "UA" });
            gv.Columns.Add(new BoundField { HeaderText = "Valor de Aquisição", DataField = "VALOR_AQUISICAO" });
            gv.Columns.Add(new BoundField { HeaderText = "Taxa de depreciação mensal(%)", DataField = "DEPRECIACAO_MENSAL" });

            gv.DataSource = Excels;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment;filename=BensPatrimoniaisPendentes_{0}.xls", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();


        }

        #endregion

        private IQueryable<AssetEAssetMovimentViewModel> SearchByFilter(string searchString, IQueryable<AssetEAssetMovimentViewModel> result, string cbFiltros)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (int.Parse(cbFiltros))
                {
                    case 0:
                        result = result.Where(s => s.Asset.InitialName.Contains(searchString) ||
                                                   s.Asset.NumberIdentification.Contains(searchString) ||
                                                   s.Asset.MaterialItemCode.ToString().Contains(searchString) ||
                                                   s.Asset.MaterialItemDescription.Contains(searchString) ||
                                                   s.Asset.RelatedManagerUnit.Code.Contains(searchString) ||
                                                   s.AssetMoviment.RelatedAdministrativeUnit.Code.ToString().Contains(searchString) ||
                                                   s.Asset.ValueAcquisition.ToString().Contains(searchString) ||
                                                   s.Asset.RateDepreciationMonthly.ToString().Contains(searchString));
                        break;
                    case 1:
                        result = result.Where(s => s.Asset.InitialName.Contains(searchString));
                        break;
                    case 2:
                        result = result.Where(s => s.Asset.NumberIdentification.Contains(searchString));
                        break;
                    case 3:
                        result = result.Where(s => s.Asset.MaterialItemCode.ToString().Contains(searchString));
                        break;
                    case 4:
                        result = result.Where(s => s.Asset.MaterialItemDescription.Contains(searchString));
                        break;
                    case 5:
                        result = result.Where(s => s.Asset.RelatedManagerUnit.Code.Contains(searchString));
                        break;
                    case 6:
                        result = result.Where(s => s.AssetMoviment.RelatedAdministrativeUnit.Code.ToString().Contains(searchString));
                        break;
                    case 7:
                        result = result.Where(s => s.Asset.ValueAcquisition.ToString().Contains(searchString));
                        break;
                    case 8:
                        result = result.Where(s => s.Asset.RateDepreciationMonthly.ToString().Contains(searchString));
                        break;
                    case 9:
                        result = result.Where(s => s.Asset.NumberIdentification.Contains(searchString));
                        break;
                }
            }

            return result;
        }

        #region Métodos Privados

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, int modelResponsibleId = 0)
        {
            getHierarquiaPerfil();
            Hierarquia hierarquia = new Hierarquia();

            //var vUser = new List<User>();
            var vResponsible = new List<Responsible>();
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

                if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(null), "Id", "Description");
            }
            else
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);

                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId).Where(b => b.Code != "99999"), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId).Where(b => b.Code != "99999"), "Id", "Description", modelBudgetUnitId);

                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId).Where(b => b.Code != "999999"), "Id", "Description", modelManagerUnitId);
                else if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId).Where(b => b.Code != "999999"), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId).Where(b => b.Code != "999999"), "Id", "Description", modelManagerUnitId);

                if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(_administrativeUnitId).Where(b => b.Code != 99999999), "Id", "Description", modelAdministrativeUnitId);
                else if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId).Where(b => b.Code != 99999999), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(_managerUnitId).Where(b => b.Code != 99999999), "Id", "Description", modelAdministrativeUnitId);

                if (_sectionId.HasValue && _sectionId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(_sectionId).Where(b => b.Code != 999), "Id", "Description", modelSectionId);
                else if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId).Where(b => b.Code != 999), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(_administrativeUnitId).Where(b => b.Code != 999), "Id", "Description", modelSectionId);
            }

            #region Combo Responsavel

            if (ContemValor(_sectionId))
                vResponsible = (from r in db.Responsibles join s in db.Sections on r.Id equals s.ResponsibleId where s.Id == _sectionId && !r.Name.Contains("RESPONSAVEL_") select r).ToList();
            else if (modelSectionId != 0)
                vResponsible = (from r in db.Responsibles join s in db.Sections on r.Id equals s.ResponsibleId where s.Id == modelSectionId && !r.Name.Contains("RESPONSAVEL_") select r).ToList();
            else if (ContemValor(_administrativeUnitId))
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == _administrativeUnitId && !r.Name.Contains("RESPONSAVEL_") select r).ToList();
            else if (modelAdministrativeUnitId != 0)
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == modelAdministrativeUnitId && !r.Name.Contains("RESPONSAVEL_") select r).ToList();

            if (modelResponsibleId != 0)
                ViewBag.Responsible = new SelectList(vResponsible.ToList(), "Id", "Name", modelResponsibleId);
            else
                ViewBag.Responsible = new SelectList(vResponsible.ToList(), "Id", "Name");

            #endregion
        }

        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null)
                retorno = true;
            return retorno;
        }

        private void CarregaViewBags(AssetViewModel asset)
        {
            SetCarregaHierarquia(asset.InstitutionId, asset.BudgetUnitId, asset.ManagerUnitId, asset.AdministrativeUnitId ?? 0, asset.SectionId ?? 0, asset.ResponsibleId ?? 0);

            var queryInitial = db.Initials.Where(i => i.InstitutionId == asset.InstitutionId && (i.BudgetUnitId == asset.BudgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == asset.ManagerUnitId || i.ManagerUnitId == null) && i.Name != "SIGLA_GENERICA" && i.Description != "SIGLA_GENERICA" && i.Status == true).AsNoTracking().ToList();
            if ((from i in db.Initials where i.Id == asset.InitialId && (i.Name == "SIGLA_GENERICA" || i.Description == "SIGLA_GENERICA") select i.Id).Any())
                ViewBag.Initial = new SelectList(queryInitial.OrderBy(i => i.Name), "Id", "Name");
            else
                ViewBag.Initial = new SelectList(queryInitial.OrderBy(i => i.Name), "Id", "Name", asset.InitialId);

            ViewBag.StateConservations = new SelectList(db.StateConservations.OrderBy(m => m.Description), "Id", "Description", asset.StateConservationId);
            //var vAuxiliaryAccounts = (from a in db.AuxiliaryAccounts where a.Code != 0 && a.Status == true select a);
            //ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaAuxiliarPorGrupoMaterial(asset.MaterialGroupCode), "Id", "Description", asset.AuxiliaryAccountId);
            CarregaViewBagContasContabeisEdicao(asset);
            ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == asset.InstitutionId && (i.BudgetUnitId == asset.BudgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == asset.ManagerUnitId || i.ManagerUnitId == null) && i.Name != "SIGLA_GENERICA" && i.Description != "SIGLA_GENERICA" && i.Status == true).OrderBy(i => i.Name), "Id", "Name", asset.OldInitial);
            ViewBag.MovementTypeId = new SelectList(db.MovementTypes.Where(i => i.GroupMovimentId == (int)EnumGroupMoviment.BensPendentes).OrderBy(i => i.Code), "Id", "Description");

            // Perfis Administradores não podem excluir
            //if ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.AdministradordeOrgao || (int)HttpContext.Items["perfilId"] == (int)EnumProfile.AdministradordeUO)
            //    ViewBag.PerfilAdministradores = false;
            //else
            //    ViewBag.PerfilAdministradores = true;
        }

        private void CarregaViewBagContasContabeisEdicao(AssetViewModel assetViewModel)
        {
            int tipo = 0;
            BuscaContasContabeis buscaContas = new BuscaContasContabeis();

            tipo = assetViewModel.checkFlagAcervo == true ? 1 : (assetViewModel.checkFlagTerceiro == true ? 2 : 0);

            if (tipo == 0)
            {
                ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaAuxiliarPorGrupoMaterial(assetViewModel.MaterialGroupCode), "Id", "Description", assetViewModel.AuxiliaryAccountId);
            }
            else
            {
                ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaContabilPorTipo(tipo), "Id", "Description", assetViewModel.AuxiliaryAccountId);
            }
        }

        //private void CarregaComboFiltrosIntegracao(string codFiltro)
        //{
        //    var lista = new List<ItemGenericViewModel>();
        //    var itemGeneric = new ItemGenericViewModel();

        //    itemGeneric = new ItemGenericViewModel { Id = 0, Description = "Todos os filtros...", Ordem = 0 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 1, Description = "Sigla", Ordem = 10 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 3, Description = "Código de Item Material", Ordem = 30 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 4, Description = "Descrição de Item Material", Ordem = 40 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 5, Description = "UGE", Ordem = 50 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 6, Description = "UA", Ordem = 60 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 7, Description = "Valor de Aquisição", Ordem = 70 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 8, Description = "Tax. Depreciação Mensal (%)", Ordem = 80 };
        //    lista.Add(itemGeneric);

        //    itemGeneric = new ItemGenericViewModel { Id = 9, Description = "Integração", Ordem = 90 };
        //    lista.Add(itemGeneric);

        //    if (codFiltro != null && codFiltro != "")
        //        ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description", int.Parse(codFiltro));
        //    else
        //        ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");
        //}
        private void CarregaComboFiltros(string codFiltro, bool index = false)
        {
            var lista = new List<ItemGenericViewModel>();

            ItemGenericViewModel itemGeneric = new ItemGenericViewModel { Id = 0, Description = "Todos os filtros...", Ordem = 0 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 1, Description = "Sigla", Ordem = 10 };
            lista.Add(itemGeneric);

            if (index)
            {
                itemGeneric = new ItemGenericViewModel { Id = 2, Description = "Chapa", Ordem = 20 };
                lista.Add(itemGeneric);
            }

            itemGeneric = new ItemGenericViewModel { Id = 3, Description = "Código de Item Material", Ordem = 30 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 4, Description = "Descrição de Item Material", Ordem = 40 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 5, Description = "UGE", Ordem = 50 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 6, Description = "UA", Ordem = 60 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 7, Description = "Valor de Aquisição", Ordem = 70 };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel { Id = 8, Description = "Tax. Depreciação Mensal (%)", Ordem = 80 };
            lista.Add(itemGeneric);

            if (!index)
            {
                itemGeneric = new ItemGenericViewModel { Id = 9, Description = "Integração", Ordem = 90 };
                lista.Add(itemGeneric);
            }

            if (codFiltro != null && codFiltro != "")
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description", int.Parse(codFiltro));
            else
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");
        }

        private IQueryable<AssetPendingViewModel> FiltraPesquisa(IQueryable<AssetPendingViewModel> lista, string searchString, string cbFiltros)
        {
            switch (int.Parse(cbFiltros))
            {
                case 0:
                    lista = lista.Where(s => s.Sigla.Contains(searchString) ||
                                     s.Chapa.ToString().Contains(searchString) ||
                                     s.CodigoMaterial.ToString().Contains(searchString) ||
                                     s.DescricaoMaterial.Contains(searchString) ||
                                     s.UGE.Contains(searchString) ||
                                     s.UA.ToString().Contains(searchString) ||
                                     s.ValorAquisicao.ToString().Contains(searchString) ||
                                     s.TaxaDepreciacaoMensal.ToString().Contains(searchString));
                    break;
                case 1: lista = lista.Where(s => s.Sigla.Contains(searchString)); break;
                case 2: lista = lista.Where(s => s.Chapa.ToString().Contains(searchString)); break;
                case 3: lista = lista.Where(s => s.CodigoMaterial.ToString().Contains(searchString)); break;
                case 4: lista = lista.Where(s => s.DescricaoMaterial.Contains(searchString)); break;
                case 5: lista = lista.Where(s => s.UGE.Contains(searchString)); break;
                case 6: lista = lista.Where(s => s.UA.ToString().Contains(searchString)); break;
                case 7: lista = lista.Where(s => s.ValorAquisicao.ToString().Contains(searchString)); break;
                case 8: lista = lista.Where(s => s.TaxaDepreciacaoMensal.ToString().Contains(searchString)); break;
            }

            return lista;
        }

        private IQueryable<AssetPendingViewModel> Ordena(IQueryable<AssetPendingViewModel> lst, int coluna, string dir)
        {
            switch (coluna)
            {
                case 1: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.Chapa) : lst.OrderByDescending(ap => ap.Chapa); break;
                case 2: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.CodigoMaterial) : lst.OrderByDescending(ap => ap.CodigoMaterial); break;
                case 3: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.DescricaoMaterial) : lst.OrderByDescending(ap => ap.DescricaoMaterial); break;
                case 4: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.UGE) : lst.OrderByDescending(ap => ap.UGE); break;
                case 5: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.UA) : lst.OrderByDescending(ap => ap.UA); break;
                case 6: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.ValorAquisicao) : lst.OrderByDescending(ap => ap.ValorAquisicao); break;
                case 7: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.TaxaDepreciacaoMensal) : lst.OrderByDescending(ap => ap.TaxaDepreciacaoMensal); break;
                default: lst = dir.Equals("asc") ? lst.OrderBy(ap => ap.Sigla) : lst.OrderByDescending(ap => ap.Sigla); break;
            }
            return lst;
        }

        #endregion
    }
}