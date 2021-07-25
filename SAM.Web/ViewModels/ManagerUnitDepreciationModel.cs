using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class ManagerUnitDepreciationModel
    {
       
        public int ManagerUnitId { get; set; }
        [Display(Name ="Item Material")]
        public int? MaterialItemCode { get; set; }
        [Display(Name ="Conta Contábil")]
        public int? BookAccount { get; set; }
        [Display(Name = "Chapa")]
        public string NumberIdentification { get; set; }
        public bool UGEemAbrilDeDoisMilEVinte { get; set; }
    }
}