using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using PagedList;
using SAM.Web.Common;

namespace SAM.Web.Controllers
{
    public class TransactionsController : BaseController
    {
        private ConfiguracaoContext db;

        #region Actions Methods
        #region Index Actions
        // GET: Transactions
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                //Guarda o filtro
                ViewBag.CurrentFilter = searchString;

                IQueryable<TransactionTelaViewModel> lstRetorno;
                using (db = new ConfiguracaoContext())
                {
                    lstRetorno = (from m in db.Transactions
                                  select new TransactionTelaViewModel
                                  {
                                      Id = m.Id,
                                      IdModulo = m.ModuleId,
                                      NomeModulo = m.RelatedModule.Name,
                                      Sigla = m.Initial,
                                      Descricao = m.Description,
                                      Caminho = m.Path,
                                      DescricaoTipoTransacao = m.RelatedTypeTransaction.Description,
                                      Status = m.Status
                                  }).AsNoTracking();


                    lstRetorno = SearchByFilter(searchString, lstRetorno);

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var retornoOrdenado = lstRetorno
                                            .OrderBy(s => s.IdModulo)
                                            .ThenBy(s => s.Sigla)
                                            .Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<TransactionTelaViewModel>(retornoOrdenado, pageNumber, pageSize, lstRetorno.Count());

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
        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new ConfiguracaoContext())
                {
                    TransactionTelaViewModel transaction = (from t in db.Transactions
                                                            where t.Id == (int)id
                                                            select new TransactionTelaViewModel
                                                            {
                                                                Id = t.Id,
                                                                NomeModulo = t.RelatedModule.Name,
                                                                DescricaoTipoTransacao = t.RelatedTypeTransaction.Description,
                                                                Sigla = t.Initial,
                                                                Descricao = t.Description,
                                                                Caminho = t.Path,
                                                                Status = t.Status
                                                            }).FirstOrDefault();
                    if (transaction == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(transaction);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Transactions/Create
        public ActionResult Create()
        {
            try
            {
                CarregaViewBags();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Transacoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionTelaViewModel transactionModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        Transaction transaction = new Transaction();
                        transaction.ModuleId = transactionModel.IdModulo;
                        transaction.TypeTransactionId = transactionModel.IdTipoTransacao;
                        transaction.Initial = transactionModel.Sigla == null ? string.Empty : transactionModel.Sigla;
                        transaction.Description = transactionModel.Descricao;
                        transaction.Path = transactionModel.Caminho;
                        transaction.Status = transactionModel.Status;
                        db.Transactions.Add(transaction);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                CarregaViewBags(transactionModel);

                return View(transactionModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                TransactionTelaViewModel transaction;
                using (db = new ConfiguracaoContext()) {
                     transaction = (from t in db.Transactions
                                     where t.Id == (int)id
                                     select new TransactionTelaViewModel
                                     {
                                         Id = t.Id,
                                         IdModulo = t.ModuleId,
                                         IdTipoTransacao = t.TypeTransactionId,
                                         Sigla = t.Initial,
                                         Descricao = t.Description,
                                         Caminho = t.Path,
                                         Status = t.Status
                                     }).FirstOrDefault();
                }

                if (transaction == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                CarregaViewBags(transaction);

                return View(transaction);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Transacoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransactionTelaViewModel transactionModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        Transaction transaction = db.Transactions.Find(transactionModel.Id);
                        transaction.ModuleId = transactionModel.IdModulo;
                        transaction.TypeTransactionId = transactionModel.IdTipoTransacao;
                        transaction.Initial = transactionModel.Sigla;
                        transaction.Description = transactionModel.Descricao;
                        transaction.Path = transactionModel.Caminho;
                        transaction.Status = transactionModel.Status;
                        db.Entry(transaction).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                CarregaViewBags(transactionModel);

                return View(transactionModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                TransactionTelaViewModel transaction;

                using (db = new ConfiguracaoContext())
                {
                    transaction = (from t in db.Transactions
                                   where t.Id == (int)id
                                   select new TransactionTelaViewModel
                                   {
                                       Id = t.Id,
                                       NomeModulo = t.RelatedModule.Name,
                                       DescricaoTipoTransacao = t.RelatedTypeTransaction.Description,
                                       Sigla = t.Initial,
                                       Descricao = t.Description,
                                       Caminho = t.Path,
                                       Status = t.Status
                                   }).FirstOrDefault();
                }

                if (transaction == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(transaction);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Transacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new ConfiguracaoContext())
                {
                    Transaction transaction = db.Transactions.Find(id);
                    transaction.Status = false;
                    db.Entry(transaction).State = EntityState.Modified;
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

        /// <summary>
        /// Filtra os dados pela expressão informada
        /// </summary>
        /// <param name="searchString">Expressão informada</param>
        /// <param name="result">Modelo que será filtrado</param>
        private IQueryable<TransactionTelaViewModel> SearchByFilter(string searchString, IQueryable<TransactionTelaViewModel> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.NomeModulo.Contains(searchString)
                                       || s.Sigla.Contains(searchString)
                                       || s.Descricao.Contains(searchString)
                                       || s.Caminho.Contains(searchString)
                                       || s.DescricaoTipoTransacao.Contains(searchString));
            return result;
        }

        private void CarregaViewBags(TransactionTelaViewModel transaction = null) {
            StructuresController structures = new StructuresController();
            if (transaction != null)
            {
                ViewBag.Modules = structures.GetModules(transaction.IdModulo);
                ViewBag.TypeTransactions = structures.GetTypeTransaction(transaction.Id);
            }
            else {
                ViewBag.Modules = structures.GetModules();
                ViewBag.TypeTransactions = structures.GetTypeTransaction();
            }
        }

        #endregion
    }
}
