using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Integracao.SIAF.Mensagem.Interface
{
    public interface ISIAFNlPatrimonial
    {
        Int32 AccountEntryTypeId { get; set; }
        string AccountEntryType { get; set; }

        string StockDescription { get; set; }
        string StockType { get; set; }
        string StockSource { get; set; }
        string StockDestination { get; set; }
        string MaterialType { get; set; }
        Int32  InputOutputReclassificationDepreciationTypeCode { get; set; }
        string InputOutputReclassificationDepreciationType { get; set; }
        string DescricaoTipoMovimento_SamPatrimonio { get; set; }
        string DescricaoTipoMovimentacao_ContabilizaSP { get; set; }
        //string[] ObservacoesMovimentacao { get; set; }
        string Observacao { get; set; }
        string SpecificControl { get; set; }
        string SpecificInputControl { get; set; }
        string SpecificOutputControl { get; set; }
        string CommitmentNumber { get; set; }
        bool Status { get; set; }
        bool FavoreceDestino { get; }
        string GestaoFavorecida { get; set; }
        string CpfCnpjUgeFavorecido { get; set; }

        int MetaDataType_AccountingValueField { get; set; }
        int MetaDataType_DateField { get; set; }
        int MetaDataType_StockSource { get; set; }
        int MetaDataType_StockDestination { get; set; }
        int MetaDataType_MovementTypeContabilizaSP { get; set; }

        DateTime MovimentDate { get; set; }
        int ManagerUnitCode { get; set; }
        int ManagerCode { get; set; }
        decimal AccountingValueField { get; set; }
        long MaterialItemCode { get; set; }
        IDictionary<string, long> RelacaoItemMaterial { get; set; }
        string DocumentNumberSAM { get; set; }
        bool EhEstorno { get; set; }

        IList<long> RelacaoMovimentacoesPatrimonaisId { get; set; }
        Guid TokenGeradoEnvioContabilizaSP { get; set; }
        string TokenEnvioContabilizaSP { get; set; }
    }
}
