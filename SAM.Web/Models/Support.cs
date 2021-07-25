using Newtonsoft.Json;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Support
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public int BudgetUnitId { get; set; }
        public int ManagerUnitId { get; set; }
        public int UserId { get; set; }
        public int SupportStatusProdespId { get; set; }
        public DateTime InclusionDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Email { get; set; }
        public int SupportStatusUserId { get; set; }
        public string Responsavel { get; set; }
        public int ModuleId { get; set; }
        public string Login { get; set; }
        public DateTime DataLogin { get; set; }
        public int SupportTypeId { get; set; }
        public string Observation { get; set; }
        public bool Status { get; set; }


        public virtual Institution RelatedInstituation { get; set; }
        public virtual BudgetUnit RelatedBudgetUnit { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        public virtual User RelatedUser { get;set; }
        public virtual SupportStatusProdesp RelatedSupportStatusProdesp { get; set; }
        public virtual SupportStatusUser RelatedSupportStatusUser { get; set; }
        public virtual Module RelatedModule { get; set; }
        public virtual SupportType RelatedSupportType { get; set; }

        public virtual ICollection<AttachmentSupport> AttachmentSupports { get; set; }
        public virtual ICollection<HistoricSupport> HistoricSupports { get; set; }
    }
}
