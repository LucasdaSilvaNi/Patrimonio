using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public partial class AssetViewModel : Asset
    {
        [Display(Name = "Chapa Final")]
        public string EndNumberIdentification { get; set; }

        [Required]
        //[DataType(DataType.Currency)]
        //[DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [Display(Name = "Valor Aquisição")]
        public string ValueAcquisitionModel { get; set; }

        //[DataType(DataType.Currency)]
        //[DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        [Display(Name = "Valor Atualização")]
        public string ValueUpdateModel { get; set; }

        [Required]
        [Display(Name = "Estado de Conservação")]
        public int StateConservationId { get; set; }

        [Display(Name = "Processo")]
        public string NumberPurchaseProcess { get; set; }

        [Required]
        [Display(Name = "Órgão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Órgão é obrigatório")]
        public int InstitutionId { get; set; }

        [Required]
        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO é obrigatório")]
        public int BudgetUnitId { get; set; }

        [Required]
        [Display(Name = "UA")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UA é obrigatório")]
        public Nullable<int> AdministrativeUnitId { get; set; }

        [Display(Name = "Divisão")]
        public Nullable<int> SectionId { get; set; }

        [Display(Name = "Conta Contábil")]
        public Nullable<int> AuxiliaryAccountId { get; set; }

        [Display(Name = "Conta Contábil")]
        public string ContaContabilApresentacaoEdicao { get; set; }

        [Required]
        [Display(Name = "Responsável")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Responsável é obrigatório")]
        public Nullable<int> ResponsibleId { get; set; }
        public string AssetsNumberIdentification { get; set; }
        public string InitialDescription { get; set; }
        public bool selected { get; set; }

        [Display(Name = "Item Material")]
        public string materialItemPesquisa { get; set; }
        public int Uge { get; set; }
        public int Ug { get; set; }
        public string numeroEmpenho { get; set; }

        public string EmpenhoResultado { get; set; }
        public string numeroDocumento { get; set; }

        [Display(Name = "UGE")]
        public string OrigemManagerUnit { get; set; }

        public string ShortDescription { get; set; }
        public string ShortDescriptionAuxiliar { get; set; }

        [Display(Name = "Grupo Material")]
        public string MaterialGroupDescription { get; set; }
        public bool checkComplemento { get; set; }
        public bool checkDepreciacao { get; set; }
        public bool checkLoteChapa { get; set; }    
        public bool checkFlagAcervo { get; set; }
        public bool checkFlagTerceiro { get; set; }
        public bool checkFlagInformaItem { get; set; }
        public bool checkFlagDecretoSefaz { get; set; }
        public string OldInitialDescription { get; set; }
        public string CPFCNPJDoTerceiro { get; set; }
        public string NomeDoTerceiro { get; set; }
        public string OutSourcedPesquisa { get; set; }
        

        //public string SupplierPesquisa { get; set; }

        public string SupplierCPFCNPJ { get; set; }
        public string SupplierName { get; set; }
        public int AssetMovementsId { get; set; }

        public string BudgetUnitCode { get; set; }
        public string ManagerUnitCode { get; set; }
        public int? AdministrativeUnitCode { get; set; }
        public int? SectionCode { get; set; }
        public int? AuxiliaryAccountCode { get; set; }
        public string ResponsibleName { get; set; }

        public int? AssetsIdTransferencia { get; set; }

        public decimal ValueDevalue { get; set; }
        public string Observation { get; set; }

        public string LoginSiafem { get; set; }
        public string SenhaSiafem { get; set; }

        public bool UGEIntegradaComSiafem { get; set; }
        public bool NaoDigitouLoginSiafem { get; set; }

        public int InstituationIdDestino { get; set; }
        public int BudgetUnitIdDestino { get; set; }
        public int ManagerUnitIdDestino { get; set; }

        [Display(Name = "Documento")]
        public string NumberDocModel { get; set; }

        public bool? AceiteManual { get; set; }

        //public bool? OrigemInventario { get; set; }
        public int? InventarioId { get; set; }
        [Display(Name = "Mês/ano referência")]
        public string MesRef { get; set; }
        //armazena os ids da integração para os mesmo sairem da pêndencias.
        public string assetIdsIntegracao { get; set; }
        //pega o último numberDoc do AssetsMovement relacionado ao BP
        [Display(Name = "Número de Documento")]
        public string NumeroDocumentoAtual { get; set; }
        public int TipoBP { get; set;}
		public bool checkflagAnimalAServico { get; set; }
        public bool podeSerExcluido { get; set; }
        public bool podeEditarItemMaterial { get; set; }
        public bool BPsIntegracao { get; set; }
        //Gambiarra para segurar valores do Item Material do Item de Inventário
        public int MaterialItemCodeAuxiliar { get; set; }
        public string MaterialItemDescriptionAuxiliar { get; set; }


        //Gambiarra para mostrar o popup de animal a serviço para BPs vindos do estoque
        //e incorporação de item do inventário. O ideal futuramente é que seja retirada
        public bool MetodoGet { get; set; }
        public  ICollection<HistoricoDepreciacaoViewModel> AssetHistoricDepreciations2 { get; set; }
        public ICollection<HistoricoValoresDecretoViewModel> HistoricoDosValoresDoDecreto { get; set; }

        public string NotaLancamento { get; set; }
        public string NotaLancamentoEstorno { get; set; }
        public string NotaLancamentoDepreciacao { get; set; }
        public string NotaLancamentoDepreciacaoEstorno { get; set; }
        public string NotaLancamentoReclassificacao { get; set; }
        public string NotaLancamentoReclassificacaoEstorno { get; set; }
        public Nullable<int> AuxiliaryMovementTypeId { get; set; }
        public ICollection<AssetViewModelDadosNLSIAFEM> DadosSIAFEM { get; set; }
        public List<string> MsgSIAFEM { get; set; }
        public string ListaIdAuditoria { get; set; }
    }

    public class AssetViewModelDadosNLSIAFEM
    {
        public string DataHoraEnvioContabilizaSP { get; set; }
        public string DocumentoSAM { get; set; }
        public string NomeTipoNL { get; set; }
        public string NumeroNL { get; set; }
        public string ValorNL { get; set; }
    }

    public partial class AssetViewModelEmpenho
    {
        public string Empenho { get; set; }
        public string CgcCpf { get; set; }
        public string DescricaoFornecedor { get; set; }
        public List<Asset> Assets { get; set; }
    }

    public partial class AssetViewModelEmpenhoRestosAPagar
    {
        public string Empenho { get; set; }
        public string Cpf_Cnpj { get; set; }
        public double ValorConta { get; set; }
        public char DebitoCredito { get; set; }
    }

    [NotMapped]
    public partial class AssetMobileViewModel 
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public int Estado { get; set; }
        public string Item { get; set; }
        public string Descricao { get; set; }
        public string Sigla { get; set; }
        public string InitialName { get; set; }
        public int? AssetId { get; set; }
    }
}