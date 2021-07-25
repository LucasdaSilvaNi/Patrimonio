using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class ManagedSystem
    {
        public ManagedSystem()
        {
            this.Modules = new List<Module>();
            this.RelationshipProfileManagedSystem = new List<RelationshipProfileManagedSystem>();
        }

        public int Id { get; set; }
        public bool Status { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(20, ErrorMessage = "Tamanho máximo de 20 caracteres.")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name="Descrição")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de 100 caracteres.")]
        public string Description { get; set; }

        [Display(Name = "Sequencia")]
        public Nullable<int> Sequence { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<RelationshipProfileManagedSystem> RelationshipProfileManagedSystem { get; set; }
    }
}
