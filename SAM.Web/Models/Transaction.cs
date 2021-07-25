using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            this.RelationshipTransactionsProfiles = new List<RelationshipTransactionProfile>();
        }

        public int Id { get; set; }
        
        [Required(ErrorMessage="Campo M�dulo � obrigat�rio")]
        [Display(Name="M�dulo")]
        public int ModuleId { get; set; }
        public bool Status { get; set; }

        [Display(Name = "Sigla")]
        [StringLength(300, ErrorMessage = "Tamanho m�ximo de 300 caracteres.")]
        public string Initial { get; set; }
        
        [Required]
        [Display(Name="Descri��o")]
        [StringLength(100, ErrorMessage = "Tamanho m�ximo de 100 caracteres.")]
        public string Description { get; set; }
        
        [Required]
        [Display(Name = "Caminho")]
        [StringLength(255, ErrorMessage = "Tamanho m�ximo de 255 caracteres.")]
        public string Path { get; set; }

        [Required(ErrorMessage = "Campo Tipo de Transa��o � obrigat�rio")]
        [Display(Name = "Tipo de Transa��o")]
        public int TypeTransactionId { get; set; }

        public virtual Module RelatedModule { get; set; }
        public virtual TypeTransaction RelatedTypeTransaction { get; set; }
        public virtual ICollection<RelationshipTransactionProfile> RelationshipTransactionsProfiles { get; set; }
    }
}
