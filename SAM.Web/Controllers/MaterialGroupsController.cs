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
using SAM.Web.Context;

namespace SAM.Web.Controllers
{
    public class MaterialGroupsController : BaseController
    {
        private SAMContext db;

        // GET: MaterialGroups
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                using (db = new SAMContext())
                {
                    var lstRetorno = (from s in db.MaterialGroups select s);
                    lstRetorno = SearchByFilter(searchString == null ? searchString : searchString.Trim(), lstRetorno);

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno
                                    .OrderBy(s => s.Code)
                                    .ThenBy(s => s.Description)
                                    .Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<MaterialGroup>(result, pageNumber, pageSize, lstRetorno.Count());

                    return View(retorno);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        private IQueryable<MaterialGroup> SearchByFilter(string searchString, IQueryable<MaterialGroup> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Description.Contains(searchString)
                                       || s.Code.ToString().Contains(searchString)
                                       || s.LifeCycle.ToString().Contains(searchString)
                                       || s.RateDepreciationMonthly.ToString().Contains(searchString)
                                       || s.ResidualValue.ToString().Contains(searchString));
            return result;
        }

        // GET: MaterialGroups/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    MaterialGroup materialGroup = db.MaterialGroups.Find(id);
                    if (materialGroup == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(materialGroup);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: MaterialGroups/Create
        public ActionResult Create()
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

        // POST: MaterialGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,LifeCycle,RateDepreciationMonthly,ResidualValue,Status")] MaterialGroup materialGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        db.MaterialGroups.Add(materialGroup);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                return View(materialGroup);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: MaterialGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    MaterialGroup materialGroup = db.MaterialGroups.Find(id);
                    if (materialGroup == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(materialGroup);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MaterialGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,LifeCycle,RateDepreciationMonthly,ResidualValue,Status")] MaterialGroup materialGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        db.Entry(materialGroup).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(materialGroup);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: MaterialGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    MaterialGroup materialGroup = db.MaterialGroups.Find(id);
                    if (materialGroup == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(materialGroup);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MaterialGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    MaterialGroup materialGroup = db.MaterialGroups.Find(id);

                    if ((from am in db.Assets where am.MaterialGroupCode == materialGroup.Code && am.Status == true select am).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    materialGroup.Status = false;
                    db.Entry(materialGroup).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #region JSON
        [HttpGet]
        public JsonResult ValoresDepreciacaoGrupoMaterial(int MaterialGroupCode)
        {
            using (ItemMaterialContext contexto = new ItemMaterialContext())
            {
                var materialGroup = (from mg in contexto.MaterialGroups
                                     where mg.Code == MaterialGroupCode
                                     select new
                                     {
                                         LifeCycle = mg.LifeCycle,
                                         ResidualValue = mg.ResidualValue,
                                         RateDepreciationMonthly = mg.RateDepreciationMonthly
                                     }).FirstOrDefault();

                return Json(materialGroup, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
