using PatrimonioBusiness.visaogeral.entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.excel.abstrcts
{
    public abstract class ExportarAbstract
    {
        public abstract void ExportExcel(IList<VisaoGeral> visaoGerals, string destination);
        public abstract void ExportExcel(DataTable dataTable, string destination);
    }
}
