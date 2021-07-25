using PatrimonioBusiness.bases;
using PatrimonioBusiness.integracao.abstracts;
using PatrimonioBusiness.integracao.business;
using PatrimonioBusiness.integracao.contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.integracao
{
    public class IntegracaoFactory
    {
        private DbConnection dbConnection = null;
        private IntegracaoFactory() { }

        public static IntegracaoFactory GetInstancia()
        {
            return new IntegracaoFactory();
        }

        public MovimentoIntegracaoAbstract CreateMovimentoIntegracao(IsolationLevel isolationLevel)
        {
            dbConnection = SqlServerFactory.CreateEstoque().Create();
            IntegracaoContexto integracaoContexto = IntegracaoContexto.GetInstancia(dbConnection, true);
            return MovimentoIntegracaoBusiness.GetInstancia(integracaoContexto, isolationLevel);
        }
    }
}
