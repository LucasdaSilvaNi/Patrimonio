using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ClosingMap : EntityTypeConfiguration<Closing>
    {
        public ClosingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Closing");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AssetId).HasColumnName("AssetId");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.ClosingYearMonthReference).HasColumnName("ClosingYearMonthReference");
            this.Property(t => t.CurrentPrice).HasColumnName("CurrentPrice");
            this.Property(t => t.DepreciationPrice).HasColumnName("DepreciationPrice");
            this.Property(t => t.MaterialItemCode).HasColumnName("MaterialItemCode");
            this.Property(t => t.AceleratedDepreciation).HasColumnName("AceleratedDepreciation");
            this.Property(t => t.MaterialGroupCode).HasColumnName("MaterialGroupCode");
            this.Property(t => t.LifeCycle).HasColumnName("LifeCycle");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.LoginID).HasColumnName("LoginID");
            this.Property(t => t.ClosingDate).HasColumnName("ClosingDate");
            this.Property(t => t.AssetMovementsId).HasColumnName("AssetMovementsId");
            this.Property(t => t.MonthUsed).HasColumnName("MonthUsed");
            this.Property(t => t.ResidualValueCalc).HasColumnName("ResidualValueCalc");
            this.Property(t => t.DepreciationAccumulated).HasColumnName("DepreciationAccumulated");
            this.Property(t => t.flagDepreciationCompleted).HasColumnName("flagDepreciationCompleted");
            this.Property(t => t.Recalculo).HasColumnName("Recalculo");

            // Relationships

        }
    }
}
