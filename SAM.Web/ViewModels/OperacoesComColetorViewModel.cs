using Sam.Common.Util;
using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SAM.Web.ViewModels
{
    public partial class OperacoesComColetorViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        public bool Status { get; set; }

        [Display(Name ="Nome Arquivo")]
        public HttpPostedFileBase PostFile { get; set; }
    }

    [NotMapped]
    public partial class OperacoesComColetorViewModel //: Section
    {
        [Display(Name = "Órgão")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Orgão é obrigatório")]
        public int InstitutionId { get; set; }

        [Display(Name = "UO")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UO é obrigatório")]
        public int BudgetUnitId { get; set; }

        [Display(Name = "UGE")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo UGE é obrigatório")]
        public int ManagerUnitId { get; set; }


        public bool MensagemModal { get; set; }

        public int AdministrativeUnitId { get; set; }
        //public int SectionId { get; set; }
        public int? SectionId { get; set; }
        public int? ResponsibleIdAux { get; set; }


        [Required]
        [Display(Name = "Tipo de Movimento")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Por favor, informe o Tipo de Movimento.")]
        //public int TipoDispositivoInventariante { get; set; }
        //public int TipoOperacaoDispositivoInventariante { get; set; }
        public int TipoDispositivoInventarianteID { get; set; }
        public int TipoOperacaoDispositivoInventarianteID { get; set; }

        public bool GeraTabela_ItemMaterial { get; set; }
        public bool GeraTabela_Responsavel { get; set; }
        public bool GeraTabela_Terceiros { get; set; }
        public bool GeraTabela_Sigla { get; set; }
        public bool GeraTabela_BemPatrimonial { get; set; }


        public IEnumerable<TipoDispositivoInventariante> TiposDispositivoInventariante { get { return TipoDispositivoInventariante.ListaTiposDispositivoInventariante(); } private set { } }
        public IEnumerable<TipoOperacaoDispositivoInventariante> TiposOperacaoDispositivoInventario { get { return TipoOperacaoDispositivoInventariante.ListaTiposOperacaoDispositivoInventario(); } private set { } }
    }

    public class TipoDispositivoInventariante
    {
        public int Id;
        public string Descricao;
        public static IList<TipoDispositivoInventariante> ListaTiposDispositivoInventariante() { return Listar(); }


        static private IList<TipoDispositivoInventariante> Listar()
        {
            IList<TipoDispositivoInventariante> listaTiposDispositivoInventariante = new List<TipoDispositivoInventariante>();
            listaTiposDispositivoInventariante.Add(new TipoDispositivoInventariante() { Id = 1, Descricao = "Android" });
            listaTiposDispositivoInventariante.Add(new TipoDispositivoInventariante() { Id = 2, Descricao = "COMPEX CPX-8000 (InventPat v1.20)" });


            return listaTiposDispositivoInventariante;
        }
    }

    [NotMapped]
    public class TipoOperacaoDispositivoInventariante
    {
        public int Id;
        public string Descricao;
        public static IList<TipoOperacaoDispositivoInventariante> ListaTiposOperacaoDispositivoInventario() { return Listar(); }


        static private IList<TipoOperacaoDispositivoInventariante> Listar()
        {
            IList<TipoOperacaoDispositivoInventariante> listaTiposOperacaoDispositivoInventario = new List<TipoOperacaoDispositivoInventariante>();
            listaTiposOperacaoDispositivoInventario.Add(new TipoOperacaoDispositivoInventariante() { Id = 1, Descricao = "Leitura Arquivos (InventPat v1.20)" });
            listaTiposOperacaoDispositivoInventario.Add(new TipoOperacaoDispositivoInventariante() { Id = 2, Descricao = "Geração Arquivos (InventPat v1.20)" });


            return listaTiposOperacaoDispositivoInventario;
        }
    }

    public partial class OperacoesComColetorMobileViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
    }
}