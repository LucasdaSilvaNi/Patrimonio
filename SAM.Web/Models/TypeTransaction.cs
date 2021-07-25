using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class TypeTransaction
    {
        public TypeTransaction()
        {
            this.Transactions = new List<Transaction>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}