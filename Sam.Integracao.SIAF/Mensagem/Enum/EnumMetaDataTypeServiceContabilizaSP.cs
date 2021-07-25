using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;



namespace Sam.Integracao.SIAF.Mensagem.Enum
{
    public class EnumMetaDataTypeServiceContabilizaSP
    {
        //MetaDataTypeServiceContabilizaSP SMALLINT NOT NULL, --Tipo de Nota de Lancamento(Valores: 1-Entrada, 2-Saida, 8-Depreciacao, 9-Reclassificacao)
        public enum MetaDataTypeServiceContabilizaSP
        {
            [Description("Valor Desconhecido / Inválido / Nulo")]
            UnknownValue = 0,

            [Description("Data de Incorporação")]
            IncorporationDate = 1,

            [Description("Data de Movimentação")]
            MovimentDate = 2,

            [Description("Valor de Aquisição")]
            AcquisitionValue = 3,

            [Description("Valor Atual")]
            CurrentValue = 4,

            [Description("Valor de Depreciação Acumulada")]
            AccumulatedDepreciationValue = 5,

            [Description("Conta Contábil")]
            AuxiliaryAccountAsset = 6,

            [Description("Conta Contábil de Depreciação")]
            DepreciationAccountAsset = 7,

            [Description("Tipo Movimentação SAM")]
            Sam_MovementTypeId = 8,

            [Description("Informação Textual")]
            TextualInformation = 9,

            [Description("Tipo Movimentação Contabiliza-SP")]
            ContabilizaSP_MovementTypeDescription = 10,

            [Description("Controle Específico Padrão")]
            StandartSpecificControl = 11,

            [Description("Controle Específico Por Tipo Movimentação")]
            SpecificControlByMovementType = 12,

            [Description("Tipo Movimentação Contabiliza-SP associado à Conta Contábil do Item Material")]
            ContabilizaSP_MovementTypeLinkedAuxiliaryAccount = 13,

            [Description("Tipo Movimentação Contabiliza-SP associado à Conta Contábil de Depreciação do Item Material")]
            ContabilizaSP_MovementTypeLinkedDepreciationAccount = 14,

            [Description("Tipo Movimentação Contabiliza-SP associado à Conta Contábil 'Original' do Item Material (Inservivel na UGE)")]
            ContabilizaSP_MovementTypeLinkedOriginalAuxiliaryAccount = 15,

            [Description("Tipo Movimentação Contabiliza-SP associado à Conta Contábil 'Original' de Depreciação do Item Material (Inservivel na UGE)")]
            ContabilizaSP_MovementTypeLinkedOriginalDepreciationAccount = 16,
        }

