using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class ErroLog
    {
        public int Id { get; set; }
        public string exMessage { get; set; }
        public string exStackTrace { get; set; }
        public string exTargetSite { get; set; }
        public DateTime DataOcorrido { get; set; }
        public string Usuario { get; set; }
    }
}