using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Manager
    {
        public Manager()
        {
            this.MaterialSubItems = new List<MaterialSubItem>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Nome Reduzido")]
        public string ShortName { get; set; }

        [Display(Name = "Endereço")]
        public int AddressId { get; set; }

        [Display(Name = "Telefone")]
        public string Telephone { get; set; }

        [Display(Name = "Código")]
        public string Code { get; set; }

        [Display(Name = "Imagem")]
        public byte[] Image { get; set; }

        public bool Status { get; set; }

        public virtual Address RelatedAddress { get; set; }
        public virtual ICollection<MaterialSubItem> MaterialSubItems { get; set; }
    }
}
