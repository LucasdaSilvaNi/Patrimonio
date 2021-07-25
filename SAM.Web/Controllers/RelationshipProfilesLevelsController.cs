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
    public class RelationshipProfileLevelsController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: RelPerfilNivel
        public ActionResult Index()
        {
            try
            {
                return View(db.RelationshipProfileLevels.ToList());
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: RelPerfilNivel/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                RelationshipProfileLevel relPerfilNivel = db.RelationshipProfileLevels.Find(id);
                if (relPerfilNivel == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(relPerfilNivel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: RelPerfilNivel/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Perfis = new SelectList(db.Profiles.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.Niveis = new SelectList(db.Levels.OrderBy(m => m.Description), "Id", "Description");

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: RelPerfilNivel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdPerfil,IdNivel")] RelationshipProfileLevel relPerfilNivel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RelationshipProfileLevels.Add(relPerfilNivel);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(relPerfilNivel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: RelPerfilNivel/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                RelationshipProfileLevel relPerfilNivel = db.RelationshipProfileLevels.Find(id);
                if (relPerfilNivel == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                ViewBag.Perfis = new SelectList(db.Profiles.OrderBy(m => m.Description), "Id", "Description", relPerfilNivel.ProfileId);
                ViewBag.Niveis = new SelectList(db.Levels.OrderBy(m => m.Description), "Id", "Description", relPerfilNivel.LevelId);

                return View(relPerfilNivel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: RelPerfilNivel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdPerfil,IdNivel")] RelationshipProfileLevel relPerfilNivel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(relPerfilNivel).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(relPerfilNivel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: RelPerfilNivel/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                RelationshipProfileLevel relPerfilNivel = db.RelationshipProfileLevels.Find(id);
                if (relPerfilNivel == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(relPerfilNivel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: RelPerfilNivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                RelationshipProfileLevel relPerfilNivel = db.RelationshipProfileLevels.Find(id);
                db.RelationshipProfileLevels.Remove(relPerfilNivel);
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
