using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class InventarioViewModel: Inventario
    {
        public InventarioViewModel()
        {
            this.InventariosList = new List<Inventario>();
        }

        [Display(Name = "Órgão")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        public int BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        public int ManagerUnitId { get; set; }

        [Display(Name = "UA")]
        [Required(ErrorMessage= "O Campo UA é obrigatório")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UA é obrigatório")]
        public int AdministrativeUnitId { get; set; }


        public List<Inventario> InventariosList { get; set; }

        [Display(Name = "Divisão")]
        public int SectionId { get; set; }

        public int? idResponsable { get; set; }
    }
}