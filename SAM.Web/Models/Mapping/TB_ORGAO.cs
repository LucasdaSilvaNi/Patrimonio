namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_ORGAO
    {
        public TB_ORGAO()
        {
            TB_GESTOR = new HashSet<TB_GESTOR>();
            TB_UO = new HashSet<TB_UO>();
        }

        [Key]
        public int TB_ORGAO_ID { get; set; }

        public int TB_ORGAO_CODIGO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_ORGAO_DESCRICAO { get; set; }

        public virtual ICollection<TB_GESTOR> TB_GESTOR { get; set; }

        public virtual ICollection<TB_UO> TB_UO { get; set; }
    }
}
