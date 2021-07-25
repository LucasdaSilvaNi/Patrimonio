using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public partial class MetaDataTypeServiceContabilizaSPViewModel
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
    }
}