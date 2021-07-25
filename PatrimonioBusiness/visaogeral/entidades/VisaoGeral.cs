using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.visaogeral.entidades
{
    public class VisaoGeral
    {

        public VisaoGeral()
        {
           
        }
 
        public int ManagerUnitId { get; set; }
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
        public String ContaContabil { get; set; }
        public decimal ValorDeAquisicao { get; set; }
        public decimal? DepreciacaoAcumulada { get; set; }
        public decimal? ValorAtual { get; set; }
        public decimal? DepreciacaoMensal { get; set; }
        public short? VidaUtil { get; set; }
        public String DataAquisicao { get; set; }
        public String DataIncorporacao { get; set; }
        public String Empenho { get; set; }
        public String NumeroDocumento { get; set; }
        public String UltimoHistorico { get; set; }
        public String DataUltimoHistorico { get; set; }
        public bool? flagAcervo { get; set; }
        public bool? flagTerceiro { get; set; }
        public bool? flagDecreto { get; set; }
        public String TipoBp { get; set; }
        public bool Status { get; set; }
        public int MovementTypeId { get; set; }
        public String DescricaoContaContabil { get; set; }
        public string NumeroDocumentoAtual { get; set; }
        public bool EntreOsTiposDeMovimento { get; set; }

        public int? IdAReclassificar { get; set; }
        public bool AReclassificar { get; set; }
        public int? LifeCycle { get; set; }

        public static List<VisaoGeral> Ordenar(string ordenacao, string ordenacaoAscDesc, List<VisaoGeral> tableVisaoGeral)
        {

            IEnumerable<VisaoGeral> ordenado = null;

            try
            {
                // Sorting
                switch (ordenacao)
                {
                    case "1":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.Sigla).ThenByDescending(p => p.Chapa) : tableVisaoGeral.OrderBy(p => p.Sigla).ThenBy(p => p.Chapa);
                        break;
                    case "2":
                        ordenado = ordenacaoAscDesc.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? tableVisaoGeral.OrderByDescending(p => p.GrupoMaterial).ThenByDescending(p => p.Item) : tableVisaoGeral.OrderBy(p => p.GrupoMaterial).ThenBy(p => p.Item);
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
                                   tableVisaoGeral.OrderBy(p => p.flagAcervo == true).ThenBy(p => p.flagDecreto == true).ThenBy(p => p.flagTerceiro == true).ThenBy(p => p.flagAcervo != true && p.flagTerceiro != true && p.flagDecreto != true) :
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
