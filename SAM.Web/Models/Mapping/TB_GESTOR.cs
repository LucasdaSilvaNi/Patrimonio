namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_GESTOR
    {
        public TB_GESTOR()
        {
            TB_ALMOXARIFADO = new HashSet<TB_ALMOXARIFADO>();
            TB_UA = new HashSet<TB_UA>();
            TB_UNIDADE = new HashSet<TB_UNIDADE>();
        }

        [Key]
        public int TB_GESTOR_ID { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_GESTOR_NOME { get; set; }

        [Required]
        [StringLength(25)]
        public string TB_GESTOR_NOME_REDUZIDO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_GESTOR_LOGRADOURO { get; set; }

        [Required]
        [StringLength(10)]
        public string TB_GESTOR_NUMERO { get; set; }

        [StringLength(10)]
        public string TB_GESTOR_COMPLEMENTO { get; set; }

        [StringLength(20)]
        public string TB_GESTOR_TELEFONE { get; set; }

        public int TB_GESTOR_CODIGO_GESTAO { get; set; }

        [Column(TypeName = "image")]
        public byte[] TB_GESTOR_IMAGEM { get; set; }

        public int TB_ORGAO_ID { get; set; }

        public int? TB_UO_ID { get; set; }

        public int? TB_UGE_ID { get; set; }

        public virtual ICollection<TB_ALMOXARIFADO> TB_ALMOXARIFADO { get; set; }

        public virtual TB_ORGAO TB_ORGAO { get; set; }

        public virtual TB_UGE TB_UGE { get; set; }

        public virtual TB_UO TB_UO { get; set; }

        public virtual ICollection<TB_UA> TB_UA { get; set; }

        public virtual ICollection<TB_UNIDADE> TB_UNIDADE { get; set; }
    }
}
