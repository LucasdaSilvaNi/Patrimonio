using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    [Serializable]
    public partial class User
    {
        public User()
        {
            this.Historics = new List<Historic>();
            this.RelationshipUsersProfiles = new List<RelationshipUserProfile>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "Tamanho máximo de 14 caracteres.", MinimumLength = 11)]
        public string CPF { get; set; }
        public bool Status { get; set; }
        public DateTime? DataUltimoTreinamento { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(200, ErrorMessage = "Tamanho máximo de 200 caracteres.")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "Tamanho máximo de 100 caracteres.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Senha")]
        [StringLength(50, ErrorMessage = "Tamanho máximo de 50 caracteres.")]
        [MinLength(8, ErrorMessage = "Tamanho mínimo de 8 caracteres.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Frase")]
        [StringLength(50, ErrorMessage = "Tamanho máximo de 50 caracteres.")]
        public string Phrase { get; set; }

        [Display(Name = "Data Cadastro")]
        public Nullable<DateTime> AddedDate { get; set; }

        [Display(Name = "Tentativas Inválidas")]
        public Nullable<int> InvalidAttempts { get; set; }

        [Display(Name = "Bloqueado")]
        public bool Blocked { get; set; }

        [Display(Name = "Trocar Senha")]
        public bool ChangePassword { get; set; }

        public virtual ICollection<Historic> Historics { get; set; }
        public virtual ICollection<RelationshipUserProfile> RelationshipUsersProfiles { get; set; }
        public virtual ICollection<Inventario> Inventarios { get; set; }
        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual ICollection<Support> Supports { get; set; }
        public virtual ICollection<HistoricSupport> HistoricSupports { get; set; }
        public virtual ICollection<LogAlteracaoDadosBP> LogAlteracaoDadosBPs { get; set; }
        public virtual ICollection<HistoricoValoresDecreto> HistoricoValoresDecretos { get; set; }
    }
}