using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class MaterialItem
    {
        public MaterialItem()
        {
            //this.Assets = new List<Asset>();
            this.Closings = new List<Closing>();
            this.MaterialSubItems = new List<MaterialSubItem>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição Item de Material")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Material")]
        public int MaterialId { get; set; }

        //public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<Closing> Closings { get; set; }
        public virtual Material RelatedMaterial { get; set; }
        
        public virtual ICollection<MaterialSubItem> MaterialSubItems { get; set; }
    }
}
