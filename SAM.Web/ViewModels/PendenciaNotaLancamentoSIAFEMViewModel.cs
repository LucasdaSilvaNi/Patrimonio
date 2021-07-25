using System;
using System.ComponentModel.DataAnnotations.Schema;
using tipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;


namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class PendenciaNotaLancamentoSIAFEMViewModel
    {
        public int Id { get; set; }
        public string Orgao { get; set; }
        public string UO { get; set; }
        public string UGE { get; set; }
        public string NumeroDocumentoSAM { get; set; }
        public tipoNotaSIAF TipoNotaNumero { get; set; }
        public string DescricaoTipoNotaSIAFEM { get; set; }
        public string ErroSIAFEM { get; set; }
        public string DataEnvioSIAFEM { get; set; }
        public DateTime DatetimeDataEnvioSIAFEM { get; set; }
        public bool? flagEstornado { get; set; }
        public int ManagerUnitId { get; set; }
        public string PrefixoNL{ get; set; }
        public int GrupoMovimentacao { get; set; }
        public int TipoMovimentacaoPatrimonialId { get; set; }

        public string TipoAgrupamentoMovimentacaoPatrimonial { get; set; }
        public string TipoMovimentacaoPatrimonial { get; set; }
        public string TokenAuditoriaIntegracao { get; set; }
        public string MsgEnvioWS { get; set; }
        public string ValorMovimentacao { get; set; }
        public string ContaContabil { get; set; }
        public string DataMovimentacao { get; set; }
    }
}