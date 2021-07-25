using Sam.Common.Util;
using Sam.Integracao.SIAF.Core;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;
using Sam.Integracao.SIAF.Mensagem.Constantes;
using Sam.Integracao.SIAF.Mensagem.Enum;
using TipoLancamentoSIAFEM = Sam.Common.Util.GeralEnum.TipoLancamentoSIAF;
using tipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using TipoMovimentacaoPatrimonial = Sam.Integracao.SIAF.Mensagem.Enum.EnumMovimentType;
using System.Linq.Expressions;
using LinqKit;
using System.Transactions;
using System.Globalization;

namespace SAM.Web.Controllers.IntegracaoContabilizaSP
{
    public static class IntegracaoSIAFEMHelper
    {
        public static IList<string> ObterDadosNLsBP(int bemPatrimonialId)
        {
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            IList<AssetMovements> historicoMovimentacoesBP = null;
            IList<AuditoriaIntegracao> registrosAuditoriaIntegracao = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> registrosDeAmarracao = null;
            IList<string> relacaoDadosNLsBemPatrimonial = new List<string>();

            string documentoSAM = null;
            string dataHoraEnvioXMLContabilizaSP = null;
            string tipoNL = null;
            string nlSIAFEM = null;
            string valorNL = null;
            string debug = null;

            if (bemPatrimonialId > 0)
            {
                try
                {
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        historicoMovimentacoesBP = contextoCamadaDados.AssetMovements
                                                                      .Where(_historicoMovimentacoesBP => _historicoMovimentacoesBP.AssetId == bemPatrimonialId)
                                                                      .OrderBy(_historicoMovimentacoesBP => _historicoMovimentacoesBP.Id)
                                                                      .ThenBy(_historicoMovimentacoesBP => _historicoMovimentacoesBP.Status)
                                                                      .AsNoTracking()
                                                                      .ToList();

                        registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                            .Where(_registroDeAmarracao => _registroDeAmarracao.AssetId == bemPatrimonialId)
                                                                            .AsNoTracking()
                                                                            .ToList();

                        registrosAuditoriaIntegracao = (from relacionamento in contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                  join auditoria in contextoCamadaDados.AuditoriaIntegracoes
                                                  on relacionamento.AuditoriaIntegracaoId equals auditoria.Id
                                                  where relacionamento.AssetId == bemPatrimonialId
                                                  select auditoria).ToList();

                        foreach (var movimentacaoBP in historicoMovimentacoesBP)
                        {
                            if (String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamento) && String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoEstorno) &&
                                String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoDepreciacao) && String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoDepreciacaoEstorno) &&
                                String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoReclassificacao) && String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoReclassificacaoEstorno))
                                continue;


                            documentoSAM = movimentacaoBP.NumberDoc;
                            dataHoraEnvioXMLContabilizaSP = "N/I";

                            #region NLs Registro
                            if (!String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoReclassificacao))
                            {
                                tipoNL = "NL Reclassificação";
                                nlSIAFEM = movimentacaoBP.NotaLancamentoReclassificacao;

                                registroAuditoriaIntegracao = (from r in registrosDeAmarracao
                                                               join a in registrosAuditoriaIntegracao
                                                               on r.AuditoriaIntegracaoId equals a.Id
                                                               where a.NotaLancamento == movimentacaoBP.NotaLancamentoReclassificacao
                                                               && r.AssetMovementsId == movimentacaoBP.Id
                                                               select a).FirstOrDefault();

                                if (registroAuditoriaIntegracao.IsNotNull())
                                {
                                    dataHoraEnvioXMLContabilizaSP = (registroAuditoriaIntegracao.DataRetorno ?? registroAuditoriaIntegracao.DataEnvio).ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                    valorNL = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", registroAuditoriaIntegracao.ValorTotal);
                                }

                                debug = String.Format("{0};{1};{2};{3};{4}", dataHoraEnvioXMLContabilizaSP, documentoSAM, tipoNL, nlSIAFEM, valorNL);
                                relacaoDadosNLsBemPatrimonial.Add(debug);
                            }

                            if (!String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamento))
                            {
                                tipoNL = "NL Liquidação";
                                nlSIAFEM = movimentacaoBP.NotaLancamento;

                                registroAuditoriaIntegracao = (from r in registrosDeAmarracao
                                                               join a in registrosAuditoriaIntegracao
                                                               on r.AuditoriaIntegracaoId equals a.Id
                                                               where a.NotaLancamento == movimentacaoBP.NotaLancamento
                                                               && r.AssetMovementsId == movimentacaoBP.Id
                                                               select a).FirstOrDefault();

                                if (registroAuditoriaIntegracao.IsNotNull())
                                {
                                    dataHoraEnvioXMLContabilizaSP = (registroAuditoriaIntegracao.DataRetorno ?? registroAuditoriaIntegracao.DataEnvio).ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                    valorNL = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", registroAuditoriaIntegracao.ValorTotal);
                                }

                                debug = String.Format("{0};{1};{2};{3};{4}", dataHoraEnvioXMLContabilizaSP, documentoSAM, tipoNL, nlSIAFEM, valorNL);
                                relacaoDadosNLsBemPatrimonial.Add(debug);
                            }

                            if (!String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoDepreciacao))
                            {
                                tipoNL = "NL Depreciação";
                                nlSIAFEM = movimentacaoBP.NotaLancamentoDepreciacao;

                                registroAuditoriaIntegracao = (from r in registrosDeAmarracao
                                                               join a in registrosAuditoriaIntegracao
                                                               on r.AuditoriaIntegracaoId equals a.Id
                                                               where a.NotaLancamento == movimentacaoBP.NotaLancamentoDepreciacao
                                                               && r.AssetMovementsId == movimentacaoBP.Id
                                                               select a).FirstOrDefault();

                                if (registroAuditoriaIntegracao.IsNotNull())
                                {
                                    dataHoraEnvioXMLContabilizaSP = (registroAuditoriaIntegracao.DataRetorno ?? registroAuditoriaIntegracao.DataEnvio).ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                    valorNL = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", registroAuditoriaIntegracao.ValorTotal);
                                }

                                debug = String.Format("{0};{1};{2};{3};{4}", dataHoraEnvioXMLContabilizaSP, documentoSAM, tipoNL, nlSIAFEM, valorNL);
                                relacaoDadosNLsBemPatrimonial.Add(debug);
                            }
                            #endregion NLs Registro

                            #region NLs de Estorno
                            if (!String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoReclassificacaoEstorno))
                            {
                                tipoNL = "NL Reclassificação";
                                nlSIAFEM = movimentacaoBP.NotaLancamentoReclassificacaoEstorno + " (estorno)";

                                registroAuditoriaIntegracao = (from r in registrosDeAmarracao
                                                               join a in registrosAuditoriaIntegracao
                                                               on r.AuditoriaIntegracaoId equals a.Id
                                                               where a.NotaLancamento == movimentacaoBP.NotaLancamentoReclassificacaoEstorno
                                                               && r.AssetMovementsId == movimentacaoBP.Id
                                                               select a).FirstOrDefault();

                                 if (registroAuditoriaIntegracao.IsNotNull())
                                 {
                                     dataHoraEnvioXMLContabilizaSP = (registroAuditoriaIntegracao.DataRetorno ?? registroAuditoriaIntegracao.DataEnvio).ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                    valorNL = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", registroAuditoriaIntegracao.ValorTotal);
                                 }

                                debug = String.Format("{0};{1};{2};{3};{4}", dataHoraEnvioXMLContabilizaSP, documentoSAM, tipoNL, nlSIAFEM, valorNL);
                                relacaoDadosNLsBemPatrimonial.Add(debug);
                            }

                            if (!String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoEstorno))
                            {
                                tipoNL = "NL Liquidação";
                                nlSIAFEM = movimentacaoBP.NotaLancamentoEstorno + "(estorno)";

                                registroAuditoriaIntegracao = (from r in registrosDeAmarracao
                                                               join a in registrosAuditoriaIntegracao
                                                               on r.AuditoriaIntegracaoId equals a.Id
                                                               where a.NotaLancamento == movimentacaoBP.NotaLancamentoEstorno
                                                               && r.AssetMovementsId == movimentacaoBP.Id
                                                               select a).FirstOrDefault();

                                if (registroAuditoriaIntegracao.IsNotNull())
                                {
                                    dataHoraEnvioXMLContabilizaSP = (registroAuditoriaIntegracao.DataRetorno ?? registroAuditoriaIntegracao.DataEnvio).ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                    valorNL = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", registroAuditoriaIntegracao.ValorTotal);
                                }

                                debug = String.Format("{0};{1};{2};{3};{4}", dataHoraEnvioXMLContabilizaSP, documentoSAM, tipoNL, nlSIAFEM, valorNL);
                                relacaoDadosNLsBemPatrimonial.Add(debug);
                            }

                            if (!String.IsNullOrWhiteSpace(movimentacaoBP.NotaLancamentoDepreciacaoEstorno))
                            {
                                tipoNL = "NL Depreciação";
                                nlSIAFEM = movimentacaoBP.NotaLancamentoDepreciacaoEstorno + " (estorno)";

                                registroAuditoriaIntegracao = (from r in registrosDeAmarracao
                                                               join a in registrosAuditoriaIntegracao
                                                               on r.AuditoriaIntegracaoId equals a.Id
                                                               where a.NotaLancamento == movimentacaoBP.NotaLancamentoDepreciacaoEstorno
                                                               && r.AssetMovementsId == movimentacaoBP.Id
                                                               select a).FirstOrDefault();

                                if (registroAuditoriaIntegracao.IsNotNull())
                                {
                                    dataHoraEnvioXMLContabilizaSP = (registroAuditoriaIntegracao.DataRetorno ?? registroAuditoriaIntegracao.DataEnvio).ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                    valorNL = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", registroAuditoriaIntegracao.ValorTotal);
                                }

                                debug = String.Format("{0};{1};{2};{3};{4}", dataHoraEnvioXMLContabilizaSP, documentoSAM, tipoNL, nlSIAFEM, valorNL);
                                relacaoDadosNLsBemPatrimonial.Add(debug);
                            }
                            #endregion NLs de Estorno
                        }
                    }
                }
                catch (Exception excErroGravacao)
                {
                    throw excErroGravacao;
                }
            }

            return relacaoDadosNLsBemPatrimonial;
        }
        private static IList<string> obterDadosNLsMovimentacao(int movimentacaoPatrimonialId, TipoNotaSIAFEM @TipoNotaSIAFEM, bool retornaNLEstorno = false, bool usaTransacao = false)
        {
            List<string> notasLancamentoMovimentacao = null;

            IQueryable<AssetMovements> qryConsulta = null;
            IQueryable<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> qryConsultatabelaAmarracao = null;

            SAMContext contextoCamadaDados = null;
            Expression<Func<AssetMovements, bool>> expWhere = null;
            Expression<Func<AssetMovements, string>> expCampoInfoNL = null;
            var _tipoNotaSIAFEM = (TipoNotaSIAFEM)@TipoNotaSIAFEM;


            //filtro básico
            expWhere = (_movimentacao => _movimentacao.Id == movimentacaoPatrimonialId);

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
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> registrosDeAmarracao = null;
            IList<int> listaAuditoriaIntegracaoIds = null;

            listaAuditoriaIntegracaoIds = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                             .Where(registroAmarracao => registroAmarracao.AssetMovementsId == movimentacaoPatrimonialId)
                                                             .Select(registroAmarracao => registroAmarracao.AuditoriaIntegracaoId)
                                                             .ToList();

            qryConsulta = contextoCamadaDados.AssetMovements
                                             .Where(expWhere)
                                             .AsQueryable();

            notasLancamentoMovimentacao = qryConsulta.Select(expCampoInfoNL)
                                                     .ToList();

            notasLancamentoMovimentacao = notasLancamentoMovimentacao.Distinct()
                                                                     .ToList();

            notasLancamentoMovimentacao.Remove((string)null);
            notasLancamentoMovimentacao.Remove(string.Empty);
            return notasLancamentoMovimentacao;
        }

        public static IList<Tuple<string, IList<string>>> ObterNLsMovimentacao(this AssetMovements movimentacaoPatrimonial, Enum @TipoNotaSIAFEM, bool retornaNLEstorno = false)
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
                        var _tipoNotaSIAFEM = (TipoNotaSIAFEM)@TipoNotaSIAFEM;
                        TipoNotaSIAFEM[] tiposNLsPatrimoniais = null;
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
        private static IList<string> obterNLsMovimentacao(this AssetMovements movimentacaoPatrimonial, TipoNotaSIAFEM @TipoNotaSIAFEM, bool retornaNLEstorno = false, bool usaTransacao = false)
        {
            List<string> notasLancamentoMovimentacao = null;

            IQueryable<AssetMovements> qryConsulta = null;
            SAMContext contextoCamadaDados = null;
            Expression<Func<AssetMovements, bool>> expWhere = null;
            Expression<Func<AssetMovements, string>> expCampoInfoNL = null;
            var _tipoNotaSIAFEM = (TipoNotaSIAFEM)@TipoNotaSIAFEM;


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
                                             .Where(expWhere)
                                             //.Select(movPatrimonial => movPatrimonial)
                                             .AsQueryable();

            notasLancamentoMovimentacao = qryConsulta//.Where(expWhere)
                                                     .Select(expCampoInfoNL)
                                                     .ToList();

            notasLancamentoMovimentacao = notasLancamentoMovimentacao.Distinct()
                                                                     .ToList();

            notasLancamentoMovimentacao.Remove((string)null);
            notasLancamentoMovimentacao.Remove(string.Empty);
            return notasLancamentoMovimentacao;
        }
    }

    public static class IntegracaoContabilizaSPExtensionMethods
    {
        static SAMContext contextoCamadaDados = null;
        
        public static bool PossuiContraPartidaContabil(this MovementType tipoMovimentacaoPatrimonial)
        {
            bool tipoMovimentacaoPossuiContrapartidaContabil = false;


            BaseController.InitDisconnectContext(ref contextoCamadaDados);
            if (tipoMovimentacaoPatrimonial.IsNotNull() && tipoMovimentacaoPatrimonial.Code > 0)
            {
                tipoMovimentacaoPossuiContrapartidaContabil = contextoCamadaDados.EventServicesContabilizaSPs
                                                                                 .Where(eventoContabilizaSP => (eventoContabilizaSP.TipoMovimento_SamPatrimonio == tipoMovimentacaoPatrimonial.Code.ToString()))
                                                                                 .ToList()
                                                                                 .HasElements();
            }

            return tipoMovimentacaoPossuiContrapartidaContabil;
        }
        public static bool TipoMovimentacaoPossuiContraPartidaContabil(int tipoMovimentacaoPatrimonialId)
        {
            bool tipoMovimentacaoPossuiContrapartidaContabil = false;
            MovementType tipoMovimentacaoPatrimonial = null;


            BaseController.InitDisconnectContext(ref contextoCamadaDados);
            tipoMovimentacaoPatrimonial = contextoCamadaDados.MovementTypes.Where(tipoMovPatrimonial => tipoMovPatrimonial.Id == tipoMovimentacaoPatrimonialId).FirstOrDefault();
            if (tipoMovimentacaoPatrimonial.IsNotNull() && tipoMovimentacaoPatrimonial.Code > 0)
            {
                tipoMovimentacaoPossuiContrapartidaContabil = contextoCamadaDados.EventServicesContabilizaSPs
                                                                                 //.Where(eventoContabilizaSP => eventoContabilizaSP.InputOutputReclassificationDepreciationTypeCode == tipoMovimentacaoPatrimonial.Code)
                                                                                 .Where(eventoContabilizaSP => (eventoContabilizaSP.TipoMovimento_SamPatrimonio == tipoMovimentacaoPatrimonial.Code.ToString()))
                                                                                 .ToList()
                                                                                 .HasElements();
            }

            return tipoMovimentacaoPossuiContrapartidaContabil;
        }

        
        public static bool ContraPartidaContabilDepreciaOuReclassifica(this MovementType tipoMovimentacaoPatrimonial)
        {
            bool tipoContrapartidaContabilDepreciaOuReclassifica = false;


            using (var _contextoCamadaDados = new SAMContext())
            {

                if (tipoMovimentacaoPatrimonial.IsNotNull() && tipoMovimentacaoPatrimonial.Code > 0)
                {
                    var listaMovimentacoesContabilizaSPAssociadas = _contextoCamadaDados.EventServicesContabilizaSPs
                                                                                        .Where(eventoContabilizaSP => (eventoContabilizaSP.TipoMovimento_SamPatrimonio == tipoMovimentacaoPatrimonial.Code.ToString()))
                                                                                        .ToList();


                    int[] agrupamentosDepreciacaoReclassificacao = new int[] { (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };
                    var ehMovimentacaoDeTipoDepreciacaoOuReclassificacao = agrupamentosDepreciacaoReclassificacao.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId);
                    var maisDeUmTipoMovimentacaoContabilizaSP = listaMovimentacoesContabilizaSPAssociadas.Count() > 1;
                    tipoContrapartidaContabilDepreciaOuReclassifica = listaMovimentacoesContabilizaSPAssociadas.Count(tipoMovimentacaoContabilizaSP => tipoMovimentacaoContabilizaSP.AccountEntryTypeId != (int)EnumAccountEntryType.AccountEntryType.Entrada
                                                                                                                                                    || tipoMovimentacaoContabilizaSP.AccountEntryTypeId != (int)EnumAccountEntryType.AccountEntryType.Saida) >= 1;

                    tipoContrapartidaContabilDepreciaOuReclassifica = maisDeUmTipoMovimentacaoContabilizaSP && !ehMovimentacaoDeTipoDepreciacaoOuReclassificacao;
                }
            }
            return tipoContrapartidaContabilDepreciaOuReclassifica;
        }
        public static bool ContraPartidaContabilLiquida(this MovementType tipoMovimentacaoPatrimonial)
        {
            bool tipoContrapartidaContabilLiquida = false;
            IList<EventServiceContabilizaSP> listaMovimentacoesContabilizaSPAssociadas = null;

            if (tipoMovimentacaoPatrimonial.IsNotNull())
            {
                using (var _contextoCamadaDados = new SAMContext())
                {
                    listaMovimentacoesContabilizaSPAssociadas = _contextoCamadaDados.EventServicesContabilizaSPs
                                                                                    .Where(eventoContabilizaSP => (eventoContabilizaSP.TipoMovimento_SamPatrimonio == tipoMovimentacaoPatrimonial.Code.ToString()))
                                                                                    .ToList();
                }

                if (tipoMovimentacaoPatrimonial.IsNotNull() && tipoMovimentacaoPatrimonial.Code > 0)
                {
                    int[] agrupamentoDepreciacaoIds = new int[] { EnumGroupMoviment.Incorporacao.GetHashCode(), EnumGroupMoviment.Movimentacao.GetHashCode() };
                    var ehTipoMovimentacaoQuePodeLiquidar = (agrupamentoDepreciacaoIds.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId));
                    var _tipoContrapartidaContabilLiquida = listaMovimentacoesContabilizaSPAssociadas.Count(tipoMovimentacaoContabilizaSP => tipoMovimentacaoContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Entrada
                                                                                                                                           || tipoMovimentacaoContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Saida) >= 1;

                    tipoContrapartidaContabilLiquida = (_tipoContrapartidaContabilLiquida && ehTipoMovimentacaoQuePodeLiquidar);
                }
            }

            return tipoContrapartidaContabilLiquida;
        }
        public static bool ContraPartidaContabilDeprecia(this MovementType tipoMovimentacaoPatrimonial)
        {
            bool tipoContrapartidaContabilDeprecia = false;
            IList<EventServiceContabilizaSP> listaMovimentacoesContabilizaSPAssociadas = null;

            using (var _contextoCamadaDados = new SAMContext())
            {
                listaMovimentacoesContabilizaSPAssociadas = _contextoCamadaDados.EventServicesContabilizaSPs
                                                                                .Where(eventoContabilizaSP => (eventoContabilizaSP.TipoMovimento_SamPatrimonio == tipoMovimentacaoPatrimonial.Code.ToString()))
                                                                                .ToList();
            }

            if (tipoMovimentacaoPatrimonial.IsNotNull() && tipoMovimentacaoPatrimonial.Code > 0)
            {
                //int[] agrupamentosDepreciacaoReclassificacaoIds = new int[] { EnumGroupMoviment.Depreciacao.GetHashCode(), EnumGroupMoviment.Reclassificacao.GetHashCode() };
                //var ehTipoMovimentacaoQuePodeDepreciar = (agrupamentosDepreciacaoReclassificacaoIds.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId));
                var maisDeUmTipoMovimentacaoContabilizaSP = listaMovimentacoesContabilizaSPAssociadas.Count() > 1;
                var _tipoContrapartidaContabilDeprecia = listaMovimentacoesContabilizaSPAssociadas.Count(tipoMovimentacaoContabilizaSP => tipoMovimentacaoContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Depreciacao) >= 1;

                tipoContrapartidaContabilDeprecia = _tipoContrapartidaContabilDeprecia && maisDeUmTipoMovimentacaoContabilizaSP;// && ehTipoMovimentacaoQuePodeDepreciar;
            }

            return tipoContrapartidaContabilDeprecia;
        }
        public static bool ContraPartidaContabilReclassifica(this MovementType tipoMovimentacaoPatrimonial)
        {
            bool tipoContrapartidaContabilReclassifica = false;
            IList<EventServiceContabilizaSP> listaMovimentacoesContabilizaSPAssociadas = null;

            using (var _contextoCamadaDados = new SAMContext())
            {
                listaMovimentacoesContabilizaSPAssociadas = _contextoCamadaDados.EventServicesContabilizaSPs
                                                                                .Where(eventoContabilizaSP => (eventoContabilizaSP.TipoMovimento_SamPatrimonio == tipoMovimentacaoPatrimonial.Code.ToString()))
                                                                                .ToList();
            }

            if (tipoMovimentacaoPatrimonial.IsNotNull() && tipoMovimentacaoPatrimonial.Code > 0)
            {
                //int[] agrupamentosDepreciacaoReclassificacaoIds = new int[] { EnumGroupMoviment.Depreciacao.GetHashCode(), EnumGroupMoviment.Reclassificacao.GetHashCode() };
                //var ehTipoMovimentacaoQuePodeReclassificar = (agrupamentosDepreciacaoReclassificacaoIds.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId));
                var maisDeUmTipoMovimentacaoContabilizaSP = listaMovimentacoesContabilizaSPAssociadas.Count() > 1;
                var _tipoContrapartidaContabilReclassifica = listaMovimentacoesContabilizaSPAssociadas.Count(tipoMovimentacaoContabilizaSP => tipoMovimentacaoContabilizaSP.AccountEntryTypeId == (int)EnumAccountEntryType.AccountEntryType.Reclassificacao) >= 1;

                tipoContrapartidaContabilReclassifica = _tipoContrapartidaContabilReclassifica && maisDeUmTipoMovimentacaoContabilizaSP; // && ehTipoMovimentacaoQuePodeReclassificar;
            }

            return tipoContrapartidaContabilReclassifica;
        }
    }
}