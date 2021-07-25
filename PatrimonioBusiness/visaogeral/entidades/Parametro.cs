using PatrimonioBusiness.visaogeral.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.visaogeral.entidades
{
    public class Parametro : IParametro
    {
        private Parametro() { }
        public static Parametro GetInstancia()
        {
            return new Parametro();
        }
        public string AssetId { get; set; }
        public int? InstitutionId { get; set; }
        public int? BudgetUnitId { get; set; }
        public int? ManagerUnitId { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public string CPF { get; set; }
        public int? Size { get; set; }
        public int? PageNumber { get; set; }
        public byte? Estado { get; set; }
        public string Filtro { get; set; }
        public string Campo { get; set; }
        public string CampoOrder { get ; set ; }
        public string OrderDirecao { get ; set; }
    }
}
