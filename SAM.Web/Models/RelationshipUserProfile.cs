using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class RelationshipUserProfile
    {
        public RelationshipUserProfile()
        {
            this.RelationshipUsersProfilesInstitutions = new List<RelationshipUserProfileInstitution>();
        }

        public int Id { get; set; }

        [Display(Name = "Usuário")]
        public int UserId { get; set; }

        [Display(Name = "Perfil")]
        public int ProfileId { get; set; }
        
        [Display(Name = "Perfil Padrão")]
        public bool DefaultProfile { get; set; }

        [Display(Name = "Perfil Responsavel?")]
        public bool? FlagResponsavel { get; set; }
        public virtual Profile RelatedProfile { get; set; }
        public virtual ICollection<RelationshipUserProfileInstitution> RelationshipUsersProfilesInstitutions { get; set; }
        public virtual User RelatedUser { get; set; }
    }
}
