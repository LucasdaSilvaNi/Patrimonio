using SAM.Web.Common;
using SAM.Web.Models;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SAM.Web.Controllers
{
    public class ServiceMobile : Controller
    {
        private SAMContext db = new SAMContext();

        public JsonResult Login(string _usuario, string _senha, string _macAddress)
        {

            List<ManagerUnit> listaUges = new List<ManagerUnit>();

            User usuarioRetorno = VerificaUsuario(_usuario, _senha);
            List<AdministrativeUnit> listaUaDisponivel = RetornaListaUasDisponiveis(listaUges);

            //var result = (from s in db.AdministrativeUnits where s.Status == true select s);
            //if (usuarioRetorno != null)
            //    result = (from a in db.AdministrativeUnits where a.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id == usuarioRetorno.)
            return Json(new SelectList(listaUaDisponivel, "Id", "Description"));
        }

        public JsonResult RetornaListaResponsaveis(int UaId)
        {
            var result = (from r in db.Users
                          join u in db.RelationshipUserProfiles on r.Id equals u.UserId
                          join p in db.RelationshipUserProfileInstitutions on u.Id equals p.RelationshipUserProfileId
                          where r.Status == true && u.FlagResponsavel == true && p.AdministrativeUnitId == UaId
                          select new { id = r.Id, Codigo = r.Name });

            return null;
        }

        public JsonResult RetornaListaBPs(int UaId, int ResponsavelId)
        {
            return null;
        }

        private List<AdministrativeUnit> RetornaListaUasDisponiveis(List<ManagerUnit> listaUges)
        {
            throw new NotImplementedException();
        }

        private User VerificaUsuario(string _usuario, string _senha)
        {
            throw new NotImplementedException();
        }
    }
}