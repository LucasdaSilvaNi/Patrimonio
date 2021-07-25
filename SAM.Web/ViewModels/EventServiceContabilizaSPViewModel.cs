using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    public class EventServiceContabilizaSPViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tipo de Agrupamento")]
        public string AccountEntryType { get; set; } 
        public int AccountEntryTypeId { get; set; }
        public string StockDescription { get; set; }
        public string StockType { get; set; }
        public string StockSource { get; set; }
        public string StockDestination { get; set; }

        public string MaterialType { get; set; }
        [Display(Name = "Código Movimentação (SAM)")]
        public int InputOutputReclassificationDepreciationTypeCode { get; set; }
        [Display(Name = "Tipo Entrada/Saída/Reclassificação/Depreciação")]
        public string InputOutputReclassificationDepreciationType { get; set; }
        [Display(Name = "Controle Específico (MetaDado)")]
        public string SpecificControl { get; set; }
        [Display(Name = "Controle Específico de Entrada (MetaDado)")]
        public string SpecificInputControl { get; set; }
        [Display(Name = "Controle Específico de Saída (MetaDado)")]
        public string SpecificOutputControl { get; set; }
        public string TipoMovimento_SamPatrimonio { get; set; }
        public string TipoMovimentacao_ContabilizaSP { get; set; }
        [Display(Name = "Tipo Movimentação (SAM)")]
        public string DescricaoTipoMovimento_SamPatrimonio { get; set; }
        [Display(Name = "Tipo Movimentação (Contabiliza)")]
        public string DescricaoTipoMovimentacao_ContabilizaSP { get; set; }
        [Display(Name = "Data Movimentação (MetaDado)")]
        public string MetaDataType_DateFieldDescription { get; set; }
        public int MetaDataType_DateField { get; set; }
        [Display(Name = "Valor Movimentação (MetaDado)")]
        public string MetaDataType_AccountingValueFieldDescription { get; set; }
        public int MetaDataType_AccountingValueField { get; set; }
        [Display(Name = "Estoque Origem (MetaDado)")]
        public string MetaDataType_StockSourceDescription { get; set; }
        public string MetaDataType_StockSource { get; set; }
        [Display(Name = "Estoque Destino (MetaDado)")]
        public string MetaDataType_StockDestinationDescription { get; set; }
        public int? MetaDataType_StockDestination { get; set; }
        public bool Status { get; set; }
        public bool MovimentacaoPatrimonialDepreciaOuReclassifica { get; set;}

        //Edicao
        public int CodigoTipoMovimento_SamPatrimonio { get; set; }
        public int CodigoTipoMovimento_ContabilizaSP { get; set; }
        public int CodigoEstoque_Origem { get; set; }
        public int CodigoEstoque_Destino { get; set; }
        public int CodigoCE { get; set; }
        public int CodigoCE_Entrada { get; set; }
        public int CodigoCE_Saida { get; set; }
        [Display(Name = "Ordem Execução/Envio (SIAFEM)")]
        public short ExecutionOrder { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data Ativação Integração")]
        public DateTime DataAtivacaoTipoMovimentacao { get; set; }

        //fim edicao

        public EventServiceContabilizaSPViewModel Create(EventServiceContabilizaSP eventServiceContabilizaSP)
        {
            return new EventServiceContabilizaSPViewModel()
            {
                Id = eventServiceContabilizaSP.Id,
                AccountEntryType = eventServiceContabilizaSP.AccountEntryType,
                AccountEntryTypeId = eventServiceContabilizaSP.AccountEntryTypeId,

                StockDescription = eventServiceContabilizaSP.StockDescription,
                StockType = eventServiceContabilizaSP.StockType,
                StockDestination = eventServiceContabilizaSP.StockDestination,
                StockSource = eventServiceContabilizaSP.StockSource,

                InputOutputReclassificationDepreciationTypeCode = eventServiceContabilizaSP.InputOutputReclassificationDepreciationTypeCode,
                InputOutputReclassificationDepreciationType = eventServiceContabilizaSP.InputOutputReclassificationDepreciationType,

                SpecificControl = eventServiceContabilizaSP.SpecificControl,
                SpecificInputControl = eventServiceContabilizaSP.SpecificInputControl,
                SpecificOutputControl = eventServiceContabilizaSP.SpecificOutputControl,

                TipoMovimentacao_ContabilizaSP = eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP,
                TipoMovimento_SamPatrimonio = eventServiceContabilizaSP.TipoMovimento_SamPatrimonio,
                DescricaoTipoMovimentacao_ContabilizaSP = eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP,
                DescricaoTipoMovimento_SamPatrimonio = eventServiceContabilizaSP.DescricaoTipoMovimento_SamPatrimonio,


                MetaDataType_AccountingValueField = eventServiceContabilizaSP.MetaDataType_AccountingValueField,
                MetaDataType_DateField = eventServiceContabilizaSP.MetaDataType_DateField,
                MetaDataType_StockDestination = eventServiceContabilizaSP.MetaDataType_StockDestination,
                MetaDataType_AccountingValueFieldDescription = eventServiceContabilizaSP.MetaDataType_AccountingValueFieldDescription,
                MetaDataType_DateFieldDescription = eventServiceContabilizaSP.MetaDataType_DateFieldDescription,
                MetaDataType_StockDestinationDescription = eventServiceContabilizaSP.MetaDataType_StockDestinationDescription,
                MetaDataType_StockSource = eventServiceContabilizaSP.StockSource,
                MetaDataType_StockSourceDescription = eventServiceContabilizaSP.MetaDataType_StockSourceDescription,
                MovimentacaoPatrimonialDepreciaOuReclassifica = eventServiceContabilizaSP.MovimentacaoPatrimonialDepreciaOuReclassifica,

                CodigoEstoque_Origem = eventServiceContabilizaSP.MetaDataType_StockSource ?? 0,
                CodigoTipoMovimento_ContabilizaSP = eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP ?? 0,
                CodigoEstoque_Destino = eventServiceContabilizaSP.MetaDataType_StockDestination ?? 0,
                CodigoCE = eventServiceContabilizaSP.MetaDataType_SpecificControl ?? 0,
                CodigoCE_Entrada = eventServiceContabilizaSP.MetaDataType_SpecificInputControl ?? 0,
                CodigoCE_Saida = eventServiceContabilizaSP.MetaDataType_SpecificOutputControl ?? 0,
                MaterialType = eventServiceContabilizaSP.MaterialType,

                Status = eventServiceContabilizaSP.Status,
                ExecutionOrder = eventServiceContabilizaSP.ExecutionOrder,
                DataAtivacaoTipoMovimentacao = eventServiceContabilizaSP.DataAtivacaoTipoMovimentacao,
            };
        }
    }
}
