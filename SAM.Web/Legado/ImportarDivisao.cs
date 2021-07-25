using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarDivisao
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = null;
        public ImportarDivisao(SAMContext contexto, Legado legado, DataRowCollection rows, string baseDeDestino)
        {
            this._legado = legado;
            this.contexto = contexto;
            foreach (DataRow row in rows)
            {
                try
                {
                    if (bool.Parse(row["Ua Validada"].ToString()))
                    {
                        if(row["Código da Divisão"].ToString().Replace(" ", "").Length == 0 && row["Nome da Divisão"].ToString().Replace(" ", "").Length == 0)
                        {
                            row["Divisao Importado"] = "Não";
                            row["Divisao Mensagem"] = "Planilha sem divisão";
                        }
                        else
                        {
                            int linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLDivisao(row, baseDeDestino));  // _legado.createComando(createSQLDivisao(row, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);
                            if (linhasFetadas < 1)
                            {
                                row["Divisao Importado"] = "Não";
                                row["Divisao Mensagem"] = "Divisão já cadasatrada";
                            }
                        }
                    }
                    else
                    {
                        row["Divisao Importado"] = "Não";
                        row["Divisao Mensagem"] = "UA Ínvalida";
                    }

                }
                catch (Exception ex)
                {
                    row["Divisao Importado"] = "Não";
                    row["Divisao Mensagem"] = ex.Message;
                }
            }

        }

        private String createSQLDivisao(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();

            int codigoDivisao = -1;

            if (row["Código da Divisão"].ToString().Replace(" ", "").Length > 0)
            {
                codigoDivisao = int.Parse(row["Código da Divisão"].ToString());
            }

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [dbo].[Section] WHERE CONVERT(int, Code) = " + codigoDivisao + " AND [AdministrativeUnitId] =(SELECT TOP 1 [id] FROM [" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE CONVERT(int, [Code]) =" + int.Parse(row["Código da UA"].ToString()) + "))");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[Section] ");
                builder.Append("([AdministrativeUnitId] ");
                builder.Append(",[Code] ");
                builder.Append(",[Description] ");
                builder.Append(",[ResponsibleId] ");
                builder.Append(",[Status])");

                builder.Append(" VALUES (");
                builder.Append("(SELECT TOP 1 [id] FROM [" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE CONVERT(int, Code) =" + _legado.getNumerosNoTextoDoLegado(row["Código da UA"].ToString()) + ")");

                //Caso não exista código da divisão na planilha gerar um código, pegando o maior código da divisão por UA + 1
                if (row["Código da Divisão"] != null && row["Código da Divisão"].ToString().Replace(" ", "").Length > 0)
                    builder.Append("," + _legado.getNumerosNoTextoDoLegado(row["Código da Divisão"].ToString()));
                else
                    builder.Append(", (SELECT TOP 1 (Code + 1) FROM [dbo].[Section] WHERE [AdministrativeUnitId] =(SELECT TOP 1 [id] FROM [" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE CONVERT(int, [Code]) =" + int.Parse(row["Código da UA"].ToString()) + ") ORDER BY Code DESC) ");

                builder.Append("," + (row["Nome da Divisão"] == DBNull.Value ? "NULL" : "'" + _legado.setTamanhoDeCaracteres(row["Nome da Divisão"].ToString().Replace("'", ""), 120) + "'"));
                builder.Append(",(SELECT TOP 1 [id] FROM [" + baseDeDestino + "].[dbo].[Responsible] WHERE [CPF] ='" + _legado.removerCaracteresCpfCnpj(row["CPF do Responsável"].ToString(), true) + "' AND [Name] = '" + _legado.setTamanhoDeCaracteres(row["Nome do Responsável"].ToString().Replace("'", ""), 100) + "')");
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