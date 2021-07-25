using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;

using SAM.Web.Common;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class ModulesController : BaseController
    {
        private ConfiguracaoContext db;

        #region Actions Methods

        #region Index Actions

        // GET: Modules
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CurrentFilter = searchString;

                IQueryable<ModuloTelaViewModel> lstRetornoViewModel;

                using (db = new ConfiguracaoContext())
                {
                    var lstRetorno = (from m in db.Modules select m);

                    lstRetorno = SearchByFilter(searchString == null ? searchString : searchString.Trim(), lstRetorno);
                    //lstRetorno = SortingByFilter(sortOrder, lstRetorno);

                    lstRetornoViewModel = (from m in lstRetorno
                                           select new ModuloTelaViewModel
                                           {
                                               Id = m.Id,
                                               IdMenu = m.ManagedSystemId,
                                               NomeMenu = m.RelatedManagedSystem.Name,
                                               IdModuloSuperior = m.ParentId,
                                               NomeModuloSuperior = m.RelatedModule.Name,
                                               Nome = m.Name,
                                               Caminho = m.Path,
                                               Sequencia = m.Sequence,
                                               Status = m.Status
                                           }).AsNoTracking();

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstRetornoViewModel
                                    .OrderBy(s => s.IdMenu)
                                    .ThenBy(s => s.IdModuloSuperior)
                                    .ThenBy(s => s.Sequencia)
                                    .Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<ModuloTelaViewModel>(result, pageNumber, pageSize, lstRetornoViewModel.Count());

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
        // GET: Modules/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);



                using (db = new ConfiguracaoContext())
                {
                    ModuloTelaViewModel module = (from m in db.Modules
                                                  where m.Id == (int)id
                                                  select new ModuloTelaViewModel
                                                  {
                                                      Id = m.Id,
                                                      NomeMenu = m.RelatedManagedSystem.Name,
                                                      NomeModuloSuperior = m.RelatedModule.Name,
                                                      Nome = m.Name,
                                                      Descricao = m.Description,
                                                      Caminho = m.Path,
                                                      Sequencia = m.Sequence,
                                                      Status = m.Status,
                                                  })
                                                  .FirstOrDefault();


                    if (module == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(module);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
        #region Create Actions
        // GET: Modules/Create
        public ActionResult Create()
        {
            try
            {
                CarregaViewsBags();
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Modulos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMenu,IdModuloSuperior,Nome,Descricao,Caminho,Sequencia,Status")] ModuloTelaViewModel moduloView)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    Module module = new Module();
                    module.ManagedSystemId = moduloView.IdMenu;
                    module.ParentId = moduloView.IdModuloSuperior;
                    module.Name = moduloView.Nome;
                    module.Description = moduloView.Descricao;
                    module.Path = moduloView.Caminho;
                    module.Sequence = moduloView.Sequencia;
                    module.Status = moduloView.Status;
                    using (db = new ConfiguracaoContext())
                    {
                        db.Modules.Add(module);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                CarregaViewsBags(moduloView);

                return View(moduloView);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Modules/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                ModuloTelaViewModel module;

                using (db = new ConfiguracaoContext())
                {
                    module = (from m in db.Modules where m.Id == (int)id
                              select new ModuloTelaViewModel {
                                  Id = m.Id,
                                  IdMenu = m.ManagedSystemId,
                                  IdModuloSuperior = m.ParentId,
                                  Nome = m.Name,
                                  Descricao = m.Description,
                                  Caminho = m.Path,
                                  Sequencia = m.Sequence,
                                  Status = m.Status
                              }).FirstOrDefault();
                }
                if (module == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                CarregaViewsBags(module);
                return View(module);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Modulos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdMenu,IdModuloSuperior,Nome,Descricao,Caminho,Sequencia,Status")] ModuloTelaViewModel moduloView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new ConfiguracaoContext())
                    {
                        Module module = db.Modules.Find(moduloView.Id);
                        module.ManagedSystemId = moduloView.IdMenu;
                        module.ParentId = moduloView.IdModuloSuperior;
                        module.Name = moduloView.Nome;
                        module.Description = moduloView.Descricao;
                        module.Path = moduloView.Caminho;
                        module.Sequence = moduloView.Sequencia;
                        module.Status = moduloView.Status;
                        db.Entry(module).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                CarregaViewsBags(moduloView);

                return View(moduloView);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: Modules/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new ConfiguracaoContext())
                {
                    ModuloTelaViewModel module = (from m in db.Modules
                                                  where m.Id == (int)id
                                                  select new ModuloTelaViewModel
                                                  {
                                                      Id = m.Id,
                                                      NomeMenu = m.RelatedManagedSystem.Name,
                                                      NomeModuloSuperior = m.RelatedModule.Name,
                                                      Nome = m.Name,
                                                      Descricao = m.Description,
                                                      Caminho = m.Path,
                                                      Sequencia = m.Sequence,
                                                      Status = m.Status,
                                                  })
                                                  .FirstOrDefault();
                    if (module == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(module);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Modulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new ConfiguracaoContext())
                {
                    Module module = db.Modules.Find(id);
                    module.Status = false;
                    db.Entry(module).State = EntityState.Modified;
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
        private IQueryable<Module> SearchByFilter(string searchString, IQueryable<Module> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.RelatedManagedSystem.Name.Contains(searchString)
                                       || s.RelatedModule.Name.Contains(searchString)
                                       || s.Name.Contains(searchString)
                                       || s.Path.Contains(searchString));
            return result;
        }

        /// <summary>
        /// Ordena os dados pelo parametro informado
        /// </summary>
        /// <param name="sortOrder">parametro de Ordenação</param>
        /// <param name="result">Modelo que será ordenado</param>
        private IQueryable<Module> SortingByFilter(string sortOrder, IQueryable<Module> result)
        {
            ViewBag.InitialSortParm = string.IsNullOrEmpty(sortOrder) ? "initials_desc" : "";
            ViewBag.DescriptionSortParm = sortOrder == "description" ? "description_desc" : "description";
            ViewBag.PathSortParm = sortOrder == "path" ? "path_desc" : "path";

            switch (sortOrder)
            {
                case "initials":
                    result = result.OrderBy(m => m.RelatedManagedSystem.Name);
                    break;
                case "initials_desc":
                    result = result.OrderByDescending(m => m.RelatedManagedSystem.Name);
                    break;
                case "description":
                    result = result.OrderBy(m => m.Name);
                    break;
                case "description_desc":
                    result = result.OrderByDescending(m => m.Name);
                    break;
                case "path":
                    result = result.OrderBy(m => m.Path);
                    break;
                case "path_desc":
                    result = result.OrderByDescending(m => m.Path);
                    break;
                default:
                    result = result.OrderBy(m => m.RelatedManagedSystem.Name);
                    break;
            }

            return result;
        }

        private void CarregaViewsBags(ModuloTelaViewModel modulo = null)
        {

            StructuresController structures = new StructuresController();
            if (modulo == null)
            {
                ViewBag.ManagedSystems = structures.GetSystems();
                ViewBag.Modules = structures.GetModules();
            }
            else
            {
                ViewBag.ManagedSystems = structures.GetSystems(modulo.IdMenu);
                ViewBag.Modules = structures.GetModules(modulo.IdModuloSuperior);
            }
        }

        #endregion
    }
}
