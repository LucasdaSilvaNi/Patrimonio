using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    public class RelationshipModuleProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Módulo é obrigatótio")]
        [Display(Name = "Módulo")]
        public int IdModulo { get; set; }

        [Required(ErrorMessage = "Campo Perfil é obrigatótio")]
        [Display(Name = "Perfil")]
        public int IdPerfil { get; set; }

        [Display(Name = "Módulo")]
        public string NomeModulo { get; set; }

        [Display(Name = "Perfil")]
        public string DescricaoPerfil { get; set; }

        public bool Status { get; set; }
    }
}