using PatrimonioBusiness.integracao.entidades;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace PatrimonioBusiness.integracao.maps
{
    internal class MovimentoIntegracaoMap:EntityTypeConfiguration<MovimentoIntegracao>
    {
        private MovimentoIntegracaoMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity).HasColumnName("TB_MOVIMENTO_INTEGRACAO_ID");
            // Properties
            //this.Property(t => t.MovimentoId).IsRequired().HasColumnName("TB_MOVIMENTO_ID");
            //this.Property(t => t.MovimentoItemId).IsRequired().HasColumnName("TB_MOVIMENTO_ITEM_ID");
            this.Property(t => t.TipoMovimentoId).IsRequired().HasColumnName("TB_TIPO_MOVIMENTO_ID");
           // this.Property(t => t.UgeId).HasColumnName("TB_UGE_ID");
           // this.Property(t => t.AlmoxarifadoId).HasColumnName("TB_ALMOXARIFADO_ID");
            //this.Property(t => t.ItemMaterialId).IsRequired().HasColumnName("TB_ITEM_MATERIAL_ID");
           // this.Property(t => t.SubItemMaterialId).HasColumnName("TB_SUBITEM_MATERIAL_ID");
            this.Property(t => t.NumeroDocumento).HasMaxLength(14).HasColumnName("TB_MOVIMENTO_NUMERO_DOCUMENTO");
            this.Property(t => t.AnoMesReferencia).HasMaxLength(6).HasColumnName("TB_MOVIMENTO_ANO_MES_REFERENCIA");
            this.Property(t => t.Empenho).HasMaxLength(11).HasColumnName("TB_MOVIMENTO_EMPENHO");
            this.Property(t => t.Obeservacoes).HasMaxLength(1000).HasColumnName("TB_MOVIMENTO_OBSERVACOES");
            this.Property(t => t.AlmoxarifadoOrigemDestinoId).HasColumnName("TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO");
            this.Property(t => t.DataMovimento).IsRequired().HasColumnName("TB_MOVIMENTO_DATA_MOVIMENTO");
            this.Property(t => t.DataOperacao).IsRequired().HasColumnName("TB_MOVIMENTO_DATA_OPERACAO");
            this.Property(t => t.QuantidadeMovimentada).IsRequired().HasColumnName("TB_MOVIMENTO_ITEM_QTDE_MOV");
            this.Property(t => t.ValorUnitario).IsRequired().HasColumnName("TB_MOVIMENTO_ITEM_VALOR_UNIT_EMP");
            this.Property(t => t.Ativo).IsRequired().HasColumnName("TB_MOVIMENTO_ATIVO");
            this.Property(t => t.ItemAtivo).IsRequired().HasColumnName("TB_MOVIMENTO_ITEM_ATIVO");
            this.Property(t => t.IntegracaoRealizado).HasColumnName("TB_INTEGRACAO_REALIZADO");
            this.Property(t => t.DataInclusao).IsRequired().HasColumnName("TB_DATA_INCLUSAO");
            this.Property(t => t.DataAlteracao).HasColumnName("TB_DATA_ALTERACAO");
            //this.Property(t => t.CodigoOrgao).HasColumnName("TB_ORGAO_CODIGO");
            this.Property(t => t.CodigoUge).HasColumnName("TB_UGE_CODIGO");
            this.Property(t => t.CodigoUa).HasColumnName("TB_UA_CODIGO");
            //this.Property(t => t.CodigoDivisao).HasColumnName("TB_DIVISAO_CODIGO");
            //this.Property(t => t.CodigoResponsavel).HasColumnName("TB_RESPONSAVEL_CODIGO");
            this.Property(t => t.ItemMaterialCode).HasColumnName("TB_ITEM_MATERIAL_CODIGO");
            this.Property(t => t.ItemMaterialDescricao).HasColumnName("TB_ITEM_MATERIAL_DESCRICAO").HasMaxLength(120);
            this.ToTable("TB_MOVIMENTO_INTEGRACAO", "dbo");
        }

        internal static MovimentoIntegracaoMap Create()
        {
            return new MovimentoIntegracaoMap();
        }
    }
}