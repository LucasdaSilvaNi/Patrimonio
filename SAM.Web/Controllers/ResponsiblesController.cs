using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SAM.Web.Models;
using SAM.Web.Common;
using SAM.Web.ViewModels;
using AutoMapper;
using System.Threading.Tasks;
using SAM.Web.Common.Enum;



namespace SAM.Web.Controllers
{
    public class ResponsiblesController : BaseController
    {
        SAMContext db;

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
            else {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
            }
        }

        #region Actions Methods
        #region Index Actions
        // GET: Responsibles
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(Responsible res) {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;
            IQueryable<ResponsibleTableViewModel> lstRetorno;

            try {

                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _budgetUnitId = IdsHieraquia[1];
                    _managerUnitId = IdsHieraquia[2];
                    _administrativeUnitId = IdsHieraquia[3];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                db = new SAMContext();

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                {
                    lstRetorno = (from r in db.Responsibles
                                  where r.Status == true
                                  select new ResponsibleTableViewModel()
                                  {
                                      Id = r.Id,
                                      Name = r.Name,
                                      Position = r.Position,
                                      AdministrativeUnitId = r.RelatedAdministrativeUnits.Id,
                                      AdministrativeUnitCode = r.RelatedAdministrativeUnits.Code,
                                      AdministrativeUnitDescription = r.RelatedAdministrativeUnits.Description,
                                      InstitutionId = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id,
                                      InstitutionCode = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code,
                                      BudgetUnitId = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Id,
                                      BudgetUnitCode = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Code,
                                      ManagerUnitId = r.RelatedAdministrativeUnits.RelatedManagerUnit.Id,
                                      ManagerUnitCode = r.RelatedAdministrativeUnits.RelatedManagerUnit.Code
                                  })
                                  .Where(s => s.Name.Contains(currentFilter)
                                        || s.InstitutionCode.Contains(currentFilter)
                                        || s.BudgetUnitCode.Contains(currentFilter)
                                        || s.ManagerUnitCode.Contains(currentFilter)
                                        || s.AdministrativeUnitCode.ToString().Contains(currentFilter)
                                        || s.AdministrativeUnitDescription.Contains(currentFilter)
                                        || s.Position.Contains(currentFilter)
                                        || s.Name.Contains(currentFilter)).AsNoTracking().OrderBy(r => r.Id);
                }
                else
                {
                    lstRetorno = (from r in db.Responsibles
                                  where r.Status == true
                                  select new ResponsibleTableViewModel()
                                  {
                                      Id = r.Id,
                                      Name = r.Name,
                                      Position = r.Position,
                                      AdministrativeUnitId = r.RelatedAdministrativeUnits.Id,
                                      AdministrativeUnitCode = r.RelatedAdministrativeUnits.Code,
                                      AdministrativeUnitDescription = r.RelatedAdministrativeUnits.Description,
                                      InstitutionId = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id,
                                      InstitutionCode = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code,
                                      BudgetUnitId = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Id,
                                      BudgetUnitCode = r.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Code,
                                      ManagerUnitId = r.RelatedAdministrativeUnits.RelatedManagerUnit.Id,
                                      ManagerUnitCode = r.RelatedAdministrativeUnits.RelatedManagerUnit.Code
                                  }).AsNoTracking().OrderBy(r => r.Id);
                }

                if (PerfilAdmGeral() != true)
                {
                    if (_administrativeUnitId != null && _administrativeUnitId != 0)
                        lstRetorno = (from r in lstRetorno where r.AdministrativeUnitId == _administrativeUnitId select r);
                    else if (_managerUnitId != null && _managerUnitId != 0)
                        lstRetorno = (from r in lstRetorno where r.ManagerUnitId == _managerUnitId select r);
                    else if (_budgetUnitId != null && _budgetUnitId != 0)
                        lstRetorno = (from r in lstRetorno where r.BudgetUnitId == _budgetUnitId select r);
                    else if (_institutionId != 0)
                        lstRetorno = (from r in lstRetorno where r.InstitutionId == _institutionId select r);
                }



                totalRegistros = await lstRetorno.CountAsync();

                var resultado = lstRetorno.Skip(startRec).Take(length)
                                        .OrderBy(s => s.InstitutionCode)
                                        .ThenBy(s => s.BudgetUnitCode)
                                        .ThenBy(s => s.ManagerUnitCode)
                                        .ThenBy(s => s.AdministrativeUnitCode)
                                        .ThenBy(s => s.Name);

                var result = await resultado.ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);
            } catch (Exception e) {
                return Json(MensagemErro(CommonMensagens.PadraoException, e), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Details Actions
        // GET: Responsibles/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);


                using (db = new SAMContext())
                {
                    Responsible responsible = db.Responsibles.Include("RelatedAdministrativeUnits").FirstOrDefault(r => r.Id == id);

                    //Responsible responsible = db.Responsibles.Find(id);
                    if (responsible == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id, responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Id, responsible.RelatedAdministrativeUnits.RelatedManagerUnit.Id, responsible.RelatedAdministrativeUnits.Id, null))
                        return View(responsible);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Responsibles/Create
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

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0)
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
        }

