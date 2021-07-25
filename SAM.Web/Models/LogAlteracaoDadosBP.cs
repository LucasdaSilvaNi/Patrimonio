using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class LogAlteracaoDadosBP
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int? AssetMovementId { get; set; }
        public string Campo { get; set; }
        public string ValorAntigo { get; set; }
        public string ValorNovo { get; set; }
        public int UserId { get; set; }
        public DateTime DataHora { get; set; }

        public virtual Asset RelatedAsset { get; set; }
        public virtual AssetMovements RelatedAssetMovements { get; set; }
        public virtual User RelatedUser { get; set; }
    }
}