using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.IO;
using System.Web.SessionState;
using System.Reflection;

namespace Sam.Common.Util
{
    public static class Constante
    {
        public static   HttpContext _currentHttpContext = (HttpContext.Current.IsNotNull()?(HttpContext.Current):null);
        public const    decimal CST_AUXILIAR_COMPILACAO_CONDICIONAL_VALOR_ZERO = 0.0000m;

        public static string pathApplication = _currentHttpContext.Request.ApplicationPath == "/" ? "" : _currentHttpContext.Request.ApplicationPath;
        public static string physicalPathApplication = _currentHttpContext.Request.PhysicalApplicationPath;
        public static string FullPhysicalPathApp = physicalPathApplication;
        public static string ReportPath = pathApplication + "/Relatorios/";
        public static string ReportUrl = pathApplication + "/Relatorios/imprimirRelatorio.aspx";
        public static string ImageUrl = pathApplication + "/Relatorios/obterImagem.ashx";
        public const  string MaterialApoioFolder = "MaterialApoio";
        public const  string ImagesFolder = "Imagens";
        public static string ReportScript = "<script language='javascript'>" + "window.open('" + ReportUrl + "', 'CustomPopUp', " + "'width=800,height=600,toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, top=0, left=0')" + "</script>";

        public static bool isSamWebDebugged = (System.Diagnostics.Debugger.IsAttached);

        private const string CST_SEPARADOR_SPLIT_CARACTER_NULO = "\0";
        private const string CST_SEPARADOR_SPLIT_INICIO_NOVA_LINHA = "\r\n";
        private const string CST_SEPARADOR_SPLIT_NOVA_LINHA = "\n";
        private const string CST_SEPARADOR_SPLIT_ESPACO_EM_BRANCO = " ";

        public static readonly string[] CST_SEPARADOR_SPLIT = new string[] { CST_SEPARADOR_SPLIT_CARACTER_NULO, CST_SEPARADOR_SPLIT_NOVA_LINHA, CST_SEPARADOR_SPLIT_ESPACO_EM_BRANCO, CST_SEPARADOR_SPLIT_INICIO_NOVA_LINHA };

        public static SortedList CST_CONTA_CONTABIL_EMITIDOS_NAO_LIQUIDADOS = new SortedList(new Dictionary<string, string>(){ {"2013", "192410101"}, 
                                                                                                                               {"2014", "522910101"},
                                                                                                                               {"2015", "522910101"},
                                                                                                                               {"2016", "522910101"},
                                                                                                                               {"2017", "522910101"},
                                                                                                                               {"2018", "522910101"},
                                                                                                                               {"2019", "522910101"},
                                                                                                                               {"2020", "522910101"}} as IDictionary);

        public static SortedList CST_CONTA_CONTABIL_RESTOS_A_PAGAR = new SortedList(new Dictionary<string, string>(){ {"2013", "292410101"}, 
                                                                                                                      {"2014", ""},
                                                                                                                      {"2015", "531110101"},
                                                                                                                      {"2016", "531110101"},
                                                                                                                      {"2017", "531110101"},
                                                                                                                      {"2018", "531110101"},
                                                                                                                      {"2019", "531110101"},
                                                                                                                      {"2020", "531110101"}} as IDictionary);

        public static SortedList CST_CONTA_CONTABIL_INCORPORADOS_EXECUCAO_ORCAMENTARIA = new SortedList(new Dictionary<string, string>(){ {"2013", "113110101"}, 
                                                                                                                                          {"2014", "115610101"} } as IDictionary);

        public static SortedList CST_CONTA_CONTABIL_MATERIAIS_NO_ESTOQUE = new SortedList(new Dictionary<string, string>() { { "2014", "115610153" } } as IDictionary);

        public static SortedList CST_CONTA_CONTABIL_MATERIAIS_NO_ESTOQUE_CONTROLE_POR_SETOR = new SortedList(new Dictionary<string, string>(){ {"2013", null}, 
                                                                                                                                               {"2014", "115610152"} } as IDictionary);

        public static SortedList CST_CONTA_CONTABIL_CREDITO_DISPONIVEL = new SortedList(new Dictionary<string, string>() { { "2014", "622110101" } } as IDictionary);

