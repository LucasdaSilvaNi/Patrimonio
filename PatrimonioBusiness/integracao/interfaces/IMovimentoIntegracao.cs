using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatrimonioBusiness.integracao.interfaces
{
    public interface IMovimentoIntegracao
    {
        int Id { get; set; }
        int MovimentoId { get; set; }
        int MovimentoItemId { get; set; }
        int TipoMovimentoId { get; set; }
        int CodigoOrgao { get; set; }
        int UgeId { get; set; }
        int CodigoUge { get; set; }
        int? CodigoUa { get; set; }
        int CodigoDivisao { get; set; }
        long CodigoResponsavel { get; set; }
        int ItemMaterialCode { get; set; }
        int AlmoxarifadoId { get; set; }
        int ItemMaterialId { get; set; }
        int SubItemMaterialId { get; set; }
        string NumeroDocumento { get; set; }
        string AnoMesReferencia { get; set; }
        string Empenho { get; set; }
        string Obeservacoes { get; set; }
        int? AlmoxarifadoOrigemDestinoId { get; set; }
        DateTime DataMovimento { get; set; }
        DateTime? DataOperacao { get; set; }
        Decimal QuantidadeMovimentada { get; set; }
        Decimal ValorUnitario { get; set; }
        bool Ativo { get; set; }
        bool ItemAtivo { get; set; }
        int? IntegracaoRealizado { get; set; }
        DateTime DataInclusao { get; set; }
        DateTime? DataAlteracao { get; set; }
        String ItemMaterialDescricao { get; set; }
    }
}