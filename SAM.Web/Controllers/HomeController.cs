using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class HomeController : Controller
    {
        //UsuarioContext db = new UsuarioContext();

        public ActionResult Index()
        {
            var notficationViewModel = new NotificationViewModel();

            if (Request.IsAuthenticated && Session["UsuarioLogado"] != null)
            {

                NotificationContext dbNotifica = new NotificationContext();

                var notificationDB = (from n in dbNotifica.Notifications
                                      where n.Status == true
                                      select n).FirstOrDefault();


                if (notificationDB != null)
                {
                    notficationViewModel = new NotificationViewModel()
                    {
                        Id = notificationDB.Id,
                        CorpoMensagem = System.Text.Encoding.UTF8.GetString(notificationDB.CorpoMensagem)
                    };
                }

            }

            //ToDo: //Melhorar isso para unificar dentro do BaseController
            //if (TrocaSenha())
            //    return RedirectToAction("ChangePassword/" + db.Users.FirstOrDefault(m => m.CPF == HttpContext.User.Identity.Name).Id, "Users");
            return View(notficationViewModel);
        }

        public ActionResult Mensagem(string m)
        {
            return View(model:m);
        }

        public ActionResult SistemaSobrecarregado()
        {
            string resposta = "Erro devido a sobrecarga momentânea do sistema. Por favor tente novamente mais tarde!";
            return View("Mensagem",model:resposta);
        }

        #region regra de Negócios

        //private bool TrocaSenha()
        //{
        //    return HttpContext.User.Identity.IsAuthenticated && db.Users.FirstOrDefault(m => m.CPF == HttpContext.User.Identity.Name).ChangePassword;
        //}

        #endregion
    }
}