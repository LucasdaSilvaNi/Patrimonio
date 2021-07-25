using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class SupportStatusUser
    {
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Support> Supports { get; set; }
    }
}
