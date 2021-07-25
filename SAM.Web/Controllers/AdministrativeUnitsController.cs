using AutoMapper;
using PagedList;
using SAM.Web.Common;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace SAM.Web.Controllers
{
    public class AdministrativeUnitsController : BaseController
    {
        private SAMContext db = new SAMContext();
        private Hierarquia hierarquia;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;

        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _administrativeUnitId = perflLogado.AdministrativeUnitId;
        }

        #region Actions Methods
        #region Index Actions
        // GET: AdministrativeUnits
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.TemPermissao = PerfilAdmGeral();
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(AdministrativeUnit administrativeUnit)
        {
            string draw = Request.Form["draw"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            IQueryable<AdministrativeUnit> lstRetorno;
            try
            {

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

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    lstRetorno = (from s in db.AdministrativeUnits.Include("RelatedManagerUnit.RelatedBudgetUnit")
                                  where s.Status == true
                                  select s)
                                 .Where(s => s.RelatedManagerUnit.Code.Contains(currentFilter) ||
                                             s.RelatedManagerUnit.Description.Contains(currentFilter) ||
                                             s.Code.ToString().Contains(currentFilter) ||
                                             s.Description.Contains(currentFilter))
                                  .AsNoTracking();
                else
                    lstRetorno = (from s in db.AdministrativeUnits.Include("RelatedManagerUnit.RelatedBudgetUnit")
                                  where s.Status == true
                                  select s).AsNoTracking();

                if (PerfilAdmGeral() != true)
                {
                    if (ContemValor(_institutionId))
                        lstRetorno = (from s in lstRetorno where s.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId == _institutionId select s);

                    if (ContemValor(_budgetUnitId))
                        lstRetorno = (from s in lstRetorno where s.RelatedManagerUnit.BudgetUnitId == _budgetUnitId select s);

                    if (ContemValor(_managerUnitId))
                        lstRetorno = (from s in lstRetorno where s.ManagerUnitId == _managerUnitId select s);

                    if (ContemValor(_administrativeUnitId))
                        lstRetorno = (from s in lstRetorno where s.Id == _administrativeUnitId select s);
                }

                int totalRegistros = await lstRetorno.CountAsync();

                var result = await lstRetorno.OrderBy(s => s.Code).Skip(startRec).Take(length).ToListAsync();

                var administrativeResult = result.ConvertAll(new Converter<AdministrativeUnit, AdministrativeUnitTableViewModel>(new AdministrativeUnitTableViewModel().Create));

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = administrativeResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Details Actions

        // GET: AdministrativeUnits/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                AdministrativeUnit administrativeUnit = db.AdministrativeUnits
                                                        .Include("RelatedManagerUnit.RelatedBudgetUnit")
                                                        .FirstOrDefault(au => au.Id == id);

                if (administrativeUnit == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (ValidarRequisicao(administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId, administrativeUnit.RelatedManagerUnit.BudgetUnitId, administrativeUnit.ManagerUnitId, administrativeUnit.Id, null))
                    return View(administrativeUnit);
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: AdministrativeUnits/Create
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

        // POST: AdministrativeUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdministrativeUnitViewModel administrativeUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if ((from b in db.AdministrativeUnits where b.Code == administrativeUnit.Code && b.Status == true select b).AsNoTracking().Any())
                    {
                        ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra UA Ativa!");
                        SetCarregaHierarquia(administrativeUnit.InstitutionId, administrativeUnit.BudgetUnitId, administrativeUnit.ManagerUnitId);
                        return View(administrativeUnit);
                    }

                    AdministrativeUnit _administrativeUnit = new AdministrativeUnit();
                    Mapper.CreateMap<AdministrativeUnitViewModel, AdministrativeUnit>();
                    _administrativeUnit = Mapper.Map<AdministrativeUnit>(administrativeUnit);

                    _administrativeUnit.Status = true;
                    db.AdministrativeUnits.Add(_administrativeUnit);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(administrativeUnit.InstitutionId, administrativeUnit.BudgetUnitId, administrativeUnit.ManagerUnitId);
                return View(administrativeUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: AdministrativeUnits/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                AdministrativeUnit administrativeUnit = db.AdministrativeUnits
                                                        .Include("RelatedManagerUnit.RelatedBudgetUnit")
                                                        .FirstOrDefault(au => au.Id == id);
                if (administrativeUnit == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (ValidarRequisicao(administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId, administrativeUnit.RelatedManagerUnit.BudgetUnitId, administrativeUnit.ManagerUnitId, administrativeUnit.Id, null))
                {
                    AdministrativeUnitViewModel _administrativeUnit = new AdministrativeUnitViewModel(administrativeUnit);
                    _administrativeUnit.InstitutionIdAux = _administrativeUnit.InstitutionId;
                    _administrativeUnit.BudgetUnitIdAux = _administrativeUnit.BudgetUnitId;
                    _administrativeUnit.ManagerUnitIdAux = _administrativeUnit.ManagerUnitId;

                    SetCarregaHierarquia(_administrativeUnit.InstitutionId, _administrativeUnit.BudgetUnitId, _administrativeUnit.ManagerUnitId);
                    return View(_administrativeUnit);
                }
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: AdministrativeUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdministrativeUnitViewModel administrativeUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if ((from b in db.AdministrativeUnits where b.Code == administrativeUnit.Code && b.Id != administrativeUnit.Id && b.Status == true select b).AsNoTracking().Any())
                    {
                        ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra UA Ativa!");
                        SetCarregaHierarquia(administrativeUnit.InstitutionId, administrativeUnit.BudgetUnitId, administrativeUnit.ManagerUnitId);
                        return View(administrativeUnit);
                    }

                    if (administrativeUnit.InstitutionId != administrativeUnit.InstitutionIdAux
                        || administrativeUnit.BudgetUnitId != administrativeUnit.BudgetUnitIdAux
                        || administrativeUnit.ManagerUnitId != administrativeUnit.ManagerUnitIdAux)
                    {
                        string diferenca = String.Empty;
                        string campo = String.Empty;
                        if (administrativeUnit.InstitutionId != administrativeUnit.InstitutionIdAux)
                        {
                            diferenca = "Orgão";
                            campo = "InstitutionId";
                        }
                        else if (administrativeUnit.BudgetUnitId != administrativeUnit.BudgetUnitIdAux)
                        {
                            diferenca = "UO";
                            campo = "BudgetUnitId";
                        }
                        else
                        {
                            diferenca = "UGE";
                            campo = "ManagerUnitId";
                        }

                        var Bps = (from am in db.AssetMovements
                                   join a in db.Assets on am.AssetId equals a.Id
                                   where !a.flagVerificado.HasValue &&
                                         a.flagDepreciaAcumulada == 1 &&
                                         am.Status == true &&
                                         a.Status == true &&
                                         am.InstitutionId == administrativeUnit.InstitutionIdAux &&
                                         am.BudgetUnitId == administrativeUnit.BudgetUnitIdAux &&
                                         am.ManagerUnitId == administrativeUnit.ManagerUnitIdAux &&
                                         am.AdministrativeUnitId == administrativeUnit.Id
                                   select am).AsNoTracking().Count();



                        if (Bps > 0)
                        {
                            ModelState.AddModelError(campo, diferenca + " não pode ser alterado, pois existe " + Bps + " item(s) vinculado(s), favor verificar!");
                            SetCarregaHierarquia(administrativeUnit.InstitutionId, administrativeUnit.BudgetUnitId, administrativeUnit.ManagerUnitId);
                            return View(administrativeUnit);
                        }
                        if (VerificaUsuario(administrativeUnit) > 0)
                        {
                            ModelState.AddModelError(campo, diferenca + " não pode ser alterada, pois existe perfis de usuários vinculado(s), favor verificar!");
                            SetCarregaHierarquia(administrativeUnit.InstitutionId, administrativeUnit.BudgetUnitId, administrativeUnit.ManagerUnitId);
                            return View(administrativeUnit);
                        }

                    }

                    AdministrativeUnit _administrativeUnit = new AdministrativeUnit();
                    Mapper.CreateMap<AdministrativeUnitViewModel, AdministrativeUnit>();
                    _administrativeUnit = Mapper.Map<AdministrativeUnit>(administrativeUnit);

                    _administrativeUnit.Status = true;
                    db.Entry(_administrativeUnit).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(administrativeUnit.InstitutionId, administrativeUnit.BudgetUnitId, administrativeUnit.ManagerUnitId);
                return View(administrativeUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }



        #endregion
        #region Delete Actions
        // GET: AdministrativeUnits/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                AdministrativeUnit administrativeUnit = db.AdministrativeUnits
                                                        .Include("RelatedManagerUnit.RelatedBudgetUnit")
                                                        .FirstOrDefault(au => au.Id == id);

                if (administrativeUnit == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (ValidarRequisicao(administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId, administrativeUnit.RelatedManagerUnit.BudgetUnitId, administrativeUnit.ManagerUnitId, administrativeUnit.Id, null))
                    return View(administrativeUnit);
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: AdministrativeUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((from b in db.Sections where b.AdministrativeUnitId == id && b.Status == true select b).AsNoTracking().Any())
                return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

            if ((from r in db.Responsibles where r.AdministrativeUnitId == id && r.Status == true select r).AsNoTracking().Any())
                return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

            if ((from am in db.AssetMovements where am.AdministrativeUnitId == id && am.Status == true select am).AsNoTracking().Any())
                return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

            AdministrativeUnit administrativeUnit = db.AdministrativeUnits.Find(id);
            administrativeUnit.Status = false;
            db.Entry(administrativeUnit).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion
        #endregion

        #region Métodos Privados
        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null && variavel != 0)
                retorno = true;
            return retorno;
        }

        private int VerificaUsuario(AdministrativeUnitViewModel administrativeUnit)
        {
            UsuarioContext dbUsuario = new UsuarioContext();
            return (from r in dbUsuario.RelationshipUserProfileInstitutions
                    where r.InstitutionId == administrativeUnit.InstitutionIdAux &&
                     r.InstitutionId == administrativeUnit.InstitutionIdAux &&
                     r.BudgetUnitId == administrativeUnit.BudgetUnitIdAux &&
                     r.ManagerUnitId == administrativeUnit.ManagerUnitIdAux &&
                     r.AdministrativeUnitId == administrativeUnit.Id
                    select r).AsNoTracking().Count();
        }

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0)
        {
            getHierarquiaPerfil();
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
        #endregion
    }
}
