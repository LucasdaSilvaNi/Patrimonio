using PatrimonioBusiness.excel.abstrcts;
using PatrimonioBusiness.excel.exportacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.excel
{
    public class ExcelFactory
    {
        public static ExportarAbstract CreateVisaoGeral()
        {
            return new Exportar();
        }
    }
}
