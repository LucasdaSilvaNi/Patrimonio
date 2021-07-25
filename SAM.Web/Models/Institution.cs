using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Institution
    {
        public Institution()
        {
            this.AssetMovements = new List<AssetMovements>();
            this.BudgetUnits = new List<BudgetUnit>();
            this.RelationshipUsersProfilesInstitutions = new List<RelationshipUserProfileInstitution>();
            this.OutSourceds = new List<OutSourced>();
            this.Initials = new List<Initial>();
            this.Mobiles = new List<Mobile>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "C�digo")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Descri��o")]
        public string Description { get; set; }

        [Display(Name = "C�digo de Gest�o")]
        [MaxLength(5, ErrorMessage = "O C�digo de Gest�o � composto por 5 n�meros")]
        public string ManagerCode { get; set; }

        public bool Status { get; set; }

        [Display(Name = "Descri��o Resumida")]
        public string NameManagerReduced { get; set; }

        [Display(Name = "Org�o Implantado")]
        public bool flagImplantado { get; set; }

        public bool flagIntegracaoSiafem { get; set; }

        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual ICollection<BudgetUnit> BudgetUnits { get; set; }
        public virtual ICollection<RelationshipUserProfileInstitution> RelationshipUsersProfilesInstitutions { get; set; }
        public virtual ICollection<OutSourced> OutSourceds { get; set; }
        public virtual ICollection<Initial> Initials { get; set; }
        public virtual ICollection<Mobile> Mobiles { get; set; }
        public virtual ICollection<Support> Supports { get; set; }
    }
}