        public const    string    CST_SIAFEM_CE_PADRAO_HOMOLOGACAO            = "CE999";
        public const    string    CST_SIAFEM_CE_PADRAO_PRODUCAO               = "CE999";
        public static   string    CST_SIAFEM_CE_PADRAO
        {
            get
            {
                string cePadrao = CST_SIAFEM_CE_PADRAO_PRODUCAO;

                if ((Constante.CST_DEBUG_IDENTIFICACAO_AMBIENTE_APLICACAO.ToLowerInvariant() == "homologacao") || 
                    (Constante.CST_DEBUG_IDENTIFICACAO_AMBIENTE_APLICACAO.ToLowerInvariant() != "producao"))
                    cePadrao = CST_SIAFEM_CE_PADRAO_HOMOLOGACAO;

                return cePadrao;
            }
        }

        public const    string    CST_CONTA_CONTABIL_OPCAO                    = "1";
        public const    string    CST_CONTA_CONTABIL_OPCAO_DETALHADA          = "1";
        public const    string    CST_CONTA_CONTABIL_OPCAO_SALDO              = "2";
        public const    string    CST_CONTA_CONTABIL_OPCAO_INVERSAO_SALDO     = "3";
        public const    string    CST_CONTA_CONTABIL_OPCAO_SALDO_ZERO         = "4";

        public const    string    CST_NATUREZA_DESPESA__BEM_CONSUMO_GERAL__INICIO_SEIS_DIGITOS       = "339030";
        public const    string    CST_NATUREZA_DESPESA__BEM_PERMAMENTE_GERAL__INICIO_SEIS_DIGITOS    = "449052";
        public const    string    CST_NATUREZA_DESPESA__BEM_PERMAMENTE_EQUIP_TI__INICIO_SEIS_DIGITOS = "449088";        

        public const    int        CST_ANO_MES_DATA_CORTE_SAP                                   = 201407;
        public const    int        CST_NUMERO_LINHAS_CAMPO_OBSERVACAO_SIAFEM                    = 3;
        public const    int        CST_NUMERO_MAXIMO_DOCUMENTOS_TRANSACAO_SIAFEM_NL             = 14;
        public const    int        CST_COMPRIMENTO_MAXIMO_CAMPO_OBSERVACAO_SIAFEM               = 77;
        public const    int        CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO                  = 40;
        public const    int        CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS                  = 50;
        public const    int        CST_COMPRIMENTO_MAXIMO_NOME_ARQUIVO_ANEXO_CHAMADO_SUPORTE    = 75;

        public const    int        CST_CODIGO_ORGAO__CLIENTE_FCASA                              = 17;
        public const    int        CST_CODIGO_ALMOX_GMAN__CLIENTE_FCASA                         = 012;

        public const    string     CST_CABECALHO_DECLARACAO_XML       = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?>";
        public const    string     CST_URL_SEFAZ_AMBIENTE_PRODUCAO    = @"https://www6.fazenda.sp.gov.br/siafisico/RecebeMSG.asmx";
        public const    string     CST_URL_SEFAZ_AMBIENTE_HOMOLOGACAO = @"(url com caminho de homologação)";
                
        public const    string     CST_DESCRICAO_EMPENHO_TIPO_BEC               = "BEC";
        public const    string     CST_DESCRICAO_EMPENHO_TIPO_PREGAO_ELETRONICO = "PREGAO";
        public const    string     CST_DESCRICAO_EMPENHO_TIPO_SIAFISICO         = "BEC";

        #region Logins Publicos Consulta

        public static   string LoginUsuarioPublicoSiafem        = "(fornecido pela SEFAZ)";
        public static   string LoginUsuarioPublicoSiafisico     = "(fornecido pela SEFAZ)";

        public const    string   SenhaUsuarioPublicoSiafem      = "(fornecido pela SEFAZ)";
        public const    string   SenhaUsuarioPublicoSiafisico   = "(fornecido pela SEFAZ)";

        #endregion Logins Publicos Consulta

        #region Siafem XML Patterns

        public const   string CST_XML_PATTERN_LIQUIDACAO_EMPENHO_MSG_RETORNO               = "MSG/SISERRO/Doc_Retorno/*/*/Msg";
        public const   string CST_XML_PATTERN_LIQUIDACAO_EMPENHO_CODIGO_NOTA_LIQUIDACAO    = "MSG/SISERRO/Doc_Retorno/*/*/documento/NL";
        public const   string CST_XML_PATTERN_LIQUIDACAO_EMPENHO_CODIGO_NUMERO_EMPENHO     = "MSG/SISERRO/Doc_Retorno/*/*/documento/NE";
        

