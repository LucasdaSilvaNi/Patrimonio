using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class AttachmentSupport
    {
        public int Id { get; set; }
        public int SupportId { get; set; }
        public byte[] File { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public DateTime InclusionDate { get; set; }
        public bool Status { get; set; }
        public virtual Support RelatedSupport { get; set; }
    }
}
