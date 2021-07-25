using System.ComponentModel;

namespace SAM.Web.Common.Enum
{
        public enum TipoNotaSIAF
        {
            [Description("Nota de Empenho")]
            NE = 1,
            [Description("NL Liquidação")]
            NL_Liquidacao = 2,
            [Description("NL Consumo")]
            NL_Consumo = 3,
            [Description("NL Reclassificação")]
            NL_Reclassificacao = 4,
            [Description("NL Depreciação")]
            NL_Depreciacao = 5,
            [Description("Tipo Desconhecido")]
            Desconhecido = 999
        }
}