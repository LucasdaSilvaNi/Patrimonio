using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Xml;
using Sam.Common;
using Sam.Common.Util;
using Sam.Integracao.SIAF.Configuracao;
using enumSistemaSIAF = Sam.Common.Util.GeralEnum.SistemaSIAF;
using Sam.Integracao.SIAF.ClientWS;


namespace Sam.Integracao.SIAF.Core
{
    //public static class ProcessadorServicoSIAF
    public class ProcessadorServicoSIAF
    {
        #region Variaveis
        private const string nomeTagFimDocumentoSIAF = "Doc_Retorno";
        private const string nomeTagStatusExecucaoOperacaoSIAF = "StatusOperacao";
        private const string nomeTagMsgRetorno = "MsgRetorno";
        private const string nomeTagMsgErro = "MsgErro";
        private const string nomeTagLogin = "Login";

        private const string hdrMsgErroLoginInvalido = "Retorno (Sistema {0}): {1}";
        private static string hdrRetornoMsgErro = "Mensagem retornada pelo sistema {1}: {0}.";
        private static string hdrMsgErrosInfra = "{0} ({1}) \n{2}";


        private ConfiguracoesSIAF configuracoesSIAF { get; set; }
        private static IDictionary<string, string> catalogoInternoMSGsSIAF { get; set; }
        private static SortedList tagsErroMsgSIAFEM { get; set; }
        private static bool isAmbienteHomologacao { get; set; }
        private static bool isAmbienteProducao { get; set; }
        private string loginUsuarioSIAF;
        private string loginUsuarioSAM;
        private string senhaUsuarioSIAF;
        private string nomeSistemaSIAF;
        private string nomeMsgEstimuloWs;
        private string msgErroRetornoWs;
        private string msgErroRetornoSimplificadoWs;
        private string retornoWsSIAF;
        private string msgEstimuloWsSIAF;
        private DateTime dataEnvioMsg;
        private bool erroProcessamentoEstimulo;
        private enumSistemaSIAF sistemaSIAF;
        #endregion

        #region Propriedades
        //public static CredenciaisSIAF Credenciais
        ////public CredenciaisSIAF Credenciais
        //{ 
        //    get { return credenciaisSIAF; }
        //    //set { credenciaisSIAF = value; }
        //}
        public ConfiguracoesSIAF Configuracoes
        { 
            get { return configuracoesSIAF; }
            set { configuracoesSIAF = value; }
        }
        public static bool IsAmbienteHomologacao
        { 
            get { return isAmbienteHomologacao; } 
        }
        public static bool IsAmbienteProducao 
        {
            get { return isAmbienteProducao; }
        }
        public string NomeSistemaSIAF
        {
            get { return nomeSistemaSIAF; }
        }
        public string NomeMensagemEstimuloWs
        {
            get { return nomeMsgEstimuloWs; }
        }
        public string RetornoWsSIAF
        {
            get { return retornoWsSIAF; }
        }
        public string EstimuloWsSIAF
        {
            get { return msgEstimuloWsSIAF; }
        }
        public string ErroRetornoWs
        {
            get { return msgErroRetornoWs; }
        }
        public string ErroRetornoSimplificadoWs
        {
            get { return msgErroRetornoSimplificadoWs; }
        }
        public bool ErroProcessamentoWs
        {
            get { return erroProcessamentoEstimulo; }
        }
        public DateTime DataEnvioMsgWs
        {
            get { return dataEnvioMsg; }
        }
        public enumSistemaSIAF SistemaSIAF;
        #endregion

        #region Métodos

        private static XmlWriterSettings obterFormatacaoPadraoParaXml()
        {
            return new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
        }

        public ProcessadorServicoSIAF()
        { }
        public ProcessadorServicoSIAF(string loginSiafUsuario, string senhaSiafUsuario)
        {
            this.loginUsuarioSIAF = loginSiafUsuario;
            this.senhaUsuarioSIAF = senhaSiafUsuario;
        }

        /// <summary>
        /// Método de inicialização da classe
        /// </summary>
        /// <returns></returns>
        private void load()
        {
            limparStatus();
            initMensagens();

            configuracoesSIAF = (configuracoesSIAF.IsNull() ? new ConfiguracoesSIAF() : configuracoesSIAF);


            //Determinando qual ambiente está sendo utilizado.
            isAmbienteHomologacao = ConfiguracoesSIAF.wsURLConsulta.Contains(Constante.CST_URL_SEFAZ_AMBIENTE_HOMOLOGACAO);
            isAmbienteProducao = !isAmbienteHomologacao;
        }
        /// <summary>
        /// Método de inicialização de limpeza de status da classe
        /// </summary>
        /// <returns></returns>
        private void limparStatus()
        {
            configuracoesSIAF = null;

            catalogoInternoMSGsSIAF = null;
            tagsErroMsgSIAFEM = null;
            
            isAmbienteHomologacao = false;
            isAmbienteProducao = false;

            nomeSistemaSIAF = null;
            nomeMsgEstimuloWs = null;
            msgErroRetornoWs = null;
            msgErroRetornoSimplificadoWs = null;
            retornoWsSIAF = null;
            msgEstimuloWsSIAF = null;
            erroProcessamentoEstimulo = false;

            loginUsuarioSAM = null;
            loginUsuarioSIAF = null;
            senhaUsuarioSIAF = null;
        }

