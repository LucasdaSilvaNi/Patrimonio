using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarSigla
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = null;
        private int cont = 0;

        public ImportarSigla(SAMContext contexto, Legado legado, DataRowCollection rows, string baseDeDestino)
        {
            this._legado = legado;
            this.contexto = contexto;

            cont = 0;

            foreach (DataRow row in rows)
            {
                try
                {

                    cont++;
                    if (row["Sigla"] != null && row["Sigla"].ToString().Trim().Length > 0)
                    {
                        int linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLSigla(row, baseDeDestino)); // _legado.createComando(createSQLSigla(row, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);
                        if (linhasFetadas < 1)
                        {
                            row["Sigla Importado"] = "Não";
                            row["Sigla Mensagem"] = "Sigla já cadastrada";
                        }
                    }
                    else
                    {
                        row["Sigla Importado"] = "Não";
                        row["Sigla Mensagem"] = "Bem Patrimonial sem sigla";
                    }


                }
                catch (Exception ex)
                {
                    row["Sigla Importado"] = "Não";
                    row["Sigla Mensagem"] = ex.Message;
                }
            }
        }

        private String createSQLSigla(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                if (bool.Parse(row["Ua Validada"].ToString()))
                {
                    builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE [Name] = '" + row["Sigla"].ToString() + "' AND [InstitutionId] = " + this._legado.getInstitutionId().ToString()
                                + " AND [BudgetUnitId] =(SELECT TOP 1[man].[BudgetUnitId] FROM[" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] INNER JOIN  [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ON [man].[Id] = [adm].[ManagerUnitId]  WHERE CONVERT(int, [adm].[Code]) =" + int.Parse(row["Código da UA"].ToString()).ToString() + "))");
                }else
                {
                    builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE [Name] = '" + row["Sigla"].ToString() + "' AND [InstitutionId] = " + this._legado.getInstitutionId().ToString()
                             + " AND [BudgetUnitId] =(SELECT TOP 1 [man].[BudgetUnitId] FROM  [" + baseDeDestino + "].[dbo].[ManagerUnit] [man]  WHERE [man].[Status] = 1 and CONVERT(int, [man].[Code]) ='" + int.Parse(row["Código da UGE"].ToString().Trim()).ToString() + "'))");

                }
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append("INSERT INTO [" + baseDeDestino + "].[dbo].[Initial] ");
                builder.Append("([Name] ");
                builder.Append(",[Description] ");
                builder.Append(",[InstitutionId] ");
                builder.Append(",[BudgetUnitId] ");
                builder.Append(",[ManagerUnitId] ");
                builder.Append(",[Status])");
                builder.Append(" VALUES(");
                builder.Append("'" + row["Sigla"].ToString() + "'");
                builder.Append("," + (row["Descrição Sigla"] == DBNull.Value ? "NULL" : "'" + _legado.setTamanhoDeCaracteres(row["Descrição Sigla"].ToString().Trim(), 60) + "'"));
                builder.Append("," + this._legado.getInstitutionId().ToString());
                if (bool.Parse(row["Ua Validada"].ToString()))
                {
                    builder.Append(",(SELECT TOP 1 [man].[BudgetUnitId] FROM  [" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ");
                    builder.Append(" INNER JOIN  [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ON [man].[Id] = [adm].[ManagerUnitId] ");
                    builder.Append("  WHERE CONVERT(int, [adm].[Code]) =" + int.Parse(row["Código da UA"].ToString()).ToString() + ")");
                }else
                {
                    builder.Append(",(SELECT TOP 1 [man].[BudgetUnitId] FROM  [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ");
                    builder.Append("  WHERE CONVERT(int, [man].[Code]) ='" + int.Parse(row["Código da UGE"].ToString().Trim()).ToString() + "')");
                }
                if (bool.Parse(row["Ua Validada"].ToString()))
                {
                    builder.Append(",(SELECT TOP 1 [adm].[ManagerUnitId] FROM  [" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ");
                    builder.Append("  WHERE CONVERT(int, [adm].[Code]) =" + int.Parse(row["Código da UA"].ToString()).ToString() + ")");
                }else
                {
                    builder.Append(",(SELECT TOP 1 [man].[Id] FROM  [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ");
                    builder.Append("  WHERE CONVERT(int, [man].[Code]) ='" + int.Parse(row["Código da UGE"].ToString().Trim()).ToString() + "')");
                }
                builder.Append(",1)");
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