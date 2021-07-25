using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.Models
{
    public partial class ItemInventario
    {
        public int? Id { get; set; }
        public int InventarioId { get; set; }

        [Required]
        [Display(Name = "Chapa")]
        public string Code { get; set; }
        public int? Estado { get; set; }

        [Display(Name = "Item Material")]
        public string Item { get; set; }

        [Display(Name = "Sigla")]
        public string InitialName { get; set; }
        public int? AssetId { get; set; }
        public int? AssetMovementsIdOriginal { get; set; }

        public virtual EstadoItemInventario RelatedEstadoItemInventario { get; set; }
        public virtual Inventario RelatedInventario { get; set; }
        public virtual Asset RelatedAsset { get; set; }
        public virtual AssetMovements RelatedAssetMovement { get; set; }

        [NotMapped]
        public string ItemsInventarioID { get; set; }
        [NotMapped]
        public int Page { get; set; }
        [NotMapped]
        [Display(Name = "Divisão")]
        public string DescricaoDivisao { get; set; }
        [NotMapped]
        public string DescricaoEstado { get; set; }
    }
}