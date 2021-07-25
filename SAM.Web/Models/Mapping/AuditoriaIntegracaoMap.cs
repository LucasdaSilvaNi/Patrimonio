using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AuditoriaIntegracaoMap : EntityTypeConfiguration<AuditoriaIntegracao>
    {
        public AuditoriaIntegracaoMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            //this.Property(t => t.AssetMovementId)
            //    .IsRequired();


            // Table & Column Mappings
            this.ToTable("AuditoriaIntegracao");
            this.Property(t => t.Id).HasColumnName("Id");
            //this.Property(t => t.AssetMovementId).HasColumnName("AssetMovementId");
            this.Property(t => t.DataEnvio).HasColumnName("DataEnvio");
            this.Property(t => t.DataRetorno).HasColumnName("DataRetorno");
            this.Property(t => t.MsgEstimuloWS).HasColumnName("MsgEstimuloWS");
            this.Property(t => t.MsgRetornoWS).HasColumnName("MsgRetornoWS");
            this.Property(t => t.NomeSistema).HasColumnName("NomeSistema");
            this.Property(t => t.UsuarioSAM).HasColumnName("UsuarioSAM");
            this.Property(t => t.UsuarioSistemaExterno).HasColumnName("UsuarioSistemaExterno");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.AssetMovementIds).HasColumnName("AssetMovementIds");
            /* Campos do XML Envio */
            this.Property(t => t.DocumentoId).HasColumnName("DocumentoId");
            this.Property(t => t.TipoMovimento).HasColumnName("TipoMovimento");
            this.Property(t => t.Data).HasColumnName("Data");
            this.Property(t => t.UgeOrigem).HasColumnName("UgeOrigem");
            this.Property(t => t.Gestao).HasColumnName("Gestao");
            this.Property(t => t.Tipo_Entrada_Saida_Reclassificacao_Depreciacao).HasColumnName("Tipo_Entrada_Saida_Reclassificacao_Depreciacao");
            this.Property(t => t.CpfCnpjUgeFavorecida).HasColumnName("CpfCnpjUgeFavorecida");
            this.Property(t => t.GestaoFavorecida).HasColumnName("GestaoFavorecida");
            this.Property(t => t.Item).HasColumnName("Item");
            this.Property(t => t.TipoEstoque).HasColumnName("TipoEstoque");
            this.Property(t => t.Estoque).HasColumnName("Estoque");
            this.Property(t => t.EstoqueDestino).HasColumnName("EstoqueDestino");
            this.Property(t => t.EstoqueOrigem).HasColumnName("EstoqueOrigem");
            this.Property(t => t.TipoMovimentacao).HasColumnName("TipoMovimentacao");
            this.Property(t => t.ValorTotal).HasColumnName("ValorTotal");
            this.Property(t => t.ControleEspecifico).HasColumnName("ControleEspecifico");
            this.Property(t => t.ControleEspecificoEntrada).HasColumnName("ControleEspecificoEntrada");
            this.Property(t => t.ControleEspecificoSaida).HasColumnName("ControleEspecificoSaida");
            this.Property(t => t.FonteRecurso).HasColumnName("FonteRecurso");
            this.Property(t => t.NLEstorno).HasColumnName("NLEstorno");
            this.Property(t => t.Empenho).HasColumnName("Empenho");
            this.Property(t => t.Observacao).HasColumnName("Observacao");
            this.Property(t => t.NotaFiscal).HasColumnName("NotaFiscal");
            this.Property(t => t.ItemMaterial).HasColumnName("ItemMaterial");
            /* Campos XML Retorno */
            this.Property(t => t.NotaLancamento).HasColumnName("NotaLancamento");
            this.Property(t => t.MsgErro).HasColumnName("MsgErro");


            // Relationships
            this.HasRequired(t => t.RelatedManagerUnit)
                .WithMany(t => t.AuditoriaIntegracoes)
                .HasForeignKey(d => d.ManagerUnitId);
        }
    }
}