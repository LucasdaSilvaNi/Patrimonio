using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Login")]
        public string CPF { get; set; }

        [Required]
        public string Senha { get; set; }

        public string Frase { get; set; }

    }
}