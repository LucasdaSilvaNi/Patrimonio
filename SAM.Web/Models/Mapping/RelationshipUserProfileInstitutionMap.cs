using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class RelationshipUserProfileInstitutionMap : EntityTypeConfiguration<RelationshipUserProfileInstitution>
    {
        public RelationshipUserProfileInstitutionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            //this.Property(t => t.Id)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("RelationshipUserProfileInstitution");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RelationshipUserProfileId).HasColumnName("RelationshipUserProfileId");
            this.Property(t => t.InstitutionId).HasColumnName("InstitutionId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedInstitution)
                .WithMany(t => t.RelationshipUsersProfilesInstitutions)
                .HasForeignKey(d => d.InstitutionId);
            this.HasOptional(t => t.RelatedBudgetUnit)
                .WithMany(t => t.RelationshipUsersProfilesInstitutions)
                .HasForeignKey(d => d.BudgetUnitId);

            this.HasRequired(t => t.RelatedRelationshipUserProfile)
                .WithMany(t => t.RelationshipUsersProfilesInstitutions)
                .HasForeignKey(d => d.RelationshipUserProfileId);

        }
    }
}
