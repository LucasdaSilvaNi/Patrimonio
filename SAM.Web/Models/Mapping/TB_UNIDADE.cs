namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_UNIDADE
    {
        public TB_UNIDADE()
        {
            TB_UA = new HashSet<TB_UA>();
        }

        [Key]
        public int TB_UNIDADE_ID { get; set; }

        public int? TB_UNIDADE_CODIGO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_UNIDADE_DESCRICAO { get; set; }

        public int TB_GESTOR_ID { get; set; }

        public virtual TB_GESTOR TB_GESTOR { get; set; }

        public virtual ICollection<TB_UA> TB_UA { get; set; }
    }
}
