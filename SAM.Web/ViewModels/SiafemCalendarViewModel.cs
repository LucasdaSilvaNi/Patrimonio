using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class SiafemCalendarViewModel
    {

        public int Id { get; set; }

        //[Required(ErrorMessage = "Ano é obrigatório")]
        [Required]
        [Display(Name = "Ano-Exercício")]
        public int? FiscalYear { get; set; }

        //[Required(ErrorMessage = "Mês Referência é obrigatório")]
        //[Display(Name = "Mes-Referência")]
        [Required]
        [Display(Name = "Mês-Referência")]
        public string ReferenceMonth { get; set; }

        //[Required(ErrorMessage = "Data é obrigatório")]
        [Required]
        public int Month { get; set; }

        [Required]
        [Display(Name = "Data de Fechamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateClosing { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        //[DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]

        [Required]
        [Display(Name = "Data de Fechamento para os Operadores")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataParaOperadores { get; set; }

        public bool Status { get; set; }

        public int InstitutionId { get; set; }

       //public int ManagerUnitId { get; set; }


    }
}