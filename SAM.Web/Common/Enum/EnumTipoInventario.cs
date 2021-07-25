using System.ComponentModel;
namespace SAM.Web.Common.Enum
{
    public enum EnumTipoInventario
    {
        [Description("Android")]
        Android = 1,

        [Description("Coletor de Dados")]
        ColetorDados = 2,

        [Description("Inventário Manual")]
        InventarioManual = 3,
    }
}