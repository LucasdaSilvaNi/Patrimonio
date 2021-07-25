using PatrimonioBusiness.visaogeral.abstracts;
using PatrimonioBusiness.visaogeral.contexto;
using PatrimonioBusiness.visaogeral.entidades;
using PatrimonioBusiness.visaogeral.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.visaogeral.business
{
    internal class VisaoGeralBusiness : VisaoGeralAbstract
    {
        private VisaoGeralContexto contexto = null;
        private IDbConnection dbConnection = null;
        private VisaoGeralBusiness(VisaoGeralContexto contexto, IsolationLevel isolationLevel):base(isolationLevel)
        {
            this.contexto = contexto;
        }
        private VisaoGeralBusiness(IDbConnection dbConnection, IsolationLevel isolationLevel) : base(isolationLevel)
        {
            this.dbConnection = dbConnection;
        }
        internal static VisaoGeralBusiness GetInstancia(VisaoGeralContexto contexto, IsolationLevel isolationLevel)
        {
            return new VisaoGeralBusiness(contexto, isolationLevel);
        }
        internal static VisaoGeralBusiness GetInstancia(IDbConnection dbConnection, IsolationLevel isolationLevel)
        {
            return new VisaoGeralBusiness(dbConnection, isolationLevel);
        }

        public async override Task<DataTable> GetDataTable(IParametro parametro)
        {
            DataTable data = new DataTable("VisaoGeral");
            SqlConnection cnn = (SqlConnection)this.dbConnection;
            cnn.Open();
            SqlCommand cmd = new SqlCommand(GetExportarExcel(parametro), cnn);
            cmd.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(data);

            return await Task.Run(() => data);

        }

        public async override Task<List<VisaoGeral>> Gets(IParametro parametro)
        {
            var transaction = this.contexto.Database.BeginTransaction(base.isolationLevel);
            try
            {
                this.contexto.Database.CommandTimeout = 0;
                
                base.TotalRegistros = await GetCount(parametro);

                var retorno = await GetsVisaoGeral(parametro);
                transaction.Commit();
                return retorno;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
        private String GetExportarExcel(IParametro parametro)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_VISAO_GERAL_EXCEL] ");
            if (parametro.Estado.HasValue)
                builder.Append("@Estado=" + parametro.Estado);
     
            if (parametro.InstitutionId.HasValue)
                builder.Append(",@InstitutionId =" + parametro.InstitutionId.Value.ToString());
            if (parametro.BudgetUnitId.HasValue)
                builder.Append(",@BudgetUnitId =" + parametro.BudgetUnitId.Value.ToString());
            if (parametro.ManagerUnitId.HasValue)
                builder.Append(",@ManagerUnitId =" + parametro.ManagerUnitId.ToString());
            if (parametro.AdministrativeUnitId.HasValue)
                builder.Append(",@AdministrativeUnitId =" + parametro.AdministrativeUnitId.ToString());

            if (!string.IsNullOrWhiteSpace(parametro.CPF))
                builder.Append(",@CPF ='" + parametro.CPF + "'");

            if (!string.IsNullOrWhiteSpace(parametro.Filtro))
                builder.Append(",@Filtro ='" + parametro.Filtro + "'");
            if (!string.IsNullOrWhiteSpace(parametro.Campo) && parametro.Campo != "0")
                builder.Append(",@Campo ='" + parametro.Campo + "'");

            return builder.ToString();
        }
        private Task<int> GetCount(IParametro parametro)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_VISAO_GERAL_COUNT] ");
            if (parametro.Estado.HasValue)
                builder.Append("@Estado=" + parametro.Estado);
            else
                builder.Append("@Estado=NULL");
            if (parametro.Size.HasValue)
            {
                builder.Append(",@Size =" + parametro.Size.ToString());
                builder.Append(",@PageNumber =" + parametro.PageNumber.ToString());
            }
            if (parametro.InstitutionId.HasValue)
                builder.Append(",@InstitutionId =" + parametro.InstitutionId.Value.ToString());
            if (parametro.BudgetUnitId.HasValue)
                builder.Append(",@BudgetUnitId =" + parametro.BudgetUnitId.Value.ToString());
            if (parametro.ManagerUnitId.HasValue)
                builder.Append(",@ManagerUnitId =" + parametro.ManagerUnitId.ToString());
            if (parametro.AdministrativeUnitId.HasValue)
                builder.Append(",@AdministrativeUnitId =" + parametro.AdministrativeUnitId.ToString());

            if (!string.IsNullOrWhiteSpace(parametro.CPF))
                builder.Append(",@CPF ='" + parametro.CPF + "'");
            if (!string.IsNullOrWhiteSpace(parametro.Filtro))
                builder.Append(",@Filtro ='" + parametro.Filtro + "'");
            if (!string.IsNullOrWhiteSpace(parametro.Campo) && parametro.Campo != "0")
                builder.Append(",@Campo ='" + parametro.Campo + "'");

           return  contexto.Database.SqlQuery<int>(builder.ToString()).FirstOrDefaultAsync();
        }

        private Task<List<VisaoGeral>> GetsVisaoGeral(IParametro parametro)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_VISAO_GERAL_ATIVO] ");
            if (parametro.Estado.HasValue)
                builder.Append("@Estado=" + parametro.Estado);
            else
                builder.Append("@Estado=NULL");

            if (parametro.Size.HasValue)
            {
                builder.Append(",@Size =" + parametro.Size.ToString());
                builder.Append(",@PageNumber =" + parametro.PageNumber.ToString());
            }
            if (parametro.InstitutionId.HasValue)
                builder.Append(",@InstitutionId =" + parametro.InstitutionId.Value.ToString());
            if (parametro.BudgetUnitId.HasValue)
                builder.Append(",@BudgetUnitId =" + parametro.BudgetUnitId.Value.ToString());
            if (parametro.ManagerUnitId.HasValue)
                builder.Append(",@ManagerUnitId =" + parametro.ManagerUnitId.ToString());
            if (parametro.AdministrativeUnitId.HasValue)
                builder.Append(",@AdministrativeUnitId =" + parametro.AdministrativeUnitId.ToString());

            if (!string.IsNullOrWhiteSpace(parametro.CPF))
                builder.Append(",@CPF ='" + parametro.CPF + "'");
            if (!string.IsNullOrWhiteSpace(parametro.Filtro))
                builder.Append(",@Filtro ='" + parametro.Filtro + "'");
            if (!string.IsNullOrWhiteSpace(parametro.Campo) && parametro.Campo != "0")
                builder.Append(",@Campo ='" + parametro.Campo + "'");

           return this.contexto.Database.SqlQuery<VisaoGeral>(builder.ToString()).ToListAsync();
        }

        public async override Task<int[]> GetsNumeroNotification(IParametro parametro)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_VISAO_GERAL_NOTIFICATION] ");

            if (parametro.InstitutionId.HasValue)
                builder.Append(" @InstitutionId =" + parametro.InstitutionId.Value.ToString());
            if (parametro.BudgetUnitId.HasValue)
                builder.Append(",@BudgetUnitId =" + parametro.BudgetUnitId.Value.ToString());
            if (parametro.ManagerUnitId.HasValue)
                builder.Append(",@ManagerUnitId =" + parametro.ManagerUnitId.ToString());

            return await this.contexto.Database.SqlQuery<int>(builder.ToString()).ToArrayAsync();
        }
    }
}