        // POST: Responsibles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResponsibleViewModel responsible)
        {
            try
            {
                if (String.IsNullOrEmpty(responsible.Email))
                {
                    ModelState.AddModelError("Email", "Por favor informe um email.");
                    SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                    return View(responsible);
                }

                if (ModelState.IsValid)
                {
                    responsible.CPF = responsible.CPF.Replace(".", string.Empty).Replace("-", string.Empty);
                    using (db = new SAMContext())
                    {
                        int _JaExiste = (from r in db.Responsibles where r.CPF == responsible.CPF && responsible.AdministrativeUnitId == r.AdministrativeUnitId && r.Status == true select r.Id).FirstOrDefault();

                        if (_JaExiste > 0)
                        {
                            ModelState.AddModelError("CPF", "CPF já está cadastrado com a UA informada!");
                            SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                            return View(responsible);
                        }

                        Responsible _responsable = new Responsible();
                        Mapper.CreateMap<ResponsibleViewModel, Responsible>();
                        _responsable = Mapper.Map<Responsible>(responsible);

                        _responsable.Status = true;
                        db.Responsibles.Add(_responsable);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId, responsible.SectionId);
                return View(responsible);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Responsibles/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    Responsible responsible = db.Responsibles
                                              .Include("RelatedAdministrativeUnits")
                                              .Include("RelatedAdministrativeUnits.RelatedManagerUnit")
                                              .Include("RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit")
                                              .Include("RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution")
                                              .FirstOrDefault(r => r.Id == id);
                    //Responsible responsible = db.Responsibles.Find(id);
                    if (responsible == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ExisteInventariosPendentesParaResponsavel(responsible.Id))
                        return MensagemErro("Responsável possui Inventários em andamento no sistema, não sendo possível a edição de sesu dados.");

                    if (ValidarRequisicao(responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id, responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Id, responsible.RelatedAdministrativeUnits.RelatedManagerUnit.Id, responsible.RelatedAdministrativeUnits.Id, null))
                    {
                        ResponsibleViewModel _responsable = new ResponsibleViewModel();
                        Mapper.CreateMap<Responsible, ResponsibleViewModel>();
                        _responsable = Mapper.Map<ResponsibleViewModel>(responsible);

                        _responsable.InstitutionId = responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id;
                        _responsable.BudgetUnitId = responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Id;
                        _responsable.ManagerUnitId = responsible.RelatedAdministrativeUnits.RelatedManagerUnit.Id;
                        _responsable.AdministrativeUnitIdAux = _responsable.AdministrativeUnitId;
                        _responsable.ManagerUnitIdAux = _responsable.ManagerUnitId;
                        _responsable.BudgetUnitIdAux = _responsable.BudgetUnitId;
                        _responsable.InstitutionIdAux = _responsable.InstitutionId;

                        SetCarregaHierarquia(_responsable.InstitutionId, _responsable.BudgetUnitId, _responsable.ManagerUnitId, _responsable.AdministrativeUnitId);
                        return View(_responsable);
                    }
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        //TODO FUNCAO SERAH MOVIDA FUTURAMENTE PARA CLASSE UTILITARIA POR SER CHAMADA POR MAIS DE UMA CONTROLLER
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

        // POST: Responsibles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ResponsibleViewModel responsible)
        {
            try
            {

                if (String.IsNullOrEmpty(responsible.Email))
                {
                    ModelState.AddModelError("Email", "Por favor informe um email.");
                    SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                    return View(responsible);
                }

                if (ModelState.IsValid)
                {
                    responsible.CPF = responsible.CPF.Replace(".", string.Empty).Replace("-", string.Empty);
                    using (db = new SAMContext())
                    {
                        int _JaExiste = (from r in db.Responsibles where r.Id != responsible.Id && r.CPF == responsible.CPF && responsible.AdministrativeUnitId == r.AdministrativeUnitId && r.Status == true select r.Id).FirstOrDefault();

                        if (_JaExiste > 0)
                        {
                            ModelState.AddModelError("CPF", "CPF já está cadastrado com a UA informada!");
                            SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                            return View(responsible);
                        }

                        if (responsible.InstitutionId != responsible.InstitutionIdAux)
                        {
                            var Bps = (from am in db.AssetMovements
                                       join a in db.Assets on am.AssetId equals a.Id
                                       where !a.flagVerificado.HasValue &&
                                             a.flagDepreciaAcumulada == 1 &&
                                             am.Status == true &&
                                             a.Status == true &&
                                             am.InstitutionId == responsible.InstitutionIdAux &&
                                             am.BudgetUnitId == responsible.BudgetUnitIdAux &&
                                             am.ManagerUnitId == responsible.ManagerUnitIdAux &&
                                             am.AdministrativeUnitId == responsible.AdministrativeUnitIdAux &&
                                             am.ResponsibleId == responsible.Id
                                       select am).AsNoTracking().Count();

                            if (Bps > 0)
                            {
                                ModelState.AddModelError("InstitutionId", "Orgão não pode ser alterado, pois existe " + Bps + " BP(s) vinculado(s) a este Responsável neste Orgão, favor verificar!");
                                SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                                return View(responsible);
                            }
                        }

                        if (responsible.BudgetUnitId != responsible.BudgetUnitIdAux)
                        {
                            var Bps = (from am in db.AssetMovements
                                       join a in db.Assets on am.AssetId equals a.Id
                                       where !a.flagVerificado.HasValue &&
                                             a.flagDepreciaAcumulada == 1 &&
                                             am.Status == true &&
                                             a.Status == true &&
                                             am.InstitutionId == responsible.InstitutionIdAux &&
                                             am.BudgetUnitId == responsible.BudgetUnitIdAux &&
                                             am.ManagerUnitId == responsible.ManagerUnitIdAux &&
                                             am.AdministrativeUnitId == responsible.AdministrativeUnitIdAux &&
                                             am.ResponsibleId == responsible.Id
                                       select am).AsNoTracking().Count();

                            if (Bps > 0)
                            {
                                ModelState.AddModelError("BudgetUnitId", "UO não pode ser alterado, pois existe " + Bps + " BP(s) vinculado(s) a este Responsável nesta UO, favor verificar!");
                                SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                                return View(responsible);
                            }
                        }

                        if (responsible.ManagerUnitId != responsible.ManagerUnitIdAux)
                        {
                            var Bps = (from am in db.AssetMovements
                                       join a in db.Assets on am.AssetId equals a.Id
                                       where !a.flagVerificado.HasValue &&
                                             a.flagDepreciaAcumulada == 1 &&
                                             am.Status == true &&
                                             a.Status == true &&
                                             am.InstitutionId == responsible.InstitutionIdAux &&
                                             am.BudgetUnitId == responsible.BudgetUnitIdAux &&
                                             am.ManagerUnitId == responsible.ManagerUnitIdAux &&
                                             am.AdministrativeUnitId == responsible.AdministrativeUnitIdAux &&
                                             am.ResponsibleId == responsible.Id
                                       select am).AsNoTracking().Count();

                            if (Bps > 0)
                            {
                                ModelState.AddModelError("ManagerUnitId", "UGE não pode ser alterado, pois existe " + Bps + " BP(s) vinculado(s) a este Responsável nesta UGE, favor verificar!");
                                SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                                return View(responsible);
                            }
                        }

                        if (responsible.AdministrativeUnitIdAux != responsible.AdministrativeUnitId)
                        {
                            var Bps = (from am in db.AssetMovements
                                       join a in db.Assets on am.AssetId equals a.Id
                                       where !a.flagVerificado.HasValue &&
                                             a.flagDepreciaAcumulada == 1 &&
                                             am.Status == true &&
                                             a.Status == true &&
                                             am.InstitutionId == responsible.InstitutionIdAux &&
                                             am.BudgetUnitId == responsible.BudgetUnitIdAux &&
                                             am.ManagerUnitId == responsible.ManagerUnitIdAux &&
                                             am.AdministrativeUnitId == responsible.AdministrativeUnitIdAux &&
                                             am.ResponsibleId == responsible.Id
                                       select am).AsNoTracking().Count();

                            if (Bps > 0)
                            {
                                ModelState.AddModelError("AdministrativeUnitId", "UA não pode ser alterada, pois existe " + Bps + " BP(s) vinculado(s) a este Responsável nesta UA, favor verificar!");

                                SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                                return View(responsible);
                            }
                        }

                        Responsible _responsable = new Responsible();
                        Mapper.CreateMap<ResponsibleViewModel, Responsible>();
                        _responsable = Mapper.Map<Responsible>(responsible);

                        _responsable.Status = true;

                        db.Entry(_responsable).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                SetCarregaHierarquia(responsible.InstitutionId, responsible.BudgetUnitId, responsible.ManagerUnitId, responsible.AdministrativeUnitId);
                return View(responsible);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
        #region Delete Actions
        // GET: Responsibles/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    Responsible responsible = db.Responsibles.Include("RelatedAdministrativeUnits").FirstOrDefault(r => r.Id == id);
                    //Responsible responsible = db.Responsibles.Find(id);

                    if (responsible == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id, responsible.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.Id, responsible.RelatedAdministrativeUnits.RelatedManagerUnit.Id, responsible.RelatedAdministrativeUnits.Id, null))
                        return View(responsible);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Responsibles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    if ((from am in db.AssetMovements where am.ResponsibleId == id && am.Status == true select am).AsNoTracking().Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    Responsible responsible = db.Responsibles.Find(id);

                    if (responsible.CPF == null)
                        responsible.CPF = "00000000000";
                    if (responsible.Email == null)
                        responsible.Email = "email@email.com";
                    responsible.Status = false;

                    Responsible _responsable = new Responsible();
                    Mapper.CreateMap<ResponsibleViewModel, Responsible>();
                    _responsable = Mapper.Map<Responsible>(responsible);

                    db.Entry(_responsable).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #endregion

    }
}
