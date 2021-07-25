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

namespace SAM.Web.Controllers
{
    public class MaterialSubItemsController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: materialSubItem
        public ActionResult Index()
        {
            try
            {
                return View(db.MaterialSubItems.Where(m => m.Status).ToList());
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: materialSubItem/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialSubItem materialSubItem = db.MaterialSubItems.Find(id);
                if (materialSubItem == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(materialSubItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: materialSubItem/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.ItemMateriais = new SelectList(db.MaterialItems.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.Gestores = new SelectList(db.Managers.OrderBy(m => m.Name), "Id", "Name");
                ViewBag.NaturezaDespesas = new SelectList(db.SpendingOrigins.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.ContasAuxiliares = new SelectList(db.AuxiliaryAccounts.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.UnidadesFornecimento = new SelectList(db.SupplyUnits.OrderBy(m => m.Description), "Id", "Description");

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: materialSubItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, MaterialItemId, Code, Description, BarCode, Lot, ActivityIndicator, ManagerId, SpendingOriginId, AuxiliaryAccountId, SupplyUnitId")] MaterialSubItem materialSubItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    materialSubItem.Status = true;
                    db.MaterialSubItems.Add(materialSubItem);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.ItemMateriais = new SelectList(db.MaterialItems.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.Gestores = new SelectList(db.Managers.OrderBy(m => m.Name), "Id", "Name");
                ViewBag.NaturezaDespesas = new SelectList(db.SpendingOrigins.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.ContasAuxiliares = new SelectList(db.AuxiliaryAccounts.OrderBy(m => m.Description), "Id", "Description");
                ViewBag.UnidadesFornecimento = new SelectList(db.SupplyUnits.OrderBy(m => m.Description), "Id", "Description");

                return View(materialSubItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: materialSubItem/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialSubItem materialSubItem = db.MaterialSubItems.Find(id);
                if (materialSubItem == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                ViewBag.ItemMateriais = new SelectList(db.MaterialItems.OrderBy(m => m.Description), "Id", "Description", materialSubItem.MaterialItemId);
                ViewBag.Gestores = new SelectList(db.Managers.OrderBy(m => m.Name), "Id", "Name", materialSubItem.ManagerId);
                ViewBag.NaturezaDespesas = new SelectList(db.SpendingOrigins.OrderBy(m => m.Description), "Id", "Description", materialSubItem.SpendingOriginId);
                ViewBag.ContasAuxiliares = new SelectList(db.AuxiliaryAccounts.OrderBy(m => m.Description), "Id", "Description", materialSubItem.AuxiliaryAccountId);
                ViewBag.UnidadesFornecimento = new SelectList(db.SupplyUnits.OrderBy(m => m.Description), "Id", "Description", materialSubItem.SupplyUnitId);

                return View(materialSubItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: materialSubItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, MaterialItemId, Code, Description, BarCode, Lot, ActivityIndicator, ManagerId, SpendingOriginId, AuxiliaryAccountId, SupplyUnitId")] MaterialSubItem materialSubItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    materialSubItem.Status = true;
                    db.Entry(materialSubItem).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.ItemMateriais = new SelectList(db.MaterialItems.OrderBy(m => m.Description), "Id", "Description", materialSubItem.MaterialItemId);
                ViewBag.Gestores = new SelectList(db.Managers.OrderBy(m => m.Name), "Id", "Name", materialSubItem.ManagerId);
                ViewBag.NaturezaDespesas = new SelectList(db.SpendingOrigins.OrderBy(m => m.Description), "Id", "Description", materialSubItem.SpendingOriginId);
                ViewBag.ContasAuxiliares = new SelectList(db.AuxiliaryAccounts.OrderBy(m => m.Description), "Id", "Description", materialSubItem.AuxiliaryAccountId);
                ViewBag.UnidadesFornecimento = new SelectList(db.SupplyUnits.OrderBy(m => m.Description), "Id", "Description", materialSubItem.SupplyUnitId);

                return View(materialSubItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: materialSubItem/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialSubItem materialSubItem = db.MaterialSubItems.Find(id);
                if (materialSubItem == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(materialSubItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: materialSubItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MaterialSubItem materialSubItem = db.MaterialSubItems.Find(id);
                materialSubItem.Status = false;
                db.Entry(materialSubItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
