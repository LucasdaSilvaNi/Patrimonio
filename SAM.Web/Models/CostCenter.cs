using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class CostCenter
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name="C�digo")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Descri��o")]
        public string Description { get; set; }

        public bool Status { get; set; }
    }
}
