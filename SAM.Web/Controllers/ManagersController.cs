using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Models;
using PagedList;
using SAM.Web.Common;

namespace SAM.Web.Controllers
{
    public class ManagersController : BaseController
    {
        private SAMContext db = new SAMContext();

        #region Actions Methods
        #region Index Actions

        // GET: Managers
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = (from s in db.Managers where s.Status == true select s);

                lstRetorno = SearchByFilter(searchString, lstRetorno);
                //lstRetorno = SortingByFilter(sortOrder, lstRetorno);

                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<Manager>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Details Actions
        // GET: Managers/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Manager manager = db.Managers.Find(id);
                if (manager == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(manager);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Managers/Create
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

        // POST: Managers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Name, ShortName, AddressId, Telephone, Code, Status")] Manager manager)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    manager.Status = true;
                    manager.RelatedAddress.PostalCode = manager.RelatedAddress.PostalCode.Replace("-", string.Empty);
                    db.Managers.Add(manager);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(manager);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Managers/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Manager manager = db.Managers.Find(id);
                if (manager == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                manager.RelatedAddress = db.Addresses.FirstOrDefault(m => m.Id == manager.AddressId);
                return View(manager);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Managers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Name, ShortName, AddressId, Telephone, Code, Status")] Manager manager)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    manager.AddressId = manager.RelatedAddress.Id;
                    manager.Status = true;
                    manager.RelatedAddress.PostalCode = manager.RelatedAddress.PostalCode.Replace("-", string.Empty);
                    db.Entry(manager).State = EntityState.Modified;
                    db.Entry(manager.RelatedAddress).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(manager);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: Managers/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Manager manager = db.Managers.Find(id);
                if (manager == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(manager);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Managers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Manager manager = db.Managers.Find(id);
                manager.Status = false;
                db.Entry(manager).State = EntityState.Modified;
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
        #region View Methods

        /// <summary>
        /// Filtra os dados pela expressão informada
        /// </summary>
        /// <param name="searchString">Expressão informada</param>
        /// <param name="result">Modelo que será filtrado</param>
        private IQueryable<Manager> SearchByFilter(string searchString, IQueryable<Manager> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Name.Contains(searchString)
                                        || s.ShortName.Contains(searchString));
            return result;
        }

        /// <summary>
        /// Ordena os dados pelo parametro informado
        /// </summary>
        /// <param name="sortOrder">parametro de Ordenação</param>
        /// <param name="result">Modelo que será ordenado</param>
        private IQueryable<Manager> SortingByFilter(string sortOrder, IQueryable<Manager> result)
        {
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ShortNameSortParm = sortOrder == "shortName" ? "shortName_desc" : "shortName";
            ViewBag.CodeSortParm = sortOrder == "code" ? "code_desc" : "code";

            switch (sortOrder)
            {
                case "name":
                    result = result.OrderBy(m => m.Name);
                    break;
                case "name_desc":
                    result = result.OrderByDescending(m => m.Name);
                    break;
                case "shortName":
                    result = result.OrderBy(m => m.ShortName);
                    break;
                case "code":
                    result = result.OrderBy(m => m.Code);
                    break;
                case "code_desc":
                    result = result.OrderByDescending(m => m.Code);
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
