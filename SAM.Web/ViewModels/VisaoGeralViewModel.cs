using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class VisaoGeralViewModel
    {
        [Obsolete("", false)]
        public int InitialId { get; set; }
        [Obsolete("", false)]
        public String NumberIdentification { get; set; }
        [Obsolete("", false)]
        public Decimal ValueAcquisition { get; set; }
        [Obsolete("", false)]
        public Decimal ValueUpdate { get; set; }
        [Obsolete("", false)]
        public Decimal DepreciationByMonth { get; set; }
        [Obsolete("", false)]
        public int MaterialItemCode { get; set; }
        [Obsolete("", false)]
        public int ShortDescriptionItemId { get; set; }
        [Obsolete("", false)]
        public int AssetMovementId { get; set; }
        [Obsolete("", false)]
        public int InstitutionId { get; set; }
        [Obsolete("", false)]
        public int BudgetUnitId { get; set; }
        [Obsolete("", false)]
        public int ManagerUnitId { get; set; }
        [Obsolete("", false)]
        public int? AdministrativeUnitId { get; set; }
        [Obsolete("", false)]
        public int? SectionId { get; set; }
        [Obsolete("", false)]
        public int? ResponsibleId { get; set; }
        [Obsolete("", false)]
        public String InitialName { get; set; }
        [Obsolete("", false)]
        public String ShortDescriptionItemDescription { get; set; }
        [Obsolete("", false)]
        public String NameManagerReduced { get; set; }
        [Obsolete("", false)]
        public String BudgetUnitCode { get; set; }
        [Obsolete("", false)]
        public String ManagerUnitCode { get; set; }
        [Obsolete("", false)]
        public int? AdministrativeUnitCode { get; set; }
        [Obsolete("", false)]
        public int? SectionCode { get; set; }
        [Obsolete("", false)]
        public String SectionDescription { get; set; }
        [Obsolete("", false)]
        public String ResponsibleName { get; set; }
        [Obsolete("", false)]
        public int? AssetTransferenciaId { get; set; }
        [Obsolete("", false)]
        public int? AuxiliaryAccountId { get; set; }
        // Fim das variáveis a serem apagadas
        
        public int Id { get; set; }
        public String Sigla { get; set; }
        public String Chapa { get; set; }
        public String SiglaChapa { get; set; }
        public int GrupoMaterial { get; set; }
        public int Item { get; set; }
        public String GrupoItem { get; set; }
        public String DescricaoDoItem { get; set; }
        public String Orgao { get; set; }
        public String UO { get; set; }

        public String UGE { get; set; }

        public int? UA { get; set; }
        public String DescricaoDaDivisao { get; set; }
        public String Responsavel { get; set; }
        public int? ContaContabil { get; set; }
        public decimal ValorDeAquisicao { get; set; }
        public decimal? DepreciacaoAcumulada { get; set; }
        public decimal? ValorAtual { get; set; }
        public decimal? DepreciacaoMensal { get; set; }
        public String VidaUtil { get; set; }
        public String Empenho { get; set; }
        public String NumeroDocumento { get; set; }
        public bool? flagAcervo { get; set; }
        public bool? flagTerceiro { get; set; }
        public bool? flagDecreto { get; set; }
        public String TipoBp { get; set; }
        public bool Status { get; set; }
        public int MovementTypeId { get; set; }
        public String DescricaoContaContabil { get; set; }
        public string NumeroDocumentoAtual { get; set; }
        public bool EntreOsTiposDeMovimento { get; set; }

        public int LifeCycle { get; set; }
        public int? AssetStartId { get; set; }
        public static List<VisaoGeralViewModel> Ordenar(string ordenacao, string ordenacaoAscDesc, List<VisaoGeralViewModel> tableVisaoGeral)
        {

            IEnumerable<VisaoGeralViewModel> ordenado = null;

            try
            {
                // Sorting
                switch (ordenacao)
                {
                    case "1":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.Sigla).ThenByDescending(p=>p.Chapa) : tableVisaoGeral.OrderBy(p => p.Sigla).ThenBy(p => p.Chapa);
                        break;
                    case "2":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.GrupoMaterial).ThenByDescending(p=> p.Item) : tableVisaoGeral.OrderBy(p => p.GrupoMaterial).ThenBy(p => p.Item);
                        break;
                    case "3":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.DescricaoDoItem) : tableVisaoGeral.OrderBy(p => p.DescricaoDoItem);
                        break;
                    case "4":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.Orgao) : tableVisaoGeral.OrderBy(p => p.Orgao);
                        break;
                    case "5":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.UO) : tableVisaoGeral.OrderBy(p => p.UO);
                        break;
                    case "6":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.UGE) : tableVisaoGeral.OrderBy(p => p.UGE);
                        break;
                    case "7":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.UA) : tableVisaoGeral.OrderBy(p => p.UA);
                        break;
                    case "8":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.DescricaoDaDivisao) : tableVisaoGeral.OrderBy(p => p.DescricaoDaDivisao);
                        break;
                    case "9":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.Responsavel) : tableVisaoGeral.OrderBy(p => p.Responsavel);
                        break;
                    case "10":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.ContaContabil) : tableVisaoGeral.OrderBy(p => p.ContaContabil);
                        break;
                    case "11":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.ValorDeAquisicao) : tableVisaoGeral.OrderBy(p => p.ValorDeAquisicao);
                        break;
                    case "12":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.DepreciacaoAcumulada) : tableVisaoGeral.OrderBy(p => p.DepreciacaoAcumulada);
                        break;
                    case "13":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.ValorAtual) : tableVisaoGeral.OrderBy(p => p.ValorAtual);
                        break;
                    case "14":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.DepreciacaoMensal) : tableVisaoGeral.OrderBy(p => p.DepreciacaoMensal);
                        break;
                    case "15":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.VidaUtil) : tableVisaoGeral.OrderBy(p => p.VidaUtil);
                        break;
                    case "16":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.Empenho) : tableVisaoGeral.OrderBy(p => p.Empenho);
                        break;
                    case "17":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.NumeroDocumento) : tableVisaoGeral.OrderBy(p => p.NumeroDocumento);
                        break;
                    case "18":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ?
                                   tableVisaoGeral.OrderBy(p=> p.flagAcervo == true).ThenBy(p => p.flagDecreto == true).ThenBy(p=> p.flagTerceiro == true).ThenBy(p => p.flagAcervo != true && p.flagTerceiro != true && p.flagDecreto != true) :
                                   tableVisaoGeral.OrderBy(p => p.flagAcervo != true && p.flagTerceiro != true && p.flagDecreto != true).ThenBy(p => p.flagTerceiro == true).ThenBy(p => p.flagDecreto == true).ThenBy(p => p.flagAcervo == true);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

            if (ordenado != null)
                return ordenado.ToList();
            else
                return tableVisaoGeral;
        }

    }
}