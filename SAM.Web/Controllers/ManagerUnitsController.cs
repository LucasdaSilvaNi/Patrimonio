using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Models;
using PagedList;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using System.Collections.Generic;
using SAM.Web.ViewModels;
using System.Threading.Tasks;

namespace SAM.Web.Controllers
{
    public class ManagerUnitsController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;

        public void getHierarquiaPerfil()
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

        #region Actions Methods
        #region Index Actions
        // GET: ManagerUnits
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                if (PerfilAdmGeral())
                {
                    ViewBag.PerfilPermissao = true;
                    ViewBag.PerfilOperadorUGE = false;
                    ViewBag.UGE = string.Empty;
                }
                else {
                    ViewBag.PerfilPermissao = false;
                    ViewBag.PerfilOperadorUGE = (int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE;

                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        ViewBag.UGE = _managerUnitId == null ?
                                      string.Empty :
                                      (from m in db.ManagerUnits where m.Id == _managerUnitId select m.Code).FirstOrDefault().ToString();
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(ManagerUnit managerUnit)
        {
            string draw = Request.Form["draw"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            IQueryable<ManagerUnitViewModel> lstRetorno;
            int totalRegistros = 0;
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
                        lstRetorno = (from s in db.ManagerUnits.Include("RelatedBudgetUnit")
                                      where s.Status == true
                                      select new ManagerUnitViewModel
                                      {
                                          Id = s.Id,
                                          Code = s.Code,
                                          Description = s.Description,
                                          InstitutionId = s.RelatedBudgetUnit.InstitutionId,
                                          BudgetUnitId = s.RelatedBudgetUnit.Id,
                                          BudgetUnitCode = s.RelatedBudgetUnit.Code,
                                          BudgetUnitDescription = s.RelatedBudgetUnit.Description,
                                          ManagmentUnit_YearMonthStart = s.ManagmentUnit_YearMonthStart,
                                          ManagmentUnit_YearMonthReference = s.ManagmentUnit_YearMonthReference,
                                          FlagSiafem = s.FlagIntegracaoSiafem ? "Sim" : "Não"
                                      })
                                 .Where(s => s.BudgetUnitCode.Contains(currentFilter)
                                        || s.BudgetUnitDescription.Contains(currentFilter)
                                        || s.Code.Contains(currentFilter)
                                        || s.Description.Contains(currentFilter))
                                        .AsNoTracking();
                    else
                        lstRetorno = (from s in db.ManagerUnits.Include("RelatedBudgetUnit")
                                      where s.Status == true
                                      select new ManagerUnitViewModel
                                      {
                                          Id = s.Id,
                                          Code = s.Code,
                                          Description = s.Description,
                                          InstitutionId = s.RelatedBudgetUnit.InstitutionId,
                                          BudgetUnitId = s.RelatedBudgetUnit.Id,
                                          BudgetUnitCode = s.RelatedBudgetUnit.Code,
                                          BudgetUnitDescription = s.RelatedBudgetUnit.Description,
                                          ManagmentUnit_YearMonthStart = s.ManagmentUnit_YearMonthStart,
                                          ManagmentUnit_YearMonthReference = s.ManagmentUnit_YearMonthReference,
                                          FlagSiafem = s.FlagIntegracaoSiafem ? "Sim" : "Não"
                                      }).AsNoTracking();

                    if (PerfilAdmGeral() != true)
                    {
                        if (ContemValor(_budgetUnitId))
                            lstRetorno = (from s in lstRetorno where s.BudgetUnitId == _budgetUnitId select s);
                        else
                            lstRetorno = (from s in lstRetorno where s.InstitutionId == _institutionId select s);
                    }

                    totalRegistros = await lstRetorno.CountAsync();

                    var result = lstRetorno.OrderBy(s => s.Id).Skip(startRec).Take(length);
                    var retorno = await Ordernar(result, Convert.ToInt16(order), orderDir).ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = retorno }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null && variavel != 0)
                retorno = true;
            return retorno;
        }

        #region Details Actions
        // GET: ManagerUnits/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    ManagerUnit managerUnit = db.ManagerUnits.Find(id);
                    if (managerUnit == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(managerUnit.RelatedBudgetUnit.InstitutionId, managerUnit.RelatedBudgetUnit.Id, managerUnit.Id, null, null))
                        return View(managerUnit);
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
        // GET: ManagerUnits/Create
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

        // POST: ManagerUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManagerUnitViewModel managerUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (managerUnit.ManagmentUnit_YearMonthReference.Length != 6) {
                        ModelState.AddModelError("DataInvalida", "Formato de data inválido");
                        SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                        return View(managerUnit);
                    }

