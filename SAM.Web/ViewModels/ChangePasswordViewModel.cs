using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int userId { get; set; }
        [Required(ErrorMessage="Senha anterior é obrigatóaria")]
        [Display(Name="Senha anterior")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nova Senha é Obrigatório")]
        [DataType(DataType.Password)]
        [DisplayName("Nova Senha")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmar Nova Senha é obrigatório")]
        [StringLength(12, ErrorMessage = "Por favor digite uma senha com no mínimo 5 caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar Nova Senha")]
        [Compare("NewPassword", ErrorMessage = "O campo Nova Senha e Confirmar Nova Senha não são iguais ")]
        public string ConfirmPassword { get; set; }
    }
}