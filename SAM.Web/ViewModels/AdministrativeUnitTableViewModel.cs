using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class AdministrativeUnitTableViewModel
    {
        public int Id { get; set; }

        public string ManagerUnitCode { get; set; }

        public string ManagerUnitDescription { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public AdministrativeUnitTableViewModel Create(AdministrativeUnit administrative)
        {
            /* return new ViewModels.SuppliersViewModel()*/
            return new AdministrativeUnitTableViewModel()
            {
                Id = administrative.Id,
                ManagerUnitCode = administrative.RelatedManagerUnit.Code,
                ManagerUnitDescription = administrative.RelatedManagerUnit.Description,
                Code = administrative.Code,
                Description = administrative.Description
            };
        }
    }
}
