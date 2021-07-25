using PatrimonioBusiness.fechamento.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.entidades
{
    internal sealed class SimulacaoResultado: ISimulacaoResultado
    {
        internal SimulacaoResultado(IList<IDepreciacao> depreciacoes, IList<IDepreciacaoErro> depreciacoesErros)
        {
            this.Depreciacoes = depreciacoes;
            this.DepreciacoesErros = depreciacoesErros;
        }
        public IList<IDepreciacao> Depreciacoes { get; private set; }
        public IList<IDepreciacaoErro> DepreciacoesErros { get; private set; }
    }
}
