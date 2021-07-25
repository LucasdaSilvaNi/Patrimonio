using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    [Serializable]
    public class TransactionViewModel
    {
        public int Id {get;set;}
        public int ModuleId { get; set; }
        public bool Status {get;set;}
        public string Initial {get;set;}
        public string Description {get;set;}
        public string Path {get;set;}
        public int?  TypeTransactionId {get;set;}
    }

    public class TransactionTelaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Módulo é obrigatório")]
        public int IdModulo { get; set; }

        [Display(Name="Módulo")]
        public string NomeModulo { get; set; }

        [StringLength(300, ErrorMessage = "Tamanho máximo de 300 caracteres.")]
        public string Sigla { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de 100 caracteres.")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Caminho")]
        [StringLength(255, ErrorMessage = "Tamanho máximo de 255 caracteres.")]
        public string Caminho { get; set; }

        [Required(ErrorMessage = "Campo Tipo de Transação é obrigatório")]
        public int IdTipoTransacao { get; set; }

        [Display(Name ="Tipo de Transação")]
        public string DescricaoTipoTransacao { get; set; }

        public bool Status { get; set; }
    }
}