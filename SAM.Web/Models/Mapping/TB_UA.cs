namespace SAM.Web.Models.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_UA
    {
        public TB_UA()
        {
            TB_DIVISAO = new HashSet<TB_DIVISAO>();
        }

        [Key]
        public int TB_UA_ID { get; set; }

        public int TB_UGE_ID { get; set; }

        public int TB_UA_CODIGO { get; set; }

        [Required]
        [StringLength(120)]
        public string TB_UA_DESCRICAO { get; set; }

        public int? TB_UA_VINCULADA { get; set; }

        public bool TB_UA_INDICADOR_ATIVIDADE { get; set; }

        public int? TB_UNIDADE_ID { get; set; }

        public int? TB_CENTRO_CUSTO_ID { get; set; }

        public int TB_GESTOR_ID { get; set; }

        public virtual ICollection<TB_DIVISAO> TB_DIVISAO { get; set; }

        public virtual TB_GESTOR TB_GESTOR { get; set; }

        public virtual TB_UGE TB_UGE { get; set; }

        public virtual TB_UNIDADE TB_UNIDADE { get; set; }
    }
}
