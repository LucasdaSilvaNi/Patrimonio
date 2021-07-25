using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Closing
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int ManagerUnitId { get; set; }
        public string ClosingYearMonthReference { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal DepreciationPrice { get; set; }
        public int MaterialItemCode { get; set; }
        public int AceleratedDepreciation { get; set; }
        public int MaterialGroupCode { get; set; }
        public int LifeCycle { get; set; }
        public bool Status { get; set; }
        public int LoginID { get; set; }
        public DateTime ClosingDate { get; set; }
        public Nullable<int> AssetMovementsId { get; set; }
        public Nullable<int> MonthUsed { get; set; }
        public double ResidualValueCalc { get; set; }
        public double DepreciationAccumulated { get; set; }
        public bool? flagDepreciationCompleted { get; set; }
        public bool? Recalculo { get; set; }

        public virtual AssetMovements RelatedAssetMovements { get; set; }

        public virtual Asset RelatedAsset { get; set; }
    }
}
