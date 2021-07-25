using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.OpenXml
{
    public class ExcelStatus
    {
        public String Mensagem { get; set; }
        public bool  Sucesso
        {
            get { return String.IsNullOrWhiteSpace(Mensagem); }
            private set { }
        }
    }
}