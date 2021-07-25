using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Legado
{
    public class RetornoImportacao
    {
        public string mensagemImportacao { get; set; }
        public int quantidadeDeRegistros { get; set; }
        public string caminhoDoArquivoDownload { get; set; }
    }
}