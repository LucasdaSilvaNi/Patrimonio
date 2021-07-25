using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class CostCenter
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name="Código")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public bool Status { get; set; }
    }
}
