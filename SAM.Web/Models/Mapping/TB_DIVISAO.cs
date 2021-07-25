namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_DIVISAO
    {
        [Key]
        public int TB_DIVISAO_ID { get; set; }

        public int TB_UA_ID { get; set; }

        public int TB_DIVISAO_CODIGO { get; set; }

        [StringLength(120)]
        public string TB_DIVISAO_DESCRICAO { get; set; }

        public int? TB_RESPONSAVEL_ID { get; set; }

        public int? TB_ALMOXARIFADO_ID { get; set; }

        [StringLength(120)]
        public string TB_DIVISAO_LOGRADOURO { get; set; }

        [StringLength(10)]
        public string TB_DIVISAO_NUMERO { get; set; }

        [StringLength(10)]
        public string TB_DIVISAO_COMPLEMENTO { get; set; }

        [StringLength(45)]
        public string TB_DIVISAO_BAIRRO { get; set; }

        [StringLength(45)]
        public string TB_DIVISAO_MUNICIPIO { get; set; }

        public int? TB_UF_ID { get; set; }

        [StringLength(8)]
        public string TB_DIVISAO_CEP { get; set; }

        [StringLength(20)]
        public string TB_DIVISAO_TELEFONE { get; set; }

        [StringLength(20)]
        public string TB_DIVISAO_FAX { get; set; }

        public int? TB_DIVISAO_AREA { get; set; }

        public int? TB_DIVISAO_QTDE_FUNC { get; set; }

        public bool? TB_DIVISAO_INDICADOR_ATIVIDADE { get; set; }

        public virtual TB_ALMOXARIFADO TB_ALMOXARIFADO { get; set; }

        public virtual TB_UA TB_UA { get; set; }
    }
}
