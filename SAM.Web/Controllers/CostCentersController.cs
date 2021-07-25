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
    public class CostCentersController : BaseController
    {
        private SAMContext db = new SAMContext();

        #region Actions Methods
        #region Index Actions
        // GET: CostCenters
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = (from s in db.CostCenters where s.Status == true select s);
                //Filter
                lstRetorno = SearchByFilter(searchString, lstRetorno);
                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<CostCenter>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Details Actions
        // GET: CostCenters/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                CostCenter costCenter = db.CostCenters.Find(id);

                if (costCenter == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(costCenter);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: CostCenters/Create
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

        // POST: CostCenters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description")] CostCenter costCenter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (CostCenterExist(costCenter.Code))
                    {
                        ViewBag.CustoExiste = "Centro de Custo já está cadastrado!";
                        return View(costCenter);
                    }

                    costCenter.Status = true;
                    db.CostCenters.Add(costCenter);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(costCenter);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
        #region Edit Actions
        // GET: CostCenters/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                CostCenter costCenter = db.CostCenters.Find(id);

                if (costCenter == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(costCenter);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: CostCenters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description")] CostCenter costCenter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    costCenter.Status = true;
                    db.Entry(costCenter).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(costCenter);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: CostCenters/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                CostCenter costCenter = db.CostCenters.Find(id);

                if (costCenter == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(costCenter);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: CostCenters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CostCenter costCenter = db.CostCenters.Find(id);
                costCenter.Status = false;
                db.Entry(costCenter).State = EntityState.Modified;
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
        private IQueryable<CostCenter> SearchByFilter(string searchString, IQueryable<CostCenter> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Description.Contains(searchString));
            return result;
        }

        /// <summary>
        /// Ordena os dados pelo parametro informado
        /// </summary>
        /// <param name="sortOrder">parametro de Ordenação</param>
        /// <param name="result">Modelo que será ordenado</param>
        private IQueryable<CostCenter> SortingByFilter(string sortOrder, IQueryable<CostCenter> result)
        {
            ViewBag.ManagerSortParm = sortOrder == "manager" ? "manager_desc" : "manager_desc";
            ViewBag.CodeSortParm = string.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewBag.DescriptionSortParm = sortOrder == "description_desc" ? "description_desc" : "description_desc";

            switch (sortOrder)
            {
                case "description":
                    result = result.OrderBy(m => m.Description);
                    break;
                case "description_desc":
                    result = result.OrderByDescending(m => m.Description);
                    break;
                case "code":
                    result = result.OrderBy(m => m.Code);
                    break;
                case "code_desc":
                    result = result.OrderByDescending(m => m.Code);
                    break;
                default:
                    result = result.OrderBy(m => m.Description);
                    break;
            }

            return result;
        }

        #endregion

        private bool CostCenterExist(string _code)
        {
            bool retorno = false;
            var result = (from a in db.CostCenters where a.Status == true&& a.Code == _code select a);

            if (result.ToList().Count > 0)
                retorno = true;

            return retorno;
        }
    }
}
