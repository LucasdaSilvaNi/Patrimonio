using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class HistoricoDepreciacaoViewModel : MonthlyDepreciation
    {
        public string VidaUtil { get; set; }

        public string ValorAquisicao { get; set; }

        public string DepreciacaoAcumulada { get; set; }

        public string ValorAtual { get; set; }

        public string CodigoUGE { get; set; }

        public string DataDepreciacao { get; set; }
    }
}