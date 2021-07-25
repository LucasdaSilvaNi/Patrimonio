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
using SAM.Web.Common.Enum;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class SuppliersController : BaseController
    {
        private SAMContext db;

        #region Actions
        #region Index Actions
        // GET: Suppliers
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.perfilOperador = (int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE || 
                                         (int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUO;

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(Supplier assetEAsset)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();

            IQueryable<Supplier> lstRetorno;
            try
            {
                int pageSize = (startRec == 0 ? 1 : startRec) + length;

                using (db = new SAMContext())
                {
                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    {
                        lstRetorno = (from s in db.Suppliers where s.Status == true select s)
                        .Where(s => s.CPFCNPJ.Contains(currentFilter) ||
                                    s.Name.Contains(currentFilter) ||
                                    s.Email.Contains(currentFilter) ||
                                    s.Telephone.Contains(currentFilter)).OrderBy(s => s.Name).AsNoTracking();
                    }
                    else
                    {
                        lstRetorno = (from s in db.Suppliers where s.Status == true select s).OrderBy(s => s.Name).AsNoTracking();
                    }


                    int totalRegistros = lstRetorno.Count();

                    var result = await lstRetorno.Skip(startRec).Take(length).ToListAsync();

                    var supplierViewModel = result.ConvertAll(new Converter<Supplier, SuppliersViewModel>(new SuppliersViewModel().Create));

                    var resultadoView = Ordenar(order, orderDir, supplierViewModel);
                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultadoView }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, e), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        #region Details Actions
        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    Supplier supplier = db.Suppliers.Include("RelatedAddress").AsNoTracking().FirstOrDefault(s => s.Id == id); //Find(id);
                    if (supplier == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(supplier);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Suppliers/Create
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

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CPFCNPJ,Name,AddressId,RelatedAddress,Telephone,Email,AdditionalData")] Supplier supplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        if (db.Suppliers.Where(i => i.CPFCNPJ == supplier.CPFCNPJ && i.Status == true).Any())
                        {
                            ModelState.AddModelError("FornecedorJaExiste", "CPF/CNPJ Fornecedor já está cadastrado!");
                            return View(supplier);
                        }

                        //Limpa caracteres do CPF e CNPJ para ficar somente numeros
                        supplier.CPFCNPJ = supplier.CPFCNPJ.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);

                        //Limpa caracteres do CEP para ficar somente os numeros
                        supplier.RelatedAddress.PostalCode = supplier.RelatedAddress.PostalCode.Replace("-", string.Empty);

                        supplier.Status = true;
                        db.Suppliers.Add(supplier);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                return View(supplier);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }


        public JsonResult VerificarFornecedor(string CgcCpf, string DescricaoFornecedor)
        {
            Supplier _supplier = new Supplier();

            _supplier.CPFCNPJ = CgcCpf;

            using (db = new SAMContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                if ((from s in db.Suppliers where s.CPFCNPJ.Contains(_supplier.CPFCNPJ) select s).Any())
                    _supplier = (from s in db.Suppliers where s.CPFCNPJ.Contains(_supplier.CPFCNPJ) select s).FirstOrDefault();
                else
                {
                    _supplier.Name = DescricaoFornecedor;
                    _supplier.Status = true;
                    db.Entry(_supplier).State = EntityState.Added;
                    db.SaveChanges();
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = serializer.Serialize(_supplier);

                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Edit Actions

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    Supplier supplier = db.Suppliers.Include("RelatedAddress").AsNoTracking().FirstOrDefault(s => s.Id == id);
                    if (supplier == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(supplier);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CPFCNPJ,Name,AddressId,RelatedAddress,Telephone,Email,AdditionalData,Status")] Supplier supplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        Supplier _supplier = db.Suppliers.Include("RelatedAddress").AsNoTracking().FirstOrDefault(s => s.Id == supplier.Id); //Find(supplier.Id);
                        _supplier.Email = supplier.Email;
                        _supplier.Telephone = supplier.Telephone;
                        _supplier.AdditionalData = supplier.AdditionalData;
                        _supplier.RelatedAddress.PostalCode = supplier.RelatedAddress.PostalCode.Replace("-", string.Empty);
                        _supplier.RelatedAddress.Street = supplier.RelatedAddress.Street;
                        _supplier.RelatedAddress.Number = supplier.RelatedAddress.Number;
                        _supplier.RelatedAddress.ComplementAddress = supplier.RelatedAddress.ComplementAddress;
                        _supplier.RelatedAddress.District = supplier.RelatedAddress.District;
                        _supplier.RelatedAddress.City = supplier.RelatedAddress.City;
                        _supplier.RelatedAddress.State = supplier.RelatedAddress.State;
                        _supplier.Status = supplier.Status;

                        db.Entry(_supplier).State = EntityState.Modified;
                        db.Entry(_supplier.RelatedAddress).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(supplier);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Delete Actions
        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    Supplier supplier = db.Suppliers.Include("RelatedAddress").AsNoTracking().FirstOrDefault(s => s.Id == id); //.Find(id);
                    if (supplier == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    return View(supplier);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    if ((from am in db.Assets where am.SupplierId == id && am.Status == true select am).Any())
                        return MensagemErro(CommonMensagens.ExcluirRegistroComVinculos);

                    Supplier supplier = db.Suppliers.Find(id);
                    supplier.Status = false;
                    db.Entry(supplier).State = EntityState.Modified;
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
        /// Ordena os dados pelo parametro informado
        /// </summary>
        /// <param name="sortOrder">parametro de Ordenação</param>
        /// <param name="result">Modelo que será ordenado</param>
        private IQueryable<Supplier> SortingByFilter(string sortOrder, IQueryable<Supplier> result)
        {
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CPFSortParm = sortOrder == "cpf" ? "cpf_desc" : "cpf";
            ViewBag.EmailSortParm = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.TelefoneSortParm = sortOrder == "telefone" ? "telefone_desc" : "telefone";

            switch (sortOrder)
            {
                case "name":
                    result = result.OrderBy(m => m.Name);
                    break;
                case "name_desc":
                    result = result.OrderByDescending(m => m.Name);
                    break;
                case "cpf":
                    result = result.OrderBy(m => m.CPFCNPJ);
                    break;
                case "cpf_desc":
                    result = result.OrderByDescending(m => m.CPFCNPJ);
                    break;
                case "email":
                    result = result.OrderBy(m => m.Email);
                    break;
                case "email_desc":
                    result = result.OrderByDescending(m => m.Email);
                    break;
                case "telefone":
                    result = result.OrderBy(m => m.Telephone);
                    break;
                case "telefone_desc":
                    result = result.OrderByDescending(m => m.Telephone);
                    break;
                default:
                    result = result.OrderBy(m => m.Name);
                    break;
            }

            return result;
        }

        public IList<SuppliersViewModel> Ordenar(string ordenacao, string ordenacaoAscDesc, IList<SuppliersViewModel> suppliersTable)
        {
            IList<SuppliersViewModel> _suppliersTable = new List<SuppliersViewModel>();
            try
            {
                // Sorting
                switch (ordenacao)
                {
                    case "0":
                        _suppliersTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? suppliersTable.OrderByDescending(p => p.CPFCNPJ).ToList() : suppliersTable.OrderBy(p => p.CPFCNPJ).ToList();
                        break;
                    case "1":
                        _suppliersTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? suppliersTable.OrderByDescending(p => p.Name).ToList() : suppliersTable.OrderBy(p => p.Name).ToList();
                        break;
                    case "2":
                        _suppliersTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? suppliersTable.OrderByDescending(p => p.Telephone).ToList() : suppliersTable.OrderBy(p => p.Telephone).ToList();
                        break;
                    case "3":
                        _suppliersTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? suppliersTable.OrderByDescending(p => p.Email).ToList() : suppliersTable.OrderBy(p => p.Email).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return _suppliersTable;
        }
        #endregion
    }
}
