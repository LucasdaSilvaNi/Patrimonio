using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public partial class ShortDescriptionItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Descrição Resumida do Item")]
        public string Description { get; set; }

        public List<Asset> Assets { get; private set; }
    }
}