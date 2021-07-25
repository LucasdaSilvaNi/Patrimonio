using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class DepreciationMaterialItem
    {
        [Required]
        [Display(Name="Conta Cont�bil de Deprecia��o")]
        public int DepreciationAccount { get; set; }

        [Required]
        [Display(Name = "C�digo Item Material")]
        public int MaterialItemCode { get; set; }
    }
}
