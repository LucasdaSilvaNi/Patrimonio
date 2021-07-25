using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class Mobile
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "O tamanho máximo desse campo é de 50 caracteres.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "O tamanho máximo desse campo é de 50 caracteres.")]
        [Display(Name = "Marca")]
        public string Brand { get; set; }

        [StringLength(50, ErrorMessage = "O tamanho máximo desse campo é de 50 caracteres.")]
        [Display(Name = "Modelo")]
        public string Model { get; set; }

        [StringLength(50, ErrorMessage = "O tamanho máximo desse campo é de 50 caracteres.")]
        [Display(Name = "MAC Address")]
        public string MacAddress { get; set; }

        [Display(Name = "Órgão")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        public int BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        public int ManagerUnitId { get; set; }

        [Display(Name = "UA")]
        public int AdministrativeUnitId { get; set; }

        public bool Status { get; set; }

        public Institution RelatedInstitutions { get; set; }
        public BudgetUnit RelatedBudgetUnits { get; set; }
        public ManagerUnit RelatedManagerUnits { get; set; }
        public AdministrativeUnit RelatedAdministrativeUnits { get; set; }
    }
}