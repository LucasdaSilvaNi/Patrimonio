using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace PatrimonioBusiness.contabiliza.entidades
{
    [XmlRoot(ElementName = "documento")]
    public class XmlDoc
    {
        [XmlElement(ElementName = "TipoMovimento")]
        public string TipoMovimento { get; set; }
        [XmlElement(ElementName = "Data")]
        public string Data { get; set; }
        [XmlElement(ElementName = "UgeOrigem")]
        public string UgeOrigem { get; set; }
        [XmlElement(ElementName = "Gestao")]
        public string Gestao { get; set; }
        [XmlElement(ElementName = "Tipo_Entrada_Saida_Reclassificacao_Depreciacao")]
        public string Tipo_Entrada_Saida_Reclassificacao_Depreciacao { get; set; }
        [XmlElement(ElementName = "CpfCnpjUgeFavorecida")]
        public string CpfCnpjUgeFavorecida { get; set; }
        [XmlElement(ElementName = "GestaoFavorecida")]
        public string GestaoFavorecida { get; set; }
        [XmlElement(ElementName = "Item")]
        public string Item { get; set; }
        [XmlElement(ElementName = "TipoEstoque")]
        public string TipoEstoque { get; set; }
        [XmlElement(ElementName = "Estoque")]
        public string Estoque { get; set; }
        [XmlElement(ElementName = "EstoqueDestino")]
        public string EstoqueDestino { get; set; }
        [XmlElement(ElementName = "EstoqueOrigem")]
        public string EstoqueOrigem { get; set; }
        [XmlElement(ElementName = "TipoMovimentacao")]
        public string TipoMovimentacao { get; set; }
        [XmlElement(ElementName = "ValorTotal")]
        public string ValorTotal { get; set; }
        [XmlElement(ElementName = "ControleEspecifico")]
        public string ControleEspecifico { get; set; }
        [XmlElement(ElementName = "ControleEspecificoEntrada")]
        public string ControleEspecificoEntrada { get; set; }
        [XmlElement(ElementName = "ControleEspecificoSaida")]
        public string ControleEspecificoSaida { get; set; }
        [XmlElement(ElementName = "FonteRecurso")]
        public string FonteRecurso { get; set; }
        [XmlElement(ElementName = "NLEstorno")]
        public string NLEstorno { get; set; }
        [XmlElement(ElementName = "Empenho")]
        public string Empenho { get; set; }
        [XmlElement(ElementName = "Observacao")]
        public string Observacao { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }
    [XmlRoot(ElementName = "NF")]
    public class NF
    {
        [XmlElement(ElementName = "NotaFiscal")]
        public string NotaFiscal { get; set; }
    }

    [XmlRoot(ElementName = "Repeticao")]
    public class Repeticao
    {
        [XmlElement(ElementName = "NF")]
        public NF NF { get; set; }
        [XmlElement(ElementName = "IM")]
        public IM IM { get; set; }
    }

    [XmlRoot(ElementName = "NotaFiscal")]
    public class NotaFiscal
    {
        [XmlElement(ElementName = "Repeticao")]
        public Repeticao Repeticao { get; set; }
    }

    [XmlRoot(ElementName = "IM")]
    public class IM
    {
        [XmlElement(ElementName = "ItemMaterial")]
        public string ItemMaterial { get; set; }
    }

    [XmlRoot(ElementName = "ItemMaterial")]
    public class ItemMaterial
    {
        [XmlElement(ElementName = "Repeticao")]
        public Repeticao Repeticao { get; set; }
    }

    [XmlRoot(ElementName = "SiafemDocNlPatrimonial")]
    public class SiafemDocNlPatrimonial
    {
        [XmlElement(ElementName = "documento")]
        public XmlDoc Documento { get; set; }
        [XmlElement(ElementName = "NotaFiscal")]
        public NotaFiscal NotaFiscal { get; set; }
        [XmlElement(ElementName = "ItemMaterial")]
        public ItemMaterial ItemMaterial { get; set; }
    }

    [XmlRoot(ElementName = "SIAFDOC")]
    public class SIAFDOC
    {
        [XmlElement(ElementName = "cdMsg")]
        public string CdMsg { get; set; }
        [XmlElement(ElementName = "SiafemDocNlPatrimonial")]
        public SiafemDocNlPatrimonial SiafemDocNlPatrimonial { get; set; }
    }
}
