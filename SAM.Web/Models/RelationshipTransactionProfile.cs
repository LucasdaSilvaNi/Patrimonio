using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class RelationshipTransactionProfile
    {
        public int Id { get; set; }

        [Display(Name = "Transação")]
        public int TransactionId { get; set; }
        
        [Display(Name="Perfil")]
        public int ProfileId { get; set; }
        public bool Status { get; set; }

        public virtual Profile RelatedProfile { get; set; }
        public virtual Transaction RelatedTransaction { get; set; }
    }
}
