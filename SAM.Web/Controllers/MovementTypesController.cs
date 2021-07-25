using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Models;
using PagedList;
using SAM.Web.Common;

namespace SAM.Web.Controllers
{
    public class MovementTypesController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: TiposMovimento
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var lstRetorno = (from m in db.MovementTypes select m);
                //Filter
                lstRetorno = SearchByFilter(searchString, lstRetorno);
                //Pagination
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                ViewBag.CurrentFilter = searchString;

                var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                var retorno = new StaticPagedList<MovementType>(result, pageNumber, pageSize, lstRetorno.Count());

                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: TiposMovimento/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                    if (id == null)
                        return MensagemErro(CommonMensagens.IdentificadorNulo);

                    MovementType tipoMovimento = db.MovementTypes.Find(id);
                    if (tipoMovimento == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(tipoMovimento);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: TiposMovimento/Create
        public ActionResult Create()
        {
            try
            {
                if (PerfilAdmGeral())
                {
                    carregaCombos();
                    return View();
                }else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: TiposMovimento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,GroupMovimentId,Status")] MovementType tipoMovimento)
        {
            try
            {
                var tipoDeMovimento = (from m in db.MovementTypes where m.Code == tipoMovimento.Code select m).FirstOrDefault();

                if (tipoDeMovimento != null) {
                    ModelState.AddModelError("Code", "Código já existente");
                    carregaCombos();
                    return View(tipoMovimento);
                }

                var grupoMovimento = (from g in db.GroupMoviments where g.Id == tipoMovimento.GroupMovimentId select g).FirstOrDefault();

                if (grupoMovimento == null)
                {
                    ModelState.AddModelError("GroupMovimentId", "Grupo de Movimento não encontrado no sistema");
                    carregaCombos();
                    return View(tipoMovimento);
                }

                if (ModelState.IsValid)
                {
                    db.MovementTypes.Add(tipoMovimento);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                carregaCombos();
                return View(tipoMovimento);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: TiposMovimento/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (PerfilAdmGeral())
                {
                    if (id == null)
                        return MensagemErro(CommonMensagens.IdentificadorNulo);

                    MovementType tipoMovimento = db.MovementTypes.Find(id);
                    if (tipoMovimento == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    carregaCombos();

                    return View(tipoMovimento);
                }else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: TiposMovimento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,GroupMovimentId,Status")] MovementType tipoMovimento)
        {
            try
            {
                var tipoDeMovimentoExistente = (from m in db.MovementTypes where m.Id == tipoMovimento.Id select m).FirstOrDefault();

                if (tipoDeMovimentoExistente.Code != tipoMovimento.Code) {
                    var tipoDeMovimentoCodigoExistente = (from m in db.MovementTypes where m.Code == tipoMovimento.Code select m).FirstOrDefault();

                    if (tipoDeMovimentoCodigoExistente != null)
                    {
                        ModelState.AddModelError("Code", "Código já existente");
                        carregaCombos();
                        return View(tipoMovimento);
                    }
                }

                var grupoMovimento = (from g in db.GroupMoviments where g.Id == tipoMovimento.GroupMovimentId select g).FirstOrDefault();

                if (grupoMovimento == null)
                {
                    ModelState.AddModelError("GroupMovimentId", "Grupo de Movimento não encontrado no sistema");
                    carregaCombos();
                    return View(tipoMovimento);
                }

                if (ModelState.IsValid)
                {
                    tipoDeMovimentoExistente.Code = tipoMovimento.Code;
                    tipoDeMovimentoExistente.Description = tipoMovimento.Description;
                    tipoDeMovimentoExistente.GroupMovimentId = tipoMovimento.GroupMovimentId;
                    tipoDeMovimentoExistente.Status = tipoMovimento.Status;

                    db.Entry(tipoDeMovimentoExistente).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                carregaCombos();
                return View(tipoMovimento);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: TiposMovimento/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (PerfilAdmGeral())
                {
                    if (id == null)
                        return MensagemErro(CommonMensagens.IdentificadorNulo);

                    MovementType tipoMovimento = db.MovementTypes.Find(id);
                    if (tipoMovimento == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(tipoMovimento);
                }else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: TiposMovimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MovementType tipoMovimento = db.MovementTypes.Find(id);
                db.MovementTypes.Remove(tipoMovimento);
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


        #region Métodos privados
        /// <summary>
        /// Filtra a pesquisa
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IQueryable<MovementType> SearchByFilter(string searchString, IQueryable<MovementType> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Code.ToString().Contains(searchString) || s.Description.Contains(searchString) || s.RelatedGroupMoviment.Description.Contains(searchString));

            return result;
        }

        private void carregaCombos() {
            ViewBag.GroupMoviments = new SelectList(db.GroupMoviments.OrderBy(m => m.Description), "Id", "Description");
        }
        #endregion
    }
}
