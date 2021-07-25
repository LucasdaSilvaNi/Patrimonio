using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Common;
using SAM.Web.Models;
using System.Data.Entity;
using SAM.Web.ViewModels;
using System.Threading.Tasks;

namespace SAM.Web.Controllers
{
    public class RelationshipAuxiliaryAccountItemGroupController : BaseController
    {
        private SAMContext db;

        #region ActionMethods
        // GET: RelationshipAuxiliaryAccountItemGroup
        #region Index
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;
                ViewBag.TemPermissao = PerfilAdmGeral();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJsonResult()
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string searchString = Request.Form["currentFilter"].ToString();

            try
            {
                IQueryable<RelacaoGrupomaterialContaContabilViewModel> lstRetorno;

                using (db = new SAMContext())
                {
                    if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrWhiteSpace(searchString))
                    {
                        lstRetorno = (from r in db.RelationshipAuxiliaryAccountItemGroups
                                      where r.RelatedAuxiliaryAccount.BookAccount.ToString().Contains(searchString) ||
                                            r.RelatedAuxiliaryAccount.Description.Contains(searchString) ||
                                            r.RelatedMaterialGroup.Code.ToString().Contains(searchString) ||
                                            r.RelatedMaterialGroup.Description.Contains(searchString)
                                      select new RelacaoGrupomaterialContaContabilViewModel
                                      {
                                          Id = r.Id,
                                          CodigoGrupoMaterial = r.RelatedMaterialGroup.Code,
                                          DescricaoGrupoMaterial = r.RelatedMaterialGroup.Description,
                                          ContaContabil = r.RelatedAuxiliaryAccount.BookAccount,
                                          DescricaoContaContabil = r.RelatedAuxiliaryAccount.Description
                                      }).OrderBy(r => r.DescricaoGrupoMaterial).AsNoTracking();
                    }
                    else
                    {
                        lstRetorno = (from r in db.RelationshipAuxiliaryAccountItemGroups
                                      select new RelacaoGrupomaterialContaContabilViewModel
                                      {
                                          Id = r.Id,
                                          CodigoGrupoMaterial = r.RelatedMaterialGroup.Code,
                                          DescricaoGrupoMaterial = r.RelatedMaterialGroup.Description,
                                          ContaContabil = r.RelatedAuxiliaryAccount.BookAccount,
                                          DescricaoContaContabil = r.RelatedAuxiliaryAccount.Description
                                      }).OrderBy(r => r.DescricaoGrupoMaterial).AsNoTracking();
                    }

                    int totalRegistros = await lstRetorno.CountAsync();

                    var resultado = await lstRetorno.Skip(startRec).Take(length).ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultado }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Details
        public ActionResult Details(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    var retorno = (from r in db.RelationshipAuxiliaryAccountItemGroups where r.Id == id select r).FirstOrDefault();

                    if (retorno == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    retorno.RelatedMaterialGroup.Description = retorno.RelatedMaterialGroup.Code.ToString() + " - " + retorno.RelatedMaterialGroup.Description;
                    retorno.RelatedAuxiliaryAccount.Description = retorno.RelatedAuxiliaryAccount.BookAccount.ToString() + " - " + retorno.RelatedAuxiliaryAccount.Description;

                    return View(retorno);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #region Create

        public ActionResult Create()
        {
            try
            {
                if (PerfilAdmGeral())
                {
                    using (db = new SAMContext())
                    {
                        CarregaCombos();
                    }
                    return View();
                }else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,MaterialGroupId,AuxiliaryAccountId")] RelationshipAuxiliaryAccountItemGroup relacao)
        {
            try
            {
                db = new SAMContext();
                var grupoMaterial = (from m in db.MaterialGroups where m.Id == relacao.MaterialGroupId select m).FirstOrDefault();
                if (grupoMaterial == null) {
                    ModelState.AddModelError("MaterialGroupId", "Grupo Material inválido.");
                    CarregaCombos();
                    return View(relacao);
                }

                var contaContabil = (from m in db.AuxiliaryAccounts where m.Id == relacao.AuxiliaryAccountId select m).FirstOrDefault();
                if (contaContabil == null)
                {
                    ModelState.AddModelError("AuxiliaryAccountId", "Conta Contábil inválida.");
                    CarregaCombos();
                    return View(relacao);
                }

                if (ModelState.IsValid)
                {

                    var temRelacaoNoBanco = ((from m in db.RelationshipAuxiliaryAccountItemGroups
                                              where m.MaterialGroupId == relacao.MaterialGroupId
                                              && m.AuxiliaryAccountId == relacao.AuxiliaryAccountId
                                              select m).FirstOrDefault() == null);

                    if (temRelacaoNoBanco)
                    {
                        db.RelationshipAuxiliaryAccountItemGroups.Add(relacao);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                CarregaCombos();
                return View(relacao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            try
            {
                if (PerfilAdmGeral())
                { 
                    if (id == null)
                        return MensagemErro(CommonMensagens.IdentificadorNulo);

                    using (db = new SAMContext())
                    {
                        RelationshipAuxiliaryAccountItemGroup relacao = (from r in db.RelationshipAuxiliaryAccountItemGroups where r.Id == id select r).FirstOrDefault();

                        if (relacao == null)
                            return MensagemErro(CommonMensagens.RegistroNaoExistente);

                        CarregaCombos(relacao);


                        return View(relacao);
                    }
                }else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MaterialGroupId,AuxiliaryAccountId")] RelationshipAuxiliaryAccountItemGroup relacao)
        {
            try
            {
                db = new SAMContext();
                var grupoMaterial = (from m in db.MaterialGroups where m.Id == relacao.MaterialGroupId select m).FirstOrDefault();
                if (grupoMaterial == null)
                {
                    ModelState.AddModelError("MaterialGroupId", "Grupo Material inválido.");
                    CarregaCombos();
                    return View(relacao);
                }

                var contaContabil = (from m in db.AuxiliaryAccounts where m.Id == relacao.AuxiliaryAccountId select m).FirstOrDefault();
                if (grupoMaterial == null)
                {
                    ModelState.AddModelError("AuxiliaryAccountId", "Conta Contábil inválida.");
                    CarregaCombos();
                    return View(relacao);
                }

                if (ModelState.IsValid)
                {
                    var temRelacaoNoBanco = ((from m in db.RelationshipAuxiliaryAccountItemGroups
                                              where m.Id != relacao.Id
                                              && m.MaterialGroupId == relacao.MaterialGroupId
                                              && m.AuxiliaryAccountId == relacao.AuxiliaryAccountId
                                              select m).FirstOrDefault() == null);

                    if (temRelacaoNoBanco)
                    {
                        db.Entry(relacao).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else {
                        ModelState.AddModelError("Id", "Relação já existente");
                        CarregaCombos(relacao);
                        return View(relacao);
                    }
                }

                CarregaCombos(relacao);
                return View(relacao);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            try
            {
                if (PerfilAdmGeral())
                {
                    if (id == null)
                        return MensagemErro(CommonMensagens.IdentificadorNulo);

                    using (db = new SAMContext())
                    {
                        RelationshipAuxiliaryAccountItemGroup relacao = db.RelationshipAuxiliaryAccountItemGroups.Find(id);
                        if (relacao == null)
                            return MensagemErro(CommonMensagens.RegistroNaoExistente);

                        relacao.RelatedMaterialGroup.Description = relacao.RelatedMaterialGroup.Code.ToString() + " - " + relacao.RelatedMaterialGroup.Description;
                        relacao.RelatedAuxiliaryAccount.Description = relacao.RelatedAuxiliaryAccount.BookAccount.ToString() + " - " + relacao.RelatedAuxiliaryAccount.Description;

                        return View(relacao);
                    }
                }else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
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
                using (db = new SAMContext())
                {
                    RelationshipAuxiliaryAccountItemGroup relacao = db.RelationshipAuxiliaryAccountItemGroups.Find(id);
                    db.RelationshipAuxiliaryAccountItemGroups.Remove(relacao);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #endregion


        #region ViewMethods
        private IQueryable<RelationshipAuxiliaryAccountItemGroup> SearchByFilter(string searchString, IQueryable<RelationshipAuxiliaryAccountItemGroup> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.RelatedAuxiliaryAccount.BookAccount.ToString().Contains(searchString) ||
                                           s.RelatedAuxiliaryAccount.Description.Contains(searchString)     ||
                                           s.RelatedMaterialGroup.Code.ToString().Contains(searchString)    ||
                                           s.RelatedMaterialGroup.Description.Contains(searchString));
            return result;
        }

        private void CarregaCombos(RelationshipAuxiliaryAccountItemGroup relacao = null){

            if (relacao == null)
            {
                var retornoGruposMateriais = (from m in db.MaterialGroups where m.Status == true select m).ToList();
                retornoGruposMateriais.ForEach(a => a.Description = a.Code.ToString() + " - " + a.Description);
                ViewBag.GruposMateriais = new SelectList(retornoGruposMateriais, "Id", "Description");

                var retornoContasContabeis = (from m in db.AuxiliaryAccounts where m.Status == true && m.RelacionadoBP == 0 select m).ToList();
                retornoContasContabeis.ForEach(a => a.Description = a.BookAccount.ToString() + " - " + a.Description);
                ViewBag.ContasContabeis = new SelectList(retornoContasContabeis, "Id", "Description");
            }
            else {
                var retornoGruposMateriais = (from m in db.MaterialGroups where m.Status == true select m).ToList();
                retornoGruposMateriais.ForEach(a => a.Description = a.Code.ToString() + " - " + a.Description);
                ViewBag.GruposMateriais = new SelectList(retornoGruposMateriais, "Id", "Description", relacao.Id);

                var retornoContasContabeis = (from m in db.AuxiliaryAccounts where m.Status == true && m.RelacionadoBP == 0 select m).ToList();
                retornoContasContabeis.ForEach(a => a.Description = a.BookAccount.ToString() + " - " + a.Description);
                ViewBag.ContasContabeis = new SelectList(retornoContasContabeis, "Id", "Description", relacao.Id);
            }
        }

        //private IQueryable<RelacaoGrupomaterialContaContabilViewModel> Ordenar(string ordenacao, string ordenacaoAscDesc, IQueryable<RelacaoGrupomaterialContaContabilViewModel> lstRelacao) {
        //    try
        //    {
        //        // Sorting
        //        switch (ordenacao)
        //        {
        //            case "0":
        //                lstRelacao = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRelacao.OrderByDescending(p => p.CodigoGrupoMaterial) : lstRelacao.OrderBy(p => p.CodigoGrupoMaterial);
        //                break;
        //            case "1":
        //                lstRelacao = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRelacao.OrderByDescending(p => p.DescricaoGrupoMaterial) : lstRelacao.OrderBy(p => p.DescricaoGrupoMaterial);
        //                break;
        //            case "2":
        //                lstRelacao = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRelacao.OrderByDescending(p => p.ContaContabil) : lstRelacao.OrderBy(p => p.ContaContabil);
        //                break;
        //            case "3":
        //                lstRelacao = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRelacao.OrderByDescending(p => p.DescricaoContaContabil) : lstRelacao.OrderBy(p => p.DescricaoContaContabil);
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex.InnerException);
        //    }

        //    return lstRelacao;
        //}
        #endregion
    }
}