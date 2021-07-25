using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Material
    {
        public Material()
        {
            this.MaterialItems = new List<MaterialItem>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Classe Material")]
        public int MaterialClassId { get; set; }

        public bool Status { get; set; }

        public virtual MaterialClass RelatedMaterialClass { get; set; }
        public virtual ICollection<MaterialItem> MaterialItems { get; set; }
    }
}
