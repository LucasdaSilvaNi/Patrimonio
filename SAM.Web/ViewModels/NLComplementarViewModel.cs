using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    public class NLComplementarViewModel
    {
        [Display(Name = "Orgão")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        public int BudgetUnitId { get; set; }

        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int ManagerUnitId { get; set; }

        [Required]
        [Display(Name = "Conta de Depreciação")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Conta de Depreciação é obrigatório")]
        public int DepreciationAccountId { get; set; }

        [Required(ErrorMessage = "O valor de Depreciação Mensal é obrigatório")]
        [Display(Name = "Depreciação Mensal")]
        public decimal DepreciacaoMensal { get; set; }

        [Display(Name = "Depreciação Acumulada")]
        public decimal DepreciacaoAcumulada { get; set; }

        [Required(ErrorMessage = "Mês de referência é obrigatório")]
        public string MesRef { get; set; }

        [Required(ErrorMessage = "NL é de estorno (ou a menor)?")]
        public bool NLEstorno { get; set; }
    }
}