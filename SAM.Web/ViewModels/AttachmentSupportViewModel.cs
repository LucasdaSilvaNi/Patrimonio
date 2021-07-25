using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public partial class AttachmentSupportViewModel
    {
        public int Id { get; set; }
        public int SupportId { get; set; }
        public byte[] File { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public DateTime InclusionDate { get; set; }
        public bool Status { get; set; }
        public bool JaGravadoNoBanco { get; set; }
    }
}