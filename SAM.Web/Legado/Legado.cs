using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
    public class Legado
    {
        private SqlConnection cnn = null;
        private SqlCommand cmd = null;
        private SqlDataAdapter apter = null;
        private StringBuilder comandoSql = null;
        private DbTransaction transaction = null;

        private int InstitutionId;
        private Legado(String stringDeConexao)
        {
            cnn = new SqlConnection(stringDeConexao);
            cnn.Open();
            comandoSql = new StringBuilder();
        }
        private Legado()
        {

        }
        private Legado(SqlConnection connection)
        {
            cnn = connection;
            cnn.Open();
            comandoSql = new StringBuilder();
        }
        internal Legado createTransaction(IsolationLevel level)
        {
            transaction = this.cnn.BeginTransaction(level, "LegadoImportacao");
            return this;
        }

        internal void Commit()
        {
            transaction.Commit();
        }
        internal void Rollback()
        {
            transaction.Rollback();
        }
        internal DbTransaction getTransaction()
        {
            return this.transaction;
        }
        internal static Legado createInstance()
        {
            return new Legado();
        }
        internal static Legado createInstance(String stringDeConexao)
        {
            return new Legado(stringDeConexao);
        }

        internal static Legado createInstance(SqlConnection connection)
        {
            return new Legado(connection);
        }
        internal DataTable getSchema(String tipoSchema)
        {
            return cnn.GetSchema(tipoSchema);
        }
        internal Legado createComando(String comandoSQL)
        {

            comandoSql = new StringBuilder();

            comandoSql.Append(comandoSQL);
            return this;
        }
        internal Legado fecharConexao(String comandoSQL)
        {
            if (this.cnn != null && this.cnn.State == ConnectionState.Open)
            {
                this.cnn.Close();
                this.cnn.Dispose();
            }
            return this;
        }
        internal int ExecuteComandoNoQuery(CommandType comandoType)
        {
            if (cmd == null)
                cmd = new SqlCommand();

            if (transaction != null)
                cmd.Transaction = transaction as SqlTransaction;

            cmd.CommandText = this.comandoSql.ToString();
            cmd.CommandType = comandoType;
            cmd.Connection = cnn;
            cmd.CommandTimeout = 0;
            int linhasFetadas = cmd.ExecuteNonQuery();
            this.comandoSql = null;

            return linhasFetadas;
        }

        internal Legado createParametro(SqlParameter  parametro)
        {
            cmd.Parameters.Add(parametro);
            return this;
        }
      

        internal Legado createAdpater(CommandType comandoType)
        {

            cmd = new SqlCommand();
            if (transaction != null)
                cmd.Transaction = transaction as SqlTransaction;

            cmd.CommandText = this.comandoSql.ToString();
            cmd.CommandType = comandoType;
            cmd.Connection = cnn;
            cmd.CommandTimeout = 0;
            apter = new SqlDataAdapter(this.cmd);
            
            return this;
        }
        internal SqlDataReader createDataReader(CommandType comandoType)
        {
            if (cmd == null)
                cmd = new SqlCommand();
            if (transaction != null)
                cmd.Transaction = transaction as SqlTransaction;

            cmd.CommandText = this.comandoSql.ToString();
            cmd.CommandType = comandoType;
            cmd.Connection = cnn;
            cmd.CommandTimeout = 0;
            SqlDataReader retorno = cmd.ExecuteReader(CommandBehavior.Default);

            this.comandoSql = null;
            cmd = null;
            return retorno;
        }

        internal DataTable createDataTable(string nomeDaTabela)
        {

            DataTable retorno = new DataTable(nomeDaTabela);
            apter.Fill(retorno);
            this.comandoSql = null;
            return retorno;
        }

        internal DataTable createDataTable(string nomeDaTabela, int currentIndex, int pageSize)
        {

            DataTable retorno = new DataTable(nomeDaTabela);
            apter.Fill(currentIndex, pageSize, retorno);
            this.comandoSql = null;
            return retorno;
        }
        
        internal void setInstitutionId(int Id)
        {
            this.InstitutionId = Id;
        }
        internal int getInstitutionId()
        {
            return this.InstitutionId;
        }

        internal string removerCaracteresCpfCnpj(string cpfCnpj, bool cpf)
        {
            if (!string.IsNullOrEmpty(cpfCnpj))
            {
                var retorno = cpfCnpj.Replace(".", "").Replace("/", "").Replace(" ", "").Replace("-", "");
                Int64 numero;
                Int64.TryParse(retorno, out numero);
                if (numero > 0)
                {
                    if (cpf)
                    {
                        if (retorno.Length > 11)
                        {
                            return string.Empty;
                        }

                        return retorno.PadLeft(11, '0');
                    }
                    else
                    {
                        if (retorno.Length > 14)
                        {
                            return string.Empty;
                        }

                        return retorno.PadLeft(14, '0');
                    }
                }
            }
            return string.Empty;
        }

        internal string removerCaracteresEspacoPontoBarraTracoApostrofa(string cpfCnpj)
        {
            return cpfCnpj.Replace(".", "").Replace("/", "").Replace(" ", "").Replace("-", "").Replace("'","");
        }
        internal void fecharConexao()
        {
            cnn.Close();
            cnn = null;
        }
        internal Int64 getNumerosNoTextoDoLegado(string texto)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in texto)
            {

                if (char.IsNumber(c) && builder.Length < 9)
                    builder.Append(c);

            }
            if (builder.Length < 1)
                builder.Append("0");

            return int.Parse(builder.ToString().Trim().TrimEnd().TrimStart().Replace(" ", ""));
        }


       
        public DateTime? conveterDataLegadoParaData(string dataLegado)
        {


            Int64 ano;
            Int64 mes;
            Int64 dia = 1;
            string data = dataLegado.Trim().TrimStart().TrimEnd().Replace(" ", "");

            if (data.Length == 8)
            {
                dia = getNumerosNoTextoDoLegado(data.Substring(0, 2));
                mes = getNumerosNoTextoDoLegado(data.Substring(data.Length - 6, 2));
                ano = getNumerosNoTextoDoLegado(data.Substring(data.Length - 4, 4));

            }
            else if (data.Length == 6)
            {
                mes = getNumerosNoTextoDoLegado(data.Substring(0, 2));
                ano = getNumerosNoTextoDoLegado(data.Substring(data.Length -4, 4));

            }
            else if (data.Length == 5)
            {
                mes = getNumerosNoTextoDoLegado(data.Substring(0, 1));
                ano = getNumerosNoTextoDoLegado(data.Substring(data.Length - 4, 4));
            }
            else
            {
                mes = DateTime.Now.Month;
                ano = DateTime.Now.Year;
            }
            try
            {
                DateTime date = new DateTime((int) ano, (int)mes, (int)dia);
                return date;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        
        private string limparCache()
        {
            StringBuilder builder = new StringBuilder();


            //Forçar a escrita das páginas em disco "limpando-as"
            builder.Append("CHECKPOINT ");
            builder.Append(Environment.NewLine);
            //Eliminar as páginas de buffer limpas
            builder.Append("DBCC DROPCLEANBUFFERS ");
            builder.Append(Environment.NewLine);
            //Eliminar todas as entradas do CACHE de "Procedures"
            builder.Append("DBCC FREEPROCCACHE ");
            builder.Append(Environment.NewLine);

            //Limpar as entradas de Cache não utilizadas
            builder.Append("DBCC FREESYSTEMCACHE ('ALL')");
            builder.Append(Environment.NewLine);

            return builder.ToString();
        }
        //private DataTable getDadosTable(ref SqlConnection cnn, string nomeDaPlanilha, String comandoSql)
        //{

        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cnn;
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandText = comandoSql;
        //    cmd.CommandTimeout = 0;
        //    SqlDataAdapter adpter = new SqlDataAdapter(cmd);

        //    DataTable retorno = new DataTable(nomeDaPlanilha);

        //    adpter.Fill(retorno);

        //    return retorno;
        //}

        public string setTamanhoDeCaracteres(string campo, UInt16 quantidadeCaracteres)
        {

            if (campo.Length > quantidadeCaracteres)
            {
                return campo.Substring(0, quantidadeCaracteres);
            }
            else
            {
                return campo;
            }

        }
    }
}