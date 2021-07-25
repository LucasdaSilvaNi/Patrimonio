using SAM.Web.Context;
using SAM.Web.ViewModels;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace SAM.Web.Controllers
{
    public class PrincipalController : Controller
    {
        NotificationContext db = new NotificationContext();

        public ActionResult Index()
        {
            
            var notficationViewModel = new NotificationViewModel();

            var notificationDB = (from n in db.Notifications
                                  where n.Status == true
                                  select n).AsNoTracking().FirstOrDefault();

            if (notificationDB != null)
            {
                notficationViewModel = new NotificationViewModel()
                {
                    Id = notificationDB.Id,
                    CorpoMensagem = System.Text.Encoding.UTF8.GetString(notificationDB.CorpoMensagem)
                };
            }

            return View(notficationViewModel);
        }

        [Authorize]
        public ActionResult Ajuda() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Video(int video)
        {
            string caminho = string.Empty;
            switch (video) {
                case 1:
                    caminho = "/Patrimonio/Files/Videos/SAM_Patrimonio_Chamados.mp4";
                break;
                case 2:
                    caminho = "/Patrimonio/Files/Videos/SAM_Patrimonio_Integracao_ContabilizaSP.mp4";
                    break;
                case 3:
                    caminho = "/Patrimonio/Files/Videos/SAM_Patrimonio_Integracao_Estoque.mp4";
                    break;
            }
            
            return View(model: caminho);
        }
        
        public FileResult Manual()
        {
            var serverPath = Server.MapPath("~/");

            byte[] fileBytes = System.IO.File.ReadAllBytes(serverPath + "/Files/handbooks/manual_v1.pdf");
            string fileName = "manual_patrimonio_v1.pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public FileResult DecretoRevalorizacaoBPs_63616()
        {
            var serverPath = Server.MapPath("~/");

            byte[] fileBytes = System.IO.File.ReadAllBytes(serverPath + "/Files/handbooks/Decreto_63616_31Jul2018__Regularizacao_Bens_Patrimoniais.pdf");
            string fileName = "Decreto_63616_31Jul2018__Regularizacao_Bens_Patrimoniais.pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}