                    using (db = new SAMContext())
                    {

                        if ((from b in db.ManagerUnits where b.Code == managerUnit.Code && b.Status == true select b).Any())
                        {
                            ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra UGE Ativa!");
                            SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                            return View(managerUnit);
                        }

                        if (int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4)) < 2000 || int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2)) > 12 || int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2)) <= 0)
                        {
                            ModelState.AddModelError("DataInvalida", "Data Invalida, favor verificar ano e mes informados!");
                            SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                            return View(managerUnit);
                        }

                        ManagerUnit _managerUnit = new ManagerUnit();
                        _managerUnit.BudgetUnitId = managerUnit.BudgetUnitId;
                        _managerUnit.Code = managerUnit.Code;
                        _managerUnit.Description = managerUnit.Description;
                        _managerUnit.Status = true;
                        _managerUnit.FlagIntegracaoSiafem = managerUnit.FlagIntegracaoSiafem;
                        _managerUnit.FlagTratarComoOrgao = managerUnit.FlagTratarComoOrgao;
                        _managerUnit.ManagmentUnit_YearMonthStart = managerUnit.ManagmentUnit_YearMonthReference;
                        _managerUnit.ManagmentUnit_YearMonthReference = managerUnit.ManagmentUnit_YearMonthReference;
                        db.ManagerUnits.Add(_managerUnit);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                return View(managerUnit);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions

        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    ManagerUnitViewModel managerUnit = db.ManagerUnits
                                                       .Where(m => m.Id == id)
                                                       .Select(m => new ManagerUnitViewModel()
                                                       {
                                                           Id = m.Id,
                                                           BudgetUnitId = m.BudgetUnitId,
                                                           Code = m.Code,
                                                           Description = m.Description,
                                                           FlagIntegracaoSiafem = m.FlagIntegracaoSiafem,
                                                           FlagTratarComoOrgao = m.FlagTratarComoOrgao,
                                                           ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference,
                                                           ManagmentUnit_YearMonthStart = m.ManagmentUnit_YearMonthStart
                                                       })
                                                       .FirstOrDefault();

                    if (managerUnit == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    managerUnit.InstitutionId = db.BudgetUnits.Where(b => b.Id == managerUnit.BudgetUnitId).FirstOrDefault().InstitutionId;

                    if (ValidarRequisicao(managerUnit.InstitutionId, managerUnit.BudgetUnitId, managerUnit.Id, null, null))
                    {
                        managerUnit.OrgaoSelecionado = managerUnit.Id;
                        managerUnit.UOSelecionado = managerUnit.BudgetUnitId;
                        managerUnit.CodigoSelecionado = managerUnit.Code;
                        managerUnit.DescricaoInicial = managerUnit.Description;

                        SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                        return View(managerUnit);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManagerUnitViewModel managerUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (managerUnit.ManagmentUnit_YearMonthReference.Length != 6)
                    {
                        ModelState.AddModelError("DataInvalida", "Formato de data inválido");
                        SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                        return View(managerUnit);
                    }

                    if (managerUnit.ManagmentUnit_YearMonthStart.Length != 6)
                    {
                        ModelState.AddModelError("DataInicialInvalida", "Formato de data inválido");
                        SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                        return View(managerUnit);
                    }

                    using (db = new SAMContext())
                    {

                        if (managerUnit.AlteracaoIntegracaoSiafem == false)
                        {
                            if ((from b in db.ManagerUnits where b.Code == managerUnit.Code && b.Id != managerUnit.Id && b.Status == true select b.Id).Any())
                            {
                                ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra UGE Ativa!");
                                SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                                return View(managerUnit);
                            }

                            if (int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4)) < 2000 || int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2)) > 12 || int.Parse(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2)) <= 0)
                            {
                                ModelState.AddModelError("DataInvalida", "Data Invalida, favor verificar ano e mes informados!");
                                SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                                return View(managerUnit);
                            }

                            if (int.Parse(managerUnit.ManagmentUnit_YearMonthStart.Substring(0, 4)) < 2000 || int.Parse(managerUnit.ManagmentUnit_YearMonthStart.Substring(4, 2)) > 12 || int.Parse(managerUnit.ManagmentUnit_YearMonthStart.Substring(4, 2)) <= 0)
                            {
                                ModelState.AddModelError("DataInicialInvalida", "Data Invalida, favor verificar ano e mes informados!");
                                SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                                return View(managerUnit);
                            }
                        }

                        ManagerUnit _managerUnit = db.ManagerUnits.Find(managerUnit.Id);
                        _managerUnit.BudgetUnitId = managerUnit.BudgetUnitId;
                        _managerUnit.Code = managerUnit.Code;
                        _managerUnit.Description = managerUnit.Description;
                        _managerUnit.Status = true;
                        _managerUnit.ManagmentUnit_YearMonthStart = managerUnit.ManagmentUnit_YearMonthStart;
                        _managerUnit.ManagmentUnit_YearMonthReference = managerUnit.ManagmentUnit_YearMonthReference;
                        _managerUnit.FlagIntegracaoSiafem = managerUnit.FlagIntegracaoSiafem;
                        _managerUnit.FlagTratarComoOrgao = managerUnit.FlagTratarComoOrgao;
                        db.Entry(_managerUnit).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(managerUnit.InstitutionId, managerUnit.BudgetUnitId);
                return View(managerUnit);
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

                using (db = new SAMContext())
                {

                    ManagerUnit managerUnit = db.ManagerUnits.Find(id);
                    if (managerUnit == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(managerUnit.RelatedBudgetUnit.InstitutionId, managerUnit.RelatedBudgetUnit.Id, managerUnit.Id, null, null))
                        return View(managerUnit);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }

            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: ManagerUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    if ((from b in db.AdministrativeUnits where b.ManagerUnitId == id && b.Status == true select b.Id).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    if ((from am in db.AssetMovements where am.ManagerUnitId == id && am.Status == true select am.Id).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    if ((from a in db.Assets where a.ManagerUnitId == id && a.Status == true select a.Id).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    ManagerUnit managerUnit = db.ManagerUnits.Find(id);
                    managerUnit.Status = false;
                    db.Entry(managerUnit).State = EntityState.Modified;
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

        private IQueryable<ManagerUnitViewModel> Ordernar(IQueryable<ManagerUnitViewModel> lst, int coluna, string direcao)
        {
            switch (coluna)
            {
                case 0: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.BudgetUnitCode) : lst.OrderByDescending(i => i.BudgetUnitCode); break;
                case 1: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.BudgetUnitDescription) : lst.OrderByDescending(i => i.BudgetUnitDescription); break;
                case 2: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.Code) : lst.OrderByDescending(i => i.Code); break;
                case 3: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.Description) : lst.OrderByDescending(i => i.Description); break;
                case 4: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.ManagmentUnit_YearMonthStart) : lst.OrderByDescending(i => i.ManagmentUnit_YearMonthStart); break;
                case 5: lst = direcao.Equals("asc") ? lst.OrderBy(i => i.ManagmentUnit_YearMonthReference) : lst.OrderByDescending(i => i.ManagmentUnit_YearMonthReference); break;
            }

            return lst;
        }

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0)
        {
            hierarquia = new Hierarquia();

            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");
            }
            else
            {
                getHierarquiaPerfil();
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);
                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description", modelBudgetUnitId);
                else if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);
            }
        }

        //public List<ItemGenericViewModel> RetornaListaMesAnoReferencia()
        //{
        //    var listaItens = new List<ItemGenericViewModel>();

        //    DateTime mesAnoInicial = new DateTime(2017, 06, 1);
        //    DateTime mesAnoFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);


        //    for (DateTime mesAnoRef = mesAnoInicial; mesAnoRef <= mesAnoFinal; mesAnoRef = mesAnoRef.AddMonths(1))
        //    {
        //        listaItens.Add(new ItemGenericViewModel()
        //        {
        //            Id = int.Parse(mesAnoInicial.Year.ToString().PadLeft(4, '0') + mesAnoInicial.Month.ToString().PadLeft(2, '0')),
        //            Description = mesAnoInicial.Month.ToString().PadLeft(2, '0') + "/" + mesAnoInicial.Year.ToString().PadLeft(4, '0')
        //        });
        //    }

        //    return listaItens.ToList().OrderByDescending(u => u.Id).ToList();
        //}


        #endregion

    }


}
