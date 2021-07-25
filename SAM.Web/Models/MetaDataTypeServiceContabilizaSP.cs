using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class MetaDataTypeServiceContabilizaSP
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_DateField { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_AccountingValueField { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_StockSource { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_StockDestination { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_MovementTypeContabilizaSP { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_SpecificControl { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_SpecificInputControl { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> RelatedEventSvcContabilizaSP_SpecificOutputControl { get; set; }

    }
}