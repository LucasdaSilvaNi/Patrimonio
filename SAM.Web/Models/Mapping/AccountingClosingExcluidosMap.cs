using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AccountingClosingExcluidosMap : EntityTypeConfiguration<AccountingClosingExcluidos>
    {
        public AccountingClosingExcluidosMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("AccountingClosingExcluidos");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ManagerUnitCode).HasColumnName("ManagerUnitCode").IsRequired();
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ManagerUnitDescription).HasColumnName("ManagerUnitDescription").HasMaxLength(100);
            this.Property(t => t.DepreciationAccount).HasColumnName("DepreciationAccount");
            this.Property(t => t.DepreciationMonth).HasColumnName("DepreciationMonth");
            this.Property(t => t.AccumulatedDepreciation).HasColumnName("AccumulatedDepreciation");
            this.Property(t => t.DepreciationDescription).HasColumnName("DepreciationDescription").HasMaxLength(500);
            this.Property(t => t.ReferenceMonth).HasColumnName("ReferenceMonth").HasMaxLength(6);
            this.Property(t => t.ItemAccounts).HasColumnName("ItemAccounts");
            this.Property(t => t.AccountName).HasColumnName("AccountName").HasMaxLength(100);
            this.Property(t => t.GeneratedNL).HasColumnName("GeneratedNL").HasMaxLength(100);
            this.Property(t => t.ClosingId).HasColumnName("ClosingId");
            this.Property(t => t.ManagerCode).HasColumnName("ManagerCode").HasMaxLength(5);

            this.Property(t => t.AuditoriaIntegracaoId).HasColumnName("AuditoriaIntegracaoId");
            this.Property(t => t.NLEstorno).HasColumnName("NLEstorno");
            this.Property(t => t.AuditoriaIntegracaoIdEstorno).HasColumnName("AuditoriaIntegracaoIdEstorno");

            this.HasRequired(t => t.RelatedAuditoria)
                .WithMany(t => t.RelatedAccountingClosingsExcluidos)
                .HasForeignKey(d => d.AuditoriaIntegracaoId);

            this.HasRequired(t => t.RelatedAuditoriaEstorno)
                .WithMany(t => t.RelatedAccountingClosingsExcluidosEstorno)
                .HasForeignKey(d => d.AuditoriaIntegracaoIdEstorno);
        }
    }
}