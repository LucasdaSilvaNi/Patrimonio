using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class ClosingViewModel
    {
        [Required]
        [Display(Name = "Órgão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Favor informar o órgão")]
        public int InstitutionId { get; set; }
        [Required]
        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Favor informar a UO")]
        public int BudgetUnitId { get; set; }
        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Favor informar a UGE")]
        public int? ManagerUnitId { get; set; }
        [Required]
        [Display(Name = "Mês de Referência")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Favor informar a mês/ano")]
        public string MesRef { get; set; }

        public bool IntegradoSIAFEM { get; set; }
    }
}