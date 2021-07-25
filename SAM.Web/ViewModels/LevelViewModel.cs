using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    public class LevelViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(30, ErrorMessage = "Tamanho máximo de 30 caracteres.")]
        public string Descricao { get; set; }

        [Display(Name = "Nível Superior")]
        public int? IdNivelSuperior { get; set; }

        [Display(Name = "Nível Superior")]
        public string NomeNivelSuperior { get; set; }
    }
}