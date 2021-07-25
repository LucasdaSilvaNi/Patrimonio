using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class NotaLancamentoPendenteSIAFEMMap : EntityTypeConfiguration<NotaLancamentoPendenteSIAFEM>
    {
        public NotaLancamentoPendenteSIAFEMMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("NotaLancamentoPendenteSIAFEM");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.AssetMovementIds).HasColumnName("AssetMovementIds");
            this.Property(t => t.AuditoriaIntegracaoId).HasColumnName("AuditoriaIntegracaoId");
            this.Property(t => t.NumeroDocumentoSAM).HasColumnName("NumeroDocumentoSAM");
            this.Property(t => t.DataHoraEnvioMsgWS).HasColumnName("DataHoraEnvioMsgWS");
            this.Property(t => t.ErroProcessamentoMsgWS).HasColumnName("ErroProcessamentoMsgWS");
            this.Property(t => t.DataHoraReenvioMsgWS).HasColumnName("DataHoraReenvioMsgWS");
            this.Property(t => t.TipoNotaPendencia).HasColumnName("TipoNotaPendencia");
            this.Property(t => t.StatusPendencia).HasColumnName("StatusPendencia");


            // Relationships
            this.HasRequired(t => t.RelatedAuditoriaIntegracao)
                .WithMany(t => t.NotaLancamentoPendenteSIAFEMs)
                .HasForeignKey(d => d.AuditoriaIntegracaoId);
        }
    }
}