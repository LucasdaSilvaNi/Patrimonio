using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MobileMap : EntityTypeConfiguration<Mobile>
    {
        public MobileMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);
            this.Property(t => t.Brand)
                .HasMaxLength(50);
            this.Property(t => t.Model)
                .HasMaxLength(50);
            this.Property(t => t.Brand)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Mobile");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Brand).HasColumnName("Brand");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.MacAddress).HasColumnName("MacAddress");
            this.Property(t => t.InstitutionId).HasColumnName("InstitutionId");
            this.Property(t => t.BudgetUnitId).HasColumnName("BudgetUnitId");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.AdministrativeUnitId).HasColumnName("AdministrativeUnitId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedInstitutions)
                .WithMany(t => t.Mobiles)
                .HasForeignKey(d => d.InstitutionId);

            this.HasRequired(t => t.RelatedBudgetUnits)
                .WithMany(t => t.Mobiles)
                .HasForeignKey(d => d.BudgetUnitId);

            this.HasRequired(t => t.RelatedManagerUnits)
                .WithMany(t => t.Mobiles)
                .HasForeignKey(d => d.ManagerUnitId);

            this.HasRequired(t => t.RelatedAdministrativeUnits)
                .WithMany(t => t.Mobiles)
                .HasForeignKey(d => d.AdministrativeUnitId);

        }
    }
}