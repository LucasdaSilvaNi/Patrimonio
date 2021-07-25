using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class OutSourcedViewModel
    {
        public int Id { get; set; }

        public string InstitutionCode { get; set; }

        public string InstitutionDescription { get; set; }

        public string BudgetUnitCode { get; set; }

        public string BudgetUnitDescription { get; set; }

        public string CPFCNPJ { get; set; }

        public string Name { get; set; }

    
    public static OutSourcedViewModel GetInstancia(OutSourced outsourced)
        {
            return new OutSourcedViewModel()
            {
                Id = outsourced.Id,
                Name = outsourced.Name,
                CPFCNPJ = outsourced.CPFCNPJ,
                InstitutionCode = outsourced.RelatedInstitution.Code,
                InstitutionDescription = outsourced.RelatedInstitution.Description,
                BudgetUnitCode = outsourced.RelatedBudgetUnit.Code,
                BudgetUnitDescription = outsourced.RelatedBudgetUnit.Description,
            };

        }


    public static IList<OutSourcedViewModel> Ordenar(string ordenacao, string ordenacaoAscDesc, IList<OutSourcedViewModel> outSourcedTable)
    {
        IList<OutSourcedViewModel> _outSourcedTable = new List<OutSourcedViewModel>();

        try
        {
            // Sorting
            switch (ordenacao)
            {
                case "1":
                    _outSourcedTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? outSourcedTable.OrderByDescending(p => p.InstitutionCode).ToList() : outSourcedTable.OrderBy(p => p.InstitutionCode).ToList();
                    break;
                case "2":
                    _outSourcedTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? outSourcedTable.OrderByDescending(p => p.InstitutionDescription).ToList() : outSourcedTable.OrderBy(p => p.InstitutionDescription).ToList();
                    break;
                case "3":
                    _outSourcedTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? outSourcedTable.OrderByDescending(p => p.BudgetUnitCode).ToList() : outSourcedTable.OrderBy(p => p.BudgetUnitCode).ToList();
                    break;
                case "4":
                    _outSourcedTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? outSourcedTable.OrderByDescending(p => p.BudgetUnitDescription).ToList() : outSourcedTable.OrderBy(p => p.BudgetUnitDescription).ToList();
                    break;
                case "5":
                    _outSourcedTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? outSourcedTable.OrderByDescending(p => p.Name).ToList() : outSourcedTable.OrderBy(p => p.Name).ToList();
                    break;
                default:
                    _outSourcedTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? outSourcedTable.OrderByDescending(p => p.CPFCNPJ).ToList() : outSourcedTable.OrderBy(p => p.CPFCNPJ).ToList();
                    break;

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex.InnerException);
        }

        return _outSourcedTable;
        }
    }
}