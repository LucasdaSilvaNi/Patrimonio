using SAM.Web.Legados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class ExportarLegadoViewModel
    {

        [Required(ErrorMessage = "Selecione a base de dados")]
        [ValidarDropListBox("Selecione a base de dados")]
        public int IdBase { get; set; }
        [Display(Name = "Base de Dados")]
        public String baseNome { get; set; }

        [ValidarDropListBox("Selecione o orgão")]
        public int IdOrgao { get; set; }
        [Display(Name = "Orgão")]
        public String orgaoNome { get; set; }
        [Display(Name = "Selecione a planilha Excel")]
        public HttpPostedFileBase PostFile { get; set; }
    }
}