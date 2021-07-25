using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class ItemSiafisico
    {
        public int ID { get; set; }
        public double Cod_Grupo { get; set; }
        public string Nome_Grupo { get; set; }
        public double Cod_Material { get; set; }
        public string Nome_Material { get; set; }
        public double Cod_Item_Mat { get; set; }
        public string Nome_Item_Mat { get; set; }
        public string STATUS { get; set; }
        public string BEC { get; set; }

    }
}