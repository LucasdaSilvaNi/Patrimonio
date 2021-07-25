using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class HistoricoValoresDecretoViewModel
    {
        public decimal ValorAquisicao { get; set; }
        public decimal ValorRevalorizacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string textoValorAquisicao { get; set; }
        public string textoValorRevalorizacao { get; set; }
        public string textoDataAlteracao { get; set; }
    }
}