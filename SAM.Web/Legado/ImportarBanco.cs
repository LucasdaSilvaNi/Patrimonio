
using ExcelDataReader;
using Newtonsoft.Json.Linq;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Legado;
using SAM.Web.Models;
using SAM.Web.OpenXml;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SAM.Web.Legados
{
    public class ImportarBanco
    {
        private SAMContext db = new SAMContext();
        private Legado _legado = null;
        private StringBuilder builder = null;
        private IList<RetornoImportacao> mensagensDeRetorno = null;
        private string caminhoFisicoDosArquivos = string.Empty;
        private string caminhoVirtualDosArquivos = string.Empty;
        private SAMContext contexto = null;
        private IList<ChaveBemPatrimonialJson> AssetsImportados = null;
        private StringBuilder builderException = null;
        private String _codigoOrgao = string.Empty;
        private String _Uge = string.Empty;
        private SqlConnection cnn;
        private DataTable tableExcel = null;
        private const String sufixoPlanilhaImportada = ".xlsx";

        private ImportarBanco(SAMContext contexto, string caminhoFisicoDosArquivos, string caminhoVirtualDosArquivos)
        {
            this.contexto = contexto;
            _legado = Legado.createInstance(this.contexto.Database.Connection.ConnectionString);

            this.caminhoFisicoDosArquivos = caminhoFisicoDosArquivos;
            this.caminhoVirtualDosArquivos = caminhoVirtualDosArquivos;
        }

        public static ImportarBanco createInstance(SAMContext contexto, string caminhoFisicoDosArquivos, string caminhoVirtualDosArquivos)
        {
            return new Legados.ImportarBanco(contexto, caminhoFisicoDosArquivos, caminhoVirtualDosArquivos);
        }
        public void createTransaction()
        {
            this.contexto.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
        }
        public void Rollback()
        {
            this.contexto.Database.CurrentTransaction.Rollback();
        }
        public void Commit()
        {
            this.contexto.Database.CurrentTransaction.Commit();
        }
        public void fecharConexao()
        {
            SqlCommand cmd = new SqlCommand(limparCache());
            cmd.Connection = cnn;
            cmd.ExecuteNonQuery();
            cnn.Close();
            cnn = null;
        }
        private string limparCache()
        {
            builder = new StringBuilder();


            //Forçar a escrita das páginas em disco "limpando-as"
            builder.Append("CHECKPOINT");
            builder.Append("GO");

            //Eliminar as páginas de buffer limpas
            builder.Append("DBCC DROPCLEANBUFFERS");
            builder.Append("GO");

            //Eliminar todas as entradas do CACHE de "Procedures"
            builder.Append("DBCC FREEPROCCACHE");
            builder.Append("GO");
            //Limpar as entradas de Cache não utilizadas
            builder.Append("DBCC FREESYSTEMCACHE ('ALL')");
            builder.Append("GO");

            return builder.ToString();
        }
        public IList<ExportarLegadoViewModel> getOrgaosSeremExportados(string nomeDaBase)
        {
            IList<ExportarLegadoViewModel> Models = new List<ExportarLegadoViewModel>();
            builder = new StringBuilder();

            builder.Append("SELECT [Id]");
            builder.Append(",[Description]");
            builder.Append(" FROM [" + nomeDaBase + "].[dbo].[Institution]");

            IDataReader reader = _legado.createComando(builder.ToString()).createDataReader(CommandType.Text);
            ExportarLegadoViewModel model;
            while (reader.Read())
            {
                model = new ExportarLegadoViewModel();
                model.IdOrgao = reader.GetInt32(0);
                model.orgaoNome = reader.GetString(1);

                Models.Add(model);
                model = null;
            }


            return Models;

        }

        public IList<ImportarLegadoViewModel> getDatabaseOrigemLocal()
        {
            IList<ImportarLegadoViewModel> Databases = new List<ImportarLegadoViewModel>();
            ImportarLegadoViewModel model = new ImportarLegadoViewModel();
            model.IdOrigem = 0;
            model.baseDeOrigem = "Selecione Origem";

            Databases.Add(model);
            model = null;

            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
            {

                DataTable databases = _legado.getSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    String databaseName = database.Field<String>("database_name");
                    short dbID = database.Field<short>("dbid");
                    //DateTime creationDate = database.Field<DateTime>("create_date");
                    //databaseName.ToLower().Contains("legado") && 
                    if (!databaseName.ToLower().Contains("master") && !databaseName.ToLower().Contains("model") && !databaseName.ToLower().Contains("msdb") && !databaseName.ToLower().Contains("tempdb"))
                    {

                        model = new ImportarLegadoViewModel();
                        model.IdOrigem = dbID;
                        model.baseDeOrigem = databaseName;

                        Databases.Add(model);
                        model = null;
                    }


                }
            }

            return Databases;
        }

        //public IList<ImportarLegadoViewModel> getDatabaseDetino()
        //{
        //    IList<ImportarLegadoViewModel> Databases = new List<ImportarLegadoViewModel>();
        //    ImportarLegadoViewModel model = new ImportarLegadoViewModel();
        //    model.IdDestino = 0;
        //    model.baseDeDestino = "Selecione Destino";

        //    Databases.Add(model);
        //    model = null;

        //    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
        //    {

        //        DataTable databases = _legado.getSchema("Databases");
        //        foreach (DataRow database in databases.Rows)
        //        {
        //            String databaseName = database.Field<String>("database_name");
        //            short dbID = database.Field<short>("dbid");

        //            if (!databaseName.ToLower().Contains("legado") && !databaseName.ToLower().Contains("reportserver") && !databaseName.ToLower().Contains("master") && !databaseName.ToLower().Contains("model") && !databaseName.ToLower().Contains("msdb") && !databaseName.ToLower().Contains("tempdb"))
        //            {
        //                model = new ImportarLegadoViewModel();
        //                model.IdDestino = dbID;
        //                model.baseDeDestino = databaseName;

        //                Databases.Add(model);
        //                model = null;
        //            }
        //        }
        //    }

        //    return Databases;
        //}
        public IList<ImportarLegadoViewModel> getDatabaseOrigem()
        {
            IList<ImportarLegadoViewModel> Databases = new List<ImportarLegadoViewModel>();
            ImportarLegadoViewModel model = new ImportarLegadoViewModel();
            model.baseDeDestino = "Selecione Destino";

            Databases.Add(model);
            model = null;

            model = new ImportarLegadoViewModel();
            model.IdOrigem = 10;
            model.IdDestino = 10;
            model.baseDeOrigem = contexto.Database.Connection.Database;
            model.baseDeDestino = contexto.Database.Connection.Database;
            Databases.Add(model);
            model = null;

            return Databases;
        }
        public IList<ImportarLegadoViewModel> getDatabaseDetino()
        {
            IList<ImportarLegadoViewModel> Databases = new List<ImportarLegadoViewModel>();
            ImportarLegadoViewModel model = new ImportarLegadoViewModel();
            model.IdDestino = 0;
            model.baseDeDestino = "Selecione Destino";

            Databases.Add(model);
            model = null;

            model = new ImportarLegadoViewModel();
            model.IdDestino = 10;
            model.baseDeDestino = contexto.Database.Connection.Database;
            Databases.Add(model);
            model = null;

            return Databases;
        }
        public IDictionary<string, IList<RetornoImportacao>> executarImportacao(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
            {
                try
                {
                    IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    _legado.createComando("IMPORTAR_DADOS_DO_LEGADO");

                    SqlParameter parametroOrigem = new SqlParameter("@BASE_DADOS_ORIGEM", SqlDbType.VarChar, 100);
                    SqlParameter parametroDestino = new SqlParameter("@BASE_DADOS_DESTINO", SqlDbType.VarChar, 100);

                    parametroOrigem.Value = nomeDaBaseOrigem;
                    _legado.createParametro(parametroOrigem);

                    parametroDestino.Value = nomeDaBaseDestino;
                    _legado.createParametro(parametroDestino);

                    _legado.ExecuteComandoNoQuery(CommandType.StoredProcedure);

                    ExportarParaExcel excel = ExportarParaExcel.createInstance();
                    DataSet dataSet = new DataSet();

                    mensagensDeRetorno = new List<RetornoImportacao>();

                    dataSet.Tables.AddRange(getTabelasParaExportarImportadosParaExcel(nomeDaBaseOrigem, nomeDaBaseDestino));
                    excel.ExportDataSet(dataSet, getCaminhoFisicoDoArquivoParaDownloadImportados(nomeDaBaseOrigem));//@"C:\Prodesp\Desenvolvimento\main\SAM.Web\Legado\Teste\Importado.xlsx");
                    resultado.Add("Importado", mensagensDeRetorno);
                    dataSet = null;

                    mensagensDeRetorno = new List<RetornoImportacao>();
                    dataSet = new DataSet();
                    dataSet.Tables.AddRange(getTabelasParaExportarNaoImportadosParaExcel(nomeDaBaseOrigem, nomeDaBaseDestino));
                    excel.ExportDataSet(dataSet, getCaminhoFisicoDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem));//@"C:\Prodesp\Desenvolvimento\main\SAM.Web\Legado\Teste\NaoImportado.xlsx");

                    resultado.Add("NaoImportado", mensagensDeRetorno);
                    dataSet = null;

                    transaction.Complete();


                    return resultado;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public string getCaminhoExcelFisicoDoArquivoParaDownloadImportados(string baseDeOrigem)
        {
            return caminhoFisicoDosArquivos + "\\" + baseDeOrigem + sufixoPlanilhaImportada;
        }
        public string getCaminhoExcelVirtualDoArquivoParaDownloadImportado(string baseDeOrigem)
        {
            return caminhoVirtualDosArquivos + "/" + baseDeOrigem + sufixoPlanilhaImportada;
        }



        public string getCaminhoFisicoDoArquivoParaDownloadNaoImportados(string baseDeOrigem)
        {
            return caminhoFisicoDosArquivos + "\\" + baseDeOrigem + "_NaoImportado.xlsx";
        }
        public string getCaminhoFisicoDoArquivoParaDownloadImportados(string baseDeOrigem)
        {
            return caminhoFisicoDosArquivos + "\\" + baseDeOrigem + "_Importado.xlsx";
        }
        public string getCaminhoFisicoDoArquivoDeUaParaDownloadSerCorrigidas(string baseDeOrigem)
        {
            return caminhoFisicoDosArquivos + "\\" + baseDeOrigem + "_UA_SER_CORRIGIDAS.xlsx";
        }

        public string getCaminhoVirtualDoArquivoParaDownloadNaoImportados(string baseDeOrigem)
        {
            return caminhoVirtualDosArquivos + "/" + baseDeOrigem + "_NaoImportado.xlsx";
        }
        public string getCaminhoVirtualDoArquivoDeUaParaDownloadSerCorrigidas(string baseDeOrigem)
        {
            return caminhoVirtualDosArquivos + "/" + baseDeOrigem + "_UA_SER_CORRIGIDAS.xlsx";
        }
        public string getCaminhoVirtualDoArquivoParaDownloadImportados(string baseDeOrigem)
        {
            return caminhoVirtualDosArquivos + "/" + baseDeOrigem + "_Importado.xlsx";
        }

        private DataTable[] getTabelasParaExportarNaoImportadosParaExcel(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            DataTable[] dataTables = new DataTable[7];
            RetornoImportacao mensagemDeRetorno = new RetornoImportacao();


            var itemMaterial = _legado.createComando(createSQLDosDadosDeItemDeMaterialNaoImportados(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                         .createAdpater(CommandType.Text)
                                                                         .createDataTable("ItemMaterial");
            mensagemDeRetorno.mensagemImportacao = "Item de Material";
            mensagemDeRetorno.quantidadeDeRegistros = itemMaterial.Rows.Count;

            if (itemMaterial.Rows.Count > 0)
            {
                dataTables[0] = itemMaterial;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem);
            }
            mensagensDeRetorno.Add(mensagemDeRetorno);

            var fornecedor = _legado.createComando(createSQLDosDadosDeFornecedorNaoImportados(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                         .createAdpater(CommandType.Text)
                                                                         .createDataTable("Fornecedor");
            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Fornecedor";
            mensagemDeRetorno.quantidadeDeRegistros = fornecedor.Rows.Count;

            if (fornecedor.Rows.Count > 0)
            {
                dataTables[1] = fornecedor;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem);
            }
            mensagensDeRetorno.Add(mensagemDeRetorno);

            var responsavel = _legado.createComando(createSQLDosDadosDeResponsavelNaoImportados(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                         .createAdpater(CommandType.Text)
                                                                         .createDataTable("Responsavel");

            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Responsavel";
            mensagemDeRetorno.quantidadeDeRegistros = responsavel.Rows.Count;

            if (responsavel.Rows.Count > 0)
            {
                dataTables[2] = responsavel;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem);
            }
            mensagensDeRetorno.Add(mensagemDeRetorno);

            var contaAuxiliar = _legado.createComando(createSQLDasContasAuxiliaresNaoImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                      .createAdpater(CommandType.Text)
                                                                     .createDataTable("ContaAuxiliar");

            if (contaAuxiliar.Rows.Count > 0)
            {
                dataTables[3] = contaAuxiliar;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem);
            }
            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Conta Auxiliar";
            mensagemDeRetorno.quantidadeDeRegistros = contaAuxiliar.Rows.Count;

            mensagensDeRetorno.Add(mensagemDeRetorno);

            var Terceiro = _legado.createComando(createSQLDosTerceirosNaoImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                       .createAdpater(CommandType.Text)
                                                                       .createDataTable("Terceiro");

            if (Terceiro.Rows.Count > 0)
            {
                dataTables[4] = Terceiro;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem);
            }
            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Terceiro";
            mensagemDeRetorno.quantidadeDeRegistros = Terceiro.Rows.Count;


            mensagensDeRetorno.Add(mensagemDeRetorno);

            var Sigla = _legado.createComando(createSQLDasSiglasNaoImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                       .createAdpater(CommandType.Text)
                                                                       .createDataTable("Sigla");
            if (Sigla.Rows.Count > 0)
            {
                dataTables[5] = Sigla;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem);
            }
            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Sigla";
            mensagemDeRetorno.quantidadeDeRegistros = Sigla.Rows.Count;

            mensagensDeRetorno.Add(mensagemDeRetorno);

            //var tipoDeIncorporacao = _legado.createComando(createSQLDosTiposDeIncorporacoesNaoImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
            //                                                           .createAdpater(CommandType.Text)
            //                                                           .createDataTable("TipoDeIncorporacao");

            //if (tipoDeIncorporacao.Rows.Count > 0)
            //{
            //    dataTables[6] = tipoDeIncorporacao;
            //    mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados();
            //}
            //mensagemDeRetorno = new RetornoImportacao();
            //mensagemDeRetorno.mensagemImportacao = "Tipo de Incorporacao";
            //mensagemDeRetorno.quantidadeDeRegistros = tipoDeIncorporacao.Rows.Count;

            //mensagensDeRetorno.Add(mensagemDeRetorno);

            var bemPatrimonial = _legado.createComando(createSQLDosBensPatrimoniaisNaoImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                       .createAdpater(CommandType.Text)
                                                                       .createDataTable("BemPatrimonial");

            foreach (DataRow row in bemPatrimonial.Rows)
            {
                if (AssetsImportados != null)
                {
                    var _Chapa = row["CHAPA"].ToString();
                    var retorno = this.AssetsImportados.Where(x => x.Chapa.Trim().TrimEnd().TrimStart() == _Chapa.Trim().TrimEnd().TrimStart() && x.BemCadastrado == true).FirstOrDefault();

                    if (retorno != null)
                        row[0] = retorno.Mensagem;
                    else
                        row[0] = "Por favor Verificar Sigla, Tipo de incorporação, Responsável e se as UAs estão com o código correto.";
                }
                else
                    row[0] = "Por favor Verificar o bem patrimonial!";
            }



            if (bemPatrimonial.Rows.Count > 0)
            {
                dataTables[6] = bemPatrimonial;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadNaoImportados(nomeDaBaseOrigem);
            }
            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Bem Patrimonial";
            mensagemDeRetorno.quantidadeDeRegistros = bemPatrimonial.Rows.Count;
            //if (column.ColumnName.Equals("MENSAGEM"))
            //    column.DefaultValue = this.chaves.Where(x => x.Chapa = )

            mensagensDeRetorno.Add(mensagemDeRetorno);

            return dataTables;
        }
        private int getInventarioQuantidadeBemPatrimonial(string nomeDaBaseOrigem)
        {
            int quantidade = 0;

            var Inventario = _legado.createComando(createSQLInventario(nomeDaBaseOrigem)).createAdpater(CommandType.Text).createDataTable("Inventario");
            quantidade = Inventario.Rows.Count;

            return quantidade;
        }
        private DataTable[] getInventarioBemPatrimonial(string nomeDaBaseOrigem, int currentIndex, int pageSize)
        {
            var Inventario = _legado.createComando(createSQLInventario(nomeDaBaseOrigem))
                                                                         .createAdpater(CommandType.Text)
                                                                         .createDataTable("Inventario", currentIndex, pageSize);

            DataTable[] dataTables = new DataTable[1];

            dataTables[0] = Inventario;

            return dataTables;
        }

        private DataTable[] getTabelasParaExportarImportadosParaExcel(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            DataTable[] dataTables = new DataTable[7];
            RetornoImportacao mensagemDeRetorno = new RetornoImportacao();

            var itemMaterial = _legado.createComando(createSQLDosDadosItemDeMaterialImportados(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                         .createAdpater(CommandType.Text)
                                                                         .createDataTable("ItemMaterial");
            mensagemDeRetorno.mensagemImportacao = "Item de Matrial";
            mensagemDeRetorno.quantidadeDeRegistros = itemMaterial.Rows.Count;

            if (itemMaterial.Rows.Count > 0)
            {
                dataTables[0] = itemMaterial;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados(nomeDaBaseOrigem);
            }

            mensagensDeRetorno.Add(mensagemDeRetorno);

            var fornecedor = _legado.createComando(createSQLDosDadosDeFornecedorImportados(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                        .createAdpater(CommandType.Text)
                                                                        .createDataTable("Fornecedor");
            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Fornecedor";
            mensagemDeRetorno.quantidadeDeRegistros = fornecedor.Rows.Count;

            if (fornecedor.Rows.Count > 0)
            {
                dataTables[1] = fornecedor;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados(nomeDaBaseOrigem);
            }

            mensagensDeRetorno.Add(mensagemDeRetorno);


            var responsavel = _legado.createComando(createSQLDosDadosDeResponsavelImportados(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                         .createAdpater(CommandType.Text)
                                                                         .createDataTable("Responsavel");
            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Responsavel";
            mensagemDeRetorno.quantidadeDeRegistros = responsavel.Rows.Count;

            if (responsavel.Rows.Count > 0)
            {
                dataTables[2] = responsavel;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados(nomeDaBaseOrigem);
            }

            mensagensDeRetorno.Add(mensagemDeRetorno);

            var contaAuxiliar = _legado.createComando(createSQLDasContasAuxiliaresImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                      .createAdpater(CommandType.Text)
                                                                     .createDataTable("ContaAuxiliar");

            if (contaAuxiliar.Rows.Count > 0)
            {
                dataTables[3] = contaAuxiliar;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados(nomeDaBaseOrigem);
            }

            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Conta Auxiliar";
            mensagemDeRetorno.quantidadeDeRegistros = contaAuxiliar.Rows.Count;

            mensagensDeRetorno.Add(mensagemDeRetorno);

            var Terceiro = _legado.createComando(createSQLDosTerceirosImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                       .createAdpater(CommandType.Text)
                                                                       .createDataTable("Terceiro");
            if (Terceiro.Rows.Count > 0)
            {
                dataTables[4] = Terceiro;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados(nomeDaBaseOrigem);
            }

            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Terceiro";
            mensagemDeRetorno.quantidadeDeRegistros = Terceiro.Rows.Count;

            mensagensDeRetorno.Add(mensagemDeRetorno);

            var Sigla = _legado.createComando(createSQLDasSiglasImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                       .createAdpater(CommandType.Text)
                                                                       .createDataTable("Sigla");

            if (Sigla.Rows.Count > 0)
            {
                dataTables[5] = Sigla;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados(nomeDaBaseOrigem);
            }

            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Sigla";
            mensagemDeRetorno.quantidadeDeRegistros = Sigla.Rows.Count;

            mensagensDeRetorno.Add(mensagemDeRetorno);

            //var tipoDeIncorporacao = _legado.createComando(createSQLDosTiposDeIncorporacoesImportadas(nomeDaBaseOrigem, nomeDaBaseDestino))
            //                                                           .createAdpater(CommandType.Text)
            //                                                           .createDataTable("TipoDeIncorporacao");

            //if (tipoDeIncorporacao.Rows.Count > 0)
            //{
            //    dataTables[6] = tipoDeIncorporacao;
            //    mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados();
            //}

            //mensagemDeRetorno = new RetornoImportacao();
            //mensagemDeRetorno.mensagemImportacao = "Tipo de Incorporacao";
            //mensagemDeRetorno.quantidadeDeRegistros = tipoDeIncorporacao.Rows.Count;

            //mensagensDeRetorno.Add(mensagemDeRetorno);

            var bemPatrimonial = _legado.createComando(createSQLDosBensPatrimoniaisImportados(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                       .createAdpater(CommandType.Text)
                                                                       .createDataTable("BemPatrimonial");

            if (bemPatrimonial.Rows.Count > 0)
            {
                dataTables[6] = bemPatrimonial;
                mensagemDeRetorno.caminhoDoArquivoDownload = getCaminhoVirtualDoArquivoParaDownloadImportados(nomeDaBaseOrigem);
            }

            mensagemDeRetorno = new RetornoImportacao();
            mensagemDeRetorno.mensagemImportacao = "Bem Patrimonial";
            mensagemDeRetorno.quantidadeDeRegistros = bemPatrimonial.Rows.Count;

            mensagensDeRetorno.Add(mensagemDeRetorno);

            return dataTables;
        }
        private String createSQLDosBensPatrimoniaisNaoImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append("SELECT bem.* FROM(SELECT DISTINCT '' as 'MENSAGEM', [bem].[SIGLA]");
            builder.Append(",[bem].[CHAPA]");
            builder.Append(",[bem].[DESD]");
            builder.Append(",[bem].[SIGLA_UA]");
            builder.Append(",[bem].[COD_RESP]");
            builder.Append(",[bem].[COD_ORGC]");
            builder.Append(",[bem].[COD_UGEC]");
            builder.Append(",[bem].[COD_ITEMMAT]");
            builder.Append(",[bem].[CONTA_AUX]");
            builder.Append(",[bem].[DT_INCL]");
            builder.Append(",[bem].[DT_AQUI]");
            builder.Append(",[bem].[VL_AQUI]");
            builder.Append(",[bem].[VL_ATU]");
            builder.Append(",[bem].[NP_COMPRA]");
            builder.Append(",[bem].[NGPB]");
            builder.Append(",[bem].[NFISCAL]");
            builder.Append(",[bem].[NSERIE]");
            builder.Append(",[bem].[NFABRIC]");
            builder.Append(",[bem].[DT_GARAN]");
            builder.Append(",[bem].[NCHASSI]");
            builder.Append(",[bem].[MARCA]");
            builder.Append(",[bem].[MODELO]");
            builder.Append(",[bem].[HISTOR1]");
            builder.Append(",[bem].[HISTOR2]");
            builder.Append(",[bem].[HISTOR3]");
            builder.Append(",[bem].[HISTOR4]");
            builder.Append(",[bem].[HISTOR5]");
            builder.Append(",[bem].[EST_CONSER]");
            builder.Append(",[bem].[TIP_MOV]");
            builder.Append(",[bem].[SITUACAO]");
            builder.Append(",[bem].[CLASSIFICACAO]");
            builder.Append(",[bem].[DT_BOLSA]");
            builder.Append(",[bem].[COD_TERC]");
            builder.Append(",[bem].[DT_TERMO]");
            builder.Append(",[bem].[COD_TPINC]");
            builder.Append(",[bem].[SIGLA_ANTIGA]");
            builder.Append(",[bem].[CHAPA_ANTIGA]");
            builder.Append(",[bem].[DESD_ANTIGO]");
            builder.Append(",[bem].[Codigo_do_Fornecedor]");
            builder.Append(",[bem].[COMPL_DESCR]");
            builder.Append(",[bem].[DT_INVENT]");
            builder.Append(",[bem].[NEMPENHO]");
            builder.Append(",[bem].[DT_ATRIB_VALOR]");
            builder.Append(",GETDATE() 'DT_RECEB_TERMO' ");
            builder.Append(",[bem].[PLACA]");
            builder.Append(",(SELECT TOP 1 [Codigo_da_ND] FROM [" + nomeDaBaseOrigem + "].[dbo].[SubItemMaterial] ");
            builder.Append("     WHERE [Codigo_do_Item] = [bem].[COD_ITEMMAT] ");
            builder.Append(") AS 'Natureza_Despesa'  ");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] WITH(NOLOCK)");
            builder.Append("  INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] WITH(NOLOCK) ON [ua].[SIGLA_UA] =[bem].[SIGLA_UA] ) bem ");
            builder.Append(" WHERE [bem].[CHAPA] NOT IN(SELECT bem.NumberIdentification   FROM [" + nomeDaBaseDestino + "].[dbo].[Asset] bem  WITH(NOLOCK) ");
            builder.Append("                                INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[AssetMovements] [assetMov] WITH(NOLOCK) ON [assetMov].[AssetId] = [bem].[Id] ");
            builder.Append("                                INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] [unit] WITH(NOLOCK) ON [unit].[Id] = [assetMov].[AdministrativeUnitId] ");
            builder.Append("                                WHERE CONVERT(int, [unit].[Code]) IN (SELECT CONVERT(int, [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO]([ua].[SIGLA_UA])) FROM [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] WITH(NOLOCK)) ");
            builder.Append("                             ) ");


            return builder.ToString();
        }
        private String createSQLDosBensPatrimoniaisNaoSemRegraImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append("SELECT DISTINCT '' as 'MENSAGEM', [bem].[SIGLA]");
            builder.Append(",[bem].[CHAPA]");
            builder.Append(",[bem].[DESD]");
            builder.Append(",[bem].[SIGLA_UA]");
            builder.Append(",[bem].[COD_RESP]");
            builder.Append(",[bem].[COD_ORGC]");
            builder.Append(",[bem].[COD_UGEC]");
            builder.Append(",[bem].[COD_ITEMMAT]");
            builder.Append(",[bem].[CONTA_AUX]");
            builder.Append(",[bem].[DT_INCL]");
            builder.Append(",[bem].[DT_AQUI]");
            builder.Append(",[bem].[VL_AQUI]");
            builder.Append(",[bem].[VL_ATU]");
            builder.Append(",[bem].[NP_COMPRA]");
            builder.Append(",[bem].[NGPB]");
            builder.Append(",[bem].[NFISCAL]");
            builder.Append(",[bem].[NSERIE]");
            builder.Append(",[bem].[NFABRIC]");
            builder.Append(",[bem].[DT_GARAN]");
            builder.Append(",[bem].[NCHASSI]");
            builder.Append(",[bem].[MARCA]");
            builder.Append(",[bem].[MODELO]");
            builder.Append(",[bem].[HISTOR1]");
            builder.Append(",[bem].[HISTOR2]");
            builder.Append(",[bem].[HISTOR3]");
            builder.Append(",[bem].[HISTOR4]");
            builder.Append(",[bem].[HISTOR5]");
            builder.Append(",[bem].[EST_CONSER]");
            builder.Append(",[bem].[TIP_MOV]");
            builder.Append(",[bem].[SITUACAO]");
            builder.Append(",[bem].[CLASSIFICACAO]");
            builder.Append(",[bem].[DT_BOLSA]");
            builder.Append(",[bem].[COD_TERC]");
            builder.Append(",[bem].[DT_TERMO]");
            builder.Append(",[bem].[COD_TPINC]");
            builder.Append(",[bem].[SIGLA_ANTIGA]");
            builder.Append(",[bem].[CHAPA_ANTIGA]");
            builder.Append(",[bem].[DESD_ANTIGO]");
            builder.Append(",[bem].[Codigo_do_Fornecedor]");
            builder.Append(",[bem].[COMPL_DESCR]");
            builder.Append(",[bem].[DT_INVENT]");
            builder.Append(",[bem].[NEMPENHO]");
            builder.Append(",[bem].[DT_ATRIB_VALOR]");
            builder.Append(",GETDATE() 'DT_RECEB_TERMO' ");
            builder.Append(",[bem].[PLACA]");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ");
            builder.Append("  INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] =[bem].[SIGLA_UA] ");

            return builder.ToString();
        }

        private String createSQLDosTiposDeIncorporacoesNaoImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT bem.MENSAGEM, bem.COD_TPINC ,bem.COD_TRANS,bem.DESCRICAO FROM( ");
            builder.Append("        SELECT DISTINCT 'Verificar código da UGA' as 'MENSAGEM', tp.COD_TPINC ,tp.COD_TRANS,tp.DESCRICAO,bem.COD_ORGC,[uo].[COD_UGO] ");
            builder.Append("          FROM [" + nomeDaBaseOrigem + "].[dbo].[TipoIncorp] tp ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON[bem].[COD_TPINC] = tp.COD_TPINC ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UGO] [uo] ON[uo].[COD_ORG] = bem.COD_ORGC) bem ");
            builder.Append(" INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[BudgetUnit] bug ON bug.Code <> bem.COD_UGO ");
            builder.Append(" INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[Institution] it ON it.Code <> bem.COD_ORGC ");

            return builder.ToString();
        }
        private String createSQLDasSiglasNaoImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT 'Verificar código da UGA' as 'MENSAGEM', REPLACE(LTRIM(RTRIM(bem.SIGLA)),' ',''),bem.[DESCRICAO],bem.[IND_PROPRIO],bem.[COD_SIGLA],bem.[IND_COD_BARRA] FROM( ");
            builder.Append("   SELECT DISTINCT [sigla].[SIGLA],[sigla].[DESCRICAO],[sigla].[IND_PROPRIO],[sigla].[COD_SIGLA],[sigla].[IND_COD_BARRA],[bem].COD_ITEMMAT 'Codigo_Item',[bem].[SIGLA_UA] ");
            builder.Append("      FROM [" + nomeDaBaseOrigem + "].[dbo].[Sigla][sigla] WITH(NOLOCK)");
            builder.Append("      INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] WITH(NOLOCK) ON [bem].[SIGLA] = [sigla].[SIGLA] ) bem ");
            builder.Append("  WHERE [bem].[SIGLA] NOT IN(SELECT [sigla].[NAME]  FROM [" + nomeDaBaseDestino + "].[dbo].[Initial] [sigla] WITH(NOLOCK)) ");


            return builder.ToString();
        }
        private String createSQLDosTerceirosNaoImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();
            builder.Append(" SELECT DISTINCT 'Verificar código da UGA/CGC_CPF' as 'MENSAGEM', [ter].[COD_TERC] ");
            builder.Append(",[ter].[NOME]");
            builder.Append(",[ter].[CGC_CPF]");
            builder.Append(",[ter].[ENDERECO]");
            builder.Append(",[ter].[END_MUN]");
            builder.Append(",[ter].[END_EST]");
            builder.Append(",[ter].[END_DDD]");
            builder.Append(",[ter].[END_FONE]");
            builder.Append(",[ter].[END_RAMAL]");
            builder.Append(",[ter].[END_FAX]");
            builder.Append(",[ter].[END_TELEX]");
            builder.Append(",[ter].[END_CEP]");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[Terceiro] [ter] ");
            builder.Append("  INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [bem].[COD_TERC] = [ter].[COD_TERC] ");
            builder.Append("  INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");
            builder.Append(" WHERE  [ter].[CGC_CPF] NOT IN(SELECT [ous].[CPFCNPJ] FROM [" + nomeDaBaseDestino + "].[dbo].[OutSourced] [ous] )");

            return builder.ToString();
        }
        private String createSQLDasContasAuxiliaresNaoImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT 'Verificar código da Conta Auxliar' as 'MENSAGEM',  CONTA_AUX");
            builder.Append(",NOME");
            builder.Append(",NOME_ANTIGO");
            builder.Append(",CCONTABIL_ANTIGO");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[ContaAux]");
            builder.Append(" WHERE CONTA_AUX NOT IN(SELECT aux.Code FROM [" + nomeDaBaseDestino + "].[dbo].[AuxiliaryAccount] aux )");

            return builder.ToString();
        }

        private String createSQLDosDadosDeResponsavelNaoImportados(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {

            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT 'Verificar código da UGA',  [COD_RESP]");
            builder.Append(",[NOME_RESP] 'Nome'");
            builder.Append(",[SIGLA_UA] 'UA'");
            builder.Append(",[CARGO]");
            builder.Append(",[REFERENCIA]");
            builder.Append(",[NIVEL]");
            builder.Append(",[GRAU]");
            builder.Append(",[FLAG_INATIVO]");
            builder.Append(",[CODIGO_ENDERECO]");
            builder.Append(" FROM[" + nomeDaBaseOrigem + "].[dbo].[Responsavel] re");
            builder.Append(" WHERE [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO](re.SIGLA_UA) NOT IN(SELECT adm.Code FROM [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] adm)");

            return builder.ToString();
        }

        private string createSQLDosDadosDeFornecedorNaoImportados(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {

            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT[fo].[Codigo] ");
            builder.Append(" ,[fo].[Nome] ");
            builder.Append(" ,[fo].[Endereco] ");
            builder.Append(",[fo].[CEP] ");
            builder.Append(" ,[fo].[Cidade] ");
            builder.Append(" ,[fo].[Estado] ");
            builder.Append(" ,[fo].[Telefone] ");
            builder.Append(" ,[fo].[Fax] ");
            builder.Append(" ,[fo].[CGC] ");
            builder.Append(" ,[fo].[Data_de_Cadastramento] ");
            builder.Append(" ,[fo].[Data_da_Ultima_Operacao] ");
            builder.Append(" ,[fo].[Inf_Complementares] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[Fornecedores] [fo] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [fo].[Codigo] = [bem].[Codigo_do_Fornecedor] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");
            builder.Append("   AND [fo].[CGC] NOT IN(SELECT [sup].[CPFCNPJ] FROM [" + nomeDaBaseDestino + "].[dbo].[Supplier] [sup]) ");

            return builder.ToString();
        }

        private string createSQLDosDadosDeItemDeMaterialNaoImportados(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {

            builder = new StringBuilder();
            builder.Append("  SELECT DISTINCT 'Verificar código da UGA e Item Material' as 'MENSAGEM', [bem].[Codigo_do_Material],[bem].[Codigo_do_Item],[bem].[Descricao],[bem].[Data_do_Cadastramento],[bem].[Indicador_Espec],[bem].[Indicador_de_Atividade],[bem].[UltSubitem],[bem].[Conta_Aux],[bem].[TipoMaterial] ");
            builder.Append("  FROM (SELECT DISTINCT [bem].[SIGLA_UA], [ite].[Codigo_do_Material], [ite].[Codigo_do_Item], [ite].[Descricao], [ite].[Data_do_Cadastramento], [ite].[Indicador_Espec], [ite].[Indicador_de_Atividade], [ite].[UltSubitem], [ite].[Conta_Aux], [ite].[TipoMaterial] ");
            builder.Append("          FROM [" + nomeDaBaseOrigem + "].[dbo].[ItemMaterial][ite] ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial][bem] ON[ite].[Codigo_do_Item] = [bem].[COD_ITEMMAT] ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ) bem  ");
            builder.Append("  WHERE [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO] ([bem].[Codigo_do_Item]) NOT IN(SELECT [mat].[Code] FROM [" + nomeDaBaseDestino + "].[dbo].[MaterialItem] [mat]) ");

            return builder.ToString();
        }

        //Comentado por falta de uso
        //private string createSQLDosDadosDasUAsSeremCorrigidas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        //{

        //    builder = new StringBuilder();

        //    builder.Append("  SELECT bem.* FROM( ");
        //    builder.Append("  SELECT DISTINCT '' AS 'SIGLA UA SER CORRIGIDO', [ua].[SIGLA_UA] ");
        //    builder.Append("         ,[ua].[COD_UNID] ");
        //    builder.Append("         ,[ua].[NOME] ");
        //    builder.Append("         ,[ua].[COD_RESP] ");
        //    builder.Append("         ,[ua].[COD_ORG] ");
        //    builder.Append("         ,[ua].[COD_UGE] ");
        //    builder.Append("         ,[ua].[ENDERECO] ");
        //    builder.Append("         ,[ua].[END_COMPL] ");
        //    builder.Append("         ,[ua].[END_BAIRRO] ");
        //    builder.Append("         ,[ua].[END_MUN] ");
        //    builder.Append("         ,[ua].[END_EST] ");
        //    builder.Append("         ,[ua].[END_DDD] ");
        //    builder.Append("         ,[ua].[END_FONE] ");
        //    builder.Append("         ,[ua].[END_RAMAL] ");
        //    builder.Append("         ,[ua].[END_FAX] ");
        //    builder.Append("         ,[ua].[END_TELEX] ");
        //    builder.Append("         ,[ua].[END_CEP] ");
        //    builder.Append("         ,[ua].[AREA] ");
        //    builder.Append("         ,[ua].[NFUNC] ");
        //    builder.Append("         ,[ua].[FLAG_INATIVO] ");
        //    builder.Append("         ,[ua].[NOME_REDUZ] ");
        //    builder.Append("         ,[ua].[SIGLA_UA_ANTIGA] ");
        //    builder.Append("     FROM [" + nomeDaBaseOrigem + "].[dbo].[UA][ua] ");
        //    builder.Append("     INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial][bem] ON[ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");
        //    builder.Append("   ) bem ");
        //    builder.Append("   WHERE [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO](bem.SIGLA_UA) NOT IN(SELECT Code FROM [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit]) ");

        //    return builder.ToString();
        //}
        private String createSQLDosBensPatrimoniaisImportados(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append("SELECT DISTINCT [bem].[SIGLA]");
            builder.Append(",[bem].[CHAPA]");
            builder.Append(",[bem].[DESD]");
            builder.Append(",[bem].[SIGLA_UA]");
            builder.Append(",[bem].[COD_RESP]");
            builder.Append(",[bem].[COD_ORGC]");
            builder.Append(",[bem].[COD_UGEC]");
            builder.Append(",[bem].[COD_ITEMMAT]");
            builder.Append(",[bem].[CONTA_AUX]");
            builder.Append(",[bem].[DT_INCL]");
            builder.Append(",[bem].[DT_AQUI]");
            builder.Append(",[bem].[VL_AQUI]");
            builder.Append(",[bem].[VL_ATU]");
            builder.Append(",[bem].[NP_COMPRA]");
            builder.Append(",[bem].[NGPB]");
            builder.Append(",[bem].[NFISCAL]");
            builder.Append(",[bem].[NSERIE]");
            builder.Append(",[bem].[NFABRIC]");
            builder.Append(",[bem].[DT_GARAN]");
            builder.Append(",[bem].[NCHASSI]");
            builder.Append(",[bem].[MARCA]");
            builder.Append(",[bem].[MODELO]");
            builder.Append(",[bem].[HISTOR1]");
            builder.Append(",[bem].[HISTOR2]");
            builder.Append(",[bem].[HISTOR3]");
            builder.Append(",[bem].[HISTOR4]");
            builder.Append(",[bem].[HISTOR5]");
            builder.Append(",[bem].[EST_CONSER]");
            builder.Append(",[bem].[TIP_MOV]");
            builder.Append(",[bem].[SITUACAO]");
            builder.Append(",[bem].[CLASSIFICACAO]");
            builder.Append(",[bem].[DT_BOLSA]");
            builder.Append(",[bem].[COD_TERC]");
            builder.Append(",[bem].[DT_TERMO]");
            builder.Append(",[bem].[COD_TPINC]");
            builder.Append(",[bem].[SIGLA_ANTIGA]");
            builder.Append(",[bem].[CHAPA_ANTIGA]");
            builder.Append(",[bem].[DESD_ANTIGO]");
            builder.Append(",[bem].[Codigo_do_Fornecedor]");
            builder.Append(",[bem].[COMPL_DESCR]");
            builder.Append(",[bem].[DT_INVENT]");
            builder.Append(",[bem].[NEMPENHO]");
            builder.Append(",[bem].[DT_ATRIB_VALOR]");
            builder.Append(",GETDATE() 'DT_RECEB_TERMO' ");
            builder.Append(",[bem].[PLACA]");
            builder.Append(",(SELECT TOP 1 [Codigo_da_ND] FROM [" + nomeDaBaseOrigem + "].[dbo].[SubItemMaterial] ");
            builder.Append("     WHERE [Codigo_do_Item] = [bem].[COD_ITEMMAT] ");
            builder.Append(") AS 'Natureza_Despesa'  ");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ");
            builder.Append(" WHERE [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO]([bem].[SIGLA_UA]) IN(SELECT adm.Code  FROM [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] adm ");
            builder.Append("                                                                                 INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[AssetMovements] [assetM] ON [assetM].[AdministrativeUnitId] = adm.Id ");
            builder.Append("                                                                                 INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[Asset] [asset] ON [asset].[Id] = [assetM].[AssetId] WHERE [asset].[NumberIdentification] = [bem].[CHAPA] )");

            return builder.ToString();
        }

        private String createSQLDosTiposDeIncorporacoesImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT bem.MENSAGEM, bem.COD_TPINC ,bem.COD_TRANS,bem.DESCRICAO FROM( ");
            builder.Append("        SELECT DISTINCT 'Verificar código da UGA' as 'MENSAGEM', tp.COD_TPINC ,tp.COD_TRANS,tp.DESCRICAO,bem.COD_ORGC,[uo].[COD_UGO] ");
            builder.Append("          FROM [" + nomeDaBaseOrigem + "].[dbo].[TipoIncorp] tp ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON[bem].[COD_TPINC] = tp.COD_TPINC ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UGO] [uo] ON[uo].[COD_ORG] = bem.COD_ORGC) bem ");
            builder.Append(" INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[BudgetUnit] bug ON bug.Code = bem.COD_UGO ");
            builder.Append(" INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[Institution] it ON it.Code = bem.COD_ORGC ");

            return builder.ToString();
        }
        private String createSQLDasSiglasImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT bem.SIGLA,bem.[DESCRICAO],bem.[IND_PROPRIO],bem.[COD_SIGLA],bem.[IND_COD_BARRA] FROM( ");
            builder.Append("   SELECT DISTINCT [sigla].[SIGLA],[sigla].[DESCRICAO],[sigla].[IND_PROPRIO],[sigla].[COD_SIGLA],[sigla].[IND_COD_BARRA],[bem].COD_ITEMMAT 'Codigo_Item',[bem].[SIGLA_UA] ");
            builder.Append("      FROM [" + nomeDaBaseOrigem + "].[dbo].[Sigla][sigla] WITH(NOLOCK) ");
            builder.Append("      INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial][bem] WITH(NOLOCK) ON [bem].[SIGLA] = [sigla].[SIGLA] ) bem ");
            builder.Append("  WHERE [bem].[SIGLA]  IN(SELECT [sigla].[NAME]  FROM [" + nomeDaBaseDestino + "].[dbo].[Initial] [sigla] WITH(NOLOCK) ) ");

            return builder.ToString();
        }
        private String createSQLDosTerceirosImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT [ter].[COD_TERC]");
            builder.Append(",[ter].[NOME]");
            builder.Append(",[ter].[CGC_CPF]");
            builder.Append(",[ter].[ENDERECO]");
            builder.Append(",[ter].[END_MUN]");
            builder.Append(",[ter].[END_EST]");
            builder.Append(",[ter].[END_DDD]");
            builder.Append(",[ter].[END_FONE]");
            builder.Append(",[ter].[END_RAMAL]");
            builder.Append(",[ter].[END_FAX]");
            builder.Append(",[ter].[END_TELEX]");
            builder.Append(",[ter].[END_CEP]");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[Terceiro] [ter] ");
            builder.Append("  INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [bem].[COD_TERC] = [ter].[COD_TERC] ");
            builder.Append(" WHERE  [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO]([bem].[SIGLA_UA]) IN(SELECT adm.Code  FROM [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] adm )");

            return builder.ToString();
        }
        private String createSQLDasContasAuxiliaresImportadas(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT CONTA_AUX");
            builder.Append(",NOME");
            builder.Append(",NOME_ANTIGO");
            builder.Append(",CCONTABIL_ANTIGO");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[ContaAux]");
            builder.Append(" WHERE CONTA_AUX IN(SELECT aux.Code FROM [" + nomeDaBaseDestino + "].[dbo].[AuxiliaryAccount] aux )");

            return builder.ToString();
        }
        private string createSQLDosDadosDeResponsavelImportados(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {

            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT [COD_RESP]");
            builder.Append(",[NOME_RESP] 'Nome'");
            builder.Append(",[SIGLA_UA] 'UA'");
            builder.Append(",[CARGO]");
            builder.Append(",[REFERENCIA]");
            builder.Append(",[NIVEL]");
            builder.Append(",[GRAU]");
            builder.Append(",[FLAG_INATIVO]");
            builder.Append(",[CODIGO_ENDERECO]");
            builder.Append(" FROM[" + nomeDaBaseOrigem + "].[dbo].[Responsavel] re");
            builder.Append(" WHERE [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO](re.SIGLA_UA)  IN(SELECT adm.Code FROM [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] adm)");


            return builder.ToString();
        }


        private string createSQLDosDadosDeFornecedorImportados(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {

            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT[fo].[Codigo] ");
            builder.Append(" ,[fo].[Nome] ");
            builder.Append(" ,[fo].[Endereco] ");
            builder.Append(",[fo].[CEP] ");
            builder.Append(" ,[fo].[Cidade] ");
            builder.Append(" ,[fo].[Estado] ");
            builder.Append(" ,[fo].[Telefone] ");
            builder.Append(" ,[fo].[Fax] ");
            builder.Append(" ,[fo].[CGC] ");
            builder.Append(" ,[fo].[Data_de_Cadastramento] ");
            builder.Append(" ,[fo].[Data_da_Ultima_Operacao] ");
            builder.Append(" ,[fo].[Inf_Complementares] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[Fornecedores] [fo] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [fo].[Codigo] = [bem].[Codigo_do_Fornecedor] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");
            builder.Append("   AND [fo].[CGC] IN(SELECT [sup].[CPFCNPJ] FROM [" + nomeDaBaseDestino + "].[dbo].[Supplier] [sup]) ");


            return builder.ToString();
        }

        private string createSQLDosDadosItemDeMaterialImportados(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {

            builder = new StringBuilder();

            builder.Append("  SELECT DISTINCT 'Verificar código da UGA e Item Material' as 'MENSAGEM', [bem].[Codigo_do_Material],[bem].[Codigo_do_Item],[bem].[Descricao],[bem].[Data_do_Cadastramento],[bem].[Indicador_Espec],[bem].[Indicador_de_Atividade],[bem].[UltSubitem],[bem].[Conta_Aux],[bem].[TipoMaterial] ");
            builder.Append("  FROM (SELECT DISTINCT [bem].[SIGLA_UA], [ite].[Codigo_do_Material], [ite].[Codigo_do_Item], [ite].[Descricao], [ite].[Data_do_Cadastramento], [ite].[Indicador_Espec], [ite].[Indicador_de_Atividade], [ite].[UltSubitem], [ite].[Conta_Aux], [ite].[TipoMaterial] ");
            builder.Append("          FROM [" + nomeDaBaseOrigem + "].[dbo].[ItemMaterial][ite] ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial][bem] ON [ite].[Codigo_do_Item] = [bem].[COD_ITEMMAT] ");
            builder.Append("          INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA]  ) bem  ");
            builder.Append("  WHERE [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO] ([bem].[Codigo_do_Item]) IN(SELECT [mat].[Code] FROM [" + nomeDaBaseDestino + "].[dbo].[MaterialItem] [mat]) ");

            return builder.ToString();

        }

        private string validarLeiautePlanilhaExcel(DataTable planilha)
        {
            builder = new StringBuilder();

            if (!planilha.Columns.Contains("Sigla"))
                builder.Append("<strong>Sigla</strong></br>");

            if (!planilha.Columns.Contains("Descrição Sigla"))
                builder.Append("<strong>Descrição Sigla</strong></br>");

            if (!planilha.Columns.Contains("Código Item de Material"))
                builder.Append("<strong>Código Item de Material</strong></br>");

            if (!planilha.Columns.Contains("Descrição do Item de Material"))
                builder.Append("<strong>Descrição do Item de Material</strong></br>");

            if (!planilha.Columns.Contains("Conta Auxiliar"))
                builder.Append("<strong>Conta Auxiliar</strong></br>");

            if (!planilha.Columns.Contains("Contabil Auxiliar"))
                builder.Append("<strong>Contabil Auxiliar</strong></br>");

            if (!planilha.Columns.Contains("Nome da Conta Auxiliar"))
                builder.Append("<strong>Nome da Conta Auxiliar</strong></br>");

            if (!planilha.Columns.Contains("Código da Divisão"))
                builder.Append("<strong>Código da Divisão</strong></br>");

            if (!planilha.Columns.Contains("Nome da Divisão"))
                builder.Append("<strong>Nome da Divisão</strong></br>");

            if (!planilha.Columns.Contains("CPF do Responsável"))
                builder.Append("<strong>CPF do Responsável</strong></br>");

            if (!planilha.Columns.Contains("Nome do Responsável"))
                builder.Append("<strong>Nome do Responsável</strong></br>");

            if (!planilha.Columns.Contains("Cargo do Responsável"))
                builder.Append("<strong>Cargo do Responsável</strong></br>");

            if (!planilha.Columns.Contains("Código da UO"))
                builder.Append("<strong>Código da UO</strong></br>");

            if (!planilha.Columns.Contains("Nome da UO"))
                builder.Append("<strong>Nome da UO</strong></br>");

            if (!planilha.Columns.Contains("Código da UGE"))
                builder.Append("<strong>Código da UGE</strong></br>");

            if (!planilha.Columns.Contains("Nome da UGE"))
                builder.Append("<strong>Nome da UGE</strong></br>");

            if (!planilha.Columns.Contains("Código da UA"))
                builder.Append("<strong>Código da UA</strong></br>");

            if (!planilha.Columns.Contains("Nome da UA"))
                builder.Append("<strong>Nome da UA</strong></br>");

            if (!planilha.Columns.Contains("Código do Orgão"))
                builder.Append("<strong>Código do Orgão</strong></br>");

            if (!planilha.Columns.Contains("Chapa"))
                builder.Append("<strong>Chapa</strong></br>");

            if (!planilha.Columns.Contains("Valor de Aquisição"))
                builder.Append("<strong>Valor de Aquisição</strong></br>");

            if (!planilha.Columns.Contains("Data de Inclusão"))
                builder.Append("<strong>Data de Inclusão</strong></br>");

            if (!planilha.Columns.Contains("Data de Aquisição"))
                builder.Append("<strong>Data de Aquisição</strong></br>");

            if (!planilha.Columns.Contains("Número de Serie"))
                builder.Append("<strong>Número de Serie</strong></br>");

            if (!planilha.Columns.Contains("Data de Fabricação"))
                builder.Append("<strong>Data de Fabricação</strong></br>");

            if (!planilha.Columns.Contains("Data de Garantia"))
                builder.Append("<strong>Data de Garantia</strong></br>");

            if (!planilha.Columns.Contains("Número do Chassi"))
                builder.Append("<strong>Número do Chassi</strong></br>");

            if (!planilha.Columns.Contains("Marca"))
                builder.Append("<strong>Marca</strong></br>");

            if (!planilha.Columns.Contains("Modelo"))
                builder.Append("<strong>Modelo</strong></br>");

            if (!planilha.Columns.Contains("PLACA"))
                builder.Append("<strong>PLACA</strong></br>");

            if (!planilha.Columns.Contains("Sigla Antiga"))
                builder.Append("<strong>Sigla Antiga</strong></br>");

            if (!planilha.Columns.Contains("Chapa Antiga"))
                builder.Append("<strong>Chapa Antiga</strong></br>");

            if (!planilha.Columns.Contains("Estado de Conservação"))
                builder.Append("<strong>Estado de Conservação</strong></br>");

            if (!planilha.Columns.Contains("Número do Empenho"))
                builder.Append("<strong>Número do Empenho</strong></br>");

            if (!planilha.Columns.Contains("Observações"))
                builder.Append("<strong>Observações</strong></br>");

            if (!planilha.Columns.Contains("Descrição Adicional"))
                builder.Append("<strong>Descrição Adicional</strong></br>");

            if (!planilha.Columns.Contains("Nota Fiscal"))
                builder.Append("<strong>Nota Fiscal</strong></br>");

            if (!planilha.Columns.Contains("CNPJ do Fornecedor"))
                builder.Append("<strong>CNPJ do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("Nome do Fornecedor"))
                builder.Append("<strong>Nome do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("Telefone do Fornecedor"))
                builder.Append("<strong>Telefone do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("Cep do Fornecedor"))
                builder.Append("<strong>Cep do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("Logradouro do Fornecedor"))
                builder.Append("<strong>Logradouro do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("Cidade do Fornecedor"))
                builder.Append("<strong>Cidade do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("Estado do Fornecedor"))
                builder.Append("<strong>Estado do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("Complemento do Fornecedor"))
                builder.Append("<strong>Complemento do Fornecedor</strong></br>");

            if (!planilha.Columns.Contains("CPF/CNPJ do Terceiro"))
                builder.Append("<strong>CPF/CNPJ do Terceiro</strong></br>");

            if (!planilha.Columns.Contains("Nome do Terceiro"))
                builder.Append("<strong>Nome do Terceiro</strong></br>");

            if (!planilha.Columns.Contains("Telefone do Terceiro"))
                builder.Append("<strong>Telefone do Terceiro</strong></br>");

            if (!planilha.Columns.Contains("Cep do Terceiro"))
                builder.Append("<strong>Cep do Terceiro</strong></br>");

            if (!planilha.Columns.Contains("Logradouro do Terceiro"))
                builder.Append("<strong>Logradouro do Terceiro</strong></br>");

            if (!planilha.Columns.Contains("Cidade do Terceiro"))
                builder.Append("<strong>Cidade do Terceiro</strong></br>");

            if (!planilha.Columns.Contains("Estado do Terceiro"))
                builder.Append("<strong>Estado do Terceiro</strong></br>");

            if (!planilha.Columns.Contains("Acervo"))
                builder.Append("<strong>Acervo</strong></br>");

            if (!planilha.Columns.Contains("Terceiro"))
                builder.Append("<strong>Terceiro</strong></br>");


            return builder.ToString();


        }

        #region "SQL para importar os dados do legado"
        public IDictionary<string, IList<RetornoImportacao>> createExportacao(string nomeDaBaseDestino, string caminhoDoExcel)
        {
            IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();
            //string filePath = @"C:\Projetos\Prodesp - SCPweb\Fontes\Desenvolvimento\main\SAM.Web\Legado\Arquivos\MigracaoSAMPatrimonio_SEFAZ\MigracaoSAMPatrimonio_SEFAZ0_ExcelInventario.xlsx";
            FileStream stream = File.Open(caminhoDoExcel, FileMode.Open, FileAccess.Read);

            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            //...
            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //...
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables

            //armazena as configuracoes para posteriormente restaurar
            CultureInfo info = System.Threading.Thread.CurrentThread.CurrentCulture;

            //Atribui a culture info

            DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }

            });

            try
            {
                result.Locale = info;



                this.tableExcel = result.Tables[0];
                var validacao = validarLeiautePlanilhaExcel(this.tableExcel);

                if (validacao.Length > 0)
                {
                    mensagensDeRetorno = new List<RetornoImportacao>();
                    mensagensDeRetorno.Add(new RetornoImportacao { caminhoDoArquivoDownload = string.Empty, mensagemImportacao = "<div class=\"col-md-12\"><h4 class=\"text-center\">Leiaute da planilha ínvalido, por favor verificar a planilha</h4></div><div class=\"col-md-12 text-center\"><p>A Planilha não contêm as informações abaixo.</p><span class = \"control-label text-center\">" + validacao + "</span></div>", quantidadeDeRegistros = 0 });

                    resultado.Add("Excel Inventario", mensagensDeRetorno);

                    return resultado;
                }

                DataColumnCollection columns = this.tableExcel.Columns;


                if (!columns.Contains("Ua Validada"))
                {
                    this.tableExcel.Columns.Add("Ua Validada", typeof(bool));
                    this.tableExcel.Columns.Add("Responsavel Importado", typeof(String));
                    this.tableExcel.Columns.Add("Responsavel Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("Divisao Importado", typeof(String));
                    this.tableExcel.Columns.Add("Divisao Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("Sigla Importado", typeof(String));
                    this.tableExcel.Columns.Add("Sigla Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("Estado de Conservação Importado", typeof(String));
                    this.tableExcel.Columns.Add("Estado de Conservação Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("Terceiro Importado", typeof(String));
                    this.tableExcel.Columns.Add("Terceiro Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("Fornecedor Importado", typeof(String));
                    this.tableExcel.Columns.Add("Fornecedor Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("ContaAuxliar Importado", typeof(String));
                    this.tableExcel.Columns.Add("ContaAuxiliar Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("Descricao Importado", typeof(String));
                    this.tableExcel.Columns.Add("Descricao Mensagem", typeof(String));
                    this.tableExcel.Columns.Add("BemPatrimonial Importado", typeof(String));
                    this.tableExcel.Columns.Add("BemPatrimonial Mensagem", typeof(String));
                }
                DataRowCollection rows = getDataTable().Rows;
                int outCodUa = 0;
                int outCodUge = 0;
                bool UaValida;
                foreach (DataRow row in rows)
                {
                    if (int.TryParse(row["Código da UA"].ToString().Replace(" ", "").TrimEnd().TrimStart(), out outCodUa) == true && int.TryParse(row["Código da UGE"].ToString().Replace(" ", "").TrimEnd().TrimStart(), out outCodUge) == true)
                    {
                        UaValida = validarUA(int.Parse(row["Código da UA"].ToString().Replace(" ", "").TrimEnd().TrimStart()), int.Parse(row["Código da UGE"].ToString().Replace(" ", "").TrimEnd().TrimStart()), nomeDaBaseDestino);
                    }
                    else
                    {
                        UaValida = false;
                    }

                    atualizarUAValidaTableExcel(row, UaValida);
                }

                if (this.tableExcel.Rows[0]["Código do Orgão"].ToString().Contains("Regras:"))
                {
                    this.tableExcel.Rows.RemoveAt(0);
                }

                var codigoDoOrgao = int.Parse(this.tableExcel.Rows[0]["Código do Orgão"].ToString());

                IDataReader reader = this._legado.createComando("SELECT [id] FROM [" + nomeDaBaseDestino + "].[dbo].[Institution] WHERE CONVERT(int, Code) ='" + codigoDoOrgao + "'").createDataReader(CommandType.Text);
                IDataReader readerNameManagerReduced = this._legado.createComando("SELECT [NameManagerReduced] FROM [" + nomeDaBaseDestino + "].[dbo].[Institution] WHERE CONVERT(int, Code) ='" + codigoDoOrgao + "'").createDataReader(CommandType.Text);

                int institutionId = 0;
                string nameManagerReduced = string.Empty;

                if (reader.Read())
                    institutionId = int.Parse(reader[0].ToString());

                if (readerNameManagerReduced.Read())
                    nameManagerReduced = readerNameManagerReduced[0].ToString();

                this._legado.fecharConexao();
                this._legado = null;
                using (var context = new SAMContext())
                {
                    try
                    {
                        this._legado = Legado.createInstance();
                        this._legado.setInstitutionId(institutionId);

                        //context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                        ImportarResponsavel responsavel = new ImportarResponsavel(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        ImportarDivisao divisao = new ImportarDivisao(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        ImportarSigla sigla = new ImportarSigla(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        ImportarTerceiro terceiro = new ImportarTerceiro(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        ImportarFornecedor fornecedor = new ImportarFornecedor(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        //ImportarContaAuxiliar contaAuxiliar = new ImportarContaAuxiliar(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        ImportarDescricaoLegado descricaoLegado = new ImportarDescricaoLegado(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        //context.Database.CurrentTransaction.Commit();

                        ImportarBemPatrimonial bemPatrimonial = new ImportarBemPatrimonial(context, this._legado, this.tableExcel.Rows, nomeDaBaseDestino);
                        var quantidade = bemPatrimonial.getTotalImportados();

                        //context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                        SqlParameter parameter = new SqlParameter("@CodOrgao", codigoDoOrgao);
                        parameter.DbType = DbType.Int32;
                        parameter.Direction = ParameterDirection.Input;
                        context.Database.ExecuteSqlCommand("CONSOLIDACAO_IMPORTACAO @CodOrgao", parameter);
                        // context.Database.CurrentTransaction.Commit();

                        this._legado = null;

                        ExportarParaExcel excel = ExportarParaExcel.createInstance();
                        // DataSet dataSet = new DataSet();

                        //string sufixoNomeArquivo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                        string nomeArquivo = "RetornoImportacao_BensPatrimoniais_Orgao_" + nameManagerReduced;

                        //dataSet.Tables.Add(getDataTable());
                        string caminhoDoArquivo = getCaminhoExcelFisicoDoArquivoParaDownloadImportados(nomeArquivo);
                        excel.ExportDataSet(result, caminhoDoArquivo);
                        mensagensDeRetorno = new List<RetornoImportacao>();
                        mensagensDeRetorno.Add(new RetornoImportacao { caminhoDoArquivoDownload = getCaminhoExcelVirtualDoArquivoParaDownloadImportado(nomeArquivo), mensagemImportacao = "Planinha:" + nomeArquivo + sufixoPlanilhaImportada, quantidadeDeRegistros = quantidade });

                        resultado.Add("Excel Inventario", mensagensDeRetorno);
                        return resultado;
                    }
                    catch (Exception ex)
                    {
                        //context.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }

        public void createImportacaoPlanilha(string nomeDaBaseDestino, string caminhoDoExcel)
        {
            IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();
            FileStream stream = File.Open(caminhoDoExcel, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            CultureInfo info = System.Threading.Thread.CurrentThread.CurrentCulture;

            DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }

            });

            try
            {
                result.Locale = info;

                this.tableExcel = result.Tables[0];
                var validacao = validarLeiautePlanilhaExcel(this.tableExcel);

                if (validacao.Length > 0)
                {
                    mensagensDeRetorno = new List<RetornoImportacao>();
                    mensagensDeRetorno.Add(new RetornoImportacao { caminhoDoArquivoDownload = string.Empty, mensagemImportacao = "<div class=\"col-md-12\"><h4 class=\"text-center\">Leiaute da planilha ínvalido, por favor verificar a planilha</h4></div><div class=\"col-md-12 text-center\"><p>A Planilha não contêm as informações abaixo.</p><span class = \"control-label text-center\">" + validacao + "</span></div>", quantidadeDeRegistros = 0 });
                }

                DataRowCollection rows = getDataTable().Rows;
                DadosImportacaoBem importarExcel = null;
                DateTime dataAtual = DateTime.Now;

                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    foreach (DataRow row in rows)
                    {
                        importarExcel = new DadosImportacaoBem();

                        importarExcel.Data_Importacao = dataAtual;

                        importarExcel.Sigla = row["Sigla"].ToString();
                        importarExcel.Sigla_Descricao = row["Descrição Sigla"].ToString();
                        importarExcel.Chapa = row["Chapa"].ToString();
                        importarExcel.Codigo_do_Item = row["Código Item de Material"].ToString();
                        importarExcel.Descricao_do_Item = row["Descrição do Item de Material"].ToString();
                        importarExcel.CPF_Responsavel = row["CPF do Responsável"].ToString();
                        importarExcel.Nome_Responsavel = row["Nome do Responsável"].ToString();
                        importarExcel.Cargo_Responsavel = row["Cargo do Responsável"].ToString();
                        importarExcel.Data_Aquisicao = row["Data de Aquisição"].ToString();
                        importarExcel.Valor_Aquisicao = row["Valor de Aquisição"].ToString();

                        importarExcel.Data_Inclusao = row["Data de Inclusão"].ToString();
                        importarExcel.Estado_de_Conservacao = row["Estado de Conservação"].ToString();
                        importarExcel.Codigo_Orgao = row["Código do Orgão"].ToString();
                        importarExcel.Codigo_UO = row["Código da UO"].ToString();
                        importarExcel.Nome_UO = row["Nome da UO"].ToString();

                        importarExcel.Codigo_UGE = row["Código da UGE"].ToString();
                        importarExcel.Nome_UGE = row["Nome da UGE"].ToString();
                        importarExcel.Codigo_UA = row["Código da UA"].ToString();
                        importarExcel.Nome_UA = row["Nome da UA"].ToString();
                        importarExcel.Codigo_Divisao = row["Código da Divisão"].ToString();

                        importarExcel.Nome_Divisao = row["Nome da Divisão"].ToString();
                        importarExcel.Conta_Auxiliar = row["Conta Auxiliar"].ToString();
                        importarExcel.Contabil_Auxiliar = row["Contabil Auxiliar"].ToString();
                        importarExcel.Nome_Conta_Auxiliar = row["Nome da Conta Auxiliar"].ToString();
                        importarExcel.Numero_Serie = row["Número de Serie"].ToString();

                        importarExcel.Data_Fabricacao = row["Data de Fabricação"].ToString();
                        importarExcel.Data_Garantia = row["Data de Garantia"].ToString();
                        importarExcel.Numero_Chassi = row["Número do Chassi"].ToString();
                        importarExcel.Marca = row["Marca"].ToString();
                        importarExcel.Modelo = row["Modelo"].ToString();

                        importarExcel.Placa = row["PLACA"].ToString();
                        importarExcel.Sigla_Antiga = row["Sigla Antiga"].ToString();
                        importarExcel.Chapa_Antiga = row["Chapa Antiga"].ToString();
                        importarExcel.NEmpenho = row["Número do Empenho"].ToString();
                        importarExcel.Observacoes = row["Observações"].ToString();

                        importarExcel.Descricao_Adicional = row["Descrição Adicional"].ToString();
                        importarExcel.Nota_Fiscal = row["Nota Fiscal"].ToString();
                        importarExcel.CNPJ_Fornecedor = row["CNPJ do Fornecedor"].ToString();
                        importarExcel.Nome_Fornecedor = row["Nome do Fornecedor"].ToString();
                        importarExcel.Telefone_Fornecedor = row["Telefone do Fornecedor"].ToString();

                        importarExcel.CEP_Fornecedor = row["Cep do Fornecedor"].ToString();
                        importarExcel.Endereco_Fornecedor = row["Logradouro do Fornecedor"].ToString();
                        importarExcel.Cidade_Fornecedor = row["Cidade do Fornecedor"].ToString();
                        importarExcel.Estado_Fornecedor = row["Estado do Fornecedor"].ToString();
                        importarExcel.Inf_Complementares_Fornecedores = row["Complemento do Fornecedor"].ToString();

                        importarExcel.CPF_CNPJ_Terceiro = row["CPF/CNPJ do Terceiro"].ToString();
                        importarExcel.Nome_Terceiro = row["Nome do Terceiro"].ToString();
                        importarExcel.Telefone_terceiro = row["Telefone do Terceiro"].ToString();
                        importarExcel.CEP_Terceiro = row["Cep do Terceiro"].ToString();
                        importarExcel.Endereco_Terceiro = row["Logradouro do Terceiro"].ToString();

                        importarExcel.Cidade_Terceiro = row["Cidade do Terceiro"].ToString();
                        importarExcel.Estado_Terceiro = row["Estado do Terceiro"].ToString();
                        importarExcel.Acervo = row["Acervo"].ToString();
                        importarExcel.Terceiro = row["Terceiro"].ToString();

                        db.Entry(importarExcel).State = EntityState.Added;
                    }

                    db.SaveChanges();
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }

        public bool validarUA(int codigoDaUa, int codigoDaUge, string nomeDaBaseDestino)
        {
            IDataReader reader = _legado.createComando("SELECT COUNT(*) FROM [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] [au] INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[ManagerUnit] [mu] ON [mu].[Id] = [au].[ManagerUnitId] WHERE CONVERT(int, [au].[Code]) = " + codigoDaUa + " AND CONVERT(int, [mu].[Code]) = " + codigoDaUge).createDataReader(CommandType.Text);

            if (reader.Read())
                if (reader.GetInt32(0) < 1)
                    return false;

            return true;
        }
        private void atualizarUAValidaTableExcel(DataRow row, bool validada)
        {
            row["Ua Validada"] = validada;
        }
        public IDictionary<string, IList<RetornoImportacao>> createExcel(string nomeDaBaseOrigem)
        {
            IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();
            try
            {
                int pageSize = 10000;
                int quantidade = getInventarioQuantidadeBemPatrimonial(nomeDaBaseOrigem);
                if (quantidade < pageSize)
                    pageSize = quantidade;

                double a = Convert.ToDouble(quantidade) / Convert.ToDouble(pageSize);
                //int b = quantidade / pageSize;
                int b = 0;
                if (pageSize == 0)
                    b = 0;

                b = quantidade / pageSize;
                int paginas = a > b ? (b + 1) : b;

                mensagensDeRetorno = new List<RetornoImportacao>();

                for (int i = 0; i < paginas; i++)
                {
                    int currentIndex = i * pageSize;

                    ExportarParaExcel excel = ExportarParaExcel.createInstance();
                    DataSet dataSet = new DataSet();


                    var tabelas = getInventarioBemPatrimonial(nomeDaBaseOrigem, currentIndex, pageSize);
                    dataSet.Tables.AddRange(tabelas);
                    string caminhoDoArquivo = getCaminhoExcelFisicoDoArquivoParaDownloadImportados(nomeDaBaseOrigem + (i + 1).ToString());
                    excel.ExportDataSet(dataSet, caminhoDoArquivo);
                    mensagensDeRetorno.Add(new RetornoImportacao { caminhoDoArquivoDownload = getCaminhoExcelVirtualDoArquivoParaDownloadImportado(nomeDaBaseOrigem + (i + 1).ToString()), mensagemImportacao = "Planinha:" + (i + 1).ToString(), quantidadeDeRegistros = tabelas[0].Rows.Count });


                }

                resultado.Add("Excel Inventario", mensagensDeRetorno);
                return resultado;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string craeteSQLUnidade(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();
            builder.Append(" SELECT DISTINCT [uni].[COD_UNID] ");
            builder.Append(" ,[uni].[NOME] ");
            builder.Append(" ,[uni].[COD_UNID] ");
            builder.Append(" ,[uni].[SIGLA_UA] ");
            //builder.Append(" ,[uni].[END_CEP] ");
            //builder.Append(" ,[ua].[COD_RESP] ");
            //builder.Append(" ,[ua].[END_FONE] ");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[Unidade] [uni] ");
            //builder.Append(" INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[COD_UNID] = [uni].[COD_UNID] ");
            //builder.Append(" INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [bem].[SIGLA_UA] = [ua].[SIGLA_UA] ");


            return builder.ToString();
        }

        private void createUnidade(string nomeDaBaseOrigem)
        {
            IDataReader reader = _legado.createComando(craeteSQLUnidade(nomeDaBaseOrigem))
                                                                       .createDataReader(CommandType.Text);
            //var Enderecos = createEnderecosDaUnidade(nomeDaBaseOrigem);
            int contador = 1;

            while (reader.Read())
            {
                try
                {
                    int codigoUnidade;

                    if (reader["COD_UNID"].ToString().Equals("0"))
                        codigoUnidade = int.Parse(reader["SIGLA_UA"].ToString() + contador.ToString());
                    else
                        codigoUnidade = int.Parse(reader["COD_UNID"].ToString());

                    //  var cep = (reader["END_CEP"].ToString().Length > 8 ? reader["END_CEP"].ToString().Substring(0, 8) : reader["END_CEP"].ToString());
                    Section section = new Section();
                    int sigla = 0;
                    sigla = int.Parse(reader["SIGLA_UA"].ToString());
                    AdministrativeUnit administrativeUnit = contexto.AdministrativeUnits.Where(x => x.Code == sigla).FirstOrDefault();
                    var responsavel = contexto.Responsibles.Where(x => x.RelatedAdministrativeUnits.Code == sigla).FirstOrDefault();
                    var unidade = contexto.Sections.Where(x => x.Code == codigoUnidade && x.ResponsibleId == responsavel.Id && x.AdministrativeUnitId == administrativeUnit.Id).AsQueryable().FirstOrDefault();

                    //var adress = Enderecos.Where(x => x.PostalCode == cep).FirstOrDefault();

                    if (administrativeUnit != null && unidade == null)
                    {
                        //section.RelatedAddress = adress;
                        section.AdministrativeUnitId = administrativeUnit.Id;
                        section.Code = codigoUnidade;
                        //section.Description = reader["NOME"].ToString();
                        //section.Telephone = reader["END_FONE"].ToString().Replace(".", "").Replace("-", "").Replace(" ", "");

                        //if (adress != null)
                        //    section.AddressId = adress.Id;
                        //else
                        //{
                        //    Address endereco = new Address();
                        //    endereco.City = "Importação";
                        //    endereco.State = "Importação";
                        //    endereco.PostalCode = "00000000";

                        //    endereco.District = "Importação";
                        //    endereco.Number = "0";
                        //    endereco.Street = "Importação do legado";

                        //    Enderecos.Add(endereco);
                        //    contexto.SaveChanges();

                        //    section.AddressId = endereco.Id;
                        //}

                        section.AddressId = null;

                        if (responsavel != null)
                            section.ResponsibleId = responsavel.Id;
                        else
                            section.ResponsibleId = null;

                        section.Status = true;

                        section.RelatedResponsible = null;
                        section.RelatedAdministrativeUnit = null;


                        builder = new StringBuilder();
                        builder.Append(" INSERT INTO [dbo].[Section] ");
                        builder.Append(" ([AdministrativeUnitId] ");
                        builder.Append(" ,[Code] ");
                        builder.Append(" ,[Description] ");
                        builder.Append(" ,[Status] ");
                        builder.Append(" ,[ResponsibleId]) ");
                        builder.Append(" VALUES ");
                        builder.Append(" (" + section.AdministrativeUnitId.ToString());
                        builder.Append(" ," + section.Code.ToString());
                        builder.Append(" ,'" + section.Description + "'");
                        builder.Append(" ,1");
                        builder.Append(" ," + (section.ResponsibleId.HasValue ? section.ResponsibleId.Value.ToString() : "Null") + ")");

                        contexto.Database.ExecuteSqlCommand(builder.ToString());
                        contador++;

                    }
                    administrativeUnit = null;
                    responsavel = null;
                    unidade = null;
                    section = null;
                }
                catch (DbEntityValidationException ex)
                {
                    builderException = new StringBuilder();
                    foreach (var entidade in ex.EntityValidationErrors)
                    {

                        builderException.AppendFormat("Entidade do tipo \"{0}\" no estado \"{1}\" contem os seguintes erros:",
                            entidade.Entry.Entity.GetType().Name, entidade.Entry.State);
                        foreach (var exception in entidade.ValidationErrors)
                        {
                            builderException.AppendFormat(" Atributo(Propriedade): \"{0}\", Erro: \"{1}\"",
                                exception.PropertyName, exception.ErrorMessage);
                        }
                    }

                    // throw new Exception(builderException.ToString(), ex.InnerException);

                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            reader = null;

        }

        private string craeteSQLEnderecoDaUnidade(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();
            builder.Append("  SELECT [ENDERECO] ");
            builder.Append(" ,[END_COMPL] ");
            builder.Append(" ,[END_BAIRRO] ");
            builder.Append(" ,[END_MUN] ");
            builder.Append(" ,[END_EST] ");
            builder.Append(" ,[END_DDD] ");
            builder.Append(" ,[END_FONE] ");
            builder.Append(" ,[END_RAMAL] ");
            builder.Append(" ,[END_FAX] ");
            builder.Append(" ,[END_TELEX] ");
            builder.Append(" ,[END_CEP] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[UA] ");
            return builder.ToString();
        }



        private IList<Address> createEnderecosDaUnidade(string nomeDaBaseOrigem)
        {
            IDataReader reader = _legado.createComando(craeteSQLEnderecoDaUnidade(nomeDaBaseOrigem))
                                                                              .createDataReader(CommandType.Text);
            IList<Address> Enderecos = new List<Address>();

            while (reader.Read())
            {
                try
                {
                    Address endereco = new Address();
                    endereco.City = reader["END_MUN"].ToString();
                    endereco.State = reader["END_EST"].ToString();
                    endereco.PostalCode = reader["END_CEP"].ToString();

                    endereco.District = reader["END_MUN"].ToString();
                    endereco.Number = getNumerosNoTextoDoLegado(reader["ENDERECO"].ToString()).ToString();
                    endereco.Street = reader["ENDERECO"].ToString();

                    Enderecos.Add(endereco);
                    contexto.SaveChanges();

                    endereco = null;
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }

            return Enderecos;
        }

        private string craeteSQLItemDeMaterial(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();
            builder.Append(" SELECT DISTINCT [ite].[Codigo_do_Material] ");
            builder.Append(" ,[ite].[Codigo_do_Item] ");
            builder.Append(" ,[ite].[Descricao] ");
            builder.Append(" ,[ite].[Data_do_Cadastramento] ");
            builder.Append(" ,[ite].[Indicador_Espec] ");
            builder.Append(" ,[ite].[Indicador_de_Atividade] ");
            builder.Append(" ,[ite].[UltSubitem] ");
            builder.Append(" ,[ite].[Conta_Aux] ");
            builder.Append(" ,[ite].[TipoMaterial] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[ItemMaterial] [ite] ");
            builder.Append("  INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [ite].[Codigo_do_Item] = [bem].[COD_ITEMMAT] ");
            builder.Append("  INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");

            return builder.ToString();
        }

        private void createItemDeMaterial(string nomeDaBaseOrigem)
        {
            IDataReader reader = _legado.createComando(craeteSQLItemDeMaterial(nomeDaBaseOrigem))
                                                                       .createDataReader(CommandType.Text);

            while (reader.Read())
            {
                MaterialItem item = new MaterialItem();
                var codigoMaterial = int.Parse(reader["Codigo_do_Material"].ToString());
                var codigoDoItem = int.Parse(reader["Codigo_do_Item"].ToString());

                Material material = contexto.Materials.Where(x => x.Code == codigoMaterial).AsQueryable().FirstOrDefault();


                if (material != null)
                {
                    //var itemMaterial = material.MaterialItems.FirstOrDefault();

                    //if (itemMaterial !=null && itemMaterial.Code != codigoDoItem)
                    //{
                    item.MaterialId = material.Id;
                    item.Description = reader["Descricao"].ToString();
                    item.Code = codigoDoItem;

                    contexto.MaterialItems.Add(item);
                    contexto.SaveChanges();
                    //}
                }
                item = null;
            }
            reader = null;
        }

        private string craeteSQLEnderecoDoFornecedor(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();
            builder.Append(" SELECT [Endereco] ");
            builder.Append(" ,[CEP] ");
            builder.Append(" ,[Cidade] ");
            builder.Append(" ,[Estado] ");
            builder.Append(" ,[Telefone] ");
            builder.Append(" ,[Fax] ");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[Fornecedores] ");

            return builder.ToString();
        }



        private IList<Address> createEnderecosFornecedor(string nomeDaBaseOrigem)
        {
            IDataReader reader = _legado.createComando(craeteSQLEnderecoDoFornecedor(nomeDaBaseOrigem))
                                                                              .createDataReader(CommandType.Text);
            IList<Address> Enderecos = new List<Address>();

            while (reader.Read())
            {
                try
                {
                    Address endereco = new Address();
                    endereco.City = reader["CIDADE"].ToString();
                    endereco.State = reader["ESTADO"].ToString();
                    endereco.PostalCode = reader["CEP"].ToString();

                    endereco.District = reader["CIDADE"].ToString();
                    endereco.Number = getNumerosNoTextoDoLegado(reader["Endereco"].ToString()).ToString();
                    endereco.Street = reader["Endereco"].ToString();

                    Enderecos.Add(endereco);
                    endereco = null;
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }

            return Enderecos;
        }


        private string craeteSQLFornecedor(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT [fo].[Codigo] ");
            builder.Append(" ,[fo].[Nome] ");
            builder.Append(" ,[fo].[Endereco] ");
            builder.Append(" ,[fo].[CEP] ");
            builder.Append(" ,[fo].[Cidade] ");
            builder.Append(" ,[fo].[Estado] ");
            builder.Append(" ,[fo].[Telefone] ");
            builder.Append(" ,[fo].[Fax] ");
            builder.Append(" ,[fo].[CGC] ");
            builder.Append(" ,[fo].[Data_de_Cadastramento] ");
            builder.Append(" ,[fo].[Data_da_Ultima_Operacao] ");
            builder.Append(" ,[fo].[Inf_Complementares] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[Fornecedores] [fo]");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [fo].[CODIGO] = [bem].[Codigo_do_Fornecedor] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");


            return builder.ToString();
        }

        private void createFornecedor(string nomeDaBaseOrigem)
        {
            IDataReader reader = null;
            try
            {
                reader = _legado.createComando(craeteSQLFornecedor(nomeDaBaseOrigem))
                                                                            .createDataReader(CommandType.Text);
                var Enederecos = createEnderecosFornecedor(nomeDaBaseOrigem);
                try
                {
                    while (reader.Read())
                    {
                        Supplier fornecedor = new Supplier();
                        var cep = (reader["CEP"].ToString().Length > 8 ? reader["CEP"].ToString().Substring(0, 8) : reader["CEP"].ToString());


                        var andress = Enederecos.Where(x => x.PostalCode == cep).FirstOrDefault();

                        if (andress != null)
                        {
                            String cnpj = reader["CGC"].ToString();
                            //Limpa caracteres do CPF e CNPJ para ficar somente numeros
                            string cnpjValido = cnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);

                            var _fornecedor = contexto.Suppliers.Where(x => x.CPFCNPJ.Equals(cnpjValido)).AsQueryable().FirstOrDefault();

                            if (_fornecedor == null)
                            {
                                fornecedor.RelatedAddress = andress;
                                fornecedor.AdditionalData = DateTime.Now.ToString();
                                fornecedor.AddressId = andress.Id;
                                fornecedor.Name = reader["NOME"].ToString();
                                fornecedor.Status = true;
                                var telefone = getNumerosNoTextoDoLegado(reader["Telefone"].ToString());
                                fornecedor.Telephone = telefone.ToString();


                                fornecedor.CPFCNPJ = cnpjValido;

                                //Limpa caracteres do CEP para ficar somente os numeros
                                fornecedor.RelatedAddress.PostalCode = fornecedor.RelatedAddress.PostalCode.Replace("-", string.Empty);
                                if (fornecedor.CPFCNPJ.Length > 10 && fornecedor.CPFCNPJ.Length < 16 && fornecedor.Telephone.Length > 0 && fornecedor.RelatedAddress.PostalCode.Length == 8)
                                {
                                    contexto.Suppliers.Add(fornecedor);
                                    contexto.SaveChanges();
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }
            }

            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                ex.Message.ToString();
            }
        }
        //private String createSQLQuantidadeDoInventario(string nomeDaBaseOrigem)
        //{
        //    builder = new StringBuilder();
        //    /****** Script do comando SelectTopNRows de SSMS  ******/
        //    builder.Append(" SELECT COUNT(*) 'Quantidade' ");

        //    builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] WHERE [Situacao] <> '1'");


        //    return builder.ToString();
        //}

        private String createSQLInventario(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();
            /****** Script do comando SelectTopNRows de SSMS  ******/
            builder.Append(" SELECT DISTINCT [sig].[Sigla] ");
            builder.Append(",[sig].[Descricao] 'Descrição Sigla'");
            builder.Append(",[bem].[Chapa] ");
            builder.Append(",[ite].[Codigo_do_Item] 'Código Item de Material' ");
            builder.Append(",[ite].[Descricao] 'Descrição do Item de Material' ");
            builder.Append(",'Digite o CPF' 'CPF do Responsável' ");
            builder.Append(",[res].[NOME_RESP] 'Nome do Responsável' ");
            builder.Append(",[res].[CARGO] 'Cargo do Responsável' ");
            builder.Append(",[bem].[DT_AQUI] 'Data de Aquisição' ");
            builder.Append(",[bem].[VL_AQUI] 'Valor de Aquisição' ");
            builder.Append(",[bem].[DT_INCL] 'Data de Inclusão' ");
            builder.Append(",(CASE WHEN [EST_CONSER] IS NULL OR  [EST_CONSER] = 0 THEN 'Bom' ELSE CASE WHEN [EST_CONSER] = 1 THEN 'Regular' ELSE 'Mau' END END) 'Estado de Conservação' ");
            builder.Append(",[bem].[COD_ORGC] 'Código do Orgão' ");
            builder.Append(",[uo].[COD_UGO] 'Código da UO' ");
            builder.Append(",[uo].[NOME] 'Nome da UO' ");
            builder.Append(",(right(replicate('0',6) + CONVERT(VARCHAR(6),[bem].[COD_ORGC] + [ug].[COD_UGE]),6)) 'Código da UGE' ");
            builder.Append(",[ug].[Nome] 'Nome da UGE' ");
            builder.Append(",[ua].[SIGLA_UA] 'Código da UA' ");
            builder.Append(",[ua].[Nome] 'Nome da UA' ");
            builder.Append(",[uni].[COD_UNID] 'Código da Divisão' ");
            builder.Append(",[uni].[NOME] 'Nome da Divisão' ");
            builder.Append(",[aux].[CONTA_AUX] 'Conta Auxiliar' ");
            builder.Append(",[aux].[CCONTABIL] 'Contabil Auxiliar' ");
            builder.Append(",[aux].[NOME] 'Nome da Conta Auxiliar' ");
            builder.Append(",[bem].[NSERIE] 'Número de Serie' ");
            builder.Append(",[bem].[NFABRIC] 'Data de Fabricação' ");
            builder.Append(",[bem].[DT_GARAN] 'Data de Garantia' ");
            builder.Append(",[bem].[NCHASSI] 'Número do Chassi' ");
            builder.Append(",[bem].[MARCA] 'Marca' ");
            builder.Append(",[bem].[MODELO] 'Modelo' ");
            builder.Append(",[bem].[PLACA]  ");
            builder.Append(",[bem].[SIGLA_ANTIGA] 'Sigla Antiga' ");
            builder.Append(",[bem].[CHAPA_ANTIGA] 'Chapa Antiga' ");
            builder.Append(",[bem].[NEMPENHO] 'Número do Empenho' ");
            builder.Append(",[bem].[HISTOR5] 'Observações' ");
            builder.Append(",'' 'Descrição Adicional' ");
            builder.Append(",CASE WHEN isNumeric([bem].[NFISCAL]) = 1 THEN [bem].[NFISCAL] ELSE '' END as 'Nota Fiscal' ");
            builder.Append(",[for].[CGC] 'CNPJ do Fornecedor' ");
            builder.Append(",[for].[Nome] 'Nome do Fornecedor' ");
            builder.Append(",[for].[Telefone] 'Telefone do Fornecedor' ");
            builder.Append(",[for].[CEP] 'Cep do Fornecedor'");
            builder.Append(",[for].[Endereco] 'Logradouro do Fornecedor'");
            builder.Append(",[for].[Cidade] 'Cidade do Fornecedor'");
            builder.Append(",[for].[Estado] 'Estado do Fornecedor'");
            builder.Append(",[for].[Inf_Complementares] 'Complemento do Fornecedor'");
            builder.Append(",[ter].[CGC_CPF] 'CPF/CNPJ do Terceiro' ");
            builder.Append(",[ter].[NOME] 'Nome do Terceiro' ");
            builder.Append(",[ter].[END_FONE] 'Telefone do Terceiro'  ");
            builder.Append(",[ter].[END_CEP] 'Cep do Terceiro'");
            builder.Append(",[ter].[ENDERECO] 'Logradouro do Terceiro'");
            builder.Append(",[ter].[END_MUN] 'Cidade do Terceiro'");
            builder.Append(",[ter].[END_EST] 'Estado do Terceiro'");
            builder.Append(",'' 'Acervo'");
            builder.Append(",'' 'Terceiro'");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ");
            builder.Append("    INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[Sigla]  [sig] ON [sig].[Sigla] = [bem].[Sigla] ");
            builder.Append("    INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");
            builder.Append("    INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UGE] [ug] ON [ug].[COD_UGE] = [bem].[COD_UGEC] ");
            builder.Append("    INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UGO] [uo] ON [uo].[COD_ORG] = [bem].[COD_ORGC] ");
            builder.Append("    LEFT OUTER JOIN [" + nomeDaBaseOrigem + "].[dbo].[Responsavel] [res] ON ([res].[COD_RESP] = [bem].[COD_RESP] AND [res].SIGLA_UA = [ua].[SIGLA_UA])");
            builder.Append("    LEFT OUTER JOIN [" + nomeDaBaseOrigem + "].[dbo].[Unidade] [uni] ON [uni].[COD_UNID] =[ua].[COD_UNID] ");
            builder.Append("    LEFT OUTER JOIN [" + nomeDaBaseOrigem + "].[dbo].[ContaAux] [aux] ON [aux].[CONTA_AUX] = [bem].[CONTA_AUX] ");
            builder.Append("    INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[ItemMaterial] [ite]  ON [ite].[Codigo_do_Item] = [bem].[COD_ITEMMAT] ");
            builder.Append("    LEFT OUTER JOIN  [" + nomeDaBaseOrigem + "].[dbo].[Fornecedores] [for] ON [for].[Codigo] = [bem].[Codigo_do_Fornecedor] ");
            builder.Append("    LEFT OUTER JOIN [" + nomeDaBaseOrigem + "].[dbo].[Terceiro] [ter] ON [ter].[COD_TERC] = [bem].[COD_TERC] ");
            builder.Append(" WHERE [bem].[Situacao] <> '1' ");
            builder.Append(" AND [ug].[COD_ORG] = [bem].[COD_ORGC] ");
            builder.Append(" AND [ug].[COD_UGO] = [uo].[COD_UGO] ");
            builder.Append(" GROUP BY ");
            builder.Append(" [sig].[Sigla] ");
            builder.Append(",[sig].[Descricao] ");
            builder.Append(",[bem].[Chapa] ");
            builder.Append(",[ite].[Codigo_do_Item]  ");
            builder.Append(",[ite].[Codigo_do_Item]  ");
            builder.Append(",[ite].[Descricao]  ");
            builder.Append(",[aux].[CONTA_AUX]  ");
            builder.Append(",[aux].[CCONTABIL] ");
            builder.Append(",[bem].[NFISCAL] ");
            builder.Append(",[aux].[NOME]  ");
            builder.Append(",[uni].[COD_UNID]  ");
            builder.Append(",[uni].[NOME] ");
            builder.Append(",[res].[NOME_RESP]  ");
            builder.Append(",[res].[CARGO] ");
            builder.Append(",[uo].[COD_UGO]  ");
            builder.Append(",[uo].[NOME]  ");
            builder.Append(",[ug].[COD_UGE]  ");
            builder.Append(",[ug].[Nome]  ");
            builder.Append(",[ua].[SIGLA_UA]  ");
            builder.Append(",[ua].[Nome]  ");
            builder.Append(",[bem].[COD_ORGC] ");
            builder.Append(",[bem].[Chapa] ");
            builder.Append(",[bem].[VL_AQUI]  ");
            builder.Append(",[bem].[DT_INCL]  ");
            builder.Append(",[bem].[DT_AQUI] ");
            builder.Append(",[bem].[NSERIE]  ");
            builder.Append(",[bem].[NFABRIC] ");
            builder.Append(",[bem].[DT_GARAN] ");
            builder.Append(",[bem].[NCHASSI] ");
            builder.Append(",[bem].[MARCA] ");
            builder.Append(",[bem].[MODELO]  ");
            builder.Append(",[bem].[PLACA]  ");
            builder.Append(",[bem].[SIGLA_ANTIGA]  ");
            builder.Append(",[bem].[CHAPA_ANTIGA] ");
            builder.Append(",[bem].[EST_CONSER]  ");
            builder.Append(",[bem].[NEMPENHO] ");
            builder.Append(",[bem].[HISTOR5]");
            builder.Append(",[for].[CGC]");
            builder.Append(",[for].[Nome]");
            builder.Append(",[for].[Telefone]");
            builder.Append(",[for].[CEP]");
            builder.Append(",[for].[Cidade] ");
            builder.Append(",[for].[Endereco]");
            builder.Append(",[for].[Estado]");
            builder.Append(",[for].[Inf_Complementares]");
            builder.Append(",[ter].[CGC_CPF]");
            builder.Append(",[ter].[NOME]");
            builder.Append(",[ter].[END_FONE]");
            builder.Append(",[ter].[END_CEP] ");
            builder.Append(",[ter].[ENDERECO] ");
            builder.Append(",[ter].[END_MUN] ");
            builder.Append(",[ter].[END_EST] ");
            builder.Append(" ORDER BY [bem].[Chapa] ASC ");
            return builder.ToString();
        }

        //Comentado por falta de uso
        //private string createSQLBemPatrimonial(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        //{
        //    builder = new StringBuilder();

        //    builder.Append("SELECT bem.* FROM(SELECT DISTINCT  TOP 100 [bem].[SIGLA]");
        //    builder.Append(",[bem].[CHAPA]");
        //    builder.Append(",[bem].[DESD]");
        //    builder.Append(",[bem].[SIGLA_UA]");
        //    builder.Append(",[bem].[COD_RESP]");
        //    builder.Append(",[bem].[COD_ORGC]");
        //    builder.Append(",[bem].[COD_UGEC]");
        //    builder.Append(",[bem].[COD_ITEMMAT]");
        //    builder.Append(",[bem].[CONTA_AUX]");
        //    builder.Append(",[bem].[DT_INCL]");
        //    builder.Append(",[bem].[DT_AQUI]");
        //    builder.Append(",[bem].[VL_AQUI]");
        //    builder.Append(",[bem].[VL_ATU]");
        //    builder.Append(",[bem].[NP_COMPRA]");
        //    builder.Append(",[bem].[NGPB]");
        //    builder.Append(",[bem].[NFISCAL]");
        //    builder.Append(",[bem].[NSERIE]");
        //    builder.Append(",[bem].[NFABRIC]");
        //    builder.Append(",[bem].[DT_GARAN]");
        //    builder.Append(",[bem].[NCHASSI]");
        //    builder.Append(",[bem].[MARCA]");
        //    builder.Append(",[bem].[MODELO]");
        //    builder.Append(",[bem].[HISTOR1]");
        //    builder.Append(",[bem].[HISTOR2]");
        //    builder.Append(",[bem].[HISTOR3]");
        //    builder.Append(",[bem].[HISTOR4]");
        //    builder.Append(",[bem].[HISTOR5]");
        //    builder.Append(",[bem].[EST_CONSER]");
        //    builder.Append(",[bem].[TIP_MOV]");
        //    builder.Append(",[bem].[SITUACAO]");
        //    builder.Append(",[bem].[CLASSIFICACAO]");
        //    builder.Append(",[bem].[DT_BOLSA]");
        //    builder.Append(",[bem].[COD_TERC]");
        //    builder.Append(",[bem].[DT_TERMO]");
        //    builder.Append(",[bem].[COD_TPINC]");
        //    builder.Append(",[bem].[SIGLA_ANTIGA]");
        //    builder.Append(",[bem].[CHAPA_ANTIGA]");
        //    builder.Append(",[bem].[DESD_ANTIGO]");
        //    builder.Append(",[bem].[Codigo_do_Fornecedor]");
        //    builder.Append(",[bem].[COMPL_DESCR]");
        //    builder.Append(",[bem].[DT_INVENT]");
        //    builder.Append(",[bem].[NEMPENHO]");
        //    builder.Append(",[bem].[DT_ATRIB_VALOR]");
        //    builder.Append(",[bem].[DT_RECEB_TERMO] ");
        //    builder.Append(",[bem].[PLACA]");
        //    builder.Append(",(SELECT TOP 1 [fo].[CGC] ");
        //    builder.Append("       FROM [" + nomeDaBaseOrigem + "].[dbo].[Fornecedores][fo] WITH(NOLOCK) ");
        //    builder.Append("        WHERE [fo].[Codigo] = [bem].[Codigo_do_Fornecedor] ) 'CGC' ");
        //    builder.Append(",(SELECT TOP 1 ISNULL([mat].[Descricao], '') ");
        //    builder.Append("       FROM [" + nomeDaBaseOrigem + "].[dbo].[ItemMaterial][mat] WITH(NOLOCK) ");
        //    builder.Append("       WHERE [mat].[Codigo_do_Item] = [bem].[COD_ITEMMAT] ");
        //    builder.Append(") AS 'Descricao' ");
        //    builder.Append(", (SELECT TOP 1 [Codigo_da_ND] ");
        //    builder.Append("       FROM [" + nomeDaBaseOrigem + "].[dbo].[SubItemMaterial][item] WITH(NOLOCK) ");
        //    builder.Append("       WHERE [bem].[COD_ITEMMAT] = [item].[Codigo_do_Item] ) AS 'Natureza_Despesa' ");
        //    builder.Append(", (SELECT TOP 1 [uo].[COD_UGO] ");
        //    builder.Append("       FROM [" + nomeDaBaseOrigem + "].[dbo].[UGO][uo] WITH(NOLOCK) ");
        //    builder.Append("       WHERE [bem].[COD_ORGC] = [uo].[COD_ORG] ) 'COD_UGO' ");
        //    builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ");
        //    //builder.Append(" LEFT OUTER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA]) bem ");
        //    //Orgão especifico
        //    //builder.Append(" WHERE  [bem].[CHAPA] NOT IN(SELECT [ast].[NumberIdentification] FROM [" + nomeDaBaseDestino + "].[dbo].[Asset] [ast] WITH(NOLOCK) ");
        //    //builder.Append("                                             INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[AssetMovements] [asm] WITH(NOLOCK) ON [asm].[AssetId] = [ast].[Id] ");
        //    //builder.Append("                                             INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[BudgetUnit] [bug] WITH(NOLOCK) ON [bug].[Id] = [asm].[BudgetUnitId] ");
        //    //builder.Append("                                             INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[ManagerUnit] [man] WITH(NOLOCK) ON [bug].[Id] = [man].[BudgetUnitId] ");
        //    //builder.Append("                                             INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] [adm] WITH(NOLOCK) ON [adm].[ManagerUnitId] = [man].[Id] ");
        //    //builder.Append("                                             INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[Initial] [sigla] WITH(NOLOCK) ON [sigla].[InstitutionId] = [asm].[InstitutionId] ");
        //    //builder.Append("                                                                                                                      AND [sigla].[BudgetUnitId] = [bug].[Id] ");
        //    //builder.Append("    WHERE [asm].[InstitutionId] = 10)");

        //    //builder.Append(" WHERE [bem].[CHAPA] ='001406' ");

        //    return builder.ToString();
        //}

        private string getSQLInsertDadosPlanilhaExcel(DadosImportacaoBem importacaoExcel)
        {
            builder = new StringBuilder();
            DateTime dataAtual = DateTime.Now;


            builder.Append(" INSERT INTO [dbo].[ImportacaoExcel] ");
            builder.Append(" ([Data_Importacao] ");
            builder.Append(" ,[Sigla] ");
            builder.Append(" ,[Sigla_Descricao] ");
            builder.Append(" ,[Chapa] ");
            builder.Append(" ,[Codigo_do_Item] ");
            builder.Append(" ,[Descricao_do_Item] ");
            builder.Append(" ,[CPF_Responsavel] ");
            builder.Append(" ,[Nome_Responsavel] ");
            builder.Append(" ,[Cargo_Responsavel] ");
            builder.Append(" ,[Data_Aquisicao] ");
            builder.Append(" ,[Valor_Aquisicao] ");
            builder.Append(" ,[Data_Inclusao] ");
            builder.Append(" ,[Estado_de_Conservacao] ");
            builder.Append(" ,[Codigo_Orgao] ");
            builder.Append(" ,[Codigo_UO] ");
            builder.Append(" ,[Nome_UO] ");
            builder.Append(" ,[Codigo_UGE] ");
            builder.Append(" ,[Nome_UGE] ");
            builder.Append(" ,[Codigo_UA] ");
            builder.Append(" ,[Nome_UA] ");
            builder.Append(" ,[Codigo_Divisao] ");
            builder.Append(" ,[Nome_Divisao] ");
            builder.Append(" ,[Conta_Auxiliar] ");
            builder.Append(" ,[Contabil_Auxiliar] ");
            builder.Append(" ,[Nome_Conta_Auxiliar] ");
            builder.Append(" ,[Numero_Serie] ");
            builder.Append(" ,[Data_Fabricacao] ");
            builder.Append(" ,[Data_Garantia] ");
            builder.Append(" ,[Numero_Chassi] ");
            builder.Append(" ,[Marca] ");
            builder.Append(" ,[Modelo] ");
            builder.Append(" ,[Placa] ");
            builder.Append(" ,[Sigla_Antiga]");
            builder.Append(" ,[Chapa_Antiga] ");
            builder.Append(" ,[NEmpenho] ");
            builder.Append(" ,[Observacoes] ");
            builder.Append(" ,[Descricao_Adicional] ");
            builder.Append(" ,[Nota_Fiscal] ");
            builder.Append(" ,[CNPJ_Fornecedor] ");
            builder.Append(" ,[Nome_Fornecedor] ");
            builder.Append(" ,[Telefone_Fornecedor] ");
            builder.Append(" ,[CEP_Fornecedor] ");
            builder.Append(" ,[Endereco_Fornecedor] ");
            builder.Append(" ,[Cidade_Fornecedor] ");
            builder.Append(" ,[Estado_Fornecedor] ");
            builder.Append(" ,[Inf_Complementares_Fornecedores] ");
            builder.Append(" ,[CPF_CNPJ_Terceiro] ");
            builder.Append(" ,[Nome_Terceiro] ");
            builder.Append(" ,[Telefone_terceiro] ");
            builder.Append(" ,[CEP_Terceiro] ");
            builder.Append(" ,[Endereco_Terceiro] ");
            builder.Append(" ,[Cidade_Terceiro] ");
            builder.Append(" ,[Estado_Terceiro] ");
            builder.Append(" ,[Acervo] ");
            builder.Append(" ,[Terceiro]) ");
            builder.Append(" VALUES");
            builder.Append(" (" + dataAtual);
            builder.Append(" ," + importacaoExcel.Sigla);
            builder.Append(" ," + importacaoExcel.Sigla_Descricao);
            builder.Append(" ," + importacaoExcel.Chapa);
            builder.Append(" ," + importacaoExcel.Codigo_do_Item);
            builder.Append(" ," + importacaoExcel.Descricao_do_Item);
            builder.Append(" ," + importacaoExcel.CPF_Responsavel);
            builder.Append(" ," + importacaoExcel.Nome_Responsavel);
            builder.Append(" ," + importacaoExcel.Cargo_Responsavel);
            builder.Append(" ," + importacaoExcel.Data_Aquisicao);
            builder.Append(" ," + importacaoExcel.Valor_Aquisicao);
            builder.Append(" ," + importacaoExcel.Data_Inclusao);
            builder.Append(" ," + importacaoExcel.Estado_de_Conservacao);
            builder.Append(" ," + importacaoExcel.Codigo_Orgao);
            builder.Append(" ," + importacaoExcel.Codigo_UO);
            builder.Append(" ," + importacaoExcel.Nome_UO);
            builder.Append(" ," + importacaoExcel.Codigo_UGE);
            builder.Append(" ," + importacaoExcel.Nome_UGE);
            builder.Append(" ," + importacaoExcel.Codigo_UA);
            builder.Append(" ," + importacaoExcel.Nome_UA);
            builder.Append(" ," + importacaoExcel.Codigo_Divisao);
            builder.Append(" ," + importacaoExcel.Nome_Divisao);
            builder.Append(" ," + importacaoExcel.Conta_Auxiliar);
            builder.Append(" ," + importacaoExcel.Contabil_Auxiliar);
            builder.Append(" ," + importacaoExcel.Nome_Conta_Auxiliar);
            builder.Append(" ," + importacaoExcel.Numero_Serie);
            builder.Append(" ," + importacaoExcel.Data_Fabricacao);
            builder.Append(" ," + importacaoExcel.Data_Garantia);
            builder.Append(" ," + importacaoExcel.Numero_Chassi);

            builder.Append(" ," + importacaoExcel.Marca);
            builder.Append(" ," + importacaoExcel.Modelo);
            builder.Append(" ," + importacaoExcel.Placa);
            builder.Append(" ," + importacaoExcel.Sigla_Antiga);
            builder.Append(" ," + importacaoExcel.Chapa_Antiga);
            builder.Append(" ," + importacaoExcel.NEmpenho);
            builder.Append(" ," + importacaoExcel.Observacoes);
            builder.Append(" ," + importacaoExcel.Descricao_Adicional);
            builder.Append(" ," + importacaoExcel.Nota_Fiscal);
            builder.Append(" ," + importacaoExcel.CNPJ_Fornecedor);

            builder.Append(" ," + importacaoExcel.Nome_Fornecedor);
            builder.Append(" ," + importacaoExcel.Telefone_Fornecedor);
            builder.Append(" ," + importacaoExcel.CEP_Fornecedor);
            builder.Append(" ," + importacaoExcel.Endereco_Fornecedor);
            builder.Append(" ," + importacaoExcel.Cidade_Fornecedor);
            builder.Append(" ," + importacaoExcel.Estado_Fornecedor);
            builder.Append(" ," + importacaoExcel.Inf_Complementares_Fornecedores);
            builder.Append(" ," + importacaoExcel.CPF_CNPJ_Terceiro);
            builder.Append(" ," + importacaoExcel.Nome_Terceiro);

            builder.Append(" ," + importacaoExcel.Telefone_terceiro);
            builder.Append(" ," + importacaoExcel.CEP_Terceiro);
            builder.Append(" ," + importacaoExcel.Endereco_Terceiro);
            builder.Append(" ," + importacaoExcel.Cidade_Terceiro);
            builder.Append(" ," + importacaoExcel.Estado_Terceiro);
            builder.Append(" ," + importacaoExcel.Acervo);
            builder.Append(" ," + importacaoExcel.Terceiro + ")");

            builder.Append(" SELECT CAST(@@IDENTITY AS INT) ");
            return builder.ToString();
        }



        public int getNumerosNoTextoDoLegado(string texto)
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


            int ano;
            int mes;
            int dia = 1;

            if (dataLegado.Length == 8)
            {
                dia = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 2, 2));
                mes = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 4, 2));
                ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));

            }
            else if (dataLegado.Length == 6)
            {
                mes = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 2, 2));
                ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));

            }
            else if (dataLegado.Length == 5)
            {
                mes = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 1, 1));
                ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));
            }
            else
            {
                mes = DateTime.Now.Month;
                ano = DateTime.Now.Year;
            }
            try
            {
                DateTime date = new DateTime(ano, mes, dia);
                return date;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
        private string createSQLTipoDeIncorporacaoImportar(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT [tip].[COD_TPINC]  ,[tip].[DESCRICAO]  ,[tip].[COD_TRANS]  ,[bem].[COD_ORGC] ,MAX([bem].[SIGLA_UA]) 'SIGLA_UA',[uo].[COD_UGO] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[TipoIncorp] [tip] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON[tip].[COD_TPINC] = [bem].[COD_TPINC] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UGO] [uo] ON[bem].[COD_ORGC] = [uo].[COD_ORG] ");
            builder.Append(" GROUP BY [tip].[COD_TPINC]  ,[tip].[DESCRICAO]  ,[tip].[COD_TRANS]  ,[bem].[COD_ORGC] ,[uo].[COD_UGO] ");

            return builder.ToString();
        }


        private string createSQLContaAuxiliarImportar(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT[con].[CONTA_AUX] ");
            builder.Append(" ,[con].[NOME] ");
            builder.Append(" ,[con].[CCONTABIL] ");
            builder.Append(" ,[con].[NOME_ANTIGO] ");
            builder.Append(" ,[con].[CCONTABIL_ANTIGO] ");
            builder.Append(" ,[bem].[COD_ORGC] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[ContaAux] [con] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [con].[CONTA_AUX] = [bem].[CONTA_AUX] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");

            return builder.ToString();
        }

        private void createContaAuxiliar(string nomeDaBaseOrigem)
        {
            IDataReader reader = _legado.createComando(createSQLContaAuxiliarImportar(nomeDaBaseOrigem))
                                                                               .createDataReader(CommandType.Text);
            AuxiliaryAccount contaAuxiliar = null;

            while (reader.Read())
            {
                var code = getNumerosNoTextoDoLegado(reader["CONTA_AUX"].ToString());
                var _contaAuxiliar = contexto.AuxiliaryAccounts.Where(x => x.Code == code).FirstOrDefault();

                if (_contaAuxiliar == null)
                {
                    contaAuxiliar = new AuxiliaryAccount();
                    contaAuxiliar.Code = int.Parse(reader["CONTA_AUX"].ToString());
                    contaAuxiliar.Description = reader["CONTA_AUX"].ToString();
                    contaAuxiliar.Status = true;

                    contexto.AuxiliaryAccounts.Add(contaAuxiliar);
                    contexto.SaveChanges();

                    contaAuxiliar = null;
                }
            }
        }

        private string createSQLSiglaImportar(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            builder = new StringBuilder();
            builder.Append(" SELECT * FROM (");
            builder.Append(" SELECT [sig].[SIGLA] ");
            builder.Append(" ,[sig].[DESCRICAO] ");
            builder.Append(" ,[bem].[SIGLA_UA] 'SIGLA_UA' ");
            builder.Append(" FROM [" + nomeDaBaseOrigem + "].[dbo].[Sigla][sig] ");
            builder.Append(" INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON[sig].[SIGLA] = [bem].[SIGLA] ");
            builder.Append(" GROUP BY [sig].[SIGLA]  ,[sig].[DESCRICAO],[bem].[SIGLA_UA] ");
            builder.Append(" ) SIGLA ");
            builder.Append(" INNER JOIN [" + nomeDaBaseDestino + "].[dbo].[AdministrativeUnit] [adm] ON CONVERT(int, [adm].[Code]) = CONVERT(int, [" + nomeDaBaseDestino + "].[dbo].[RETORNA_NUMERO]([SIGLA].[SIGLA_UA]))  ");

            return builder.ToString();
        }

        private void createSigla(string nomeDaBaseOrigem, string nomeDaBaseDestino)
        {
            IDataReader reader = _legado.createComando(createSQLSiglaImportar(nomeDaBaseOrigem, nomeDaBaseDestino))
                                                                                .createDataReader(CommandType.Text);
            while (reader.Read())
            {
                Initial sigla = null;
                var uaCode = getNumerosNoTextoDoLegado(reader["SIGLA_UA"].ToString());
                var nodeSigla = reader["SIGLA"].ToString();

                var administrativeUnit = contexto.AdministrativeUnits.Where(x => x.Code == uaCode).FirstOrDefault();
                var _sigla = contexto.Initials.Where(x => x.Name == nodeSigla && x.InstitutionId == administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId && x.BudgetUnitId == administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id).FirstOrDefault();

                if (administrativeUnit != null && _sigla == null)
                {
                    try
                    {
                        sigla = new Initial();
                        sigla.BudgetUnitId = administrativeUnit.RelatedManagerUnit.BudgetUnitId;
                        sigla.Description = reader["DESCRICAO"].ToString();
                        sigla.InstitutionId = administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
                        sigla.Name = nodeSigla;
                        sigla.Status = true;

                        contexto.Initials.Add(sigla);
                        contexto.SaveChanges();
                        administrativeUnit = null;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        builderException = new StringBuilder();
                        foreach (var entidade in ex.EntityValidationErrors)
                        {

                            builderException.AppendFormat("Entidade do tipo \"{0}\" no estado \"{1}\" contem os seguintes erros:",
                                entidade.Entry.Entity.GetType().Name, entidade.Entry.State);
                            foreach (var exception in entidade.ValidationErrors)
                            {
                                builderException.AppendFormat(" Atributo(Propriedade): \"{0}\", Erro: \"{1}\"",
                                    exception.PropertyName, exception.ErrorMessage);
                            }
                        }

                        // throw new Exception(builderException.ToString(), ex.InnerException);

                    }
                }
                _sigla = null;
            }
        }
        private string createSQLTerceiroImportar(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT [ter].[COD_TERC] ");
            builder.Append(" ,[ter].[NOME] ");
            builder.Append(" ,[ter].[CGC_CPF] ");
            builder.Append(" ,[ter].[ENDERECO] ");
            builder.Append(" ,[ter].[END_MUN] ");
            builder.Append(" ,[ter].[END_EST] ");
            builder.Append(" ,[ter].[END_DDD] ");
            builder.Append(" ,[ter].[END_FONE] ");
            builder.Append(" ,[ter].[END_RAMAL] ");
            builder.Append(" ,[ter].[END_FAX] ");
            builder.Append(" ,[ter].[END_TELEX] ");
            builder.Append(" ,[ter].[END_CEP] ");
            builder.Append(" ,[bem].[SIGLA_UA] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[Terceiro] [ter] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON [bem].[COD_TERC] = [ter].[COD_TERC] ");
            builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[UA] [ua] ON [ua].[SIGLA_UA] = [bem].[SIGLA_UA] ");


            return builder.ToString();
        }

        private void createTerceiro(string nomeDaBaseOrigem)
        {
            IDataReader reader = _legado.createComando(createSQLTerceiroImportar(nomeDaBaseOrigem))
                                                                                .createDataReader(CommandType.Text);
            OutSourced terceiro = null;
            while (reader.Read())
            {
                try
                {
                    //var orgaoCode = reader["COD_ORGC"].ToString();
                    //var uoCod = orgaoCode + reader["COD_UGO"].ToString();
                    var cnpj = (reader["CGC_CPF"].ToString().Length > 14 ? reader["CGC_CPF"].ToString().Substring(0, 14) : reader["CGC_CPF"].ToString());
                    var uaCode = getNumerosNoTextoDoLegado(reader["SIGLA_UA"].ToString());

                    var administrativeUnit = contexto.AdministrativeUnits.Where(x => x.Code == uaCode).FirstOrDefault();
                    var _terceiro = contexto.OutSourceds.Where(x => x.CPFCNPJ.Equals(cnpj)
                                                                && x.InstitutionId == administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId
                                                                && x.BudgetUnitId == administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id
                                                               ).AsQueryable().FirstOrDefault();

                    if (administrativeUnit != null && _terceiro == null)
                    {
                        terceiro = new OutSourced();
                        terceiro.AddressId = 1;
                        terceiro.BudgetUnitId = administrativeUnit.RelatedManagerUnit.BudgetUnitId;
                        terceiro.InstitutionId = administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
                        terceiro.CPFCNPJ = cnpj;
                        terceiro.Name = reader["NOME"].ToString();
                        terceiro.Status = true;
                        terceiro.Telephone = reader["END_DDD"].ToString() + "-" + reader["END_FONE"].ToString();
                        if (terceiro.CPFCNPJ.Length > 10 && terceiro.CPFCNPJ.Length < 15)
                        {
                            contexto.OutSourceds.Add(terceiro);
                            contexto.SaveChanges();
                        }
                        terceiro = null;
                        administrativeUnit = null;
                    }
                    _terceiro = null;
                    administrativeUnit = null;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    terceiro = null;
                }


            }

            reader = null;
        }

        private string createSQLResponsavelImportar(string nomeDaBaseOrigem)
        {
            builder = new StringBuilder();

            builder.Append(" SELECT DISTINCT[res].[CPF] ");
            builder.Append(" ,[res].[NOME_RESP] ");
            builder.Append(" ,[res].[SIGLA_UA] ");
            builder.Append(" ,[res].[CARGO] ");
            builder.Append(" ,[res].[REFERENCIA] ");
            builder.Append(" ,[res].[NIVEL] ");
            builder.Append(" ,[res].[GRAU] ");
            builder.Append(" ,[res].[FLAG_INATIVO] ");
            builder.Append(" ,[res].[CODIGO_ENDERECO] ");
            //builder.Append(" ,[og].[COD_ORG] ");
            builder.Append("   FROM [" + nomeDaBaseOrigem + "].[dbo].[Responsavel] [res] ");
            //builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[BemPatrimonial] [bem] ON[res].[COD_RESP] = [bem].[COD_RESP] ");
            //builder.Append("   INNER JOIN [" + nomeDaBaseOrigem + "].[dbo].[Orgao] [og] ON[og].[COD_ORG] = [bem].[COD_ORGC] ");


            return builder.ToString();
        }

        private string createSQLImportacaoPlanilha(ImportacaoPlanilha importacaoPlanilha)
        {
            builder = new StringBuilder();
            builder.Append(" INSERT INTO[dbo].[AssetMovements] ");
            builder.Append(" ([NomeArquivo] ");
            builder.Append(" ,[Processado] ");
            builder.Append(" ,[Login_Importacao] ");
            builder.Append(" ,[Data_Importacao] ");
            builder.Append(" ,[Login_Processamento] ");
            builder.Append(" ,[Data_Processamento])");

            builder.Append(" VALUES ");
            builder.Append(" ('" + importacaoPlanilha.NomeArquivo + "'");
            builder.Append(" ," + importacaoPlanilha.Processado);
            builder.Append(" ,'" + importacaoPlanilha.Login_Importacao + "'");
            builder.Append(" ,'" + importacaoPlanilha.Data_Importacao + "'");
            builder.Append(" ,'" + importacaoPlanilha.Login_Processamento + "'");
            builder.Append(" ,'" + importacaoPlanilha.Data_Processamento + "')");
            builder.Append(" SELECT CAST(@@IDENTITY AS INT) ");

            return builder.ToString();
        }

        private string createSQLDadosImportacaoUsuario(DadosImportacaoUsuario dadosImportacaoUsuario)
        {
            builder = new StringBuilder();
            builder.Append(" INSERT INTO[dbo].[DadosImportacaoUsuario] ");
            builder.Append(" ([ImportacaoPlanilhaId] ");
            builder.Append(" ,[Data_Importacao] ");
            builder.Append(" ,[CARGA_SEQ] ");
            builder.Append(" ,[PERFIL] ");
            builder.Append(" ,[USUARIO_CPF] ");
            builder.Append(" ,[USUARIO_NOME_USUARIO] ");
            builder.Append(" ,[ORGAO_CODIGO] ");
            builder.Append(" ,[UO_CODIGO] ");
            builder.Append(" ,[UGE_CODIGO])");

            builder.Append(" VALUES ");
            builder.Append(" ('" + dadosImportacaoUsuario.ImportacaoPlanilhaId + "'");
            builder.Append(" ," + dadosImportacaoUsuario.Data_Importacao);
            builder.Append(" ,'" + dadosImportacaoUsuario.CARGA_SEQ + "'");
            builder.Append(" ,'" + dadosImportacaoUsuario.PERFIL + "'");
            builder.Append(" ,'" + dadosImportacaoUsuario.USUARIO_CPF + "'");
            builder.Append(" ,'" + dadosImportacaoUsuario.USUARIO_NOME_USUARIO + "'");
            builder.Append(" ,'" + dadosImportacaoUsuario.ORGAO_CODIGO + "'");
            builder.Append(" ,'" + dadosImportacaoUsuario.UO_CODIGO + "'");
            builder.Append(" ,'" + dadosImportacaoUsuario.UGE_CODIGO + "')");

            return builder.ToString();
        }

        private void createResponsavel(string nomeDaBaseOrigem)
        {
            IDataReader reader = _legado.createComando(createSQLResponsavelImportar(nomeDaBaseOrigem))
                                                                                .createDataReader(CommandType.Text);
            Responsible responsavel = null;
            while (reader.Read())
            {
                string uaCod = reader.GetString(2);
                var code = getNumerosNoTextoDoLegado(uaCod);

                var AdministrativeUnit = contexto.AdministrativeUnits.Where(x => x.Code == code).FirstOrDefault();

                if (AdministrativeUnit != null)
                {
                    var _responsavel = contexto.Responsibles.Where(x => x.AdministrativeUnitId == AdministrativeUnit.Id).AsQueryable().FirstOrDefault();
                    if (_responsavel == null)
                    {
                        responsavel = new Responsible();
                        responsavel.AdministrativeUnitId = AdministrativeUnit.Id;
                        responsavel.Name = reader["NOME_RESP"].ToString();
                        responsavel.Position = reader["CARGO"].ToString();
                        responsavel.Status = true;

                        contexto.Responsibles.Add(responsavel);

                        responsavel = null;
                    }
                    _responsavel = null;
                }
            }
            reader = null;
        }

        public IList<ChaveBemPatrimonialJson> getChavePatrimonio()
        {
            return this.AssetsImportados;
        }


        public void exportarPlanilhaExcel()
        {

        }


        private SqlConnection createConnection()
        {
            cnn = new SqlConnection(@"Data Source=D-PROD-BP100906\MSQL_SERVERR2;database=dbSAM_ProducaoSEFAZ_Validado;Persist Security Info=True;User ID=sa;Password=@prodesp2016;multipleactiveresultsets=True");
            cnn.Open();

            return cnn;
        }

        public DataTable getDataTable()
        {
            return this.tableExcel;
        }
        private void getPlanilhaParaImportacao()
        {
            string filePath = @"C:\Projetos\Prodesp - SCPweb\Fontes\Desenvolvimento\main\SAM.Web\Legado\Arquivos\SAMW_Habitacao_Patrimonio\SAMW_Habitacao_Patrimonio0_ExcelInventario.xlsx";
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }

            });



            this.tableExcel = result.Tables[0];
            this.tableExcel.Columns.Add("Importado", typeof(string));
            this.tableExcel.Columns.Add("Mensagem", typeof(string));

        }

        private void createResponsavelDaPlanilha(string baseDeDestino)
        {
            DataRowCollection rows = this.getDataTable().Rows;
            ImportarResponsavel responsavel = new ImportarResponsavel(this.contexto, this._legado, rows, baseDeDestino);

        }
        private void createDivisaoDaPlanilha(string baseDeDestino)
        {
            DataRowCollection rows = this.getDataTable().Rows;
            ImportarDivisao divisao = new ImportarDivisao(this.contexto, this._legado, rows, baseDeDestino);

        }

        private void createContaAuxiliarDaPlanilha(string baseDeDestino)
        {
            DataRowCollection rows = this.getDataTable().Rows;
            ImportarContaAuxiliar contaAuxiliar = new ImportarContaAuxiliar(this.contexto, this._legado, rows, baseDeDestino);

        }
        private void createFornecedorDaPlanilha(string baseDeDestino)
        {
            DataRowCollection rows = this.getDataTable().Rows;
            ImportarFornecedor fornecedor = new ImportarFornecedor(this.contexto, this._legado, rows, baseDeDestino);

        }

        private void createSiglaDaPlanilha(string baseDeDestino)
        {
            DataRowCollection rows = this.getDataTable().Rows;
            ImportarSigla sigla = new ImportarSigla(this.contexto, this._legado, rows, baseDeDestino);

        }
        private void createDescricaoDaPlanilha(string baseDeDestino)
        {
            DataRowCollection rows = this.getDataTable().Rows;
            ImportarDescricaoLegado descricao = new ImportarDescricaoLegado(this.contexto, this._legado, rows, baseDeDestino);

        }
        private void createTerceiroDaPlanilha(string baseDeDestino)
        {
            DataRowCollection rows = this.getDataTable().Rows;

            ImportarTerceiro terceiro = new ImportarTerceiro(this.contexto, this._legado, rows, baseDeDestino);

        }


        #endregion
    }
}