using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;



namespace Sam.Integracao.SIAF.Mensagem.Enum
{
    public class EnumAccountEntryType
    {
        //AccountEntryType SMALLINT NOT NULL, --Tipo de Nota de Lancamento(Valores: 1-Entrada, 2-Saida, 8-Depreciacao, 9-Reclassificacao)
        public enum AccountEntryType
        {
            [Description("ENTRADA")]
            Entrada = (Int16)1,

            [Description("SAÍDA")]
            Saida = (Int16)2,

            [Description("DEPRECIAÇÃO")]
            Depreciacao = (Int16)8,

            [Description("RECLASSIFICAÇÃO")]
            Reclassificacao = (Int16)9
        }
    }

    public class AccountEntryType
    {
        private readonly string _name;
        public static AccountEntryType Entrada { get; } = new AccountEntryType(1, "ENTRADA");
        public static AccountEntryType Saida { get; } = new AccountEntryType(2, "SAÍDA");
        public static AccountEntryType Depreciacao { get; } = new AccountEntryType(8, "DEPRECIAÇÃO");
        public static AccountEntryType Reclassificacao { get; } = new AccountEntryType(9, "RECLASSIFICAÇÃO");

        private AccountEntryType(int val, string name)
        {
            Value = val;
            Name = name;
        }
        private AccountEntryType()
        {
            // required for EF
        }

        public int Value { get; private set; }
        public string Name { get; private set; }

        public static IEnumerable<AccountEntryType> GetEnumValues()
        {
            return new[] { Entrada, Saida, Depreciacao, Reclassificacao };
        }

        public static AccountEntryType FromString(string accountEntryTypeString)
        {
            return GetEnumValues().FirstOrDefault(r => String.Equals(r.Name, accountEntryTypeString, StringComparison.OrdinalIgnoreCase));
        }
        public static AccountEntryType FromValue(int value)
        {
            return GetEnumValues().FirstOrDefault(r => r.Value == value);
        }
    }
}