using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Models;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using PagedList;
using System;
using System.Threading.Tasks;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class OutSourcedsController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;

        private int _institutionId;
        private int? _budgetUnitId;

        public void getHierarquiaPerfil()
        {
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
            }
            else
            {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
            }
        }

        #region Index

        // GET: OutSourceds
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.Operador = ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE || 
                                    (int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUO);

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(OutSourced outsourced)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            IQueryable<OutSourced> lstRetorno = null;

            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _budgetUnitId = IdsHieraquia[1];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                db = new SAMContext();

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                {
                    lstRetorno = (from o in db.OutSourceds where o.Status == true select o)
                    .Where(o => o.RelatedInstitution.Code.Contains(currentFilter) ||
                           o.RelatedInstitution.Description.Contains(currentFilter) ||
                           o.RelatedBudgetUnit.Code.Contains(currentFilter) ||
                           o.RelatedBudgetUnit.Description.Contains(currentFilter) ||
                           o.Name.ToString().Contains(currentFilter) ||
                           o.CPFCNPJ.ToString().Contains(currentFilter))
                           .AsNoTracking();
                }

                else
                {
                    lstRetorno = (from o in db.OutSourceds where o.Status == true select o).AsNoTracking();
                }

                if (!PerfilAdmGeral())
                {
                    if (ContemValor(_budgetUnitId) && _budgetUnitId != 0)
                        lstRetorno = (from s in lstRetorno where s.BudgetUnitId == _budgetUnitId select s);
                    else if (ContemValor(_institutionId))
                        lstRetorno = (from s in lstRetorno where s.InstitutionId == _institutionId select s);
                }

                int totalRegistros = await lstRetorno.CountAsync();

                var result = await lstRetorno.OrderBy(r => r.Id).Skip(startRec).Take(length).ToListAsync();

                var outResult = result.ConvertAll(new Converter<OutSourced, OutSourcedViewModel>(OutSourcedViewModel.GetInstancia));

                var outSourceds = OutSourcedViewModel.Ordenar(order, orderDir, outResult);

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = outSourceds }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region Details

        // GET: OutSourceds/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    OutSourced outSourced = db.OutSourceds.Include("RelatedInstitution")
                        .Include("RelatedBudgetUnit")
                        .Include("RelatedAddress")
                        .FirstOrDefault(d => d.Id == id);

                    if (outSourced == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(outSourced.InstitutionId, outSourced.BudgetUnitId, null, null, null))
                        return View(outSourced);
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

        #region Create
        // GET: OutSourceds/Create
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

        // POST: OutSourceds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Name, CPFCNPJ, AddressId, Telephone, RelatedAddress, InstitutionId, BudgetUnitId")] OutSourced outSourced)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        if (db.OutSourceds.Where(i => i.CPFCNPJ == outSourced.CPFCNPJ && i.InstitutionId == outSourced.InstitutionId && i.BudgetUnitId == outSourced.BudgetUnitId && i.Status == true).Any())
                        {
                            ModelState.AddModelError("TerceiroJaExiste", "CPF/CNPJ Terceiro já está cadastrado!");
                            SetCarregaHierarquia(outSourced.InstitutionId, outSourced.BudgetUnitId);
                            return View(outSourced);
                        }

                        //Limpa caracteres do CEP para ficar somente os numeros
                        outSourced.RelatedAddress.PostalCode = outSourced.RelatedAddress.PostalCode.Replace("-", string.Empty);
                        outSourced.Status = true;
                        db.OutSourceds.Add(outSourced);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(outSourced.InstitutionId, outSourced.BudgetUnitId);
                return View(outSourced);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Edit

        // GET: OutSourceds/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    OutSourced outSourced = db.OutSourceds.Include("RelatedBudgetUnit")
                                            .Include("RelatedInstitution")
                                            .Include("RelatedAddress")
                                            .FirstOrDefault(o => o.Id == id);

                    if (outSourced == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(outSourced.InstitutionId, outSourced.BudgetUnitId, null, null, null))
                    {
                        SetCarregaHierarquia(outSourced.InstitutionId, outSourced.BudgetUnitId);
                        return View(outSourced);
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

        // POST: OutSourceds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Name, CPFCNPJ, AddressId, Telephone, RelatedAddress, InstitutionId, BudgetUnitId, Status")] OutSourced outSourced)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        OutSourced _outSourced = db.OutSourceds.Include("RelatedBudgetUnit")
                           .Include("RelatedInstitution")
                           .Include("RelatedAddress")
                           .FirstOrDefault(o => o.Id == outSourced.Id);
                        _outSourced.Telephone = outSourced.Telephone;
                        _outSourced.RelatedAddress.PostalCode = outSourced.RelatedAddress.PostalCode.Replace("-", string.Empty);
                        _outSourced.RelatedAddress.Street = outSourced.RelatedAddress.Street;
                        _outSourced.RelatedAddress.Number = outSourced.RelatedAddress.Number;
                        _outSourced.RelatedAddress.ComplementAddress = outSourced.RelatedAddress.ComplementAddress;
                        _outSourced.RelatedAddress.District = outSourced.RelatedAddress.District;
                        _outSourced.RelatedAddress.City = outSourced.RelatedAddress.City;
                        _outSourced.RelatedAddress.State = outSourced.RelatedAddress.State;
                        _outSourced.Status = outSourced.Status;

                        db.Entry(_outSourced).State = EntityState.Modified;
                        db.Entry(_outSourced.RelatedAddress).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                SetCarregaHierarquia(outSourced.InstitutionId, outSourced.BudgetUnitId);
                return View(outSourced);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Delete

        // GET: OutSourceds/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    OutSourced outSourced = db.OutSourceds.Include("RelatedInstitution")
                        .Include("RelatedBudgetUnit")
                        .Include("RelatedAddress")
                        .FirstOrDefault(d => d.Id == id);
                    if (outSourced == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(outSourced.InstitutionId, outSourced.BudgetUnitId, null, null, null))
                        return View(outSourced);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: OutSourceds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    if ((from am in db.Assets where am.OutSourcedId == id && am.Status == true select am).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    OutSourced outSourced = db.OutSourceds.Find(id);
                    outSourced.Status = false;
                    db.Entry(outSourced).State = EntityState.Modified;
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

        #region
        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null)
                retorno = true;
            return retorno;
        }

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int? modelBudgetUnitId = 0)
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

        #endregion
    }
}
