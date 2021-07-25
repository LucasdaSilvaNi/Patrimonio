using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.visaogeral.interfaces
{
    /// <summary>
    /// Paramentros para execução da procedure "SAM_VISAO_GERAL_ATIVO"
    /// </summary>
    public interface IParametro
    {
        string AssetId { get; set; }
        int? InstitutionId { get; set; }
        int? BudgetUnitId { get; set; }
        int? ManagerUnitId { get; set; }
        int? AdministrativeUnitId { get; set; }
        string CPF { get; set; }
        int? Size { get; set; }
        int? PageNumber { get; set; }
        byte? Estado { get; set; }
        string Filtro { get; set; }
        string Campo { get; set; }
        string CampoOrder { get; set; }
        string OrderDirecao { get; set; }
    }
}
