using Newtonsoft.Json;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class SupportType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Support> Supports { get; set; }
    }
}
