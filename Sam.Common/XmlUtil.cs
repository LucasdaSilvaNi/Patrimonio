using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Sam.Common.Siafem;
using Sam.Common.Util;



namespace Sam.Common
{
    public class XmlUtil
    {
        public static System.Xml.XmlNode lerXml(string DocumentoXML, string Node) 
        {
            // usar no parâmetro Node: "NIVEL1/NIVEL2/NIVEL3..."
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(@"<?xml version=""1.0""?>" + DocumentoXML);
            return doc.SelectSingleNode(Node);
        }

        public static System.Xml.XmlDocument docXml(string DocumentoXML) 
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            doc.LoadXml(@"<?xml version=""1.0""?>" + DocumentoXML);
            return doc;
        }

        public static DateTime converterDataEmpenho(string data) 
        {
            int dia = Convert.ToInt32(data.Substring(0,2));
            string mes = data.Substring(2, 3);

            Dictionary<string, int> meses = new Dictionary<string, int>(12, StringComparer.InvariantCultureIgnoreCase);
            meses.Add("JAN", 1);
            meses.Add("FEV", 2);
            meses.Add("MAR", 3);
            meses.Add("ABR", 4);
            meses.Add("MAI", 5);
            meses.Add("JUN", 6);
            meses.Add("JUL", 7);
            meses.Add("AGO", 8);
            meses.Add("SET", 9);
            meses.Add("OUT", 10);
            meses.Add("NOV", 11);
            meses.Add("DEZ", 12);
            int ano = Convert.ToInt32(data.Substring(5, 4));

            DateTime data1 = new DateTime(ano, meses[mes], dia);
            return data1;
        }

        public static string getXmlValue(string DocumentoXML, string Node)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(@"<?xml version=""1.0""?>" + DocumentoXML);
            System.Xml.XmlNode singleNode = doc.SelectSingleNode(Node);
            
            if (singleNode == null)
                return null;

