using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class AdministrativeUnit
    {
        public AdministrativeUnit()
        {
            this.AssetMovements = new List<AssetMovements>();
            this.Sections = new List<Section>();
            //this.Responsibles = new List<Responsible>();
            this.Mobiles = new List<Mobile>();
       }

        public int Id { get; set; }

        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int ManagerUnitId { get; set; }
        
        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "UA Vinculada")]
        public Nullable<int> RelationshipAdministrativeUnitId { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        //public virtual ICollection<Responsible> Responsibles { get; set; }
        public virtual ICollection<Mobile> Mobiles { get; set; }

        public virtual ICollection<Inventario> Inventarios { get; set; }

        public virtual ICollection<Responsible> Responsibles { get; set; }
    }
}