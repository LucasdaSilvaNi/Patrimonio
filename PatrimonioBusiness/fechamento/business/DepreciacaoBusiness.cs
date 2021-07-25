using PatrimonioBusiness.fechamento.abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PatrimonioBusiness.fechamento.interfaces;
using PatrimonioBusiness.fechamento.contexto;
using System.Data.SqlClient;
using PatrimonioBusiness.fechamento.entidades;

namespace PatrimonioBusiness.fechamento.business
{
    internal class DepreciacaoBusiness : DepreciacaoAbstract
    {
        private FechamentoContexto contexto = null;

        private DepreciacaoBusiness(FechamentoContexto contexto, IsolationLevel isolationLevel) : base(isolationLevel)
        {
            this.contexto = contexto;
        }

        public static DepreciacaoBusiness GetInstancia(FechamentoContexto contexto, IsolationLevel isolationLevel)
        {
            return new DepreciacaoBusiness(contexto, isolationLevel);
        }

        public override int CreateRelatorioContabil(int managerUnitId, string mesReferencia)
        {
            var cnn = new SqlConnection(this.contexto.Database.Connection.ConnectionString);
            cnn.Open();
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cnn;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add("@MonthReference", SqlDbType.NVarChar, 6);
            cmd.Parameters.Add("@ManagerUnitId", SqlDbType.Int);
            cmd.Parameters["@MonthReference"].Value = mesReferencia;
            cmd.Parameters["@ManagerUnitId"].Value = managerUnitId;

            cmd.CommandText = "[dbo].[INSERT_RELATORIO_CONTABIL]";
            var retorno = cmd.ExecuteNonQuery();
            cnn.Close();
            cnn = null;
            return retorno;
        }

        public override async Task<bool> Depreciar(long assetStartId, int managerUnitId, int materialItemCode, DateTime dataFinal)
        {
            try
            {
                SqlParameter[] parametros =
                    {
                         new SqlParameter("@ManagerUnitId", managerUnitId),
                                         new SqlParameter("@MaterialItemCode", materialItemCode),
                                         new SqlParameter("@AssetStartId", assetStartId),
                                         new SqlParameter("@DataFinal", dataFinal),
                                         new SqlParameter("@Fechamento", false),
                                         new SqlParameter("@Retorno",SqlDbType.VarChar,1000) { Direction= ParameterDirection.Output },
                                         new SqlParameter("@Erro",SqlDbType.Bit) { Direction= ParameterDirection.Output }

                    };


                var resultadoTransferencia = await this.contexto.Database.ExecuteSqlCommandAsync("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT,@Erro OUT", parametros);

                var msg = parametros[5].Value.ToString();


                return true;
            }catch(Exception ex)
            {
                throw ex;
            }
             

        }

        public override Task<List<IDepreciacao>> Get(long assetStartId, int managerUnitId, int materialItemCode, DateTime currentDate)
        {
            throw new NotImplementedException();
        }

        public override Task<List<IDepreciacao>> Gets(long assetStartId, int managerUnitId, int materialItemCode)
        {
            throw new NotImplementedException();
        }

        public async override Task<ISimulacaoResultado> SimularDepreciacao(long assetStartId, int managerUnitId, int materialItemCode, DateTime dataFinal)
        {



            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = (SqlConnection)this.contexto.Database.Connection;

            cmd.Parameters.Add("@ManagerUnitId", SqlDbType.Int);
            cmd.Parameters.Add("@MaterialItemCode", SqlDbType.Int);
            cmd.Parameters.Add("@assetStartId", SqlDbType.Int);
            cmd.Parameters.Add("@DataFinal", SqlDbType.DateTime);
            cmd.Parameters.Add("@Retorno", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
            cmd.Parameters["@ManagerUnitId"].Value = managerUnitId;
            cmd.Parameters["@MaterialItemCode"].Value = materialItemCode;
            cmd.Parameters["@AssetStartId"].Value = assetStartId;
            cmd.Parameters["@DataFinal"].Value = dataFinal;
         

            cmd.CommandText = "SAM_DEPRECIACAO_UGE_SIMULAR";
            var adp = new SqlDataAdapter(cmd);
            var dataSet = new DataSet("Depreciacao");

            adp.Fill(dataSet);

            cmd.Cancel();
            cmd.Dispose();
            cmd = null;

            adp.Dispose();
            adp = null;


            IList<IDepreciacaoErro> depreciacaoErro = dataSet.Tables[0].AsEnumerable().ToList().ConvertAll(new Converter<DataRow, IDepreciacaoErro>(DepreciacaoErro.ConverterDataRowParaLista));
            IList<IDepreciacao> depreciacao = dataSet.Tables[1].AsEnumerable().ToList().ConvertAll(new Converter<DataRow, IDepreciacao>(Depreciacao.ConverterDataRowParaIDepreciacao));

            ISimulacaoResultado simulacao = new SimulacaoResultado(depreciacao, depreciacaoErro);

            return await Task.Run(() => simulacao);

        }
    }
}