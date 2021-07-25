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
    public partial class ResponsibleViewModel : Responsible
    {
        [Display(Name = "Órgão")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO é obrigatório")]
        public int BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int ManagerUnitId { get; set; }

        [Display(Name = "Divisão")]
        public int SectionId { get; set; }

        public int InstitutionIdAux { get; set; }
        public int BudgetUnitIdAux { get; set; }
        public int ManagerUnitIdAux { get; set; }
        public int AdministrativeUnitIdAux { get; set; }
    }

    [NotMapped]
    public partial class ResponsibleMobileViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
    }
}