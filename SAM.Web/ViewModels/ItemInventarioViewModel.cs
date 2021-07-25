using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class ItemInventarioViewModel: ItemInventario
    {
        public ItemInventarioViewModel()
        {
            this.ItemInventarioList = new List<ItemInventario>();
        }

        [Display(Name = "Sigla *")]
        public string InitialName { get; set; }

        [Display(Name = "Pesquisa de Chapa")]
        public string NumberCode { get; set; }

        public string Mensagem { get; set; }

        public string EstadoDescricao { get; set; }

        public List<ItemInventario> ItemInventarioList { get; set; }

        public int QtdBensSelecionados { get; set; }

        public int PageNumber { get; set; }
    }

    [NotMapped]
    public class ItemInventarioBPViewModel {

        [Display(Name = "Chapa")]
        public string Chapa { get; set; }

        [Display(Name = "Sigla")]
        public string Sigla { get; set; }

        [Display(Name = "Conta Contábil")]
        public string ContaContabil { get; set; }

        [Display(Name = "Orgão")]
        public string Orgao { get; set; }

        [Display(Name = "UO")]
        public string UO { get; set; }

        [Display(Name = "UGE")]
        public string UGE { get; set; }

        [Display(Name = "UA")]
        public string UA { get; set; }

        [Display(Name = "Divisão")]
        public string Divisao { get; set; }

        [Display(Name = "Responsável")]
        public string Responsavel { get; set; }

        [Display(Name = "Grupo Material")]
        public int GrupoMaterial { get; set; }

        public int CodigoItemMaterial { get; set; }

        [Display(Name = "Item Material")]
        public string DescricaoMaterial { get; set; }

        public bool Status { get; set; }

        public int InventarioId { get; set; }
    }
}