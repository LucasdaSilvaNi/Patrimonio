using LinqKit;
using Sam.Common;
using Sam.Common.Util;
using Sam.Integracao.SIAF.Core;
using Sam.Integracao.SIAF.Mensagem.Constantes;
using Sam.Integracao.SIAF.Mensagem.Enum;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using SAM.Web.Models;
using SAM.Web.Common.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Xml;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
//using TipoMovimentacaoPatrimonial = Sam.Integracao.SIAF.Mensagem.Enum.EnumMovimentType;
using TipoMovimentacaoPatrimonial = SAM.Web.Common.Enum.EnumMovimentType;
using EnumMovimentType = SAM.Web.Common.Enum.EnumMovimentType;
using Sam.Integracao.SIAF.Mensagem.Interface;
using System.Globalization;
using SAM.Web.ViewModels;
using System.Web.UI.WebControls;

namespace SAM.Web.Controllers
{
    public partial class IntegracaoContabilizaSPController : BaseController
    {
        private SAMContext contextoCamadaDados = new SAMContext();
        private RelationshipUserProfile rup = new RelationshipUserProfile();
        internal static IDictionary<int, int> relacaoTipoMovimentacao_TiposAgrupamentoMovimentacaoPatrimonial = null;


        #region Constantes de suporte a retorno ao usuario
        internal static string fmtMsgGeracaoNLsLiquidacao = @"Gerada NL ""{0}"" no SIAFEM";
        internal static string fmtMsgGeracaoNLsLiquidacaoEReclassificacao = @"Geradas NLs números ""{0}"" (Liquidação) e ""{1}"" (Depreciação) no SIAFEM";
        internal static string fmtMsgGravacaoExecutadaSucesso = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" salva com sucesso!{2}";
        internal static string fmtMsgEstornoExecutadoSucesso = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" estornada com sucesso!{2}";
        internal static string fmtMsgGravacaoExecutadaComPendencias = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" salva com pendências!{2}";
        internal static string fmtMsgEstornoExecutadoComPendencias = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" estornada com pendências!{2}";
        internal static string fmtMsgGravacaoExecutadaSucessoMovimentacaoSemContraPartidaContabilizaSP = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" sem contrapartida FINANCEIRA salva com sucesso!{2}";
        internal static string fmtMsgEstornoExecutadoSucessoMovimentacaoSemContraPartidaContabilizaSP = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" sem contrapartida FINANCEIRA estornada com sucesso!{2}";
        internal static string fmtMsgGravacaoExecutadaComNLNoSIAFEM = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" gerou NL ""{2}"" no SIAFEM!";
        internal static string fmtMsgGravacaoExecutadaComErroSIAFEM = @"Movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" retornou erro SIAFEM ""{2}""!";
        internal static string fmtMsgEstornoExecutadoComNLNoSIAFEM = @"Estorno da movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" gerou NL ""{2}"" no SIAFEM!";
        internal static string fmtMsgEstornoExecutadoComErroSIAFEM = @"Estorno da movimentação Patrimonial do tipo ""{0}"", documento ""{1}"" retornou erro SIAFEM ""{2}""!";

        internal static string fmtMsgGravacaoFechamentoMensalIntegradoExecutadaComNLNoSIAFEM = @"Fechamento Mensal Integrado, documento ""{1}"" gerou NL ""{2}"" no SIAFEM!";
        internal static string fmtMsgGravacaoFechamentoMensalIntegradoExecutadaComErroSIAFEM = @"Fechamento Mensal Integrado, documento ""{1}"" retornou erro SIAFEM ""{2}""!";
        internal static string fmtMsgEstornoFechamentoMensalIntegradoExecutadoComNLNoSIAFEM = @"Estorno de Fechamento Mensal Integrado, documento ""{1}"" gerou NL ""{2}"" no SIAFEM!";
        internal static string fmtMsgEstornoFechamentoMensalIntegradoExecutadoComErroSIAFEM = @"Estorno de Fechamento Mensal Integrado, documento ""{1}"" retornou erro SIAFEM ""{2}""!";
        #endregion Constantes de suporte a retorno ao usuario

        #region Listagens de apoio
        static IDictionary<string, AuxiliaryAccount> dicContasContabeis = new Dictionary<string, AuxiliaryAccount>();
        static IDictionary<long, string> dicContasContabeis_TipoMovimentacaoContabilizaSP = new Dictionary<long, string>();
        private readonly int[] tiposDeIncorporacaoMovimentacaoComCpfCnpj = new int[] {
                                                                                         (int)TipoMovimentacaoPatrimonial.IncorpDoacaoConsolidacao
                                                                                       , (int)TipoMovimentacaoPatrimonial.IncorpDoacaoMunicipio
                                                                                       , (int)TipoMovimentacaoPatrimonial.IncorpDoacaoOutrosEstados
                                                                                       , (int)TipoMovimentacaoPatrimonial.IncorpDoacaoUniao
                                                                                       , (int)TipoMovimentacaoPatrimonial.MovDoacaoConsolidacao
                                                                                       , (int)TipoMovimentacaoPatrimonial.MovDoacaoMunicipio
                                                                                       , (int)TipoMovimentacaoPatrimonial.MovDoacaoOutrosEstados
                                                                                       , (int)TipoMovimentacaoPatrimonial.MovDoacaoUniao
                                                                                     };
        private readonly int[] tiposDeIncorporacaoMovimentacaoQueFavorecemDestino = new int[] {
                                                                                                  (int)TipoMovimentacaoPatrimonial.IncorpDoacaoConsolidacao
                                                                                                , (int)TipoMovimentacaoPatrimonial.IncorpDoacaoIntraNoEstado
                                                                                                , (int)TipoMovimentacaoPatrimonial.IncorpDoacaoMunicipio
                                                                                                , (int)TipoMovimentacaoPatrimonial.IncorpDoacaoOutrosEstados
                                                                                                , (int)TipoMovimentacaoPatrimonial.IncorpDoacaoUniao
                                                                                                , (int)TipoMovimentacaoPatrimonial.IncorpTransferenciaMesmoOrgaoPatrimoniado
                                                                                                , (int)TipoMovimentacaoPatrimonial.IncorpTransferenciaOutroOrgaoPatrimoniado
                                                                                                , (int)TipoMovimentacaoPatrimonial.IncorpIntegracaoSAMEstoque_SAMPatrimonio_OutraUGE
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovDoacaoConsolidacao
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovDoacaoIntraNoEstado
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovDoacaoMunicipio
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovDoacaoOutrosEstados
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovDoacaoUniao
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovTransferenciaMesmoOrgaoPatrimoniado
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovTransferenciaOutroOrgaoPatrimoniado
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovSaidaInservivelUGEDoacao
                                                                                                , (int)TipoMovimentacaoPatrimonial.MovSaidaInservivelUGETransferencia
                                                                                            };

        private readonly int[] tiposDeIncorporacaoMovimentacaoDeTransferenciaOuDoacao = new int[] {
                                                                                                      (int)TipoMovimentacaoPatrimonial.IncorpDoacaoIntraNoEstado
                                                                                                    , (int)TipoMovimentacaoPatrimonial.IncorpTransferenciaMesmoOrgaoPatrimoniado
                                                                                                    , (int)TipoMovimentacaoPatrimonial.IncorpTransferenciaOutroOrgaoPatrimoniado
                                                                                                    , (int)TipoMovimentacaoPatrimonial.IncorpIntegracaoSAMEstoque_SAMPatrimonio_OutraUGE
                                                                                                    , (int)TipoMovimentacaoPatrimonial.MovDoacaoIntraNoEstado
                                                                                                    , (int)TipoMovimentacaoPatrimonial.MovTransferenciaMesmoOrgaoPatrimoniado
                                                                                                    , (int)TipoMovimentacaoPatrimonial.MovTransferenciaOutroOrgaoPatrimoniado
                                                                                                    , (int)TipoMovimentacaoPatrimonial.MovSaidaInservivelUGEDoacao
                                                                                                    , (int)TipoMovimentacaoPatrimonial.MovSaidaInservivelUGETransferencia
                                                                                                  };
        #endregion Listagens de apoio

        #region Mini-caches (poupar idas-vindas banco de dados)
        public void InitAuxiliaryAccountsCache()
        {
            var contextoCamadaDados = new SAMContext();
            BaseController.InitDisconnectContext(ref contextoCamadaDados);
            if (!dicContasContabeis.HasElements())
                contextoCamadaDados.AuxiliaryAccounts.Where(contaContabil => contaContabil.Status == true)
                                                     .ToList()
                                                         .ForEach(contaContabil =>
                                                                                    {
                                                                                        System.Diagnostics.Debug.WriteLine("Conta Contabil: " + contaContabil.ContaContabilApresentacao);
                                                                                        if (contaContabil.RelationshipAuxiliaryAccountItemGroups.IsNull())
                                                                                        {
                                                                                            contaContabil.RelationshipAuxiliaryAccountItemGroups = contextoCamadaDados.RelationshipAuxiliaryAccountItemGroups.Where(relacaoContaContabilGrupoMaterialConsultada => relacaoContaContabilGrupoMaterialConsultada.AuxiliaryAccountId == contaContabil.Id)
                                                                                                                                                                                                         .ToList();

                                                                                            contaContabil.RelationshipAuxiliaryAccountItemGroups.ToList().ForEach(relacaoContaContabilGrupoMaterial =>
                                                                                            {
                                                                                                if (relacaoContaContabilGrupoMaterial.RelatedMaterialGroup.IsNull())
                                                                                                    relacaoContaContabilGrupoMaterial.RelatedMaterialGroup = contextoCamadaDados.MaterialGroups.Where(grupoMaterial => grupoMaterial.Id == relacaoContaContabilGrupoMaterial.MaterialGroupId).FirstOrDefault();

                                                                                                if (relacaoContaContabilGrupoMaterial.RelatedAuxiliaryAccount.IsNull())
                                                                                                    relacaoContaContabilGrupoMaterial.RelatedAuxiliaryAccount = contaContabil;
                                                                                            });
                                                                                        }

                                                                                        if (contaContabil.RelatedDepreciationAccount.IsNull())
                                                                                        {
                                                                                            contaContabil.RelatedDepreciationAccount = contextoCamadaDados.DepreciationAccounts.Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabil.DepreciationAccountId)
                                                                                                                                                                           .FirstOrDefault();
                                                                                        }

                                                                                        if (!dicContasContabeis.ContainsKey(String.Format("{0}|Id:{1}", contaContabil.BookAccount.GetValueOrDefault().ToString(), contaContabil.Id)))
                                                                                            dicContasContabeis.Add(String.Format("{0}|Id:{1}", contaContabil.BookAccount.GetValueOrDefault().ToString(), contaContabil.Id), contaContabil);

                                                                                        if (!dicContasContabeis.ContainsKey(contaContabil.BookAccount.GetValueOrDefault().ToString()))
                                                                                            dicContasContabeis.Add(contaContabil.BookAccount.GetValueOrDefault().ToString(), contaContabil);

                                                                                        //if (contaContabil.RelatedDepreciationAccount.IsNotNull() && !dicContasContabeisDepreciacao_DescricaoContaDepreciacaoContabilizaSP.ContainsKey(contaContabil.RelatedDepreciationAccount.Code))
                                                                                        //    dicContasContabeisDepreciacao_DescricaoContaDepreciacaoContabilizaSP.Add(contaContabil.RelatedDepreciationAccount.Code, contaContabil.RelatedDepreciationAccount.DescricaoContaDepreciacaoContabilizaSP);
                                                                                    });
        }
        public void InitMovementTypesCache()
        {
            dicContasContabeis_TipoMovimentacaoContabilizaSP.Clear();

            var contextoCamadaDados = new SAMContext();
            BaseController.InitDisconnectContext(ref contextoCamadaDados);
            contextoCamadaDados.AuxiliaryAccounts.Where(contaContabil => contaContabil.Status == true)
                                                 .ToList()
                                                 .ForEach(contaContabil => {
                                                     if (!String.IsNullOrWhiteSpace(contaContabil.TipoMovimentacaoContabilizaSP))
                                                         if (!dicContasContabeis_TipoMovimentacaoContabilizaSP.ContainsKey(contaContabil.BookAccount.GetValueOrDefault()))
                                                             dicContasContabeis_TipoMovimentacaoContabilizaSP.Add(contaContabil.BookAccount.GetValueOrDefault(), contaContabil.TipoMovimentacaoContabilizaSP);
                                                 });
        }
        #endregion Mini-caches (poupar idas-vindas banco de dados)

        internal static bool EhEstorno(AssetMovements movimentacaoPatrimonial)
        {
            bool ehEstorno = false;

            if (movimentacaoPatrimonial.IsNotNull())
            {
                ehEstorno = (movimentacaoPatrimonial.IsNotNull() && (movimentacaoPatrimonial.DataEstorno.HasValue && movimentacaoPatrimonial.DataEstorno.Value != DateTime.MinValue) &&
                                                                    (movimentacaoPatrimonial.LoginEstorno.HasValue && movimentacaoPatrimonial.LoginEstorno.Value != 0) &&
                                                                    (movimentacaoPatrimonial.FlagEstorno.HasValue && movimentacaoPatrimonial.FlagEstorno.Value == true) &&
                                                                    (movimentacaoPatrimonial.Status == false));
            }
            else
            {
                throw new Exception("Parâmetro 'movimentacaoPatrimonial' não pode ser nulo!");
            }


            return ehEstorno;
        }

        private bool verificaUGEIntegradaSIAFEM(int ugeId)
        {
            bool ugePossuiIntegracaoAtiva = false;

            int mesReferenciaUGE = 0;
            string mesRefUGE = null;
            using (var _contextoCamadaDados = new SAMContext())
            {
                var ugeConsultada = _contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Id == ugeId && ugeSIAFEM.Status == true).FirstOrDefault();
                if (ugeConsultada.IsNotNull())
                {
                    mesRefUGE = ugeConsultada.ManagmentUnit_YearMonthReference;
                    Int32.TryParse(mesRefUGE, out mesReferenciaUGE);

                    ugePossuiIntegracaoAtiva = ((ugeConsultada.FlagIntegracaoSiafem == true) && (mesReferenciaUGE >= ugeConsultada.MesRefInicioIntegracaoSIAFEM));
                }
            }


