using System;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class ReclassificacaoContabilViewModel
    {
        [Required]
        [Display(Name = "Órgão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Favor informar o órgão")]
        public int InstitutionId { get; set; }

        [Required]
        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Favor informar a UO")]
        public int BudgetUnitId { get; set; }

        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Favor informar a UGE")]
        public int? ManagerUnitId { get; set; }

        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public string Descricao { get; set; }
        public string ContaContabilAtual { get; set; }
    }

    public class ReclassificaoEscolherContaViewModel {
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public int GrupoMaterial { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public int IdUGE { get; set; }
        public int IdContaContabil { get; set; }
        public string DescricaoContaContabil { get; set; }
        public bool GrupoPossuiMaisDeUmaConta { get; set; }
        public decimal ValorDeAquisicao { get; set; }
        public decimal DepreciacaoAcmumulada { get; set; }
        public int? AssetStartId { get; set; }
        public int MaterialItemCode { get; set; }
        public int AssetId { get; set; }
        public decimal totalValorAquisicao { get; set; }
        public decimal totalDepreciacaoAcumulada { get; set; }
    }
}