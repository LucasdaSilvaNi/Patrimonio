using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Historico
    {
        public Historico()
        {
            this.HistoricoCampos = new List<HistoricoCampo>();
        }
        public Int64 Id { get; set; }
        public string Acao { get; set; }
        public string Tabela { get; set; }
        public int TabelaId { get; set; }
        public DateTime Data { get; set; }
        public string Login { get; set; }

        public virtual ICollection<HistoricoCampo> HistoricoCampos { get; set; }
    }
}