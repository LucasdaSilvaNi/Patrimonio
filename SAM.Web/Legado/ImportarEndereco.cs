using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarEndereco
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = new SAMContext();
        public ImportarEndereco(Legado legado,string logradouro, int numero, string complemento, string bairro, string cidade, string estado, string cep, string baseDeDestino)
        {
            this._legado = legado;
            int linhasFetadas;
            if (!string.IsNullOrWhiteSpace(cep))
                linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLEndereco(logradouro, numero, complemento, bairro, cidade, estado, cep, baseDeDestino)); //_legado.createComando(createSQLEndereco(logradouro, numero, complemento, bairro, cidade, estado, cep, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);


        }

        private String createSQLEndereco(string logradouro,int numero,string complemento,string bairro,string cidade, string estado, string cep, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [dbo].[Address] WHERE [PostalCode] = '" + _legado.removerCaracteresEspacoPontoBarraTracoApostrofa(cep) + "')");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append("INSERT INTO [" + baseDeDestino + "].[dbo].[Address] ");
                builder.Append("([Street] ");
                builder.Append(",[Number] ");
                builder.Append(",[ComplementAddress] ");
                builder.Append(",[District] ");
                builder.Append(",[City] ");
                builder.Append(",[State] ");
                builder.Append(",[PostalCode]) ");
                builder.Append("VALUES(");
                builder.Append((logradouro.Trim().TrimEnd().TrimStart().Length < 1 ? "NULL" : "'" + logradouro.Replace("'","")  + "'"));
                builder.Append("," + (numero < 1 ? "NULL" :  numero.ToString()));
                builder.Append("," + (complemento.TrimStart().TrimEnd().Trim().Length < 1 ? "NULL" : "'" + complemento.Replace("'","") +"'"));
                builder.Append("," + (String.IsNullOrWhiteSpace(bairro) ? "NULL" : "'" + bairro.Replace("'","") + "'"));
                builder.Append("," +(String.IsNullOrWhiteSpace(cidade) ? "NULL" : "'" + cidade.Replace("'","") + "'"));
                builder.Append("," + (String.IsNullOrWhiteSpace(estado) ? "NULL" : "'" + estado.Replace("'","") + "'"));
                builder.Append("," + (String.IsNullOrWhiteSpace(cep) ? "NULL" : "'" + _legado.removerCaracteresEspacoPontoBarraTracoApostrofa(cep) + "')"));
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