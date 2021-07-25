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
    public partial class AuditoriaIntegracaoViewModel
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data Envio")]
        public DateTime DataHoraEnvio { get; set; }
        [Display(Name = "Data Envio XML")]
        public string DataHoraEnvioSIAFEM { get; set; }
        [Display(Name = "Data Retorno XML")]
        public string DataHoraRetornoSIAFEM { get; set; }
        [Display(Name = "Data Movimento")]
        public string DataMovimento { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data Retorno")]
        public DateTime? DataHoraRetorno { get; set; }
        [Display(Name = "XML Enviado")]
        public string MsgEstimuloWS { get; set; }
        [Display(Name = "XML Retornado (VHI)")]
        public string MsgRetornoWS { get; set; }
        [Display(Name = "Sistema Externo: ContabilizaSP")]
        public string NomeSistema { get; set; }
        [Display(Name = "Chave/CPF Usuário SAM")]
        public string UsuarioSAM { get; set; }
        [Display(Name ="Chave/CPF Usuário SIAFEM")]
        public string UsuarioSistemaExterno { get; set; }
        public int InstitutionId { get; set; }
        public int BudgetUnitId { get; set; }
        public int ManagerUnitId { get; set; }
        public string Orgao { get; set; }
        public string UO { get; set; }
        public string UGE { get; set; }


        public int InstitutionIdAux { get; set; }
        public int BudgetUnitIdAux { get; set; }
        public int ManagerUnitIdAux { get; set; }
        public string ContaContabil { get; set; }

        [Display(Name = "Token SAM-Patrimonio")]
        public Guid TokenAuditoriaIntegracao { get; set; }

        /* Campos do XML Envio */
        [Display(Name = "Chave SIAFMONITORA")]
        public string DocumentoId { get; set; }
        [Display(Name = "Tipo Movimento (ContabilizaSP)")]
        public string TipoMovimento { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data Movimento")]
        public DateTime? Data { get; set; }
        [Display(Name = "UGE")]
        public string UgeOrigem { get; set; }
        [Display(Name = "Gestão")]
        public string Gestao { get; set; }
        [Display(Name = "Tipo Entrada / Saída / Reclassificação / Depreciação (ContabilizaSP)")]
        public string Tipo_Entrada_Saida_Reclassificacao_Depreciacao { get; set; }
        [Display(Name = "CPF / CNPJ / UGE Favorecida (ContabilizaSP)")]
        public string CpfCnpjUgeFavorecida { get; set; }
        [Display(Name = "Gestão Favorecida (ContabilizaSP)")]
        public string GestaoFavorecida { get; set; }
        [Display(Name = "Item (Material) (ContabilizaSP)")]
        public string Item { get; set; }
        [Display(Name = "Tipo Estoque (ContabilizaSP)")]
        public string TipoEstoque { get; set; }
        [Display(Name = "Estoque (ContabilizaSP)")]
        public string Estoque { get; set; }
        [Display(Name = "Estoque Origem (ContabilizaSP)")]
        public string EstoqueDestino { get; set; }
        [Display(Name = "Estoque Destino (ContabilizaSP)")]
        public string EstoqueOrigem { get; set; }
        [Display(Name = "Tipo Movimentação (ContabilizaSP)")]
        public string TipoMovimentacao { get; set; }
        [Display(Name = "Valor Total (NL) (ContabilizaSP)")]
        public decimal? ValorTotal { get; set; }
        [Display(Name = "Controle Específico (ContabilizaSP)")]
        public string ControleEspecifico { get; set; }
        [Display(Name = "Controle Específico (ENTRADA)")]
        public string ControleEspecificoEntrada { get; set; }
        [Display(Name = "Controle Específico (SAÍDA)")]
        public string ControleEspecificoSaida { get; set; }
        [Display(Name = "Fonte  Recurso")]
        public string FonteRecurso { get; set; }
        [Display(Name = "NL de Estorno? (S/N)")]
        public string NLEstorno { get; set; }
        [Display(Name = "Empenho")]
        public string Empenho { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Observação (SIAFEM)")]
        public string Observacao { get; set; }
        [Display(Name = "Nota Fiscal (Documento SAM)")]
        public string NotaFiscal { get; set; }
        [Display(Name = "Item Material")]
        public string ItemMaterial { get; set; }
        /* Campos XML Retorno */
        [Display(Name = "NL SIAFEM")]
        public string NotaLancamento { get; set; }
        [Display(Name = "Erro SIAFEM")]
        public string MsgErro { get; set; }
        /* Campos de status */
        public bool RegistroOK {
                                    get
                                        {
                                            bool registroOK = false;

                                            registroOK = (   (!String.IsNullOrWhiteSpace(this.NotaLancamento))
                                                          && (this.NotaLancamento.Contains("NL"))
                                                          && (String.IsNullOrWhiteSpace(this.MsgErro)));

                                            return registroOK;
                                        }
                                    //set
                                    //    {
                                    //        this.RegistroOK = value;
                                    //    }
                                }
        public bool NLManual { get; set; }
        public bool PossuiPendenciaNL { get; set; }
        public bool PossuiInconsistencias {
                                            get {
                                                    bool possuiInconsistencias = false;

                                                    possuiInconsistencias = (   (String.IsNullOrWhiteSpace(this.NotaLancamento))
                                                                             && (!String.IsNullOrWhiteSpace(this.MsgErro)));


                                                    return possuiInconsistencias;
                                                }
                                          }
        public bool TemPermissao { get; set; }
    }
}