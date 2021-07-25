using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class RelationshipProfileLevel
    {
        public int Id { get; set; }
        
        [Display(Name="Perfil")]
        public int ProfileId { get; set; }

        [Display(Name = "Nível")]
        public int LevelId { get; set; }
        public virtual Level RelatedLevel { get; set; }
        public virtual Profile RelatedProfile { get; set; }
    }
}
