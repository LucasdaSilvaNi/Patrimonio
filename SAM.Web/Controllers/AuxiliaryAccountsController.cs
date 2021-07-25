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
using Data = System.Collections.Generic.KeyValuePair<string, string>;

namespace SAM.Web.Controllers
{
    public class AuxiliaryAccountsController : BaseController
    {
        private SAMContext db;

        #region Actions
        #region Index Actions


        public async Task<ActionResult> Index()
        {
            ViewBag.TemPermissao = PerfilAdmGeral();
            var view = new AuxiliaryAccount();
            return await Task.Run(() => View());
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(AuxiliaryAccount auxiliary)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();

            
            List<AuxiliaryAccountViewModel> auxiliaryResult;
            int totalRegistros = 0;

            try
            {
                using (db = new SAMContext())
                {
                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    {
                        var lstRetornoPesquisa = (from a in db.AuxiliaryAccounts.Include("RelatedDepreciationAccount")
                                                  join d in db.DepreciationAccounts on a.DepreciationAccountId equals d.Id into gj
                                                  from x in gj.DefaultIfEmpty()
                                                  where a.Status == true
                                                  select new AuxiliaryAccountViewModel
                                                  {
                                                      Id = a.Id,
                                                      Description = a.Description,
                                                      BookAccount = a.BookAccount,
                                                      CodigoContaContabil = a.ContaContabilApresentacao,
                                                      CodigoContaDepreciada = (a.RelatedDepreciationAccount == null ? -1 : a.RelatedDepreciationAccount.Code),
                                                      DescricaoContaDepreciada = (a.RelatedDepreciationAccount == null ? String.Empty : a.RelatedDepreciationAccount.Description)
                                                  }).Where(a => a.Description.Contains(currentFilter) ||
                                                               a.CodigoContaContabil.ToString().Contains(currentFilter) ||
                                                               a.CodigoContaDepreciada.ToString().Contains(currentFilter) ||
                                                               a.DescricaoContaDepreciada.ToString().Contains(currentFilter)
                                                          )
                                                   .OrderBy(a => a.Description).AsNoTracking();

                        totalRegistros = await lstRetornoPesquisa.CountAsync();

                        auxiliaryResult = await lstRetornoPesquisa.ToListAsync();
                    }
                    else
                    {
                        var lstRetorno = (from a in db.AuxiliaryAccounts where a.Status == true select a).OrderBy(a => a.Description).AsNoTracking();

                        totalRegistros = await lstRetorno.CountAsync();

                        var result = await lstRetorno.Skip(startRec).Take(length).ToListAsync();

                        auxiliaryResult = result.ConvertAll(new Converter<AuxiliaryAccount, AuxiliaryAccountViewModel>(new AuxiliaryAccountViewModel().Create));
                    }
                }

                var auxiliaryAccount = Ordenar(order, orderDir, auxiliaryResult);

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = auxiliaryAccount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
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

                db = new SAMContext();
                AuxiliaryAccount auxiliaryAccount = db.AuxiliaryAccounts.Find(id);

                if (auxiliaryAccount == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (auxiliaryAccount.RelatedDepreciationAccount != null) {
                    auxiliaryAccount.RelatedDepreciationAccount.Description = auxiliaryAccount.RelatedDepreciationAccount.Code.ToString() + " - " + auxiliaryAccount.RelatedDepreciationAccount.Description;
                }

                return View(auxiliaryAccount);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: AuxiliaryAccount/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.ValorRelacao = 0;
                CarregaCombo();
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: AuxiliaryAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,BookAccount,DepreciationAccountId,RelacionadoBP,ContaContabilApresentacao,ControleEspecificoResumido,TipoMovimentacaoContabilizaSP")] AuxiliaryAccount auxiliaryAccount)
        {
            try
            {
                ViewBag.ValorRelacao = auxiliaryAccount.RelacionadoBP;

                if (auxiliaryAccount.RelacionadoBP != 1 && auxiliaryAccount.RelacionadoBP != 2)
                {
                    if (auxiliaryAccount.DepreciationAccountId == null)
                    {
                        ModelState.AddModelError("DepreciationAccountId", "O campo Conta de Depreciação é obrigatório.");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }
                }
                else
                {
                    if (auxiliaryAccount.DepreciationAccountId != null)
                    {
                        ModelState.AddModelError("Relacao", "Contas Contábeis para acervos ou terceiros não devem possuir campo de depreciação");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }
                }

                if(auxiliaryAccount.ContaContabilApresentacao != null)
                {
                    if (auxiliaryAccount.BookAccount.ToString().Length + auxiliaryAccount.ContaContabilApresentacao.Length > 50)
                    {
                        ModelState.AddModelError("ContaContabilApresentacao", "Tamanho inválido");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }
                    else
                    {
                        auxiliaryAccount.ContaContabilApresentacao = auxiliaryAccount.BookAccount.ToString() + auxiliaryAccount.ContaContabilApresentacao;
                    }
                }
                else {
                    auxiliaryAccount.ContaContabilApresentacao = auxiliaryAccount.BookAccount.ToString();
                }

                if (auxiliaryAccount.ControleEspecificoResumido != null) {
                    if (auxiliaryAccount.ControleEspecificoResumido.Length > 15) {
                        ModelState.AddModelError("ControleEspecificoResumido", "Tamanho inválido");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }
                }

                if (!String.IsNullOrWhiteSpace(auxiliaryAccount.TipoMovimentacaoContabilizaSP))
                    if (auxiliaryAccount.TipoMovimentacaoContabilizaSP.Length > 80)
                        ModelState.AddModelError("TipoMovimentacaoContabilizaSP", "Tamanho inválido");


                ModelState.Remove("RelacionadoBP");

                if (ModelState.IsValid)
                {
                    db = new SAMContext();
                    if ((from a in db.AuxiliaryAccounts where a.Status == true && a.Code == auxiliaryAccount.Code select a.Id).Count() > 0)//AuxiliaryAccountExist(auxiliaryAccount.Code))
                    {
                        ViewBag.auxiliaryAccountExiste = "Conta Contábil já está cadastrada para a UO!";
                        CarregaCombo();
                        CarregaViewBags(auxiliaryAccount);
                        return View(auxiliaryAccount);
                    }

                    auxiliaryAccount.Status = true;
                    db.AuxiliaryAccounts.Add(auxiliaryAccount);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                CarregaCombo();
                CarregaViewBags(auxiliaryAccount);
                return View(auxiliaryAccount);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        /// <summary>
        /// Verifica se a conta auxiliar existe na UO
        /// </summary>
        /// <returns></returns>
        private bool AuxiliaryAccountExist(int _code)
        {
            bool retorno = false;
            var result = (from a in db.AuxiliaryAccounts where a.Status == true && a.Code == _code select a);

            if (result.ToList().Count > 0)
                retorno = true;

            return retorno;
        }
        #endregion
        #region Edit Actions
        // GET: AuxiliaryAccount/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                db = new SAMContext();
                AuxiliaryAccount auxiliaryAccount = db.AuxiliaryAccounts.Find(id);

                if (auxiliaryAccount == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                CarregaCombo();
                CarregaViewBags(auxiliaryAccount);
                if (auxiliaryAccount.RelacionadoBP == 0 &&
                    (from r in db.RelationshipAuxiliaryAccountItemGroups
                     where r.AuxiliaryAccountId == auxiliaryAccount.Id
                     select r).Count() > 0)
                {
                    ViewBag.Trava = 1;
                }

                if(auxiliaryAccount.ContaContabilApresentacao != null)
                    auxiliaryAccount.ContaContabilApresentacao = auxiliaryAccount.ContaContabilApresentacao.Substring(9);

                if (!String.IsNullOrWhiteSpace(auxiliaryAccount.TipoMovimentacaoContabilizaSP))
                    if (auxiliaryAccount.TipoMovimentacaoContabilizaSP.Length > 80)
                        auxiliaryAccount.TipoMovimentacaoContabilizaSP = auxiliaryAccount.TipoMovimentacaoContabilizaSP.Substring(80);

                return View(auxiliaryAccount);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: AuxiliaryAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,BookAccount,DepreciationAccountId,RelacionadoBP,ContaContabilApresentacao,ControleEspecificoResumido,TipoMovimentacaoContabilizaSP")] AuxiliaryAccount auxiliaryAccount)
        {
            try
            {
                if (auxiliaryAccount.RelacionadoBP != 1 && auxiliaryAccount.RelacionadoBP != 2)
                {
                    if (auxiliaryAccount.DepreciationAccountId == null)
                    {
                        ModelState.AddModelError("DepreciationAccountId", "O campo Conta de Depreciação é obrigatório.");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }
                }
                else
                {
                    if (auxiliaryAccount.DepreciationAccountId != null)
                    {
                        ModelState.AddModelError("Relacao", "Contas Contábeis para acervos ou terceiros não devem possuir campo de depreciação");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }

                }

                if (auxiliaryAccount.ContaContabilApresentacao != null)
                {
                    if (auxiliaryAccount.BookAccount.ToString().Length + auxiliaryAccount.ContaContabilApresentacao.Length > 50)
                    {
                        ModelState.AddModelError("ContaContabilApresentacao", "Tamanho inválido");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }
                    else
                    {
                        auxiliaryAccount.ContaContabilApresentacao = auxiliaryAccount.BookAccount.ToString() + auxiliaryAccount.ContaContabilApresentacao;
                    }
                }
                else {
                    auxiliaryAccount.ContaContabilApresentacao = auxiliaryAccount.BookAccount.ToString();
                }

                if (auxiliaryAccount.ControleEspecificoResumido != null)
                {
                    if (auxiliaryAccount.ControleEspecificoResumido.Length > 15)
                    {
                        ModelState.AddModelError("ControleEspecificoResumido", "Tamanho inválido");
                        //CarregaCombo();
                        //CarregaViewBags(auxiliaryAccount);
                        //return View(auxiliaryAccount);
                    }
                }

                if (!String.IsNullOrWhiteSpace(auxiliaryAccount.TipoMovimentacaoContabilizaSP))
                    if (auxiliaryAccount.TipoMovimentacaoContabilizaSP.Length > 80)
                        ModelState.AddModelError("TipoMovimentacaoContabilizaSP", "Tamanho inválido");

                ModelState.Remove("RelacionadoBP");

                if (ModelState.IsValid)
                {
                    db = new SAMContext();
                    auxiliaryAccount.Status = true;
                    db.Entry(auxiliaryAccount).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index"); 
                }

                CarregaCombo();
                CarregaViewBags(auxiliaryAccount);
                return View(auxiliaryAccount);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: AuxiliaryAccount/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                db = new SAMContext();
                AuxiliaryAccount auxiliaryAccount = db.AuxiliaryAccounts.Find(id);

                if (auxiliaryAccount == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (auxiliaryAccount.RelatedDepreciationAccount != null)
                {
                    auxiliaryAccount.RelatedDepreciationAccount.Description = auxiliaryAccount.RelatedDepreciationAccount.Code.ToString() + " - " + auxiliaryAccount.RelatedDepreciationAccount.Description;
                }

                return View(auxiliaryAccount);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: AuxiliaryAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db = new SAMContext();

                if ((from am in db.AssetMovements where am.AuxiliaryAccountId == id && am.Status == true select am).Any())
                    return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                AuxiliaryAccount auxiliaryAccount = db.AuxiliaryAccounts.Find(id);
                auxiliaryAccount.Status = false;
                db.Entry(auxiliaryAccount).State = EntityState.Modified;
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

        #region Metodos privados

        public IList<AuxiliaryAccountViewModel> Ordenar(string ordenacao, string ordenacaoAscDesc, IList<AuxiliaryAccountViewModel> tableAuxiliary)
        {
            try
            {
                // Sorting
                switch (ordenacao)
                {
                    case "0":
                        tableAuxiliary = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableAuxiliary.OrderByDescending(p => p.Description).ToList()  : tableAuxiliary.OrderBy(p => p.Description).ToList();
                        break;
                    case "1":
                        tableAuxiliary = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableAuxiliary.OrderByDescending(p => p.BookAccount).ToList() : tableAuxiliary.OrderBy(p => p.BookAccount).ToList();
                        break;
                    case "2":
                        tableAuxiliary = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableAuxiliary.OrderByDescending(p => p.CodigoContaDepreciada).ToList() : tableAuxiliary.OrderBy(p => p.CodigoContaDepreciada).ToList();
                        break;
                    case "3":
                        tableAuxiliary = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableAuxiliary.OrderByDescending(p => p.DescricaoContaDepreciada).ToList() : tableAuxiliary.OrderBy(p => p.DescricaoContaDepreciada).ToList();
                        break;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return tableAuxiliary;
        }

        private void CarregaCombo() {
            if (db == null) {
                db = new SAMContext();
            }

            List<DepreciationAccount> listaDepreciationAccount = (from da in db.DepreciationAccounts select da).AsNoTracking().ToList();
            listaDepreciationAccount.ForEach(da => da.Description = da.Code.ToString() + " - " + da.Description);

            ViewBag.DepreciationAccounts = new SelectList(listaDepreciationAccount, "Id", "Description");
        }

        private void CarregaViewBags(AuxiliaryAccount auxiliaryAccount) {
            ViewBag.ValorRelacao = auxiliaryAccount.RelacionadoBP;
        }

        #region JSON
        public JsonResult CarregaComboContaContabilPorGrupo(string code)
        {
            List<Data> lista = new List<Data>();
            List<AuxiliaryAccount> listaRetorno;

            try
            {
                int codigo = Convert.ToInt32(code);

                using (db = new SAMContext())
                {
                    listaRetorno = (from ac in db.AuxiliaryAccounts
                                    join rag in db.RelationshipAuxiliaryAccountItemGroups on ac.Id equals rag.AuxiliaryAccountId
                                    join mg in db.MaterialGroups on rag.MaterialGroupId equals mg.Id
                                    where mg.Code == codigo &&
                                          ac.Status == true &&
                                          !ac.Description.Contains("Importado do legado")
                                    select ac).ToList();
                }

                listaRetorno.ForEach(o => o.Description = o.ContaContabilApresentacao + " - " + o.Description);

                listaRetorno.OrderBy(a => a.BookAccount);

                listaRetorno.ForEach(o => lista.Add(new Data(o.Id.ToString(), o.Description.ToString())));

                var _listaRetorno = new SelectList(lista, "Key", "Value", "0");

                return Json(_listaRetorno, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Erro", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CarregaComboContaContabilPorTipo(int tipo)
        {
            List<Data> lista = new List<Data>();
            List<AuxiliaryAccount> listaRetorno;

            try
            {
                if (tipo < 1)
                {
                    return Json("código inválido", JsonRequestBehavior.AllowGet);
                }

                using (db = new SAMContext())
                {
                    listaRetorno = (from ac in db.AuxiliaryAccounts
                                    where ac.RelacionadoBP == tipo
                                       && ac.Status == true
                                    select ac).ToList();
                }

                listaRetorno.ForEach(o => o.Description = o.ContaContabilApresentacao + " - " + o.Description);

                listaRetorno.OrderBy(a => a.BookAccount);

                listaRetorno.ForEach(o => lista.Add(new Data(o.Id.ToString(), o.Description.ToString())));

                var _listaRetorno = new SelectList(lista, "Key", "Value", "0");

                return Json(_listaRetorno, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Erro", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion
    }
}
