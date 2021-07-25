namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_ALMOXARIFADO
    {
        public TB_ALMOXARIFADO()
        {
            TB_DIVISAO = new HashSet<TB_DIVISAO>();
        }

        [Key]
        public int TB_ALMOXARIFADO_ID { get; set; }

        public int TB_GESTOR_ID { get; set; }

        public int TB_ALMOXARIFADO_CODIGO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_ALMOXARIFADO_DESCRICAO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_ALMOXARIFADO_LOGRADOURO { get; set; }

        [Required]
        [StringLength(10)]
        public string TB_ALMOXARIFADO_NUMERO { get; set; }

        [StringLength(10)]
        public string TB_ALMOXARIFADO_COMPLEMENTO { get; set; }

        [StringLength(45)]
        public string TB_ALMOXARIFADO_BAIRRO { get; set; }

        [StringLength(45)]
        public string TB_ALMOXARIFADO_MUNICIPIO { get; set; }

        public int TB_UF_ID { get; set; }

        [Required]
        [StringLength(8)]
        public string TB_ALMOXARIFADO_CEP { get; set; }

        [StringLength(20)]
        public string TB_ALMOXARIFADO_TELEFONE { get; set; }

        [StringLength(20)]
        public string TB_ALMOXARIFADO_FAX { get; set; }

        [StringLength(120)]
        public string TB_ALMOXARIFADO_RESPONSAVEL { get; set; }

        public int? TB_UGE_ID { get; set; }

        [StringLength(6)]
        public string TB_ALMOXARIFADO_MES_REF_INICIAL { get; set; }

        [StringLength(6)]
        public string TB_ALMOXARIFADO_MES_REF { get; set; }

        public bool TB_ALMOXARIFADO_INDICADOR_ATIVIDADE { get; set; }

        public bool? TB_ALMOXARIFADO_EFETUA_SAIDA { get; set; }

        public virtual TB_GESTOR TB_GESTOR { get; set; }

        public virtual TB_UGE TB_UGE { get; set; }

        public virtual TB_UGE TB_UGE1 { get; set; }

        public virtual ICollection<TB_DIVISAO> TB_DIVISAO { get; set; }
    }
}
