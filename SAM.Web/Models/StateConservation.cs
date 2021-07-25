using System;
using System.Collections.Generic;

namespace SAM.Web.Models
{
    public partial class StateConservation
    {
        public StateConservation()
        {
            this.AssetMovements = new List<AssetMovements>();      
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        
    }
}
