using PatrimonioBusiness.integracao.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.integracao.entidades
{
    public class IntegracaoDepreciacao : IIntegracaoDepreciacao
    {
        public int AssetStartId
        {
            get;set;
        }

        public int CodigoDaUge
        {
            get; set;
        }

        public DateTime DataFinal
        {
            get; set;
        }
    }
}
