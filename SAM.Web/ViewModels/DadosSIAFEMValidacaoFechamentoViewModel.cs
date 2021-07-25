using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class DadosSIAFEMValidacaoFechamentoViewModel
    {
        public List<DadosSiafemTabelaViewModel> dadosPorContaContabil { get; set; }
        public List<DadosSiafemTabelaViewModel> dadosPorContaDepreciacao { get; set; }

        public List<string> LstMsgsPendencias { get; set; }

        public string MensagemDeErro { get; set; }

        public bool primeiraNLComErro { get; set; }

        public bool gerouPendencia { get; set; }
    }

    public class DadosSiafemTabelaViewModel {
        public string NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string ValorFormatado { get {
                return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", this.Valor);
            } }
        public string NL { get; set; }
        public int AuditoriaIdEstorno { get; set; }
    }
}