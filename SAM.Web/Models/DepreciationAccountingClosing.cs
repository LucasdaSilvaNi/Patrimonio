using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public partial class DepreciationAccountingClosing
    {
        public int Id { get; set; }
	    public int AccountingClosingId { get; set; }
        public int BookAccount { get; set; }
        public decimal AccountingValue { get; set; }
        public decimal DepreciationMonth { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public bool Status { get; set; }
        public string AccountingDescription { get; set; }
        public string ReferenceMonth { get; set; }
        public int ManagerUnitCode { get; set; }
        public string ManagerDescription { get; set; }
    }
}