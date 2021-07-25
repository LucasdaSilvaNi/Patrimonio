using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class IncorporacaoEmLoteViewModel : Asset
    {
    }

    [NotMapped]
    public class NumeroDocumentosIncorpEmLote
    {
        public string NumeroDeDocumento { get; set; }
        public string OrigemUGE { get; set; }
    }
}