using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class AuxiliaryAccount
    {
        public AuxiliaryAccount()
        {
            this.AssetMovements = new List<AssetMovements>();
            this.MaterialSubItems = new List<MaterialSubItem>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "C�digo *")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descri��o *")]
        public string Description { get; set; }

        [Display(Name = "Conta Cont�bil *")]
        public Nullable<int> BookAccount { get; set; }

        public bool Status { get; set; }

        public int? DepreciationAccountId { get; set; }

        public int RelacionadoBP { get; set; }

        [MaxLength(50)]
        [Display(Name = "Complemento")]
        public string ContaContabilApresentacao { get; set; }

        [MaxLength(15)]
        [Display(Name = "Controle Espec�fico Resumido")]
        public string ControleEspecificoResumido { get; set; }


        [MaxLength(80)]
        [Display(Name = "Tipo Movimenta��o ContabilizaSP")]
        public string TipoMovimentacaoContabilizaSP { get; set; }


        public virtual DepreciationAccount RelatedDepreciationAccount { get; set; }
        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual ICollection<MaterialSubItem> MaterialSubItems { get; set; }
        public virtual ICollection<RelationshipAuxiliaryAccountItemGroup> RelationshipAuxiliaryAccountItemGroups { get; set; }
        public virtual ICollection<RelationshipAuxiliaryAccountMovementType> RelationshipAuxiliaryAccountMovementTypes { get; set; }
    }
}
