using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class Historic
    {

        public int Id { get; set; }

        [Display(Name = "Usuário")]
        public int UserId { get; set; }

        [Display(Name = "Data Inclusão")]
        public DateTime InsertDate { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name="Complemento")]
        public string Description { get; set; }
        public virtual User RelatedUsers { get; set; }
    }
}
