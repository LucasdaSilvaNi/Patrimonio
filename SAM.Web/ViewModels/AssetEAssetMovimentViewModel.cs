using PatrimonioBusiness.integracao.interfaces;
using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class AssetEAssetMovimentViewModel
    {
        public Asset Asset { get; set; }
        public AssetMovements AssetMoviment { get; set; }
        public bool Selecionado { get; set; }
        public string codigo_descricao_orgao_emissor_transferencia { get; set; }
        public string codigo_descricao_uge_emissor_transferencia { get; set; }
        public string codigo_descricao_ua_emissor_transferencia { get; set; }
        //TODO MARCADOR INTEGRACAO SAM-ESTOQUE COM XML
        [Display(Name = "UGE Almoxarifado (Origem)")]
        public string codigo_uge_almox_emissor_transferencia { get; set; }
        public string materialItemPesquisa { get; set; }
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public string ChapaCompleta { get; set; }
        public int CodigoMaterial { get; set; }
        public string DescricaoMaterial { get; set; }
        public string DescricaoTipodeMovimento { get; set; }
        public string Responsavel { get; set; }
        public string CodigoUGE { get; set; }
        public string CodigoUA { get; set; }
        public string DescricaoDivisao { get; set; }
        public decimal? ValorAtual { get; set; }
        public DateTime DataAquisicao { get; set; }
        public int Id { get; set; }
        public int MovementTypeId { get; set; }
        public int UGEId { get; set; }
        public string DataAquisicaoFormatada { get; set; }
        public bool PodeSerEstornadaPorEstarAtiva { get; set; }
        public bool PodeSerEstornadaPorNaoDependerDeAceite { get; set; }
        public bool PodeSerEstornadaPorSerAceiteAutomatico { get; set; }
        public bool PodeEstornarBPPorCompleto { get; set; }
        public bool PodeEstornaMovimento { get; set; }
        public bool Terceiro { get; set; }
        public bool Acervo { get; set; }

        public int? IdContaContabil { get; set; }
        public string ContaContabil { get; set; }
        public int GrupoMaterial { get; set; }

        public bool vindoComoTransferencia { get; set; }
        public bool movimentacaoComOrigem { get; set; }
        public string codigoUGEOrigem { get; set; }

        public bool transferido { get; set; }
        public bool movimentacaoComDestino { get; set; }
        public string siglaChapaOutroBP { get; set; }
        public bool transferenciaFeita { get; set; }

        public bool possuiTipoMovimentoIntegradoAoSIAFEM { get; set; }
        public bool possuiNLParaSerEstornada { get; set; }
        public bool possuiPendenciaNLAtivaVinculada { get; set; }
        public string descricaoPossuiPendenciaNLAtivaVinculada { get; set; }
        public string NotaLancamento { get; set; }
        public string NotaLancamentoEstorno { get; set; }
        public string NotaLancamentoDepreciacao { get; set; }
        public string NotaLancamentoDepreciacaoEstorno { get; set; }
        public string NotaLancamentoReclassificacao { get; set; }
        public string NotaLancamentoReclassificacaoEstorno { get; set; }
    }

    [NotMapped]
    public class GridDocumentoViewModel
    {
        public int IdDoBP { get; set; }
        [Display(Name ="Documento")]
        public string NumeroDocumento { get; set; }
        [Display(Name = "Chapa")]
        public string Chapa { get; set; }
        [Display(Name = "UGE de origem")]
        public string CodigoUGE { get; set; }
        [Display(Name = "Código de Item Material")]
        public int CodigoItemMaterial { get; set; }
        [Display(Name = "Descrição de Item Material")]
        public string DescricaoItemMaterial { get; set; }
    }
}