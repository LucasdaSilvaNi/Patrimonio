using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    public class BudgetUnitTableViewModel
    {
        public int Id { get; set; }

        public string InstitutionCode { get; set; }

        public string InstitutionDescription { get; set; }

        public string BudgetUnitCode { get; set; }

        public string BudgetUnitDescription { get; set; }

        public bool Status { get; set; }

        public static BudgetUnitTableViewModel GetInstancia(BudgetUnit budgetUnit)
        {
            return new BudgetUnitTableViewModel()
            {
                Id = budgetUnit.Id,
                InstitutionCode = budgetUnit.RelatedInstitution.Code,
                InstitutionDescription = budgetUnit.RelatedInstitution.Description,
                BudgetUnitCode = budgetUnit.Code,
                BudgetUnitDescription = budgetUnit.Description,
                Status = budgetUnit.Status
            };
        }

        public static IList<BudgetUnitTableViewModel> Ordenar(string ordenacao, string ordenacaoAscDesc, IList<BudgetUnitTableViewModel> budgetUnitTable)
        {
            IList<BudgetUnitTableViewModel> _tableBudgetUnit = new List<BudgetUnitTableViewModel>();

            try
            {
                // Sorting
                switch (ordenacao)
                {
                    case "1":
                        _tableBudgetUnit = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? budgetUnitTable.OrderByDescending(p => p.InstitutionCode).ToList() : budgetUnitTable.OrderBy(p => p.InstitutionCode).ToList();
                        break;
                    case "2":
                        _tableBudgetUnit = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? budgetUnitTable.OrderByDescending(p => p.InstitutionDescription).ToList() : budgetUnitTable.OrderBy(p => p.InstitutionDescription).ToList();
                        break;
                    case "3":
                        _tableBudgetUnit = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? budgetUnitTable.OrderByDescending(p => p.BudgetUnitCode).ToList() : budgetUnitTable.OrderBy(p => p.BudgetUnitCode).ToList();
                        break;
                    case "4":
                        _tableBudgetUnit = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? budgetUnitTable.OrderByDescending(p => p.BudgetUnitDescription).ToList() : budgetUnitTable.OrderBy(p => p.BudgetUnitDescription).ToList();
                        break;
                    case "5":
                        _tableBudgetUnit = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? budgetUnitTable.OrderByDescending(p => p.Status).ToList() : budgetUnitTable.OrderBy(p => p.Status).ToList();
                        break;
                    default:
                        _tableBudgetUnit = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? budgetUnitTable.OrderByDescending(p => p.Id).ToList() : budgetUnitTable.OrderBy(p => p.Id).ToList();
                        break;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return _tableBudgetUnit;
        }

    }
}