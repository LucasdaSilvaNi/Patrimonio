using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Section
    {
        public Section()
        {
            this.AssetMovements = new List<AssetMovements>();
            this.Inventarios = new List<Inventario>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Unidade Administrativa")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UA é obrigatório")]
        public int AdministrativeUnitId { get; set; }

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Somente números")]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        public Nullable<int> AddressId { get; set; }

        [Display(Name = "Telefone")]
        public string Telephone { get; set; }

        public bool Status { get; set; }

        [Display(Name = "Responsável")]
        public int? ResponsibleId { get; set; }

        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual Address RelatedAddress { get; set; }
        public virtual Responsible RelatedResponsible { get; set; }
        public virtual AdministrativeUnit RelatedAdministrativeUnit { get; set; }
        public virtual ICollection<Inventario> Inventarios { get; set; }
    }
}
