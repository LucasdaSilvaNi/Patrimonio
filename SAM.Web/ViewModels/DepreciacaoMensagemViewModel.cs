using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class DepreciacaoMensagemViewModel
    {
        public int AssetId { get; set; }
        public int AssetStartId { get; set; }
        public int MaterialItemCode { get; set; }
        public String NumberIdentification { get; set; }
        public String AcquisitionDate { get; set; }
        public String MovimentDate { get; set; }
        public int BookAccount { get; set; }
        public String Mensagem { get; set; }
        public bool Erro { get; set; }
        public int quantidadeDepreciada { get; set; }
        public string MesReferencia { get; set; }
        public string Tipo { get; set; }
    }
}