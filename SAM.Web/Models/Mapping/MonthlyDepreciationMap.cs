using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MonthlyDepreciationMap : EntityTypeConfiguration<MonthlyDepreciation>
    {
        public MonthlyDepreciationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.ToTable("MonthlyDepreciation");
            this.Property(p => p.Id).HasColumnName("Id").IsRequired() ;
            this.Property(t => t.AssetStartId).HasColumnName("AssetStartId").IsRequired();
            this.Property(p => p.ResidualValue).HasColumnName("NumberIdentification").IsRequired();
            this.Property(p => p.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(p => p.MaterialItemCode).HasColumnName("MaterialItemCode").IsRequired();
            this.Property(p => p.InitialId).HasColumnName("InitialId").IsRequired().IsRequired();
            this.Property(p => p.AcquisitionDate).HasColumnName("AcquisitionDate").IsRequired();
            this.Property(p => p.CurrentDate).HasColumnName("CurrentDate").IsRequired();
            this.Property(p => p.DateIncorporation).HasColumnName("DateIncorporation").IsRequired();
            this.Property(p => p.LifeCycle).HasColumnName("LifeCycle").IsRequired();
            this.Property(p => p.CurrentMonth).HasColumnName("CurrentMonth").IsRequired();
            this.Property(p => p.ValueAcquisition).HasColumnName("ValueAcquisition").HasPrecision(18, 2).IsRequired();
            this.Property(p => p.CurrentValue).HasColumnName("CurrentValue").HasPrecision(18, 2).IsRequired();
            this.Property(p => p.ResidualValue).HasColumnName("ResidualValue").HasPrecision(18, 2).IsRequired();
            this.Property(p => p.RateDepreciationMonthly).HasColumnName("RateDepreciationMonthly").HasPrecision(18, 2).IsRequired();
            this.Property(p => p.AccumulatedDepreciation).HasColumnName("AccumulatedDepreciation").HasPrecision(18, 2).IsRequired();
            this.Property(p => p.UnfoldingValue).HasColumnName("UnfoldingValue").HasPrecision(18, 2);
            this.Property(p => p.Decree).HasColumnName("Decree").IsRequired();
            this.Property(p => p.ManagerUnitTransferId).HasColumnName("ManagerUnitTransferId");
            this.Property(p => p.MonthlyDepreciationId).HasColumnName("MonthlyDepreciationId");
            this.Property(p => p.MesAbertoDoAceite).HasColumnName("MesAbertoDoAceite");
            this.Property(p => p.QtdLinhaRepetida).HasColumnName("QtdLinhaRepetida");

            // Relationships
            this.HasRequired(t => t.RelatedManagerUnit)
                .WithMany(t => t.MonthlyDepreciations)
                .HasForeignKey(d => d.ManagerUnitId);
        }
    }
}