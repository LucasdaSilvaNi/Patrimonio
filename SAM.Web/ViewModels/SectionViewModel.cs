using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public partial class SectionViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public bool Status { get; set; }
    }

    [NotMapped]
    public partial class SectionsViewModel : Section
    {
        [Display(Name = "Órgão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Orgão é obrigatório")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO é obrigatório")]
        public int BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int ManagerUnitId { get; set; }

        public int? ResponsibleIdAux { get; set; }
        public bool MensagemModal { get; set; }

        [Display(Name = "Qtd de BPs")]
        public int Bp { get; set; }

        public string Responsavel { get; set; }
        public int AdministrativeUnitIdAux { get; set; }

        public string CodigoUO { get; set; }
        public string CodigoUGE { get; set; }
        public string CodigoOrgao { get; set; }
        public int CodigoUA { get; set; }
        public string DescricaoUA { get; set; }

        public SectionsViewModel() { }

        public SectionsViewModel(Section section)
        {
            Id = section.Id;
            RelatedAddress = section.RelatedAddress;
            RelatedAdministrativeUnit = section.RelatedAdministrativeUnit;
            RelatedResponsible = section.RelatedResponsible;
            Code = section.Code;
            Description = section.Description;
            AddressId = section.AddressId;
            Telephone = section.Telephone;
            Status = section.Status;
            ResponsibleId = section.ResponsibleId;
            ResponsibleIdAux = section.ResponsibleId;
            InstitutionId = section.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id;
            BudgetUnitId = section.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id;
            ManagerUnitId = section.RelatedAdministrativeUnit.RelatedManagerUnit.Id;
            AdministrativeUnitId = section.AdministrativeUnitId;
            RelatedAdministrativeUnit = section.RelatedAdministrativeUnit;
        }
    }

    public partial class SectionMobileViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
    }
}