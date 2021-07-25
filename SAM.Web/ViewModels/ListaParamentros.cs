using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Web.ViewModels
{
    public class ListaParamentros
    {
        public string nomeParametro { get; set; }
        public object valor { get; set; }
    }

    public class ListMesRef
    {
        public string mesRef { get; set; }
        public DateTime dateValue { get; set; }
    }
}
