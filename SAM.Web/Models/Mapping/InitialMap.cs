using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class InitialMap : EntityTypeConfiguration<Initial>
    {
        public InitialMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Description)
                .HasMaxLength(60);

            this.Property(t => t.BarCode)
                .HasMaxLength(2);

            // Table & Column Mappings
            this.ToTable("Initial");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.BarCode).HasColumnName("BarCode");
            this.Property(t => t.InstitutionId).HasColumnName("InstitutionId");
            this.Property(t => t.BudgetUnitId).HasColumnName("BudgetUnitId");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedInstitution)
                .WithMany(t => t.Initials)
                .HasForeignKey(d => d.InstitutionId);

            // Relationships
            this.HasOptional(t => t.RelatedBudgetUnit)
                .WithMany(t => t.Initials)
                .HasForeignKey(d => d.BudgetUnitId);

            // Relationships
            this.HasOptional(t => t.RelatedManagerUnit)
                .WithMany(t => t.Initials)
                .HasForeignKey(d => d.ManagerUnitId);

        }
    }
}
