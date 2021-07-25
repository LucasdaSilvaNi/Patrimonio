using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        public string CorpoMensagem { get; set; }
        [Display(Name="Data de criação")]
        public DateTime? DataCriacao { get; set; }
        public bool Status { get; set; }
    }
}
