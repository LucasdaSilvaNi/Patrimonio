using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class Repair
    {
        public int Id { get; set; }
        public int InitialId { get; set; }
        public int AssetId { get; set; }
        public int NumberIdentification { get; set; }
        public Decimal PartNumberIdentification { get; set; }
        public DateTime DateOut { get; set; }
        public String Destination { get; set; }
        public DateTime? DateExpectedReturn { get; set; }
        public Decimal EstimatedCost { get; set; }
        public DateTime? ReturnDate { get; set; }
        public Decimal? FinalCost { get; set; }
        public String Reaseon { get; set; }
        public virtual Asset RelatedAssets { get; set; }
        public virtual Initial RelatedInitials { get; set; }
    }
}