using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.Models
{
    public partial class AssetMovements
    {
        public int Id { get; set; }

        [Display(Name = "Bem Patrimonial")]
        public int AssetId { get; set; }

        public bool Status { get; set; }

        [Display(Name ="Data Movimentação")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime MovimentDate { get; set; }

        public int MovementTypeId { get; set; }

        [Required]
        [Display(Name = "Estado de Conservação")]
        public int StateConservationId { get; set; }

        [Display(Name = "Processo")]
        public string NumberPurchaseProcess { get; set; }

        [Required]
        [Display(Name = "Órgão")]
        public int InstitutionId { get; set; }

        [Required]
        [Display(Name = "UO")]
        public int BudgetUnitId { get; set; }

        [Required]
        [Display(Name = "UGE")]
        public int ManagerUnitId { get; set; }

        [Display(Name = "UA")]
        public Nullable<int> AdministrativeUnitId { get; set; }

        [Display(Name = "Divisão")]
        public Nullable<int> SectionId { get; set; }

        [Display(Name = "Conta Contábil")]
        public Nullable<int> AuxiliaryAccountId { get; set; }

        [Display(Name = "Responsável")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Responsável é obrigatório")]
        public Nullable<int> ResponsibleId { get; set; }

        [Display(Name = "UGE de Destino")]
        public Nullable<int> SourceDestiny_ManagerUnitId { get; set; }

        [Display(Name = "Bem Patrimonial Transferido")]
        public Nullable<int> AssetTransferenciaId { get; set; }

        [Display(Name = "Bolsa")]
        public Nullable<int> ExchangeId { get; set; }

        [Display(Name = "Data Bolsa")]
        public Nullable<DateTime> ExchangeDate { get; set; }

        [Display(Name = "Usuário Bolsa")]
        public Nullable<int> ExchangeUserId { get; set; }

        [Display(Name = "Tipo de Doação")]
        public Nullable<int> TypeDocumentOutId { get; set; }

        [Display(Name = "Observação")]
        public string Observation { get; set; }

        [Display(Name = "Estorno")]
        public bool? FlagEstorno { get; set; }

        [Display(Name = "Data de Estorno")]
        public DateTime? DataEstorno { get; set; }

        [Display(Name = "Login de Estorno")]
        public int? LoginEstorno { get; set; }

        [Display(Name = "Valor do Conserto")]
        public Nullable<decimal> RepairValue { get; set; }

        public string Login { get; set; }
        public DateTime? DataLogin { get; set; }

        [Display(Name = "Documento")]
        public string NumberDoc { get; set; }

        public bool? flagUGENaoUtilizada { get; set; }

        [Display(Name = "CPF/CNPJ")]
        public string CPFCNPJ { get; set; }

        //public string NumeroNL { get; set; }
        public int? MonthlyDepreciationId { get; set; }

        public string NotaLancamento { get; set; }
        public string NotaLancamentoEstorno { get; set; }
        public string NotaLancamentoDepreciacao { get; set; }
        public string NotaLancamentoDepreciacaoEstorno { get; set; }
        public string NotaLancamentoReclassificacao { get; set; }
        public string NotaLancamentoReclassificacaoEstorno { get; set; }
        public int? NotaLancamentoPendenteSIAFEMId { get; set; }
        public int? AuditoriaIntegracaoId { get; set; }
        public Nullable<int> ContaContabilAntesDeVirarInservivel { get; set; }
        public Nullable<int> AuxiliaryMovementTypeId { get; set; }

        //Relacionamentos
        public virtual AdministrativeUnit RelatedAdministrativeUnit { get; set; }
        public virtual Asset RelatedAssets { get; set; }
        public virtual AuxiliaryAccount RelatedAuxiliaryAccount { get; set; }
        public virtual BudgetUnit RelatedBudgetUnit { get; set; }
        public virtual Institution RelatedInstitution { get; set; }
        public virtual MovementType RelatedMovementType { get; set; }
        public virtual Responsible RelatedResponsible { get; set; }
        public virtual Section RelatedSection { get; set; }
        public virtual ManagerUnit RelatedManagerUnit { get; set; }
        public virtual ManagerUnit RelatedSourceDestinyManagerUnit { get; set; }
        public virtual User RelatedUser { get; set; }
        public virtual AuditoriaIntegracao RelatedAuditoriaIntegracao { get; set; }
        public virtual NotaLancamentoPendenteSIAFEM RelatedNotaLancamentoPendenteSIAFEM { get; set; }

        public virtual ICollection<LogAlteracaoDadosBP> LogAlteracaoDadosBPs { get; set; }

        [NotMapped]
        public IList<int> RelacaoIdsMovimentacoesVinculadasPorAuditoriaIntegracao { get; set; }

        public virtual ICollection<ItemInventario> RelatedItemInventarios { get; set; }
        //[NotMapped]
        //public IDictionary<string, long> RelacaoItemMaterial { get; set; }

        [NotMapped]
        public Guid TokenGeradoEnvioContabilizaSP { get; set; }
    }
}