        public static explicit operator EnumMetaDataTypeServiceContabilizaSP(int v)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class MetaDataTypeServiceContabilizaSPType
    {
        /*
        ('0', 'UnknownValue') --ParametroDesconhecido_Inválido
        ('1', 'IncorporationDate') --DataIncorporacao
        ('2', 'MovimentDate') --DataMovimentacao 
        ('3', 'AcquisitionValue') --ValorAquisicao
        ('4', 'CurrentValue') --ValorAtual
        ('5', 'AccumulatedDepreciationValue') --ValorDepreciacaoAcumulada
        ('6', 'AuxiliaryAccountAsset') --ContaContabil
        ('7', 'DepreciationAuxiliaryAccountAsset') --ContaContabil
        ('8', 'Sam_MovementTypeId') --TipoMovimentoSamId
        ('9', 'TextualInformation') --InformacaoTextual
        ('10', 'ContabilizaSP_MovementTypeDescription' --DescricaoTipoMovimentoContabilizaSP
        ('11', 'StandartSpecificControl') --ControleEspecificoPorContaAuxiliar
        ('12', 'SpecificControlByMovementType') --ControleEspecificoPorTipo Movimentacao")
        ('13', 'ContabilizaSP_MovementTypeLinkedAuxiliaryAccount') --TipoMovimentacao_Contabiliza-SP_associado_ContaContábil_Item Material");
        ('14', 'ContabilizaSP_MovementTypeLinkedDepreciationAccount') --TipoMovimentacao_Contabiliza-SP_associado_ContaContábilDepreciação_Item Material");
        */
        //private readonly string _name;
        public static MetaDataTypeServiceContabilizaSPType UnknownValue { get; } = new MetaDataTypeServiceContabilizaSPType(0, "Parametro Desconhecido/Inválido");
        public static MetaDataTypeServiceContabilizaSPType IncorporationDate { get; } = new MetaDataTypeServiceContabilizaSPType(1, "Data de Incorporação");
        public static MetaDataTypeServiceContabilizaSPType MovimentDate { get; } = new MetaDataTypeServiceContabilizaSPType(2, "Data de Movimentação");
        public static MetaDataTypeServiceContabilizaSPType AcquisitionValue { get; } = new MetaDataTypeServiceContabilizaSPType(3, "Valor de Aquisição");
        public static MetaDataTypeServiceContabilizaSPType CurrentValue { get; } = new MetaDataTypeServiceContabilizaSPType(4, "Valor Atual");
        public static MetaDataTypeServiceContabilizaSPType AccumulatedDepreciationValue { get; } = new MetaDataTypeServiceContabilizaSPType(5, "Valor de Depreciação Acumulada");
        public static MetaDataTypeServiceContabilizaSPType AuxiliaryAccountAsset { get; } = new MetaDataTypeServiceContabilizaSPType(6, "Conta Contábil");
        public static MetaDataTypeServiceContabilizaSPType DepreciationAuxiliaryAccountAsset { get; } = new MetaDataTypeServiceContabilizaSPType(7, "Conta Contábil de Depreciação");
        public static MetaDataTypeServiceContabilizaSPType Sam_MovementTypeId { get; } = new MetaDataTypeServiceContabilizaSPType(8, "Tipo Movimentação SAM");
        public static MetaDataTypeServiceContabilizaSPType TextualInformation { get; } = new MetaDataTypeServiceContabilizaSPType(9, "Informação Textual");
        public static MetaDataTypeServiceContabilizaSPType ContabilizaSP_MovementTypeDescription { get; } = new MetaDataTypeServiceContabilizaSPType(10, "Tipo Movimentação Contabiliza-SP");
        public static MetaDataTypeServiceContabilizaSPType StandartSpecificControl { get; } = new MetaDataTypeServiceContabilizaSPType(11, "Controle Específico Padrão");
        public static MetaDataTypeServiceContabilizaSPType SpecificControlByMovementType { get; } = new MetaDataTypeServiceContabilizaSPType(12, "Controle Específico Por Tipo Movimentação");
        public static MetaDataTypeServiceContabilizaSPType ContabilizaSP_MovementTypeLinkedAuxiliaryAccount { get; } = new MetaDataTypeServiceContabilizaSPType(13, "Tipo Movimentação Contabiliza-SP associado à Conta Contábil do Item Material");
        public static MetaDataTypeServiceContabilizaSPType ContabilizaSP_MovementTypeLinkedDepreciationAccount { get; } = new MetaDataTypeServiceContabilizaSPType(14, "Tipo Movimentação Contabiliza-SP associado à Conta Contábil de Depreciação do Item Material");
        public static MetaDataTypeServiceContabilizaSPType ContabilizaSP_MovementTypeLinkedOriginalAuxiliaryAccount { get; } = new MetaDataTypeServiceContabilizaSPType(15, "Tipo Movimentação Contabiliza-SP associado à Conta Contábil de Depreciação do Item Material");
        public static MetaDataTypeServiceContabilizaSPType ContabilizaSP_MovementTypeLinkedOriginalDepreciationAccount { get; } = new MetaDataTypeServiceContabilizaSPType(16, "Tipo Movimentação Contabiliza-SP associado à Conta Contábil 'Original' de Depreciação do Item Material (Inservivel na UGE)");

        private MetaDataTypeServiceContabilizaSPType(int val, string name)
        {
            Value = val;
            Name = name;
        }

        private MetaDataTypeServiceContabilizaSPType()
        {
            // required for EF
        }

        public int Value { get; private set; }
        public string Name { get; private set; }

        public static IEnumerable<MetaDataTypeServiceContabilizaSPType> GetEnumValues()
        {
            return new[] { UnknownValue, IncorporationDate, MovimentDate, AcquisitionValue, CurrentValue, AccumulatedDepreciationValue, AuxiliaryAccountAsset, DepreciationAuxiliaryAccountAsset, Sam_MovementTypeId, TextualInformation, ContabilizaSP_MovementTypeDescription, StandartSpecificControl, SpecificControlByMovementType, ContabilizaSP_MovementTypeLinkedAuxiliaryAccount, ContabilizaSP_MovementTypeLinkedDepreciationAccount, ContabilizaSP_MovementTypeLinkedOriginalAuxiliaryAccount, ContabilizaSP_MovementTypeLinkedOriginalDepreciationAccount };
        }

        public static MetaDataTypeServiceContabilizaSPType FromString(string MetaDataTypeServiceContabilizaSPString)
        {
            return GetEnumValues().FirstOrDefault(r => String.Equals(r.Name, MetaDataTypeServiceContabilizaSPString, StringComparison.OrdinalIgnoreCase));
        }

        public static MetaDataTypeServiceContabilizaSPType FromValue(int value)
        {
            return GetEnumValues().FirstOrDefault(r => r.Value == value);
        }
    }
}