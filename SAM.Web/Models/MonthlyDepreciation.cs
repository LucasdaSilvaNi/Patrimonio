using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class MonthlyDepreciation
    {

        public int Id { get; set; }
	    public int AssetStartId { get; set; }
	    public string NumberIdentification { get; set; }
        public int ManagerUnitId { get; set; }
	    public int MaterialItemCode { get; set; }
	    public int InitialId { get; set; }
	    public DateTime AcquisitionDate { get; set; }
        public DateTime CurrentDate { get; set; }
	    public DateTime DateIncorporation { get; set; }
        public short LifeCycle { get; set; }
        public short CurrentMonth { get; set; }
        public decimal ValueAcquisition { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal ResidualValue { get; set; }
        public decimal RateDepreciationMonthly { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal? UnfoldingValue { get; set; }
        public bool Decree { get; set; }
        public int? ManagerUnitTransferId { get; set; }
        public int? MonthlyDepreciationId { get; set; }
        public bool? MesAbertoDoAceite { get; set; }
        public int QtdLinhaRepetida { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }

        public MonthlyDepreciation Clone()
        {
            MonthlyDepreciation monthlyDepreciation = new MonthlyDepreciation()
            {
                AccumulatedDepreciation = this.AccumulatedDepreciation,
                AcquisitionDate = this.AcquisitionDate,
                AssetStartId = this.AssetStartId,
                CurrentDate = this.CurrentDate,
                CurrentMonth = this.CurrentMonth,
                CurrentValue = this.CurrentValue,
                DateIncorporation = this.DateIncorporation,
                Decree = this.Decree,
                InitialId = this.InitialId,
                LifeCycle = this.LifeCycle,
                ManagerUnitId = this.ManagerUnitId,
                ManagerUnitTransferId = this.ManagerUnitTransferId,
                MaterialItemCode = this.MaterialItemCode,
                MonthlyDepreciationId = this.MonthlyDepreciationId,
                NumberIdentification = this.NumberIdentification,
                RateDepreciationMonthly = this.RateDepreciationMonthly,
                RelatedManagerUnit = this.RelatedManagerUnit,
                ResidualValue = this.ResidualValue,
                UnfoldingValue = this.UnfoldingValue,
                ValueAcquisition = this.ValueAcquisition,
                QtdLinhaRepetida = this.QtdLinhaRepetida
            };

            return monthlyDepreciation;
        }
    }
}