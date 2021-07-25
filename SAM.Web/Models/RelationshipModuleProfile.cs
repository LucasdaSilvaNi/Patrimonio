using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public partial class RelationshipModuleProfile
    {
        public int Id { get; set; }

        [Display(Name = "Módulo")]
        public int ModuleId { get; set; }

        [Display(Name = "Perfil")]
        public int ProfileId { get; set; }
        public bool Status { get; set; }

        public virtual Profile RelatedProfile { get; set; }
        public virtual Module RelatedModule { get; set; }
    }
}