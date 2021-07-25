using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AssetMovementsMap : EntityTypeConfiguration<AssetMovements>
    {
        public AssetMovementsMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.InstitutionId)
                .IsRequired();

            this.Property(t => t.Observation)
                .IsOptional(); this.Property(t => t.Observation);

            // Table & Column Mappings
            this.ToTable("AssetMovements");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AssetId).HasColumnName("AssetId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.MovimentDate).HasColumnName("MovimentDate");
            this.Property(t => t.MovementTypeId).HasColumnName("MovementTypeId");
            this.Property(t => t.StateConservationId).HasColumnName("StateConservationId");
            this.Property(t => t.NumberPurchaseProcess).HasColumnName("NumberPurchaseProcess");
            this.Property(t => t.InstitutionId).HasColumnName("InstitutionId");
            this.Property(t => t.BudgetUnitId).HasColumnName("BudgetUnitId");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.AdministrativeUnitId).HasColumnName("AdministrativeUnitId");
            this.Property(t => t.SectionId).HasColumnName("SectionId");
            this.Property(t => t.AuxiliaryAccountId).HasColumnName("AuxiliaryAccountId");
            this.Property(t => t.ResponsibleId).HasColumnName("ResponsibleId");
            this.Property(t => t.SourceDestiny_ManagerUnitId).HasColumnName("SourceDestiny_ManagerUnitId");
            this.Property(t => t.AssetTransferenciaId).HasColumnName("AssetTransferenciaId");
            this.Property(t => t.ExchangeId).HasColumnName("ExchangeId");
            this.Property(t => t.ExchangeDate).HasColumnName("ExchangeDate");
            this.Property(t => t.ExchangeUserId).HasColumnName("ExchangeUserId");
            this.Property(t => t.TypeDocumentOutId).HasColumnName("TypeDocumentOutId");
            this.Property(t => t.Observation).HasColumnName("Observation");
            this.Property(t => t.FlagEstorno).HasColumnName("FlagEstorno");
            this.Property(t => t.DataEstorno).HasColumnName("DataEstorno");
            this.Property(t => t.LoginEstorno).HasColumnName("LoginEstorno");
            this.Property(t => t.RepairValue).HasColumnName("RepairValue");
            this.Property(t => t.Login).HasColumnName("Login");
            this.Property(t => t.DataLogin).HasColumnName("DataLogin");
            this.Property(t => t.flagUGENaoUtilizada).HasColumnName("flagUGENaoUtilizada");
            this.Property(t => t.CPFCNPJ).HasColumnName("CPFCNPJ");
            //this.Property(t => t.NumeroNL).HasColumnName("NumeroNL");
            this.Property(t => t.MonthlyDepreciationId).HasColumnName("MonthlyDepreciationId");

            //Integracao ContabilizaSP
            this.Property(t => t.NotaLancamento).HasColumnName("NotaLancamento");
            this.Property(t => t.NotaLancamentoEstorno).HasColumnName("NotaLancamentoEstorno");
            this.Property(t => t.NotaLancamentoDepreciacao).HasColumnName("NotaLancamentoDepreciacao");
            this.Property(t => t.NotaLancamentoDepreciacaoEstorno).HasColumnName("NotaLancamentoDepreciacaoEstorno");
            this.Property(t => t.AuditoriaIntegracaoId).HasColumnName("AuditoriaIntegracaoId");
            this.Property(t => t.NotaLancamentoPendenteSIAFEMId).HasColumnName("NotaLancamentoPendenteSIAFEMId");
            this.Property(t => t.ContaContabilAntesDeVirarInservivel).HasColumnName("ContaContabilAntesDeVirarInservivel");
            this.Property(t => t.AuxiliaryMovementTypeId).HasColumnName("AuxiliaryMovementTypeId");

            // Relationships
            this.HasOptional(t => t.RelatedAdministrativeUnit)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.AdministrativeUnitId);

            this.HasRequired(t => t.RelatedAssets)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.AssetId);

            this.HasOptional(t => t.RelatedAuxiliaryAccount)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.AuxiliaryAccountId);

            this.HasRequired(t => t.RelatedBudgetUnit)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.BudgetUnitId);

            this.HasRequired(t => t.RelatedInstitution)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.InstitutionId);

            this.HasRequired(t => t.RelatedManagerUnit)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.ManagerUnitId);

            this.HasRequired(t => t.RelatedMovementType)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.MovementTypeId);

            this.HasOptional(t => t.RelatedResponsible)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.ResponsibleId);

            this.HasOptional(t => t.RelatedSection)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.SectionId);

            this.HasOptional(t => t.RelatedSourceDestinyManagerUnit)
                .WithMany(t => t.SourceDestiny_AssetMovements)
                .HasForeignKey(d => d.SourceDestiny_ManagerUnitId);

            this.HasOptional(t => t.RelatedUser)
                .WithMany(t => t.AssetMovements)
                .HasForeignKey(d => d.LoginEstorno);

            this.HasOptional(t => t.RelatedAuditoriaIntegracao)
                .WithMany(t => t.RelatedAssetMovements)
                .HasForeignKey(d => d.AuditoriaIntegracaoId);

            //TODO Verificar como amarrar via BD/EF6 tabelas NotaLancamentoPendenteSIAFEM/AssetMovements
            //this.HasOptional(t => t.RelatedNotaLancamentoPendenteSIAFEM)
            //    .WithMany(t => t.RelatedAssetMovements)
            //    .HasForeignKey(d => d.NotaLancamentoPendenteSIAFEMId);

        }
    }
}


