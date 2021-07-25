using PatrimonioBusiness.fechamento.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.entidades
{
    internal class Depreciacao : IDepreciacao
    {
        internal Depreciacao() { }

        internal static Depreciacao GetInstancia()
        {
            return new entidades.Depreciacao();
        }
        internal static IDepreciacao ConverterDataRowParaIDepreciacao(DataRow row)
        {
            IDepreciacao depreciacao = new Depreciacao();

            depreciacao.AccumulatedDepreciation =decimal.Parse(row["AccumulatedDepreciation"].ToString());
            depreciacao.AcquisitionDate = DateTime.Parse(row["AcquisitionDate"].ToString());
            depreciacao.AssetStartId = int.Parse(row["AssetStartId"].ToString());
            depreciacao.CurrentDate =DateTime.Parse(row["CurrentDate"].ToString());
            depreciacao.CurrentMonth = Int16.Parse(row["CurrentMonth"].ToString());
            depreciacao.DateIncorporation =DateTime.Parse(row["DateIncorporation"].ToString());
            depreciacao.Decree = bool.Parse(row["Decree"].ToString());
            depreciacao.Id = int.Parse(row["Id"].ToString());
            depreciacao.InitialId = int.Parse(row["InitialId"].ToString());
            depreciacao.LifeCycle = Int16.Parse(row["LifeCycle"].ToString());
            depreciacao.ManagerUnitId = int.Parse(row["ManagerUnitId"].ToString());
            depreciacao.MaterialItemCode =int.Parse(row["MaterialItemCode"].ToString());
            depreciacao.MonthlyDepreciationId = int.Parse(row["MonthlyDepreciationId"].ToString());
            depreciacao.NumberIdentification = row["NumberIdentification"].ToString();
            depreciacao.RateDepreciationMonthly = decimal.Parse(row["RateDepreciationMonthly"].ToString());
            depreciacao.ResidualValue =decimal.Parse(row["ResidualValue"].ToString());
            depreciacao.UnfoldingValue =(row["UnfoldingValue"] == DBNull.Value ? 0: decimal.Parse(row["UnfoldingValue"].ToString()));
            depreciacao.ValueAcquisition = decimal.Parse(row["ValueAcquisition"].ToString());

            return depreciacao;

        }
        public decimal AccumulatedDepreciation
        {
            get;

            set;
        }

        public DateTime AcquisitionDate
        {
            get;
            set;
           
        }

        public int AssetStartId
        {
            get;
            set;
        }

        public DateTime CurrentDate
        {
            get;
            set;
        }

        public short CurrentMonth
        {
            get;
            set;
        }

        public DateTime DateIncorporation
        {
            get;
            set;
        }

        public bool Decree
        {
            get;
            set;
        }

        public long Id
        {
            get;
            set;
        }

        public int InitialId
        {
            get;
            set;
        }

        public short LifeCycle
        {
            get;
            set;
        }

        public int ManagerUnitId
        {
            get;
            set;
        }

        public int ManagerUnitTransferId
        {
            get;
            set;
        }

        public int MaterialItemCode
        {
            get;
            set;
        }

        public int MonthlyDepreciationId
        {
            get;
            set;
        }

        public string NumberIdentification
        {
            get;
            set;
        }

        public decimal RateDepreciationMonthly
        {
            get;
            set;
        }

        public decimal ResidualValue
        {
            get;
            set;
        }

        public decimal UnfoldingValue
        {
            get;
            set;
        }

        public decimal ValueAcquisition
        {
            get;
            set;
        }
    }
}
