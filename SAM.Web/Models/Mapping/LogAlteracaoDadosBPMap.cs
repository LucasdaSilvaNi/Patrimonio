using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class LogAlteracaoDadosBPMap : EntityTypeConfiguration<LogAlteracaoDadosBP>
    {
        public LogAlteracaoDadosBPMap()
        {
            this.ToTable("LogAlteracaoDadosBP");
            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("Id");
            this.Property(p => p.AssetId).HasColumnName("AssetId");
            this.Property(p => p.AssetMovementId).HasColumnName("AssetMovementId");
            this.Property(p => p.Campo).HasColumnName("Campo").HasMaxLength(30);
            this.Property(p => p.ValorAntigo).HasColumnName("ValorAntigo");
            this.Property(p => p.ValorNovo).HasColumnName("ValorNovo");
            this.Property(p => p.UserId).HasColumnName("UserId");
            this.Property(p => p.DataHora).HasColumnName("DataHora");

            this.HasRequired(l => l.RelatedAsset)
                .WithMany(a => a.LogAlteracaoDadosBPs)
                .HasForeignKey(l => l.AssetId);

            this.HasRequired(l => l.RelatedUser)
                .WithMany(a => a.LogAlteracaoDadosBPs)
                .HasForeignKey(l => l.UserId);

            this.HasOptional(l => l.RelatedAssetMovements)
                .WithMany(am => am.LogAlteracaoDadosBPs)
                .HasForeignKey(l => l.AssetMovementId);
        }
    }
}