using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class SupportConsultViewModel
    {

        [Display(Name = "Órgão")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        public int? BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        public int? ManagerUnitId { get; set; }

        [Display(Name = "Status(Prodesp)")]
        public int? SupportStatusProdespId { get; set; }

        [Display(Name = "Status(Usuário)")]
        public int? SupportStatusUserId { get; set; }

        [Display(Name = "Nº Chamado")]
        public int? SupportId { get; set; }

        [Display(Name = "Data de Abertura")]
        public DateTime? DataInclusao { get; set; }

        public bool AdmGeral { get; set; }

        public short ultimaResposta { get; set; }

        public string historicoContenha { get; set; }

        public string listaLote { get; set; }

        public StaticPagedList<SupportGridViewModel> listaSupportGridViewModel { get; set; }
    }

    public class SuporteRespostaEmLote{
        [Display(Name = "Respostas ao chamados:")]
        public string listaString { get; set; }
        [Required]
        public List<int> listaLoteEscolhidos { get; set; }
        [Required]
        public string Observacao { get; set; }

        [Display(Name = "Status(Prodesp)")]
        public short StatusProdesp { get; set; }
    }
}
