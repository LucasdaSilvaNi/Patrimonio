using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public partial class HistoricoValoresDecreto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public decimal ValorAquisicao { get; set; }
        public decimal ValorRevalorizacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public int LoginId { get; set; }

        public virtual Asset RelatedAsset { get; set; }
        public virtual User RelatedUser { get; set; }
    }
}