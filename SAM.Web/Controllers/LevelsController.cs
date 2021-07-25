using PagedList;
using SAM.Web.Common;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class LevelsController : BaseController
    {
        private SAMContext db;

        #region Actions
        #region Index Actions
        // GET: Levels
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                IQueryable<LevelViewModel> lstRetorno;

                using (db = new SAMContext())
                {
                    if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrWhiteSpace(searchString))
                    {
                        lstRetorno = (from l in db.Levels
                                      where l.Status == true
                                      && l.Description.Contains(searchString)
                                      select new LevelViewModel
                                      {
                                          Id = l.Id,
                                          Descricao = l.Description,
                                          NomeNivelSuperior = l.RelatedLevel.Description
                                      }).AsNoTracking();
                    }
                    else
                    {
                        lstRetorno = (from l in db.Levels
                                      where l.Status == true
                                      select new LevelViewModel
                                      {
                                          Id = l.Id,
                                          Descricao = l.Description,
                                          NomeNivelSuperior = l.RelatedLevel.Description
                                      }).AsNoTracking();
                    }



                    //Pagination
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<LevelViewModel>(result, pageNumber, pageSize, lstRetorno.Count());

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

        // GET: Levels/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                LevelViewModel level = null;
                using (db = new SAMContext())
                {
                    level = db.Levels.Where(l => l.Id == id)
                     .Select(l => new LevelViewModel
                     {
                         Id = l.Id,
                         Descricao = l.Description,
                         NomeNivelSuperior = l.RelatedLevel.Description,
                     }).FirstOrDefault();
                }

                if (level == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(level);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Levels/Create
        public ActionResult Create()
        {
            try
            {
                using (db = new SAMContext())
                    CarregaViewBag();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Niveis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdNivelSuperior,Descricao")] LevelViewModel levelModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Level level = new Level();
                    level.Id = levelModel.Id;
                    level.ParentId = levelModel.IdNivelSuperior;
                    level.Description = levelModel.Descricao;
                    level.Status = true;
                    using (db = new SAMContext())
                    {
                        db.Levels.Add(level);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                using (db = new SAMContext())
                    CarregaViewBag(levelModel);
                return View(levelModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Levels/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                LevelViewModel level = null;

                using (db = new SAMContext())
                {
                    level = db.Levels.Where(l => l.Id == id)
                                     .Select(l => new LevelViewModel
                                     {
                                         Id = l.Id,
                                         Descricao = l.Description,
                                         IdNivelSuperior = l.ParentId,
                                     }).FirstOrDefault();

                    if (level == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    CarregaViewBag(level);
                }
                return View(level);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Niveis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdNivelSuperior,Descricao")] LevelViewModel levelModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Level level = new Level();
                    level.Id = levelModel.Id;
                    level.ParentId = levelModel.IdNivelSuperior;
                    level.Description = levelModel.Descricao;
                    level.Status = true;
                    using (db = new SAMContext())
                    {
                        db.Entry(level).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                using (db = new SAMContext())
                CarregaViewBag(levelModel);
                return View(levelModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: Levels/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                LevelViewModel level = null;

                using (db = new SAMContext())
                        level = db.Levels.Where(l => l.Id == id)
                         .Select(l => new LevelViewModel
                         {
                             Id = l.Id,
                             Descricao = l.Description,
                             NomeNivelSuperior = l.RelatedLevel.Description,
                         }).FirstOrDefault();

                if (level == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);
                return View(level);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Niveis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    Level level = db.Levels.Find(id);
                    level.Status = false;
                    db.Entry(level).State = EntityState.Modified;
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

        private void CarregaViewBag(LevelViewModel level = null)
        {
            var result = db.Levels.Where(m => m.Status == true).OrderBy(m => m.Description).AsNoTracking().ToList();

            if (level == null)
                ViewBag.Levels = new SelectList(result, "Id", "Description");
            else
                ViewBag.Levels = new SelectList(result, "Id", "Description", level.IdNivelSuperior ?? 0);
        }
        #endregion
    }
}
