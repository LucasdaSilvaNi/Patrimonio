using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class HistoricSupport
    {
        public int Id { get; set; }
        public int SupportId { get; set; }
        public int UserId { get; set; }
        public DateTime InclusionDate { get; set; }
        public string Observation { get; set; }
        public bool Status { get; set; }

        public Support RelatedSupport { get; set; }
        public User RelatedUser { get; set; }
    }
}
