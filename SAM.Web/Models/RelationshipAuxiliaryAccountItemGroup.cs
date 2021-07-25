using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public class RelationshipAuxiliaryAccountItemGroup
    {
        public int Id { get; set; }

        [Required(ErrorMessage="O campo Grupo Material é obrigatório")]
        public int MaterialGroupId { get; set; }

        [Required(ErrorMessage = "O campo Conta Contábil é obrigatório")]
        public int AuxiliaryAccountId { get; set; }

        public virtual AuxiliaryAccount RelatedAuxiliaryAccount { get;set; }

        public virtual MaterialGroup RelatedMaterialGroup { get; set; }
    }
}