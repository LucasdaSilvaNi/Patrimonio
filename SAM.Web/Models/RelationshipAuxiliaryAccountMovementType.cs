using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public class RelationshipAuxiliaryAccountMovementType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Grupo Material é obrigatório")]
        public int AuxiliaryAccountId { get; set; }

        [Required(ErrorMessage = "O campo Conta Contábil é obrigatório")]
        public int MovementTypeId { get; set; }

        public virtual AuxiliaryAccount RelatedAuxiliaryAccount { get; set; }

        public virtual MovementType RelatedMovementType { get; set; }
    }
}