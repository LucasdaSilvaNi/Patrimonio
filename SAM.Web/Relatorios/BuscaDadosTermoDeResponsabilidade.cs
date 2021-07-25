using SAM.Web.Common;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Data;

namespace SAM.Web.Relatorios
{
    public class BuscaDadosTermoDeResponsabilidade
    {
        public DataTable dtRetornoDados;
        public BuscaDadosTermoDeResponsabilidade(RelatorioTermoDeResponsabilidadeViewModel param)
        {
            string spName = "REPORT_RESPONSIBLE";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "idUa", valor = param.AdministrativeUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "idResponsavel", valor = param.idResponsable });
            listaParam.Add(new ListaParamentros { nomeParametro = "idDivisao", valor = (param.SectionId != 0 ? (int?)param.SectionId : null) });

            FunctionsCommon common = new FunctionsCommon();
            dtRetornoDados = common.ReturnDataFromStoredProcedureReport(listaParam, spName);
        }
    }
}