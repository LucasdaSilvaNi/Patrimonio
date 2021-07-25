using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sam.Common.Enums
{
    public static class UsuarioNivelEnum
    {
        public enum UsuarioNivel
        {
            [Description("Usuário Nível I")]
            Nivel_I = 1,

            [Description("Usuário Nível II")]
            Nivel_II = 2
        }
    }
}
