using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using Sam.Common.Util;
using System.Collections;
using System.Globalization;
using System.EnterpriseServices;
using Sam.Integracao.SIAF.Mensagem.Enum;
using Sam.Integracao.SIAF.Mensagem.Interface;
using TipoMovimentacaoPatrimonial = Sam.Integracao.SIAF.Mensagem.Enum.EnumMovimentType;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;





namespace Sam.Integracao.SIAF.Core
{
    public static class GeradorEstimuloSIAF
    {
        static StringBuilder sbXml = new StringBuilder();
        static XmlWriter xmlMontadorEstimulo = null;
        static XmlWriterSettings xmlSettings = null;
        public const int CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_CONTABILIZASP = 231;
        public const int CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN__SAM_PATRIMONIO = 169;


        private static void Init()
        {
            sbXml = null;
            xmlMontadorEstimulo = null;
            xmlSettings = null;

            sbXml = new StringBuilder();
            xmlSettings = GeradorEstimuloSIAF.ObterFormatacaoPadraoParaXml();
            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
        }
        private static void Dispose()
        {
            sbXml = null;
            xmlMontadorEstimulo = null;
            xmlSettings = null;
        }

        #region Consulta Cadastros SIAFISICO

        public static string SiafisicoDocConsultaF(long cpfCnpjFornecedor)
        {
            string strRetorno = null;

            
            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOConsultaF");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoDocConsultaF");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CgcCpf", cpfCnpjFornecedor.ToString());
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();

