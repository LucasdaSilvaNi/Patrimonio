using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class ManagerViewModel
    {
        public ManagerViewModel() {
            this.BudgetUnits = new List<BudgetUnitViewModel>();
        }

        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<BudgetUnitViewModel> BudgetUnits { get; set; }
    }
}