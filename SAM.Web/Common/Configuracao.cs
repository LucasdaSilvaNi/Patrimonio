using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Common
{
    internal static class Configuracao
    {
        internal static String converterObjetoParaJSON<T>(T objeto)
        {
            var resultado = JsonConvert.SerializeObject(objeto, Formatting.Indented,
                                                                                    new JsonSerializerSettings
                                                                                    {
                                                                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                                                        NullValueHandling = NullValueHandling.Ignore
                                                                                        
                                                                                       
                                                                                    });

            return resultado;

        }
    }
}