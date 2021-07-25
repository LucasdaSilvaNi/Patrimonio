using Sam.Common.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SAM.Web.Models
{
    public partial class AuditoriaIntegracao
    {
        public AuditoriaIntegracao()
        {
            this.RelatedAssetMovements = new List<AssetMovements>();
        }



        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Envio")]
        public DateTime DataEnvio { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Retorno")]
        public DateTime? DataRetorno { get; set; }
        public string MsgEstimuloWS { get; set; }
        public string MsgRetornoWS { get; set; }
        public string NomeSistema { get; set; }
        public string UsuarioSAM { get; set; }
        public string UsuarioSistemaExterno { get; set; }
        public int ManagerUnitId { get; set; }

        [NotMapped]
        public Guid chaveSIAFMonitora { get; set; }

        public Guid TokenAuditoriaIntegracao { get; set; }

        /* Campos do XML Envio */
        public string DocumentoId { get; set; }
        public string TipoMovimento { get; set; }
        public DateTime? Data { get; set; }
        public string UgeOrigem { get; set; }
        public string Gestao { get; set; }
        public string Tipo_Entrada_Saida_Reclassificacao_Depreciacao { get; set; }
        public string CpfCnpjUgeFavorecida { get; set; }
        public string GestaoFavorecida { get; set; }
        public string Item { get; set; }
        public string TipoEstoque { get; set; }
        public string Estoque { get; set; }
        public string EstoqueDestino { get; set; }
        public string EstoqueOrigem { get; set; }
        public string TipoMovimentacao { get; set; }
        public decimal? ValorTotal { get; set; }
        public string ControleEspecifico { get; set; }
        public string ControleEspecificoEntrada { get; set; }
        public string ControleEspecificoSaida { get; set; }
        public string FonteRecurso { get; set; }
        public string NLEstorno { get; set; }
        public string Empenho { get; set; }
        public string Observacao { get; set; }
        public string NotaFiscal { get; set; }
        public string ItemMaterial { get; set; }
        /* Campos XML Retorno */
        public string NotaLancamento { get; set; }
        public string MsgErro { get; set; }


        [NotMapped]
        public IList<int> ListagemMovimentacaoPatrimonialIds { get; set; }

        [NotMapped]
        public string RelacaoMovimentacaoPatrimonialIds
        {
            get
            {
                string valorRetorno = null;
                if (ListagemMovimentacaoPatrimonialIds.HasElements())
                { valorRetorno = String.Join(" ", this.ListagemMovimentacaoPatrimonialIds.Select(movPatrimonialId => movPatrimonialId.ToString())); };

                return valorRetorno;
            }

            private set
            { }
        } 

        public string AssetMovementIds { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        public virtual ICollection<AssetMovements> RelatedAssetMovements { get; set; }
        public virtual ICollection<AccountingClosing> RelatedAccountingClosings { get; set; }
        public virtual ICollection<AccountingClosingExcluidos> RelatedAccountingClosingsExcluidos { get; set; }
        public virtual ICollection<AccountingClosingExcluidos> RelatedAccountingClosingsExcluidosEstorno { get; set; }
        public virtual ICollection<NotaLancamentoPendenteSIAFEM> NotaLancamentoPendenteSIAFEMs { get; set; }
    }
}