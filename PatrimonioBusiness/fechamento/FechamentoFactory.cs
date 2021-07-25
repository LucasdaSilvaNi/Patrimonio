using PatrimonioBusiness.bases;
using PatrimonioBusiness.fechamento.abstracts;
using PatrimonioBusiness.fechamento.business;
using PatrimonioBusiness.fechamento.contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento
{
    public class FechamentoFactory
    {
        private FechamentoFactory() { }
        private DbConnection dbConnection = null;

        public static FechamentoFactory GetInstancia()
        {
            return new FechamentoFactory();
        }

        public MaterialItemDepreciacaoAbstract CreateMaterialItemDepreciacao(IsolationLevel isolationLevel)
        {
            dbConnection = SqlServerFactory.CreatePatrimonio().Create();
            FechamentoContexto contexto = FechamentoContexto.GetInstancia(dbConnection, false);
            return MaterialItemDepreciacaoBusiness.GetInstancia(contexto, isolationLevel);
        }
        public DepreciacaoAbstract CreateDepreciacao(IsolationLevel isolationLevel)
        {
            dbConnection = SqlServerFactory.CreatePatrimonio().Create();
            FechamentoContexto contexto = FechamentoContexto.GetInstancia(dbConnection, false);
            return DepreciacaoBusiness.GetInstancia(contexto, isolationLevel);
        }
    }
}
