using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class OutSourced
    {
        public OutSourced()
        {
            this.Assets = new List<Asset>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name="CPF/CNPJ")]
        [StringLength(14, ErrorMessage = "Tamanho m�nimo de 11 e m�ximo de 14 caracteres.", MinimumLength = 11)]
        public string CPFCNPJ { get; set; }

        public Nullable<int> AddressId { get; set; }

        [Display(Name = "Telefone")]
        public string Telephone { get; set; }

        public bool Status { get; set; }

        [Display(Name = "�rg�o")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo �rg�o � obrigat�rio")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO � obrigat�rio")]
        public int BudgetUnitId { get; set; }

        public virtual Institution RelatedInstitution { get; set; }
        public virtual BudgetUnit RelatedBudgetUnit { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual Address RelatedAddress { get; set; }
    }
}
