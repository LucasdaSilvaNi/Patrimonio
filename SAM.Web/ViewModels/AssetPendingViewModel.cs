using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class AssetPendingViewModel
    {
        public int Id { get; set; }
        public string Sigla { get; set; }

        public string Chapa { get; set; }

        public int CodigoMaterial { get; set; }

        public string DescricaoMaterial { get; set; }

        public string UGE { get; set; }

        public int UA { get; set; }

        public decimal? ValorAquisicao { get; set; }

        public string TaxaDepreciacaoMensal { get; set; }
    }
}