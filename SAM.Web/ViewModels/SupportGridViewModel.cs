using SAM.Web.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class SupportGridViewModel
    {
        [Display(Name = "N� do Chamado")]
        public int Id { get; set; }
        [Display(Name = "�rg�o")]
        public string NameManagerReduced { get; set; }
        [Display(Name = "UO")]
        public string BudgetUnitCode { get; set; }
        [Display(Name = "UGE")]
        public string ManagerUnitCode{ get; set; }
        [Display(Name = "Usu�rio")]
        public string UserDescription { get; set; }
        [Display(Name = "Data de Abertura")]
        public DateTime InclusionDate { get; set; }
        [Display(Name = "�ltima Altera��o")]
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "CPF")]
        public string UserCPF { get; set; }
        [Display(Name = "Funcionalidade")]
        public string Functionanality { get; set; }
        [Display(Name = "Tipo de Chamado")]
        public string SupportTypeDescription { get; set; }
        [Display(Name = "Status(Usu�rio)")]
        public string SupportStatusUserDescription { get; set; }

        public int SupportStatusProdespId { get; set; }

        [Display(Name = "Status(Prodesp)")]
        public string SupportStatusProdespDescription { get; set; }
        [Display(Name = "Respons�vel")]
        public string Responsavel { get; set; }
        [Display(Name = "Data de Fechamento")]
        public DateTime? CloseDate { get; set; }

        //Enum que indica se o �ltimo atendimento foi realizado pela Prodesp ou se foi efetuada pelo Usu�rio
        public int UltimoAtendimento { get; set; }
    }
}
