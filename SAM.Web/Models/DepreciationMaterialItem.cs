using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class DepreciationMaterialItem
    {
        [Required]
        [Display(Name="Conta Contábil de Depreciação")]
        public int DepreciationAccount { get; set; }

        [Required]
        [Display(Name = "Código Item Material")]
        public int MaterialItemCode { get; set; }
    }
}
