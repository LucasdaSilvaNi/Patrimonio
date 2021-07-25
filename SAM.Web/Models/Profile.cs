using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Profile
    {
        public Profile()
        {
            this.RelationshipProfilesLevels = new List<RelationshipProfileLevel>();
            this.RelationshipTransactionsProfiles = new List<RelationshipTransactionProfile>();
            this.RelationShipUsersProfiles = new List<RelationshipUserProfile>();
            this.RelationshipProfileManagedSystem = new List<RelationshipProfileManagedSystem>();
            this.RelationShipModulesProfiles = new List<RelationshipModuleProfile>();
        }

        public int Id { get; set; }
        public bool Status { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de 100 caracteres.")]
        public string Description { get; set; }

        public bool? flagPerfilMaster { get; set; }
        public virtual ICollection<RelationshipProfileLevel> RelationshipProfilesLevels { get; set; }
        public virtual ICollection<RelationshipTransactionProfile> RelationshipTransactionsProfiles { get; set; }
        public virtual ICollection<RelationshipUserProfile> RelationShipUsersProfiles { get; set; }
        public virtual ICollection<RelationshipProfileManagedSystem> RelationshipProfileManagedSystem { get; set; }
        public virtual ICollection<RelationshipModuleProfile> RelationShipModulesProfiles { get; set; }
    }
}
