using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class RelationshipUserProfileMap : EntityTypeConfiguration<RelationshipUserProfile>
    {
        public RelationshipUserProfileMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RelationshipUserProfile");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("userId");
            this.Property(t => t.ProfileId).HasColumnName("ProfileId");
            this.Property(t => t.DefaultProfile).HasColumnName("DefaultProfile");
            this.Property(t => t.FlagResponsavel).HasColumnName("FlagResponsavel");

            // Relationships
            this.HasRequired(t => t.RelatedProfile)
                .WithMany(t => t.RelationShipUsersProfiles)
                .HasForeignKey(d => d.ProfileId);
            this.HasRequired(t => t.RelatedUser)
                .WithMany(t => t.RelationshipUsersProfiles)
                .HasForeignKey(d => d.UserId);

        }
    }
}
