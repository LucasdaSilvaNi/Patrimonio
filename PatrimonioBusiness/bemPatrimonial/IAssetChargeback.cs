using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.bemPatrimonial
{
    public interface IAssetChargeback
    {
        int IncorporationType { get; set; }
        int InitialId { get; set; }
        String InitialName { get; set; }
        String NumberIdentification { get; set; }
        int MaterialItemCode { get; set; }
        int GroupMaterialId { get; set; }
        int StateConservationId { get; set; }
        int AdministrativeUnitId { get; set; }
        int ResponsibleId { get; set; }
        String Processo { get; set; }
        Decimal ValueAcquisition { get; set; }
        DateTime AcquisitionDate { get; set; }
        String AssetType { get; set; }
    }
}
