using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class BolsaEstadualViewModel
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public int Item { get; set; }
        public string DescricaoDoItem { get; set; }
        public string Orgao { get; set; }
        public string Gestor { get; set; }
        public string UO { get; set; }
        public string UGE { get; set; }
        public string DescricaoUGE { get; set; }
        public bool Selecionado { get; set; }
        public int InstitutionId { get; set; }
        public int BudgetUnitId { get; set; }
        public int ManagerUnitId { get; set; }
    }
}

//public string Initial { get; set; }

    //public string NumberIdentification { get; set; }

    //public string MaterialItem { get; set; }

    //public string Institution { get; set; }

    //public string Manager  { get; set; }

    //public string BudgetUnit { get; set; }

    //public string ManagerUnit { get; set; }



