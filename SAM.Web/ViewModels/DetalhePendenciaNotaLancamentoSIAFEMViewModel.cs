using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using tipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;




namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class DetalhePendenciaNotaLancamentoSIAFEMViewModel
    {
        public string Orgao { get; set; }
        public string UO { get; set; }
        public string UGE { get; set; }
        public string NumeroDocumentoSAM { get; set; }
        public string DescricaoTipoNotaSIAFEM { get; set; }
        public string ErroSIAFEM { get; set; }
        public string DataHoraEnvioSIAFEM { get; set; }
        
        public string CpfUsuarioSAM { get; set; }
        public string CpfUsuarioSIAFEM { get; set; }
        public string TipoMovimentoContabilizaSP { get; set; }
        public string TipoMovimentacaoContabilizaSP { get; set; }
        public string ContaContabil_TipoMovimentacaoContabilizaSP { get; set; }
        public string EstoqueOrigem { get; set; }
        public string EstoqueDestino { get; set; }
        public string ContaContabil_EstoqueOrigem { get; set; }
        public string ContaContabil_EstoqueDestino { get; set; }
        public string TipoMovimentacaoSAMPatrimonio { get; set; }
        public string TokenAuditoriaIntegracao { get; set; }
        public string ValorMovimentacao { get; set; }
        public string ContaContabil { get; set; }
        public string DataMovimentacao { get; set; }


        public IList<DadosBemPatrimonialViewModel> ListagemBPsVinculados { get; set; }
    }

    public class DadosBemPatrimonialViewModel
    {
        public int Codigo { get; set; }
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public string GrupoMaterial { get; set; }
        public string ItemMaterial { get; set; }
        public string ContaContabil { get; set; }
        public string ValorAquisicao { get; set; }
        public string ValorAtual { get; set; }
        public string ValorDepreciacaoAcumulada { get; set; }
        public string ValorDepreciacaoMensal { get; set; }
        public string ValorContabil { get; set; }
    }
}