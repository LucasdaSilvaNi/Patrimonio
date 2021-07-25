using SAM.Web.Legados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class ImportarLegadoViewModel
    {

        [Required(ErrorMessage ="Selecione a base de dados de origem")]
        [ValidarDropListBox("Selecione a base de dados de origem")]
        public int IdOrigem { get; set; }
        [Display(Name = "Origem")]
        public String baseDeOrigem { get; set; }

        [Required(ErrorMessage = "Selecione a base de dados de destino")]
        [ValidarDropListBox("Selecione a base de dados de destino")]
        public int IdDestino { get; set; }
        [Display(Name = "Destino")]
        public String baseDeDestino { get; set; }

    }
}