using PatrimonioBusiness.bases;
using PatrimonioBusiness.visaogeral.abstracts;
using PatrimonioBusiness.visaogeral.business;
using PatrimonioBusiness.visaogeral.contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.visaogeral
{
    public class VisaoGeralFactory
    {
        private DbConnection dbConnection = null;
        private VisaoGeralFactory() { }

        public static VisaoGeralFactory GetInstancia()
        {
            return new VisaoGeralFactory();
        }

        public VisaoGeralAbstract CreateVisaoGeral(IsolationLevel isolationLevel)
        {
            dbConnection = SqlServerFactory.CreatePatrimonio().Create();
            VisaoGeralContexto contexto = VisaoGeralContexto.GetInstancia(dbConnection, false);
            return VisaoGeralBusiness.GetInstancia(contexto, isolationLevel);
        }
        public VisaoGeralAbstract CreateVisaoGeralAdo(IsolationLevel isolationLevel)
        {
            dbConnection = SqlServerFactory.CreatePatrimonio().Create();
            return VisaoGeralBusiness.GetInstancia(dbConnection, isolationLevel);
        }
    }
}
