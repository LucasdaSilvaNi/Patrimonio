using SAM.Web.Common;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Data;

namespace SAM.Web.Relatorios
{
    public class BuscaDadosSaldoContabilOrgao
    {
        public DataTable resultado;
        public BuscaDadosSaldoContabilOrgao(RelatorioViewModel param) {
            string spName = "PRC_RELATORIO__SALDO_CONTABIL_ORGAO";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "InstitutionId", valor = param.InstitutionId });

            if (param.MesRef != "0")
                listaParam.Add(new ListaParamentros { nomeParametro = "MonthReference", valor = param.MesRef });

            FunctionsCommon common = new FunctionsCommon();
            resultado = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
        }
    }
}