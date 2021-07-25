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
    public class StatesConservationController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: EstadosConservacao
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = (from s in db.StateConservations select s);

                //Filter
                lstRetorno = SearchByFilter(searchString == null ? searchString : searchString.Trim(), lstRetorno);
                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<StateConservation>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: EstadosConservacao/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                StateConservation estadoConservacao = db.StateConservations.Find(id);
                if (estadoConservacao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(estadoConservacao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: EstadosConservacao/Create
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

        // POST: EstadosConservacao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description")] StateConservation estadoConservacao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.StateConservations.Add(estadoConservacao);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(estadoConservacao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: EstadosConservacao/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                StateConservation estadoConservacao = db.StateConservations.Find(id);
                if (estadoConservacao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(estadoConservacao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: EstadosConservacao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description")] StateConservation estadoConservacao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(estadoConservacao).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(estadoConservacao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: EstadosConservacao/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                StateConservation estadoConservacao = db.StateConservations.Find(id);
                if (estadoConservacao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(estadoConservacao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: EstadosConservacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if ((from am in db.AssetMovements where am.StateConservationId == id && am.Status == true select am).Any())
                    return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                StateConservation estadoConservacao = db.StateConservations.Find(id);
                db.StateConservations.Remove(estadoConservacao);
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
        private IQueryable<StateConservation> SearchByFilter(string searchString, IQueryable<StateConservation> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Description.Contains(searchString));

            return result;
        }
        #endregion
    }
}
