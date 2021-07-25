using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarContaAuxiliar
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = null;

        public ImportarContaAuxiliar(SAMContext contexto,Legado legado,DataRowCollection rows, string baseDeDestino)
        {
            this._legado = legado;
            this.contexto = contexto;

            foreach (DataRow row in rows)
            {
                try
                {
                    
                        if (row["Conta Auxiliar"] != null || row["Conta Auxiliar"].ToString().Replace(" ", "").Length > 0)
                        {
                            int linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLContaAuxiliar(row, baseDeDestino));// _legado.createComando(createSQLContaAuxiliar(row, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);

                            if (linhasFetadas < 1)
                            {
                                row["ContaAuxliar Importado"] = "Não";
                                row["ContaAuxiliar Mensagem"] = "Conta auxiliar já cadastrada";
                            }
                        }
                        else
                        {
                            row["ContaAuxliar Importado"] = "Não";
                            row["ContaAuxiliar Mensagem"] = "Bem Patrimonial sem conta auxiliar";
                        }
                    

                }
                catch (Exception ex)
                {
                    row["ContaAuxliar Importado"] = "Não";
                    row["ContaAuxiliar Mensagem"] = ex.Message;
                }
            }
        }

        private String createSQLContaAuxiliar(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [dbo].[AuxiliaryAccount] WHERE [Code] = " + _legado.getNumerosNoTextoDoLegado(row["Conta Auxiliar"].ToString()) + ")");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append("INSERT INTO [" + baseDeDestino + "].[dbo].[AuxiliaryAccount] ");
                builder.Append("([Code] ");
                builder.Append(",[Description] ");
                builder.Append(",[BookAccount] ");
                builder.Append(",[Status]) ");
                builder.Append("VALUES(");
                builder.Append(_legado.getNumerosNoTextoDoLegado(row["Conta Auxiliar"].ToString()));
                builder.Append("," + (row["Nome da Conta Auxiliar"] == DBNull.Value ? "NULL" : "'" + _legado.setTamanhoDeCaracteres(row["Nome da Conta Auxiliar"].ToString().Replace("'", ""), 60) + "'"));
                builder.Append("," + (row["Contabil Auxiliar"] == DBNull.Value ? "NULL" : _legado.getNumerosNoTextoDoLegado(row["Contabil Auxiliar"].ToString()).ToString()));
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