using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Sam.Common.Util;
using SAM.Web.Controllers;
using SAM.Web.ViewModels;



namespace SAM.Web.Common
{
    public class FunctionsCommon : BaseController
    {
        /// <summary>
        /// Retorna dados das Stored Procedures de relatórios
        /// </summary>
        /// <param name="listParam"></param>
        /// <param name="spname"></param>
        /// <returns></returns>
        public DataTable ReturnDataFromStoredProcedureReport(List<ListaParamentros> listParam, string spName)
        {
            DataTable retorno = new DataTable();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString))
            {
                using (var cmd = new SqlCommand(spName, con))
                {

                    foreach (var item in listParam)
                    {
                        cmd.CommandTimeout = TimeSpan.FromHours(10).Minutes;
                        cmd.Parameters.AddWithValue(item.nomeParametro, item.valor);
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        da.Fill(retorno);
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Retorna dados das Stored Procedures de relatórios
        /// </summary>
        /// <param name="listParam"></param>
        /// <param name="spname"></param>
        /// <returns></returns>
        public DataTable[] ReturnDataTablesFromMultiplesStoredsProcedureReport(IList<Tuple<string, string, bool, List<ListaParamentros>>> listaNomeSPsEParametrosExecucao)
        {
            List<DataTable> dadosRetornados = null;


            if (listaNomeSPsEParametrosExecucao.HasElements())
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString))
                {

                    DataTable dtRetornoProcessamento = new DataTable();
                    dadosRetornados = new List<DataTable>();

                    foreach (var tupleNomeSP_e_Parametros in listaNomeSPsEParametrosExecucao)
                    {
                        if (tupleNomeSP_e_Parametros.Item3)
                        {
                            using (var cmd = new SqlCommand(tupleNomeSP_e_Parametros.Item2, con))
                            {
                                dtRetornoProcessamento = new DataTable(tupleNomeSP_e_Parametros.Item1);
                                foreach (var parametroExecucao in tupleNomeSP_e_Parametros.Item4)
                                {
                                    cmd.CommandTimeout = TimeSpan.FromHours(10).Minutes;
                                    cmd.Parameters.AddWithValue(parametroExecucao.nomeParametro, parametroExecucao.valor);
                                }

                                using (var da = new SqlDataAdapter(cmd))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    da.Fill(dtRetornoProcessamento);


                                    dadosRetornados.Add(dtRetornoProcessamento);
                                    dtRetornoProcessamento = null;
                                }
                            }
                        }
                        else {
                            dtRetornoProcessamento = new DataTable(tupleNomeSP_e_Parametros.Item1);
                            dadosRetornados.Add(dtRetornoProcessamento);
                            dtRetornoProcessamento = null;
                        }
                    }
                }

            return dadosRetornados.ToArray();
        }

        public int ExecuteNonQueryStoredProcedure(List<ListaParamentros> listParam, string spName)
        {
            int numeroRegistrosManipulados = 0;

            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString))
                {
                    using (var cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandTimeout = TimeSpan.FromHours(10).Minutes;
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (var item in listParam)
                            cmd.Parameters.AddWithValue(item.nomeParametro, item.valor);

                        con.Open();
                        numeroRegistrosManipulados = cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception erroEmTempoDeExecucao)
            {
                base.GravaLogErro(erroEmTempoDeExecucao);
                throw erroEmTempoDeExecucao;
            }

            return numeroRegistrosManipulados;
        }
    }
}
