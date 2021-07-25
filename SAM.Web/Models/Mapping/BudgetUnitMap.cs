using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class BudgetUnitMap : EntityTypeConfiguration<BudgetUnit>
    {
        public BudgetUnitMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(120);

            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("BudgetUnit");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InstitutionId).HasColumnName("InstitutionId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Direct).HasColumnName("Direct");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedInstitution )
                .WithMany(t => t.BudgetUnits)
                .HasForeignKey(d => d.InstitutionId);

        }
    }
}
