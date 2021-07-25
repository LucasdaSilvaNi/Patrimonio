using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Models;
using SAM.Web.Common;
using PagedList;
using System.Threading.Tasks;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
   
    public class SiafemCalendarController : BaseController
    {
        private SAMContext db;
        //private RelationshipUserProfile rup;
        private int _institutionId;

        public void getHierarquiaPerfil()
        {
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                _institutionId = perflLogado.InstitutionId;
            }
            else
            {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
            }
        }

        #region Actions
        #region Index Actions

        public async Task<ActionResult> Index()
        {
            ViewBag.TemPermissao = PerfilAdmGeral();
            return await Task.Run(() => View());
        }

        
        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(SiafemCalendar calendar)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();

            IQueryable<SiafemCalendar> lstRetorno;

            db = new SAMContext();

            try
            {
                //db.SetTransacao(System.Data.IsolationLevel.ReadUncommitted);
                int pageSize = (startRec == 0 ? 1 : startRec) + length;

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                {
                    var mesAno = currentFilter.Split('/');
                    var reverse = reverseString(currentFilter);

                    DateTime _data = new DateTime();

                    var data = DateTime.TryParse(currentFilter, out _data);
                    if(data == false)
                    {
                        lstRetorno = (from m in db.SiafemCalendars
                                      where m.Status == true &&
                                      m.FiscalYear.Value.ToString().Contains(currentFilter.Replace("/", ""))
                                      || m.ReferenceMonth.ToString().Contains(currentFilter.Replace("/", ""))
                                      || m.ReferenceMonth.ToString().Contains(reverse.Replace("/", ""))
                                      || m.DateClosing.Value.Day.ToString().Contains(currentFilter)

                                      select m).OrderBy(m => m.FiscalYear).AsNoTracking();
                    }else
                    {
                        lstRetorno = (from m in db.SiafemCalendars
                                      where m.Status == true &&
                                      m.FiscalYear.Value.ToString().Contains(currentFilter.Replace("/", ""))
                                      || m.ReferenceMonth.ToString().Contains(currentFilter.Replace("/", ""))
                                      || m.ReferenceMonth.ToString().Contains(reverse.Replace("/", ""))
                                      || m.DateClosing == _data

                                      select m).OrderBy(m => m.FiscalYear).AsNoTracking();
                    }
                }
                else
                {
                    lstRetorno = (from m in db.SiafemCalendars where m.Status == true select m).OrderBy(m => m.FiscalYear).AsNoTracking();
                }

                    int totalRegistros = lstRetorno.Count();

                    var result = await lstRetorno.Skip(startRec).Take(length).ToListAsync();

                    //db.Commit();
                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);
          
            }
            catch (Exception e)
            {
                //db.Rollback();
                return Json(MensagemErro(CommonMensagens.PadraoException, e), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Details Actions
        // GET: AuxiliaryAccount/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    SiafemCalendar siafemCalendarComesFromScreen = db.SiafemCalendars.Find(id);
                    if (siafemCalendarComesFromScreen == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);


                    getHierarquiaPerfil();
                    //string[] referenceMonth = Convert.ToInt32(siafemCalendarComesFromScreen.ReferenceMonth[1].Split('/') + siafemCalendarComesFromScreen.ReferenceMonth[0].Split('/'));
                    var model = new SiafemCalendarViewModel()
                    {

                        Id = siafemCalendarComesFromScreen.Id,
                        FiscalYear = siafemCalendarComesFromScreen.FiscalYear,
                        InstitutionId = _institutionId,
                        ReferenceMonth = string.Format("{0}/{1}", siafemCalendarComesFromScreen.ReferenceMonth.ToString().Substring(4, 2), siafemCalendarComesFromScreen.ReferenceMonth.ToString().Substring(0, 4)),
                        DateClosing = siafemCalendarComesFromScreen.DateClosing,
                        Month = siafemCalendarComesFromScreen.ReferenceMonth,
                        Status = siafemCalendarComesFromScreen.Status
                    };

                    //var teste = model.DateClosing;


                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
        #region Create Actions
        
        // GET: MonthlyClosing/Create
        public ActionResult Create()
        {
            try
            {
                getHierarquiaPerfil();
                SiafemCalendarViewModel model = new SiafemCalendarViewModel();
                model.InstitutionId = _institutionId;
                return View(model);
            }

            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MonthlyClosing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FiscalYear,ReferenceMonth,DateClosing,DataParaOperadores")] SiafemCalendarViewModel siafemCalendarComesFromScreen)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var referenceMonth = Convert.ToInt32(siafemCalendarComesFromScreen.ReferenceMonth.Split('/')[1] + siafemCalendarComesFromScreen.ReferenceMonth.Split('/')[0]);

                    using (db = new SAMContext())
                    {
                        if (db.SiafemCalendars.Where(h => h.ReferenceMonth == referenceMonth && h.Status == true).Any())
                        {
                            ModelState.AddModelError("ReferenceMonth", "Mês Escolhido já está cadastrado!");

                            return View(siafemCalendarComesFromScreen);
                        }


                        SiafemCalendar siafemCalendar = new SiafemCalendar();
                        siafemCalendar.FiscalYear = siafemCalendarComesFromScreen.FiscalYear;
                        siafemCalendar.ReferenceMonth = referenceMonth;
                        siafemCalendar.DateClosing = siafemCalendarComesFromScreen.DateClosing;
                        siafemCalendar.DataParaOperadores = siafemCalendarComesFromScreen.DataParaOperadores;

                        siafemCalendar.Status = true;
                        db.SiafemCalendars.Add(siafemCalendar);
                        db.SaveChanges();
                    }
                    
                    return RedirectToAction("Index");
                }

                //getHierarquiaPerfil();
                //MonthlyClosingViewModel model = new MonthlyClosingViewModel();
                //model.FiscalYear = siafemCalendarComesFromScreen.FiscalYear;
                //model.ReferenceMonth = siafemCalendarComesFromScreen.ReferenceMonth;
                //model.DateClosing = siafemCalendarComesFromScreen.DateClosing;
                //model.ManagerUnitId = _managerUnitId.Value;

                return View(siafemCalendarComesFromScreen);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }


        #endregion
        #region Edit Actions
        // GET: MonthlyClosing/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    SiafemCalendar siafemCalendarComesFromScreen = db.SiafemCalendars.Find(id);
                    if (siafemCalendarComesFromScreen == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);


                    getHierarquiaPerfil();
                    var model = new SiafemCalendarViewModel()
                    {

                        Id = siafemCalendarComesFromScreen.Id,
                        FiscalYear = siafemCalendarComesFromScreen.FiscalYear,
                        InstitutionId = _institutionId,
                        ReferenceMonth = string.Format("{0}/{1}", siafemCalendarComesFromScreen.ReferenceMonth.ToString().Substring(4, 2), siafemCalendarComesFromScreen.ReferenceMonth.ToString().Substring(0, 4)),
                        DateClosing = siafemCalendarComesFromScreen.DateClosing,
                        DataParaOperadores = siafemCalendarComesFromScreen.DataParaOperadores,
                        Month = siafemCalendarComesFromScreen.ReferenceMonth,
                        Status = siafemCalendarComesFromScreen.Status
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MonthlyClosing/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FiscalYear,ReferenceMonth,DateClosing,DataParaOperadores")] SiafemCalendarViewModel siafemCalendarComesFromScreen)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        if (SiafemCalendarExist(siafemCalendarComesFromScreen.FiscalYear.Value))
                        {
                            ViewBag.siafemCalendarExiste = "Fechamento Mensal já está cadastrado para a UO!";
                            return View(siafemCalendarComesFromScreen);
                        }

                        SiafemCalendar siafemCalendar = (from m in db.SiafemCalendars where m.Id == siafemCalendarComesFromScreen.Id select m).FirstOrDefault();
                        //MonthlyClosing monthlyClosing = new MonthlyClosing();
                        siafemCalendar.FiscalYear = siafemCalendarComesFromScreen.FiscalYear;
                        siafemCalendar.DateClosing = siafemCalendarComesFromScreen.DateClosing;
                        siafemCalendar.DataParaOperadores = siafemCalendarComesFromScreen.DataParaOperadores;
                        //monthlyClosing.ReferenceMonth = Convert.ToInt32(siafemCalendarComesFromScreen.ReferenceMonth.ToString());


                        siafemCalendar.Status = true;
                        db.Entry(siafemCalendar).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                //getHierarquiaPerfil();
                //MonthlyClosingViewModel model = new MonthlyClosingViewModel();
                //model.FiscalYear = monthlyClosing.FiscalYear;
                ////model.ReferenceMonth = monthlyClosing.ReferenceMonth;
                //model.DateClosing = monthlyClosing.DateClosing;
                //model.ManagerUnitId = _managerUnitId.Value;

                return View(siafemCalendarComesFromScreen);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: MonthlyClosing/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    SiafemCalendar siafemCalendarComesFromScreen = db.SiafemCalendars.Find(id);
                    if (siafemCalendarComesFromScreen == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);


                    getHierarquiaPerfil();
                    //string[] referenceMonth = Convert.ToInt32(siafemCalendarComesFromScreen.ReferenceMonth[1].Split('/') + siafemCalendarComesFromScreen.ReferenceMonth[0].Split('/'));
                    var model = new SiafemCalendarViewModel()
                    {

                        Id = siafemCalendarComesFromScreen.Id,
                        FiscalYear = siafemCalendarComesFromScreen.FiscalYear,
                        InstitutionId = _institutionId,
                        ReferenceMonth = string.Format("{0}/{1}", siafemCalendarComesFromScreen.ReferenceMonth.ToString().Substring(4, 2), siafemCalendarComesFromScreen.ReferenceMonth.ToString().Substring(0, 4)),
                        DateClosing = siafemCalendarComesFromScreen.DateClosing,
                        Month = siafemCalendarComesFromScreen.ReferenceMonth,
                        Status = siafemCalendarComesFromScreen.Status
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MonthlyClosing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //if ((from am in db.AssetMovements where am. == id && am.Status == true select am).Any())
                //    return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                using (db = new SAMContext())
                {
                    SiafemCalendar siafemCalendarComesFromScreen = db.SiafemCalendars.Find(id);
                    db.SiafemCalendars.Remove(siafemCalendarComesFromScreen);
                    //db.Entry(siafemCalendarComesFromScreen).State = EntityState.Modified;
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

        #region Metodos privados
        private bool SiafemCalendarExist(int _code)
        {
            bool retorno = false;
            var result = (from m in db.SiafemCalendars where m.Status == true && m.Id == _code select m);

            if (result.ToList().Count > 0)
                retorno = true;

            return retorno;
        }

        private string reverseString(string Word)
        {
            string[] arrChar = Word.Split('/');
            string invertida = "";
            if (arrChar.Length > 2)
            {
                invertida = arrChar[2] + "/" + arrChar[1] + "/" + arrChar[0];
            }
            else if (arrChar.Length > 1)
            {
                invertida = arrChar[1] + "/" + arrChar[0];
            }
            else
            {
                invertida = arrChar[0].ToString();
            }
            return invertida;
        }

        #endregion
    }
}
