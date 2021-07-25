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
    public partial class AnaliticoDeBemPatrimonialViewModel
    {
        public AnaliticoDeBemPatrimonialViewModel()
        {
            ListAssets = new List<AssetAnaliticoBemPatrimonialViewModel>();
        }


        public int InstitutionId { get; set; }
        public int? BudgetUnitId { get; set; }
        public int? ManagerUnitId { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public int? SectionId { get; set; }

        public bool? StatusAsset { get; set; }

        public string NumberIdentification { get; set; }
        public string ListAssetsSelected { get; set; }

        public List<AssetAnaliticoBemPatrimonialViewModel> ListAssets { get; set; }
    }

    [NotMapped]
    public partial class AssetAnaliticoBemPatrimonialViewModel
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public string ChapaCompleta { get; set; }
        [Display(Name = "Descrição de Item Material")]
        public string MaterialItemDescription { get; set; }
    }
}