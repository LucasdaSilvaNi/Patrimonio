using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Level
    {
        public Level()
        {
            this.Levels = new List<Level>();
            this.RelationshipProfilesLevels = new List<RelationshipProfileLevel>();
        }

        public int Id { get; set; }
        
        public Nullable<int> ParentId { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Level> Levels { get; set; }
        public virtual Level RelatedLevel { get; set; }
        public virtual ICollection<RelationshipProfileLevel> RelationshipProfilesLevels { get; set; }
    }
}
