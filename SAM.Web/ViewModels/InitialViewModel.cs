using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class InitialViewModel: Initial
    {
        public string InstitutionCode { get; set; }

        public string InstitutionDescription { get; set; }

        public string BudgetUnitCode { get; set; }

        public string BudgetUnitDescription { get; set; }

        public string ManagerUnitCode { get; set; }

        public string ManagerUnitDescription { get; set; }
    }
}