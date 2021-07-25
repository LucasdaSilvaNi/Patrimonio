using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class RepairViewModel
    {
        public int Id { get; set; }
        //[Required(ErrorMessage ="Sigla é obrigatório")]
        [Display(Name="Sigla")]
        public int InitialId { get; set; }
        [Required(ErrorMessage = "Não existe bem patrimonial com esse número.")]
        public int AssetId { get; set; }
        //[Required(ErrorMessage = "Número é obrigatório")]
        [Display(Name="Número")]
        public int NumberIdentification { get; set; }
        //[Required(ErrorMessage = "Desdobramento é obrigatório")]
        [Display(Name="Desdobramento")]
        public decimal PartNumberIdentification { get; set; }
        [Display(Name="Data de saída")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOut { get; set; }
        [Display(Name="Destino")]
        public String Destination { get; set; }
        [Display(Name="Data prev. para retorno")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateExpectedReturn { get; set; }
        [Display(Name="Custo previsto")]
        public Decimal EstimatedCost { get; set; }
        [Display(Name="Data de retorno")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ReturnDate { get; set; }
        [Display(Name="Custo final")]
        public Decimal? FinalCost { get; set; }
        [Display(Name="Motivo")]
        public String Reaseon { get; set; }
        public virtual Asset RelatedAssets { get; set; }
        public virtual Initial RelatedInitials { get; set; }
        public virtual List<Asset> Assets { get; set; }
        public List<Initial> Initials { get; set; }
        public string AssetSelecionados { get; set; }
        public string MaterialDescricao { get; set; }
        public int UaId { get; set; }
        public Decimal? ValorAtual { get; set; }
    }
}