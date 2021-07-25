using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class MovementType
    {
        public MovementType()
        {
            this.Assets = new List<Asset>();
            this.AssetMovements = new List<AssetMovements>();
            this.EventServiceContabilizaSPs = new List<EventServiceContabilizaSP>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Código é obrigatório")]
        [DisplayName("Código")]
        public int Code { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório")]
        [DisplayName("Descrição")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo Grupo de Movimento é obrigatório")]
        public int GroupMovimentId { get; set; }
        public bool Status { get; set; }
        public bool IncorporacaoEmLote { get; set; }
        public bool IncorporacaoSimples { get; set; }
        public int? TipoMovimentacaoRelacionada { get; set; }
        
        public virtual GroupMoviment RelatedGroupMoviment { get;set;}
        public virtual MovementType RelatedMovementType { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual ICollection<EventServiceContabilizaSP> EventServiceContabilizaSPs { get; set; }
        public virtual ICollection<RelationshipAuxiliaryAccountMovementType> RelationshipAuxiliaryAccountMovementTypes { get; set; }
        public virtual ICollection<MovementType> MovementTypes { get; set; }
    }
}
