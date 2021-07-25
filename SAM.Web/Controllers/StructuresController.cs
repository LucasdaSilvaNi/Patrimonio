using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class StructuresController : Controller
    {
        private StructureContext db;

        /// <summary>
        /// Get institutions for selects elements
        /// </summary>
        /// <returns></returns>
            //public SelectList GetInstitutions()
            //{
            //    User u = SAM.Web.Common.UserCommon.CurrentUser();
            //    RelationshipUserProfile rup = SAM.Web.Common.UserCommon.CurrentRelationshipUsersProfile(u.Id);
            //    var CurrentProfile = SAM.Web.Common.UserCommon.CurrentProfileLogin(rup.Id);

            //    using (dbHier = new HierarquiaContext())
            //    {
            //        var result = dbHier.Institutions.Where(m => m.Status == true && m.Id == CurrentProfile.InstitutionId).OrderBy(m => m.Description);
            //        return new SelectList(result, "Id", "Description");
            //    }
            //}
        /// <summary>
        /// Get institutions for selects elements with manager parameter
        /// </summary>
        /// <returns></returns>
            //public SelectList GetInstitutions(RelationshipUserProfileInstitution relationshipUserProfileInstitution)
            //{

            //    return new SelectList(GetInstitutions().ToList(), "Value", "Text", relationshipUserProfileInstitution.InstitutionId);
            //}


        /// <summary>
        /// Get modules for selects elements
        /// </summary>
        /// <returns></returns>
        public SelectList GetModules()
        {
            using (db = new StructureContext())
            {
                var result = db.Modules.Where(m => m.Status == true).OrderBy(m => m.Name).ToList();
                result.ForEach(r => r.Name = r.Name + " - " + r.Sequence);

                return new SelectList(result, "Id", "Name");
            }
        }
        /// <summary>
        /// Get modules for selects elements with transanction parameter
        /// </summary>
        /// <returns></returns>
        public SelectList GetModules(int? ModuleId)
        {
            return new SelectList(GetModules().ToList(), "Value", "Text", ModuleId);
        }
        /// <summary>
        /// Get modules for selects elements with transanction parameter
        /// </summary>
        /// <returns></returns>
        public SelectList GetTypeTransaction()
        {
            db = new StructureContext();
            var result = db.TypeTransactions.OrderBy(m => m.Description);
            return new SelectList(result, "Id", "Description");
        }
        /// <summary>
        /// Get modules for selects elements with transanction Id parameter
        /// </summary>
        /// <returns></returns>
        public SelectList GetTypeTransaction(int IdTipoTransacao)
        {
            db = new StructureContext();
            var result = db.TypeTransactions.OrderBy(m => m.Description);
            return new SelectList(result, "Id", "Description", IdTipoTransacao);

        }
        /// <summary>
        /// Get systems for selects elements
        /// </summary>
        /// <returns></returns>
        public SelectList GetSystems()
        {
            using (db = new StructureContext())
            {
                var result = db.ManagedSystems.Where(m => m.Status == true).OrderBy(m => m.Name).ToList();

                result.ForEach(r => r.Name = r.Name + " - " + r.Sequence);

                return new SelectList(result, "Id", "Name");
            }
        }
        /// <summary>
        /// Get systems for selects elements with module parameter
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public SelectList GetSystems(int IdDoMenu)
        {
            return new SelectList(GetSystems().ToList(), "Value", "Text", IdDoMenu);
        }

    }
}