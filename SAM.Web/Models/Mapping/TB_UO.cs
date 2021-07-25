namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_UO
    {
        public TB_UO()
        {
            TB_GESTOR = new HashSet<TB_GESTOR>();
            TB_UGE = new HashSet<TB_UGE>();
        }

        [Key]
        public int TB_UO_ID { get; set; }

        public int TB_ORGAO_ID { get; set; }

        public int TB_UO_CODIGO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_UO_DESCRICAO { get; set; }

        public virtual ICollection<TB_GESTOR> TB_GESTOR { get; set; }

        public virtual TB_ORGAO TB_ORGAO { get; set; }

        public virtual ICollection<TB_UGE> TB_UGE { get; set; }
    }
}
