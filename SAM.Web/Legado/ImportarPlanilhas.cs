using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarPlanilhas
    {
        StringBuilder builder = null;

        public string createSQLImportacaoPlanilha(ImportacaoPlanilha importarPlanilhas, DataRowCollection rows, string baseDeDestino)
        {
            builder = new StringBuilder();
            int bemPatrimonialId = 0;

            if (bemPatrimonialId < 1)
            {
                builder = new StringBuilder();

                builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[ImportacaoPlanilha] ");
                builder.Append(" ([NomeArquivo] ");
                builder.Append(" ,[Processado] ");
                builder.Append(" ,[Login_Importacao] ");
                builder.Append(" ,[Data_Importacao] ");
                builder.Append(" ,[Login_Processamento] ");
                builder.Append(" ,[Data_Processamento]) ");
                builder.Append(" VALUES");
                //builder.Append(" (" + rows["ImportacaoPlanilhaId"].ToString();

                builder.Append(" SELECT CAST(@@IDENTITY AS INT) ");

                return builder.ToString();
            }
            else
                return "SELECT - 1";
        }

        private String createSQL(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[DadosImportacaoUsuario] ");
                builder.Append(" ([ImportacaoPlanilhaId] ");
                builder.Append(" ,[Status] ");
                builder.Append(" ,[MovimentDate] ");
                builder.Append(" ,[MovementTypeId] ");
                builder.Append(" ,[StateConservationId] ");
                builder.Append(" ,[InstitutionId] ");
                builder.Append(" ,[BudgetUnitId] ");
                builder.Append(" ,[ManagerUnitId] ");
                builder.Append(" ,[AdministrativeUnitId] ");
                builder.Append(" ,[SectionId] ");
                builder.Append(" ,[AuxiliaryAccountId] ");
                builder.Append(" ,[ResponsibleId] ");
                builder.Append(" ,[SourceDestiny_ManagerUnitId] ");
                builder.Append(" ,[AssetTransferenciaId] ");
                builder.Append(" ,[ExchangeId] ");
                builder.Append(" ,[ExchangeDate] ");
                builder.Append(" ,[NumberPurchaseProcess]");
                builder.Append(" ,[Observation]");
                builder.Append(" ,[ExchangeUserId] ");
                builder.Append(" ,[Login] ");
                builder.Append(" ,[DataLogin]) ");
                builder.Append(" VALUES ");
                //builder.Append(" (" + assetMoviments.AssetId);
                //builder.Append(" ,1");
                //builder.Append(" ,'" + assetMoviments.MovimentDate.ToShortDateString() + "'");
                //builder.Append(" ," + assetMoviments.MovementTypeId);
                //builder.Append(" ," + assetMoviments.StateConservationId);
                //builder.Append(" ," + assetMoviments.InstitutionId);
                //builder.Append(" ," + assetMoviments.BudgetUnitId);
                //builder.Append(" ," + assetMoviments.ManagerUnitId);
                //builder.Append(" ," + (assetMoviments.AdministrativeUnitId.HasValue ? assetMoviments.AdministrativeUnitId.ToString() : "Null"));
                //builder.Append(" ," + (assetMoviments.SectionId.HasValue ? assetMoviments.SectionId.ToString() : "Null"));
                //builder.Append(" ," + (assetMoviments.AuxiliaryAccountId.HasValue && assetMoviments.AuxiliaryAccountId.Value > 0 ? assetMoviments.AuxiliaryAccountId.ToString() : "Null"));
                //builder.Append(" ," + (assetMoviments.ResponsibleId.HasValue ? assetMoviments.ResponsibleId.ToString() : "Null"));
                //builder.Append(" ," + (assetMoviments.SourceDestiny_ManagerUnitId.HasValue ? assetMoviments.SourceDestiny_ManagerUnitId.ToString() : "Null"));
                //builder.Append(" ," + (assetMoviments.AssetTransferenciaId.HasValue ? assetMoviments.AssetTransferenciaId.ToString() : "Null"));
                //builder.Append(" ," + (assetMoviments.ExchangeId.HasValue ? assetMoviments.ExchangeId.ToString() : "Null"));
                //builder.Append(" ," + (assetMoviments.ExchangeDate.HasValue ? assetMoviments.ExchangeDate.ToString() : "Null"));
                //builder.Append(" ,'" + (assetMoviments.NumberPurchaseProcess != null && assetMoviments.NumberPurchaseProcess.Length > 25 ? assetMoviments.NumberPurchaseProcess.Substring(0, 25) : assetMoviments.NumberPurchaseProcess) + "'");
                //builder.Append(" ," + (string.IsNullOrEmpty(assetMoviments.Observation) ? "Null" : "'" + assetMoviments.Observation + "'"));
                //builder.Append(" ," + (assetMoviments.ExchangeUserId.HasValue ? assetMoviments.ExchangeUserId.ToString() : "Null"));
                //builder.Append(" ,'" + assetMoviments.Login + "'");
                //builder.Append(" ,'" + assetMoviments.DataLogin + "'" + ")");

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string createSQLSiglaGenerica(AssetMovements assetMovement, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE InstitutionId = " + assetMovement.InstitutionId + " AND Description = 'SIGLA_GENERICA')");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[Initial] ");
                builder.Append("([Name] ");
                builder.Append(",[Description] ");
                builder.Append(",[BarCode] ");
                builder.Append(",[InstitutionId] ");
                builder.Append(",[BudgetUnitId] ");
                builder.Append(",[ManagerUnitId] ");
                builder.Append(",[Status])");


                builder.Append(" VALUES (");
                builder.Append("'SIGLA_GENE'");
                builder.Append("," + "'SIGLA_GENERICA'");
                builder.Append("," + "NULL");
                builder.Append("," + assetMovement.InstitutionId);
                builder.Append("," + "NULL");
                builder.Append("," + "NULL");
                builder.Append("," + "1)");
                builder.Append(Environment.NewLine);
                builder.Append("END");
                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string createSQLUOGenerica(AssetMovements assetMovement, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[BudgetUnit] WHERE InstitutionId = " + assetMovement.InstitutionId + " AND Code = '99999')");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[BudgetUnit] ");
                builder.Append("([InstitutionId] ");
                builder.Append(",[Code] ");
                builder.Append(",[Description] ");
                builder.Append(",[Direct] ");
                builder.Append(",[Status])");

                builder.Append(" VALUES (");
                builder.Append(assetMovement.InstitutionId);
                builder.Append("," + "'99999' ");
                builder.Append("," + "'UO_GENERICA' ");
                builder.Append("," + "1 ");
                builder.Append("," + "1)");
                builder.Append(Environment.NewLine);
                builder.Append("END");
                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string createSQLUGEGenerica(AssetMovements assetMovement, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] WHERE BudgetUnitId = " + assetMovement.BudgetUnitId + " AND Code = '999999')");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[ManagerUnit] ");
                builder.Append("([BudgetUnitId] ");
                builder.Append(",[Code] ");
                builder.Append(",[Description] ");
                builder.Append(",[Status] ");
                builder.Append(",[ManagmentUnit_YearMonthStart] ");
                builder.Append(",[ManagmentUnit_YearMonthReference]");
                builder.Append(",[RowId])");

                builder.Append(" VALUES (");
                builder.Append(assetMovement.BudgetUnitId);
                builder.Append("," + "'999999' ");
                builder.Append("," + "'UGE_GENERICA'");
                builder.Append("," + "0 ");
                builder.Append("," + "'" + Convert.ToString(DateTime.Now.Year).PadLeft(4, '0') + Convert.ToString(DateTime.Now.Month).PadLeft(2, '0') + "'");
                builder.Append("," + "'" + Convert.ToString(DateTime.Now.Year).PadLeft(4, '0') + Convert.ToString(DateTime.Now.Month).PadLeft(2, '0') + "'");
                builder.Append("," + "NULL)");
                builder.Append(Environment.NewLine);
                builder.Append("END");
                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string createSQLUAGenerica(AssetMovements assetMovement, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE ManagerUnitId = " + assetMovement.ManagerUnitId + " AND Code = '99999999')");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[AdministrativeUnit] ");
                builder.Append("([ManagerUnitId] ");
                builder.Append(",[Code] ");
                builder.Append(",[Description] ");
                builder.Append(",[RelationshipAdministrativeUnitId] ");
                builder.Append(",[Status] ");
                builder.Append(",[RowId])");

                builder.Append(" VALUES (");
                builder.Append(assetMovement.ManagerUnitId);
                builder.Append("," + "'99999999' ");
                builder.Append("," + "'UA_GENERICA'");
                builder.Append("," + "NULL ");
                builder.Append("," + "0 ");
                builder.Append("," + "NULL)");
                builder.Append(Environment.NewLine);
                builder.Append("END");
                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}