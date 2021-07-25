using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SAM.Web.Models.Mapping
{
    public class RelationshipModuleProfileMap : EntityTypeConfiguration<RelationshipModuleProfile>
    {
        public RelationshipModuleProfileMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RelationshipModuleProfile");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.ProfileId).HasColumnName("ProfileId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedProfile)
                .WithMany(t => t.RelationShipModulesProfiles)
                .HasForeignKey(d => d.ProfileId);
            this.HasRequired(t => t.RelatedModule)
                .WithMany(t => t.RelationshipModulesProfiles)
                .HasForeignKey(d => d.ModuleId);
        }
    }
}