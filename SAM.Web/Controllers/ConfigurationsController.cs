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
    public class ConfigurationsController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: Configuracao
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = (from c in db.Configurations select c);
                //Filter
                lstRetorno = SearchByFilter(searchString, lstRetorno);
                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<Configuration>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Configuracao/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Configuration configuracao = db.Configurations.Find(id);
                if (configuracao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(configuracao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Configuracao/Create
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

        // POST: Configuracao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InitialYearMonthAsset,ReferenceYearMonthAsset")] Configuration configuracao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Configurations.Add(configuracao);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(configuracao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Configuracao/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Configuration configuracao = db.Configurations.Find(id);

                if (configuracao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(configuracao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Configuracao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InitialYearMonthAsset,ReferenceYearMonthAsset")] Configuration configuracao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(configuracao).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(configuracao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Configuracao/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Configuration configuracao = db.Configurations.Find(id);
                if (configuracao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(configuracao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Configuracao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Configuration configuracao = db.Configurations.Find(id);
                db.Configurations.Remove(configuracao);
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
        private IQueryable<Configuration> SearchByFilter(string searchString, IQueryable<Configuration> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.InitialYearMonthAsset.ToString().Contains(searchString) || s.ReferenceYearMonthAsset.ToString().Contains(searchString));

            return result;
        }
        #endregion
    }
}
