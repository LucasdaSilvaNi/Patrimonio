using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarTerceiro
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = null;

        public ImportarTerceiro(SAMContext contexto, Legado legado, DataRowCollection rows, string baseDeDestino)
        {
            this._legado = legado;
            this.contexto = contexto;
            foreach (DataRow row in rows)
            {
                try
                {

                    if (row["Nome do Terceiro"] != null && row["Nome do Terceiro"].ToString().Replace(" ", "").Length > 0)
                    {
                        string logradouro = (row["Logradouro do Terceiro"] == DBNull.Value ? "" : row["Logradouro do Terceiro"].ToString());
                        int numero = 0;
                        string complemento = "";
                        string bairro = "";
                        string cidade = (row["Cidade do Terceiro"] == DBNull.Value ? "" : row["Cidade do Terceiro"].ToString());
                        string estado = (row["Estado do Terceiro"] == DBNull.Value ? "" : row["Estado do Terceiro"].ToString());
                        string cep = (row["Cep do Terceiro"] == DBNull.Value ? "" : row["Cep do Terceiro"].ToString());

                        ImportarEndereco endereco = new ImportarEndereco(this._legado, logradouro, numero, complemento, bairro, cidade, estado, cep, baseDeDestino);
                        String sqlTerceiro = createSQLTerceiro(row, baseDeDestino);
                        if (sqlTerceiro != string.Empty)
                        {
                            int linhasFetadas = contexto.Database.ExecuteSqlCommand(sqlTerceiro);// _legado.createComando(createSQLTerceiro(row, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);
                            if (linhasFetadas < 1)
                            {
                                row["Terceiro Importado"] = "Não";
                                row["Terceiro Mensagem"] = "Terceiro já cadastrado";
                            }
                        }else
                        {
                            row["Terceiro Importado"] = "Não";
                            row["Terceiro Mensagem"] = "Verificar os códigos de UGE e UA";
                        }
                    }
                    else
                    {
                        row["Terceiro Importado"] = "Não";
                        row["Terceiro Mensagem"] = "Bem Patrimonial sem terceiro";
                    }



                }
                catch (Exception ex)
                {
                    row["Terceiro Importado"] = "Não";
                    row["Terceiro Mensagem"] = ex.Message;
                }
            }
        }

        private String createSQLTerceiro(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();
            int? ManagerUnitId = null;
            bool cadastrarPorUGE = false;
            try
            {
                if (bool.Parse(row["Ua Validada"].ToString()))
                {
                    builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[OutSourced] WHERE [Name] = '" + row["Nome do Terceiro"].ToString() + "' AND ISNULL([CPFCNPJ],'') =" + (row["CPF/CNPJ do Terceiro"] == DBNull.Value ? "''" : "'" + _legado.removerCaracteresCpfCnpj(row["CPF/CNPJ do Terceiro"].ToString(), false).ToString() + "'") + " AND [InstitutionId] = " + this._legado.getInstitutionId().ToString()
                            + " AND [BudgetUnitId] =(SELECT TOP 1 [man].[BudgetUnitId] FROM[" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] INNER JOIN  [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ON [man].[Id] = [adm].[ManagerUnitId]  WHERE CONVERT(int, [adm].[Code]) =" + int.Parse(row["Código da UA"].ToString()) + "))");
                }
                else
                {
                    cadastrarPorUGE = true;
                    builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[OutSourced] WHERE [Name] = '" + row["Nome do Terceiro"].ToString() + "' AND ISNULL([CPFCNPJ],'') =" + (row["CPF/CNPJ do Terceiro"] == DBNull.Value ? "''" : "'" +  _legado.removerCaracteresCpfCnpj(row["CPF/CNPJ do Terceiro"].ToString(), false).ToString() + "'") + " AND [InstitutionId] = " + this._legado.getInstitutionId().ToString()
                        + " AND [BudgetUnitId] =(SELECT TOP 1 [man].[BudgetUnitId] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] [man]  WHERE [man].[Status] = 1 AND CONVERT(int, [man].[Code]) ='" + int.Parse(row["Código da UGE"].ToString().Replace(" ","")) + "'))");


                    ManagerUnitId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [man].[Id] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] WHERE [man].[Status] = 1 AND CONVERT(int, [man].[Code]) = '" + int.Parse(row["Código da UGE"].ToString().Replace(" ", "")) + "' ORDER BY [man].[Id] DESC").FirstOrDefault();
                  
                }

                if(cadastrarPorUGE && !ManagerUnitId.HasValue)
                {
                    return String.Empty;
                }
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append("INSERT INTO [" + baseDeDestino + "].[dbo].[OutSourced] ");
                builder.Append("([Name] ");
                builder.Append(",[CPFCNPJ] ");
                builder.Append(",[AddressId] ");
                builder.Append(",[Telephone] ");
                builder.Append(",[Status] ");
                builder.Append(",[InstitutionId] ");
                builder.Append(",[BudgetUnitId]) ");
                builder.Append(" VALUES( ");
                builder.Append("'" + _legado.setTamanhoDeCaracteres(row["Nome do Terceiro"].ToString().Replace("'", ""), 60) + "'");
                builder.Append("," + (row["CPF/CNPJ do Terceiro"] == DBNull.Value ? "NULL" : "'" + _legado.removerCaracteresCpfCnpj(row["CPF/CNPJ do Terceiro"].ToString(),false) + "'"));
                builder.Append(",(SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[Address] WHERE [PostalCode] = '" + _legado.removerCaracteresEspacoPontoBarraTracoApostrofa(row["Cep do Terceiro"].ToString()) + "')");
                builder.Append("," + (row["Telefone do Terceiro"] == DBNull.Value ? "NULL" : "'" + _legado.getNumerosNoTextoDoLegado(row["Telefone do Terceiro"].ToString()) + "'"));
                builder.Append(",1");
                builder.Append("," + this._legado.getInstitutionId().ToString());
                if (bool.Parse(row["Ua Validada"].ToString()))
                {
                    builder.Append(",(SELECT TOP 1 [man].[BudgetUnitId] FROM  [" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ");
                    builder.Append(" INNER JOIN  [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ON [man].[Id] = [adm].[ManagerUnitId] ");
                    builder.Append("  WHERE CONVERT(int, [adm].[Code]) =" + int.Parse(row["Código da UA"].ToString()) + "))");
                }else
                {
                    builder.Append(",(SELECT TOP 1 [man].[BudgetUnitId] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ");
                    builder.Append("  WHERE [man].[Status] = 1 AND CONVERT(int, [man].[Code]) ='" + int.Parse(row["Código da UGE"].ToString().Replace(" ","")) + "'))");
                }
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