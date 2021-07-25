using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.Models
{
    public partial class ManagerUnit
    {
        public ManagerUnit()
        {
            this.AssetMovements = new List<AssetMovements>();
            this.SourceDestiny_AssetMovements = new List<AssetMovements>();
            this.Assets = new List<Asset>();
            this.AdministrativeUnits = new List<AdministrativeUnit>();
            this.Mobiles = new List<Mobile>();
            this.Initials = new List<Initial>();

            this.AuditoriaIntegracoes = new List<AuditoriaIntegracao>();
            this.NotaLancamentoPendenteSIAFEMs = new List<NotaLancamentoPendenteSIAFEM>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Unidade Orçamentária")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO é obrigatório")]
        public int BudgetUnitId { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public bool Status { get; set; }
        [Display(Name = "Integração SIAFEM?")]
        public bool FlagIntegracaoSiafem { get; set; }

        [Display(Name = "Tratar como Órgão?")]
        public bool FlagTratarComoOrgao { get; set; }

        public string ManagmentUnit_YearMonthStart { get; set; }

        [Required]
        public string ManagmentUnit_YearMonthReference { get; set; }

        [Required]
        public int MesRefInicioIntegracaoSIAFEM { get; set; }

        public virtual ICollection<AdministrativeUnit> AdministrativeUnits { get; set; }
        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual ICollection<AssetMovements> SourceDestiny_AssetMovements { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual BudgetUnit RelatedBudgetUnit { get; set; }

        public virtual ICollection<Mobile> Mobiles { get; set; }
        public virtual ICollection<Initial> Initials { get; set; }
        public virtual ICollection<Support> Supports { get; set; }
        public virtual ICollection<MonthlyDepreciation> MonthlyDepreciations { get; set; }
        public virtual ICollection<AuditoriaIntegracao> AuditoriaIntegracoes { get; set; }
        public virtual ICollection<NotaLancamentoPendenteSIAFEM> NotaLancamentoPendenteSIAFEMs { get; set; }
    }
}
