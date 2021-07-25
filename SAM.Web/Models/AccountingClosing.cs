using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class AccountingClosing
    {
       public Int64 Id { get; set; }
       public int ManagerUnitCode { get; set; }
       public string ManagerUnitDescription { get; set; }
       public int? DepreciationAccount { get; set; }
       public decimal DepreciationMonth { get; set; }
       public decimal AccumulatedDepreciation { get; set; }
       public bool? Status { get; set; }
       public string DepreciationDescription { get; set; }
       public string ReferenceMonth { get; set; }
       public int? ItemAccounts { get; set; }
       public string AccountName { get; set; }
       public string GeneratedNL { get; set; }
       public Guid? ClosingId { get; set; }
       public string ManagerCode { get; set; }

        public int? AuditoriaIntegracaoId { get; set; }

        public virtual AuditoriaIntegracao RelatedAuditoria { get; set; }
    }
}