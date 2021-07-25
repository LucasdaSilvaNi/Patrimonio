using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.Models
{
    public class Inventario
    {
        public Inventario()
        {
            this.ItemInventarios = new List<ItemInventario>();
        }
        public int? Id { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Display(Name = "Data do Inventário")]
        public DateTime DataInventario { get; set; }

        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int UgeId { get; set; }

        [Required]
        [Display(Name = "Unidade Administrativa")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UA é obrigatório")]
        public int UaId { get; set; }

        [Display(Name = "Divisão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Divisão é obrigatório")]
        public int? DivisaoId { get; set; }

        [Display(Name = "Usuário Responsável")]
        public int? UserId { get; set; }

        [Required]
        [Display(Name = "Responsável")]
        public int ResponsavelId { get; set; }

        [Display(Name = "Responsável Legal")]
        public string ResponsavelLegal { get { return ( (RelatedResponsible != null) ? (String.Format("{0} - {1}", (String.IsNullOrWhiteSpace(RelatedResponsible.CPF) ?  "000.000.000-00" : RelatedResponsible.CPF), RelatedResponsible.Name)) : string.Empty); } }

        [Display(Name = "Usuário")]
        public string Usuario { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public string OrigemDados { get; set; }

        [NotMapped]
        [Display(Name = "Dados UA")]
        public string DadosUA { get { return ((RelatedAdministrativeUnit != null) ? String.Format("{0} - {1}", RelatedAdministrativeUnit.Code, RelatedAdministrativeUnit.Description) : null); } }

        public int? TipoInventario { get; set; }

        [NotMapped]
        [Display(Name = "Qtde. BPs")]
        public int QtdeBPs { get { return ((ItemInventarios != null) ? ItemInventarios.Count : 0); } }

        public virtual ICollection<ItemInventario> ItemInventarios { get; set; }
        public virtual Section RelatedSection { get; set; }
        public virtual Responsible RelatedResponsible { get; set; }
        public virtual User RelatedUser { get; set; }
        public virtual AdministrativeUnit RelatedAdministrativeUnit { get; set; }
    }
}