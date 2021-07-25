using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SAM.Web.Common
{
    public class CalculaDepreciacao
    {

        private FunctionsCommon common = new FunctionsCommon();

        public DataTable CalculaDepreciacaoAcumulada(int? assetId, string Login, bool ehAcervo, bool ehTerceiro, string valueUpdateModel, int? movementTypeId = null)
        {
            string spName = "DEPRECIACAO_ACUMULADA_UNITARIO";
            decimal valueUpdate = string.IsNullOrEmpty(valueUpdateModel) ? 0 : decimal.Parse(valueUpdateModel);

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "@assetId", valor = assetId });
            listaParam.Add(new ListaParamentros { nomeParametro = "@Login", valor = Login });
            listaParam.Add(new ListaParamentros { nomeParametro = "@movementTypeId", valor = movementTypeId });

            if (movementTypeId != null && movementTypeId != (int)EnumMovimentType.IncorporacaoDeInventarioInicial)
                listaParam.Add(new ListaParamentros { nomeParametro = "@flagDepreciaPelaDataIncorporacao", valor = 0 });


            if (ehAcervo || ehTerceiro)
                listaParam.Add(new ListaParamentros { nomeParametro = "@valueUpdate", valor = valueUpdate });

            return common.ReturnDataFromStoredProcedureReport(listaParam, spName);
        }
    }
}