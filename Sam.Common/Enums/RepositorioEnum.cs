using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sam.Common.Enums
{
    public class RepositorioEnum
    {
        public enum Repositorio
        {
            [Description("Repositório Principal")]
            Principal = 1,

            [Description("Repositório Complementar")]
            Complementar = 2
        }
    }
}
