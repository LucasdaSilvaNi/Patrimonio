using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using LinqKit;
using Sam.Common;
using Sam.Common.Enums;
using Sam.Common.Util;
using Sam.Integracao.SIAF.Configuracao;
using Sam.Integracao.SIAF.Core;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System.Collections;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;




namespace SAM.Web.Controllers
{
    public class PendenciaNotaLancamentoSIAFEMController : BaseController
    {
        IDictionary<TipoNotaSIAF, string> listagemTiposNotaSIAFEM = null;
        IEnumerable<KeyValuePair<TipoNotaSIAF, string>> relacaoDescricaoComTipoNotasSIAFEM = null;
        IDictionary<int, string> relacaoDescricaoComTiposMovimentacaoPatrimonial = null;
        IDictionary<int, string> relacaoDescricaoComTiposAgrupamentoMovimentacaoPatrimonial = null;
        IDictionary<int, string> relacaoTipoMovimentacao_TiposAgrupamentoMovimentacaoPatrimonial = null;
        /* mini-cache --> estrutura: CODIGO ORGAO; CODIGO UO; CODIGO UGE; ANO-MES-REFERENCIA;ID UGE */
        IList<Tuple<int, int, int, string, int>> dadosEstruturaOrganizacionalOrgao = null;
        private SAMContext contextoCamadaDados = new SAMContext();
        static string[] tiposMovimentoDepreciacao = new string[] { "DEPRECIAÇÃO", "DEPRECIACAO" };
        static string[] tiposMovimentoReclassificacao = new string[] { "RECLASSIFICAÇÃO", "RECLASSIFICACAO" };
        static string[] tiposMovimentoEntradaSaida = new string[] { "SAÍDA", "SAIDA", "ENTRADA" };


        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _perfilId;

        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _perfilId = perflLogado.RelatedRelationshipUserProfile.ProfileId;
        }

        #region  Dados Login SIAFEM
        public ActionResult ReloadIndex(string LoginSIAFEM, string SenhaSIAFEM)
        {
            TempData["LoginSIAFEM"] = LoginSIAFEM;
            TempData["SenhaSIAFEM"] = SenhaSIAFEM;
            return RedirectToAction("Index");
        }
        #endregion  Dados Login SIAFEM

        private void initAuxiliarStructure()
        {
            if (!dadosEstruturaOrganizacionalOrgao.HasElements())
            {
                int codigoOrgao = -1;
                int codigoUO = -1;
                int codigoUGE = -1;
                string anoMesReferencia = null;
                Expression<Func<ManagerUnit, bool>> expWhere = null;



                expWhere = (ugeSIAFEM => ugeSIAFEM.Status == true);
                if (!PerfilAdmGeral())
                    expWhere = expWhere.And(ugeSIAFEM => ugeSIAFEM.RelatedBudgetUnit.RelatedInstitution.Id == _institutionId);

                dadosEstruturaOrganizacionalOrgao = new List<Tuple<int, int, int, string, int>>();
                var _contextoCamadaDados = new SAMContext();
                _contextoCamadaDados.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                _contextoCamadaDados.Configuration.LazyLoadingEnabled = false;
                _contextoCamadaDados.Configuration.ProxyCreationEnabled = false;
                _contextoCamadaDados.Configuration.AutoDetectChangesEnabled = false;
                var listaUGEs = _contextoCamadaDados.ManagerUnits
                                                    .AsNoTracking()
                                                    .Include("RelatedBudgetUnit.RelatedInstitution")
                                                    .Where(expWhere)
                                                    .ToList();
                foreach (var ugeSIAFEM in listaUGEs)
                {
                    Int32.TryParse(ugeSIAFEM.RelatedBudgetUnit.RelatedInstitution.Code, out codigoOrgao);
                    Int32.TryParse(ugeSIAFEM.RelatedBudgetUnit.Code, out codigoUO);
                    Int32.TryParse(ugeSIAFEM.Code, out codigoUGE);
                    anoMesReferencia = ugeSIAFEM.ManagmentUnit_YearMonthReference;

                    dadosEstruturaOrganizacionalOrgao.Add(Tuple.Create(codigoOrgao, codigoUO, codigoUGE, anoMesReferencia, ugeSIAFEM.Id));
                }
            }
        }

        #region Index
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            //User u = UserCommon.CurrentUser();
            //var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            //string mesRefUGELogado = base.ObterMesReferenciaUGE(perflLogado.ManagerUnitId);

            //this.initDadosSIAFEM();
            //int mesReferencia = -1;
            //bool habilitaIntegracaoContabilizaSP = (base.ugeIntegradaSiafem &&
            //                                        Int32.TryParse(mesRefUGELogado, out mesReferencia) &&
            //                                        (mesReferencia >= BaseController.AnoMesReferencia_InicioIntegracao));
            //ViewData["flagIntegracaoSiafem"] = (habilitaIntegracaoContabilizaSP ? 1 : 0);

            ViewData["perfilOperador"] = ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE ||
                                         (int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUO ||
                                         PerfilAdmGeral()) ? "1" : "";

            if (TempData["LoginSIAFEM"] != null)
            {
                ViewData["LoginSiafem"] = (string)TempData["LoginSIAFEM"];
                ViewData["SenhaSiafem"] = (string)TempData["SenhaSIAFEM"];
            }
            else
            {
                ViewData["LoginSiafem"] = string.Empty;
                ViewData["SenhaSiafem"] = string.Empty;
            }

            ViewBag.CurrentFilter = searchString;

