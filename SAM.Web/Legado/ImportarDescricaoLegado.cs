using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarDescricaoLegado
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = null;

        public ImportarDescricaoLegado(SAMContext contexto, Legado legado, DataRowCollection rows, string baseDeDestino)
        {
            this._legado = legado;
            this.contexto = contexto;

            foreach (DataRow row in rows)
            {
                try
                {

                    if (row["Descrição do Item de Material"] != null && row["Descrição do Item de Material"].ToString().Replace(" ", "").Length > 0)
                    {
                        int linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLDescricaoLegado(row, baseDeDestino)); // _legado.createComando(createSQLDescricaoLegado(row, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);

                        if (linhasFetadas < 1)
                        {
                            row["Descricao Importado"] = "Não";
                            row["Descricao Mensagem"] = "Descrição resumida ja cadastrada";
                        }
                    }
                    else
                    {
                        row["Descricao Importado"] = "Não";
                        row["Descricao Mensagem"] = "Bem patrimonial sem descrição resumida";
                    }


                }
                catch (Exception ex)
                {
                    row["Descricao Importado"] = "Não";
                    row["Descricao Mensagem"] = ex.Message;
                }
            }
        }

        private String createSQLDescricaoLegado(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[ShortDescriptionItem] WHERE [Description] = '" + _legado.setTamanhoDeCaracteres(row["Descrição do Item de Material"].ToString().Replace("'", ""), 255) + "')");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append("INSERT INTO [" + baseDeDestino + "].[dbo].[ShortDescriptionItem] ");
                builder.Append("([Description]) ");
                builder.Append(" VALUES('" + _legado.setTamanhoDeCaracteres(row["Descrição do Item de Material"].ToString().Replace("'", ""), 255) + "')");
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