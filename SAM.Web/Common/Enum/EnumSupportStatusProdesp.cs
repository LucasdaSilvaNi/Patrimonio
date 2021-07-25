using System.ComponentModel;
namespace SAM.Web.Common.Enum
{
    public enum EnumSupportStatusProdesp
    {
        [Description("Aberto")]
        Aberto = 1,
        [Description("Em Atendimento")]
        EmAtendimento = 2,
        [Description("Aguardando Usuário")]
        AguardandoUsuario = 3,
        [Description("Finalizado")]
        Finalizado = 4,
        [Description("Reaberto")]
        Reaberto = 5
    }
}