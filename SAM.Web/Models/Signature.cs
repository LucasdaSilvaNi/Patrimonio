using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Signature
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        
        [Display(Name="Função CAMEX")]
        public string PositionCamex { get; set; }


        [Display(Name = "Posição")]
        public string Post { get; set; }

        public bool Status { get; set; }

    }
}
