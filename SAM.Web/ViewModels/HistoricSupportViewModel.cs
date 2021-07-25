using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class HistoricSupportViewModel
    {
        public int SupportId { get; set; }
        public string UserCPF { get; set; }
        public string UserName { get; set; }
        public DateTime InclusionDate { get; set; }
        public string Observation { get; set; }
    }
}