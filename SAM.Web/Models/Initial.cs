using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Initial
    {
        public Initial()
        {
            this.Assets = new List<Asset>();
            this.Concerts = new List<Repair>();
            this.AssetNumberIdentifications = new List<AssetNumberIdentification>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Tamanho m�ximo de 10 caracteres.")]
        [Display(Name = "Sigla")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Descri��o")]
        public string Description { get; set; }

        [Display(Name = "C�digo de Barras")]
        [MaxLength(2, ErrorMessage = "Tamanho m�ximo de 02 caracteres.")]
        public string BarCode { get; set; }

        [Required]
        [Display(Name = "Org�o")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Org�o � obrigat�rio")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        public int? BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        public int? ManagerUnitId { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
        public virtual Institution RelatedInstitution { get; set; }
        public virtual BudgetUnit RelatedBudgetUnit { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        public virtual ICollection<Repair> Concerts { get; set; }
        public virtual ICollection<AssetNumberIdentification> AssetNumberIdentifications { get; set; }
    }
}