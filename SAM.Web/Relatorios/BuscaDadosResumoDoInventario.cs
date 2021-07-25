using SAM.Web.Common;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SAM.Web.Relatorios
{
    public class BuscaDadosResumoDoInventario
    {
        private HierarquiaContext db;
        public DataTable[] dtRetornoDados;

        public BuscaDadosResumoDoInventario(ReportResumoInventarioViewModel objReport) {
            Dictionary<string, string> storedProceduresNames = null;
            Dictionary<string, bool> storedProceduresExecutaveis = null;
            string[] storedProceduresNamesWithMonthReference = null;
            dtRetornoDados = null;
            List<Tuple<string, string, bool, List<ListaParamentros>>> listaParametrosSP = null;




            List<ListaParamentros> listaParametros = null;
            listaParametros = new List<ListaParamentros>() { new ListaParamentros() { nomeParametro = "ManagerUnitId", valor = objReport.ManagerUnitId } };


            string ManagerUnitMonthReference = null;
            ManagerUnit managerUnit;

            using (db = new HierarquiaContext())
            {
                managerUnit = (from a in db.ManagerUnits where a.Id == objReport.ManagerUnitId select a).FirstOrDefault();
            }

            if (objReport.MesRef == "0")
                ManagerUnitMonthReference = managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2).PadLeft(2, '0') + "/" + managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
            else
                ManagerUnitMonthReference = objReport.MesRef.Substring(4, 2).PadLeft(2, '0') + "/" + objReport.MesRef.Substring(0, 4).PadLeft(4, '0');

            if (managerUnit.FlagIntegracaoSiafem)
            {
                storedProceduresNames = new Dictionary<string, string>()
                                                                    {
                                                                        {"dsRelatorio_ResumoContasContabeis",  "PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO" },
                                                                        {"dsBPs_IncorporadosMesRef", "PRC_RELATORIO__INCORPORACOES_BP__MESREF" },
                                                                        {"dsBPS_TotalmenteDepreciados", "PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO" },
                                                                        {"dsBPs_Acervo", "PRC_RELATORIO_RESUMO_INVENTARIO_ACERVOS" },
                                                                        {"dsBPs_Terceiros", "PRC_RELATORIO_RESUMO_INVENTARIO_TERCEIROS" },
                                                                        {"dsBPs_BaixadosMesRef",  "PRC_RELATORIO__BAIXAS_BP__MESREF" },
                                                                    };
            }
            else
            {
                storedProceduresNames = new Dictionary<string, string>()
                                                                    {
                                                                        {"dsRelatorio_ResumoContasContabeis",  "PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS" },
                                                                        {"dsBPs_IncorporadosMesRef", "PRC_RELATORIO__INCORPORACOES_BP__MESREF" },
                                                                        {"dsBPS_TotalmenteDepreciados", "PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO" },
                                                                        {"dsBPs_Acervo", "PRC_RELATORIO__BP_ACERVO" },
                                                                        {"dsBPs_Terceiros", "PRC_RELATORIO__BP_TERCEIRO" },
                                                                        {"dsBPs_BaixadosMesRef",  "PRC_RELATORIO__BAIXAS_BP__MESREF" },
                                                                    };
            }

            storedProceduresExecutaveis = new Dictionary<string, bool>() {
                                                                           {"dsRelatorio_ResumoContasContabeis",  objReport.ResumoConsolidado },
                                                                           {"dsBPs_IncorporadosMesRef",  objReport.AquisicoesCorrentes },
                                                                           {"dsBPS_TotalmenteDepreciados",  objReport.BPTotalDepreciados },
                                                                           {"dsBPs_Acervo",  objReport.Acervos },
                                                                           {"dsBPs_Terceiros",  objReport.Terceiros },
                                                                           {"dsBPs_BaixadosMesRef",  objReport.BaixasCorrentes }
                                                                        };

            if (managerUnit.FlagIntegracaoSiafem)
            {
                storedProceduresNamesWithMonthReference = new string[] {
                                                    "PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO",
                                                    "PRC_RELATORIO__INCORPORACOES_BP__MESREF",
                                                    "PRC_RELATORIO__BAIXAS_BP__MESREF",
                                                  };
            }
            else
            {
                storedProceduresNamesWithMonthReference = new string[] {
                                                    "PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS",
                                                    "PRC_RELATORIO__INCORPORACOES_BP__MESREF",
                                                    "PRC_RELATORIO__BAIXAS_BP__MESREF",
                                                  };
            }

            listaParametrosSP = new List<Tuple<string, string, bool, List<ViewModels.ListaParamentros>>>();
            foreach (var storedProcedureName in storedProceduresNames)
            {
                listaParametros = new List<ListaParamentros>();

                bool seraExecutado = storedProceduresExecutaveis[storedProcedureName.Key];


                //MES-REFERENCIA DE GERACAO DO RELATORIO
                listaParametros.Add(new ListaParamentros() { nomeParametro = "MonthReference", valor = objReport.MesRef });

                //ID DA UGE
                listaParametros.Add(new ListaParamentros() { nomeParametro = "ManagerUnitId", valor = objReport.ManagerUnitId });
                listaParametrosSP.Add(new Tuple<string, string, bool, List<ListaParamentros>>(storedProcedureName.Key, storedProcedureName.Value, seraExecutado, listaParametros));

                listaParametros = null;
            }


            FunctionsCommon common = new FunctionsCommon();
            dtRetornoDados = common.ReturnDataTablesFromMultiplesStoredsProcedureReport(listaParametrosSP);
        }
    }
}