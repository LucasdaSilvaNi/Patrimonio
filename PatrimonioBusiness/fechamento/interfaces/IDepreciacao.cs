using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.interfaces
{
    public interface IDepreciacao
    {
        Int64 Id { get; set; }
        int AssetStartId { get; set; }
        String NumberIdentification { get; set; }
        int ManagerUnitId { get; set; }
        int MaterialItemCode { get; set;}
        int InitialId { get; set; }
        DateTime AcquisitionDate { get; set; }
        DateTime CurrentDate { get; set; }
        DateTime DateIncorporation { get; set; }
        Int16 LifeCycle { get; set; }
        Int16 CurrentMonth { get; set; }
        Decimal ValueAcquisition { get; set; }
        Decimal ResidualValue { get; set; }
        Decimal RateDepreciationMonthly { get; set; }
        Decimal AccumulatedDepreciation { get; set; }
        Decimal UnfoldingValue { get; set;}
        Boolean Decree { get; set; }
        int ManagerUnitTransferId { get; set; }
        int MonthlyDepreciationId { get; set; }
    }
}
