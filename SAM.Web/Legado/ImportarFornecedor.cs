using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class ImportarFornecedor
    {
        private Legado _legado = null;
        private StringBuilder builder = null;
        private SAMContext contexto = null;

        public ImportarFornecedor(SAMContext contexto,Legado legado,DataRowCollection rows, string baseDeDestino)
        {
            this._legado = legado;
            this.contexto = contexto;

            foreach (DataRow row in rows)
            {
                try
                {
                    
                        if (row["Nome do Fornecedor"] != null && row["Nome do Fornecedor"].ToString().Replace(" ", "").Length > 0)
                        {
                            string logradouro = (row["Logradouro do Fornecedor"] == DBNull.Value ? "" : row["Logradouro do Fornecedor"].ToString());
                            int numero = 0;
                            string complemento = (row["Complemento do Fornecedor"] == DBNull.Value ? "" : row["Complemento do Fornecedor"].ToString());
                            string bairro = "";
                            string cidade = (row["Cidade do Fornecedor"] == DBNull.Value ? "" : row["Cidade do Fornecedor"].ToString());
                            string estado = (row["Estado do Fornecedor"] == DBNull.Value ? "" : row["Estado do Fornecedor"].ToString());
                            string cep = (row["Cep do Fornecedor"] == DBNull.Value ? "" : row["Cep do Fornecedor"].ToString());

                            ImportarEndereco endereco;
                            if (!string.IsNullOrWhiteSpace(cep))
                                endereco = new ImportarEndereco(this._legado, logradouro, numero, complemento, bairro, cidade, estado, cep, baseDeDestino);

                            int linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLFornecedor(row, baseDeDestino)); //_legado.createComando(createSQLFornecedor(row, baseDeDestino)).ExecuteComandoNoQuery(CommandType.Text);
                            if (linhasFetadas < 1)
                            {
                                row["Fornecedor Importado"] = "Não";
                                row["Fornecedor Mensagem"] = "Fornecedor já cadastrado";
                            }
                        }else
                        {
                            row["Fornecedor Importado"] = "Não";
                            row["Fornecedor Mensagem"] = "Bem Patrimonial sem fornecedor";
                        }
                    

                }
                catch (Exception ex)
                {
                    row["Fornecedor Importado"] = "Não";
                    row["Fornecedor Mensagem"] = ex.Message;
                }
            }
        }

        private String createSQLFornecedor(DataRow row, string baseDeDestino)
        {
            builder = new StringBuilder();

            try
            {
                builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[Supplier] WHERE [Name] = '" + _legado.setTamanhoDeCaracteres(row["Nome do Fornecedor"].ToString().Replace("'", ""), 60) + "' AND ISNULL([CPFCNPJ],'') =" + (row["CNPJ do Fornecedor"] == DBNull.Value ? "''" : "'" + _legado.removerCaracteresCpfCnpj(row["CNPJ do Fornecedor"].ToString(), false) + "'") + ")");
                builder.Append(Environment.NewLine);
                builder.Append("BEGIN");
                builder.Append(Environment.NewLine);
                builder.Append("INSERT INTO [" + baseDeDestino + "].[dbo].[Supplier] ");
                builder.Append(" ([CPFCNPJ] ");
                builder.Append(",[Name] ");
                builder.Append(",[AddressId] ");
                builder.Append(",[Telephone] ");
                builder.Append(",[AdditionalData] ");
                builder.Append(",[Status]) ");

                builder.Append(" VALUES(");
                builder.Append("'" + _legado.removerCaracteresCpfCnpj(row["CNPJ do Fornecedor"].ToString(), false) + "'");
                builder.Append(",'" + _legado.setTamanhoDeCaracteres(row["Nome do Fornecedor"].ToString().Replace("'", ""), 60) + "'");
                builder.Append(",(SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[Address] WHERE [PostalCode] = '" + _legado.removerCaracteresEspacoPontoBarraTracoApostrofa(row["Cep do Fornecedor"].ToString()) + "')");
                builder.Append("," + (row["Telefone do Fornecedor"] == DBNull.Value ? "NULL" : "'" + _legado.getNumerosNoTextoDoLegado(row["Telefone do Fornecedor"].ToString()) + "'"));
                builder.Append(",NULL");
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