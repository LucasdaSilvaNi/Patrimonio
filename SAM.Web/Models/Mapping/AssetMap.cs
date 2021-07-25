using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AssetMap : EntityTypeConfiguration<Asset>
    {
        public AssetMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MovementTypeId)
                .IsRequired();

            this.Property(t => t.Status)
                .IsRequired();

            this.Property(t => t.NumberIdentification)
                .IsRequired();

            this.Property(t => t.SerialNumber)
                .HasMaxLength(50);

            this.Property(t => t.ChassiNumber)
                .HasMaxLength(20);

            this.Property(t => t.Brand)
                .HasMaxLength(20);

            this.Property(t => t.Model)
                .HasMaxLength(20);

            this.Property(t => t.NumberPlate)
                .HasMaxLength(8);

            this.Property(t => t.AdditionalDescription)
                .HasMaxLength(20);

            this.Property(t => t.DiferenciacaoChapa)
                .HasMaxLength(7).IsRequired();

            this.Property(t => t.DiferenciacaoChapaAntiga)
                .HasMaxLength(7);

            // Table & Column Mappings
            this.ToTable("Asset");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.MovementTypeId).HasColumnName("MovementTypeId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.InitialId).HasColumnName("InitialId");
            this.Property(t => t.NumberIdentification).HasColumnName("NumberIdentification");
            this.Property(t => t.AcquisitionDate).HasColumnName("AcquisitionDate");
            this.Property(t => t.ValueAcquisition).HasColumnName("ValueAcquisition");
            this.Property(t => t.ValueUpdate).HasColumnName("ValueUpdate");
            this.Property(t => t.MaterialItemCode).HasColumnName("MaterialItemCode");
            this.Property(t => t.MaterialItemDescription).HasColumnName("MaterialItemDescription");
            this.Property(t => t.MaterialGroupCode).HasColumnName("MaterialGroupCode");
            this.Property(t => t.LifeCycle).HasColumnName("LifeCycle");
            this.Property(t => t.RateDepreciationMonthly).HasColumnName("RateDepreciationMonthly");
            this.Property(t => t.ResidualValue).HasColumnName("ResidualValue");
            this.Property(t => t.AceleratedDepreciation).HasColumnName("AceleratedDepreciation");
            this.Property(t => t.Empenho).HasColumnName("Empenho");
            this.Property(t => t.ShortDescriptionItemId).HasColumnName("ShortDescriptionItemId");
            this.Property(t => t.OldInitial).HasColumnName("OldInitial");
            this.Property(t => t.OldNumberIdentification).HasColumnName("OldNumberIdentification");
            this.Property(t => t.NumberDoc).HasColumnName("NumberDoc").HasMaxLength(20);
            this.Property(t => t.MovimentDate).HasColumnName("MovimentDate");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.OutSourcedId).HasColumnName("OutSourcedId");
            this.Property(t => t.SupplierId).HasColumnName("SupplierId");
            this.Property(t => t.SerialNumber).HasColumnName("SerialNumber");
            this.Property(t => t.ManufactureDate).HasColumnName("ManufactureDate");
            this.Property(t => t.DateGuarantee).HasColumnName("DateGuarantee");
            this.Property(t => t.ChassiNumber).HasColumnName("ChassiNumber");
            this.Property(t => t.Brand).HasColumnName("Brand");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.NumberPlate).HasColumnName("NumberPlate");
            this.Property(t => t.AdditionalDescription).HasColumnName("AdditionalDescription");
            this.Property(t => t.flagVerificado).HasColumnName("flagVerificado");
            this.Property(t => t.flagDepreciaAcumulada).HasColumnName("flagDepreciaAcumulada");
            this.Property(t => t.InitialName).HasColumnName("InitialName");
            this.Property(t => t.ResidualValueCalc).HasColumnName("ResidualValueCalc");
            this.Property(t => t.MonthUsed).HasColumnName("MonthUsed");
            this.Property(t => t.DepreciationByMonth).HasColumnName("DepreciationByMonth").HasPrecision(18, 10);
            this.Property(t => t.DepreciationAccumulated).HasColumnName("DepreciationAccumulated");
            this.Property(t => t.flagCalculoPendente).HasColumnName("flagCalculoPendente");
            this.Property(t => t.flagAcervo).HasColumnName("flagAcervo");
            this.Property(t => t.flagTerceiro).HasColumnName("flagTerceiro");
            this.Property(t => t.flagDepreciationCompleted).HasColumnName("flagDepreciationCompleted");
			this.Property(t => t.flagAnimalNaoServico).HasColumnName("flagAnimalNaoServico");
            this.Property(t => t.ValorDesdobramento).HasColumnName("ValorDesdobramento");
            this.Property(t => t.flagDecreto).HasColumnName("flagDecreto");
            this.Property(t => t.CPFCNPJ).HasColumnName("CPFCNPJ");
            this.Property(t => t.Recalculo).HasColumnName("Recalculo");
            this.Property(t => t.AssetStartId).HasColumnName("AssetStartId");
            this.Property(t => t.DepreciationDateStart).HasColumnName("DepreciationDateStart");
            this.Property(t => t.DepreciationAmountStart).HasColumnName("DepreciationAmountStart");
            this.Property(t => t.flagVindoDoEstoque).HasColumnName("flagVindoDoEstoque");
            this.Property(t => t.IndicadorOrigemInventarioInicial).HasColumnName("IndicadorOrigemInventarioInicial");
            this.Property(t => t.DiferenciacaoChapa).HasColumnName("DiferenciacaoChapa");
            this.Property(t => t.ChapaCompleta).HasColumnName("ChapaCompleta");
            this.Property(t => t.DiferenciacaoChapaAntiga).HasColumnName("DiferenciacaoChapaAntiga");
            this.Property(t => t.ChapaAntigaCompleta).HasColumnName("ChapaAntigaCompleta");

            // Relationships
            this.HasRequired(t => t.RelatedMovementType)
                .WithMany(t => t.Assets)
                .HasForeignKey(d => d.MovementTypeId);

            this.HasRequired(t => t.RelatedInitial)
                .WithMany(t => t.Assets)
                .HasForeignKey(d => d.InitialId);

            this.HasRequired(t => t.RelatedManagerUnit)
                .WithMany(t => t.Assets)
                .HasForeignKey(d => d.ManagerUnitId);

            this.HasOptional(t => t.RelatedOutSourced)
                .WithMany(t => t.Assets)
                .HasForeignKey(d => d.OutSourcedId);

            this.HasRequired(t => t.RelatedShortDescriptionItem)
                .WithMany(t => t.Assets)
                .HasForeignKey(d => d.ShortDescriptionItemId);

            this.HasOptional(t => t.RelatedSupplier)
                .WithMany(t => t.Assets)
                .HasForeignKey(d => d.SupplierId);
        }
    }
}
