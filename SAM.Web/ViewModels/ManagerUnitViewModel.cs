using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class ManagerUnitViewModel : ManagerUnit
    {
        public ManagerUnitViewModel() { }
        public static ManagerUnitViewModel SetViewModel(ManagerUnit managerUnit)
        {
            if (managerUnit == null)
                throw new Exception("ManagerUnit nulo");

            return new ManagerUnitViewModel()
            {
                Code = managerUnit.Code,
                Description = managerUnit.Description,
                Id = managerUnit.Id,
                BudgetUnitId = managerUnit.BudgetUnitId,
                FlagIntegracaoSiafem = managerUnit.FlagIntegracaoSiafem,
                Status = managerUnit.Status,
                ManagmentUnit_YearMonthStart = managerUnit.ManagmentUnit_YearMonthStart,
                ManagmentUnit_YearMonthReference = managerUnit.ManagmentUnit_YearMonthReference

            };
        }
        [Required]
        [Display(Name = "Orgao")]
        [Range(1, Int32.MaxValue, ErrorMessage = "O Campo Órgão é obrigatório")]
        public int InstitutionId { get; set; }
        public bool AlteracaoIntegracaoSiafem { get; set; }
        public string BudgetUnitCode { get; set; }
        public string BudgetUnitDescription { get; set; }
        public int OrgaoSelecionado { get; set; }
        public int UOSelecionado { get; set; }
        public string CodigoSelecionado { get; set; }
        public string DescricaoInicial { get; set; }
        public string FlagSiafem { get; set; }

    }


    [NotMapped]
    public partial class ManagerUnitMobileViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}