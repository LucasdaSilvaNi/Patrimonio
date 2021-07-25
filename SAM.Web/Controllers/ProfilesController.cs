using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Context;
using SAM.Web.Models;
using PagedList;
using System.Transactions;
using System.Collections.Generic;
using SAM.Web.ViewModels;
using System.Web.Security;
using SAM.Web.Common;

namespace SAM.Web.Controllers
{
    public class ProfilesController : BaseController
    {
        private ConfiguracaoContext db;

        #region Actions Methods
        #region Index Actions
        // GET: Profiles
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                List<Profile> lstRetorno;

                using (db = new ConfiguracaoContext())
                {
                    if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrWhiteSpace(searchString))
                        lstRetorno = (from m in db.Profiles where m.Description.Contains(searchString) select m).ToList();
                    else
                        lstRetorno = (from m in db.Profiles select m).ToList();
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var result = lstRetorno.OrderBy(s => s.Description).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<Profile>(result.Distinct().ToList(), pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Details Actions
        // GET: Profiles/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Profile profile;
                using (db = new ConfiguracaoContext())
                {
                    profile = db.Profiles.Find(id);
                }
                if (profile == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(profile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Profiles/Create
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
        public ActionResult Create([Bind(Include = "Id,Status,Description")] Profile profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        using (db = new ConfiguracaoContext())
                        {
                            db.Entry((profile)).State = EntityState.Added;
                            db.SaveChanges();


                            RelationshipProfileManagedSystem relationshipProfileManagedSystem = new RelationshipProfileManagedSystem();
                            relationshipProfileManagedSystem.ProfileId = profile.Id;
                            relationshipProfileManagedSystem.ManagedSystemId = 3;
                            db.Entry(relationshipProfileManagedSystem).State = EntityState.Added;
                            db.SaveChanges();


                            RelationshipModuleProfile relationshipModuleProfile;
                            var listaModuleProfile = (from t in db.Modules select t);

                            foreach (var _moduleProfile in listaModuleProfile)
                            {
                                relationshipModuleProfile = new RelationshipModuleProfile();
                                relationshipModuleProfile.ProfileId = profile.Id;
                                relationshipModuleProfile.ModuleId = _moduleProfile.Id;
                                relationshipModuleProfile.Status = false;
                                db.Entry(relationshipModuleProfile).State = EntityState.Added;
                                db.SaveChanges();
                            }

                            RelationshipTransactionProfile relationshipTransactionProfile;
                            var listaTransaction = (from t in db.Transactions select t);

                            foreach (var _transaction in listaTransaction)
                            {
                                relationshipTransactionProfile = new RelationshipTransactionProfile();
                                relationshipTransactionProfile.ProfileId = profile.Id;
                                relationshipTransactionProfile.TransactionId = _transaction.Id;
                                relationshipTransactionProfile.Status = false;
                                db.Entry(relationshipTransactionProfile).State = EntityState.Added;
                                db.SaveChanges();
                            }

                            transaction.Complete();
                        }
                        return RedirectToAction("Index");
                    }
                }
                return View(profile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Profiles/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Profile profile;
                using (db = new ConfiguracaoContext())
                {
                    profile = db.Profiles.Find(id);
                    if (profile == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);
                }
                return View(profile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Status,Description")] Profile profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(profile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: Profiles/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                Profile profile;
                using (db = new ConfiguracaoContext())
                {
                    profile = db.Profiles.Find(id);
                    if (profile == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);
                }

                return View(profile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Perfis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new ConfiguracaoContext())
                {
                    Profile profile = db.Profiles.Find(id);
                    profile.Status = false;
                    db.Entry(profile).State = EntityState.Modified;
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
    
    }

}