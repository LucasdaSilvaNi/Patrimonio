using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            this.Assets = new List<Asset>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "CPF/CNPJ")]
        [StringLength(14, ErrorMessage = "Tamanho m?nimo de 11 e m?ximo de 14 caracteres.", MinimumLength = 11)]
        public string CPFCNPJ { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        public Nullable<int> AddressId { get; set; }

        [Display(Name = "Telefone")]
        public string Telephone { get; set; }

        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Dados Complementares")]
        public string AdditionalData { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }

        public virtual Address RelatedAddress { get; set; }
    }
}
