using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Context;
using SAM.Web.Models;
using PagedList;
using SAM.Web.Common;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAM.Web.Controllers
{
    public class InstitutionsController : BaseController
    {
        private SAMContext db;
        //private RelationshipUserProfile rup;

        //public void getHierarquiaPerfil()
        //{
        //    User u = UserCommon.CurrentUser();
        //    rup = UserCommon.CurrentRelationshipUsersProfile(u.Id);
        //    var perflLogado = BuscaHierarquiaPerfilLogado(rup.Id);
        //}

        #region Actions Methods
        #region Index Actions
        // GET: Institutions
        public ActionResult Index(string sortOrder, string searchString, int? page, string cbFiltro)
        {
            try
            {
                //Guarda o filtro
                ViewBag.CurrentFilter = searchString;

                CarregaComboFiltro(cbFiltro);
                ViewBag.CurrentFilterCbFiltro = cbFiltro;

                ViewBag.PerfilPermissao = PerfilAdmGeral();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(Institution assetEAsset)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            bool implantado = Request.Form["cbFiltro"] == null ? false : Convert.ToBoolean(Convert.ToInt32(Request.Form["cbFiltro"].ToString()));

            int totalRegistros = 0;

            try
            {
                IQueryable<InstitutionViewModel> lstRetorno;
                db = new SAMContext();

                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                if (string.IsNullOrEmpty(currentFilter) || string.IsNullOrWhiteSpace(currentFilter))
                    lstRetorno = (from s in db.Institutions
                                  where s.Status == true && s.flagImplantado == implantado
                                  select new InstitutionViewModel
                                  {
                                      Id = s.Id,
                                      Codigo = s.Code,
                                      Description = s.Description,
                                      DescricaoResumida = s.NameManagerReduced,
                                      CodigoGestao = s.ManagerCode,
                                      OrgaoImplantado = s.flagImplantado,
                                      IntegradoComSiafem = s.flagIntegracaoSiafem
                                  }).AsNoTracking();
                else
                    lstRetorno = (from s in db.Institutions
                                  where s.Status == true &&
                                        s.flagImplantado == implantado &&
                                        (s.Code.Contains(currentFilter) ||
                                        s.Description.Contains(currentFilter) ||
                                        s.ManagerCode.Contains(currentFilter))
                                  select new InstitutionViewModel
                                  {
                                      Id = s.Id,
                                      Codigo = s.Code,
                                      Description = s.Description,
                                      DescricaoResumida = s.NameManagerReduced,
                                      CodigoGestao = s.ManagerCode,
                                      OrgaoImplantado = s.flagImplantado,
                                      IntegradoComSiafem = s.flagIntegracaoSiafem
                                  }).AsNoTracking();

                totalRegistros = await lstRetorno.CountAsync();

                lstRetorno = lstRetorno.OrderBy(u => u.Codigo).Skip(startRec).Take(length);

                switch (order)
                {
                    case "1":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Description) : lstRetorno.OrderBy(p => p.Description);
                        break;
                    case "2":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.DescricaoResumida) : lstRetorno.OrderBy(p => p.DescricaoResumida);
                        break;
                    case "3":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.CodigoGestao) : lstRetorno.OrderBy(p => p.CodigoGestao);
                        break;
                    default:
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Codigo) : lstRetorno.OrderBy(p => p.Codigo);
                        break;
                }


                var result = await lstRetorno.ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }


        }
        #endregion
        #region Details Actions
        // GET: Institutions/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);


                using (db = new SAMContext())
                {
                    Institution institution = db.Institutions.Find(id);

                    if (institution == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(institution.Id, null, null, null, null))
                        return View(institution);
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

        // GET: Institutions/Create
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

        // POST: Institutions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,ManagerCode,NameManagerReduced,flagImplantado,flagIntegracaoSiafem")] Institution institution)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db = new SAMContext();
                    if ((from b in db.Institutions where b.Code == institution.Code && b.Status == true select b).Any())
                    {
                        ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outro Orgao Ativo!");
                        return View(institution);
                    }

                    institution.Status = true;
                    db.Institutions.Add(institution);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(institution);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: Institutions/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {

                    Institution institution = db.Institutions.Find(id);

                    if (institution == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(institution.Id, null, null, null, null))
                        return View(institution);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Institutions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,ManagerCode,NameManagerReduced,flagImplantado,flagIntegracaoSiafem")] Institution institution)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db = new SAMContext();

                    if ((from b in db.Institutions where b.Code == institution.Code && b.Id != institution.Id && b.Status == true select b).Any())
                    {
                        ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outro Orgao Ativo!");
                        return View(institution);
                    }

                    institution.Status = true;
                    db.Entry(institution).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(institution);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: Institutions/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {

                    Institution institution = db.Institutions.Find(id);

                    if (institution == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(institution.Id, null, null, null, null))
                        return View(institution);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Institutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db = new SAMContext();

                if ((from b in db.BudgetUnits where b.InstitutionId == id && b.Status == true select b).AsNoTracking().Any())
                    return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                if ((from am in db.AssetMovements where am.InstitutionId == id && am.Status == true select am).AsNoTracking().Any())
                    return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                if ((from i in db.Initials where i.InstitutionId == id && i.Status == true select i).AsNoTracking().Any())
                    return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                if ((from o in db.OutSourceds where o.InstitutionId == id && o.Status == true select o).AsNoTracking().Any())
                    return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                Institution institution = db.Institutions.Find(id);
                institution.Status = false;
                db.Entry(institution).State = EntityState.Modified;
                db.SaveChanges();
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

        private void CarregaComboFiltro(string codFiltro)
        {
            var lista = new List<ItemGenericViewModel>();

            var itemGeneric = new ItemGenericViewModel
            {
                Id = 0,
                Description = "Não Implantados",
                Ordem = 10
            };

            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel
            {
                Id = 1,
                Description = "Implantados",
                Ordem = 5
            };

            lista.Add(itemGeneric);

            if (codFiltro != null && codFiltro != "")
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description", int.Parse(codFiltro));
            else
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");
        }

        #endregion

        public JsonResult CarregaComboPeriodos(int orgaoId, int orgaoLogadoId)
        {
            try
            {
                List<SelectListItem> data = new List<SelectListItem>();

                if (orgaoId == 0)
                    return Json(new SelectList(data, "Value", "Text"), JsonRequestBehavior.AllowGet);

                if (!PerfilAdmGeral() && orgaoId != orgaoLogadoId)
                    orgaoId = orgaoLogadoId;

                string primeiroMesReferencia;
                string ultimoMesReferencia;

                using (var contextoHierarquia = new HierarquiaContext())
                {
                    var listaMesInicial = (from m in contextoHierarquia.ManagerUnits where m.RelatedBudgetUnit.InstitutionId == orgaoId select m.ManagmentUnit_YearMonthStart).AsNoTracking().ToList();
                    primeiroMesReferencia = listaMesInicial.Select(x => Convert.ToInt32(x)).Min().ToString();
                    var listaMesReferencia = (from m in contextoHierarquia.ManagerUnits where m.RelatedBudgetUnit.InstitutionId == orgaoId select m.ManagmentUnit_YearMonthReference).AsNoTracking().ToList();
                    ultimoMesReferencia = listaMesReferencia.Select(x => Convert.ToInt32(x)).Max().ToString();
                }

                string anoIni = primeiroMesReferencia.Substring(0, 4);
                string mesIni = primeiroMesReferencia.Substring(4, 2);

                string anoRefUGE = ultimoMesReferencia.Substring(0, 4);
                string mesRefUGE = ultimoMesReferencia.Substring(4, 2);

                var DataInicialUGE = new DateTime(int.Parse(anoIni), int.Parse(mesIni), 1);
                var DataRefUGE = new DateTime(int.Parse(anoRefUGE), int.Parse(mesRefUGE), 1);


                string AnoMesId = primeiroMesReferencia;
                string AnoMes = string.Concat(mesIni.PadLeft(2, '0'), "/", anoIni);

                data.Add(new SelectListItem { Value = "0", Text = "Selecione" });

                while (DataInicialUGE < DataRefUGE)
                {
                    AnoMesId = string.Concat(DataInicialUGE.Year.ToString(), DataInicialUGE.Month.ToString().PadLeft(2, '0'));
                    AnoMes = string.Concat(DataInicialUGE.Month.ToString().PadLeft(2, '0'), "/", DataInicialUGE.Year.ToString());
                    data.Add(new SelectListItem { Value = AnoMesId, Text = AnoMes });

                    DataInicialUGE = DataInicialUGE.AddDays(36);
                    DataInicialUGE = new DateTime(DataInicialUGE.Year, DataInicialUGE.Month, 1);
                }

                return Json(new SelectList(data, "Value", "Text", "0"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}