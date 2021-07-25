using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;

namespace SAM.Web.Controllers
{
    public class MenuController : Controller //BaseController
    {
        private MenuContext db;

        /// <summary>
        /// Get menu and yours childrens for menu element
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMenu(int id)
        {

            try
            {
                using (db = new MenuContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var listaSistemas = (from rpms in db.RelationshipProfileManagedSystems
                                         join m in db.ManagedSystems on rpms.ManagedSystemId equals m.Id
                                         where rpms.ProfileId == id && rpms.ManagedSystem.Status == true
                                         select new ManagedSystemViewModel()
                                         {
                                             Id = m.Id,
                                             Name = m.Name,
                                             Description = m.Description,
                                             Sequence = m.Sequence
                                         }).AsNoTracking().ToList();

                    foreach (var system in listaSistemas)
                    {
                        var listaModulos = (from rmp in db.RelationshipModuleProfiles
                                            join m in db.Modules on rmp.ModuleId equals m.Id
                                            where m.ManagedSystemId == system.Id
                                            && rmp.ProfileId == id
                                            && rmp.Status == true
                                            && m.Status == true
                                            select m).AsNoTracking().ToList();

                        if (listaModulos.Count > 0)
                        {
                            foreach (var modulo in listaModulos.OrderBy(a => a.Sequence).ToList())
                            {
                                //Insert root module
                                system.ModulesViewModel.Add(new ModuleViewModel
                                {
                                    Id = modulo.Id,
                                    Description = modulo.Description,
                                    ManagedSystemId = modulo.ManagedSystemId,
                                    MenuName = modulo.MenuName,
                                    Name = modulo.Name,
                                    ParentId = modulo.ParentId,
                                    Path = modulo.Path,
                                    Sequence = modulo.Sequence
                                });
                            }
                        }
                    }

                    return Json(listaSistemas.OrderBy(a => a.Sequence).ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}


//Método Original - Cópia de segurança --- apagar quando terminar
//public JsonResult GetMenu(string id)
//{
//    try
//    {
//        int UserId = db.Users.FirstOrDefault(m => m.CPF == id).Id;
//        //int ProfileId = Common.UserCommon.CurrentUser().RelationshipUsersProfiles.Where(m => m.DefaultProfile).FirstOrDefault().ProfileId;
//        var RelationshipUsersProfile = Common.UserCommon.CurrentRelationshipUsersProfile(UserId);
//        int ProfileId = Common.UserCommon.CurrentProfile(RelationshipUsersProfile.Id).Id;

//        List<ManagedSystemViewModel> managedSystemsViewModel = new List<ManagedSystemViewModel>();

//        #region comentado
//        //Get all root modules by profileId by user
//        //var relatedModulesRoot = from rtp in db.RelationshipTransactionProfiles
//        //                         join t in db.Transactions on rtp.TransactionId equals t.Id
//        //                         join m in db.Modules on t.ModuleId equals m.Id
//        //                         join pr in db.Profiles on rtp.ProfileId equals pr.Id 
//        //                         join rpu in db.RelationshipUserProfiles on pr.Id equals rpu.ProfileId 
//        //                         where rtp.ProfileId == ProfileId 
//        //                            && rtp.Status
//        //                            && rpu.DefaultProfile 
//        //                         group rtp by new
//        //                         {
//        //                             ParentId = rtp.RelatedTransaction.RelatedModule.ParentId                                             

//        //                         } into groupModules
//        //                         select new
//        //                         {
//        //                             ParentId = groupModules.Key.ParentId
//        //                         };
//        #endregion

//        var relatedModulesRoot = (from rtp in db.RelationshipTransactionProfiles
//                                  join t in db.Transactions on rtp.TransactionId equals t.Id
//                                  join m in db.Modules on t.ModuleId equals m.Id
//                                  join pr in db.Profiles on rtp.ProfileId equals pr.Id
//                                  join rpu in db.RelationshipUserProfiles on pr.Id equals rpu.ProfileId
//                                  where rtp.ProfileId == ProfileId
//                                     && rtp.Status
//                                     && rpu.DefaultProfile
//                                  group rtp by new
//                                  {
//                                      ParentId = rtp.RelatedTransaction.RelatedModule.ParentId

//                                  } into groupModules
//                                  select new
//                                  {
//                                      ParentId = groupModules.Key.ParentId
//                                  }).AsQueryable();

//        var retorno = relatedModulesRoot.ToList();

//        foreach (var item in relatedModulesRoot)
//        {
//            if (item.ParentId != null)
//            {
//                ManagedSystem managedSystem = db.Modules.FirstOrDefault(m => m.Id == item.ParentId).RelatedManagedSystem;

//                //IQueryable<ManagedSystem> query = (from m in db.ManagedSystems
//                //                                    join mr in db.RelationshipProfileManagedSystems on m.Id equals mr.ManagedSystemId
//                //                                    where mr.ManagedSystemId == 3
//                //                                    select m).AsQueryable();

//                //ManagedSystem managedSystem = query.Distinct().FirstOrDefault();


//                ManagedSystemViewModel managedSystemViewModel = new ManagedSystemViewModel();

//                managedSystemViewModel.Id = managedSystem.Id;
//                managedSystemViewModel.Name = managedSystem.Name;

//                Module module = db.Modules.FirstOrDefault(m => m.Id == item.ParentId);

//                //Insert root module
//                managedSystemViewModel.ModulesViewModel.Add(new ModuleViewModel
//                {
//                    Id = module.Id,
//                    Description = module.Description,
//                    ManagedSystemId = module.ManagedSystemId,
//                    MenuName = module.MenuName,
//                    Name = module.Name,
//                    ParentId = module.ParentId,
//                    Path = module.Path,
//                    Sequence = module.Sequence
//                });

//                var modules = (from rtp in db.RelationshipTransactionProfiles
//                               join t in db.Transactions on rtp.TransactionId equals t.Id
//                               join m in db.Modules.OrderBy(m => m.Sequence) on t.ModuleId equals m.Id
//                               join pr in db.Profiles on rtp.ProfileId equals pr.Id
//                               join rpu in db.RelationshipUserProfiles on pr.Id equals rpu.ProfileId
//                               where rtp.ProfileId == ProfileId
//                                && rtp.Status
//                                && m.ManagedSystemId == managedSystem.Id
//                                && rpu.DefaultProfile
//                               group m by new
//                               {
//                                   m.Id,
//                                   m.ManagedSystemId,
//                                   m.Name,
//                                   m.MenuName,
//                                   m.Description,
//                                   m.Path,
//                                   m.ParentId,
//                                   m.Sequence
//                               } into groupModules
//                               select groupModules).AsQueryable();

//                var modulos = modules.ToList();

//                //Insert modules in transaction
//                foreach (var m in modules)
//                {
//                    managedSystemViewModel.ModulesViewModel.Add(new ModuleViewModel
//                    {
//                        Id = m.Key.Id,
//                        Description = m.Key.Description,
//                        ManagedSystemId = m.Key.ManagedSystemId,
//                        MenuName = m.Key.MenuName,
//                        Name = m.Key.Name,
//                        ParentId = m.Key.ParentId,
//                        Path = m.Key.Path,
//                        Sequence = m.Key.Sequence
//                    });
//                }

//                managedSystemsViewModel.Add(managedSystemViewModel);
//            }
//        }

//        return Json(managedSystemsViewModel, JsonRequestBehavior.AllowGet);
//    }
//    catch (Exception ex)
//    {
//        return Json(ex.Message, JsonRequestBehavior.AllowGet);
//    }
//}
