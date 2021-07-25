using SAM.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Web.Incorporacao
{
    public class BuscaSiglas
    {
        private SAMContext db;
        public List<Initial> lista;
        public BuscaSiglas(SAMContext param, int IdOrgao, int? IdUO, int? IdUGE)
        {
            db = param;

            if (db == null)
                db = new SAMContext();

            lista = db.Initials.Where(i => i.InstitutionId == IdOrgao && (i.BudgetUnitId == IdUO || i.BudgetUnitId == null) && (i.ManagerUnitId == IdUGE || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList();
        }
    }
}