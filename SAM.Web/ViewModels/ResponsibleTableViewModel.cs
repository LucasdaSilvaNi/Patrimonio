using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class ResponsibleTableViewModel
    {
       
        public int Id { get; set; }
        public string InstitutionCode { get; set; }

        public string BudgetUnitCode { get; set; }

        public string ManagerUnitCode { get; set; }

        public int AdministrativeUnitCode { get; set; }

        public string AdministrativeUnitDescription { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }
  
        public int AdministrativeUnitId { get; set; }

        public int ManagerUnitId { get; set;}

        public int BudgetUnitId { get; set; }

        public int InstitutionId { get; set;}

    }
}