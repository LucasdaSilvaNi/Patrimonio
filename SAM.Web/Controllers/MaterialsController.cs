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

namespace SAM.Web.Controllers
{
    public class MaterialsController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: Materiais
        public ActionResult Index()
        {
            try
            {
                return View(db.Materials.Where(m => m.Status).ToList());
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Materiais/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Material material = db.Materials.Find(id);
                if (material == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(material);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Materiais/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.ClassesMateriais = new SelectList(db.MaterialClasses.OrderBy(m => m.Description), "Id", "Description");
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Materiais/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,MaterialClassId")] Material material)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    material.Status = true;
                    db.Materials.Add(material);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(material);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Materiais/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Material material = db.Materials.Find(id);
                if (material == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                ViewBag.ClassesMateriais = new SelectList(db.MaterialClasses.OrderBy(m => m.Description), "Id", "Description", material.MaterialClassId);
                return View(material);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Materiais/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,MaterialClassId")] Material material)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    material.Status = true;
                    db.Entry(material).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(material);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: Materiais/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Material material = db.Materials.Find(id);
                if (material == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                ViewBag.ClassesMateriais = new SelectList(db.MaterialClasses.OrderBy(m => m.Description), "Id", "Description", material.MaterialClassId);
                return View(material);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Materiais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Material material = db.Materials.Find(id);
                material.Status = false;
                db.Entry(material).State = EntityState.Modified;
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
    }
}
