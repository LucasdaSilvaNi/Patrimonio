using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sam.Common.Enums
{
    public static class PerfilNivelAcessoEnum
    {
        public enum PerfilNivelAcesso
        {
            /// <summary>
            /// Perfil Nível I
            /// </summary>
            [Description("Perfil Nível I")]
            Nivel_I = 1,

            /// <summary>
            /// Perfil Nível II
            /// </summary>
            [Description("Perfil Nível II")]
            Nivel_II = 2,

            /// <summary>
            /// Perfil Adminsitrativo (Não necessita de contrato para acessar)
            /// </summary>
            [Description("Perfil Adminsitrativo (Não necessita de licenças para acessar o sistema)")]
            Nivel_III = 3
        }
    }
}
