using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class GroupMoviment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code")]
        public int Code { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(200, ErrorMessage = "Tamanho máximo de 200 caracteres.")]
        public string Description { get; set; }

        public virtual ICollection<MovementType> MovementTypes { get; set; }
    }
}
