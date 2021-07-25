using System;
using System.Collections.Generic;

namespace SAM.Web.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public byte[] CorpoMensagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Status { get; set; }
    }
}
