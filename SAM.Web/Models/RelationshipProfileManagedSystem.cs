using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class RelationshipProfileManagedSystem
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public int ManagedSystemId { get; set; }

        public virtual ManagedSystem ManagedSystem { get; set; }

        public virtual Profile Profile { get; set; }
    }
}