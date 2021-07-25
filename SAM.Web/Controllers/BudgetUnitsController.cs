using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Models;
using PagedList;
using SAM.Web.Common;
using System.Threading.Tasks;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class BudgetUnitsController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;

        private int _institutionId;
        private int? _budgetUnitId;

        public void getHierarquiaPerfil()
        {

            if (db == null)
                db = new SAMContext();

            var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
        }

        #region Actions Methods
        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null && variavel != 0)
                retorno = true;
            return retorno;
        }
        #region Index Actions
        // GET: BudgetUnits
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.TemPermissao = PerfilAdmGeral();

                return View();
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(BudgetUnit budgetunit)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            IQueryable<BudgetUnit> lstRetorno = null;

            try
            {
                db = new SAMContext();
                try
                {
                    if (hierarquiaLogin.Contains(','))
                    {
                        int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                        _institutionId = IdsHieraquia[0];
                        _budgetUnitId = IdsHieraquia[1];
                    }
                    else
                    {
                        getHierarquiaPerfil();
                    }
                }
                catch (Exception e) {
                    getHierarquiaPerfil();
                }

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                {
                    lstRetorno = (from b in db.BudgetUnits where b.Status == true select b).Include(x => x.RelatedInstitution)
                    .Where(b => b.Code.ToString().Contains(currentFilter) ||
                                b.Description.ToString().Contains(currentFilter) ||
                                b.RelatedInstitution.Code.Contains(currentFilter) ||
                                b.RelatedInstitution.Description.Contains(currentFilter))
                                .AsNoTracking().OrderBy(b => b.Id);
                }
                else
                {
                    lstRetorno = (from r in db.BudgetUnits where r.Status == true select r).Include(x => x.RelatedInstitution).AsNoTracking().OrderBy(r => r.Id);
                }

                if (PerfilAdmGeral() != true)
                {
                    if (ContemValor(_institutionId))
                        lstRetorno = (from s in lstRetorno where s.InstitutionId == _institutionId select s);

                    if (ContemValor(_budgetUnitId))
                        lstRetorno = (from s in lstRetorno where s.Id == _budgetUnitId select s);
                }

                int totalRegistros = lstRetorno.Count();

                var result = await lstRetorno.Skip(startRec).Take(length).ToListAsync();

                var budgetResult = result.ConvertAll(new Converter<BudgetUnit, BudgetUnitTableViewModel>(BudgetUnitTableViewModel.GetInstancia));

                var budgetUnits = BudgetUnitTableViewModel.Ordenar(order, orderDir, budgetResult);

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = budgetUnits }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }

        }

        #endregion
        #region Details Actions
        // GET: BudgetUnits/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    BudgetUnit budgetUnit = db.BudgetUnits.Include("RelatedInstitution").AsNoTracking().FirstOrDefault(b => b.Id == id);

                    if (budgetUnit == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(budgetUnit.InstitutionId, budgetUnit.Id, null, null, null))
                        return View(budgetUnit);
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
        // GET: BudgetUnits/Create
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

        private void SetCarregaHierarquia(int modelInstitutionId = 0)
        {
            hierarquia = new Hierarquia();
            if (PerfilAdmGeral())
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
            else
            {
                getHierarquiaPerfil();
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);
            }
        }

        // POST: BudgetUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InstitutionId,Code,Description,Direct")] BudgetUnit budgetUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db = new SAMContext();
                    if ((from b in db.BudgetUnits where b.Code == budgetUnit.Code && b.Status == true select b).Any())
                    {
                        ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra UO Ativa!");
                        SetCarregaHierarquia(budgetUnit.InstitutionId);
                        return View(budgetUnit);
                    }

                    budgetUnit.Status = true;
                    db.BudgetUnits.Add(budgetUnit);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                SetCarregaHierarquia(budgetUnit.InstitutionId);
                return View(budgetUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: BudgetUnits/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    BudgetUnit budgetUnit = db.BudgetUnits.Find(id);

                    if (budgetUnit == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(budgetUnit.InstitutionId, budgetUnit.Id, null, null, null))
                    {
                        SetCarregaHierarquia();
                        return View(budgetUnit);
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

        // POST: BudgetUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InstitutionId,Code,Description,Direct")] BudgetUnit budgetUnit)
        {
            if (ModelState.IsValid)
            {
                db = new SAMContext();
                if ((from b in db.BudgetUnits where b.Code == budgetUnit.Code && b.Id != budgetUnit.Id && b.Status == true select b).Any())
                {
                    ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra UO Ativa!");
                    SetCarregaHierarquia(budgetUnit.InstitutionId);
                    return View(budgetUnit);
                }

                budgetUnit.Status = true;
                db.Entry(budgetUnit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            SetCarregaHierarquia(budgetUnit.InstitutionId);
            return View(budgetUnit);
        }
        #endregion
        #region Delete Actions
        // GET: BudgetUnits/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    BudgetUnit budgetUnit = db.BudgetUnits.Include("RelatedInstitution").FirstOrDefault(b => b.Id == id);
                    if (budgetUnit == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(budgetUnit.InstitutionId, budgetUnit.Id, null, null, null))
                        return View(budgetUnit);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: BudgetUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db = new SAMContext();
            if ((from b in db.ManagerUnits where b.BudgetUnitId == id && b.Status == true select b).AsNoTracking().Any())
                return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

            if ((from am in db.AssetMovements where am.BudgetUnitId == id && am.Status == true select am).AsNoTracking().Any())
                return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

            if ((from i in db.Initials where i.BudgetUnitId == id && i.Status == true select i).AsNoTracking().Any())
                return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

            if ((from o in db.OutSourceds where o.BudgetUnitId == id && o.Status == true select o).AsNoTracking().Any())
                return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

            BudgetUnit budgetUnit = db.BudgetUnits.Find(id);
            budgetUnit.Status = false;
            db.Entry(budgetUnit).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        

        #endregion
    }
}
