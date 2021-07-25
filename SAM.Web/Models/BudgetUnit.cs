using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class BudgetUnit
    {
        public BudgetUnit()
        {
            this.AssetMovements = new List<AssetMovements>();
            this.ManagerUnits = new List<ManagerUnit>();
            this.OutSourceds = new List<OutSourced>();
            this.Initials = new List<Initial>();
            this.Mobiles = new List<Mobile>();
        }

        public int Id { get; set; }
        
        [Required]
        [Display(Name="Orgao")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Órgão é obrigatório")]
        public int InstitutionId { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "UO Superior?")]
        public bool Direct { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual Institution RelatedInstitution { get; set; }
        public virtual ICollection<RelationshipUserProfileInstitution> RelationshipUsersProfilesInstitutions { get; set; }
        public virtual ICollection<ManagerUnit> ManagerUnits { get; set; }
        public virtual ICollection<OutSourced> OutSourceds { get; set; }
        public virtual ICollection<Initial> Initials { get; set; }
        public virtual ICollection<Mobile> Mobiles { get; set; }
        public virtual ICollection<Support> Supports { get; set; }
    }
}
