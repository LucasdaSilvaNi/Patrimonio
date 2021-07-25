using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class SaldoContabilAtualUGEContaDepreciacao
    {
        public int Id { get; set; }
	    public int IdOrgao { get; set; }
        public int IdUO { get; set; }
        public int IdUGE { get; set; }
        public string CodigoOrgao { get; set; }
        public string  CodigoUO { get; set; }
        public string  CodigoUGE { get; set; }
	    public int DepreciationAccountId { get; set; }
	    public int ContaDepreciacao { get; set; }
	    public decimal DepreciacaoAcumulada { get; set; }
    }
}