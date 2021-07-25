using PatrimonioBusiness.visaogeral.entidades;
using PatrimonioBusiness.visaogeral.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.visaogeral.abstracts
{
    public abstract class VisaoGeralAbstract
    {
        protected IsolationLevel isolationLevel { get; set; }
        public VisaoGeralAbstract(IsolationLevel isolationLevel)
        {
            this.isolationLevel = isolationLevel;
        }
        public int TotalRegistros { get; protected set; }
        public abstract Task<List<VisaoGeral>> Gets(IParametro parametro);
        public abstract Task<DataTable> GetDataTable(IParametro parametro);
        public abstract Task<int[]> GetsNumeroNotification(IParametro parametro);
    }
}
