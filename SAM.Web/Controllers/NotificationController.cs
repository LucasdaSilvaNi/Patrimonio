using PagedList;
using SAM.Web.Context;
using SAM.Web.Common;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class NotificationController : BaseController
    {
        private NotificationContext db;

        // GET: Notification
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page, string cbStatus)
        {
            try
            {
                using (db = new NotificationContext())
                {
                    var lstNotifications = (from n in db.Notifications
                                            select new NotificationViewModel()
                                            {
                                                Id = n.Id,
                                                Titulo = n.Titulo,
                                                //CorpoMensagem = System.Text.Encoding.UTF8.GetString(n.CorpoMensagem),
                                                DataCriacao = n.DataCriacao,
                                                Status = n.Status
                                            }).ToList();

                    //Pagination
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var result = lstNotifications.OrderByDescending(s => s.Status == true).ThenBy(s => s.Titulo).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);

                    var listaparaCount = (from n in lstNotifications select n.Id).AsEnumerable();
                    int contador = listaparaCount.Count();

                    var retorno = new StaticPagedList<NotificationViewModel>(result.AsEnumerable(), pageNumber, pageSize, contador);

                    return View(retorno);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }


        public ActionResult Create()
        {
            try
            {
                return View(new NotificationViewModel());
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NotificationViewModel notificationViewModel)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    if (ModelState.IsValid)
                    {
                        using (db = new NotificationContext())
                        {
                            var notification = new Notification();
                            notification.Titulo = notificationViewModel.Titulo;
                            notification.CorpoMensagem = System.Text.Encoding.UTF8.GetBytes(notificationViewModel.CorpoMensagem);
                            notification.DataCriacao = DateTime.Now;
                            notification.Status = notificationViewModel.Status;

                            db.Entry(notification).State = EntityState.Added;
                            db.SaveChanges();


                            if (notificationViewModel.Status == true)
                            {
                                var listNotificationDB = (from n in db.Notifications
                                                          select n).ToList();

                                listNotificationDB.RemoveAll(x => x.Id == notificationViewModel.Id);

                                foreach (var item in listNotificationDB)
                                {
                                    item.Status = false;

                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }

                        transaction.Complete();

                        return RedirectToAction("Index");
                    }

                    return View(notificationViewModel);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            try
            {
                using (db = new NotificationContext())
                {
                    var notificationDB = (from n in db.Notifications
                                          where n.Id == Id
                                          select n).FirstOrDefault();


                    var notificationViewModel = new NotificationViewModel()
                    {
                        Id = notificationDB.Id,
                        Titulo = notificationDB.Titulo,
                        CorpoMensagem = System.Text.Encoding.UTF8.GetString(notificationDB.CorpoMensagem),
                        DataCriacao = notificationDB.DataCriacao,
                        Status = notificationDB.Status
                    };

                    return View(notificationViewModel);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NotificationViewModel notificationViewModel)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {

                    if (ModelState.IsValid)
                    {
                        using (db = new NotificationContext())
                        {
                            var listNotificationDB = (from n in db.Notifications
                                                      select n).ToList();


                            var notificationDB = listNotificationDB.Where(x => x.Id == notificationViewModel.Id).FirstOrDefault();

                            notificationDB.Titulo = notificationViewModel.Titulo;
                            notificationDB.CorpoMensagem = System.Text.Encoding.UTF8.GetBytes(notificationViewModel.CorpoMensagem);
                            notificationDB.Status = notificationViewModel.Status;

                            if (notificationViewModel.Status == true)
                            {
                                listNotificationDB.RemoveAll(x => x.Id == notificationViewModel.Id);

                                foreach (var item in listNotificationDB)
                                {
                                    item.Status = false;

                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            db.Entry(notificationDB).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        transaction.Complete();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(notificationViewModel);
                    }                    
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
    }
}