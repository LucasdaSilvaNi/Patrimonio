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

namespace SAM.Web.Controllers
{
    public class MaterialClassesController : BaseController
    {
        private SAMContext db = new SAMContext();

        #region Actions
        #region Index Actions
        // GET: MaterialClasses
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = (from m in db.MaterialClasses where m.Status == true select m);
                //Filter
                lstRetorno = SearchByFilter(searchString, lstRetorno);
                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<MaterialClass>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Details Actions
        // GET: MaterialClasses/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialClass materialClass = db.MaterialClasses.Find(id);
                if (materialClass == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);
                return View(materialClass);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: MaterialClasses/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.GruposMateriais = new SelectList(db.MaterialGroups.OrderBy(m => m.Description), "Id", "Description");
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MaterialClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,MaterialGroupId")] MaterialClass materialClass)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    materialClass.Status = true;
                    db.MaterialClasses.Add(materialClass);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(materialClass);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: MaterialClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialClass materialClass = db.MaterialClasses.Find(id);
                if (materialClass == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                ViewBag.GruposMateriais = new SelectList(db.MaterialGroups.OrderBy(m => m.Description), "Id", "Description", materialClass.MaterialGroupId);
                return View(materialClass);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MaterialClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,MaterialGroupId")] MaterialClass materialClass)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    materialClass.Status = true;
                    db.Entry(materialClass).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(materialClass);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: MaterialClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialClass materialClass = db.MaterialClasses.Find(id);
                if (materialClass == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);
                return View(materialClass);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MaterialClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MaterialClass materialClass = db.MaterialClasses.Find(id);
                materialClass.Status = false;
                db.Entry(materialClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion


        #region Métodos privados
        /// <summary>
        /// Filtra a pesquisa
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IQueryable<MaterialClass> SearchByFilter(string searchString, IQueryable<MaterialClass> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Description.ToString().Contains(searchString) || s.RelatedMaterialGroup.Description.ToString().Contains(searchString));

            return result;
        }
        #endregion
    }
}
