using Newtonsoft.Json;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.Models
{
    [Serializable]
    public partial class Asset
    {
        public Asset()
        {
            this.AssetNumberIdentifications = new List<AssetNumberIdentification>();
            this.AssetMovements = new List<AssetMovements>();
            this.Repairs = new List<Repair>();
            this.Exchanges = new List<Exchange>();
            this.RelatedItemInventarios = new List<ItemInventario>();
            //this.AuditoriaIntegracaos = new List<AuditoriaIntegracao>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Tipo de Movimento")]
        public int MovementTypeId { get; set; }

        public bool Status { get; set; }

        [Required]
        [Display(Name = "Sigla")]
        public int InitialId { get; set; }

        [Required]
        [Display(Name = "Chapa")]
        public string NumberIdentification { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Aquisi��o")]
        public virtual DateTime AcquisitionDate { get; set; }

        [Required]
        [Display(Name = "Valor de Aquisi��o")]
        public virtual decimal ValueAcquisition { get; set; }

        [Display(Name = "Valor Atual")]
        public virtual Nullable<decimal> ValueUpdate { get; set; }

        [Required]
        [Display(Name = "C�digo de Item Material")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Realizar a pesquisa do Item de Material")]
        public int MaterialItemCode { get; set; }

        [Required]
        [Display(Name = "Descri��o de Item Material")]
        public string MaterialItemDescription { get; set; }

        [Required]
        [Display(Name = "C�digo do Grupo Material")]
        public int MaterialGroupCode { get; set; }

        [Required]
        [Display(Name = "Vida �til (em meses)")]
        public int LifeCycle { get; set; }

        [Required]
        [Display(Name = "Tax. Deprecia��o Mensal (%)")]
        public decimal RateDepreciationMonthly { get; set; }

        [Required]
        [Display(Name = "Valor Residual")]
        public decimal ResidualValue { get; set; }

        [Display(Name = "Deprecia��o Acelerada")]
        public bool AceleratedDepreciation { get; set; }

        [Display(Name = "Empenho")]
        [StringLength(maximumLength: 15, ErrorMessage = "Digite no m�ximo 15 caracteres")]
        public string Empenho { get; set; }

        [Required]
        [Display(Name = "Descri��o Resumida do Item")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Descri��o Resumida do Item � obrigat�rio")]
        public int ShortDescriptionItemId { get; set; }

        [Display(Name = "Sigla Antiga")]
        public Nullable<int> OldInitial { get; set; }

        [Display(Name = "Chapa Antiga")]
        public string OldNumberIdentification { get; set; }

        [Required]
        [Display(Name = "N�mero de Documento")]
        [StringLength(maximumLength: 20,ErrorMessage ="Digite no m�ximo 20 caracteres")]
        public string NumberDoc { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Movimento")]
        public DateTime MovimentDate { get; set; }

        [Required]
        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE � obrigat�rio")]
        public int ManagerUnitId { get; set; }

        [Display(Name = "Terceiro")]
        public Nullable<int> OutSourcedId { get; set; }

        [Display(Name = "Fornecedor")]
        public Nullable<int> SupplierId { get; set; }

        [Display(Name = "N�mero S�rie")]
        public string SerialNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Fabrica��o")]
        public Nullable<DateTime> ManufactureDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Data Garantia")]
        public Nullable<DateTime> DateGuarantee { get; set; }

        [Display(Name = "N�mero Chassi")]
        public string ChassiNumber { get; set; }

        [Display(Name = "Marca")]
        public string Brand { get; set; }

        [Display(Name = "Modelo")]
        public string Model { get; set; }

        [Display(Name = "N� da Placa (Ve�culo)")]
        public string NumberPlate { get; set; }

        [Display(Name = "Descri��o Adicional")]
        public string AdditionalDescription { get; set; }

        [Display(Name = "Pendencia Importa��o")]
        public int? flagVerificado { get; set; }

        [Display(Name = "Depreciacao Acumulada?")]
        public int? flagDepreciaAcumulada { get; set; }

        [Display(Name = "Sigla")]
        public string InitialName { get; set; }
        public Nullable<decimal> ResidualValueCalc { get; set; }
        public Nullable<int> MonthUsed { get; set; }
        public Nullable<decimal> DepreciationByMonth { get; set; }
        public Nullable<decimal> DepreciationAccumulated { get; set; }
        public bool? flagCalculoPendente { get; set; }
        public bool? flagAcervo { get; set; }
        public bool? flagTerceiro { get; set; }
        public bool? flagDepreciationCompleted { get; set; }
		public bool? flagAnimalNaoServico { get; set; }
        public bool flagVindoDoEstoque { get; set; }
        public byte IndicadorOrigemInventarioInicial { get; set; }
        [Display(Name = "Diferencia��o da chapa")]
        [StringLength(maximumLength:7, ErrorMessage = "Digite no m�ximo 7 caracteres")]
        public string DiferenciacaoChapa { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ChapaCompleta { get; private set; }
        [Display(Name = "Diferencia��o da chapa antiga")]
        [StringLength(maximumLength: 7, ErrorMessage = "Digite no m�ximo 7 caracteres")]
        public string DiferenciacaoChapaAntiga { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ChapaAntigaCompleta { get; private set; }

        public Nullable<decimal> ValorDesdobramento { get; set; }

        public bool? flagDecreto { get; set; }

        [NotMapped]
        public int? ItemInventarioId { get; set; }
        [NotMapped]
        public bool Incorporar { get; set; }

        [Display(Name = "CPF/CNPJ")]
        public string CPFCNPJ { get; set; }

        public bool? Recalculo { get; set; }
        public int? AssetStartId { get; set; }

        public string Login { get; set; }
        public DateTime? DataLogin { get; set; }
        public DateTime? DepreciationDateStart { get; set; }
        public Decimal? DepreciationAmountStart { get; set; }
        // Relacionamentos
        public virtual Initial RelatedInitial { get; set; }

        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        public virtual MovementType RelatedMovementType { get; set; }
        public virtual OutSourced RelatedOutSourced { get; set; }
        public virtual ShortDescriptionItem RelatedShortDescriptionItem { get; set; }
        public virtual Supplier RelatedSupplier { get; set; }


        public virtual ICollection<AssetNumberIdentification> AssetNumberIdentifications { get; set; }
        public virtual ICollection<AssetMovements> AssetMovements { get; set; }
        public virtual ICollection<HistoricoValoresDecreto> HistoricoValoresDecretos { get; set; }
        public virtual ICollection<Repair> Repairs { get; set; }
        public virtual ICollection<ItemInventario> RelatedItemInventarios { get; set; }
        public List<Exchange> Exchanges { get; private set; }
        public virtual ICollection<LogAlteracaoDadosBP> LogAlteracaoDadosBPs { get; set; }

        public partial class AssetManagerUnit
        {
            public string InitialName { get; set; }
            public string NumberIdentification { get; set; }
            public string ManagerUnitCode { get; set; }
            public string ManagerUnitDescription { get; set; }
        }
    }
}