        //WS Fechamento
        public const   string CST_NOME_WS_LIQUIDACAO_FECHAMENTO                            = "SiafemDocNLConsumo";
        public const   string CST_XML_PATTERN_LIQUIDACAO_FECHAMENTO_DOCUMENTO              = "MSG/BCMSG/Doc_Estimulo/SIAFDOC/SiafemDocNLConsumo/documento";
        public const   string CST_XML_PATTERN_LIQUIDACAO_FECHAMENTO_CODIGO_NOTA_LIQUIDACAO = "MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDocNLConsumo/documento/NumeroNL";

        public const   string CST_MSG_ERRO_MUDAPAH                                         = "===>  PARAMETROS ADICIONAIS ATUALIZADOS PARA ESTA SESSAO";
        public const   string CST_MSG_ERRO_LOGIN_MENSAGEM                                  = "ERRO/SIAFDOC/SiafemLogin/login/MsgErro";
        public const   string CST_MSG_ERRO_LOGIN_STATUS_OPERACAO                           = "ERRO/SIAFDOC/SiafemLogin/login/StatusOperacao";
        
        public static   string CST_XML_PATTERN_MSG_ERRO_MUDAPAH                             = "SIAFDOC/SiafemMudapah/Mudapah/MsgErro";

        //WS's Consultas diversas
        public const   string CST_NOME_WS_CONSULTA_LISTA_PT_RES                            = "SiafemDocDetPTRES";
        public const   string CST_NOME_WS_CONSULTA_EMPENHOS_UNIDADE_GESTORA                = "SiafemDocConsultaEmpenhos";
        public const   string CST_NOME_WS_CONSULTA_DETALHE_CONTA_CONTABIL_GENERICA         = "SiafemDetaconta";
        
        public const   string CST_XML_PATTERN_HORARIO_ACESSO                               = "MSG/SISERRO/Doc_Retorno/*/HorarioAcesso/*";
        public const   string CST_XML_PATTERN_MSG_RETORNO                                  = "/MSG/SISERRO/Doc_Retorno/*/*/MsgRetorno";
        public const   string CST_XML_PATTERN_STATUS_OPERACAO                              = "/MSG/SISERRO/Doc_Retorno/*/*/StatusOperacao";
        public const   string CST_XML_PATTERN_STATUS_OPERACAO_DOCUMENTO                    = "/MSG/SISERRO/Doc_Retorno/*/*/documento/StatusOperacao";
        public const   string CST_XML_PATTERN_CONSULTA_ITENS_EMPENHO                       = "MSG/SISERRO/Doc_Retorno/SFCODOC/*/documento/TabelaItem/";

        public const   string CST_XML_PATTERN_GENERICO_CONSULTA_DOCUMENTO                  = "/MSG/SISERRO/Doc_Retorno/*/*/*/Repete/Documento";

        #endregion Siafem XML Patterns

        #region Mensagens Erro

        public const   string CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR                   = "O tempo limite da operação foi atingido";
        public const   string CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_EN_US                   = "The operation has timed out";
        public const   string CST_MSG_ERRO_CONEXAO_INTERMITENTE_EN_US                      = "The underlying connection was closed: An unexpected error occurred on a send";
        public const   string CST_MSG_ERRO_CONEXAO_INTERMITENTE_PT_BR                      = "A conexão subjacente estava fechada: Erro inesperado em um envio";
        public const   string CST_MSG_ERRO_SOLICITACAO_GATEWAY_NAO_ENCONTRADO_EN_US        = "The remote server returned an error: (504) Gateway Timeout";
        public const   string CST_MSG_ERRO_SOLICITACAO_GATEWAY_NAO_ENCONTRADO_PT_BR        = "O servidor remoto retornou um erro: (504) Tempo excedido do gateway";
        public const   string CST_MSG_ERRO_CONEXAO_SERVIDOR_NAO_ENCONTRADO_EN_US           = "The remote server returned an error: (404) Not Found";
        public const   string CST_MSG_ERRO_CONEXAO_SERVIDOR_NAO_ENCONTRADO_PT_BR           = "O servidor remoto retornou um erro: (404) Não Localizado";
        public const   string CST_MSG_ERRO_REQUISICAO_ABORTADA_ERRO_SEG_SSL_TLS_EN_US      = "The request was aborted: Could not create SSL/TLS secure channel";
        public const   string CST_MSG_ERRO_PROCESSAMENTO_XML_EN_US                         = "Data at the root level is invalid";
        public const   string CST_MSG_ERRO_PROCESSAMENTO_XML_PT_BR                         = "Erro ao processar XML (Dados no nível raiz inválidos)";
        public const   string CST_MSG_ERRO_PROCESSAMENTO_OPERACAO                          = "Erro ao processar solicitação.";

