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
        [Display(Name = "Código")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "Código de Gestão")]
        [MaxLength(5, ErrorMessage = "O Código de Gestão é composto por 5 números")]
        public string ManagerCode { get; set; }

        public bool Status { get; set; }

        [Display(Name = "Descrição Resumida")]
        public string NameManagerReduced { get; set; }

        [Display(Name = "Orgão Implantado")]
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
