using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class DepreciationAccountViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [MaxLength(80)]
        [Display(Name = "Descrição para o ContabilizaSP")]
        public string DescricaoProContabilizaSP { get; set; }

        [Display(Name = "Conta Ativa?")]
        public bool Status { get; set; }

        [Display(Name = "Item Material do fechamento dessa conta")]
        public int ItemMaterialRelacionado { get; set; }
    }
}