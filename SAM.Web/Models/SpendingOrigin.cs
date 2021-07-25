using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class SpendingOrigin
    {
        public SpendingOrigin()
        {
            this.MaterialSubItems = new List<MaterialSubItem>();
        }

        public int Id { get; set; }
        
        [Required]
        [Display(Name="Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Indicador Atividade")]
        public bool ActivityIndicator { get; set; }

        public bool Status { get; set; }
        public virtual ICollection<MaterialSubItem> MaterialSubItems { get; set; }
    }
}
