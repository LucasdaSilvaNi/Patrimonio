using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SAM.Web.Models.Mapping
{
    public class BPsExcluidosMap : EntityTypeConfiguration<BPsExcluidos>
    {
        public BPsExcluidosMap()
        {
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TipoIncorporacao)
                .IsRequired();

            this.Property(t => t.InitialId)
                .IsRequired();

            this.Property(t => t.SiglaInicial)
                .HasMaxLength(10);

            this.Property(t => t.Chapa)
                .IsRequired();

            this.Property(t => t.ItemMaterial)
                .IsRequired();

            this.Property(t => t.GrupoMaterial)
                .IsRequired();

            this.Property(t => t.StateConservationId)
                .IsRequired();

            this.Property(t => t.ManagerUnitId)
                .IsRequired();

            this.Property(t => t.Processo)
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("BPsExcluidos");
            this.Property(t => t.TipoIncorporacao).HasColumnName("TipoIncorporacao");
            this.Property(t => t.InitialId).HasColumnName("InitialId");
            this.Property(t => t.SiglaInicial).HasColumnName("SiglaInicial");
            this.Property(t => t.Chapa).HasColumnName("Chapa");
            this.Property(t => t.ItemMaterial).HasColumnName("ItemMaterial");
            this.Property(t => t.GrupoMaterial).HasColumnName("GrupoMaterial");
            this.Property(t => t.StateConservationId).HasColumnName("StateConservationId");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.AdministrativeUnitId).HasColumnName("AdministrativeUnitId");
            this.Property(t => t.ResponsibleId).HasColumnName("ResponsibleId");
            this.Property(t => t.Processo).HasColumnName("Processo");
            this.Property(t => t.ValorAquisicao).HasColumnName("ValorAquisicao");
            this.Property(t => t.DataAquisicao).HasColumnName("DataAquisicao");
            this.Property(t => t.DataIncorporacao).HasColumnName("DataIncorporacao");
            this.Property(t => t.flagAcervo).HasColumnName("flagAcervo");
            this.Property(t => t.flagTerceiro).HasColumnName("flagTerceiro");
            this.Property(t => t.flagDecretoSEFAZ).HasColumnName("flagDecretoSEFAZ");
            this.Property(t => t.DataAcao).HasColumnName("DataAcao");
            this.Property(t => t.LoginAcao).HasColumnName("LoginAcao");
            this.Property(t => t.NotaLancamento).HasColumnName("NotaLancamento");
            this.Property(t => t.NotaLancamentoEstorno).HasColumnName("NotaLancamentoEstorno");
            this.Property(t => t.NotaLancamentoDepreciacao).HasColumnName("NotaLancamentoDepreciacao");
            this.Property(t => t.NotaLancamentoDepreciacaoEstorno).HasColumnName("NotaLancamentoDepreciacaoEstorno");
            this.Property(t => t.NotaLancamentoReclassificacao).HasColumnName("NotaLancamentoReclassificacao");
            this.Property(t => t.NotaLancamentoReclassificacaoEstorno).HasColumnName("NotaLancamentoReclassificacaoEstorno");
            this.Property(t => t.Observacoes).HasColumnName("Observacoes");
            this.Property(t => t.FlagModoExclusao).HasColumnName("FlagModoExclusao");
            this.Property(t => t.MotivoExclusao).HasColumnName("MotivoExclusao");
            this.Property(t => t.NumeroDocumento).HasColumnName("NumeroDocumento");

        }

    }
}