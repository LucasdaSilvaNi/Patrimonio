using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarResponsavel
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = null;
        public ImportarResponsavel(SAMContext contexto, Legado legado, DataRowCollection rows, string baseDeDestino)
        {
            this._legado = legado;
            this.contexto = contexto;

            foreach (DataRow row in rows)
            {
                try
                {
                    if (bool.Parse(row["Ua Validada"].ToString()))
                    {
                        if (row["Nome do Responsável"] != null && row["Nome do Responsável"].ToString().Replace(" ", "").Length > 0)
                        {
                            int linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLResponsavel(row, baseDeDestino));  //_legado.createComando(createSQLResponsavel(row, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);
                            if (linhasFetadas < 1)
                            {
                                row["Responsavel Importado"] = "Não";
                                row["Responsavel Mensagem"] = "Responsavél ja cadastrado";
                            }
                        }
                        else
                        {
                            row["Responsavel Importado"] = "Não";
                            row["Responsavel Mensagem"] = "Planilha sem responsavél";
                        }
                    }
                    else
                    {
                        row["Responsavel Importado"] = "Não";
                        row["Responsavel Mensagem"] = "UA Ínvalida";
                    }

                }
                catch (Exception ex)
                {

                    row["Responsavel Importado"] = "Não";
                    row["Responsavel Mensagem"] = ex.Message;

                    throw ex;
                }
            }

        }

        private String createSQLResponsavel(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 [resp].[Id] FROM [" + baseDeDestino + "].[dbo].[Responsible] [resp] WHERE [resp].[Name] = '" + _legado.setTamanhoDeCaracteres(row["Nome do Responsável"].ToString().Replace("'", "").Trim().Trim(), 100) + "' AND [AdministrativeUnitId] =(SELECT TOP 1 [id] FROM[" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE CONVERT(int, [Code]) = " + int.Parse(row["Código da UA"].ToString().Trim()) + "))");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append("INSERT INTO [" + baseDeDestino + "].[dbo].[Responsible](");
                builder.Append("[Name] ");
                builder.Append(",[Position] ");
                builder.Append(",[Status] ");
                builder.Append(",[AdministrativeUnitId] ");
                builder.Append(",[CPF] ");
                builder.Append(",[Email]) ");
                builder.Append(" VALUES (");
                builder.Append("'" + _legado.setTamanhoDeCaracteres(row["Nome do Responsável"].ToString().Replace("'", "").Trim().Trim(), 100) + "'");
                builder.Append("," + (row["Cargo do Responsável"] == DBNull.Value || row["Cargo do Responsável"] == null ? "Null" : "'" + _legado.setTamanhoDeCaracteres(row["Cargo do Responsável"].ToString().Replace("'", ""), 60) + "'"));
                builder.Append(",1");
                builder.Append(",(SELECT TOP 1 [id] FROM [" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE CONVERT(int, [Code]) =" + int.Parse(row["Código da UA"].ToString().Trim()) + ")");
                builder.Append("," + (row["CPF do Responsável"] == DBNull.Value || row["CPF do Responsável"]  == null ? "''" : "'" + _legado.removerCaracteresCpfCnpj(row["CPF do Responsável"].ToString(), true) + "'"));
                builder.Append(",NULL)");
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