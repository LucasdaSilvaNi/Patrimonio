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
    public partial class AdministrativeUnitViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Órgão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Órgão é obrigatório")]
        public int InstitutionId { get; set; }

        public int InstitutionIdAux { get; set; }

        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO é obrigatório")]
        public int BudgetUnitId { get; set; }

        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int ManagerUnitId { get; set; }

        public int BudgetUnitIdAux { get; set; }
        public int ManagerUnitIdAux { get; set; }

        public string ManagerUnitCode { get; set; }
        public string ManagerUnitDescription { get; set; }

        public AdministrativeUnitViewModel()
        {

        }

        public AdministrativeUnitViewModel(AdministrativeUnit administrativeUnit)
        {
            Id = administrativeUnit.Id;
            Code = administrativeUnit.Code;
            Description = administrativeUnit.Description;
            InstitutionId = administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
            BudgetUnitId = administrativeUnit.RelatedManagerUnit.BudgetUnitId;
            ManagerUnitId = administrativeUnit.RelatedManagerUnit.Id;
        }
    }

    [NotMapped]
    public partial class AdministrativeUnitMobileViewModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Description { get; set; }
    }


}