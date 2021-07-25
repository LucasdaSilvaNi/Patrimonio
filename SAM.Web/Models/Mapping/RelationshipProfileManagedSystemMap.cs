using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SAM.Web.Models.Mapping
{
    public class RelationshipProfileManagedSystemMap:EntityTypeConfiguration<RelationshipProfileManagedSystem>
    {
        public RelationshipProfileManagedSystemMap()
        {
            this.HasKey(h=> h.Id);
            this.Property(p => p.Id).HasColumnName("Id");
            this.ToTable("RelationshipProfileManagedSystem");
        }
    }
}