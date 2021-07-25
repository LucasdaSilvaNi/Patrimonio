using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class ModuleViewModel
    {
        public int Id { get; set; }

        public int ManagedSystemId { get; set; }
        public bool Status { get; set; }
        public string Name { get; set; }
        public string MenuName { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> Sequence { get; set; }
    }

    public class ModuloTelaViewModel {
        public int Id { get; set; }

        [Display(Name = "Menu")]
        public int IdMenu { get; set; }

        [Display(Name = "Menu")]
        public string NomeMenu { get; set; }

        public int? IdModuloSuperior { get; set; }

        [Display(Name = "Módulo Superior")]
        public string NomeModuloSuperior { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [Display(Name = "Módulo")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de 100 caracteres.")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo caminho é obrigatório")]
        [Display(Name = "Caminho")]
        [StringLength(255, ErrorMessage = "Tamanho máximo de 255 caracteres.")]
        public string Caminho { get; set; }

        [Display(Name = "Sequencia")]
        public int? Sequencia { get; set; }

        [Display(Name = "Módulo Ativo?")]
        public bool Status { get; set; }
    }
}
