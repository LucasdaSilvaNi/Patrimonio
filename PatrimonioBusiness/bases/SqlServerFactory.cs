using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.bases
{
    public class SqlServerFactory : ISqlBase
    {
        private SqlConnection cnnSqlServer = null;

        private SqlServerFactory(bool patrimonio)
        {
            if (patrimonio)
                this.cnnSqlServer = new SqlConnection(GetConnectionStringByName("SAMContext"));
            else
                this.cnnSqlServer = new SqlConnection(GetConnectionStringByName("SAMContextIntegracao"));
        }
        public DbConnection Create()
        {
            return this.cnnSqlServer;
        }
        public static SqlServerFactory CreateEstoque()
        {
            return new SqlServerFactory(false);
        }
        public static SqlServerFactory CreatePatrimonio()
        {
            return new SqlServerFactory(true);
        }
        private string GetConnectionStringByName(string name)
        {
            string returnValue = null;

            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[name];
            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }
    }
}
