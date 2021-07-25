using SAM.Web.Context;
using SAM.Web.Models;
using System.Linq;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class HierarquiaController : Controller
    {
        private SAMContext db;
        private HierarquiaContext dbHierarquia;
        // GET: Hierarquia
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetOrgaos()
        {
            using (dbHierarquia = new HierarquiaContext())
            {
                this.dbHierarquia.Configuration.AutoDetectChangesEnabled = false;
                this.dbHierarquia.Configuration.LazyLoadingEnabled = false;

                IQueryable<Institution> query = (from og in this.dbHierarquia.Institutions
                                                 where og.Status == true
                                                 select og).AsQueryable();

                var lista = query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
                lista.ForEach(o => o.Description = o.Code + " - " + o.Description);

                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }

        public JsonResult GetUos(int? orgaoId)
        {
            using (dbHierarquia = new HierarquiaContext())
            {
                this.dbHierarquia.Configuration.AutoDetectChangesEnabled = false;
                this.dbHierarquia.Configuration.LazyLoadingEnabled = false;
                IQueryable<BudgetUnit> query;

                query = (from uo in this.dbHierarquia.BudgetUnits
                         where uo.InstitutionId == orgaoId && uo.Status == true
                         select uo).AsQueryable();

                var lista = query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
                lista.ForEach(o => o.Description = o.Code + " - " + o.Description);

                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }

        public JsonResult GetUges(int? uoId)
        {
            using (dbHierarquia = new HierarquiaContext())
            {
                this.dbHierarquia.Configuration.AutoDetectChangesEnabled = false;
                this.dbHierarquia.Configuration.LazyLoadingEnabled = false;
                IQueryable<ManagerUnit> query;

                query = (from ug in this.dbHierarquia.ManagerUnits
                         where ug.BudgetUnitId == uoId && ug.Status == true
                         select ug).AsQueryable();

                var lista = query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
                lista.ForEach(o => o.Description = o.Code + " - " + o.Description);

                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }

        public JsonResult GetUgesIntegradasAoSIAFEM(int? uoId)
        {
            using (dbHierarquia = new HierarquiaContext())
            {
                this.dbHierarquia.Configuration.AutoDetectChangesEnabled = false;
                this.dbHierarquia.Configuration.LazyLoadingEnabled = false;
                IQueryable<ManagerUnit> query;

                query = (from ug in this.dbHierarquia.ManagerUnits
                         where ug.BudgetUnitId == uoId 
                         && ug.Status == true
                         && ug.FlagIntegracaoSiafem 
                         select ug).AsQueryable();

                var lista = query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
                lista.ForEach(o => o.Description = o.Code + " - " + o.Description);

                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }

        /// <summary>
        /// Busca responsaveis por UA
        /// </summary>
        /// <param name="uaId"></param>
        /// <returns></returns>
        public JsonResult GetResponsavel(int? uaId)
        {
            using (db = new SAMContext())
            {
                this.db.Configuration.AutoDetectChangesEnabled = false;
                this.db.Configuration.LazyLoadingEnabled = false;

                //IQueryable<User> query;
                // TODO: MUDAR DE RESPONSAVEL PARA USUARIO QUANDO DEFINIR REGRA
                //query = (from r in dbHierarquia.Users
                //         join u in dbHierarquia.RelationshipUserProfiles on r.Id equals u.UserId
                //         join p in dbHierarquia.RelationshipUserProfileInstitutions on u.Id equals p.RelationshipUserProfileId
                //         where r.Status == true && u.FlagResponsavel == true && p.AdministrativeUnitId == uaId
                //         select r).AsQueryable();

                IQueryable<Responsible> query;
                query = (from r in db.Responsibles
                         where r.Status == true && r.AdministrativeUnitId == uaId
                         select r).AsQueryable();

                var lista = query.Distinct().ToList().OrderBy(x => x.Name).ToList();
                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }

        /// <summary>
        /// Buscar responsaveis por divisao
        /// </summary>
        /// <param name="uaId"></param>
        /// <returns></returns>
        public JsonResult GetResponsavelPorDivisao(int? divisaoId)
        {
            using (db = new SAMContext())
            {
                this.db.Configuration.AutoDetectChangesEnabled = false;
                this.db.Configuration.LazyLoadingEnabled = false;

                //IQueryable<User> query;
                // TODO: MUDAR DE RESPONSAVEL PARA USUARIO QUANDO DEFINIR REGRA
                //query = (from r in dbHierarquia.Users
                //         join u in dbHierarquia.RelationshipUserProfiles on r.Id equals u.UserId
                //         join p in dbHierarquia.RelationshipUserProfileInstitutions on u.Id equals p.RelationshipUserProfileId
                //         where r.Status == true && u.FlagResponsavel == true && p.AdministrativeUnitId == uaId
                //         select r).AsQueryable();

                IQueryable<Responsible> query;
                query = (from r in db.Responsibles
                         join s in db.Sections
                         on r.Id equals s.ResponsibleId
                         where r.Status == true && s.Id == divisaoId
                         select r).AsQueryable();

                var lista = query.Distinct().ToList().OrderBy(x => x.Name).ToList();
                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }
        public JsonResult GetUas(int? ugeId)
        {
            using (dbHierarquia = new HierarquiaContext())
            {
                this.dbHierarquia.Configuration.AutoDetectChangesEnabled = false;
                this.dbHierarquia.Configuration.LazyLoadingEnabled = false;
                IQueryable<AdministrativeUnit> query;

                query = (from ua in this.dbHierarquia.AdministrativeUnits
                         where ua.ManagerUnitId == ugeId && ua.Status == true
                         select ua).AsQueryable();

                var lista = query.ToList().OrderBy(o => o.Code).ToList();
                lista.ForEach(o => o.Description = o.Code + " - " + o.Description);

                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }

        public JsonResult GetDivisoes(int? uaId)
        {
            using (dbHierarquia = new HierarquiaContext())
            {
                this.dbHierarquia.Configuration.AutoDetectChangesEnabled = false;
                this.dbHierarquia.Configuration.LazyLoadingEnabled = false;
                IQueryable<Section> query;

                query = (from div in this.dbHierarquia.Sections
                         where div.AdministrativeUnitId == uaId && div.Status == true
                         select div).AsQueryable();

                var lista = query.ToList().OrderBy(o => o.Code).ToList();
                lista.ForEach(o => o.Description = o.Code + " - " + o.Description);

                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }

        public JsonResult GetInitial(int orgaoId, int? uoId, int? ugeId)
        {
            using (db = new SAMContext())
            {
                this.db.Configuration.AutoDetectChangesEnabled = false;
                this.db.Configuration.LazyLoadingEnabled = false;
                IQueryable<Initial> query;

                query = (from i in this.db.Initials
                         where i.InstitutionId == orgaoId &&
                         (i.BudgetUnitId == uoId || i.BudgetUnitId == null) &&
                         (i.ManagerUnitId == ugeId || i.ManagerUnitId == null) &&
                         i.Status == true
                         select i).AsQueryable();

                var lista = query.ToList().OrderBy(i => i.Name).ToList();

                var retorno = Json(lista, JsonRequestBehavior.AllowGet);

                return retorno;
            }
        }
    }
}