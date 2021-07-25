using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class TransactionMap : EntityTypeConfiguration<Transaction>
    {
        public TransactionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Initial)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Path)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Transaction");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Initial).HasColumnName("Initial");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.TypeTransactionId).HasColumnName("TypeTransactionId");

            // Relationships
            this.HasRequired(t => t.RelatedModule)
                .WithMany(t => t.Transactions)
                .HasForeignKey(d => d.ModuleId);
            this.HasRequired(t => t.RelatedTypeTransaction)
                .WithMany(t => t.Transactions)
                .HasForeignKey(d => d.TypeTransactionId);
        }
    }
}
