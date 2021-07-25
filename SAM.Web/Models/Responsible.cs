using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Responsible
    {
        public Responsible()
        {
            this.AssetMovements = new List<AssetMovements>();
            this.Sections = new List<Section>();
            this.Inventarios = new List<Inventario>();
        }

        public int Id { get; set; }
        
        [Required]
        [Display(Name="Nome")]
        public string Name { get; set; }

        [Display(Name = "Cargo")]
        public string Position { get; set; }


        [Required]
        [StringLength(14, ErrorMessage = "Tamanho m�ximo de 14 caracteres.", MinimumLength = 11)]
        public string CPF { get; set; }

        [StringLength(100, ErrorMessage = "Tamanho m�ximo de 100 caracteres.")]
        public string Email { get; set; }

        public bool Status { get; set; }

        [Required]
        [Display(Name = "UA")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UA � obrigat�rio")]
        public int AdministrativeUnitId { get; set; }
        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual AdministrativeUnit RelatedAdministrativeUnits { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<Inventario> Inventarios { get; set; }
    }
}
