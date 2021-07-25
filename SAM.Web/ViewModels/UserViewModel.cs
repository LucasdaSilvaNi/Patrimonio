using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class UserViewModel : User
    {

        [Required(ErrorMessage = "Senha Atual é Obrigatório")]
        //[StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("Senha Atual")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Nova Senha é Obrigatório")]
        //[StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("Nova Senha")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmar Nova Senha é obrigatório")]
        //[StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar Nova Senha")]
        [Compare("NewPassword", ErrorMessage = "O campo Nova Senha e Confirmar Nova Senha não são iguais ")]
        public string ConfirmPassword { get; set; }

        
    }
}