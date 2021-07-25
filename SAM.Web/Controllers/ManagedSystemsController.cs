using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Context;
using SAM.Web.Models;
using PagedList;
using SAM.Web.Common;

namespace SAM.Web.Controllers
{
    public class ManagedSystemsController : BaseController
    {
        private ConfiguracaoContext db;

        #region Actions Methods
        #region Index Actions

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                using (db = new ConfiguracaoContext())
                {
                    var lstRetorno = (from s in db.ManagedSystems select s);
                    lstRetorno = SearchByFilter(searchString, lstRetorno);
                    //lstRetorno = SortingByFilter(sortOrder, lstRetorno);

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno.OrderBy(s => s.Sequence).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<ManagedSystem>(result, pageNumber, pageSize, lstRetorno.Count());

                    return View(retorno);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
        #region Details Actions

        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new ConfiguracaoContext())
                {
                    ManagedSystem managedSystem = db.ManagedSystems.Find(id);
                    if (managedSystem == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(managedSystem);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
        #region Create Actions

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Name, Description, Sequence, Status")] ManagedSystem managedSystem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        db.ManagedSystems.Add(managedSystem);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                return View(managedSystem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions

        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new ConfiguracaoContext())
                {
                    ManagedSystem managedSystem = db.ManagedSystems.Find(id);
                    if (managedSystem == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(managedSystem);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Name, Description, Sequence, Status")] ManagedSystem managedSystem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        db.Entry(managedSystem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(managedSystem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions

        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new ConfiguracaoContext())
                {
                    ManagedSystem managedSystem = db.ManagedSystems.Find(id);
                    if (managedSystem == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(managedSystem);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new ConfiguracaoContext())
                {
                    ManagedSystem managedSystem = db.ManagedSystems.Find(id);
                    managedSystem.Status = false;
                    db.Entry(managedSystem).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #endregion
        #region View Methods

        /// <summary>
        /// Filtra os dados pela expressão informada
        /// </summary>
        /// <param name="searchString">Expressão informada</param>
        /// <param name="result">Modelo que será filtrado</param>
        private IQueryable<ManagedSystem> SearchByFilter(string searchString, IQueryable<ManagedSystem> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString));
            return result;
        }

        /// <summary>
        /// Ordena os dados pelo parametro informado
        /// </summary>
        /// <param name="sortOrder">parametro de Ordenação</param>
        /// <param name="result">Modelo que será ordenado</param>
        private IQueryable<ManagedSystem> SortingByFilter(string sortOrder, IQueryable<ManagedSystem> result)
        {
            ViewBag.SiglaSortParm = string.IsNullOrEmpty(sortOrder) ? "sigla_desc" : "";
            ViewBag.DescriptionSortParm = sortOrder == "description" ? "description_desc" : "description";

            switch (sortOrder)
            {
                case "sigla":
                    result = result.OrderBy(m => m.Name);
                    break;
                case "sigla_desc":
                    result = result.OrderByDescending(m => m.Name);
                    break;
                case "description":
                    result = result.OrderBy(m => m.Description);
                    break;
                case "description_desc":
                    result = result.OrderByDescending(m => m.Description);
                    break;
                default:
                    result = result.OrderBy(m => m.Name);
                    break;
            }

            return result;
        }

        #endregion
    }
}
