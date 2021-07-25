using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using SAM.Web.Common;
using PagedList;

namespace SAM.Web.Controllers
{
    public class RelationshipModuleProfilesController : BaseController
    {
        private ConfiguracaoContext db;

        #region Index Actions

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                using (db = new ConfiguracaoContext())
                {
                    var lstRetorno = (from r in db.RelationshipModuleProfiles
                                      select new RelationshipModuleProfileViewModel
                                      {
                                          Id = r.Id,
                                          IdModulo = r.ModuleId,
                                          IdPerfil = r.ProfileId,
                                          NomeModulo = r.RelatedModule.Name,
                                          DescricaoPerfil = r.RelatedProfile.Description,
                                          Status = r.Status
                                      });

                    lstRetorno = SearchByFilter(searchString, lstRetorno);
                    //lstRetorno = SortingByFilter(sortOrder, lstRetorno);

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno
                                    .OrderBy(s => s.IdModulo)
                                    .ThenBy(s => s.IdPerfil)
                                    .Skip(((pageNumber) - 1) * pageSize).Take(pageSize);

                    var retorno = new StaticPagedList<RelationshipModuleProfileViewModel>(result, pageNumber, pageSize, lstRetorno.Count());

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

                RelationshipModuleProfileViewModel relationshipModuleProfile;
                using (db = new ConfiguracaoContext())
                {
                    relationshipModuleProfile = (from r in db.RelationshipModuleProfiles
                                                 where r.Id == (int)id
                                                 select new RelationshipModuleProfileViewModel
                                                 {
                                                     Id = r.Id,
                                                     NomeModulo = r.RelatedModule.Name,
                                                     DescricaoPerfil = r.RelatedProfile.Description,
                                                     Status = r.Status
                                                 }).FirstOrDefault();
                }
                if (relationshipModuleProfile == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(relationshipModuleProfile);
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
                SetCarregaCombos();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RelationshipModuleProfileViewModel moduloPerfilView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        RelationshipModuleProfile rmp = (from r in db.RelationshipModuleProfiles
                                                         where r.ModuleId == moduloPerfilView.IdModulo && r.ProfileId == moduloPerfilView.IdPerfil
                                                         select r).FirstOrDefault();
                        if (rmp != null)
                        {
                            RelationshipModuleProfile moduloPerfil = (from r in db.RelationshipModuleProfiles
                                                                      where r.ModuleId == moduloPerfilView.IdModulo && r.ProfileId == moduloPerfilView.IdPerfil
                                                                      select r).FirstOrDefault();
                            moduloPerfil.Status = moduloPerfilView.Status;
                            db.Entry(moduloPerfil).State = EntityState.Modified;
                        }
                        else
                        {
                            RelationshipModuleProfile moduloPerfil = new RelationshipModuleProfile();
                            moduloPerfil.ModuleId = moduloPerfilView.IdModulo;
                            moduloPerfil.ProfileId = moduloPerfilView.IdPerfil;
                            moduloPerfil.Status = moduloPerfilView.Status;

                            db.RelationshipModuleProfiles.Add(moduloPerfil);
                        }
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                SetCarregaCombos(moduloPerfilView);

                return View(moduloPerfilView);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #region Edit Actions
        // GET: RelationshipTransactionsProfiles/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                RelationshipModuleProfileViewModel relationshipModuleProfile;
                using (db = new ConfiguracaoContext())
                {
                    relationshipModuleProfile = (from r in db.RelationshipModuleProfiles
                                                 where r.Id == (int)id
                                                 select new RelationshipModuleProfileViewModel
                                                 {
                                                     Id = r.Id,
                                                     IdModulo = r.ModuleId,
                                                     IdPerfil = r.ProfileId,
                                                     Status = r.Status
                                                 }).FirstOrDefault();
                }
                if (relationshipModuleProfile == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                SetCarregaCombos(relationshipModuleProfile);

                return View(relationshipModuleProfile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RelationshipModuleProfileViewModel moduloPerfilView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        RelationshipModuleProfile moduloPerfil = db.RelationshipModuleProfiles.Find(moduloPerfilView.Id);
                        moduloPerfil.Status = moduloPerfilView.Status;
                        db.Entry(moduloPerfil).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                SetCarregaCombos(moduloPerfilView);

                return View(moduloPerfilView);
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

                RelationshipModuleProfileViewModel relationshipModuleProfile;
                using (db = new ConfiguracaoContext())
                {
                    relationshipModuleProfile = (from r in db.RelationshipModuleProfiles
                                                 where r.Id == (int)id
                                                 select new RelationshipModuleProfileViewModel
                                                 {
                                                     Id = r.Id,
                                                     NomeModulo = r.RelatedModule.Name,
                                                     DescricaoPerfil = r.RelatedProfile.Description,
                                                     Status = r.Status
                                                 }).FirstOrDefault();
                }
                if (relationshipModuleProfile == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(relationshipModuleProfile);
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
                    RelationshipModuleProfile moduleProfile = db.RelationshipModuleProfiles.Find(id);
                    moduleProfile.Status = false;
                    db.Entry(moduleProfile).State = EntityState.Modified;
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

        #region View Methods

        /// <summary>
        /// Filtra os dados pela expressão informada
        /// </summary>
        /// <param name="searchString">Expressão informada</param>
        /// <param name="result">Modelo que será filtrado</param>
        private IQueryable<RelationshipModuleProfileViewModel> SearchByFilter(string searchString, IQueryable<RelationshipModuleProfileViewModel> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.NomeModulo.Contains(searchString)
                                       || s.DescricaoPerfil.Contains(searchString));
            return result;
        }

        private void SetCarregaCombos(RelationshipModuleProfileViewModel moduloPerfil = null)
        {
            using (db = new ConfiguracaoContext())
            {
                var listaPerfil = db.Profiles.OrderBy(p => p.Description).ToList();
                listaPerfil.ForEach(l => l.Description = l.Description + (l.Status ? " (Ativo)" : " (Inativo)"));

                var listaModulos = db.Modules.OrderBy(m => m.Name).ToList();
                listaModulos.ForEach(l => l.Name = l.Name + (l.Status ? " (Ativo)" : " (Inativo)"));

                if (moduloPerfil != null)
                {
                    ViewBag.Profiles = new SelectList(listaPerfil, "Id", "Description", moduloPerfil.IdPerfil);
                    ViewBag.Modules = new SelectList(listaModulos, "Id", "Name", moduloPerfil.IdModulo);
                }
                else
                {
                    ViewBag.Profiles = new SelectList(listaPerfil, "Id", "Description");
                    ViewBag.Modules = new SelectList(listaModulos, "Id", "Name");
                }
            }
            
        }

        #endregion
    }
}