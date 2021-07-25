using LinqKit;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Globalization;
using System.Linq.Expressions;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using TipoMovimentacaoPatrimonial = SAM.Web.Common.Enum.EnumMovimentType;



namespace SAM.Web.Controllers
{
    public class EstornoController : BaseController
    {
        private MovimentoContext db;

        [HttpGet]
        public JsonResult EstornoDaMovimentacao(int assetId, int assetMovementId, int movimentTypeId, int groupMovimentId, string loginSiafem, string senhaSiafem)
        {
            string msgInformeAoUsuario = null;

            try
            {
                db = new MovimentoContext();
                bool _recarregaPagina = false;
                bool EhManual = false;
                var movimentoASerEstornado = (from am in db.AssetMovements where am.Id == assetMovementId select am).FirstOrDefault();
                int ManagerUnitId = movimentoASerEstornado.ManagerUnitId;
                int? auditoriaIntegracaoVinculadaId = movimentoASerEstornado.AuditoriaIntegracaoId;

                if (movimentTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
                {

                    //Verifica se for no modo manual
                    EhManual = (from am in db.AssetMovements
                                where am.AssetId == assetId
                                   && am.MovementTypeId == movimentTypeId
                                select am.SourceDestiny_ManagerUnitId != null).FirstOrDefault();
                }

                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    var _user = UserCommon.CurrentUser();

                    if (AssetEhAtivo(assetId) == true)
                    {
                        if (groupMovimentId == (int)EnumGroupMoviment.Incorporacao)
                        {
                            if (movimentTypeId == (int)EnumMovimentType.Transferencia ||
                                movimentTypeId == (int)EnumMovimentType.Doacao ||
                                movimentTypeId == (int)EnumMovimentType.IncorporacaoPorTransferencia ||
                                movimentTypeId == (int)EnumMovimentType.IncorporacaoPorDoacao ||
                                movimentTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                                movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
                                movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado)
                            {
                                //Só o modo da incorporação não for manual (ou não tiver), ele reativa o BP antigo
                                if (!EhManual)
                                {
                                    ReativarMovimentoEAssetDeOutraUGE(assetId);
                                }
                            }
                            else if (movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                                     movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
                            {
                                if (!EhManual)
                                {
                                    RetiraReferenciaNaOrigem(assetId);
                                }
                            }

                            int ultimoAssetMovementId = (from am in db.AssetMovements
                                                         where am.AssetId == assetId
                                                         && am.Status
                                                         select am.Id).Max();

                            int? IdUGEDestino = db.AssetMovements.Find(ultimoAssetMovementId).SourceDestiny_ManagerUnitId;

                            if (IdUGEDestino == null || EhManual)
                            {
                                //EXCLUSAO DE PENDENCIA NL SIAFEM, CASO EXISTA
                                {
                                    string mensagemParaUsuario = null;
                                    //if (existePendenciaNLAtivaEEhUnicoBPVinculado(assetId, assetMovementId))
                                    //    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), out mensagemParaUsuario);
                                    //else if (existePendenciaNLAtiva(assetId, assetMovementId, movimentTypeId, groupMovimentId))
                                    //    this.atualizaDadosNotaLancamentoPendente(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), assetMovementId, out mensagemParaUsuario);
                                    if (existeMaisDeUmaPendenciaNLAtivaMasEEhUnicoBPVinculado02(assetId, assetMovementId))
                                        this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                    else if (existePendenciaNLAtivaEEhUnicoBPVinculado02(assetId, assetMovementId))
                                        this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                    else if (existePendenciaNLAtivaComMaisDeUmBPVinculado(assetId, assetMovementId))
                                        this.atualizaDadosNotaLancamentoPendente02(assetMovementId, out mensagemParaUsuario);
                                }

                                RegistraBPExcluido(assetId, assetMovementId, _user.Id);
                            }
                            else
                            {
                                if (verificaNaoImplantacao((int)IdUGEDestino))
                                {
                                    EstornaAceiteAutomatico(assetId, ultimoAssetMovementId, _user.Id);
                                }
                                else
                                {
                                    //EXCLUSAO DE PENDENCIA NL SIAFEM, CASO EXISTA
                                    {
                                        string mensagemParaUsuario = null;
                                        //if (existePendenciaNLAtivaEEhUnicoBPVinculado(assetId, assetMovementId))
                                        //    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), out mensagemParaUsuario);
                                        //else if (existePendenciaNLAtiva(assetId, assetMovementId, movimentTypeId, groupMovimentId))
                                        //    this.atualizaDadosNotaLancamentoPendente(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), assetMovementId, out mensagemParaUsuario);
                                        if (existeMaisDeUmaPendenciaNLAtivaMasEEhUnicoBPVinculado02(assetId, assetMovementId))
                                            this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                        else if (existePendenciaNLAtivaEEhUnicoBPVinculado02(assetId, assetMovementId))
                                            this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                        else if (existePendenciaNLAtivaComMaisDeUmBPVinculado(assetId, assetMovementId))
                                            this.atualizaDadosNotaLancamentoPendente02(assetMovementId, out mensagemParaUsuario);
                                    }

                                    RegistraBPExcluido(assetId, assetMovementId, _user.Id);
                                }
                            }
                        }
                        else if (groupMovimentId == (int)EnumGroupMoviment.Movimentacao)
                        {
                            EstornaMovimento(assetId, assetMovementId, _user.Id);

                            //EXCLUSAO DE PENDENCIA NL SIAFEM, CASO EXISTA
                            {
                                string mensagemParaUsuario = null;
                                if (existeMaisDeUmaPendenciaNLAtivaMasEEhUnicoBPVinculado02(assetId, assetMovementId))
                                    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                else if (existePendenciaNLAtivaEEhUnicoBPVinculado02(assetId, assetMovementId))
                                    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                else if (existePendenciaNLAtivaComMaisDeUmBPVinculado(assetId, assetMovementId))
                                    this.atualizaDadosNotaLancamentoPendente02(assetMovementId, out mensagemParaUsuario);
                            }
                    }

                        _recarregaPagina = true;
                    }
                    else
                    {
                        if (groupMovimentId == (int)EnumGroupMoviment.Movimentacao)
                        {
                            if (movimentTypeId == (int)EnumMovimentType.VoltaConserto ||
                               movimentTypeId == (int)EnumMovimentType.SaidaConserto ||
                               movimentTypeId == (int)EnumMovimentType.Extravio ||
                               movimentTypeId == (int)EnumMovimentType.Obsoleto ||
                               movimentTypeId == (int)EnumMovimentType.Danificado ||
                               movimentTypeId == (int)EnumMovimentType.Sucata ||
                               movimentTypeId == (int)EnumMovimentType.MovComodatoTerceirosRecebidos ||
                               movimentTypeId == (int)EnumMovimentType.MovDoacaoConsolidacao ||
                               movimentTypeId == (int)EnumMovimentType.MovDoacaoMunicipio ||
                               movimentTypeId == (int)EnumMovimentType.MovDoacaoOutrosEstados ||
                               movimentTypeId == (int)EnumMovimentType.MovDoacaoUniao ||
                               movimentTypeId == (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis ||
                               movimentTypeId == (int)EnumMovimentType.MovMorteAnimalPatrimoniado ||
                               movimentTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis ||
                               movimentTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis ||
                               movimentTypeId == (int)EnumMovimentType.MovSementesPlantasInsumosArvores ||
                               movimentTypeId == (int)EnumMovimentType.MovVendaLeilaoSemoventes)
                            {
	                            //EXCLUSAO DE PENDENCIA NL SIAFEM, CASO EXISTA
	                            {
                                    string mensagemParaUsuario = null;
                                    //if (existePendenciaNLAtivaEEhUnicoBPVinculado(assetId, assetMovementId))
                                    //    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), out mensagemParaUsuario);
                                    //else if (existePendenciaNLAtiva(assetId, assetMovementId, movimentTypeId, groupMovimentId))
                                    //    this.atualizaDadosNotaLancamentoPendente(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), assetMovementId, out mensagemParaUsuario);
                                    if (existeMaisDeUmaPendenciaNLAtivaMasEEhUnicoBPVinculado02(assetId, assetMovementId))
                                        this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                    else if (existePendenciaNLAtivaEEhUnicoBPVinculado02(assetId, assetMovementId))
                                        this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                    else if (existePendenciaNLAtivaComMaisDeUmBPVinculado(assetId, assetMovementId))
                                        this.atualizaDadosNotaLancamentoPendente02(assetMovementId, out mensagemParaUsuario);
                                }

                                EstornaMovimento(assetId, assetMovementId, _user.Id);
                                ReativarAsset(assetId);
                                _recarregaPagina = true;
                            }
                            else if (movimentTypeId == (int)EnumMovimentType.Transferencia ||
                                        movimentTypeId == (int)EnumMovimentType.Doacao ||
                                        movimentTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                        movimentTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                                        movimentTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado)
                            {
                                //BPs que estejam inativos pelo fato de serem transferidos para UGEs inativas, podem ser estornados.
                                bool flagUGENaoUtilizada = MovimentoPossuiFlagUGENaoUtilizada(assetMovementId);

                                if (flagUGENaoUtilizada == true)
                                {
                                    EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);

                                    _recarregaPagina = true;
                                }
                                else
                                {
                                    int IdUGEDestino = (int)db.AssetMovements.Find(assetMovementId).SourceDestiny_ManagerUnitId;
                                    if (verificaNaoImplantacao(IdUGEDestino))
                                    {
                                        EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);

                                        _recarregaPagina = true;
                                    }
                                    else
                                        //Barrar o estorno de Bps que ja foram incorporados pela Uge receptora da transferencia que não sejam BPs com o flagUGENaoUtilizada.
                                        return Json(new { trava = "Não foi possivel realizar o estorno, pois a transferência ja foi incorporada pela UGE receptora!" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else if (movimentTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
                                       movimentTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao)
                            {
                                //BPs que estejam inativos pelo fato de serem transferidos para UGEs inativas, podem ser estornados.
                                bool flagUGENaoUtilizada = MovimentoPossuiFlagUGENaoUtilizada(assetMovementId);

                                if (flagUGENaoUtilizada == true)
                                {
                                    EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);

                                    _recarregaPagina = true;
                                }
                                else
                                {
                                    int IdUGEDestino = (int)db.AssetMovements.Find(assetMovementId).SourceDestiny_ManagerUnitId;
                                    if (verificaNaoImplantacao(IdUGEDestino))
                                    {
                                        EstornaAceiteAutomatico(assetId, assetMovementId, _user.Id);
                                        _recarregaPagina = true;
                                    }
                                    else
                                    {
                                        //EXCLUSAO DE PENDENCIA NL SIAFEM, CASO EXISTA
                                        {
                                            string mensagemParaUsuario = null;
                                            //if (existePendenciaNLAtivaEEhUnicoBPVinculado(assetId, assetMovementId))
                                            //    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), out mensagemParaUsuario);
                                            //else if (existePendenciaNLAtiva(assetId, assetMovementId, movimentTypeId, groupMovimentId))
                                            //    this.atualizaDadosNotaLancamentoPendente(auditoriaIntegracaoVinculadaId.GetValueOrDefault(), assetMovementId, out mensagemParaUsuario);
                                            if (existeMaisDeUmaPendenciaNLAtivaMasEEhUnicoBPVinculado02(assetId, assetMovementId))
                                                this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                            else if (existePendenciaNLAtivaEEhUnicoBPVinculado02(assetId, assetMovementId))
                                                this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementId, out mensagemParaUsuario);
                                            else if (existePendenciaNLAtivaComMaisDeUmBPVinculado(assetId, assetMovementId))
                                                this.atualizaDadosNotaLancamentoPendente02(assetMovementId, out mensagemParaUsuario);
                                        }

                                        EstornaMovimento(assetId, assetMovementId, _user.Id);
                                        ReativarAsset(assetId);
                                        _recarregaPagina = true;
                                    }
                                }
                            }
                        }
                    }

                    //TODO VERIFICA SE EXISTE PENDENCIA E CASO SIM EXLUI A MESMA SE PENDENCIA DE UM BP SOH

                    transaction.Complete();
                }

                ComputaAlteracaoContabil(movimentoASerEstornado.InstitutionId, movimentoASerEstornado.BudgetUnitId,
                                             movimentoASerEstornado.ManagerUnitId, movimentoASerEstornado.AuxiliaryAccountId);

                if (movimentoASerEstornado.MovementTypeId == (int)EnumMovimentType.MovInservivelNaUGE)
                {
                    ComputaAlteracaoContabil(movimentoASerEstornado.InstitutionId, movimentoASerEstornado.BudgetUnitId,
                                             movimentoASerEstornado.ManagerUnitId, movimentoASerEstornado.ContaContabilAntesDeVirarInservivel);
                }

                return Json(new { recarregaPagina = _recarregaPagina, retornoContabiliza = msgInformeAoUsuario }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        private void ComputaAlteracaoContabil(int IdOrgao, int IdUO, int IdUGE, int? IdContaContabil)
        {
            var implantado = UGEIntegradaAoSIAFEM(IdUGE);

            if (implantado) {
                if (IdContaContabil != null)
                {
                    var registro = db.HouveAlteracaoContabeis
                                     .Where(h => h.IdOrgao == IdOrgao &&
                                                 h.IdUO == IdUO &&
                                                 h.IdUGE == IdUGE &&
                                                 h.IdContaContabil == IdContaContabil)
                                      .FirstOrDefault();

                    if (registro != null)
                    {
                        registro.HouveAlteracao = true;
                        db.Entry(registro).State = EntityState.Modified;
                    }
                    else
                    {
                        registro = new HouveAlteracaoContabil();
                        registro.IdOrgao = IdOrgao;
                        registro.IdUO = IdUO;
                        registro.IdUGE = IdUGE;
                        registro.IdContaContabil = (int)IdContaContabil;
                        registro.HouveAlteracao = true;
                        db.Entry(registro).State = EntityState.Added;
                    }

                    db.SaveChanges();
                }
            }
        }

        ////Método era utilizado estornar todo o histórico do BP de uma vez. Retirado em 21/07/2020
        //[HttpGet]
        //public JsonResult EstornoCompletoDaMovimentacao(int assetId, int assetMovementId, int movimentTypeId, int groupMovimentId, string loginSiafem, string senhaSiafem)
        //{
        //    try
        //    {
        //        db = new MovimentoContext();
        //        int ManagerUnitId = (from am in db.AssetMovements where am.Id == assetMovementId select am.ManagerUnitId).FirstOrDefault();

        //        //var tiposMovimentosDoBp = (from am in db.AssetMovements
        //        //                           where am.AssetId == assetId
        //        //                           && am.FlagEstorno != true
        //        //                           select am.MovementTypeId).ToList();

        //        //foreach (int tipoMovimento in tiposMovimentosDoBp)
        //        //{
        //        //    if (InvalidaSemLoginSiafem(tipoMovimento, loginSiafem, senhaSiafem, ManagerUnitId))
        //        //    {
        //        //        return Json(new { trava = "Estorno não realizado. Login SIAFEM completo e obrigatório" }, JsonRequestBehavior.AllowGet);
        //        //    }
        //        //}

        //        bool _recarregaPagina = false;
        //        bool EhManual = false;
        //        //COMENTADO CONTABILIZA
        //        //bool integracaoSIAFEMAtivada = false;
        //        //bool tipoMovimentacaoGeraNL = false;
        //        //bool MaisDeUmTipoMovimentacaoGeraNL = false;
        //        //bool existeNL_ParaEstorno = false;
        //        //bool existeNL_Pendente = false;
        //        ////bool primeiraMovimentacao = false;
        //        //string msgInformeAoUsuario = null;
        //        //AssetMovements movPatrimonialSendoEstornada = null;
        //        //TipoNotaSIAFEM @TipoNotaSIAFEM = default(TipoNotaSIAFEM);
        //        //IList<Tuple<string, IList<string>>> dadosNLsEstorno = null;

        //        //----- Travas do estorno -----//

        //        // Todos movimentos do BP
        //        var MovimentosBp = (from am in db.AssetMovements
        //                            where am.AssetId == assetId
        //                            select am);

        //        //Verificacao se a UGE possui integracao ativada com o SIAFEM/Contabiliza-SP
        //        //COMENTADO CONTABILIZA
        //        //this.initDadosSIAFEM();
        //        //integracaoSIAFEMAtivada = this.ugeIntegradaSiafem;
        //        //if (integracaoSIAFEMAtivada)
        //        //{
        //        //    int contadorTipoMovimentacoesIntegradas = 0;

        //        //    //Verifica se alguma movimentacao do historico do BP integra com ContabilizaSP/SIAFEM
        //        //    MovimentosBp.Select(movimentacaoBP => movimentacaoBP.MovementTypeId)
        //        //                .ToList()
        //        //                .ForEach(movimentacaoBP => {
        //        //                                             tipoMovimentacaoGeraNL = IntegracaoContabilizaSPExtensionMethods.TipoMovimentacaoPossuiContraPartidaContabil(movimentTypeId);
        //        //                                             contadorTipoMovimentacoesIntegradas = (tipoMovimentacaoGeraNL ? contadorTipoMovimentacoesIntegradas++ : contadorTipoMovimentacoesIntegradas);
        //        //                                           });

        //        //    MaisDeUmTipoMovimentacaoGeraNL = (contadorTipoMovimentacoesIntegradas > 1);
        //        //    if (MaisDeUmTipoMovimentacaoGeraNL)
        //        //    {
        //        //        return Json(new { trava = "Este Bem Patrimonial possui mais de um movimentação patrimonial que integra ao sistema SIAFEM/Contabiliza-SP, não sendo possível estorná-lo totalmente deste modo!" }, JsonRequestBehavior.AllowGet);
        //        //    }
        //        //}

        //        // MesRef do Movimento anterior não pode ser diferente do MesRef atual em caso de Transferencia e Doacao
        //        if (movimentTypeId == (int)EnumMovimentType.IncorporacaoPorTransferencia ||
        //            movimentTypeId == (int)EnumMovimentType.IncorporacaoPorDoacao ||
        //            movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado)
        //        {
        //            // Movimento Ativo
        //            var IdUGEMovimentoAtivo = MovimentosBp.Where(a => a.Status == true).Select(a => a.ManagerUnitId).FirstOrDefault();

        //            var IdUGEBPmovimentoOrigem = (from a in db.AssetMovements where a.AssetTransferenciaId == assetId select a.ManagerUnitId).FirstOrDefault();

        //            var MesRefMovAnterior = (from m in db.ManagerUnits where m.Id == IdUGEBPmovimentoOrigem select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
        //            var MesRefMovAtual = (from m in db.ManagerUnits where m.Id == IdUGEMovimentoAtivo select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

        //            if (MesRefMovAnterior != MesRefMovAtual)
        //            {
        //                return Json(new { trava = "O mês/ano de referência da UGE emissora do Bem Patrimonial, não se encontra no mesmo mês de referência da UGE atual." }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        if (movimentTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
        //            movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
        //            movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia ||
        //            movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao)
        //        {

        //            //Verifica se for no modo manual
        //            EhManual = (from am in db.AssetMovements
        //                        where am.AssetId == assetId
        //                           && am.MovementTypeId == movimentTypeId
        //                        select am.SourceDestiny_ManagerUnitId != null).FirstOrDefault();

        //            //se não for, faz a validação dos meses de referências da UGE
        //            if (!EhManual)
        //            {
        //                var IdUGEMovimentoAtivo = (from am in db.AssetMovements
        //                                           where am.AssetId == assetId
        //                                           && am.Status == true
        //                                           select am.ManagerUnitId).FirstOrDefault();

        //                var IdUGEBPmovimentoOrigem = (from a in db.AssetMovements where a.AssetTransferenciaId == assetId select a.ManagerUnitId).FirstOrDefault();

        //                var MesRefMovAnterior = (from m in db.ManagerUnits where m.Id == IdUGEBPmovimentoOrigem select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
        //                var MesRefMovAtual = (from m in db.ManagerUnits where m.Id == IdUGEMovimentoAtivo select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

        //                if (MesRefMovAnterior != MesRefMovAtual)
        //                {
        //                    return Json(new { trava = "O mês/ano de referência da UGE emissora do Bem Patrimonial, não se encontra no mesmo mês de referência da UGE atual." }, JsonRequestBehavior.AllowGet);
        //                }
        //            }

        //        }

        //        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
        //        {
        //            var _user = UserCommon.CurrentUser();

        //            if (movimentTypeId == (int)EnumMovimentType.IncorporacaoPorTransferencia ||
        //                movimentTypeId == (int)EnumMovimentType.IncorporacaoPorDoacao ||
        //                movimentTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
        //                movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
        //                movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado)
        //            {
        //                //se não for no modo manual (ou não tiver), ele reativa o BP antigo
        //                if (!EhManual)
        //                {
        //                    ReativarMovimentoEAssetDeOutraUGE(assetId);
        //                    //COMENTADO CONTABILIZA
        //                    //movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                    //existeNL_ParaEstorno = (movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, false).HasElements()  //NL
        //                    //                    && !movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true).HasElements()); //NL estorno

        //                    //existeNL_Pendente = ExisteNotaLancamentoPendenteVinculada(assetId, assetMovementId, false);
        //                    //tipoMovimentacaoGeraNL = IntegracaoContabilizaSPExtensionMethods.TipoMovimentacaoPossuiContraPartidaContabil(movimentTypeId);
        //                    //if (!integracaoSIAFEMAtivada || !tipoMovimentacaoGeraNL)
        //                    //{
        //                    //ReativarMovimentoEAssetDeOutraUGE(assetId);
        //                    //}
        //                    //else if (integracaoSIAFEMAtivada && tipoMovimentacaoGeraNL)
        //                    //{
        //                    //    //TODO [DCBATISTA] INCLUSAO CODIGO INTEGRACAO CONTABILIZA-SP
        //                    //    #region Processamento estorno no Contabiliza-SP
        //                    //    if (existeNL_ParaEstorno)
        //                    //    {
        //                    //        @TipoNotaSIAFEM = (TipoNotaSIAFEM)groupMovimentId;
        //                    //        movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                    //        msgInformeAoUsuario = ProcessamentoEstornoMovimentacaoNoContabilizaSP(loginSiafem, senhaSiafem, movPatrimonialSendoEstornada);
        //                    //        dadosNLsEstorno = movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true);

        //                    //        if (dadosNLsEstorno.HasElements())
        //                    //            ReativarMovimentoEAssetDeOutraUGE(assetId);
        //                    //        else
        //                    //            return Json(new { trava = "Erro ao gerar NL(s) SIAFEM de estorno para este BP/Movimentação.\nEstorno não permitido até regularização!" }, JsonRequestBehavior.AllowGet);
        //                    //    }
        //                    //    #endregion Processamento estorno no Contabiliza-SP
        //                    //}
        //                }
        //                RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                //_recarregaPagina = true;
        //                //COMENTADO CONTABILIZA
        //                //existeNL_Pendente = ExisteNotaLancamentoPendenteVinculada(assetId, assetMovementId, false);
        //                //tipoMovimentacaoGeraNL = IntegracaoContabilizaSPExtensionMethods.TipoMovimentacaoPossuiContraPartidaContabil(movimentTypeId);
        //                //movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                //existeNL_ParaEstorno = (movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, false).HasElements()  //NL
        //                //                    && !movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true).HasElements()); //NL estorno
        //                //if (!integracaoSIAFEMAtivada || !tipoMovimentacaoGeraNL)
        //                //{
        //                //RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                //}
        //                //else if (integracaoSIAFEMAtivada && tipoMovimentacaoGeraNL)
        //                //{
        //                //    //TODO [DCBATISTA] INCLUSAO CODIGO INTEGRACAO CONTABILIZA-SP
        //                //    #region Processamento estorno no Contabiliza-SP
        //                //    if (existeNL_ParaEstorno)
        //                //    {
        //                //        @TipoNotaSIAFEM = (TipoNotaSIAFEM)groupMovimentId;
        //                //        movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                //        msgInformeAoUsuario = ProcessamentoEstornoMovimentacaoNoContabilizaSP(loginSiafem, senhaSiafem, movPatrimonialSendoEstornada);
        //                //        dadosNLsEstorno = movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true);

        //                //        if (dadosNLsEstorno.HasElements())
        //                //            RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                //        else
        //                //            return Json(new { trava = "Erro ao gerar NL(s) SIAFEM de estorno para este BP/Movimentação.\nEstorno não permitido até regularização!" }, JsonRequestBehavior.AllowGet);
        //                //    }
        //                //    #endregion Processamento estorno no Contabiliza-SP
        //                //}

        //                _recarregaPagina = true;
        //            }
        //            else if (movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
        //                       movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
        //            {
        //                //se não for no modo manual (ou não tiver), ele reativa o BP antigo
        //                if (!EhManual)
        //                {
        //                    RetiraReferenciaNaOrigem(assetId);
        //                }

        //                RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                //_recarregaPagina = true;
        //                //COMENTADO CONTABILIZA
        //                //existeNL_Pendente = ExisteNotaLancamentoPendenteVinculada(assetId, assetMovementId, false);
        //                //tipoMovimentacaoGeraNL = IntegracaoContabilizaSPExtensionMethods.TipoMovimentacaoPossuiContraPartidaContabil(movimentTypeId);
        //                //movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                //existeNL_ParaEstorno = (movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, false).HasElements()  //NL
        //                //                    && !movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true).HasElements()); //NL estorno
        //                //if (!integracaoSIAFEMAtivada)
        //                //{
        //                //RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                //}
        //                //else if (integracaoSIAFEMAtivada && tipoMovimentacaoGeraNL)
        //                //{
        //                //    //TODO [DCBATISTA] INCLUSAO CODIGO INTEGRACAO CONTABILIZA-SP
        //                //    #region Processamento estorno no Contabiliza-SP
        //                //    if (existeNL_ParaEstorno)
        //                //    {
        //                //        @TipoNotaSIAFEM = (TipoNotaSIAFEM)groupMovimentId;
        //                //        movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                //        msgInformeAoUsuario = ProcessamentoEstornoMovimentacaoNoContabilizaSP(loginSiafem, senhaSiafem, movPatrimonialSendoEstornada);
        //                //        dadosNLsEstorno = movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true);

        //                //        if (dadosNLsEstorno.HasElements())
        //                //            RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                //        else
        //                //            return Json(new { trava = "Erro ao gerar NL(s) SIAFEM de estorno para este BP/Movimentação.\nEstorno não permitido até regularização!" }, JsonRequestBehavior.AllowGet);
        //                //    }
        //                //    #endregion Processamento estorno no Contabiliza-SP
        //                //}

        //                _recarregaPagina = true;
        //            }
        //            else if (movimentTypeId == (int)EnumMovimentType.Transferencia ||
        //                     movimentTypeId == (int)EnumMovimentType.Doacao ||
        //                     movimentTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
        //                     movimentTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
        //                     movimentTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado ||
        //                     movimentTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
        //                     movimentTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao)
        //            {
        //                bool flagUGENaoUtilizada = MovimentoPossuiFlagUGENaoUtilizada(assetMovementId);

        //                if (flagUGENaoUtilizada == true)
        //                {
        //                    EstornaMovimento(assetId, assetMovementId, _user.Id);
        //                    ReativarAsset(assetId);
        //                    //COMENTADO CONTABILIZA
        //                    //if (!integracaoSIAFEMAtivada)
        //                    //{
        //                    //EstornaMovimento(assetId, assetMovementId, _user.Id);
        //                    //ReativarAsset(assetId);
        //                    //}
        //                    //else if (integracaoSIAFEMAtivada && tipoMovimentacaoGeraNL)
        //                    //{
        //                    //    movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                    //    existeNL_ParaEstorno = (movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, false).HasElements()  //NL
        //                    //                        && !movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true).HasElements()); //NL estorno
        //                    //    //TODO [DCBATISTA] INCLUSAO CODIGO INTEGRACAO CONTABILIZA-SP
        //                    //    #region Processamento estorno no Contabiliza-SP
        //                    //    if (existeNL_ParaEstorno)
        //                    //    {
        //                    //        @TipoNotaSIAFEM = (TipoNotaSIAFEM)groupMovimentId;
        //                    //        movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                    //        msgInformeAoUsuario = ProcessamentoEstornoMovimentacaoNoContabilizaSP(loginSiafem, senhaSiafem, movPatrimonialSendoEstornada);
        //                    //        dadosNLsEstorno = movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true);

        //                    //        if (dadosNLsEstorno.HasElements())
        //                    //        {
        //                    //            EstornaMovimento(assetId, assetMovementId, _user.Id);
        //                    //            ReativarAsset(assetId);
        //                    //        }
        //                    //        else
        //                    //        {
        //                    //            return Json(new { trava = "Erro ao gerar NL(s) SIAFEM de estorno para este BP/Movimentação.\nEstorno não permitido até regularização!" }, JsonRequestBehavior.AllowGet);
        //                    //        }
        //                    //    }
        //                    //    #endregion Processamento estorno no Contabiliza-SP
        //                    //}

        //                    _recarregaPagina = true;
        //                }
        //                else
        //                {
        //                    //Barrar o estorno de Bps que ja foram incorporados pela Uge receptora da transferencia que não sejam BPs com o flagUGENaoUtilizada.
        //                    return Json(new { trava = "Não foi possivel realizar o estorno, pois a transferência ja foi incorporada pela UGE receptora!" }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                if (groupMovimentId == (int)EnumGroupMoviment.Incorporacao)
        //                {
        //                    int ultimoAssetMovementId = (from am in db.AssetMovements
        //                                                 where am.AssetId == assetId
        //                                                 && am.FlagEstorno == null
        //                                                 select am.Id).Max();

        //                    int? IdUGEDestino = db.AssetMovements.Find(ultimoAssetMovementId).SourceDestiny_ManagerUnitId;

        //                    if (IdUGEDestino == null)
        //                    {
        //                        RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                        //COMENTADO CONTABILIZA
        //                        //    existeNL_Pendente = ExisteNotaLancamentoPendenteVinculada(assetId, assetMovementId, false);
        //                        //    tipoMovimentacaoGeraNL = IntegracaoContabilizaSPExtensionMethods.TipoMovimentacaoPossuiContraPartidaContabil(movimentTypeId);
        //                        //    movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                        //    existeNL_ParaEstorno = (movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, false).HasElements()  //NL
        //                        //                        && !movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true).HasElements()); //NL estorno
        //                        //    if (!integracaoSIAFEMAtivada)
        //                        //    {
        //                        //        RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                        //    }
        //                        //    else if (integracaoSIAFEMAtivada && tipoMovimentacaoGeraNL)
        //                        //    {
        //                        //        if (existeNL_ParaEstorno)
        //                        //        {
        //                        //            @TipoNotaSIAFEM = (TipoNotaSIAFEM)groupMovimentId;
        //                        //            movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                        //            msgInformeAoUsuario = ProcessamentoEstornoMovimentacaoNoContabilizaSP(loginSiafem, senhaSiafem, movPatrimonialSendoEstornada);
        //                        //            dadosNLsEstorno = movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true);

        //                        //            if (dadosNLsEstorno.HasElements())
        //                        //    RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                        //else
        //                        //                return Json(new { trava = "Erro ao gerar NL(s) SIAFEM de estorno para este BP/Movimentação.\nEstorno não permitido até regularização!" }, JsonRequestBehavior.AllowGet);
        //                        //        }
        //                        //    }
        //                    }
        //                    else
        //                    {
        //                        if (verificaNaoImplantacao((int)IdUGEDestino))
        //                        {
        //                            EstornaAceiteAutomatico(assetId, ultimoAssetMovementId, _user.Id);
        //                        }

        //                        RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                        //COMENTADO CONTABILIZA
        //                        //    movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                        //    existeNL_ParaEstorno = (movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, false).HasElements()  //NL
        //                        //                        && !movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true).HasElements()); //NL estorno
        //                        //    existeNL_Pendente = ExisteNotaLancamentoPendenteVinculada(assetId, assetMovementId, false);
        //                        //    tipoMovimentacaoGeraNL = IntegracaoContabilizaSPExtensionMethods.TipoMovimentacaoPossuiContraPartidaContabil(movimentTypeId);
        //                        //    if (!integracaoSIAFEMAtivada)
        //                        //    {
        //                        //    RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                        //}
        //                        //    else if (integracaoSIAFEMAtivada && tipoMovimentacaoGeraNL)
        //                        //    {
        //                        //        if (existeNL_ParaEstorno)
        //                        //        {
        //                        //            @TipoNotaSIAFEM = (TipoNotaSIAFEM)groupMovimentId;
        //                        //            movPatrimonialSendoEstornada = obterMovimentacaoParaProcessamento(assetId, assetMovementId, movimentTypeId, groupMovimentId);
        //                        //            msgInformeAoUsuario = ProcessamentoEstornoMovimentacaoNoContabilizaSP(loginSiafem, senhaSiafem, movPatrimonialSendoEstornada);
        //                        //            dadosNLsEstorno = movPatrimonialSendoEstornada.ObterNLsMovimentacao(@TipoNotaSIAFEM, true);

        //                        //            if (dadosNLsEstorno.HasElements())
        //                        //                RegistraBPExcluido(assetId, assetMovementId, _user.Id);
        //                        //            else
        //                        //                return Json(new { trava = "Erro ao gerar NL(s) SIAFEM de estorno para este BP/Movimentação.\nEstorno não permitido até regularização!" }, JsonRequestBehavior.AllowGet);
        //                        //        }
        //                        //    }
        //                    }
        //                }
        //                else
        //                {


        //                    var MovimentosBpAtivo = (from am in db.AssetMovements
        //                                             where am.AssetId == assetId && am.Status == true
        //                                             select am).FirstOrDefault();

        //                    MovimentosBpAtivo.DataEstorno = DateTime.Now;
        //                    MovimentosBpAtivo.LoginEstorno = _user.Id;

        //                    db.Entry(MovimentosBpAtivo).State = EntityState.Modified;
        //                    db.SaveChanges();

        //                    var assetDB = (from a in db.Assets
        //                                   where a.Id == assetId
        //                                   select a).FirstOrDefault();
        //                    assetDB.Status = false;

        //                    db.Entry(assetDB).State = EntityState.Modified;

        //                    foreach (var item in MovimentosBp.ToList())
        //                    {
        //                        item.Status = false;
        //                        db.Entry(item).State = EntityState.Modified;
        //                    }
        //                    db.SaveChanges();
        //                }

        //                _recarregaPagina = true;
        //            }

        //            transaction.Complete();
        //        }

        //        return Json(new { recarregaPagina = _recarregaPagina, retornoContabiliza = string.Empty }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return MensagemErroJson(CommonMensagens.PadraoException, ex);
        //    }
        //}


        #region Outros Métodos Estorno Único histórico

        public JsonResult ValidaEstornoMovimentoEVerificaMovimentoIntegrado(int assetId, int assetMovementId, int movimentTypeId, int groupMovimentId)
        {
            db = new MovimentoContext();

            int ManagerUnitId = (from am in db.AssetMovements where am.Id == assetMovementId select am.ManagerUnitId).FirstOrDefault();

            if (UGEEstaComPendenciaSIAFEMNoFechamento(ManagerUnitId))
            {
                return Json(new { trava = CommonMensagens.OperacaoInvalidaIntegracaoFechamento }, JsonRequestBehavior.AllowGet);
            }

            //variável para validar se Doação Intra no Estado ou Transferência outro orgão
            // foi feito de modo manual. Ele já é setado como false para ajudar nas transferência que
            // não possuem modo manual.
            bool EhManual = false;

            // MesRef do Movimento anterior não pode ser diferente do MesRef atual em caso de Transferencia e Doacao
            if (movimentTypeId == (int)EnumMovimentType.IncorporacaoPorTransferencia ||
                movimentTypeId == (int)EnumMovimentType.IncorporacaoPorDoacao ||
                movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado)
            {
                // Movimento Ativo
                var IdUGEMovimentoAtivo = (from am in db.AssetMovements
                                           where am.AssetId == assetId
                                           && am.Status == true
                                           select am.ManagerUnitId).FirstOrDefault();

                var IdUGEBPmovimentoOrigem = (from a in db.AssetMovements where a.AssetTransferenciaId == assetId select a.ManagerUnitId).FirstOrDefault();

                var MesRefMovAnterior = (from m in db.ManagerUnits where m.Id == IdUGEBPmovimentoOrigem select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
                var MesRefMovAtual = (from m in db.ManagerUnits where m.Id == IdUGEMovimentoAtivo select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

                if (MesRefMovAnterior != MesRefMovAtual)
                {
                    return Json(new { trava = "O mês/ano de referência da UGE emissora do Bem Patrimonial, não se encontra no mesmo mês de referência da UGE atual." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            if (movimentTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                movimentTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                movimentTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
            {

                //Verifica se for no modo manual
                EhManual = (from am in db.AssetMovements
                            where am.AssetId == assetId
                               && am.MovementTypeId == movimentTypeId
                            select am.SourceDestiny_ManagerUnitId != null).FirstOrDefault();

                //se não for, faz a validação dos meses de referências da UGE
                if (!EhManual)
                {
                    var IdUGEMovimentoAtivo = (from am in db.AssetMovements
                                               where am.AssetId == assetId
                                               && am.Status == true
                                               select am.ManagerUnitId).FirstOrDefault();

                    var IdUGEBPmovimentoOrigem = (from a in db.AssetMovements where a.AssetTransferenciaId == assetId select a.ManagerUnitId).FirstOrDefault();

                    var MesRefMovAnterior = (from m in db.ManagerUnits where m.Id == IdUGEBPmovimentoOrigem select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
                    var MesRefMovAtual = (from m in db.ManagerUnits where m.Id == IdUGEMovimentoAtivo select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

                    if (MesRefMovAnterior != MesRefMovAtual)
                    {
                        return Json(new { trava = "O mês/ano de referência da UGE emissora do Bem Patrimonial, não se encontra no mesmo mês de referência da UGE atual." }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                //Movimentos Inativos com excessao dos estornados
                var MovimentosInativos = (from am in db.AssetMovements
                                          where am.AssetId == assetId
                                          && am.Status == false
                                          && am.FlagEstorno == null
                                          && am.Id != assetMovementId
                                          select am);

                if (MovimentosInativos != null)
                {
                    // Penultimo Movimento do BP
                    var assetMovementPenultimoDB = MovimentosInativos.OrderByDescending(l => l.Id).FirstOrDefault();

                    if (assetMovementPenultimoDB != null)
                    {

                        // Verifica se a divisão do movimento anterior esta ativa 
                        if (assetMovementPenultimoDB.SectionId != null)
                        {
                            var _DivisaoAtiva = (from s in db.Sections
                                                 where s.Id == assetMovementPenultimoDB.SectionId
                                                 select s).FirstOrDefault();

                            if (_DivisaoAtiva.Status == false)
                                return Json(new { trava = "Não foi possivel realizar o estorno, pois a Divisão do movimento anterior esta inativa!" }, JsonRequestBehavior.AllowGet);

                            // Verifica se a Divisao do movimento anterior pertence a UA 
                            if (_DivisaoAtiva.AdministrativeUnitId != assetMovementPenultimoDB.AdministrativeUnitId)
                            {
                                return Json(new { trava = "Não foi possivel realizar o estorno, pois a Divisão do movimento anterior, não pertence mais a UA!" }, JsonRequestBehavior.AllowGet);
                            }

                            // Verifica se o responsavel do movimento anterior pertence a divisão 
                            if (_DivisaoAtiva.ResponsibleId != assetMovementPenultimoDB.ResponsibleId)
                            {
                                return Json(new { trava = "Não foi possivel realizar o estorno, pois o Responsável do movimento anterior, não pertence mais a Divisão!" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        // Verifica se a UA do movimento anterior esta ativa 
                        if (assetMovementPenultimoDB.AdministrativeUnitId != null)
                        {
                            var _UAAtiva = (from a in db.AdministrativeUnits
                                            where a.Id == assetMovementPenultimoDB.AdministrativeUnitId
                                            select a).FirstOrDefault();

                            if (_UAAtiva.Status == false)
                                return Json(new { trava = "Não foi possivel realizar o estorno, pois a UA do movimento anterior esta inativa!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            bool integrado = false;
            //Tratamento Contabiliza
            initDadosSiafemIntegracao();

            if (this.ugeIntegradaSiafem) {
                var uge = (from am in db.AssetMovements
                           join m in db.ManagerUnits on am.ManagerUnitId equals m.Id
                           where am.Id == assetMovementId
                           select m).First();

                int mesReferenciaMovimentacaoPatrimonial = Convert.ToInt32(uge.ManagmentUnit_YearMonthReference);

                if (mesReferenciaMovimentacaoPatrimonial >= uge.MesRefInicioIntegracaoSIAFEM)
                {
                    if (movimentTypeId == EnumMovimentType.IncorpIntegracaoSAMEstoque_SAMPatrimonio.GetHashCode())
                    {
                        integrado = (mesReferenciaMovimentacaoPatrimonial >= 202102);
                    }
                    else
                    {
                        MovementType tipoMovimento = db.MovementTypes.Find(movimentTypeId);

                        integrado = tipoMovimento.PossuiContraPartidaContabil();
                    }
                }
            }

            bool contemNL = this.existeNLSIAFEM(assetId, assetMovementId);
            //bool contemPendencia = this.existePendenciaNLAtiva(assetId, assetMovementId, movimentTypeId, groupMovimentId);
            bool contemPendencia = (this.existePendenciaNLAtivaEEhUnicoBPVinculado02(assetId, assetMovementId) || this.existePendenciaNLAtivaComMaisDeUmBPVinculado(assetId, assetMovementId));

            if (contemNL && !contemPendencia)
                integrado = !this.NLsJaForamEstornada(assetMovementId);

            return Json(new { integrado = integrado, contemNL = contemNL, contemPendencia = contemPendencia }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CorrigePendenciaSiafem(int historico)
        {
            AssetMovements movPatrimonial = null;
            MovementType tipoMovimentacao = null;
            int assetMovementsId = 0;
            int assetId = 0;
            int auditoriaIntegracaoVinculadaId = 0;
            string mensagemParaUsuario = null;


            assetMovementsId = historico;
            if (assetMovementsId > 0)
            {
                movPatrimonial = obterEntidadeAssetMovements(assetMovementsId);
                auditoriaIntegracaoVinculadaId = movPatrimonial.AuditoriaIntegracaoId.GetValueOrDefault();
                if (auditoriaIntegracaoVinculadaId == 0)
                {
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        var registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                      .Where(registroDeAmarracao => registroDeAmarracao.AssetMovementsId == assetMovementsId)
                                                                      .ToList();

                        auditoriaIntegracaoVinculadaId = registrosDeAmarracao.Select(_registrosDeAmarracao => _registrosDeAmarracao.AuditoriaIntegracaoId)
                                                                             .FirstOrDefault();
                    }
                }

                assetId = movPatrimonial.AssetId;
                if (movPatrimonial.IsNotNull())
                {
                    tipoMovimentacao = obterEntidadeMovementType(movPatrimonial.MovementTypeId);
                    //if (existePendenciaNLAtivaEEhUnicoBPVinculado(assetId, assetMovementsId))
                    if (existePendenciaNLAtivaEEhUnicoBPVinculado02(assetId, assetMovementsId))
                    {
                        //this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoVinculadaId, out mensagemParaUsuario);
                        this.excluiNotaLancamentoPendencia__AuditoriaIntegracao02(assetId, assetMovementsId, out mensagemParaUsuario);
                        mensagemParaUsuario = string.Empty;

                    }
                    //else if (existePendenciaNLAtiva(assetId, assetMovementsId, movPatrimonial.MovementTypeId, tipoMovimentacao.GroupMovimentId))
                    else if (existePendenciaNLAtivaComMaisDeUmBPVinculado(assetId, assetMovementsId))
                    {
                        //this.atualizaDadosNotaLancamentoPendente(auditoriaIntegracaoVinculadaId, assetMovementsId, out mensagemParaUsuario);
                        this.atualizaDadosNotaLancamentoPendente02(assetMovementsId, out mensagemParaUsuario);
                    }
                }
            }


            //mensagem para exibicao ao usuario
            return Json(new { retorno = mensagemParaUsuario }, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("Metodo sob substituicao", true)]
        public bool atualizaDadosNotaLancamentoPendente(int auditoriaIntegracaoVinculadaId, int assetMovementsId, out string mensagemAoUsuario)
        {
            #region Variaveis
            bool pendenciaNLFoiAtualizada = false;
            bool ehEstorno = false;
            int numeroRegistrosManipulados = 0;
            decimal valorOriginal = 0.00m;
            decimal valorAtualizado = 0.00m;
            string numeroDocumentoSAM = null;
            string siglaBP = null;
            string chapaBP = null;
            IList<AssetMovements> listaMovPatrimonial = null;
            IntegracaoContabilizaSPController svcIntegracaoContabilizaSP = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoTemporarioParaCalculo = null;
            AuditoriaIntegracao auditoriaIntegracaoParaAtualizacao = null;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracaoParaExclusao = null;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracaoTemporario = null;
            TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.Desconhecido;
            #endregion Variaveis





            listaMovPatrimonial = new List<AssetMovements>() { obterEntidadeAssetMovements(assetMovementsId) };
            svcIntegracaoContabilizaSP = new IntegracaoContabilizaSPController();
            mensagemAoUsuario = null;
            if ((auditoriaIntegracaoVinculadaId > 0) && listaMovPatrimonial.HasElements())
            {
                siglaBP = listaMovPatrimonial[0].RelatedAssets.InitialName;
                chapaBP = listaMovPatrimonial[0].RelatedAssets.NumberIdentification;
                numeroDocumentoSAM = listaMovPatrimonial[0].NumberDoc;
                using (var contextoCamadaDados = new SAMContext())
                {
                    //registro AuditoriaIntegracao da pendencia, que vai ter o valor atualizado
                    auditoriaIntegracaoParaAtualizacao = contextoCamadaDados.AuditoriaIntegracoes
                                                                            .Where(_registroAuditoriaIntegracao => _registroAuditoriaIntegracao.Id == auditoriaIntegracaoVinculadaId)
                                                                            .FirstOrDefault();

                    //registro de amarracao que vai ser excluido
                    registroDeAmarracaoParaExclusao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                         .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoVinculadaId
                                                                                                     && _registroDeAmarracao.AssetMovementsId == assetMovementsId)
                                                                         .FirstOrDefault();

                    tipoNotaSIAFEM = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoVinculadaId);
                    ehEstorno = (auditoriaIntegracaoParaAtualizacao.NLEstorno.ToUpperInvariant() == "S");
                    //eu crio um extrato 'temporario' para obter o valor do BP que vai 'descontado' do valor do registro de AuditoriaIntegracao que vai ser atualizado
                    var dadosExtratoFicticioBP = svcIntegracaoContabilizaSP.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovPatrimonial, true, tipoNotaSIAFEM, ehEstorno);
                    if ((dadosExtratoFicticioBP.HasElements()) && (dadosExtratoFicticioBP.Count() == 1))
                    {
                        //AuditoriaIntegracaoId 'temporario'
                        var registroAuditoriaIntegracaoTemporarioParaCalculoId = dadosExtratoFicticioBP.FirstOrDefault().Item3;
                        //registro AuditoriaIntegracao 'temporario' (serah excluido ao final do processamento)
                        registroAuditoriaIntegracaoTemporarioParaCalculo = contextoCamadaDados.AuditoriaIntegracoes
                                                                                              .Where(_registroAuditoriaIntegracao => _registroAuditoriaIntegracao.Id == registroAuditoriaIntegracaoTemporarioParaCalculoId)
                                                                                              .FirstOrDefault();

                        //registro de amarracao 'temporario' que vai ser excluido ao final do processamento
                        registroDeAmarracaoTemporario = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                           .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoTemporarioParaCalculo.Id)
                                                                           .FirstOrDefault();

                        if (registroAuditoriaIntegracaoTemporarioParaCalculo.IsNotNull())
                        {
                            valorOriginal = auditoriaIntegracaoParaAtualizacao.ValorTotal.GetValueOrDefault();
                            auditoriaIntegracaoParaAtualizacao.ValorTotal -= registroAuditoriaIntegracaoTemporarioParaCalculo.ValorTotal;
                            valorAtualizado = auditoriaIntegracaoParaAtualizacao.ValorTotal.GetValueOrDefault();
                        }


                        if (registroDeAmarracaoParaExclusao.IsNotNull())
                        {
                            contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Remove(registroDeAmarracaoParaExclusao);
                            contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Remove(registroDeAmarracaoTemporario);
                            numeroRegistrosManipulados += contextoCamadaDados.SaveChanges();

                            contextoCamadaDados.AuditoriaIntegracoes.Remove(registroAuditoriaIntegracaoTemporarioParaCalculo);
                            numeroRegistrosManipulados += contextoCamadaDados.SaveChanges();
                        }
                    }
                }

                var cultureInfo = new CultureInfo("pt-BR");
                mensagemAoUsuario = String.Format(cultureInfo, "'Pendencia NL SIAFEM' de documento SAM '{0}' teve seu valor atualizado de {1:C} para {2:C} devido ao estorno do BP '{3}'-'{4}'", numeroDocumentoSAM, valorOriginal, valorAtualizado, siglaBP, chapaBP);
            }

            pendenciaNLFoiAtualizada = (numeroRegistrosManipulados > 0);
            return pendenciaNLFoiAtualizada;
        }

        public bool atualizaDadosNotaLancamentoPendente02(int assetMovementsId, out string mensagemAoUsuario)
        {
            #region Variaveis
            bool pendenciaNLFoiAtualizada = false;
            bool ehEstorno = false;
            int numeroRegistrosManipulados = 0;
            decimal valorOriginal = 0.00m;
            decimal valorAtualizado = 0.00m;
            string numeroDocumentoSAM = null;
            string siglaBP = null;
            string chapaBP = null;
            IList<AssetMovements> listaMovPatrimonial = null;
            IList<AuditoriaIntegracao> registrosXmlIntegracao = null;
            IntegracaoContabilizaSPController svcIntegracaoContabilizaSP = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoTemporarioParaCalculo = null;
            AuditoriaIntegracao auditoriaIntegracaoParaAtualizacao = null;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracaoParaExclusao = null;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracaoTemporario = null;
            NotaLancamentoPendenteSIAFEM pendenciaNLSiafemVinculada = null;
            TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.Desconhecido;
            string tempMsgUsuario = null;
            #endregion Variaveis



            mensagemAoUsuario = null;
            using (var contextoCamadaDados = new SAMContext())
            {
                registrosXmlIntegracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                            .Where(_registroDeAmarracao => _registroDeAmarracao.AssetMovementsId == assetMovementsId)
                                                            .Select(_registroDeAmarracao => obterEntidadeAuditoriaIntegracao(_registroDeAmarracao.AuditoriaIntegracaoId))
                                                            .ToList();

                if (registrosXmlIntegracao.HasElements())
                {
                    var cultureInfo = new CultureInfo("pt-BR");
                    foreach (var registroXmlIntegracao in registrosXmlIntegracao)
                    {

                        listaMovPatrimonial = new List<AssetMovements>() { obterEntidadeAssetMovements(assetMovementsId) };
                        svcIntegracaoContabilizaSP = new IntegracaoContabilizaSPController();
                        mensagemAoUsuario = null;
                        int auditoriaIntegracaoVinculadaId = registroXmlIntegracao.Id;
                        if ((auditoriaIntegracaoVinculadaId > 0) && listaMovPatrimonial.HasElements())
                        {
                            siglaBP = listaMovPatrimonial[0].RelatedAssets.InitialName;
                            chapaBP = listaMovPatrimonial[0].RelatedAssets.NumberIdentification;
                            numeroDocumentoSAM = listaMovPatrimonial[0].NumberDoc;
                            //using (var contextoCamadaDados = new SAMContext())
                            //{
                            //registro AuditoriaIntegracao da pendencia, que vai ter o valor atualizado
                            auditoriaIntegracaoParaAtualizacao = contextoCamadaDados.AuditoriaIntegracoes
                                                                                    .Where(_registroAuditoriaIntegracao => _registroAuditoriaIntegracao.Id == auditoriaIntegracaoVinculadaId)
                                                                                    .FirstOrDefault();

                            //registro de amarracao que vai ser excluido
                            registroDeAmarracaoParaExclusao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                 .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoVinculadaId
                                                                                                             && _registroDeAmarracao.AssetMovementsId == assetMovementsId)
                                                                                 .FirstOrDefault();

                            tipoNotaSIAFEM = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoVinculadaId);
                            ehEstorno = (auditoriaIntegracaoParaAtualizacao.NLEstorno.ToUpperInvariant() == "S");
                            //eu crio um extrato 'temporario' para obter o valor do BP que vai 'descontado' do valor do registro de AuditoriaIntegracao que vai ser atualizado
                            var dadosExtratoFicticioBP = svcIntegracaoContabilizaSP.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovPatrimonial, true, tipoNotaSIAFEM, ehEstorno);
                            if ((dadosExtratoFicticioBP.HasElements()) && (dadosExtratoFicticioBP.Count() == 1))
                            {
                                //AuditoriaIntegracaoId 'temporario'
                                var registroAuditoriaIntegracaoTemporarioParaCalculoId = dadosExtratoFicticioBP.FirstOrDefault().Item3;
                                //registro AuditoriaIntegracao 'temporario' (serah excluido ao final do processamento)
                                registroAuditoriaIntegracaoTemporarioParaCalculo = contextoCamadaDados.AuditoriaIntegracoes
                                                                                                      .Where(_registroAuditoriaIntegracao => _registroAuditoriaIntegracao.Id == registroAuditoriaIntegracaoTemporarioParaCalculoId)
                                                                                                      .FirstOrDefault();

                                //registro de amarracao 'temporario' que vai ser excluido ao final do processamento
                                registroDeAmarracaoTemporario = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                   .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == registroAuditoriaIntegracaoTemporarioParaCalculo.Id)
                                                                                   .FirstOrDefault();

                                if (registroAuditoriaIntegracaoTemporarioParaCalculo.IsNotNull())
                                {
                                    valorOriginal = auditoriaIntegracaoParaAtualizacao.ValorTotal.GetValueOrDefault();
                                    auditoriaIntegracaoParaAtualizacao.ValorTotal -= registroAuditoriaIntegracaoTemporarioParaCalculo.ValorTotal;
                                    valorAtualizado = auditoriaIntegracaoParaAtualizacao.ValorTotal.GetValueOrDefault();


                                    pendenciaNLSiafemVinculada = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                                    .Where(pendenciaNL => pendenciaNL.AuditoriaIntegracaoId == registroAuditoriaIntegracaoTemporarioParaCalculo.Id)
                                                                                    .FirstOrDefault();
                                }


                                if (registroDeAmarracaoParaExclusao.IsNotNull())
                                {
                                    if (pendenciaNLSiafemVinculada.IsNotNull())
                                        contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Remove(pendenciaNLSiafemVinculada);

                                    contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Remove(registroDeAmarracaoParaExclusao);
                                    contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Remove(registroDeAmarracaoTemporario);
                                    numeroRegistrosManipulados += contextoCamadaDados.SaveChanges();

                                    contextoCamadaDados.AuditoriaIntegracoes.Remove(registroAuditoriaIntegracaoTemporarioParaCalculo);
                                    numeroRegistrosManipulados += contextoCamadaDados.SaveChanges();
                                }
                            }
                        }

                        //mensagemAoUsuario = String.Format(cultureInfo, "'Pendencia NL SIAFEM' de documento SAM '{0}' teve seu valor atualizado de {1:C} para {2:C} devido ao estorno do BP '{3}'-'{4}'", numeroDocumentoSAM, valorOriginal, valorAtualizado, siglaBP, chapaBP);
                        tempMsgUsuario = String.Format(cultureInfo, "'Pendencia NL SIAFEM' de documento SAM '{0}' teve seu valor atualizado de {1:C} para {2:C} devido ao estorno do BP '{3}'-'{4}'", numeroDocumentoSAM, valorOriginal, valorAtualizado, siglaBP, chapaBP);
                        if (mensagemAoUsuario == null)
                            mensagemAoUsuario = tempMsgUsuario;
                        else if (!String.IsNullOrWhiteSpace(mensagemAoUsuario) && !mensagemAoUsuario.Contains(tempMsgUsuario))
                            mensagemAoUsuario += tempMsgUsuario;
                    }
                }
            }

            pendenciaNLFoiAtualizada = (numeroRegistrosManipulados > 0);
            return pendenciaNLFoiAtualizada;
        }

        private bool inativaNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            bool nlFoiInativada = false;
            int numeroRegistrosManipulados = 0;


            if (auditoriaIntegracaoId > 0)
            {
                //NotaLancamentoPendenteSIAFEM pendenciaNL = null;
                IList<NotaLancamentoPendenteSIAFEM> pendenciasNL = null;

                using (var contextoCamadaDados = new SAMContext())
                {
                    pendenciasNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                      .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                      .ToList();


                    foreach (var pendenciaNL in pendenciasNL)
                    {
                        pendenciaNL.StatusPendencia = 0;
                        contextoCamadaDados.Entry(pendenciaNL).State = EntityState.Modified;
                    }

                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                }
            }


            nlFoiInativada = (numeroRegistrosManipulados > 0);
            return nlFoiInativada;
        }

        private bool excluiNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId, bool efetuaDesvinculacaoMovimentacaoPatrimonial = true)
        {
            bool nlFoiExcluida = false;
            int numeroRegistrosManipulados = 0;
            NotaLancamentoPendenteSIAFEM pendenciaNL = null;
            IList<int> listaMovimentacaoPatrimonialId = null;


            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                     .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                     .FirstOrDefault();


                    if (pendenciaNL.IsNotNull())
                    {
                        contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Remove(pendenciaNL);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                    }

                    if (efetuaDesvinculacaoMovimentacaoPatrimonial)
                    {
                        listaMovimentacaoPatrimonialId = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                            .Where(registroDeAmarracao => registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                            .Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId)
                                                                            .ToList();
                        if (listaMovimentacaoPatrimonialId.HasElements())
                        {
                            var listaMovimentacaoPatrimonial = contextoCamadaDados.AssetMovements
                                                                                  .Where(_movPatrimonial => listaMovimentacaoPatrimonialId.Contains(_movPatrimonial.Id))
                                                                                  .ToList();
                            if (listaMovimentacaoPatrimonial.HasElements())
                            {
                                foreach (var movimentacaoPatrimonial in listaMovimentacaoPatrimonial)
                                    movimentacaoPatrimonial.NotaLancamentoPendenteSIAFEMId = null;

                                numeroRegistrosManipulados += contextoCamadaDados.SaveChanges();
                            }
                        }
                    }
                }
            }


            nlFoiExcluida = (numeroRegistrosManipulados > 0);
            return nlFoiExcluida;
        }
        [Obsolete("Metodo sob substituicao", true)]
        private bool excluiNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId, out string mensagemParaUsuario)
        {
            bool nlFoiInativada = false;
            int numeroRegistrosManipulados = 0;
            IList<int> assetMovementsIds = null;
            string numeroDocumentoSAM = null;
            string siglaBP = null;
            string chapaBP = null;
            decimal valorPendenciaNL = 0.00m;
            NotaLancamentoPendenteSIAFEM pendenciaNL = null;



            mensagemParaUsuario = null;
            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    var registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                  .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                  .ToList();

                    if (registrosDeAmarracao.HasElements() && registrosDeAmarracao.Count() >= 1)
                        assetMovementsIds = registrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).ToList();

                    if (registrosDeAmarracao.HasElements() && registrosDeAmarracao.Count() == 1)
                    {
                        //assetMovementsIds = registrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).ToList();
                        pendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                         .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                         .FirstOrDefault();

                        var movPatrimoniais = contextoCamadaDados.AssetMovements.Where(movPatrimonial => assetMovementsIds.Contains(movPatrimonial.Id)).ToList();
                        movPatrimoniais.ForEach(movPatrimonial => movPatrimonial.NotaLancamentoPendenteSIAFEMId = null);

                        contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.RemoveRange(registrosDeAmarracao);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();

                        if (pendenciaNL.IsNotNull())
                        {
                            contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Remove(pendenciaNL);
                            numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                        }

                    }
                }

                if (assetMovementsIds.HasElements())
                {
                    if (assetMovementsIds.Count() == 1)
                    {
                        var cultureInfo = new CultureInfo("pt-BR");
                        var movPatrimonial = obterEntidadeAssetMovements(assetMovementsIds.FirstOrDefault());
                        var auditoriaIntegracaoVinculada = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);

                        numeroDocumentoSAM = movPatrimonial.NumberDoc;
                        siglaBP = movPatrimonial.RelatedAssets.InitialName;
                        chapaBP = movPatrimonial.RelatedAssets.NumberIdentification;
                        valorPendenciaNL = auditoriaIntegracaoVinculada.ValorTotal.GetValueOrDefault();
                        mensagemParaUsuario = String.Format(cultureInfo, "A 'Pendencia NL SIAFEM' de documento SAM '{0} de valor {1:C} foi excluída devido ao estorno do BP '{2}'-'{3}'.", numeroDocumentoSAM, valorPendenciaNL, siglaBP, chapaBP);
                    }
                }
            }


            nlFoiInativada = (numeroRegistrosManipulados > 0);
            return nlFoiInativada;
        }
        private bool excluiNotaLancamentoPendencia__AuditoriaIntegracao02(int assetId, int assetMovementId, out string mensagemParaUsuario)
        {
            bool nlsForamInativadas = false;
            int numeroRegistrosManipulados = 0;
            string numeroDocumentoSAM = null;
            string siglaBP = null;
            string chapaBP = null;
            decimal valorPendenciaNL = 0.00m;
            AssetMovements movPatrimonial = null;
            AuditoriaIntegracao auditoriaIntegracaoVinculada = null;
            IList<NotaLancamentoPendenteSIAFEM> listaPendenciasNL = null;
            string tempMsgUsuario = null;


            mensagemParaUsuario = null;
            if ((assetId > 0) && (assetMovementId > 0))
            {
                var cultureInfo = new CultureInfo("pt-BR");
                using (var contextoCamadaDados = new SAMContext())
                {
                    var registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                              .Where(_registroDeAmarracao => _registroDeAmarracao.AssetId == assetId
                                                                                          && _registroDeAmarracao.AssetMovementsId == assetMovementId)
                                                              .ToList();
                    foreach (var registroDeAmarracao in registrosDeAmarracao)
                    {
                        if (registroDeAmarracao.AuditoriaIntegracaoId > 0)
                        {
                            listaPendenciasNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                   .Where(_pendenciaNL => _pendenciaNL.AuditoriaIntegracaoId == registroDeAmarracao.AuditoriaIntegracaoId)
                                                                   .ToList();

                            movPatrimonial = contextoCamadaDados.AssetMovements.Where(_movPatrimonial => _movPatrimonial.Id == registroDeAmarracao.AssetMovementsId).FirstOrDefault();
                            movPatrimonial.NotaLancamentoPendenteSIAFEMId = null;


                            if (listaPendenciasNL.HasElements())
                            {
                                contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.RemoveRange(listaPendenciasNL);
                                numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                            }


                            auditoriaIntegracaoVinculada = obterEntidadeAuditoriaIntegracao(registroDeAmarracao.AuditoriaIntegracaoId);
                            numeroDocumentoSAM = movPatrimonial.NumberDoc;
                            siglaBP = movPatrimonial.RelatedAssets.InitialName;
                            chapaBP = movPatrimonial.RelatedAssets.NumberIdentification;
                            valorPendenciaNL = auditoriaIntegracaoVinculada.ValorTotal.GetValueOrDefault();

                            tempMsgUsuario = String.Format(cultureInfo, "A 'Pendencia NL SIAFEM' de documento SAM '{0} de valor {1:C} foi excluída devido ao estorno do BP '{2}'-'{3}'.\n", numeroDocumentoSAM, valorPendenciaNL, siglaBP, chapaBP);
                            if (mensagemParaUsuario == null)
                                mensagemParaUsuario = tempMsgUsuario;
                            else if (!String.IsNullOrWhiteSpace(mensagemParaUsuario) && !mensagemParaUsuario.Contains(tempMsgUsuario))
                                mensagemParaUsuario += tempMsgUsuario;

                        }
                    }

                    if (registrosDeAmarracao.HasElements())
                    {
                        contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.RemoveRange(registrosDeAmarracao);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                    }
                }
            }

            nlsForamInativadas = (numeroRegistrosManipulados > 0);
            return nlsForamInativadas;
        }

        [HttpPost]
        public JsonResult GeraExtratos(int historico)
        {
            IList<string> extratoParaEnvioAoSIAFEM = new List<string>();
            int assetMovementId = 0;
            string auditorias = string.Empty;

            assetMovementId = historico;
            if (assetMovementId > 0)
            {
                var listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>() { this.obterEntidadeAssetMovements(assetMovementId) };
                var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                var dadosMsgsExtratoSIAFEM = svcIntegracaoSIAFEM.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovimentacoesPatrimoniaisEmLote, true, TipoNotaSIAFEM.Desconhecido, true);

                foreach (var dadosMsgExtratoSIAFEM in dadosMsgsExtratoSIAFEM)
                {
                    extratoParaEnvioAoSIAFEM.Add(dadosMsgExtratoSIAFEM.Item1);
                    auditorias += (dadosMsgExtratoSIAFEM.Item3.ToString() + ",");
                };

            }

            return Json(new { textos = extratoParaEnvioAoSIAFEM, auditorias = auditorias }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Prosseguir(string auditoria, string LoginSiafem, string SenhaSiafem, int assetId, int assetMovementId, int movimentTypeId, int groupMovimentId)
        {
            //Tratamento Contabiliza
            string msgInformeAoUsuario = null;

            //AQUI
            bool SeEhMsgSucesso = false;
            bool exibeBotaoGerarPendencia = false;
            bool exibeBotaoAbortar = false;
            bool NaoEstornou = false;
            if (!String.IsNullOrWhiteSpace(auditoria))
            {
                using (db = new MovimentoContext())
                {
                    MovementType tipoMovPatrimonial = null;
                    int numeroNLs = 1;
                    string nlLiquidacao = null;
                    string nlDepreciacao = null;
                    string nlReclassificacao = null;
                    IList<Tuple<string, string>> listaNLsGeradas = null;
                    string msgErroSIAFEM = null;
                    string msgErroSIAFEMSimplificado = null;
                    string msgNLGerada = null;
                    Tuple<string, string, string, string, bool> envioComandoGeracaoNL = null;
                    TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.Desconhecido;
                    string cpfUsuarioSessaoLogada = null;
                    int contadorNL = 0;
                    bool ehEstorno = false;
                    AuditoriaIntegracao registroAuditoriaIntegracao = null;
                    List<int> IdsAuditorias = null;



                    if (auditoria[auditoria.Length - 1] == ',')
                        IdsAuditorias = auditoria.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
                    else
                        IdsAuditorias = auditoria.Split(',').Select(Int32.Parse).ToList();

                    IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();
                    var ListaMovimentacoes = (from r in db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                              join am in db.AssetMovements on r.AssetMovementsId equals am.Id
                                              where IdsAuditorias.Contains(r.AuditoriaIntegracaoId)
                                              select am).Distinct().ToList();

                    var PrimeiraMovimentacao = ListaMovimentacoes.FirstOrDefault();
                    var movementTypeId = PrimeiraMovimentacao.MovementTypeId;
                    var NumberDoc = PrimeiraMovimentacao.NumberDoc;


                    tipoMovPatrimonial = db.MovementTypes.Where(movPatrimonial => movPatrimonial.Id == movementTypeId).FirstOrDefault();
                    if (IdsAuditorias.HasElements())
                    {
                        var usuarioLogado = UserCommon.CurrentUser();
                        var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                        cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);
                        numeroNLs = IdsAuditorias.Count();
                        listaNLsGeradas = new List<Tuple<string, string>>();


                        foreach (var auditoriaIntegracaoId in IdsAuditorias)
                        {
                            //tipoNotaSIAFEM = ((tipoNotaSIAFEM != TipoNotaSIAF.Desconhecido) ? tipoNotaSIAFEM : obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId));
                            tipoNotaSIAFEM = obterTipoNotaSIAFEM__AuditoriaIntegracao(auditoriaIntegracaoId);
                            registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                            ehEstorno = (registroAuditoriaIntegracao.NLEstorno.ToUpperInvariant() == "S");


                            #region NL Liquidacao
                            //NL Liquidacao
                            if (tipoMovPatrimonial.ContraPartidaContabilLiquida() && tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Liquidacao)
                            {
                                ++contadorNL;
                                envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, LoginSiafem, SenhaSiafem, ehEstorno, tipoNotaSIAFEM);
                                if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                {
                                    nlLiquidacao = envioComandoGeracaoNL.Item3; //captura a NL
                                    listaNLsGeradas.Add(Tuple.Create("(liquidação)", nlLiquidacao));
                                    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                                    continue; //segue o loop
                                }
                                else
                                {
                                    msgErroSIAFEMSimplificado = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                    break; //interrompe o loop
                                }
                            }
                            #endregion NL Liquidacao


                            #region NL Depreciacao
                            //NL Depreciacao
                            ++contadorNL;
                            var tipoMovimentacaoNaoFazLiquidacao = (String.IsNullOrWhiteSpace(nlLiquidacao) && !tipoMovPatrimonial.ContraPartidaContabilLiquida());
                            var temNL_Liquidacao = !String.IsNullOrWhiteSpace(nlLiquidacao);

                            //if ((((numeroNLs > 1) && temNL_Liquidacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                            //    tipoMovPatrimonial.ContraPartidaContabilDeprecia()) //...e deprecia?
                            if (((numeroNLs > 1) && tipoMovPatrimonial.ContraPartidaContabilDeprecia()) && tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Depreciacao)
                            {
                                envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, LoginSiafem, SenhaSiafem, ehEstorno, tipoNotaSIAFEM);
                                if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                {
                                    nlDepreciacao = envioComandoGeracaoNL.Item3; //captura a NL
                                    listaNLsGeradas.Add(Tuple.Create("(depreciação)", nlDepreciacao));
                                    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                                    continue; //segue o loop
                                }
                                else
                                {
                                    msgErroSIAFEMSimplificado = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                    break; //interrompe o loop
                                }
                            }
                            #endregion NL Depreciacao


                            #region NL Reclassificacao
                            //NL Reclassificacao
                            ++contadorNL;
                            var temNL_Depreciacao = !String.IsNullOrWhiteSpace(nlDepreciacao);
                            //if ((((numeroNLs > 1) && temNL_Depreciacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                            //    tipoMovPatrimonial.ContraPartidaContabilReclassifica()) //...e reclassifica?
                            if (((numeroNLs > 1) && tipoMovPatrimonial.ContraPartidaContabilReclassifica()) && tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Reclassificacao)
                            {
                                envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, LoginSiafem, SenhaSiafem, ehEstorno, tipoNotaSIAFEM);
                                if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                {
                                    nlReclassificacao = envioComandoGeracaoNL.Item3; //captura a NL
                                    listaNLsGeradas.Add(Tuple.Create("(reclassificação)", nlReclassificacao));
                                    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                                    continue; //segue o loop
                                }
                                else
                                {
                                    msgErroSIAFEMSimplificado = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                    break; //interrompe o loop
                                }
                            }
                            #endregion NL Reclassificacao

                            #region Tratamento Especifico para integracao SAM-Estoque/SAM-Patrimonio
                            if (tipoMovPatrimonial.Id == TipoMovimentacaoPatrimonial.IncorpIntegracaoSAMEstoque_SAMPatrimonio.GetHashCode())
                            {
                                var descricaoTipoNotaSIAFEM = ((tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Liquidacao) ? ("(liquidação)") : ((tipoNotaSIAFEM == TipoNotaSIAFEM.NL_Reclassificacao) ? ("(reclassificação)") : (String.Empty)));
                                envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, LoginSiafem, SenhaSiafem, ehEstorno, tipoNotaSIAFEM);
                                if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                {
                                    nlReclassificacao = envioComandoGeracaoNL.Item3; //captura a NL
                                    listaNLsGeradas.Add(Tuple.Create("(reclassificação)", nlReclassificacao));
                                    this.excluiNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId);
                                    continue; //segue o loop
                                }
                                else
                                {
                                    msgErroSIAFEMSimplificado = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                    break; //interrompe o loop
                                }
                            }
                            #endregion Tratamento Especifico para integracao SAM-Estoque/SAM-Patrimonio
                        }
                    }

                    //formatacao mensagem de erro
                    msgErroSIAFEM = ((!String.IsNullOrWhiteSpace(msgErroSIAFEMSimplificado)) ? String.Format("Erro retornado pela integração SAM/Contabiliza-SP: {0}", msgErroSIAFEMSimplificado) : null);

                    //formatacao mensagem de exibicao de NL's
                    if ((listaNLsGeradas.Count() == 1) && (numeroNLs == 1))
                    {
                        msgNLGerada = String.Format("NL retornada pela integração SAM/Contabiliza-SP: {0} {1}", listaNLsGeradas[0].Item2, listaNLsGeradas[0].Item1);
                    }
                    else if ((listaNLsGeradas.Count() > 1) && (numeroNLs == listaNLsGeradas.Count()))
                    {
                        bool jahTemNL = false;
                        foreach (var nLGerada in listaNLsGeradas)
                        {
                            jahTemNL = !String.IsNullOrWhiteSpace(msgNLGerada);
                            msgNLGerada += String.Format("{0}{1} {2}", (jahTemNL ? " e " : null), nLGerada.Item2, nLGerada.Item1);
                        }

                        msgNLGerada = String.Format("NLs retornadas pela integração SAM/Contabiliza-SP: {0}", msgNLGerada);
                    }
                    else if ((listaNLsGeradas.Count() == 1) && (numeroNLs > 1))
                    {
                        msgInformeAoUsuario = String.Format("Integração SAM/Contabiliza-SP retornou a NL {0} {1} e o erro '{2}'.", listaNLsGeradas[0].Item2, listaNLsGeradas[0].Item1, msgErroSIAFEMSimplificado);
                    }


                    if (String.IsNullOrWhiteSpace(msgInformeAoUsuario))
                    {
                        //sucesso OU erro?
                        msgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgNLGerada) ? msgNLGerada : msgErroSIAFEM);
                        //sucesso E erro?
                        if (!String.IsNullOrWhiteSpace(msgNLGerada) && !String.IsNullOrWhiteSpace(msgErroSIAFEM))
                            msgInformeAoUsuario = String.Format("Retorno de processamento:\n1) {0}\n2) {1}", msgNLGerada, msgErroSIAFEM);
                    }

                    bool houveErroSIAFEM = (!String.IsNullOrWhiteSpace(msgErroSIAFEM));
                    exibeBotaoAbortar = ((listaNLsGeradas.Count() == 0) && houveErroSIAFEM); //se na primeira jah parar (isso tendo gerado dois XMLs)
                    exibeBotaoGerarPendencia = ((listaNLsGeradas.Count() < numeroNLs) && houveErroSIAFEM); //se soh faltou uma NL e houve erro SIAFEM
                }
            }
            else
            {
                msgInformeAoUsuario = "Erro ao passar parametro!";
            }


            //AQUI
            SeEhMsgSucesso = !exibeBotaoGerarPendencia && !exibeBotaoAbortar;

            try
            {
                if(SeEhMsgSucesso)
                    EstornoDaMovimentacao(assetId, assetMovementId, movimentTypeId, groupMovimentId,"","");
            }
            catch (Exception e) {
                GravaLogErroSemRetorno(e);
                NaoEstornou = true;
            }

            DadosSIAFEMValidacaoMovimentacoes praTela = new DadosSIAFEMValidacaoMovimentacoes();
            praTela.mensagem = msgInformeAoUsuario;
            praTela.mensagemDeSuccesso = SeEhMsgSucesso;
            praTela.podeGerarPendenciaEstorno = exibeBotaoGerarPendencia;
            praTela.NaoEstornouBP = NaoEstornou;

            return PartialView("_RetornoMovimentoSIAFEM", praTela);
        }

        [HttpPost]
        public JsonResult Abortar(string auditoria)
        {
            List<int> IdsAuditorias = null;


            if (auditoria[auditoria.Length - 1] == ',')
                IdsAuditorias = auditoria.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
            else
                IdsAuditorias = auditoria.Split(',').Select(Int32.Parse).ToList();

            IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();
            if (IdsAuditorias.HasElements())
            {
                foreach (var auditoriaIntegracaoId in IdsAuditorias)
                {
                    this.excluiAuditoriaIntegracao(auditoriaIntegracaoId);
                }
            }


            //Tratamento Contabiliza
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AbortarAposProsseguirMovimento(string auditoria)
        {
            List<int> IdsAuditorias = null;


            if (auditoria[auditoria.Length - 1] == ',')
                IdsAuditorias = auditoria.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
            else
                IdsAuditorias = auditoria.Split(',').Select(Int32.Parse).ToList();

            IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();
            if (IdsAuditorias.HasElements())
            {
                foreach (var auditoriaIntegracaoId in IdsAuditorias)
                {
                    this.excluiAuditoriaIntegracao(auditoriaIntegracaoId);
                }
            }



            //Tratamento Contabiliza
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void GerarPendenciaSIAFEMEstorno(string auditorias)
        {
            //COPIADO DA ASSETSCONTROLLER. PODE ALTERAR COMO QUISER
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            List<int> IdsAuditorias = null;




            if (auditorias[auditorias.Length - 1] == ',')
                IdsAuditorias = auditorias.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
            else
                IdsAuditorias = auditorias.Split(',').Select(Int32.Parse).ToList();

            IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();
            foreach (var auditoriaIntegracaoId in IdsAuditorias)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                if (String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.NotaLancamento) && (!String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.MsgErro)))
                {
                    if (!this.existeNotaLancamentoPendencia__AuditoriaIntegracao(auditoriaIntegracaoId))
                        this.geraNotaLancamentoPendencia(auditoriaIntegracaoId);
                    else
                        this.atualizaMensagemErro_NotaLancamentoPendencia(auditoriaIntegracaoId, registroAuditoriaIntegracao.MsgErro);
                }
            }
        }

        private bool atualizaMensagemErro_NotaLancamentoPendencia(int auditoriaIntegracaoId, string msgErro)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM notaLancamentoPendente = null;
            int numeroRegistrosManipulados = 0;
            bool notaLancamentoPendenteAtualizada = false;

            if (auditoriaIntegracaoId > 0)
            {
                using (_contextoCamadaDados = new SAMContext())
                {
                    notaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                 .Where(pendenciaNL => pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                 .FirstOrDefault();

                    if (notaLancamentoPendente.IsNotNull())
                    {
                        notaLancamentoPendente.ErroProcessamentoMsgWS = msgErro;
                        numeroRegistrosManipulados += _contextoCamadaDados.SaveChanges();
                    }
                }
            }

            notaLancamentoPendenteAtualizada = (numeroRegistrosManipulados > 0);
            return notaLancamentoPendenteAtualizada;
        }

        #endregion

        #region Outros Métodos

        #region Usados na Validação
        private bool AssetEhAtivo(int assetId)
        {
            return (from a in db.Assets
                    where a.Id == assetId
                    select a.Status).FirstOrDefault();
        }

        public bool verificaNaoImplantacao(int IdUGEDestino)
        {
            bool naoImplantado = (from i in db.Institutions
                                  join b in db.BudgetUnits
                                  on i.Id equals b.InstitutionId
                                  join m in db.ManagerUnits
                                  on b.Id equals m.BudgetUnitId
                                  where m.Id == IdUGEDestino
                                  select (!i.flagImplantado || m.FlagTratarComoOrgao)).FirstOrDefault();

            return (naoImplantado);
        }

        private bool MovimentoPossuiFlagUGENaoUtilizada(int assetMovementId)
        {
            bool? flagUGENaoUtilizada = (from am in db.AssetMovements
                                         where am.Id == assetMovementId
                                         select am.flagUGENaoUtilizada).FirstOrDefault();

            return flagUGENaoUtilizada == true ? true : false;
        }

        private bool existePendenciaNLAtiva(int assetId, int assetMovementId, int movimentTypeId, int groupMovimentId)
        {
            bool existePendenicaNL = false;
            int auditoriaIntegracaoId = 0;
            SAMContext contextoCamadaDados = null;
            AssetMovements movPatrimonial = null;
            NotaLancamentoPendenteSIAFEM registroPendenciaNL = null;


            if ((assetId > 0) && (assetMovementId > 0) && (movimentTypeId > 0) && (groupMovimentId > 0))
            {
                contextoCamadaDados = new SAMContext();

                movPatrimonial = obterEntidadeAssetMovements(assetMovementId);
                if (movPatrimonial.IsNotNull())
                {
                    auditoriaIntegracaoId = movPatrimonial.AuditoriaIntegracaoId.GetValueOrDefault();
                    registroPendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                             .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                               && nlPendente.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                             .FirstOrDefault();
                }
            }

            existePendenicaNL = registroPendenciaNL.IsNotNull();
            return existePendenicaNL;
        }

        private bool existePendenciaNLAtivaEEhUnicoBPVinculado(int assetId, int assetMovementId)
        {
            bool existePendenciaNLAtiva = false;
            bool unicoBPDaPendenciaNL = false;
            bool existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = false;
            int auditoriaIntegracaoVinculadaId = 0;
            SAMContext contextoCamadaDados = null;
            AssetMovements movPatrimonial = null;
            NotaLancamentoPendenteSIAFEM registroPendenciaNL = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> listaRegistrosDeAmarracao = null;
            IList<int> movPatrimonialIds = null;


            if ((assetId > 0) && (assetMovementId > 0))
            {
                contextoCamadaDados = new SAMContext();
                movPatrimonial = obterEntidadeAssetMovements(assetMovementId);
                if (movPatrimonial.IsNotNull())
                {
                    auditoriaIntegracaoVinculadaId = movPatrimonial.AuditoriaIntegracaoId.GetValueOrDefault();
                    registroPendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                             .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                               && nlPendente.AuditoriaIntegracaoId == auditoriaIntegracaoVinculadaId)
                                                             .FirstOrDefault();

                    existePendenciaNLAtiva = registroPendenciaNL.IsNotNull();
                    if (existePendenciaNLAtiva)
                    {
                        listaRegistrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                       .Where(registroDeAmarracao => registroDeAmarracao.AssetId != assetId
                                                                                                  && registroDeAmarracao.AssetMovementsId != assetMovementId
                                                                                                  && registroDeAmarracao.AuditoriaIntegracaoId == registroPendenciaNL.AuditoriaIntegracaoId)
                                                                       .ToList();

                        if (listaRegistrosDeAmarracao.HasElements())
                        {
                            movPatrimonialIds = listaRegistrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).ToList();
                            existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = contextoCamadaDados.AssetMovements
                                                                                                        .Where(_movPatrimonial => movPatrimonialIds.Contains(_movPatrimonial.Id)
                                                                                                                               && _movPatrimonial.Status == true)
                                                                                                        .Count() > 0;
                        }
                    }
                }
            }

            unicoBPDaPendenciaNL = (existePendenciaNLAtiva && !existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva);
            return unicoBPDaPendenciaNL;
        }

        private bool existeNLSIAFEM(int assetId, int assetMovementId, bool nlDeEstorno = false)
        {
            bool existeNL = false;
            bool existeNLDeEstorno = false;
            SAMContext contextoCamadaDados = null;
            AssetMovements movPatrimonial = null;



            if ((assetId > 0) && (assetMovementId > 0))
            {
                contextoCamadaDados = new SAMContext();
                movPatrimonial = obterEntidadeAssetMovements(assetMovementId);
                if (movPatrimonial.IsNotNull())
                {
                    if (!nlDeEstorno)
                    {
                        existeNL = ((!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamento)) ||
                                    (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoDepreciacao)) ||
                                    (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoReclassificacao)));
                    }
                    else if (nlDeEstorno)
                    {
                        existeNLDeEstorno = ((!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoEstorno)) ||
                                             (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoDepreciacaoEstorno)) ||
                                             (!String.IsNullOrWhiteSpace(movPatrimonial.NotaLancamentoReclassificacaoEstorno)));
                    }
                }

