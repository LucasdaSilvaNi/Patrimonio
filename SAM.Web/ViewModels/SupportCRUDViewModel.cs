using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class SupportCRUDViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "�rg�o")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo �rg�o � obrigat�rio")]
        public int InstitutionId { get; set; }
        [Required]
        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO � obrigat�rio")]
        public int BudgetUnitId { get; set; }
        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE � obrigat�rio")]
        public int ManagerUnitId { get; set; }
        public string UserDescription { get; set; }
        public string UserCPF { get; set; }
        public int PerfilId { get; set; }
        public string UserPerfil { get; set; }
        public int SupportStatusProdespId { get; set; }
        public DateTime InclusionDate { get; set; }
        public DateTime? CloseDate { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "O campo E-Mail n�o � um endere�o de email v�lido.")]
        [StringLength(100, ErrorMessage = "Tamanho m�ximo de 100 caracteres.")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
        public int SupportStatusUserId { get; set; }
        [Required]
        [Display(Name = "Respons�vel")]
        public string Responsavel { get; set; }
        [Required]
        [Display(Name = "Funcionalidade")]
        public int ModuleId { get; set; }
        public string Login { get; set; }
        public DateTime DataLogin { get; set; }
        [Required]
        [Display(Name = "Tipo de Suporte")]
        public int SupportTypeId { get; set; }
        [Required]
        [Display(Name = "Observa��o")]
        public string Observation { get; set; }
    }
}
