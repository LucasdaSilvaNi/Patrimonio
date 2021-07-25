using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class MaterialClass
    {
        public MaterialClass()
        {
            this.Materials = new List<Material>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Grupo Material")]
        public int MaterialGroupId { get; set; }

        public bool Status { get; set; }

        public virtual MaterialGroup RelatedMaterialGroup { get; set; }
        public virtual ICollection<Material> Materials { get; set; }
    }
}
