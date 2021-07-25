using SAM.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public partial class MaterialItemViewModel : MaterialItem
    {       
        [Display(Name = "Vida útil (em meses)")]
        public int? LifeCycle { get; set; }
        [Display(Name = "Tax. Depreciação Mensal (%)")]
        public decimal? RateDepreciationMonthly { get; set; }
        [Display(Name = "Valor Residual (%)")]
        public decimal? ResidualValue { get; set; }
        public string MaterialGroupDescription { get; set; }
        public int MaterialGroupCode { get; set; }
        public string MaterialItemCode { get; set; }

        public string Material { get; set; }
        public string Natureza1 { get; set; }
        public string Natureza2 { get; set; }
        public string Natureza3 { get; set; }

    }
}