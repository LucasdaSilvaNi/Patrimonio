using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Common.Siafem
{
    #region POCOs Auxiliares

    public sealed class ItemEmpenhoRepete
    {
        public string NumeroSequencia;
        public string NumeroItem;
        public string CodigoItemMaterial;
        public string CodigoUnidadeFornecimento;
        public string QtdeItemMaterial;
        public string SaldoItemMaterial;
        public string ValorUnitarioItemMaterial;
        public string PrecoTotal;
        public string Descricao;
        public string Descricao1;
        public string Descricao2;
        public string Descricao3;
        public string ProdutorPPais;
        public string ItMeEpp;
    }


    public static class SiafemXmlHelper
    {
        public static string ToXML(this ItemEmpenhoRepete itemEmpenhoParaXml, bool restosAPagar = false)
        {
            string strRetorno = null;

            StringBuilder sbMensagemEstimulo = null;
            System.Xml.XmlWriter xmlMontadorEstimulo = null;
            System.Xml.XmlWriterSettings xmlSettings = null;
            

            xmlSettings          = new System.Xml.XmlWriterSettings();
            xmlSettings.Indent   = true;
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.OmitXmlDeclaration = true;
            xmlSettings.ConformanceLevel = System.Xml.ConformanceLevel.Fragment;
            
            sbMensagemEstimulo  = new StringBuilder();
            xmlMontadorEstimulo = System.Xml.XmlWriter.Create(sbMensagemEstimulo, xmlSettings);

            xmlMontadorEstimulo.WriteStartElement("tabela");
            xmlMontadorEstimulo.WriteElementString("sequencia", itemEmpenhoParaXml.NumeroSequencia);
            
            xmlMontadorEstimulo.WriteElementString("item", itemEmpenhoParaXml.NumeroItem);
            xmlMontadorEstimulo.WriteElementString("material", itemEmpenhoParaXml.CodigoItemMaterial);
            xmlMontadorEstimulo.WriteElementString("unidade", itemEmpenhoParaXml.CodigoUnidadeFornecimento);
            xmlMontadorEstimulo.WriteElementString("qtdeitem", itemEmpenhoParaXml.QtdeItemMaterial);
            xmlMontadorEstimulo.WriteElementString("valorunitario", itemEmpenhoParaXml.ValorUnitarioItemMaterial);
            xmlMontadorEstimulo.WriteElementString("precototal", itemEmpenhoParaXml.PrecoTotal);
            //xmlMontadorEstimulo.WriteElementString("descricao", itemEmpenhoParaXml.Descricao);
            xmlMontadorEstimulo.WriteElementString("descricaoitem", itemEmpenhoParaXml.Descricao);
            xmlMontadorEstimulo.WriteElementString("descricao1", itemEmpenhoParaXml.Descricao1);
            xmlMontadorEstimulo.WriteElementString("descricao2", itemEmpenhoParaXml.Descricao2);
            xmlMontadorEstimulo.WriteElementString("descricao3", itemEmpenhoParaXml.Descricao3);

            xmlMontadorEstimulo.WriteEndElement();

            //Descarrega o conteúdo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbMensagemEstimulo.ToString();

            return strRetorno;
        }
        public static string ToXML(this ItemEmpenhoRepete itemEmpenhoParaXml)
        {
            string tagInicioItemDetalhe = "Descricao";
            string tagSequencia = "Seq";
            string tagCodigoItemMaterial = "Item";
            string tagQuantidadeItem = "Quantidade";
            string tagSaldoItemMaterial = "Saldo";
            string tagUnidadeFornecimento = "UF";
            string tagPrecoUnitario = "ValorUnitario";
            //string tagDescricaoItemMaterial = "Descricao";
            //string tagProdutor = "Produtor";
            string tagDescricaoItemMaterial = "DescricaoItem";
            string tagProdutorPPais = "ProdutorPPais";
            string tagItMeEpp = "ItMeEpp";
 
            string strRetorno = null;

            StringBuilder sbMensagemEstimulo = null;
            System.Xml.XmlWriter xmlMontadorEstimulo = null;
            System.Xml.XmlWriterSettings xmlSettings = null;


            xmlSettings = new System.Xml.XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.OmitXmlDeclaration = true;
            xmlSettings.ConformanceLevel = System.Xml.ConformanceLevel.Fragment;

            sbMensagemEstimulo = new StringBuilder();
            xmlMontadorEstimulo = System.Xml.XmlWriter.Create(sbMensagemEstimulo, xmlSettings);

            xmlMontadorEstimulo.WriteStartElement(tagInicioItemDetalhe);
            xmlMontadorEstimulo.WriteElementString(tagSequencia, itemEmpenhoParaXml.NumeroSequencia);
            xmlMontadorEstimulo.WriteElementString(tagCodigoItemMaterial, itemEmpenhoParaXml.CodigoItemMaterial);
            xmlMontadorEstimulo.WriteElementString(tagQuantidadeItem, itemEmpenhoParaXml.QtdeItemMaterial);
            xmlMontadorEstimulo.WriteElementString(tagSaldoItemMaterial, itemEmpenhoParaXml.SaldoItemMaterial);
            xmlMontadorEstimulo.WriteElementString(tagUnidadeFornecimento, itemEmpenhoParaXml.CodigoUnidadeFornecimento);
            xmlMontadorEstimulo.WriteElementString(tagPrecoUnitario, itemEmpenhoParaXml.ValorUnitarioItemMaterial);
            //xmlMontadorEstimulo.WriteElementString(tagProdutor, itemEmpenhoParaXml.Produtor);
            xmlMontadorEstimulo.WriteElementString(tagDescricaoItemMaterial, itemEmpenhoParaXml.Descricao);
            xmlMontadorEstimulo.WriteElementString(tagProdutorPPais, itemEmpenhoParaXml.ProdutorPPais);
            xmlMontadorEstimulo.WriteElementString(tagItMeEpp, itemEmpenhoParaXml.ItMeEpp);
            
            //xmlMontadorEstimulo.WriteElementString("descricao1", itemEmpenhoParaXml.Descricao1);
            //xmlMontadorEstimulo.WriteElementString("descricao2", itemEmpenhoParaXml.Descricao2);
            //xmlMontadorEstimulo.WriteElementString("descricao3", itemEmpenhoParaXml.Descricao3);

            xmlMontadorEstimulo.WriteEndElement();

            //Descarrega o conteúdo do XML.
            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbMensagemEstimulo.ToString();

            return strRetorno;
        }
    }
    #endregion POCOs Auxiliares
}
