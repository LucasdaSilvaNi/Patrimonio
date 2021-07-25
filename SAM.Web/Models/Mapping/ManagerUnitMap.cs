using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ManagerUnitMap : EntityTypeConfiguration<ManagerUnit>
    {
        public ManagerUnitMap()
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

            this.Property(t => t.MesRefInicioIntegracaoSIAFEM)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ManagerUnit");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.BudgetUnitId).HasColumnName("BudgetUnitId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.FlagIntegracaoSiafem).HasColumnName("FlagIntegracaoSiafem");
            this.Property(t => t.FlagTratarComoOrgao).HasColumnName("FlagTratarComoOrgao");
            this.Property(t => t.ManagmentUnit_YearMonthStart).HasColumnName("ManagmentUnit_YearMonthStart");
            this.Property(t => t.ManagmentUnit_YearMonthReference).HasColumnName("ManagmentUnit_YearMonthReference");
            this.Property(t => t.MesRefInicioIntegracaoSIAFEM).HasColumnName("MesRefInicioIntegracaoSIAFEM");

            // Relationships
            this.HasRequired(t => t.RelatedBudgetUnit)
                .WithMany(t => t.ManagerUnits)
                .HasForeignKey(d => d.BudgetUnitId);
        }
    }
}
