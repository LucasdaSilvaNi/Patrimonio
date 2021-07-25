using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.Models
{
    public partial class EventServiceContabilizaSP
    {
        public int Id { get; set; }

        [Required]
        public int AccountEntryTypeId { get; set; }

        [NotMapped]
        [Display(Name = "Tipo Agrupamento")]
        public string AccountEntryType { get; set; }

        [Required]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "Campo obrigatório")]
        public String StockDescription { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Campo obrigatório")]
        public String StockType { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string StockSource { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public String StockDestination { get; set; }

        [Required]
        [StringLength(18, MinimumLength = 1, ErrorMessage = "Campo obrigatório")]
        public String MaterialType { get; set; }

        [Required]
        [Display(Name = "Código Movimentação (SAM)")]
        public Int32 InputOutputReclassificationDepreciationTypeCode { get; set; }

        [NotMapped]
        [StringLength(100, MinimumLength = 1)]
        [Display(Name = "Tipo Entrada/Saída/Reclassificação/Depreciação")]
        public String InputOutputReclassificationDepreciationType { get; set; }

        [StringLength(15)]
        public String SpecificControl { get; set; }

        [StringLength(15)]
        public String SpecificInputControl { get; set; }

        [StringLength(15)]
        public String SpecificOutputControl { get; set; }

        [NotMapped]
        [StringLength(11, MinimumLength = 1)]
        public String CommitmentNumber { get; set; }

        [Required]
        [StringLength(100)]
        public String TipoMovimento_SamPatrimonio { get; set; }

        [NotMapped]
        public int CodigoTipoMovimento_SamPatrimonio { get; set; }

        [StringLength(100)]
        public String TipoMovimentacao_ContabilizaSP { get; set; }

        [Required]
        public int MetaDataType_DateField { get; set; }
        [Required]
        public int MetaDataType_AccountingValueField { get; set; }
        public int? MetaDataType_StockSource { get; set; }
        public int? MetaDataType_StockDestination { get; set; }
        public int? MetaDataType_MovementTypeContabilizaSP { get; set; }
        public int? MetaDataType_SpecificControl { get; set; }
        public int? MetaDataType_SpecificInputControl { get; set; }
        public int? MetaDataType_SpecificOutputControl { get; set; }

        [NotMapped]
        public bool MovimentacaoPatrimonialDepreciaOuReclassifica { get; set; }
        [NotMapped]
        [Display(Name = "Data Movimentação (MetaDado)")]
        public string MetaDataType_DateFieldDescription { get; set; }
        [NotMapped]
        [Display(Name = "Valor Movimentação (MetaDado)")]
        public string MetaDataType_AccountingValueFieldDescription { get; set; }
        [NotMapped]
        [Display(Name = "Estoque Origem (MetaDado)")]
        public string MetaDataType_StockSourceDescription { get; set; }
        [NotMapped]
        [Display(Name = "Estoque Destino (MetaDado)")]
        public string MetaDataType_StockDestinationDescription { get; set; }
        [NotMapped]
        [Display(Name = "Controle Específico (MetaDado)")]
        public string MetaDataType_SpecificControlDescription { get; set; }
        [NotMapped]
        [Display(Name = "Controle Específico Entrada (MetaDado)")]
        public string MetaDataType_SpecificInputControlDescription { get; set; }
        [NotMapped]
        [Display(Name = "Controle Específico Saída (MetaDado)")]
        public string MetaDataType_SpecificOutputControlDescription { get; set; }
        [NotMapped]
        [Display(Name = "Tipo Movimentação (SAM)")]
        public string DescricaoTipoMovimento_SamPatrimonio { get; set; }

        [NotMapped]
        [Display(Name = "Tipo Movimentação (ContabilizaSP)")]
        public string DescricaoTipoMovimentacao_ContabilizaSP { get; set; }

        public bool Status { get; set; }

        [Required]
        [Display(Name = "Ordem Execução/Envio (SIAFEM)")]
        public short ExecutionOrder { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data Ativação Integração")]
        public DateTime DataAtivacaoTipoMovimentacao { get; set; }

        public bool FavoreceDestino { get; set; }
        public bool VerificaSeOrigemUtilizaSAM { get; set; }
        public bool UtilizaTipoMovimentacaoContabilizaSPAlternativa { get; set; }
        public string TipoMovimentacaoContabilizaSPAlternativa { get; set; }


        //Relacionamentos
        public virtual MovementType RelatedMovementType { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_DateField { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_AccountingValueField { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_StockSource { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_StockDestination { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_MovementTypeContabilizaSP { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_SpecificControl { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_SpecificInputControl { get; set; }
        public virtual MetaDataTypeServiceContabilizaSP RelatedMetaDataType_SpecificOutputControl { get; set; }
    }
}