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
using SAM.Web.Common;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class RelationshipTransactionsProfilesController : BaseController
    {
        private ConfiguracaoContext db = new ConfiguracaoContext();

        #region Actions
        #region Index Actions
        // GET: RelationshipTransactionsProfiles
        public ActionResult Index()
        {
            try
            {
                ViewBag.Profiles = new SelectList(db.Profiles.Where(m => m.Status).OrderBy(m => m.Description), "Id", "Description");
                return View();
                //return View(db.RelationshipTransactionsProfiles.ToList());
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Details Actions
        // GET: RelationshipTransactionsProfiles/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                RelationshipTransactionProfile relationshipTransactionProfile = db.RelationshipTransactionProfiles.Find(id);
                if (relationshipTransactionProfile == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(relationshipTransactionProfile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: RelationshipTransactionsProfiles/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Perfis = new SelectList(db.Profiles.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.Transacoes = new SelectList(db.Transactions.OrderBy(m => m.Description), "Id", "Description");

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: RelationshipTransactionsProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdTransacao,IdPerfil,Status,Editavel,FiltraCombo")] RelationshipTransactionProfile relTransacaoPerfil)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RelationshipTransactionProfiles.Add(relTransacaoPerfil);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(relTransacaoPerfil);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Save Actions - Ajax Calls
        // POST: RelationshipTransactionsProfiles/Save
        public JsonResult Save(int transactionId, int profileId)
        {
            RelationshipTransactionProfile relationshipTransactionProfile
                = db.RelationshipTransactionProfiles.FirstOrDefault(r => r.TransactionId == transactionId && r.ProfileId == profileId);

            if (relationshipTransactionProfile != null)
            {
                relationshipTransactionProfile.Status = !relationshipTransactionProfile.Status;

                db.Entry(relationshipTransactionProfile).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(Content(""), JsonRequestBehavior.AllowGet);
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

                RelationshipTransactionProfile relationshipTransactionProfile = db.RelationshipTransactionProfiles.Find(id);
                if (relationshipTransactionProfile == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                ViewBag.Perfis = new SelectList(db.Profiles.OrderBy(m => m.Description), "Id", "Description", relationshipTransactionProfile.ProfileId);
                ViewBag.Transacoes = new SelectList(db.Transactions.OrderBy(m => m.Description), "Id", "Description", relationshipTransactionProfile.TransactionId);
                return View(relationshipTransactionProfile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: RelationshipTransactionsProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdTransacao,IdPerfil,Status,Editavel,FiltraCombo")] RelationshipTransactionProfile relTransacaoPerfil)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(relTransacaoPerfil).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(relTransacaoPerfil);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: RelationshipTransactionsProfiles/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                RelationshipTransactionProfile relationshipTransactionProfile = db.RelationshipTransactionProfiles.Find(id);
                if (relationshipTransactionProfile == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(relationshipTransactionProfile);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: RelationshipTransactionsProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                RelationshipTransactionProfile relTransacaoPerfil = db.RelationshipTransactionProfiles.Find(id);
                db.RelationshipTransactionProfiles.Remove(relTransacaoPerfil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        public JsonResult RecuperaDados(int profileId)
        {
            try
            {
                var _transacoesRelacionadasAoPerfil = (from tp in db.RelationshipTransactionProfiles
                                                       join t in db.Transactions on tp.TransactionId equals t.Id
                                                       where tp.ProfileId == profileId && tp.Status == true

                                                       select t
                                                                     ).AsQueryable();


                var _trasacoesNaoRelacionadasAoPerfil = (from t in db.Transactions
                                                         where !_transacoesRelacionadasAoPerfil.Select(x => x.Id).Contains(t.Id)

                                                         select t
                                                         ).AsQueryable();

                var transacaoRel = (from t in _transacoesRelacionadasAoPerfil

                                    select new TransactionViewModel()
                                    {
                                        Id = t.Id,
                                        ModuleId = t.ModuleId,
                                        Status = t.Status,
                                        Initial = t.Initial,
                                        Description = t.Description,
                                        Path = t.Path,
                                        TypeTransactionId = t.TypeTransactionId
                                    }).ToList();

                var transacaoNaoRel = (from t in _trasacoesNaoRelacionadasAoPerfil

                                       select new TransactionViewModel()
                                       {
                                           Id = t.Id,
                                           ModuleId = t.ModuleId,
                                           Status = t.Status,
                                           Initial = t.Initial,
                                           Description = t.Description,
                                           Path = t.Path,
                                           TypeTransactionId = t.TypeTransactionId
                                       }).ToList();


                return Json(new
                {
                    transacoesRelacionadasAoPerfil = transacaoRel.OrderBy(t=>t.Path).ToList(),
                    trasacoesNaoRelacionadasAoPerfil = transacaoNaoRel.OrderBy(t => t.Path).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        public JsonResult GravaTransacoesPorpefil(int profileId, string arrayTransactionPorPerfil)
        {
            try
            {
                List<int> listRelationshipUserProfileInstitutionModel = arrayTransactionPorPerfil != "" ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(arrayTransactionPorPerfil) : new List<int>();


                var _transacoesRelacionadasAoPerfilAtivas = (from tp in db.RelationshipTransactionProfiles
                                                             join t in db.Transactions on tp.TransactionId equals t.Id
                                                             where tp.ProfileId == profileId &&
                                                                   tp.Status == true

                                                             select t
                                                            ).AsQueryable();

                var _transacoesRelacionadasAoPerfilDesativadas = (from tp in db.RelationshipTransactionProfiles
                                                                  join t in db.Transactions on tp.TransactionId equals t.Id
                                                                  where tp.ProfileId == profileId &&
                                                                  tp.Status == false

                                                                  select t
                                                                  ).AsQueryable();


                var _trasacoesNaoRelacionadasAoPerfil = (from t in db.Transactions
                                                         where !_transacoesRelacionadasAoPerfilAtivas.Select(x => x.Id).Contains(t.Id) && !_transacoesRelacionadasAoPerfilDesativadas.Select(x => x.Id).Contains(t.Id)

                                                         select t
                                                        ).AsQueryable();


                var transacaoRelPerfilAtivas = _transacoesRelacionadasAoPerfilAtivas.ToList();
                var transacaoRelPerfilDesativadas = _transacoesRelacionadasAoPerfilDesativadas.ToList();
                var transacaoNaoRel = _trasacoesNaoRelacionadasAoPerfil.ToList();

                //Lista os ids das transações que devem ser desativadas no relacionameno com o perfil
                List<int> idsTransacoesParaDesativar = transacaoRelPerfilAtivas.Where(t => !listRelationshipUserProfileInstitutionModel.Contains(t.Id)).Select(x=>x.Id).ToList();

                //Lista os ids das transações que devem ser ativadas no relacionameno com o perfil
                List<int> idsTransacoesParaAtivar = transacaoRelPerfilDesativadas.Where(t => listRelationshipUserProfileInstitutionModel.Contains(t.Id)).Select(x => x.Id).ToList();

                //Lista os ids das transações que devem ser incluidas no relacionameno com o perfil
                List<int> idsTransacoesParaInclusao = transacaoNaoRel.Where(t => listRelationshipUserProfileInstitutionModel.Contains(t.Id)).Select(x => x.Id).ToList();


                if (idsTransacoesParaDesativar.Any())
                {
                    var relTransacaoPerfilParaDesativar = (from tp in db.RelationshipTransactionProfiles
                                                           where tp.ProfileId == profileId && idsTransacoesParaDesativar.Contains(tp.TransactionId)

                                                           select tp).ToList();

                    foreach (var item in relTransacaoPerfilParaDesativar)
                    {
                        item.Status = false;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }

                if (idsTransacoesParaAtivar.Any())
                {
                    var relTransacaoPerfilParaAtivar = (from tp in db.RelationshipTransactionProfiles
                                                           where tp.ProfileId == profileId && idsTransacoesParaAtivar.Contains(tp.TransactionId)

                                                           select tp).ToList();

                    foreach (var item in relTransacaoPerfilParaAtivar)
                    {
                        item.Status = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }                    

                if (idsTransacoesParaInclusao.Any())
                {
                    RelationshipTransactionProfile trasnsacaoPerfil = null;

                    foreach (var Id in idsTransacoesParaInclusao)
                    {
                        trasnsacaoPerfil = new RelationshipTransactionProfile();
                        trasnsacaoPerfil.TransactionId = Id;
                        trasnsacaoPerfil.ProfileId = profileId;
                        trasnsacaoPerfil.Status = true;

                        db.Entry(trasnsacaoPerfil).State = EntityState.Added;
                    }
                    db.SaveChanges();
                }

                return Json(new
                {
                    mensagem = "Relacionamentos gravados com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }


        [HttpGet]
        public ActionResult LoadDataTransactionsProfiles(int ProfileId)
        {
            try
            {
                var listTransactionsDB = (from a in db.RelationshipTransactionProfiles
                                          join b in db.Transactions on a.TransactionId equals b.Id
                                          where a.ProfileId == ProfileId
                                          select a);

                var listTransactions = new List<Transaction>();

                if (ContemValor(ProfileId))
                {
                    listTransactionsDB = (from a in listTransactionsDB
                                          where a.ProfileId == ProfileId
                                          select a
                                 );
                }

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        private bool ContemValor(string variavel)
        {
            bool retorno = false;
            if (variavel != string.Empty && variavel != null)
                retorno = true;
            return retorno;
        }

        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null)
                retorno = true;
            return retorno;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
