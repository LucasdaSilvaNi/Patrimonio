using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class RelationshipUserProfileInstitution
    {
        public int Id { get; set; }
        public int RelationshipUserProfileId { get; set; }

        [Required]
        [Display(Name = "Órgão")]
        public int InstitutionId { get; set; }
        public bool Status { get; set; }
        public int? BudgetUnitId { get; set; }
        public int? ManagerUnitId { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public int? SectionId { get; set; }

        public virtual Institution RelatedInstitution { get; set; }
        public virtual BudgetUnit RelatedBudgetUnit { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        public virtual AdministrativeUnit RelatedAdministrativeUnit { get; set; }
        public virtual Section RelatedSection { get; set; }
        public virtual RelationshipUserProfile RelatedRelationshipUserProfile { get; set; }
    }
}