            return strRetorno;
        }
        public static string SiafisicoDocConsultaI(string strCodigoItemMaterial)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOConsultaI");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoDocConsultaI");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CodigoItem", strCodigoItemMaterial);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();

            return strRetorno;
        }

        #endregion

        #region Notas Lancamento (SIAFEM)

        public static string SiafemDocNLPatrimonial(ISIAFNlPatrimonial implMsgWsIntegracaoContabilizaSP, bool isEstorno = false)
        {
            string strRetorno = null;
            string _cpfCnpjUgeFavorecida = null;
            string _gestaoFavorecida = null;
            string tipoNotaContabilizaSP = null;
            string acaoPagamentoNotaLancamento = null;
            string tipoLancamento = null;
            string dataMovimentacao = null;
            string observacaoMovimentacao = null;
            string quebraDeLinha = null;
            int codigoUG = 0;
            int codigoGestao = 0;



            #region PRE-LOAD - Chaves para MONITORASIAF
            switch ((EnumAccountEntryType.AccountEntryType)implMsgWsIntegracaoContabilizaSP.AccountEntryTypeId)
            {
                case EnumAccountEntryType.AccountEntryType.Entrada:         { tipoNotaContabilizaSP = "E"; }; break;
                case EnumAccountEntryType.AccountEntryType.Saida:           { tipoNotaContabilizaSP = "S"; }; break;
                case EnumAccountEntryType.AccountEntryType.Depreciacao:     { tipoNotaContabilizaSP = "D"; }; break;
                case EnumAccountEntryType.AccountEntryType.Reclassificacao: { tipoNotaContabilizaSP = "R"; }; break;
            }
            tipoLancamento = ((isEstorno) ? "S" : "N");
            acaoPagamentoNotaLancamento = ((isEstorno) ? "E" : "N");
            #endregion PRE-LOAD - Chaves para MONITORASIAF

            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("documento");

            #region Chave para MONITORASIAF
            //Monitoramento, via WS SIAF SIAFMONITORA
            if (implMsgWsIntegracaoContabilizaSP.RelacaoMovimentacoesPatrimonaisId.HasElements())
                if (implMsgWsIntegracaoContabilizaSP.RelacaoMovimentacoesPatrimonaisId.Count() == 1)
                    xmlMontadorEstimulo.WriteAttributeString("id", String.Format("{0}{1}_tokenSAMPatrimonio:{2}", tipoNotaContabilizaSP, acaoPagamentoNotaLancamento, implMsgWsIntegracaoContabilizaSP.TokenGeradoEnvioContabilizaSP));
                else
                    xmlMontadorEstimulo.WriteAttributeString("id", String.Format("{0}{1}_tokenSAMPatrimonio:{2}", tipoNotaContabilizaSP, acaoPagamentoNotaLancamento, implMsgWsIntegracaoContabilizaSP.TokenGeradoEnvioContabilizaSP));
            #endregion Chave para MONITORASIAF

            dataMovimentacao = implMsgWsIntegracaoContabilizaSP.MovimentDate.ToString("dd/MM/yyyy");
            codigoGestao = implMsgWsIntegracaoContabilizaSP.ManagerCode;
            codigoUG = implMsgWsIntegracaoContabilizaSP.ManagerUnitCode;
            xmlMontadorEstimulo.WriteFullElementString("TipoMovimento", implMsgWsIntegracaoContabilizaSP.AccountEntryType);
            xmlMontadorEstimulo.WriteFullElementString("Data", dataMovimentacao);
            xmlMontadorEstimulo.WriteFullElementString("UgeOrigem", codigoUG == 0 ? string.Empty : codigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteFullElementString("Gestao", codigoGestao == 0 ? string.Empty : codigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteFullElementString("Tipo_Entrada_Saida_Reclassificacao_Depreciacao", implMsgWsIntegracaoContabilizaSP.InputOutputReclassificationDepreciationType);

            if (implMsgWsIntegracaoContabilizaSP.FavoreceDestino)
            {
                _cpfCnpjUgeFavorecida = implMsgWsIntegracaoContabilizaSP.CpfCnpjUgeFavorecido;
                _gestaoFavorecida = implMsgWsIntegracaoContabilizaSP.GestaoFavorecida;
            }

            xmlMontadorEstimulo.WriteFullElementString("CpfCnpjUgeFavorecida", _cpfCnpjUgeFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("GestaoFavorecida", _gestaoFavorecida);


            //VERIFICAR REGRA PARA MULTIPLOS BPs E UMA UNICA INCORPORACAO
            xmlMontadorEstimulo.WriteFullElementString("Item", implMsgWsIntegracaoContabilizaSP.RelacaoItemMaterial.FirstOrDefault().Key);

            xmlMontadorEstimulo.WriteFullElementString("TipoEstoque", implMsgWsIntegracaoContabilizaSP.StockType);
            xmlMontadorEstimulo.WriteFullElementString("Estoque", implMsgWsIntegracaoContabilizaSP.StockDescription);
            xmlMontadorEstimulo.WriteFullElementString("EstoqueDestino", implMsgWsIntegracaoContabilizaSP.StockDestination);
            xmlMontadorEstimulo.WriteFullElementString("EstoqueOrigem", implMsgWsIntegracaoContabilizaSP.StockSource);
            xmlMontadorEstimulo.WriteFullElementString("TipoMovimentacao", implMsgWsIntegracaoContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP);

            //VALOR MOVIMENTACAO
            string valorDoc = implMsgWsIntegracaoContabilizaSP.AccountingValueField.ToString("0,0.00");


            xmlMontadorEstimulo.WriteFullElementString("ValorTotal", valorDoc);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecifico", implMsgWsIntegracaoContabilizaSP.SpecificControl);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoEntrada", implMsgWsIntegracaoContabilizaSP.SpecificInputControl);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoSaida", implMsgWsIntegracaoContabilizaSP.SpecificOutputControl);
            xmlMontadorEstimulo.WriteFullElementString("FonteRecurso", "");
            xmlMontadorEstimulo.WriteFullElementString("NLEstorno", tipoLancamento);
            xmlMontadorEstimulo.WriteFullElementString("Empenho", implMsgWsIntegracaoContabilizaSP.CommitmentNumber);


            quebraDeLinha = Environment.NewLine;
            string tokenSamPatrimonio = String.Format("'Token SAM-Patrimonio': '{0}'", implMsgWsIntegracaoContabilizaSP.TokenGeradoEnvioContabilizaSP.ToString());
            string descricaoTipoMovimentacao = String.Format("'{0}':'{1}'", implMsgWsIntegracaoContabilizaSP.AccountEntryType, implMsgWsIntegracaoContabilizaSP.InputOutputReclassificationDepreciationType);
            string documentoSAM = String.Format("'DocumentoSAM': '{0}'", implMsgWsIntegracaoContabilizaSP.DocumentNumberSAM);
            //observacaoMovimentacao = String.Format("{0}{1}{2}", observacaoMovimentacao, quebraDeLinha, tokenSamPatrimonio);
            observacaoMovimentacao = String.Format("{1};{0}{2};", quebraDeLinha, descricaoTipoMovimentacao, documentoSAM);
            int tamanhoString = ((observacaoMovimentacao.Length > GeradorEstimuloSIAF.CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN__SAM_PATRIMONIO) ? GeradorEstimuloSIAF.CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN__SAM_PATRIMONIO : observacaoMovimentacao.Length);
            observacaoMovimentacao = observacaoMovimentacao.Substring(0, tamanhoString);
            observacaoMovimentacao = String.Format("{0}{1}{2}.", observacaoMovimentacao, quebraDeLinha, tokenSamPatrimonio);
            xmlMontadorEstimulo.WriteFullElementString("Observacao", observacaoMovimentacao);



            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO NOTA FISCAL
            xmlMontadorEstimulo.WriteStartElement("NotaFiscal");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("NF");
            xmlMontadorEstimulo.WriteFullElementString("NotaFiscal", implMsgWsIntegracaoContabilizaSP.DocumentNumberSAM);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO ITEM
            xmlMontadorEstimulo.WriteStartElement("ItemMaterial");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");

            //VERIFICAR REGRA
            foreach (var itemMaterial in implMsgWsIntegracaoContabilizaSP.RelacaoItemMaterial.Keys)
            {
                xmlMontadorEstimulo.WriteStartElement("IM");
                xmlMontadorEstimulo.WriteFullElementString("ItemMaterial", itemMaterial);
                xmlMontadorEstimulo.WriteEndElement();
            }

            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }

        #endregion

        #region Consultas Empenhos

        public static string SiafemDocDetaContaGen(int iCodigoUG, int iCodigoGestao, string strMes, string strContaContabil, string strOpcao)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFDetaContaGen");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocDetaContaGen");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CodigoUG", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("Mes", strMes);
            xmlMontadorEstimulo.WriteElementString("ContaContabil", strContaContabil);
            xmlMontadorEstimulo.WriteElementString("ContaCorrente", null);
            xmlMontadorEstimulo.WriteElementString("Opcao", strOpcao);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();


            return strRetorno;
        }
        public static string SiafemDocConsultaEmpenhos(int iCodigoUG, int iCodigoGestao, string strNumeroEmpenho)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFConsultaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocConsultaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("NumeroNe", strNumeroEmpenho);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string SiafemDocListaEmpenhos(string strCpfCnpj, int iCodigoGestao, int iAnoBase, int iCodigoUG)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("CgcCpf", strCpfCnpj);
            xmlMontadorEstimulo.WriteElementString("Data", null);
            xmlMontadorEstimulo.WriteElementString("Fonte", null);
            xmlMontadorEstimulo.WriteElementString("Gestao", iCodigoGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("GestaoCredor", null);
            xmlMontadorEstimulo.WriteElementString("Licitacao", null);
            xmlMontadorEstimulo.WriteElementString("ModalidadeEmpenho", null);
            xmlMontadorEstimulo.WriteElementString("Natureza", null);
            xmlMontadorEstimulo.WriteElementString("NumeroNe", null);
            xmlMontadorEstimulo.WriteElementString("Prefixo", String.Format("{0}NE", iAnoBase.ToString("D4")));
            xmlMontadorEstimulo.WriteElementString("Processo", null);
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", iCodigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string SiafemDocListaEmpenhos(SortedList parametrosConsulta)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocListaEmpenhos");
            xmlMontadorEstimulo.WriteStartElement("documento");

            foreach (var parametroConsulta in parametrosConsulta)
                xmlMontadorEstimulo.WriteElementString(parametroConsulta.ToString(), parametrosConsulta[parametroConsulta].ToString());

            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }



        [Description("Metodo sob homologacao")]
        public static string SiafisicoConPrecoNE(int codigoUG, int codigGestao, string numeroEmpenho)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOConPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoConPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", codigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", codigGestao.ToString("D5"));

            if (numeroEmpenho.Contains("NE"))
                numeroEmpenho = numeroEmpenho.Split(new string[] { "NE" }, StringSplitOptions.RemoveEmptyEntries)[1];

            xmlMontadorEstimulo.WriteElementString("NumNE", numeroEmpenho);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        [Description("Metodo sob homologacao")]
        public static string SiafisicoDocListaPrecoNE(int codigoUG, int codigGestao, string numeroEmpenho)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SFCODOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SFCOListaPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("SiafisicoDocListaPrecoNE");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("UnidadeGestora", codigoUG.ToString("D6"));
            xmlMontadorEstimulo.WriteElementString("Gestao", codigGestao.ToString("D5"));
            xmlMontadorEstimulo.WriteElementString("NumeroNe", numeroEmpenho);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        #endregion

        #region Monitoramento
        public static string SiafMonitora(string strChaveConsulta)
        {
            string strRetorno = null;


            GeradorEstimuloSIAF.Init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteElementString("cdMsg", "SIAFMONITORA");
            xmlMontadorEstimulo.WriteStartElement("SiafMonitora");
            xmlMontadorEstimulo.WriteStartElement("documento");
            xmlMontadorEstimulo.WriteElementString("id", strChaveConsulta);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            //Descarrega o conteudo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            GeradorEstimuloSIAF.Dispose();

            return strRetorno;
        }
        #endregion

        #region Metodos Auxiliares (XML)
        /// <summary>
        /// Formatacao padrao para geracao de estimulos para os webservices SIAF.
        /// </summary>
        /// <returns></returns>
        private static XmlWriterSettings ObterFormatacaoPadraoParaXml()
        {
            return new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
        }

        /// <summary>
        /// Função para tratamento de caracteres inválidos, na mensagem XML retornada pelos sistemas SIAFEM/SIAFISICO
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string TratarDescricaoXml(string xml)
        {
            //if (xml.Contains("<descricao>"))
            if ((xml.ToLowerInvariant().Contains("<descricao>")) || (xml.ToLowerInvariant().Contains("<tabela>")) || (xml.ToLowerInvariant().Contains("<documento>")))
            {
                xml = xml.Replace(" <=", " &lt;=");
                xml = xml.Replace(" <,", " &lt;,");
                xml = xml.Replace(" >=", " &gt;=");
                xml = xml.Replace(" >,", " &gt;,");
                xml = xml.Replace(" < ", " &lt; ");
                xml = xml.Replace(" > ", " &gt; ");
                xml = xml.Replace(" & ", " &amp; ");
                xml = xml.Replace("&", "&amp;");
                xml = xml.Replace(@" "" ", " &quot; ");
                xml = xml.Replace("'", "&#39");
                xml = xml.Replace("º", "&#186");

                for (int iContador = 0; iContador < 9; iContador++)
                {
                    if (xml.IndexOf(String.Format(" <{0}", iContador)) != -1)
                        xml = xml.Replace(String.Format(" <{0}", iContador), String.Format(" &lt;{0}", iContador));
                }
            }

            return xml;
        }

        /// <summary>
        /// Sistema para reverter a ação efetuada pela função .TratarDescricaoXml(string)
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string ReverterTratarDescricaoXml(string xml)
        {
            //Remove os sinais de  < e >, ocorre exception ao montar o xmlDoc
            xml = xml.Replace(" &lt#61;", " <=");
            xml = xml.Replace(" &gt#61;", " >=");
            xml = xml.Replace(" &lt; ", " < ");
            xml = xml.Replace(" &gt; ", " > ");
            xml = xml.Replace(" &amp; ", " & ");
            xml = xml.Replace("&amp;", "&");
            xml = xml.Replace(" &quot; ", @" "" ");
            xml = xml.Replace("&#39", "'");
            xml = xml.Replace("&#39;", "'");
            xml = xml.Replace(" &lt#61;,", " <,");
            xml = xml.Replace(" &gt#61;,", " >,");
            xml = xml.Replace("&#186", "º");
            xml = xml.Replace("&#186;", "º");

            return xml;
        }
        #endregion
    }

    public static class xmlWriterExtensionMethods
    {
        [System.Diagnostics.DebuggerStepThrough]
        [System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static void WriteFullElementString(this XmlWriter xmlWriter, string localName, string value)
        {
            value = (value.IsNull() ? string.Empty : value);

            xmlWriter.WriteStartElement(localName);
            xmlWriter.WriteValue(value);
            xmlWriter.WriteEndElement();
        }
    }

    public class ImplSIAFNlPatrimonial : ISIAFNlPatrimonial
    {
        #region Listagem Auxiliar
        private static TipoMovimentacaoPatrimonial[] tiposDeMovimentacaoQueFavorecemDestino = new TipoMovimentacaoPatrimonial[] {
                                                                                                                                      TipoMovimentacaoPatrimonial.IncorpDoacaoConsolidacao
                                                                                                                                    , TipoMovimentacaoPatrimonial.IncorpDoacaoIntraNoEstado
                                                                                                                                    , TipoMovimentacaoPatrimonial.IncorpDoacaoMunicipio
                                                                                                                                    , TipoMovimentacaoPatrimonial.IncorpDoacaoOutrosEstados
                                                                                                                                    , TipoMovimentacaoPatrimonial.IncorpDoacaoUniao
                                                                                                                                    , TipoMovimentacaoPatrimonial.IncorpTransferenciaMesmoOrgaoPatrimoniado
                                                                                                                                    , TipoMovimentacaoPatrimonial.IncorpTransferenciaOutroOrgaoPatrimoniado
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovDoacaoConsolidacao
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovDoacaoIntraNoEstado
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovDoacaoMunicipio
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovDoacaoOutrosEstados
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovDoacaoUniao
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovTransferenciaMesmoOrgaoPatrimoniado
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovTransferenciaOutroOrgaoPatrimoniado
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovSaidaInservivelUGEDoacao
                                                                                                                                    , TipoMovimentacaoPatrimonial.MovSaidaInservivelUGETransferencia
                                                                                                                                };
        #endregion Listagem Auxiliar

        private bool _favoreceDestino = false;
        public Int32 AccountEntryTypeId { get; set; }
        public string AccountEntryType { get; set; }
        public string CommitmentNumber { get; set; }
        public string InputOutputReclassificationDepreciationType { get; set; }
        public Int32 InputOutputReclassificationDepreciationTypeCode { get; set; }
        public string DescricaoTipoMovimento_SamPatrimonio { get; set; }
        public string DescricaoTipoMovimentacao_ContabilizaSP { get; set; }
        public string MaterialType { get; set; }
        //public string[] ObservacoesMovimentacao { get; set; }
        public string Observacao { get; set; }
        public string SpecificControl { get; set; }
        public string SpecificInputControl { get; set; }
        public string SpecificOutputControl { get; set; }
        public bool Status { get; set; }
        public string StockDescription { get; set; }
        public string StockDestination { get; set; }
        public string StockSource { get; set; }
        public string StockType { get; set; }

        public int MetaDataType_AccountingValueField { get; set; }
        public int MetaDataType_DateField { get; set; }
        public int MetaDataType_StockSource { get; set; }
        public int MetaDataType_StockDestination { get; set; }
        public int MetaDataType_MovementTypeContabilizaSP { get; set; }
        public int MetaDataType_SpecificControl { get; set; }
        public int MetaDataType_SpecificInputControl { get; set; }
        public int MetaDataType_SpecificOutputControl { get; set; }
        public bool FavoreceDestino {
                                        get
                                        {
                                                bool favoreceDestino = false;

                                                if (InputOutputReclassificationDepreciationTypeCode != 0)
                                                    favoreceDestino = (tiposDeMovimentacaoQueFavorecemDestino.ToList()
                                                                                                             .Select(tipoMovPatrimonial => tipoMovPatrimonial.GetHashCode())
                                                                                                             .Contains(InputOutputReclassificationDepreciationTypeCode));

                                            return (favoreceDestino || _favoreceDestino);
                                        }
                                        set
                                        {
                                            _favoreceDestino = value;
                                          }
                                    }

        public string GestaoFavorecida { get; set; }
        public string CpfCnpjUgeFavorecido { get; set; }

        public DateTime MovimentDate { get; set; }
        public int ManagerUnitCode { get; set; }
        public int ManagerCode { get; set; }
        public decimal AccountingValueField { get; set; }
        public long MaterialItemCode { get; set; }
        public IDictionary<string, long> RelacaoItemMaterial { get; set; }
        public string DocumentNumberSAM { get; set; }
        public bool EhEstorno { get; set; }


        public short ExecutionOrder { get; set; }
        public IList<long> RelacaoMovimentacoesPatrimonaisId { get; set; }
        public Guid TokenGeradoEnvioContabilizaSP { get; set; }
        public string TokenEnvioContabilizaSP { get; set; }
    }
}
