using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class MaterialSubItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Item Material")]
        public int MaterialItemId { get; set; }

        [Required]
        [Display(Name = "Código")]
        public long Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "Código de barras")]
        public string BarCode { get; set; }

        [Display(Name = "Lote")]
        public bool Lot { get; set; }

        [Required]
        [Display(Name = "Indicador Atividade")]
        public bool ActivityIndicator { get; set; }

        [Required]
        [Display(Name = "Gestor")]
        public int ManagerId { get; set; }

        [Required]
        [Display(Name = "Natureza Despesa")]
        public int SpendingOriginId { get; set; }

        [Required]
        [Display(Name = "Conta Contábil")]
        public int AuxiliaryAccountId { get; set; }

        [Required]
        [Display(Name = "Unidade Fornecimento")]
        public int SupplyUnitId { get; set; }

        public bool Status { get; set; }

        public virtual AuxiliaryAccount RelatedAuxiliaryAccount { get; set; }
        public virtual Manager RelatedManager { get; set; }
        public virtual MaterialItem RelatedMaterialItem { get; set; }
        public virtual SpendingOrigin RelatedSpendingOrigin { get; set; }
        public virtual SupplyUnit RelatedSupplyUnit { get; set; }
    }
}