                movPatrimonial = null;
                contextoCamadaDados = null;
            }

            existeNL = (existeNL || existeNLDeEstorno);
            return existeNL;
        }

        private bool existePendenciaNLAtivaComMaisDeUmBPVinculado(int assetId, int assetMovementId)
        {
            bool existePendenciaNLAtiva = false;
            bool unicoBPDaPendenciaNL = false;
            bool existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = false;
            int auditoriaIntegracaoVinculadaId = 0;
            SAMContext contextoCamadaDados = null;
            AssetMovements movPatrimonial = null;
            NotaLancamentoPendenteSIAFEM registroPendenciaNL = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> listaRegistrosDeAmarracao = null;
            IList<int> movPatrimonialIds = null;
            IList<int> auditoriaIntegracaoIds = null;


            if ((assetId > 0) && (assetMovementId > 0))
            {
                contextoCamadaDados = new SAMContext();
                movPatrimonial = obterEntidadeAssetMovements(assetMovementId);
                if (movPatrimonial.IsNotNull())
                {
                    auditoriaIntegracaoVinculadaId = movPatrimonial.AuditoriaIntegracaoId.GetValueOrDefault();
                    registroPendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                             .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                               && nlPendente.AuditoriaIntegracaoId == auditoriaIntegracaoVinculadaId)
                                                             .FirstOrDefault();

                    existePendenciaNLAtiva = registroPendenciaNL.IsNotNull();
                    if (existePendenciaNLAtiva)
                    {
                        listaRegistrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                       .Where(registroDeAmarracao => registroDeAmarracao.AssetId != assetId
                                                                                                  && registroDeAmarracao.AssetMovementsId != assetMovementId
                                                                                                  && registroDeAmarracao.AuditoriaIntegracaoId == registroPendenciaNL.AuditoriaIntegracaoId)
                                                                       .ToList();

                        if (listaRegistrosDeAmarracao.HasElements())
                        {
                            movPatrimonialIds = listaRegistrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).ToList();
                            existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = contextoCamadaDados.AssetMovements
                                                                                                        .Where(_movPatrimonial => movPatrimonialIds.Contains(_movPatrimonial.Id)
                                                                                                                               && _movPatrimonial.Status == true)
                                                                                                        .Count() > 0;
                        }
                    }
                    //'FORCANDO A BARRA' NA VERIFICACAO DE PENDENCIA ATIVAS
                    else
                    {
                        var listaRegistrosDeAmarracaoExclusivos = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                     .Where(registroDeAmarracao => registroDeAmarracao.AssetId == assetId
                                                                                                                && registroDeAmarracao.AssetMovementsId == assetMovementId)
                                                                                     .ToList();

                        if (listaRegistrosDeAmarracaoExclusivos.HasElements())
                        {
                            auditoriaIntegracaoIds = listaRegistrosDeAmarracaoExclusivos.Select(registroDeAmarracao => registroDeAmarracao.AuditoriaIntegracaoId).Distinct().ToList();
                            movPatrimonialIds = listaRegistrosDeAmarracaoExclusivos.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).Distinct().ToList();
                            var outrasMovPatrimonialIds = movPatrimonialIds.Where(_movPatrimonialId => _movPatrimonialId != assetMovementId).ToList();

                            var listaRegistrosDeAmarracaoOutrasMovPatrimoniais = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                                    .Where(registroDeAmarracao => registroDeAmarracao.AssetId != assetId
                                                                                                                               && registroDeAmarracao.AssetMovementsId != assetMovementId
                                                                                                                               && auditoriaIntegracaoIds.Contains(registroDeAmarracao.AuditoriaIntegracaoId))
                                                                                                    .ToList();

                            existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = (outrasMovPatrimonialIds.Count() > 0);
                            if (!existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva)
                            {
                                registroPendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                         .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                                           && auditoriaIntegracaoIds.Contains(nlPendente.AuditoriaIntegracaoId))
                                                                         .FirstOrDefault();

                                existePendenciaNLAtiva = registroPendenciaNL.IsNotNull();
                            }
                        }

                    }
                }
            }

            unicoBPDaPendenciaNL = (existePendenciaNLAtiva && existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva);
            return unicoBPDaPendenciaNL;
        }
        private bool existePendenciaNLAtivaEEhUnicoBPVinculado02(int assetId, int assetMovementId)
        {
            bool existePendenciaNLAtiva = false;
            bool unicoBPDaPendenciaNL = false;
            bool existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = false;
            int auditoriaIntegracaoVinculadaId = 0;
            SAMContext contextoCamadaDados = null;
            AssetMovements movPatrimonial = null;
            NotaLancamentoPendenteSIAFEM registroPendenciaNL = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> listaRegistrosDeAmarracao = null;
            IList<int> movPatrimonialIds = null;
            IList<int> auditoriaIntegracaoIds = null;


            if ((assetId > 0) && (assetMovementId > 0))
            {
                contextoCamadaDados = new SAMContext();
                movPatrimonial = obterEntidadeAssetMovements(assetMovementId);
                if (movPatrimonial.IsNotNull())
                {
                    auditoriaIntegracaoVinculadaId = movPatrimonial.AuditoriaIntegracaoId.GetValueOrDefault();
                    registroPendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                             .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                               && nlPendente.AuditoriaIntegracaoId == auditoriaIntegracaoVinculadaId)
                                                             .FirstOrDefault();

                    existePendenciaNLAtiva = registroPendenciaNL.IsNotNull();
                    if (existePendenciaNLAtiva)
                    {
                        listaRegistrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                       .Where(registroDeAmarracao => registroDeAmarracao.AssetId != assetId
                                                                                                  && registroDeAmarracao.AssetMovementsId != assetMovementId
                                                                                                  && registroDeAmarracao.AuditoriaIntegracaoId == registroPendenciaNL.AuditoriaIntegracaoId)
                                                                       .ToList();

                        if (listaRegistrosDeAmarracao.HasElements())
                        {
                            movPatrimonialIds = listaRegistrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).ToList();
                            existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = contextoCamadaDados.AssetMovements
                                                                                                        .Where(_movPatrimonial => movPatrimonialIds.Contains(_movPatrimonial.Id)
                                                                                                                               && _movPatrimonial.Status == true)
                                                                                                        .Count() > 0;
                        }
                    }
                    //'FORCANDO A BARRA' NA VERIFICACAO DE PENDENCIA ATIVAS
                    else
                    {
                        var listaRegistrosDeAmarracaoExclusivos = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                     .Where(registroDeAmarracao => registroDeAmarracao.AssetId == assetId
                                                                                                                && registroDeAmarracao.AssetMovementsId == assetMovementId)
                                                                                     .ToList();

                        if (listaRegistrosDeAmarracaoExclusivos.HasElements())
                        {
                            auditoriaIntegracaoIds = listaRegistrosDeAmarracaoExclusivos.Select(registroDeAmarracao => registroDeAmarracao.AuditoriaIntegracaoId).Distinct().ToList();
                            movPatrimonialIds = listaRegistrosDeAmarracaoExclusivos.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).Distinct().ToList();
                            var outrasMovPatrimonialIds = movPatrimonialIds.Where(_movPatrimonialId => _movPatrimonialId != assetMovementId).ToList();
                            var listaRegistrosDeAmarracaoOutrasMovPatrimoniais = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                                    .Where(registroDeAmarracao => registroDeAmarracao.AssetId != assetId
                                                                                                                               && registroDeAmarracao.AssetMovementsId != assetMovementId
                                                                                                                               && auditoriaIntegracaoIds.Contains(registroDeAmarracao.AuditoriaIntegracaoId))
                                                                                                    .ToList();

                            existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = (outrasMovPatrimonialIds.Count() > 0);
                            if (!existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva)
                            {
                                registroPendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                         .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                                           && auditoriaIntegracaoIds.Contains(nlPendente.AuditoriaIntegracaoId))
                                                                         .FirstOrDefault();

                                existePendenciaNLAtiva = registroPendenciaNL.IsNotNull();
                            }
                        }

                    }
                }
            }

            unicoBPDaPendenciaNL = (existePendenciaNLAtiva && !existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva);
            return unicoBPDaPendenciaNL;
        }
        private bool existeMaisDeUmaPendenciaNLAtivaMasEEhUnicoBPVinculado02(int assetId, int assetMovementId)
        {
            bool existePendenciaNLAtiva = false;
            bool unicoBPDaPendenciaNL = false;
            bool existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = false;
            int auditoriaIntegracaoVinculadaId = 0;
            SAMContext contextoCamadaDados = null;
            AssetMovements movPatrimonial = null;
            NotaLancamentoPendenteSIAFEM registroPendenciaNL = null;
            IList<NotaLancamentoPendenteSIAFEM> listaPendenciasNL = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> listaRegistrosDeAmarracao = null;
            IList<int> movPatrimonialIds = null;
            IList<int> auditoriaIntegracaoIds = null;


            if ((assetId > 0) && (assetMovementId > 0))
            {
                contextoCamadaDados = new SAMContext();
                movPatrimonial = obterEntidadeAssetMovements(assetMovementId);
                if (movPatrimonial.IsNotNull())
                {
                    auditoriaIntegracaoVinculadaId = movPatrimonial.AuditoriaIntegracaoId.GetValueOrDefault();
                    registroPendenciaNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                             .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                               && nlPendente.AuditoriaIntegracaoId == auditoriaIntegracaoVinculadaId)
                                                             .FirstOrDefault();

                    existePendenciaNLAtiva = registroPendenciaNL.IsNotNull();
                    if (existePendenciaNLAtiva)
                    {
                        listaRegistrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                       .Where(registroDeAmarracao => registroDeAmarracao.AssetId != assetId
                                                                                                  && registroDeAmarracao.AssetMovementsId != assetMovementId
                                                                                                  && registroDeAmarracao.AuditoriaIntegracaoId == registroPendenciaNL.AuditoriaIntegracaoId)
                                                                       .ToList();

                        if (listaRegistrosDeAmarracao.HasElements())
                        {
                            movPatrimonialIds = listaRegistrosDeAmarracao.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).ToList();
                            existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = contextoCamadaDados.AssetMovements
                                                                                                        .Where(_movPatrimonial => movPatrimonialIds.Contains(_movPatrimonial.Id)
                                                                                                                               && _movPatrimonial.Status == true)
                                                                                                        .Count() > 0;
                        }
                    }
                    //'FORCANDO A BARRA' NA VERIFICACAO DE PENDENCIA ATIVAS
                    else
                    {
                        var listaRegistrosDeAmarracaoExclusivos = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                     .Where(registroDeAmarracao => registroDeAmarracao.AssetId == assetId
                                                                                                                && registroDeAmarracao.AssetMovementsId == assetMovementId)
                                                                                     .ToList();

                        if (listaRegistrosDeAmarracaoExclusivos.HasElements())
                        {
                            auditoriaIntegracaoIds = listaRegistrosDeAmarracaoExclusivos.Select(registroDeAmarracao => registroDeAmarracao.AuditoriaIntegracaoId).Distinct().ToList();
                            movPatrimonialIds = listaRegistrosDeAmarracaoExclusivos.Select(registroDeAmarracao => registroDeAmarracao.AssetMovementsId).Distinct().ToList();
                            var outrasMovPatrimonialIds = movPatrimonialIds.Where(_movPatrimonialId => _movPatrimonialId != assetMovementId).ToList();

                            //existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = contextoCamadaDados.AssetMovements
                            //                                                                            .Where(_movPatrimonial => movPatrimonialIds.Contains(_movPatrimonial.Id)
                            //                                                                                                   && _movPatrimonial.Status == true)
                            //                                                                            .Count() > 0;
                            var listaRegistrosDeAmarracaoOutrasMovPatrimoniais = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                                                                                    .Where(registroDeAmarracao => registroDeAmarracao.AssetId != assetId
                                                                                                                               && registroDeAmarracao.AssetMovementsId != assetMovementId
                                                                                                                               && auditoriaIntegracaoIds.Contains(registroDeAmarracao.AuditoriaIntegracaoId))
                                                                                                    .ToList();

                            existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva = (outrasMovPatrimonialIds.Count() > 0);
                            if (!existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva)
                            {
                                listaPendenciasNL = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                         .Where(nlPendente => nlPendente.StatusPendencia == 1
                                                                                           && auditoriaIntegracaoIds.Contains(nlPendente.AuditoriaIntegracaoId))
                                                                         .ToList();

                                existePendenciaNLAtiva = (listaPendenciasNL.HasElements() && listaPendenciasNL.Count() > 1);
                            }
                        }

                    }
                }
            }

            unicoBPDaPendenciaNL = (existePendenciaNLAtiva && !existeAlgumaOutraMovPatrimonialNaMesmaPendenciaNLAtiva);
            return unicoBPDaPendenciaNL;
        }

        private bool NLsJaForamEstornada(int AssetMovementsId) {

            AssetMovements historico = db.AssetMovements.Find(AssetMovementsId);

            if (
                 !(string.IsNullOrEmpty(historico.NotaLancamento) || string.IsNullOrWhiteSpace(historico.NotaLancamento)) &&
                 (string.IsNullOrEmpty(historico.NotaLancamentoEstorno) || string.IsNullOrWhiteSpace(historico.NotaLancamentoEstorno))
               )
                return false;

            if (
                !(string.IsNullOrEmpty(historico.NotaLancamentoDepreciacao) || string.IsNullOrWhiteSpace(historico.NotaLancamentoDepreciacao)) &&
                (string.IsNullOrEmpty(historico.NotaLancamentoDepreciacaoEstorno) || string.IsNullOrWhiteSpace(historico.NotaLancamentoDepreciacaoEstorno))
               )
                return false;

            if (
                !(string.IsNullOrEmpty(historico.NotaLancamentoReclassificacao) || string.IsNullOrWhiteSpace(historico.NotaLancamentoReclassificacao)) &&
                (string.IsNullOrEmpty(historico.NotaLancamentoReclassificacaoEstorno) || string.IsNullOrWhiteSpace(historico.NotaLancamentoReclassificacaoEstorno))
               )
                return false;

            //Se não passou por nenhuma das condições acima, então o método retorna como se o estorno tivesse ocorrido ok
            return true;
        }

        #endregion

        #region Usados na Exclusão
        private void RetiraReferenciaNaOrigem(int assetId)
        {
            var _user = UserCommon.CurrentUser();

            var assetMovimentoDB = (from am in db.AssetMovements
                                    where am.AssetTransferenciaId == assetId
                                    select am).FirstOrDefault();

            assetMovimentoDB.AssetTransferenciaId = null;

            db.Entry(assetMovimentoDB).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void ReativarMovimentoEAssetDeOutraUGE(int assetId)
        {
            /* Atribui o status ativo novamente para o movimento e limpa o AssetTransferenciaId para que 
            fique novamente pendente para o cliente que transferiu o item anteriormente */
            var _user = UserCommon.CurrentUser();

            var assetMovimentoDB = (from am in db.AssetMovements
                                    where am.AssetTransferenciaId == assetId
                                    select am).FirstOrDefault();

            assetMovimentoDB.Status = true;
            assetMovimentoDB.AssetTransferenciaId = null;

            db.Entry(assetMovimentoDB).State = EntityState.Modified;
            db.SaveChanges();
            //---------------------------------------------------------------------------------------

            //Reativa o último assetsMovimento antigo -----------------------------------------------
            var assetsMovimentoIdDB = (from am in db.AssetMovements
                                       where am.AssetId == assetMovimentoDB.AssetId &&
                                             am.FlagEstorno == null
                                       select am);

            var assetsMovimentoDB = (from a in db.AssetMovements
                                     where a.Id == assetsMovimentoIdDB.Max(x => x.Id)
                                     select a).FirstOrDefault();

            assetsMovimentoDB.Status = true;

            db.Entry(assetsMovimentoDB).State = EntityState.Modified;
            db.SaveChanges();

            //---------------------------------------------------------------------------------------

            //Reativa o asset -----------------------------------------------------------------------
            ReativarAsset(assetMovimentoDB.AssetId);
            //---------------------------------------------------------------------------------------
        }

        private void ReativarAsset(int assetId)
        {
            var assetDB = (from a in db.Assets
                           where a.Id == assetId
                           select a).FirstOrDefault();

            assetDB.Status = true;

            db.Entry(assetDB).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void RegistraBPExcluido(int assetId, int assetMovementId, int usuario)
        {

            Asset BP = (from a in db.Assets where a.Id == assetId select a).FirstOrDefault();

            AssetMovements incorporacao = (from am in db.AssetMovements where am.Id == assetMovementId select am).FirstOrDefault();

            BPsExcluidos bpExcluido = new BPsExcluidos();
            bpExcluido.InitialId = BP.InitialId;
            bpExcluido.SiglaInicial = BP.InitialName;
            bpExcluido.Chapa = BP.ChapaCompleta;
            bpExcluido.ItemMaterial = BP.MaterialItemCode;
            bpExcluido.GrupoMaterial = BP.MaterialGroupCode;
            bpExcluido.ValorAquisicao = BP.ValueAcquisition;
            bpExcluido.DataAquisicao = BP.AcquisitionDate;
            bpExcluido.DataIncorporacao = BP.MovimentDate;
            bpExcluido.flagAcervo = BP.flagAcervo == null ? false : Convert.ToBoolean(BP.flagAcervo);
            bpExcluido.flagTerceiro = BP.flagTerceiro == null ? false : Convert.ToBoolean(BP.flagTerceiro);
            bpExcluido.flagDecretoSEFAZ = BP.flagDecreto == null ? false : Convert.ToBoolean(BP.flagDecreto);
            bpExcluido.TipoIncorporacao = incorporacao.MovementTypeId;
            bpExcluido.StateConservationId = incorporacao.StateConservationId;
            bpExcluido.ManagerUnitId = incorporacao.ManagerUnitId;
            bpExcluido.AdministrativeUnitId = incorporacao.AdministrativeUnitId;
            bpExcluido.ResponsibleId = incorporacao.ResponsibleId;
            bpExcluido.Processo = incorporacao.NumberPurchaseProcess;
            bpExcluido.NotaLancamento = incorporacao.NotaLancamento;
            bpExcluido.NotaLancamentoEstorno = incorporacao.NotaLancamentoEstorno;
            bpExcluido.NotaLancamentoDepreciacao = incorporacao.NotaLancamentoDepreciacao;
            bpExcluido.NotaLancamentoDepreciacaoEstorno = incorporacao.NotaLancamentoDepreciacaoEstorno;
            bpExcluido.NotaLancamentoReclassificacao = incorporacao.NotaLancamentoReclassificacao;
            bpExcluido.NotaLancamentoReclassificacaoEstorno = incorporacao.NotaLancamentoReclassificacaoEstorno;
            bpExcluido.Observacoes = null;
            bpExcluido.NumeroDocumento = incorporacao.NumberDoc ?? BP.NumberDoc;
            bpExcluido.DataAcao = DateTime.Now;
            bpExcluido.LoginAcao = usuario;

            db.BPsExcluidos.Add(bpExcluido);
            db.SaveChanges();

            //remove registros da tabela AssetMovements e suas depêndencias
            var historicos = (from am in db.AssetMovements where am.AssetId == assetId select am.Id.ToString());

            foreach (string historicoId in historicos)
            {
                //db.NotaLancamentoPendenteSIAFEMs.RemoveRange(db.NotaLancamentoPendenteSIAFEMs.Where(n => n.AssetMovementId == historico.Id));
                db.Database.ExecuteSqlCommand("delete from [dbo].[Closing] where AssetMovementsId = " + historicoId);
                db.Database.ExecuteSqlCommand("delete from [dbo].[LogAlteracaoDadosBP] where AssetMovementId = " + historicoId);
                //db.Closings.RemoveRange(db.Closings.Where(c => c.AssetMovementsId == historico.Id));
                db.Database.ExecuteSqlCommand("delete from [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] where AssetMovementsId = " + historicoId);
                db.Database.ExecuteSqlCommand("delete from [dbo].[AssetMovements] where Id = " + historicoId);
            }

            db.SaveChanges();

            string assetIdString = assetId.ToString();
            //remove registros relacionados ao registro da tabela Asset
            db.Database.ExecuteSqlCommand("delete from [dbo].[AssetNumberIdentification] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Repair] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[ItemInventario] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Exchange] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Closing] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[LogAlteracaoDadosBP] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[HistoricoValoresDecreto] where AssetId = " + assetIdString);
            db.Database.ExecuteSqlCommand("delete from [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] where AssetId = " + assetIdString);

            if (incorporacao.MovementTypeId != (int)EnumMovimentType.Transferencia &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.Doacao &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorporacaoPorTransferencia &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorporacaoPorDoacao &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpDoacaoIntraNoEstado &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao &&
                incorporacao.MovementTypeId != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia &&
                BP.AssetStartId != null)
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MonthlyDepreciation] where AssetStartId = " + BP.AssetStartId.ToString());
            }

            //remove o registro da Asset
            db.Database.ExecuteSqlCommand("delete from [dbo].[Asset] where Id = " + assetIdString);

            db.SaveChanges();
        }

        private void EstornaAceiteAutomatico(int assetId, int assetMovementsId, int usuario)
        {
            int assetIdAposAceite = (int)db.AssetMovements.Find(assetMovementsId).AssetTransferenciaId;
            int assetMovementIdAposAceite = db.AssetMovements.Where(x => x.AssetId == assetIdAposAceite).Select(x => x.Id).FirstOrDefault();
            RegistraBPExcluido(assetIdAposAceite, assetMovementIdAposAceite, usuario);
            EstornaMovimento(assetId, assetMovementsId, usuario);
            Asset asset = (from a in db.Assets where a.Id == assetId select a).AsNoTracking().FirstOrDefault();
            asset.Status = true;
            db.Entry(asset).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void EstornaMovimento(int assetId, int assetMovementId, int userID)
        {
            var assetMovimentDB = (from am in db.AssetMovements
                                   where am.Id == assetMovementId
                                   select am).FirstOrDefault();

            assetMovimentDB.Status = false;
            assetMovimentDB.FlagEstorno = true;
            assetMovimentDB.DataEstorno = DateTime.Now;
            assetMovimentDB.LoginEstorno = userID;

            db.Entry(assetMovimentDB).State = EntityState.Modified;
            db.SaveChanges();


            var assetsMovementSemUltimoMovimento = (from am in db.AssetMovements
                                                    where am.AssetId == assetId &&
                                                          am.FlagEstorno == null &&
                                                          am.Id != assetMovementId
                                                    select am.Id).OrderByDescending(am => am).FirstOrDefault();


            if (assetsMovementSemUltimoMovimento != 0)
            {
                var assetMovementPenultimoDB = (from a in db.AssetMovements
                                                where a.Id == assetsMovementSemUltimoMovimento
                                                select a).FirstOrDefault();

                assetMovementPenultimoDB.Status = true;

                db.Entry(assetMovementPenultimoDB).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region Metodos Importados de outras Controllers
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
                string name = "EstornoController.obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria";
                GravaLogErro(messageErro, stackTrace, name);
                throw excErroRuntime;
            }

            return listaMovPatrimoniais;
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
        private TipoNotaSIAFEM obterTipoNotaSIAFEM__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            TipoNotaSIAFEM tipoNotaSIAF = TipoNotaSIAFEM.Desconhecido;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;


            if (auditoriaIntegracaoId > 0)
            {
                registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                switch (registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant())
                {
                    case "ENTRADA":
                    case "SAIDA":
                    case "SAÍDA":           { tipoNotaSIAF = TipoNotaSIAFEM.NL_Liquidacao; } break;
                    case "DEPRECIACAO":
                    case "DEPRECIAÇÃO":     { tipoNotaSIAF = TipoNotaSIAFEM.NL_Depreciacao; } break;
                    case "RECLASSIFICACAO":
                    case "RECLASSIFICAÇÃO": { tipoNotaSIAF = TipoNotaSIAFEM.NL_Reclassificacao; } break;
                }
            }



            return tipoNotaSIAF;
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
        private AssetMovements obterEntidadeAssetMovements(int assetMovementsId)
        {
            AssetMovements objEntidade = null;


            if (assetMovementsId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AssetMovements.Include("RelatedAssets")
                                                     .Where(movPatrimonial => movPatrimonial.Id == assetMovementsId)
                                                     .FirstOrDefault();
                }

            return objEntidade;
        }
        public int obterAnoMesReferenciaMovimentacaoPatrimonial(int assetMovementsId)
        {
            AssetMovements objEntidade = null;
            int anoMesReferenciaMovimentacaoPatrimonial = 0;
            string _anoMesReferenciaMovimentacaoPatrimonial = null;

            if (assetMovementsId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AssetMovements
                                                     .Where(movPatrimonial => movPatrimonial.Id == assetMovementsId)
                                                     .FirstOrDefault();
                    if (objEntidade.IsNotNull())
                    {
                        _anoMesReferenciaMovimentacaoPatrimonial = objEntidade.MovimentDate.ToString("yyyyMM");
                        Int32.TryParse(_anoMesReferenciaMovimentacaoPatrimonial, out anoMesReferenciaMovimentacaoPatrimonial);
                    }
                }

            return anoMesReferenciaMovimentacaoPatrimonial;
        }
        private bool excluiAuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            bool auditoriaIntegracaoFoiExcluida = false;
            int numeroRegistrosManipulados = 0;
            AuditoriaIntegracao registroAuditoriaIntegracaoTemporaria = null;
            IList<NotaLancamentoPendenteSIAFEM> pendenciasNLSiafemVinculadas = null;


            if (auditoriaIntegracaoId > 0)
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    var registrosDeAmarracao = contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                              .Where(_registroDeAmarracao => _registroDeAmarracao.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                              .ToList();


                    registroAuditoriaIntegracaoTemporaria = contextoCamadaDados.AuditoriaIntegracoes
                                                                               .Where(_registroAuditoriaIntegracao => _registroAuditoriaIntegracao.Id == auditoriaIntegracaoId)
                                                                               .FirstOrDefault();

                    pendenciasNLSiafemVinculadas = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                      .Where(pendenciaNL => pendenciaNL.AuditoriaIntegracaoId == registroAuditoriaIntegracaoTemporaria.Id)
                                                                      .ToList();

                    if (registrosDeAmarracao.HasElements())
                    {
                        contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.RemoveRange(registrosDeAmarracao);
                        numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();

                        if (registroAuditoriaIntegracaoTemporaria.IsNotNull())
                        {
                            if (pendenciasNLSiafemVinculadas.HasElements())
                                contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.RemoveRange(pendenciasNLSiafemVinculadas);

                            contextoCamadaDados.AuditoriaIntegracoes.Remove(registroAuditoriaIntegracaoTemporaria);
                            numeroRegistrosManipulados = +contextoCamadaDados.SaveChanges();
                        }
                    }
                }
            }


            auditoriaIntegracaoFoiExcluida = (numeroRegistrosManipulados > 0);
            return auditoriaIntegracaoFoiExcluida;
        }
        private bool criaOuRegeraNotaLancamentoPendenciaParaMovimentacaoVinculada(IEnumerable<AssetMovements> listaMovimentacoesPatrimoniais, TipoNotaSIAFEM tipoNotaSIAFEM = TipoNotaSIAFEM.Desconhecido)
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
                            if (tipoNotaSIAFEM != TipoNotaSIAFEM.Desconhecido)
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
                            else if (tipoNotaSIAFEM == TipoNotaSIAFEM.Desconhecido)
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
        private bool existeNotaLancamentoPendencia__AuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            bool existeNotaLancamentoPendente = false;

            if (auditoriaIntegracaoId > 0)
            {
                using (_contextoCamadaDados = new SAMContext())
                {
                    existeNotaLancamentoPendente = _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                                       .Where(pendenciaNL => pendenciaNL.AuditoriaIntegracaoId == auditoriaIntegracaoId)
                                                                       .FirstOrDefault()
                                                                       .IsNotNull();
                }
            }

            return existeNotaLancamentoPendente;
        }
        private bool geraNotaLancamentoPendencia(int auditoriaIntegracaoId)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracao = null;
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            IList<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> listadeRegistrosDeAmarracao = null;
            TipoNotaSIAFEM tipoAgrupamentoNotaLancamento = TipoNotaSIAFEM.Desconhecido;
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
                                                                                            AssetMovementIds = registroAuditoriaIntegracao.AssetMovementIds,
                                                                                            StatusPendencia = 1,
                                                                                            ErroProcessamentoMsgWS = registroAuditoriaIntegracao.MsgErro,
                                                                                        };


                        idsMovPatrimoniais = ((idsMovPatrimoniais.HasElements()) ? idsMovPatrimoniais : listaMovimentacoesPatrimoniais.Select(movPatrimonial => movPatrimonial.Id).ToArray());
                        using (_contextoCamadaDados = new SAMContext())
                        {
                            var _listaMovimentacoesPatrimoniais = _contextoCamadaDados.AssetMovements.Where(movPatrimonial => idsMovPatrimoniais.Contains(movPatrimonial.Id)).ToList();
                            var _registroAuditoriaIntegracao = _contextoCamadaDados.AuditoriaIntegracoes.Where(rowAuditoriaintegracao => rowAuditoriaintegracao.Id == registroAuditoriaIntegracao.Id).FirstOrDefault();

                            _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                            numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();

                            _listaMovimentacoesPatrimoniais.ForEach(movPatrimonial =>  {
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
        #endregion Metodos Importados de outras Controllers

        #endregion

    }
}