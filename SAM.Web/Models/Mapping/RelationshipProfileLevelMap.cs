using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class RelationshipProfileLevelMap : EntityTypeConfiguration<RelationshipProfileLevel>
    {
        public RelationshipProfileLevelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RelationshipProfileLevel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProfileId).HasColumnName("ProfileId");
            this.Property(t => t.LevelId).HasColumnName("LevelId");

            // Relationships
            this.HasRequired(t => t.RelatedLevel)
                .WithMany(t => t.RelationshipProfilesLevels)
                .HasForeignKey(d => d.LevelId);
            this.HasRequired(t => t.RelatedProfile)
                .WithMany(t => t.RelationshipProfilesLevels)
                .HasForeignKey(d => d.ProfileId);

        }
    }
}