            return singleNode.InnerText;
        }

        public static string getXmlAttributeValue(string DocumentoXML, string Node, string AttributeName)
        {
            XmlDocument doc = null;
            XmlNode singleNode = null;
            XmlAttribute xmlAttribute = null;



            doc = new XmlDocument();
            doc.LoadXml(@"<?xml version=""1.0""?>" + DocumentoXML);
            singleNode = doc.SelectSingleNode(Node);

            if (singleNode == null)
                return null;

            if (singleNode.Attributes.Cast<XmlAttribute>().ToList().Count(nodeXML => nodeXML.Name == AttributeName) == 1)
                xmlAttribute = singleNode.Attributes.Cast<XmlAttribute>().ToList().FirstOrDefault(nodeXML => nodeXML.Name == AttributeName);

            return xmlAttribute.InnerText;
        }

        public static System.Xml.XmlNodeList lerNodeListXml(string DocumentoXML, string Node)
        {
            // usar no parâmetro Node: "NIVEL1/NIVEL2/NIVEL3..."
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(@"<?xml version=""1.0""?>" + DocumentoXML);
            return doc.SelectNodes(Node);
        }

        public static string AgruparDescricoesItemEmpenho(string xmlVerificar)
        {
            XDocument docXML = null;
            XElement[] arrElem = null;
            XElement elementoXML = null;
            XmlReader xmlReader = null;
            StringReader txtReader = null;

            string strRetorno = null;
            string strPatternTabela = null;
            string strPatternItemMaterial = null;
            string xmlToMove = null;


            try
            {
                strPatternTabela = "RepeteDescricao";
                strPatternItemMaterial = "Descricao";


                docXML = XDocument.Parse(xmlVerificar, LoadOptions.PreserveWhitespace);
                xmlToMove = serializarItensEmpenho(xmlVerificar);
                txtReader = new StringReader(xmlToMove);

                xmlReader = XmlTextReader.Create(txtReader);
                xmlReader.MoveToContent();

                elementoXML = (XNode.ReadFrom(xmlReader) as XElement);
                arrElem = elementoXML.Descendants(strPatternItemMaterial)
                                     .Where(elementoDescricaoItemMaterial => elementoDescricaoItemMaterial.Descendants().Count() > 1)
                                     .ToArray();

                docXML.Descendants(strPatternItemMaterial).ToList().ForEach(node => node.Remove());

                strPatternTabela = String.Format("//{0}", strPatternTabela);
                elementoXML = docXML.XPathSelectElement(strPatternTabela);
                arrElem.ToList().ForEach(descricaoItemMaterial => elementoXML.Add(descricaoItemMaterial));


                strRetorno = XmlUtil.IndentarXml(XDocument.Parse(docXML.ToString(), LoadOptions.PreserveWhitespace).ToString());

                if (String.IsNullOrWhiteSpace(strRetorno))
                    strRetorno = xmlVerificar;

            }
            catch (XmlException xmlErroParsing)
            {
                var descErro = getErroParsingTags(xmlErroParsing.Message);
                throw new XmlException(descErro);
            }

            return strRetorno;
        }

        private static string getErroParsingTags(string excMsgErro)
        {
            string strErroDetalhado = "Erro (genérico) ao efetuar parsing XML";

            //Sample message XmlTagMismatch error: "The 'Oc' start tag on line 33 position 5 does not match the end tag of 'UnidadeOc'. Line 33, position 10."
            if (excMsgErro.Contains("does not match the end tag of"))
            {
                var tagInicial = excMsgErro.Substring(4, (excMsgErro.IndexOf(" start tag") - 4));
                var tagFinal = excMsgErro.Substring((excMsgErro.IndexOf("end tag of ") + 11), ((excMsgErro.IndexOf("'. Line") - (excMsgErro.IndexOf("end tag of ") + 10))));

                strErroDetalhado = String.Format("XML mal-formado (Erro fechamento entre tags {0} e {1}).", tagInicial, tagFinal);
            }

            return strErroDetalhado;
        }

        private static string serializarItensEmpenho(string xmlVerificar)
        {
            XDocument docXML = null;
            ItemEmpenhoRepete itemAux = null;
            Dictionary<string, ItemEmpenhoRepete> dicValores = null;

            int contadorItens = 1;
            string chaveInsert = null;
            string xmlPatternTabela = "RepeteDescricao";
            string xmlPatternItemTabela = "Descricao";
            string xmlRetorno = null;

            string tagSequencia = "Seq";
            string tagCodigoItemMaterial = "Item";
            string tagQuantidadeItemMaterial = "Quantidade";
            string tagSaldoItemMaterial = "Saldo";
            string tagUnidadeFornecimento = "UF";
            string tagPrecoUnitarioItemMaterial = "ValorUnitario";
            //string tagDescricaoItemMaterial = "Descricao";
            string tagDescricaoItemMaterial = "DescricaoItem";
            string tagProdutorPPais = "ProdutorPPais";
            string tagItMeEpp = "ItMeEpp";
            ItemEmpenhoRepete itemEmpenho = null;
            IList<ItemEmpenhoRepete> lstItensEmpenho = null;



            dicValores = new Dictionary<string, ItemEmpenhoRepete>();
            docXML = XDocument.Parse(xmlVerificar, LoadOptions.PreserveWhitespace);

            lstItensEmpenho = new List<ItemEmpenhoRepete>();

            IList<XElement> lst = docXML.Descendants(xmlPatternTabela)
                                        .Descendants(xmlPatternItemTabela)
                                        .Cast<XElement>().ToList();

            docXML.Descendants(xmlPatternTabela)
                  .Descendants(xmlPatternItemTabela)
                  .Cast<XElement>().ToList()
                  .ForEach(elementoItemEmpenho =>
                  {
                      if (elementoItemEmpenho.Descendants().Count() > 1)
                      {
                          itemEmpenho = new ItemEmpenhoRepete()
                          {
                              NumeroSequencia = elementoItemEmpenho.Element(tagSequencia).Value,
                              NumeroItem = elementoItemEmpenho.Element(tagSequencia).Value,
                              CodigoItemMaterial = elementoItemEmpenho.Element(tagCodigoItemMaterial).Value,
                              CodigoUnidadeFornecimento = elementoItemEmpenho.Element(tagUnidadeFornecimento).Value,
                              QtdeItemMaterial = elementoItemEmpenho.Element(tagQuantidadeItemMaterial).Value,
                              SaldoItemMaterial = elementoItemEmpenho.Element(tagSaldoItemMaterial).Value,
                              ValorUnitarioItemMaterial = elementoItemEmpenho.Element(tagPrecoUnitarioItemMaterial).Value,
                              Descricao = elementoItemEmpenho.Element(tagDescricaoItemMaterial).Value,
                              //Produtor = elementoItemEmpenho.Element(tagProdutorPPais).Value,
                              ProdutorPPais = elementoItemEmpenho.Element(tagProdutorPPais).Value,
                              ItMeEpp = elementoItemEmpenho.Element(tagItMeEpp).Value
                          };

                          lstItensEmpenho.Add(itemEmpenho);
                      }
                  });

            lstItensEmpenho.ToList()
                           .ForEach(output =>
                           {
                               //chaveInsert = String.Format("ItemMaterialCodigo{0}__UF{1}_Produtor{2}", output.CodigoItemMaterial, output.CodigoUnidadeFornecimento, output.Produtor);
                               chaveInsert = String.Format("ItemMaterialCodigo{0}__UF{1}_ProdutorPPais{2}_ItMeEpp{3}", output.CodigoItemMaterial, output.CodigoUnidadeFornecimento, (String.IsNullOrWhiteSpace(output.ProdutorPPais) ? "0" : output.ProdutorPPais), (String.IsNullOrWhiteSpace(output.ItMeEpp) ? "0" : output.ItMeEpp));
                               if (dicValores.ContainsKey(chaveInsert))
                               {
                                   itemAux = dicValores[chaveInsert];

                                   if ((itemAux.CodigoItemMaterial == output.CodigoItemMaterial) && (itemAux.CodigoUnidadeFornecimento == output.CodigoUnidadeFornecimento))
                                   {
                                       itemAux.Descricao = (String.IsNullOrWhiteSpace(itemAux.Descricao)) ? output.Descricao : String.Format("{0} {1}", itemAux.Descricao, output.Descricao);
                                       itemAux.Descricao1 = (String.IsNullOrWhiteSpace(itemAux.Descricao1)) ? output.Descricao1 : String.Format("{0} {1}", itemAux.Descricao1, output.Descricao1);
                                       itemAux.Descricao2 = (String.IsNullOrWhiteSpace(itemAux.Descricao2)) ? output.Descricao2 : String.Format("{0} {1}", itemAux.Descricao2, output.Descricao2);
                                       itemAux.Descricao3 = (String.IsNullOrWhiteSpace(itemAux.Descricao3)) ? output.Descricao3 : String.Format("{0} {1}", itemAux.Descricao3, output.Descricao3);


                                       itemAux.Descricao = (!String.IsNullOrWhiteSpace(itemAux.Descricao) ? itemAux.Descricao.Replace("  ", "") : itemAux.Descricao);
                                       itemAux.Descricao1 = (!String.IsNullOrWhiteSpace(itemAux.Descricao) ? itemAux.Descricao.Replace("  ", "") : itemAux.Descricao);
                                       itemAux.Descricao2 = (!String.IsNullOrWhiteSpace(itemAux.Descricao) ? itemAux.Descricao.Replace("  ", "") : itemAux.Descricao);
                                       itemAux.Descricao3 = (!String.IsNullOrWhiteSpace(itemAux.Descricao) ? itemAux.Descricao.Replace("  ", "") : itemAux.Descricao);

                                       output = itemAux;
                                   }
                               }

                               //chaveInsert = String.Format("ItemMaterialCodigo{0}__UF{1}_Produtor{2}_ItMeEpp{3}", output.CodigoItemMaterial, output.CodigoUnidadeFornecimento, output.Produtor, output.ItMeEpp);
                               chaveInsert = String.Format("ItemMaterialCodigo{0}__UF{1}_ProdutorPPais{2}_ItMeEpp{3}", output.CodigoItemMaterial, output.CodigoUnidadeFornecimento, (String.IsNullOrWhiteSpace(output.ProdutorPPais) ? "0" : output.ProdutorPPais), (String.IsNullOrWhiteSpace(output.ItMeEpp) ? "0" : output.ItMeEpp));
                               dicValores.InserirValor(chaveInsert, output);
                           });

            xmlRetorno = "<RepeteDescricao>";
            foreach (var item in dicValores.Values)
            {
                item.NumeroSequencia = String.Format("{0:000}", contadorItens);
                xmlRetorno += item.ToXML();

                contadorItens++;
            }

            xmlRetorno += "</RepeteDescricao>";

            return xmlRetorno;
        }

        public static bool IsXML(string documentoXML)
        {
            string strMsgErro = "";
            return IsXML(documentoXML, ref strMsgErro);
        }

        public static bool IsXML(string documentoXML, ref string strErroParsingXml)
        {
            try
            {
                XmlDocument mXmlDoc = new XmlDocument();
                mXmlDoc.LoadXml(documentoXML);

                mXmlDoc = null;

                return true;
            }
            catch (XmlException excErroParsingXml)
            {
                strErroParsingXml = excErroParsingXml.Message;
                return false;
            }
            catch (Exception excErroGenerico)
            {
                strErroParsingXml = excErroGenerico.Message;
                return false;
            }
        }

        public static string IndentarXml(string strXmlDocument)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlDocument doc = new XmlDocument();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.LoadXml(strXmlDocument);
                doc.Save(writer);
            }
            return sb.ToString();
        }
    }
}
