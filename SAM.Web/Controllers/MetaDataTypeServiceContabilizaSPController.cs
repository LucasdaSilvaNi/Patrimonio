using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using SAM.Web.Models;
using SAM.Web.Common;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class MetaDataTypeServiceContabilizaSPController : BaseController
    {
        private SAMContext contexto;

        public ActionResult Index()
        {
           if (PerfilAdmGeral())
               return View();
           else 
               return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult()
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();

            IQueryable<MetaDataTypeServiceContabilizaSPViewModel> lstRetorno = null;

            try
            {
                contexto = new SAMContext();
                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                {
                    lstRetorno = (from m in contexto.MetaDataTypesServicesContabilizaSPs
                                           where m.Name.Contains(currentFilter) || m.Id.ToString().Contains(currentFilter)
                                           select new MetaDataTypeServiceContabilizaSPViewModel {
                                               Codigo = m.Id,
                                               Nome = m.Name
                                           });
                }
                else
                {
                    lstRetorno = (from m in contexto.MetaDataTypesServicesContabilizaSPs
                                  select new MetaDataTypeServiceContabilizaSPViewModel
                                  {
                                      Codigo = m.Id,
                                      Nome = m.Name
                                  });
                }

                int totalRegistros = lstRetorno.Count();

                var result = await lstRetorno.OrderBy(r => r.Codigo).Skip(startRec).Take(length).ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = lstRetorno }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #region Edit
        public ActionResult Edit(int? id)
        {
            try
            {
                if (!PerfilAdmGeral())
                {
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
                
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (contexto = new SAMContext())
                {
                    MetaDataTypeServiceContabilizaSP metadataContabilizaSP = contexto.MetaDataTypesServicesContabilizaSPs
                                                                .FirstOrDefault(o => o.Id == id);

                    if (metadataContabilizaSP == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(metadataContabilizaSP);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Name")] MetaDataTypeServiceContabilizaSP metadataAlterado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    contexto = new SAMContext();
                    var comMesmoNome = (from m in contexto.MetaDataTypesServicesContabilizaSPs
                                        where m.Name.Equals(metadataAlterado.Name) && m.Id != metadataAlterado.Id
                                        select m).FirstOrDefault();


                    if (comMesmoNome == null)
                    {
                        contexto.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
                        contexto.Entry(metadataAlterado).State = EntityState.Modified;
                        contexto.SaveChanges();
                        contexto.Database.CurrentTransaction.Commit();
                        return RedirectToAction("Index");
                    }
                    else {
                        ModelState.AddModelError("Name", "Nome de metadado já cadastrado.");
                        return View(metadataAlterado);
                    }

                }

                return View(metadataAlterado);
            }
            catch (Exception ex)
            {
                if (contexto.Database.CurrentTransaction != null)
                    contexto.Database.CurrentTransaction.Rollback();

                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            try
            {
                if (!PerfilAdmGeral())
                {
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }

                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (contexto = new SAMContext())
                {
                    MetaDataTypeServiceContabilizaSP metadataContabilizaSP = contexto.MetaDataTypesServicesContabilizaSPs
                                                                    .FirstOrDefault(o => o.Id == id);

                    if (metadataContabilizaSP == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    ViewBag.LigadoAEventServices = contexto.EventServicesContabilizaSPs
                                                      .Where(e => e.MetaDataType_AccountingValueField == metadataContabilizaSP.Id ||
                                                                  e.MetaDataType_DateField == metadataContabilizaSP.Id ||
                                                                  e.MetaDataType_StockDestination == metadataContabilizaSP.Id ||
                                                                  e.MetaDataType_StockSource == metadataContabilizaSP.Id)
                                                      .Count();

                    return View(metadataContabilizaSP);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                if (!PerfilAdmGeral())
                {
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }

                using (contexto = new SAMContext())
                {
                    contexto.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

                    MetaDataTypeServiceContabilizaSP metadadoNoBanco = contexto.MetaDataTypesServicesContabilizaSPs.Find(id);
                    contexto.MetaDataTypesServicesContabilizaSPs.Remove(metadadoNoBanco);
                    contexto.SaveChanges();

                    contexto.Database.CurrentTransaction.Commit();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (contexto.Database.CurrentTransaction != null) {
                    contexto.Database.CurrentTransaction.Rollback();
                }

                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            try
            {
                if (!PerfilAdmGeral())
                {
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult Create(MetaDataTypeServiceContabilizaSP metadado)
        {
            try
            {
                if (!PerfilAdmGeral())
                {
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }

                if (ModelState.IsValid)
                {

                    using (contexto = new SAMContext())
                    {
                        MetaDataTypeServiceContabilizaSP metadadoComMesmoNome = (from m in contexto.MetaDataTypesServicesContabilizaSPs
                                                                                 where m.Name.Equals(metadado.Name)
                                                                                 select m).FirstOrDefault();

                        if (metadadoComMesmoNome == null)
                        {
                            contexto.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

                            contexto.Entry(metadado).State = EntityState.Added;
                            contexto.SaveChanges();

                            contexto.Database.CurrentTransaction.Commit();
                        }
                    }

                    return RedirectToAction("Index");
                }

                return View(metadado);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
    }
}