using PatrimonioBusiness.fechamento.abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PatrimonioBusiness.fechamento.interfaces;
using PatrimonioBusiness.fechamento.contexto;
using PatrimonioBusiness.fechamento.entidades;

namespace PatrimonioBusiness.fechamento.business
{
    internal class MaterialItemDepreciacaoBusiness : MaterialItemDepreciacaoAbstract
    {
        private FechamentoContexto contexto = null;

        private MaterialItemDepreciacaoBusiness(FechamentoContexto contexto, IsolationLevel isolationLevel) : base(isolationLevel)
        {
            this.contexto = contexto;
        }

        public static MaterialItemDepreciacaoBusiness GetInstancia(FechamentoContexto contexto, IsolationLevel isolationLevel)
        {
            return new MaterialItemDepreciacaoBusiness(contexto, isolationLevel);
        }

        public override IEnumerable<IMaterialItemDepreciacao> GetNaoDepreciados(int managerUnitId, DateTime mesAnoReferencia)
        {
            var mesAnoAnterior = mesAnoReferencia.AddMonths(-1);


            StringBuilder builder = new StringBuilder();

            builder.Append(" SELECT [ast].[Id] AS 'AssetId',[ast].[MaterialItemCode],[ast].[AcquisitionDate], [ast].[MovimentDate],[ast].[NumberIdentification], MAX('') AS 'Mensagem', '900' AS MesReferencia ");
            builder.Append("  FROM [dbo].[Asset] [ast] ");
            builder.Append("  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] ");
            builder.Append("  WHERE [ast].[flagVerificado] IS NULL ");
            builder.Append("   AND [ast].[flagDepreciaAcumulada] = 1 ");
            builder.Append("   AND [ast].[AssetStartId] IS NOT NULL ");
            builder.Append("   AND ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0) ");
            builder.Append("   AND ([ast].[flagTerceiro] = 0 OR [ast].[flagTerceiro] IS NULL) ");
            builder.Append("   AND [ast].[ManagerUnitId] = " + managerUnitId.ToString());
            builder.Append("   AND ([ast].[AssetStartId]  NOT IN(SELECT [dep].[AssetStartId] FROM [dbo].[MonthlyDepreciation] [dep] ");
            builder.Append(" WHERE [dep].[AssetStartId] =  [ast].[AssetStartId] ");
            builder.Append("     OR [ast].[AssetStartId] IS NULL ) )");
            builder.Append("   AND YEAR([ast].[AcquisitionDate]) < 1900 ");
            builder.Append("   AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] = 0) ");

            builder.Append(" GROUP BY [ast].[Id],[ast].[MaterialItemCode],[ast].[AcquisitionDate], [ast].[MovimentDate],[ast].[NumberIdentification] ");

            var resultado = this.contexto.Database.SqlQuery<MaterialItemDepreciacao>(builder.ToString()).ToList();

            return resultado;
        }
    }
}
