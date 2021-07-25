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
            /// Perfil N�vel I
            /// </summary>
            [Description("Perfil N�vel I")]
            Nivel_I = 1,

            /// <summary>
            /// Perfil N�vel II
            /// </summary>
            [Description("Perfil N�vel II")]
            Nivel_II = 2,

            /// <summary>
            /// Perfil Adminsitrativo (N�o necessita de contrato para acessar)
            /// </summary>
            [Description("Perfil Adminsitrativo (N�o necessita de licen�as para acessar o sistema)")]
            Nivel_III = 3
        }
    }
}
