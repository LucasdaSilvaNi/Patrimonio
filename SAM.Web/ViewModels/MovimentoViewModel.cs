using PagedList;
using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class MovimentoViewModel
    {
        public int AssetId { get; set; }
        [Required]
        [Display(Name = "Tipo de Movimento")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Por favor, informe o Tipo de Movimento.")]
        public int MovementTypeId { get; set; }

        public int InstituationId { get; set; }
        public int BudgetUnitId { get; set; }
        public int ManagerUnitId { get; set; }
        public int AdministrativeUnitId { get; set; }
        public int SectionId { get; set; }

        public int ContaContabilId { get; set; }

        public int InstituationIdDestino { get; set; }
        public int BudgetUnitIdDestino { get; set; }
        public int ManagerUnitIdDestino { get; set; }
        public int AdministrativeUnitIdDestino { get; set; }
        public int SectionIdDestino { get; set; }

        public int ResponsibleId { get; set; }
        public string NumberProcess { get; set; }
        public int TypeDocumentOutId { get; set; }
        [Required]
        [Display(Name = "Data de Movimento")]
        public string MovimentoDate { get; set; }
        public string Observation { get; set; }
        public string RepairValue { get; set; }

        public string SearchString { get; set; }
        public string ListAssetsParaMovimento { get; set; }
        public string ListAssets { get; set; }

        [Display(Name = "CPF/CNPJ")]
        public string CPFCNPJ { get; set; }
        public string LoginSiafem { get; set; }
        public string SenhaSiafem { get; set; }
        public string UGESiafem { get; set; }
        public bool UGEIntegradaComSiafem { get; set; }
        public bool NaoDigitouLoginSiafem { get; set; }
        public int ItemInventarioId { get; set; }
        public bool Proibido { get; set; }

        public List<string> MsgSIAFEM { get; set; }
        public string ListaIdAuditoria { get; set; }

        public StaticPagedList<AssetEAssetMovimentViewModel> listaAssetEAssetViewModel { get; set; }
    }

    public class MovimentoIndexViewModel {
        public string sortOrder { get; set; }
        public string searchString { get; set; }
        public string currentFilter { get; set; }
        public int? page { get; set; }
        public string cbStatus { get; set; }
        public string cbFiltros { get; set; }

        public bool perfilOperador { get; set; }
        public bool perfilOperadorUGE { get; set; }
    }

    public class BPValidoParaViewModel {
        public bool ContaContabilInservivel { get; set; }
        public bool Terceiro { get; set; }
        public int TipoMovimento { get; set; }
    }
}