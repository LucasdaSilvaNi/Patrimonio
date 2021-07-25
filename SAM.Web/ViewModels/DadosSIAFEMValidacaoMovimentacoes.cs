using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class DadosSIAFEMValidacaoMovimentacoes
    {
        public string mensagem { get; set; }
        public bool mensagemDeSuccesso { get; set; }
        public bool primeiraNLGeraPendecia { get; set; }

        public bool podeGerarPendenciaEstorno { get; set; }
        public bool NaoEstornouBP { get; set; }
    }
}