using PagedList;
using SAM.Web.Common;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SAM.Web.Controllers
{
    public class ShortDescriptionItemController : BaseController
    {
        private SAMContext db;

        // GET: ShortDescriptionItem
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                IQueryable<ShortDescriptionItem> lstRetorno;
                using (db = new SAMContext()) {
                    if (!String.IsNullOrEmpty(searchString) || !String.IsNullOrWhiteSpace(searchString))
                        lstRetorno = (from d in db.ShortDescriptionItens select d)
                                     .Where(s => s.Id.ToString().Contains(searchString)
                                               || s.Description.Contains(searchString));
                    else
                        lstRetorno = (from d in db.ShortDescriptionItens select d);

                    //Guarda o filtro
                    ViewBag.CurrentFilter = searchString;
                    //Pagination
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<ShortDescriptionItem>(result, pageNumber, pageSize, lstRetorno.Count());

                    return View(retorno);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        //GET: ShortDescriptionItem/Details/id
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    var descriptionItem = db.ShortDescriptionItens.Find(id);

                    if (descriptionItem == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(descriptionItem);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: ShortDescriptionItem/Edit/id
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    var result = (from d in db.ShortDescriptionItens where d.Id == id select d).FirstOrDefault();
                    if (result == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    var shortDescriptionItemModel = new ShortDescriptionItem()
                    {
                        Id = result.Id,
                        Description = result.Description
                    };

                    return View(shortDescriptionItemModel);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: DescriptionItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShortDescriptionItem shortDescriptionItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        var shortDescriptionItemDB = (from d in db.ShortDescriptionItens where d.Id == shortDescriptionItem.Id select d).FirstOrDefault();
                        shortDescriptionItemDB.Description = shortDescriptionItem.Description;

                        db.Entry(shortDescriptionItemDB).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        //GET: ShortDescriptionItem/Delete/id
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    var shortDescriptionItem = db.ShortDescriptionItens.Find(id);
                    if (shortDescriptionItem == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(shortDescriptionItem);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: ShortDescriptionItem/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    if ((from am in db.Assets where am.ShortDescriptionItemId == id && am.Status == true select am).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    var descriptionItem = db.ShortDescriptionItens.Find(id);
                    db.Entry(descriptionItem).State = EntityState.Deleted;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: ShortDescriptionItem/Create
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

        // POST: ShortDescriptionItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Description")] ShortDescriptionItem shortDescriptionItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        db.ShortDescriptionItens.Add(shortDescriptionItem);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
                return View(shortDescriptionItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        public JsonResult VerificarDescricaoResumida(string Material)
        {
            ShortDescriptionItem _shortDescriptionItem;
            ShortDescriptionItem existeShortDescription;

            using (db = new SAMContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                existeShortDescription = (from s in db.ShortDescriptionItens where s.Description.Trim() == Material.Trim() select s).FirstOrDefault();

                if (existeShortDescription != null)
                    _shortDescriptionItem = existeShortDescription;
                else
                {
                    _shortDescriptionItem = new ShortDescriptionItem();
                    _shortDescriptionItem.Description = Material;

                    db.Entry(_shortDescriptionItem).State = EntityState.Added;
                    db.SaveChanges();
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var retorno = serializer.Serialize(_shortDescriptionItem);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

    }
}