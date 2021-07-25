using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Models;
using System.Data.Entity;
using PagedList;
using SAM.Web.Common;
using SAM.Web.Context;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class DepreciationAccountController : BaseController
    {
        private SAMContext db;

        #region
        // GET: DepreciationAccount
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                using (db = new SAMContext())
                {

                    List<DepreciationAccount> lstRetorno;

                    if(string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchString))
                        lstRetorno = (from d in db.DepreciationAccounts select d).AsNoTracking().ToList();
                    else
                        lstRetorno = (from d in db.DepreciationAccounts
                                   where d.Description.Contains(searchString)
                                       || d.Code.ToString().Contains(searchString)
                                   select d).AsNoTracking().ToList();

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno
                                    .OrderBy(s => s.Code)
                                    .ThenBy(s => s.Description)
                                    .Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<DepreciationAccount>(result, pageNumber, pageSize, lstRetorno.Count());

                    return View(retorno);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Details
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    DepreciationAccount contaDepreciacao = db.DepreciationAccounts.Find(id);

                    if (contaDepreciacao == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(contaDepreciacao);
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
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Codigo,Descricao,Status,DescricaoProContabilizaSP")] DepreciationAccountViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        DepreciationAccount novaConta = new DepreciationAccount();

                        novaConta.Code = viewModel.Codigo;
                        novaConta.Description = viewModel.Descricao;
                        novaConta.Status = viewModel.Status;
                        novaConta.DescricaoContaDepreciacaoContabilizaSP = viewModel.DescricaoProContabilizaSP;

                        db.Entry(novaConta).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                return View(viewModel);
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
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    DepreciationAccountViewModel contaDepreciacao =
                        (from d in db.DepreciationAccounts
                         where d.Id == id
                         select new DepreciationAccountViewModel
                         {
                             Id = d.Id,
                             Codigo = d.Code,
                             Descricao = d.Description,
                             DescricaoProContabilizaSP = d.DescricaoContaDepreciacaoContabilizaSP,
                             Status = d.Status
                         }).AsNoTracking().FirstOrDefault();

                    if (contaDepreciacao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    contaDepreciacao.ItemMaterialRelacionado = db.DepreciationMaterialItems
                                                                 .Where(d => d.DepreciationAccount == contaDepreciacao.Codigo)
                                                                 .Select(d => d.MaterialItemCode)
                                                                 .FirstOrDefault();

                    return View(contaDepreciacao);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Codigo,Descricao,Status,DescricaoProContabilizaSP,ItemMaterialRelacionado")] DepreciationAccountViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        DepreciationAccount contaDepreciacao = db.DepreciationAccounts.Find(viewModel.Id);

                        contaDepreciacao.Code = viewModel.Codigo;
                        contaDepreciacao.Description = viewModel.Descricao;
                        contaDepreciacao.Status = viewModel.Status;
                        contaDepreciacao.DescricaoContaDepreciacaoContabilizaSP = viewModel.DescricaoProContabilizaSP;

                        db.Entry(contaDepreciacao).State = EntityState.Modified;

                        DepreciationMaterialItem relacaoContaDepreciacaoItemMaterial = db.DepreciationMaterialItems
                                                                                         .Where(d => d.DepreciationAccount == contaDepreciacao.Code)
                                                                                         .FirstOrDefault();

                        relacaoContaDepreciacaoItemMaterial.MaterialItemCode = viewModel.ItemMaterialRelacionado;
                        db.Entry(relacaoContaDepreciacaoItemMaterial).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(viewModel);
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
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    DepreciationAccount contaDepreciacao = db.DepreciationAccounts.Find(id);
                    
                    if (contaDepreciacao == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(contaDepreciacao);
                }
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
                    DepreciationAccount contaDepreciacao = db.DepreciationAccounts.Find(id);

                    contaDepreciacao.Status = false;
                    db.Entry(contaDepreciacao).State = EntityState.Modified;
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

    }
}