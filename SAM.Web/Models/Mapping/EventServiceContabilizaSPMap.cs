using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class EventServiceContabilizaSPMap : EntityTypeConfiguration<EventServiceContabilizaSP>
    {
        public EventServiceContabilizaSPMap() {
            this.HasKey(t => t.Id);

            this.Property(t => t.AccountEntryTypeId).IsRequired();
            this.Property(t => t.StockDescription).IsRequired();
            this.Property(t => t.StockType).IsRequired();
            this.Property(t => t.MaterialType).IsRequired();
            this.Property(t => t.InputOutputReclassificationDepreciationTypeCode).IsRequired();
            this.Property(t => t.TipoMovimento_SamPatrimonio).IsRequired();
            this.Property(t => t.MetaDataType_DateField).IsRequired();
            this.Property(t => t.TipoMovimentacao_ContabilizaSP).IsRequired();
            this.Property(t => t.DataAtivacaoTipoMovimentacao).IsRequired();

            this.ToTable("EventServiceContabilizaSP");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AccountEntryTypeId).HasColumnName("AccountEntryTypeId");
            this.Property(t => t.StockDescription).HasColumnName("StockDescription");
            this.Property(t => t.StockType).HasColumnName("StockType");
            this.Property(t => t.StockSource).HasColumnName("StockSource");
            this.Property(t => t.StockDestination).HasColumnName("StockDestination");
            this.Property(t => t.MaterialType).HasColumnName("MaterialType");
            this.Property(t => t.InputOutputReclassificationDepreciationTypeCode).HasColumnName("InputOutputReclassificationDepreciationTypeCode");
            this.Property(t => t.SpecificControl).HasColumnName("SpecificControl");
            this.Property(t => t.SpecificInputControl).HasColumnName("SpecificInputControl");
            this.Property(t => t.SpecificOutputControl).HasColumnName("SpecificOutputControl");
            this.Property(t => t.TipoMovimento_SamPatrimonio).HasColumnName("TipoMovimento_SamPatrimonio");
            this.Property(t => t.TipoMovimentacao_ContabilizaSP).HasColumnName("TipoMovimentacao_ContabilizaSP");

            this.Property(t => t.MetaDataType_DateField).HasColumnName("MetaDataType_DateField");
            this.Property(t => t.MetaDataType_AccountingValueField).HasColumnName("MetaDataType_AccountingValueField");
            this.Property(t => t.MetaDataType_StockSource).HasColumnName("MetaDataType_StockSource");
            this.Property(t => t.MetaDataType_StockDestination).HasColumnName("MetaDataType_StockDestination");
            this.Property(t => t.MetaDataType_MovementTypeContabilizaSP).HasColumnName("MetaDataType_MovementTypeContabilizaSP");

            this.Property(t => t.MetaDataType_SpecificControl).HasColumnName("MetaDataType_SpecificControl");
            this.Property(t => t.MetaDataType_SpecificInputControl).HasColumnName("MetaDataType_SpecificInputControl");
            this.Property(t => t.MetaDataType_SpecificOutputControl).HasColumnName("MetaDataType_SpecificOutputControl");
            this.Property(t => t.ExecutionOrder).HasColumnName("ExecutionOrder");
            this.Property(t => t.DataAtivacaoTipoMovimentacao).HasColumnName("DataAtivacaoTipoMovimentacao");
            this.Property(t => t.Status).HasColumnName("Status");

            this.Property(t => t.FavoreceDestino).HasColumnName("FavoreceDestino");
            this.Property(t => t.VerificaSeOrigemUtilizaSAM).HasColumnName("VerificaSeOrigemUtilizaSAM");
            this.Property(t => t.UtilizaTipoMovimentacaoContabilizaSPAlternativa).HasColumnName("UtilizaTipoMovimentacaoContabilizaSPAlternativa");
            this.Property(t => t.TipoMovimentacaoContabilizaSPAlternativa).HasColumnName("TipoMovimentacaoContabilizaSPAlternativa");

            // Relationships
            this.HasRequired(t => t.RelatedMovementType)
                .WithMany(t => t.EventServiceContabilizaSPs)
                .HasForeignKey(t => t.InputOutputReclassificationDepreciationTypeCode)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.RelatedMetaDataType_DateField)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_DateField)
                .HasForeignKey(t => t.MetaDataType_DateField)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.RelatedMetaDataType_AccountingValueField)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_AccountingValueField)
                .HasForeignKey(t => t.MetaDataType_AccountingValueField)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.RelatedMetaDataType_StockSource)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_StockSource)
                .HasForeignKey(t => t.MetaDataType_StockSource)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.RelatedMetaDataType_StockDestination)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_StockDestination)
                .HasForeignKey(t => t.MetaDataType_StockDestination)
                .WillCascadeOnDelete(false);


            this.HasRequired(t => t.RelatedMetaDataType_MovementTypeContabilizaSP)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_MovementTypeContabilizaSP)
                .HasForeignKey(t => t.MetaDataType_MovementTypeContabilizaSP)
                .WillCascadeOnDelete(false);


            #region Controles Especificos
            this.HasRequired(t => t.RelatedMetaDataType_SpecificControl)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_SpecificControl)
                .HasForeignKey(t => t.MetaDataType_SpecificControl)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.RelatedMetaDataType_SpecificInputControl)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_SpecificInputControl)
                .HasForeignKey(t => t.MetaDataType_SpecificInputControl)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.RelatedMetaDataType_SpecificOutputControl)
                .WithMany(t => t.RelatedEventSvcContabilizaSP_SpecificOutputControl)
                .HasForeignKey(t => t.MetaDataType_SpecificOutputControl)
                .WillCascadeOnDelete(false);
            #endregion Controles Especificos
        }
    }
}