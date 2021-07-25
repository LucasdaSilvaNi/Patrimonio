using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class RelationshipTransactionProfileMap : EntityTypeConfiguration<RelationshipTransactionProfile>
    {
        public RelationshipTransactionProfileMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RelationshipTransactionProfile");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TransactionId).HasColumnName("TransactionId");
            this.Property(t => t.ProfileId).HasColumnName("ProfileId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedProfile)
                .WithMany(t => t.RelationshipTransactionsProfiles)
                .HasForeignKey(d => d.ProfileId);
            this.HasRequired(t => t.RelatedTransaction)
                .WithMany(t => t.RelationshipTransactionsProfiles)
                .HasForeignKey(d => d.TransactionId);

        }
    }
}