            return View();
        }

        private string obterDescricaoTipoNotaSIAFEM(TipoNotaSIAF tipoNotaSIAFEM)
        {
            string descricaoTipoNotaSIAFEM = null;

            if (tipoNotaSIAFEM.GetHashCode() > 0)
            {
                if (!listagemTiposNotaSIAFEM.HasElements())
                    initDadosNotasSIAFEM();

                descricaoTipoNotaSIAFEM = listagemTiposNotaSIAFEM[tipoNotaSIAFEM];
            }

            return (descricaoTipoNotaSIAFEM ?? string.Empty);
        }



        #region Novos Metodos Listagem Registros
        private string montaPrefixoNL02(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            string prefixoNL = null;
            bool ehEstorno = false;
            string anoRerenciaRegistroAuditoriaIntegracao = null;
            string indicativoEstorno = null;


            if (registroAuditoriaIntegracao.IsNotNull())
            {
                ehEstorno = (registroAuditoriaIntegracao.NLEstorno.ToUpperInvariant() == "S");

                anoRerenciaRegistroAuditoriaIntegracao = registroAuditoriaIntegracao.Data.GetValueOrDefault().Year.ToString();
                indicativoEstorno = (ehEstorno ? " (estornado)" : null);
                prefixoNL = String.Format("<b>{0}NL{1}</b>", anoRerenciaRegistroAuditoriaIntegracao, indicativoEstorno);
            }


            return (prefixoNL ?? string.Empty);
        }
        private string obterDescricaoTipoMovimentacao02(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            string descricaoTipoMovimentacao = null;
            AssetMovements movimentacaoPatrimonial = null;


            if (registroAuditoriaIntegracao.IsNotNull())
            {
                if (pendenciaNLSiafemEhDeMovimentacaoPatrimonial02(registroAuditoriaIntegracao))
                {
                    Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracao = null;
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        registroDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                 .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracao.Id)
                                                                 .FirstOrDefault();

                        if (registroDeAmarracao.IsNotNull())
                        {
                            movimentacaoPatrimonial = contextoCamadaDados.AssetMovements
                                                                         .Include("RelatedMovementType")
                                                                         .Where(movPatrimonial => movPatrimonial.Id == registroDeAmarracao.AssetMovementsId)
                                                                         .FirstOrDefault();

                            if (movimentacaoPatrimonial.RelatedMovementType.IsNotNull())
                                descricaoTipoMovimentacao = movimentacaoPatrimonial.RelatedMovementType.Description;
                        }
                        else
                        {
                            descricaoTipoMovimentacao = registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                        }
                    }
                }
                else if (pendenciaNLSiafemEhDeFechamentoMensalIntegrado02(registroAuditoriaIntegracao))
                {
                    descricaoTipoMovimentacao = "FECHAMENTO MENSAL INTEGRADO";
                }
            }


            return (descricaoTipoMovimentacao ?? string.Empty);
        }
        private string obterValorTotalPendenciaNL__AuditoriaIntegracao02(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            string valorPendenciaNL = null;


            if (registroAuditoriaIntegracao.IsNotNull())
            {
                var cultureInfo = new CultureInfo("pt-BR");
                valorPendenciaNL = String.Format("{0:C}", registroAuditoriaIntegracao.ValorTotal.GetValueOrDefault());
            }


            return (valorPendenciaNL ?? string.Empty);
        }
        private TipoNotaSIAF obterTipoNotaSIAFEM__AuditoriaIntegracao02(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            TipoNotaSIAF tipoNotaSIAF = TipoNotaSIAF.Desconhecido;


            if (registroAuditoriaIntegracao.IsNotNull() && !String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.TipoMovimento))
            {
                var nomeTipoMovimentoContabilizaSP = registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant();
                switch (nomeTipoMovimentoContabilizaSP)
                {
                    case "ENTRADA":
                    case "SAIDA":
                    case "SAÍDA":               { tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao; } break;
                    case "DEPRECIACAO":
                    case "DEPRECIAÇÃO":         { tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao; } break;
                    case "RECLASSIFICACAO":
                    case "RECLASSIFICAÇÃO":     { tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao; } break;
                }
            }



            return tipoNotaSIAF;
        }
        private bool pendenciaNLSiafemEhDeMovimentacaoPatrimonial02(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            bool pendenciaNLEhDeMovimentacaoPatrimonial = false;
            string[] tiposMovimentoContabilizaSP = null;
            string[] estoqueDestino_FechamentoMensalIntegrado = null;



            tiposMovimentoContabilizaSP = new string[] { "ENTRADA", "SAIDA", "SAÍDA", "DEPRECIAÇÃO", "DEPRECIACAO", "RECLASSIFICAÇÃO", "RECLASSIFICACAO" };
            estoqueDestino_FechamentoMensalIntegrado = new string[] { "DESPESA DE DEPRECIAÇÃO DE BENS MÓVEIS (VARIAÇÃO)", "DESPESA DE DEPRECIACAO DE BENS MOVEIS (VARIACAO)" };

            if (registroAuditoriaIntegracao.IsNotNull())
            {
                pendenciaNLEhDeMovimentacaoPatrimonial = tiposMovimentoContabilizaSP.Contains(registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant());
                pendenciaNLEhDeMovimentacaoPatrimonial &= !estoqueDestino_FechamentoMensalIntegrado.Contains(registroAuditoriaIntegracao.EstoqueDestino);
            }


            return pendenciaNLEhDeMovimentacaoPatrimonial;
        }
        private bool pendenciaNLSiafemEhDeFechamentoMensalIntegrado02(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            bool pendenciaNLEhDeFechamentoMensalIntegrado = false;
            string[] tipoMovimentoContabilizaSP_Depreciacao = null;
            string[] estoqueDestino_FechamentoMensalIntegrado = null;



            tipoMovimentoContabilizaSP_Depreciacao = new string[] { "DEPRECIAÇÃO", "DEPRECIACAO" };
            estoqueDestino_FechamentoMensalIntegrado = new string[] { "DESPESA DE DEPRECIAÇÃO DE BENS MÓVEIS (VARIAÇÃO)", "DESPESA DE DEPRECIACAO DE BENS MOVEIS (VARIACAO)" };

            if (registroAuditoriaIntegracao.IsNotNull())
            {
                pendenciaNLEhDeFechamentoMensalIntegrado = tipoMovimentoContabilizaSP_Depreciacao.Contains(registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant());
                pendenciaNLEhDeFechamentoMensalIntegrado &= estoqueDestino_FechamentoMensalIntegrado.Contains(registroAuditoriaIntegracao.EstoqueDestino);
            }


            return pendenciaNLEhDeFechamentoMensalIntegrado;
        }
        private Func<NotaLancamentoPendenteSIAFEM, PendenciaNotaLancamentoSIAFEMViewModel> _instanciadorDTO02()
        {
            Func<NotaLancamentoPendenteSIAFEM, PendenciaNotaLancamentoSIAFEMViewModel> _actionSeletor = null;
            PendenciaNotaLancamentoSIAFEMViewModel viewModel = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoVinculado = null;

            if (!dadosEstruturaOrganizacionalOrgao.HasElements())
                initAuxiliarStructure();

            
            _actionSeletor = (notaSIAFEMPendente => 
                                                    {
                                                        registroAuditoriaIntegracaoVinculado = obterEntidadeAuditoriaIntegracao(notaSIAFEMPendente.AuditoriaIntegracaoId);

                                                        viewModel = new PendenciaNotaLancamentoSIAFEMViewModel();
                                                        viewModel.Id = notaSIAFEMPendente.Id;
                                                        viewModel.Orgao = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == notaSIAFEMPendente.ManagerUnitId)
                                                                                                                                                                                                                                            .Select(dadoParaConsulta => dadoParaConsulta.Item1.ToString())
                                                                                                                                                                                                                                            .FirstOrDefault()) : null);
                                                        viewModel.UO = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == notaSIAFEMPendente.ManagerUnitId)
                                                                                                                                                                                                                                        .Select(dadoParaConsulta => dadoParaConsulta.Item2.ToString())
                                                                                                                                                                                                                                        .FirstOrDefault()) : null);
                                                        viewModel.UGE = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == notaSIAFEMPendente.ManagerUnitId)
                                                                                                                                                                                                                                        .Select(dadoParaConsulta => dadoParaConsulta.Item3.ToString())
                                                                                                                                                                                                                                        .FirstOrDefault()) : null);

                                                        viewModel.NumeroDocumentoSAM = notaSIAFEMPendente.NumeroDocumentoSAM;
                                                        viewModel.ErroSIAFEM = notaSIAFEMPendente.ErroProcessamentoMsgWS;
                                                        viewModel.DatetimeDataEnvioSIAFEM = notaSIAFEMPendente.DataHoraEnvioMsgWS;
                                                        viewModel.DataEnvioSIAFEM = notaSIAFEMPendente.DataHoraEnvioMsgWS.ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                                        viewModel.ManagerUnitId = notaSIAFEMPendente.ManagerUnitId;

                                                        if (registroAuditoriaIntegracaoVinculado.IsNotNull())
                                                        {
                                                            viewModel.TipoAgrupamentoMovimentacaoPatrimonial = registroAuditoriaIntegracaoVinculado.TipoMovimento;
                                                            viewModel.TipoMovimentacaoPatrimonial = obterDescricaoTipoMovimentacao02(registroAuditoriaIntegracaoVinculado);
                                                            viewModel.PrefixoNL = montaPrefixoNL02(registroAuditoriaIntegracaoVinculado);
                                                            viewModel.DescricaoTipoNotaSIAFEM = obterDescricaoTipoNotaSIAFEM((@TipoNotaSIAF)notaSIAFEMPendente.TipoNotaPendencia);
                                                            viewModel.ValorMovimentacao = obterValorTotalPendenciaNL__AuditoriaIntegracao02(registroAuditoriaIntegracaoVinculado);
                                                            viewModel.DataMovimentacao = registroAuditoriaIntegracaoVinculado.Data.GetValueOrDefault().ToString(BaseController.fmtDataFormatoBrasileiro);
                                                            viewModel.ContaContabil = obtemDadosContasContabeisVinculadas_TipoMovimentacaoContabilizaSP(registroAuditoriaIntegracaoVinculado);
                                                        }

                                                        return viewModel;
                                                    });

            return _actionSeletor;
        }

        private DetalhePendenciaNotaLancamentoSIAFEMViewModel _instanciadorDTO_DetalhePendenciaNL(int notaLancamentoPendenteId)
        {
            DetalhePendenciaNotaLancamentoSIAFEMViewModel viewModel = null;
            NotaLancamentoPendenteSIAFEM registroPendenciaNLSIAFEM = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoVinculado = null;

            getHierarquiaPerfil();
            if (!dadosEstruturaOrganizacionalOrgao.HasElements())
                initAuxiliarStructure();


            if (notaLancamentoPendenteId > 0)
            {
                registroPendenciaNLSIAFEM = obterEntidadeNotaLancamentoPendenteSIAFEM(notaLancamentoPendenteId);
                registroAuditoriaIntegracaoVinculado = obterEntidadeAuditoriaIntegracao(registroPendenciaNLSIAFEM.AuditoriaIntegracaoId);

                viewModel = new DetalhePendenciaNotaLancamentoSIAFEMViewModel() { ListagemBPsVinculados = new List<DadosBemPatrimonialViewModel>() };
                viewModel.Orgao = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == registroPendenciaNLSIAFEM.ManagerUnitId)
                                                                                                                                                                                                     .Select(dadoParaConsulta => dadoParaConsulta.Item1.ToString())
                                                                                                                                                                                                     .FirstOrDefault()) : null);
                viewModel.UO = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == registroPendenciaNLSIAFEM.ManagerUnitId)
                                                                                                                                                                                                  .Select(dadoParaConsulta => dadoParaConsulta.Item2.ToString())
                                                                                                                                                                                                  .FirstOrDefault()) : null);
                viewModel.UGE = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == registroPendenciaNLSIAFEM.ManagerUnitId)
                                                                                                                                                                                                   .Select(dadoParaConsulta => dadoParaConsulta.Item3.ToString())
                                                                                                                                                                                                   .FirstOrDefault()) : null);

                viewModel.NumeroDocumentoSAM = registroPendenciaNLSIAFEM.NumeroDocumentoSAM;
                viewModel.DataHoraEnvioSIAFEM = registroPendenciaNLSIAFEM.DataHoraEnvioMsgWS.ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                viewModel.TipoMovimentoContabilizaSP = registroAuditoriaIntegracaoVinculado.TipoMovimento;
                viewModel.TipoMovimentacaoContabilizaSP = registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                viewModel.ContaContabil_TipoMovimentacaoContabilizaSP = obtemContaContabilVinculada_TipoMovimentacaoContabilizaSP(registroAuditoriaIntegracaoVinculado);
                viewModel.EstoqueOrigem = registroAuditoriaIntegracaoVinculado.EstoqueOrigem ?? String.Empty;
                viewModel.ContaContabil_EstoqueOrigem = obtemContaContabilVinculada_EstoqueOrigemContabilizaSP(registroAuditoriaIntegracaoVinculado);
                viewModel.EstoqueDestino = registroAuditoriaIntegracaoVinculado.EstoqueDestino ?? String.Empty;
                viewModel.ContaContabil_EstoqueDestino = obtemContaContabilVinculada_EstoqueDestinoContabilizaSP(registroAuditoriaIntegracaoVinculado);
                viewModel.TipoMovimentacaoSAMPatrimonio = obterDescricaoTipoMovimentacao02(registroAuditoriaIntegracaoVinculado);
                viewModel.DescricaoTipoNotaSIAFEM = obterDescricaoTipoNotaSIAFEM((@TipoNotaSIAF)registroPendenciaNLSIAFEM.TipoNotaPendencia);
                viewModel.ValorMovimentacao = obterValorTotalPendenciaNL__AuditoriaIntegracao02(registroAuditoriaIntegracaoVinculado);
                viewModel.DataMovimentacao = registroAuditoriaIntegracaoVinculado.Data.GetValueOrDefault().ToString(BaseController.fmtDataFormatoBrasileiro);
                //viewModel.ContaContabil = obtemContaContabilVinculadaAuditoriaIntegracao(registroAuditoriaIntegracaoVinculado);
                viewModel.TokenAuditoriaIntegracao = registroAuditoriaIntegracaoVinculado.TokenAuditoriaIntegracao.ToString().ToUpperInvariant();
                viewModel.ErroSIAFEM = registroPendenciaNLSIAFEM.ErroProcessamentoMsgWS;
                viewModel.CpfUsuarioSAM = registroAuditoriaIntegracaoVinculado.UsuarioSAM;
                viewModel.CpfUsuarioSIAFEM = registroAuditoriaIntegracaoVinculado.UsuarioSistemaExterno;


                using (var _contextoCamadadados = new SAMContext())
                {
                    var registrosTabelaDeAmarracao = _contextoCamadadados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                         .AsNoTracking()
                                                                         .Where(registroAmarracao => registroAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoVinculado.Id)
                                                                         .ToList();

                    var bpVinculadoIds = registrosTabelaDeAmarracao.Select(registroAmarracao => registroAmarracao.AssetId)
                                                                   .ToList();

                    var movPatrimonialId = registrosTabelaDeAmarracao.Select(registroAmarracao => registroAmarracao.AssetMovementsId)
                                                                     .FirstOrDefault();

                    if (bpVinculadoIds.HasElements())
                    {
                        var dadosBPsVinculados = obtemDadosDepreciacaoBemPatrimonialPorMesReferencia(bpVinculadoIds, movPatrimonialId);
                        if (dadosBPsVinculados.HasElements())
                            ((List<DadosBemPatrimonialViewModel>)viewModel.ListagemBPsVinculados).AddRange(dadosBPsVinculados);
                    }
                }
            }

            return viewModel;
        }

        public SortedList obtemDadosDepreciacaoBemPatrimonialPorMesReferencia(Asset bemPatrimonial, int anoMesReferencia)
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
            MonthlyDepreciation registroDadosDepreciacaoBemPatrimonial = null;
            IList<MonthlyDepreciation> registrosDadosDepreciacaoBemPatrimonial = null;
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


                        var primeiroDiaDoMes = anoMesReferencia.ToString().RetornarMesAnoReferenciaDateTime();
                        var ultimoDiaDoMes = primeiroDiaDoMes.MonthLastDay();


                        //COMPLEMENTACAO CLAUSULA 'WHERE' NO SELECT SE BP FOR DO TIPO 'DECRETO'
                        expWhere = ((bemPatrimonial.flagDecreto.HasValue && bemPatrimonial.flagDecreto == true) ? expWhere.And(dadosDepreciacaoMensal => dadosDepreciacaoMensal.Decree == true) : expWhere);
                        //expWhere = expWhere.And(_dadosDepreciacaoMensal => _dadosDepreciacaoMensal.CurrentDate >= primeiroDiaDoMes && _dadosDepreciacaoMensal.CurrentDate <= ultimoDiaDoMes);
                        expWhere = expWhere.And(_dadosDepreciacaoMensal => _dadosDepreciacaoMensal.CurrentDate <= ultimoDiaDoMes);
                        registroDadosDepreciacaoBemPatrimonial = contextoCamadaDados.MonthlyDepreciations
                                                                                    .AsNoTracking()
                                                                                    .AsExpandable()
                                                                                    .Where(expWhere)
                                                                                    .OrderByDescending(_dadosDepreciacaoMensal => _dadosDepreciacaoMensal.CurrentDate)
                                                                                    .FirstOrDefault();

                        //SE EXISTE HISTORICO DE DEPRECIACAO CALCULADO
                        if (registroDadosDepreciacaoBemPatrimonial.IsNotNull())
                        {
                            valorAtual = registroDadosDepreciacaoBemPatrimonial.CurrentValue;
                            valorDepreciacaoMensal = registroDadosDepreciacaoBemPatrimonial.RateDepreciationMonthly;
                            valorDepreciacaoAcumulada = registroDadosDepreciacaoBemPatrimonial.AccumulatedDepreciation;
                        }
                    }
                    //SE BP EH DO TIPO 'ACERVO' OU DO TIPO 'TERCEIRO'
                    else if (((bemPatrimonial.flagAcervo.HasValue && bemPatrimonial.flagAcervo == true) || (bemPatrimonial.flagTerceiro.HasValue && bemPatrimonial.flagTerceiro == true))
                    //[OU] BP NAO POSSUI HISTORICO DE DEPRECIACAO CALCULADO
                             || registroDadosDepreciacaoBemPatrimonial.IsNull())
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
        public IList<DadosBemPatrimonialViewModel> obtemDadosDepreciacaoBemPatrimonialPorMesReferencia(IList<int> assetIds, int assetMovementId)
        {
            #region Variaveis
            long assetStartId = -1;
            decimal valorAquisicao = 0.00m;
            decimal valorAtual = 0.00m;
            decimal valorDepreciacaoMensal = 0.00m;
            decimal valorDepreciacaoAcumulada = 0.00m;
            Expression<Func<MonthlyDepreciation, bool>> expWhere = null;
            MonthlyDepreciation registroDadosDepreciacaoBemPatrimonial = null;
            IList<DadosBemPatrimonialViewModel> listaDadosBPs = null;
            DadosBemPatrimonialViewModel dadosBP = null;
            Asset bemPatrimonial = null;
            #endregion Variaveis




            try
            {
                listaDadosBPs = new List<DadosBemPatrimonialViewModel>();


                if (assetIds.HasElements() && (assetMovementId > 0))
                {

                    using (var _contextoCamadaDados = new SAMContext())
                    {

                        var movimentacaoPatrimonial = _contextoCamadaDados.AssetMovements
                                                                          .AsNoTracking()
                                                                          .Where(movPatrimonial => movPatrimonial.Id == assetMovementId)
                                                                          .FirstOrDefault();
                        if (movimentacaoPatrimonial.IsNotNull())
                        {
                            var dataMovimento = movimentacaoPatrimonial.MovimentDate;
                            foreach (var assetId in assetIds)
                            {
                                bemPatrimonial = _contextoCamadaDados.Assets
                                                                     .AsNoTracking()
                                                                     .Where(_bemPatrimonial => _bemPatrimonial.Id == assetId)
                                                                     .FirstOrDefault();


                                if (bemPatrimonial.IsNotNull() &&
                                    bemPatrimonial.ManagerUnitId.IsNotNull() &&
                                    bemPatrimonial.MaterialItemCode.IsNotNull())
                                {
                                    dadosBP = new DadosBemPatrimonialViewModel();
                                    valorAquisicao = bemPatrimonial.ValueAcquisition;
                                    valorAtual = bemPatrimonial.ValueAcquisition;
                                    assetStartId = (bemPatrimonial.AssetStartId.HasValue ? bemPatrimonial.AssetStartId.Value : 0);
                                    if (assetStartId > 0)
                                    {
                                        expWhere = (dadosDepreciacaoMensal => dadosDepreciacaoMensal.AssetStartId == assetStartId
                                                                           && dadosDepreciacaoMensal.ManagerUnitId == bemPatrimonial.ManagerUnitId
                                                                           && dadosDepreciacaoMensal.MaterialItemCode == bemPatrimonial.MaterialItemCode);


                                        var ultimoDiaDoMes = dataMovimento.MonthLastDay();
                                        expWhere = expWhere.And(_dadosDepreciacaoMensal => _dadosDepreciacaoMensal.CurrentDate <= ultimoDiaDoMes);

                                        //COMPLEMENTACAO CLAUSULA 'WHERE' NO SELECT SE BP FOR DO TIPO 'DECRETO'
                                        expWhere = ((bemPatrimonial.flagDecreto.HasValue && bemPatrimonial.flagDecreto == true) ? expWhere.And(dadosDepreciacaoMensal => dadosDepreciacaoMensal.Decree == true) : expWhere);
                                        registroDadosDepreciacaoBemPatrimonial = _contextoCamadaDados.MonthlyDepreciations
                                                                                                     .AsNoTracking()
                                                                                                     .AsExpandable()
                                                                                                     .Where(expWhere)
                                                                                                     .OrderByDescending(_dadosDepreciacaoMensal => _dadosDepreciacaoMensal.CurrentDate)
                                                                                                     .FirstOrDefault();

                                        //SE EXISTE HISTORICO DE DEPRECIACAO CALCULADO
                                        if (registroDadosDepreciacaoBemPatrimonial.IsNotNull())
                                        {
                                            valorAtual = registroDadosDepreciacaoBemPatrimonial.CurrentValue;
                                            valorDepreciacaoMensal = registroDadosDepreciacaoBemPatrimonial.RateDepreciationMonthly;
                                            valorDepreciacaoAcumulada = registroDadosDepreciacaoBemPatrimonial.AccumulatedDepreciation;
                                        }
                                    }
                                    //SE BP EH DO TIPO 'ACERVO' OU DO TIPO 'TERCEIRO'
                                    else if (((bemPatrimonial.flagAcervo.HasValue && bemPatrimonial.flagAcervo == true) || (bemPatrimonial.flagTerceiro.HasValue && bemPatrimonial.flagTerceiro == true))
                                             //[OU] BP NAO POSSUI HISTORICO DE DEPRECIACAO CALCULADO
                                             || registroDadosDepreciacaoBemPatrimonial.IsNull())
                                    {
                                        valorDepreciacaoMensal = 0.00m;
                                        valorDepreciacaoAcumulada = 0.00m;
                                    }
                                }

                                dadosBP = new DadosBemPatrimonialViewModel();
                                dadosBP.Codigo = bemPatrimonial.Id;
                                dadosBP.Sigla = bemPatrimonial.InitialName;
                                dadosBP.Chapa = bemPatrimonial.NumberIdentification;
                                dadosBP.GrupoMaterial = bemPatrimonial.MaterialGroupCode.ToString("D2");
                                dadosBP.ItemMaterial = String.Format("{0} ({1})", bemPatrimonial.MaterialItemCode, bemPatrimonial.MaterialItemDescription);
                                //dadosBP.ContaContabil = (movimentacaoPatrimonial.RelatedAuxiliaryAccount.IsNotNull() ? String.Format("{0} ({1})", movimentacaoPatrimonial.RelatedAuxiliaryAccount.ContaContabilApresentacao, movimentacaoPatrimonial.RelatedAuxiliaryAccount.Description) : String.Empty);
                                dadosBP.ValorAquisicao = valorAquisicao.ToString("R$ #0.00");
                                dadosBP.ValorAtual = valorAtual.ToString("R$ #0.00");
                                dadosBP.ValorDepreciacaoAcumulada = valorDepreciacaoAcumulada.ToString("R$ #0.00");
                                dadosBP.ValorDepreciacaoMensal = valorDepreciacaoMensal.ToString("R$ #0.00");

                                listaDadosBPs.Add(dadosBP);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + "\n" + (ex.InnerException.IsNotNull() ? ex.InnerException.Message : null));
            }

            return listaDadosBPs;
        }
        private string obtemDadosContasContabeisVinculadas_TipoMovimentacaoContabilizaSP(AuditoriaIntegracao registroAuditoriaIntegracaoVinculado)
        {
            string dadosContasContabeisVinculadas = null;
            string codigoContaContabilVinculada = null;
            string codigoContaContabilDepreciacaoVinculada = null;
            int contaContabilId = 0;
            int contaContabilDepreciacaoId = 0;
            int movPatrimonialVinculadaId = 0;
            AuxiliaryAccount contaContabilVinculada = null;

            try
            {
                if (registroAuditoriaIntegracaoVinculado.IsNotNull())
                {
                    dadosContasContabeisVinculadas = String.Empty;
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        if (pendenciaNLSiafemEhDeMovimentacaoPatrimonial02(registroAuditoriaIntegracaoVinculado))
                        {
                            movPatrimonialVinculadaId = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                           .Where(registroAmarracao => registroAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoVinculado.Id)
                                                                           .Select(registroAmarracao => registroAmarracao.AssetMovementsId)
                                                                           .FirstOrDefault();

                            contaContabilId = contextoCamadaDados.AssetMovements
                                                                 .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                 .Select(movPatrimonial => movPatrimonial.AuxiliaryAccountId.Value)
                                                                 .FirstOrDefault();

                            contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                        .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                        .FirstOrDefault();

                            if (tiposMovimentoEntradaSaida.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento.ToUpperInvariant()))
                            {
                                #region Entrada/Saida
                                codigoContaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                  .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                  .Select(contaContabil => contaContabil.ContaContabilApresentacao)
                                                                                  .FirstOrDefault();

                                if (codigoContaContabilVinculada == "123110805")
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                         .FirstOrDefault();

                                    if (contaContabilId > 0)
                                    {
                                        codigoContaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                          .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                          .Select(contaContabil => contaContabil.ContaContabilApresentacao)
                                                                                          .FirstOrDefault();
                                    }

                                    dadosContasContabeisVinculadas = String.Format("123110805 P/ {0}", codigoContaContabilVinculada);
                                }
                                else
                                {
                                    dadosContasContabeisVinculadas = codigoContaContabilVinculada;
                                }
                                #endregion Entrada/Saida
                            }
                            else if (tiposMovimentoDepreciacao.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento.ToUpperInvariant()))
                            {
                                #region Depreciacao
                                if (registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao.ToUpperInvariant() == "DEPRECIAÇÃO - BAIXA".ToUpperInvariant())
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                         .FirstOrDefault();

                                    codigoContaContabilVinculada = contaContabilVinculada.ContaContabilApresentacao;
                                    if (codigoContaContabilVinculada == "123110805")
                                    {
                                        contaContabilId = contextoCamadaDados.AssetMovements
                                                                             .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                             .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                             .FirstOrDefault();

                                        contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                    .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                    .FirstOrDefault();


                                        codigoContaContabilVinculada = contaContabilVinculada.ContaContabilApresentacao;
                                        contaContabilDepreciacaoId = contaContabilVinculada.DepreciationAccountId.GetValueOrDefault();
                                        if (contaContabilVinculada.IsNotNull() && (contaContabilDepreciacaoId > 0))
                                        {
                                            var contaContabilDepreciacaoVinculada = contextoCamadaDados.DepreciationAccounts
                                                                                                       .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                                                                                       .FirstOrDefault();

                                            codigoContaContabilDepreciacaoVinculada = contaContabilDepreciacaoVinculada.Code.ToString();
                                        }

                                    }

                                    dadosContasContabeisVinculadas = String.Format("{0} P/ {1}", codigoContaContabilDepreciacaoVinculada, codigoContaContabilVinculada);
                                }
                                else if (registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao.ToUpperInvariant() == "DEPRECIAÇÃO - SALDO".ToUpperInvariant())
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.AuxiliaryAccountId.Value)
                                                                         .FirstOrDefault();

                                    contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                .FirstOrDefault();


                                    if (contaContabilId > 0)
                                    {
                                        contaContabilDepreciacaoId = contaContabilVinculada.DepreciationAccountId.GetValueOrDefault();
                                        if (contaContabilVinculada.IsNotNull() && (contaContabilDepreciacaoId > 0))
                                        {
                                            var contaContabilDepreciacaoVinculada = contextoCamadaDados.DepreciationAccounts
                                                                                                       .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                                                                                       .FirstOrDefault();

                                            codigoContaContabilDepreciacaoVinculada = contaContabilDepreciacaoVinculada.Code.ToString();
                                        }
                                    }

                                    dadosContasContabeisVinculadas = codigoContaContabilDepreciacaoVinculada;
                                }
                                #endregion Depreciacao
                            }
                            else if (tiposMovimentoReclassificacao.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento.ToUpperInvariant()))
                            {
                                if (contaContabilId > 0)
                                {
                                    contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                .FirstOrDefault();

                                    codigoContaContabilVinculada = contaContabilVinculada.ContaContabilApresentacao;
                                    if (codigoContaContabilVinculada == "123110805")
                                    {
                                        contaContabilId = contextoCamadaDados.AssetMovements
                                                                             .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                             .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                             .FirstOrDefault();

                                        contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                    .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                    .FirstOrDefault();


                                        codigoContaContabilVinculada = contaContabilVinculada.ContaContabilApresentacao;
                                        //contaContabilDepreciacaoId = contaContabilVinculada.DepreciationAccountId.GetValueOrDefault();
                                        //if (contaContabilVinculada.IsNotNull() && (contaContabilDepreciacaoId > 0))
                                        //{
                                        //    var contaContabilDepreciacaoVinculada = contextoCamadaDados.DepreciationAccounts
                                        //                                                               .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                        //                                                               .FirstOrDefault();

                                        //    codigoContaContabilDepreciacaoVinculada = contaContabilDepreciacaoVinculada.Code.ToString();
                                        //}
                                    }
                                }

                                #region Reclassificacao
                                if (registroAuditoriaIntegracaoVinculado.EstoqueOrigem.ToUpperInvariant() == "INSERVÍVEL NA UGE - BENS MÓVEIS".ToUpperInvariant())
                                {
                                    dadosContasContabeisVinculadas = String.Format("123110805 P/ {0}", codigoContaContabilVinculada);
                                }
                                else if (registroAuditoriaIntegracaoVinculado.EstoqueDestino.ToUpperInvariant() == "INSERVÍVEL NA UGE - BENS MÓVEIS".ToUpperInvariant())
                                {
                                    dadosContasContabeisVinculadas = String.Format("{0} P/ 123110805", codigoContaContabilVinculada);
                                }
                                #endregion Reclassificacao
                            }
                        }
                        else if (pendenciaNLSiafemEhDeFechamentoMensalIntegrado02(registroAuditoriaIntegracaoVinculado))
                        {
                            dadosContasContabeisVinculadas = obtemNumeroContaContabilDepreciacao(registroAuditoriaIntegracaoVinculado.NotaFiscal);
                        }
                    }
                }
            }
            catch (Exception excRuntime)
            {
                base.GravaLogErro(excRuntime);
            }

            return dadosContasContabeisVinculadas;
        }
        private string obtemContaContabilVinculada_TipoMovimentacaoContabilizaSP(AuditoriaIntegracao registroAuditoriaIntegracaoVinculado)
        {
            string codigoContaContabilVinculada = null;
            int contaContabilId = 0;

            try
            {
                if (registroAuditoriaIntegracaoVinculado.IsNotNull())
                {
                    codigoContaContabilVinculada = String.Empty;
                    if (tiposMovimentoEntradaSaida.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento.ToUpperInvariant()))
                    {
                        using (var contextoCamadaDados = new SAMContext())
                        {
                            if (pendenciaNLSiafemEhDeMovimentacaoPatrimonial02(registroAuditoriaIntegracaoVinculado))
                            {
                                var movPatrimonialVinculadaId = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                       .Where(registroAmarracao => registroAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoVinculado.Id)
                                                                       .Select(registroAmarracao => registroAmarracao.AssetMovementsId)
                                                                       .FirstOrDefault();

                                contaContabilId = contextoCamadaDados.AssetMovements
                                                                     .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                     .Select(movPatrimonial => movPatrimonial.AuxiliaryAccountId.Value)
                                                                     .FirstOrDefault();

                                codigoContaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                  .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                  .Select(contaContabil => contaContabil.ContaContabilApresentacao)
                                                                                  .FirstOrDefault();

                                if (codigoContaContabilVinculada == "123110805")
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                         .FirstOrDefault();

                                    if (contaContabilId > 0)
                                    {
                                        codigoContaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                          .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                          .Select(contaContabil => contaContabil.ContaContabilApresentacao)
                                                                                          .FirstOrDefault();
                                    }
                                }
                            }
                            else if (pendenciaNLSiafemEhDeFechamentoMensalIntegrado02(registroAuditoriaIntegracaoVinculado))
                            {
                                codigoContaContabilVinculada = obtemNumeroContaContabilDepreciacao(registroAuditoriaIntegracaoVinculado.NotaFiscal);
                            }
                        }
                    }
                }
            }
            catch (Exception excRuntime)
            {
                base.GravaLogErro(excRuntime);
            }

            return codigoContaContabilVinculada;
        }
        private string obtemContaContabilVinculada_EstoqueOrigemContabilizaSP(AuditoriaIntegracao registroAuditoriaIntegracaoVinculado)
        {
            string codigoContaContabilVinculada = null;
            int contaContabilId = 0;
            int contaContabilDepreciacaoId = 0;
            int movPatrimonialVinculadaId = 0;
            AuxiliaryAccount contaContabilVinculada = null;



            try
            {
                if (registroAuditoriaIntegracaoVinculado.IsNotNull())
                {
                    if (tiposMovimentoEntradaSaida.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento))
                    {
                        codigoContaContabilVinculada = String.Empty;
                    }
                    else if (tiposMovimentoReclassificacao.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento))
                    {
                        if (registroAuditoriaIntegracaoVinculado.EstoqueOrigem.ToUpperInvariant() == "INSERVÍVEL NA UGE - BENS MÓVEIS".ToUpperInvariant())
                        {
                            codigoContaContabilVinculada = "123110805";
                        }
                        else
                        {
                            using (var contextoCamadaDados = new SAMContext())
                            {
                                movPatrimonialVinculadaId = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                               .Where(registroAmarracao => registroAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoVinculado.Id)
                                                                               .Select(registroAmarracao => registroAmarracao.AssetMovementsId)
                                                                               .FirstOrDefault();

                                contaContabilId = contextoCamadaDados.AssetMovements
                                                                     .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                     .Select(movPatrimonial => movPatrimonial.AuxiliaryAccountId.Value)
                                                                     .FirstOrDefault();

                                codigoContaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                  .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                  .Select(contaContabil => contaContabil.ContaContabilApresentacao)
                                                                                  .FirstOrDefault();
                                if (codigoContaContabilVinculada == "123110805")
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                         .FirstOrDefault();

                                    if (contaContabilId > 0)
                                    {
                                        codigoContaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                          .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                          .Select(contaContabil => contaContabil.ContaContabilApresentacao)
                                                                                          .FirstOrDefault();
                                    }
                                }
                            }
                        }
                    }
                    else if (tiposMovimentoDepreciacao.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento))
                    {
                        using (var contextoCamadaDados = new SAMContext())
                        {

                            if (pendenciaNLSiafemEhDeMovimentacaoPatrimonial02(registroAuditoriaIntegracaoVinculado))
                            {
                                movPatrimonialVinculadaId = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                               .Where(registroAmarracao => registroAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoVinculado.Id)
                                                                               .Select(registroAmarracao => registroAmarracao.AssetMovementsId)
                                                                               .FirstOrDefault();


                                if (registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao.ToUpperInvariant() == "DEPRECIAÇÃO - BAIXA".ToUpperInvariant())
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                         .FirstOrDefault();

                                    contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                .FirstOrDefault();


                                    if (contaContabilId > 0)
                                    {
                                        contaContabilDepreciacaoId = contaContabilVinculada.DepreciationAccountId.GetValueOrDefault();
                                        if (contaContabilVinculada.IsNotNull() && (contaContabilDepreciacaoId > 0))
                                        {
                                            var contaContabilDepreciacaoVinculada = contextoCamadaDados.DepreciationAccounts
                                                                                                       .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                                                                                       .FirstOrDefault();

                                            codigoContaContabilVinculada = contaContabilDepreciacaoVinculada.Code.ToString();
                                        }
                                    }

                                }
                                else if (registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao.ToUpperInvariant() == "DEPRECIAÇÃO - SALDO".ToUpperInvariant())
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.AuxiliaryAccountId.Value)
                                                                         .FirstOrDefault();

                                    contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                .FirstOrDefault();


                                    if (contaContabilId > 0)
                                    {
                                        contaContabilDepreciacaoId = contaContabilVinculada.DepreciationAccountId.GetValueOrDefault();
                                        if (contaContabilVinculada.IsNotNull() && (contaContabilDepreciacaoId > 0))
                                        {
                                            var contaContabilDepreciacaoVinculada = contextoCamadaDados.DepreciationAccounts
                                                                                                       .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                                                                                       .FirstOrDefault();

                                            codigoContaContabilVinculada = contaContabilDepreciacaoVinculada.Code.ToString();
                                        }
                                    }
                                }
                            }
                            else if (pendenciaNLSiafemEhDeFechamentoMensalIntegrado02(registroAuditoriaIntegracaoVinculado))
                            {
                                codigoContaContabilVinculada = obtemNumeroContaContabilDepreciacao(registroAuditoriaIntegracaoVinculado.NotaFiscal);
                            }
                        }
                    }
                }
            }
            catch (Exception excRuntime)
            {
                base.GravaLogErro(excRuntime);
            }

            return codigoContaContabilVinculada;
        }
        private string obtemContaContabilVinculada_EstoqueDestinoContabilizaSP(AuditoriaIntegracao registroAuditoriaIntegracaoVinculado)
        {
            string codigoContaContabilVinculada = null;
            int contaContabilId = 0;
            int contaContabilDepreciacaoId = 0;
            AuxiliaryAccount contaContabilVinculada = null;


            try
            {
                if (registroAuditoriaIntegracaoVinculado.IsNotNull())
                {
                    if (registroAuditoriaIntegracaoVinculado.EstoqueDestino.ToUpperInvariant() == "INSERVÍVEL NA UGE - BENS MÓVEIS".ToUpperInvariant())
                    {
                        codigoContaContabilVinculada = "123110805";
                    }
                    else
                    {
                        using (var contextoCamadaDados = new SAMContext())
                        {
                            if (pendenciaNLSiafemEhDeMovimentacaoPatrimonial02(registroAuditoriaIntegracaoVinculado))
                            {
                                var movPatrimonialVinculadaId = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                   .Where(registroAmarracao => registroAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoVinculado.Id)
                                                                                   .Select(registroAmarracao => registroAmarracao.AssetMovementsId)
                                                                                   .FirstOrDefault();

                                contaContabilId = contextoCamadaDados.AssetMovements
                                                                     .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                     .Select(movPatrimonial => (movPatrimonial.AuxiliaryAccountId.HasValue ? movPatrimonial.AuxiliaryAccountId.Value : 0))
                                                                     .FirstOrDefault();

                                if (tiposMovimentoDepreciacao.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento))
                                {
                                    if (registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao.ToUpperInvariant() == "DEPRECIAÇÃO - BAIXA".ToUpperInvariant())
                                    {
                                        contaContabilId = contextoCamadaDados.AssetMovements
                                                                             .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                             .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                             .FirstOrDefault();

                                        contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                    .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                    .FirstOrDefault();


                                        codigoContaContabilVinculada = contaContabilVinculada.ContaContabilApresentacao;
                                    }
                                    else if (registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao.ToUpperInvariant() == "DEPRECIAÇÃO - SALDO".ToUpperInvariant())
                                    {
                                        contaContabilId = contextoCamadaDados.AssetMovements
                                                                             .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                             .Select(movPatrimonial => movPatrimonial.AuxiliaryAccountId.Value)
                                                                             .FirstOrDefault();

                                        contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                    .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                    .FirstOrDefault();


                                        if (contaContabilId > 0)
                                            contaContabilDepreciacaoId = contaContabilVinculada.DepreciationAccountId.GetValueOrDefault();

                                    }

                                    if (contaContabilVinculada.IsNotNull() && (contaContabilDepreciacaoId > 0))
                                    {
                                        var contaContabilDepreciacaoVinculada = contextoCamadaDados.DepreciationAccounts
                                                                                                   .Where(contaContabilDepreciacao => contaContabilDepreciacao.Id == contaContabilDepreciacaoId)
                                                                                                   .FirstOrDefault();

                                        codigoContaContabilVinculada = contaContabilDepreciacaoVinculada.Code.ToString();
                                    }
                                }
                                else if (tiposMovimentoReclassificacao.Contains(registroAuditoriaIntegracaoVinculado.TipoMovimento))
                                {
                                    contaContabilId = contextoCamadaDados.AssetMovements
                                                                         .Where(movPatrimonial => movPatrimonial.Id == movPatrimonialVinculadaId)
                                                                         .Select(movPatrimonial => movPatrimonial.ContaContabilAntesDeVirarInservivel.Value)
                                                                         .FirstOrDefault();

                                    contaContabilVinculada = contextoCamadaDados.AuxiliaryAccounts
                                                                                .Where(contaContabil => contaContabil.Id == contaContabilId)
                                                                                .FirstOrDefault();

                                    codigoContaContabilVinculada = contaContabilVinculada.ContaContabilApresentacao;
                                }
                            }
                            else if (pendenciaNLSiafemEhDeFechamentoMensalIntegrado02(registroAuditoriaIntegracaoVinculado))
                            {
                                codigoContaContabilVinculada = String.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception excRuntime)
            {
                base.GravaLogErro(excRuntime);
            }

            return codigoContaContabilVinculada;
        }

        private string obtemNumeroContaContabilDepreciacao(string notaFiscal)
        {
            string contaContabilDepreciacao = null;

            if (!String.IsNullOrWhiteSpace(notaFiscal) && (notaFiscal.Length >= 10))
            {
                contaContabilDepreciacao = notaFiscal.Substring(6, 4);
                if (contaContabilDepreciacao == "0000")
                {
                    contaContabilDepreciacao = null;
                }
                else
                {
                    if (contaContabilDepreciacao == "9999")
                        contaContabilDepreciacao = "99999" + contaContabilDepreciacao;
                    else
                        contaContabilDepreciacao = "12381" + contaContabilDepreciacao;
                }
            }


            return contaContabilDepreciacao;
        }
        #endregion Novos Metodos Listagem Registros

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult()
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();

            IEnumerable<PendenciaNotaLancamentoSIAFEMViewModel> listagemNotasSiafemPendentes = null;
            Expression<Func<NotaLancamentoPendenteSIAFEM, bool>> expWhere = null;
            Expression<Func<NotaLancamentoPendenteSIAFEM, bool>> expWhereAuxiliar = null;
            IQueryable<NotaLancamentoPendenteSIAFEM> qryConsulta;
            int totalRegistros = 0;



            try
            {
                getHierarquiaPerfil();
                initAuxiliarStructure();
                initDadosNotasSIAFEM();
                InitDisconnectContext(ref contextoCamadaDados);



                expWhere = (filtroConsulta => filtroConsulta.StatusPendencia == 1); //Pendencias NL 'ativas'
                switch (_perfilId.GetValueOrDefault())
                {
                    case (int)EnumProfile.AdministradordeOrgao: { expWhereAuxiliar = (filtroConsulta => filtroConsulta.RelatedManagerUnit
                                                                                                                      .RelatedBudgetUnit
                                                                                                                      .RelatedInstitution.Id == _institutionId); } break;
                    case (int)EnumProfile.AdministradordeUO:
                    case (int)EnumProfile.OperadordeUO:         { expWhereAuxiliar = (filtroConsulta => filtroConsulta.RelatedManagerUnit
                                                                                                                      .RelatedBudgetUnit.Id == _budgetUnitId); } break;

                    case (int)EnumProfile.OperadordeUGE:        { expWhereAuxiliar = (filtroConsulta => filtroConsulta.ManagerUnitId == _managerUnitId); } break;
                }


                expWhere = (expWhereAuxiliar.IsNotNull() ? expWhere.And(expWhereAuxiliar) : expWhere);
                qryConsulta = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                 .Include("RelatedAuditoriaIntegracao")
                                                 .AsQueryable();
                qryConsulta = qryConsulta.Where(expWhere);
                listagemNotasSiafemPendentes = qryConsulta.AsNoTracking().OrderByDescending(s => s.DataHoraEnvioMsgWS).Select(_instanciadorDTO02());

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    listagemNotasSiafemPendentes = Pesquisa(listagemNotasSiafemPendentes.AsQueryable(), currentFilter);

                totalRegistros = listagemNotasSiafemPendentes.Count();
                listagemNotasSiafemPendentes = listagemNotasSiafemPendentes.AsQueryable().Skip(startRec).Take(length).ToList();



                string descricaoTipoMovimentacao = null;
                string descricaoAgrupamentoTipoMovimentacao = null;
                string notaExcecao = null;
                foreach (PendenciaNotaLancamentoSIAFEMViewModel notaContabilizaSPPendente in listagemNotasSiafemPendentes)
                {
                    //if (String.IsNullOrWhiteSpace(notaContabilizaSPPendente.AssetMovementIds))
                    //{
                        descricaoTipoMovimentacao = "\"TIPO INVALIDO\"";
                        descricaoAgrupamentoTipoMovimentacao = "\"AGRUPAMENTO INVALIDO\"";
                        notaExcecao = "\"PENDENCIA EM ESTADO INCONSISTENTE\"";

                        notaContabilizaSPPendente.ErroSIAFEM = ((String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial) && String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoMovimentacaoPatrimonial)) ? notaExcecao : notaContabilizaSPPendente.ErroSIAFEM);
                        notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial = ((!String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial)) ? notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial : descricaoAgrupamentoTipoMovimentacao);
                        notaContabilizaSPPendente.TipoMovimentacaoPatrimonial = ((!String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoMovimentacaoPatrimonial)) ? notaContabilizaSPPendente.TipoMovimentacaoPatrimonial : descricaoTipoMovimentacao);
                    //}

                    descricaoTipoMovimentacao = null;
                    descricaoAgrupamentoTipoMovimentacao = null;
                }

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = listagemNotasSiafemPendentes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Index

        #region Reenvio Automatico

        private static IEnumerable<KeyValuePair<TipoNotaSIAF, string>> obterDescricaoTiposNotasSIAFEM()
        {
            return Enum.GetValues(typeof(TipoNotaSIAF))
                       .Cast<TipoNotaSIAF>()
                       .ToList()
                       .Select(dadosEnumValue => new KeyValuePair<TipoNotaSIAF, string>(dadosEnumValue, EnumGeral.GetEnumDescription(dadosEnumValue)));
        }
        private void initDadosNotasSIAFEM()
        {
            if (!listagemTiposNotaSIAFEM.HasElements())
            {
                this.listagemTiposNotaSIAFEM = new Dictionary<TipoNotaSIAF, string>();
                this.relacaoDescricaoComTiposAgrupamentoMovimentacaoPatrimonial = new Dictionary<int, string>();
                this.relacaoDescricaoComTipoNotasSIAFEM = obterDescricaoTiposNotasSIAFEM();
                relacaoDescricaoComTipoNotasSIAFEM.ToList().ForEach(dadosEnumValue => listagemTiposNotaSIAFEM.Add(dadosEnumValue));
            }

            if (!relacaoDescricaoComTiposMovimentacaoPatrimonial.HasElements())
            {
                using (contextoCamadaDados = new SAMContext())
                {
                    relacaoDescricaoComTiposMovimentacaoPatrimonial = new Dictionary<int, string>();
                    relacaoTipoMovimentacao_TiposAgrupamentoMovimentacaoPatrimonial = new Dictionary<int, string>();
                    contextoCamadaDados.MovementTypes
                                       .Where(tipoMovimentacaoPatrimonial => tipoMovimentacaoPatrimonial.Status == true).ToList()
                                       .ForEach(tipoMovimentacaoPatrimonial => {
                                                                                    relacaoDescricaoComTiposMovimentacaoPatrimonial.Add(tipoMovimentacaoPatrimonial.Id, tipoMovimentacaoPatrimonial.Description);
                                                                                    relacaoTipoMovimentacao_TiposAgrupamentoMovimentacaoPatrimonial.Add(tipoMovimentacaoPatrimonial.Id, tipoMovimentacaoPatrimonial.RelatedGroupMoviment.Description);
                                                                               });

                    
                    contextoCamadaDados.GroupMoviments
                                       .ToList()
                                       .ForEach(agrupamentoTipoMovimentacaoPatrimonial => relacaoDescricaoComTiposAgrupamentoMovimentacaoPatrimonial.Add(agrupamentoTipoMovimentacaoPatrimonial.Id, agrupamentoTipoMovimentacaoPatrimonial.Description.ToUpperInvariant()));
                }
            }
        }

        [HttpGet]
        public ActionResult PreencheDetalhesNotaLancamentoPendente(int notaLancamentoPendenteSIAFEMCodigo)
        {
            DetalhePendenciaNotaLancamentoSIAFEMViewModel viewModel = null;



            try
            {
                if (notaLancamentoPendenteSIAFEMCodigo > 0)
                        viewModel = _instanciadorDTO_DetalhePendenciaNL(notaLancamentoPendenteSIAFEMCodigo);



                return PartialView("_detalhePendenciaNotaLancamentoSIAFEM", viewModel);
            }
            catch (Exception excErroRuntime)
            {
                return MensagemErro("Erro ao executar função 'PreencheDetalhesNotaLancamentoPendente'");
            }

        }
        
        private string gerarXmlAuditoriaAlteracaoManualMovimentacaoPatrimonial(string loginUsuarioSAM, IList<AssetMovements> listaMovimentacoesPatrimoniais, string numeroNL, string tipoLancamento, TipoNotaSIAF @tipoNotaSIAF)
        {
            string strRetorno = null;
            string nomeCampoTipoNL = null;
            string sufixoTipoLancamento = null;
            XElement xml = null;
            XElement xmlPENDENCIA_NOTA_LANCAMENTO_MOVIMENTACAO_PATRIMONIAL = null;
            XElement xmlMOVIMENTACAO_PATRIMONIALs = null;
            XElement xmlMOVIMENTACAO_PATRIMONIAL = null;
            XElement xmlBEM_PATRIMONIAL = null;





            sufixoTipoLancamento = ((tipoLancamento.ToUpperInvariant() == "E") ? "_Estorno" : null);
            nomeCampoTipoNL = String.Format("{0}{1}", nomeCampoTipoNL, sufixoTipoLancamento);
            switch (@tipoNotaSIAF)
            {
                case TipoNotaSIAF.NL_Liquidacao: nomeCampoTipoNL = "NotaLancamento_Liquidacao"; break;
                case TipoNotaSIAF.NL_Reclassificacao: nomeCampoTipoNL = "NotaLancamento_Reclassificacao"; break;
                case TipoNotaSIAF.NL_Depreciacao: nomeCampoTipoNL = "NotaLancamento_Depreciacao"; break;
            }



            xmlMOVIMENTACAO_PATRIMONIALs = new XElement("AssetMovements");
            foreach (var movimentacaoPatrimonial in listaMovimentacoesPatrimoniais)
            {
                xmlBEM_PATRIMONIAL = new XElement("AssetId", movimentacaoPatrimonial.AssetId);
                xmlMOVIMENTACAO_PATRIMONIAL = new XElement("AssetMovement", new XAttribute("Id", movimentacaoPatrimonial.Id)
                                                                          , new XAttribute(nomeCampoTipoNL, numeroNL)
                                                                          , xmlBEM_PATRIMONIAL);


                xmlMOVIMENTACAO_PATRIMONIALs.Add(xmlMOVIMENTACAO_PATRIMONIAL);
                xmlMOVIMENTACAO_PATRIMONIAL = null;
                xmlBEM_PATRIMONIAL = null;
            }

            xmlPENDENCIA_NOTA_LANCAMENTO_MOVIMENTACAO_PATRIMONIAL = new XElement("NotaLancamentoPendenteSIAFEM", xmlMOVIMENTACAO_PATRIMONIALs);
            xml = new XElement("AuditoriaIntegracao"
                                                    , new XElement("AlteracaoRegistro_TelaPendenciaSIAFEM"
                                                                                                            , new XElement("AlteracaoRegistros"
                                                                                                                                                , new XAttribute("DataAlteracao", DateTime.Now.ToString(BaseController.fmtDataHoraFormatoBrasileiro))
                                                                                                                                                , new XAttribute("AlteradoPor", loginUsuarioSAM)
                                                                                                                                                , xmlMOVIMENTACAO_PATRIMONIALs)));
            var xmlDoc = XElement.Parse(xml.ToString());
            strRetorno = xmlDoc.ToString();

            return strRetorno;
        }
        private string gerarXmlAuditoriaAlteracaoManualFechamentoMensalIntegrado(string loginUsuarioSAM, long rowTabelaId, string contaContabilDepreciacao, string anoMesReferencia, string numeroNL, string tipoLancamento, TipoNotaSIAF @tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao)
        {
            string strRetorno = null;
            string nomeCampoTipoNL = null;
            string sufixoTipoLancamento = null;
            XElement xml = null;
            XElement xmlFECHAMENTO_MENSAL_CONTA_CONTABIL_DEPRECIACAO = null;
            XElement xmlPENDENCIA_NOTA_LANCAMENTO_FECHAMENTO_MENSAL = null;
            XElement xmlFECHAMENTO_MENSAL_CONTA_CONTABIL_DEPRECIACAO_ID = null;
            XElement xmlCONTA_CONTABIL_CODIGO = null;
            XElement xmlANO_MES_REFERENCIA = null;
            XElement xmlNOTA_LANCAMENTO_SIAFEM = null;




            if ((rowTabelaId > 0) &&
                !String.IsNullOrWhiteSpace(contaContabilDepreciacao) &&
                !String.IsNullOrWhiteSpace(anoMesReferencia) &&
                !String.IsNullOrWhiteSpace(numeroNL) &&
                !String.IsNullOrWhiteSpace(tipoLancamento))
            {

                sufixoTipoLancamento = ((tipoLancamento.ToUpperInvariant() == "E") ? "_Estorno" : null);
                nomeCampoTipoNL = String.Format("{0}{1}", nomeCampoTipoNL, sufixoTipoLancamento);
                switch (@tipoNotaSIAF)
                {
                    case TipoNotaSIAF.NL_Liquidacao: nomeCampoTipoNL = "NotaLancamento_Liquidacao"; break;
                    case TipoNotaSIAF.NL_Reclassificacao: nomeCampoTipoNL = "NotaLancamento_Reclassificacao"; break;
                    case TipoNotaSIAF.NL_Depreciacao: nomeCampoTipoNL = "NotaLancamento_Depreciacao"; break;
                }



                xmlFECHAMENTO_MENSAL_CONTA_CONTABIL_DEPRECIACAO_ID = new XElement("Id", rowTabelaId);
                xmlCONTA_CONTABIL_CODIGO = new XElement("ContaContabilDepreciacao", contaContabilDepreciacao);
                xmlANO_MES_REFERENCIA = new XElement("AnoMesReferencia", anoMesReferencia);
                xmlNOTA_LANCAMENTO_SIAFEM = new XElement(nomeCampoTipoNL, numeroNL);

                xmlFECHAMENTO_MENSAL_CONTA_CONTABIL_DEPRECIACAO = new XElement("AccountingClosing"
                                                                                                  , xmlFECHAMENTO_MENSAL_CONTA_CONTABIL_DEPRECIACAO_ID
                                                                                                  , xmlCONTA_CONTABIL_CODIGO
                                                                                                  , xmlANO_MES_REFERENCIA
                                                                                                  , xmlNOTA_LANCAMENTO_SIAFEM);

                xmlPENDENCIA_NOTA_LANCAMENTO_FECHAMENTO_MENSAL = new XElement("NotaLancamentoPendenteSIAFEM", xmlFECHAMENTO_MENSAL_CONTA_CONTABIL_DEPRECIACAO);
                xml = new XElement("AuditoriaIntegracao"
                                                        , new XElement("AlteracaoRegistro_TelaPendenciaSIAFEM"
                                                                                                                , new XElement("AlteracaoRegistros"
                                                                                                                                                    , new XAttribute("DataAlteracao", DateTime.Now.ToString(BaseController.fmtDataHoraFormatoBrasileiro))
                                                                                                                                                    , new XAttribute("AlteradoPor", loginUsuarioSAM)
                                                                                                                                                    , xmlFECHAMENTO_MENSAL_CONTA_CONTABIL_DEPRECIACAO)));
            }

            var xmlDoc = XElement.Parse(xml.ToString());
            strRetorno = xmlDoc.ToString();

            return strRetorno;
        }

        public JsonResult InclusaoManualNL(int Codigo, string ComplNL)
        {
            bool atualizacaoEfetuadaComSucesso = false;
            string mensagem = "";
            string finalNumeroNLManual = null;
            int notaLancamentoPendenteId = 0;
            Regex regexValidacao = null;




            getHierarquiaPerfil();
            ComplNL = ComplNL.PadLeft(5, '0');
            regexValidacao = new Regex("[0-9A-Z]*$");
            finalNumeroNLManual = ComplNL.Substring(0, 5);
            if (!regexValidacao.IsMatch(ComplNL))
            {
                mensagem = "Dado informado no campo NL deve ser alfanumérico, com números e/ou letras maiúsculas com cinco dígitos!";
                return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = mensagem }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                notaLancamentoPendenteId = Codigo;
                if (notaLancamentoPendenteId > 0)
                {
                    if (pendenciaNLSiafemEhDeMovimentacaoPatrimonial(notaLancamentoPendenteId))
                        return inclusaoManualNLMovimentacaoPatrimonial(notaLancamentoPendenteId, finalNumeroNLManual);
                    else if (pendenciaNLSiafemEhDeFechamentoMensalIntegrado(notaLancamentoPendenteId))
                        return inclusaoManualNLFechamentoMensalIntegrado(notaLancamentoPendenteId, finalNumeroNLManual);
                }
            }

            return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem }, JsonRequestBehavior.AllowGet);
        }
        private JsonResult inclusaoManualNLMovimentacaoPatrimonial(int Codigo, string ComplNL)
        {
            bool atualizacaoEfetuadaComSucesso = false;
            string mensagem = "";
            Regex regexValidacao = null;
            bool ehEstorno = false;
            AuditoriaIntegracao registroAuditoria = null;
            string numeroNLManual = null;



            getHierarquiaPerfil();
            ComplNL = ComplNL.PadLeft(5, '0');
            regexValidacao = new Regex("[0-9A-Z]*$");
            if (!regexValidacao.IsMatch(ComplNL))
            {
                mensagem = "Dado informado no campo NL deve ser alfanumérico, com números e/ou letras maiúsculas com cinco dígitos!";
                return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = mensagem }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                IDictionary<TipoNotaSIAF, string> listagemTiposNotaSIAFEM = new Dictionary<TipoNotaSIAF, string>();
                IEnumerable<KeyValuePair<TipoNotaSIAF, string>> relacaoDescricaoComTipoNotasSIAFEM = Enum.GetValues(typeof(TipoNotaSIAF))
                                                                                         .Cast<TipoNotaSIAF>()
                                                                                         .ToList()
                                                                                         .Select(dadosEnumValue => new KeyValuePair<TipoNotaSIAF, string>(dadosEnumValue, EnumGeral.GetEnumDescription(dadosEnumValue)));

                var _contextoCamadaDados = new SAMContext();
                if (_contextoCamadaDados.IsNull())
                    InitDisconnectContext(ref _contextoCamadaDados);

                var notaLancamentoPendenteId = Codigo;
                var notaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(notaLancamentoPendenteParaConsulta => notaLancamentoPendenteParaConsulta.Id == notaLancamentoPendenteId).FirstOrDefault();
                var auditoriaIntegracaoVinculada = _contextoCamadaDados.AuditoriaIntegracoes.AsNoTracking().Where(registroAuditoriaIntegracaoVinculado => registroAuditoriaIntegracaoVinculado.Id == notaLancamentoPendente.AuditoriaIntegracaoId).FirstOrDefault();
                IList<int> relacaoMovimentacoesPatrimoniaisIds = null;

                //procurar linhas na tabela de amarracao (pendencias geradas pelo novo metodo de interacao com usuario, com amarracao Asset/AssetMovements/AuditoriaIntegracao)
                {
                    var listadeRegistrosDeAmarracao = _contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                   .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == notaLancamentoPendente.AuditoriaIntegracaoId)
                                                                   .ToList();
                    if (listadeRegistrosDeAmarracao.HasElements())
                    {
                        relacaoMovimentacoesPatrimoniaisIds = listadeRegistrosDeAmarracao.Select(_registroDeAmarracao => _registroDeAmarracao.AssetMovementsId).ToList();
                    }
                    else
                    {
                        using (var contextoCamadaDados = new SAMContext())
                        {
                            relacaoMovimentacoesPatrimoniaisIds = _contextoCamadaDados.AssetMovements
                                                                                      .Where(movPatrimonial => movPatrimonial.NotaLancamentoPendenteSIAFEMId == notaLancamentoPendente.Id)
                                                                                      .Select(movPatrimonial => movPatrimonial.Id)
                                                                                      .ToList();

                            if (!relacaoMovimentacoesPatrimoniaisIds.HasElements())
                            {
                                relacaoMovimentacoesPatrimoniaisIds = _contextoCamadaDados.AssetMovements
                                                                                          .Where(movPatrimonial => movPatrimonial.AuditoriaIntegracaoId == notaLancamentoPendente.AuditoriaIntegracaoId)
                                                                                          .Select(movPatrimonial => movPatrimonial.Id)
                                                                                          .ToList();
                            }
                        }
                    }
                }

                if (relacaoMovimentacoesPatrimoniaisIds.HasElements())
                {
                    var listaMovimentacoes = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => relacaoMovimentacoesPatrimoniaisIds.Contains(movPatrimonial.Id)).ToList();
                    var _user = UserCommon.CurrentUser();
                    var cpfUsuarioLogado = _user.CPF;
                    var primeiraMovimentacaoPatrimonial = listaMovimentacoes.FirstOrDefault();
                    var managerUnitId = primeiraMovimentacaoPatrimonial.ManagerUnitId;
                    ehEstorno = ((auditoriaIntegracaoVinculada.IsNotNull() && !String.IsNullOrWhiteSpace(auditoriaIntegracaoVinculada.NLEstorno)) ? (auditoriaIntegracaoVinculada.NLEstorno.ToUpperInvariant() == "S") : false);
                    int anoMesReferenciaMovimentacao = listaMovimentacoes.FirstOrDefault().MovimentDate.Year;
                    string tipoLancamentoMovimentacao = (ehEstorno ? "E" : "N");
                    numeroNLManual = string.Format("{0}NL{1}", anoMesReferenciaMovimentacao, ComplNL.Substring(0, 5));
                    var xmlAuditoriaManual = this.gerarXmlAuditoriaAlteracaoManualMovimentacaoPatrimonial(cpfUsuarioLogado, listaMovimentacoes, numeroNLManual, tipoLancamentoMovimentacao, (TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia);
                    string tipoNotaLancamentoSIAFEM = null;
                    string siglaChapa = String.Format("'{0}-{1}'", listaMovimentacoes.FirstOrDefault().RelatedAssets.InitialName, listaMovimentacoes.FirstOrDefault().RelatedAssets.NumberIdentification);
                    string numeroDocumentoSAM = listaMovimentacoes.FirstOrDefault().NumberDoc;
                    relacaoDescricaoComTipoNotasSIAFEM.ToList().ForEach(dadosEnumValue => listagemTiposNotaSIAFEM.Add(dadosEnumValue));

                    //CHECAGEM DE EXISTENCIA ANTERIOR DE NL INFORMADA 'MANUALMENTE'
                    //if (_perfilId.GetValueOrDefault() == EnumProfile.OperadordeUGE.GetHashCode())
                    {
                        mensagem = verificacaoDuplicidadeNLs(primeiraMovimentacaoPatrimonial.Id, numeroNLManual);
                        atualizacaoEfetuadaComSucesso = !String.IsNullOrWhiteSpace(mensagem);

                        if (atualizacaoEfetuadaComSucesso)
                            return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem }, JsonRequestBehavior.AllowGet);
                    }

                    this.VinculacaoNLContabilizaSP(listaMovimentacoes, (TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia, numeroNLManual, ehEstorno);
                    this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculada.Id);


                    if (listaMovimentacoes.HasElements())
                    {
                        /*
                        this.criaOuRegeraNotaLancamentoPendenciaParaMovimentacaoVinculada(listaMovimentacoes, (TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia);
                        registroAuditoria = new AuditoriaIntegracao();
                        registroAuditoria.DataEnvio = DateTime.Now;
                        registroAuditoria.MsgEstimuloWS = xmlAuditoriaManual;
                        registroAuditoria.MsgRetornoWS = null;
                        registroAuditoria.NomeSistema = auditoriaIntegracaoVinculada.NomeSistema;
                        registroAuditoria.ManagerUnitId = auditoriaIntegracaoVinculada.ManagerUnitId;
                        registroAuditoria.UsuarioSAM = cpfUsuarioLogado;
                        registroAuditoria.UsuarioSistemaExterno = null;
                        registroAuditoria.NotaLancamento = numeroNLManual;
                        //registroAuditoria.AssetMovementIds = String.Join(" ", relacaoMovimentacoesPatrimoniaisIds);
                        //registroAuditoria.AssetMovementIds.TrimEnd();
                        this.RegistraAuditoriaIntegracao(listaMovimentacoes, registroAuditoria);
                        */
                        this.criaOuRegeraNotaLancamentoPendenciaParaMovimentacaoVinculada(listaMovimentacoes, (TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia);
                        auditoriaIntegracaoVinculada.DataRetorno = DateTime.Now;
                        auditoriaIntegracaoVinculada.MsgRetornoWS = xmlAuditoriaManual;
                        auditoriaIntegracaoVinculada.UsuarioSAM = cpfUsuarioLogado;
                        auditoriaIntegracaoVinculada.NotaLancamento = numeroNLManual;
                        this.atualizaAuditoriaIntegracao(auditoriaIntegracaoVinculada);


                        var dadosNLsMovimentacao = obterDadosNLsMovimentacao(primeiraMovimentacaoPatrimonial, (TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia, ehEstorno);
                        if (dadosNLsMovimentacao.HasElements())
                        {
                            var _tipoNotaLancamentoSIAFEM = listagemTiposNotaSIAFEM[(TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia];
                            var infoNLManualRecemInclusa = dadosNLsMovimentacao.Where(detalheDadosNLMovimentacao => detalheDadosNLMovimentacao.Item1 == _tipoNotaLancamentoSIAFEM
                                                                                                                 && detalheDadosNLMovimentacao.Item2.Contains(numeroNLManual))
                                                                                .FirstOrDefault();

                            atualizacaoEfetuadaComSucesso = infoNLManualRecemInclusa.IsNotNull();
                        }

                        bool pluralizaMsgRetorno = (listaMovimentacoes.Count() > 1);
                        string sufixoS = (pluralizaMsgRetorno ? "s" : null);
                        string verboConjugado = (pluralizaMsgRetorno ? "foram" : "foi");
                        string dadosBemPatrimonial = (!pluralizaMsgRetorno ? String.Format(" ({0}) ", siglaChapa) : " ");
                        tipoNotaLancamentoSIAFEM = listagemTiposNotaSIAFEM[(TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia];
                        mensagem = String.Format("O{0} BP{0}{5}vinculado{0} ao documento SAM '{1}' {2} atualizado{0} com a inclusão manual da NL '{3}' ('{4}')", sufixoS, numeroDocumentoSAM, verboConjugado, numeroNLManual, tipoNotaLancamentoSIAFEM, dadosBemPatrimonial);
                    }
                    else
                    {
                        //SENAO ACHAR NADA VINCULADA A ESTA PENDENCIA, INSERIR REGISTRO NA TABELA ERROLOG E INATIVAR PENDENCIA
                        atualizacaoEfetuadaComSucesso = true;
                        numeroNLManual = string.Format("{0}NL{1}", auditoriaIntegracaoVinculada.Data.GetValueOrDefault().Year, ComplNL.Substring(0, 5));
                        base.GravaLogErro("PENDENCIA SEM REGISTROS VINCULADOS", String.Format("UGE: {0}, NL {1}, AuditoriaIntegracaoId_NL_Manual: {2}", auditoriaIntegracaoVinculada.UgeOrigem, numeroNLManual, auditoriaIntegracaoVinculada.Id), "Inclusão NL Manual");
                        this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculada.Id);

                        mensagem = "Registro em estado inconsistente";
                    }
                }
                //SENAO ACHAR NADA VINCULADA A ESTA PENDENCIA, INATIVAR A MESMA
                else
                {
                    atualizacaoEfetuadaComSucesso = true;
                    numeroNLManual = string.Format("{0}NL{1}", auditoriaIntegracaoVinculada.Data.GetValueOrDefault().Year, ComplNL.Substring(0, 5));
                    base.GravaLogErro("PENDENCIA SEM REGISTROS VINCULADOS", String.Format("UGE: {0}, NL {1}, AuditoriaIntegracaoId_NL_Manual: {2}", auditoriaIntegracaoVinculada.UgeOrigem, numeroNLManual, auditoriaIntegracaoVinculada.Id), "Inclusão NL Manual");
                    this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculada.Id);

                    mensagem = "Registro em estado inconsistente";
                }
            }
            return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem }, JsonRequestBehavior.AllowGet);
        }

        private string verificacaoDuplicidadeNLs(int assetMovementsId, string numeroNLManual)
        {
            string msgRetorno = null;

            var _contextoCamadaDados = new SAMContext();
            if (_contextoCamadaDados.IsNull())
                InitDisconnectContext(ref _contextoCamadaDados);

            var movPatrimonialParaConsulta = _contextoCamadaDados.AssetMovements
                                                                 .AsNoTracking()
                                                                 .Where(movPatrimonial => movPatrimonial.Id == assetMovementsId)
                                                                 .FirstOrDefault();

            if (movPatrimonialParaConsulta.IsNotNull())
            {
                var managerUnitId = movPatrimonialParaConsulta.ManagerUnitId;
                var movPatrimonialOrigemUtilizaSAMPatrimonio = _contextoCamadaDados.AssetMovements
                                                                                   .Include("RelatedMovementType")
                                                                                   .Include("RelatedManagerUnit")
                                                                                   .Where(movPatrimonial => (movPatrimonial.AssetTransferenciaId == movPatrimonialParaConsulta.AssetId)
                                                                                                         && (movPatrimonial.NotaLancamento == numeroNLManual
                                                                                                             || movPatrimonial.NotaLancamentoEstorno == numeroNLManual
                                                                                                             || movPatrimonial.NotaLancamentoDepreciacao == numeroNLManual
                                                                                                             || movPatrimonial.NotaLancamentoDepreciacaoEstorno == numeroNLManual
                                                                                                             || movPatrimonial.NotaLancamentoReclassificacao == numeroNLManual
                                                                                                             || movPatrimonial.NotaLancamentoReclassificacaoEstorno == numeroNLManual))
                                                                                   .FirstOrDefault();

                if (movPatrimonialOrigemUtilizaSAMPatrimonio.IsNotNull())
                {
                    msgRetorno = String.Format("NL informada ('{0}') é idêntica à NL vinculada à movimentação patrimonial de documento SAM '{1}' (UGE:{2}; '{3}')\n\nInclusão Manual de NL só será possível mediante utilização de perfil 'Operador de UO'!.", numeroNLManual, movPatrimonialOrigemUtilizaSAMPatrimonio.NumberDoc, movPatrimonialOrigemUtilizaSAMPatrimonio.RelatedManagerUnit.Code.PadLeft(6, '0'), movPatrimonialOrigemUtilizaSAMPatrimonio.RelatedMovementType.Description);
                }
                else
                {
                    //MOVIMENTACAO PATRIMONIAL DE INCORPORACAO SEM AMARRACAO
                    var movPatrimoniaisComNL = _contextoCamadaDados.AssetMovements.Include("RelatedMovementType")
                                                                                  .Where(movPatrimonial => (movPatrimonial.ManagerUnitId == managerUnitId)
                                                                                                        && (movPatrimonial.NotaLancamento == numeroNLManual
                                                                                                            || movPatrimonial.NotaLancamentoEstorno == numeroNLManual
                                                                                                            || movPatrimonial.NotaLancamentoDepreciacao == numeroNLManual
                                                                                                            || movPatrimonial.NotaLancamentoDepreciacaoEstorno == numeroNLManual
                                                                                                            || movPatrimonial.NotaLancamentoReclassificacao == numeroNLManual
                                                                                                            || movPatrimonial.NotaLancamentoReclassificacaoEstorno == numeroNLManual))
                                                                                  .ToList();

                    if (movPatrimoniaisComNL.HasElements())
                    {
                        var primeiraMovPatrimonial = movPatrimoniaisComNL.FirstOrDefault();
                        msgRetorno = String.Format("NL informada ('{0}') está vinculada à movimentação patrimonial de documento SAM '{1}' ('{2}').\n\nNão é permitida a vinculação de uma NL mais de uma vez para a mesma movimentação ou para movimentações diferentes!", numeroNLManual, primeiraMovPatrimonial.NumberDoc, primeiraMovPatrimonial.RelatedMovementType.Description);
                    }
                }
            }

            return msgRetorno;
        }

        private JsonResult inclusaoManualNLFechamentoMensalIntegrado(int Codigo, string ComplNL)
        {
            bool atualizacaoEfetuadaComSucesso = false;
            string mensagem = "";
            string numeroNLManual = null;
            string cpfUsuarioLogado = null;
            bool ehEstorno = false;
            int anoMesReferenciaFechamentoMensalIntegrado = 0;
            string tipoLancamentoFechamentoMensalIntegrado = null;
            string tipoLancamentoSIAFEM = null;
            string numeroDocumentoSAM = null;
            string codigoContaContabilDepreciacao = null;
            AccountingClosing fechamentoContaContabil = null;
            AccountingClosingExcluidos fechamentoContaContabilEstornado = null;
            NotaLancamentoPendenteSIAFEM notaLancamentoPendente = null;
            AuditoriaIntegracao auditoriaIntegracaoParaInclusao = null;
            AuditoriaIntegracao auditoriaIntegracaoVinculada = null;
            Regex regexValidacao = null;
            long rowTabelaId = 0;
            string anoMesReferencia = null;




            getHierarquiaPerfil();
            ComplNL = ComplNL.PadLeft(5, '0');
            regexValidacao = new Regex("[0-9A-Z]*$");
            if (!regexValidacao.IsMatch(ComplNL))
            {
                mensagem = "Dado informado no campo NL deve ser alfanumérico, com números e/ou letras maiúsculas com cinco dígitos!";
                return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = mensagem }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var notaLancamentoPendenteId = Codigo;
                if (notaLancamentoPendenteId > 0)
                {
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        notaLancamentoPendente = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                    .Where(notaLancamentoPendenteParaConsulta => notaLancamentoPendenteParaConsulta.Id == notaLancamentoPendenteId)
                                                                    .FirstOrDefault();

                        if (notaLancamentoPendente.IsNotNull())
                        {
                            auditoriaIntegracaoVinculada = contextoCamadaDados.AuditoriaIntegracoes
                                                                              .Where(registroAuditoriaIntegracaoVinculado => registroAuditoriaIntegracaoVinculado.Id == notaLancamentoPendente.AuditoriaIntegracaoId)
                                                                              .FirstOrDefault();

                            ehEstorno = (auditoriaIntegracaoVinculada.NLEstorno.ToUpperInvariant() == "S");
                            if (!ehEstorno)
                            {
                                fechamentoContaContabil = contextoCamadaDados.AccountingClosings
                                                                             .Where(registroFechamentoContaContabil => registroFechamentoContaContabil.AuditoriaIntegracaoId == notaLancamentoPendente.AuditoriaIntegracaoId)
                                                                             .FirstOrDefault();
                            }
                            else
                            {
                                fechamentoContaContabilEstornado = contextoCamadaDados.AccountingClosingExcluidos
                                                                                      .Where(registroFechamentoContaContabilEstornado => registroFechamentoContaContabilEstornado.AuditoriaIntegracaoIdEstorno == notaLancamentoPendente.AuditoriaIntegracaoId)
                                                                                      .FirstOrDefault();
                            }

                            if (auditoriaIntegracaoVinculada.IsNotNull())
                            {
                                var _user = UserCommon.CurrentUser();
                                cpfUsuarioLogado = _user.CPF;
                                ehEstorno = (auditoriaIntegracaoVinculada.NLEstorno.ToUpperInvariant() == "S");
                                anoMesReferenciaFechamentoMensalIntegrado = auditoriaIntegracaoVinculada.Data.Value.Year;
                                tipoLancamentoFechamentoMensalIntegrado = (ehEstorno ? "estorno de " : null);
                                numeroNLManual = string.Format("{0}NL{1}", anoMesReferenciaFechamentoMensalIntegrado, ComplNL.Substring(0, 5));
                                numeroDocumentoSAM = auditoriaIntegracaoVinculada.NotaFiscal;


                                if (fechamentoContaContabil.IsNotNull() || fechamentoContaContabilEstornado.IsNotNull())
                                {
                                    if (!ehEstorno)
                                    {
                                        fechamentoContaContabil.GeneratedNL = numeroNLManual;
                                        rowTabelaId = fechamentoContaContabil.Id;
                                        codigoContaContabilDepreciacao = fechamentoContaContabil.DepreciationAccount.GetValueOrDefault().ToString();
                                        anoMesReferencia = fechamentoContaContabil.ReferenceMonth;
                                    }
                                    else
                                    {
                                        fechamentoContaContabilEstornado.NLEstorno = numeroNLManual;
                                        rowTabelaId = fechamentoContaContabilEstornado.Id;
                                        codigoContaContabilDepreciacao = fechamentoContaContabilEstornado.DepreciationAccount.GetValueOrDefault().ToString();
                                        anoMesReferencia = fechamentoContaContabilEstornado.ReferenceMonth;
                                    }


                                    tipoLancamentoSIAFEM = (ehEstorno ? "E" : "N");
                                    var xmlAuditoriaManual = this.gerarXmlAuditoriaAlteracaoManualFechamentoMensalIntegrado(cpfUsuarioLogado, rowTabelaId, codigoContaContabilDepreciacao, anoMesReferencia, numeroNLManual, tipoLancamentoSIAFEM);
                                    auditoriaIntegracaoParaInclusao = new AuditoriaIntegracao();
                                    auditoriaIntegracaoParaInclusao.DataEnvio = DateTime.Now;
                                    auditoriaIntegracaoParaInclusao.MsgEstimuloWS = xmlAuditoriaManual;
                                    auditoriaIntegracaoParaInclusao.NomeSistema = auditoriaIntegracaoVinculada.NomeSistema;
                                    auditoriaIntegracaoParaInclusao.ManagerUnitId = auditoriaIntegracaoVinculada.ManagerUnitId;
                                    auditoriaIntegracaoParaInclusao.UsuarioSAM = cpfUsuarioLogado;

                                    contextoCamadaDados.SaveChanges();
                                    this.insereRegistroAuditoriaParcialNaBaseDeDados(auditoriaIntegracaoParaInclusao);
                                    this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculada.Id);
                                    //this.DeleteRegistroNaAccountingClosings(numeroDocumentoSAM, numeroNLManual);
                                }
                            }
                        }
                    }

                    atualizacaoEfetuadaComSucesso = true;
                    mensagem = String.Format("Os dados de {0} Fechamento Mensal Integrado para a conta contábil de depreciação '{1}' foram atualizados com a inclusão manual da NL '{2}'", tipoLancamentoFechamentoMensalIntegrado, codigoContaContabilDepreciacao, numeroNLManual);
                }
            }

            return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem }, JsonRequestBehavior.AllowGet);
        }

        private NotaLancamentoPendenteSIAFEM obterEntidadeNotaLancamentoPendenteSIAFEM(int notaLancamentoPendenteSIAFEMId)
        {
            NotaLancamentoPendenteSIAFEM objEntidade = null;


            if (notaLancamentoPendenteSIAFEMId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                     .Where(notaLancamentoPendenteSIAFEM => notaLancamentoPendenteSIAFEM.Id == notaLancamentoPendenteSIAFEMId)
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
                string name = "PendenciaNotalancamentoSIAFEMController.obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria";
                GravaLogErro(messageErro, stackTrace, name);
                throw excErroRuntime;
            }

            return listaMovPatrimoniais;
        }
        public bool geraNotaLancamentoPendencia(int auditoriaIntegracaoId)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracao = null;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> listadeRegistrosDeAmarracao = null;
            TipoNotaSIAF tipoAgrupamentoNotaLancamento = TipoNotaSIAF.Desconhecido;
            IList<AssetMovements> listaMovimentacoesPatrimoniais = null;
            string numeroDocumentoSAM = null;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;

            int[] idsMovPatrimoniais = null;

            if (auditoriaIntegracaoId > 0)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);

                if (registroAuditoriaIntegracao.IsNotNull())
                {
                    listadeRegistrosDeAmarracao = _contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                      .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                      .ToList();
                    if (listadeRegistrosDeAmarracao.HasElements())
                        idsMovPatrimoniais = listadeRegistrosDeAmarracao.Select(_registroDeAmarracao => _registroDeAmarracao.AssetMovementsId).ToArray();

                    tipoAgrupamentoNotaLancamento = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);
                    listaMovimentacoesPatrimoniais = obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria(auditoriaIntegracaoId);
                    if (listaMovimentacoesPatrimoniais.HasElements())
                    {
                        registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                        numeroDocumentoSAM = listaMovimentacoesPatrimoniais.FirstOrDefault().NumberDoc;
                        pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
                        {
                            AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id,
                            DataHoraEnvioMsgWS = registroAuditoriaIntegracao.DataEnvio,
                            ManagerUnitId = registroAuditoriaIntegracao.ManagerUnitId,
                            TipoNotaPendencia = (short)tipoAgrupamentoNotaLancamento,
                            NumeroDocumentoSAM = numeroDocumentoSAM,
                            StatusPendencia = 1,
                            AssetMovementIds = registroAuditoriaIntegracao.AssetMovementIds,
                            ErroProcessamentoMsgWS = registroAuditoriaIntegracao.MsgErro,
                        };


                        idsMovPatrimoniais = ((idsMovPatrimoniais.HasElements()) ? idsMovPatrimoniais : listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray());
                        using (_contextoCamadaDados = new SAMContext())
                        {
                            var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                            var _registroAuditoriaIntegracao = _contextoCamadaDados.AuditoriaIntegracoes.Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id).FirstOrDefault();

                            _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                            numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();

                            _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial =>
                            {
                                movPatrimonial.NotaLancamentoPendenteSIAFEMId = pendenciaNLContabilizaSP.Id;
                                pendenciaNLContabilizaSP.AuditoriaIntegracaoId = _registroAuditoriaIntegracao.Id;

                                if (!listadeRegistrosDeAmarracao.HasElements())
                                {
                                    registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                                    registroDeAmarracao.AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id;
                                    registroDeAmarracao.AssetMovementsId = movPatrimonial.Id;
                                    registroDeAmarracao.AssetId = movPatrimonial.AssetId;

                                    _contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Add(registroDeAmarracao);
                                }
                            });
                            //TODO DCBATISTA ACOMPANHAR ELIMINACAO
                            pendenciaNLContabilizaSP.AssetMovementIds = _registroAuditoriaIntegracao.AssetMovementIds;
                            numeroRegistrosManipulados = +_contextoCamadaDados.SaveChanges();
                        }
                    }
                }
            }


            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }

        public JsonResult ReenvioAutomatico(int Codigo, string LoginUsuarioSIAFEM, string SenhaUsuarioSIAFEM)
        {
            string cpfUsuarioSIAFEM = null;
            string senhaUsuarioSIAFEM = null;
            bool dadosAcessoSiafemInformados = false;
            bool atualizacaoEfetuadaComSucesso = false;
            string msgInformeProcessamento = null;
            int notaLancamentoPendenteId = 0;
            NotaLancamentoPendenteSIAFEM notaLancamentoPendente = null;




            if (String.IsNullOrWhiteSpace(LoginUsuarioSIAFEM) || String.IsNullOrWhiteSpace(SenhaUsuarioSIAFEM))
            {
                msgInformeProcessamento = "Dados de acesso ao sistema Contabiliza-SP não informados!";
                return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
            }


            dadosAcessoSiafemInformados = (!String.IsNullOrWhiteSpace(LoginUsuarioSIAFEM) && !String.IsNullOrWhiteSpace(SenhaUsuarioSIAFEM));
            notaLancamentoPendenteId = Codigo;
            if (notaLancamentoPendenteId > 0)
            {
                initDadosNotasSIAFEM();
                cpfUsuarioSIAFEM = LoginUsuarioSIAFEM;
                senhaUsuarioSIAFEM = SenhaUsuarioSIAFEM;
                getHierarquiaPerfil();


                if (dadosAcessoSiafemInformados)
                {
                    SAMContext _contextoCamadaDados = new SAMContext();
                    notaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.AsNoTracking().Where(notaLancamentoPendenteParaConsulta => notaLancamentoPendenteParaConsulta.Id == notaLancamentoPendenteId).FirstOrDefault();

                    if (notaLancamentoPendente.IsNotNull())
                    {
                        if (pendenciaNLSiafemEhDeMovimentacaoPatrimonial(notaLancamentoPendenteId))
                            return reenvioAutomaticoMovimentacaoPatrimonial(notaLancamentoPendenteId, cpfUsuarioSIAFEM, senhaUsuarioSIAFEM);
                        else if (pendenciaNLSiafemEhDeFechamentoMensalIntegrado(notaLancamentoPendenteId))
                            return reenvioAutomaticoFechamentoMensalIntegrado(notaLancamentoPendenteId, cpfUsuarioSIAFEM, senhaUsuarioSIAFEM);
                    }
                }
                else if (!dadosAcessoSiafemInformados)
                {
                    msgInformeProcessamento = "Dados de acesso ao sistema Contabiliza-SP não informados!";
                    return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
        }
        private JsonResult reenvioAutomaticoMovimentacaoPatrimonial(int Codigo, string LoginUsuarioSIAFEM, string SenhaUsuarioSIAFEM)
        {
            string cpfUsuarioSIAFEM = null;
            string senhaUsuarioSIAFEM = null;
            string cpfUsuarioLogadoSAM = null;
            bool dadosAcessoSiafemInformados = false;
            bool atualizacaoEfetuadaComSucesso = false;
            string msgInformeProcessamento = null;
            string tipoNotaLancamentoSIAFEM = null;
            User usuarioLogado = null;
            int notaLancamentoPendenteId = 0;
            bool ehEstorno = false;
            string numeroDocumentoSAM = null;
            string descricaoTipoMovimentacao = null;
            NotaLancamentoPendenteSIAFEM notaLancamentoPendente = null;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            IList<AssetMovements> listaMovimentacoesPatrimoniais = null;
            IList<int> relacaoMovimentacoesPatrimoniaisIds = null;
            int auditoriaIntegracaoId = 0;
            string msgErroSIAFEM = null;
            string nlSIAFEM = null;
            string msgNLGerada = null;
            //IList<Tuple<string, string>> listaNLsGeradas = null;
            Tuple<string, string, string, string, bool> dadosProcessamentoSIAFEM = null;
            bool erroRetornadoDoSIAFEM = false;
            string complementoSeEstorno = null;




            if (String.IsNullOrWhiteSpace(LoginUsuarioSIAFEM) || String.IsNullOrWhiteSpace(SenhaUsuarioSIAFEM))
            {
                msgInformeProcessamento = "Dados de acesso ao sistema Contabiliza-SP não informados!";
                return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
            }


            dadosAcessoSiafemInformados = (!String.IsNullOrWhiteSpace(LoginUsuarioSIAFEM) && !String.IsNullOrWhiteSpace(SenhaUsuarioSIAFEM));
            usuarioLogado = UserCommon.CurrentUser();
            notaLancamentoPendenteId = Codigo;
            if (notaLancamentoPendenteId > 0)
            {
                initDadosNotasSIAFEM();
                cpfUsuarioSIAFEM = LoginUsuarioSIAFEM;
                senhaUsuarioSIAFEM = SenhaUsuarioSIAFEM;
                getHierarquiaPerfil();


                if (dadosAcessoSiafemInformados)
                {
                    cpfUsuarioLogadoSAM = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : "00000000000");
                    SAMContext _contextoCamadaDados = new SAMContext();
                    notaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.AsNoTracking().Where(notaLancamentoPendenteParaConsulta => notaLancamentoPendenteParaConsulta.Id == notaLancamentoPendenteId).FirstOrDefault();

                    if (notaLancamentoPendente.IsNotNull())
                    {
                        auditoriaIntegracaoId = notaLancamentoPendente.AuditoriaIntegracaoId;
                        if (auditoriaIntegracaoId > 0)
                        {
                            //consulta ao SIAFMONITORA (servico de consulta de token/chave, criado para evitar duplicidade de geracao de NL por nao captura de retorno por nao-envio/timeout/whatever do XML por conta do VHI/SIAFEM/CONTABILIZA-SP
                            //se nao foi gerada NL SIAFEM e/ou a geracao nao foi detectado pelo SIAFMONITORA via token
                            if (!VerificaSeExisteNLNoServicoMonitoramentoSIAFEMEAtualizaMovimentacoes(cpfUsuarioSIAFEM, senhaUsuarioSIAFEM, notaLancamentoPendente, out msgErroSIAFEM))
                            {
                                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                                IntegracaoContabilizaSPController controllerIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                                dadosProcessamentoSIAFEM = controllerIntegracaoSIAFEM.ProcessaReenvioPendenciaNLMovimentacaoPatrimonialParaSIAFEM(auditoriaIntegracaoId, cpfUsuarioLogadoSAM, cpfUsuarioSIAFEM, senhaUsuarioSIAFEM);

                                descricaoTipoMovimentacao = dadosProcessamentoSIAFEM.Item1;
                                erroRetornadoDoSIAFEM = dadosProcessamentoSIAFEM.Item5;

                                //retorno do registro atualizado do banco...
                                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                                if (!erroRetornadoDoSIAFEM)
                                {
                                    nlSIAFEM = dadosProcessamentoSIAFEM.Item3; //captura a NL
                                    msgNLGerada = String.Format("NL retornada pela integração SAM/Contabiliza-SP: {0}", nlSIAFEM);

                                    //inativo a pendencia, jah que a NL foi gerada com base nos dados da linha de AuditoriaIntegracao
                                    this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);


                                    //vincula NL aos BPs estao amarrados a AuditoriaIntegracaoId //TODO DCBATISTA REFATORAR COMO METODO POSTERIORMENTE
                                    #region Vinculacao NL/BPs
                                    {
                                        //procurar linhas na tabela de amarracao (pendencias geradas pelo novo metodo de interacao com usuario, com amarracao AssetMovements/Asset/AuditoriaIntegracao)
                                        if (String.IsNullOrWhiteSpace(notaLancamentoPendente.AssetMovementIds))
                                        {
                                            var listadeRegistrosDeAmarracao = _contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                                  .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == notaLancamentoPendente.AuditoriaIntegracaoId)
                                                                                                  .ToList();
                                            if (listadeRegistrosDeAmarracao.HasElements())
                                            {
                                                relacaoMovimentacoesPatrimoniaisIds = listadeRegistrosDeAmarracao.Select(_registroDeAmarracao => _registroDeAmarracao.AssetMovementsId).ToArray();
                                            }
                                            else
                                            {
                                                using (var contextoCamadaDados = new SAMContext())
                                                {
                                                    relacaoMovimentacoesPatrimoniaisIds = contextoCamadaDados.AssetMovements
                                                                                                             .Where(movPatrimonial => movPatrimonial.NotaLancamentoPendenteSIAFEMId == notaLancamentoPendente.Id)
                                                                                                             .Select(movPatrimonial => movPatrimonial.Id)
                                                                                                             .ToList();
                                                }
                                            }
                                        }
                                        //senao utiliza o metodo antigo (parsing da linha string com Id's separados por espcos em branco
                                        else
                                        {
                                            relacaoMovimentacoesPatrimoniaisIds = notaLancamentoPendente.AssetMovementIds.BreakLine().ToList().Select(movPatrimonialId => Int32.Parse(movPatrimonialId)).ToList();
                                        }

                                        if (relacaoMovimentacoesPatrimoniaisIds.HasElements())
                                        {
                                            listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements
                                                                                                 .Where(movPatrimonial => relacaoMovimentacoesPatrimoniaisIds.Contains(movPatrimonial.Id))
                                                                                                 .ToList();
                                            ehEstorno = this.ehEstorno(listaMovimentacoesPatrimoniais.FirstOrDefault());
                                            this.VinculacaoNLContabilizaSP(listaMovimentacoesPatrimoniais, (TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia, nlSIAFEM, ehEstorno);
                                        }


                                        //atualiza registro na tabela NotaLancamentoPendente com dados de novo reenvio para ContabilizaSP
                                        registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                                        registroAuditoriaIntegracao.NotaLancamento = nlSIAFEM;
                                        registroAuditoriaIntegracao.UsuarioSAM = cpfUsuarioLogadoSAM;
                                        registroAuditoriaIntegracao.MsgErro = null;
                                        notaLancamentoPendente.DataHoraEnvioMsgWS = registroAuditoriaIntegracao.DataEnvio;
                                        notaLancamentoPendente.StatusPendencia = 0; //inativacao pendencia
                                        this.atualizaNotaLancamentoPendente(notaLancamentoPendente); //atualiza registro da pendencia
                                    }
                                    #endregion Vinculacao NL/BPs

                                    //formatacao mensagem retorno ao usuario //TODO DCBATISTA REFATORAR COMO METODO POSTERIORMENTE
                                    #region Formatacao Mensagem Retorno
                                    {
                                        complementoSeEstorno = (ehEstorno ? " estorno " : null);
                                        tipoNotaLancamentoSIAFEM = EnumUtils.GetEnumDescription((TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia);

                                        string NLsVinculadas = null;
                                        var dadosNLsSIAFEM = obterDadosNLsMovimentacao(listaMovimentacoesPatrimoniais.FirstOrDefault(), (TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia, ehEstorno);
                                        if (dadosNLsSIAFEM.HasElements())
                                        {
                                            NLsVinculadas = String.Join(" ", dadosNLsSIAFEM.Where(dadosNL => dadosNL.Item1.ToUpperInvariant() == tipoNotaLancamentoSIAFEM.ToUpperInvariant())
                                                                                           .Select(dadosNL => dadosNL.Item2)
                                                                                           .FirstOrDefault());


                                            numeroDocumentoSAM = listaMovimentacoesPatrimoniais.FirstOrDefault().NumberDoc;
                                            msgNLGerada = String.Format("Os BP's vinculados a {0} movimentação patrimonial do tipo '{1}', documento SAM {2} foram atualizados com a {3} {4}.",
                                                                                    complementoSeEstorno, descricaoTipoMovimentacao, numeroDocumentoSAM, tipoNotaLancamentoSIAFEM, NLsVinculadas);


                                        }
                                    }
                                    #endregion Formatacao Mensagem Retorno

                                    registroAuditoriaIntegracao.MsgErro = null;
                                    registroAuditoriaIntegracao.NotaLancamento = nlSIAFEM;
                                    registroAuditoriaIntegracao.DataRetorno = DateTime.Now;
                                    this.atualizaAuditoriaIntegracao(registroAuditoriaIntegracao);
                                }
                                else
                                {
                                    msgErroSIAFEM = dadosProcessamentoSIAFEM.Item2; //captura o erro SIAFEM retornado
                                    registroAuditoriaIntegracao.MsgErro = msgErroSIAFEM;
                                    registroAuditoriaIntegracao.DataEnvio = DateTime.Now;
                                    registroAuditoriaIntegracao.NotaLancamento = null;
                                    this.atualizaAuditoriaIntegracao(registroAuditoriaIntegracao);

                                    notaLancamentoPendente.ErroProcessamentoMsgWS = registroAuditoriaIntegracao.MsgErro;
                                    notaLancamentoPendente.DataHoraEnvioMsgWS = DateTime.Now;
                                    notaLancamentoPendente.DataHoraReenvioMsgWS = DateTime.Now;
                                    this.atualizaNotaLancamentoPendente(notaLancamentoPendente);


                                    //formatacao mensagem de erro
                                    msgErroSIAFEM = ((!String.IsNullOrWhiteSpace(msgErroSIAFEM)) ? String.Format("Erro retornado pela integração SAM/Contabiliza-SP: {0}", msgErroSIAFEM) : null);
                                }

                                //tratamento de mensagem de retorno ao usuario
                                msgInformeProcessamento = (!String.IsNullOrWhiteSpace(msgNLGerada) ? msgNLGerada : msgErroSIAFEM);
                            }
                            //se foi gerada NL SIAFEM e a consulta via token ao SIAFMONITORA retornou dados
                            else
                            {
                                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);


                                complementoSeEstorno = ((registroAuditoriaIntegracao.NLEstorno.ToUpperInvariant() == "S") ? "estorno" : null);
                                descricaoTipoMovimentacao = registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                                numeroDocumentoSAM = registroAuditoriaIntegracao.NotaFiscal;
                                tipoNotaLancamentoSIAFEM = obterDescricaoTipoNotaSIAFEM((TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia);
                                nlSIAFEM = registroAuditoriaIntegracao.NotaLancamento;


                                msgInformeProcessamento = String.Format("O(s) BP(s) vinculado(s) a {0} movimentação patrimonial do tipo '{1}', documento SAM '{2}' foi (foram) atualizado(s) com a NL {3} ({4}).", complementoSeEstorno, descricaoTipoMovimentacao, numeroDocumentoSAM, nlSIAFEM, tipoNotaLancamentoSIAFEM);
                            }
                        }
                    }
                }
                else if (!dadosAcessoSiafemInformados)
                {
                    msgInformeProcessamento = "Dados de acesso ao sistema Contabiliza-SP não informados!";
                    return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
        }
        private JsonResult reenvioAutomaticoFechamentoMensalIntegrado(int Codigo, string LoginUsuarioSIAFEM, string SenhaUsuarioSIAFEM)
        {
            string cpfUsuarioSIAFEM = null;
            string senhaUsuarioSIAFEM = null;
            string cpfUsuarioLogadoSAM = null;
            bool dadosAcessoSiafemInformados = false;
            bool atualizacaoEfetuadaComSucesso = false;
            string msgInformeProcessamento = null;
            string tipoNotaLancamentoSIAFEM = null;
            User usuarioLogado = null;
            int notaLancamentoPendenteId = 0;
            bool ehEstorno = false;
            string descricaoTipoMovimentacao = null;
            NotaLancamentoPendenteSIAFEM notaLancamentoPendente = null;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            string documentoSAM = null;
            int auditoriaIntegracaoId = 0;
            string msgErroSIAFEM = null;
            string nlSIAFEM = null;
            string msgNLGerada = null;
            //IList<Tuple<string, string>> listaNLsGeradas = null;
            Tuple<string, string, string, string, bool> dadosProcessamentoSIAFEM = null;
            bool erroRetornadoDoSIAFEM = false;
            string complementoSeEstorno = null;
            string codigoContaContabilDepreciacao = null;




            if (String.IsNullOrWhiteSpace(LoginUsuarioSIAFEM) || String.IsNullOrWhiteSpace(SenhaUsuarioSIAFEM))
            {
                msgInformeProcessamento = "Dados de acesso ao sistema Contabiliza-SP não informados!";
                return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
            }


            dadosAcessoSiafemInformados = (!String.IsNullOrWhiteSpace(LoginUsuarioSIAFEM) && !String.IsNullOrWhiteSpace(SenhaUsuarioSIAFEM));
            usuarioLogado = UserCommon.CurrentUser();
            notaLancamentoPendenteId = Codigo;
            if (notaLancamentoPendenteId > 0)
            {
                initDadosNotasSIAFEM();
                cpfUsuarioSIAFEM = LoginUsuarioSIAFEM;
                senhaUsuarioSIAFEM = SenhaUsuarioSIAFEM;
                getHierarquiaPerfil();


                if (dadosAcessoSiafemInformados)
                {
                    cpfUsuarioLogadoSAM = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : "00000000000");
                    SAMContext _contextoCamadaDados = new SAMContext();
                    notaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.AsNoTracking().Where(notaLancamentoPendenteParaConsulta => notaLancamentoPendenteParaConsulta.Id == notaLancamentoPendenteId).FirstOrDefault();

                    if (notaLancamentoPendente.IsNotNull())
                    {
                        auditoriaIntegracaoId = notaLancamentoPendente.AuditoriaIntegracaoId;
                        if (auditoriaIntegracaoId > 0)
                        {
                            //consulta ao SIAFMONITORA (servico de consulta de token/chave, criado para evitar duplicidade de geracao de NL por nao captura de retorno por nao-envio/timeout/whatever do XML por conta do VHI/SIAFEM/CONTABILIZA-SP
                            //se nao foi gerada NL SIAFEM e/ou a geracao nao foi detectado pelo SIAFMONITORA via token
                            if (!VerificaSeExisteNLNoServicoMonitoramentoSIAFEMEAtualizaRegistroFechamentoMensalIntegrado(cpfUsuarioSIAFEM, senhaUsuarioSIAFEM, notaLancamentoPendente, out msgErroSIAFEM))
                            {
                                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                                IntegracaoContabilizaSPController controllerIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                                dadosProcessamentoSIAFEM = controllerIntegracaoSIAFEM.ProcessaReenvioPendenciaNLFechamentoMensalIntegradoParaSIAFEM(auditoriaIntegracaoId, cpfUsuarioLogadoSAM, cpfUsuarioSIAFEM, senhaUsuarioSIAFEM);

                                descricaoTipoMovimentacao = dadosProcessamentoSIAFEM.Item1;
                                erroRetornadoDoSIAFEM = dadosProcessamentoSIAFEM.Item5;
                                documentoSAM = notaLancamentoPendente.NumeroDocumentoSAM;
                                ehEstorno = ((registroAuditoriaIntegracao.IsNotNull() && !String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.NLEstorno)) ? (registroAuditoriaIntegracao.NLEstorno.ToUpperInvariant() == "S") : false);


                                if (!erroRetornadoDoSIAFEM)
                                {
                                    nlSIAFEM = dadosProcessamentoSIAFEM.Item3; //captura a NL
                                    msgNLGerada = String.Format("NL retornada pela integração SAM/Contabiliza-SP: {0} ({1})", nlSIAFEM, descricaoTipoMovimentacao.ToLowerInvariant());

                                    //inativo a pendencia, jah que a NL foi gerada com base nos dados da linha de AuditoriaIntegracao
                                    this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);


                                    #region Vinculacao NL/Tabela Fechamento
                                    {
                                        //atualiza linha na tabela 'AccountingClosings' (ou 'AccountingClosings' se for o caso de reabertura; estorno) com a NL SIAFEM retornada
                                        if (!ehEstorno)
                                            this.GravaNLGerada(documentoSAM, nlSIAFEM, auditoriaIntegracaoId);
                                        else
                                            this.GravaNLEstornadaGerada(documentoSAM, nlSIAFEM, auditoriaIntegracaoId);


                                        //atualiza registro na tabela NotaLancamentoPendente com dados de novo reenvio para ContabilizaSP
                                        registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                                        registroAuditoriaIntegracao.NotaLancamento = nlSIAFEM;
                                        registroAuditoriaIntegracao.UsuarioSAM = cpfUsuarioLogadoSAM;
                                        registroAuditoriaIntegracao.MsgErro = null;
                                        notaLancamentoPendente.DataHoraEnvioMsgWS = registroAuditoriaIntegracao.DataEnvio;
                                        notaLancamentoPendente.StatusPendencia = 0; //inativacao pendencia
                                        this.atualizaNotaLancamentoPendente(notaLancamentoPendente); //atualiza registro da pendencia
                                        //this.DeleteRegistroNaAccountingClosings(documentoSAM, nlSIAFEM);
                                    }
                                    #endregion Vinculacao NL/BPs

                                    //formatacao mensagem retorno ao usuario
                                    #region Formatacao Mensagem Retorno
                                    {
                                        complementoSeEstorno = (ehEstorno ? " estorno " : null);
                                        var dadosFechamentoMensalContaContabil = contextoCamadaDados.AccountingClosings
                                                                                                    .Where(fechamentoMensalContaContabil => fechamentoMensalContaContabil.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                                                    .FirstOrDefault();
                                        codigoContaContabilDepreciacao = dadosFechamentoMensalContaContabil.DepreciationAccount.GetValueOrDefault().ToString();
                                        msgNLGerada = String.Format("Os dados de {0} Fechamento Mensal Integrado para a conta contábil de depreciação '{1}' foram atualizados com a NL '{2}'", complementoSeEstorno, codigoContaContabilDepreciacao, nlSIAFEM);
                                    }
                                    #endregion Formatacao Mensagem Retorno

                                    registroAuditoriaIntegracao.MsgErro = null;
                                    registroAuditoriaIntegracao.NotaLancamento = nlSIAFEM;
                                    registroAuditoriaIntegracao.DataRetorno = DateTime.Now;
                                    this.atualizaAuditoriaIntegracao(registroAuditoriaIntegracao);
                                }
                                else
                                {
                                    msgErroSIAFEM = dadosProcessamentoSIAFEM.Item2; //captura o erro SIAFEM retornado
                                    registroAuditoriaIntegracao.MsgErro = msgErroSIAFEM;
                                    registroAuditoriaIntegracao.DataEnvio = DateTime.Now;
                                    registroAuditoriaIntegracao.NotaLancamento = null;
                                    this.atualizaAuditoriaIntegracao(registroAuditoriaIntegracao);

                                    notaLancamentoPendente.ErroProcessamentoMsgWS = registroAuditoriaIntegracao.MsgErro;
                                    notaLancamentoPendente.DataHoraEnvioMsgWS = DateTime.Now;
                                    notaLancamentoPendente.DataHoraReenvioMsgWS = DateTime.Now;
                                    this.atualizaNotaLancamentoPendente(notaLancamentoPendente);


                                    //formatacao mensagem de erro
                                    msgErroSIAFEM = ((!String.IsNullOrWhiteSpace(msgErroSIAFEM)) ? String.Format("Erro retornado pela integração SAM/Contabiliza-SP: {0}", msgErroSIAFEM) : null);
                                }

                                //tratamento de mensagem de retorno ao usuario
                                msgInformeProcessamento = (!String.IsNullOrWhiteSpace(msgNLGerada) ? msgNLGerada : msgErroSIAFEM);
                            }
                            //se foi gerada NL SIAFEM e a consulta via token ao SIAFMONITORA retornou dados
                            else
                            {
                                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);


                                complementoSeEstorno = ((registroAuditoriaIntegracao.NLEstorno.ToUpperInvariant() == "S") ? "estorno" : null);
                                descricaoTipoMovimentacao = registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                                tipoNotaLancamentoSIAFEM = obterDescricaoTipoNotaSIAFEM((TipoNotaSIAF)notaLancamentoPendente.TipoNotaPendencia);
                                nlSIAFEM = registroAuditoriaIntegracao.NotaLancamento;


                                msgInformeProcessamento = String.Format("Os dados de {0} Fechamento Mensal Integrado para a conta contábil de depreciação '{1}' foram atualizados com a NL '{2}'", complementoSeEstorno, codigoContaContabilDepreciacao, nlSIAFEM);
                            }
                        }
                    }
                }
                else if (!dadosAcessoSiafemInformados)
                {
                    msgInformeProcessamento = "Dados de acesso ao sistema Contabiliza-SP não informados!";
                    return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { resultado = atualizacaoEfetuadaComSucesso, mensagem = msgInformeProcessamento }, JsonRequestBehavior.AllowGet);
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
        private bool atualizaNotaLancamentoPendente(NotaLancamentoPendenteSIAFEM registroNotaLancamentoPendente)
        {
            bool pendenciaNLAtualizada = false;
            int numeroRegistrosManipulados = 0;


            if (registroNotaLancamentoPendente.IsNotNull())
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Attach(registroNotaLancamentoPendente);
                    contextoCamadaDados.Entry(registroNotaLancamentoPendente).State = EntityState.Modified;
                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                }
            }


            pendenciaNLAtualizada = (numeroRegistrosManipulados > 0);
            return pendenciaNLAtualizada;
        }

        private bool inativaNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            bool nlFoiInativada = false;
            int numeroRegistrosManipulados = 0;


            if (auditoriaIntegracaoId > 0)
            {
                NotaLancamentoPendenteSIAFEM pendenciaNL = null;

                using (var contextoCamadaDados = new SAMContext())
                {
                    pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                     .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                     .FirstOrDefault();

                    if (pendenciaNL.IsNotNull())
                    {
                        pendenciaNL.StatusPendencia = 0;
                        contextoCamadaDados.Entry(pendenciaNL).State = EntityState.Modified;

                        numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    }
                }
            }


            nlFoiInativada = (numeroRegistrosManipulados > 0);
            return nlFoiInativada;
        }
        #endregion Reenvio Automatico

        #region Verificacao NL SIAFEM Jah Existente

        private string gerarEstimuloConsultaMonitoramentoSIAF(string strChaveConsulta)
        {
            string estimuloWsConsultaMonitoramentoSIAF = null;


            estimuloWsConsultaMonitoramentoSIAF = GeradorEstimuloSIAF.SiafMonitora(strChaveConsulta);
            return estimuloWsConsultaMonitoramentoSIAF;
        }
        private string obterNLSiafemDoMonitoramento(string msgRetornoWsSIAF)
        {
            string strXmlRetornoPattern = "/*/*/Doc_Retorno/*/*/*/";
            string nlLiquidacao = XmlUtil.getXmlValue(msgRetornoWsSIAF, String.Format("{0}{1}", strXmlRetornoPattern, "retorno"));



            return nlLiquidacao;
        }


        private IList<Tuple<string, IList<string>>> obterDadosNLsMovimentacao(AssetMovements movimentacaoPatrimonial, Enum @TipoNotaSIAFEM, bool retornaNLEstorno = false)
        {
            IList<Tuple<string, IList<string>>> relacaoNLsMovimentacaoMaterial = null;
            TransactionScopeOption opcaoConsulta = TransactionScopeOption.Suppress;

            if (movimentacaoPatrimonial.IsNotNull())
            {
                using (TransactionScope ts = new TransactionScope(opcaoConsulta, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    try
                    {
                        int movPatrimonialId = movimentacaoPatrimonial.Id;
                        var _tipoNotaSIAFEM = (TipoNotaSIAF)@TipoNotaSIAFEM;
                        TipoNotaSIAF[] tiposNLsPatrimoniais = null;
                        string nomeTipoNL = null;
                        IList<string> listaNLsSIAFEM = null;
                        relacaoNLsMovimentacaoMaterial = new List<Tuple<string, IList<string>>>();
                        switch (_tipoNotaSIAFEM)
                        {
                            case TipoNotaSIAF.NL_Liquidacao:
                            case TipoNotaSIAF.NL_Depreciacao:
                            case TipoNotaSIAF.NL_Reclassificacao: { tiposNLsPatrimoniais = new TipoNotaSIAF[] { _tipoNotaSIAFEM }; } break;
                            case TipoNotaSIAF.Desconhecido:
                            default: { tiposNLsPatrimoniais = new TipoNotaSIAF[] { TipoNotaSIAF.NL_Liquidacao, TipoNotaSIAF.NL_Depreciacao, TipoNotaSIAF.NL_Reclassificacao }; } break;
                        }

                        foreach (var tipoNLPatrimonial in tiposNLsPatrimoniais)
                        {
                            listaNLsSIAFEM = obterNLsMovimentacao(movimentacaoPatrimonial, tipoNLPatrimonial, retornaNLEstorno);
                            if (listaNLsSIAFEM.HasElements())
                            {
                                nomeTipoNL = GeralEnum.GetEnumDescription((Enum)tipoNLPatrimonial);
                                relacaoNLsMovimentacaoMaterial.Add(Tuple.Create(nomeTipoNL, listaNLsSIAFEM));
                            }
                        }
                    }
                    catch (Exception excErroGravacao)
                    {
                        throw excErroGravacao;
                    }

                    ts.Complete();
                }
            }

            return relacaoNLsMovimentacaoMaterial;
        }
        private IList<string> obterNLsMovimentacao(AssetMovements movimentacaoPatrimonial, TipoNotaSIAF @TipoNotaSIAFEM, bool retornaNLEstorno = false, bool usaTransacao = false)
        {
            List<string> notasLancamentoMovimentacao = null;

            IQueryable<AssetMovements> qryConsulta = null;
            SAMContext contextoCamadaDados = null;
            Expression<Func<AssetMovements, bool>> expWhere = null;
            Expression<Func<AssetMovements, string>> expCampoInfoNL = null;
            var _tipoNotaSIAFEM = (TipoNotaSIAF)@TipoNotaSIAFEM;


            //filtro básico
            expWhere = (_movimentacao => _movimentacao.Id == movimentacaoPatrimonial.Id);

            //Se NL SIAFEM
            if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Liquidacao)
            {
                if (!retornaNLEstorno)
                    expCampoInfoNL = (_movimentacao => _movimentacao.NotaLancamento);
                else
                    expCampoInfoNL = (_movimentacao => _movimentacao.NotaLancamentoEstorno);
            }
            //Se NL DEPRECIACAO SIAFEM
            else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Depreciacao)
            {
                if (!retornaNLEstorno)
                    expCampoInfoNL = (_movimentacao => _movimentacao.NotaLancamentoDepreciacao);
                else
                    expCampoInfoNL = (_movimentacao => _movimentacao.NotaLancamentoDepreciacaoEstorno);
            }
            //Se NL RECLASSIFICACAO SIAFEM
            else if (_tipoNotaSIAFEM == GeralEnum.TipoNotaSIAF.NL_Reclassificacao)
            {
                if (!retornaNLEstorno)
                    expCampoInfoNL = (_movimentacao => _movimentacao.NotaLancamentoReclassificacao);
                else
                    expCampoInfoNL = (_movimentacao => _movimentacao.NotaLancamentoReclassificacaoEstorno);
            }

            contextoCamadaDados = new SAMContext();
            BaseController.InitDisconnectContext(ref contextoCamadaDados);

            qryConsulta = contextoCamadaDados.AssetMovements
                                             .Select(movPatrimonial => movPatrimonial)
                                             .AsQueryable();

            notasLancamentoMovimentacao = qryConsulta.Where(expWhere)
                                                     .Select(expCampoInfoNL)
                                                     .ToList();

            notasLancamentoMovimentacao = notasLancamentoMovimentacao.Distinct()
                                                                     .ToList();

            notasLancamentoMovimentacao.Remove((string)null);
            notasLancamentoMovimentacao.Remove(string.Empty);
            return notasLancamentoMovimentacao;
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
        private string obterValorTotalPendenciaNL__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            string valorPendenciaNL = null;

            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    var registroAuditoriaIntegracao = contextoCamadaDados.AuditoriaIntegracoes
                                                                         .Where(auditoriaIntegracao => auditoriaIntegracao.Id == auditoriaIntegracaoId)
                                                                         .FirstOrDefault();

                    if (registroAuditoriaIntegracao.IsNotNull())
                    {
                        if (registroAuditoriaIntegracao.ValorTotal.GetValueOrDefault() > 0.00m)
                        {
                            var cultureInfo = new CultureInfo("pt-BR");
                            valorPendenciaNL = String.Format("{0:C}", registroAuditoriaIntegracao.ValorTotal);
                        }
                        else
                        {
                            valorPendenciaNL = obterValorTotalPendenciaNL__ViaXML(registroAuditoriaIntegracao.MsgEstimuloWS);
                        }
                    }
                }
            }


            return (valorPendenciaNL ?? string.Empty);
        }
        private string obterValorTotalPendenciaNL__ViaXML(string msgEnvioWS)
        {
            string valorPendenciaNL = null;


            valorPendenciaNL = obterValorTagXmlEnvioContabilizaSP(msgEnvioWS, "ValorTotal").Trim();
            return (valorPendenciaNL ?? string.Empty);
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

        //REFATORADO
        private bool VerificaSeExisteNLNoServicoMonitoramentoSIAFEMEAtualizaMovimentacoes(string loginSiafemUsuario, string senhaSiafemUsuario, NotaLancamentoPendenteSIAFEM notaLancamentoPendente, out string mensagemErroSIAFEM)
        {
            #region Variaveis
            bool obteveNLComSIAFMonitora = false;
            string estimuloWsMonitoramentoSIAF = null;
            string anoBasePagamento = null;
            int managerUnitId = 0;
            string ugeGestora = null;
            string strRetornoWS = null;
            string nlRetornadaContabilizaSP = null;
            string strChaveConsulta = null;
            string documentoSAM = null;
            IList<int> relacaoMovimentacoesPatrimoniaisIds = null;
            string tipoLancamento = null;
            AuditoriaIntegracao auditoriaIntegracaoRelacionadaPendenciaNL = null;
            ProcessadorServicoSIAF procWsSiafem = null;
            IList<AssetMovements> listaMovimentacoes = null;
            TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.NL_Liquidacao;
            string hdrMensagemErroSIAF = null;
            bool ehEstorno = false;
            #endregion Variaveis



            mensagemErroSIAFEM = string.Empty;
            try
            {
                if (notaLancamentoPendente.IsNotNull())
                {
                    contextoCamadaDados = new SAMContext();
                    relacaoMovimentacoesPatrimoniaisIds = null;
                    //procurar linhas na tabela de amarracao (pendencias geradas pelo novo metodo de interacao com usuario, com amarracao Asset/AssetMovements/AuditoriaIntegracao)
                    if (String.IsNullOrWhiteSpace(notaLancamentoPendente.AssetMovementIds))
                    {
                        var listadeRegistrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                             .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == notaLancamentoPendente.AuditoriaIntegracaoId)
                                                                             .ToList();
                        if (listadeRegistrosDeAmarracao.HasElements())
                            relacaoMovimentacoesPatrimoniaisIds = listadeRegistrosDeAmarracao.Select(_registroDeAmarracao => _registroDeAmarracao.AssetMovementsId).ToList();
                    }
                    //senao utiliza o metodo antigo (parsing da linha string com Id's separados por espcos em branco
                    else
                    {
                        relacaoMovimentacoesPatrimoniaisIds = notaLancamentoPendente.AssetMovementIds.BreakLine().ToList().Select(movPatrimonialId => Int32.Parse(movPatrimonialId)).ToList();
                    }


                    if (relacaoMovimentacoesPatrimoniaisIds.HasElements())
                    {
                        listaMovimentacoes = contextoCamadaDados.AssetMovements.Where(movPatrimonial => relacaoMovimentacoesPatrimoniaisIds.Contains(movPatrimonial.Id)).ToList();
                        auditoriaIntegracaoRelacionadaPendenciaNL = obterEntidadeAuditoriaIntegracao(notaLancamentoPendente.AuditoriaIntegracaoId);
                        if (listaMovimentacoes.HasElements() && auditoriaIntegracaoRelacionadaPendenciaNL.IsNotNull())
                        {
                            hdrMensagemErroSIAF = "Erro ao gerar obter NL SIAFEM do WS de monitoramento, para Movimentação SAM";

                            documentoSAM = auditoriaIntegracaoRelacionadaPendenciaNL.NotaFiscal;
                            ugeGestora = auditoriaIntegracaoRelacionadaPendenciaNL.UgeOrigem;
                            managerUnitId = auditoriaIntegracaoRelacionadaPendenciaNL.ManagerUnitId;
                            anoBasePagamento = auditoriaIntegracaoRelacionadaPendenciaNL.Data.GetValueOrDefault().Year.ToString();
                            tipoLancamento = auditoriaIntegracaoRelacionadaPendenciaNL.NLEstorno;
                            strChaveConsulta = auditoriaIntegracaoRelacionadaPendenciaNL.DocumentoId;
                            ehEstorno = (auditoriaIntegracaoRelacionadaPendenciaNL.NLEstorno.ToUpperInvariant() == "S");


                            tipoNotaSIAFEM = this.obterTipoNotaSIAFEM__AuditoriaIntegracao(notaLancamentoPendente.AuditoriaIntegracaoId);
                            estimuloWsMonitoramentoSIAF = gerarEstimuloConsultaMonitoramentoSIAF(strChaveConsulta);


                            if (!String.IsNullOrWhiteSpace(estimuloWsMonitoramentoSIAF))
                            {
                                procWsSiafem = new ProcessadorServicoSIAF();
                                procWsSiafem.SistemaSIAF = GeralEnum.SistemaSIAF.SIAFEM;
                                procWsSiafem.ConsumirWS(anoBasePagamento, ugeGestora, estimuloWsMonitoramentoSIAF, true, true);

                                if (!procWsSiafem.ErroProcessamentoWs)
                                {
                                    strRetornoWS = procWsSiafem.RetornoWsSIAF;
                                    nlRetornadaContabilizaSP = obterNLSiafemDoMonitoramento(strRetornoWS);


                                    obteveNLComSIAFMonitora = this.VinculacaoNLContabilizaSP(listaMovimentacoes, tipoNotaSIAFEM, nlRetornadaContabilizaSP, ehEstorno);
                                    obteveNLComSIAFMonitora |= (!String.IsNullOrWhiteSpace(nlRetornadaContabilizaSP));
                                    this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoRelacionadaPendenciaNL.Id);

                                    auditoriaIntegracaoRelacionadaPendenciaNL.MsgErro = null;
                                    auditoriaIntegracaoRelacionadaPendenciaNL.DataRetorno = DateTime.Now;
                                    auditoriaIntegracaoRelacionadaPendenciaNL.MsgRetornoWS = procWsSiafem.RetornoWsSIAF;
                                    auditoriaIntegracaoRelacionadaPendenciaNL.NotaLancamento = nlRetornadaContabilizaSP;
                                    this.atualizaAuditoriaIntegracao(auditoriaIntegracaoRelacionadaPendenciaNL); //atualizacao dados registro AuditoriaIntegracao

                                    notaLancamentoPendente.StatusPendencia = 0; //inativacao registro pendencia NL SIAFEM
                                    this.atualizaNotaLancamentoPendente(notaLancamentoPendente); //atualizacao dados pendencia
                                }
                                else
                                {
                                    mensagemErroSIAFEM = procWsSiafem.ErroRetornoWs;

                                    //Auditoria Integracao SIAFEM
                                    notaLancamentoPendente.DataHoraReenvioMsgWS = auditoriaIntegracaoRelacionadaPendenciaNL.DataEnvio;
                                    notaLancamentoPendente.ErroProcessamentoMsgWS = mensagemErroSIAFEM;
                                    this.atualizaNotaLancamentoPendente(notaLancamentoPendente);  //inativacao registro pendencia NL SIAFEM
                                }
                            }
                        }
                    }
                    else if (relacaoMovimentacoesPatrimoniaisIds.HasElements())
                    {
                        return false;
                    }
                }
            }
            catch (InvalidOperationException excErroGeracaoEstimuloWsSIAF)
            {
                var erroProcessamento = String.Format("Operação inválida em processamento de dados com servidores SEFAZ [{0}]", excErroGeracaoEstimuloWsSIAF.Message);
                excErroGeracaoEstimuloWsSIAF = new InvalidOperationException(erroProcessamento, excErroGeracaoEstimuloWsSIAF);

                this.RegistraNotaLancamentoPendenciaPorException(listaMovimentacoes, excErroGeracaoEstimuloWsSIAF, auditoriaIntegracaoRelacionadaPendenciaNL, tipoNotaSIAFEM);
                //throw new Exception(erroProcessamento);
            }
            catch (TimeoutException excTimeoutConexao)
            {
                var erroProcessamento = String.Format("Timeout em conexão com servidores SEFAZ [{0}]", excTimeoutConexao.Message);
                excTimeoutConexao = new TimeoutException(erroProcessamento, excTimeoutConexao);
                this.RegistraNotaLancamentoPendenciaPorException(listaMovimentacoes, excTimeoutConexao, auditoriaIntegracaoRelacionadaPendenciaNL, tipoNotaSIAFEM);

                //throw new Exception(erroProcessamento);
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                var complementoMsgErro = String.Format("SIAFEM|{0}", procWsSiafem.ErroRetornoWs);
                var msgErro = String.Format(@"{0} {1}\n{2}", hdrMensagemErroSIAF, documentoSAM, complementoMsgErro);
                excErroProcessamentoWsSIAF = new Exception(msgErro, excErroProcessamentoWsSIAF);

                this.RegistraNotaLancamentoPendenciaPorException(listaMovimentacoes, excErroProcessamentoWsSIAF, auditoriaIntegracaoRelacionadaPendenciaNL, tipoNotaSIAFEM);
            }

            return obteveNLComSIAFMonitora;
        }

        private bool VerificaSeExisteNLNoServicoMonitoramentoSIAFEMEAtualizaRegistroFechamentoMensalIntegrado(string loginSiafemUsuario, string senhaSiafemUsuario, NotaLancamentoPendenteSIAFEM notaLancamentoPendente, out string mensagemErroSIAFEM)
        {
            #region Variaveis
            bool obteveNLComSIAFMonitora = false;
            string estimuloWsMonitoramentoSIAF = null;
            string anoBasePagamento = null;
            int managerUnitId = 0;
            string ugeGestora = null;
            string strRetornoWS = null;
            string nlRetornadaContabilizaSP = null;
            string strChaveConsulta = null;
            string documentoSAM = null;
            string tipoLancamento = null;
            AuditoriaIntegracao auditoriaIntegracaoRelacionadaPendenciaNL = null;
            ProcessadorServicoSIAF procWsSiafem = null;
            TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.NL_Depreciacao;
            string hdrMensagemErroSIAF = null;
            bool ehEstorno = false;
            #endregion Variaveis



            mensagemErroSIAFEM = string.Empty;
            try
            {
                if (notaLancamentoPendente.IsNotNull())
                {
                    contextoCamadaDados = new SAMContext();


                    if (true)//if (relacaoMovimentacoesPatrimoniaisIds.HasElements())
                    {
                        auditoriaIntegracaoRelacionadaPendenciaNL = obterEntidadeAuditoriaIntegracao(notaLancamentoPendente.AuditoriaIntegracaoId);
                        {
                            hdrMensagemErroSIAF = "Erro ao gerar obter NL SIAFEM do WS de monitoramento, para Movimentação SAM";

                            documentoSAM = auditoriaIntegracaoRelacionadaPendenciaNL.NotaFiscal;
                            ugeGestora = auditoriaIntegracaoRelacionadaPendenciaNL.UgeOrigem;
                            managerUnitId = auditoriaIntegracaoRelacionadaPendenciaNL.ManagerUnitId;
                            anoBasePagamento = auditoriaIntegracaoRelacionadaPendenciaNL.Data.GetValueOrDefault().Year.ToString();
                            tipoLancamento = auditoriaIntegracaoRelacionadaPendenciaNL.NLEstorno;
                            strChaveConsulta = auditoriaIntegracaoRelacionadaPendenciaNL.DocumentoId;
                            ehEstorno = (auditoriaIntegracaoRelacionadaPendenciaNL.NLEstorno.ToUpperInvariant() == "S");


                            tipoNotaSIAFEM = this.obterTipoNotaSIAFEM__AuditoriaIntegracao(notaLancamentoPendente.AuditoriaIntegracaoId);
                            estimuloWsMonitoramentoSIAF = gerarEstimuloConsultaMonitoramentoSIAF(strChaveConsulta);


                            if (!String.IsNullOrWhiteSpace(estimuloWsMonitoramentoSIAF))
                            {
                                procWsSiafem = new ProcessadorServicoSIAF();
                                procWsSiafem.SistemaSIAF = GeralEnum.SistemaSIAF.SIAFEM;
                                procWsSiafem.ConsumirWS(anoBasePagamento, ugeGestora, estimuloWsMonitoramentoSIAF, true, true);


                                if (!procWsSiafem.ErroProcessamentoWs)
                                {
                                    strRetornoWS = procWsSiafem.RetornoWsSIAF;
                                    nlRetornadaContabilizaSP = obterNLSiafemDoMonitoramento(strRetornoWS);


                                    //atualiza linha na tabela 'AccountingClosings' (ou 'AccountingClosings' se for o caso de reabertura; estorno) com a NL SIAFEM retornada
                                    if (!ehEstorno)
                                        this.GravaNLGerada(documentoSAM, nlRetornadaContabilizaSP);
                                    else
                                        this.GravaNLEstornadaGerada(documentoSAM, nlRetornadaContabilizaSP);


                                    obteveNLComSIAFMonitora |= (!String.IsNullOrWhiteSpace(nlRetornadaContabilizaSP));
                                    this.inativaNotaLancamentoPendencia__AuditoriaIntegracao(notaLancamentoPendente.AuditoriaIntegracaoId);

                                    auditoriaIntegracaoRelacionadaPendenciaNL.MsgErro = null;
                                    auditoriaIntegracaoRelacionadaPendenciaNL.DataRetorno = DateTime.Now;
                                    auditoriaIntegracaoRelacionadaPendenciaNL.MsgRetornoWS = procWsSiafem.RetornoWsSIAF;
                                    auditoriaIntegracaoRelacionadaPendenciaNL.NotaLancamento = nlRetornadaContabilizaSP;
                                    this.atualizaAuditoriaIntegracao(auditoriaIntegracaoRelacionadaPendenciaNL); //atualizacao dados registro AuditoriaIntegracao

                                    notaLancamentoPendente.StatusPendencia = 0; //inativacao registro pendencia NL SIAFEM
                                    this.atualizaNotaLancamentoPendente(notaLancamentoPendente); //atualizacao dados pendencia

                                    //this.DeleteRegistroNaAccountingClosings(documentoSAM, nlRetornadaContabilizaSP);
                                }
                                else
                                {
                                    mensagemErroSIAFEM = procWsSiafem.ErroRetornoWs;

                                    //Auditoria Integracao SIAFEM
                                    notaLancamentoPendente.DataHoraReenvioMsgWS = auditoriaIntegracaoRelacionadaPendenciaNL.DataEnvio;
                                    notaLancamentoPendente.ErroProcessamentoMsgWS = mensagemErroSIAFEM;
                                    this.atualizaNotaLancamentoPendente(notaLancamentoPendente);  //inativacao registro pendencia NL SIAFEM
                                }
                            }
                        }
                    }
                    else //if (relacaoMovimentacoesPatrimoniaisIds.HasElements())
                    {
                        return false;
                    }
                }
            }
            catch (InvalidOperationException excErroGeracaoEstimuloWsSIAF)
            {
                var erroProcessamento = String.Format("Operação inválida em processamento de dados com servidores SEFAZ [{0}]", excErroGeracaoEstimuloWsSIAF.Message);
                excErroGeracaoEstimuloWsSIAF = new InvalidOperationException(erroProcessamento, excErroGeracaoEstimuloWsSIAF);

                this.registraNotaLancamentoPendenciaPorExceptionFechamentoMensalIntegrado(excErroGeracaoEstimuloWsSIAF, auditoriaIntegracaoRelacionadaPendenciaNL, tipoNotaSIAFEM);
                //throw new Exception(erroProcessamento);
            }
            catch (TimeoutException excTimeoutConexao)
            {
                var erroProcessamento = String.Format("Timeout em conexão com servidores SEFAZ [{0}]", excTimeoutConexao.Message);
                excTimeoutConexao = new TimeoutException(erroProcessamento, excTimeoutConexao);
                this.registraNotaLancamentoPendenciaPorExceptionFechamentoMensalIntegrado(excTimeoutConexao, auditoriaIntegracaoRelacionadaPendenciaNL, tipoNotaSIAFEM);

                //throw new Exception(erroProcessamento);
            }
            catch (Exception excErroProcessamentoWsSIAF)
            {
                var complementoMsgErro = String.Format("SIAFEM|{0}", procWsSiafem.ErroRetornoWs);
                var msgErro = String.Format(@"{0} {1}\n{2}", hdrMensagemErroSIAF, documentoSAM, complementoMsgErro);
                excErroProcessamentoWsSIAF = new Exception(msgErro, excErroProcessamentoWsSIAF);

                this.registraNotaLancamentoPendenciaPorExceptionFechamentoMensalIntegrado(excErroProcessamentoWsSIAF, auditoriaIntegracaoRelacionadaPendenciaNL, tipoNotaSIAFEM);
            }

            return obteveNLComSIAFMonitora;
        }

        private bool pendenciaNLSiafemEhDeMovimentacaoPatrimonial(int notaLancamentoPendenteId)
        {
            bool pendenciaNLEhDeMovimentacaoPatrimonial = false;
            string[] tiposMovimentoContabilizaSP = null;
            string[] estoqueDestino_FechamentoMensalIntegrado = null;



            tiposMovimentoContabilizaSP = new string[] { "ENTRADA", "SAIDA", "SAÍDA", "DEPRECIAÇÃO", "DEPRECIACAO", "RECLASSIFICAÇÃO", "RECLASSIFICACAO" };
            estoqueDestino_FechamentoMensalIntegrado = new string[] { "DESPESA DE DEPRECIAÇÃO DE BENS MÓVEIS (VARIAÇÃO)", "DESPESA DE DEPRECIACAO DE BENS MOVEIS (VARIACAO)" };

            if (notaLancamentoPendenteId > 0)
            {
                AuditoriaIntegracao auditoriaIntegracaoVinculada = null;
                NotaLancamentoPendenteSIAFEM pendenciaNL = null;

                using (var contextoCamadaDados = new SAMContext())
                {
                    pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                     .Where(_pendenciaNL => _pendenciaNL.Id == notaLancamentoPendenteId)
                                                     .FirstOrDefault();

                    if (pendenciaNL.IsNotNull())
                    {
                        auditoriaIntegracaoVinculada = obterEntidadeAuditoriaIntegracao(pendenciaNL.AuditoriaIntegracaoId);
                        if (auditoriaIntegracaoVinculada.IsNotNull())
                        {
                            pendenciaNLEhDeMovimentacaoPatrimonial = tiposMovimentoContabilizaSP.Contains(auditoriaIntegracaoVinculada.TipoMovimento.ToUpperInvariant());
                            pendenciaNLEhDeMovimentacaoPatrimonial &= !estoqueDestino_FechamentoMensalIntegrado.Contains(auditoriaIntegracaoVinculada.EstoqueDestino);
                        }
                    }
                }
            }


            return pendenciaNLEhDeMovimentacaoPatrimonial;
        }

        private bool pendenciaNLSiafemEhDeFechamentoMensalIntegrado(int notaLancamentoPendenteId)
        {
            bool pendenciaNLEhDeFechamentoMensalIntegrado = false;
            string[] tipoMovimentoContabilizaSP_Depreciacao = null;
            string[] estoqueDestino_FechamentoMensalIntegrado = null;



            tipoMovimentoContabilizaSP_Depreciacao = new string[] { "DEPRECIAÇÃO", "DEPRECIACAO" };
            estoqueDestino_FechamentoMensalIntegrado = new string[] { "DESPESA DE DEPRECIAÇÃO DE BENS MÓVEIS (VARIAÇÃO)", "DESPESA DE DEPRECIACAO DE BENS MOVEIS (VARIACAO)" };

            if (notaLancamentoPendenteId > 0)
            {
                AuditoriaIntegracao auditoriaIntegracaoVinculada = null;
                NotaLancamentoPendenteSIAFEM pendenciaNL = null;

                using (var contextoCamadaDados = new SAMContext())
                {
                    pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                     .Where(_pendenciaNL => _pendenciaNL.Id == notaLancamentoPendenteId)
                                                     .FirstOrDefault();

                    if (pendenciaNL.IsNotNull())
                    {
                        auditoriaIntegracaoVinculada = obterEntidadeAuditoriaIntegracao(pendenciaNL.AuditoriaIntegracaoId);
                        if (auditoriaIntegracaoVinculada.IsNotNull())
                        {
                            pendenciaNLEhDeFechamentoMensalIntegrado = tipoMovimentoContabilizaSP_Depreciacao.Contains(auditoriaIntegracaoVinculada.TipoMovimento.ToUpperInvariant());
                            pendenciaNLEhDeFechamentoMensalIntegrado &= estoqueDestino_FechamentoMensalIntegrado.Contains(auditoriaIntegracaoVinculada.EstoqueDestino);
                        }
                    }
                }
            }


            return pendenciaNLEhDeFechamentoMensalIntegrado;
        }
        #endregion Verificacao NL SIAFEM Jah Existente

        internal bool ehEstorno(AssetMovements movimentacaoPatrimonial)
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
        public IQueryable<PendenciaNotaLancamentoSIAFEMViewModel> Pesquisa(IQueryable<PendenciaNotaLancamentoSIAFEMViewModel> lista, string textoPesquisa)
        {

            int paraValidacaoInteiro;
            long paraValidacaoLong;
            if (int.TryParse(textoPesquisa, out paraValidacaoInteiro))  //Inteiros pesquisam Documento e Data de Envio
            {
                lista = lista.Where(l => l.NumeroDocumentoSAM.Contains(textoPesquisa) || l.DataEnvioSIAFEM.Contains(textoPesquisa));
            }
            else if (long.TryParse(textoPesquisa, out paraValidacaoLong)) //Inteiros longos pesquisam Documento
            {
                lista = lista.Where(l => l.NumeroDocumentoSAM.Contains(textoPesquisa));
            }
            else //Alfanuméricos pesquisa Tipo Nota, ErroSIAFEM e Data de Envio(formatada) 
            {
                var tipos = new int[Enum.GetValues(typeof(TipoNotaSIAF)).Length];
                int contTipos = 0;

                foreach (int str in Enum.GetValues(typeof(TipoNotaSIAF)))
                {
                    var attr = typeof(TipoNotaSIAF).GetField(((TipoNotaSIAF)str).ToString())
                                               .GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attr.Length > 0)
                    {
                        if ((attr[0] as DescriptionAttribute).Description.Contains(textoPesquisa))
                        {
                            tipos[contTipos] = str;
                        }
                        else
                        {
                            tipos[contTipos] = -1;
                        }
                    }

                    contTipos++;
                }

                if (textoPesquisa.Contains("/"))
                    lista = lista.Where(l => l.ErroSIAFEM.Contains(textoPesquisa) || l.DataEnvioSIAFEM.Contains(textoPesquisa) || tipos.Contains((int)l.TipoNotaNumero));
                else
                {
                    lista = lista.Where(l => l.ErroSIAFEM.Contains(textoPesquisa) || l.DataEnvioSIAFEM.Contains(textoPesquisa) || tipos.Contains((int)l.TipoNotaNumero));
                }
            }

            return lista;
        }


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
        private bool criaOuRegeraNotaLancamentoPendenciaParaMovimentacaoVinculada(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            int numeroRegistrosManipulados = 0;
            int? notaLancamentoPendenteSIAFEMId = null;
            int? assetMovements_notaLancamentoPendenteSIAFEMId = null;
            int auditoriaIntegracaoVinculadaAPendenciaNLId = 0;
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
            List<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> registrosDeAmarracao = null;
            int[] idsAuditoriaIntegracao = null;




            if (listaMovimentacoesPatrimoniais.HasElements())
            {
                int[] idsMovPatrimoniais = listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray();
                notaLancamentoPendenteSIAFEMId = listaMovimentacoesPatrimoniais.FirstOrDefault().NotaLancamentoPendenteSIAFEMId;
                if (notaLancamentoPendenteSIAFEMId.HasValue && notaLancamentoPendenteSIAFEMId.Value > 0)
                {
                    using (var _contextoCamadaDados = new SAMContext())
                    {
                        listaNLsPendentesAtivasParaCancelamento = new List<NotaLancamentoPendenteSIAFEM>();
                        _notaLancamentoPendenteSIAFEMParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(notaLancamentoPendenteSIAFEM => notaLancamentoPendenteSIAFEM.Id == notaLancamentoPendenteSIAFEMId).FirstOrDefault();
                        if (_notaLancamentoPendenteSIAFEMParaCancelamento.IsNotNull())
                        {
                            //obter AuditoriaIntegracaoId da pendencia atual vinculada ao BP para poder obter ID's de todas as pendencias
                            auditoriaIntegracaoVinculadaAPendenciaNLId = _notaLancamentoPendenteSIAFEMParaCancelamento.AuditoriaIntegracaoId;
                            registrosDeAmarracao = _contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                       .Where(registroDeAmarracao => idsMovPatrimoniais.Contains(registroDeAmarracao.AssetMovementsId))
                                                                       .ToList();


                            idsAuditoriaIntegracao = registrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AuditoriaIntegracaoId).ToArray();

                            //consulta padrao para trazer todas as pendencias ativas para um determinado lancamento no SIAFEM
                            //expWhere = (nlPendente => nlPendente.AssetMovementIds == _notaLancamentoPendenteSIAFEMParaCancelamento.AssetMovementIds && nlPendente.StatusPendencia == 1);
                            expWhere = (nlPendente => idsAuditoriaIntegracao.Contains(nlPendente.AuditoriaIntegracaoId) && nlPendente.StatusPendencia == 1);


                            //lista de movimentacoes que terao o campo 'NotaLancamentoPendenteSIAFEMId' atualizado
                            var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();

                            //listaNLsPendentesAtivasParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(expWhere).ToList();
                            if (tipoNotaSIAFEM != TipoNotaSIAF.Desconhecido)
                            {
                                //expWhereNLsMesmoTipo = (nlPendente => nlPendente.TipoNotaPendencia == (short)tipoNotaSIAFEM); //_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
                                expWhereNLsMesmoTipo = (nlPendente => idsAuditoriaIntegracao.Contains(nlPendente.AuditoriaIntegracaoId) && nlPendente.TipoNotaPendencia == (short)tipoNotaSIAFEM); //_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
                                expWhereNLsMesmoTipo = expWhere.And(expWhereNLsMesmoTipo);

                                //expWhereNLsOutrosTipos = (nlPendente => nlPendente.TipoNotaPendencia != (short)tipoNotaSIAFEM);//_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
                                expWhereNLsOutrosTipos = (nlPendente => idsAuditoriaIntegracao.Contains(nlPendente.AuditoriaIntegracaoId) && nlPendente.TipoNotaPendencia != (short)tipoNotaSIAFEM);//_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
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
        internal bool RegistraNotaLancamentoPendenciaPorException(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, Exception runtimeException, AuditoriaIntegracao registroAuditoria, TipoNotaSIAF tipoAgrupamentoNotaLancamento)
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
                //this.InativaNotaLancamentoPendenciaVinculada(listaMovimentacoesPatrimoniais);
                this.criaOuRegeraNotaLancamentoPendenciaParaMovimentacaoVinculada(listaMovimentacoesPatrimoniais);

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

        private bool criaOuRegeraNotaLancamentoPendenciaParaFechamentoMensalIntegrado(AuditoriaIntegracao registroAuditoriaIntegracao, TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido)
        {
            int numeroRegistrosManipulados = 0;
            int? notaLancamentoPendenteSIAFEMId = null;
            //int? assetMovements_notaLancamentoPendenteSIAFEMId = null;
            int auditoriaIntegracaoVinculadaAPendenciaNLId = 0;
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
            //List<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> registrosDeAmarracao = null;
            int[] idsPendenciaNLSiafem = null;




            if (registroAuditoriaIntegracao.IsNotNull())
            {
                var contextoCamadadados = new SAMContext();
                idsPendenciaNLSiafem = contextoCamadadados.NotaLancamentoPendenteSIAFEMs
                                                          .Where(pendenciaNL => pendenciaNL.AuditoriaIntegracaoId == registroAuditoriaIntegracao.Id)
                                                          .Select(pendenciaNL => pendenciaNL.Id)
                                                          .ToArray();

                //notaLancamentoPendenteSIAFEMId = (_notaLancamentoPendenteSIAFEM.IsNotNull() ? _notaLancamentoPendenteSIAFEM.Id : 0);
                //if (notaLancamentoPendenteSIAFEMId.HasValue && notaLancamentoPendenteSIAFEMId.Value > 0)
                if (idsPendenciaNLSiafem.HasElements())
                {
                    using (var _contextoCamadaDados = new SAMContext())
                    {
                        listaNLsPendentesAtivasParaCancelamento = new List<NotaLancamentoPendenteSIAFEM>();
                        _notaLancamentoPendenteSIAFEMParaCancelamento = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Where(notaLancamentoPendenteSIAFEM => idsPendenciaNLSiafem.Contains(notaLancamentoPendenteSIAFEM.Id)).FirstOrDefault();
                        if (_notaLancamentoPendenteSIAFEMParaCancelamento.IsNotNull())
                        {
                            //obter AuditoriaIntegracaoId da pendencia atual vinculada ao BP para poder obter ID's de todas as pendencias
                            auditoriaIntegracaoVinculadaAPendenciaNLId = registroAuditoriaIntegracao.Id;
                            expWhere = (nlPendente => idsPendenciaNLSiafem.Contains(nlPendente.Id) && nlPendente.StatusPendencia == 1);


                            if (tipoNotaSIAFEM != TipoNotaSIAF.Desconhecido)
                            {
                                expWhereNLsMesmoTipo = (nlPendente => idsPendenciaNLSiafem.Contains(nlPendente.Id) && nlPendente.TipoNotaPendencia == (short)tipoNotaSIAFEM); //_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
                                expWhereNLsMesmoTipo = expWhere.And(expWhereNLsMesmoTipo);

                                expWhereNLsOutrosTipos = (nlPendente => idsPendenciaNLSiafem.Contains(nlPendente.Id) && nlPendente.TipoNotaPendencia != (short)tipoNotaSIAFEM);//_notaLancamentoPendenteSIAFEMParaCancelamento.TipoNotaPendencia);
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
                        }

                        numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                    }
                }
            }

            gravacaoInativacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoInativacaoNotaLancamentoPendente;
        }
        internal bool registraNotaLancamentoPendenciaPorExceptionFechamentoMensalIntegrado(Exception runtimeException, AuditoriaIntegracao registroAuditoria, TipoNotaSIAF tipoAgrupamentoNotaLancamento)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            string numeroDocumentoSAM = null;
            string msgDetalhadaErro = null;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;




            var dadosErroException = (runtimeException.InnerException.IsNull() ? runtimeException.Message : String.Format("{0}; DetalheErro: {1}", runtimeException.Message, runtimeException.InnerException.Message));
            if (registroAuditoria.IsNotNull())
            {
                msgDetalhadaErro = String.Format("Erro ao gerar NL Contabiliza-SP para Movimentação Patrimonial {0}; DadosException: {1}", numeroDocumentoSAM, dadosErroException);
                pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
                                                                                {
                                                                                    RelatedAuditoriaIntegracao = registroAuditoria,
                                                                                    DataHoraEnvioMsgWS = registroAuditoria.DataEnvio,
                                                                                    ManagerUnitId = registroAuditoria.ManagerUnitId,
                                                                                    TipoNotaPendencia = (short)tipoAgrupamentoNotaLancamento,
                                                                                    NumeroDocumentoSAM = registroAuditoria.NotaFiscal,
                                                                                    StatusPendencia = 1,
                                                                                    ErroProcessamentoMsgWS = msgDetalhadaErro,
                                                                                };

                //Desvincula qqr. pendencia de NL vinculada ao registro de auditoria passado como prametro
                this.criaOuRegeraNotaLancamentoPendenciaParaFechamentoMensalIntegrado(registroAuditoria);
            }

            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }
        private bool registraAuditoriaIntegracaoParaFechamentoMensalIntegrado(AccountingClosing fechamentoContaContabil, AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            int numeroRegistrosManipulados = 0;
            bool retornoGravacaoAuditoriaIntegracao = false;



            using (var _contextoCamadaDados = new SAMContext())
            {
                if (registroAuditoriaIntegracao.Id == 0)
                    _contextoCamadaDados.AuditoriaIntegracoes.Attach(registroAuditoriaIntegracao);

                var _registroAuditoriaIntegracao = _contextoCamadaDados.AuditoriaIntegracoes.Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id).FirstOrDefault();
                var _fechamentoContaContabil = _contextoCamadaDados.AccountingClosings.Where(rowFechamentoContaContabil => rowFechamentoContaContabil.Id == fechamentoContaContabil.Id).FirstOrDefault();


                if (_registroAuditoriaIntegracao.IsNotNull() && _fechamentoContaContabil.IsNotNull())
                    _fechamentoContaContabil.AuditoriaIntegracaoId = _registroAuditoriaIntegracao.Id;

                numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
            }


            retornoGravacaoAuditoriaIntegracao = (numeroRegistrosManipulados > 0);
            return retornoGravacaoAuditoriaIntegracao;
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
        private bool atualizaAccountingClosing(AccountingClosing registroAccountingClosing)
        {
            int numeroRegistrosManipulados = 0;

            try
            {
                long registroAccountingClosingId = registroAccountingClosing.Id;
                if (registroAccountingClosingId > 0)
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        contextoCamadaDados.AccountingClosings.Attach(registroAccountingClosing);
                        contextoCamadaDados.Entry(registroAccountingClosing).State = EntityState.Modified;


                        numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    }
            }
            catch (Exception excErroRuntime)
            {
                throw excErroRuntime;
            }

            return (numeroRegistrosManipulados > 0);
        }

        #region Metodos Relacionados a AuditoriaIntegracao
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

        public IList<AssetMovements> ObterRelacaoMovimentacoesPatrimoniaisPorIds(IEnumerable<int> listaIdsMovimentacoesPatrimoniais)
        {
            IList<AssetMovements> listaMovimentacoesPatrimoniais = null;



            using (var _contextoCamadaDados = new SAMContext())
                listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => listaIdsMovimentacoesPatrimoniais.Contains(movPatrimonial.Id)).AsNoTracking().ToList();


            return listaMovimentacoesPatrimoniais;
        }
        #endregion Metodos Relacionados a AuditoriaIntegracao

        #region Metodos Importados de Outras Controller
        private void GravaNLGerada(string NotaFiscal, string NLGerada, int? auditoriaIntegracaoId = null)
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

            if (auditoriaIntegracaoId.GetValueOrDefault() > 0)
                query = string.Format("UPDATE [dbo].[AccountingClosing] SET GeneratedNL = '{0}' where AuditoriaIntegracaoId = {1}", NLGerada, auditoriaIntegracaoId.GetValueOrDefault());

            this.contextoCamadaDados.Database.ExecuteSqlCommand(query);
        }
        private void GravaNLEstornadaGerada(string NotaFiscal, string NLGerada, int? auditoriaIntegracaoId = null)
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

            string query = string.Empty;

            if (auditoriaIntegracaoId.GetValueOrDefault() > 0)
            {
                query = string.Format("UPDATE [dbo].[AccountingClosingExcluidos] SET GeneratedNL = '{0}' where AuditoriaIntegracaoId = {1}", NLGerada, auditoriaIntegracaoId.GetValueOrDefault());
                this.contextoCamadaDados.Database.ExecuteSqlCommand(query);
                query = string.Format("UPDATE [dbo].[AccountingClosing] SET GeneratedNL = '{0}' where AuditoriaIntegracaoId = {1}", NLGerada, auditoriaIntegracaoId.GetValueOrDefault());
                this.contextoCamadaDados.Database.ExecuteSqlCommand(query);
            }
            else
            {
                query = string.Format("UPDATE [dbo].[AccountingClosingExcluidos] SET NLEstorno = '{0}' where ManagerUnitCode = {1} AND DepreciationAccount = {2} AND ReferenceMonth = {3}",
                NLGerada, UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

                this.contextoCamadaDados.Database.ExecuteSqlCommand(query);

                query = string.Format("UPDATE [dbo].[AccountingClosing] SET GeneratedNL = '{0}' where ManagerUnitCode = {1} AND DepreciationAccount = {2} AND ReferenceMonth = {3} AND DepreciationAccount is not null AND ClosingId is not null",
                NLGerada, UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

                this.contextoCamadaDados.Database.ExecuteSqlCommand(query);
            }

        }

        private void DeleteRegistroNaAccountingClosings(string NotaFiscal, string NLGerada)
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

            string tiraDepreciation = string.Format("DELETE from [dbo].[DepreciationAccountingClosing] where [AccountingClosingId] IN (select Id from [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND DepreciationAccount = {1} AND ReferenceMonth = {2})",
            UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.contextoCamadaDados.Database.ExecuteSqlCommand(tiraDepreciation);

            string tiraDaAccountingClosing = string.Format("DELETE [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND DepreciationAccount = {1} AND ReferenceMonth = {2}",
            UGE, (ContaDepreciacao == null ? "null" : ContaDepreciacao), mesRef);

            this.contextoCamadaDados.Database.ExecuteSqlCommand(tiraDaAccountingClosing);

            string tiraDepreciationValorZero = string.Format("DELETE from [dbo].[DepreciationAccountingClosing] where [AccountingClosingId] IN (select Id from [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND ReferenceMonth = {1} AND DepreciationMonth = 0)",
            UGE, mesRef);

            this.contextoCamadaDados.Database.ExecuteSqlCommand(tiraDepreciationValorZero);

            string tiraDaAccountingClosingValorZero = string.Format("DELETE [dbo].[AccountingClosing] where ManagerUnitCode = {0} AND ReferenceMonth = {1} AND DepreciationMonth = 0",
            UGE, mesRef);

            this.contextoCamadaDados.Database.ExecuteSqlCommand(tiraDaAccountingClosingValorZero);
        }
        #endregion Metodos Importados de Outras Controller
    }
}