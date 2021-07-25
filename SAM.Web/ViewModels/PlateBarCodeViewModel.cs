using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class PlateBarCodeViewModel
    {
        public PlateBarCodeViewModel()
        {
            ListAssets = new List<PlateBarAssetsViewModel>();
        }

        public int InstitutionId { get; set; }
        public int? BudgetUnitId { get; set; }
        public int? ManagerUnitId { get; set; }
        public string briefDescription { get; set; }
        public string dataInicioRef { get; set; }
        public string dataFimRef { get; set; }
        public int? numChapaInicioRef { get; set; }
        public int? numChapaFimRef { get; set; }
        public bool geraNovasChapas { get; set; }
        public int? numChapaInicioGerar { get; set; }
        public int? numChapaFimGerar { get; set; }
        public string ListAssetsSelected { get; set; }
        public short TypeReading { get; set; }
        public string ColorPlate { get; set; }
        public string Contenha { get; set; }
        public List<PlateBarAssetsViewModel> ListAssets { get; set; }
    }

    [NotMapped]
    public class PlateBarAssetsViewModel
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public string ChapaCompleta { get; set; }
        [Display(Name = "Descrição de Item Material")]
        public string MaterialItemDescription { get; set; }
    }
}