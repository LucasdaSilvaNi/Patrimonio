using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    [Serializable]
    public class RelationshipUserProfileInstitutionModel
    {
        public int Id{get;set;}
        public int ProfileId { get; set; }
        public string DescProfile { get; set; }
        public int? RelationshipUserProfileId { get; set; }
        public bool ProfilePadrao { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionCode { get; set; }
        public bool Status { get; set; }
        public int? ManagerId { get; set; }
        public int? BudgetUnitId { get; set; }
        public string BudgetUnitCode { get; set; }
        public int? ManagerUnitId { get; set; }
        public string ManagerUnitCode { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public int? AdministrativeUnitCode { get; set; }
        public int? SectionId { get; set; }
        public int? SectionCode { get; set; }
    }
}