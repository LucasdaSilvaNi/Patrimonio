using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    [NotMapped]
    public class DepreciacaoParametroModel
    {
        public int MaterialItemCode { get; set; }
        public int? AssetStartId { get; set; }
        public int AssetId { get; set; }
    }
}