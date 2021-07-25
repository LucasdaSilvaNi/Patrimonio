using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class AuxiliaryAccountViewModel
    {

        public int Id { get; set; }

        public int Code { get; set; }

        public int? BookAccount { get; set; }

        public string Description { get; set; }

        public int CodigoContaDepreciada { get; set; }

        public string DescricaoContaDepreciada { get; set; }

        public string CodigoContaContabil { get; set; }

        public string TipoMovimentacaoContabilizaSP { get; set; }

        public AuxiliaryAccountViewModel Create(AuxiliaryAccount auxiliary)
        {
            return new AuxiliaryAccountViewModel()
            {
                Id = auxiliary.Id,
                Code = auxiliary.Code,
                Description = auxiliary.Description,
                BookAccount = auxiliary.BookAccount,
                CodigoContaContabil = auxiliary.ContaContabilApresentacao,
                CodigoContaDepreciada = auxiliary.RelatedDepreciationAccount == null ? -1 : auxiliary.RelatedDepreciationAccount.Code,
                DescricaoContaDepreciada = auxiliary.RelatedDepreciationAccount == null ?  String.Empty : auxiliary.RelatedDepreciationAccount.Description,
                TipoMovimentacaoContabilizaSP = auxiliary.TipoMovimentacaoContabilizaSP
            };
       }

        public static IList<AuxiliaryAccountViewModel> Ordenar(string ordenacao, string ordenacaoAscDesc, IList<AuxiliaryAccountViewModel> auxiliaryTable)
        {
            IList<AuxiliaryAccountViewModel> _auxiliaryTable = new List<AuxiliaryAccountViewModel>();
            try
            {
                // Sorting
                switch (ordenacao)
                {
                    case "0":
                        _auxiliaryTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? auxiliaryTable.OrderByDescending(p => p.Code).ToList() : auxiliaryTable.OrderBy(p => p.Code).ToList();
                        break;
                    case "1":
                        _auxiliaryTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? auxiliaryTable.OrderByDescending(p => p.Description).ToList() : auxiliaryTable.OrderBy(p => p.Description).ToList();
                        break;
                    case "2":
                        _auxiliaryTable = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? auxiliaryTable.OrderByDescending(p => p.BookAccount).ToList() : auxiliaryTable.OrderBy(p => p.BookAccount).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            return auxiliaryTable;
        }
    }
}