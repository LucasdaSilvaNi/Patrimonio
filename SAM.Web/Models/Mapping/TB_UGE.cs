namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_UGE
    {
        public TB_UGE()
        {
            TB_ALMOXARIFADO = new HashSet<TB_ALMOXARIFADO>();
            TB_ALMOXARIFADO1 = new HashSet<TB_ALMOXARIFADO>();
            TB_GESTOR = new HashSet<TB_GESTOR>();
            TB_UA = new HashSet<TB_UA>();
        }

        [Key]
        public int TB_UGE_ID { get; set; }

        public int TB_UO_ID { get; set; }

        public int TB_UGE_CODIGO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_UGE_DESCRICAO { get; set; }

        [Required]
        [StringLength(1)]
        public string TB_UGE_TIPO { get; set; }

        public virtual ICollection<TB_ALMOXARIFADO> TB_ALMOXARIFADO { get; set; }

        public virtual ICollection<TB_ALMOXARIFADO> TB_ALMOXARIFADO1 { get; set; }

        public virtual ICollection<TB_GESTOR> TB_GESTOR { get; set; }

        public virtual ICollection<TB_UA> TB_UA { get; set; }

        public virtual TB_UO TB_UO { get; set; }
    }
}
