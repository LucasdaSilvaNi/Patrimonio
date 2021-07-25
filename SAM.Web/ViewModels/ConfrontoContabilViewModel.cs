using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;




namespace SAM.Web.ViewModels
{
    public class ConfrontoContabilViewModel
    {
        [Display(Name = "Orgao")]
        [Required(ErrorMessage = "O campo Orgão é obrigatório")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        public int? BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        public int? ManagerUnitId { get; set; }

        [Display(Name = "Mês/ano referência")]
        public string MesRef { get; set; }
    }

    public class DadosSIAFEMContaContabilViewModel
    {
        //[Display(Name = "Conta Contábil")]
        public int Conta { get; set; }
        public string DescricaoConta { get; set; }

        [Display(Name = "Valor Contábil SAM")]
        public decimal ValorContabilSAM { get; set; }

        [Display(Name = "Valor Contábil SIAFEM")]
        public decimal ValorContabilSIAFEM { get; set; }
        public decimal Diferenca { get; set; }
        public string UGE { get; set; }
        public string UO { get; set; }
        [Required]
        public string Orgao { get; set; }
        public string Mes { get; set; }
        public string Ano { get; set; }
    }

}