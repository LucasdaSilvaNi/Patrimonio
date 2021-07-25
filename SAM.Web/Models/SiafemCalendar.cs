using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public partial class SiafemCalendar
    {
        //[Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ano é obrigatório")]
        [Display(Name = "Exercício")]
        public int? FiscalYear { get; set; }

        [Required(ErrorMessage = "Mês Referência é obrigatório")]
        [Display(Name = "Mes-Referência")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public int ReferenceMonth { get; set; }

        [Required(ErrorMessage = "Data é obrigatório")]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateClosing { get; set; }

        [Required(ErrorMessage = "Data é obrigatório")]
        [Display(Name = "Data de Fechamento para Operadores")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataParaOperadores { get; set; }

        public bool Status { get; set; }

    }
}

