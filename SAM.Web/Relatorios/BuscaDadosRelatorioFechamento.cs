using SAM.Web.Common;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Data;

namespace SAM.Web.Relatorios
{
    public class BuscaDadosRelatorioFechamento
    {
        public DataTable resultado;
        public BuscaDadosRelatorioFechamento(RelatorioViewModel param) {
            string spName = "REPORT_FECHAMENTO_MENSAL";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "UgeId", valor = param.ManagerUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "mesRef", valor = param.MesRef });

            FunctionsCommon common = new FunctionsCommon();
            resultado = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
        }
    }
}