        /// <summary>
        /// Método responsável pelo catálogo de todas as mensagens processadas (a procura de erros), pelo sistema.
        /// </summary>
        /// <returns></returns>
        private static void initMensagens()
        {
            KeyValuePair<string, string>[] arrNomeMsgSiafemComTagErro = null;
            string[] arrNomeMsgSiafisico = null;

            List<string> lstMsgSiafisico = new List<string>(); ;
            List<KeyValuePair<string, string>> lstMsgSiafemComTagErro = new List<KeyValuePair<string, string>>();

            arrNomeMsgSiafisico = new string[] { "SiafisicoDocCONNLItem", 
                                                 "SiafisicoDocConItemDetalha", 
                                                 "SiafisicoDocNLPregaoExecuta", 
                                                 "SiafisicoDocCONNLPregaoItem", 
                                                 "SiafisicoDocContNLBec", 
                                                 "SiafisicoDocContNLBecDet",
                                                 "SFCOLiquidaNL" };

            arrNomeMsgSiafemComTagErro = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("SiafisicoDocConsultaI", nomeTagMsgRetorno),
                                                                              new KeyValuePair<string, string>("SiafisicoDocConsultaF", nomeTagMsgRetorno),
                                                                              new KeyValuePair<string, string>("SiafisicoLogin", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafemLogin", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafemDocDetPTRES", nomeTagMsgRetorno), 
                                                                              new KeyValuePair<string, string>("SiafemDocConsultaEmpenhos", nomeTagMsgRetorno), 
                                                                              new KeyValuePair<string, string>("SiafemDocNLConsumo", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafemDocNL", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafemDocNLEmLiq", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafemDocDetaContaGen", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafMonitora", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SFCOLiqNLBec", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SFCONLPregao", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafemDetaContaGen", nomeTagMsgErro),
                                                                              new KeyValuePair<string, string>("SiafisicoConPrecoNE", nomeTagMsgRetorno),
                                                                              new KeyValuePair<string, string>("SiafisicoDocListaPrecoNE", nomeTagMsgRetorno),
                                                                              new KeyValuePair<string, string>("SiafemDocNlPatrimonial", nomeTagMsgErro)
                                                                            };

            catalogoInternoMSGsSIAF = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            lstMsgSiafisico.AddRange(arrNomeMsgSiafisico);
            lstMsgSiafemComTagErro.AddRange(arrNomeMsgSiafemComTagErro);


            foreach (var mensagemSiafisico in lstMsgSiafisico)
                catalogoInternoMSGsSIAF.Add(mensagemSiafisico, "Msg");


            foreach (var mensagemSiafemTagErro in lstMsgSiafemComTagErro)
                catalogoInternoMSGsSIAF.Add(mensagemSiafemTagErro.Key, mensagemSiafemTagErro.Value);

            tagsErroMsgSIAFEM = new SortedList(catalogoInternoMSGsSIAF as IDictionary, StringComparer.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// Método utilizado para ignorar validação de certificado SSL
        /// </summary>
        private static void initiateSSLTrust()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        }

        /// <summary>
        /// Função de tratamento para erro de autenticação aos sistemas SIAFEM/SIAFISICO, retornados via mensagem XML
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private bool loginInvalido(string strRetornoMsgEstimulo)
        {
            #region Variaveis
            bool blnRetorno = false;
            string strNomeSistema = null;
            #endregion Variaveis

            #region Tratamento Erro Servidor VHI
            var lstXmlElements = (new XmlDocument()).LoadXmlDocument(strRetornoMsgEstimulo)
                                                    .GetElementsByTagName("SIAFDOC")
                                                    .Cast<System.Xml.XmlElement>()
                                                    .ToList();

            if (!lstXmlElements.IsNullOrEmpty() && lstXmlElements.Count > 1)
            {
                foreach (var item in lstXmlElements)
                {
                    if (item.NextSibling.IsNotNull() && item.NextSibling.InnerText.Contains("RobosSiafNetVHI"))
                    {
                        strNomeSistema = item.ChildNodes[1].Name.Replace(nomeTagLogin, "").ToUpperInvariant();
                        var strPrefixoMensagemErro = String.Format(hdrMsgErroLoginInvalido, strNomeSistema, msgErroRetornoWs);


                        msgErroRetornoWs = String.Format("{0}.\nFalha no servidor VHI/{1}. Favor contatar equipe CAU/SEFAZ para regularização.", strPrefixoMensagemErro, strNomeSistema);
                        blnRetorno = true;
                        erroProcessamentoEstimulo = blnRetorno;
                        break;
                    }
                }

            }
            #endregion Tratamento Erro Servidor VHI

            #region Outros Erros Login
            var elementoLogin = (new XmlDocument()).LoadXmlDocument(strRetornoMsgEstimulo)
                                                   .GetElementsByTagName(nomeTagMsgErro)
                                                   .Cast<System.Xml.XmlElement>()
                                                   .ToList()
                                                   .FirstOrDefault();
            if (elementoLogin.IsNotNull())
            {
                msgErroRetornoWs = elementoLogin.InnerText.BreakLine('-', 0).Trim();
                msgErroRetornoSimplificadoWs = msgErroRetornoWs;
                strNomeSistema = elementoLogin.ParentNode.ParentNode.Name.Replace(nomeTagLogin, "").ToUpperInvariant();

                string strPrefixoMensagemErro = String.Format(hdrMsgErroLoginInvalido, strNomeSistema, msgErroRetornoWs);
                msgErroRetornoWs = String.Format(hdrMsgErroLoginInvalido, strNomeSistema, msgErroRetornoWs);

                blnRetorno = true;
            }
            #endregion Outros Erros Login

            retornoWsSIAF = strRetornoMsgEstimulo;
            erroProcessamentoEstimulo = blnRetorno;
            return blnRetorno;
        }
        /// <summary>
        /// Função para tratar erros de conexão (timeout, infra, etc), quando do envio de mensagem de estÃ­mulo ao servidor SIAFEM/SIAFISICO
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private bool erroAcessoOuConexao(string strRetornoMsgEstimulo, out string strMensagemErro)
        {
            bool blnRetorno = false;
            string strRetorno = string.Empty;

            #region Erros Infra
            //erros quaisquer retornados por servidor SEFAZ (conexão)
            IList<string> listaErros = Constante.ErrosDeConexao;
            listaErros.ToList().ForEach(DescricaoErro => { if ((!String.IsNullOrWhiteSpace(strRetorno)) && (String.Format("{0}.", strRetorno).ToLowerInvariant() == DescricaoErro.ToLowerInvariant() || strRetorno.ToLowerInvariant() == DescricaoErro.ToLowerInvariant())) { strRetorno = DescricaoErro; } });

            blnRetorno = !String.IsNullOrWhiteSpace(strRetorno);
            if (blnRetorno)
            {
                strRetorno = strRetorno.Replace("..", ".");
                if ((strRetorno == Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_EN_US) || (strRetorno == Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR) ||
                     (strRetorno == Constante.CST_MSG_ERRO_CONEXAO_INTERMITENTE_EN_US) || (strRetorno == Constante.CST_MSG_ERRO_CONEXAO_INTERMITENTE_PT_BR))
                    strRetorno = String.Format("Erro no Webservice! {0} ('{1}')\n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, "Serviço SIAFISICO/SIAFEM indisponível no momento.", "Favor entrar em contato com setor DTI da Secretaria da Fazenda.");
                else
                    strRetorno = String.Format("Erro no Webservice! {0} ('{1}')\n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, strRetorno, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);

                strMensagemErro = strRetorno;
                return blnRetorno;
            }
            #endregion Erros Infra

            msgErroRetornoSimplificadoWs = msgErroRetornoWs = strMensagemErro = strRetorno;
            erroProcessamentoEstimulo = blnRetorno;

            return blnRetorno;
        }
        /// <summary>
        /// Função para tratar retornos não-padrão do servidor SIAFEM/SIAFISICO
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private bool retornoSemXml(string strRetornoMsgEstimulo, out string strMensagemErro)
        {
            bool blnRetorno = false;
            strMensagemErro = string.Empty;

            //Nenhum texto retornado
            if (String.IsNullOrWhiteSpace(strRetornoMsgEstimulo))
            {
                strMensagemErro = "Interface com sistemas SIAFEM/SIAFISICO/Contabiliza-SP não retornou dados.";
                blnRetorno = true;
            }
            else if (!String.IsNullOrWhiteSpace(strRetornoMsgEstimulo) && !XmlUtil.IsXML(strRetornoMsgEstimulo))
            {

                //ULTIMA TENTATIVA DE LEITURA DO XML ANTES DE DEVOLVER MENSAGEM RELATANDO XML MAL-FORMADO AO USUARIO
                {
                    string _strRetornoMsgEstimuloGeradorEstimuloSIAF = strRetornoMsgEstimulo;
                    _strRetornoMsgEstimuloGeradorEstimuloSIAF = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(_strRetornoMsgEstimuloGeradorEstimuloSIAF);

                    if (XmlUtil.IsXML(_strRetornoMsgEstimuloGeradorEstimuloSIAF))
                    {
                        strMensagemErro = _strRetornoMsgEstimuloGeradorEstimuloSIAF;
                        return false;
                    }
                }

                var _msgErro = strRetornoMsgEstimulo.Replace(Environment.NewLine, " ").Trim();
                strMensagemErro = String.Format("Retorno SIAFEM/SIAFISICO (erro): {0}", _msgErro);
                blnRetorno = true;
            }
            else if (!String.IsNullOrWhiteSpace(strRetornoMsgEstimulo) && XmlUtil.IsXML(strRetornoMsgEstimulo, ref strMensagemErro)) //Melhor pecar pelo excesso que pela falta...
            {
                blnRetorno = false;
            }

            msgErroRetornoWs = strMensagemErro;
            retornoWsSIAF = strRetornoMsgEstimulo;
            erroProcessamentoEstimulo = blnRetorno;

            return blnRetorno;
        }
        /// <summary>
        /// Função para tratar retornos não-padrão do servidor SIAFEM/SIAFISICO
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private bool retornoInvalido(string strRetornoMsgEstimulo, out string strMensagemErro)
        {
            bool blnRetorno = false;
            XmlNode elRetornoInvalido = null;
            strMensagemErro = string.Empty;

            //Nenhum texto retornado
            if (String.IsNullOrWhiteSpace(strRetornoMsgEstimulo))
            {
                strMensagemErro = "Servidor SIAFEM/SIAFISICO não retornou dados.";
                blnRetorno = true;
            }
            else if (strRetornoMsgEstimulo.ToLowerInvariant().Contains("tried to invoke method") ||
                     strRetornoMsgEstimulo.ToLowerInvariant().Contains("java.lang.classcastexception@")) //Erro de processamento no container web webservice
            {
                strRetornoMsgEstimulo = strRetornoMsgEstimulo.Replace("\r", " ").Replace("\t", "").Replace("\n", "");

                strMensagemErro = "Consulta aos servidores SIAFEM/SIAFISICO indisponível no momento.\nFavor enviar e-mail ao suporte do sistema, informando esta mensagem.";
                blnRetorno = true;
            }
            else if (strRetornoMsgEstimulo.IndexOf("->") > 0) //MudaPah (primeira linha não é válida, logo terá que ser omitido)
            {
                strMensagemErro = strRetornoMsgEstimulo.Substring(strRetornoMsgEstimulo.IndexOf("->") + 2, strRetornoMsgEstimulo.Length - strRetornoMsgEstimulo.IndexOf("->") - 2);
                elRetornoInvalido = Sam.Common.XmlUtil.lerXml(strRetornoMsgEstimulo, "SIAFDOC/SiafemMudapah/Mudapah/MsgErro");

                if (elRetornoInvalido.InnerText.ToUpperInvariant() == "===>  PARAMETROS ADICIONAIS ATUALIZADOS PARA ESTA SESSAO")
                    strMensagemErro = "Erro no Webservice. Procedimento efetuado com sucesso, porém com retorno SIAFEM/SIAFISICO inesperado (MUDAPAH).";
                else
                    strMensagemErro = String.Format("Erro no Webservice: {0}", elRetornoInvalido.InnerText);

                blnRetorno = true;
            }
            else
            {
                XmlUtil.IsXML(strRetornoMsgEstimulo, ref strMensagemErro);
                strMensagemErro = "Erro processando retorno dos sistemas SIAFEM/SIAFEM. Informar administrador do sistema SAM.";

                blnRetorno = true;

                throw new XmlException(strMensagemErro);
            }

            msgErroRetornoWs = strMensagemErro;
            retornoWsSIAF = strRetornoMsgEstimulo;
            erroProcessamentoEstimulo = blnRetorno;

            return blnRetorno;
        }
        /// <summary>
        /// Função para tratar retornos de acesso ao SIAFEM/SIAFISICO fora do horário comercial
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private bool horarioAcessoInvalido(string strRetornoMsgEstimulo)
        {
            bool blnRetorno = false;
            var docXml = new XmlDocument();
            
            docXml.LoadXml(strRetornoMsgEstimulo);
            if (docXml.IsNotNull())
            {
                nomeMsgEstimuloWs = docXml.GetElementsByTagName("cdMsg").Cast<XmlNode>().FirstOrDefault().NextSibling.Name;
                msgErroRetornoSimplificadoWs = msgErroRetornoWs = (docXml.GetElementsByTagName(nomeTagMsgErro).Cast<XmlNode>().FirstOrDefault() ?? docXml.GetElementsByTagName(nomeTagMsgRetorno).Cast<XmlNode>().FirstOrDefault()).InnerText;
            }

            blnRetorno = (!String.IsNullOrWhiteSpace(msgErroRetornoWs));
            retornoWsSIAF = strRetornoMsgEstimulo;
            erroProcessamentoEstimulo = blnRetorno;

            return blnRetorno;
        }
        /// <summary>
        /// Função para verificar qualquer erro/má-formação de retorno de chamada do método .recebeMsg(string, string, string, string, string, bool)
        /// </summary>
        /// <param name="strRetornoMsgEstimulo"></param>
        /// <param name="strNomeMsgEstimulo"></param>
        /// <param name="strMensagemErro"></param>
        /// <returns></returns>
        private bool verificarErroMensagem(string strRetornoMsgEstimulo)
        {
            #region Variaveis
            bool problemaRetornoSiafem = false;
            bool naoExisteTagStatusOperacao = false;

            string strStatusOperacao = null;
            string strTagErroMsgSEFAZ = null;

            XmlDocument docXml = null;

            string _msgErroRetornoWs = null;
            string erroParsingXmlMsgRetornoWs = null;
            #endregion Variaveis

            if (catalogoInternoMSGsSIAF.IsNull())
                load();

            if (String.IsNullOrWhiteSpace(strRetornoMsgEstimulo))
            {
                msgErroRetornoSimplificadoWs = msgErroRetornoWs = "Interface com sistemas SEFAZ não retornou mensagem";
                erroProcessamentoEstimulo = problemaRetornoSiafem = true;
            }
            else if (!String.IsNullOrWhiteSpace(strRetornoMsgEstimulo))
            {
                //TRATAMENTO PRIMARIO CONTRA STRING NAO-XML RETORNADA POR SIAFEM
                if (retornoSemXml(strRetornoMsgEstimulo, out erroParsingXmlMsgRetornoWs))
                {
                    msgErroRetornoWs = erroParsingXmlMsgRetornoWs;
                    return true;
                }
                else if (!XmlUtil.IsXML(strRetornoMsgEstimulo, ref erroParsingXmlMsgRetornoWs))
                {
                    //ULTIMA TENTATIVA DE LEITURA DO XML ANTES DE DEVOLVER MENSAGEM RELATANDO XML MAL-FORMADO AO USUARIO
                    string _strRetornoMsgEstimuloGeradorEstimuloSIAF = strRetornoMsgEstimulo;
                    _strRetornoMsgEstimuloGeradorEstimuloSIAF = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(_strRetornoMsgEstimuloGeradorEstimuloSIAF);

                    if (XmlUtil.IsXML(_strRetornoMsgEstimuloGeradorEstimuloSIAF))
                    {
                        strRetornoMsgEstimulo = _strRetornoMsgEstimuloGeradorEstimuloSIAF;
                    }
                    else
                    {
                        msgErroRetornoWs = "ERRO AO PROCESSAR RETORNO DA INTEGRACAO SAM-PATRIMONIO/SIAFEM/CONTABILIZA-SP";
                        return true;
                    }
                }


                docXml = new XmlDocument();
                docXml.LoadXml(strRetornoMsgEstimulo);

                nomeMsgEstimuloWs = docXml.GetElementsByTagName("cdMsg").Cast<XmlNode>().FirstOrDefault().NextSibling.Name;
                nomeSistemaSIAF = ((nomeMsgEstimuloWs.Contains("Siafisico") || nomeMsgEstimuloWs.Contains("SFCODOC")) ? "SIAFISICO" : ((nomeMsgEstimuloWs.Contains("SiafemDocNlPatrimonial") || nomeMsgEstimuloWs.Contains("SIAFNlPatrimonial")) ? "CONTABILIZASP" : "SIAFEM"));

                if (tagsErroMsgSIAFEM[nomeMsgEstimuloWs].IsNotNull())
                {
                    strTagErroMsgSEFAZ = tagsErroMsgSIAFEM[nomeMsgEstimuloWs].ToString();
                }
                else
                {
                    var _descErro = String.Format("SAM/SIAF: Erro Processamento Transação {1} (não cadastrada): {0}", nomeMsgEstimuloWs, nomeSistemaSIAF);

                    //TODO: [Douglas Batista] Gerar Trace Universal para a aplicação
                    //LogErro.GravarMsgInfo("Erro Processamanto Retorno VHI/SEFAZ", String.Format("\nMensagem {0} não cadastrada.\n\n{1}", _sistemaSIAF, _descErro));
                    msgErroRetornoWs = _descErro;
                }

                #region Tratamento Erros Diversos
                //Se não há mensagem XML retornada...
                if (!XmlUtil.IsXML(strRetornoMsgEstimulo, ref erroParsingXmlMsgRetornoWs))
                {
                    //TODO: [Douglas Batista] Gerar Trace Universal para a aplicação
                    //LogErro.GravarStackTrace(String.Format("Retorno SIAFEM (erro): {0}, Retorno Estímulo: {1}", strMensagemErro, strRetornoMsgEstimulo));

                    //ULTIMA TENTATIVA DE LEITURA DO XML ANTES DE DEVOLVER MENSAGEM RELATANDO XML MAL-FORMADO AO USUARIO
                    {
                        string _strRetornoMsgEstimuloGeradorEstimuloSIAF = strRetornoMsgEstimulo;
                        _strRetornoMsgEstimuloGeradorEstimuloSIAF = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(_strRetornoMsgEstimuloGeradorEstimuloSIAF);

                        if (XmlUtil.IsXML(_strRetornoMsgEstimuloGeradorEstimuloSIAF))
                        {
                            this.retornoWsSIAF = _strRetornoMsgEstimuloGeradorEstimuloSIAF;
                            return false;
                        }
                    }


                    if (!String.IsNullOrWhiteSpace(erroParsingXmlMsgRetornoWs))
                        msgErroRetornoWs = erroParsingXmlMsgRetornoWs = String.Format("Retorno SIAFEM/SIAFISICO (erro): {0}", "XML mal-formado retornado por SIAFEM.");

                    return true;
                }
                else if (!XmlUtil.IsXML(strRetornoMsgEstimulo))
                {
                    return (retornoInvalido(strRetornoMsgEstimulo, out _msgErroRetornoWs) ||
                            erroAcessoOuConexao(strRetornoMsgEstimulo, out _msgErroRetornoWs));
                }
                else if (strRetornoMsgEstimulo.ToLowerInvariant().Contains("horarioacesso"))
                {
                    return horarioAcessoInvalido(strRetornoMsgEstimulo);
                }
                #endregion Tratamento Erros Diversos

                if (docXml.GetElementsByTagName(nomeTagStatusExecucaoOperacaoSIAF).Cast<XmlNode>().FirstOrDefault().IsNotNull())
                    strStatusOperacao = docXml.GetElementsByTagName(nomeTagStatusExecucaoOperacaoSIAF).Cast<XmlNode>().FirstOrDefault().InnerText;

                naoExisteTagStatusOperacao = !Boolean.TryParse(strStatusOperacao, out problemaRetornoSiafem);

                if (!naoExisteTagStatusOperacao)
                    problemaRetornoSiafem |= !Boolean.Parse(strStatusOperacao);

                if (problemaRetornoSiafem || naoExisteTagStatusOperacao)
                {
                    if (tagsErroMsgSIAFEM.ContainsKey(nomeMsgEstimuloWs) &&
                        tagsErroMsgSIAFEM[nomeMsgEstimuloWs].ToString().IsNotNull() &&
                        (docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().Count() > 0) &&
                        !String.IsNullOrWhiteSpace(docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().FirstOrDefault().InnerText))
                    {
                        if (configuracoesSIAF.IsNotNull() && !configuracoesSIAF.ExibeMsgErroCompleta)
                            hdrRetornoMsgErro = "{0}";

                        msgErroRetornoSimplificadoWs = docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().FirstOrDefault().InnerText;
                        msgErroRetornoWs = String.Format(hdrRetornoMsgErro, docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().FirstOrDefault().InnerText, nomeSistemaSIAF).Replace("-", "").Trim();
                    }
                    //retornou espaco em branco na tag de erro...
                    else if (tagsErroMsgSIAFEM.ContainsKey(nomeMsgEstimuloWs) &&
                             tagsErroMsgSIAFEM[nomeMsgEstimuloWs].ToString().IsNotNull() &&
                             (docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().Count() > 0) &&
                              //...mas a palavra 'null' como valor de NL (tag 'NumeroNL')
                             (String.IsNullOrWhiteSpace(docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().FirstOrDefault().InnerText)) &&
                             ((docXml.GetElementsByTagName("NumeroNL").Cast<XmlNode>().Count() > 0) &&
                              (!String.IsNullOrWhiteSpace(docXml.GetElementsByTagName("NumeroNL").Cast<XmlNode>().FirstOrDefault().InnerText)) && 
                              (docXml.GetElementsByTagName("NumeroNL").Cast<XmlNode>().FirstOrDefault().InnerText.ToLowerInvariant() == "null")))
                    {
                        msgErroRetornoSimplificadoWs = msgErroRetornoWs = "TIME_OUT CONATBILIZA SP";
                    }
                    //retornou espaco em branco na tag de erro...
                    else if (tagsErroMsgSIAFEM.ContainsKey(nomeMsgEstimuloWs) &&
                             tagsErroMsgSIAFEM[nomeMsgEstimuloWs].ToString().IsNotNull() &&
                             (docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().Count() > 0) &&
                             //...e tambem espaco em branco como 'valor' de NL (tag 'NumeroNL')
                             ((docXml.GetElementsByTagName("NumeroNL").Cast<XmlNode>().Count() > 0) && 
                              (String.IsNullOrWhiteSpace(docXml.GetElementsByTagName(strTagErroMsgSEFAZ).Cast<XmlNode>().FirstOrDefault().InnerText) && 
                              (String.IsNullOrWhiteSpace(docXml.GetElementsByTagName("NumeroNL").Cast<XmlNode>().FirstOrDefault().InnerText)))))
                    {
                        msgErroRetornoSimplificadoWs = msgErroRetornoWs = "TIME_OUT CONATBILIZA SP";
                    }


                    // Erro Login
                    if (nomeMsgEstimuloWs.Contains(nomeTagLogin) && String.IsNullOrWhiteSpace(msgErroRetornoWs))
                    {
                        msgErroRetornoWs = null;
                        loginInvalido(strRetornoMsgEstimulo);
                    }

                    //Retorno Tag Vazia
                    if (docXml.GetElementsByTagName(nomeTagFimDocumentoSIAF).IsNotNull() &&
                        docXml.GetElementsByTagName(nomeTagFimDocumentoSIAF).Count == 1 &&
                        !docXml.GetElementsByTagName(nomeTagFimDocumentoSIAF)[0].HasChildNodes)
                    {
                        msgErroRetornoWs = String.Format("Sistema {0} não retornou dados para estímulo enviado [{1}].", nomeSistemaSIAF, nomeMsgEstimuloWs);

                        return true;
                    }

                    if (String.IsNullOrWhiteSpace(msgErroRetornoWs) &&
                        docXml.GetElementsByTagName(nomeTagMsgErro).Cast<XmlNode>().FirstOrDefault().IsNotNull() &&
                        docXml.GetElementsByTagName(nomeTagMsgRetorno).Cast<XmlNode>().FirstOrDefault().IsNotNull())
                    {
                        var capturaMsgErro = (docXml.GetElementsByTagName(nomeTagMsgErro).Cast<XmlNode>().FirstOrDefault() ?? docXml.GetElementsByTagName(nomeTagMsgRetorno).Cast<XmlNode>().FirstOrDefault()).InnerText;
                        msgErroRetornoSimplificadoWs = capturaMsgErro;
                    }

                    problemaRetornoSiafem = (((problemaRetornoSiafem && naoExisteTagStatusOperacao) || !String.IsNullOrWhiteSpace(msgErroRetornoWs)) && ((!String.IsNullOrWhiteSpace(strStatusOperacao) && strStatusOperacao.ToLowerInvariant() == "false" && !String.IsNullOrWhiteSpace(msgErroRetornoWs)) || (String.IsNullOrWhiteSpace(strStatusOperacao) && !String.IsNullOrWhiteSpace(msgErroRetornoWs))));
                }
            }
			//TODO [DCBATISTA] INCLUIR VERIFICACAO DE EXISTENCIA DE NOTA SIAFEM VALIDA
            ////Verificar se estímulo retornado, possui tag que informe número de NL
            //if (docXml.GetElementsByTagName("NumeroNL").Count > 0)
            //    if (!string.IsNullOrEmpty(docXml.GetElementsByTagName("NumeroNL").Cast<XmlNode>().FirstOrDefault().InnerText))
            //        problemaRetornoSiafem = false;
            
            this.retornoWsSIAF = strRetornoMsgEstimulo;
            erroProcessamentoEstimulo = problemaRetornoSiafem;


            return problemaRetornoSiafem;
        }

        /// <summary>
        /// Método de consulta os webservices SEFAZ (SIAFEM/SIAFISICO)
        /// </summary>
        /// <param name="strLoginUsuario"></param>
        /// <param name="strSenhaUsuario"></param>
        /// <param name="strAnoBase"></param>
        /// <param name="strUnidadeGestora"></param>
        /// <param name="strMsgEstimulo"></param>
        /// <param name="isConsulta"></param>
        /// <returns></returns>
        public string ConsumirWS(string chaveSiafUsuario, string senhaSiafUsuario, string anoBaseConsulta, string ugeGestora, string msgEstimulo, bool isConsulta, bool exibeSistemaOrigemErro = false)
        {
            string strRetorno = null;
            string urlConsulta = null;
            string proxyIIS = null;
            bool ambienteDesenvolvimento = false;
            vhiSIAF.RecebeMSG svcWsSiaf = null;

            try
            {
                svcWsSiaf = new vhiSIAF.RecebeMSG();
                urlConsulta = ConfiguracoesSIAF.wsURLConsulta;
                proxyIIS = ConfiguracoesSIAF.enderecoProxy;
                svcWsSiaf.Url = ConfiguracoesSIAF.wsURLConsulta;
                ambienteDesenvolvimento = ConfiguracoesSIAF.ambienteDesenvolvimento;

                load();
                initiateSSLTrust();

                if (isConsulta)
                {
                    ugeGestora = string.Empty;
                    svcWsSiaf.Url = urlConsulta;
                }

                if (!ambienteDesenvolvimento)
                    if (isAmbienteHomologacao)
                        svcWsSiaf.UseDefaultCredentials = true;
                    else if (isAmbienteProducao)
                        svcWsSiaf.Proxy = new WebProxy(proxyIIS); //IIS Proxy.


                if (String.IsNullOrWhiteSpace(chaveSiafUsuario) || String.IsNullOrWhiteSpace(senhaSiafUsuario))
                {
                    msgErroRetornoWs = "Dados de acesso ao sistema SIAFEM não fornecidos!";
                    erroProcessamentoEstimulo = true;
                }


                configuracoesSIAF.ExibeMsgErroCompleta = exibeSistemaOrigemErro;
                strRetorno = svcWsSiaf.Mensagem(chaveSiafUsuario, senhaSiafUsuario, anoBaseConsulta, ugeGestora, msgEstimulo);
                dataEnvioMsg = DateTime.Now;
                strRetorno = strRetorno.RetirarGrandesEspacosEmBranco();
                strRetorno = GeradorEstimuloSIAF.TratarDescricaoXml(strRetorno);

                
                msgEstimuloWsSIAF = msgEstimulo;
                verificarErroMensagem(strRetorno);

                return strRetorno;
            }
            catch (TimeoutException excTimeoutSolicitacao)
            {
                Exception excErroParaPropagacao;
                string erroRetorno;

                erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                excErroParaPropagacao = new Exception(erroRetorno, excTimeoutSolicitacao);

                throw excErroParaPropagacao;
            }
            catch (WebException excErroExecucao)
            {
                Exception excErroParaPropagacao;
                string erroRetorno;

                if (excErroExecucao.Message.Contains(" (404) "))
                    erroRetorno = String.Format(hdrMsgErrosInfra, Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_CONEXAO_SERVIDOR_NAO_ENCONTRADO_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else if (excErroExecucao.Message.Contains(" (504) "))
                    erroRetorno = String.Format(hdrMsgErrosInfra, Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_SOLICITACAO_GATEWAY_NAO_ENCONTRADO_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else if ((excErroExecucao.Message.Contains(Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_EN_US)) || (excErroExecucao.Message.Contains(Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR)))
                    erroRetorno = String.Format(hdrMsgErrosInfra, Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_AMIGAVEL_SERVIDORES_SIAF_INDISPONIVEIS, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else
                    erroRetorno = String.Format("{0}", excErroExecucao.Message);

                excErroParaPropagacao = new Exception(erroRetorno, excErroExecucao);

                throw excErroParaPropagacao;
            }
            catch (InvalidOperationException excErroExecucaoProcedimento)
            {
                //TODO: [Douglas Batista] Gerar Trace Universal para a aplicação
                //new LogErro().GravarLogErro(excErroExecucaoProcedimento);

                return string.Format("{0}\n{1}", Constante.CST_MSG_ERRO_PROCESSAMENTO_OPERACAO, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
            }
            catch (Exception excErroRuntime)
            {
                //return string.Format("Erro: {0}{1}Acionar administrador do sistema!", lExcErroRuntime.Message, Environment.NewLine);
                return string.Format("{0}", excErroRuntime.Message);
            }
        }
        public string ConsumirWS(string anoBaseConsulta, string ugeGestora, string msgEstimulo, bool isConsulta, bool useChaveSiafemDoSAM = false, bool exibeSistemaOrigemErro = false)
        {
            string strRetorno = null;
            string urlConsulta = null;
            string proxyIIS = null;
            bool ambienteDesenvolvimento = false;
            vhiSIAF.RecebeMSG svcWsSiaf = null;

            try
            {
                svcWsSiaf = new vhiSIAF.RecebeMSG();
                configuracoesSIAF = new ConfiguracoesSIAF();
                urlConsulta = ConfiguracoesSIAF.wsURLConsulta;
                proxyIIS = ConfiguracoesSIAF.enderecoProxy;
                svcWsSiaf.Url = ConfiguracoesSIAF.wsURLConsulta;
                ambienteDesenvolvimento = ConfiguracoesSIAF.ambienteDesenvolvimento;


                load();
                initiateSSLTrust();

                if (isConsulta)
                {
                    ugeGestora = string.Empty;
                    svcWsSiaf.Url = urlConsulta;
                }

                if (!ambienteDesenvolvimento)
                    if (isAmbienteHomologacao)
                        svcWsSiaf.UseDefaultCredentials = true;
                    else if (isAmbienteProducao)
                        svcWsSiaf.Proxy = new WebProxy(proxyIIS, true); //IIS Proxy.

                if (useChaveSiafemDoSAM)
                {
                    if (isAmbienteProducao)
                    {
                        this.loginUsuarioSIAF = ConfiguracoesSIAF.userNameConsulta;
                        this.senhaUsuarioSIAF = ConfiguracoesSIAF.passConsulta;
                    }
                    else if (isAmbienteHomologacao && (this.SistemaSIAF == enumSistemaSIAF.SIAFEM))
                    {
                        this.loginUsuarioSIAF = Constante.LoginUsuarioPublicoSiafem;
                        this.senhaUsuarioSIAF = Constante.SenhaUsuarioPublicoSiafem;
                    }
                    else if (isAmbienteHomologacao && (this.SistemaSIAF == enumSistemaSIAF.SIAFISICO))
                    {
                        this.loginUsuarioSIAF = Constante.LoginUsuarioPublicoSiafisico;
                        this.senhaUsuarioSIAF = Constante.SenhaUsuarioPublicoSiafisico; 
                    }
                }

                configuracoesSIAF.ExibeMsgErroCompleta = exibeSistemaOrigemErro;
                strRetorno = svcWsSiaf.Mensagem(this.loginUsuarioSIAF, this.senhaUsuarioSIAF, anoBaseConsulta, ugeGestora, msgEstimulo);
                dataEnvioMsg = DateTime.Now;
                strRetorno = strRetorno.RetirarGrandesEspacosEmBranco();
                strRetorno = GeradorEstimuloSIAF.TratarDescricaoXml(strRetorno);

                msgEstimuloWsSIAF = msgEstimulo;
                verificarErroMensagem(strRetorno);

                return strRetorno;
            }
            catch (TimeoutException excTimeoutSolicitacao)
            {
                Exception excErroParaPropagacao;
                string erroRetorno;

                erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                excErroParaPropagacao = new Exception(erroRetorno, excTimeoutSolicitacao);

                throw excErroParaPropagacao;
            }
            catch (WebException excErroExecucao)
            {
                Exception excErroParaPropagacao;
                string erroRetorno;

                if (excErroExecucao.Message.Contains(" (404) "))
                    erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_CONEXAO_SERVIDOR_NAO_ENCONTRADO_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else if (excErroExecucao.Message.Contains(" (504) "))
                    erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_SOLICITACAO_GATEWAY_NAO_ENCONTRADO_PT_BR, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else if ((excErroExecucao.Message.Contains(Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_EN_US)) || (excErroExecucao.Message.Contains(Constante.CST_MSG_ERRO_SOLICITACAO_POR_TIMEOUT_PT_BR)))
                    erroRetorno = String.Format("{0} ({1}) \n{2}", Constante.CST_MSG_ERRO_AMIGAVEL_SOLICITACAO_CONTATO_SEFAZ, Constante.CST_MSG_ERRO_AMIGAVEL_SERVIDORES_SIAF_INDISPONIVEIS, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
                else
                    erroRetorno = String.Format("{0}", excErroExecucao.Message);

                excErroParaPropagacao = new Exception(erroRetorno, excErroExecucao);

                throw excErroParaPropagacao;
            }
            catch (InvalidOperationException excErroExecucaoProcedimento)
            {
                //TODO: [Douglas Batista] Gerar Trace Universal para a aplicação
                //new LogErro().GravarLogErro(excErroExecucaoProcedimento);

                return string.Format("{0}\n{1}", Constante.CST_MSG_ERRO_PROCESSAMENTO_OPERACAO, Constante.CST_MSG_ERRO_SOLICITACAO_CONTATO_ADMINISTRADOR);
            }
            catch (Exception excErroRuntime)
            {
                return string.Format("{0}", excErroRuntime.Message);
            }
        }

        /// <summary>
        /// Método de consulta os webservices SEFAZ (SIAFEM/SIAFISICO), utilizando nova tecnologia, gerando classe de proxy on-the-fly, sem amanrração hard-coded no componente.
        /// </summary>
        /// <param name="chaveSiafUsuario"></param>
        /// <param name="senhaSiafUsuario"></param>
        /// <param name="anoBaseConsulta"></param>
        /// <param name="ugeGestora"></param>
        /// <param name="msgEstimulo"></param>
        /// <param name="isConsulta"></param>
        /// <param name="exibeSistemaOrigemErro"></param>
        /// <returns></returns>
        public string ConsumirWS(string chaveSiafUsuario, string senhaSiafUsuario, string anoBaseConsulta, string ugeGestora, string msgEstimulo, bool isConsulta, string usaNovoComponente, bool exibeSistemaOrigemErro = false)
        {
            string retornoWS = null;

            if (String.IsNullOrWhiteSpace(usaNovoComponente))
            {
                WsClient clientWS = new WsClient();
                retornoWS = clientWS.Mensagem(chaveSiafUsuario, senhaSiafUsuario, anoBaseConsulta, ugeGestora, msgEstimulo, isConsulta);

                dataEnvioMsg = DateTime.Now;
                retornoWS = retornoWS.RetirarGrandesEspacosEmBranco();
                retornoWS = GeradorEstimuloSIAF.TratarDescricaoXml(retornoWS);

                msgEstimuloWsSIAF = msgEstimulo;

                
                erroProcessamentoEstimulo = !XmlUtil.IsXML(retornoWS);
                if (!erroProcessamentoEstimulo)
                    verificarErroMensagem(retornoWS);

                //msgErroRetornoWs = retornoWS;


                return retornoWS;
            }
            else
            {
                retornoWS = this.ConsumirWS(chaveSiafUsuario, senhaSiafUsuario, anoBaseConsulta, ugeGestora, msgEstimulo, isConsulta, exibeSistemaOrigemErro);
            }

            return retornoWS;
        }

        public string SimulaConsumoWS(string msgEstimulo, bool simulaDevolucaoErroSIAF = false, string msgErroSimulado = null)
        {
            string retornoSimulado = null;

            if (catalogoInternoMSGsSIAF.IsNull())
                load();

            var docXml = new XmlDocument();

            retornoSimulado = SimularRetornoSIAF(msgEstimulo, simulaDevolucaoErroSIAF, msgErroSimulado);

            docXml.LoadXml(retornoSimulado);
            this.nomeMsgEstimuloWs = docXml.GetElementsByTagName("cdMsg").Cast<XmlNode>().FirstOrDefault().NextSibling.Name;
            //this.nomeSistemaSIAF = ((nomeMsgEstimuloWs.Contains("Siafisico") || nomeMsgEstimuloWs.Contains("SFCODOC")) ? "SIAFISICO" : "SIAFEM");
            nomeSistemaSIAF = ((nomeMsgEstimuloWs.Contains("Siafisico") || nomeMsgEstimuloWs.Contains("SFCODOC")) ? "SIAFISICO" : ((nomeMsgEstimuloWs.Contains("SiafemDocNlPatrimonial") || nomeMsgEstimuloWs.Contains("SIAFNlPatrimonial")) ? "CONTABILIZASP" : "SIAFEM"));
            this.dataEnvioMsg = DateTime.Now;
            string nomeTagErroMsgSIAF = tagsErroMsgSIAFEM[nomeMsgEstimuloWs] as string;

            retornoSimulado = retornoSimulado.Replace("NOME_TAG_ERRO", nomeTagErroMsgSIAF);
            this.msgEstimuloWsSIAF = msgEstimulo;
            this.retornoWsSIAF = retornoSimulado;
            this.erroProcessamentoEstimulo = simulaDevolucaoErroSIAF;
            this.msgErroRetornoWs = msgErroSimulado;

            return retornoSimulado;
        }

        public static string SimularRetornoSIAF(string msgEstimulo, bool simulaDevolucaoErroSIAF = false, string msgErroSimulado = null)
        {
                return Simulacra.simulaSIAFEM_SIAFISICO.gerarSIAFNLEmLiq_SIAFNL_OpFlex(msgEstimulo, simulaDevolucaoErroSIAF, msgErroSimulado);
        }

        static internal string tratarErroLoginSistema(string strMensagemErro)
        {
            string strMsgParam = strMensagemErro;

            if (!String.IsNullOrWhiteSpace(strMsgParam) && (strMsgParam.IndexOf("(Senha SIAF") != -1))
            {
                int compMsgErro = strMsgParam.Length;
                int idxMsgSenha = strMsgParam.IndexOf("(Senha SIAF");

                strMsgParam = strMsgParam.Remove(idxMsgSenha - 1, compMsgErro - idxMsgSenha);
                strMsgParam = String.Format("{0}\n{1}", strMensagemErro, "Senha Sistema SAM expirada. Favor informar administrador do sistema!");
            }
            return strMsgParam;
        }
        #endregion
    }
}
