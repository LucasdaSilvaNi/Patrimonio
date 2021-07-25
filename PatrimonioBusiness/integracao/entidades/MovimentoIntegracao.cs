using PatrimonioBusiness.integracao.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace PatrimonioBusiness.integracao.entidades
{
    public class MovimentoIntegracao : IMovimentoIntegracao
    {
        protected MovimentoIntegracao()
        {

        }

        internal static MovimentoIntegracao GetInstancia()
        {
            return new MovimentoIntegracao();
        }
        public int Id { get; set; }
        [NotMapped]
        public int MovimentoId { get; set; }
        public int TipoMovimentoId { get; set; }
        [NotMapped]
        public int UgeId { get; set; }
        [NotMapped]
        public int AlmoxarifadoId { get; set; }
        [NotMapped]
        public int ItemMaterialId { get; set; }
        [NotMapped]
        public int SubItemMaterialId { get; set; }
        public string NumeroDocumento { get; set; }
        public string AnoMesReferencia { get; set; }
        public string Empenho { get; set; }
        public string Obeservacoes { get; set; }
        public int? AlmoxarifadoOrigemDestinoId { get; set; }
        public DateTime DataMovimento { get; set; }
        public DateTime? DataOperacao { get; set; }
        public Decimal QuantidadeMovimentada { get; set; }
        public decimal ValorUnitario { get; set; }
        public bool Ativo { get; set; }
        public bool ItemAtivo { get; set; }
        public int? IntegracaoRealizado { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        [NotMapped]
        public int MovimentoItemId { get; set ; }
        [NotMapped]
        public int CodigoOrgao { get; set; }
        public int CodigoUge { get; set; }
        public int? CodigoUa { get; set; }
        [NotMapped]
        public int CodigoDivisao { get; set; }
        [NotMapped]
        public long CodigoResponsavel { get; set; }
        public int ItemMaterialCode { get; set; }
        public string ItemMaterialDescricao { get; set; }
        internal IMovimentoIntegracao GetMovimentoIntegracao()
        {
            return new MovimentoIntegracao()
            {
                AlmoxarifadoId = this.AlmoxarifadoId,
                AlmoxarifadoOrigemDestinoId = this.AlmoxarifadoOrigemDestinoId,
                AnoMesReferencia = this.AnoMesReferencia,
                Ativo = this.Ativo,
                DataAlteracao = this.DataAlteracao,
                DataInclusao = this.DataInclusao,
                DataMovimento = this.DataMovimento,
                DataOperacao = this.DataOperacao,
                Empenho = this.Empenho,
                Id = this.Id,
                IntegracaoRealizado = this.IntegracaoRealizado,
                ItemAtivo = this.ItemAtivo,
                ItemMaterialId = this.ItemMaterialId,
                MovimentoId = this.MovimentoId,
                NumeroDocumento = this.NumeroDocumento,
                Obeservacoes = this.Obeservacoes,
                QuantidadeMovimentada = this.QuantidadeMovimentada,
                SubItemMaterialId = this.SubItemMaterialId,
                TipoMovimentoId = this.TipoMovimentoId,
                UgeId = this.UgeId,
                ValorUnitario = this.ValorUnitario,
                CodigoDivisao = this.CodigoDivisao,
                CodigoOrgao = this.CodigoOrgao,
                CodigoResponsavel = this.CodigoResponsavel,
                CodigoUa = this.CodigoUa,
                CodigoUge = this.CodigoUge,
                ItemMaterialCode = this.ItemMaterialCode,
                MovimentoItemId = this.MovimentoItemId,
                ItemMaterialDescricao = this.ItemMaterialDescricao
            };
        }
        internal static MovimentoIntegracao GetMovimentoIntegracao(IMovimentoIntegracao movimentoIntegracao)
        {
            return new MovimentoIntegracao()
            {
                AlmoxarifadoId = movimentoIntegracao.AlmoxarifadoId,
                AlmoxarifadoOrigemDestinoId = movimentoIntegracao.AlmoxarifadoOrigemDestinoId,
                AnoMesReferencia = movimentoIntegracao.AnoMesReferencia,
                Ativo = movimentoIntegracao.Ativo,
                DataAlteracao = movimentoIntegracao.DataAlteracao,
                DataInclusao = movimentoIntegracao.DataInclusao,
                DataMovimento = movimentoIntegracao.DataMovimento,
                DataOperacao = movimentoIntegracao.DataOperacao,
                Empenho = movimentoIntegracao.Empenho,
                Id = movimentoIntegracao.Id,
                IntegracaoRealizado = movimentoIntegracao.IntegracaoRealizado,
                ItemAtivo = movimentoIntegracao.ItemAtivo,
                ItemMaterialId = movimentoIntegracao.ItemMaterialId,
                MovimentoId = movimentoIntegracao.MovimentoId,
                NumeroDocumento = movimentoIntegracao.NumeroDocumento,
                Obeservacoes = movimentoIntegracao.Obeservacoes,
                QuantidadeMovimentada = movimentoIntegracao.QuantidadeMovimentada,
                SubItemMaterialId = movimentoIntegracao.SubItemMaterialId,
                TipoMovimentoId = movimentoIntegracao.TipoMovimentoId,
                UgeId = movimentoIntegracao.UgeId,
                ValorUnitario = movimentoIntegracao.ValorUnitario,
                CodigoDivisao = movimentoIntegracao.CodigoDivisao,
                CodigoOrgao = movimentoIntegracao.CodigoOrgao,
                CodigoResponsavel = movimentoIntegracao.CodigoResponsavel,
                CodigoUa = movimentoIntegracao.CodigoUa,
                CodigoUge = movimentoIntegracao.CodigoUge,
                ItemMaterialCode = movimentoIntegracao.ItemMaterialCode,
                MovimentoItemId = movimentoIntegracao.MovimentoItemId
            };
        }
    }
}