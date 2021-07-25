using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.interfaces
{
    public interface ISimulacaoResultado
    {
        IList<IDepreciacao> Depreciacoes { get; }
        IList<IDepreciacaoErro> DepreciacoesErros { get; }
    }
}
