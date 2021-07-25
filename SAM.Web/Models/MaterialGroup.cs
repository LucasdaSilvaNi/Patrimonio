using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class MaterialGroup
    {
        public MaterialGroup()
        {
            this.MaterialClasses = new List<MaterialClass>();
        }

        public int Id { get; set; }
        
        [Required]
        [Display(Name="Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public bool Status { get; set; }

        [Required]
        [Display(Name ="Vida útil (em meses)")]
        public int LifeCycle { get; set; }

        [Required]
        [Display(Name = "Tax. Depreciação Mensal (%)")]
        public decimal RateDepreciationMonthly { get; set; }

        [Required]
        [Display(Name = "Valor Residual (%)")]
        public decimal ResidualValue { get; set; }
        
        public virtual ICollection<MaterialClass> MaterialClasses { get; set; }

        public virtual ICollection<RelationshipAuxiliaryAccountItemGroup> RelationshipAuxiliaryAccountItemGroups { get; set; }
    }
}
