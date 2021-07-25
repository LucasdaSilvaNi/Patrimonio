using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Module
    {
        public Module()
        {
            this.Modules = new List<Module>();
            this.Transactions = new List<Transaction>();
            this.RelationshipModulesProfiles = new List<RelationshipModuleProfile>();
        }

        public int Id { get; set; }

        [Display(Name = "Sistema")]
        public int ManagedSystemId { get; set; }
        public bool Status { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de 100 caracteres.")]
        public string Name { get; set; }

        [Display(Name = "Nome do Menu")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de 100 caracteres.")]
        public string MenuName { get; set; }

        [Display(Name="Descrição")]
        [StringLength(300, ErrorMessage = "Tamanho máximo de 300 caracteres.")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Caminho")]
        [StringLength(255, ErrorMessage = "Tamanho máximo de 255 caracteres.")]
        public string Path { get; set; }

        [Display(Name = "Menu Superior")]
        public Nullable<int> ParentId { get; set; }

        [Display(Name = "Sequencia")]
        public Nullable<int> Sequence { get; set; }

        public virtual ICollection<Module> Modules { get; set; }
        public virtual Module RelatedModule { get; set; }
        public virtual ManagedSystem RelatedManagedSystem { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<RelationshipModuleProfile> RelationshipModulesProfiles { get; set; }
        public virtual ICollection<Support> Supports { get; set; }
    }
}