            return ugePossuiIntegracaoAtiva;
        }

        private TipoNotaSIAF obterTipoNotaSIAFEM__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            TipoNotaSIAF tipoNotaSIAF = TipoNotaSIAF.Desconhecido;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;


            if (auditoriaIntegracaoId > 0)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                switch (registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant())
                {
                    case "ENTRADA":
                    case "SAIDA":
                    case "SAÍDA": { tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao; } break;
                    case "DEPRECIACAO":
                    case "DEPRECIAÇÃO": { tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao; } break;
                    case "RECLASSIFICACAO":
                    case "RECLASSIFICAÇÃO": { tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao; } break;
                }
            }



            return tipoNotaSIAF;
        }
        private TipoNotaSIAF obterTipoNotaSIAFEM__AuditoriaIntegracao(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            TipoNotaSIAF tipoNotaSIAF = TipoNotaSIAF.Desconhecido;


            if (registroAuditoriaIntegracao.IsNotNull() && !String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.TipoMovimento))
            {
                switch (registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant())
                {
                    case "ENTRADA":
                    case "SAIDA":
                    case "SAÍDA": { tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao; } break;
                    case "DEPRECIACAO":
                    case "DEPRECIAÇÃO": { tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao; } break;
                    case "RECLASSIFICACAO":
                    case "RECLASSIFICAÇÃO": { tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao; } break;
                }
            }



            return tipoNotaSIAF;
        }

        /// <summary>
        /// Método para geração de NL's para movimentações patrimoniais (incorporação/movimentação) que possuam lotes de BPs associados
        /// Para incorporação, a única diferença básica são as chapas dos BPs, que os demais detalhes são iguais a todos os BPs que estão sendo inseridos via lote no sistema.
        /// </summary>
        /// <param name="listaMovimentacoesPatrimoniaisEmLote"></param>
        /// <returns></returns>
        public Tuple<string, string, string, string, bool> ProcessaReenvioPendenciaNLMovimentacaoPatrimonialParaSIAFEM(int auditoriaIntegracaoId, string cpfUsuarioSAM, string cpfUsuarioSIAFEM, string senhaUsuarioSIAFEM)
        {
            this.initDadosSiafemIntegracao();
            bool integracaoSIAFEMAtivada = false;
            #region Variaveis
            Tuple<string, string, string, string, bool> objRetorno = null;
            string fmtPadraoMsgInformeAoUsuario = null;
            string msgRetornoProcessamento = null;
            string msgProcessamentoSIAFComErro = null;
            string msgProcessamentoSIAFComSucesso = null;
            bool tipoMovimentacaoPossuiContrapartidaContabil = false;
            bool incorporacaoGeraNLs = false;
            string descricaoTipoMovimentacao = null;
            string[] nlGerada = null;
            string _nlGerada = null;
            string msgRetornoEstimuloXML = null;
            string erroNotaLancamentoContabilizaSP = null;
            SortedList dadosTransacaoSIAFEM = null;
            int managerUnitId = 0;
            AuditoriaIntegracao registroAuditoriaPreExistente = null;
            string nlRetornadaContabilizaSP = null;
            @TipoNotaSIAF tipoNotaContabilizaSP = TipoNotaSIAF.NL_Liquidacao;
            string anoBaseMovimentacao = null;
            string ugeMovimentacao = null;
            string numeroDocumentoSAM = null;
            bool ehEstorno = false;
            string msgRetornoProcessamentoComErro = null;
            string msgRetornoProcessamentoSucesso = null;
            string msgErroRetornada = null;
            string descricaoTipoNL = null;
            string msgEstimuloXML = null;
            AssetMovements primeiraMovPatrimonialVinculadaAuditoria = null;
            IList<int> relacaoMovimentacoesPatrimoniaisIds = null;
            #endregion Variaveis

            //try
            {
                if (auditoriaIntegracaoId > 0)
                {
                    //Obtem auditoria SIAFEM pre-existente
                    registroAuditoriaPreExistente = this.obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);


                    if (registroAuditoriaPreExistente.IsNotNull())
                    {
                        primeiraMovPatrimonialVinculadaAuditoria = obterPrimeiraMovimentacaoVinculadaRegistroAuditoria(auditoriaIntegracaoId);
                        if (primeiraMovPatrimonialVinculadaAuditoria.IsNull())
                        {
                            //retrocompatibilidade (pendencias 'legadas'
                            //procurar linhas na tabela de amarracao (pendencias geradas pelo novo metodo de interacao com usuario, com amarracao AssetMovements/Asset/AuditoriaIntegracao)
                            var contextoCamadaDados = new SAMContext();
                            if (String.IsNullOrWhiteSpace(registroAuditoriaPreExistente.AssetMovementIds))
                            {
                                var listadeRegistrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                      .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == registroAuditoriaPreExistente.Id)
                                                                                      .ToList();
                                if (listadeRegistrosDeAmarracao.HasElements())
                                    relacaoMovimentacoesPatrimoniaisIds = listadeRegistrosDeAmarracao.Select(_registroDeAmarracao => _registroDeAmarracao.AssetMovementsId).ToArray();
                            }
                            //senao utiliza o metodo antigo (parsing da linha string com Id's separados por espcos em branco
                            else
                            {
                                relacaoMovimentacoesPatrimoniaisIds = registroAuditoriaPreExistente.AssetMovementIds.BreakLine().ToList().Select(movPatrimonialId => Int32.Parse(movPatrimonialId)).ToList();
                            }

                            //verifica se conseguiu trazer os id's das movimentacoes vinculadas a pendencia
                            if (relacaoMovimentacoesPatrimoniaisIds.HasElements())
                            {
                                var listaMovimentacoes = contextoCamadaDados.AssetMovements
                                                                            .Where(movPatrimonial => relacaoMovimentacoesPatrimoniaisIds.Contains(movPatrimonial.Id))
                                                                            .ToList();

                                primeiraMovPatrimonialVinculadaAuditoria = listaMovimentacoes.FirstOrDefault();
                            }
                        }

                        
                        if (primeiraMovPatrimonialVinculadaAuditoria.IsNotNull())
                        {
                            ehEstorno = (registroAuditoriaPreExistente.NLEstorno.ToUpperInvariant() == "S");
                            anoBaseMovimentacao = registroAuditoriaPreExistente.Data.GetValueOrDefault().Year.ToString();
                            numeroDocumentoSAM = registroAuditoriaPreExistente.NotaFiscal;

                            //codigo UGE, parametro para envio do webservice 
                            ugeMovimentacao = registroAuditoriaPreExistente.UgeOrigem;

                            //verificacao se a UGE de envio estah com integracao ativa
                            managerUnitId = primeiraMovPatrimonialVinculadaAuditoria.ManagerUnitId;
                            integracaoSIAFEMAtivada = verificaUGEIntegradaSIAFEM(managerUnitId);

                            //puxando da integracao
                            descricaoTipoMovimentacao = registroAuditoriaPreExistente.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;

                            //verificacao se TipoMovmentacao possui contrapartida no ContablizaSP
                            var tipoMovPatrimonial = obterEntidadeMovementType(primeiraMovPatrimonialVinculadaAuditoria.MovementTypeId);
                            tipoMovimentacaoPossuiContrapartidaContabil = tipoMovPatrimonial.PossuiContraPartidaContabil();
                            incorporacaoGeraNLs = (integracaoSIAFEMAtivada && tipoMovimentacaoPossuiContrapartidaContabil);
                            if (incorporacaoGeraNLs)
                            {
                                dadosTransacaoSIAFEM = new SortedList() {
                                                                      { "NL_Liquidacao", string.Empty }
                                                                    , { "NL_Depreciacao", string.Empty }
                                                                    , { "NL_Reclassificacao", string.Empty }
                                                                    , { "ErroRetornoSIAFEM", string.Empty }
                                                                    , { "ErroExceptionRuntime", string.Empty }
                                                                };

                                //monto o estimulo a partir dos dados na linha da tabela de auditoria 
                                msgEstimuloXML = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(registroAuditoriaPreExistente, ehEstorno);
                                if (!String.IsNullOrWhiteSpace(msgEstimuloXML))
                                {
                                    var procWsSiafem = new ProcessadorServicoSIAF();
                                    try
                                    {
                                        //PRE-PROCESSAMENTO XML
                                        tipoNotaContabilizaSP = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);

                                        //CHAMADA AO SISTEMA CONTABILIZA-SP/SIAFEM
                                        procWsSiafem.ConsumirWS(cpfUsuarioSIAFEM, senhaUsuarioSIAFEM, anoBaseMovimentacao, ugeMovimentacao, msgEstimuloXML, false, true);
                                        //procWsSiafem.SimulaConsumoWS(msgEstimuloXML);

                                        msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                                        if (!procWsSiafem.ErroProcessamentoWs) //se SIAFEM nao retornou erro
                                        {
                                            nlRetornadaContabilizaSP = this.obterNotaLancamentoContabilizaSP(msgRetornoEstimuloXML); //capturo a NL
                                            this.VinculacaoNLContabilizaSP(auditoriaIntegracaoId, tipoNotaContabilizaSP, nlRetornadaContabilizaSP, ehEstorno);

                                            switch (tipoNotaContabilizaSP)
                                            {
                                                case TipoNotaSIAF.NL_Liquidacao: { descricaoTipoNL = "NL_Liquidacao"; } break;
                                                case TipoNotaSIAF.NL_Reclassificacao: { descricaoTipoNL = "NL_Reclassificacao"; } break;
                                                case TipoNotaSIAF.NL_Depreciacao: { descricaoTipoNL = "NL_Depreciacao"; } break;
                                            }

                                            dadosTransacaoSIAFEM[descricaoTipoNL] = String.Format("{0}{1}", dadosTransacaoSIAFEM[descricaoTipoNL], nlRetornadaContabilizaSP);
                                        }
                                        else //se SIAFEM retornou erro...
                                        {
                                            erroNotaLancamentoContabilizaSP = procWsSiafem.ErroRetornoSimplificadoWs; //capturo o erro
                                            dadosTransacaoSIAFEM["ErroRetornoSIAFEM"] = erroNotaLancamentoContabilizaSP;
                                        }

                                        //Atualiza o registro de auditoria integracao SIAFEM pre-existente
                                        registroAuditoriaPreExistente.NotaLancamento = nlRetornadaContabilizaSP;
                                        registroAuditoriaPreExistente.MsgErro = erroNotaLancamentoContabilizaSP;
                                        registroAuditoriaPreExistente.MsgEstimuloWS = msgEstimuloXML;
                                        registroAuditoriaPreExistente.MsgRetornoWS = msgRetornoEstimuloXML;
                                        registroAuditoriaPreExistente.DataEnvio = DateTime.Now;
                                        this.atualizaAuditoriaIntegracao(registroAuditoriaPreExistente);



                                        msgRetornoEstimuloXML = null;
                                        registroAuditoriaPreExistente = null;
                                    }
                                    catch (Exception runtimeExceptionGenerica)
                                    {
                                        msgErroRetornada = (runtimeExceptionGenerica.InnerException.IsNull() ? (runtimeExceptionGenerica.Message) : (runtimeExceptionGenerica.InnerException.Message));
                                        msgErroRetornada = msgErroRetornada.Replace(Environment.NewLine, " ").Trim();

                                        var separadorVirgula = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()) ? ", " : string.Empty);
                                        dadosTransacaoSIAFEM["ErroExceptionRuntime"] = msgErroRetornada;

                                        //TODO DCBATISTA VERIFICAR MEIO GENERICO DE TRATAR EXCEPTION
                                        //TODO INSERIR ENTRADA ERROLOG AQUI
                                    }

                                    nlGerada = new string[] { (dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()), (dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()), (dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()) };
                                    var gerouNL = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()));
                                    var teveErroProcessamento = ((dadosTransacaoSIAFEM["ErroExceptionRuntime"].IsNotNull() && !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString())) || (dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].IsNotNull() && !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString())));


                                    _nlGerada = String.Join("", nlGerada);
                                    msgProcessamentoSIAFComErro = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString() ?? dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString();
                                    msgRetornoProcessamento = String.Format("{0}{1}", msgProcessamentoSIAFComSucesso, msgProcessamentoSIAFComErro);
                                }

                                msgRetornoProcessamentoComErro = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaComErroSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoComErroSIAFEM);
                                msgRetornoProcessamentoSucesso = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaComNLNoSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoComNLNoSIAFEM);
                                fmtPadraoMsgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgProcessamentoSIAFComErro) ? msgRetornoProcessamentoComErro : msgRetornoProcessamentoSucesso);
                                msgRetornoProcessamento = String.Format(fmtPadraoMsgInformeAoUsuario,
                                                                        descricaoTipoMovimentacao,
                                                                        numeroDocumentoSAM,
                                                                        msgRetornoProcessamento);
                            }

                            var houveErroSIAFEM = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()));
                            var msgErroSIAFEM = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString();


                            objRetorno = Tuple.Create<string, string, string, string, bool>(descricaoTipoMovimentacao, msgErroSIAFEM, _nlGerada, msgRetornoProcessamento, houveErroSIAFEM);
                        }
                    }
                }
            }

            return objRetorno;
        }

        public Tuple<string, string, string, string, bool> ProcessaReenvioPendenciaNLFechamentoMensalIntegradoParaSIAFEM(int auditoriaIntegracaoId, string cpfUsuarioSAM, string cpfUsuarioSIAFEM, string senhaUsuarioSIAFEM)
        {
            this.initDadosSiafemIntegracao();
            bool integracaoSIAFEMAtivada = false;
            #region Variaveis
            Tuple<string, string, string, string, bool> objRetorno = null;
            string fmtPadraoMsgInformeAoUsuario = null;
            string msgRetornoProcessamento = null;
            string msgProcessamentoSIAFComErro = null;
            string msgProcessamentoSIAFComSucesso = null;
            bool tipoMovimentacaoPossuiContrapartidaContabil = false;
            bool incorporacaoGeraNLs = false;
            string descricaoTipoMovimentacao = null;
            string[] nlGerada = null;
            string _nlGerada = null;
            string msgRetornoEstimuloXML = null;
            string erroNotaLancamentoContabilizaSP = null;
            SortedList dadosTransacaoSIAFEM = null;
            int managerUnitId = 0;
            AuditoriaIntegracao registroAuditoriaPreExistente = null;
            string nlRetornadaContabilizaSP = null;
            @TipoNotaSIAF tipoNotaContabilizaSP = TipoNotaSIAF.NL_Liquidacao;
            string anoBaseMovimentacao = null;
            string ugeMovimentacao = null;
            string numeroDocumentoSAM = null;
            bool ehEstorno = false;
            string msgRetornoProcessamentoComErro = null;
            string msgRetornoProcessamentoSucesso = null;
            string msgErroRetornada = null;
            string descricaoTipoNL = null;
            string msgEstimuloXML = null;
            //AssetMovements primeiraMovPatrimonialVinculadaAuditoria = null;
            //IList<int> relacaoMovimentacoesPatrimoniaisIds = null;
            #endregion Variaveis

            //try
            {
                if (auditoriaIntegracaoId > 0)
                {
                    //Obtem auditoria SIAFEM pre-existente
                    registroAuditoriaPreExistente = this.obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);


                    if (registroAuditoriaPreExistente.IsNotNull())
                    {
                        {
                            ehEstorno = (registroAuditoriaPreExistente.NLEstorno.ToUpperInvariant() == "S");
                            anoBaseMovimentacao = registroAuditoriaPreExistente.Data.GetValueOrDefault().Year.ToString();
                            numeroDocumentoSAM = registroAuditoriaPreExistente.NotaFiscal;

                            //codigo UGE, parametro para envio do webservice 
                            ugeMovimentacao = registroAuditoriaPreExistente.UgeOrigem;

                            //verificacao se a UGE de envio estah com integracao ativa
                            //managerUnitId = primeiraMovPatrimonialVinculadaAuditoria.ManagerUnitId;
                            managerUnitId = registroAuditoriaPreExistente.ManagerUnitId;
                            integracaoSIAFEMAtivada = verificaUGEIntegradaSIAFEM(managerUnitId);

                            //puxando da integracao
                            descricaoTipoMovimentacao = registroAuditoriaPreExistente.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;

                            //verificacao se TipoMovmentacao possui contrapartida no ContablizaSP
                            if (integracaoSIAFEMAtivada)
                            {
                                dadosTransacaoSIAFEM = new SortedList() {
                                                                      { "NL_Liquidacao", string.Empty }
                                                                    , { "NL_Depreciacao", string.Empty }
                                                                    , { "NL_Reclassificacao", string.Empty }
                                                                    , { "ErroRetornoSIAFEM", string.Empty }
                                                                    , { "ErroExceptionRuntime", string.Empty }
                                                                };

                                //monto o estimulo a partir dos dados na linha da tabela de auditoria 
                                msgEstimuloXML = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(registroAuditoriaPreExistente, ehEstorno);
                                if (!String.IsNullOrWhiteSpace(msgEstimuloXML))
                                {
                                    var procWsSiafem = new ProcessadorServicoSIAF();
                                    try
                                    {
                                        //PRE-PROCESSAMENTO XML
                                        tipoNotaContabilizaSP = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);

                                        //CHAMADA AO SISTEMA CONTABILIZA-SP/SIAFEM
                                        procWsSiafem.ConsumirWS(cpfUsuarioSIAFEM, senhaUsuarioSIAFEM, anoBaseMovimentacao, ugeMovimentacao, msgEstimuloXML, false, true);
                                        //procWsSiafem.SimulaConsumoWS(msgEstimuloXML);

                                        msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                                        if (!procWsSiafem.ErroProcessamentoWs) //se SIAFEM nao retornou erro
                                        {
                                            nlRetornadaContabilizaSP = this.obterNotaLancamentoContabilizaSP(msgRetornoEstimuloXML); //capturo a NL
                                            switch (tipoNotaContabilizaSP)
                                            {
                                                case TipoNotaSIAF.NL_Liquidacao: { descricaoTipoNL = "NL_Liquidacao"; } break;
                                                case TipoNotaSIAF.NL_Reclassificacao: { descricaoTipoNL = "NL_Reclassificacao"; } break;
                                                case TipoNotaSIAF.NL_Depreciacao: { descricaoTipoNL = "NL_Depreciacao"; } break;
                                            }

                                            dadosTransacaoSIAFEM[descricaoTipoNL] = String.Format("{0}{1}", dadosTransacaoSIAFEM[descricaoTipoNL], nlRetornadaContabilizaSP);
                                        }
                                        else //se SIAFEM retornou erro...
                                        {
                                            erroNotaLancamentoContabilizaSP = procWsSiafem.ErroRetornoSimplificadoWs; //capturo o erro
                                            dadosTransacaoSIAFEM["ErroRetornoSIAFEM"] = erroNotaLancamentoContabilizaSP;
                                        }

                                        //Atualiza o registro de auditoria integracao SIAFEM pre-existente
                                        registroAuditoriaPreExistente.NotaLancamento = nlRetornadaContabilizaSP;
                                        registroAuditoriaPreExistente.MsgErro = erroNotaLancamentoContabilizaSP;
                                        registroAuditoriaPreExistente.MsgEstimuloWS = msgEstimuloXML;
                                        registroAuditoriaPreExistente.MsgRetornoWS = msgRetornoEstimuloXML;
                                        registroAuditoriaPreExistente.DataEnvio = DateTime.Now;
                                        this.atualizaAuditoriaIntegracao(registroAuditoriaPreExistente);



                                        msgRetornoEstimuloXML = null;
                                        registroAuditoriaPreExistente = null;
                                    }
                                    catch (Exception runtimeExceptionGenerica)
                                    {
                                        msgErroRetornada = (runtimeExceptionGenerica.InnerException.IsNull() ? (runtimeExceptionGenerica.Message) : (runtimeExceptionGenerica.InnerException.Message));
                                        msgErroRetornada = msgErroRetornada.Replace(Environment.NewLine, " ").Trim();

                                        var separadorVirgula = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()) ? ", " : string.Empty);
                                        dadosTransacaoSIAFEM["ErroExceptionRuntime"] = msgErroRetornada;

                                        //TODO DCBATISTA VERIFICAR MEIO GENERICO DE TRATAR EXCEPTION
                                        //TODO INSERIR ENTRADA ERROLOG AQUI
                                    }

                                    nlGerada = new string[] { (dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()), (dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()), (dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()) };
                                    var gerouNL = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()));
                                    var teveErroProcessamento = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()));


                                    _nlGerada = String.Join("", nlGerada);
                                    msgProcessamentoSIAFComErro = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString() ?? dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString();
                                    msgRetornoProcessamento = String.Format("{0}{1}", msgProcessamentoSIAFComSucesso, msgProcessamentoSIAFComErro);
                                }

                                msgRetornoProcessamentoComErro = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaComErroSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoComErroSIAFEM);
                                msgRetornoProcessamentoSucesso = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaComNLNoSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoComNLNoSIAFEM);
                                fmtPadraoMsgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgProcessamentoSIAFComErro) ? msgRetornoProcessamentoComErro : msgRetornoProcessamentoSucesso);
                                msgRetornoProcessamento = String.Format(fmtPadraoMsgInformeAoUsuario,
                                                                        descricaoTipoMovimentacao,
                                                                        numeroDocumentoSAM,
                                                                        msgRetornoProcessamento);
                            }

                            var houveErroSIAFEM = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()));
                            var msgErroSIAFEM = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString();


                            objRetorno = Tuple.Create<string, string, string, string, bool>(descricaoTipoMovimentacao, msgErroSIAFEM, _nlGerada, msgRetornoProcessamento, houveErroSIAFEM);
                        }
                    }
                }
            }

            return objRetorno;
        }

        /// <summary>
        /// Método para geração de NL's para movimentações patrimoniais (incorporação/movimentação) que possuam lotes de BPs associados
        /// Para incorporação, a única diferença básica são as chapas dos BPs, que os demais detalhes são iguais a todos os BPs que estão sendo inseridos via lote no sistema.
        /// </summary>
        /// <param name="listaMovimentacoesPatrimoniaisEmLote"></param>
        /// <returns></returns>
        public Tuple<string, string, string, string, bool> NovoTipoDeProcessamentoMovimentacaoNoSIAF(int auditoriaIntegracaoId, string cpfUsuarioSAM, string cpfUsuarioSIAFEM, string senhaUsuarioSIAFEM, bool ehEstorno = false, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            this.initDadosSiafemIntegracao();
            bool integracaoSIAFEMAtivada = false;
            #region Variaveis
            Tuple<string, string, string, string, bool> objRetorno = null;
            string fmtPadraoMsgInformeAoUsuario = null;
            string msgRetornoProcessamento = null;
            string msgProcessamentoSIAFComErro = null;
            string msgProcessamentoSIAFComSucesso = null;
            bool tipoMovimentacaoPossuiContrapartidaContabil = false;
            bool incorporacaoGeraNLs = false;
            string descricaoTipoMovimentacao = null;
            string[] nlGerada = null;
            string _nlGerada = null;
            string msgRetornoEstimuloXML = null;
            string erroNotaLancamentoContabilizaSP = null;
            SortedList dadosTransacaoSIAFEM = null;
            int managerUnitId = 0;
            AuditoriaIntegracao registroAuditoriaPreExistente = null;
            string nlRetornadaContabilizaSP = null;
            @TipoNotaSIAF tipoNotaContabilizaSP = TipoNotaSIAF.NL_Liquidacao;
            string anoBaseMovimentacao = null;
            string ugeMovimentacao = null;
            string numeroDocumentoSAM = null;
            bool _ehEstorno = false;
            string msgRetornoProcessamentoComErro = null;
            string msgRetornoProcessamentoSucesso = null;
            string msgErroRetornada = null;
            string descricaoTipoNL = null;
            string msgEstimuloXML = null;
            #endregion Variaveis

            //try
            {
                if (auditoriaIntegracaoId > 0)
                {
                    //Obtem auditoria SIAFEM pre-existente
                    registroAuditoriaPreExistente = this.obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);



                    if (registroAuditoriaPreExistente.IsNotNull())
                    {
                        this.initDadosNotasSIAFEM();
                        AssetMovements primeiraMovPatrimonialVinculadaAuditoria = obterPrimeiraMovimentacaoVinculadaRegistroAuditoria(auditoriaIntegracaoId);
                        _ehEstorno = (registroAuditoriaPreExistente.NLEstorno.ToUpperInvariant() == "S");
                        _ehEstorno |= ehEstorno;
                        msgEstimuloXML = registroAuditoriaPreExistente.MsgEstimuloWS;
                        anoBaseMovimentacao = primeiraMovPatrimonialVinculadaAuditoria.MovimentDate.Year.ToString();
                        numeroDocumentoSAM = primeiraMovPatrimonialVinculadaAuditoria.NumberDoc;
                        var tipoMovPatrimonial = obterEntidadeMovementType(primeiraMovPatrimonialVinculadaAuditoria.MovementTypeId);
                        managerUnitId = primeiraMovPatrimonialVinculadaAuditoria.ManagerUnitId;
                        ugeMovimentacao = this.obterCodigoUGE(managerUnitId);
                        integracaoSIAFEMAtivada = verificaUGEIntegradaSIAFEM(managerUnitId);
                        descricaoTipoMovimentacao = ((tipoMovPatrimonial.IsNotNull()) ? tipoMovPatrimonial.Description : "DESCONHECIDO");


                        tipoMovimentacaoPossuiContrapartidaContabil = tipoMovPatrimonial.PossuiContraPartidaContabil();
                        incorporacaoGeraNLs = (integracaoSIAFEMAtivada && tipoMovimentacaoPossuiContrapartidaContabil);
                        if (incorporacaoGeraNLs)
                        {
                            dadosTransacaoSIAFEM = new SortedList() {
                                                                        { "NL_Liquidacao", string.Empty }
                                                                      , { "NL_Depreciacao", string.Empty }
                                                                      , { "NL_Reclassificacao", string.Empty }
                                                                      , { "ErroRetornoSIAFEM", string.Empty }
                                                                      , { "ErroExceptionRuntime", string.Empty }
                                                                    };

                            if (!String.IsNullOrWhiteSpace(msgEstimuloXML))
                            {
                                var procWsSiafem = new ProcessadorServicoSIAF();
                                try
                                {
                                    //PRE-PROCESSAMENTO XML
                                    tipoNotaContabilizaSP = obterTipoMovimentacaoContabilizaSP(msgEstimuloXML);

                                    //CHAMADA AO SISTEMA CONTABILIZA-SP/SIAFEM
                                    procWsSiafem.ConsumirWS(cpfUsuarioSIAFEM, senhaUsuarioSIAFEM, anoBaseMovimentacao, ugeMovimentacao, msgEstimuloXML, false, true);
                                    //procWsSiafem.SimulaConsumoWS(msgEstimuloXML);

                                    msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                                    if (!procWsSiafem.ErroProcessamentoWs) //se SIAFEM nao retornou erro
                                    {
                                        nlRetornadaContabilizaSP = obterNotaLancamentoContabilizaSP(msgRetornoEstimuloXML); //capturo a NL
                                        this.VinculacaoNLContabilizaSP(auditoriaIntegracaoId, tipoNotaContabilizaSP, nlRetornadaContabilizaSP, _ehEstorno);

                                        switch (tipoNotaContabilizaSP)
                                        {
                                            case TipoNotaSIAF.NL_Liquidacao:        { descricaoTipoNL = "NL_Liquidacao"; } break;
                                            case TipoNotaSIAF.NL_Reclassificacao:   { descricaoTipoNL = "NL_Reclassificacao"; } break;
                                            case TipoNotaSIAF.NL_Depreciacao:       { descricaoTipoNL = "NL_Depreciacao"; } break;
                                        }

                                        string separador = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM[descricaoTipoNL].ToString()) ? ", " : null);
                                        dadosTransacaoSIAFEM[descricaoTipoNL] = String.Format("{0}{1}{2}", nlRetornadaContabilizaSP, separador, dadosTransacaoSIAFEM[descricaoTipoNL]);
                                    }
                                    else //se SIAFEM retornou erro...
                                    {
                                        erroNotaLancamentoContabilizaSP = procWsSiafem.ErroRetornoSimplificadoWs; //capturo o erro
                                        dadosTransacaoSIAFEM["ErroRetornoSIAFEM"] = erroNotaLancamentoContabilizaSP;
                                    }

                                    //Atualiza o registro de auditoria integracao SIAFEM pre-existente
                                    registroAuditoriaPreExistente.NotaLancamento = nlRetornadaContabilizaSP;
                                    registroAuditoriaPreExistente.MsgErro = erroNotaLancamentoContabilizaSP;
                                    registroAuditoriaPreExistente.MsgRetornoWS = msgRetornoEstimuloXML;
                                    registroAuditoriaPreExistente.UsuarioSistemaExterno = cpfUsuarioSIAFEM;
                                    this.atualizaAuditoriaIntegracao(registroAuditoriaPreExistente);



                                    msgRetornoEstimuloXML = null;
                                    registroAuditoriaPreExistente = null;
                                }
                                catch (Exception runtimeExceptionGenerica)
                                {
                                    msgErroRetornada = (runtimeExceptionGenerica.InnerException.IsNull() ? (runtimeExceptionGenerica.Message) : (runtimeExceptionGenerica.InnerException.Message));
                                    msgErroRetornada = msgErroRetornada.Replace(Environment.NewLine, " ").Trim();

                                    var separadorVirgula = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()) ? ", " : string.Empty);
                                    dadosTransacaoSIAFEM["ErroExceptionRuntime"] = msgErroRetornada;

                                    //TODO DCBATISTA VERIFICAR MEIO GENERICO DE TRATAR EXCEPTION
                                    //TODO INSERIR ENTRADA ERROLOG AQUI
                                }

                                nlGerada = new string[] { (dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()), (dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()), (dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()) };
                                var gerouNL = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()));
                                var teveErroProcessamento = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()));


                                _nlGerada = String.Join("", nlGerada);
                                msgProcessamentoSIAFComErro = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString() ?? dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString();
                                if (gerouNL)
                                {
                                    msgProcessamentoSIAFComSucesso += (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()) ? dadosTransacaoSIAFEM["NL_Liquidacao"].ToString() : null);
                                    msgProcessamentoSIAFComSucesso += (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()) ? dadosTransacaoSIAFEM["NL_Depreciacao"].ToString() : null);
                                    msgProcessamentoSIAFComSucesso += (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()) ? dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString() : null);
                                }

                                
                                msgRetornoProcessamento = String.Format("{0}{1}", msgProcessamentoSIAFComSucesso, msgProcessamentoSIAFComErro);
                            }

                            msgRetornoProcessamentoComErro = (!_ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaComErroSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoComErroSIAFEM);
                            msgRetornoProcessamentoSucesso = (!_ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaComNLNoSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoComNLNoSIAFEM);
                            fmtPadraoMsgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgProcessamentoSIAFComErro) ? msgRetornoProcessamentoComErro : msgRetornoProcessamentoSucesso);
                            msgRetornoProcessamento = String.Format(fmtPadraoMsgInformeAoUsuario,
                                                                    descricaoTipoMovimentacao,
                                                                    numeroDocumentoSAM,
                                                                    msgRetornoProcessamento);
                        }

                        var houveErroSIAFEM = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()));
                        var msgErroSIAFEM = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString();


                        objRetorno = Tuple.Create<string, string, string, string, bool>(descricaoTipoMovimentacao, msgErroSIAFEM, _nlGerada, msgRetornoProcessamento, houveErroSIAFEM);
                    }
                }
            }
            //catch (Exception excErroRuntime)
            //{
            //    throw excErroRuntime;
            //}
            return objRetorno;
        }

        public Tuple<string, string, string, string, bool> NovoTipoDeProcessamentoFechamentoMensalIntegradoNoSIAF(int auditoriaIntegracaoId, string cpfUsuarioSAM, string cpfUsuarioSIAFEM, string senhaUsuarioSIAFEM, bool ehEstorno = false, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            this.initDadosSiafemIntegracao();
            bool integracaoSIAFEMAtivada = false;
            #region Variaveis
            Tuple<string, string, string, string, bool> objRetorno = null;
            string fmtPadraoMsgInformeAoUsuario = null;
            string msgRetornoProcessamento = null;
            string msgProcessamentoSIAFComErro = null;
            string msgProcessamentoSIAFComSucesso = null;
            bool incorporacaoGeraNLs = false;
            string descricaoTipoMovimentacao = null;
            string[] nlGerada = null;
            string _nlGerada = null;
            string msgRetornoEstimuloXML = null;
            string erroNotaLancamentoContabilizaSP = null;
            SortedList dadosTransacaoSIAFEM = null;
            int managerUnitId = 0;
            AuditoriaIntegracao registroAuditoriaPreExistente = null;
            string nlRetornadaContabilizaSP = null;
            @TipoNotaSIAF tipoNotaContabilizaSP = TipoNotaSIAF.NL_Liquidacao;
            string anoBaseMovimentacao = null;
            string ugeFechamentoMensal = null;
            string numeroDocumentoSAM = null;
            bool _ehEstorno = false;
            string msgRetornoProcessamentoComErro = null;
            string msgRetornoProcessamentoSucesso = null;
            string msgErroRetornada = null;
            string descricaoTipoNL = null;
            string msgEstimuloXML = null;
            #endregion Variaveis

            //try
            {
                if (auditoriaIntegracaoId > 0)
                {
                    //Obtem auditoria SIAFEM pre-existente
                    registroAuditoriaPreExistente = this.obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);



                    if (registroAuditoriaPreExistente.IsNotNull())
                    {
                        this.initDadosNotasSIAFEM();
                        _ehEstorno = (registroAuditoriaPreExistente.NLEstorno.ToUpperInvariant() == "S");
                        _ehEstorno |= ehEstorno;
                        msgEstimuloXML = registroAuditoriaPreExistente.MsgEstimuloWS;
                        anoBaseMovimentacao = registroAuditoriaPreExistente.Data.Value.Year.ToString();
                        numeroDocumentoSAM = registroAuditoriaPreExistente.NotaFiscal;
                        managerUnitId = registroAuditoriaPreExistente.ManagerUnitId;
                        ugeFechamentoMensal = this.obterCodigoUGE(managerUnitId);
                        integracaoSIAFEMAtivada = verificaUGEIntegradaSIAFEM(managerUnitId);
                        descricaoTipoMovimentacao = "FECHAMENTO MENSAL";


                        incorporacaoGeraNLs = (integracaoSIAFEMAtivada);
                        if (incorporacaoGeraNLs)
                        {
                            dadosTransacaoSIAFEM = new SortedList() {
                                                                        { "NL_Liquidacao", string.Empty }
                                                                      , { "NL_Depreciacao", string.Empty }
                                                                      , { "NL_Reclassificacao", string.Empty }
                                                                      , { "ErroRetornoSIAFEM", string.Empty }
                                                                      , { "ErroExceptionRuntime", string.Empty }
                                                                    };

                            if (!String.IsNullOrWhiteSpace(msgEstimuloXML))
                            {
                                var procWsSiafem = new ProcessadorServicoSIAF();
                                try
                                {
                                    //PRE-PROCESSAMENTO XML
                                    tipoNotaContabilizaSP = obterTipoMovimentacaoContabilizaSP(msgEstimuloXML);

                                    //CHAMADA AO SISTEMA CONTABILIZA-SP/SIAFEM
                                    procWsSiafem.ConsumirWS(cpfUsuarioSIAFEM, senhaUsuarioSIAFEM, anoBaseMovimentacao, ugeFechamentoMensal, msgEstimuloXML, false, true);
                                    //procWsSiafem.SimulaConsumoWS(msgEstimuloXML);

                                    msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                                    if (!procWsSiafem.ErroProcessamentoWs) //se SIAFEM nao retornou erro
                                    {
                                        nlRetornadaContabilizaSP = obterNotaLancamentoContabilizaSP(msgRetornoEstimuloXML); //capturo a NL


                                        if (!ehEstorno)
                                            this.GravaNLGerada(numeroDocumentoSAM, nlRetornadaContabilizaSP);
                                        else
                                            this.GravaNLEstornadaGerada(numeroDocumentoSAM, nlRetornadaContabilizaSP);



                                        switch (tipoNotaContabilizaSP)
                                        {
                                            case TipoNotaSIAF.NL_Liquidacao:        { descricaoTipoNL = "NL_Liquidacao"; } break;
                                            case TipoNotaSIAF.NL_Reclassificacao:   { descricaoTipoNL = "NL_Reclassificacao"; } break;
                                            case TipoNotaSIAF.NL_Depreciacao:       { descricaoTipoNL = "NL_Depreciacao"; } break;
                                        }

                                        string separador = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM[descricaoTipoNL].ToString()) ? ", " : null);
                                        dadosTransacaoSIAFEM[descricaoTipoNL] = String.Format("{0}{1}{2}", nlRetornadaContabilizaSP, separador, dadosTransacaoSIAFEM[descricaoTipoNL]);
                                    }
                                    else //se SIAFEM retornou erro...
                                    {
                                        erroNotaLancamentoContabilizaSP = procWsSiafem.ErroRetornoSimplificadoWs; //capturo o erro
                                        dadosTransacaoSIAFEM["ErroRetornoSIAFEM"] = erroNotaLancamentoContabilizaSP;
                                    }

                                    //Atualiza o registro de auditoria integracao SIAFEM pre-existente
                                    registroAuditoriaPreExistente.NotaLancamento = nlRetornadaContabilizaSP;
                                    registroAuditoriaPreExistente.MsgErro = erroNotaLancamentoContabilizaSP;
                                    registroAuditoriaPreExistente.MsgRetornoWS = msgRetornoEstimuloXML;
                                    registroAuditoriaPreExistente.UsuarioSistemaExterno = cpfUsuarioSIAFEM;
                                    this.atualizaAuditoriaIntegracao(registroAuditoriaPreExistente);



                                    msgRetornoEstimuloXML = null;
                                    registroAuditoriaPreExistente = null;
                                }
                                catch (Exception runtimeExceptionGenerica)
                                {
                                    msgErroRetornada = (runtimeExceptionGenerica.InnerException.IsNull() ? (runtimeExceptionGenerica.Message) : (runtimeExceptionGenerica.InnerException.Message));
                                    msgErroRetornada = msgErroRetornada.Replace(Environment.NewLine, " ").Trim();

                                    var separadorVirgula = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()) ? ", " : string.Empty);
                                    dadosTransacaoSIAFEM["ErroExceptionRuntime"] = msgErroRetornada;

                                    //TODO DCBATISTA VERIFICAR MEIO GENERICO DE TRATAR EXCEPTION
                                    //TODO INSERIR ENTRADA ERROLOG AQUI
                                }

                                nlGerada = new string[] { (dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()), (dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()), (dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()) };
                                var gerouNL = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()));
                                var teveErroProcessamento = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString()) || !String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()));


                                _nlGerada = String.Join("", nlGerada);
                                msgProcessamentoSIAFComErro = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString() ?? dadosTransacaoSIAFEM["ErroExceptionRuntime"].ToString();
                                if (gerouNL)
                                {
                                    msgProcessamentoSIAFComSucesso += (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Liquidacao"].ToString()) ? dadosTransacaoSIAFEM["NL_Liquidacao"].ToString() : null);
                                    msgProcessamentoSIAFComSucesso += (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Depreciacao"].ToString()) ? dadosTransacaoSIAFEM["NL_Depreciacao"].ToString() : null);
                                    msgProcessamentoSIAFComSucesso += (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString()) ? dadosTransacaoSIAFEM["NL_Reclassificacao"].ToString() : null);
                                }


                                msgRetornoProcessamento = String.Format("{0}{1}", msgProcessamentoSIAFComSucesso, msgProcessamentoSIAFComErro);
                            }

                            msgRetornoProcessamentoComErro = (!_ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoFechamentoMensalIntegradoExecutadaComErroSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoFechamentoMensalIntegradoExecutadoComErroSIAFEM);
                            msgRetornoProcessamentoSucesso = (!_ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoFechamentoMensalIntegradoExecutadaComNLNoSIAFEM : IntegracaoContabilizaSPController.fmtMsgEstornoFechamentoMensalIntegradoExecutadoComNLNoSIAFEM);
                            fmtPadraoMsgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgProcessamentoSIAFComErro) ? msgRetornoProcessamentoComErro : msgRetornoProcessamentoSucesso);
                            msgRetornoProcessamento = String.Format(fmtPadraoMsgInformeAoUsuario,
                                                                    descricaoTipoMovimentacao,
                                                                    numeroDocumentoSAM,
                                                                    msgRetornoProcessamento);
                        }

                        var houveErroSIAFEM = (!String.IsNullOrWhiteSpace(dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString()));
                        var msgErroSIAFEM = dadosTransacaoSIAFEM["ErroRetornoSIAFEM"].ToString();


                        objRetorno = Tuple.Create<string, string, string, string, bool>(descricaoTipoMovimentacao, msgErroSIAFEM, _nlGerada, msgRetornoProcessamento, houveErroSIAFEM);
                    }
                }
            }
            //catch (Exception excErroRuntime)
            //{
            //    throw excErroRuntime;
            //}
            return objRetorno;
        }

        private AssetMovements obterPrimeiraMovimentacaoVinculadaRegistroAuditoria(int auditoriaIntegracaoId)
        {
            AssetMovements movPatrimonial = null;

            try
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    var registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                    registroDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                             .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                             .FirstOrDefault();

                    if (registroDeAmarracao.IsNotNull())
                    {
                        movPatrimonial = contextoCamadaDados.AssetMovements
                                                            .Where(_movPatrimonial => _movPatrimonial.Id == registroDeAmarracao.AssetMovementsId)
                                                            .FirstOrDefault();
                    }
                }
            }
            catch (Exception excErroRuntime)
            {
                throw excErroRuntime;
            }

            return movPatrimonial;
        }
        private IList<AssetMovements> obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria(int auditoriaIntegracaoId)
        {
            IList<AssetMovements> listaMovPatrimoniais = null;
            AssetMovements movPatrimonial = null;


            try
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    listaMovPatrimoniais = new List<AssetMovements>();
                    var registrosDeAmarracao = new List<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao>();
                    registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                             .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                             .ToList();

                    if (registrosDeAmarracao.HasElements())
                    {
                        foreach (var registroDeAmarracao in registrosDeAmarracao)
                        {
                            movPatrimonial = contextoCamadaDados.AssetMovements
                                                                .Where(_movPatrimonial => _movPatrimonial.Id == registroDeAmarracao.AssetMovementsId)
                                                                .FirstOrDefault();

                            if (movPatrimonial.IsNotNull())
                                listaMovPatrimoniais.Add(movPatrimonial);
                        }

                    }
                }
            }
            catch (Exception excErroRuntime)
            {
                string messageErro = excErroRuntime.Message;
                string stackTrace = excErroRuntime.StackTrace;
                string name = "IntegracaoContabilizaSPController.obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria";
                GravaLogErro(messageErro, stackTrace, name);
                throw excErroRuntime;
            }

            return listaMovPatrimoniais;
        }

        /// <summary>
        /// Método para geração de NL's para movimentações patrimoniais (incorporação/movimentação) que possuam lotes de BPs associados
        /// Para incorporação, a única diferença básica são as chapas dos BPs, que os demais detalhes são iguais a todos os BPs que estão sendo inseridos via lote no sistema.
        /// </summary>
        /// <param name="listaMovimentacoesPatrimoniaisEmLote"></param>
        /// <returns></returns>
        public string ExecutaProcessamentoMovimentacaoNoSIAF(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniaisEmLote, string cpfUsuarioSAM, string cpfUsuarioSIAFEM, string senhaUsuarioSIAFEM, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            this.initDadosSiafemIntegracao();
            bool integracaoSIAFEMAtivada = false;
            #region Variaveis
            string fmtPadraoMsgInformeAoUsuario = null;
            string msgRetornoProcessamento = null;
            string msgProcessamentoSIAFComErro = null;
            string msgProcessamentoSIAFComSucesso = null;
            bool tipoMovimentacaoPossuiContrapartidaContabil = false;
            bool incorporacaoGeraNLs = false;
            string descricaoTipoMovimentacao = null;
            string[] msgsEstimuloXML = null;
            string msgRetornoEstimuloXML = null;
            string erroNotaLancamentoContabilizaSP = null;
            string notasLancamentoContabilizaSP = null;
            string notasDepreciacaoContabilizaSP = null;
            string notasReclassificacaoContabilizaSP = null;
            int managerUnitId = 0;
            AuditoriaIntegracao registroAuditoria = null;
            string nlRetornadaContabilizaSP = null;
            ISIAFNlPatrimonial objImplISIAFNlPatrimonial = null;
            IList<string> retornoMsgsEstimuloXML = null;
            IList<string> retornoMsgsEstimuloXMLComErro = null;
            IList<string> retornoMsgsErroEstimulosXML = null;
            IList<AssetMovements> listaMovimentacoesProcessadas = null;
            IList<int> relacaoMovimentacaoPatrimonialIds = null;
            IList<Tuple<string[], IList<int>>> relacaoDadosMsgEstimulos_ContabilizaSP = null;
            @TipoNotaSIAF tipoNotaContabilizaSP = TipoNotaSIAF.NL_Liquidacao;
            SAMContext contextoCamadaDados = null;
            string anoBaseMovimentacao = null;
            string ugeMovimentacao = null;
            string numeroDocumentoSAM = null;
            bool ehEstorno = false;
            string msgRetornoProcessamentoComErro = null;
            string msgRetornoProcessamentoSucesso = null;
            int tipoMovPatrimonialId = 0;
            string sufixoPluralizaPalavra = null;
            int contadorPluralizaFrase = 0;
            string listaErrosContabilizaSP = null;
            string msgErroRetornada = null;
            string strRelacaoMovimentacaoPatrimonialIds = null;
            #endregion Variaveis


            BaseController.InitDisconnectContext(ref contextoCamadaDados);
            if (listaMovimentacoesPatrimoniaisEmLote.HasElements())
            {
                var primeiraMovimentacaoVinculadaLote = listaMovimentacoesPatrimoniaisEmLote.FirstOrDefault();
                if (primeiraMovimentacaoVinculadaLote.IsNotNull())
                {
                    ehEstorno = EhEstorno(primeiraMovimentacaoVinculadaLote);
                    numeroDocumentoSAM = primeiraMovimentacaoVinculadaLote.NumberDoc;
                    anoBaseMovimentacao = primeiraMovimentacaoVinculadaLote.MovimentDate.Year.ToString();
                    tipoMovPatrimonialId = primeiraMovimentacaoVinculadaLote.MovementTypeId;
                    var tipoMovPatrimonial = contextoCamadaDados.MovementTypes.Where(movPatrimonial => movPatrimonial.Id == tipoMovPatrimonialId).FirstOrDefault();
                    this.initDadosNotasSIAFEM();
                    managerUnitId = primeiraMovimentacaoVinculadaLote.ManagerUnitId;
                    ugeMovimentacao = this.obterCodigoUGE(managerUnitId);
                    integracaoSIAFEMAtivada = verificaUGEIntegradaSIAFEM(managerUnitId);
                    descricaoTipoMovimentacao = ((tipoMovPatrimonial.IsNotNull()) ? tipoMovPatrimonial.Description : "DESCONHECIDO");


                    tipoMovimentacaoPossuiContrapartidaContabil = tipoMovPatrimonial.PossuiContraPartidaContabil();
                    incorporacaoGeraNLs = (integracaoSIAFEMAtivada && tipoMovimentacaoPossuiContrapartidaContabil);
                    string fmtMsgInformeAoUsuario = null;
                    string tokenAuditoriaIntegracao = null;

                    if (incorporacaoGeraNLs)
                    {
                        msgsEstimuloXML = new string[] { };
                        relacaoDadosMsgEstimulos_ContabilizaSP = this.GeradorEstimulosXML_ContabilizaSP(listaMovimentacoesPatrimoniaisEmLote.ToList(), tipoNotaSIAFEM);

                        if (!relacaoDadosMsgEstimulos_ContabilizaSP.HasElements())
                        {
                            registroAuditoria = this.RegistraAuditoriaIntegracaoSemXML(listaMovimentacoesPatrimoniaisEmLote, cpfUsuarioSAM, cpfUsuarioSIAFEM, managerUnitId, tokenAuditoriaIntegracao);
                            RegistraAuditoriaIntegracao(listaMovimentacoesPatrimoniaisEmLote, registroAuditoria);

                            erroNotaLancamentoContabilizaSP = "Não foram gerados dados para envio ao sistema Contabiliza-SP";


                            switch (relacaoTipoMovimentacao_TiposAgrupamentoMovimentacaoPatrimonial[tipoMovPatrimonialId])
                            {
                                case (int)EnumAccountEntryType.AccountEntryType.Entrada:
                                case (int)EnumAccountEntryType.AccountEntryType.Saida: { tipoNotaContabilizaSP = TipoNotaSIAF.NL_Liquidacao; }; break;
                                case (int)EnumAccountEntryType.AccountEntryType.Depreciacao: { tipoNotaContabilizaSP = TipoNotaSIAF.NL_Depreciacao; }; break;
                                case (int)EnumAccountEntryType.AccountEntryType.Reclassificacao: { tipoNotaContabilizaSP = TipoNotaSIAF.NL_Reclassificacao; }; break;
                            }

                            this.RegistraNotaLancamentoPendencia(listaMovimentacoesPatrimoniaisEmLote, registroAuditoria, erroNotaLancamentoContabilizaSP, tipoNotaContabilizaSP);
                            msgProcessamentoSIAFComErro = erroNotaLancamentoContabilizaSP;
                            fmtMsgInformeAoUsuario = "Gerada Pendencia NL SIAFEM para movimentação patrimonial do tipo {0}, documento SAM {1}";
                        }
                        else
                        {
                            var procWsSiafem = new ProcessadorServicoSIAF();
                            registroAuditoria = null;
                            retornoMsgsEstimuloXML = new List<string>();
                            retornoMsgsEstimuloXMLComErro = new List<string>();
                            retornoMsgsErroEstimulosXML = new List<string>();
                            foreach (var dadosMsgEstimulos_ContabilizaSP in relacaoDadosMsgEstimulos_ContabilizaSP)
                            {
                                msgsEstimuloXML = dadosMsgEstimulos_ContabilizaSP.Item1;
                                relacaoMovimentacaoPatrimonialIds = dadosMsgEstimulos_ContabilizaSP.Item2;
                                strRelacaoMovimentacaoPatrimonialIds = (relacaoMovimentacaoPatrimonialIds.HasElements() ? ((relacaoMovimentacaoPatrimonialIds.Count() > 1) ? String.Join(" ", relacaoMovimentacaoPatrimonialIds)
                                                                                                                                                                            : relacaoMovimentacaoPatrimonialIds[0].ToString())
                                                                                                                        : null);
                                foreach (var msgEstimuloXML in msgsEstimuloXML)
                                {
                                    try
                                    {
                                        //PRE-PROCESSAMENTO XML
                                        tipoNotaContabilizaSP = obterTipoMovimentacaoContabilizaSP(msgEstimuloXML);
                                        tokenAuditoriaIntegracao = ObterTokenAuditoriaSIAFEM(msgEstimuloXML);

                                        //CHAMADA AO SISTEMA CONTABILIZA-SP/SIAFEM
                                        procWsSiafem.ConsumirWS(cpfUsuarioSIAFEM, senhaUsuarioSIAFEM, anoBaseMovimentacao, ugeMovimentacao, msgEstimuloXML, false, true);
                                        //procWsSiafem.SimulaConsumoWS(msgEstimuloXML);

                                        //Auditoria Integracao SIAFEM
                                        registroAuditoria = this.RegistraAuditoriaIntegracao(procWsSiafem, cpfUsuarioSAM, cpfUsuarioSIAFEM, managerUnitId, tokenAuditoriaIntegracao, strRelacaoMovimentacaoPatrimonialIds);
                                        listaMovimentacoesProcessadas = this.obterRelacaoMovimentacoesPatrimoniaisPorIds(relacaoMovimentacaoPatrimonialIds);
                                        this.RegistraAuditoriaIntegracao(listaMovimentacoesProcessadas, registroAuditoria);
                                        if (!procWsSiafem.ErroProcessamentoWs)
                                        {
                                            msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                                            retornoMsgsEstimuloXML.Add(msgRetornoEstimuloXML);

                                            nlRetornadaContabilizaSP = obterNotaLancamentoContabilizaSP(msgRetornoEstimuloXML);
                                            this.VinculacaoNLContabilizaSP(listaMovimentacoesProcessadas, tipoNotaContabilizaSP, nlRetornadaContabilizaSP);

                                            switch (tipoNotaContabilizaSP)
                                            {
                                                case TipoNotaSIAF.NL_Liquidacao: { notasLancamentoContabilizaSP = (String.IsNullOrWhiteSpace(nlRetornadaContabilizaSP) ? (notasLancamentoContabilizaSP) : (notasLancamentoContabilizaSP += String.Format("{0}, ", nlRetornadaContabilizaSP))); } break;
                                                case TipoNotaSIAF.NL_Reclassificacao: { notasReclassificacaoContabilizaSP = (String.IsNullOrWhiteSpace(nlRetornadaContabilizaSP) ? (notasReclassificacaoContabilizaSP) : (notasReclassificacaoContabilizaSP += String.Format("{0}, ", nlRetornadaContabilizaSP))); } break;
                                                case TipoNotaSIAF.NL_Depreciacao: { notasDepreciacaoContabilizaSP = (String.IsNullOrWhiteSpace(nlRetornadaContabilizaSP) ? (notasDepreciacaoContabilizaSP) : (notasDepreciacaoContabilizaSP += String.Format("{0}, ", nlRetornadaContabilizaSP))); } break;
                                            }
                                        }
                                        else
                                        {
                                            erroNotaLancamentoContabilizaSP = procWsSiafem.ErroRetornoWs;
                                            msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                                            this.RegistraNotaLancamentoPendencia(listaMovimentacoesProcessadas, registroAuditoria, erroNotaLancamentoContabilizaSP, tipoNotaContabilizaSP);

                                            if (!retornoMsgsErroEstimulosXML.Contains(erroNotaLancamentoContabilizaSP))
                                                retornoMsgsErroEstimulosXML.Add(erroNotaLancamentoContabilizaSP);

                                            if (!retornoMsgsEstimuloXMLComErro.Contains(msgEstimuloXML))
                                                retornoMsgsEstimuloXMLComErro.Add(msgEstimuloXML);
                                        }

                                        registroAuditoria.NotaLancamento = nlRetornadaContabilizaSP;
                                        registroAuditoria.MsgErro = erroNotaLancamentoContabilizaSP;
                                        AmarracaoTabelasMovimentacaoAuditoriaBP(listaMovimentacoesProcessadas, registroAuditoria);


                                        msgRetornoEstimuloXML = null;
                                    }
                                    catch (Exception runtimeExceptionGenerica)
                                    {
                                        //TODO DCBATISTA VERIFICAR MEIO GENERICO DE TRATAR EXCEPTION
                                        this.RegistraNotaLancamentoPendenciaPorException(listaMovimentacoesProcessadas, runtimeExceptionGenerica, registroAuditoria, tipoNotaContabilizaSP);
                                        msgErroRetornada = (runtimeExceptionGenerica.InnerException.IsNull() ? (runtimeExceptionGenerica.Message) : (runtimeExceptionGenerica.InnerException.Message));
                                        msgErroRetornada = msgErroRetornada.Replace(Environment.NewLine, " ").Trim();
                                        //TODO INSERIR ENTRADA ERROLOG AQUI
                                        listaErrosContabilizaSP = ((!String.IsNullOrWhiteSpace(listaErrosContabilizaSP) ? (listaErrosContabilizaSP) : (listaErrosContabilizaSP += String.Format("{0}, ", msgErroRetornada))));
                                        if (!String.IsNullOrWhiteSpace(listaErrosContabilizaSP))
                                        {
                                            if (listaErrosContabilizaSP.TrimEnd()[listaErrosContabilizaSP.Length - 2] == ',')
                                                listaErrosContabilizaSP = listaErrosContabilizaSP.Remove(listaErrosContabilizaSP.Length - 1);

                                            if (listaErrosContabilizaSP.Count(chrPalavra => chrPalavra == ',') > 1)
                                                contadorPluralizaFrase++;
                                        }

                                        sufixoPluralizaPalavra = ((contadorPluralizaFrase > 0) ? "(s)" : sufixoPluralizaPalavra);
                                        var verboInicioMensagem = ((contadorPluralizaFrase > 0) ? "ocorreram" : "ocorreu");
                                        fmtMsgInformeAoUsuario = "Gerada NL Pendente: {2} o{0} seguinte{0} erro{0} interno ao se tentar a geração de NL{0} no sistema Contabiliza-SP: {1}\n, favor verificar!";
                                        msgProcessamentoSIAFComErro = String.Format(fmtMsgInformeAoUsuario, sufixoPluralizaPalavra, listaErrosContabilizaSP, verboInicioMensagem);
                                        continue;
                                    }
                                }
                                //objImplISIAFNlPatrimonial = null;
                                continue;
                            }

                            if (retornoMsgsEstimuloXML.HasElements())
                            {
                                if ((!String.IsNullOrWhiteSpace(notasLancamentoContabilizaSP)) && (notasLancamentoContabilizaSP.Count(nlContabilizaSP => nlContabilizaSP == ',') > 1))
                                { notasLancamentoContabilizaSP = notasLancamentoContabilizaSP.RemoveUltimoCaracter(); contadorPluralizaFrase++; }


                                if ((!String.IsNullOrWhiteSpace(notasDepreciacaoContabilizaSP)) && (notasDepreciacaoContabilizaSP.Count(nlContabilizaSP => nlContabilizaSP == ',') > 1))
                                { notasDepreciacaoContabilizaSP = notasDepreciacaoContabilizaSP.RemoveUltimoCaracter(); contadorPluralizaFrase++; }

                                if ((!String.IsNullOrWhiteSpace(notasReclassificacaoContabilizaSP)) && (notasReclassificacaoContabilizaSP.Count(nlContabilizaSP => nlContabilizaSP == ',') > 1))
                                { notasReclassificacaoContabilizaSP = notasReclassificacaoContabilizaSP.RemoveUltimoCaracter(); contadorPluralizaFrase++; }



                                sufixoPluralizaPalavra = ((contadorPluralizaFrase > 0) ? "(s)" : sufixoPluralizaPalavra);
                                fmtMsgInformeAoUsuario = "Gerada{0} NL{0} número{0} {1}{2}{3} no sistema Contabiliza-SP.";
                                msgProcessamentoSIAFComSucesso = String.Format(fmtMsgInformeAoUsuario, sufixoPluralizaPalavra, notasLancamentoContabilizaSP, notasDepreciacaoContabilizaSP, notasReclassificacaoContabilizaSP);
                            }


                            if (retornoMsgsErroEstimulosXML.HasElements())
                            {
                                listaErrosContabilizaSP = null;
                                sufixoPluralizaPalavra = null;
                                contadorPluralizaFrase = 0;


                                retornoMsgsErroEstimulosXML.ToList().ForEach(_msgErroRetornada => { listaErrosContabilizaSP = ((!String.IsNullOrWhiteSpace(listaErrosContabilizaSP) ? (listaErrosContabilizaSP) : (listaErrosContabilizaSP += String.Format("{0}, ", _msgErroRetornada)))); });
                                if (!String.IsNullOrWhiteSpace(listaErrosContabilizaSP))
                                {
                                    if (listaErrosContabilizaSP.TrimEnd()[listaErrosContabilizaSP.Length - 2] == ',')
                                        listaErrosContabilizaSP = listaErrosContabilizaSP.Remove(listaErrosContabilizaSP.Length - 2);

                                    if (listaErrosContabilizaSP.Count(chrPalavra => chrPalavra == ',') > 1)
                                        contadorPluralizaFrase++;
                                }


                                sufixoPluralizaPalavra = ((contadorPluralizaFrase > 0) ? "(s)" : sufixoPluralizaPalavra);
                                var verboInicioMensagem = ((contadorPluralizaFrase > 0) ? "Ocorreram" : "Ocorreu");
                                fmtMsgInformeAoUsuario = "{2} o{0} seguinte{0} erro{0} ao se tentar a geração de NL{0} no sistema Contabiliza-SP: \"{1}\"\nGerada{0} Nota{0} Contabiliza-SP Pendente{0}, favor verificar!";
                                listaErrosContabilizaSP = listaErrosContabilizaSP.Replace(Environment.NewLine, " ").Trim();
                                msgProcessamentoSIAFComErro = String.Format(fmtMsgInformeAoUsuario, sufixoPluralizaPalavra, listaErrosContabilizaSP, verboInicioMensagem);
                            }
                        }
                    }

                    var separadorMsgsRetorno = ((!String.IsNullOrWhiteSpace(msgProcessamentoSIAFComErro)) ? "\n" : null);
                    msgRetornoProcessamento = String.Format("{0}{1}{2}", msgProcessamentoSIAFComSucesso, separadorMsgsRetorno, msgProcessamentoSIAFComErro);
                }

                msgRetornoProcessamentoComErro = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaComPendencias : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoComPendencias);
                msgRetornoProcessamentoSucesso = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaSucesso : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoSucesso);
                fmtPadraoMsgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgProcessamentoSIAFComErro) ? msgRetornoProcessamentoComErro : msgRetornoProcessamentoSucesso);
                msgRetornoProcessamento = String.Format(fmtPadraoMsgInformeAoUsuario,
                                                        descricaoTipoMovimentacao,
                                                        numeroDocumentoSAM,
                                                        msgRetornoProcessamento);
            }

            return msgRetornoProcessamento;
        }

        public IEnumerable<DadosSIAFEMContaContabilViewModel> obtemDadosContabeisNoSIAFEM(List<DadosSIAFEMContaContabilViewModel> lista)
        {
            if (lista != null && lista.Count > 0)
            {
                string xmlConsulta = null;
                string msgRetornoEstimuloXML = null;

                int numeroMes;

                int codigoUGE = Convert.ToInt32(lista.FirstOrDefault().UGE);
                int codigoGestaoUGE = Int32.Parse(obterGestaoPorCodigoUGE(codigoUGE));

                var procWsSiafem = new ProcessadorServicoSIAF();

                DadosSIAFEMContaContabilViewModel dadosContaContabilSIAFEM = null;

                foreach (var item in lista)
                {
                    codigoUGE = Convert.ToInt32(item.UGE);
                    numeroMes = Convert.ToInt32(item.Mes);

                    xmlConsulta = GeradorEstimuloSIAF.SiafemDocDetaContaGen(codigoUGE, codigoGestaoUGE, MesExtenso.Mes[numeroMes], item.Conta.ToString(), Constante.CST_CONTA_CONTABIL_OPCAO_DETALHADA);
                    procWsSiafem.ConsumirWS(item.Ano, codigoUGE.ToString("D6"), xmlConsulta, true, true, true);

                    if (!procWsSiafem.ErroProcessamentoWs) //se SIAFEM nao retornou erro
                    {
                        msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                        dadosContaContabilSIAFEM = processaRetornoDetaConta_ContabilContabil_OpcaoDetalhada(msgRetornoEstimuloXML);
                        item.ValorContabilSIAFEM = dadosContaContabilSIAFEM.ValorContabilSIAFEM;
                    }
                    else //se SIAFEM retornou erro...
                    {
                        //tenta denovo, mas com o parâmtero saldo
                        xmlConsulta = GeradorEstimuloSIAF.SiafemDocDetaContaGen(codigoUGE, codigoGestaoUGE, MesExtenso.Mes[numeroMes], item.Conta.ToString(), Constante.CST_CONTA_CONTABIL_OPCAO_SALDO);
                        procWsSiafem.ConsumirWS(item.Ano, codigoUGE.ToString("D6"), xmlConsulta, true, true, true);

                        if (!procWsSiafem.ErroProcessamentoWs) //se SIAFEM nao retornou erro
                        {
                            msgRetornoEstimuloXML = procWsSiafem.RetornoWsSIAF;
                            dadosContaContabilSIAFEM = processaRetornoDetaConta_ContabilContabil_OpcaoSaldo(msgRetornoEstimuloXML);
                            item.ValorContabilSIAFEM = dadosContaContabilSIAFEM.ValorContabilSIAFEM;
                        }
                        else
                        {
                            item.ValorContabilSIAFEM = 0;
                        }
                            
                    }

                    item.Diferenca = item.ValorContabilSAM - item.ValorContabilSIAFEM;
                }
            }

            return lista;
        }
        private DadosSIAFEMContaContabilViewModel processaRetornoDetaConta_ContabilContabil_OpcaoDetalhada(string msgRetornoEstimuloXML)
        {
            DadosSIAFEMContaContabilViewModel objRetorno = null;
            string xmlPathValorContabil = null;
            string xmlPathContaContabil = null;
            string strValorContabil = null;
            string strContaContabil = null;
            int contaContabil = 0;
            decimal valorContabil = 0.00m;




            xmlPathValorContabil = "/MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDetaconta/documento/Valor";
            xmlPathContaContabil = "/MSG/BCMSG/Doc_Estimulo/SIAFDOC/SiafemDocDetaContaGen/documento/ContaContabil";
            strValorContabil = XmlUtil.getXmlValue(msgRetornoEstimuloXML, xmlPathValorContabil);
            strContaContabil = XmlUtil.getXmlValue(msgRetornoEstimuloXML, xmlPathContaContabil);


            Int32.TryParse(strContaContabil, out contaContabil);
            if (!Decimal.TryParse(strValorContabil, out valorContabil))
            {
                strValorContabil = strValorContabil.RemoveUltimoCaracter();
                Decimal.TryParse(strValorContabil, out valorContabil);
            }

            
            objRetorno = new DadosSIAFEMContaContabilViewModel() { ValorContabilSIAFEM = valorContabil, Conta = contaContabil };
            return objRetorno;
        }

        private DadosSIAFEMContaContabilViewModel processaRetornoDetaConta_ContabilContabil_OpcaoSaldo(string msgRetornoEstimuloXML)
        {
            DadosSIAFEMContaContabilViewModel objRetorno = null;
            string xmlPathValorContabil = null;
            string xmlPathContaContabil = null;
            string strValorContabil = null;
            string strContaContabil = null;
            int contaContabil = 0;
            decimal valorContabil = 0.00m;




            xmlPathValorContabil = "/MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDetaconta/documento/SaldoateoMes";
            xmlPathContaContabil = "/MSG/BCMSG/Doc_Estimulo/SIAFDOC/SiafemDocDetaContaGen/documento/ContaContabil";
            strValorContabil = XmlUtil.getXmlValue(msgRetornoEstimuloXML, xmlPathValorContabil);
            strContaContabil = XmlUtil.getXmlValue(msgRetornoEstimuloXML, xmlPathContaContabil);


            Int32.TryParse(strContaContabil, out contaContabil);
            if (!Decimal.TryParse(strValorContabil, out valorContabil))
            {
                strValorContabil = strValorContabil.RemoveUltimoCaracter();
                Decimal.TryParse(strValorContabil, out valorContabil);
            }


            objRetorno = new DadosSIAFEMContaContabilViewModel() { ValorContabilSIAFEM = valorContabil, Conta = contaContabil };
            return objRetorno;
        }

        #region Consolidadacao Dados Lista Movimentacoes
        private int obterNumeroMesesVidaUtil(int codigoGrupoMaterial)
        {
            int numeroMesesVidaUtil = 0;
            SAMContext contextoCamadaDados = null;
            using (contextoCamadaDados = new SAMContext())
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                numeroMesesVidaUtil = contextoCamadaDados.MaterialGroups
                                                  .Where(grupoMaterialSIAFISICO => grupoMaterialSIAFISICO.Code == codigoGrupoMaterial)
                                                  .Select(grupoMaterialSIAFISICO => grupoMaterialSIAFISICO.LifeCycle)
                                                  .FirstOrDefault();
            }


            return numeroMesesVidaUtil;
        }
        private IList<KeyValuePair<string, decimal>> preencheListagemDadosBemPatrimonial(IList<KeyValuePair<string, decimal>> listagemDadosBemPatrimonial)
        {
            if (listagemDadosBemPatrimonial.HasElements())
                listagemDadosBemPatrimonial.Clear();

            listagemDadosBemPatrimonial = new List<KeyValuePair<string, decimal>>();
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("ValorAquisicao", 0.00m));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("ValorAtual", 0.00m));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("DepreciacaoAcumulada", 0.00m));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("DepreciacaoMensal", 0.00m));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("MesesEmUso", 0));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("VidaUtil", 0));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("GrupoMaterial", 0));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("ItemMaterial", 0));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("ContaContabil", 0));
            listagemDadosBemPatrimonial.Add(new KeyValuePair<string, decimal>("ContaContabilDepreciacao", 0));


            return listagemDadosBemPatrimonial;
        }
        internal IDictionary<string, SortedList> __obterDadosAtuaisDepreciacaoBemPatrimonial(AssetMovements movimentacaoBemPatrimonial)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            IDictionary<string, SortedList> dadosDepreciacaoBemPatrimonial = null;
            IList<MonthlyDepreciation> retornoHistoricoDepreciacaoCompletoBemPatrimonial = null;
            SortedList detalhesDadosDepreciacaoBemPatrimonial = null;
            int vidaUtil = 0;
            int mesesEmUso = 0;
            int contaContabil = 0;
            int contaContabilDepreciacao = 0;
            int grupoMaterial = 0;
            int itemMaterial = 0;
            int assetMovementId = 0;
            string chaveContaContabilGrupoMaterial = null;
            string relacaoBPs_SemDepreciacaoCalculada = null;
            decimal valorAquisicao = 0.00m;
            decimal valorAtual = 0.00m;
            decimal valorDepreciacaoMensal = 0.00m;
            decimal valorDepreciacaoAcumulada = 0.00m;
            Expression<Func<MonthlyDepreciation, bool>> expWhere = null;
            long assetStartId = -1;




            try
            {
                var bemPatrimonial = movimentacaoBemPatrimonial.RelatedAssets;
                dadosDepreciacaoBemPatrimonial = new Dictionary<string, SortedList>();



                if (bemPatrimonial.ManagerUnitId.IsNotNull() &&
                    bemPatrimonial.MaterialItemCode.IsNotNull())
                {
                    InitAuxiliaryAccountsCache();

                    //A informação de conta contábil será puxada do BP
                    if (movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.IsNull())
                    {
                        //monta chave de pesquisa no cache de contas contabeis
                        var _chavePesquisaDadosContaContabil = dicContasContabeis.Keys.Where(chaveContaContabil => chaveContaContabil.Contains(String.Format("|Id:{0}", movimentacaoBemPatrimonial.AuxiliaryAccountId)))
                                                                                      .FirstOrDefault();

                        //pega a entidade no cache de conta contabeis
                        movimentacaoBemPatrimonial.RelatedAuxiliaryAccount = dicContasContabeis[_chavePesquisaDadosContaContabil];

                        //vinculacao da conta contabil de depreciacao, caso a conta contabil tenha uma conta de depreciação associada
                        if ((movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.DepreciationAccountId.HasValue && movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.DepreciationAccountId > 0) &&
                            (movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.RelatedDepreciationAccount.IsNull()))
                            movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.RelatedDepreciationAccount = contextoCamadaDados.DepreciationAccounts
                                                                                                                               .Where(_contaContabilDepreciacao => _contaContabilDepreciacao.Id == movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.DepreciationAccountId)
                                                                                                                               .FirstOrDefault();
                    }

                    valorAquisicao = bemPatrimonial.ValueAcquisition;
                    valorAtual = bemPatrimonial.ValueAcquisition;
                    assetStartId = (bemPatrimonial.AssetStartId.HasValue ? bemPatrimonial.AssetStartId.Value : 0);
                    if (assetStartId > 0)
                    {
                        expWhere = (dadosDepreciacaoMensal => dadosDepreciacaoMensal.AssetStartId == assetStartId
                                                           && dadosDepreciacaoMensal.ManagerUnitId == bemPatrimonial.ManagerUnitId
                                                           && dadosDepreciacaoMensal.MaterialItemCode == bemPatrimonial.MaterialItemCode);

                        //COMPLEMENTACAO CLAUSULA 'WHERE' NO SELECT SE BP FOR DO TIPO 'DECRETO'
                        expWhere = ((bemPatrimonial.flagDecreto.HasValue && bemPatrimonial.flagDecreto == true) ? expWhere.And(dadosDepreciacaoMensal => dadosDepreciacaoMensal.Decree == true) : expWhere);

                        retornoHistoricoDepreciacaoCompletoBemPatrimonial = contextoCamadaDados.MonthlyDepreciations
                                                                                               .AsNoTracking()
                                                                                               .AsExpandable()
                                                                                               .Where(expWhere)
                                                                                               .ToListNoLock();

                        //SE EXISTE HISTORICO DE DEPRECIACAO CALCULADO
                        if (retornoHistoricoDepreciacaoCompletoBemPatrimonial.HasElements())
                        {
                            //valorAquisicao = bemPatrimonial.ValueAcquisition;
                            mesesEmUso = retornoHistoricoDepreciacaoCompletoBemPatrimonial.Max(x => x.CurrentMonth);
                            valorAtual = retornoHistoricoDepreciacaoCompletoBemPatrimonial.Min(x => x.CurrentValue);
                            valorDepreciacaoMensal = retornoHistoricoDepreciacaoCompletoBemPatrimonial.OrderByDescending(x => x.Id)
                                                                                                      .Take(1)
                                                                                                      .FirstOrDefault()
                                                                                                      .RateDepreciationMonthly;
                            valorDepreciacaoAcumulada = retornoHistoricoDepreciacaoCompletoBemPatrimonial.Max(x => x.AccumulatedDepreciation);
                        }
                    }
                    //SE BP EH DO TIPO 'ACERVO' OU DO TIPO 'TERCEIRO'
                    else if (((bemPatrimonial.flagAcervo.HasValue && bemPatrimonial.flagAcervo == true) || (bemPatrimonial.flagTerceiro.HasValue && bemPatrimonial.flagTerceiro == true))
                    //[OU] BP NAO POSSUI HISTORICO DE DEPRECIACAO CALCULADO
                            || retornoHistoricoDepreciacaoCompletoBemPatrimonial.IsNull() || !retornoHistoricoDepreciacaoCompletoBemPatrimonial.HasElements())
                    {
                        //mesesEmUso = 0;
                        //valorAquisicao = bemPatrimonial.ValueAcquisition;
                        //valorAtual = bemPatrimonial.ValueAcquisition;
                        valorDepreciacaoMensal = 0.00m;
                        valorDepreciacaoAcumulada = 0.00m;
                    }

                    grupoMaterial = bemPatrimonial.MaterialGroupCode;
                    itemMaterial = bemPatrimonial.MaterialItemCode;
                    assetMovementId = movimentacaoBemPatrimonial.Id;
                    vidaUtil = this.obterNumeroMesesVidaUtil(grupoMaterial);
                    contaContabil = movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.BookAccount.GetValueOrDefault();
                    contaContabilDepreciacao = ((movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.DepreciationAccountId.HasValue) ? movimentacaoBemPatrimonial.RelatedAuxiliaryAccount.RelatedDepreciationAccount.Code : 0);
                    chaveContaContabilGrupoMaterial = String.Format("{0}|{1}", contaContabil, grupoMaterial);


                    detalhesDadosDepreciacaoBemPatrimonial = new SortedList();
                    detalhesDadosDepreciacaoBemPatrimonial.Add("ValorAquisicao", valorAquisicao);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("ValorAtual", valorAtual);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("DepreciacaoAcumulada", valorDepreciacaoAcumulada);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("DepreciacaoMensal", valorDepreciacaoMensal);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("MesesEmUso", mesesEmUso);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("VidaUtil", vidaUtil);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("GrupoMaterial", grupoMaterial);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("ContaContabil", contaContabil);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("ContaContabilDepreciacao", contaContabilDepreciacao);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("ItemMaterial", itemMaterial);
                    detalhesDadosDepreciacaoBemPatrimonial.Add("AssetMovementId", assetMovementId);


                    dadosDepreciacaoBemPatrimonial.Add(chaveContaContabilGrupoMaterial, detalhesDadosDepreciacaoBemPatrimonial);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(relacaoBPs_SemDepreciacaoCalculada))
                        relacaoBPs_SemDepreciacaoCalculada = String.Format("{0}-{1}; ", bemPatrimonial.RelatedInitial.Name, bemPatrimonial.NumberIdentification);
                    else
                        relacaoBPs_SemDepreciacaoCalculada += String.Format("{0}-{1}; ", bemPatrimonial.RelatedInitial.Name, bemPatrimonial.NumberIdentification);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + "\n" + (ex.InnerException.IsNotNull() ? ex.InnerException.Message : null));
            }

            return dadosDepreciacaoBemPatrimonial;
        }


        internal SortedList obtemDadosAtuaisDepreciacaoBemPatrimonial(Asset bemPatrimonial)
        {
            #region Variaveis
            SortedList dadosDepreciacaoBemPatrimonial = null;
            SAMContext contextoCamadaDados = new SAMContext();
            int assetId = 0;
            long assetStartId = -1;
            decimal valorAquisicao = 0.00m;
            decimal valorAtual = 0.00m;
            decimal valorDepreciacaoMensal = 0.00m;
            decimal valorDepreciacaoAcumulada = 0.00m;
            Expression<Func<MonthlyDepreciation, bool>> expWhere = null;
            IList<MonthlyDepreciation> retornoHistoricoDepreciacaoCompletoBemPatrimonial = null;
            #endregion Variaveis




            try
            {
                if (bemPatrimonial.ManagerUnitId.IsNotNull() &&
                    bemPatrimonial.MaterialItemCode.IsNotNull())
                {
                    valorAquisicao = bemPatrimonial.ValueAcquisition;
                    valorAtual = bemPatrimonial.ValueAcquisition;
                    assetStartId = (bemPatrimonial.AssetStartId.HasValue ? bemPatrimonial.AssetStartId.Value : 0);
                    if (assetStartId > 0)
                    {
                        expWhere = (dadosDepreciacaoMensal => dadosDepreciacaoMensal.AssetStartId == assetStartId
                                                           && dadosDepreciacaoMensal.ManagerUnitId == bemPatrimonial.ManagerUnitId
                                                           && dadosDepreciacaoMensal.MaterialItemCode == bemPatrimonial.MaterialItemCode);

                        //COMPLEMENTACAO CLAUSULA 'WHERE' NO SELECT SE BP FOR DO TIPO 'DECRETO'
                        expWhere = ((bemPatrimonial.flagDecreto.HasValue && bemPatrimonial.flagDecreto == true) ? expWhere.And(dadosDepreciacaoMensal => dadosDepreciacaoMensal.Decree == true) : expWhere);

                        retornoHistoricoDepreciacaoCompletoBemPatrimonial = contextoCamadaDados.MonthlyDepreciations
                                                                                               .AsNoTracking()
                                                                                               .AsExpandable()
                                                                                               .Where(expWhere)
                                                                                               .ToListNoLock();

                        //SE EXISTE HISTORICO DE DEPRECIACAO CALCULADO
                        if (retornoHistoricoDepreciacaoCompletoBemPatrimonial.HasElements())
                        {
                            valorAtual = retornoHistoricoDepreciacaoCompletoBemPatrimonial.Min(x => x.CurrentValue);
                            valorDepreciacaoMensal = retornoHistoricoDepreciacaoCompletoBemPatrimonial.OrderByDescending(x => x.Id)
                                                                                                      .Take(1)
                                                                                                      .FirstOrDefault()
                                                                                                      .RateDepreciationMonthly;
                            valorDepreciacaoAcumulada = retornoHistoricoDepreciacaoCompletoBemPatrimonial.Max(x => x.AccumulatedDepreciation);
                        }
                    }
                    //SE BP EH DO TIPO 'ACERVO' OU DO TIPO 'TERCEIRO'
                    else if (((bemPatrimonial.flagAcervo.HasValue && bemPatrimonial.flagAcervo == true) || (bemPatrimonial.flagTerceiro.HasValue && bemPatrimonial.flagTerceiro == true))
                    //[OU] BP NAO POSSUI HISTORICO DE DEPRECIACAO CALCULADO
                            || retornoHistoricoDepreciacaoCompletoBemPatrimonial.IsNull() || !retornoHistoricoDepreciacaoCompletoBemPatrimonial.HasElements())
                    {
                        valorDepreciacaoMensal = 0.00m;
                        valorDepreciacaoAcumulada = 0.00m;
                    }

                    assetId = bemPatrimonial.Id;
                    dadosDepreciacaoBemPatrimonial = new SortedList();
                    dadosDepreciacaoBemPatrimonial.Add("ValorAquisicao", valorAquisicao);
                    dadosDepreciacaoBemPatrimonial.Add("ValorAtual", valorAtual);
                    dadosDepreciacaoBemPatrimonial.Add("DepreciacaoMensal", valorDepreciacaoMensal);
                    dadosDepreciacaoBemPatrimonial.Add("DepreciacaoAcumulada", valorDepreciacaoAcumulada);
                    dadosDepreciacaoBemPatrimonial.Add("AssetId", assetId);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + "\n" + (ex.InnerException.IsNotNull() ? ex.InnerException.Message : null));
            }

            return dadosDepreciacaoBemPatrimonial;
        }
        private SortedList consolidadorDadosUmaSohContaContabil(IList<AssetMovements> listaMovimentacoesPatrimoniais)
        {
            #region Variaveis
            int auxiliaryAccountId = 0;
            int codigoContaContabil = 0;
            int codigoContaContabilOrigem = 0;
            int codigoContaContabilDepreciacao = 0;
            int contaContabilOriginalId = 0;
            int codigoItemMaterial = 0;
            int assetId = 0;
            int assetTransferenciaId = 0;
            int assetMovementId = 0;
            int movementTypeId = 0;
            int auxiliaryMovementTypeId = 0;
            int managerUnitId = 0;
            int administrativeUnitId = 0;
            int sourceDestiny_ManagerUnitId = 0;
            string cpfCnpj = null;
            string numeroDocumentoSAM = null;
            AssetMovements primeiraMovPatrimonialLote = null;
            SortedList dadosConsolidadosMovPatrimoniaisBP = null;
            SortedList dadosContabeisBP = null;
            Guid tokenSamPatrimonio = Guid.Empty;
            decimal valorAquisicaoTotal = 0.00m;
            decimal valorAtualTotal = 0.00m;
            decimal valorDepreciacaoAcumuladaTotal = 0.00m;
            decimal valorDepreciacaoMensalTotal = 0.00m;
            DateTime dataAquisicao = DateTime.MinValue;
            DateTime dataMovimentacao = DateTime.MinValue;
            IList<KeyValuePair<int, int>> parMovPatrimonialId_BemPatrimonialId = null;
            MovementType tipoMovimentacao = null;
            string nomeTipoMovimentacao = null;
            #endregion Variaveis




            if (listaMovimentacoesPatrimoniais.HasElements())
            {
                #region Obtencao Dados
                dadosConsolidadosMovPatrimoniaisBP = new SortedList();
                primeiraMovPatrimonialLote = listaMovimentacoesPatrimoniais.FirstOrDefault();

                //Dados Movimentacao
                dataAquisicao = primeiraMovPatrimonialLote.RelatedAssets.AcquisitionDate;
                dataMovimentacao = primeiraMovPatrimonialLote.MovimentDate;
                numeroDocumentoSAM = primeiraMovPatrimonialLote.NumberDoc;
                auxiliaryAccountId = primeiraMovPatrimonialLote.AuxiliaryAccountId.GetValueOrDefault();
                contaContabilOriginalId = primeiraMovPatrimonialLote.ContaContabilAntesDeVirarInservivel.GetValueOrDefault();
                codigoContaContabil = obterCodigoContaContabil(auxiliaryAccountId);
                codigoContaContabilOrigem = obterCodigoContaContabil(contaContabilOriginalId);
                //codigoContaContabilDepreciacao = obterCodigoContaContabilDepreciacao__ContaContabil(auxiliaryAccountId);
                codigoContaContabilDepreciacao = ((contaContabilOriginalId > 0) ? obterCodigoContaContabilDepreciacao__ContaContabil(contaContabilOriginalId) : obterCodigoContaContabilDepreciacao__ContaContabil(auxiliaryAccountId));
                movementTypeId = primeiraMovPatrimonialLote.MovementTypeId;
                auxiliaryMovementTypeId = primeiraMovPatrimonialLote.AuxiliaryMovementTypeId.GetValueOrDefault();
                tipoMovimentacao = this.obterEntidadeMovementType(movementTypeId);
                nomeTipoMovimentacao = (tipoMovimentacao.IsNotNull() ? tipoMovimentacao.Description : String.Empty);
                managerUnitId = primeiraMovPatrimonialLote.ManagerUnitId;
                administrativeUnitId = primeiraMovPatrimonialLote.AdministrativeUnitId.GetValueOrDefault();
                cpfCnpj = primeiraMovPatrimonialLote.CPFCNPJ;
                sourceDestiny_ManagerUnitId = primeiraMovPatrimonialLote.SourceDestiny_ManagerUnitId.GetValueOrDefault();
                assetTransferenciaId = primeiraMovPatrimonialLote.AssetTransferenciaId.GetValueOrDefault();
                tokenSamPatrimonio = geraTokenSamPatrimonio();

                //ContabilizaSP
                codigoItemMaterial = primeiraMovPatrimonialLote.RelatedAssets.MaterialItemCode;


                parMovPatrimonialId_BemPatrimonialId = new List<KeyValuePair<int, int>>();
                foreach (var movimentacaoPatrimonial in listaMovimentacoesPatrimoniais)
                {
                    dadosContabeisBP = obtemDadosAtuaisDepreciacaoBemPatrimonial(movimentacaoPatrimonial.RelatedAssets);


                    valorAquisicaoTotal += Decimal.Parse(dadosContabeisBP["ValorAquisicao"].ToString());
                    valorAtualTotal += Decimal.Parse(dadosContabeisBP["ValorAtual"].ToString());
                    valorDepreciacaoMensalTotal += Decimal.Parse(dadosContabeisBP["DepreciacaoMensal"].ToString());
                    valorDepreciacaoAcumuladaTotal += Decimal.Parse(dadosContabeisBP["DepreciacaoAcumulada"].ToString());

                    assetId = Int32.Parse(dadosContabeisBP["AssetId"].ToString());
                    assetMovementId = movimentacaoPatrimonial.Id;
                    parMovPatrimonialId_BemPatrimonialId.Add(new KeyValuePair<int, int>(assetMovementId, assetId));
                }

                dadosConsolidadosMovPatrimoniaisBP = new SortedList();
                #endregion Obtencao Dados


                #region Filling Objeto Retorno
                //Dados Movimentacao Patrimonial
                dadosConsolidadosMovPatrimoniaisBP.Add("DataAquisicao", dataAquisicao);
                dadosConsolidadosMovPatrimoniaisBP.Add("DataMovimentacao", dataMovimentacao);
                dadosConsolidadosMovPatrimoniaisBP.Add("NumeroDocumentoSAM", numeroDocumentoSAM);
                dadosConsolidadosMovPatrimoniaisBP.Add("AuxiliaryAccountId", auxiliaryAccountId);
                dadosConsolidadosMovPatrimoniaisBP.Add("ContaContabil", codigoContaContabil);
                dadosConsolidadosMovPatrimoniaisBP.Add("ContaContabilOrigem", codigoContaContabilOrigem);
                dadosConsolidadosMovPatrimoniaisBP.Add("ContaContabilDepreciacao", codigoContaContabilDepreciacao);
                dadosConsolidadosMovPatrimoniaisBP.Add("ContaContabilAntesDeVirarInservivelId", contaContabilOriginalId);
                dadosConsolidadosMovPatrimoniaisBP.Add("NomeTipoMovimentacao", nomeTipoMovimentacao);
                dadosConsolidadosMovPatrimoniaisBP.Add("MovementTypeId", movementTypeId);
                dadosConsolidadosMovPatrimoniaisBP.Add("AuxiliaryMovementTypeId", auxiliaryMovementTypeId);
                dadosConsolidadosMovPatrimoniaisBP.Add("ManagerUnitId", managerUnitId);
                dadosConsolidadosMovPatrimoniaisBP.Add("AdministrativeUnitId", administrativeUnitId);
                dadosConsolidadosMovPatrimoniaisBP.Add("CpfCnpj", cpfCnpj);
                dadosConsolidadosMovPatrimoniaisBP.Add("SourceDestiny_ManagerUnitId", sourceDestiny_ManagerUnitId);
                dadosConsolidadosMovPatrimoniaisBP.Add("TokenSamPatrimonio", tokenSamPatrimonio);

                dadosConsolidadosMovPatrimoniaisBP.Add("AssetTransferenciaId", assetTransferenciaId);
                assetId = parMovPatrimonialId_BemPatrimonialId.FirstOrDefault().Value;
                dadosConsolidadosMovPatrimoniaisBP.Add("AssetId", assetId);

                //ContabilizaSP Apenas um ItemMaterial serah enviado
                dadosConsolidadosMovPatrimoniaisBP.Add("ItemMaterial", codigoItemMaterial);

                //Dados Sumarizados BP
                dadosConsolidadosMovPatrimoniaisBP.Add("ValorAquisicao", valorAquisicaoTotal);
                dadosConsolidadosMovPatrimoniaisBP.Add("ValorAtual", valorAtualTotal);
                dadosConsolidadosMovPatrimoniaisBP.Add("DepreciacaoMensal", valorDepreciacaoMensalTotal);
                dadosConsolidadosMovPatrimoniaisBP.Add("DepreciacaoAcumulada", valorDepreciacaoAcumuladaTotal);

                //Dados Agrupamento
                dadosConsolidadosMovPatrimoniaisBP.Add("MovPatrimonialId_BemPatrimonialId", parMovPatrimonialId_BemPatrimonialId);
                #endregion Filling Objeto Retorno
            }

            return dadosConsolidadosMovPatrimoniaisBP;
        }
        private AssetMovements consolidadorDadosEmObjetoAssetMovements(SortedList dadosParaConsolidacao)
        {
            #region Variaveis
            AssetMovements movPatrimonialDadosConsolidados = null;
            Asset bemPatrimonialDadosConsolidados = null;
            string numeroDocumentoSAM = null;
            int auxiliaryAccountId = 0;
            int contaContabilOriginalId = 0;
            int movementTypeId = 0;
            int auxiliaryMovementTypeId = 0;
            int managerUnitId = 0;
            int administrativeUnitId = 0;
            string cpfCnpj = null;
            int codigoItemMaterial = 0;
            int sourceDestiny_ManagerUnitId = 0;
            int assetTransferenciaId = 0;
            int assetId = 0;
            decimal valorAquisicaoTotal = 0.00m;
            decimal valorAtualTotal = 0.00m;
            decimal valorDepreciacaoAcumuladaTotal = 0.00m;
            decimal valorDepreciacaoMensalTotal = 0.00m;
            Guid tokenSamPatrimonio = Guid.Empty;
            IList<int> listaAssetMovementsId = null;
            DateTime dataAquisicao = DateTime.MinValue;
            DateTime dataMovimentacao = DateTime.MinValue;
            #endregion Variaveis


            #region Recuperacao Dados
            movPatrimonialDadosConsolidados = new AssetMovements();
            bemPatrimonialDadosConsolidados = new Asset();


            //Dados Movimentacao Patrimonial
            dataAquisicao = (DateTime)dadosParaConsolidacao["DataAquisicao"];
            dataMovimentacao = (DateTime)dadosParaConsolidacao["DataMovimentacao"];
            numeroDocumentoSAM = dadosParaConsolidacao["NumeroDocumentoSAM"].ToString();
            auxiliaryAccountId = Int32.Parse(dadosParaConsolidacao["AuxiliaryAccountId"].ToString());
            contaContabilOriginalId = Int32.Parse(dadosParaConsolidacao["ContaContabilAntesDeVirarInservivelId"].ToString());
            movementTypeId = Int32.Parse(dadosParaConsolidacao["MovementTypeId"].ToString());
            auxiliaryMovementTypeId = Int32.Parse(dadosParaConsolidacao["AuxiliaryMovementTypeId"].ToString());
            managerUnitId = Int32.Parse(dadosParaConsolidacao["ManagerUnitId"].ToString());
            administrativeUnitId = Int32.Parse(dadosParaConsolidacao["AdministrativeUnitId"].ToString());
            cpfCnpj = (string)dadosParaConsolidacao["CpfCnpj"];
            sourceDestiny_ManagerUnitId = Int32.Parse(dadosParaConsolidacao["SourceDestiny_ManagerUnitId"].ToString());
            listaAssetMovementsId = (IList<int>)dadosParaConsolidacao["AssetMovementIds"];
            assetTransferenciaId = Int32.Parse(dadosParaConsolidacao["AssetTransferenciaId"].ToString());
            assetId = Int32.Parse(dadosParaConsolidacao["AssetId"].ToString());
            tokenSamPatrimonio = (Guid)dadosParaConsolidacao["TokenSamPatrimonio"];

            //ContabilizaSP Apenas um ItemMaterial serah enviado
            codigoItemMaterial = Int32.Parse(dadosParaConsolidacao["ItemMaterial"].ToString());

            //Dados Sumarizados BP
            valorAquisicaoTotal = Decimal.Parse(dadosParaConsolidacao["ValorAquisicao"].ToString());
            valorAtualTotal = Decimal.Parse(dadosParaConsolidacao["ValorAtual"].ToString());
            valorDepreciacaoMensalTotal = Decimal.Parse(dadosParaConsolidacao["DepreciacaoMensal"].ToString());
            valorDepreciacaoAcumuladaTotal = Decimal.Parse(dadosParaConsolidacao["DepreciacaoAcumulada"].ToString());
            #endregion Recuperacao Dados


            #region Filling Objetos Retorno
            movPatrimonialDadosConsolidados.MovimentDate = dataMovimentacao;
            movPatrimonialDadosConsolidados.NumberDoc = numeroDocumentoSAM;
            movPatrimonialDadosConsolidados.CPFCNPJ = cpfCnpj;
            movPatrimonialDadosConsolidados.ContaContabilAntesDeVirarInservivel = contaContabilOriginalId;
            movPatrimonialDadosConsolidados.TokenGeradoEnvioContabilizaSP = tokenSamPatrimonio;
            //popular entidade relacionada
            movPatrimonialDadosConsolidados.AuxiliaryAccountId = auxiliaryAccountId;
            movPatrimonialDadosConsolidados.RelatedAuxiliaryAccount = obterEntidadeAuxiliaryAccount(auxiliaryAccountId);
            //popular entidade relacionada
            movPatrimonialDadosConsolidados.SourceDestiny_ManagerUnitId = sourceDestiny_ManagerUnitId;
            movPatrimonialDadosConsolidados.AssetTransferenciaId = assetTransferenciaId;
            movPatrimonialDadosConsolidados.AssetId = assetId;

            movPatrimonialDadosConsolidados.RelatedSourceDestinyManagerUnit = obterEntidadeManagerUnit(sourceDestiny_ManagerUnitId);
            //popular entidade relacionada
            movPatrimonialDadosConsolidados.ManagerUnitId = managerUnitId;
            movPatrimonialDadosConsolidados.RelatedManagerUnit = obterEntidadeManagerUnit(managerUnitId);
            //popular entidade relacionada
            movPatrimonialDadosConsolidados.AdministrativeUnitId = administrativeUnitId;
            movPatrimonialDadosConsolidados.RelatedAdministrativeUnit = obterEntidadeAdministrativeUnit(administrativeUnitId);
            //popular entidade relacionada
            movPatrimonialDadosConsolidados.MovementTypeId = movementTypeId;
            movPatrimonialDadosConsolidados.AuxiliaryMovementTypeId = auxiliaryMovementTypeId;
            movPatrimonialDadosConsolidados.RelatedMovementType = obterEntidadeMovementType(movementTypeId);


            bemPatrimonialDadosConsolidados.AcquisitionDate = dataAquisicao;
            bemPatrimonialDadosConsolidados.MovimentDate = dataMovimentacao;
            bemPatrimonialDadosConsolidados.ValueAcquisition = valorAquisicaoTotal;
            bemPatrimonialDadosConsolidados.ValueUpdate = valorAtualTotal;
            bemPatrimonialDadosConsolidados.DepreciationAccumulated = valorDepreciacaoAcumuladaTotal;
            bemPatrimonialDadosConsolidados.RateDepreciationMonthly = valorDepreciacaoMensalTotal;
            bemPatrimonialDadosConsolidados.MaterialItemCode = codigoItemMaterial;
            bemPatrimonialDadosConsolidados.AssetMovements = new List<AssetMovements>() { movPatrimonialDadosConsolidados };

            movPatrimonialDadosConsolidados.RelatedAssets = bemPatrimonialDadosConsolidados;
            #endregion Filling Objetos Retorno

            return movPatrimonialDadosConsolidados;
        }
        private AuxiliaryAccount obterEntidadeAuxiliaryAccount(int auxiliaryAccountId)
        {
            AuxiliaryAccount objEntidade = null;


            if (auxiliaryAccountId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AuxiliaryAccounts
                                                     .Where(contaContabil => contaContabil.Id == auxiliaryAccountId)
                                                     .FirstOrDefault();

                    if (objEntidade.DepreciationAccountId.GetValueOrDefault() > 0)
                        objEntidade.RelatedDepreciationAccount = contextoCamadaDados.DepreciationAccounts
                                                                                    .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == objEntidade.DepreciationAccountId)
                                                                                    .FirstOrDefault();
                }

            return objEntidade;
        }
        private AdministrativeUnit obterEntidadeAdministrativeUnit(int administrativeUnit)
        {
            AdministrativeUnit objEntidade = null;


            if (administrativeUnit > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AdministrativeUnits
                                                     .Where(uaSIAFEM => uaSIAFEM.Id == administrativeUnit)
                                                     .FirstOrDefault();
                }

            return objEntidade;
        }
        private ManagerUnit obterEntidadeManagerUnit(int managerUnitId)
        {
            ManagerUnit objEntidade = null;


            if (managerUnitId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.ManagerUnits
                                                     .Where(ugeSIAFEM => ugeSIAFEM.Id == managerUnitId)
                                                     .FirstOrDefault();
                }

            return objEntidade;
        }
        private MovementType obterEntidadeMovementType(int movementTypeId)
        {
            MovementType objEntidade = null;


            if (movementTypeId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.MovementTypes
                                                     .Where(tipoMovimentacao => tipoMovimentacao.Id == movementTypeId)
                                                     .FirstOrDefault();
                }

            return objEntidade;
        }
        private AuditoriaIntegracao obterEntidadeAuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            AuditoriaIntegracao objEntidade = null;


            if (auditoriaIntegracaoId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AuditoriaIntegracoes
                                                     .Where(auditoriaIntegracao => auditoriaIntegracao.Id == auditoriaIntegracaoId)
                                                     .FirstOrDefault();
                }

            return objEntidade;
        }
        private Guid geraTokenSamPatrimonio()
        {
            return Guid.NewGuid();
        }


        public IList<AssetMovements> ParticionadorLista_ContaContabilGrupoMaterial(IList<AssetMovements> listaMovimentacoesPatrimoniais)
        {
            IList<AssetMovements> listagemDeListaParaRetorno = new List<AssetMovements>();
            IList<KeyValuePair<string, decimal>> listagemDadosBemPatrimonial;
            IDictionary<string, SortedList> listagemDeSaida = new Dictionary<string, SortedList>();
            decimal valorEmCache = 0.00m;
            decimal valorDecimal = 0.00m;
            string chaveContaContabilGrupoMaterial = null;
            string codigoContaContabil = null;
            SortedList tempCache = null;
            string codigoItemMaterial = null;
            string assetMovementId = null;
            SortedList cacheItemMaterial = null;
            IDictionary<string, long> relacaoItemMaterialOrdenada = null;
            IList<int> cacheAssetMovementsId = null;
            SortedList dadosMovPatrimoniais = null;
            AssetMovements movPatrimonial = null;
            Asset bemPatrimonial = null;
            string numeroDocumentoSAM = null;
            SAMContext contextoCamadaDados = null;



            BaseController.InitDisconnectContext(ref contextoCamadaDados);
            listagemDadosBemPatrimonial = new List<KeyValuePair<string, decimal>>();
            listagemDadosBemPatrimonial = this.preencheListagemDadosBemPatrimonial(listagemDadosBemPatrimonial);
            InitAuxiliaryAccountsCache();
            InitMovementTypesCache();
            tempCache = new SortedList();
            cacheItemMaterial = new SortedList();
            cacheAssetMovementsId = new List<int>();
            dadosMovPatrimoniais = new SortedList();
            numeroDocumentoSAM = listaMovimentacoesPatrimoniais.FirstOrDefault().NumberDoc;

            var contaContabilOriginalId = listaMovimentacoesPatrimoniais.FirstOrDefault().ContaContabilAntesDeVirarInservivel.GetValueOrDefault();
            var ugeMovimentacoesPatrimoniaisId = listaMovimentacoesPatrimoniais.FirstOrDefault().ManagerUnitId;
            var dadosUgeMovimentacoesPatrimoniais = contextoCamadaDados.ManagerUnits.Where(ugeMovPatrimonial => ugeMovPatrimonial.Id == ugeMovimentacoesPatrimoniaisId).FirstOrDefault();
            var tipoMovimentacaoPatrimonialId = listaMovimentacoesPatrimoniais.FirstOrDefault().MovementTypeId;
            var dadosTipoMovimentacaoPatrimoniais = contextoCamadaDados.MovementTypes.Where(tipoMovPatrimonial => tipoMovPatrimonial.Id == tipoMovimentacaoPatrimonialId).FirstOrDefault();

            string[] infosDepreciacaoParaIgnorar = new string[] { "MesesEmUso", "VidaUtil", "GrupoMaterial", "ContaContabil", "ContaContabilDepreciacao", "ItemMaterial", "AssetMovementId" };
            foreach (var movimentacaoPatrimonial in listaMovimentacoesPatrimoniais)
            {
                cacheItemMaterial = new SortedList();
                cacheAssetMovementsId = new List<int>();
                //foreach (var detalheDadosDepreciacao in movimentacaoPatrimonial.__obterDadosAtuaisDepreciacaoBemPatrimonial()) //obter os dados contabeis/depreciação de cada BP
                foreach (var detalheDadosDepreciacao in this.__obterDadosAtuaisDepreciacaoBemPatrimonial(movimentacaoPatrimonial)) //obter os dados contabeis/depreciação de cada BP
                {
                    chaveContaContabilGrupoMaterial = detalheDadosDepreciacao.Key;
                    dadosMovPatrimoniais = detalheDadosDepreciacao.Value;
                    cacheItemMaterial = new SortedList();
                    cacheAssetMovementsId = new List<int>();
                    if (listagemDeSaida.ContainsKey(chaveContaContabilGrupoMaterial))
                    {
                        tempCache = listagemDeSaida[chaveContaContabilGrupoMaterial];
                        tempCache.Keys
                                 .Cast<string>()
                                 .ToList()
                                 .ForEach(infoDepreciacao => {
                                                                 if (!infosDepreciacaoParaIgnorar.Contains(infoDepreciacao))
                                                                 {
                                                                     Decimal.TryParse(tempCache[infoDepreciacao].ToString(), out valorEmCache);
                                                                     Decimal.TryParse(dadosMovPatrimoniais[infoDepreciacao].ToString(), out valorDecimal);

                                                                     tempCache[infoDepreciacao] = valorEmCache + valorDecimal;
                                                                     valorEmCache = 0.00m;
                                                                     valorDecimal = 0.00m;
                                                                 }
                                                                 else
                                                                 {
                                                                     if (infoDepreciacao.ToUpperInvariant() == "ItemMaterial".ToUpperInvariant())
                                                                     {
                                                                         if (tempCache[infoDepreciacao].GetType() == typeof(SortedList))
                                                                         {
                                                                            codigoItemMaterial = dadosMovPatrimoniais[infoDepreciacao].ToString();
                                                                            cacheItemMaterial = (SortedList)tempCache[infoDepreciacao];
                                                                            if (cacheItemMaterial.ContainsKey(codigoItemMaterial))
                                                                                cacheItemMaterial[codigoItemMaterial] = Int32.Parse(cacheItemMaterial[codigoItemMaterial].ToString()) + 1;
                                                                            else
                                                                                cacheItemMaterial.Add(codigoItemMaterial, 1);
                                                                         }
                                                                         else if (tempCache[infoDepreciacao].GetType() == typeof(Int32))
                                                                         {
                                                                             codigoItemMaterial = tempCache[infoDepreciacao].ToString();
                                                                             cacheItemMaterial.Add(codigoItemMaterial, 1);
                                                                         }

                                                                         tempCache[infoDepreciacao] = cacheItemMaterial;
                                                                     }
                                                                     else if (infoDepreciacao.ToUpperInvariant() == "AssetMovementId".ToUpperInvariant())
                                                                     {
                                                                         assetMovementId = dadosMovPatrimoniais[infoDepreciacao].ToString();

                                                                         if (tempCache[infoDepreciacao].GetType() == typeof(List<int>))
                                                                           cacheAssetMovementsId = (List<int>)tempCache[infoDepreciacao];

                                                                         cacheAssetMovementsId.Add(Int32.Parse(assetMovementId));
                                                                         tempCache[infoDepreciacao] = cacheAssetMovementsId;
                                                                     }
                                                                 }
                                                             });

                        listagemDeSaida[chaveContaContabilGrupoMaterial] = tempCache;
                    }
                    else
                    {
                        //AssetMoevementId
                        assetMovementId = dadosMovPatrimoniais["AssetMovementId"].ToString();
                        cacheAssetMovementsId.Add(Int32.Parse(assetMovementId));
                        dadosMovPatrimoniais["AssetMovementId"] = cacheAssetMovementsId;

                        //ItemMaterial
                        codigoItemMaterial = dadosMovPatrimoniais["ItemMaterial"].ToString();
                        cacheItemMaterial.Add(codigoItemMaterial, 1);
                        dadosMovPatrimoniais["ItemMaterial"] = cacheItemMaterial;


                        listagemDeSaida.Add(chaveContaContabilGrupoMaterial, dadosMovPatrimoniais);
                    }

                    tempCache = null;
                    cacheItemMaterial = null;
                    cacheAssetMovementsId = null;
                    chaveContaContabilGrupoMaterial = null;
                    assetMovementId = null;
                }

                dadosMovPatrimoniais = null;
            }

            listagemDeSaida.ToList().ForEach(dadosAgrupamentoMovPatrimonais => {
                                                                                 if (dadosAgrupamentoMovPatrimonais.Value["ItemMaterial"].GetType() == typeof(Int32))
                                                                                     dadosAgrupamentoMovPatrimonais.Value["ItemMaterial"] = (new SortedList() { { dadosAgrupamentoMovPatrimonais.Value["ItemMaterial"].ToString(), 1 } });

                                                                                 if (dadosAgrupamentoMovPatrimonais.Value["AssetMovementId"].GetType() == typeof(Int32))
                                                                                     dadosAgrupamentoMovPatrimonais.Value["AssetMovementId"] = (new List<int>() { (int)dadosAgrupamentoMovPatrimonais.Value["AssetMovementId"] } );
                                                                               });

            #region Geracao Token SIAFEM MONITORASIAF
            IDictionary<IList<int>, Guid> chavesParaSvcMonitoramentoSIAF = null;
            Guid tokenSIAF = Guid.Empty;
            chavesParaSvcMonitoramentoSIAF = new Dictionary<IList<int>, Guid>();
            listagemDeSaida.ToList().ForEach(dadosAgrupamentoMovPatrimonais => {
                                                                                    IList<int> chavePesquisa = ((IList<int>)dadosAgrupamentoMovPatrimonais.Value["AssetMovementId"]);
                                                                                    if (!chavesParaSvcMonitoramentoSIAF.ContainsKey(chavePesquisa))
                                                                                        chavesParaSvcMonitoramentoSIAF.Add(chavePesquisa, Guid.NewGuid());
                                                                               });
            #endregion Geracao Token SIAFEM MONITORASIAF

            foreach (var dadosAgrupamentoMovPatrimonais in listagemDeSaida)
            {
                var relacaoMovPatrimonialIds = ((List<int>)dadosAgrupamentoMovPatrimonais.Value["AssetMovementId"]);
                chavesParaSvcMonitoramentoSIAF.TryGetValue(relacaoMovPatrimonialIds, out tokenSIAF);




                codigoContaContabil = dadosAgrupamentoMovPatrimonais.Value["ContaContabil"].ToString();
                movPatrimonial = new AssetMovements();
                bemPatrimonial = new Asset();

                movPatrimonial.TokenGeradoEnvioContabilizaSP = tokenSIAF;
                movPatrimonial.ContaContabilAntesDeVirarInservivel = contaContabilOriginalId;
                movPatrimonial.RelatedAuxiliaryAccount = dicContasContabeis[codigoContaContabil];
                bemPatrimonial.ValueAcquisition = Decimal.Parse(dadosAgrupamentoMovPatrimonais.Value["ValorAquisicao"].ToString());
                bemPatrimonial.ValueUpdate = Decimal.Parse(dadosAgrupamentoMovPatrimonais.Value["ValorAtual"].ToString());
                bemPatrimonial.DepreciationAccumulated = Decimal.Parse(dadosAgrupamentoMovPatrimonais.Value["DepreciacaoAcumulada"].ToString());
                bemPatrimonial.RateDepreciationMonthly = Decimal.Parse(dadosAgrupamentoMovPatrimonais.Value["DepreciacaoMensal"].ToString());
                bemPatrimonial.MaterialGroupCode = Int32.Parse(dadosAgrupamentoMovPatrimonais.Value["GrupoMaterial"].ToString());

                {
                    var consolidacaoItemMaterial = ((SortedList)dadosAgrupamentoMovPatrimonais.Value["ItemMaterial"]);
                    relacaoItemMaterialOrdenada = new Dictionary<string, long>();
                    consolidacaoItemMaterial.Cast<DictionaryEntry>()
                                            .OrderByDescending(parChaveValor => parChaveValor.Value)
                                            .Take(15)
                                            .ToList()
                                            .ForEach(parChaveValor => relacaoItemMaterialOrdenada.Add(parChaveValor.Key.ToString(), Int64.Parse(parChaveValor.Value.ToString())));

                    var maiorNumeroOcorrencias_ItemMaterial = consolidacaoItemMaterial.Values.Cast<int>().Max();
                    var itemMaterial = consolidacaoItemMaterial.GetKey(consolidacaoItemMaterial.IndexOfValue(maiorNumeroOcorrencias_ItemMaterial)).ToString();
                    bemPatrimonial.MaterialItemCode = Int32.Parse(itemMaterial);

                    //var relacaoMovPatrimonialIds = ((List<int>)dadosAgrupamentoMovPatrimonais.Value["AssetMovementId"]);
                    movPatrimonial.RelacaoIdsMovimentacoesVinculadasPorAuditoriaIntegracao = relacaoMovPatrimonialIds;
                    //movPatrimonial.RelacaoItemMaterial = relacaoItemMaterialOrdenada;
                }

                movPatrimonial.RelatedAssets = bemPatrimonial;
                movPatrimonial.RelatedMovementType = dadosTipoMovimentacaoPatrimoniais;
                movPatrimonial.RelatedManagerUnit = dadosUgeMovimentacoesPatrimoniais;
                movPatrimonial.NumberDoc = numeroDocumentoSAM;
                movPatrimonial.CPFCNPJ = listaMovimentacoesPatrimoniais.FirstOrDefault().CPFCNPJ;
                movPatrimonial.SourceDestiny_ManagerUnitId = listaMovimentacoesPatrimoniais.FirstOrDefault().SourceDestiny_ManagerUnitId;

                listagemDeListaParaRetorno.Add(movPatrimonial);
            }

            return listagemDeListaParaRetorno;
        }
        public ImplSIAFNlPatrimonial[] createObjetoInterface_ContabilizaSP(Asset bemPatrimonial, bool ehEstorno, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            var contextoCamadaDados = new SAMContext();

            IList<ImplSIAFNlPatrimonial> listaObjsImplSIAFNlPatrimonial = null;
            ImplSIAFNlPatrimonial objImplSIAFNlPatrimonial = null;

            bool vaiFavorecerDestino = false;
            bool insereSufixoContaInservivel = false;
            int[] agrupamentoEntradaSaida = new int[] {   EnumAccountEntryType.AccountEntryType.Entrada.GetHashCode()
                                                        , EnumAccountEntryType.AccountEntryType.Saida.GetHashCode()
                                                      };
            var movimentacaoBP = bemPatrimonial.AssetMovements.FirstOrDefault();
            if (movimentacaoBP.RelatedMovementType.IsNotNull())
            {
                var relacaoEventosRelacionado_ContabilizaSP = contextoCamadaDados.EventServicesContabilizaSPs
                                                                                 .Where(eventoContabilizaSP => (eventoContabilizaSP.TipoMovimento_SamPatrimonio == movimentacaoBP.RelatedMovementType.Code.ToString()))
                                                                                 .OrderBy(eventoContabilizaSP => eventoContabilizaSP.ExecutionOrder)
                                                                                 .ToList();

                //TODO DCBATISTA ELIMINAR QG COM NOVA COLUNA NA TABELA DE MOVIMENTACAO INTEGRACAO
                if ((relacaoEventosRelacionado_ContabilizaSP.Count() > 1) &&
                    (relacaoEventosRelacionado_ContabilizaSP.Count(tipoMovIntegracao => agrupamentoEntradaSaida.Contains(tipoMovIntegracao.AccountEntryTypeId)) == 1) &&
                    (relacaoEventosRelacionado_ContabilizaSP.Where(tipoMovIntegracao => agrupamentoEntradaSaida.Contains(tipoMovIntegracao.AccountEntryTypeId) && tiposDeIncorporacaoMovimentacaoQueFavorecemDestino.Contains(Int32.Parse(tipoMovIntegracao.TipoMovimento_SamPatrimonio))).Count() == 1)
                   )
                    vaiFavorecerDestino = true;

                //AJUSTE PARA INCORPORACAO DE BP's VINDOS DO SAM-ESTOQUE
                #region Tratamento Para BPs oriundos de SAM-Estoque (Tipo Movimento Reclassificação/Saída ContabilizaSP)
                int tipoMovimentacao_IntegracaoSAMEstoque = TipoMovimentacaoPatrimonial.IncorpIntegracaoSAMEstoque_SAMPatrimonio.GetHashCode();
                if (movimentacaoBP.RelatedMovementType.Id == tipoMovimentacao_IntegracaoSAMEstoque)
                {
                    //if (movimentacaoBP.RelatedAdministrativeUnit.IsNotNull() && (movimentacaoBP.ManagerUnitId != movimentacaoBP.RelatedAdministrativeUnit.ManagerUnitId))
                    if (    (movimentacaoBP.SourceDestiny_ManagerUnitId.GetValueOrDefault() > 0) && (movimentacaoBP.ManagerUnitId != movimentacaoBP.SourceDestiny_ManagerUnitId.GetValueOrDefault())
                        ||  (movimentacaoBP.AuxiliaryMovementTypeId.GetValueOrDefault() == EnumMovimentType.IncorpIntegracaoSAMEstoque_SAMPatrimonio_OutraUGE.GetHashCode())
                       )
                            relacaoEventosRelacionado_ContabilizaSP = relacaoEventosRelacionado_ContabilizaSP.Where(eventoContabilizaSP => eventoContabilizaSP.AccountEntryTypeId == EnumAccountEntryType.AccountEntryType.Saida.GetHashCode()).ToList();
                    //    else if (movimentacaoBP.AuxiliaryMovementTypeId.GetValueOrDefault() == EnumMovimentType.ReclassificacaoIntegracaoSAMEstoque_SAMPatrimonio_MesmaUGE.GetHashCode())
                    else
                            relacaoEventosRelacionado_ContabilizaSP = relacaoEventosRelacionado_ContabilizaSP.Where(eventoContabilizaSP => eventoContabilizaSP.AccountEntryTypeId == EnumAccountEntryType.AccountEntryType.Reclassificacao.GetHashCode()).ToList();
                }
                #endregion Tratamento Para BPs oriundos de SAM-Estoque (Tipo Movimento Reclassificação/Saída ContabilizaSP)

                listaObjsImplSIAFNlPatrimonial = new List<ImplSIAFNlPatrimonial>();
                foreach (var eventoRelacionado_ContabilizaSP in relacaoEventosRelacionado_ContabilizaSP)
                {
                    insereSufixoContaInservivel = false;
                    if (tipoNotaSIAFEM != TipoNotaSIAF.Desconhecido)
                    {
                        if ((tipoNotaSIAFEM == TipoNotaSIAF.NL_Depreciacao) && (eventoRelacionado_ContabilizaSP.AccountEntryTypeId != (int)EnumAccountEntryType.AccountEntryType.Depreciacao))
                            continue;

                        if ((tipoNotaSIAFEM == TipoNotaSIAF.NL_Reclassificacao) && (eventoRelacionado_ContabilizaSP.AccountEntryTypeId != (int)EnumAccountEntryType.AccountEntryType.Reclassificacao))
                            continue;

                        if ((tipoNotaSIAFEM == TipoNotaSIAF.NL_Liquidacao) && 
                            ((eventoRelacionado_ContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Depreciacao) ||
                             (eventoRelacionado_ContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Reclassificacao)))
                            continue;
                    }

                    if (eventoRelacionado_ContabilizaSP.IsNotNull())
                    {
                        objImplSIAFNlPatrimonial = new ImplSIAFNlPatrimonial();


                        objImplSIAFNlPatrimonial.EhEstorno = ehEstorno;
                        objImplSIAFNlPatrimonial.ExecutionOrder                                                                                                                 = eventoRelacionado_ContabilizaSP.ExecutionOrder;
                        objImplSIAFNlPatrimonial.InputOutputReclassificationDepreciationTypeCode                                                                                = eventoRelacionado_ContabilizaSP.InputOutputReclassificationDepreciationTypeCode;

                        #region Tratamento Para Incorporacao 'TRANSFERENCIA OUTRO ORGAO - PATRIMONIADO' Nao-Implantados (Nova Transacao ContabilizaSP)
                        {
                            bool ehMovimentacaoEntrada = eventoRelacionado_ContabilizaSP.AccountEntryTypeId == EnumAccountEntryType.AccountEntryType.Entrada.GetHashCode();
                            if (ehMovimentacaoEntrada && (eventoRelacionado_ContabilizaSP.VerificaSeOrigemUtilizaSAM))
                            {
                                bool ugeNaoImplantadaENaoIntegrada = false;
                                int managerUnitId = 0;

                                using (var _contextoCamadaDados = new SAMContext())
                                {
                                    //aqui eu sei que eh incorporacao nao-manual (existe movimentacao de origem na base de dados)
                                    var movPatrimonialOrigemBPASerIncorporado = _contextoCamadaDados.AssetMovements
                                                                                                    .Where(_movPatrimonial => _movPatrimonial.AssetTransferenciaId.Value == movimentacaoBP.AssetId)
                                                                                                    .FirstOrDefault();

                                    //existe movimentacao de origem na base de dados, UGE-origem utiliza o SAM-Patrimonio
                                    if (movPatrimonialOrigemBPASerIncorporado.IsNotNull())
                                        managerUnitId = movPatrimonialOrigemBPASerIncorporado.ManagerUnitId;
                                    //se nao tem AssetMovements de origem, estah movimentacao eh incorporacao/doacao manual
                                    else
                                        managerUnitId = movimentacaoBP.SourceDestiny_ManagerUnitId.GetValueOrDefault();
                                }

                                ugeNaoImplantadaENaoIntegrada = verificaNaoImplantacaoENaoIntegracaoSIAFEM(managerUnitId);

                                if (ugeNaoImplantadaENaoIntegrada && eventoRelacionado_ContabilizaSP.UtilizaTipoMovimentacaoContabilizaSPAlternativa)
                                    objImplSIAFNlPatrimonial.InputOutputReclassificationDepreciationType = eventoRelacionado_ContabilizaSP.TipoMovimentacaoContabilizaSPAlternativa;
                                else
                                    objImplSIAFNlPatrimonial.InputOutputReclassificationDepreciationType = eventoRelacionado_ContabilizaSP.RelatedMovementType.Description;
                            }
                            else
                            {
                                objImplSIAFNlPatrimonial.InputOutputReclassificationDepreciationType = eventoRelacionado_ContabilizaSP.RelatedMovementType.Description;
                            }
                        }
                        #endregion Tratamento Para Incorporacao Outros Orgaos Nao-Implantados (Nova Transacao ContabilizaSP)

                        #region Flag Para Inclusao Sufixo ' - INS'
                        {
                            if (//   (tipoNotaSIAFEM == TipoNotaSIAF.NL_Reclassificacao) && (eventoRelacionado_ContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Reclassificacao)
                                //&& (movimentacaoBP.RelatedAuxiliaryAccount.BookAccount.GetValueOrDefault() == Constante.CST_CONTA_CONTABIL_INSERVIVEL_CONTABILIZA_SP_SIAFEM)
                                   (eventoRelacionado_ContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Reclassificacao)
                                && (movimentacaoBP.MovementTypeId == TipoMovimentacaoPatrimonial.MovInservivelNaUGE.GetHashCode())
                               )
                            {
                                insereSufixoContaInservivel = true;
                            }
                        }
                        #endregion Flag Para Inclusao Sufixo ' - INS'



                        objImplSIAFNlPatrimonial.DescricaoTipoMovimento_SamPatrimonio                                                                                           = eventoRelacionado_ContabilizaSP.RelatedMovementType.Description;

                        objImplSIAFNlPatrimonial.AccountEntryTypeId = eventoRelacionado_ContabilizaSP.AccountEntryTypeId;
                        switch (objImplSIAFNlPatrimonial.AccountEntryTypeId)
                        {
                            case (int)EnumAccountEntryType.AccountEntryType.Entrada:                                                                                            { objImplSIAFNlPatrimonial.AccountEntryType = AccountEntryType.Entrada.Name; }; break;
                            case (int)EnumAccountEntryType.AccountEntryType.Saida:                                                                                              { objImplSIAFNlPatrimonial.AccountEntryType = AccountEntryType.Saida.Name; }; break;
                            case (int)EnumAccountEntryType.AccountEntryType.Depreciacao:                                                                                        { objImplSIAFNlPatrimonial.AccountEntryType = AccountEntryType.Depreciacao.Name; }; break;
                            case (int)EnumAccountEntryType.AccountEntryType.Reclassificacao:                                                                                    { objImplSIAFNlPatrimonial.AccountEntryType = AccountEntryType.Reclassificacao.Name; }; break;
                        }

                        objImplSIAFNlPatrimonial.CommitmentNumber                                                                                                               = bemPatrimonial.Empenho;
                        objImplSIAFNlPatrimonial.MaterialType                                                                                                                   = eventoRelacionado_ContabilizaSP.MaterialType;
                        objImplSIAFNlPatrimonial.Observacao                                                                                                                     = movimentacaoBP.Observation;
                        objImplSIAFNlPatrimonial.SpecificControl                                                                                                                = eventoRelacionado_ContabilizaSP.SpecificControl;
                        objImplSIAFNlPatrimonial.SpecificInputControl                                                                                                           = eventoRelacionado_ContabilizaSP.SpecificInputControl;
                        objImplSIAFNlPatrimonial.SpecificOutputControl                                                                                                          = eventoRelacionado_ContabilizaSP.SpecificOutputControl;

                        objImplSIAFNlPatrimonial.StockDescription                                                                                                               = eventoRelacionado_ContabilizaSP.StockDescription;
                        objImplSIAFNlPatrimonial.StockType                                                                                                                      = eventoRelacionado_ContabilizaSP.StockType;


                        objImplSIAFNlPatrimonial.DocumentNumberSAM                                                                                                              = movimentacaoBP.NumberDoc;
                        objImplSIAFNlPatrimonial.MetaDataType_AccountingValueField                                                                                              = eventoRelacionado_ContabilizaSP.MetaDataType_AccountingValueField;
                        objImplSIAFNlPatrimonial.MetaDataType_DateField                                                                                                         = eventoRelacionado_ContabilizaSP.MetaDataType_DateField;
                        objImplSIAFNlPatrimonial.MetaDataType_StockSource                                                                                                       = eventoRelacionado_ContabilizaSP.MetaDataType_StockSource ?? 0;
                        objImplSIAFNlPatrimonial.MetaDataType_StockDestination                                                                                                  = eventoRelacionado_ContabilizaSP.MetaDataType_StockDestination ?? 0;
                        objImplSIAFNlPatrimonial.MetaDataType_MovementTypeContabilizaSP                                                                                         = eventoRelacionado_ContabilizaSP.MetaDataType_MovementTypeContabilizaSP ?? 0;
                        objImplSIAFNlPatrimonial.MetaDataType_SpecificControl                                                                                                   = eventoRelacionado_ContabilizaSP.MetaDataType_SpecificControl ?? 0;
                        objImplSIAFNlPatrimonial.MetaDataType_SpecificInputControl                                                                                              = eventoRelacionado_ContabilizaSP.MetaDataType_SpecificInputControl ?? 0;
                        objImplSIAFNlPatrimonial.MetaDataType_SpecificOutputControl                                                                                             = eventoRelacionado_ContabilizaSP.MetaDataType_SpecificOutputControl ?? 0;
                        //campo para indicar se transacao/movimentacao ContabilizaSP vai favorecer destino (outra UGE/orgao)
                        objImplSIAFNlPatrimonial.FavoreceDestino                                                                                                                = eventoRelacionado_ContabilizaSP.FavoreceDestino;

                        //nova regra de negócio: sera enviado apenas um Item Material no xml de estimulo
                        objImplSIAFNlPatrimonial.RelacaoItemMaterial = new Dictionary<string, long>();
                        objImplSIAFNlPatrimonial.RelacaoItemMaterial.Add(bemPatrimonial.MaterialItemCode.ToString(), bemPatrimonial.MaterialItemCode);

                        #region Tratamento para Transferencias/Doacoes
                        //AQUI PREENCHO OS CAMPOS DE UGE/GESTAO DA MOVIMENTACAO
                        {
                            string _codigoUGE = null;
                            string _codigoGestaoUGE = null;
                            int codigoUGE = 0;
                            int codigoGestao = 0;


                            if (movimentacaoBP.ManagerUnitId > 0)
                            {
                                _codigoUGE = this.obterCodigoUGE(movimentacaoBP.ManagerUnitId);
                                _codigoGestaoUGE = obterGestaoUGE(movimentacaoBP.ManagerUnitId);

                                if ((!String.IsNullOrWhiteSpace(_codigoUGE) && !String.IsNullOrWhiteSpace(_codigoGestaoUGE)) &&
                                    (Int32.TryParse(_codigoUGE, out codigoUGE) && Int32.TryParse(_codigoGestaoUGE, out codigoGestao)))
                                {
                                    objImplSIAFNlPatrimonial.ManagerCode = codigoGestao;
                                    objImplSIAFNlPatrimonial.ManagerUnitCode = codigoUGE;
                                }
                            }
                        }
                        //AQUI PREENCHO OS CAMPOS DE ORIGEM/DESTINO DA MOVIMENTACAO
                        if (objImplSIAFNlPatrimonial.FavoreceDestino || vaiFavorecerDestino)
                            if ((objImplSIAFNlPatrimonial.AccountEntryTypeId != AccountEntryType.Depreciacao.Value)
                                || (objImplSIAFNlPatrimonial.AccountEntryTypeId != AccountEntryType.Reclassificacao.Value))
                            {
                                int tipoMovPatrimonialId = 0;
                                string gestaoOrigemDoacaoOuTransferencia = null;
                                string ugeCpfCnpjOrigemDoacaoOuTransferencia = null;


                                tipoMovPatrimonialId = movimentacaoBP.MovementTypeId;
                                if (movimentacaoBP.RelatedMovementType.Id == tipoMovimentacao_IntegracaoSAMEstoque)
                                    tipoMovPatrimonialId = eventoRelacionado_ContabilizaSP.InputOutputReclassificationDepreciationTypeCode;

                                var tipoMovPatrimonial = obterEntidadeMovementType(tipoMovPatrimonialId);
                                bool ehMovimentacaoSaida = tipoMovPatrimonial.GroupMovimentId == EnumGroupMoviment.Movimentacao.GetHashCode();
                                if (tiposDeIncorporacaoMovimentacaoQueFavorecemDestino.Contains(tipoMovPatrimonialId))
                                    if (tiposDeIncorporacaoMovimentacaoDeTransferenciaOuDoacao.Contains(tipoMovPatrimonialId))
                                    {
                                        if (ehMovimentacaoSaida) //se eh movimentacao/saida
                                        {
                                            gestaoOrigemDoacaoOuTransferencia = this.obterGestaoUGE(movimentacaoBP.SourceDestiny_ManagerUnitId);
                                            ugeCpfCnpjOrigemDoacaoOuTransferencia = this.obterCodigoUGE(movimentacaoBP.SourceDestiny_ManagerUnitId);
                                        }
                                        else if (!ehMovimentacaoSaida)  //se eh incorporacao/entrada
                                        {
                                            using (var _contextoCamadaDados = new SAMContext())
                                            {
                                                //aqui eu sei que eh incorporacao nao-manual (existe movimentacao de origem na base de dados)
                                                var movPatrimonialOrigemBPASerIncorporado = _contextoCamadaDados.AssetMovements
                                                                                                                .Where(_movPatrimonial => _movPatrimonial.AssetTransferenciaId.Value == movimentacaoBP.AssetId)
                                                                                                                .FirstOrDefault();

                                                if (movPatrimonialOrigemBPASerIncorporado.IsNotNull())
                                                {
                                                    gestaoOrigemDoacaoOuTransferencia = this.obterGestaoUGE(movPatrimonialOrigemBPASerIncorporado.ManagerUnitId);
                                                    ugeCpfCnpjOrigemDoacaoOuTransferencia = this.obterCodigoUGE(movPatrimonialOrigemBPASerIncorporado.ManagerUnitId);
                                                }
                                                //se nao tem AssetMovements de origem, estah movimentacao eh incorporacao/doacao manual
                                                else
                                                {
                                                    gestaoOrigemDoacaoOuTransferencia = this.obterGestaoUGE(movimentacaoBP.SourceDestiny_ManagerUnitId);
                                                    ugeCpfCnpjOrigemDoacaoOuTransferencia = this.obterCodigoUGE(movimentacaoBP.SourceDestiny_ManagerUnitId);
                                                }
                                            }
                                        }
                                    }
                                    else if (tiposDeIncorporacaoMovimentacaoComCpfCnpj.Contains(tipoMovPatrimonialId))
                                    {
                                        gestaoOrigemDoacaoOuTransferencia = null;
                                        ugeCpfCnpjOrigemDoacaoOuTransferencia = movimentacaoBP.CPFCNPJ;
                                    }


                                objImplSIAFNlPatrimonial.GestaoFavorecida = gestaoOrigemDoacaoOuTransferencia;
                                objImplSIAFNlPatrimonial.CpfCnpjUgeFavorecido = ugeCpfCnpjOrigemDoacaoOuTransferencia;
                                objImplSIAFNlPatrimonial.FavoreceDestino |= vaiFavorecerDestino;
                            }
                        #endregion Tratamento para Transferencias/Doacoes

                        #region Tratamento para NL de Depreciacao Para Incorporacao/Doacao Manual
                        //AQUI PREENCHO OS CAMPOS DE UGE/GESTAO DA MOVIMENTACAO
                        if (objImplSIAFNlPatrimonial.AccountEntryTypeId == EnumAccountEntryType.AccountEntryType.Depreciacao.GetHashCode() &&
                            (movimentacaoBP.RelatedMovementType.IsNotNull() && movimentacaoBP.RelatedMovementType.GroupMovimentId == EnumAccountEntryType.AccountEntryType.Entrada.GetHashCode())
                           )
                        {
                            //AQUI PREENCHO OS CAMPOS DE ORIGEM/DESTINO DA MOVIMENTACAO
                            if (objImplSIAFNlPatrimonial.FavoreceDestino || vaiFavorecerDestino)
                            //if ((objImplSIAFNlPatrimonial.AccountEntryTypeId != AccountEntryType.Depreciacao.Value)
                            //    || (objImplSIAFNlPatrimonial.AccountEntryTypeId != AccountEntryType.Reclassificacao.Value))
                            {
                                int tipoMovPatrimonialId = 0;
                                string gestaoOrigemDoacaoOuTransferencia = null;
                                string ugeCpfCnpjOrigemDoacaoOuTransferencia = null;


                                tipoMovPatrimonialId = movimentacaoBP.MovementTypeId;
                                var tipoMovPatrimonial = obterEntidadeMovementType(tipoMovPatrimonialId);
                                //bool ehMovimentacaoSaida = tipoMovPatrimonial.GroupMovimentId == EnumGroupMoviment.Movimentacao.GetHashCode();
                                bool ehMovimentacaoEntrada = tipoMovPatrimonial.GroupMovimentId == EnumGroupMoviment.Incorporacao.GetHashCode();
                                if (tiposDeIncorporacaoMovimentacaoQueFavorecemDestino.Contains(tipoMovPatrimonialId))
                                    if (tiposDeIncorporacaoMovimentacaoDeTransferenciaOuDoacao.Contains(tipoMovPatrimonialId))
                                    {
                                        if (ehMovimentacaoEntrada)  //se eh incorporacao/entrada
                                        {
                                            using (var _contextoCamadaDados = new SAMContext())
                                            {
                                                //aqui eu sei que eh incorporacao nao-manual (existe movimentacao de origem na base de dados)
                                                var movPatrimonialOrigemBPASerIncorporado = _contextoCamadaDados.AssetMovements
                                                                                                                .Where(_movPatrimonial => _movPatrimonial.AssetTransferenciaId.Value == movimentacaoBP.AssetId)
                                                                                                                .FirstOrDefault();

                                                if (movPatrimonialOrigemBPASerIncorporado.IsNotNull())
                                                {
                                                    continue;
                                                }
                                                else if (movPatrimonialOrigemBPASerIncorporado.IsNull())
                                                {
                                                    gestaoOrigemDoacaoOuTransferencia = this.obterGestaoUGE(movimentacaoBP.SourceDestiny_ManagerUnitId);
                                                    ugeCpfCnpjOrigemDoacaoOuTransferencia = this.obterCodigoUGE(movimentacaoBP.SourceDestiny_ManagerUnitId);
                                                }
                                            }
                                        }
                                    }
                                    else if (tiposDeIncorporacaoMovimentacaoComCpfCnpj.Contains(tipoMovPatrimonialId))
                                    {
                                        gestaoOrigemDoacaoOuTransferencia = null;
                                        ugeCpfCnpjOrigemDoacaoOuTransferencia = movimentacaoBP.CPFCNPJ;
                                    }


                                objImplSIAFNlPatrimonial.GestaoFavorecida = gestaoOrigemDoacaoOuTransferencia;
                                objImplSIAFNlPatrimonial.CpfCnpjUgeFavorecido = ugeCpfCnpjOrigemDoacaoOuTransferencia;
                                objImplSIAFNlPatrimonial.FavoreceDestino |= vaiFavorecerDestino;
                            }
                        }
                        #endregion Tratamento para NL de Depreciacao Para Incorporacao/Doacao Manual

                        //ESTOQUE ORIGEM
                        switch (objImplSIAFNlPatrimonial.MetaDataType_StockSource)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.AuxiliaryAccountAsset:                                            { objImplSIAFNlPatrimonial.StockSource = movimentacaoBP.RelatedAuxiliaryAccount.Description; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.DepreciationAccountAsset:                                         { objImplSIAFNlPatrimonial.StockSource = movimentacaoBP.RelatedAuxiliaryAccount.RelatedDepreciationAccount.Description; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedAuxiliaryAccount:                 { objImplSIAFNlPatrimonial.StockSource = obterTipoMovimentacaoContabilizaSP__ContaContabil(movimentacaoBP.AuxiliaryAccountId.GetValueOrDefault(), insereSufixoContaInservivel); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedDepreciationAccount:              { objImplSIAFNlPatrimonial.StockSource = obterTipoMovimentacaoDepreciacaoContabilizaSP__ContaContabil(movimentacaoBP.AuxiliaryAccountId.GetValueOrDefault()); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedOriginalAuxiliaryAccount:         { objImplSIAFNlPatrimonial.StockSource = obterTipoMovimentacaoContabilizaSP__ContaContabil(movimentacaoBP.ContaContabilAntesDeVirarInservivel.GetValueOrDefault(), insereSufixoContaInservivel); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedOriginalDepreciationAccount:      { objImplSIAFNlPatrimonial.StockSource = obterTipoMovimentacaoDepreciacaoContabilizaSP__ContaContabil(movimentacaoBP.ContaContabilAntesDeVirarInservivel.Value); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.TextualInformation:                                               { objImplSIAFNlPatrimonial.StockSource = eventoRelacionado_ContabilizaSP.StockSource; }; break;
                        }

                        //ESTOQUE DESTINO
                        switch (objImplSIAFNlPatrimonial.MetaDataType_StockDestination)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.AuxiliaryAccountAsset:                                            { objImplSIAFNlPatrimonial.StockDestination = movimentacaoBP.RelatedAuxiliaryAccount.Description; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.DepreciationAccountAsset:                                         { objImplSIAFNlPatrimonial.StockDestination = movimentacaoBP.RelatedAuxiliaryAccount.RelatedDepreciationAccount.Description; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedAuxiliaryAccount:                 { objImplSIAFNlPatrimonial.StockDestination = obterTipoMovimentacaoContabilizaSP__ContaContabil(movimentacaoBP.AuxiliaryAccountId.GetValueOrDefault()); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedDepreciationAccount:              { objImplSIAFNlPatrimonial.StockDestination = obterTipoMovimentacaoDepreciacaoContabilizaSP__ContaContabil(movimentacaoBP.AuxiliaryAccountId.GetValueOrDefault()); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedOriginalAuxiliaryAccount:         { objImplSIAFNlPatrimonial.StockDestination = obterTipoMovimentacaoContabilizaSP__ContaContabil(movimentacaoBP.ContaContabilAntesDeVirarInservivel.GetValueOrDefault()); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedOriginalDepreciationAccount:      { objImplSIAFNlPatrimonial.StockDestination = obterTipoMovimentacaoDepreciacaoContabilizaSP__ContaContabil(movimentacaoBP.ContaContabilAntesDeVirarInservivel.Value); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.TextualInformation:                                               { objImplSIAFNlPatrimonial.StockDestination = eventoRelacionado_ContabilizaSP.StockDestination; } break;
                        }

                        switch (objImplSIAFNlPatrimonial.MetaDataType_DateField)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.IncorporationDate:                                                { objImplSIAFNlPatrimonial.MovimentDate = bemPatrimonial.MovimentDate; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.MovimentDate:                                                     { objImplSIAFNlPatrimonial.MovimentDate = movimentacaoBP.MovimentDate; }; break;
                        }

                        switch (objImplSIAFNlPatrimonial.MetaDataType_AccountingValueField)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.AcquisitionValue:                                                 { objImplSIAFNlPatrimonial.AccountingValueField = bemPatrimonial.ValueAcquisition; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.CurrentValue:                                                     { objImplSIAFNlPatrimonial.AccountingValueField = bemPatrimonial.ValueUpdate ?? 0.00m; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.AccumulatedDepreciationValue:                                     { objImplSIAFNlPatrimonial.AccountingValueField = bemPatrimonial.DepreciationAccumulated ?? 0.00m; }; break;
                        }

                        //TRATAMENTO PARA NAO GERAR NL DE DEPRECIACAO COM VALOR FINANCEIRO ZERADO
                        if (objImplSIAFNlPatrimonial.AccountingValueField == 0.00m)
                            continue;

                        switch (objImplSIAFNlPatrimonial.MetaDataType_SpecificControl)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.TextualInformation:                                               { objImplSIAFNlPatrimonial.SpecificControl = eventoRelacionado_ContabilizaSP.SpecificControl; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.StandartSpecificControl:                                          { objImplSIAFNlPatrimonial.SpecificControl = ControleEspecifico.CE_Padrao; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.SpecificControlByMovementType:                                    { objImplSIAFNlPatrimonial.SpecificControl = movimentacaoBP.RelatedAuxiliaryAccount.ControleEspecificoResumido; }; break;
                        }

                        switch (objImplSIAFNlPatrimonial.MetaDataType_SpecificInputControl)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.TextualInformation:                                               { objImplSIAFNlPatrimonial.SpecificInputControl = eventoRelacionado_ContabilizaSP.SpecificInputControl; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.StandartSpecificControl:                                          { objImplSIAFNlPatrimonial.SpecificInputControl = ControleEspecifico.CE_Padrao; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.SpecificControlByMovementType:                                    { objImplSIAFNlPatrimonial.SpecificInputControl = movimentacaoBP.RelatedAuxiliaryAccount.ControleEspecificoResumido; }; break;
                        }

                        switch (objImplSIAFNlPatrimonial.MetaDataType_SpecificOutputControl)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.TextualInformation:                                               { objImplSIAFNlPatrimonial.SpecificOutputControl = eventoRelacionado_ContabilizaSP.SpecificOutputControl; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.StandartSpecificControl:                                          { objImplSIAFNlPatrimonial.SpecificOutputControl = ControleEspecifico.CE_Padrao; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.SpecificControlByMovementType:                                    { objImplSIAFNlPatrimonial.SpecificOutputControl = movimentacaoBP.RelatedAuxiliaryAccount.ControleEspecificoResumido; }; break;
                        }

                        switch (objImplSIAFNlPatrimonial.MetaDataType_MovementTypeContabilizaSP)
                        {
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.Sam_MovementTypeId:                                               { objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP = eventoRelacionado_ContabilizaSP.DescricaoTipoMovimento_SamPatrimonio; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.AuxiliaryAccountAsset:                                            { objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP = movimentacaoBP.RelatedAuxiliaryAccount.Description; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.DepreciationAccountAsset:                                         { objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP = movimentacaoBP.RelatedAuxiliaryAccount.Description; }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedAuxiliaryAccount:                 {
                                                                                                                                                                                    if (eventoRelacionado_ContabilizaSP.UtilizaTipoMovimentacaoContabilizaSPAlternativa)
                                                                                                                                                                                        objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP = eventoRelacionado_ContabilizaSP.TipoMovimentacaoContabilizaSPAlternativa;
                                                                                                                                                                                    else
                                                                                                                                                                                        objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP = movimentacaoBP.RelatedAuxiliaryAccount.TipoMovimentacaoContabilizaSP;
                                                                                                                                                                                }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.ContabilizaSP_MovementTypeLinkedDepreciationAccount:              { objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP = obterTipoMovimentacaoDepreciacaoContabilizaSP__ContaContabil(movimentacaoBP.RelatedAuxiliaryAccount.Id); }; break;
                            case (short)EnumMetaDataTypeServiceContabilizaSP.MetaDataTypeServiceContabilizaSP.TextualInformation:                                               { objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP = eventoRelacionado_ContabilizaSP.TipoMovimentacao_ContabilizaSP; }; break;
                        }

                        objImplSIAFNlPatrimonial.RelacaoMovimentacoesPatrimonaisId                                                                                              = bemPatrimonial.AssetMovements.Select(movPatrimonial => (long)movPatrimonial.Id).ToList();
                        objImplSIAFNlPatrimonial.TokenGeradoEnvioContabilizaSP                                                                                                  = movimentacaoBP.TokenGeradoEnvioContabilizaSP;
                    }

                    listaObjsImplSIAFNlPatrimonial.Add(objImplSIAFNlPatrimonial);
                }

                listaObjsImplSIAFNlPatrimonial = listaObjsImplSIAFNlPatrimonial.OrderBy(_objImplSIAFNlPatrimonial => _objImplSIAFNlPatrimonial.ExecutionOrder)
                                                                               .ToList();
            }

            return listaObjsImplSIAFNlPatrimonial.ToArray();
        }
        #endregion Consolidacao Dados Lista Movimentacoes

        #region Metodos Auxiliares Preenchimento XML
        internal string obterGestaoUGE(int? ugeId)
        {
            string codigoGestao = null;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (ugeId.HasValue && ugeId > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Id == ugeId).FirstOrDefault();
                if (dadosUGE.IsNotNull())
                {
                    dadosUGE.RelatedBudgetUnit = contextoCamadaDados.BudgetUnits.Where(uoSIAFEM => uoSIAFEM.Id == dadosUGE.BudgetUnitId).FirstOrDefault();
                    if (dadosUGE.RelatedBudgetUnit.IsNotNull())
                    {
                        dadosUGE.RelatedBudgetUnit.RelatedInstitution = contextoCamadaDados.Institutions.Where(orgaoSIAFEM => orgaoSIAFEM.Id == dadosUGE.RelatedBudgetUnit.InstitutionId).FirstOrDefault();
                        if (dadosUGE.RelatedBudgetUnit.RelatedInstitution.IsNotNull())
                        {
                            codigoGestao = dadosUGE.RelatedBudgetUnit.RelatedInstitution.ManagerCode;
                            codigoGestao = codigoGestao.PadLeft(5, '0');
                        }
                    }
                }
            }

            return codigoGestao;
        }
        internal string obterGestaoPorCodigoUGE(int codigoUGE)
        {
            string codigoGestao = null;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (codigoUGE > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Code == codigoUGE.ToString()).FirstOrDefault();
                if (dadosUGE.IsNotNull())
                {
                    dadosUGE.RelatedBudgetUnit = contextoCamadaDados.BudgetUnits.Where(uoSIAFEM => uoSIAFEM.Id == dadosUGE.BudgetUnitId).FirstOrDefault();
                    if (dadosUGE.RelatedBudgetUnit.IsNotNull())
                    {
                        dadosUGE.RelatedBudgetUnit.RelatedInstitution = contextoCamadaDados.Institutions.Where(orgaoSIAFEM => orgaoSIAFEM.Id == dadosUGE.RelatedBudgetUnit.InstitutionId).FirstOrDefault();
                        if (dadosUGE.RelatedBudgetUnit.RelatedInstitution.IsNotNull())
                        {
                            codigoGestao = dadosUGE.RelatedBudgetUnit.RelatedInstitution.ManagerCode;
                            codigoGestao = codigoGestao.PadLeft(5, '0');
                        }
                    }
                }
            }

            return codigoGestao;
        }
        internal string obterCodigoUGE(int? ugeId)
        {
            string codigoUGE = null;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (ugeId.HasValue && ugeId > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Id == ugeId && ugeSIAFEM.Status == true).FirstOrDefault();
                if (dadosUGE.IsNotNull())
                {
                    codigoUGE = dadosUGE.Code;
                    codigoUGE = codigoUGE.PadLeft(6, '0');
                }
            }

            return codigoUGE;
        }
        public bool ugeEhImplantadaNoSAMPatrimonio(int? ugeId)
        {
            bool ugeImplantadaNoSAM = false;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (ugeId.HasValue && ugeId > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits
                                              .Include("RelatedBudgetUnit.RelatedInstitution")
                                              .Where(ugeSIAFEM => ugeSIAFEM.Id == ugeId && ugeSIAFEM.Status == true)
                                              .FirstOrDefault();
                if (dadosUGE.IsNotNull())
                {
                    ugeImplantadaNoSAM = (dadosUGE.FlagTratarComoOrgao || dadosUGE.RelatedBudgetUnit.RelatedInstitution.flagImplantado);
                }
            }

            return ugeImplantadaNoSAM;
        }
        public bool verificaNaoImplantacaoENaoIntegracaoSIAFEM(int IdUGEOrigem)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            bool naoImplantadoENaoIntegrado = (from i in contextoCamadaDados.Institutions
                                               join b in contextoCamadaDados.BudgetUnits
                                               on i.Id equals b.InstitutionId
                                               join m in contextoCamadaDados.ManagerUnits
                                               on b.Id equals m.BudgetUnitId
                                               where m.Id == IdUGEOrigem
                                               select (!i.flagImplantado || m.FlagTratarComoOrgao || !m.FlagIntegracaoSiafem)).FirstOrDefault();

            return (naoImplantadoENaoIntegrado);
        }
        private int obterUgeId(int codigoUGE)
        {
            int UGEId = 0;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (codigoUGE > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Code == codigoUGE.ToString() 
                                                                            && ugeSIAFEM.Status == true)
                                                           .FirstOrDefault();
                if (dadosUGE.IsNotNull())
                    UGEId = dadosUGE.Id;
            }

            return UGEId;
        }
        private int obterCodigoContaContabil(int? contaContabilId)
        {
            int codigoContaContabil = 0;


            if (contaContabilId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    codigoContaContabil = contextoCamadaDados.AuxiliaryAccounts
                                                             .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                             .Select(contaContabil => contaContabil.BookAccount.Value)
                                                             .FirstOrDefault();
                }

            return codigoContaContabil;
        }
        private int obterCodigoContaContabilDepreciacao(int contaContabilDepreciacaoId)
        {
            int codigoContaContabilDepreciacao = 0;


            if (contaContabilDepreciacaoId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    codigoContaContabilDepreciacao = contextoCamadaDados.DepreciationAccounts
                                                                        .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                                                        .Select(contaContabilDepreciacao => contaContabilDepreciacao.Code)
                                                                        .FirstOrDefault();
                }

            return codigoContaContabilDepreciacao;
        }
        private int obterCodigoContaContabilDepreciacao__ContaContabil(int contaContabilId)
        {
            int codigoContaContabilDepreciacao = 0;


            if (contaContabilId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    codigoContaContabilDepreciacao = contextoCamadaDados.AuxiliaryAccounts
                                                                        .Where(contaContabil => (contaContabil.Id == contaContabilId) && (contaContabil.RelatedDepreciationAccount != null))
                                                                        .Select(contaContabil => ((contaContabil.RelatedDepreciationAccount != null) ? contaContabil.RelatedDepreciationAccount.Code : 0))
                                                                        .FirstOrDefault();
                }

            return codigoContaContabilDepreciacao;
        }

        /// <summary>
        /// Função para retornar o valor de 'TipoMovimentação ContabilizaSP' associado a Conta Contábil de Depreciação vinculado a Conta Contábil informada.
        /// </summary>
        /// <param name="codigoContaContabilId">Id da Conta Contábil</param>
        /// <returns></returns>
        public string obterTipoMovimentacaoDepreciacaoContabilizaSP__ContaContabil(int contaContabilId)
        {
            string tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao = null;
            if (contaContabilId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                    tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao = contextoCamadaDados.AuxiliaryAccounts
                                                                                                           .Where(contaContabil => contaContabil.Id == contaContabilId
                                                                                                                               && (contaContabil.DepreciationAccountId.HasValue && contaContabil.DepreciationAccountId > 0))
                                                                                                           .Select(contaContabil => contaContabil.RelatedDepreciationAccount.DescricaoContaDepreciacaoContabilizaSP)
                                                                                                           .FirstOrDefault();
                }

            return tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao;
        }
        /// <summary>
        /// Função para retornar o valor de 'TipoMovimentação ContabilizaSP' associado a Conta Contábil de Depreciação vinculado a Conta Contábil informada.
        /// </summary>
        /// <param name="codigoContaContabil">Código da Conta Contábil</param>
        /// <returns></returns>
        public string obterTipoMovimentacaoDepreciacaoContabilizaSP__ContaContabil(string codigoContaContabil)
        {
            string tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao = null;
            int iCodigoContaContabil = -1;

            if (!String.IsNullOrWhiteSpace(codigoContaContabil) && Int32.TryParse(codigoContaContabil, out iCodigoContaContabil))
                using (var contextoCamadaDados = new SAMContext())
                {
                    //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                    tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao = contextoCamadaDados.AuxiliaryAccounts
                                                                                                           .Where(contaContabil => contaContabil.BookAccount == iCodigoContaContabil
                                                                                                                               && (contaContabil.DepreciationAccountId.HasValue && contaContabil.DepreciationAccountId > 0))
                                                                                                           .Select(contaContabil => contaContabil.RelatedDepreciationAccount.DescricaoContaDepreciacaoContabilizaSP)
                                                                                                           .FirstOrDefault();
                }

            return tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao;
        }
        /// <summary>
        /// Função para retornar o valor de 'TipoMovimentação ContabilizaSP' vinculado a Conta Contábil informando o id da mesma.
        /// </summary>
        /// <param name="codigoContaContabilId">Id da Conta Contábil</param>
        /// <returns></returns>
        public string obterTipoMovimentacaoContabilizaSP__ContaContabil(int contaContabilId, bool insereSufixoPadraoParaInservivel = false)
        {
            string tipoMovimentacaoContabilizaSP_ContaContabil = null;


            if (contaContabilId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                    tipoMovimentacaoContabilizaSP_ContaContabil = contextoCamadaDados.AuxiliaryAccounts
                                                                                     .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                     .Select(contaContabil => contaContabil.TipoMovimentacaoContabilizaSP)
                                                                                     .FirstOrDefault();

                    if (insereSufixoPadraoParaInservivel)
                        tipoMovimentacaoContabilizaSP_ContaContabil = tipoMovimentacaoContabilizaSP_ContaContabil + " - INS";

                }

            return tipoMovimentacaoContabilizaSP_ContaContabil;
        }
        /// <summary>
        /// Função para retornar o valor de 'TipoMovimentação ContabilizaSP' vinculado a Conta Contábil informando o código da mesma.
        /// </summary>
        /// <param name="codigoContaContabil">Código da Conta Contábil</param>
        /// <returns></returns>
        /// <summary>
        /// Função para retornar o valor de 'TipoMovimentação ContabilizaSP' vinculado a Conta Contábil de Depreciação informada
        /// </summary>
        /// <param name="contaContabilDepreciacaoId">Id da Conta Contábil de Depreciação</param>
        /// <returns></returns>
        public string obterTipoMovimentacaoContabilizaSP__ContaContabilDepreciacao(int contaContabilDepreciacaoId)
        {
            string tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao = null;
            if (contaContabilDepreciacaoId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                    tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao = contextoCamadaDados.DepreciationAccounts
                                                                                                           .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                                                                                           .Select(contaContabilDepreciacao => contaContabilDepreciacao.DescricaoContaDepreciacaoContabilizaSP)
                                                                                                           .FirstOrDefault();

                }

            return tipoMovimentacaoDepreciacaoContabilizaSP_ContaContabilDepreciacao;
        }
        /// <summary>
        /// Função para retornar o valor de 'TipoMovimentação ContabilizaSP' vinculado a Conta Contábil de Depreciação informada
        /// </summary>
        /// <param name="codigoContaContabilDepreciacao">Código da Conta Contábil de Depreciação</param>
        /// <returns></returns>
        public string obterTipoMovimentacaoContabilizaSP__ContaContabilDepreciacao(string codigoContaContabilDepreciacao)
        {
            string tipoMovimentacaoContabilizaSP_ContaContabil = null;
            int iCodigoContaContabilDepreciacao = -1;

            if (!String.IsNullOrWhiteSpace(codigoContaContabilDepreciacao) && Int32.TryParse(codigoContaContabilDepreciacao, out iCodigoContaContabilDepreciacao))
                using (var contextoCamadaDados = new SAMContext())
                {
                    //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                    tipoMovimentacaoContabilizaSP_ContaContabil = contextoCamadaDados.DepreciationAccounts
                                                                                     .Where(contaContabilDepreciacao => contaContabilDepreciacao.Code == iCodigoContaContabilDepreciacao)
                                                                                     .Select(contaContabilDepreciacao => contaContabilDepreciacao.DescricaoContaDepreciacaoContabilizaSP)
                                                                                     .FirstOrDefault();
                }

            return tipoMovimentacaoContabilizaSP_ContaContabil;
        }

        private string obterGestaoOrigemBP(int? assetTransferenciaId)
        {
            string gestaoOriginalBP = null;
            int ugeId = -1;

            using (var contextoCamadaDados = new SAMContext())
            {
                //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                ugeId = contextoCamadaDados.Assets
                                           .Where(bemPatrimonial => bemPatrimonial.Id == assetTransferenciaId)
                                           .Select(bemPatrimonial => bemPatrimonial.ManagerUnitId)
                                           .FirstOrDefault();

                gestaoOriginalBP = ((ugeId > 0) ? obterGestaoUGE(ugeId) : null);
            }

            return gestaoOriginalBP;
        }
        private string obterUgeOrigemBP(int? assetTransferenciaId)
        {
            string ugeOriginalBP = null;
            int ugeId = -1;

            using (var contextoCamadaDados = new SAMContext())
            {
                //BaseController.InitDisconnectContext(ref contextoCamadaDados);
                ugeId = contextoCamadaDados.Assets
                                           .Where(bemPatrimonial => bemPatrimonial.Id == assetTransferenciaId)
                                           .Select(bemPatrimonial => bemPatrimonial.ManagerUnitId)
                                           .FirstOrDefault();

                ugeOriginalBP = ((ugeId > 0) ? this.obterCodigoUGE(ugeId) : null);
            }

            return ugeOriginalBP;
        }

        private string montaChaveSIAFMonitora(ISIAFNlPatrimonial objImplSIAFNlPatrimonial, bool ehEstorno)
        {
            string chavePesquisaSIAFMonitora = null;
            string tipoNotaContabilizaSP = null;
            string acaoPagamentoNotaLancamento = null;
            string tipoLancamento = null;



            #region Chaves para MONITORASIAF
            switch ((EnumAccountEntryType.AccountEntryType)objImplSIAFNlPatrimonial.AccountEntryTypeId)
            {
                case EnumAccountEntryType.AccountEntryType.Entrada:         { tipoNotaContabilizaSP = "E"; }; break;
                case EnumAccountEntryType.AccountEntryType.Saida:           { tipoNotaContabilizaSP = "S"; }; break;
                case EnumAccountEntryType.AccountEntryType.Depreciacao:     { tipoNotaContabilizaSP = "D"; }; break;
                case EnumAccountEntryType.AccountEntryType.Reclassificacao: { tipoNotaContabilizaSP = "R"; }; break;
            }
            #endregion Chaves para MONITORASIAF

            tipoLancamento = ((ehEstorno) ? "S" : "N");
            acaoPagamentoNotaLancamento = ((ehEstorno) ? "E" : "N");
            chavePesquisaSIAFMonitora = String.Format("{0}{1}_tokenSAMP:{2}", tipoNotaContabilizaSP, acaoPagamentoNotaLancamento, objImplSIAFNlPatrimonial.TokenGeradoEnvioContabilizaSP);
            return chavePesquisaSIAFMonitora;
        }

        private AuditoriaIntegracao criaRegistroAuditoria(ISIAFNlPatrimonial objImplSIAFNlPatrimonial, bool ehEstorno = false)
        {
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            if (objImplSIAFNlPatrimonial.IsNotNull())
            {
                registroAuditoriaIntegracao = new AuditoriaIntegracao();

                registroAuditoriaIntegracao.DocumentoId                                    = montaChaveSIAFMonitora(objImplSIAFNlPatrimonial, ehEstorno);
                registroAuditoriaIntegracao.TipoMovimento                                  = objImplSIAFNlPatrimonial.AccountEntryType;
                registroAuditoriaIntegracao.Data                                           = objImplSIAFNlPatrimonial.MovimentDate;
                registroAuditoriaIntegracao.UgeOrigem                                      = objImplSIAFNlPatrimonial.ManagerUnitCode.ToString("D6");
                registroAuditoriaIntegracao.Gestao                                         = objImplSIAFNlPatrimonial.ManagerCode.ToString("D5") ;
                registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = objImplSIAFNlPatrimonial.InputOutputReclassificationDepreciationType ;
                registroAuditoriaIntegracao.CpfCnpjUgeFavorecida                           = (objImplSIAFNlPatrimonial.FavoreceDestino ? objImplSIAFNlPatrimonial.CpfCnpjUgeFavorecido : null);
                registroAuditoriaIntegracao.GestaoFavorecida                               = (objImplSIAFNlPatrimonial.FavoreceDestino ? objImplSIAFNlPatrimonial.GestaoFavorecida : null);
                registroAuditoriaIntegracao.Item                                           = objImplSIAFNlPatrimonial.RelacaoItemMaterial.Keys.FirstOrDefault();
                registroAuditoriaIntegracao.TipoEstoque                                    = objImplSIAFNlPatrimonial.StockType;
                registroAuditoriaIntegracao.Estoque                                        = objImplSIAFNlPatrimonial.StockDescription;
                registroAuditoriaIntegracao.EstoqueDestino                                 = objImplSIAFNlPatrimonial.StockDestination;
                registroAuditoriaIntegracao.EstoqueOrigem                                  = objImplSIAFNlPatrimonial.StockSource;
                registroAuditoriaIntegracao.TipoMovimentacao                               = objImplSIAFNlPatrimonial.DescricaoTipoMovimentacao_ContabilizaSP;
                registroAuditoriaIntegracao.ValorTotal                                     = objImplSIAFNlPatrimonial.AccountingValueField;
                registroAuditoriaIntegracao.ControleEspecifico                             = objImplSIAFNlPatrimonial.SpecificControl;
                registroAuditoriaIntegracao.ControleEspecificoEntrada                      = objImplSIAFNlPatrimonial.SpecificInputControl;
                registroAuditoriaIntegracao.ControleEspecificoSaida                        = objImplSIAFNlPatrimonial.SpecificOutputControl;
                registroAuditoriaIntegracao.FonteRecurso                                   = string.Empty;  //sempre enviar em brabnco
                registroAuditoriaIntegracao.NLEstorno                                      = (ehEstorno ? "S" : "N");
                registroAuditoriaIntegracao.Empenho                                        = objImplSIAFNlPatrimonial.CommitmentNumber;
                registroAuditoriaIntegracao.Observacao                                     = montaCampoObservacao(objImplSIAFNlPatrimonial);
                registroAuditoriaIntegracao.NotaFiscal                                     = objImplSIAFNlPatrimonial.DocumentNumberSAM;
                registroAuditoriaIntegracao.ItemMaterial                                   = objImplSIAFNlPatrimonial.RelacaoItemMaterial.Keys.FirstOrDefault();
                registroAuditoriaIntegracao.TokenAuditoriaIntegracao                       = objImplSIAFNlPatrimonial.TokenGeradoEnvioContabilizaSP;
                registroAuditoriaIntegracao.MsgErro                                        = "SESSÃO EXPIRADA OU CONEXÃO INTERROMPIDA ANTES DE FINALIZAR O PROCESSO DE INTEGRAÇÃO. FAVOR CLICAR NO  BOTÃO REENVIAR PARA GERAR A NL NO SIAFEM";


                registroAuditoriaIntegracao.ManagerUnitId                                  = obterUgeId(objImplSIAFNlPatrimonial.ManagerUnitCode);
                registroAuditoriaIntegracao.DataEnvio                                      = DateTime.Now;
            }



            return registroAuditoriaIntegracao;
        }

        private string montaCampoObservacao(ISIAFNlPatrimonial objImplSIAFNlPatrimonial)
        {
            string textoCampoObservacao = null;
            string quebraDeLinha = null;
            string tokenSamPatrimonio = null;
            string descricaoTipoMovimentacao = null;
            string documentoSAM = null;
            string indicativoEstorno = null;
            int tamanhoString = -1;



            //quebra de linha
            quebraDeLinha = Environment.NewLine;

            //token SAM-Patrimonio
            tokenSamPatrimonio = String.Format("'Token SAM-Patrimonio': '{0}'", objImplSIAFNlPatrimonial.TokenGeradoEnvioContabilizaSP.ToString());

            //se movimentacao eh de estorno
            indicativoEstorno = (objImplSIAFNlPatrimonial.EhEstorno ? "(ESTORNO) " : "");

            //descricao TipoMovimento+TipoMovimentacao (p.e.: 'ENTRADA':'DOAÇÃO CONSOLIDAÇÃO')
            //descricaoTipoMovimentacao = String.Format("{0}'{1}':'{2}'", indicativoEstorno, objImplSIAFNlPatrimonial.AccountEntryType, objImplSIAFNlPatrimonial.InputOutputReclassificationDepreciationType);
            descricaoTipoMovimentacao = String.Format("{0}'{1}':'{2}'", indicativoEstorno, objImplSIAFNlPatrimonial.AccountEntryType, objImplSIAFNlPatrimonial.DescricaoTipoMovimento_SamPatrimonio);

            //numero documento SAM (p.e.: 'DocumentoSAM':'20203802620013')
            documentoSAM = String.Format("'DocumentoSAM': '{0}'", objImplSIAFNlPatrimonial.DocumentNumberSAM);

            //momento 1 --> ([TipoMovimento+TipoMovimentacao] + \n + [numero documento SAM])
            textoCampoObservacao = String.Format("{1};{0}{2};", quebraDeLinha, descricaoTipoMovimentacao, documentoSAM);
            tamanhoString = ((textoCampoObservacao.Length > GeradorEstimuloSIAF.CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN__SAM_PATRIMONIO) ? GeradorEstimuloSIAF.CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN__SAM_PATRIMONIO : textoCampoObservacao.Length);
            //truncar primeira string, se comprimento ultrapassar 169 (para preservar tamanho completo da descricao do token; 62 caracteres)
            textoCampoObservacao = textoCampoObservacao.Substring(0, tamanhoString);

            //momento 2 ([TipoMovimento+TipoMovimentacao] + \n + [numero documento SAM] + \n + [token SAM-Patrimonio])
            textoCampoObservacao = String.Format("{0}{1}{2}.", textoCampoObservacao, quebraDeLinha, tokenSamPatrimonio);



            return textoCampoObservacao;
        }

        public IList<Tuple<string, TipoNotaSIAF, int>> GeraMensagensExtratoPreProcessamentoSIAFEM(IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote, bool geraRegistrosAuditoriaIntegracao = false, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido, bool ehEstorno = false)
        {
            #region Variaveis
            Asset bemPatrimonial = null;
            AssetMovements primeiraMovimentacaoLote = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoParaBD = null;
            ImplSIAFNlPatrimonial[] objsParaGerarXML = null;
            List<ImplSIAFNlPatrimonial> listaObjParaGerarXML = null;
            IList<Tuple<string, TipoNotaSIAF, int>> listaMovPatrimonialNoExtrato = null;
            string fmtMensagemPreProcessamentoPadrao = null;
            string fmtMensagemPreProcessamentoDepreciacao = null;
            string fmtMensagemPreProcessamentoReclassificacao = null;
            string fmtMensagemPreProcessamentoIncorporacao_OrigemSAMEstoque = null;
            string fmtMensagemParaUsuario = null;
            string msgParaUsuario = null;
            string nomeTipoMovimento = null;
            string nomeTipoMovimentacao = null;
            string contaContabil = null;
            string contaContabilDepreciacao = null;
            string contaContabilOrigem = null;
            string caracterFinalizadorFrase = null;
            decimal valorNotaLancamento = 0.00m;
            CultureInfo cultureInfo = null;
            int contadorMovimentacoes = 1;
            TipoNotaSIAF tipoNotaSIAF = TipoNotaSIAF.Desconhecido;
            IList<KeyValuePair<int, int>> paresMovPatrimonialId_BemPatrimonialId = null;
            string xmlParaWebserviceModoNovo = null;
            int auditoriaIntegracaoId = 0;
            string indicativoDeEstorno = null;
            #endregion Variaveis





            if (listaMovimentacoesPatrimoniaisEmLote.HasElements())
            {
                primeiraMovimentacaoLote = listaMovimentacoesPatrimoniaisEmLote.FirstOrDefault();
                if (primeiraMovimentacaoLote.IsNotNull())
                {
                    indicativoDeEstorno = (ehEstorno ? "Estorno de " : null);
                    fmtMensagemParaUsuario = null;
                    fmtMensagemPreProcessamentoPadrao = "{0}'{1}' do tipo '{2}' na conta contábil '{3}' no valor de {4:C2}{5}";
                    fmtMensagemPreProcessamentoDepreciacao = "{0}'{1}' do tipo '{2}' na conta contábil '{7}' no valor de {4:C2}{5}";
                    fmtMensagemPreProcessamentoReclassificacao = "{0}'{1}' do tipo '{2}' com conta contábil de origem '{6}' para conta contábil de destino '{3}' no valor de {4:C2}{5}";
                    fmtMensagemPreProcessamentoIncorporacao_OrigemSAMEstoque = "{0}'Reclassificação da Conta de Estoque para Patrimônio' com incorporação no valor de {4:C2} para a conta contábil {3}{5}";
                    cultureInfo = new CultureInfo("pt-BR");
                    objsParaGerarXML = new ImplSIAFNlPatrimonial[] { };
                    listaObjParaGerarXML = new List<ImplSIAFNlPatrimonial>();
                    listaMovPatrimonialNoExtrato = new List<Tuple<string, TipoNotaSIAF, int>>();
                    caracterFinalizadorFrase = ((listaObjParaGerarXML.Count() > 1) ? ";" : ".");


                    //objeto 'SortedList' consolidando os dados da lista de movimentacoes passada como parametro
                    var dadosMovimentacoesPatrimoniais = this.consolidadorDadosUmaSohContaContabil(listaMovimentacoesPatrimoniaisEmLote);

                    //AssetMovements consolidando os dados da lista de movimentacoes passada como parametro
                    var movPatrimonialConsolidada = this.consolidadorDadosEmObjetoAssetMovements(dadosMovimentacoesPatrimoniais);
                    bemPatrimonial = movPatrimonialConsolidada.RelatedAssets;


                    //Pares AssetMovementsId/AssetId para amarracao com a AuditoriaIntegracao
                    paresMovPatrimonialId_BemPatrimonialId = (IList<KeyValuePair<int, int>>)dadosMovimentacoesPatrimoniais["MovPatrimonialId_BemPatrimonialId"];


                    //Dados para popular as colunas da tabela AuditoriaIntegracao para gerar o XML
                    listaObjParaGerarXML = createObjetoInterface_ContabilizaSP(bemPatrimonial, ehEstorno, tipoNotaSIAFEM).ToList();

                    contaContabil = dadosMovimentacoesPatrimoniais["ContaContabil"].ToString();
                    contaContabilDepreciacao = dadosMovimentacoesPatrimoniais["ContaContabilDepreciacao"].ToString();
                    contaContabilOrigem = dadosMovimentacoesPatrimoniais["ContaContabilOrigem"].ToString();
                    nomeTipoMovimentacao = dadosMovimentacoesPatrimoniais["NomeTipoMovimentacao"].ToString();


                    if (listaObjParaGerarXML.HasElements())
                    {
                        int[] tiposIncorporacao_Origem_SAMEstoque = new int[] {
                                                                                  (int)EnumMovimentType.IncorpIntegracaoSAMEstoque_SAMPatrimonio_OutraUGE
                                                                                , (int)EnumMovimentType.ReclassificacaoIntegracaoSAMEstoque_SAMPatrimonio_MesmaUGE
                                                                              };

                        foreach (var objImplSIAFNlPatrimonial in listaObjParaGerarXML)
                        {
                            #region Mensagem-Resumo do Extrato da Movimentacao
                            if (tiposIncorporacao_Origem_SAMEstoque.Contains(objImplSIAFNlPatrimonial.InputOutputReclassificationDepreciationTypeCode))
                            {
                                tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao;
                                fmtMensagemParaUsuario = fmtMensagemPreProcessamentoIncorporacao_OrigemSAMEstoque;
                            }
                            else
                            {
                                switch (objImplSIAFNlPatrimonial.AccountEntryTypeId)
                                {
                                    case (int)EnumAccountEntryType.AccountEntryType.Entrada:
                                    case (int)EnumAccountEntryType.AccountEntryType.Saida:              { tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao;        fmtMensagemParaUsuario = fmtMensagemPreProcessamentoPadrao; }; break;
                                    case (int)EnumAccountEntryType.AccountEntryType.Depreciacao:        { tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao;       fmtMensagemParaUsuario = fmtMensagemPreProcessamentoDepreciacao; }; break;
                                    case (int)EnumAccountEntryType.AccountEntryType.Reclassificacao:    { tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao;   fmtMensagemParaUsuario = fmtMensagemPreProcessamentoReclassificacao; }; break;
                                }
                            }

                            caracterFinalizadorFrase = ((contadorMovimentacoes < listaObjParaGerarXML.Count()) ? ";" : ".");
                            //fmtMensagemParaUsuario = ((objImplSIAFNlPatrimonial.AccountEntryTypeId != EnumAccountEntryType.AccountEntryType.Reclassificacao.GetHashCode()) ? fmtMensagemPreProcessamentoPadrao : fmtMensagemPreProcessamentoReclassificacao);
                            nomeTipoMovimento = objImplSIAFNlPatrimonial.AccountEntryType;
                            valorNotaLancamento = objImplSIAFNlPatrimonial.AccountingValueField;

                            msgParaUsuario = String.Format(cultureInfo, fmtMensagemParaUsuario, indicativoDeEstorno, nomeTipoMovimento, nomeTipoMovimentacao, contaContabil, valorNotaLancamento, caracterFinalizadorFrase, contaContabilOrigem, contaContabilDepreciacao);
                            contadorMovimentacoes++;
                            #endregion Mensagem-Resumo do Extrato da Movimentacao

                            #region Criacao dos XML
                            if (geraRegistrosAuditoriaIntegracao)
                            {
                                registroAuditoriaIntegracaoParaBD = this.criaRegistroAuditoria(objImplSIAFNlPatrimonial, ehEstorno);
                                xmlParaWebserviceModoNovo = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(registroAuditoriaIntegracaoParaBD, ehEstorno);

                                registroAuditoriaIntegracaoParaBD.MsgEstimuloWS = xmlParaWebserviceModoNovo;
                                insereRegistroAuditoriaParcialNaBaseDeDados(registroAuditoriaIntegracaoParaBD);
                                geraNotaLancamentoPendencia(registroAuditoriaIntegracaoParaBD);
                                amarraAuditoriaComMovimentacaoEBemPatrimonial(registroAuditoriaIntegracaoParaBD, paresMovPatrimonialId_BemPatrimonialId);


                                auditoriaIntegracaoId = registroAuditoriaIntegracaoParaBD.Id;
                            }
                            #endregion Criacao dos XML

                            listaMovPatrimonialNoExtrato.Add(Tuple.Create<string, TipoNotaSIAF, int>(msgParaUsuario, tipoNotaSIAF, auditoriaIntegracaoId));

                            //limpeza
                            registroAuditoriaIntegracaoParaBD = null;
                            xmlParaWebserviceModoNovo = null;
                            auditoriaIntegracaoId = 0;
                        }
                    }
                }
            }

            return listaMovPatrimonialNoExtrato;
        }

        public bool geraNotaLancamentoPendencia(AuditoriaIntegracao registroAuditoriaIntegracao, bool criaAmarracaoComMovimentacaoPatrimonial = true, IList<KeyValuePair<int, int>> paresMovPatrimonialId_BemPatrimonialId = null)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            IList<AssetMovements> listaMovimentacaoPatrimonial = null;
            IList<int> listaMovimentacaoPatrimonialId = null;
            TipoNotaSIAF tipoAgrupamentoNotaLancamento = TipoNotaSIAF.Desconhecido;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;


            if (registroAuditoriaIntegracao.IsNotNull())
            {
                tipoAgrupamentoNotaLancamento = obterTipoNotaSIAFEM__AuditoriaIntegracao(registroAuditoriaIntegracao);
                pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
															                 {
															                     AuditoriaIntegracaoId  = registroAuditoriaIntegracao.Id,
															                     DataHoraEnvioMsgWS     = registroAuditoriaIntegracao.DataEnvio,
															                     ManagerUnitId          = registroAuditoriaIntegracao.ManagerUnitId,
															                     TipoNotaPendencia      = (short)tipoAgrupamentoNotaLancamento,
															                     NumeroDocumentoSAM     = registroAuditoriaIntegracao.NotaFiscal,
															                     StatusPendencia        = 1, //Status: Ativo
															                     ErroProcessamentoMsgWS = "SESSÃO EXPIRADA OU CONEXÃO INTERROMPIDA ANTES DE FINALIZAR O PROCESSO DE INTEGRAÇÃO. FAVOR CLICAR NO  BOTÃO REENVIAR PARA GERAR A NL NO SIAFEM",
															                 };

                using (_contextoCamadaDados = new SAMContext())
                {
                    _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                    numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();

                    if (criaAmarracaoComMovimentacaoPatrimonial && paresMovPatrimonialId_BemPatrimonialId.HasElements())
                    {
                        listaMovimentacaoPatrimonialId = paresMovPatrimonialId_BemPatrimonialId.Select(parMovimentacaoPatrimonial_BemPatrimonialId => parMovimentacaoPatrimonial_BemPatrimonialId.Key)
                                                                                               .ToList();
                        if (listaMovimentacaoPatrimonialId.HasElements())
                        {
                            listaMovimentacaoPatrimonial = _contextoCamadaDados.AssetMovements
                                                                               .Where(_movPatrimonial => listaMovimentacaoPatrimonialId.Contains(_movPatrimonial.Id))
                                                                               .ToList();
                            if (listaMovimentacaoPatrimonial.HasElements())
                            {
                                foreach (var movimentacaoPatrimonial in listaMovimentacaoPatrimonial)
                                    movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId = pendenciaNLContabilizaSP.Id;

                                numeroRegistrosManipulados += _contextoCamadaDados.SaveChanges();
                            }
                        }
                    }
                }
            }


            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }

        public IList<Tuple<string[], IList<KeyValuePair<int, int>>, int>> GeraXMLsDeOutroModo(IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            #region Variaveis
            Asset bemPatrimonial = null;
            AssetMovements primeiraMovimentacaoLote = null;
            ImplSIAFNlPatrimonial[] objsParaGerarXML = null;
            List<ImplSIAFNlPatrimonial> listaObjParaGerarXML = null;
            List<string> listaMsgXml_ContabilizaSP = null;
            IList<Tuple<string[], IList<KeyValuePair<int, int>>, int>> relacaoDadosMsgEstimulos_ContabilizaSP = null;
            Tuple<string[], IList<KeyValuePair<int, int>>, int> dadosMsgEstimulos_ContabilizaSP = null;
            IList<KeyValuePair<int, int>> paresMovPatrimonialId_BemPatrimonialId = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoParaBD = null;
            #endregion Variaveis





            if (listaMovimentacoesPatrimoniaisEmLote.HasElements())
            {
                primeiraMovimentacaoLote = listaMovimentacoesPatrimoniaisEmLote.FirstOrDefault();
                if (primeiraMovimentacaoLote.IsNotNull())
                {
                    objsParaGerarXML = new ImplSIAFNlPatrimonial[] { };
                    listaObjParaGerarXML = new List<ImplSIAFNlPatrimonial>();
                    relacaoDadosMsgEstimulos_ContabilizaSP = new List<Tuple<string[], IList<KeyValuePair<int, int>>, int>>();


                    //objeto 'SortedList' consolidando os dados da lista de movimentacoes passada como parametro
                    var agrupamentoDadosMovimentacoesPatrimoniais = this.consolidadorDadosUmaSohContaContabil(listaMovimentacoesPatrimoniaisEmLote);

                    //AssetMovements consolidando os dados da lista de movimentacoes passada como parametro
                    var movPatrimonialConsolidada = this.consolidadorDadosEmObjetoAssetMovements(agrupamentoDadosMovimentacoesPatrimoniais);

                    //Pares AssetMovementsId/AssetId para amarracao com a AuditoriaIntegracao
                    paresMovPatrimonialId_BemPatrimonialId = (IList<KeyValuePair<int, int>>)agrupamentoDadosMovimentacoesPatrimoniais["MovPatrimonialId_BemPatrimonialId"];


                    bemPatrimonial = movPatrimonialConsolidada.RelatedAssets;

                    bool ehEstorno = (primeiraMovimentacaoLote.Status == false &&
                                      primeiraMovimentacaoLote.FlagEstorno == true &&
                                      primeiraMovimentacaoLote.DataEstorno.HasValue &&
                                      primeiraMovimentacaoLote.LoginEstorno.HasValue);

                    //Dados para popular as colunas da tabela AuditoriaIntegracao para gerar o XML
                    listaObjParaGerarXML = createObjetoInterface_ContabilizaSP(bemPatrimonial, ehEstorno, tipoNotaSIAFEM).ToList();

                    if (listaObjParaGerarXML.HasElements())
                    {
                        listaMsgXml_ContabilizaSP = new List<string>();
                        foreach (var objImplSIAFNlPatrimonial in listaObjParaGerarXML)
                        {
                            //var xmlParaWebserviceModoAntigo = GeradorEstimuloSIAF.SiafemDocNLPatrimonial(objImplSIAFNlPatrimonial, ehEstorno);
                            //listaMsgXml_ContabilizaSP.Add(xmlParaWebserviceModoAntigo);


                            registroAuditoriaIntegracaoParaBD = this.criaRegistroAuditoria(objImplSIAFNlPatrimonial, ehEstorno);
                            var xmlParaWebserviceModoNovo = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(registroAuditoriaIntegracaoParaBD, ehEstorno);

                            registroAuditoriaIntegracaoParaBD.MsgEstimuloWS = xmlParaWebserviceModoNovo;
                            insereRegistroAuditoriaParcialNaBaseDeDados(registroAuditoriaIntegracaoParaBD);
                            amarraAuditoriaComMovimentacaoEBemPatrimonial(registroAuditoriaIntegracaoParaBD, paresMovPatrimonialId_BemPatrimonialId);

                            listaMsgXml_ContabilizaSP.Add(xmlParaWebserviceModoNovo);
                            //var ehIgual = (xmlParaWebserviceModoAntigo == xmlParaWebserviceModoNovo);

                            dadosMsgEstimulos_ContabilizaSP = Tuple.Create<string[], IList<KeyValuePair<int, int>>, int>(listaMsgXml_ContabilizaSP.ToArray(), paresMovPatrimonialId_BemPatrimonialId, registroAuditoriaIntegracaoParaBD.Id);

                            registroAuditoriaIntegracaoParaBD = null;
                            relacaoDadosMsgEstimulos_ContabilizaSP.Add(dadosMsgEstimulos_ContabilizaSP);
                        }
                    }
                }
            }
            return relacaoDadosMsgEstimulos_ContabilizaSP;
        }

        private bool amarraAuditoriaComMovimentacaoEBemPatrimonial(AuditoriaIntegracao registroAuditoriaIntegracao, IList<KeyValuePair<int, int>> paresMovPatrimonialId_BemPatrimonialId)
        {
            int numeroRegistrosManipulados = 0;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracao = null;
            AssetMovements registroMovimentacaoPatrimonial = null;

            try
            {
                if (registroAuditoriaIntegracao.Id > 0)
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        foreach (var parMovPatrimonialId_BemPatrimonialId in paresMovPatrimonialId_BemPatrimonialId)
                        {
                            registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                            registroDeAmarracao.AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id;
                            registroDeAmarracao.AssetMovementsId = parMovPatrimonialId_BemPatrimonialId.Key;
                            registroDeAmarracao.AssetId = parMovPatrimonialId_BemPatrimonialId.Value;

                            contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Add(registroDeAmarracao);

                            //atualizacao da linha da tabela AssetMovements com o dado AuditoriaintegracaoId
                            registroMovimentacaoPatrimonial = contextoCamadaDados.AssetMovements.Where(movPatrimonial => movPatrimonial.Id == parMovPatrimonialId_BemPatrimonialId.Key).FirstOrDefault();
                            if (registroMovimentacaoPatrimonial.IsNotNull())
                            {

                                contextoCamadaDados.AssetMovements.Attach(registroMovimentacaoPatrimonial);
                                registroMovimentacaoPatrimonial.AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id;
                                contextoCamadaDados.Entry(registroMovimentacaoPatrimonial).State = EntityState.Modified;

                            }
                        }

                        numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    }
            }
            catch (Exception excErroRuntime)
            {
                string messageErro = excErroRuntime.Message;
                string stackTrace = excErroRuntime.StackTrace;
                string name = "amarraAuditoriaComMovimentacaoEBemPatrimonial";
                GravaLogErro(messageErro, stackTrace, name);
                throw excErroRuntime;
            }

            return (numeroRegistrosManipulados > 0);
        }
        private bool insereRegistroAuditoriaParcialNaBaseDeDados(AuditoriaIntegracao entidadeAuditoria)
        {
            int numeroRegistrosManipulados = 0;
            SAMContext contextoCamadaDados = null;
            try
            {
                using (contextoCamadaDados = new SAMContext())
                {
                    contextoCamadaDados.AuditoriaIntegracoes.Add(entidadeAuditoria);
                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";
                ErroLog rowTabelaLOG = new ErroLog()
                {
                    DataOcorrido = DateTime.Now,
                    Usuario = entidadeAuditoria.UsuarioSAM,
                    exMessage = msgErro,
                    exStackTrace = excErroOperacaoBancoDados.StackTrace,
                    exTargetSite = excErroOperacaoBancoDados.TargetSite.ToString()
                };

                contextoCamadaDados.ErroLogs.Add(rowTabelaLOG);
            }

            return (numeroRegistrosManipulados > 0);
        }
        private bool atualizaAuditoriaIntegracao(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            int numeroRegistrosManipulados = 0;

            try
            {
                if (registroAuditoriaIntegracao.Id > 0)
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        contextoCamadaDados.AuditoriaIntegracoes.Attach(registroAuditoriaIntegracao);
                        contextoCamadaDados.Entry(registroAuditoriaIntegracao).State = EntityState.Modified;


                        numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    }
            }
            catch (Exception excErroRuntime)
            {
                throw excErroRuntime;
            }

            return (numeroRegistrosManipulados > 0);
        }

        public IList<Tuple<string[], IList<int>>> GeradorEstimulosXML_ContabilizaSP(IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            string descricaoTipoMovimentacao = null;
            string numeroDocumentoSAM = null;
            Asset bemPatrimonial = null;
            IList<AssetMovements> agrupamentosMovPatrimonial = null;
            ImplSIAFNlPatrimonial[] objsParaGerarXML = null;
            List<ImplSIAFNlPatrimonial> listaObjParaGerarXML = null;
            List<string> listaMsgXml_ContabilizaSP = null;
            IList<Tuple<string[], IList<int>>> relacaoDadosMsgEstimulos_ContabilizaSP = null;
            Tuple<string[], IList<int>> dadosMsgEstimulos_ContabilizaSP = null;
            int? assetTransferenciaId = null;
            int? ugeOrigemDestinoTransferenciaOuDoacaoId = null;
            string cpfCnpjUgeFavorecida = null;
            DateTime dataMovimento = DateTime.MinValue;
            DateTime dataAquisicao = DateTime.MinValue;





            SAMContext contextoCamadaDados = new Models.SAMContext();
            BaseController.InitDisconnectContext(ref contextoCamadaDados);
            if (listaMovimentacoesPatrimoniaisEmLote.HasElements())
            {
                relacaoDadosMsgEstimulos_ContabilizaSP = new List<Tuple<string[], IList<int>>>();
                var primeiraMovimentacaoVinculadaLote = listaMovimentacoesPatrimoniaisEmLote.FirstOrDefault();
                if (primeiraMovimentacaoVinculadaLote.IsNotNull())
                {
                    var tipoMovPatrimonialId            = primeiraMovimentacaoVinculadaLote.MovementTypeId;
                    var tipoMovPatrimonial              = contextoCamadaDados.MovementTypes.Where(movPatrimonial => movPatrimonial.Id == tipoMovPatrimonialId).FirstOrDefault();
                    var ugeMovimentacaoPatrimonialId    = primeiraMovimentacaoVinculadaLote.ManagerUnitId;
                    var ugeMovimentacaoPatrimonial      = contextoCamadaDados.ManagerUnits.Where(ugeMovPatrimonial => ugeMovPatrimonial.Id == ugeMovimentacaoPatrimonialId).FirstOrDefault();
                    var orgaoMovimentacaoPatrimonialId  = primeiraMovimentacaoVinculadaLote.InstitutionId;
                    var orgaoMovimentacaoPatrimonial    = contextoCamadaDados.Institutions.Where(orgaoMovPatrimonial => orgaoMovPatrimonial.Id == orgaoMovimentacaoPatrimonialId).FirstOrDefault();

                    numeroDocumentoSAM = primeiraMovimentacaoVinculadaLote.NumberDoc;

                    if (tipoMovPatrimonial.IsNull())
                        primeiraMovimentacaoVinculadaLote.RelatedMovementType = tipoMovPatrimonial = contextoCamadaDados.MovementTypes.Where(tipoMovimentacao => tipoMovimentacao.Id == primeiraMovimentacaoVinculadaLote.MovementTypeId)
                                                                                                                                      .FirstOrDefault();

                    if (ugeMovimentacaoPatrimonial.IsNull())
                        primeiraMovimentacaoVinculadaLote.RelatedManagerUnit = ugeMovimentacaoPatrimonial = contextoCamadaDados.ManagerUnits.Where(_ugeMovimentacaoPatrimonial => _ugeMovimentacaoPatrimonial.Id == primeiraMovimentacaoVinculadaLote.ManagerUnitId)
                                                                                                                                            .FirstOrDefault();

                    if (orgaoMovimentacaoPatrimonial.IsNull())
                        primeiraMovimentacaoVinculadaLote.RelatedInstitution = orgaoMovimentacaoPatrimonial = contextoCamadaDados.Institutions.Where(_orgaoMovimentacaoPatrimonial => _orgaoMovimentacaoPatrimonial.Id == primeiraMovimentacaoVinculadaLote.InstitutionId)
                                                                                                                                              .FirstOrDefault();


                    descricaoTipoMovimentacao = ((tipoMovPatrimonial.IsNotNull()) ? tipoMovPatrimonial.Description : "DESCONHECIDO");
                    objsParaGerarXML = new ImplSIAFNlPatrimonial[] { };
                    listaObjParaGerarXML = new List<ImplSIAFNlPatrimonial>();
                    assetTransferenciaId = primeiraMovimentacaoVinculadaLote.AssetTransferenciaId;
                    ugeOrigemDestinoTransferenciaOuDoacaoId = primeiraMovimentacaoVinculadaLote.SourceDestiny_ManagerUnitId;
                    cpfCnpjUgeFavorecida = primeiraMovimentacaoVinculadaLote.CPFCNPJ;
                    dataMovimento = primeiraMovimentacaoVinculadaLote.MovimentDate;
                    dataAquisicao = primeiraMovimentacaoVinculadaLote.RelatedAssets.AcquisitionDate;


                    InitAuxiliaryAccountsCache();
                    agrupamentosMovPatrimonial = this.ParticionadorLista_ContaContabilGrupoMaterial(listaMovimentacoesPatrimoniaisEmLote);
                    bemPatrimonial = new Asset() { RelatedMovementType = tipoMovPatrimonial, NumberDoc = numeroDocumentoSAM, MovimentDate = dataMovimento, AcquisitionDate = dataAquisicao };
                    foreach (var dadosAgrupamentosMovPatrimonial in agrupamentosMovPatrimonial)
                    {
                        bemPatrimonial.MaterialItemCode = dadosAgrupamentosMovPatrimonial.RelatedAssets.MaterialItemCode;
                        bemPatrimonial.MaterialGroupCode = dadosAgrupamentosMovPatrimonial.RelatedAssets.MaterialGroupCode;
                        bemPatrimonial.ValueAcquisition = dadosAgrupamentosMovPatrimonial.RelatedAssets.ValueAcquisition;
                        bemPatrimonial.ValueUpdate = dadosAgrupamentosMovPatrimonial.RelatedAssets.ValueUpdate;
                        bemPatrimonial.DepreciationAccumulated = dadosAgrupamentosMovPatrimonial.RelatedAssets.DepreciationAccumulated;
                        bemPatrimonial.ResidualValue = dadosAgrupamentosMovPatrimonial.RelatedAssets.ResidualValue;

                        dadosAgrupamentosMovPatrimonial.MovimentDate = DateTime.Now;

                        if (dadosAgrupamentosMovPatrimonial.RelatedAuxiliaryAccount.IsNull())
                            dadosAgrupamentosMovPatrimonial.RelatedAuxiliaryAccount = dicContasContabeis[dadosAgrupamentosMovPatrimonial.RelatedAuxiliaryAccount.BookAccount.GetValueOrDefault().ToString()];

                        dadosAgrupamentosMovPatrimonial.RelatedMovementType = tipoMovPatrimonial;
                        dadosAgrupamentosMovPatrimonial.MovementTypeId = tipoMovPatrimonialId;
                        dadosAgrupamentosMovPatrimonial.AssetTransferenciaId = assetTransferenciaId;
                        dadosAgrupamentosMovPatrimonial.SourceDestiny_ManagerUnitId = ugeOrigemDestinoTransferenciaOuDoacaoId;
                        dadosAgrupamentosMovPatrimonial.CPFCNPJ = cpfCnpjUgeFavorecida;
                        dadosAgrupamentosMovPatrimonial.ManagerUnitId = ugeMovimentacaoPatrimonialId;
                        dadosAgrupamentosMovPatrimonial.MovimentDate = dataMovimento;


                        bool ehEstorno = (primeiraMovimentacaoVinculadaLote.Status == false &&
                                          primeiraMovimentacaoVinculadaLote.FlagEstorno == true &&
                                          primeiraMovimentacaoVinculadaLote.DataEstorno.HasValue &&
                                          primeiraMovimentacaoVinculadaLote.LoginEstorno.HasValue);

                        bemPatrimonial.AssetMovements = new List<AssetMovements>() { dadosAgrupamentosMovPatrimonial };
                        listaObjParaGerarXML = createObjetoInterface_ContabilizaSP(bemPatrimonial, ehEstorno, tipoNotaSIAFEM).ToList();


                        if (listaObjParaGerarXML.HasElements())
                        {
                            listaMsgXml_ContabilizaSP = new List<string>();
                            foreach (var objImplSIAFNlPatrimonial in listaObjParaGerarXML)
                            {
                                var xmlParaWebservice = GeradorEstimuloSIAF.SiafemDocNLPatrimonial(objImplSIAFNlPatrimonial, ehEstorno);
                                listaMsgXml_ContabilizaSP.Add(xmlParaWebservice);
                            }

                            dadosMsgEstimulos_ContabilizaSP = Tuple.Create<string[], IList<int>>(listaMsgXml_ContabilizaSP.ToArray(), dadosAgrupamentosMovPatrimonial.RelacaoIdsMovimentacoesVinculadasPorAuditoriaIntegracao);
                        }

                        relacaoDadosMsgEstimulos_ContabilizaSP.Add(dadosMsgEstimulos_ContabilizaSP);
                        dadosMsgEstimulos_ContabilizaSP = null;
                    }
                }
            }

            return relacaoDadosMsgEstimulos_ContabilizaSP;
        }
        #endregion Metodos Auxiliares Preenchimento XML

        #region Metodos Relacionados a 'Pendencia NL SIAFEM'
        public bool RegistraNotaLancamentoPendencia(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, AuditoriaIntegracao registroAuditoriaIntegracao, string erroNotaLancamentoContabilizaSP, TipoNotaSIAF tipoAgrupamentoNotaLancamento)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            string numeroDocumentoSAM = null;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;

            int[] idsMovPatrimoniais = null;
            if (listaMovimentacoesPatrimoniais.HasElements())
            {
                numeroDocumentoSAM = listaMovimentacoesPatrimoniais.FirstOrDefault().NumberDoc;
                pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM() {
                                                                                AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id,
                                                                                DataHoraEnvioMsgWS = registroAuditoriaIntegracao.DataEnvio,
                                                                                ManagerUnitId = registroAuditoriaIntegracao.ManagerUnitId,
                                                                                TipoNotaPendencia = (short)tipoAgrupamentoNotaLancamento,
                                                                                NumeroDocumentoSAM = numeroDocumentoSAM,
                                                                                StatusPendencia = 1,
                                                                                ErroProcessamentoMsgWS = erroNotaLancamentoContabilizaSP,
                                                                              };

                
                idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray();
                using (var _contextoCamadaDados = new SAMContext())
                {
                    var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                    var _registroAuditoriaIntegracao = _contextoCamadaDados.AuditoriaIntegracoes.Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id).FirstOrDefault();

                    _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                    numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                    _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial => {
                                                                                movPatrimonial.NotaLancamentoPendenteSIAFEMId = pendenciaNLContabilizaSP.Id;
                                                                                pendenciaNLContabilizaSP.AuditoriaIntegracaoId = _registroAuditoriaIntegracao.Id;    
                                                                              });
                    pendenciaNLContabilizaSP.AssetMovementIds = _registroAuditoriaIntegracao.AssetMovementIds;

                    numeroRegistrosManipulados =+ _contextoCamadaDados.SaveChanges();
                }
            }



            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }
        internal bool InativaNotaLancamentoPendenciaVinculada(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            int numeroRegistrosManipulados = 0;
            int? notaLancamentoPendenteSIAFEMId = null;
            int? assetMovements_notaLancamentoPendenteSIAFEMId = null;
            IList<int> listaIdsNLsPendentesOutrosTiposQueFicaraoAtivas = null;
            bool gravacaoInativacaoNotaLancamentoPendente = false;
            Expression<Func<NotaLancamentoPendenteSIAFEM, bool>> expWhere = null;
            Expression<Func<NotaLancamentoPendenteSIAFEM, bool>> expWhereNLsMesmoTipo = null;
            Expression<Func<NotaLancamentoPendenteSIAFEM, bool>> expWhereNLsOutrosTipos = null;
            NotaLancamentoPendenteSIAFEM _notaLancamentoPendenteSIAFEMParaCancelamento = null;
            List<NotaLancamentoPendenteSIAFEM> listaNLsPendentesAtivasParaCancelamento = null;
            List<NotaLancamentoPendenteSIAFEM> listaNLsPendentesAtivasMesmoTipoParaCancelamento = null;
            List<NotaLancamentoPendenteSIAFEM> listaNLsPendentesAtivasOutrosTiposParaCancelamento = null;
            List<NotaLancamentoPendenteSIAFEM> listaNLsPendentesQueFicaraoAtivas = null;


            if (listaMovimentacoesPatrimoniais.HasElements())
            {
                int[] idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray(); ;
                notaLancamentoPendenteSIAFEMId = listaMovimentacoesPatrimoniais.FirstOrDefault().NotaLancamentoPendenteSIAFEMId;
                if (notaLancamentoPendenteSIAFEMId.HasValue && notaLancamentoPendenteSIAFEMId.Value > 0)
                {
                using (var _contextoCamadaDados = new SAMContext())
                {
                        listaNLsPendentesAtivasParaCancelamento = new List<NotaLancamentoPendenteSIAFEM>();
                        _notaLancamentoPendenteSIAFEMParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(notaLancamentoPendenteSIAFEM => notaLancamentoPendenteSIAFEM.Id == notaLancamentoPendenteSIAFEMId).FirstOrDefault();
                    if (_notaLancamentoPendenteSIAFEMParaCancelamento.IsNotNull())
                    {
                            //consulta padrao para trazer todas as pendencias ativas para um determinado lancamento no SIAFEM
                            expWhere = (nlPendente => nlPendente.AssetMovementIds == _notaLancamentoPendenteSIAFEMParaCancelamento.AssetMovementIds && nlPendente.StatusPendencia == 1);

                            //lista de movimentacoes que terao o campo 'NotaLancamentoPendenteSIAFEMId' atualizado
                            var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();

                            //listaNLsPendentesAtivasParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(expWhere).ToList();
                            if (tipoNotaSIAFEM != TipoNotaSIAF.Desconhecido)
                            {
                                expWhereNLsMesmoTipo = (nlPendente => nlPendente.TipoNotaPendencia == (short)tipoNotaSIAFEM); //_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
                                expWhereNLsMesmoTipo = expWhere.And(expWhereNLsMesmoTipo);

                                expWhereNLsOutrosTipos = (nlPendente => nlPendente.TipoNotaPendencia != (short)tipoNotaSIAFEM);//_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
                                expWhereNLsOutrosTipos = expWhere.And(expWhereNLsOutrosTipos);


                                
                                //obter todas as pendencias ativas de NL do tipo informado
                                listaNLsPendentesAtivasMesmoTipoParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(expWhereNLsMesmoTipo).ToList();

                                //obter todas as pendencias ativas de NL de outro tipo (se houver)
                                listaNLsPendentesAtivasOutrosTiposParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(expWhereNLsOutrosTipos).ToList();

                                //se houver pendencias ativas de NL de tipo diferente do informado manter apenas a pendencia mais recente de cada tipo
                                listaIdsNLsPendentesOutrosTiposQueFicaraoAtivas = listaNLsPendentesAtivasOutrosTiposParaCancelamento.OrderByDescending(nlPendente => nlPendente.Id)
                                                                                                                                    .GroupBy(nlPendente => nlPendente.TipoNotaPendencia)
                                                                                                                                    .Select(agrupamentoNLs => agrupamentoNLs.Max(nlPendente => nlPendente.Id))
                                                                                                                                    .ToList();

                                listaNLsPendentesQueFicaraoAtivas = listaNLsPendentesAtivasOutrosTiposParaCancelamento.Where(nlPendente => listaIdsNLsPendentesOutrosTiposQueFicaraoAtivas.Contains(nlPendente.Id)).ToList();
                                listaNLsPendentesAtivasOutrosTiposParaCancelamento = listaNLsPendentesAtivasOutrosTiposParaCancelamento.Where(nlPendente => !listaIdsNLsPendentesOutrosTiposQueFicaraoAtivas.Contains(nlPendente.Id)).ToList();

                                //obter a pendencia de outro tipo mais recente para vincular a coluna AssetMovements.NotaLancamentoPendenteSIAFEMId
                                assetMovements_notaLancamentoPendenteSIAFEMId = ((listaIdsNLsPendentesOutrosTiposQueFicaraoAtivas.LastOrDefault() == 0) ? (int?)null : listaIdsNLsPendentesOutrosTiposQueFicaraoAtivas.LastOrDefault());

                                //todas as pendencias de NL do tipo informado serao inativadas
                                listaNLsPendentesAtivasParaCancelamento.AddRange(listaNLsPendentesAtivasMesmoTipoParaCancelamento);

                                //se houver mais de uma NL de outros tipos ativa (lixo) as duplicatas serao eliminadas tambem
                                listaNLsPendentesAtivasParaCancelamento.AddRange(listaNLsPendentesAtivasOutrosTiposParaCancelamento);
                            }
                            else if (tipoNotaSIAFEM == TipoNotaSIAF.Desconhecido)
                            {
                                listaNLsPendentesAtivasParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(expWhere).ToList();
                            }

                        _notaLancamentoPendenteSIAFEMParaCancelamento.StatusPendencia = 0; //cancelamento
                            listaNLsPendentesQueFicaraoAtivas.ForEach(nlPendente => nlPendente.StatusPendencia = 1); //ativamento (forcing 'por via das duvidas')
                            listaNLsPendentesAtivasParaCancelamento.ForEach(nlPendente => nlPendente.StatusPendencia = 0); //cancelamento/desativacao
                            _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial => movPatrimonial.NotaLancamentoPendenteSIAFEMId = assetMovements_notaLancamentoPendenteSIAFEMId);
                    }

                    numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                }
            }
            }

            gravacaoInativacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoInativacaoNotaLancamentoPendente;
        }
        public bool RegistraNotaLancamentoPendenciaPorException(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, Exception runtimeException, AuditoriaIntegracao registroAuditoria, TipoNotaSIAF tipoAgrupamentoNotaLancamento)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            string numeroDocumentoSAM = null;
            string msgDetalhadaErro = null;
            int[] idsMovPatrimoniais = null;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;
            int managerUnitId = 0;




            var dadosErroException = (runtimeException.InnerException.IsNull() ? runtimeException.Message : String.Format("{0}; DetalheErro: {1}", runtimeException.Message, runtimeException.InnerException.Message));
            var movPatrimonial = listaMovimentacoesPatrimoniais.FirstOrDefault();
            if (movPatrimonial.IsNotNull())
            {
                msgDetalhadaErro = String.Format("Erro ao gerar NL Contabiliza-SP para Movimentação Patrimonial {0}; DadosException: {1}", numeroDocumentoSAM, dadosErroException);
                idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(_movPatrimonial => movPatrimonial.Id).ToArray();
                managerUnitId = movPatrimonial.ManagerUnitId;
                numeroDocumentoSAM = movPatrimonial.NumberDoc;
                pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
                                                                                 {
                                                                                    RelatedAuditoriaIntegracao = registroAuditoria,
                                                                                    DataHoraEnvioMsgWS = registroAuditoria.DataEnvio,
                                                                                    ManagerUnitId = registroAuditoria.ManagerUnitId,
                                                                                    TipoNotaPendencia = (short)tipoAgrupamentoNotaLancamento,
                                                                                    NumeroDocumentoSAM = numeroDocumentoSAM,
                                                                                    StatusPendencia = 1,
                                                                                    ErroProcessamentoMsgWS = msgDetalhadaErro,
                                                                                 };

                //Desvincula qqr. pendencia de NL vinculada ao grupo de movimentacoes passado como prametro
                this.InativaNotaLancamentoPendenciaVinculada(listaMovimentacoesPatrimoniais);

                //listaMovimentacoesPatrimoniais.RegistraAuditoriaIntegracao(registroAuditoria);                
                using (var _contextoCamadaDados = new SAMContext())
                {
                    _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                    var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(_movPatrimonial => idsMovPatrimoniais.Contains(_movPatrimonial.Id)).ToList();
                    _listaMovimentacoesPatrimoniais.ForEach(_movPatrimonial => {
                                                                                    _movPatrimonial.NotaLancamentoPendenteSIAFEMId = pendenciaNLContabilizaSP.Id;
                                                                                    _movPatrimonial.AuditoriaIntegracaoId = registroAuditoria.Id;
                                                                               });
                    numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                }
            }

            //listaMovimentacoesPatrimoniais.RegistraNotaLancamentoPendencia(registroAuditoria, )
            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }
        #endregion Metodos relacionados a 'Pendencia NL SIAFEM'

        #region Metodos Relacionados a AuditoriaIntegracao        
        internal void initDadosNotasSIAFEM()
        {
            SAMContext contextoCamadaDados = new SAMContext();
            using (contextoCamadaDados = new SAMContext())
            {
                relacaoTipoMovimentacao_TiposAgrupamentoMovimentacaoPatrimonial = new Dictionary<int, int>();
                contextoCamadaDados.MovementTypes
                                   .Where(tipoMovimentacaoPatrimonial => tipoMovimentacaoPatrimonial.Status == true).ToList()
                                   .ForEach(tipoMovimentacaoPatrimonial => {
                                                                               relacaoTipoMovimentacao_TiposAgrupamentoMovimentacaoPatrimonial.Add(tipoMovimentacaoPatrimonial.Id, tipoMovimentacaoPatrimonial.GroupMovimentId);
                                                                           });
            }
        }

        private string obterNomeTipoMovimentacaoContabilizaSP(string msgRetornoWsSIAF)
        {
            string nomeTipoMovimentacao = null;
            XmlDocument docXML = null;



            if (!String.IsNullOrWhiteSpace(msgRetornoWsSIAF) && msgRetornoWsSIAF.Contains("SiafemDocNlPatrimonial"))
            {
                docXML = new XmlDocument();
                docXML.LoadXml(msgRetornoWsSIAF);
                nomeTipoMovimentacao = docXML.GetElementsByTagName("SiafemDocNlPatrimonial")
                                             .Cast<XmlElement>()
                                             .FirstOrDefault()
                                             .GetElementsByTagName("TipoMovimento")
                                             .Cast<XmlElement>()
                                             .FirstOrDefault()
                                             .InnerText
                                             .ToUpperInvariant();
            }
            return nomeTipoMovimentacao;
        }
        private TipoNotaSIAF obterTipoMovimentacaoContabilizaSP(string msgRetornoWsSIAF)
        {
            TipoNotaSIAF @tipoNotaSIAF = TipoNotaSIAF.Desconhecido;
            XmlDocument docXML = null;
            string agrupamentoTipoMovimentoContabilizaSP = null;
            string[] tiposMovimentacaoPatrimonialNormais = null;
            string[] tipoMovimentacaoPatrimonialDepreciacao = null;
            string[] tipoMovimentacaoPatrimonialReclassificacao = null;


            if (String.IsNullOrWhiteSpace(msgRetornoWsSIAF) || msgRetornoWsSIAF.Contains("SIAFMONITORA"))
                return @tipoNotaSIAF;

            if (!String.IsNullOrWhiteSpace(msgRetornoWsSIAF))
            {
                docXML = new XmlDocument();
                docXML.LoadXml(msgRetornoWsSIAF);
                agrupamentoTipoMovimentoContabilizaSP = obterNomeTipoMovimentacaoContabilizaSP(msgRetornoWsSIAF);
                tipoMovimentacaoPatrimonialDepreciacao = new string[] { "DEPRECIACAO", "DEPRECIAÇÃO" };
                tipoMovimentacaoPatrimonialReclassificacao = new string[] { "RECLASSIFICACAO", "RECLASSIFICAÇÃO" };
                tiposMovimentacaoPatrimonialNormais = new string[] { "ENTRADA", "SAÍDA", "SAIDA" };


                if (tiposMovimentacaoPatrimonialNormais.Contains(agrupamentoTipoMovimentoContabilizaSP))
                    @tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao;
                else if (tipoMovimentacaoPatrimonialDepreciacao.Contains(agrupamentoTipoMovimentoContabilizaSP))
                    @tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao;
                else if (tipoMovimentacaoPatrimonialReclassificacao.Contains(agrupamentoTipoMovimentoContabilizaSP))
                    @tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao;
            }


            return @tipoNotaSIAF;
        }
        private string obterNomeMensagemVHI(string xmlRetornoVHI)
        {
            var docXml = new XmlDocument();
            docXml.LoadXml(xmlRetornoVHI);

            return docXml.GetElementsByTagName("cdMsg").Cast<XmlNode>().FirstOrDefault().NextSibling.Name;
        }
        private string obterNotaLancamentoContabilizaSP(string msgRetornoWsSIAF)
        {
            string[] tiposMovimentacaoPatrimonialNormais = null;
            string[] tipoMovimentacaoPatrimonialDepreciacao = null;
            string[] tipoMovimentacaoPatrimonialReclassificacao = null;
            bool ehXmlSiafemDocNlPatrimonial = false;
            bool ehMovimentacaoEntradaOuSaida = false;
            bool ehMovimentacaoDepreciacaoOuReclassificacao = false;

            string nlLiquidacao = null;
            tipoMovimentacaoPatrimonialDepreciacao = new string[] { "DEPRECIACAO", "DEPRECIAÇÃO" };
            tipoMovimentacaoPatrimonialReclassificacao = new string[] { "RECLASSIFICACAO", "RECLASSIFICAÇÃO" };
            tiposMovimentacaoPatrimonialNormais = new string[] { "ENTRADA", "SAÍDA", "SAIDA" };
            var tipoMovimentacao = obterNomeTipoMovimentacaoContabilizaSP(msgRetornoWsSIAF);
            var tipoMsgTransacaoVHI = obterNomeMensagemVHI(msgRetornoWsSIAF);
            ehXmlSiafemDocNlPatrimonial = (!String.IsNullOrWhiteSpace(tipoMsgTransacaoVHI) && (tipoMsgTransacaoVHI.ToUpperInvariant() == "SiafemDocNlPatrimonial".ToUpperInvariant()));
            ehMovimentacaoEntradaOuSaida = (!String.IsNullOrWhiteSpace(tipoMovimentacao) && tiposMovimentacaoPatrimonialNormais.Contains(tipoMovimentacao.ToUpperInvariant()));
            ehMovimentacaoDepreciacaoOuReclassificacao = (!String.IsNullOrWhiteSpace(tipoMovimentacao) && (tipoMovimentacaoPatrimonialDepreciacao.Contains(tipoMovimentacao.ToUpperInvariant()) || tipoMovimentacaoPatrimonialReclassificacao.Contains(tipoMovimentacao.ToUpperInvariant())));

            if (ehXmlSiafemDocNlPatrimonial && (ehMovimentacaoEntradaOuSaida || ehMovimentacaoDepreciacaoOuReclassificacao))
            {
                string strXmlRetornoPattern = "/*/*/Doc_Retorno/*/*/*/";
                nlLiquidacao = XmlUtil.getXmlValue(msgRetornoWsSIAF, String.Format("{0}{1}", strXmlRetornoPattern, "NumeroNL"));
            }

            nlLiquidacao = ((!String.IsNullOrWhiteSpace(nlLiquidacao) && (nlLiquidacao.ToLowerInvariant() == "null")) ? null : nlLiquidacao);
            return nlLiquidacao;
        }
        private string obterChaveTokenSIAFEM(string msgRetornoWsSIAF, bool forcaObtencaoChaveSIAFMonitora = false)
        {
            string[] tiposMovimentacaoPatrimonialNormais = null;
            string[] tipoMovimentacaoPatrimonialDepreciacao = null;
            string[] tipoMovimentacaoPatrimonialReclassificacao = null;
            bool ehXmlSiafemDocNlPatrimonial = false;
            bool ehMovimentacaoDepreciacaoOuReclassificacao = false;

            string chaveMonitoraSIAFEM = null;
            tipoMovimentacaoPatrimonialDepreciacao = new string[] { "DEPRECIACAO", "DEPRECIAÇÃO" };
            tipoMovimentacaoPatrimonialReclassificacao = new string[] { "RECLASSIFICACAO", "RECLASSIFICAÇÃO" };
            tiposMovimentacaoPatrimonialNormais = new string[] { "ENTRADA", "SAÍDA", "SAIDA" };
            var tipoMovimentacao = obterNomeTipoMovimentacaoContabilizaSP(msgRetornoWsSIAF);
            var tipoMsgTransacaoVHI = obterNomeMensagemVHI(msgRetornoWsSIAF);
            ehXmlSiafemDocNlPatrimonial = (!String.IsNullOrWhiteSpace(tipoMsgTransacaoVHI) && (tipoMsgTransacaoVHI.ToUpperInvariant() == "SiafemDocNlPatrimonial".ToUpperInvariant()));
            ehMovimentacaoDepreciacaoOuReclassificacao = (!String.IsNullOrWhiteSpace(tipoMovimentacao) && (tipoMovimentacaoPatrimonialReclassificacao.Contains(tipoMovimentacao.ToUpperInvariant()) || tipoMovimentacaoPatrimonialDepreciacao.Contains(tipoMovimentacao.ToUpperInvariant())));

            if ((ehXmlSiafemDocNlPatrimonial && ehMovimentacaoDepreciacaoOuReclassificacao) || forcaObtencaoChaveSIAFMonitora)
            {
                string strXmlRetornoPattern = "/SIAFDOC/*/";
                chaveMonitoraSIAFEM = XmlUtil.getXmlAttributeValue(msgRetornoWsSIAF, String.Format("{0}{1}", strXmlRetornoPattern, "documento"), "id");
            }

            return chaveMonitoraSIAFEM;
        }
        private string obterValorTagXmlEnvioContabilizaSP(string msgRetornoWsSIAF, string nomeTagXml)
        {
            string valorTotalMovimentacao = null;
            XmlDocument docXML = null;



            if (!String.IsNullOrWhiteSpace(msgRetornoWsSIAF) && msgRetornoWsSIAF.Contains("SiafemDocNlPatrimonial"))
            {
                docXML = new XmlDocument();
                docXML.LoadXml(msgRetornoWsSIAF);
                valorTotalMovimentacao = docXML.GetElementsByTagName("SiafemDocNlPatrimonial")
                                             .Cast<XmlElement>()
                                             .FirstOrDefault()
                                             .GetElementsByTagName(nomeTagXml)
                                             .Cast<XmlElement>()
                                             .FirstOrDefault()
                                             .InnerText
                                             .ToUpperInvariant();
            }
            return valorTotalMovimentacao;
        }
        private string ObterTokenAuditoriaSIAFEM(string msgRetornoWsSIAF)
        {
            string chaveMonitoraSIAFEM = null;
            string tokenAuditoriaIntegracao = null;

            chaveMonitoraSIAFEM = obterChaveTokenSIAFEM(msgRetornoWsSIAF, true);
            tokenAuditoriaIntegracao = ((!String.IsNullOrWhiteSpace(chaveMonitoraSIAFEM)) ? chaveMonitoraSIAFEM.Remove(0, 3).Replace("tokenSAMPatrimonio:", null) : null);
            return tokenAuditoriaIntegracao;
        }

        public void complementaAuditoriaViaXMLEnvio(string xmlEnvioSIAFEM, ref AuditoriaIntegracao entidadeAuditoriaIntegracao, bool isEstorno = false)
        {
            if (!String.IsNullOrWhiteSpace(xmlEnvioSIAFEM))
            {
                entidadeAuditoriaIntegracao.DocumentoId = obterChaveTokenSIAFEM(xmlEnvioSIAFEM, true);
                entidadeAuditoriaIntegracao.TipoMovimento = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "TipoMovimento");
                entidadeAuditoriaIntegracao.Data = DateTime.ParseExact(obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "Data"), "dd/MM/yyyy", new CultureInfo("pt-BR"));
                entidadeAuditoriaIntegracao.UgeOrigem = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "UgeOrigem");
                entidadeAuditoriaIntegracao.Gestao = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "Gestao");
                entidadeAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "Tipo_Entrada_Saida_Reclassificacao_Depreciacao");
                entidadeAuditoriaIntegracao.CpfCnpjUgeFavorecida = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "CpfCnpjUgeFavorecida");
                entidadeAuditoriaIntegracao.GestaoFavorecida = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "GestaoFavorecida");
                entidadeAuditoriaIntegracao.Item = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "Item");
                entidadeAuditoriaIntegracao.TipoEstoque = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "TipoEstoque");
                entidadeAuditoriaIntegracao.Estoque = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "Estoque");
                entidadeAuditoriaIntegracao.EstoqueDestino = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "EstoqueDestino");
                entidadeAuditoriaIntegracao.EstoqueOrigem = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "EstoqueOrigem");
                entidadeAuditoriaIntegracao.TipoMovimentacao = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "TipoMovimentacao");

                //VALOR MOVIMENTACAO
                entidadeAuditoriaIntegracao.ValorTotal = Decimal.Parse(obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "ValorTotal").Replace("R$", "").Trim());

                entidadeAuditoriaIntegracao.ControleEspecifico = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "ControleEspecifico");
                entidadeAuditoriaIntegracao.ControleEspecificoEntrada = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "ControleEspecificoEntrada");
                entidadeAuditoriaIntegracao.ControleEspecificoSaida = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "ControleEspecificoSaida");
                entidadeAuditoriaIntegracao.FonteRecurso = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "FonteRecurso");
                entidadeAuditoriaIntegracao.NLEstorno = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "NLEstorno");
                entidadeAuditoriaIntegracao.Empenho = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "Empenho");
                entidadeAuditoriaIntegracao.Observacao = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "Observacao");

                //CAMPO NOTA FISCAL
                entidadeAuditoriaIntegracao.NotaFiscal = obterValorTagXmlEnvioContabilizaSP(xmlEnvioSIAFEM, "NotaFiscal");

                //CAMPO ITEM
                entidadeAuditoriaIntegracao.ItemMaterial = entidadeAuditoriaIntegracao.Item;

                if (entidadeAuditoriaIntegracao.Id > 0)
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        contextoCamadaDados.Entry(entidadeAuditoriaIntegracao).State = EntityState.Modified;
                        contextoCamadaDados.SaveChanges();
                    }
            }
        }

        public bool GeraRegistroAuditoria(AssetMovements movimentacaoPatrimonial, string loginUsuarioSAM, string cpfUsuarioSIAF, string senhaUsuarioSIAF, string msgEstimuloWS, string msgRetornoEstimuloWS)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            bool blnRetorno = false;


            try
            {
                registroAuditoriaIntegracao = new AuditoriaIntegracao();
                registroAuditoriaIntegracao.ManagerUnitId = movimentacaoPatrimonial.ManagerUnitId;
                registroAuditoriaIntegracao.NomeSistema = "ContabilizaSP";
                registroAuditoriaIntegracao.UsuarioSAM = loginUsuarioSAM;
                registroAuditoriaIntegracao.UsuarioSistemaExterno = cpfUsuarioSIAF;
                registroAuditoriaIntegracao.MsgEstimuloWS = msgEstimuloWS;
                registroAuditoriaIntegracao.MsgRetornoWS = msgRetornoEstimuloWS;
                registroAuditoriaIntegracao.DataEnvio = DateTime.Now;


                //if (contextoCamadaDados.IsNull())
                //    contextoCamadaDados = new SAMContext();

                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    contextoCamadaDados.AuditoriaIntegracoes.Add(registroAuditoriaIntegracao);
                    contextoCamadaDados.SaveChanges();
                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";
                ErroLog rowTabelaLOG = new ErroLog() {
                                                          DataOcorrido = DateTime.Now
                                                        , Usuario = registroAuditoriaIntegracao.UsuarioSAM
                                                        , exMessage = msgErro
                                                        , exStackTrace = excErroOperacaoBancoDados.StackTrace
                                                        , exTargetSite = excErroOperacaoBancoDados.TargetSite.ToString()
                                                     };

                contextoCamadaDados.ErroLogs.Add(rowTabelaLOG);
            }

            return blnRetorno;
        }
        public AuditoriaIntegracao RegistraAuditoriaIntegracao(ProcessadorServicoSIAF svcProcessadorSIAF, string loginUsuarioSAM, string loginUsuarioSIAFEM, int managerUnitId, string tokenAuditoriaIntegracao, string relacaoIdsMovimentacaoPatrimonials = null)
        {
            AuditoriaIntegracao registroAuditoria = null;
            AuditoriaIntegracaoController objController = new AuditoriaIntegracaoController();
            Guid _tokenAuditoriaIntegracao = Guid.Empty;


            registroAuditoria                           = new AuditoriaIntegracao();
            registroAuditoria.DataEnvio                 = ((svcProcessadorSIAF.DataEnvioMsgWs != DateTime.MinValue) ? svcProcessadorSIAF.DataEnvioMsgWs : DateTime.Now);
            registroAuditoria.DataRetorno               = DateTime.Now;
            registroAuditoria.MsgEstimuloWS             = svcProcessadorSIAF.EstimuloWsSIAF;
            registroAuditoria.MsgRetornoWS              = svcProcessadorSIAF.RetornoWsSIAF;
            registroAuditoria.NomeSistema               = svcProcessadorSIAF.NomeSistemaSIAF;
            registroAuditoria.ManagerUnitId             = managerUnitId;
            registroAuditoria.UsuarioSAM                = loginUsuarioSAM;
            registroAuditoria.UsuarioSistemaExterno     = loginUsuarioSIAFEM;
            registroAuditoria.AssetMovementIds          = relacaoIdsMovimentacaoPatrimonials;
            registroAuditoria.MsgErro                   = svcProcessadorSIAF.ErroRetornoWs;

            Guid.TryParse(tokenAuditoriaIntegracao, out _tokenAuditoriaIntegracao);
            registroAuditoria.TokenAuditoriaIntegracao  = _tokenAuditoriaIntegracao;

            complementaAuditoriaViaXMLEnvio(svcProcessadorSIAF.EstimuloWsSIAF, ref registroAuditoria);
            registroAuditoria.NotaLancamento = ((!String.IsNullOrWhiteSpace(registroAuditoria.MsgRetornoWS) && (registroAuditoria.MsgRetornoWS.ToLowerInvariant() != "null")) ? obterNotaLancamentoContabilizaSP(registroAuditoria.MsgRetornoWS) : null);

            objController.InserirRegistro(registroAuditoria);
            return registroAuditoria;
        }
        public AuditoriaIntegracao RegistraAuditoriaIntegracaoSemXML(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, string loginUsuarioSAM, string loginUsuarioSIAFEM, int managerUnitId, string tokenAuditoriaIntegracao)
        {
            AuditoriaIntegracao registroAuditoria = null;
            AuditoriaIntegracaoController objController = new AuditoriaIntegracaoController();
            Guid _tokenAuditoriaIntegracao = Guid.Empty;


            registroAuditoria = new AuditoriaIntegracao();
            registroAuditoria.DataEnvio = DateTime.Now;
            registroAuditoria.DataRetorno = null;
            registroAuditoria.MsgEstimuloWS = null;
            registroAuditoria.MsgRetornoWS = null;
            registroAuditoria.NomeSistema = "SAM-Patrimonio";
            registroAuditoria.ManagerUnitId = managerUnitId;
            registroAuditoria.UsuarioSAM = loginUsuarioSAM;
            registroAuditoria.UsuarioSistemaExterno = loginUsuarioSIAFEM;
            registroAuditoria.AssetMovementIds = String.Join(" ", listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id));

            Guid.TryParse(tokenAuditoriaIntegracao, out _tokenAuditoriaIntegracao);
            registroAuditoria.TokenAuditoriaIntegracao = _tokenAuditoriaIntegracao;

            objController.InserirRegistro(registroAuditoria);

            return registroAuditoria;
        }
        public bool RegistraAuditoriaIntegracao(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            int numeroRegistrosManipulados = 0;
            bool retornoGravacaoAuditoriaIntegracao = false;



            
            int[] idsMovPatrimoniais = null;
            idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray();
            using (var _contextoCamadaDados = new SAMContext())
            {
                if (registroAuditoriaIntegracao.Id == 0)
                {
                    //_contextoCamadaDados.AuditoriaIntegracoes.Add(registroAuditoriaIntegracao);
                    _contextoCamadaDados.Entry(registroAuditoriaIntegracao).State = EntityState.Added;
                }

                var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                var _registroAuditoriaIntegracao = _contextoCamadaDados.AuditoriaIntegracoes.Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id).FirstOrDefault();

                _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial => movPatrimonial.AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id);

                if (_registroAuditoriaIntegracao.IsNotNull())
                {
                    _registroAuditoriaIntegracao.ListagemMovimentacaoPatrimonialIds = idsMovPatrimoniais;
                    _registroAuditoriaIntegracao.AssetMovementIds = _registroAuditoriaIntegracao.RelacaoMovimentacaoPatrimonialIds;
                }

                numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
            }


            retornoGravacaoAuditoriaIntegracao = (numeroRegistrosManipulados > 0);
            return retornoGravacaoAuditoriaIntegracao;
        }
        public bool VinculacaoNLContabilizaSP(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, TipoNotaSIAF tipoNotaContabilizaSP, string nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial, bool EhEstorno = false)
        {
            int numeroRegistrosManipulados = 0;
            bool retornoGravacaoAuditoriaIntegracao = false;




            int[] idsMovPatrimoniais = null;
            idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray();
            if (!String.IsNullOrWhiteSpace(nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial))
                using (var _contextoCamadaDados = new SAMContext())
                {
                    var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                    _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial => {
                                                                                if (tipoNotaContabilizaSP == TipoNotaSIAF.NL_Depreciacao)
                                                                                {
                                                                                    if (!EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoDepreciacao = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                    else if (EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoDepreciacaoEstorno = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                }

                                                                                if (tipoNotaContabilizaSP == TipoNotaSIAF.NL_Liquidacao)
                                                                                {
                                                                                    if (!EhEstorno)
                                                                                        movPatrimonial.NotaLancamento = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                    else if (EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoEstorno = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                }

                                                                                if (tipoNotaContabilizaSP == TipoNotaSIAF.NL_Reclassificacao)
                                                                                {
                                                                                    if (!EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoReclassificacao = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                    else if (EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoReclassificacaoEstorno = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                }
                                                                              });

                    numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                }


            retornoGravacaoAuditoriaIntegracao = (numeroRegistrosManipulados > 0);
            return retornoGravacaoAuditoriaIntegracao;
        }

        //TODO DBATISTA MELHORAR ISTO AQUI DEPOIS
        public bool VinculacaoNLContabilizaSP(int auditoriaIntegracaoId, TipoNotaSIAF tipoNotaContabilizaSP, string nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial, bool EhEstorno = false)
        {
            int numeroRegistrosManipulados = 0;
            bool retornoGravacaoAuditoriaIntegracao = false;




            int[] idsMovPatrimoniais = null;
            //TODO DBATISTA MELHORAR ISTO AQUI DEPOIS
            var listaMovimentacoesPatrimoniais = obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria(auditoriaIntegracaoId);
            idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray();
            if (!String.IsNullOrWhiteSpace(nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial))
                using (var _contextoCamadaDados = new SAMContext())
                {
                    var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                    _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial => {
                                                                                if (tipoNotaContabilizaSP == TipoNotaSIAF.NL_Depreciacao)
                                                                                {
                                                                                    if (!EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoDepreciacao = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                    else if (EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoDepreciacaoEstorno = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                }

                                                                                if (tipoNotaContabilizaSP == TipoNotaSIAF.NL_Liquidacao)
                                                                                {
                                                                                    if (!EhEstorno)
                                                                                        movPatrimonial.NotaLancamento = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                    else if (EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoEstorno = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                }

                                                                                if (tipoNotaContabilizaSP == TipoNotaSIAF.NL_Reclassificacao)
                                                                                {
                                                                                    if (!EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoReclassificacao = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                    else if (EhEstorno)
                                                                                        movPatrimonial.NotaLancamentoReclassificacaoEstorno = nlRetornadaContabilizaSPParaAssociacaoComMovPatrimonial;
                                                                                }
                                                                            });

                    numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                }


            retornoGravacaoAuditoriaIntegracao = (numeroRegistrosManipulados > 0);
            return retornoGravacaoAuditoriaIntegracao;
        }

        public bool AmarracaoTabelasMovimentacaoAuditoriaBP(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            int numeroRegistrosManipulados = 0;
            bool retornoGravacaoAuditoriaIntegracao = false;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao amarracaoBP_MovPatrimonial_AuditoriaIntegracao = null;


            
            int[] idsMovPatrimoniais = null;
            idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray();
            using (var contextoCamadaDados = new SAMContext())
            {
                amarracaoBP_MovPatrimonial_AuditoriaIntegracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                if (registroAuditoriaIntegracao.Id == 0)
                {
                    //contextoCamadaDados.AuditoriaIntegracoes.Add(registroAuditoriaIntegracao);
                    contextoCamadaDados.Entry(registroAuditoriaIntegracao).State = EntityState.Added;
                }

                var _listaMovimentacoesPatrimoniais = contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                var _registroAuditoriaIntegracao = contextoCamadaDados.AuditoriaIntegracoes.Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id).FirstOrDefault();

                _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial => {
                                                                            movPatrimonial.AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id;
                                                                            amarracaoBP_MovPatrimonial_AuditoriaIntegracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao() { AssetId = movPatrimonial.AssetId,
                                                                                                                                                                                              AssetMovementsId = movPatrimonial.Id,
                                                                                                                                                                                              AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id
                                                                                                                                                                                            };
                                                                            contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Add(amarracaoBP_MovPatrimonial_AuditoriaIntegracao);
                                                                            
                                                                          });

                if (_registroAuditoriaIntegracao.IsNotNull())
                {
                    _registroAuditoriaIntegracao.ListagemMovimentacaoPatrimonialIds = idsMovPatrimoniais;
                    _registroAuditoriaIntegracao.AssetMovementIds = _registroAuditoriaIntegracao.RelacaoMovimentacaoPatrimonialIds;
                }

                numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
            }


            retornoGravacaoAuditoriaIntegracao = (numeroRegistrosManipulados > 0);
            return retornoGravacaoAuditoriaIntegracao;
        }
        private IList<AssetMovements> obterRelacaoMovimentacoesPatrimoniaisPorIds(IEnumerable<int> listaIdsMovimentacoesPatrimoniais)
        {
            IList<AssetMovements> listaMovimentacoesPatrimoniais = null;

            using (var _contextoCamadaDados = new SAMContext())
                listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => listaIdsMovimentacoesPatrimoniais.Contains(movPatrimonial.Id)).AsNoTracking().ToList();


            return listaMovimentacoesPatrimoniais;
        }
        #endregion Metodos Relacionados a AuditoriaIntegracao

        #region Metodos Importados de Outras Controller
        private void GravaNLGerada(string NotaFiscal, string NLGerada)
        {
            string UGE = NotaFiscal.Substring(0, 6);
            string ContaDepreciacao = NotaFiscal.Substring(6, 4);

            string mes = NotaFiscal.Substring(10, 2);
            string ano = NotaFiscal.Substring(12, 2);

            if (ContaDepreciacao == "0000")
            {
                ContaDepreciacao = null;
            }
            else
            {
                if (ContaDepreciacao == "9999")
                    ContaDepreciacao = "99999" + ContaDepreciacao;
                else
                    ContaDepreciacao = "12381" + ContaDepreciacao;
            }

            string mesRef = "20" + ano + mes;

            string query = string.Format("UPDATE [dbo].[AccountingClosing] SET GeneratedNL = '{0}' where ManagerUnitCode = {1} AND DepreciationAccount = {2} AND ReferenceMonth = {3}",
            NLGerada, UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.contextoCamadaDados.Database.ExecuteSqlCommand(query);
        }
        private void GravaNLEstornadaGerada(string NotaFiscal, string NLGerada)
        {
            string UGE = NotaFiscal.Substring(0, 6);
            string ContaDepreciacao = NotaFiscal.Substring(6, 4);

            string mes = NotaFiscal.Substring(10, 2);
            string ano = NotaFiscal.Substring(12, 2);

            if (ContaDepreciacao == "0000")
            {
                ContaDepreciacao = null;
            }
            else
            {
                if (ContaDepreciacao == "9999")
                    ContaDepreciacao = "99999" + ContaDepreciacao;
                else
                    ContaDepreciacao = "12381" + ContaDepreciacao;
            }

            string mesRef = "20" + ano + mes;

            string query = string.Format("UPDATE [dbo].[AccountingClosingExcluidos] SET NLEstorno = '{0}' where ManagerUnitCode = {1} AND DepreciationAccount = {2} AND ReferenceMonth = {3}",
            NLGerada, UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.contextoCamadaDados.Database.ExecuteSqlCommand(query);
        }
        #endregion Metodos Importados de Outras Controller
    }
}