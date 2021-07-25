using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.integracao.interfaces
{
    public interface IIntegracaoDepreciacao
    {
        int CodigoDaUge { get; set; }
        DateTime DataFinal { get; set; }
        int AssetStartId { get; set; }
    }
}
