using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public class DepreciationAccount
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [MaxLength(80)]
        [Display(Name = "Conta Depreciação ContabilizaSP")]
        public string DescricaoContaDepreciacaoContabilizaSP { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<AuxiliaryAccount> AuxiliaryAccount { get; set; }
    }
}