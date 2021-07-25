using SAM.Web.Common;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Data;

namespace SAM.Web.Relatorios
{
    public class BuscaDadosInventarioFisico
    {
        public DataTable dtRetornoDados;
        public BuscaDadosInventarioFisico(RelatorioInventarioFisicoViewModel objReport) {
            string spName = "PRC_RELATORIO__INVENTARIO_FISICO";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "InstitutionId", valor = objReport.InstitutionId });
            listaParam.Add(new ListaParamentros { nomeParametro = "BudgetUnitId", valor = objReport.BudgetUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "ManagerUnitId", valor = objReport.ManagerUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "AdministrativeUnitId", valor = objReport.AdministrativeUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "SectionId", valor = objReport.SectionId });
            listaParam.Add(new ListaParamentros { nomeParametro = "GroupingType", valor = objReport.Agrupamento });

            if (objReport.MesRef != "0")
                listaParam.Add(new ListaParamentros { nomeParametro = "MonthReference", valor = objReport.MesRef });

            FunctionsCommon common = new FunctionsCommon();
            dtRetornoDados = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
        }
    }
}