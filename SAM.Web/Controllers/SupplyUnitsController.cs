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
    public class SupplyUnitsController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: SupplyUnits
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = (from s in db.SupplyUnits where s.Status == true select s);
                //Filter
                lstRetorno = SearchByFilter(searchString == null ? searchString : searchString.Trim(), lstRetorno);
                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<SupplyUnit>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SupplyUnits/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                SupplyUnit supplyUnit = db.SupplyUnits.Find(id);
                if (supplyUnit == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(supplyUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SupplyUnits/Create
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

        // POST: SupplyUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description")] SupplyUnit supplyUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    supplyUnit.Status = true;
                    db.SupplyUnits.Add(supplyUnit);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(supplyUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SupplyUnits/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                SupplyUnit supplyUnit = db.SupplyUnits.Find(id);
                if (supplyUnit == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(supplyUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: SupplyUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description")] SupplyUnit supplyUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    supplyUnit.Status = true;
                    db.Entry(supplyUnit).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                return View(supplyUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: SupplyUnits/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                SupplyUnit supplyUnit = db.SupplyUnits.Find(id);
                if (supplyUnit == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(supplyUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: SupplyUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                SupplyUnit supplyUnit = db.SupplyUnits.Find(id);
                supplyUnit.Status = false;
                db.Entry(supplyUnit).State = EntityState.Modified;
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
        private IQueryable<SupplyUnit> SearchByFilter(string searchString, IQueryable<SupplyUnit> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Description.Contains(searchString));

            return result;
        }
        #endregion           
    }
}
