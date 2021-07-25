using System.ComponentModel;
namespace SAM.Web.Common.Enum
{
    public class EnumTelaVisaoGeral
    {
        public enum FiltroVisaoGeral
        {
            [Description("Baixados")]
            Baixados = 0,

            [Description("Ativo")]
            Ativo = 1,

            [Description("PendenteDeTranferenciaOuDoacao")]
            PendenteDeTranferenciaOuDoacao = 2,

            [Description("Reclassificacao")]
            Reclassificacao = 3
        }
    }
}