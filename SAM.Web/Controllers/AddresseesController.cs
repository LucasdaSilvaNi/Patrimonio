using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Models;
using SAM.Web.Common;
using PagedList;

namespace SAM.Web.Controllers
{
    public class AddresseesController : BaseController
    {
        private SAMContext db = new SAMContext();

        #region Actions
        #region Index Actions
        // GET: Addressees
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var lstRetorno = (from s in db.Addressees where s.Status == true select s);
                var lstFiltro = SearchByFilter(searchString, lstRetorno);

                var result = lstFiltro.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<Addressee>(result, pageNumber, pageSize, lstFiltro.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Details Actions
        // GET: Addressees/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Addressee addressee = db.Addressees.Find(id);
                if (addressee == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(addressee);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Addressees/Create
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

        // POST: Addressees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description")] Addressee addressee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    addressee.Status = true;
                    db.Addressees.Add(addressee);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(addressee);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Addressees/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Addressee addressee = db.Addressees.Find(id);

                if (addressee == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(addressee);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Addressees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description")] Addressee addressee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    addressee.Status = true;
                    db.Entry(addressee).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(addressee);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: Addressees/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Addressee addressee = db.Addressees.Find(id);

                if (addressee == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(addressee);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Addressees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Addressee addressee = db.Addressees.Find(id);
                addressee.Status = false;
                db.Entry(addressee).State = EntityState.Modified;
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
        private IQueryable<Addressee> SearchByFilter(string searchString, IQueryable<Addressee> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Description.Contains(searchString));
            return result;
        }
        #endregion
    }
}
