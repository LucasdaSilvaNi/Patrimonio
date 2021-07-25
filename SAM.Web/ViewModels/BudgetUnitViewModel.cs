using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class BudgetUnitViewModel
    {
        public BudgetUnitViewModel() {
            this.ManagerUnits = new List<ManagerUnitViewModel>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<ManagerUnitViewModel> ManagerUnits { get; set; }
    }
}