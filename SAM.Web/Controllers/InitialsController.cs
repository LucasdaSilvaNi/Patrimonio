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
using System.Data.Entity.Validation;
using SAM.Web.ViewModels;
using System.Threading.Tasks;

namespace SAM.Web.Controllers
{
    public class InitialsController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;

        private int? _budgetUnitId;
        private int _institutionId;
        private int? _managerUnitId;

        #region Actions Methods


        private void getHierarquiaPerfil()
        {
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
            }
            else
            {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
            }
        }

        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null)
                retorno = true;
            return retorno;
        }

        #region Index Actions
        // GET: Initials
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
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
        public async Task<JsonResult> IndexJSONResult(Initial user)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;
            IQueryable<InitialViewModel> lstRetorno = null;

            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _budgetUnitId = IdsHieraquia[1];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                using (db = new SAMContext())
                {
                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                        lstRetorno = (from s in db.Initials.Include("RelatedInstitution").Include("RelatedBudgetUnit").Include("RelatedManagerUnit")
                                      where s.Status == true
                                      select new InitialViewModel()
                                      {
                                          Id = s.Id,
                                          Name = s.Name,
                                          InstitutionCode = s.RelatedInstitution.Code,
                                          InstitutionDescription = s.RelatedInstitution.Description,
                                          InstitutionId = s.InstitutionId,
                                          BudgetUnitId = s.RelatedBudgetUnit.Id,
                                          BudgetUnitCode = s.RelatedBudgetUnit.Code,
                                          BudgetUnitDescription = s.RelatedBudgetUnit.Description,
                                          ManagerUnitId = s.RelatedManagerUnit.Id,
                                          ManagerUnitCode = s.RelatedManagerUnit.Code,
                                          ManagerUnitDescription = s.RelatedManagerUnit.Description
                                      }).Where(s => s.InstitutionCode.Contains(currentFilter)
                                        || s.InstitutionDescription.Contains(currentFilter)
                                        || s.BudgetUnitCode.Contains(currentFilter)
                                        || s.BudgetUnitDescription.Contains(currentFilter)
                                        || s.Name.Contains(currentFilter)).AsNoTracking().OrderBy(s => s.Id);
                    else
                        lstRetorno = (from s in db.Initials.Include("RelatedInstitution").Include("RelatedBudgetUnit").Include("RelatedManagerUnit")
                                      where s.Status == true
                                      select new InitialViewModel()
                                      {
                                          Id = s.Id,
                                          Name = s.Name,
                                          InstitutionCode = s.RelatedInstitution.Code,
                                          InstitutionDescription = s.RelatedInstitution.Description,
                                          InstitutionId = s.InstitutionId,
                                          BudgetUnitId = s.RelatedBudgetUnit.Id,
                                          BudgetUnitCode = s.RelatedBudgetUnit.Code,
                                          BudgetUnitDescription = s.RelatedBudgetUnit.Description,
                                          ManagerUnitId = s.RelatedManagerUnit.Id,
                                          ManagerUnitCode = s.RelatedManagerUnit.Code,
                                          ManagerUnitDescription = s.RelatedManagerUnit.Description
                                      }).AsNoTracking().OrderBy(s => s.Id);

                    if (!PerfilAdmGeral())
                    {

                        if (_managerUnitId != null && _managerUnitId != 0)
                            lstRetorno = (from s in lstRetorno where s.ManagerUnitId == _managerUnitId select s);
                        else
                        if (_budgetUnitId != null && _budgetUnitId != 0)
                            lstRetorno = (from s in lstRetorno where s.BudgetUnitId == _budgetUnitId select s);
                        else
                            lstRetorno = (from s in lstRetorno where s.InstitutionId == _institutionId select s);
                    }

                    totalRegistros = await lstRetorno.CountAsync();

                    var resultado = lstRetorno.Skip(startRec).Take(length);
                    var resultadoOrdenado = await Ordernar(resultado, order == null ? 0 : Convert.ToInt32(order), orderDir).ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultadoOrdenado }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }

        }

        #endregion


        #region Details Actions
        // GET: Initials/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    InitialViewModel initial =
                                     db.Initials.
                                     Select(i => new InitialViewModel
                                     {
                                        Name = i.Name,
                                        Description = i.Description,
                                        InstitutionId = i.InstitutionId,
                                        BudgetUnitId = i.BudgetUnitId,
                                        ManagerUnitId = i.ManagerUnitId,
                                        InstitutionDescription = i.RelatedInstitution.Description,
                                        BudgetUnitDescription = i.RelatedBudgetUnit.Description,
                                        ManagerUnitDescription = i.RelatedManagerUnit.Description,
                                        Id = i.Id
                                     }).
                                     FirstOrDefault(i => i.Id == id);

                    if (initial == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(initial.InstitutionId, initial.BudgetUnitId, initial.ManagerUnitId, null, null))
                        return View(initial);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        
        #region Create Actions
        // GET: Initials/Create
        public ActionResult Create()
        {
            try
            {
                SetCarregaHierarquia();
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Initials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,InstitutionId,BudgetUnitId,ManagerUnitId")]Initial initial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        if (ContemValor(initial.ManagerUnitId) && initial.ManagerUnitId != 0)
                        {
                            initial.BudgetUnitId = initial.BudgetUnitId == 0 ? null : initial.BudgetUnitId;
                            initial.ManagerUnitId = initial.ManagerUnitId == 0 ? null : initial.ManagerUnitId;

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.InstitutionId == initial.InstitutionId && !i.BudgetUnitId.HasValue && !i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada há nivel de Orgão, favor informar outra Sigla!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.BudgetUnitId == initial.BudgetUnitId && !i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada há nivel de UO, favor informar outra Sigla!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.ManagerUnitId == initial.ManagerUnitId && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada na mesma UGE, favor informar outra Sigla!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }
                        }
                        else if (ContemValor(initial.BudgetUnitId) && initial.BudgetUnitId != 0)
                        {
                            initial.BudgetUnitId = initial.BudgetUnitId == 0 ? null : initial.BudgetUnitId;
                            initial.ManagerUnitId = initial.ManagerUnitId == 0 ? null : initial.ManagerUnitId;

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.InstitutionId == initial.InstitutionId && !i.BudgetUnitId.HasValue && !i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada há nivel de Orgão, favor informar outra Sigla!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.BudgetUnitId == initial.BudgetUnitId && !i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada na mesma UO, favor informar outra Sigla!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.BudgetUnitId == initial.BudgetUnitId && i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada no nivel de UGE, caso necessário exclua do Nivel de UGE e cadastre novamente a Nivel de UO!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }
                        }
                        else
                        {
                            initial.BudgetUnitId = initial.BudgetUnitId == 0 ? null : initial.BudgetUnitId;
                            initial.ManagerUnitId = initial.ManagerUnitId == 0 ? null : initial.ManagerUnitId;

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.InstitutionId == initial.InstitutionId && !i.BudgetUnitId.HasValue && !i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada no mesmo Orgão, favor informar outra Sigla!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.InstitutionId == initial.InstitutionId && i.BudgetUnitId.HasValue && !i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada no nivel de UO, caso necessário exclua do Nivel de UO e cadastre novamente a Nivel de Orgão!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }

                            if (db.Initials.Where(i => i.Name.Trim() == initial.Name.Trim() && i.InstitutionId == initial.InstitutionId && i.ManagerUnitId.HasValue && i.Status == true).Any())
                            {
                                ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada no nivel de UGE, caso necessário exclua do Nivel de UGE e cadastre novamente a Nivel de Orgão!");
                                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                                return View(initial);
                            }
                        }

                        initial.Status = true;
                        db.Initials.Add(initial);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                return View(initial);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        
        #region Edit Actions
        // GET: Initials/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    InitialViewModel initial = 
                                     db.Initials.
                                     Select(i => new InitialViewModel {
                                         Id = i.Id,
                                         BarCode = i.BarCode,
                                         InstitutionId = i.InstitutionId,
                                         BudgetUnitId = i.BudgetUnitId,
                                         ManagerUnitId = i.ManagerUnitId,
                                         Name = i.Name,
                                         Description = i.Description
                                     }).
                                     FirstOrDefault(i => i.Id == id);

                    if (initial == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(initial.InstitutionId, initial.BudgetUnitId, null, null, null))
                    {
                        SetCarregaHierarquia(initial.InstitutionId, initial.BudgetUnitId ?? 0, initial.ManagerUnitId ?? 0);
                        return View(initial);
                    }
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Initials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BarCode,Name,Description,InstitutionId,BudgetUnitId,ManagerUnitId")]InitialViewModel initialModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    initialModel.BudgetUnitId = initialModel.BudgetUnitId == 0 ? null : initialModel.BudgetUnitId;
                    initialModel.ManagerUnitId = initialModel.ManagerUnitId == 0 ? null : initialModel.ManagerUnitId;

                    using (db = new SAMContext())
                    {
                        if (db.Initials.Where(i => i.Id != initialModel.Id && i.Name.Trim() == initialModel.Name.Trim() && i.InstitutionId == initialModel.InstitutionId && i.BudgetUnitId == initialModel.BudgetUnitId && i.ManagerUnitId == initialModel.ManagerUnitId && i.Status == true).Any())
                        {
                            ModelState.AddModelError("SiglaJaExiste", "Sigla já está cadastrada, favor informar outra Sigla!");
                            SetCarregaHierarquia(initialModel.InstitutionId, initialModel.BudgetUnitId ?? 0, initialModel.ManagerUnitId ?? 0);
                            return View(initialModel);
                        }

                        Initial initial = db.Initials.Find(initialModel.Id);

                        initial.InstitutionId = initialModel.InstitutionId;
                        initial.BudgetUnitId = initialModel.BudgetUnitId;
                        initial.ManagerUnitId = initialModel.ManagerUnitId;
                        initial.Name = initialModel.Name;
                        initial.Description = initialModel.Description;
                        initial.Status = true;
                        db.Entry(initial).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(initialModel.InstitutionId, initialModel.BudgetUnitId ?? 0, initialModel.ManagerUnitId ?? 0);
                return View(initialModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        
        #region Delete Actions
        // GET: Initials/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    InitialViewModel initial =
                                     db.Initials.
                                     Select(i => new InitialViewModel
                                     {
                                         Name = i.Name,
                                         Description = i.Description,
                                         InstitutionId = i.InstitutionId,
                                         BudgetUnitId = i.BudgetUnitId,
                                         ManagerUnitId = i.ManagerUnitId,
                                         InstitutionDescription = i.RelatedInstitution.Description,
                                         BudgetUnitDescription = i.RelatedBudgetUnit.Description,
                                         ManagerUnitDescription = i.RelatedManagerUnit.Description,
                                         Id = i.Id
                                     }).
                                     FirstOrDefault(i => i.Id == id);

                    if (initial == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(initial.InstitutionId, initial.BudgetUnitId, initial.ManagerUnitId, null, null))
                    {
                        ViewBag.NaoPodeExcluir = string.Empty;
                        if ((from a in db.Assets where a.InitialId == initial.Id && a.Status == true select a.Id).Any())
                            ViewBag.NaoPodeExcluir = "Não é possível excluir essa sigla, pois existem BPs registrados com ela";

                        return View(initial);
                    }
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        
        // POST: Initials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    Initial initial = db.Initials.Find(id);

                    //Validação duplicada, para impedir alguma anormalidade
                    if ((from a in db.Assets where a.InitialId == initial.Id && a.Status == true select a.Id).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    initial.Status = false;
                    db.Entry(initial).State = EntityState.Modified;
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

        private IQueryable<InitialViewModel> Ordernar(IQueryable<InitialViewModel> lst, int coluna, string direcao)
        {
            switch (coluna)
            {
                case 0: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.InstitutionCode) : lst.OrderByDescending(i => i.InstitutionCode); break;
                case 1: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.InstitutionDescription) : lst.OrderByDescending(i => i.InstitutionDescription); break;
                case 2: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.BudgetUnitCode) : lst.OrderByDescending(i => i.BudgetUnitCode); break;
                case 3: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.BudgetUnitDescription) : lst.OrderByDescending(i => i.BudgetUnitDescription); break;
                case 4: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.ManagerUnitCode) : lst.OrderByDescending(i => i.ManagerUnitCode); break;
                case 5: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.ManagerUnitDescription) : lst.OrderByDescending(i => i.ManagerUnitDescription); break;
                case 6: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.Name) : lst.OrderByDescending(i => i.Name); break;
            }

            return lst;
        }

        #endregion     

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0)
        {
            hierarquia = new Hierarquia();
            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");
                if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");
            }
            else
            {
                getHierarquiaPerfil();
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);
                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);
                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId), "Id", "Description", modelManagerUnitId);
                else if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId), "Id", "Description", modelManagerUnitId);
            }
        }   
    }
}
