using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class InstitutionViewModel
    {
        public InstitutionViewModel()
        {
            this.Managers = new List<ManagerViewModel>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public string Codigo { get; set; }
        public string DescricaoResumida { get; set; }
        public string CodigoGestao { get; set; }
        public bool OrgaoImplantado { get; set; }
        public bool IntegradoComSiafem { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<ManagerViewModel> Managers { get; set; }
    }
}