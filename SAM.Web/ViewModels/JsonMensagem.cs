using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class JsonMensagem
    {
        public string Chave { get; set; }
        public string Conteudo { get; set; }
        public object Dados { get; set; }
    }
}