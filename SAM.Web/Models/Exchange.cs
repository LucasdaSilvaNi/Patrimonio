using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class Exchange
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int InstitutionId { get; set; }
        public int BudgetUnitId { get; set; }
        public int ManagerUnitId { get; set; }
        public DateTime DateRequisition { get; set; }
        public DateTime? DateService { get; set; }
        public string Login { get; set; }
        public bool Status { get; set; }
        public virtual Asset RelatedAsset { get; set; } 
    }
}