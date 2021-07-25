using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class HistoricoCampo
    {
        public Int64 Id { get; set; }
        public Int64 HistoricoId { get; set; }
        public string Campo { get; set; }
        public string ValorAntigo { get; set; }
        public string ValorNovo { get; set; }

        public virtual Historico RelatedHistorico { get; set; }
    }
}