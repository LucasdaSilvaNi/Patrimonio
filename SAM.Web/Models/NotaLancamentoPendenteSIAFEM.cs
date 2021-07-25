using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace SAM.Web.Models
{
    public partial class NotaLancamentoPendenteSIAFEM
    {
        public NotaLancamentoPendenteSIAFEM()
        {
            //this.RelatedAssetMovements = new List<AssetMovements>();
        }

        public int Id { get; set; }
        public int ManagerUnitId { get; set; }
        public int AuditoriaIntegracaoId { get; set; }
        public string NumeroDocumentoSAM { get; set; }

        [NotMapped]
        public string TipoAgrupamentoMovimentacaoPatrimonial { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Envio MSG WS")]
        public DateTime DataHoraEnvioMsgWS { get; set; }

        public string ErroProcessamentoMsgWS { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Retorno MSG WS")]
        public DateTime? DataHoraReenvioMsgWS { get; set; }

        public short StatusPendencia { get; set; }
        public short TipoNotaPendencia { get; set; }


        public string AssetMovementIds { get; set; }
        public virtual AuditoriaIntegracao RelatedAuditoriaIntegracao { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        //public virtual ICollection<AssetMovements> RelatedAssetMovements { get; set; }
    }
}