using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SAM.Web.Models;
using AutoMapper;
using SAM.Web.Common;
namespace SAM.Web.Controllers
{
    public class RepairController : BaseController
    {
        private static int pageSize = 10;
        private int pageNumber = 1;
        private SAMContext db = new SAMContext();
        private int? _administrativeUnitId;
        private int? _budgetUnitId;
        private int _institutionId;
        private int? _managerUnitId;
        private int? _sectionId;

        private void GetIdsDaHierarquia()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _administrativeUnitId = perflLogado.AdministrativeUnitId;
            _sectionId = perflLogado.SectionId;
        }
        
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                IQueryable<Repair> lstRetorno = null;
                GetIdsDaHierarquia();

                // TODO: RETIRADO APOS ALTERACAO DA TABELA ASSET E ASSETMOVEMENTS
                //lstRetorno = this.db.Repair.Where(q => !q.ReturnDate.HasValue && q.RelatedAssets.InstitutionId == _institutionId).AsQueryable();

                //if (_budgetUnitId.HasValue && _budgetUnitId > 0)
                //    lstRetorno = lstRetorno.Where(q => q.RelatedAssets.BudgetUnitId == _budgetUnitId);
                //if (_managerUnitId.HasValue && _managerUnitId > 0)
                //    lstRetorno = lstRetorno.Where(q => q.RelatedAssets.ManagerUnitId == _managerUnitId);
                //if (_administrativeUnitId.HasValue && _administrativeUnitId > 0)
                //    lstRetorno = lstRetorno.Where(q => q.RelatedAssets.AdministrativeUnitId == _administrativeUnitId);
                //if (_sectionId.HasValue && _sectionId > 0)
                //    lstRetorno = lstRetorno.Where(q => q.RelatedAssets.SectionId == _sectionId);


                //Guarda o filtro
                ViewBag.CurrentFilter = searchString;

                //Filter
                lstRetorno = SearchByFilter(searchString, lstRetorno);

                //Pagination
                pageSize = 10;
                pageNumber = (page ?? 1);

                var lstRetornoPaginado = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);

                Mapper.CreateMap<Repair, RepairViewModel>();
                var result = Mapper.Map<List<RepairViewModel>>(lstRetornoPaginado);
                var retorno = new StaticPagedList<RepairViewModel>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            try{
                InitialsController initialsController = new InitialsController();
                RepairViewModel repair = new RepairViewModel();
                AssetsController assentController = new AssetsController();

                // TODO: RETIRADO APOS ALTERACAO DA TABELA ASSET E ASSETMOVEMENTS
                //repair.Assets = assentController.Assets();
                return View(repair);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult Create(RepairViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AssetsController assetsController = new AssetsController();
                    string[] assetId;

                    if (model.AssetSelecionados == null)
                    {
                        ModelState.AddModelError("selecaoPatrimonio", "Por favor selecione algum Bem Patrimonial.");
                        return View(model);
                    }
                    else
                    {
                        assetId = model.AssetSelecionados.Split(':');
                        for (int i = 0; i <= assetId.Length - 1; i++)
                        {
                            Asset asset = db.Assets.Where(a => a.Id == Convert.ToInt32(assetId[i])).FirstOrDefault();
                            if (asset == null)
                            {
                                ModelState.AddModelError("NumberIdentification", "Número de identificação do bem patrimonial não existe.");
                                return View(model);
                            }

                            model.AssetId = asset.Id;
                            model.InitialId = asset.InitialId;
                            model.NumberIdentification = Convert.ToInt32(asset.NumberIdentification);
                            //model.PartNumberIdentification = (asset.PartNumberIdentification != null ? Decimal.Parse(asset.PartNumberIdentification) : 0);

                            Repair concert = null;
                            Mapper.CreateMap<RepairViewModel, Repair>();
                            concert = Mapper.Map<Repair>(model);

                            this.db.Repair.Add(concert);
                            this.db.SaveChanges();

                        }
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    model.AssetSelecionados = null;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        public ActionResult Return(int? repairId)
        {
            try
            {
                RepairViewModel concert = null;
                if (repairId.HasValue)
                {

                    var retorno = this.db.Repair.Where(c => c.Id == repairId.Value).AsQueryable().FirstOrDefault();

                    Mapper.CreateMap<Repair, RepairViewModel>();
                    concert = Mapper.Map<RepairViewModel>(retorno);
                }
                else
                    concert = new RepairViewModel();

                return View(concert);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        [HttpPost]
        public ActionResult Return(RepairViewModel model)
        {
            try
            {
                Repair repair = this.db.Repair.Where(c => c.Id == model.Id).AsQueryable().FirstOrDefault();
                RepairViewModel concert = null;

                if (repair == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                if (!model.ReturnDate.HasValue)
                {
                    ModelState.AddModelError("ReturnDate", "Data de retorno é obrigatório.");
                    Mapper.CreateMap<Repair, RepairViewModel>();
                    concert = Mapper.Map<RepairViewModel>(repair);

                    return View(concert);
                }

                if (ModelState.IsValid)
                {
                    repair.ReturnDate = model.ReturnDate.Value;
                    repair.FinalCost = model.FinalCost;

                    this.db.SaveChanges();

                    return RedirectToAction("Index");
                }

                Mapper.CreateMap<Repair, RepairViewModel>();
                concert = Mapper.Map<RepairViewModel>(repair);
                return View(concert);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        // TODO: RETIRADO APOS ALTERACAO DA TABELA ASSET E ASSETMOVEMENTS
        //[HttpPost]
        //public JsonResult GridAsset(string nomeDoItemMaterial)
        //{
        //    AssetsController assentController = new AssetsController();

        //    List<Asset> assets =  assentController.AssetsByFilter(nomeDoItemMaterial);
        //    List<RepairViewModel> concertViewModels = new List<RepairViewModel>();

        //    foreach (Asset asset in assets)
        //    {
        //        RepairViewModel concertViewModel = new RepairViewModel();
        //        concertViewModel.AssetId = asset.Id;
        //        concertViewModel.NumberIdentification = Convert.ToInt32(asset.NumberIdentification);
        //        concertViewModel.MaterialDescricao = asset.MaterialItemDescription;
        //        concertViewModel.UaId = asset.AdministrativeUnitId;
        //        concertViewModel.ValorAtual = asset.ValueUpdate;

        //        concertViewModels.Add(concertViewModel);
        //    }

        //    var retorno = Json(concertViewModels, JsonRequestBehavior.AllowGet);

        //    return retorno;
        //}
        //[HttpPost]
        //public ActionResult GridAsset(string sortOrder, string searchString, string currentFilter, int? page)
        //{
        //    AssetsController assentController = new AssetsController();
        //    pageNumber = (page ?? 1);

        //    List<Asset> assets = assentController.Assets();
        //    if (assets == null)
        //        assets = new List<Asset>();


        //    return PartialView(assets.ToPagedList(pageNumber, pageSize));
        //}

        #region Métodos privados
        /// <summary>
        /// Filtra a pesquisa
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IQueryable<Repair> SearchByFilter(string searchString, IQueryable<Repair> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.RelatedInitials.Description.Contains(searchString) || 
                                           s.NumberIdentification.ToString().Contains(searchString) ||
                                           s.DateOut.ToString().Contains(searchString));

            return result;
        }
        #endregion
    }
}