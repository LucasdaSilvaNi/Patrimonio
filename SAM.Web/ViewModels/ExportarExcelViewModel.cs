using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class ExportarExcelViewModelBPPendentes
    {
        public String SIGLA { get; set; }
        public String CHAPA { get; set; }
        public int CODIGO_ITEM { get; set; }
        public String DESCRICAO_ITEM { get; set; }
        public string UGE { get; set; }
        public int? UA { get; set; }
        public string VALOR_AQUISICAO { get; set; }
        public string DEPRECIACAO_MENSAL { get; set; }
    }
}