using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class UserModel : User
    {
        public List<Profile> Profiles { get; set; }
        public List<Institution> Institutions { get; set; }
        public List<Manager> Managers { get; set; }
        public List<BudgetUnit> BudgetUnits { get; set; }
        public List<ManagerUnit> ManagerUnits { get; set; }
        public List<AdministrativeUnit> AdministrativeUnits { get; set; }
        public List<Section> Sections { get; set; }
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public String AddUserPerfil { get; set; }
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        [Required(ErrorMessage = "Relacionar um perfil para o usuário")]
        public string relationshipUserProfileInstitution { get; set; }

        [Display(Name = "Trocar Senha")]
        [StringLength(50, ErrorMessage = "Tamanho máximo de 50 caracteres.")]
        [MinLength(8, ErrorMessage = "Tamanho mínimo de 8 caracteres.")]
        public string PasswordNew { get; set; }

        public string Orgao { get; set; }
        public string Uo { get; set; }
        public string Uge { get; set; }
        public bool AdmGeral { get; set; }

    }
}