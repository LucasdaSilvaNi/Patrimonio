using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Address
    {
        public Address()
        {
            this.RelatedSections = new List<Section>();
            this.RelatedSuppliers = new List<Supplier>();
            this.RelatedManagers = new List<Manager>();
            this.RelatedOutSourced = new List<OutSourced>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Logradouro")]
        [StringLength(200, ErrorMessage = "Tamanho m�ximo de 200 caracteres.")]
        public string Street { get; set; }

        [Display(Name = "N�mero")]
        [StringLength(10, ErrorMessage = "Tamanho m�ximo de 10 caracteres.")]
        public string Number { get; set; }

        [Display(Name = "Complemento")]
        [StringLength(30, ErrorMessage = "Tamanho m�ximo de 30 caracteres.")]
        public string ComplementAddress { get; set; }

        [Display(Name = "Bairro")]
        [StringLength(50, ErrorMessage = "Tamanho m�ximo de 50 caracteres.")]
        public string District { get; set; }

        [Required]
        [Display(Name = "Cidade")]
        [StringLength(50, ErrorMessage = "Tamanho m�ximo de 50 caracteres.")]
        public string City { get; set; }

        [Required]
        [Display(Name = "UF")]
        [StringLength(2, ErrorMessage = "Tamanho m�ximo de 2 caracteres.", MinimumLength=2)]
        public string State { get; set; }

        [Required]
        [Display(Name = "CEP")]
        [StringLength(9, ErrorMessage = "Tamanho m�ximo de 9 caracteres.", MinimumLength = 8)]
        public string PostalCode { get; set; }

        public virtual ICollection<Section> RelatedSections { get; set; }
        public virtual ICollection<Supplier> RelatedSuppliers { get; set; }
        public virtual ICollection<Manager> RelatedManagers { get; set; }
        public virtual ICollection<OutSourced> RelatedOutSourced { get; set; }
    }
}
