using System;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class RelatorioViewModel
    {
        [Required]
        [Display(Name = "Órgão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Órgão é obrigatório")]
        public int InstitutionId { get; set; }

        [Required]
        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO é obrigatório")]
        public int BudgetUnitId { get; set; }

        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int ManagerUnitId { get; set; }

        [Required]
        [Display(Name = "UA")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UA é obrigatório")]
        public Nullable<int> AdministrativeUnitId { get; set; }

        [Display(Name = "Divisão")]
        public Nullable<int> SectionId { get; set; }

        [Display(Name = "Mês Referência")]
        public string MesRef { get; set; }

        public bool Excel { get; set; }
    }

    public class ReportResumoInventarioViewModel : RelatorioViewModel
    {
        [Display(Name = "Resumo Consolidado de Contas Contábeis")]
        public bool ResumoConsolidado { get; set; }

        [Display(Name = "Aquisições (BP's) Mês-Referência Corrente")]
        public bool AquisicoesCorrentes { get; set; }

        [Display(Name = "Bens Totalmente Depreciados")]
        public bool BPTotalDepreciados { get; set; }

        [Display(Name = "Baixas (BP's) Mês-Referência Corrente")]
        public bool BaixasCorrentes { get; set; }

        [Display(Name = "Bens Patrimoniais de Acervo")]
        public bool Acervos { get; set; }

        [Display(Name = "Bens Patrimoniais de Terceiros")]
        public bool Terceiros { get; set; }
    }

    public class RelatorioInventarioFisicoViewModel : RelatorioViewModel
    {
        public string Agrupamento { get; set; }
        public bool checkRelatorio { get; set; }
    }

    public class RelatorioTermoDeResponsabilidadeViewModel : RelatorioViewModel
    {
        [Required(ErrorMessage = "O Campo Responsável é obrigatório")]
        [Display(Name = "Responsável")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Responsável é obrigatório")]
        public int? idResponsable { get; set; }
    }

}