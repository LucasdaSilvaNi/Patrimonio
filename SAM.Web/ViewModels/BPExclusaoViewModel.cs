using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.ViewModels
{
    public class BPExclusaoViewModel
    {
        public int Id { get; set; }
        public int IdOrgaoAtual { get; set; }
        public int IdUOAtual { get; set; }
        public int IdUGEAtual { get; set; }
        public int? IdUAtual { get; set; }
        public int UACode { get; set; }
        public string UGECode { get; set; }
        public int? Responsavel { get; set; }
        public int LoginAcao { get; set; }
        public int IdEstadoDeConservacao { get; set; }
        public string EstadoDeConservacao { get; set; }
        public string Sigla { get; set; }
        public string Chapa { get; set; }
        public string NomeResponsavel { get; set; }
        public string Cpf { get; set; }
        public string Tipo { get; set; }
        public string NotaLancamento { get; set; }
        public string NotaLancamentoEstorno { get; set; }
        public string NotaLancamentoDepreciacao { get; set; }
        public string NotaLancamentoDepreciacaoEstorno { get; set; }
        public string NotaLancamentoReclassificacao { get; set; }
        public string NotaLancamentoReclassificacaoEstorno { get; set; }

        public virtual DateTime DataIncorporacao { get; set; }

        public DateTime DataAcao { get; set; }


        [Display(Name = "Item Material")]
        public int ItemMaterial { get; set; }

        [Display(Name = "Grupo Material")]
        public int GrupoMaterial { get; set; }
        public string DescricaoDoGrupo { get; set; }

        [Display(Name = "Valor de Aquisição")]
        public decimal ValorDeAquisicao { get; set; }

        public DateTime DataDeAquisicaoCompleta { get; set; }

        [Display(Name = "Data de Aquisição")]
        public string ApenasDataAquisicao
        {
            get
            {
                return DataDeAquisicaoCompleta.Date.ToString("d");
            }
        }
        [Display(Name = "Descrição Resumida do Item")]
        public string DescricaoResumida { get; set; }
        public bool flagAcervo { get; set; }
        public bool flagTerceiro { get; set; }
        public bool flagDecreto { get; set; }
        public bool BPTemUmTipo { get {
                return flagAcervo || flagTerceiro || flagDecreto;
             } }
        public string Processo { get; set; }
        [Display(Name = "Observação")]
        public string Observacao { get; set; }
        public List<HistoricoBPExclusaoViewModel> historico { get; set; }
        public int? BudgetUnitId { get; internal set; }
        public int InstitutionId { get; internal set; }
    }

    public class HistoricoBPExclusaoViewModel
    {
        public DateTime DataCompletoHistorico { get; set; }
        [Display(Name = "Data do Movimento")]
        public string ApenasDataHistorico
        { get {
                return DataCompletoHistorico.Date.ToString("d");
            } }
        [Display(Name = "Tipo de Movimento")]
        public string Historico { get; set; }
        [Display(Name = "Orgao")]
        public string Orgao { get; set; }
        public string UO { get; set; }
        public string UGE { get; set; }
        public string UA { get; set; }
        [Display(Name = "Divisão")]
        public string Divisao { get; set; }
        [Display(Name = "Responsável")]
        public string Responsavel { get; set; }
        [Display(Name = "Conta Contábil")]
        public string ContaContabil { get; set; }
        [Display(Name = "Número de documento")]
        public string NumeroDocumento { get; set; }
        [Display(Name = "Valor do Reparo")]
        public decimal ValorDoReparo { get; set; }
        [Display(Name = "Observação")]
        public string Observacao { get; set; }
        [Display(Name = "CPF/CNPJ")]
        public string CPFCPNJ { get; set; }
    }
}