        public const   string CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ              = "Falha de comunicação com a Secretaria da Fazenda";
        public const   string CST_MSG_ERRO_AMIGAVEL_SERVIDORES_SIAF_INDISPONIVEIS          = "Servidores SIAFEM/SIAFISICO indisponíveis no momento";
        public const   string CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR               = "Acionar administrador do sistema!";

        public const   string CST_PREFIXO_MSG_ERRO_RETORNO_WEBSERVICES_SEFAZ               = "Erro Webservice SEFAZ (retornado)";

        public const    string CST_MSG_ERRO_SIAFEM_USUARIO_BLOQUEADO                        = "USUARIO BLOQUEADO. CONTACTAR O SEU CADASTRADOR";
        public const    string CST_MSG_ERRO_SIAFEM_SENHA_INCORRETA                          = "ACESSO NAO PERMITIDO";
        public const    string CST_MSG_ERRO_SIAFEM_SENHA_EXPIRADA                           = "POR FAVOR, TROQUE SUA SENHA";
        public const    string CST_MSG_ERRO_WS_SIAFNET_ERRO_EXECUCAO                        = "Erro de Execução";

        public const    string CST_MSG_EXISTENCIA_ERRO_PAGAMENTO_NLCONSUMO                  = @"Inconsistência(s) ocorrida(s) ao pagar NL Consumo. Favor verificar descrição do erro, na coluna 'NL Consumo'.";

        public const    string CST_MSG_ERRO_ESP_DUPLICIDA                                   = "Já existe ESP cadastrada para este Gestor e Módulo";

        //Consolidacao Mensagens de Erro
        public static IList<string> ErrosDeConexao                                          = new List<string>(){  Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR, 
                                                                                                                   Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_EN_US,
                                                                                                                   Constante.CST_MSG_ERRO_CONEXAO_INTERMITENTE_EN_US,
                                                                                                                   Constante.CST_MSG_ERRO_CONEXAO_INTERMITENTE_PT_BR,
                                                                                                                   Constante.CST_MSG_ERRO_CONEXAO_SERVIDOR_NAO_ENCONTRADO_PT_BR,
                                                                                                                   Constante.CST_MSG_ERRO_CONEXAO_SERVIDOR_NAO_ENCONTRADO_EN_US,
                                                                                                                   Constante.CST_MSG_ERRO_REQUISICAO_ABORTADA_ERRO_SEG_SSL_TLS_EN_US,
                                                                                                                   Constante.CST_MSG_ERRO_SOLICITACAO_GATEWAY_NAO_ENCONTRADO_EN_US };
        #endregion Mensagens Erro

        #region Constantes DEBUG
        
        public const    string CST_DEBUG_DEPLOY_HOMOLOGACAO                                 = "Debug Ambiente HML";
        public const    string CST_DEBUG_TRACE_HOMOLOGACAO                                  = "Trace Ambiente HML";

        public static   string CST_DEBUG_IDENTIFICACAO_AMBIENTE_APLICACAO
        {
            get
            {
                string identificacaoAmbiente = null;

                if (HttpContext.Current.IsNotNull() && HttpContext.Current.Request.IsNotNull() && HttpContext.Current.Request.Url.IsNotNull())
                {
                    var url = HttpContext.Current.Request.Url.AbsolutePath;

                    if (url.Contains("(caminho de url de homologação do patrimonio)"))
                        identificacaoAmbiente = "Homologacao";
                    else if (url.Contains("(caminho de url de produção do patrimonio)"))
                        identificacaoAmbiente = "Producao";
                    else
                    {
                        var nomeAmbienteDEV = HttpContext.Current.Request.Url.ToString()
                                                                             .Replace(HttpContext.Current.Request.Url.AbsolutePath, "")
                                                                             .Replace("https://", "")
                                                                             .Replace("http://", "");
                        identificacaoAmbiente = String.Format("Ambiente Interno/Desenvolvimento (Prodesp) [{0}]", nomeAmbienteDEV);
                    }
                }

                return identificacaoAmbiente;
            }
        }
    
        #endregion Constantes DEBUG
    }
}
