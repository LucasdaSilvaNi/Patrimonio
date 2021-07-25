using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class AssetNumberIdentification
    {
        public long Id { get; set; }
        public string NumberIdentification { get; set; }
        public bool Status { get; set; }
        public int AssetId { get; set; }
        public string Login { get; set; }
        public DateTime InclusionDate { get; set; }
        public int? InitialId { get; set; }
        public string DiferenciacaoChapa { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ChapaCompleta { get; private set; }
        public virtual Asset RelatedAsset { get; set; }
        public virtual Initial RelatedInitial { get; set; }
    }
}