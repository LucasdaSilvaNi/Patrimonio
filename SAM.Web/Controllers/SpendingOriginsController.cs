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
    public class SpendingOriginsController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: SpendingOrigins
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = db.SpendingOrigins.Where(m => m.Status);
                //Filter
                lstRetorno = SearchByFilter(searchString, lstRetorno);
                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<SpendingOrigin>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SpendingOrigins/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                SpendingOrigin spendingOrigin = db.SpendingOrigins.Find(id);
                if (spendingOrigin == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(spendingOrigin);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SpendingOrigins/Create
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

        // POST: SpendingOrigins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,ActivityIndicator")] SpendingOrigin spendingOrigin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    spendingOrigin.Status = true;
                    db.SpendingOrigins.Add(spendingOrigin);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(spendingOrigin);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SpendingOrigins/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                SpendingOrigin spendingOrigin = db.SpendingOrigins.Find(id);
                if (spendingOrigin == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(spendingOrigin);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: SpendingOrigins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,ActivityIndicator")] SpendingOrigin spendingOrigin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    spendingOrigin.Status = true;
                    db.Entry(spendingOrigin).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(spendingOrigin);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SpendingOrigins/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                SpendingOrigin spendingOrigin = db.SpendingOrigins.Find(id);
                if (spendingOrigin == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(spendingOrigin);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: SpendingOrigins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                SpendingOrigin spendingOrigin = db.SpendingOrigins.Find(id);
                spendingOrigin.Status = false;
                db.Entry(spendingOrigin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Métodos privados
        /// <summary>
        /// Filtra a pesquisa
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IQueryable<SpendingOrigin> SearchByFilter(string searchString, IQueryable<SpendingOrigin> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Description.Contains(searchString));

            return result;
        }
        #endregion
    }
}
