using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class DadosGeraColetorBPViewModel
    {
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public int? IdUA { get; set; }
        public int CodigoUA { get; set; }
        public int CodigoItemMaterial { get; set; }
        public int IdEstadoConservacao { get; set; }
        public string NumeroDeSerie { get; set; }
        public string CPFResponsavel { get; set; }
        public int IdResponsavel { get; set; }
        public string pseudoCodigoResponsavel { get {
                if (string.IsNullOrEmpty(CPFResponsavel) || string.IsNullOrWhiteSpace(CPFResponsavel) || CPFResponsavel.Length < 11)
                    return null;

                return CPFResponsavel.Substring(7, 3);
            } }
        public string pseudoIdResponsavel { get; set; }
    }

    public class DadosGeraColetorResponsaveisViewModel
    {
        public int CodigoUA { get; set; }
        public int IdResponsavel { get; set; }
        public string NomeResponsavel { get; set; }
        public string CPFResponsavel { get; set; }
        public string CargoResponsavel { get; set; }
        public string pseudoCodigoResponsavel
        {
            get
            {
                if (string.IsNullOrEmpty(CPFResponsavel) || string.IsNullOrWhiteSpace(CPFResponsavel) || CPFResponsavel.Length < 11)
                    return null;

                return CPFResponsavel.Substring(7, 3);
            }
        }
        public string pseudoIdResponsavel { get; set; }
    }
}