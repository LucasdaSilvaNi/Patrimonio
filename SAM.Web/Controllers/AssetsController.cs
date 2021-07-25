using AutoMapper;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using PagedList;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Integracao.SIAF.Configuracao;
using Sam.Integracao.SIAF.Core;
using Sam.Integracao.SIAF.Mensagem.Interface;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Xml;
using LinqKit;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using SAM.Web.Incorporacao;

namespace SAM.Web.Controllers
{
    public class AssetsController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;

        public void getHierarquiaPerfil()
        {
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
            }
            else
            {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
            }
        }

        #region IncorporaInventario
        [HttpGet]
        public ActionResult IncorporarInventario(int Id)
        {
            string msgErroSIAFISICO;

            try
            {
                using (db = new SAMContext())
                {
                    getHierarquiaPerfil();

                    if (UGEEstaComPendenciaSIAFEMNoFechamento(_managerUnitId))
                        return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                    ItemInventario itemInventario = (from i in db.ItemInventarios
                                                                 .Include("RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit")
                                                     where i.Id == Id
                                                     select i).FirstOrDefault();

                    LoadSelectElementsInventario(itemInventario);

                    getHierarquiaPerfil();
                    var registroSiglaBP = obterDadosSigla(itemInventario.InitialName);
                    BuscaItensMateriais buscaItensMateriais = new BuscaItensMateriais();
                    MaterialItemViewModel materialItemViewModel;
                    msgErroSIAFISICO = buscaItensMateriais.BuscaMensagemValidaCodigoItemMaterial(itemInventario.Item);

                    if (!string.IsNullOrEmpty(msgErroSIAFISICO) && !string.IsNullOrWhiteSpace(msgErroSIAFISICO))
                    {
                        materialItemViewModel = buscaItensMateriais.GetItemMaterial(itemInventario.Item, ref msgErroSIAFISICO);
                        if(materialItemViewModel == null)
                            return MensagemErro(msgErroSIAFISICO);
                    }
                    else {
                        materialItemViewModel = buscaItensMateriais.GetItemMaterialBD(itemInventario.Item);
                    }

                    
                    ShortDescriptionItem shortDescriptionItem = VerificarDescricaoResumida(materialItemViewModel.Description);

                    //Conta Contabil - ver como amarrar com GrupoMaterial
                    BuscaContasContabeis buscaContas = new BuscaContasContabeis();
                    ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaAuxiliarPorGrupoMaterial(materialItemViewModel.MaterialGroupCode), "Id", "Description", null);

                    AssetViewModel _asset = new AssetViewModel()
                    {
                        NumberIdentification = itemInventario.Code,

                        InitialId = (registroSiglaBP.IsNotNull() ? registroSiglaBP.Id : -1),
                        MovementTypeId = EnumMovimentType.IncorporacaoDeInventarioInicial.GetHashCode(),
                        InstitutionId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId,
                        BudgetUnitId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id,
                        ManagerUnitId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.Id,
                        AdministrativeUnitId = itemInventario.RelatedInventario.UaId,
                        SectionId = itemInventario.RelatedInventario.DivisaoId,
                        ResponsibleId = itemInventario.RelatedInventario.ResponsavelId,

                        materialItemPesquisa = itemInventario.Item,
                        MaterialItemCode = materialItemViewModel.Code,
                        MaterialItemCodeAuxiliar = materialItemViewModel.Code,
                        MaterialItemDescription = materialItemViewModel.Description,
                        MaterialItemDescriptionAuxiliar = materialItemViewModel.Description,
                        RelatedShortDescriptionItem = shortDescriptionItem,


                        MaterialGroupCode = materialItemViewModel.MaterialGroupCode,
                        MaterialGroupDescription = materialItemViewModel.MaterialGroupDescription,
                        LifeCycle = materialItemViewModel.LifeCycle.Value,
                        ResidualValue = materialItemViewModel.ResidualValue.Value,
                        RateDepreciationMonthly = materialItemViewModel.RateDepreciationMonthly.Value,

                        InventarioId = itemInventario.InventarioId,
                        ItemInventarioId = itemInventario.Id,

                        MetodoGet = true
                    };

                    return View(_asset);
                }
            }
            catch (Exception e) {
                return MensagemErro(CommonMensagens.PadraoException, e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncorporarInventario(AssetViewModel assetViewModel)
        {
            try
            {
                assetViewModel.MetodoGet = false;
                assetViewModel.DiferenciacaoChapa = (assetViewModel.DiferenciacaoChapa ?? "");
                assetViewModel.DiferenciacaoChapaAntiga = (assetViewModel.DiferenciacaoChapaAntiga ?? "");

                if (   assetViewModel.MovementTypeId != (int)EnumMovimentType.IncorporacaoDeInventarioInicial
                    && assetViewModel.MovementTypeId != (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos)
                {
                    ModelState.AddModelError("MovementTypeId", "Tipo de Incorporação inválida");
                }

                // Tratamento Acervo e Terceiro: Tipo de Movimento - Incorporacao de Inventario Inicial
                if (assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorporacaoDeInventarioInicial)
                {
                    if (assetViewModel.checkFlagAcervo)
                    {
                        ValidaCamposValorAtualizado(assetViewModel);
                    }

                    if (assetViewModel.checkFlagTerceiro) {
                        ModelState.AddModelError("MovementTypeId", "Tipo de Incorporação inválida");
                    }

                    if (assetViewModel.checkFlagDecretoSefaz)
                    {
                        if (string.IsNullOrEmpty(assetViewModel.ValueUpdateModel) || string.IsNullOrWhiteSpace(assetViewModel.ValueUpdateModel))
                        {
                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado é obrigatório");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(assetViewModel.ValueAcquisitionModel) && !string.IsNullOrWhiteSpace(assetViewModel.ValueAcquisitionModel))
                            {
                                if (Convert.ToDecimal(assetViewModel.ValueUpdateModel) < 10)
                                {
                                    ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado de um decreto não pode ser inferior a R$10,00");
                                }
                                else if (Convert.ToDecimal(assetViewModel.ValueUpdateModel) <= Convert.ToDecimal(assetViewModel.ValueAcquisitionModel))
                                {
                                    ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado de um decreto deve ser superior ao Valor de Aquisição");
                                }
                            }
                        }
                    }

                    //Incorporacao de Item de Inventário do tipo "Inventário Inicial..." não permite tipo Terceiro
                    assetViewModel.checkFlagTerceiro = false;
                    assetViewModel.OutSourcedId = null;
                    assetViewModel.CPFCNPJDoTerceiro = null;
                    assetViewModel.NomeDoTerceiro = null;
                }

                if (assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos){
                    if (!assetViewModel.checkFlagTerceiro)
                    {
                        ModelState.AddModelError("MovementTypeId", "Tipo de Incorporação inválida para Inventário de Terceiro");
                    }
                    else
                        ValidaCamposValorAtualizado(assetViewModel);


                    //Incorporacao de Item de Inventário do tipo "COMODATO CONCEDIDOS..." só permite tipo Terceiro
                    assetViewModel.checkFlagAcervo = false;
                    assetViewModel.checkFlagDecretoSefaz = false;
                }

                if (!string.IsNullOrEmpty(assetViewModel.NumberDoc) && assetViewModel.NumberDoc == "0")
                {
                    ModelState.AddModelError("NumberDoc", "Não é permitido informar 0, favor verificar!");
                }

                if (!string.IsNullOrEmpty(assetViewModel.NumberDoc) && assetViewModel.NumberDoc.Length > 14)
                {
                    ModelState.AddModelError("NumberDoc", "Digite no máximo 14 caracteres");
                }

                using (db = new SAMContext())
                {
                    //verifica se possui conta contábil
                    ValidaContaContabil(assetViewModel);
                    //fim verifica se possui conta contábil
                }

                ValidaDescricaoResumida(assetViewModel);

                if (ModelState.IsValid)
                {
                    using (db = new SAMContext())
                    {
                        if (UGEEstaComPendenciaSIAFEMNoFechamento(assetViewModel.ManagerUnitId))
                            return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                        if (!assetViewModel.checkFlagAcervo && !assetViewModel.checkFlagTerceiro)
                        {
                            if (decimal.Parse(assetViewModel.ValueAcquisitionModel) < 10)
                            {
                                ModelState.AddModelError("ValueAcquisitionModel", "Valor de aquisição não pode ser menor que 10,00");
                                LoadSelectElementsInventarioPorId(assetViewModel.ItemInventarioId);
                                CarregaViewBagContasContabeis(assetViewModel);
                                return View(assetViewModel);
                            }
                        }

                        var mensagemErro = ValidaDataDeAcquisicao(assetViewModel.AcquisitionDate, assetViewModel.ManagerUnitId, assetViewModel.MovementTypeId);
                        if (mensagemErro != string.Empty)
                        {
                            ModelState.AddModelError("AcquisitionDate", mensagemErro);
                            LoadSelectElementsInventarioPorId(assetViewModel.ItemInventarioId);
                            CarregaViewBagContasContabeis(assetViewModel);
                            return View(assetViewModel);
                        }

                        mensagemErro = ValidaDataDeIncorporacao(assetViewModel.MovimentDate, assetViewModel.ManagerUnitId, assetViewModel.MovementTypeId, assetViewModel.AcquisitionDate);
                        if (mensagemErro != string.Empty)
                        {
                            ModelState.AddModelError("MovimentDate", mensagemErro);
                            LoadSelectElementsInventarioPorId(assetViewModel.ItemInventarioId);
                            CarregaViewBagContasContabeis(assetViewModel);
                            return View(assetViewModel);
                        }

                        if (VerificaSeDuplicaraBPs(assetViewModel))
                        {
                            LoadSelectElementsInventarioPorId(assetViewModel.ItemInventarioId);
                            CarregaViewBagContasContabeis(assetViewModel);
                            return View(assetViewModel);
                        }
                        //}

                        if (assetViewModel.checkFlagAcervo)
                        {
                            PreencheValoresPadraoParaAcervo(assetViewModel);
                            ModelState.Remove("MaterialGroupCode");
                        }
                        else if (assetViewModel.checkFlagTerceiro) {
                            PreencheValoresPadraoParaTerceiro(assetViewModel);
                            ModelState.Remove("MaterialGroupCode");
                        }

                        assetViewModel = PreencheDados(assetViewModel);

                        db.Configuration.LazyLoadingEnabled = false;

                        //int ErroDepreciacao = 0;

                        // SERVICO FUNCIONAR SOMENTE PARA FCASA
                        //int FCasa = (from i in db.Institutions where i.Id == _institutionId select i.Code).Count();

                        //int AssetIdGravado = 0;

                        Asset asset;
                        AssetMovements assetMovements;

                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                        {
                            AtribuirValores(assetViewModel, out asset, out assetMovements);

                            asset.flagAcervo = assetViewModel.checkFlagAcervo;
                            asset.flagTerceiro = assetViewModel.checkFlagTerceiro;

                            if (asset.MovementTypeId == (int)EnumMovimentType.IncorporacaoDeInventarioInicial)
                                asset.IndicadorOrigemInventarioInicial = (byte)EnumOrigemInventarioInicial.TelaIncorporacaoDeInventario;
                            else
                                asset.IndicadorOrigemInventarioInicial = (byte)EnumOrigemInventarioInicial.NaoEhInventarioInicial;

                            db.Entry(asset).State = EntityState.Added;
                            db.SaveChanges();

                            PreencheValorAquisicaoDeDecreto(asset);

                            assetMovements.Observation = assetViewModel.Observation;
                            assetMovements.AssetId = asset.Id;
                            db.Entry(assetMovements).State = EntityState.Added;
                            db.SaveChanges();

                            DepreciacaoNaIncorporacao objDepreciacao = new DepreciacaoNaIncorporacao(db);
                            objDepreciacao.Deprecia(asset, assetMovements);

                            ItemInventario itemInventarioAtualizado = (from ii in db.ItemInventarios
                                                                       where ii.Id == assetViewModel.ItemInventarioId
                                                                       select ii).AsNoTracking().FirstOrDefault();

                            itemInventarioAtualizado.AssetId = asset.Id;
                            itemInventarioAtualizado.Estado = (int)EnumSituacaoFisicaBP.Incorporado;
                            db.Entry(itemInventarioAtualizado).State = EntityState.Modified;
                            db.SaveChanges();

                            ComputaAlteracaoContabil(assetMovements.InstitutionId, assetMovements.BudgetUnitId,
                                                     assetMovements.ManagerUnitId, (int)assetMovements.AuxiliaryAccountId);
                            transaction.Complete();

                        }
                    }
                    return RedirectToAction("Index", "Movimento");
                }

                using (db = new SAMContext())
                {
                    LoadSelectElementsInventarioPorId(assetViewModel.ItemInventarioId);
                    CarregaViewBagContasContabeis(assetViewModel);
                }

                return View(assetViewModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        private Initial obterDadosSigla(string siglaChapaInventario)
        {
            Expression<Func<Initial, bool>> expWhere = null;
            expWhere = (siglaOrgao => siglaOrgao.Name == siglaChapaInventario && siglaOrgao.Status == true);
            expWhere = ((_institutionId > 0) ? expWhere.And(siglaOrgao => siglaOrgao.InstitutionId == _institutionId) : expWhere);
            expWhere = ((_budgetUnitId.HasValue) ? expWhere.And(siglaOrgao => siglaOrgao.BudgetUnitId == _budgetUnitId) : expWhere);
            expWhere = ((_managerUnitId.HasValue) ? expWhere.And(siglaOrgao => siglaOrgao.ManagerUnitId == _managerUnitId) : expWhere);


            var registroSiglaBP = this.db.Initials.Where(expWhere.Compile()).FirstOrDefault();
            if (registroSiglaBP.IsNull())
            {
                expWhere = (siglaOrgao => siglaOrgao.Name == siglaChapaInventario && siglaOrgao.Status == true);
                expWhere = ((_institutionId > 0) ? expWhere.And(siglaOrgao => siglaOrgao.InstitutionId == _institutionId) : expWhere);
                expWhere = ((_budgetUnitId.HasValue) ? expWhere.And(siglaOrgao => siglaOrgao.BudgetUnitId == _budgetUnitId) : expWhere);

                registroSiglaBP = this.db.Initials.Where(expWhere.Compile()).FirstOrDefault();
                if (registroSiglaBP.IsNull())
                {
                    expWhere = (siglaOrgao => siglaOrgao.Name == siglaChapaInventario && siglaOrgao.Status == true);
                    expWhere = ((_institutionId > 0) ? expWhere.And(siglaOrgao => siglaOrgao.InstitutionId == _institutionId) : expWhere);

                    registroSiglaBP = this.db.Initials.Where(expWhere.Compile()).FirstOrDefault();
                    if (registroSiglaBP.IsNull())
                    {
                        expWhere = (siglaOrgao => siglaOrgao.Name == siglaChapaInventario && siglaOrgao.Status == true);
                        registroSiglaBP = this.db.Initials.Where(expWhere.Compile()).FirstOrDefault();
                    }
                }
            }

            return registroSiglaBP;
        }

        #endregion

        #region Create
        // GET: Assets/Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                using (db = new SAMContext())
                {

                    LoadSelectElements(null);

                    if (UGEEstaComPendenciaSIAFEMNoFechamento(_managerUnitId))
                        return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                    return View();
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Assets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAnotacaoMultiplosBotoes(Name = "action", Argument = "Create")]
        public ActionResult Create(AssetViewModel assetViewModel)
        {
            try
            {
                db = new SAMContext();

                bool validaTamanhoNumeroDocumento = true;
                assetViewModel.DiferenciacaoChapa = (assetViewModel.DiferenciacaoChapa ?? "");
                assetViewModel.DiferenciacaoChapaAntiga = (assetViewModel.DiferenciacaoChapaAntiga ?? "");

                if (assetViewModel.MovementTypeId > 0)
                {
                    switch (assetViewModel.MovementTypeId)
                    {
                        case (int)EnumMovimentType.IncorporacaoDeInventarioInicial:
                            if (assetViewModel.checkFlagAcervo || assetViewModel.checkFlagTerceiro)
                            {
                                ValidaCamposValorAtualizado(assetViewModel);
                                if (assetViewModel.checkFlagAcervo)
                                    PreencheValoresPadraoParaAcervo(assetViewModel);
                                else
                                    PreencheValoresPadraoParaTerceiro(assetViewModel);

                                ModelState.Remove("MaterialGroupCode");
                            }

                            if (assetViewModel.checkFlagDecretoSefaz) {
                                if (string.IsNullOrEmpty(assetViewModel.ValueUpdateModel) || string.IsNullOrWhiteSpace(assetViewModel.ValueUpdateModel))
                                {
                                    ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado é obrigatório");
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(assetViewModel.ValueAcquisitionModel) && !string.IsNullOrWhiteSpace(assetViewModel.ValueAcquisitionModel))
                                    {
                                        if (Convert.ToDecimal(assetViewModel.ValueUpdateModel) < 10)
                                        {
                                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado de um decreto não pode ser inferior a R$10,00");
                                        }else if (Convert.ToDecimal(assetViewModel.ValueUpdateModel) <= Convert.ToDecimal(assetViewModel.ValueAcquisitionModel))
                                        {
                                            ModelState.AddModelError("ValueUpdateModel", "O Valor Atualizado de um decreto deve ser superior ao Valor de Aquisição");
                                        }
                                    }
                                }
                            }

                            break;
                        case (int)EnumMovimentType.IncorpDoacaoIntraNoEstado:
                        case (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado:

                            if (assetViewModel.AssetsIdTransferencia != null && assetViewModel.AssetsIdTransferencia > 0)
                            {
                                assetViewModel = PreencheValoresAquisicaoBPVindoDeOutraUGE(assetViewModel);
                                validaTamanhoNumeroDocumento = false;
                                ModelState.Remove("ValueAcquisitionModel");
                            }

                            if (assetViewModel.AceiteManual == true) {
                                validaTamanhoNumeroDocumento = true;
                                if (assetViewModel.ManagerUnitIdDestino <= 0) {
                                    ModelState.AddModelError("ManagerUnitIdDestino", "O campo UGE é obrigatório");
                                    if (assetViewModel.BudgetUnitIdDestino <= 0)
                                    {
                                        ModelState.AddModelError("BudgetUnitIdDestino", "O campo UO é obrigatório");
                                        if (assetViewModel.InstituationIdDestino <= 0)
                                        {
                                            ModelState.AddModelError("InstituationIdDestino", "O campo Orgão é obrigatório");
                                        }
                                    }
                                }
                            }

                            assetViewModel.checkLoteChapa = false;
                            assetViewModel.EndNumberIdentification = "-2";
                            break;
                        case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao:
                        case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia:
                            if (RecebimentoSendoFeitoPeloFundoSocial(assetViewModel.ManagerUnitId))
                            {

                                if (assetViewModel.AssetsIdTransferencia != null && assetViewModel.AssetsIdTransferencia > 0)
                                {
                                    validaTamanhoNumeroDocumento = false;
                                    assetViewModel = PreencheValoresAquisicaoBPInservivelVindoDeOutraUGE(assetViewModel);
                                    ModelState.Remove("ValueAcquisitionModel");
                                }

                                if (assetViewModel.AceiteManual == true)
                                {
                                    validaTamanhoNumeroDocumento = true;
                                    if (assetViewModel.ManagerUnitIdDestino <= 0)
                                    {
                                        ModelState.AddModelError("ManagerUnitIdDestino", "O campo UGE é obrigatório");
                                        if (assetViewModel.BudgetUnitIdDestino <= 0)
                                        {
                                            ModelState.AddModelError("BudgetUnitIdDestino", "O campo UO é obrigatório");
                                            if (assetViewModel.InstituationIdDestino <= 0)
                                            {
                                                ModelState.AddModelError("InstituationIdDestino", "O campo Orgão é obrigatório");
                                            }
                                        }
                                    }
                                }

                                assetViewModel.checkLoteChapa = false;
                                assetViewModel.EndNumberIdentification = "-2";
                            }
                            else {
                                ModelState.AddModelError("ManagerUnitId", "Tipo de Incorporação permitida somente para UGE 510032");
                            }
                            break;
                        case (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado:

                            assetViewModel = PreencheValoresAquisicaoBPVindoDeOutraUGE(assetViewModel);
                            ModelState.Remove("ValueAcquisitionModel");
                            validaTamanhoNumeroDocumento = false;
                            assetViewModel.checkLoteChapa = false;
                            assetViewModel.EndNumberIdentification = "-2";
                            break;


                        case (int)EnumMovimentType.IncorpDoacaoUniao:
                        case (int)EnumMovimentType.IncorpDoacaoMunicipio:
                        case (int)EnumMovimentType.IncorpDoacaoOutrosEstados:
                        case (int)EnumMovimentType.IncorpDoacaoConsolidacao:
                        case (int)EnumMovimentType.IncorpConfiscoBensMoveis:
                            if (string.IsNullOrEmpty(assetViewModel.CPFCNPJ) || string.IsNullOrWhiteSpace(assetViewModel.CPFCNPJ))
                            {
                                ModelState.AddModelError("CPFCNPJ", "O campo CPF/CNPJ é obrigatório.");
                            }

                            break;
                        case (int)EnumMovimentType.IncorporacaoPorEmpenho:
                            if (assetViewModel.Empenho != assetViewModel.EmpenhoResultado)
                                assetViewModel.Empenho = assetViewModel.EmpenhoResultado;
                            break;
                        case (int)EnumMovimentType.IncorporacaoPorEmpenhoRestosAPagar:
                            if (assetViewModel.Empenho != assetViewModel.EmpenhoResultado)
                                assetViewModel.Empenho = assetViewModel.EmpenhoResultado;

                            if (assetViewModel.Empenho == null || assetViewModel.Empenho.Trim() == string.Empty)
                            {
                                ModelState.AddModelError("numeroEmpenho", "Por favor, consulte um empenho válido.");
                            }
                            break;
                        case (int)EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao:
                            if (assetViewModel.checkFlagAcervo || assetViewModel.checkFlagTerceiro)
                            {
                                ValidaCamposValorAtualizado(assetViewModel);
                                if (assetViewModel.checkFlagAcervo)
                                    PreencheValoresPadraoParaAcervo(assetViewModel);
                                else
                                    PreencheValoresPadraoParaTerceiro(assetViewModel);

                                ModelState.Remove("MaterialGroupCode");
                            }

                            break;
                        case (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos:
                            //BP nessa incorporação é obrigatoriamente de terceiro
                            assetViewModel.checkFlagAcervo = false;
                            assetViewModel.checkFlagTerceiro = true;

                            ValidaCamposValorAtualizado(assetViewModel);
                            PreencheValoresPadraoParaTerceiro(assetViewModel);
                            ModelState.Remove("MaterialGroupCode");
                            ModelState.Remove("LifeCycle");
                            ModelState.Remove("RateDepreciationMonthly");
                            ModelState.Remove("ResidualValue");

                            break;
                    }

                    ValidaContaContabil(assetViewModel);
                    ValidaDescricaoResumida(assetViewModel);

                    //Número de Documento
                    if (!string.IsNullOrEmpty(assetViewModel.NumeroDocumentoAtual) || !string.IsNullOrWhiteSpace(assetViewModel.NumeroDocumentoAtual))
                    {
                        if (assetViewModel.NumeroDocumentoAtual == "0")
                        {
                            ModelState.AddModelError("NumberDoc", "Não é permitido informar 0, favor verificar!");
                        }

                        if (validaTamanhoNumeroDocumento)
                        {
                            if (assetViewModel.NumeroDocumentoAtual.Length > 14)
                            {
                                ModelState.AddModelError("NumberDoc", "Digite no máximo de 14 caracteres");
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(assetViewModel.NumberDoc) && !string.IsNullOrEmpty(assetViewModel.NumberDoc))
                    {
                        if (assetViewModel.NumberDoc == "0")
                        {
                            ModelState.AddModelError("NumberDoc", "Não é permitido informar 0, favor verificar!");
                        }

                        if (validaTamanhoNumeroDocumento)
                        {
                            if (assetViewModel.NumberDoc.Length > 14)
                            {
                                ModelState.AddModelError("NumberDoc", "Digite no máximo de 14 caracteres");
                            }
                        }
                    }

                    

                    if (string.IsNullOrEmpty(assetViewModel.ValueAcquisitionModel))
                    {
                        ModelState.AddModelError("ValueAcquisitionModel", "O campo Valor Aquisição é obrigatório.");
                    }
                    //Fim Número de Documento

                    //Incorporação em lote
                    if (assetViewModel.checkLoteChapa)
                    {
                        if (assetViewModel.EndNumberIdentification == null)
                        {
                            ModelState.AddModelError("EndNumberIdentification", "Número da chapa final não foi informada!");
                        }

                        if (Convert.ToInt64(assetViewModel.NumberIdentification) > Convert.ToInt64(assetViewModel.EndNumberIdentification))
                        {
                            ModelState.AddModelError("EndNumberIdentification", "Número da chapa final menor que chapa inicial");
                        }
                    }
                    //Fim Incorporação em lote
                }

                //Na primeira vez, o checkflagAnimalAServico não é assinalado
                //se não for feita pesquisa de um item material com grupo 88, causan erro no ModelState
                if (assetViewModel.MaterialGroupCode != 88)
                {
                    assetViewModel.checkflagAnimalAServico = false;
                    ModelState.Remove("checkflagAnimalAServico");
                }

                //Validação se incorporação pode possuir dados de terceiros
                if (assetViewModel.MovementTypeId != (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos)
                {
                    assetViewModel.OutSourcedId = null;
                    assetViewModel.CPFCNPJDoTerceiro = null;
                    assetViewModel.NomeDoTerceiro = null;
                }

                if (ModelState.IsValid)
                {

                    if (UGEEstaComPendenciaSIAFEMNoFechamento(assetViewModel.ManagerUnitId))
                        return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                    switch (assetViewModel.MovementTypeId)
                    {
                        case (int)EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao:
                            if (assetViewModel.MaterialGroupCode != 88)
                            {

                                ModelState.AddModelError("MaterialItemCode", "Somente BPs do grupo 88 são aceitos para essa incorporação"); // diferente de 88 nao movimenta, 88 movimenta
                                LoadSelectElements(assetViewModel);
                                return View(assetViewModel);
                            }
                            break;


                        case (int)EnumMovimentType.IncorporacaoPorEmpenho:
                            if (assetViewModel.MovimentDate < new DateTime(2021, 1, 1)) {
                                ModelState.AddModelError("NumberDoc", "Incorporação permitida somente a partir de janeiro/2021");
                                LoadSelectElements(assetViewModel);
                                return View(assetViewModel);
                            }
                            break;
                        case (int)EnumMovimentType.IncorpDoacaoIntraNoEstado:
                        case (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado:
                            if (assetViewModel.AssetsIdTransferencia != null && assetViewModel.AssetsIdTransferencia > 0)
                            {
                                if (!VerificaSeIncorporacaoEstaNoMesRef(assetViewModel))
                                {
                                    LoadSelectElements(assetViewModel);
                                    return View(assetViewModel);
                                }

                                if (VerificaSeTransferenciaOuDoacaoNaOrigemPossuiPendencia(assetViewModel))
                                {
                                    LoadSelectElements(assetViewModel);
                                    return View(assetViewModel);
                                }

                            }
                            break;
                        case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao:
                        case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia:
                            if (assetViewModel.AssetsIdTransferencia != null && assetViewModel.AssetsIdTransferencia > 0)
                            {
                                if (!VerificaSeIncorporacaoInservivelEstaNoMesRef(assetViewModel))
                                {
                                    LoadSelectElements(assetViewModel);
                                    return View(assetViewModel);
                                }

                                if (VerificaSeSaidaNaOrigemPossuiPendencia(assetViewModel))
                                {
                                    LoadSelectElements(assetViewModel);
                                    return View(assetViewModel);
                                }
                            }
                            break;
                        case (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado:

                            if (!VerificaSeIncorporacaoEstaNoMesRef(assetViewModel))
                            {
                                LoadSelectElements(assetViewModel);
                                return View(assetViewModel);
                            }

                            if (VerificaSeTransferenciaOuDoacaoNaOrigemPossuiPendencia(assetViewModel))
                            {
                                LoadSelectElements(assetViewModel);
                                return View(assetViewModel);
                            }

                            break;
                    }

                    bool validaDataAquisicaoCompleto = true;
                    if (assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado)
                    {
                        validaDataAquisicaoCompleto = false;
                    }

                    if (assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                        assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                        assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                        assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
                    {
                        if (assetViewModel.AceiteManual != true)
                        {
                            validaDataAquisicaoCompleto = false;
                        }
                    }

                    var mensagemErro = ValidaDataDeAcquisicao(assetViewModel.AcquisitionDate, assetViewModel.ManagerUnitId, assetViewModel.MovementTypeId, validaDataAquisicaoCompleto);
                    if (mensagemErro != string.Empty)
                    {
                        ModelState.AddModelError("AcquisitionDate", mensagemErro);
                        LoadSelectElements(assetViewModel);
                        return View(assetViewModel);
                    }

                    mensagemErro = ValidaDataDeIncorporacao(assetViewModel.MovimentDate, assetViewModel.ManagerUnitId, assetViewModel.MovementTypeId, assetViewModel.AcquisitionDate);
                    if (mensagemErro != string.Empty)
                    {
                        ModelState.AddModelError("MovimentDate", mensagemErro);
                        LoadSelectElements(assetViewModel);
                        return View(assetViewModel);
                    }

                    if (!assetViewModel.checkFlagAcervo && !assetViewModel.checkFlagDecretoSefaz && !assetViewModel.checkFlagTerceiro)
                    {
                        switch (assetViewModel.MovementTypeId)
                        {
                            case (int)EnumMovimentType.IncorpDoacaoIntraNoEstado:
                            case (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado:
                            case (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado:
                            case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao:
                            case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia:

                                if (decimal.Parse(assetViewModel.ValueAcquisitionModel) < 1)
                                {
                                    ModelState.AddModelError("ValueAcquisitionModel", "Valor de aquisição não pode ser menor que 1,00");
                                    LoadSelectElements(assetViewModel);
                                    return View(assetViewModel);
                                }

                                break;
                            default:
                                if (decimal.Parse(assetViewModel.ValueAcquisitionModel) < 10)
                                {
                                    ModelState.AddModelError("ValueAcquisitionModel", "Valor de aquisição não pode ser menor que 10,00");
                                    LoadSelectElements(assetViewModel);
                                    return View(assetViewModel);
                                }

                                break;
                        }
                    }

                    Int64 ChapaInicial = Convert.ToInt64(assetViewModel.NumberIdentification);
                    Int64 ChapaFinal = 0;

                    //Verifica se é uma inclusão individual ou em lote.
                    if (Convert.ToInt64(assetViewModel.EndNumberIdentification) > 0)
                    {
                        if (VerificaSeDuplicaraBPsEmLote(assetViewModel))
                        {
                            LoadSelectElements(assetViewModel);
                            return View(assetViewModel);
                        }

                        ChapaFinal = Convert.ToInt64(assetViewModel.EndNumberIdentification);
                    }
                    else
                    {
                        ChapaFinal = ChapaInicial;

                        if (VerificaSeDuplicaraBPs(assetViewModel)){
                            LoadSelectElements(assetViewModel);
                            return View(assetViewModel);
                        }
                    }

                    assetViewModel = PreencheDados(assetViewModel);

                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;
                    int ErroDepreciacao = 0;
                    IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>();

                    base.initDadosSIAFEM();

                    var incorporacao = (from m in db.MovementTypes
                                        where m.Id == assetViewModel.MovementTypeId
                                        select m).FirstOrDefault();

                    var dataInicioIntegracao = new DateTime(2020, 06, 01);
                    bool MovimentacaoIntegradoAoSIAFEM = incorporacao.PossuiContraPartidaContabil() && base.ugeIntegradaSiafem && assetViewModel.MovimentDate >= dataInicioIntegracao;

                    Asset asset;
                    AssetMovements assetMovements;
                    DepreciacaoNaIncorporacao objDepreciacao = new DepreciacaoNaIncorporacao(db);
                    int PrimeiroAssetStartId = 0;

                    // Percorre o Range entre as Chapas e grava no banco o asset e o assetmovements de cada um somente atualizando o Id e o NumberIdentification = [Chapa]
                    for (Int64 i = ChapaInicial; i <= ChapaFinal; i++)
                    {
                        int AssetIdGravado = 0;

                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead }))
                        {
                            AtribuirValores(assetViewModel, out asset, out assetMovements);

                            string casasFrente = assetViewModel.NumberIdentification.Replace(ChapaInicial.ToString(), "");
                            asset.NumberIdentification = casasFrente + i.ToString();
                            asset.flagAcervo = assetViewModel.checkFlagAcervo;
                            asset.flagTerceiro = assetViewModel.checkFlagTerceiro;

                            if (
                                asset.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado && assetViewModel.AceiteManual != true) ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado && assetViewModel.AceiteManual != true) ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao && assetViewModel.AceiteManual != true) ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia && assetViewModel.AceiteManual != true)
                               )
                            {
                                var AssetTransferidoDecreto = (from a in db.Assets where a.Id == assetViewModel.AssetsIdTransferencia select a).FirstOrDefault();
                                asset.MonthUsed = AssetTransferidoDecreto.MonthUsed;
                                asset.ResidualValueCalc = AssetTransferidoDecreto.ResidualValueCalc;
                                asset.DepreciationByMonth = AssetTransferidoDecreto.DepreciationByMonth;
                                asset.DepreciationAccumulated = AssetTransferidoDecreto.DepreciationAccumulated;
                                asset.ValorDesdobramento = AssetTransferidoDecreto.ValorDesdobramento;
                                asset.flagDepreciationCompleted = AssetTransferidoDecreto.flagDepreciationCompleted;
                            }

                            if (asset.MovementTypeId == (int)EnumMovimentType.IncorporacaoDeInventarioInicial)
                                asset.IndicadorOrigemInventarioInicial = (byte)EnumOrigemInventarioInicial.TelaIncorporacao;
                            else
                                asset.IndicadorOrigemInventarioInicial = (byte)EnumOrigemInventarioInicial.NaoEhInventarioInicial;

                            db.Entry(asset).State = EntityState.Added;
                            db.SaveChanges();

                            if (asset.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado && assetViewModel.AceiteManual != true) ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado && assetViewModel.AceiteManual != true) ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao && assetViewModel.AceiteManual != true) ||
                                (asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia && assetViewModel.AceiteManual != true)
                                )
                            {
                                var lstMovimentosAssetDoacao = (from am in db.AssetMovements where am.AssetId == assetViewModel.AssetsIdTransferencia select am).ToList();
                                int IdMaxDoacao = (from am in db.AssetMovements where am.AssetId == assetViewModel.AssetsIdTransferencia select am.Id).OrderByDescending(am => am).FirstOrDefault();

                                foreach (var item in lstMovimentosAssetDoacao)
                                {
                                    if (item.Id == IdMaxDoacao)
                                        item.AssetTransferenciaId = asset.Id;

                                    item.Status = false;
                                    db.Entry(item).State = EntityState.Modified;
                                }

                                Asset _assetDoacao = db.Assets.Find(assetViewModel.AssetsIdTransferencia);
                                _assetDoacao.Status = false;
                                db.Entry(_assetDoacao).State = EntityState.Modified;

                                CopiaValorAquisicaoDeDecreto(_assetDoacao, asset);
                                CopiaValorDepreciacaoDeAceite(_assetDoacao, asset.ManagerUnitId);
                                db.SaveChanges();
                            }
                            else {
                                //Atualiza os campos do decreto se o BP não precisou de acecite
                                PreencheValorAquisicaoDeDecreto(asset);
                            }


                            //Tratamento para incorporações do tipo Doação Intra no Estado Manual - Leonardo: 14/12/2018
                            if (
                                (
                                 asset.MovementTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado || 
                                 asset.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                                 asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                                 asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia
                                ) &&
                                assetViewModel.AceiteManual == true
                               )
                            {
                                assetMovements.SourceDestiny_ManagerUnitId = assetViewModel.ManagerUnitIdDestino;
                            }

                            //Tratamento para incorporacoes dos tipos de doacoes que requerem CPF/CNPJ
                            EnumMovimentType[] tiposDeIncorporacaoComCpfCnpj = new EnumMovimentType[] {
                                                                                                          EnumMovimentType.IncorpDoacaoConsolidacao
                                                                                                        , EnumMovimentType.IncorpDoacaoMunicipio
                                                                                                        , EnumMovimentType.IncorpDoacaoOutrosEstados
                                                                                                        , EnumMovimentType.IncorpDoacaoUniao
                                                                                                        , EnumMovimentType.IncorpConfiscoBensMoveis
                                                                                                    };
                            if (tiposDeIncorporacaoComCpfCnpj.ToList()
                                                             .Select(tipoMovPatrimonial => tipoMovPatrimonial.GetHashCode())
                                                             .Contains(asset.MovementTypeId)
                               )
                            {
                                assetMovements.CPFCNPJ = assetMovements.CPFCNPJ.RetirarCaracteresEspeciaisCpfCnpj();
                            }


                            assetMovements.Observation = assetViewModel.Observation;
                            assetMovements.NumberDoc = assetViewModel.NumberDoc;
                            assetMovements.AssetId = asset.Id;
                            db.Entry(assetMovements).State = EntityState.Added;
                            asset.AssetStartId = asset.Id;
                            asset.MaterialItemCode = assetViewModel.MaterialItemCode;
                            db.SaveChanges();

                            ComputaAlteracaoContabil(assetMovements.InstitutionId, assetMovements.BudgetUnitId, assetMovements.ManagerUnitId, (int)assetMovements.AuxiliaryAccountId);
                            if (PrimeiroAssetStartId == 0)
                            {
                                objDepreciacao.Deprecia(asset, assetMovements);
                                PrimeiroAssetStartId = asset.AssetStartId == null ? 0 : (int)asset.AssetStartId;
                            }
                            else
                            {
                                objDepreciacao.CopiaDepreciacaoNaIncorporacaoEmLote(asset, assetMovements, PrimeiroAssetStartId);
                            }

                            transaction.Complete();
                            AssetIdGravado = asset.Id;
                        }

                        if (assetViewModel.checkFlagDecretoSefaz != true)
                        {
                            CalculaDepreciacao depreciacao = new CalculaDepreciacao();
                            var retorno = depreciacao.CalculaDepreciacaoAcumulada(AssetIdGravado, (string)HttpContext.Items["CPF"], assetViewModel.checkFlagAcervo, assetViewModel.checkFlagTerceiro, assetViewModel.ValueUpdateModel, assetViewModel.MovementTypeId);

                            if (retorno.Rows.Count > 0)
                            {
                                ErroDepreciacao = 1;
                                string messageErro = retorno.Rows[0]["ErrorMessage"].ToString();
                                string stackTrace = "Procedure: DEPRECIACAO_ACUMULADA_UNITARIO";
                                string name = "DEPRECIACAO_ACUMULADA_UNITARIO";
                                GravaLogErro(messageErro, stackTrace, name);
                            }
                        }

                        {
                            //SALVAGUARDA
                            assetMovements.AssetTransferenciaId = assetViewModel.AssetsIdTransferencia;
                            assetMovements.SourceDestiny_ManagerUnitId = assetViewModel.ManagerUnitIdDestino;
                            assetMovements.CPFCNPJ = assetViewModel.CPFCNPJ.RetirarCaracteresEspeciaisCpfCnpj();
                        }
                        //INCLUSAO PARA CAPTURA DE MOVIMENTACAO PATRIMONIAL EM LOTE
                        listaMovimentacoesPatrimoniaisEmLote.Add(assetMovements);
                    }

                    if (ErroDepreciacao > 0)
                        return MensagemErro("O Bem Patrimonial foi salvo, porém a depreciação não ocorreu para algum(s) item(s), favor verificar na tela de Bem Patrimonial Pendente!");

                    if (MovimentacaoIntegradoAoSIAFEM)
                    {
                        var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                        var tuplas = svcIntegracaoSIAFEM.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovimentacoesPatrimoniaisEmLote, true);

                        DadosPopupContabiliza objPopUp = new DadosPopupContabiliza();

                        objPopUp.MsgSIAFEM = new List<string>();
                        objPopUp.ListaIdAuditoria = string.Empty;
                        objPopUp.LoginSiafem = assetViewModel.LoginSiafem;
                        objPopUp.SenhaSiafem = assetViewModel.SenhaSiafem;

                        foreach (var item in tuplas)
                        {
                            objPopUp.MsgSIAFEM.Add(item.Item1);
                            objPopUp.ListaIdAuditoria += item.Item3.ToString() + ",";
                        }

                        objPopUp.ListaIdAuditoria += "0";

                        return View("_VazioBaseParaPopupsSIAFEM", objPopUp);
                    }
                    else {
                        string msgInformeAoUsuario = null;
                        //EXIBICAO DE POP-UP COM AS INFORMACOES REFERENTES A MOVIMENTACAO PATRIMONIAL RECEM-EFETIVADA (MAIS INFORMACOES ENVIADAS/RETORNADAS PARA/DE SISTEMA CONTABILIZA-SP)
                        var loginUsuarioSIAFEM = assetViewModel.LoginSiafem;
                        var senhaUsuarioSIAFEM = assetViewModel.SenhaSiafem;
                        msgInformeAoUsuario = base.processamentoMovimentacaoPatrimonialNoContabilizaSP(assetViewModel.MovementTypeId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, assetViewModel.NumberDoc, listaMovimentacoesPatrimoniaisEmLote);
                        msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();
                        TempData["msgSucesso"] = msgInformeAoUsuario;
                        TempData["numeroDocumento"] = assetViewModel.NumberDoc;
                        TempData.Keep();

                        return RedirectToAction("Index", "Movimento");
                    }

                    
                }

                LoadSelectElements(assetViewModel);
                return View(assetViewModel);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }
            catch (NullReferenceException ex)
            {
                LoadSelectElements(assetViewModel);
                return View(assetViewModel);
            }
			catch (Exception ex) {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion Create

        public JsonResult GerarXMLSiafemDocNLPatrimonial(int? _ManagerUnitId, string _MovimentDate, int? _InstitutionId, string _NumberDoc, string _CPFCNPJ, int? _MovementTypeId, string _MaterialItemCode, string _Valor, string LoginSiafem, string SenhaSiafem)
        {
            MensagemModel mensagem = new MensagemModel();
            List<MensagemModel> mensagens = new List<MensagemModel>();

            if (LoginSiafem.IsNullOrEmpty())
            {
                mensagem.Id = 1;
                mensagem.Mensagem = "Login Siafem não informado.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (SenhaSiafem.IsNullOrEmpty())
            {
                mensagem.Id = 2;
                mensagem.Mensagem = "Senha Siafem não informado.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_MovementTypeId.IsNull())
            {
                mensagem.Id = 3;
                mensagem.Mensagem = "Tipo Movimento não informado.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_InstitutionId.IsNull())
            {
                mensagem.Id = 4;
                mensagem.Mensagem = "Data Movimento não informada.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_ManagerUnitId.IsNull())
            {
                mensagem.Id = 5;
                mensagem.Mensagem = "UGE não informada.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            //if (_NumberDoc.IsNullOrEmpty())
            //{
            //    mensagem.Id = 6;
            //    mensagem.Mensagem = "Documento não informado.";
            //    mensagens.Add(mensagem);
            //    return Json(mensagens, JsonRequestBehavior.AllowGet);
            //}
            //if (_CPFCNPJ.IsNullOrEmpty())
            //{
            //    mensagem.Id = 7;
            //    mensagem.Mensagem = "CPF/CNPJ não informado.";
            //    mensagens.Add(mensagem);
            //    return Json(mensagens, JsonRequestBehavior.AllowGet);
            //}
            if (_MaterialItemCode.IsNullOrEmpty())
            {
                mensagem.Id = 8;
                mensagem.Mensagem = "Item Material não informado.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_MovimentDate.IsNullOrEmpty())
            {
                mensagem.Id = 9;
                mensagem.Mensagem = "Data Incorporação não informada.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (db = new SAMContext())
                {
                    int IntegracaoSiafem = (from i in db.Institutions where i.Id == _InstitutionId && i.flagIntegracaoSiafem == true select i.Code).Count();

                    if (IntegracaoSiafem > 0)
                    {
                        string CE = "ce999"; // VERIFICAR
                        string ugefavorecida = "0";
                        int gestaofavorecida = 0;
                        int NL = 2; // VERIFICAR

                        int codigoUg = 0;
                        int codigoGestao = 0;
                        string strDataAdmissao = "";
                        string documentoSAM = "";
                        string observacoesMovimentacao = "";
                        string anoBasePagamento = "";

                        EventoSiafemEntity _EventoSiafemEntity = new EventoSiafemEntity();
                        EventoSiafem _EventoSiafem = new EventoSiafem();

                        _EventoSiafemEntity = _EventoSiafem.RetornaParametrosEventoSiafem(_MovementTypeId);

                        string UGECode = (from m in db.ManagerUnits where m.Id == _ManagerUnitId select m.Code).FirstOrDefault();
                        codigoUg = UGECode == null || UGECode == "" ? 0 : int.Parse(UGECode);

                        string CodigoGestao = (from m in db.Institutions where m.Id == _InstitutionId select m.ManagerCode).FirstOrDefault();
                        codigoGestao = CodigoGestao == null || CodigoGestao == "" ? 0 : int.Parse(CodigoGestao);

                        //strDataAdmissao = _MovimentDate.Date.ToString().Substring(0, 10);
                        documentoSAM = _NumberDoc; // VERIFICAR
                        observacoesMovimentacao = "MovId = " + 0 + " GERADO ATRAVÉS DO SAM";

                        string MesRef = (from m in db.ManagerUnits where m.Id == _ManagerUnitId select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
                        anoBasePagamento = MesRef == null || MesRef == "" ? "" : MesRef.Substring(0, 4);
                        ugefavorecida = _CPFCNPJ;

                        Dictionary<string, decimal?> Item = new Dictionary<string, decimal?>();

                        decimal? ValorFinal = _Valor.IsNullOrEmpty() ? 0 : Convert.ToDecimal(_Valor);

                        Item.Add(_MaterialItemCode, ValorFinal);

                        //TODO DCBATISTA AJUSTAR CHAMADA
                        //string XMLPatrimonial = GeradorEstimuloSIAF.SiafemDocNLPatrimonial(codigoUg, codigoGestao, strDataAdmissao, documentoSAM, observacoesMovimentacao, CE, true, ugefavorecida, gestaofavorecida, false, _EventoSiafemEntity, Item);

                        //return Json(XMLPatrimonial, JsonRequestBehavior.AllowGet);
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                    else
                        return null;

                }
            }
            catch (Exception ex)
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "Não foi possivel gerar o XML!";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
        }

        //context em outros métodos
        internal void AtribuirValores(AssetViewModel assetViewModel, out Asset asset, out AssetMovements assetMovements)
        {
            asset = new Asset();
            assetMovements = new AssetMovements();
            Mapper.CreateMap<AssetViewModel, Asset>();
            asset = Mapper.Map<Asset>(assetViewModel);

            if (assetViewModel.MaterialItemDescription.Length > 120)
            {
                asset.MaterialItemDescription = assetViewModel.MaterialItemDescription.Substring(0, 119);
            }

            // Se houver alguma alteração em um dos campos de depreciação que sai do padrão do grupo de material o BP será considerado com depreciação acelerada
            if (VerificaGrupoMaterial(assetViewModel))
                asset.AceleratedDepreciation = true;
            else
                asset.AceleratedDepreciation = false;

            asset.Status = true;
            asset.flagVerificado = null;
            asset.flagDepreciaAcumulada = 1;
            asset.LifeCycle = assetViewModel.LifeCycle;
            asset.RateDepreciationMonthly = Convert.ToDecimal(assetViewModel.RateDepreciationMonthly);
            asset.ResidualValue = Convert.ToDecimal(assetViewModel.ResidualValue);
            asset.ValueAcquisition = Convert.ToDecimal(assetViewModel.ValueAcquisitionModel);
            asset.NumberDoc = assetViewModel.NumberDoc;
            asset.Empenho = assetViewModel.Empenho;

            if (assetViewModel.checkFlagDecretoSefaz || assetViewModel.checkFlagAcervo || assetViewModel.checkFlagTerceiro)
                asset.ValueUpdate = Convert.ToDecimal(assetViewModel.ValueUpdateModel);
            else
                asset.ValueUpdate = asset.ValueAcquisition;

            asset.InitialName = (from i in db.Initials where i.Id == assetViewModel.InitialId select i.Name).FirstOrDefault();
            asset.Login = UserCommon.CurrentUser().CPF;
            asset.DataLogin = DateTime.Now;
            asset.flagAcervo = assetViewModel.checkFlagAcervo;
            asset.flagTerceiro = assetViewModel.checkFlagTerceiro;
            asset.flagDecreto = assetViewModel.checkFlagDecretoSefaz;
            asset.flagAnimalNaoServico = assetViewModel.flagAnimalNaoServico;
            asset.flagVindoDoEstoque = false; //Nenhuma incorporação dessa controller vem do Estoque

            asset.AssetStartId = assetViewModel.AssetStartId;

            if (assetViewModel.CPFCNPJ.IsNotNull())
            {
                asset.CPFCNPJ = assetViewModel.CPFCNPJ.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
                assetMovements.CPFCNPJ = asset.CPFCNPJ;
            }

            // Adiciono o movimento para o Asset
            assetMovements.Status = true;
            assetMovements.MovimentDate = asset.MovimentDate;
            assetMovements.MovementTypeId = asset.MovementTypeId;
            assetMovements.StateConservationId = assetViewModel.StateConservationId;
            assetMovements.NumberPurchaseProcess = assetViewModel.NumberPurchaseProcess;
            assetMovements.InstitutionId = assetViewModel.InstitutionId;
            assetMovements.BudgetUnitId = assetViewModel.BudgetUnitId;
            assetMovements.ManagerUnitId = assetViewModel.ManagerUnitId;
            assetMovements.AdministrativeUnitId = assetViewModel.AdministrativeUnitId == 0 ? null : assetViewModel.AdministrativeUnitId;
            assetMovements.SectionId = assetViewModel.SectionId == 0 ? null : assetViewModel.SectionId;
            assetMovements.AuxiliaryAccountId = assetViewModel.AuxiliaryAccountId == 0 ? null : assetViewModel.AuxiliaryAccountId;
            assetMovements.ResponsibleId = assetViewModel.ResponsibleId == 0 ? null : assetViewModel.ResponsibleId;
            assetMovements.DataLogin = DateTime.Now;
            assetMovements.Login = UserCommon.CurrentUser().CPF;
        }

        //context em outros métodos
        public string ValidaDataDeIncorporacao(DateTime DataDeIncorporacao, int ManagerUnitIdInformado, int tipoIncorporacao, DateTime DataDeAquisicao)
        {
            if (DataDeIncorporacao.Year < 1900)
            {
                return "Por favor, informe uma Data de Incorporação com ano posterior à 1900";
            }

            if (DataDeIncorporacao < DataDeAquisicao) {
                return "A Data de incorporação deve ser posterior a data de aquisição";
            }

            if (DataDeIncorporacao.Date > DateTime.Now.Date)
            {
                return "Por favor, informe uma data de Incorporação igual ou inferior a data atual.";
            }

            string _mensagemRetorno = String.Empty;
            string mesRefDaIncorporacao = DataDeIncorporacao.Year.ToString().PadLeft(4, '0') + DataDeIncorporacao.Month.ToString().PadLeft(2, '0');
            string mesRefAtualDaUGE = RecuperaAnoMesReferenciaFechamento(ManagerUnitIdInformado);
            
            if ((int)EnumMovimentType.IncorporacaoDeInventarioInicial != tipoIncorporacao)
            {
                if (mesRefAtualDaUGE != mesRefDaIncorporacao)
                {
                    _mensagemRetorno = "Por favor, informe uma Data Incorporação que corresponda ao mês/ano de referência " + mesRefAtualDaUGE.Substring(4) + "/" + mesRefAtualDaUGE.Substring(0, 4) + ".";
                    return _mensagemRetorno;
                }
            }
            else
            {
                string mesRefInicialDaUGE = RecuperaAnoMesStartUGE(ManagerUnitIdInformado);
                if (int.Parse(mesRefAtualDaUGE) != int.Parse(mesRefInicialDaUGE))
                {
                    if (mesRefAtualDaUGE != mesRefDaIncorporacao)
                    {
                        _mensagemRetorno = "Por favor, informe uma Data Incorporação que corresponda ao mês/ano de referência " + mesRefAtualDaUGE.Substring(4) + "/" + mesRefAtualDaUGE.Substring(0, 4) + ".";
                    }
                }
                else
                {
                    if (int.Parse(mesRefDaIncorporacao) > int.Parse(mesRefAtualDaUGE))
                    {
                        _mensagemRetorno = "Por favor, informe uma data cujo mês/ano sejam iguais ou inferiores ao mês de referência " + mesRefAtualDaUGE.Substring(4) + "/" + mesRefAtualDaUGE.Substring(0, 4) + ".";
                    }
                }
            }
            
            return _mensagemRetorno;
        }
        
        //context em outros métodos
        public string ValidaDataDeAcquisicao(DateTime DataDeAquisicao, int ManagerUnitIdInformado, int tipo, bool valida = true)
        {
            string _mensagemRetorno = String.Empty;

            if (DataDeAquisicao.Date > DateTime.Now.Date)
            {
                return "Por favor, informe uma data de Aquisição igual ou inferior a data atual.";
            }

            if (valida)
            {
                if (DataDeAquisicao.Year < 1900)
                {
                    _mensagemRetorno = "Por favor, informe uma Data Aquisição com ano posterior à 1900";
                }
            }
            

            return _mensagemRetorno;
        }

        //context em outros métodos
        private string RecuperaAnoMesReferenciaFechamento(int ManagerUnitIdInformado)
        {
            return (from m in db.ManagerUnits
                               where m.Id == ManagerUnitIdInformado
                               select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
        }
        //context em outros métodos
        private string RecuperaAnoMesStartUGE(int ManagerUnitIdInformado)
        {
            return (from m in db.ManagerUnits
                               where m.Id == ManagerUnitIdInformado
                               select m.ManagmentUnit_YearMonthStart).FirstOrDefault();
        }
        //context em outros métodos
        private bool VerificaGrupoMaterial(AssetViewModel assetViewModel)
        {
            var materialGroup = (from mg in db.MaterialGroups
                                 where mg.Code == assetViewModel.MaterialGroupCode
                                 select mg).ToList().FirstOrDefault();

            if (materialGroup.IsNotNull())
            {
                return (assetViewModel.LifeCycle != materialGroup.LifeCycle ||
                        assetViewModel.RateDepreciationMonthly != materialGroup.RateDepreciationMonthly ||
                        assetViewModel.ResidualValue != materialGroup.ResidualValue);
            }
            else
            {
                return (assetViewModel.LifeCycle != 0 ||
                        assetViewModel.RateDepreciationMonthly != 0 ||
                        assetViewModel.ResidualValue != 0);
            }
        }

        private void ComputaAlteracaoContabil(int IdOrgao, int IdUO, int IdUGE, int IdContaContabil) {
            var implantado = UGEIntegradaAoSIAFEM(IdUGE);

            if (implantado)
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
                    registro.IdContaContabil = IdContaContabil;
                    registro.HouveAlteracao = true;
                    db.Entry(registro).State = EntityState.Added;
                }

                db.SaveChanges();
            }
        }

        #region Edit
        // GET: Assets/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    AssetViewModel BPASerEditado = (from a in db.Assets
                                                    where a.Id == id
                                                    select new AssetViewModel
                                                    {
                                                        AssetStartId = a.AssetStartId,
                                                        Id = a.Id,
                                                        InitialId = a.InitialId,
                                                        NumberIdentification = a.NumberIdentification,
                                                        DiferenciacaoChapa = a.DiferenciacaoChapa,
                                                        AcquisitionDate = a.AcquisitionDate,
                                                        MovimentDate = a.MovimentDate,
                                                        ValueAcquisition = a.ValueAcquisition,
                                                        ValueUpdate = a.ValueUpdate,
                                                        MaterialItemCode = a.MaterialItemCode,
                                                        MaterialItemDescription = a.MaterialItemDescription,
                                                        MaterialGroupCode = a.MaterialGroupCode,
                                                        MaterialGroupDescription = (from m in db.MaterialGroups where m.Code == a.MaterialGroupCode select m.Description).FirstOrDefault(),
                                                        ShortDescription = a.RelatedShortDescriptionItem == null ? null : a.RelatedShortDescriptionItem.Description,
                                                        LifeCycle = a.LifeCycle,
                                                        RateDepreciationMonthly = a.RateDepreciationMonthly,
                                                        ShortDescriptionItemId = a.ShortDescriptionItemId,
                                                        OldInitial = a.OldInitial,
                                                        NumberDoc = a.NumberDoc,
                                                        Empenho = a.Empenho,
                                                        flagCalculoPendente = a.flagCalculoPendente,
                                                        flagDepreciationCompleted = a.flagDepreciationCompleted == null ? false : a.flagDepreciationCompleted,
                                                        // Dados Complementares
                                                        SerialNumber = a.SerialNumber,
                                                        DateGuarantee = a.DateGuarantee,
                                                        Brand = a.Brand,
                                                        NumberPlate = a.NumberPlate,
                                                        ManufactureDate = a.ManufactureDate,
                                                        ChassiNumber = a.ChassiNumber,
                                                        Model = a.Model,
                                                        AdditionalDescription = a.AdditionalDescription,
                                                        flagAcervo = a.flagAcervo,
                                                        flagTerceiro = a.flagTerceiro,
                                                        checkFlagAcervo = a.flagAcervo == null ? false : ((bool)a.flagAcervo),
                                                        checkFlagTerceiro = a.flagTerceiro == null ? false : ((bool)a.flagTerceiro),
                                                        checkFlagDecretoSefaz = a.flagDecreto == null ? false : ((bool)a.flagDecreto),
                                                        checkflagAnimalAServico = a.flagAnimalNaoServico == null ? true : !((bool)a.flagAnimalNaoServico),
                                                        Status = a.Status,
                                                        MovementTypeId = a.MovementTypeId
                                                    })
                                           .AsNoTracking().FirstOrDefault();

                    if (BPASerEditado == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    AssetMovements HistoricoDoBP;

                    if (BPASerEditado.Status)
                        HistoricoDoBP = (from am in db.AssetMovements where am.AssetId == id && am.Status == true select am).AsNoTracking().FirstOrDefault();
                    else
                        HistoricoDoBP = (from am in db.AssetMovements where am.AssetId == id && am.FlagEstorno == null select am).AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefault();

                    if (ValidarRequisicao(HistoricoDoBP.InstitutionId, HistoricoDoBP.BudgetUnitId, HistoricoDoBP.ManagerUnitId, HistoricoDoBP.AdministrativeUnitId, HistoricoDoBP.SectionId))
                    {

                        
                        BPASerEditado.ValueAcquisitionModel = BPASerEditado.ValueAcquisition.ToString();

                        BPASerEditado.ManagerUnitId = HistoricoDoBP.ManagerUnitId;
                        BPASerEditado.NumeroDocumentoAtual = HistoricoDoBP.NumberDoc;
                        BPASerEditado.AssetMovementsId = HistoricoDoBP.Id;
                        BPASerEditado.StateConservationId = HistoricoDoBP.StateConservationId;
                        BPASerEditado.NumberPurchaseProcess = HistoricoDoBP.NumberPurchaseProcess;
                        BPASerEditado.InstitutionId = HistoricoDoBP.InstitutionId;
                        BPASerEditado.BudgetUnitId = HistoricoDoBP.BudgetUnitId;
                        BPASerEditado.ManagerUnitId = HistoricoDoBP.ManagerUnitId;
                        BPASerEditado.AdministrativeUnitId = HistoricoDoBP.AdministrativeUnitId;
                        BPASerEditado.SectionId = HistoricoDoBP.SectionId;
                        BPASerEditado.AuxiliaryAccountId = HistoricoDoBP.AuxiliaryAccountId;
                        BPASerEditado.ResponsibleId = HistoricoDoBP.ResponsibleId;

                        if(HistoricoDoBP.AuxiliaryAccountId != null)
                        BPASerEditado.ContaContabilApresentacaoEdicao = RetornaDescricaoDaContaContabil((int)HistoricoDoBP.AuxiliaryAccountId);

                        BPASerEditado.AssetNumberIdentifications = RetornaHistoricoDeChapas(BPASerEditado.Id);

                        if (BPASerEditado.Status)
                        {
                            BPASerEditado.podeSerExcluido = BPNaoSeTratarDeAceite(BPASerEditado.Id) && BPPodeSerExcluidoPorNaoTerPendenciaSIAFEM(BPASerEditado.Id);
                            BPASerEditado.podeEditarItemMaterial = PermitidoAlterarItemMaterial(BPASerEditado);
                        }

                        PreencheDadosDepreciacao(BPASerEditado);
                        PreencheDadosHistoricoValoresDecreto(BPASerEditado);

                        BPASerEditado.RelatedItemInventarios = BPASerEditado.RelatedItemInventarios;
                        PreencheDadosNLsSIAFEM02(BPASerEditado);
                        PreencheDadosTerceiro(BPASerEditado);
                        CarregaViewBags(BPASerEditado);

                        return View(BPASerEditado);
                    }
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Assets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, InstitutionId, BudgetUnitId, ManagerUnitId, AdministrativeUnitId, SectionId, ResponsibleId, MaterialItemCode, MaterialGroupCode, MaterialGroupDescription, ShortDescription, StateConservationId, NumberDoc, InitialId, ValueAcquisitionModel, MaterialItemDescription, ShortDescriptionItemId, ContaContabilApresentacaoEdicao, NumberIdentification, DiferenciacaoChapa,LifeCycle, ValueDevalue, Observation, ValueAcquisition, ValueUpdate, flagAcervo, flagTerceiro,checkFlagAcervo, checkFlagTerceiro, NumberPurchaseProcess, SerialNumber,DateGuarantee,Brand,NumberPlate,ManufactureDate,ChassiNumber,Model,AdditionalDescription,StateConservationId, Empenho, checkflagAnimalAServico, Status")] AssetViewModel asset)
        {
            try
            {
                bool gravaNovaChapa = false;
                asset.DiferenciacaoChapa = asset.DiferenciacaoChapa ?? "";
                using (db = new SAMContext())
                {
                    Asset _asset = db.Assets.Find(asset.Id);

                    if (!_asset.Status)
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);

                    if (asset.NumberIdentification != null)
                    {
                        int paraValidarChapa;
                        if (!int.TryParse(asset.NumberIdentification, out paraValidarChapa))
                        {
                            ModelState.AddModelError("NumberIdentification", "Favor informar uma chapa numérica!");
                        }
                        else
                        {

                            if (_asset.NumberIdentification.Trim() != asset.NumberIdentification.Trim() || _asset.InitialId != asset.InitialId || _asset.DiferenciacaoChapa != asset.DiferenciacaoChapa)
                            {
                                var assetDB = BPAtivoComMesmaSiglaChapa(asset);

                                if (assetDB != null)
                                {
                                    var inicialDB = (from i in db.Initials
                                                     where i.Id == assetDB.InitialId
                                                     select i).FirstOrDefault();

                                    var managerUnit = (from i in db.ManagerUnits
                                                       where i.Id == assetDB.ManagerUnitId
                                                       select i).FirstOrDefault();

                                    ModelState.AddModelError("NumberIdentification", $"A Sigla {inicialDB.Name} e Chapa {assetDB.NumberIdentification + assetDB.DiferenciacaoChapa} já estão cadastradas na UGE \"{managerUnit.Code}\" - {managerUnit.Description}");
                                }
                                else
                                {
                                    gravaNovaChapa = true;
                                }
                            }
                        }
                    }

                    // Numero do processo nao requirido na edicao.
                    ModelState.Remove("NumberPurchaseProcess");
                    // Responsavel nao requirido na edicao.
                    ModelState.Remove("ResponsibleId");
                    // UA nao requirido na edicao.
                    ModelState.Remove("AdministrativeUnitId");

                    if (ModelState.IsValid)
                    {
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                        {
                        
                            if (gravaNovaChapa)
                            {
                                var usuario = UserCommon.CurrentUser();

                                //Insere novo histórico de chapa do bem
                                var assetNumberIdentification = new AssetNumberIdentification();
                                assetNumberIdentification.NumberIdentification = _asset.NumberIdentification;
                                assetNumberIdentification.DiferenciacaoChapa = _asset.DiferenciacaoChapa;
                                assetNumberIdentification.InitialId = _asset.InitialId;
                                assetNumberIdentification.Status = true;
                                assetNumberIdentification.AssetId = asset.Id;
                                assetNumberIdentification.Login = usuario.CPF;
                                assetNumberIdentification.InclusionDate = DateTime.Now;

                                db.Entry(assetNumberIdentification).State = EntityState.Added;
                                db.SaveChanges();
                            }

                            if (asset.MaterialItemCode != _asset.MaterialItemCode)
                            {
                                if (ValidaAlteracaoDeItemMaterial(asset, _asset.MaterialItemCode))
                                {
                                    AlteraItemMaterialDaDepreciacaoDoBP(_asset.AssetStartId, asset.MaterialItemCode);
                                    _asset.MaterialItemCode = asset.MaterialItemCode;
                                    _asset.MaterialItemDescription = asset.MaterialItemDescription;
                                }
                            }

                            //Altera a conta auxiliar no movimento----------------------------------------------
                            AssetMovements historicoAtivoAtualDoBP = db.AssetMovements.Where(a => a.AssetId == _asset.Id && a.Status == true).FirstOrDefault();
                            historicoAtivoAtualDoBP.NumberPurchaseProcess = asset.NumberPurchaseProcess;

                            //Permitir que o estado de conservação do BP seja realizado - Solicitado: 04/07/2018 -----------------------
                            if (historicoAtivoAtualDoBP.StateConservationId != asset.StateConservationId)
                            {
                                historicoAtivoAtualDoBP.StateConservationId = asset.StateConservationId;
                            }
                            //---------------------------------------------------------------------------------------------------------

                            db.Entry(historicoAtivoAtualDoBP).State = EntityState.Modified;
                            db.SalvaEdicao(_asset.Id, historicoAtivoAtualDoBP.Id);

                            _asset.NumberIdentification = asset.NumberIdentification;
                            _asset.DiferenciacaoChapa = asset.DiferenciacaoChapa;
                            _asset.InitialId = asset.InitialId;
                            _asset.InitialName = (from i in db.Initials where i.Id == asset.InitialId select i.Name).FirstOrDefault();

                            //Dados complementares
                            if (string.IsNullOrEmpty(_asset.Empenho) || string.IsNullOrWhiteSpace(_asset.Empenho))
                                _asset.Empenho = asset.Empenho;
                            _asset.SerialNumber = asset.SerialNumber;
                            _asset.DateGuarantee = asset.DateGuarantee;
                            _asset.Brand = asset.Brand;
                            _asset.NumberPlate = asset.NumberPlate;
                            _asset.ManufactureDate = asset.ManufactureDate;
                            _asset.ChassiNumber = asset.ChassiNumber;
                            _asset.Model = asset.Model;
                            _asset.AdditionalDescription = asset.AdditionalDescription;

                            // Permitir que os usuarios alterem a "Descricao Resumida" dos BPs manualmente - Solicitacao: 23/05/2018
                            if (_asset.RelatedShortDescriptionItem.Description != asset.ShortDescription)
                            {

                                ShortDescriptionItem _shortDescriptionItem = new ShortDescriptionItem();
                                _shortDescriptionItem.Description = asset.ShortDescription;

                                var _ShortDescription = (from a in db.ShortDescriptionItens where a.Description == _shortDescriptionItem.Description select a).FirstOrDefault();

                                if (_ShortDescription.IsNotNull())
                                {
                                    _asset.ShortDescriptionItemId = _ShortDescription.Id;
                                }
                                else
                                {
                                    db.Entry(_shortDescriptionItem).State = EntityState.Added;
                                    db.SaveChanges();

                                    _asset.ShortDescriptionItemId = _shortDescriptionItem.Id;
                                }
                            }

                            db.Entry(_asset).State = EntityState.Modified;

                            db.SalvaEdicao(_asset.Id, historicoAtivoAtualDoBP.Id);

                            transaction.Complete();
                        }

                        return RedirectToAction("Index", "Movimento");
                    }
                    else
                    {
                        asset.AssetNumberIdentifications = RetornaHistoricoDeChapas(asset.Id);
                        PreencheDadosDepreciacao(asset);
                        PreencheDadosHistoricoValoresDecreto(asset);
                        PreencheDadosNLsSIAFEM02(asset);
                        PreencheDadosTerceiro(asset);
                        CarregaViewBags(asset);
                        asset.ValueAcquisitionModel = _asset.ValueAcquisition.ToString();
                        asset.ValueUpdateModel = _asset.ValueUpdate.ToString();

                        if (asset.Status)
                        {
                            asset.podeSerExcluido = BPNaoSeTratarDeAceite(asset.Id) && BPPodeSerExcluidoPorNaoTerPendenciaSIAFEM(asset.Id);
                            asset.podeEditarItemMaterial = PermitidoAlterarItemMaterial(asset);
                        }

                        return View(asset);
                    }
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion Edit

        #region comentada
        //private AssetViewModel CarregaAsset(AssetEAssetMovimentViewModel assetEAssetMoviment)
        //{
        //    AssetViewModel asset = new AssetViewModel();
        //    asset.AssetStartId = assetEAssetMoviment.Asset.AssetStartId;
        //    asset.Id = assetEAssetMoviment.Asset.Id;
        //    asset.InitialId = assetEAssetMoviment.Asset.InitialId;
        //    asset.InitialDescription = assetEAssetMoviment.Asset.RelatedInitial == null ? null : assetEAssetMoviment.Asset.RelatedInitial.Description;
        //    asset.NumberIdentification = assetEAssetMoviment.Asset.NumberIdentification;
        //    asset.AcquisitionDate = assetEAssetMoviment.Asset.AcquisitionDate;
        //    asset.MovimentDate = assetEAssetMoviment.Asset.MovimentDate;
        //    asset.ValueAcquisition = assetEAssetMoviment.Asset.ValueAcquisition;
        //    asset.ValueUpdate = assetEAssetMoviment.Asset.ValueUpdate;
        //    asset.ValueUpdateModel = assetEAssetMoviment.Asset.ValueUpdate.ToString();
        //    asset.ValueAcquisitionModel = assetEAssetMoviment.Asset.ValueAcquisition.ToString();
        //    asset.MaterialItemCode = assetEAssetMoviment.Asset.MaterialItemCode;
        //    asset.MaterialItemDescription = assetEAssetMoviment.Asset.MaterialItemDescription;
        //    asset.MaterialGroupCode = assetEAssetMoviment.Asset.MaterialGroupCode;
        //    asset.MaterialGroupDescription = (from m in db.MaterialGroups where m.Code == assetEAssetMoviment.Asset.MaterialGroupCode select m.Description).FirstOrDefault();
        //    asset.ShortDescription = assetEAssetMoviment.Asset.RelatedShortDescriptionItem == null ? null : assetEAssetMoviment.Asset.RelatedShortDescriptionItem.Description;
        //    asset.LifeCycle = assetEAssetMoviment.Asset.LifeCycle;
        //    asset.RateDepreciationMonthly = assetEAssetMoviment.Asset.RateDepreciationMonthly;
        //    asset.ResidualValue = assetEAssetMoviment.Asset.ResidualValue;
        //    asset.ShortDescriptionItemId = assetEAssetMoviment.Asset.ShortDescriptionItemId;
        //    asset.OldInitial = assetEAssetMoviment.Asset.OldInitial;
        //    asset.OldNumberIdentification = assetEAssetMoviment.Asset.OldNumberIdentification;
        //    asset.NumberDoc = assetEAssetMoviment.Asset.NumberDoc;
        //    asset.Empenho = assetEAssetMoviment.Asset.Empenho;
        //    asset.ManagerUnitId = assetEAssetMoviment.AssetMoviment.ManagerUnitId;
        //    asset.flagCalculoPendente = assetEAssetMoviment.Asset.flagCalculoPendente;
        //    asset.flagDepreciationCompleted = assetEAssetMoviment.Asset.flagDepreciationCompleted == null ? false : assetEAssetMoviment.Asset.flagDepreciationCompleted;
        //    asset.NumeroDocumentoAtual = assetEAssetMoviment.AssetMoviment.NumberDoc;

        //    // Dados Complementares
        //    asset.SerialNumber = assetEAssetMoviment.Asset.SerialNumber;
        //    asset.DateGuarantee = assetEAssetMoviment.Asset.DateGuarantee;
        //    asset.Brand = assetEAssetMoviment.Asset.Brand;
        //    asset.NumberPlate = assetEAssetMoviment.Asset.NumberPlate;
        //    asset.ManufactureDate = assetEAssetMoviment.Asset.ManufactureDate;
        //    asset.ChassiNumber = assetEAssetMoviment.Asset.ChassiNumber;
        //    asset.Model = assetEAssetMoviment.Asset.Model;
        //    asset.AdditionalDescription = assetEAssetMoviment.Asset.AdditionalDescription;

        //    asset.AssetMovementsId = assetEAssetMoviment.AssetMoviment.Id;
        //    asset.StateConservationId = assetEAssetMoviment.AssetMoviment.StateConservationId;
        //    asset.NumberPurchaseProcess = assetEAssetMoviment.AssetMoviment.NumberPurchaseProcess;
        //    asset.InstitutionId = assetEAssetMoviment.AssetMoviment.InstitutionId;
        //    asset.BudgetUnitId = assetEAssetMoviment.AssetMoviment.BudgetUnitId;
        //    asset.BudgetUnitCode = assetEAssetMoviment.AssetMoviment.RelatedBudgetUnit.Code;
        //    asset.ManagerUnitId = assetEAssetMoviment.AssetMoviment.ManagerUnitId;
        //    asset.ManagerUnitCode = assetEAssetMoviment.AssetMoviment.RelatedManagerUnit.Code;
        //    asset.AdministrativeUnitId = assetEAssetMoviment.AssetMoviment.AdministrativeUnitId;
        //    asset.AdministrativeUnitCode = assetEAssetMoviment.AssetMoviment.RelatedAdministrativeUnit == null ? 0 : assetEAssetMoviment.AssetMoviment.RelatedAdministrativeUnit.Code;
        //    asset.SectionId = assetEAssetMoviment.AssetMoviment.SectionId;
        //    asset.SectionCode = assetEAssetMoviment.AssetMoviment.RelatedSection == null ? 0 : assetEAssetMoviment.AssetMoviment.RelatedSection.Code;
        //    asset.AuxiliaryAccountId = assetEAssetMoviment.AssetMoviment.AuxiliaryAccountId;
        //    asset.AuxiliaryAccountCode = assetEAssetMoviment.AssetMoviment.RelatedAuxiliaryAccount == null ? 0 : assetEAssetMoviment.AssetMoviment.RelatedAuxiliaryAccount.Code;
        //    asset.ResponsibleId = assetEAssetMoviment.AssetMoviment.ResponsibleId;
        //    asset.ResponsibleName = assetEAssetMoviment.AssetMoviment.RelatedResponsible == null ? null : assetEAssetMoviment.AssetMoviment.RelatedResponsible.Name;

        //    asset.LifeCycle = assetEAssetMoviment.Asset.LifeCycle;
        //    asset.checkFlagAcervo = assetEAssetMoviment.Asset.flagAcervo == null ? false : ((bool)assetEAssetMoviment.Asset.flagAcervo);
        //    asset.checkFlagTerceiro = assetEAssetMoviment.Asset.flagTerceiro == null ? false : ((bool)assetEAssetMoviment.Asset.flagTerceiro);
        //    asset.checkFlagDecretoSefaz = assetEAssetMoviment.Asset.flagDecreto == null ? false : ((bool)assetEAssetMoviment.Asset.flagDecreto);

        //    asset.MovementTypeId = assetEAssetMoviment.Asset.MovementTypeId;
        //    VerificaAnimal(asset);
        //    asset.AssetNumberIdentifications = RetornaHistoricoDeChapas(assetEAssetMoviment.Asset.Id);
        //    PreencheDadosDepreciacao(asset);

        //    asset.RelatedItemInventarios = assetEAssetMoviment.Asset.RelatedItemInventarios;
        //    CarregaViewBags(asset);
        //    CarregaViewBagContasContabeisEdicao(asset);

        //    return asset;
        //}

        //
        #endregion
        //context em outros métodos
        private void CarregaViewBags(AssetViewModel asset)
        {
            SetCarregaHierarquia2(asset.InstitutionId, asset.BudgetUnitId, asset.ManagerUnitId, asset.AdministrativeUnitId ?? 0, asset.SectionId ?? 0, asset.ResponsibleId ?? 0);

            ViewBag.Initial = new SelectList(db.Initials.Where(i => i.InstitutionId == asset.InstitutionId && (i.BudgetUnitId == asset.BudgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == asset.ManagerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).AsNoTracking().ToList(), "Id", "Name", asset.InitialId);
            ViewBag.StateConservations = new SelectList(db.StateConservations.OrderBy(m => m.Description).AsNoTracking().ToList(), "Id", "Description", asset.StateConservationId);
            //CarregaViewBagContasContabeisEdicao(asset);
            //ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == _institutionId && (i.BudgetUnitId == _budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == _managerUnitId || i.ManagerUnitId == null) && i.Name != "SIGLA_GENERICA" && i.Description != "SIGLA_GENERICA" && i.Status == true).OrderBy(i=>i.Name), "Id", "Name", asset.OldInitial);
        }

        //context em outros métodos
        private void CarregaViewBagContasContabeis(AssetViewModel assetViewModel)
        {
            int tipo = 0;
            bool? tipoAcervo = false;
            bool? tipoTerceiro = false;
            BuscaContasContabeis buscaContas = new BuscaContasContabeis();
            if (   assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorporacaoDeInventarioInicial
                || assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao
                || assetViewModel.MovementTypeId == (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos)
            {
                tipo = assetViewModel.checkFlagAcervo == true ? 1 : (assetViewModel.checkFlagTerceiro == true ? 2 : 0);

                if (tipo == 0)
                {
                    ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaAuxiliarPorGrupoMaterial(assetViewModel.MaterialGroupCode), "Id", "Description", assetViewModel.AuxiliaryAccountId);
                }
                else
                {
                    ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaContabilPorTipo(tipo), "Id", "Description", assetViewModel.AuxiliaryAccountId);
                }
            }
            else
            {
                if (assetViewModel.AssetsIdTransferencia == null)
                {
                    ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaAuxiliarPorGrupoMaterial(assetViewModel.MaterialGroupCode), "Id", "Description", assetViewModel.AuxiliaryAccountId);
                }
                else
                {
                    tipoAcervo = (from a in db.Assets
                                  where a.Id == assetViewModel.AssetsIdTransferencia
                                  select a.flagAcervo).FirstOrDefault();

                    tipoTerceiro = (from a in db.Assets
                                    where a.Id == assetViewModel.AssetsIdTransferencia
                                    select a.flagTerceiro).FirstOrDefault();

                    tipo = tipoAcervo == true ? 1 : (tipoTerceiro == true ? 2 : 0);

                    if (tipo == 0)
                    {
                        ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaAuxiliarPorGrupoMaterial(assetViewModel.MaterialGroupCode), "Id", "Description", assetViewModel.AuxiliaryAccountId);
                    }
                    else {
                        ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaContabilPorTipo(tipo), "Id", "Description", assetViewModel.AuxiliaryAccountId);
                    }

                }
            }
        }
        //context em outros métodos
        //private void CarregaViewBagContasContabeisEdicao(AssetViewModel assetViewModel)
        //{
        //    if ((from ram in db.RelationshipAuxiliaryAccountMovementTypes
        //         join am in db.AssetMovements on ram.MovementTypeId equals am.MovementTypeId
        //         where am.AssetId == assetViewModel.Id 
        //         && (am.FlagEstorno == null || am.FlagEstorno == false)
        //         select ram).Any())
        //    {

        //        int ultimoHistoricoComRelacao = (from ram in db.RelationshipAuxiliaryAccountMovementTypes
        //                                         join am in db.AssetMovements on ram.MovementTypeId equals am.MovementTypeId
        //                                         where am.AssetId == assetViewModel.Id
        //                                         select am).OrderByDescending(x => x.Id).FirstOrDefault().MovementTypeId;

        //        int ContaContabilId = (from ram in db.RelationshipAuxiliaryAccountMovementTypes
        //                               where ram.MovementTypeId == ultimoHistoricoComRelacao
        //                               select ram.AuxiliaryAccountId).FirstOrDefault();

        //        var listaRetorno = (from a in db.AuxiliaryAccounts where a.Id == ContaContabilId && a.Status == true select a).ToList();

        //        listaRetorno.ForEach(o => o.Description = o.ContaContabilApresentacao + " - " + o.Description);

        //        listaRetorno.OrderBy(a => a.BookAccount);

        //        ViewBag.AuxiliaryAccounts = new SelectList(listaRetorno, "Id", "Description", "0");

        //    }
        //    else
        //    {

        //        int tipo = 0;
        //        BuscaContasContabeis buscaContas = new BuscaContasContabeis();

        //        tipo = assetViewModel.checkFlagAcervo == true ? 1 : (assetViewModel.checkFlagTerceiro == true ? 2 : 0);

        //        if (tipo == 0)
        //        {
        //            ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaAuxiliarPorGrupoMaterial(assetViewModel.MaterialGroupCode), "Id", "Description", assetViewModel.AuxiliaryAccountId);
        //        }
        //        else
        //        {
        //            ViewBag.AuxiliaryAccounts = new SelectList(buscaContas.GetContaContabilPorTipo(tipo), "Id", "Description", assetViewModel.AuxiliaryAccountId);
        //        }
        //    }
        //}

        #region SetCarregaHierarquia
        //context em outros métodos
        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, int modelInstitutionDestinoId = 0, int modelBudgetUnitDestinoId = 0, int modelManagerUnitDestinoId = 0)
        {
            hierarquia = new Hierarquia();
            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");

                if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");

                if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(null), "Id", "Description");

                if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(null), "Id", "Description");
            }
            else
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);

                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);

                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId), "Id", "Description", modelManagerUnitId);
                else if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId), "Id", "Description", modelManagerUnitId);

                if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(_administrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
                else if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(_managerUnitId), "Id", "Description", modelAdministrativeUnitId);

                if (_sectionId.HasValue && _sectionId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(_sectionId), "Id", "Description", modelSectionId);
                else if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(_administrativeUnitId), "Id", "Description", modelSectionId);
            }

            bool habilitaIntegracaoContabilizaSP = false;

            var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
            if (perflLogado.ManagerUnitId != null && perflLogado.ManagerUnitId != 0)
            {
                this.initDadosSIAFEM();
                var uge = db.ManagerUnits.Find(perflLogado.ManagerUnitId);

                if (uge != null)
                {
                    int mesReferencia = int.Parse(uge.ManagmentUnit_YearMonthReference);

                    habilitaIntegracaoContabilizaSP = (base.ugeIntegradaSiafem && (mesReferencia >= uge.MesRefInicioIntegracaoSIAFEM));
                }
            }

            ViewData["flagIntegracaoSiafem"] = (habilitaIntegracaoContabilizaSP ? 1 : 0);

        }
        //context em outros métodos
        private void SetCarregaHierarquia2(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, int modelResponsibleId = 0)
        {
            
            hierarquia = new Hierarquia();

            //var vUser = new List<User>();
            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");

                if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");

                if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(null), "Id", "Description");

                if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(null), "Id", "Description");
            }
            else
            {
                getHierarquiaPerfil();
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);

                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId).Where(b => b.Code != "99999"), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId).Where(b => b.Code != "99999"), "Id", "Description", modelBudgetUnitId);

                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId).Where(b => b.Code != "999999"), "Id", "Description", modelManagerUnitId);
                else if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId).Where(b => b.Code != "999999"), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId).Where(b => b.Code != "999999"), "Id", "Description", modelManagerUnitId);

                if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(_administrativeUnitId).Where(b => b.Code != 99999999), "Id", "Description", modelAdministrativeUnitId);
                else if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId).Where(b => b.Code != 99999999), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(_managerUnitId).Where(b => b.Code != 99999999), "Id", "Description", modelAdministrativeUnitId);

                if (_sectionId.HasValue && _sectionId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(_sectionId).Where(b => b.Code != 999), "Id", "Description", modelSectionId);
                else if (modelAdministrativeUnitId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId).Where(b => b.Code != 999), "Id", "Description", modelSectionId);
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(_administrativeUnitId).Where(b => b.Code != 999), "Id", "Description", modelSectionId);
            }

            #region Combo Responsavel

            IEnumerable<Responsible> vResponsible;
            if (modelAdministrativeUnitId != 0)
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == modelAdministrativeUnitId && !r.Name.Contains("RESPONSAVEL_") select r).ToList();
            else
                vResponsible = new List<Responsible>();

            if (modelResponsibleId != 0)
                ViewBag.Responsible = new SelectList(vResponsible.ToList(), "Id", "Name", modelResponsibleId);
            else
                ViewBag.Responsible = new SelectList(vResponsible.ToList(), "Id", "Name");

            #endregion
        }

        //context em outros métodos
        private void SetCarregaHierarquiaInventario(int InstitutionId, int BudgetUnitId, int ManagerUnitId, int UaId, int? DivisaoId, int ResponsibleId) {

            ViewBag.Institutions = new SelectList(
                (from i in db.Institutions where i.Id == InstitutionId select new { Id = i.Id, Description = i.Code.ToString() + " - " + i.Description }).AsNoTracking().ToList(), 
                "Id", "Description", InstitutionId);

            ViewBag.BudgetUnits = new SelectList(
                (from b in db.BudgetUnits where b.Id == BudgetUnitId select new { Id = b.Id, Description = b.Code.ToString() + " - " + b.Description }).AsNoTracking().ToList(),
                "Id", "Description", BudgetUnitId);

            ViewBag.ManagerUnitId = new SelectList(
                (from m in db.ManagerUnits where m.Id == ManagerUnitId select new { Id = m.Id, Description = m.Code.ToString() + " - " + m.Description }).AsNoTracking().ToList(),
                "Id", "Description", ManagerUnitId);

            ViewBag.AdministrativeUnits = new SelectList(
                (from a in db.AdministrativeUnits where a.Id == UaId select new { Id = a.Id, Description = a.Code.ToString() + " - " + a.Description }).AsNoTracking().ToList(),
                "Id", "Description", UaId);

            List<ItemGenericViewModel> divisoes = new List<ItemGenericViewModel>();

            divisoes.Add(new ItemGenericViewModel() { Id = 0, Description = "Selecione uma divisão" });

            if (DivisaoId.IsNotNull() && DivisaoId != 0)
            {
                var testeb = (from s in db.Sections where s.Id == DivisaoId select new ItemGenericViewModel { Id = s.Id, Description = s.Code.ToString() + " - " + s.Description }).AsNoTracking().ToList();

                testeb.ForEach(o => divisoes.Add(o));

                ViewBag.Sections = new SelectList(
                    divisoes,
                    "Id", "Description", DivisaoId);
            }
            else
            {
                var testeb = (from s in db.Sections where s.AdministrativeUnitId == UaId && s.ResponsibleId == ResponsibleId select new ItemGenericViewModel { Id = s.Id, Description = s.Code.ToString() + " - " + s.Description }).AsNoTracking().ToList();

                testeb.ForEach(o => divisoes.Add(o));

                ViewBag.Sections = new SelectList(divisoes, "Id", "Description",0);
            }
        }

        #endregion 
        
        #region Delete
        // GET: Assets/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    BPExclusaoViewModel asset = (from a in db.Assets
                                                 join am in db.AssetMovements
                                                 on a.Id equals am.AssetId
                                                 where a.Id == id
                                                 && am.Status
                                                 && a.flagDepreciaAcumulada == 1
                                                 && a.flagVerificado == null
                                                 select new BPExclusaoViewModel {
                                                     Id = a.Id,
                                                     IdOrgaoAtual = am.InstitutionId,
                                                     IdUOAtual = am.BudgetUnitId,
                                                     IdUGEAtual = am.ManagerUnitId,
                                                     IdEstadoDeConservacao = am.StateConservationId,
                                                     Sigla = a.RelatedInitial.Name,
                                                     Chapa = a.NumberIdentification,
                                                     ItemMaterial = a.MaterialItemCode,
                                                     GrupoMaterial = a.MaterialGroupCode,
                                                     DataDeAquisicaoCompleta = a.AcquisitionDate,
                                                     ValorDeAquisicao = a.ValueAcquisition,
                                                     DescricaoResumida = a.RelatedShortDescriptionItem.Description,
                                                     flagAcervo = (a.flagAcervo == true),
                                                     flagTerceiro = (a.flagTerceiro == true),
                                                     flagDecreto = (a.flagDecreto == true),
                                                     Processo = am.NumberPurchaseProcess
                                                 }).FirstOrDefault();

                    if (asset == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(asset.IdOrgaoAtual, asset.IdUOAtual, asset.IdUGEAtual, null, null))
                    {
                        if (!BPNaoSeTratarDeAceite(asset.Id))
                            return MensagemErro("BP não pode ser excluído devido a seu histórico");

                        if (!BPPodeSerExcluidoPorNaoTerPendenciaSIAFEM(asset.Id))
                            return MensagemErro("BP não pode ser excluído, pois possui pendências atreladas à integração com o SIAFEM");

                        asset.DescricaoDoGrupo = db.MaterialGroups
                                                   .Where(g => g.Code == asset.GrupoMaterial)
                                                   .Select(g => g.Description)
                                                   .FirstOrDefault();

                        asset.historico = (from am in db.AssetMovements
                                           where am.AssetId == id &&
                                           am.FlagEstorno != true
                                           select new HistoricoBPExclusaoViewModel
                                           {
                                               DataCompletoHistorico = am.MovimentDate,
                                               Historico = am.RelatedMovementType.Description,
                                               Orgao = am.RelatedInstitution.NameManagerReduced,
                                               UO = am.RelatedBudgetUnit.Code,
                                               UGE = am.RelatedManagerUnit.Code,
                                               UA = am.AdministrativeUnitId == null ? "" : am.RelatedAdministrativeUnit.Description,
                                               Divisao = am.SectionId == null ? "" : am.RelatedSection.Description,
                                               Responsavel = am.ResponsibleId == null ? "" : am.RelatedResponsible.Name,
                                               ContaContabil = am.AuxiliaryAccountId == null ? "" : am.RelatedAuxiliaryAccount.ContaContabilApresentacao,
                                               NumeroDocumento = am.NumberDoc,
                                               Observacao = am.Observation,
                                               ValorDoReparo = am.RepairValue ?? 0,
                                               CPFCPNJ = am.CPFCNPJ
                                           }).ToList();

                        return View(asset);
                    }
                    
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include = "Id, Observacao")]BPExclusaoViewModel viewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(viewModel.Observacao) || string.IsNullOrWhiteSpace(viewModel.Observacao))
                    return MensagemErro("Exclusão não foi realizada, pois não possui o campo observação");

                using (db = new SAMContext()) { 
                    if (!BPNaoSeTratarDeAceite(viewModel.Id))
                        return MensagemErro("BP não pode ser excluído devido a seu histórico");

                    if (!BPPodeSerExcluidoPorNaoTerPendenciaSIAFEM(viewModel.Id))
                        return MensagemErro("BP não pode ser excluído, pois possui pendências atreladas à integração com o SIAFEM");

                    BPExclusaoViewModel asset = (from a in db.Assets
                                                 join am in db.AssetMovements
                                                 on a.Id equals am.AssetId
                                                 where a.Id == viewModel.Id
                                                 && am.Status
                                                 && a.flagDepreciaAcumulada == 1
                                                 && a.flagVerificado == null
                                                 select new BPExclusaoViewModel
                                                 {
                                                     IdOrgaoAtual = am.InstitutionId,
                                                     IdUOAtual = am.BudgetUnitId,
                                                     IdUGEAtual = am.ManagerUnitId
                                                 }).FirstOrDefault();

                    if (!ValidarRequisicao(asset.IdOrgaoAtual, asset.IdUOAtual, asset.IdUGEAtual, null, null))
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);

                    DateTime DataAcao = DateTime.Now;
                    var userId = UserCommon.CurrentUser().Id;

                    List<BPsExcluidos> listaBPsExcluidos = (from a in db.Assets
                                                            join am in db.AssetMovements
                                                            on a.Id equals am.AssetId
                                                            join m in db.MovementTypes
                                                            on am.MovementTypeId equals m.Id
                                                            where a.Id == viewModel.Id
                                                            && am.FlagEstorno != true
                                                            select new {
                                                                TipoIncorporacao = am.MovementTypeId,
                                                                InitialId = a.InitialId,
                                                                SiglaInicial = a.InitialName,
                                                                Chapa = a.NumberIdentification,
                                                                ItemMaterial = a.MaterialItemCode,
                                                                GrupoMaterial = a.MaterialGroupCode,
                                                                StateConservationId = am.StateConservationId,
                                                                ManagerUnitId = am.ManagerUnitId,
                                                                AdministrativeUnitId = am.AdministrativeUnitId,
                                                                ResponsibleId = am.ResponsibleId,
                                                                Processo = am.NumberPurchaseProcess,
                                                                DataAquisicao = a.AcquisitionDate,
                                                                ValorAquisicao = a.ValueAcquisition,
                                                                DataIncorporacao = am.MovimentDate,
                                                                flagAcervo = a.flagAcervo ?? false,
                                                                flagTerceiro = a.flagTerceiro ?? false,
                                                                flagDecretoSEFAZ = a.flagDecreto ?? false,
                                                                NotaLancamento = am.NotaLancamento,
                                                                NotaLancamentoEstorno = am.NotaLancamentoEstorno,
                                                                NotaLancamentoDepreciacao = am.NotaLancamentoDepreciacao,
                                                                NotaLancamentoDepreciacaoEstorno = am.NotaLancamentoDepreciacaoEstorno,
                                                                NotaLancamentoReclassificacao = am.NotaLancamentoReclassificacao,
                                                                NotaLancamentoReclassificacaoEstorno = am.NotaLancamentoReclassificacaoEstorno,
                                                                Observacoes = am.Observation,
                                                                NumeroDocumento = am.NumberDoc ?? (m.GroupMovimentId == (int)EnumGroupMoviment.Incorporacao ? a.NumberDoc : null)
                                                            }).ToList()
                                                            .Select(bp => new BPsExcluidos {
                                                                TipoIncorporacao = bp.TipoIncorporacao,
                                                                InitialId = bp.InitialId,
                                                                SiglaInicial = bp.SiglaInicial,
                                                                Chapa = bp.Chapa,
                                                                ItemMaterial = bp.ItemMaterial,
                                                                GrupoMaterial = bp.GrupoMaterial,
                                                                StateConservationId = bp.StateConservationId,
                                                                ManagerUnitId = bp.ManagerUnitId,
                                                                AdministrativeUnitId = bp.AdministrativeUnitId,
                                                                ResponsibleId = bp.ResponsibleId,
                                                                Processo = bp.Processo,
                                                                DataAquisicao = bp.DataAquisicao,
                                                                ValorAquisicao = bp.ValorAquisicao,
                                                                DataIncorporacao = bp.DataIncorporacao,
                                                                flagAcervo = bp.flagAcervo,
                                                                flagTerceiro = bp.flagTerceiro,
                                                                flagDecretoSEFAZ = bp.flagDecretoSEFAZ,
                                                                DataAcao = DataAcao,
                                                                LoginAcao = userId,
                                                                NotaLancamento = bp.NotaLancamento,
                                                                NotaLancamentoEstorno = bp.NotaLancamentoEstorno,
                                                                NotaLancamentoDepreciacao = bp.NotaLancamentoDepreciacao,
                                                                NotaLancamentoDepreciacaoEstorno = bp.NotaLancamentoDepreciacaoEstorno,
                                                                NotaLancamentoReclassificacao = bp.NotaLancamentoReclassificacao,
                                                                NotaLancamentoReclassificacaoEstorno = bp.NotaLancamentoReclassificacaoEstorno,
                                                                Observacoes = bp.Observacoes,
                                                                NumeroDocumento = bp.NumeroDocumento,
                                                                FlagModoExclusao = (byte)EnumMotivoExclusaoDoBP.ExclusaoPorDadosErrados,
                                                                MotivoExclusao = viewModel.Observacao
                                                            }).ToList();

                    int IdContaContabil = db.AssetMovements
                                            .Where(am => am.AssetId == viewModel.Id)
                                            .First()
                                            .AuxiliaryAccountId ?? 0;

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        db.BPsExcluidos.AddRange(listaBPsExcluidos);
                        db.SaveChanges();

                        ExcluirRegistrosdoBP(viewModel.Id);
                        ComputaAlteracaoContabil(asset.IdOrgaoAtual, asset.IdUOAtual, asset.IdUGEAtual, IdContaContabil);
                        transaction.Complete();
                    }
                }

                return RedirectToAction("Index","Movimento");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region LoadSelects

        //context em outros métodos
        private void LoadSelectElements(AssetViewModel asset)
        {
            getHierarquiaPerfil();

            this.db.Configuration.AutoDetectChangesEnabled = false;
            this.db.Configuration.ProxyCreationEnabled = false;
            this.db.Configuration.LazyLoadingEnabled = false;
            try
            {
                
                //var vUser = new List<User>();   //TODO: Mudar quando for usuario
                var vResponsible = new List<Responsible>();
                if (asset == null)
                {
                    SetCarregaHierarquia();

                    ViewBag.MovementTypes = new SelectList(new BuscaIncorporacoesAtivas(db,_institutionId).listaIncorporacoes, "Id", "Description");

                    //if (!OrgaoFundoSocial(_institutionId))
                    //{
                    //    ViewBag.MovementTypes = new SelectList(db.MovementTypes.Where(m => m.GroupMovimentId == (int)EnumGroupMoviment.Incorporacao && m.Status == true
                    //                                           && m.Id != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao && m.Id != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
                    //                                           .OrderBy(m => m.Description).ToList(), "Id", "Description");
                    //}
                    //else {
                    //    ViewBag.MovementTypes = new SelectList(db.MovementTypes.Where(m => m.GroupMovimentId == (int)EnumGroupMoviment.Incorporacao && m.Status == true).OrderBy(m => m.Description).ToList(), "Id", "Description");
                    //}

                    var listaDeSiglas = new BuscaSiglas(db, _institutionId, _budgetUnitId, _managerUnitId).lista;
                    ViewBag.Initial = new SelectList(listaDeSiglas, "Id", "Name");
                    ViewBag.OldInitial = new SelectList(listaDeSiglas, "Id", "Name");
                    ViewBag.StateConservations = new SelectList(db.StateConservations.OrderBy(m => m.Description).ToList(), "Id", "Description");

                    #region Combo Responsavel
                        if (_sectionId.HasValue)
                            vResponsible = (from r in db.Responsibles join s in db.Sections on r.Id equals s.ResponsibleId where s.Id == _sectionId select r).ToList();
                        else if (_administrativeUnitId.HasValue)
                            vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == _administrativeUnitId select r).ToList();

                        ViewBag.User = new SelectList(vResponsible, "Id", "Name");
                        #endregion
                }
                else
                {
                    SetCarregaHierarquia(asset.InstitutionId, asset.BudgetUnitId, asset.ManagerUnitId, asset.AdministrativeUnitId ?? 0, asset.SectionId ?? 0, asset.InstituationIdDestino, asset.BudgetUnitIdDestino, asset.ManagerUnitIdDestino);

                    if (asset.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                        asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
                    {
                        CarregaHierarquiaDestinoNaoImplantados(asset.InstituationIdDestino, asset.BudgetUnitIdDestino, asset.ManagerUnitIdDestino, true);
                    } else 
                    if (asset.MovementTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                        asset.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao)
                    {
                        CarregaHierarquiaDestinoNaoImplantados(asset.InstituationIdDestino, asset.BudgetUnitIdDestino, asset.ManagerUnitIdDestino, false);
                    }
                    else
                    {
                        CarregaHierarquiaDestino(asset.InstituationIdDestino, asset.BudgetUnitIdDestino, asset.ManagerUnitIdDestino, 0, 0, false, RecuperaGestao());
                    }

                    if (asset.MovementTypeId == (int)EnumMovimentType.IncorporacaoPorEmpenho
                        || asset.MovementTypeId == (int)EnumMovimentType.IncorporacaoPorEmpenhoRestosAPagar)
                    {
                        // Verificar o preenchimento desta UG
                        ViewBag.Ug = (from o in db.Institutions where o.Id == _institutionId && o.Status == true select o.ManagerCode).First();
                    }

                    ViewBag.MovementTypes = new SelectList(new BuscaIncorporacoesAtivas(db, _institutionId).listaIncorporacoes, "Id", "Description");

                    CarregaViewBagContasContabeis(asset);

                    var listaDeSiglas = new BuscaSiglas(db, asset.InstitutionId, asset.BudgetUnitId, asset.ManagerUnitId).lista;
                    ViewBag.Initial = new SelectList(listaDeSiglas, "Id", "Name");
                    ViewBag.OldInitial = new SelectList(listaDeSiglas, "Id", "Name");
                    ViewBag.StateConservations = new SelectList(db.StateConservations.OrderBy(m => m.Description).ToList(), "Id", "Description", asset.StateConservationId);
                    
                    #region Combo Responsavel
                        if (_sectionId.HasValue)
                        {
                            vResponsible = (from r in db.Responsibles join s in db.Sections on r.Id equals s.ResponsibleId where s.Id == _sectionId select r).ToList();
                            ViewBag.User = new SelectList(vResponsible, "Id", "Name", _sectionId);
                        }
                        else if (asset.SectionId > 0)
                        {
                            vResponsible = (from r in db.Responsibles join s in db.Sections on r.Id equals s.ResponsibleId where s.Id == asset.SectionId select r).ToList();
                            ViewBag.User = new SelectList(vResponsible, "Id", "Name", asset.SectionId);
                        }
                        else if (_administrativeUnitId.HasValue)
                        {
                            vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == _administrativeUnitId select r).ToList();
                            ViewBag.User = new SelectList(vResponsible, "Id", "Name", _administrativeUnitId);
                        }
                        else if (asset.AdministrativeUnitId > 0)
                        {
                            vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == asset.AdministrativeUnitId select r).ToList();
                            ViewBag.User = new SelectList(vResponsible, "Id", "Name", asset.AdministrativeUnitId);
                        }
                        else
                        {
                            ViewBag.User = new SelectList(vResponsible, "Id", "Name");
                        }

                        #endregion
                }
            }
            catch (Exception ex)
            {
                if (db!= null && db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();
                throw ex;
            }
        }

        //context em outros métodos
        private void LoadSelectElementsInventarioPorId(int? id){
            if (id.HasValue) {
                ItemInventario itemInventario = (from i in db.ItemInventarios
                                                                 .Include("RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit")
                                                 where i.Id == id
                                                 select i).FirstOrDefault();
                LoadSelectElementsInventario(itemInventario);
            }
        }
        //context em outros métodos
        private void LoadSelectElementsInventario(ItemInventario itemInventario) {

            this.db.Configuration.AutoDetectChangesEnabled = false;
            this.db.Configuration.ProxyCreationEnabled = false;
            this.db.Configuration.LazyLoadingEnabled = false;

            int institutionId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
            int budgetUnitId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id;
            int managerUnitId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.Id;

            try
            {
                SetCarregaHierarquiaInventario(institutionId, budgetUnitId, managerUnitId, itemInventario.RelatedInventario.UaId,
                                               itemInventario.RelatedInventario.DivisaoId, itemInventario.RelatedInventario.ResponsavelId);

                ViewBag.Responsibles = new SelectList(
                (from r in db.Responsibles where r.Id == itemInventario.RelatedInventario.ResponsavelId select new { Id = r.Id, Name = r.Name }).AsNoTracking().ToList(),
                "Id", "Name", itemInventario.RelatedInventario.ResponsavelId);

                ViewBag.MovementTypes = new SelectList(db.MovementTypes.Where(m => (m.Id == (int)EnumMovimentType.IncorporacaoDeInventarioInicial ||
                                                                                    m.Id == (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos)
                                                       && m.Status == true).OrderBy(m => m.Description).ToList(), "Id", "Description");

                ViewBag.Initial = new SelectList(db.Initials.Where(i => i.InstitutionId == institutionId && (i.BudgetUnitId == budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList(), "Id", "Name");
                ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == institutionId && (i.BudgetUnitId == budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList(), "Id", "Name");
                
                ViewBag.StateConservations = new SelectList(db.StateConservations.OrderBy(m => m.Description).ToList(), "Id", "Description");
            }
            catch (Exception ex)
            {
                if (db != null && db.Database.CurrentTransaction != null)
                    db.Database.CurrentTransaction.Rollback();
                throw ex;
            }
        }
        
        #endregion
        //private IEnumerable GetSuppliersCombo()
        //{
        //    return db.Suppliers
        //                  .OrderBy(l => l.Name)
        //                  .Select(l => new ItemGenericViewModel { Id = l.Id, Description = l.CPFCNPJ + " - " + l.Name }).AsNoTracking().ToList();
        //}

        public JsonResult GetInicioDaProximaNumberIdentification(Int64 NumberIdentification)
        {
            db = new SAMContext();
            var result = db.Assets.Where(a => a.NumberIdentification.Equals(NumberIdentification.ToString())).ToList();

            if (result != null && result.Count > 0)
            {
                var number = result.Select(n => n.NumberIdentification).Max();
                return Json(number, JsonRequestBehavior.AllowGet);
            }

            return Json("False", JsonRequestBehavior.AllowGet);
        }

        public ActionResult CarregaPartialViewIncorporation(EnumMovimentType tipoMovimento)
        {
            switch (tipoMovimento)
            {
                case EnumMovimentType.IncorporacaoPorEmpenho:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        ViewBag.Ug = (from o in db.Institutions where o.Id == _institutionId && o.Status == true select o.ManagerCode).FirstOrDefault();
                    }
                    return PartialView("Incorporacao/_1partialEmpenho");

                case EnumMovimentType.IncorporacaoDeInventarioInicial:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == _institutionId && (i.BudgetUnitId == _budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == _managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList(), "Id", "Name");
                    }
                    return PartialView("Incorporacao/_5partialInventarioInicial");

                case EnumMovimentType.IncorporacaoDeMateriaisTransformadoPorTerceiro:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == _institutionId && (i.BudgetUnitId == _budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == _managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList(), "Id", "Name");
                    }
                    return PartialView("Incorporacao/_8partialMaterialTransforTerceiro");

                case EnumMovimentType.IncorporacaoPorEmpenhoRestosAPagar:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == _institutionId && (i.BudgetUnitId == _budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == _managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList(), "Id", "Name");
                        ViewBag.Ug = (from o in db.Institutions where o.Id == _institutionId && o.Status == true select o.ManagerCode).FirstOrDefault();
                    }
                    return PartialView("Incorporacao/_25partialEmpenhoRestosAPagar");

                case EnumMovimentType.IncorpAnimaisPesquisaSememPeixe:
                    return PartialView("Incorporacao/_26partialAnimaisPesquisaSememPeixe");

                case EnumMovimentType.IncorpComodatoDeTerceirosRecebidos:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == _institutionId && (i.BudgetUnitId == _budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == _managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList(), "Id", "Name");
                    }
                    return PartialView("Incorporacao/_27partialComodatoTerceiroRecebido");

                case EnumMovimentType.IncorpComodatoConcedidoBensMoveis:
                    return PartialView("Incorporacao/_28partialComodatoConcedidoBensMoveis");

                case EnumMovimentType.IncorpConfiscoBensMoveis:
                    return PartialView("Incorporacao/_29partialConfiscoBensMoveis");

                case EnumMovimentType.IncorpDoacaoConsolidacao:
                    return PartialView("Incorporacao/_30partialDoacaoConsolidacao");

                case EnumMovimentType.IncorpDoacaoIntraNoEstado:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        CarregaHierarquiaDestinoNaoImplantados(mesmoOrgao: false);
                    }
                    return PartialView("Incorporacao/_31partialDoacaoIntraNoEstado");

                case EnumMovimentType.IncorpDoacaoMunicipio:
                    return PartialView("Incorporacao/_32partialDoacaoMunicipio");

                case EnumMovimentType.IncorpDoacaoOutrosEstados:
                    return PartialView("Incorporacao/_33partialDoacaoOutrosEstados");

                case EnumMovimentType.IncorpDoacaoUniao:
                    return PartialView("Incorporacao/_34partialDoacaoUniao");

                case EnumMovimentType.IncorpVegetal:
                    return PartialView("Incorporacao/_35partialVegetal");

                case EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        ViewBag.OldInitial = new SelectList(db.Initials.Where(i => i.InstitutionId == _institutionId && (i.BudgetUnitId == _budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == _managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name).ToList(), "Id", "Name");
                    }
                    return PartialView("Incorporacao/_36partialMudancaCategoriaRevalorizacao");

                case EnumMovimentType.IncorpNascimentoDeAnimais:
                    return PartialView("Incorporacao/_37partialNascimentoAnimais");

                case EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        CarregaHierarquiaDestinoNaoImplantados(mesmoOrgao: false);
                    }
                    return PartialView("Incorporacao/_38partialRecebimentoInservivelUGEDoacao");

                case EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        CarregaHierarquiaDestinoNaoImplantados(mesmoOrgao: true);
                    }
                    return PartialView("Incorporacao/_39partialRecebimentoInservivelUGETransfer");

                case EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado:
                    return PartialView("Incorporacao/_40partialTransferMesmoOrgaoPatrimoniado");

                case EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado:
                    getHierarquiaPerfil();
                    using (db = new SAMContext())
                    {
                        CarregaHierarquiaDestinoNaoImplantados(mesmoOrgao: true);
                    }
                    return PartialView("Incorporacao/_41partialTransferOutroOrgaoPatrimoniado");

                default:
                    return PartialView("");
            }
        }

        //context em outros métodos
        private void CarregaHierarquiaDestino(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null)
        {
            hierarquia = new Hierarquia();
            if (managerCode == null)
            {
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
            }


            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if (modelManagerUnitId != 0)
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else 
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            if (modelAdministrativeUnitId != 0)
                ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
            else
                ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            if (modelSectionId != 0)
                ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
            else
                ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
        }

        private void CarregaHierarquiaDestinoNaoImplantados(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, bool mesmoOrgao = true)
        {
            hierarquia = new Hierarquia();
            if(mesmoOrgao)
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestaoNaoImplantados(RecuperaGestao()), "Id", "Description", modelInstitutionId);
            else
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosOutraGestaoNaoImplantados(RecuperaGestao()), "Id", "Description", modelInstitutionId);

            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
        }

        //context em outros métodos
        private string RecuperaGestao()
        {
            return (from i in db.Institutions
                          where i.Id == _institutionId
                          select i.ManagerCode).FirstOrDefault();
        }

        #region Busca Documento
        public JsonResult PesquisaPorDocumento(string numeroDocumento, int codigoUGE, string chapa, int tipoTransferencia, int AssetId)
        {
            MensagemModel mensagem;
            List<MensagemModel> mensagens;

            using (db = new SAMContext())
            {
                db.Configuration.LazyLoadingEnabled = true;

                int _asset = (from a in db.Assets where a.Id == AssetId select a.Id).FirstOrDefault();
                if (_asset == 0)
                {
                    mensagem = new MensagemModel();
                    mensagem.Mensagem = "Numero Documento não foi encontrado, favor verificar!";
                    mensagens = new List<MensagemModel>();
                    mensagens.Add(mensagem);
                    return Json(mensagens, JsonRequestBehavior.AllowGet);
                }

                //GROUPBY Removido 
                //var lstAssetMovimentoAgrupado = (from  am in db.AssetMovements
                //                                where am.AssetId == _asset 
                //                                && am.MovementTypeId == tipoTransferencia
                //                                && am.Status == true
                //                                group am by new { am.AssetId} into g
                //                                select new AssetMovementsViewModelAgrupado
                //                                {
                //                                    Ultimoregistro = g.Max(p => p.Id),
                //                                    AssetId = g.Key.AssetId
                //                                });

                //var lstAssetMovements = (from am in db.AssetMovements
                //                         join r in lstAssetMovimentoAgrupado
                //                         on am.Id equals r.Ultimoregistro
                //                         select am);

                AssetEAssetMovimentViewModel _assetMovimentoViewModel;

                if (tipoTransferencia == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao ||
                    tipoTransferencia == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia)
                {
                    _assetMovimentoViewModel = (from am in db.AssetMovements
                                                join a in db.Assets
                                                on am.AssetId equals a.Id
                                                where am.Status == false &&
                                                      am.FlagEstorno != true &&
                                                      am.AssetTransferenciaId == null &&
                                                      am.MovementTypeId == tipoTransferencia &&
                                                      a.Id == _asset
                                                select new AssetEAssetMovimentViewModel
                                                {
                                                    Asset = a,
                                                    AssetMoviment = am
                                                }).AsNoTracking().FirstOrDefault();
                }
                else {
                    _assetMovimentoViewModel = (from am in db.AssetMovements
                                                join a in db.Assets
                                                on am.AssetId equals a.Id
                                                where am.Status == true &&
                                                      am.MovementTypeId == tipoTransferencia &&
                                                      a.Id == _asset
                                                select new AssetEAssetMovimentViewModel
                                                {
                                                    Asset = a,
                                                    AssetMoviment = am
                                                }).AsNoTracking().FirstOrDefault();
                }

                if (_assetMovimentoViewModel.AssetMoviment.MovementTypeId != tipoTransferencia)
                {
                    mensagem = new MensagemModel();
                    mensagem.Mensagem = "Não foi realizada uma movimentação de " + ((EnumMovimentType)tipoTransferencia).ToString() + " com este Bem Patrimonial, favor verificar!";
                    mensagens = new List<MensagemModel>();
                    mensagens.Add(mensagem);
                    return Json(mensagens, JsonRequestBehavior.AllowGet);
                }
                if (_assetMovimentoViewModel.AssetMoviment.SourceDestiny_ManagerUnitId != codigoUGE)
                {
                    mensagem = new MensagemModel();
                    mensagem.Mensagem = "A UGE do usuário logado não é a mesma da UGE que o Bem Patrimonial foi transferido, favor verificar!";
                    mensagens = new List<MensagemModel>();
                    mensagens.Add(mensagem);
                    return Json(mensagens, JsonRequestBehavior.AllowGet);
                }

                if (MovimentacaoComPendenciasDeNLContabiliza(_assetMovimentoViewModel.AssetMoviment.ManagerUnitId, _assetMovimentoViewModel.AssetMoviment.AuditoriaIntegracaoId))
                {
                    mensagem = new MensagemModel();
                    mensagem.Mensagem = "BP não pode ser aceito, pois está com Pendências no SIAFEM a serem resolvidas. Por favor, entre em contato com a UGE de origem.";
                    mensagens = new List<MensagemModel>();
                    mensagens.Add(mensagem);
                    return Json(mensagens, JsonRequestBehavior.AllowGet);
                }

                AssetViewModel _AssetView = new AssetViewModel();

                // Dados do Movimento Patrimonio
                _AssetView.StateConservationId = _assetMovimentoViewModel.AssetMoviment.StateConservationId;
                _AssetView.AuxiliaryAccountId = _assetMovimentoViewModel.AssetMoviment.AuxiliaryAccountId;
                _AssetView.OrigemManagerUnit = (from m in db.ManagerUnits where m.Id == _assetMovimentoViewModel.AssetMoviment.ManagerUnitId select m.Code + " - " + m.Description).FirstOrDefault();


                // Id do item transferido
                _AssetView.AssetsIdTransferencia = _asset;

                // Dados do Patrimonio
                _AssetView.NumberDoc = _assetMovimentoViewModel.AssetMoviment != null && !string.IsNullOrEmpty(_assetMovimentoViewModel.AssetMoviment.NumberDoc) && !string.IsNullOrWhiteSpace(_assetMovimentoViewModel.AssetMoviment.NumberDoc)
                                       ? _assetMovimentoViewModel.AssetMoviment.NumberDoc :
                                         _assetMovimentoViewModel.Asset.NumberDoc;
                _AssetView.NumeroDocumentoAtual = _assetMovimentoViewModel.AssetMoviment != null && !string.IsNullOrEmpty(_assetMovimentoViewModel.AssetMoviment.NumberDoc) && !string.IsNullOrWhiteSpace(_assetMovimentoViewModel.AssetMoviment.NumberDoc)
                                       ? _assetMovimentoViewModel.AssetMoviment.NumberDoc :
                                         _assetMovimentoViewModel.Asset.NumberDoc;
                _AssetView.NumberDocModel = _AssetView.NumeroDocumentoAtual;
                _AssetView.MaterialItemCode = _assetMovimentoViewModel.Asset.MaterialItemCode;
                _AssetView.MaterialItemDescription = _assetMovimentoViewModel.Asset.MaterialItemDescription;
                _AssetView.MaterialGroupCode = _assetMovimentoViewModel.Asset.MaterialGroupCode;
                _AssetView.MaterialGroupDescription = (from g in db.MaterialGroups where g.Code == _assetMovimentoViewModel.Asset.MaterialGroupCode select g.Description).FirstOrDefault();
                _AssetView.ShortDescriptionItemId = _assetMovimentoViewModel.Asset.ShortDescriptionItemId;
                _AssetView.ShortDescription = _assetMovimentoViewModel.Asset.RelatedShortDescriptionItem.Description;
                _AssetView.LifeCycle = _assetMovimentoViewModel.Asset.LifeCycle;
                _AssetView.ResidualValue = _assetMovimentoViewModel.Asset.ResidualValue;
                _AssetView.RateDepreciationMonthly = _assetMovimentoViewModel.Asset.RateDepreciationMonthly;
                _AssetView.AceleratedDepreciation = _assetMovimentoViewModel.Asset.AceleratedDepreciation;
                _AssetView.OldNumberIdentification = _assetMovimentoViewModel.Asset.NumberIdentification;
                _AssetView.DiferenciacaoChapaAntiga = _assetMovimentoViewModel.Asset.DiferenciacaoChapa;
                _AssetView.OldInitial = _assetMovimentoViewModel.Asset.InitialId;
                _AssetView.OldInitialDescription = _assetMovimentoViewModel.Asset.RelatedInitial.Name;
                _AssetView.ValueAcquisition = _assetMovimentoViewModel.Asset.ValueAcquisition;
                _AssetView.ValueUpdate = _assetMovimentoViewModel.Asset.ValueUpdate;
                _AssetView.AcquisitionDate = _assetMovimentoViewModel.Asset.AcquisitionDate;
                _AssetView.Empenho = _assetMovimentoViewModel.Asset.Empenho;
                _AssetView.checkFlagAcervo = _assetMovimentoViewModel.Asset.flagAcervo == null ? false : _assetMovimentoViewModel.Asset.flagAcervo.Value;
                _AssetView.checkFlagTerceiro = _assetMovimentoViewModel.Asset.flagTerceiro == null ? false : _assetMovimentoViewModel.Asset.flagTerceiro.Value;
                _AssetView.checkFlagDecretoSefaz = _assetMovimentoViewModel.Asset.flagDecreto == null ? false : _assetMovimentoViewModel.Asset.flagDecreto.Value;
                _AssetView.checkflagAnimalAServico = _assetMovimentoViewModel.Asset.flagAnimalNaoServico == null ? true : !_assetMovimentoViewModel.Asset.flagAnimalNaoServico.Value;
                _AssetView.NumeroDocumentoAtual = _assetMovimentoViewModel.AssetMoviment.NumberDoc;
                _AssetView.TipoBP = _assetMovimentoViewModel.Asset.flagAcervo == true ? 1 : (_assetMovimentoViewModel.Asset.flagTerceiro == true ? 2 : 0);

                // Dados Complementares
                _AssetView.SerialNumber = _assetMovimentoViewModel.Asset.SerialNumber;
                _AssetView.ManufactureDate = _assetMovimentoViewModel.Asset.ManufactureDate;
                _AssetView.DateGuarantee = _assetMovimentoViewModel.Asset.DateGuarantee;
                _AssetView.ChassiNumber = _assetMovimentoViewModel.Asset.ChassiNumber;
                _AssetView.Brand = _assetMovimentoViewModel.Asset.Brand;
                _AssetView.Model = _assetMovimentoViewModel.Asset.Model;
                _AssetView.NumberPlate = _assetMovimentoViewModel.Asset.NumberPlate;
                _AssetView.AdditionalDescription = _assetMovimentoViewModel.Asset.AdditionalDescription;

                if (!_AssetView.SerialNumber.IsNull() || !_AssetView.ManufactureDate.IsNull() || !_AssetView.DateGuarantee.IsNull()
                    || !_AssetView.ChassiNumber.IsNull() || !_AssetView.Brand.IsNull() || !_AssetView.Model.IsNull()
                    || !_AssetView.NumberPlate.IsNull() || !_AssetView.AdditionalDescription.IsNull())
                    _AssetView.checkComplemento = true;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = serializer.Serialize(_AssetView);
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GridDocumento(int tipoTransferencia, string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                IQueryable<GridDocumentoViewModel> lstRetorno = null;


                getHierarquiaPerfil();
                string gestao = string.Empty;

                using (db = new SAMContext())
                {
                    switch (tipoTransferencia) {
                        case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                            lstRetorno = (from m in db.AssetMovements
                                          join a in db.Assets
                                          on m.AssetId equals a.Id
                                          where m.SourceDestiny_ManagerUnitId == _managerUnitId
                                          && m.InstitutionId == _institutionId
                                          && m.MovementTypeId == tipoTransferencia
                                          && m.Status == true
                                          select new GridDocumentoViewModel
                                          {
                                              IdDoBP = a.Id,
                                              NumeroDocumento = m.NumberDoc == null ? a.NumberDoc : m.NumberDoc,
                                              Chapa = a.ChapaCompleta,
                                              CodigoUGE = m.RelatedManagerUnit.Code,
                                              CodigoItemMaterial = a.MaterialItemCode,
                                              DescricaoItemMaterial = a.MaterialItemDescription
                                          }).AsNoTracking();
                            break;
                        case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                            gestao = (from i in db.Institutions
                                      where i.Id == _institutionId
                                      select i.ManagerCode).FirstOrDefault();

                            lstRetorno = (from m in db.AssetMovements
                                          join a in db.Assets
                                          on m.AssetId equals a.Id
                                          join i in db.Institutions
                                          on m.InstitutionId equals i.Id
                                          where m.SourceDestiny_ManagerUnitId == _managerUnitId
                                          && m.InstitutionId != _institutionId
                                          && m.MovementTypeId == tipoTransferencia
                                          && m.Status == true
                                          && i.ManagerCode == gestao
                                          select new GridDocumentoViewModel
                                          {
                                              IdDoBP = a.Id,
                                              NumeroDocumento = m.NumberDoc == null ? a.NumberDoc : m.NumberDoc,
                                              Chapa = a.NumberIdentification,
                                              CodigoUGE = m.RelatedManagerUnit.Code,
                                              CodigoItemMaterial = a.MaterialItemCode,
                                              DescricaoItemMaterial = a.MaterialItemDescription
                                          }).AsNoTracking();
                            break;
                        case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                            gestao = (from i in db.Institutions
                                      where i.Id == _institutionId
                                      select i.ManagerCode).FirstOrDefault();

                            lstRetorno = (from m in db.AssetMovements
                                          join a in db.Assets
                                          on m.AssetId equals a.Id
                                          join i in db.Institutions
                                          on m.InstitutionId equals i.Id
                                          where m.SourceDestiny_ManagerUnitId == _managerUnitId
                                          && m.MovementTypeId == tipoTransferencia
                                          && m.Status == true
                                          && m.InstitutionId != _institutionId
                                          && i.ManagerCode != gestao
                                          select new GridDocumentoViewModel
                                          {
                                              IdDoBP = a.Id,
                                              NumeroDocumento = m.NumberDoc == null ? a.NumberDoc : m.NumberDoc,
                                              Chapa = a.ChapaCompleta,
                                              CodigoUGE = m.RelatedManagerUnit.Code,
                                              CodigoItemMaterial = a.MaterialItemCode,
                                              DescricaoItemMaterial = a.MaterialItemDescription
                                          }).AsNoTracking();
                            break;
                        case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                            gestao = (from i in db.Institutions
                                      where i.Id == _institutionId
                                      select i.ManagerCode).FirstOrDefault();

                            lstRetorno = (from m in db.AssetMovements
                                          join a in db.Assets
                                          on m.AssetId equals a.Id
                                          join i in db.Institutions
                                          on m.InstitutionId equals i.Id
                                          where m.SourceDestiny_ManagerUnitId == _managerUnitId
                                          && m.ManagerUnitId != _managerUnitId
                                          && m.MovementTypeId == tipoTransferencia
                                          && m.Status == false
                                          && i.ManagerCode == gestao
                                          && m.FlagEstorno != true
                                          && m.AssetTransferenciaId == null
                                          select new GridDocumentoViewModel
                                          {
                                              IdDoBP = a.Id,
                                              NumeroDocumento = m.NumberDoc == null ? a.NumberDoc : m.NumberDoc,
                                              Chapa = a.ChapaCompleta,
                                              CodigoUGE = m.RelatedManagerUnit.Code,
                                              CodigoItemMaterial = a.MaterialItemCode,
                                              DescricaoItemMaterial = a.MaterialItemDescription
                                          }).AsNoTracking();
                            break;
                        case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                            gestao = (from i in db.Institutions
                                      where i.Id == _institutionId
                                      select i.ManagerCode).FirstOrDefault();

                            lstRetorno = (from m in db.AssetMovements
                                          join a in db.Assets
                                          on m.AssetId equals a.Id
                                          join i in db.Institutions
                                          on m.InstitutionId equals i.Id
                                          where m.SourceDestiny_ManagerUnitId == _managerUnitId
                                          && m.MovementTypeId == tipoTransferencia
                                          && m.Status == false
                                          && m.InstitutionId != _institutionId
                                          && i.ManagerCode != gestao
                                          && m.FlagEstorno != true
                                          && m.AssetTransferenciaId == null
                                          select new GridDocumentoViewModel
                                          {
                                              IdDoBP = a.Id,
                                              NumeroDocumento = m.NumberDoc == null ? a.NumberDoc : m.NumberDoc,
                                              Chapa = a.ChapaCompleta,
                                              CodigoUGE = m.RelatedManagerUnit.Code,
                                              CodigoItemMaterial = a.MaterialItemCode,
                                              DescricaoItemMaterial = a.MaterialItemDescription
                                          }).AsNoTracking();
                            break;
                    }

                    ViewBag.CurrentFilter = searchString;
                    ViewBag.Uge = _managerUnitId;

                    lstRetorno = SearchByFilterDocumento(searchString, lstRetorno);

                    int pageSize = 6;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno.OrderBy(s => s.IdDoBP).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<GridDocumentoViewModel>(result, pageNumber, pageSize, lstRetorno.Count());

                    return PartialView("_partialBuscarDocumento", retorno);
                }
                
            }
            catch (Exception ex)
            {
                return PartialView("");
            }
        }
        //context em outros métodos
        private IQueryable<GridDocumentoViewModel> SearchByFilterDocumento(string searchString, IQueryable<GridDocumentoViewModel> result)
        {
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrWhiteSpace(searchString))
                result = result.Where(s => s.NumeroDocumento.Contains(searchString) ||
                                           s.Chapa.Contains(searchString) ||
                                           s.CodigoUGE.Contains(searchString) ||
                                           s.CodigoItemMaterial.ToString().Contains(searchString) ||
                                           s.DescricaoItemMaterial.Contains(searchString));
            return result;
        }


        private bool MovimentacaoComPendenciasDeNLContabiliza(int IdDaUGEDeOrigem, int? AuditoriaIntegracaoId) {
            if (AuditoriaIntegracaoId == null)
            {

                var UGEIntegrada = (from i in db.Institutions
                                    join b in db.BudgetUnits on i.Id equals b.InstitutionId
                                    join m in db.ManagerUnits on b.Id equals m.BudgetUnitId
                                    where m.Id == IdDaUGEDeOrigem
                                    select i.flagIntegracaoSiafem || m.FlagIntegracaoSiafem).FirstOrDefault();

                if (!UGEIntegrada)
                {
                    return false;
                }
                else {
                    return true;
                }
            }

            var possuiPendencia = (from n in db.NotaLancamentoPendenteSIAFEMs
                                   where n.AuditoriaIntegracaoId == AuditoriaIntegracaoId
                                   && n.StatusPendencia == 1
                                   select n.Id).Count() > 0;
            return possuiPendencia;

        }
        #endregion

        #region Busca Terceiro
        public JsonResult PesquisaPorTerceiro(int OutSourcedPesquisa)
        {
            OutSourcedViewModel Terceiro = null;
            getHierarquiaPerfil();

            using (db = new SAMContext())
            {
                db.Configuration.LazyLoadingEnabled = false;

                Terceiro = (from o in db.OutSourceds
                            where o.Id == OutSourcedPesquisa 
                               && o.Status == true
                               && o.InstitutionId == _institutionId
                            select new OutSourcedViewModel
                            {
                                Id = o.Id,
                                Name = o.Name,
                                CPFCNPJ = o.CPFCNPJ
                            }).FirstOrDefault();
            }

            if (Terceiro == null)
            {
                return Json(new { Erro = "Terceiro não foi encontrado, favor verificar!" }, JsonRequestBehavior.AllowGet);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var retorno = serializer.Serialize(Terceiro);
            return Json(retorno, JsonRequestBehavior.AllowGet);            
        }
        public ActionResult GridTerceiro(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                getHierarquiaPerfil();

                IQueryable<OutSourcedViewModel> lstRetorno;

                using (db = new SAMContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    if (string.IsNullOrEmpty(searchString) || string.IsNullOrWhiteSpace(searchString))
                    {
                        lstRetorno = (from a in db.OutSourceds
                                      where a.InstitutionId == _institutionId
                                      && a.Status == true
                                      select new OutSourcedViewModel
                                      {
                                          Id = a.Id,
                                          CPFCNPJ = a.CPFCNPJ,
                                          Name = a.Name
                                      }).AsNoTracking();
                    }
                    else
                    {
                        lstRetorno = (from a in db.OutSourceds
                                      where a.InstitutionId == _institutionId
                                      && a.Status == true
                                      && (a.CPFCNPJ.Contains(searchString) || a.Name.Contains(searchString))
                                      select new OutSourcedViewModel
                                      {
                                          Id = a.Id,
                                          CPFCNPJ = a.CPFCNPJ,
                                          Name = a.Name
                                      }).AsNoTracking();
                    }


                    ViewBag.CurrentFilter = searchString;

                    int pageSize = 6;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno.OrderBy(s => s.Name.Trim()).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<OutSourcedViewModel>(result, pageNumber, pageSize, lstRetorno.Count());

                    return PartialView("_partialBuscarTerceiro", retorno);
                }
            }
            catch (Exception ex)
            {
                return PartialView("");
            }
        }
        #endregion

        #region Busca Fornecedor
        public JsonResult PesquisaPorFornecedor(string FornecedorPesquisa)
        {
            using (db = new SAMContext())
            {
                db.Configuration.LazyLoadingEnabled = false;

                Supplier Fornecedor = (from a in db.Suppliers where a.CPFCNPJ.Trim() == FornecedorPesquisa.Trim() && a.Status == true select a).FirstOrDefault();
                if (Fornecedor == null)
                {
                    MensagemModel mensagem = new MensagemModel();
                    List<MensagemModel> mensagens = new List<MensagemModel>();
                    mensagem.Id = 0;
                    mensagem.Mensagem = "Fornecedor não foi encontrado, favor verificar!";
                    mensagens.Add(mensagem);
                    return Json(mensagens, JsonRequestBehavior.AllowGet);
                }

                AssetViewModel _AssetView = new AssetViewModel();
                _AssetView.SupplierId = Fornecedor.Id;
                //_AssetView.SupplierName = Fornecedor.Name;
                //_AssetView.SupplierCPFCNPJ = Fornecedor.CPFCNPJ;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = serializer.Serialize(_AssetView);
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GridFornecedor(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                IQueryable<Supplier> lstRetorno;

                using (db = new SAMContext())
                {
                    lstRetorno = (from a in db.Suppliers
                                  where a.Status == true
                                  select a).AsNoTracking();


                    ViewBag.CurrentFilter = searchString;

                    lstRetorno = SearchByFilterFornecedor(searchString, lstRetorno);

                    int pageSize = 6;
                    int pageNumber = (page ?? 1);

                    var result = lstRetorno.OrderBy(s => s.Name.Trim()).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    var retorno = new StaticPagedList<Supplier>(result, pageNumber, pageSize, lstRetorno.Count());

                    return PartialView("_partialBuscarFornecedor", retorno);
                }
            }
            catch (Exception ex)
            {
                return PartialView("");
            }
        }
        private IQueryable<Supplier> SearchByFilterFornecedor(string searchString, IQueryable<Supplier> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.CPFCNPJ.Contains(searchString) ||
                                           s.Name.Contains(searchString));
            return result;
        }
        #endregion

        #region Empenho

        public JsonResult GetEmpenhoMaterial(int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            MensagemModel mensagem = new MensagemModel();
            List<MensagemModel> mensagens = new List<MensagemModel>();
            string AnoMesRef = null;

            db = new SAMContext();
            var managerUnit = (from m in db.ManagerUnits where m.Id == codigoUGE select m).AsNoTracking().FirstOrDefault();

            if (managerUnit != null || managerUnit.Code != "")
            {
                codigoUGE = int.Parse(managerUnit.Code);
                AnoMesRef = managerUnit.ManagmentUnit_YearMonthReference.ToString().Substring(0, 4);
            }
            else
            {
                mensagem.Mensagem = "Não foi possivel recuperar o Codigo da UGE, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            try
            {
                AssetViewModelEmpenho _asset = GetEmpenhoSIAFISICO(AnoMesRef, codigoUGE, codigoGestao, numeroEmpenho.Trim());

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = serializer.Serialize(_asset);
                var _retorno = Json(retorno, JsonRequestBehavior.AllowGet);

                return ValidaRetornoSIAFISICO(_retorno);
            }
            catch (Exception ex)
            {
                mensagem = new MensagemModel();

                mensagens = new List<MensagemModel>();
                if (ex.InnerException != null)
                    mensagem.Mensagem = string.Format("{0} - {1} - {2}", numeroEmpenho, ex.Message, ex.InnerException.Message);
                else
                    mensagem.Mensagem = string.Format("{0} - {1}", numeroEmpenho, ex.Message);

                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult GridMaterialEmpenho(AssetViewModelEmpenho empenho)
        {
            return PartialView("_partialMaterialEmpenho", empenho);
        }

        [HttpGet]
        public JsonResult GetItemMaterial(string materialItemCode, string Empenho, string CgcCpf, string DescricaoFornecedor)
        {
            

            MaterialItemsController material = new MaterialItemsController();
            AssetViewModel _Asset = new AssetViewModel();

            float codigo = Convert.ToInt64(materialItemCode);

            db = new SAMContext();
            // ALTERACAO - CONSULTAR NO BANCO TABELA ITEM_SIAFISICO
            ItemSiafisico ItemSiafisico = (from m in db.ItemSiafisicos where m.Cod_Item_Mat == codigo select m).FirstOrDefault();

            MaterialItem materialItem = new MaterialItem();

            if (ItemSiafisico.IsNotNull())
            {
                materialItem = new MaterialItemViewModel
                {
                    MaterialGroupCode = (int)ItemSiafisico.Cod_Grupo,
                    MaterialGroupDescription = ItemSiafisico.Nome_Grupo,
                    MaterialItemCode = ItemSiafisico.Cod_Item_Mat.ToString(),
                    Code = (int)ItemSiafisico.Cod_Item_Mat,
                    Description = ItemSiafisico.Nome_Item_Mat,
                    Material = ItemSiafisico.Nome_Material,
                    Natureza1 = "4490"
                };
            }
            else
            {
                materialItem = material.GetMaterialItemFromSIAFISICO(materialItemCode.Trim());
            }

            MensagemModel mensagem = new MensagemModel();
            mensagem.Id = 0;
            mensagem.Mensagem = "";
            List<MensagemModel> mensagens = new List<MensagemModel>();

            if (TempData["Mensagens"].IsNotNull())
            {
                mensagem.Mensagem = TempData["Mensagens"].ToString();
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (materialItem.Description.IsNullOrEmpty())
            {
                mensagem.Mensagem = "Item Material não encontrado, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            if (((MaterialItemViewModel)materialItem).Description == null)
            {
                mensagem.Mensagem = "A Descrição do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (((MaterialItemViewModel)materialItem).MaterialItemCode == null)
            {
                mensagem.Mensagem = "O código do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (!((MaterialItemViewModel)materialItem).Natureza1.Substring(0, 4).Contains("4490") && !((MaterialItemViewModel)materialItem).Natureza2.Substring(0, 4).Contains("4490") && !((MaterialItemViewModel)materialItem).Natureza3.Substring(0, 4).Contains("4490"))
            {
                mensagem.Mensagem = "O Item Material informado não é um tipo de Despesa de Bem Permanente, favor verificar!";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            MaterialGroup _materialgroup = (from a in db.MaterialGroups where a.Code == ((MaterialItemViewModel)materialItem).MaterialGroupCode select a).FirstOrDefault();
            if (_materialgroup == null)
            {
                mensagem.Mensagem = "O Grupo do Material selecionado não está cadastrado no sistema, favor entrar em contato com a Administração.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (((MaterialItemViewModel)materialItem).Material == null)
            {
                mensagem.Mensagem = "A Descrição Resumida do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            _Asset.Empenho = Empenho;
            _Asset.SupplierCPFCNPJ = CgcCpf;
            _Asset.SupplierName = DescricaoFornecedor;
            _Asset.MaterialItemCode = materialItem.Code;
            _Asset.MaterialItemDescription = materialItem.Description;
            _Asset.MaterialGroupCode = _materialgroup.Code;
            _Asset.MaterialGroupDescription = _materialgroup.Description;
            _Asset.LifeCycle = _materialgroup.LifeCycle;
            _Asset.ResidualValue = _materialgroup.ResidualValue;
            _Asset.RateDepreciationMonthly = _materialgroup.RateDepreciationMonthly;
            _Asset.ShortDescription = ((MaterialItemViewModel)materialItem).Material;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var retorno = serializer.Serialize(_Asset);
            var _retorno = Json(retorno, JsonRequestBehavior.AllowGet);

            return _retorno;
        }

        [HttpGet]
        public JsonResult GetEmpenho(int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            
            string AnoMesRef = null;
            db = new SAMContext();
            var managerUnit = (from m in db.ManagerUnits where m.Id == codigoUGE select m).AsNoTracking().FirstOrDefault();

            if (managerUnit != null || managerUnit.Code != "")
            {
                codigoUGE = int.Parse(managerUnit.Code);
                AnoMesRef = managerUnit.ManagmentUnit_YearMonthReference.ToString().Substring(0, 4);
            }
            else
            {
                MensagemModel mensagem = new MensagemModel();
                List<MensagemModel> mensagens = new List<MensagemModel>();
                mensagem.Mensagem = "Não foi possivel recuperar o Codigo da UGE, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            AssetViewModelEmpenho _asset = GetEmpenhoFromSIAFISICO(AnoMesRef, codigoUGE, codigoGestao, numeroEmpenho.Trim());
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var retorno = serializer.Serialize(_asset);

            var _retorno = Json(retorno, JsonRequestBehavior.AllowGet);

            return ValidaRetornoSIAFISICO(_retorno);
        }

        [HttpGet]
        public JsonResult GetEmpenhoRestosAPagar(int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            MensagemModel mensagem = new MensagemModel();
            List<MensagemModel> mensagens = new List<MensagemModel>();

            string AnoMesRef = null;
            db = new SAMContext();
            var managerUnit = (from m in db.ManagerUnits where m.Id == codigoUGE select m).AsNoTracking().FirstOrDefault();

            if (managerUnit != null || managerUnit.Code != "")
            {
                codigoUGE = int.Parse(managerUnit.Code);
                AnoMesRef = managerUnit.ManagmentUnit_YearMonthReference.ToString();
            }
            else
            {
                mensagem.Mensagem = "Não foi possivel recuperar o Codigo da UGE, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            AssetViewModelEmpenhoRestosAPagar _asset = GetEmpenhoRestosAPagarFromSIAFISICO(AnoMesRef, codigoUGE, codigoGestao, numeroEmpenho.Trim());

            if (_asset.Empenho == null)
            {
                mensagem.Mensagem = "Empenho não encontrado, favor informe o empenho novamente.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var retorno = serializer.Serialize(_asset);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidaRetornoSIAFISICO(JsonResult _retorno)
        {
            var _objDeserializado = JsonConvert.DeserializeObject<AssetViewModelEmpenho>(_retorno.Data.ToString());

            MensagemModel mensagem = new MensagemModel();

            List<MensagemModel> mensagens = new List<MensagemModel>();

            if (TempData["Mensagens"].IsNotNull())
            {
                mensagem.Mensagem = TempData["Mensagens"].ToString();
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_objDeserializado.Empenho == null)
            {
                mensagem.Mensagem = "O Empenho está nulo, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_objDeserializado.CgcCpf == null)
            {
                mensagem.Mensagem = "Descrição do Fornecedor está nulo, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_objDeserializado.Assets == null)
            {
                mensagem.Mensagem = "Não existe nenhum item material para este empenho, favor verificar.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_objDeserializado.Assets.Where(a => a.MaterialItemCode == 0).Any())
            {
                mensagem.Mensagem = "Não existe nenhum item material para este empenho, favor verificar.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var objSerialize = JsonConvert.SerializeObject(_objDeserializado);

                return Json(objSerialize, JsonRequestBehavior.AllowGet);
            }
        }
        [ValidateAntiForgeryToken]
        public AssetViewModelEmpenho GetEmpenhoFromSIAFISICO(string AnoMesRef, int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            try
            {
                var rowAsset = RecuperarEmpenhoDoSiafisico(AnoMesRef, codigoUGE, codigoGestao, numeroEmpenho);
                return rowAsset;
            }
            catch (Exception excErroExecucao)
            {
                TempData["Mensagens"] = excErroExecucao.Message;
                return new AssetViewModelEmpenho();
            }
        }
        private AssetViewModelEmpenho GetEmpenhoSIAFISICO(string AnoMesRef, int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            try
            {
                var rowAsset = RecuperarEmpenhoDoSiafisico(AnoMesRef, codigoUGE, codigoGestao, numeroEmpenho);
                return rowAsset;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ValidateAntiForgeryToken]
        public AssetViewModelEmpenhoRestosAPagar GetEmpenhoRestosAPagarFromSIAFISICO(string AnoMesRef, int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            try
            {
                return RecuperarEmpenhoRestosAPagarDoSiafisico(AnoMesRef, codigoUGE, codigoGestao, numeroEmpenho);
            }
            catch (Exception excErroExecucao)
            {
                TempData["Mensagens"] = excErroExecucao.Message;
                return new AssetViewModelEmpenhoRestosAPagar();
            }
        }

        public AssetViewModelEmpenho RecuperarEmpenhoDoSiafisico(string AnoMesRef, int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            AssetViewModelEmpenho rowAsset = null;
            ProcessadorServicoSIAF svcSIAFISICO = null;
            string msgEstimulo = null;
            string ugeConsulta = null;
            string retornoMsgEstimulo = null;
            string patternXmlConsulta = null;
            string loginUsuarioSIAFISICO = null;
            string senhaUsuarioSIAFISICO = null;
            string patternXmlConsultaMaterial = null;

            msgEstimulo = GeradorEstimuloSIAF.SiafemDocConsultaEmpenhos(codigoUGE, codigoGestao, numeroEmpenho);
            ugeConsulta = "380236";
            patternXmlConsulta = "/MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDocConsultaEmpenhos/documento/";
            patternXmlConsultaMaterial = "/MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDocConsultaEmpenhos/documento/Repete/tabela";

            try
            {
                svcSIAFISICO = new ProcessadorServicoSIAF();
                loginUsuarioSIAFISICO = ConfiguracoesSIAF.userNameConsulta;
                senhaUsuarioSIAFISICO = ConfiguracoesSIAF.passConsulta;
                retornoMsgEstimulo = svcSIAFISICO.ConsumirWS(loginUsuarioSIAFISICO, senhaUsuarioSIAFISICO, AnoMesRef, ugeConsulta, msgEstimulo, true, true);

                if (svcSIAFISICO.ErroProcessamentoWs)
                {
                    throw new Exception(svcSIAFISICO.ErroRetornoWs);
                }
                else
                {
                    if (retornoMsgEstimulo == "Erro ao processar solicitação.\nAcionar administrador do sistema!")
                    {
                        throw new Exception("Ocorreu uma exceção no retorno do serviço. Favor acionar o administrador do sistema!");
                    }

                    string lStrNumeroNe = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "NumeroNe"));
                    string lStrCgcCpf = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "CgcCpf"));

                    XmlNodeList ndlElementos = null;
                    ndlElementos = XmlUtil.lerNodeListXml(retornoMsgEstimulo, patternXmlConsultaMaterial);

                    List<Asset> lstAsset = new List<Asset>();
                    Asset _Asset;

                    XmlElement noDoXML;
                    string materialComoString;
                    int tamanhodescricaoMaterial;

                    foreach (var item in ndlElementos)
                    {
                        _Asset = new Asset();

                        noDoXML = (System.Xml.XmlElement)item;

                        materialComoString = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(noDoXML["material"].InnerText.Replace("-", ""));
                        if (materialComoString.Length > 0)
                            _Asset.MaterialItemCode = int.Parse(materialComoString);
                        else
                            _Asset.MaterialItemCode = 0;

                        tamanhodescricaoMaterial = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(noDoXML.ChildNodes.Item(8).InnerText).Length;
                        if (tamanhodescricaoMaterial == 0)
                            _Asset.MaterialItemDescription = "";
                        else {
                            string descricao = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(noDoXML["descricao"].InnerText);
                            _Asset.MaterialItemDescription = descricao.Substring(0, tamanhodescricaoMaterial >= 120 ? 120 : tamanhodescricaoMaterial);
                        }

                        lstAsset.Add(_Asset);
                    }

                    lStrCgcCpf = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(lStrCgcCpf);
                    lStrNumeroNe = GeradorEstimuloSIAF.ReverterTratarDescricaoXml(lStrNumeroNe);

                    rowAsset = new AssetViewModelEmpenho()
                    {
                        Empenho = lStrNumeroNe,
                        CgcCpf = lStrCgcCpf.BreakLine(0).Replace("-", "").Trim(),
                        DescricaoFornecedor = lStrCgcCpf.Replace(lStrCgcCpf.BreakLine(0).Trim(), "").Trim(),
                        Assets = lstAsset
                    };
                }
            }
            catch (Exception excErroExecucao)
            {
                throw excErroExecucao;
            }

            return rowAsset;
        }

        public AssetViewModelEmpenhoRestosAPagar RecuperarEmpenhoRestosAPagarDoSiafisico(string AnoMesRef, int codigoUGE, int codigoGestao, string numeroEmpenho)
        {
            string contaContabil = null;
            string msgEstimuloWs = string.Empty;
            string mesRef = string.Empty;
            string anoRef = string.Empty;
            string retornoMsgEstimuloWs = string.Empty;
            string[] opcoesConsultaEmpenho = null;
            IList<string> listaRetornoMsgEstimuloWs = null;

            ProcessadorServicoSIAF svcSIAFEM = null;

            anoRef = AnoMesRef.Substring(0, 4);
            mesRef = MesExtenso.Mes[int.Parse(AnoMesRef.Substring(4, 2))];

            int anoExercicio = Int32.Parse(anoRef);
            anoExercicio -= 1;

            if (Constante.CST_CONTA_CONTABIL_RESTOS_A_PAGAR.Keys.Cast<string>().Count(anoContaContabil => anoContaContabil == anoExercicio.ToString()) == 1)
                contaContabil = Constante.CST_CONTA_CONTABIL_RESTOS_A_PAGAR[anoExercicio.ToString()].ToString();
            else
                throw new Exception(String.Format("Conta Contábil para listagem de empenhos de Restos a Pagar, exercício fiscal de {0} não registrada no sistema. Informar Prodesp, por gentileza.", anoExercicio));

            //msgEstimuloWs = GeradorEstimuloSIAF.SiafemDocDetaContaGen(codigoUGE, codigoGestao, mesRef, contaContabil, Constante.CST_CONTA_CONTABIL_OPCAO); //RETORNA EMPENHOS COM SALDO
            listaRetornoMsgEstimuloWs = new List<string>();
            opcoesConsultaEmpenho = new string[2] {   Constante.CST_CONTA_CONTABIL_OPCAO  //RETORNA EMPENHOS COM SALDO
                                                    , Constante.CST_CONTA_CONTABIL_OPCAO_SALDO_ZERO //RETORNA EMPENHOS SEM SALDO
                                                  };

            //Consulta/Retorno WS SEFAZ
            svcSIAFEM = new ProcessadorServicoSIAF();
            svcSIAFEM.Configuracoes = new ConfiguracoesSIAF();

            foreach (var opcaoConsulta in opcoesConsultaEmpenho)
            {
                msgEstimuloWs = GeradorEstimuloSIAF.SiafemDocDetaContaGen(codigoUGE, codigoGestao, mesRef, contaContabil, opcaoConsulta);
                svcSIAFEM.ConsumirWS(ConfiguracoesSIAF.userNameConsulta, ConfiguracoesSIAF.passConsulta, anoRef, codigoUGE.ToString(), msgEstimuloWs, false, true);
                if (!svcSIAFEM.ErroProcessamentoWs)
                    //retornoMsgEstimuloWs = svcSIAFEM.RetornoWsSIAF;
                    listaRetornoMsgEstimuloWs.Add(svcSIAFEM.RetornoWsSIAF);
                else
                    throw new Exception(svcSIAFEM.ErroRetornoWs);
            }

            return MontaEmpenhoRestosAPagar(listaRetornoMsgEstimuloWs, numeroEmpenho);
        }

        //private AssetViewModelEmpenhoRestosAPagar MontaEmpenhoRestosAPagar(string retornoMsgEstimuloWs, string empenho)
        private AssetViewModelEmpenhoRestosAPagar MontaEmpenhoRestosAPagar(IList<string> listaRetornoMsgEstimuloWs, string empenho)
        {
            AssetViewModelEmpenhoRestosAPagar empenhoRestosAPagar = new AssetViewModelEmpenhoRestosAPagar();
            string patternGenericoPesquisaXml = null;
            string query = null;

            patternGenericoPesquisaXml = "/MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDetaconta/documento/Repete/Documento";

            try
            {
                XmlNodeList ndlElementos = null;
                IQueryable<XmlNode> qryConsulta = null;

                //ndlElementos = XmlUtil.lerNodeListXml(retornoMsgEstimuloWs, patternGenericoPesquisaXml);
                foreach (var retornoMsgEstimuloWs in listaRetornoMsgEstimuloWs)
                {
                    ndlElementos = XmlUtil.lerNodeListXml(retornoMsgEstimuloWs, patternGenericoPesquisaXml);

                    if (ndlElementos.Cast<XmlNode>().HasElements())
                    {
                        qryConsulta = ndlElementos.Cast<XmlNode>()
                                                  .AsQueryable<XmlNode>()
                                                  .SelectMany(nodes => nodes.ChildNodes.Cast<XmlNode>());

                        foreach (var item in ndlElementos)
                        {
                            query = ((XmlElement)item)["ContaCorrente"].InnerText;

                            if (query.Split(' ')[2].StartsWith("4490"))
                            {
                                if (query.Split(' ')[1].ToString().Trim() == empenho.Trim())
                                {
                                    empenhoRestosAPagar = new AssetViewModelEmpenhoRestosAPagar()
                                    {
                                        Empenho = query.Split(' ')[1].ToString(),
                                        Cpf_Cnpj = query.Split(' ')[0].ToString(),
                                        ValorConta = Double.Parse(((System.Xml.XmlElement)item)["ValorConta"].InnerText),
                                        DebitoCredito = char.Parse(((System.Xml.XmlElement)item)["DebitoCredito"].InnerText),
                                    };
                                    break;
                                }
                            }
                        }
                    }
                    ndlElementos = null;
                }
            }
            catch (Exception excErroExecucao)
            {
                throw excErroExecucao;
            }

            return empenhoRestosAPagar;
        }

        #endregion

        //context em outros métodos
        private List<AssetNumberIdentification> RetornaHistoricoDeChapas(int assetId)
        {
            return (from c in db.AssetNumberIdentifications
                    where c.AssetId == assetId
                    select c).OrderByDescending(x => x.InclusionDate).ToList();
        }

        private string RetornaDescricaoDaContaContabil(int IdContaContabil)
        {
            var conta = (from c in db.AuxiliaryAccounts
                          where c.Id == IdContaContabil
                          select c).FirstOrDefault();

            if (conta == null) return string.Empty;
            return conta.ContaContabilApresentacao + " - " + conta.Description;
        }

        //context em outros métodos
        private void PreencheDadosDepreciacao(AssetViewModel asset) {
            if (!asset.checkFlagAcervo && !asset.checkFlagTerceiro)
            {
                int idUGE = asset.ManagerUnitId;

                //Caso usuário seja operador de UGE, se baseia na UGE logada. Senão, se baseia na UGE onde se encontra o BP
                if (_managerUnitId != null && _managerUnitId != 0)
                {
                    idUGE = (int)_managerUnitId;
                }

                string mesDeReferencia = (from mu in db.ManagerUnits
                                          where mu.Id == idUGE
                                          select mu.ManagmentUnit_YearMonthReference).FirstOrDefault();

                int ano = Convert.ToInt32(mesDeReferencia.Substring(0, 4));
                int mes = Convert.ToInt32(mesDeReferencia.Substring(4, 2));
                Asset objParaPesquisa = db.Assets
                                          .Where(a => a.Id == asset.Id)
                                          .Select(a => new
                                          {
                                              AssetStartId = a.AssetStartId,
                                              MaterialItemCode = a.MaterialItemCode,
                                              ManagerUnitId = idUGE
                                          })
                                          .ToList()
                                          .Select(a => new Asset {
                                              AssetStartId = a.AssetStartId,
                                              MaterialItemCode = a.MaterialItemCode,
                                              ManagerUnitId = a.ManagerUnitId
                                          }).FirstOrDefault();

                var listaDepreciacaoBP = (from m in db.MonthlyDepreciations
                                          where m.AssetStartId == objParaPesquisa.AssetStartId
                                           && m.MaterialItemCode == objParaPesquisa.MaterialItemCode
                                           && m.ManagerUnitId == objParaPesquisa.ManagerUnitId
                                           && (m.CurrentDate.Year < ano || m.CurrentDate.Year == ano && m.CurrentDate.Month <= mes)
                                           && m.QtdLinhaRepetida == 0
                                          select m).AsNoTracking();

                if (listaDepreciacaoBP.IsNotNull() && listaDepreciacaoBP.Count() > 0)
                {
                    asset.ValueUpdate = listaDepreciacaoBP.Min(x => x.CurrentValue);

                    List<HistoricoDepreciacaoViewModel> listaMonthlyDepreciation = (from m in listaDepreciacaoBP
                                                                                    select new HistoricoDepreciacaoViewModel
                                                                                    {
                                                                                        CurrentMonth = m.CurrentMonth,
                                                                                        VidaUtil = m.CurrentMonth + "/" + m.LifeCycle,
                                                                                        ValueAcquisition = m.ValueAcquisition,
                                                                                        AccumulatedDepreciation = m.AccumulatedDepreciation,
                                                                                        CurrentValue = m.CurrentValue,
                                                                                        DataDepreciacao = m.CurrentDate.Month + "/" + m.CurrentDate.Year,
                                                                                        CodigoUGE = m.RelatedManagerUnit.Code
                                                                                    }).ToList();

                    listaMonthlyDepreciation.ForEach(l => l.ValorAquisicao = "R$" + String.Format("{0:n2}", l.ValueAcquisition));
                    listaMonthlyDepreciation.ForEach(l => l.DepreciacaoAcumulada = "R$" + String.Format("{0:n2}", l.AccumulatedDepreciation));
                    listaMonthlyDepreciation.ForEach(l => l.ValorAtual = "R$" + String.Format("{0:n2}", l.CurrentValue));

                    asset.AssetHistoricDepreciations2 = listaMonthlyDepreciation.OrderByDescending(x => x.CurrentMonth).ToList();
                }
                else {
                    if (!asset.checkFlagDecretoSefaz)
                    {
                        asset.ValueUpdate = asset.ValueAcquisition;
                    }
                }
            }

            asset.ValueUpdateModel = asset.ValueUpdate.Value.ToString();
        }

        private void PreencheDadosHistoricoValoresDecreto(AssetViewModel asset)
        {
            if (asset.checkFlagDecretoSefaz == true)
            {
                List<HistoricoValoresDecretoViewModel> historico =
                    (from h in db.HistoricoValoresDecretos
                     where h.AssetId == asset.Id
                     select new HistoricoValoresDecretoViewModel {
                         ValorAquisicao = h.ValorAquisicao,
                         ValorRevalorizacao = h.ValorRevalorizacao,
                         DataAlteracao = h.DataAlteracao
                     }).ToList();

                historico.ForEach(h => h.textoValorAquisicao = string.Format("R$ {0:n2}", h.ValorAquisicao));
                historico.ForEach(h => h.textoValorRevalorizacao = string.Format("R$ {0:n2}", h.ValorRevalorizacao));
                historico.ForEach(h => h.textoDataAlteracao = h.DataAlteracao.ToString("dd/MM/yyyy"));

                asset.HistoricoDosValoresDoDecreto = historico;
            }
        }

        private void VerificaAlteracaoNumeroProcesso(AssetMovements AssetMovimento, string processo) {
            bool houveAlteracao = false;

            //Verifica se o Número do Processo foi alterado
            if (AssetMovimento.NumberPurchaseProcess != null)
            {
                if (string.IsNullOrEmpty(processo) ||
                    !AssetMovimento.NumberPurchaseProcess.Trim().Equals(processo.Trim()))
                {
                    houveAlteracao = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(processo) &&
                    !string.IsNullOrWhiteSpace(processo)
                    )
                {
                    houveAlteracao = true;
                }
            }

            if (houveAlteracao) {
                LogAlteracaoDadosBP log = new LogAlteracaoDadosBP();
                log.AssetId = AssetMovimento.AssetId;
                log.AssetMovementId = AssetMovimento.Id;
                log.Campo = "Processo";
                log.ValorAntigo = AssetMovimento.NumberPurchaseProcess;
                log.ValorNovo = processo;
                log.UserId = UserCommon.CurrentUser().Id;
                log.DataHora = DateTime.Now;
                db.Entry(log).State = EntityState.Added;
                AssetMovimento.NumberPurchaseProcess = processo;
            }
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
        private void PreencheDadosNLsSIAFEM02(AssetViewModel viewBemPatrimonial)
        {
            int assetId = 0;
            IList<string> relacaoDadosContabeisBP = null;
            string[] detalhesDadosContabeis = null;
            AssetViewModelDadosNLSIAFEM viewModeloDadosContabeis = null;
            IList<AssetViewModelDadosNLSIAFEM> listaViewModeloDadosContabeis = null;

            if (viewBemPatrimonial.Id > 0)
            {
                assetId = viewBemPatrimonial.Id;
                listaViewModeloDadosContabeis = new List<AssetViewModelDadosNLSIAFEM>();
                relacaoDadosContabeisBP = IntegracaoSIAFEMHelper.ObterDadosNLsBP(assetId);

                foreach (var dadosContabeisBP in relacaoDadosContabeisBP)
                {
                    viewModeloDadosContabeis = new AssetViewModelDadosNLSIAFEM();
                    detalhesDadosContabeis = dadosContabeisBP.BreakLine(";");

                    viewModeloDadosContabeis.DataHoraEnvioContabilizaSP = detalhesDadosContabeis[0];
                    viewModeloDadosContabeis.DocumentoSAM = detalhesDadosContabeis[1];
                    viewModeloDadosContabeis.NomeTipoNL = detalhesDadosContabeis[2];
                    viewModeloDadosContabeis.NumeroNL = detalhesDadosContabeis[3];
                    viewModeloDadosContabeis.ValorNL = detalhesDadosContabeis[4];

                    listaViewModeloDadosContabeis.Add(viewModeloDadosContabeis);
                }


                viewBemPatrimonial.DadosSIAFEM = listaViewModeloDadosContabeis;
            }
        }

        private void PreencheDadosTerceiro(AssetViewModel asset)
        {
            int? IdTerceiro = db.Assets.Find(asset.Id).OutSourcedId;
            if (IdTerceiro != null) {
                var obTerceiro = db.OutSourceds.Find(IdTerceiro);
                asset.OutSourcedId = IdTerceiro;
                asset.CPFCNPJDoTerceiro = obTerceiro.CPFCNPJ;
                asset.NomeDoTerceiro = obTerceiro.Name;
            }
        }

        #region Relatorios

        #region Relatório de Itens Inventario

        private LocalReport SetaPathRelatorio(string path)
        {
            LocalReport lr = new LocalReport();
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }

            return lr;
        }
        #endregion

        #region Relatório Analítico de Bem Patrimonial

        [HttpGet]
        public ActionResult ReportAnaliticoDeBemPatrimonial()
        {
            try
            {
                var retorno = new AnaliticoDeBemPatrimonialViewModel();

                getHierarquiaPerfil();
                CarregaHierarquiaFiltroReportAnaliticoBemPatrimonial(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0, _administrativeUnitId ?? 0, _sectionId ?? 0);
                CarregaComboStatus();
                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult ReportAnaliticoDeBemPatrimonial(AnaliticoDeBemPatrimonialViewModel analiticoModel)
        {
            try
            {
                LocalReport lr = SetaPathRelatorio(Path.Combine(Server.MapPath("~/Report"), "AssetAnaliticoDeBemPatrimonial.rdlc"));

                ReportDataSource rdBalance = new ReportDataSource("DataSource", RetornaDadosReportAnaliticoDeBemPatrimonial(analiticoModel.ListAssetsSelected));
                lr.DataSources.Add(rdBalance);

                string data = DateTime.Now.ToString("yyyyMMddHHmmss");

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =
                    "<DeviceInfo>" +
                        "  <OutputFormat>" + "PDF" + "</OutputFormat>" +
                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report
                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=RelatorioAnalitico" + data + ".pdf");
                Response.BinaryWrite(renderedBytes); // create the file
                Response.Flush(); // send it to the client to download

                CarregaHierarquiaFiltroReportAnaliticoBemPatrimonial(analiticoModel.InstitutionId, analiticoModel.BudgetUnitId ?? 0, 
                                                                     analiticoModel.ManagerUnitId ?? 0, analiticoModel.AdministrativeUnitId ?? 0, analiticoModel.SectionId ?? 0);
                CarregaComboStatus();

                return View(analiticoModel);
            }
            catch (HttpException ex)
            {
                //Código caso usuário feche a página antes de recebe o download
                switch (ex.ErrorCode)
                {
                    case -2147023901:
                    case -2147024832:
                        return HttpNotFound();
                    default:
                        return MensagemErro(CommonMensagens.PadraoException, ex);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        public ActionResult LoadDataAnaliticoDeBemPatrimonial(int? institutionId, int? budgetUnitId, int? managerUnitId, int? administrativeUnitId, int? sectionId, int? statusAsset, string numberIdentification)
        {
            List<AssetAnaliticoBemPatrimonialViewModel> listAssetsDB;

            try
            {

                using (db = new SAMContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    if (statusAsset != null && statusAsset == 1)
                    {

                        if (!string.IsNullOrEmpty(numberIdentification))
                        {
                            var listaNumberIdentifications = numberIdentification.Trim().Split(',').AsQueryable();

                            listAssetsDB = (from a in db.Assets
                                            join am in db.AssetMovements
                                            on a.Id equals am.AssetId
                                            join l in listaNumberIdentifications
                                            on a.NumberIdentification equals l.Trim()
                                            where
                                                  a.Status == true &&
                                                  am.Status == true &&
                                                  !a.flagVerificado.HasValue &&
                                                  a.flagDepreciaAcumulada == 1 &&
                                                  am.InstitutionId == ((institutionId.HasValue && institutionId != 0) ? institutionId : am.InstitutionId) &&
                                                  am.BudgetUnitId == ((budgetUnitId.HasValue && budgetUnitId != 0) ? budgetUnitId : am.BudgetUnitId) &&
                                                  am.ManagerUnitId == ((managerUnitId.HasValue && managerUnitId != 0) ? managerUnitId : am.ManagerUnitId) &&
                                                  am.AdministrativeUnitId == ((administrativeUnitId.HasValue && administrativeUnitId != 0) ? administrativeUnitId : am.AdministrativeUnitId) &&
                                                  am.SectionId == ((sectionId.HasValue && sectionId != 0) ? sectionId : am.SectionId)
                                            select new AssetAnaliticoBemPatrimonialViewModel
                                            {
                                                Id = a.Id,
                                                Sigla = a.InitialName,
                                                Chapa = a.NumberIdentification,
                                                ChapaCompleta = a.ChapaCompleta,
                                                MaterialItemDescription = a.MaterialItemDescription
                                            }).OrderBy(x => x.Sigla).AsNoTracking().ToList();
                        }
                        else
                        {
                            listAssetsDB = (from a in db.Assets
                                            join am in db.AssetMovements
                                            on a.Id equals am.AssetId
                                            where
                                                  a.Status == true &&
                                                  am.Status == true &&
                                                  !a.flagVerificado.HasValue &&
                                                  a.flagDepreciaAcumulada == 1 &&
                                                  am.InstitutionId == ((institutionId.HasValue && institutionId != 0) ? institutionId : am.InstitutionId) &&
                                                  am.BudgetUnitId == ((budgetUnitId.HasValue && budgetUnitId != 0) ? budgetUnitId : am.BudgetUnitId) &&
                                                  am.ManagerUnitId == ((managerUnitId.HasValue && managerUnitId != 0) ? managerUnitId : am.ManagerUnitId) &&
                                                  am.AdministrativeUnitId == ((administrativeUnitId.HasValue && administrativeUnitId != 0) ? administrativeUnitId : am.AdministrativeUnitId) &&
                                                  am.SectionId == ((sectionId.HasValue && sectionId != 0) ? sectionId : am.SectionId)
                                            select new AssetAnaliticoBemPatrimonialViewModel
                                            {
                                                Id = a.Id,
                                                Sigla = a.InitialName,
                                                Chapa = a.NumberIdentification,
                                                ChapaCompleta = a.ChapaCompleta,
                                                MaterialItemDescription = a.MaterialItemDescription
                                            }).OrderBy(x => x.Sigla).AsNoTracking().ToList();
                        }
                    }
                    else
                    {
                        IQueryable<int> lstAgrupada;

                        if (!string.IsNullOrEmpty(numberIdentification))
                        {
                            var listaNumberIdentifications = numberIdentification.Trim().Split(',').AsQueryable();

                            lstAgrupada = (from am in db.AssetMovements
                                           join a in db.Assets on am.AssetId equals a.Id
                                           join l in listaNumberIdentifications on a.NumberIdentification equals l.Trim()
                                           where a.Status == false &&
                                           !a.flagVerificado.HasValue &&
                                           a.flagDepreciaAcumulada == 1 &&
                                           (am.MovementTypeId == (int)EnumMovimentType.VoltaConserto ||
                                            am.MovementTypeId == (int)EnumMovimentType.SaidaConserto ||
                                            am.MovementTypeId == (int)EnumMovimentType.Extravio ||
                                            am.MovementTypeId == (int)EnumMovimentType.Obsoleto ||
                                            am.MovementTypeId == (int)EnumMovimentType.Danificado ||
                                            am.MovementTypeId == (int)EnumMovimentType.Sucata ||
                                            am.MovementTypeId == (int)EnumMovimentType.Transferencia ||
                                            am.MovementTypeId == (int)EnumMovimentType.Doacao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovComodatoTerceirosRecebidos ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoConsolidacao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoMunicipio ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoOutrosEstados ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoUniao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovMorteAnimalPatrimoniado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovSementesPlantasInsumosArvores ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovVendaLeilaoSemoventes
                                            ) &&
                                           am.Status == false &&
                                           am.FlagEstorno != true &&
                                           am.InstitutionId == ((institutionId.HasValue && institutionId != 0) ? institutionId : am.InstitutionId) &&
                                           am.BudgetUnitId == ((budgetUnitId.HasValue && budgetUnitId != 0) ? budgetUnitId : am.BudgetUnitId) &&
                                           am.ManagerUnitId == ((managerUnitId.HasValue && managerUnitId != 0) ? managerUnitId : am.ManagerUnitId) &&
                                           am.AdministrativeUnitId == ((administrativeUnitId.HasValue && administrativeUnitId != 0) ? administrativeUnitId : am.AdministrativeUnitId) &&
                                           am.SectionId == ((sectionId.HasValue && sectionId != 0) ? sectionId : am.SectionId)
                                           group am by new { am.AssetId, am.InstitutionId } into g
                                           select g.Max(p => p.Id)
                                       );
                        }
                        else
                        {
                            lstAgrupada = (from am in db.AssetMovements
                                           join a in db.Assets on am.AssetId equals a.Id
                                           where a.Status == false &&
                                           !a.flagVerificado.HasValue &&
                                           a.flagDepreciaAcumulada == 1 &&
                                           (am.MovementTypeId == (int)EnumMovimentType.VoltaConserto ||
                                            am.MovementTypeId == (int)EnumMovimentType.SaidaConserto ||
                                            am.MovementTypeId == (int)EnumMovimentType.Extravio ||
                                            am.MovementTypeId == (int)EnumMovimentType.Obsoleto ||
                                            am.MovementTypeId == (int)EnumMovimentType.Danificado ||
                                            am.MovementTypeId == (int)EnumMovimentType.Sucata ||
                                            am.MovementTypeId == (int)EnumMovimentType.Transferencia ||
                                            am.MovementTypeId == (int)EnumMovimentType.Doacao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGEDoacao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovSaidaInservivelUGETransferencia ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoConsolidacao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoMunicipio ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoOutrosEstados ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoUniao ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovDoacaoIntraNoEstado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovMorteAnimalPatrimoniado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovSementesPlantasInsumosArvores ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado ||
                                            am.MovementTypeId == (int)EnumMovimentType.MovVendaLeilaoSemoventes
                                            ) &&
                                           am.Status == false &&
                                           am.FlagEstorno != true &&
                                           am.InstitutionId == ((institutionId.HasValue && institutionId != 0) ? institutionId : am.InstitutionId) &&
                                           am.BudgetUnitId == ((budgetUnitId.HasValue && budgetUnitId != 0) ? budgetUnitId : am.BudgetUnitId) &&
                                           am.ManagerUnitId == ((managerUnitId.HasValue && managerUnitId != 0) ? managerUnitId : am.ManagerUnitId) &&
                                           am.AdministrativeUnitId == ((administrativeUnitId.HasValue && administrativeUnitId != 0) ? administrativeUnitId : am.AdministrativeUnitId) &&
                                           am.SectionId == ((sectionId.HasValue && sectionId != 0) ? sectionId : am.SectionId)
                                           group am by new { am.AssetId, am.InstitutionId } into g
                                           select g.Max(p => p.Id)
                                       );
                        }

                        var lstAssetMovements = (from am in db.AssetMovements
                                                 join r in lstAgrupada
                                                 on am.Id equals r
                                                 select am.AssetId);

                            listAssetsDB = (from a in db.Assets
                                            join am in lstAssetMovements
                                            on a.Id equals am
                                            select new AssetAnaliticoBemPatrimonialViewModel
                                            {
                                                Id = a.Id,
                                                Sigla = a.InitialName,
                                                Chapa = a.NumberIdentification,
                                                ChapaCompleta = a.ChapaCompleta,
                                                MaterialItemDescription = a.MaterialItemDescription
                                            }).OrderBy(x => x.Sigla).AsNoTracking().ToList();
                    }
                }

                long paraValidacao = 0;

                var oAnaliticoDeBemPatrimonial = new AnaliticoDeBemPatrimonialViewModel()
                {
                    InstitutionId = institutionId ?? 0,
                    BudgetUnitId = budgetUnitId,
                    ManagerUnitId = managerUnitId,
                    AdministrativeUnitId = administrativeUnitId,
                    SectionId = sectionId,
                    ListAssets = listAssetsDB.OrderBy(y => long.TryParse(y.Chapa, out paraValidacao) ? long.Parse(y.Chapa) : 99999999999).ToList()
                };

                return PartialView("ReportAnalitico/_dataReportAnaliticoDeBemPatrimonial", oAnaliticoDeBemPatrimonial);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        private void CarregaHierarquiaFiltroReportAnaliticoBemPatrimonial(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0)
        {
            hierarquia = new Hierarquia();
            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description", modelBudgetUnitId);
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(null), "Id", "Description", modelManagerUnitId);
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(null), "Id", "Description", modelAdministrativeUnitId);
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(null), "Id", "Description", modelSectionId);
            }
            else
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);

                if (modelBudgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

                if (modelManagerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

                if (modelAdministrativeUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
                else 
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

                if (modelSectionId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
                else 
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
            }
        }

        private DataTable RetornaDadosReportAnaliticoDeBemPatrimonial(string listAssetsSelected)
        {
            List<int> listAssertsSelected = listAssetsSelected != null ?
                                            Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(listAssetsSelected) :
                                            new List<int>();

            string spName = "REPORT_ANALITICO_DE_BEM_PATRIMONIAL";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "assertsSelecteds", valor = (listAssertsSelected == null ? string.Empty : string.Join(",", listAssertsSelected)) });

            FunctionsCommon common = new FunctionsCommon();
            DataTable dtBarCodes = common.ReturnDataFromStoredProcedureReport(listaParam, spName);

            return dtBarCodes;
        }

        private void CarregaComboStatus()
        {
            var lista = new List<ItemGenericViewModel>();

            var itemGeneric = new ItemGenericViewModel
            {
                Id = 0,
                Description = "Baixados",
                Ordem = 10
            };

            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel
            {
                Id = 1,
                Description = "Ativos",
                Ordem = 5
            };

            lista.Add(itemGeneric);

            ViewBag.StatusAsset = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");
        }
        #endregion

        #region Relatório de Resumo do Inventario de Bens Moveis

        public JsonResult CarregaComboPeriodosPorUGE(int budgetUnitId, int? managerUnitId)
        {
            using (db = new SAMContext())
            {
                return Json(CarregaListaComPeriodosDaUGE(budgetUnitId, managerUnitId), JsonRequestBehavior.AllowGet);
            }
        }

        private SelectList CarregaListaComPeriodosDaUGE(int budgetUnitId, int? managerUnitId, string mesRef = null) {
            List<SelectListItem> data = new List<SelectListItem>();

            data.Add(new SelectListItem() { Value = "0", Text = "Selecione"} );

            if (managerUnitId != null)
            {
                var managerUnit = (from m in db.ManagerUnits
                                   where m.BudgetUnitId == budgetUnitId &&
                                         m.Id == managerUnitId
                                   select m).AsNoTracking().FirstOrDefault();

                if (managerUnit != null)
                {
                    string anoIni = managerUnit.ManagmentUnit_YearMonthStart.Substring(0, 4);
                    string mesIni = managerUnit.ManagmentUnit_YearMonthStart.Substring(4, 2);

                    string anoRefUGE = managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4);
                    string mesRefUGE = managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2);

                    var DataInicialUGE = new DateTime(int.Parse(anoIni), int.Parse(mesIni), 1);
                    var DataRefUGE = new DateTime(int.Parse(anoRefUGE), int.Parse(mesRefUGE), 1);


                    string AnoMesId = managerUnit.ManagmentUnit_YearMonthStart;
                    string AnoMes = string.Concat(mesIni.PadLeft(2, '0'), "/", anoIni);

                    while (DataInicialUGE < DataRefUGE)
                    {
                        AnoMesId = string.Concat(DataInicialUGE.Year.ToString(), DataInicialUGE.Month.ToString().PadLeft(2, '0'));
                        AnoMes = string.Concat(DataInicialUGE.Month.ToString().PadLeft(2, '0'), "/", DataInicialUGE.Year.ToString());
                        data.Add(new SelectListItem() { Value = AnoMesId, Text = AnoMes });

                        DataInicialUGE = DataInicialUGE.AddDays(36);
                        DataInicialUGE = new DateTime(DataInicialUGE.Year, DataInicialUGE.Month, 1);
                    }
                }
                
            }

            if(!string.IsNullOrEmpty(mesRef) && !string.IsNullOrWhiteSpace(mesRef))
                return new SelectList(data, "Value", "Text", mesRef);
            else
                return new SelectList(data, "Value", "Text", "0");
        }

        #endregion


        //[HttpPost]
        //[CustomAnotacaoMultiplosBotoes(Name = "action", Argument = "RelatorioSaldoContaOrgao")]
        //public ActionResult RelatorioSaldoContaOrgao(AssetViewModel objAsset)
        //{
        //    try
        //    {
        //        getHierarquiaPerfil();

        //        if (objAsset.InstitutionId != 0 && (!PerfilAdmGeral() && objAsset.InstitutionId == _institutionId))
        //        {
        //            List<ReportParameter> parametros = new List<ReportParameter>();


        //            //ACESSO AO CAMINHO FÍSICO DO RELTÓRIO
        //            string path = PathDoTipoDeRelatorio("SaldoContabilOrgao.rdlc");
        //            LocalReport lr = SetaPathRelatorio(path);
        //            //FIM - ACESSO AO CAMINHO FÍSICO DO RELTÓRIO

        //            //ACESSO A PROCEDURE PARA CONSULTA DOS DADOS
        //            DataTable dtAssetInv = RetornaDadosRelatorioSaldoContabilOrgao(objAsset.InstitutionId, objAsset.MesRef);
        //            //FIM - ACESSO A PROCEDURE PARA CONSULTA DOS DADOS

        //            string codigoOrgao = (from i in db.Institutions where i.Id == objAsset.InstitutionId select i.Code + " - " + i.Description).AsNoTracking().FirstOrDefault();
        //            parametros.Add(new ReportParameter("ORGAO", codigoOrgao));

        //            //DESCOMENTAR PARA GERAR O RELATÓRIO

        //            lr.SetParameters(parametros);

        //            ReportDataSource rdBalance = new ReportDataSource("dsSaldoContabil", dtAssetInv);
        //            lr.DataSources.Add(rdBalance);

        //            string data = DateTime.Now.ToString("yyyyMMddHHmmss");

        //            string reportType = "PDF";
        //            string mimeType;
        //            string encoding;
        //            string fileNameExtension;

        //            string deviceInfo =
        //                "<DeviceInfo>" +
        //                    "  <OutputFormat>" + "PDF" + "</OutputFormat>" +
        //                "</DeviceInfo>";

        //            Warning[] warnings;
        //            string[] streams;
        //            byte[] renderedBytes;


        //            //Render the report
        //            renderedBytes = lr.Render(
        //                reportType,
        //                deviceInfo,
        //                out mimeType,
        //                out encoding,
        //                out fileNameExtension,
        //                out streams,
        //                out warnings);
        //            Response.Buffer = true;
        //            Response.Clear();
        //            Response.ContentType = mimeType;
        //            Response.AddHeader("content-disposition", "attachment; filename=RelatorioSaldoContabilOrgao" + data + ".pdf");
        //            Response.BinaryWrite(renderedBytes); // create the file
        //            Response.Flush(); // send it to the client to download

        //            //FIM - DESCOMENTAR PARA GERAR O RELATÓRIO

        //        }
        //        else
        //        {
        //            if (objAsset.InstitutionId == 0)
        //                ModelState.AddModelError("InstitutionId", "Por favor, informe o Orgão");
        //            else {
        //                if((!PerfilAdmGeral() && objAsset.InstitutionId != _institutionId))
        //                    ModelState.AddModelError("InstitutionId", "Ação inválida");
        //            }
        //        }

        //        return RedirectToAction("RelatorioSaldoContaOrgao", "Assets");

        //    }
        //    catch (Exception ex)
        //    {
        //        return MensagemErro(CommonMensagens.PadraoException, ex);
        //    }
        //}

        #endregion

        #region Metodos Privados Importados de outras controllers

        #region ShortDescriptionItem
        //context em outros métodos
        public ShortDescriptionItem VerificarDescricaoResumida(string Material)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;

            ShortDescriptionItem _shortDescriptionItem;

            if ((from s in db.ShortDescriptionItens where s.Description.Trim() == Material.Trim() select s).Any())
                _shortDescriptionItem = (from s in db.ShortDescriptionItens where s.Description.Trim() == Material.Trim() select s).FirstOrDefault();
            else
            {
                _shortDescriptionItem = new ShortDescriptionItem();
                _shortDescriptionItem.Description = Material;

                db.Entry(_shortDescriptionItem).State = EntityState.Added;
                db.SaveChanges();
            }

            return _shortDescriptionItem;
        }

        #endregion

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

        private void ExcluirRegistrosdoBP(int assetId) {

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

            var tipoIncorporacao = (from am in db.AssetMovements
                                    join m in db.MovementTypes on am.MovementTypeId equals m.Id
                                    where am.AssetId == assetId
                                    && m.GroupMovimentId == (int)EnumGroupMoviment.Incorporacao
                                    select m.Id).FirstOrDefault();

            var AssetStartIdDoBP = (from a in db.Assets where a.Id == assetId select a).FirstOrDefault().AssetStartId;

            if (tipoIncorporacao != (int)EnumMovimentType.Transferencia &&
                tipoIncorporacao != (int)EnumMovimentType.Doacao &&
                tipoIncorporacao != (int)EnumMovimentType.IncorporacaoPorTransferencia &&
                tipoIncorporacao != (int)EnumMovimentType.IncorporacaoPorDoacao &&
                tipoIncorporacao != (int)EnumMovimentType.IncorpDoacaoIntraNoEstado &&
                tipoIncorporacao != (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado &&
                tipoIncorporacao != (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado &&
                tipoIncorporacao != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao &&
                tipoIncorporacao != (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia &&
                AssetStartIdDoBP != null)
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MonthlyDepreciation] where AssetStartId = " + AssetStartIdDoBP.ToString());
            }

            //remove o registro da Asset
            db.Database.ExecuteSqlCommand("delete from [dbo].[Asset] where Id = " + assetIdString);

            db.SaveChanges();
        }

        #endregion

        #region Validação ExcluirBP

        private bool BPNaoSeTratarDeAceite(int Id) {
            AssetMovements incorporacao = (from am in db.AssetMovements
                                           join m in db.MovementTypes 
                                           on am.MovementTypeId equals m.Id
                                           join a in db.Assets
                                           on am.AssetId equals a.Id
                                           where am.AssetId == Id
                                           && m.GroupMovimentId == 1
                                           && a.Status
                                           select am).FirstOrDefault();

            if(incorporacao == null) //muito provavelmente porque o BP não está ativo (aqui, só verificado pela coluna Status na tabela Asset = 1)
                return false;

            switch (incorporacao.MovementTypeId) {
                case (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado:
                    return false;
                case (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado:
                case (int)EnumMovimentType.IncorpDoacaoIntraNoEstado:
                case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao:
                case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia:
                    //Caso os aceites dessas incorporações tenham sidos feitas manualmente
                    if (incorporacao.SourceDestiny_ManagerUnitId == null)
                        return false;
                    break;
            }

            return true;
        }

        private bool BPPodeSerExcluidoPorNaoTerPendenciaSIAFEM(int AssetId)
        {
            var lista = db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                          .Where(r => r.AssetId == AssetId)
                          .Select(r => r.AuditoriaIntegracaoId);

            if (lista.Count() == 0)
                return true;

            return (from p in db.NotaLancamentoPendenteSIAFEMs
                    join l in lista on p.AuditoriaIntegracaoId equals l
                    where p.StatusPendencia == 1
                    select p.Id).Count() == 0;
        }
        #endregion

        #region Outros Métodos Incorporacao
        private AssetViewModel PreencheValoresAquisicaoBPVindoDeOutraUGE (AssetViewModel viewModel){
            var BPnaUGEDeOriggem = (from a in db.Assets where a.Id == viewModel.AssetsIdTransferencia && a.Status == true select a).FirstOrDefault();
            viewModel.AcquisitionDate = BPnaUGEDeOriggem.AcquisitionDate;
            viewModel.ValueAcquisitionModel = BPnaUGEDeOriggem.ValueAcquisition.ToString();
            return viewModel;
        }

        private AssetViewModel PreencheValoresAquisicaoBPInservivelVindoDeOutraUGE(AssetViewModel viewModel)
        {
            var BPnaUGEDeOriggem = (from a in db.Assets where a.Id == viewModel.AssetsIdTransferencia && a.Status == false select a).FirstOrDefault();
            viewModel.AcquisitionDate = BPnaUGEDeOriggem.AcquisitionDate;
            viewModel.ValueAcquisitionModel = BPnaUGEDeOriggem.ValueAcquisition.ToString();
            return viewModel;
        }

        private void ValidaCamposValorAtualizado(AssetViewModel viewModel) {
            if (string.IsNullOrEmpty(viewModel.ValueUpdateModel))
            {
                ModelState.AddModelError("ValueUpdateModel", "Por favor, informe o Valor Atual do Bem.");
            }
            else
            {
                if (Decimal.Parse(viewModel.ValueUpdateModel) < 10)
                {
                    ModelState.AddModelError("ValueUpdateModel", "Valor Atual do Bem a ser incorporado não pode ser menor que R$10,00");
                }
            }
        }

        private void PreencheValoresPadraoParaAcervo(AssetViewModel viewModel) {
            viewModel.MaterialItemCode = 5628156;
            viewModel.MaterialItemDescription = "Acervos";
            viewModel.MaterialGroupCode = 99;
            viewModel.LifeCycle = 0;
            viewModel.RateDepreciationMonthly = 0;
            viewModel.ResidualValue = 0;
        }

        private void PreencheValoresPadraoParaTerceiro(AssetViewModel viewModel)
        {
            viewModel.MaterialItemCode = 5628121;
            viewModel.MaterialItemDescription = "Bens de Terceiros";
            viewModel.MaterialGroupCode = 99;
            viewModel.LifeCycle = 0;
            viewModel.RateDepreciationMonthly = 0;
            viewModel.ResidualValue = 0;
        }

        private bool RecebimentoSendoFeitoPeloFundoSocial(int ManagerUnitId) {
            return (from i in db.Institutions
                    join b in db.BudgetUnits on i.Id equals b.InstitutionId
                    join m in db.ManagerUnits on b.Id equals m.BudgetUnitId
                    where i.Code == "51004" 
                    && m.Code == "510032" 
                    && m.Status == true
                    && m.Id == ManagerUnitId
                    select m.Id).Count() > 0;
        }

        private bool OrgaoFundoSocial(int InstitutionId) {
            return (from i in db.Institutions
                    where i.Id == InstitutionId
                    && i.Code == "51004"
                    select i.Id).Any();
        }

        private void ValidaContaContabil(AssetViewModel viewmodel) {

            int tipo = 0;
            bool? tipoAcervo = false;
            bool? tipoTerceiro = false;

            if (viewmodel.AuxiliaryAccountId == null)
            {
                ModelState.AddModelError("AuxiliaryAccountId", "O campo Conta Contábil é obrigatório.");
            }
            else
            {

                if (viewmodel.MovementTypeId == (int)EnumMovimentType.IncorporacaoDeInventarioInicial
                    || viewmodel.MovementTypeId == (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos
                    || viewmodel.MovementTypeId == (int)EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao)
                {
                    tipo = viewmodel.checkFlagAcervo == true ? 1 : (viewmodel.checkFlagTerceiro == true ? 2 : 0);

                    BuscaContaPorGrupoOuTipo(viewmodel,tipo);
                }
                else
                {
                    if (viewmodel.AssetsIdTransferencia == null)
                    {
                        BuscaContaPorGrupoOuTipo(viewmodel);
                    }
                    else
                    {
                        tipoAcervo = (from a in db.Assets
                                      where a.Id == viewmodel.AssetsIdTransferencia
                                      select a.flagAcervo).FirstOrDefault();

                        tipoTerceiro = (from a in db.Assets
                                        where a.Id == viewmodel.AssetsIdTransferencia
                                        select a.flagTerceiro).FirstOrDefault();

                        tipo = tipoAcervo == true ? 1 : (tipoTerceiro == true ? 2 : 0);

                        BuscaContaPorGrupoOuTipo(viewmodel, tipo);
                    }
                }
            }
        }

        private void ValidaDescricaoResumida(AssetViewModel viewmodel) {
            if (string.IsNullOrEmpty(viewmodel.ShortDescription) ||
                string.IsNullOrWhiteSpace(viewmodel.ShortDescription)
               )
            {
                ModelState.AddModelError("ShortDescription", "O Campo Descrição Resumida do Item é obrigatório");
            }
            else {
                ModelState.Remove("ShortDescriptionItemId");
            }
        }

        private void BuscaContaPorGrupoOuTipo(AssetViewModel viewmodel, int tipo = 0) {
            if (tipo == 0)
            {
                if (!(from racmg in db.RelationshipAuxiliaryAccountItemGroups
                      join mg in db.MaterialGroups on racmg.MaterialGroupId equals mg.Id
                      where mg.Code == viewmodel.MaterialGroupCode
                            && racmg.AuxiliaryAccountId == viewmodel.AuxiliaryAccountId
                      select racmg).Any()
                )
                {
                    ModelState.AddModelError("AuxiliaryAccountId", "O campo Conta Contábil é obrigatório.");
                }
            }
            else
            {
                if (!(from ac in db.AuxiliaryAccounts
                      where ac.RelacionadoBP == tipo
                         && ac.Id == viewmodel.AuxiliaryAccountId
                      select ac).Any())
                {
                    ModelState.AddModelError("AuxiliaryAccountId", "O campo Conta Contábil é obrigatório.");
                }
            }
        }

        private bool VerificaSeIncorporacaoEstaNoMesRef(AssetViewModel viewmodel) {
            var AssetNaUGEOrigem = (from a in db.AssetMovements where a.AssetId == viewmodel.AssetsIdTransferencia && a.Status == true select a).FirstOrDefault();

            string MesReferenciaUgeDoado = RecuperaAnoMesReferenciaFechamento(AssetNaUGEOrigem.ManagerUnitId);
            string MesReferenciaUgeInformada = RecuperaAnoMesReferenciaFechamento(viewmodel.ManagerUnitId);

            if (int.Parse(MesReferenciaUgeDoado) != int.Parse(MesReferenciaUgeInformada))
            {
                ModelState.AddModelError("MovimentDate", "Só é possivel Incorporar o item Doado no Mês Ref " + AssetNaUGEOrigem.MovimentDate.Month + "/" + AssetNaUGEOrigem.MovimentDate.Year);
                return false;
            }
            if (viewmodel.MovimentDate < AssetNaUGEOrigem.MovimentDate)
            {
                ModelState.AddModelError("MovimentDate", "A Data de Incorporação precisa ser maior ou igual a Data de Movimento do Item Doado - " + AssetNaUGEOrigem.MovimentDate.Date);
                return false;
            }

            return true;
        }

        private bool VerificaSeTransferenciaOuDoacaoNaOrigemPossuiPendencia(AssetViewModel viewmodel)
        {
            var AssetNaUGEOrigem = (from a in db.AssetMovements where a.AssetId == viewmodel.AssetsIdTransferencia && a.Status == true select a).FirstOrDefault();
            bool possuiPendencia = MovimentacaoComPendenciasDeNLContabiliza(AssetNaUGEOrigem.ManagerUnitId, AssetNaUGEOrigem.AuditoriaIntegracaoId);

            if (possuiPendencia) {
                ModelState.AddModelError("NumberDoc", "BP não pode ser aceito, pois está com Pendências no SIAFEM a serem resolvidas. Por favor, entre em contato com a UGE de origem.");
            }

            return possuiPendencia;
        }

        private bool VerificaSeSaidaNaOrigemPossuiPendencia(AssetViewModel viewmodel)
        {
            var AssetNaUGEOrigem = (from a in db.AssetMovements
                                    where a.AssetId == viewmodel.AssetsIdTransferencia &&
                                          a.Status == false &&
                                          a.FlagEstorno != true &&
                                          a.AssetTransferenciaId == null
                                    select a).FirstOrDefault();


            bool possuiPendencia = MovimentacaoComPendenciasDeNLContabiliza(AssetNaUGEOrigem.ManagerUnitId, AssetNaUGEOrigem.AuditoriaIntegracaoId);

            if (possuiPendencia)
            {
                ModelState.AddModelError("NumberDoc", "BP não pode ser aceito, pois está com Pendências no SIAFEM a serem resolvidas. Por favor, entre em contato com a UGE de origem.");
            }

            return possuiPendencia;
        }
        private bool VerificaSeIncorporacaoInservivelEstaNoMesRef(AssetViewModel viewmodel)
        {
            var AssetNaUGEOrigem = (from a in db.AssetMovements
                                    where a.AssetId == viewmodel.AssetsIdTransferencia && 
                                          a.Status == false &&
                                          a.FlagEstorno != true &&
                                          a.AssetTransferenciaId == null
                                    select a).FirstOrDefault();


            string MesReferenciaUgeDoado = RecuperaAnoMesReferenciaFechamento(AssetNaUGEOrigem.ManagerUnitId);
            string MesReferenciaUgeInformada = RecuperaAnoMesReferenciaFechamento(viewmodel.ManagerUnitId);

            if (int.Parse(MesReferenciaUgeDoado) != int.Parse(MesReferenciaUgeInformada))
            {
                ModelState.AddModelError("MovimentDate", "Só é possivel Incorporar a Saída Inservivel no Mês Ref " + AssetNaUGEOrigem.MovimentDate.Month + "/" + AssetNaUGEOrigem.MovimentDate.Year);
                return false;
            }
            if (viewmodel.MovimentDate < AssetNaUGEOrigem.MovimentDate)
            {
                ModelState.AddModelError("MovimentDate", "A Data de Incorporação precisa ser maior ou igual a Data de Movimento a Saída Inservivel - " + AssetNaUGEOrigem.MovimentDate.Date);
                return false;
            }

            return true;
        }

        private bool VerificaSeDuplicaraBPsEmLote(AssetViewModel viewmodel) {
            Int64 ChapaInicial = Convert.ToInt64(viewmodel.NumberIdentification);
            Int64 ChapaFinal = Convert.ToInt64(viewmodel.EndNumberIdentification);

            Asset _asset = null;
            AssetViewModel viewModelParaBPDuplicado = new AssetViewModel();

            viewModelParaBPDuplicado.DiferenciacaoChapa = viewmodel.DiferenciacaoChapa;
            viewModelParaBPDuplicado.InstitutionId = viewmodel.InstitutionId;
            viewModelParaBPDuplicado.InitialId = viewmodel.InitialId;

            string casasFrente = viewmodel.NumberIdentification.Replace(ChapaInicial.ToString(), "");

            for (Int64 i = ChapaInicial; i <= ChapaFinal; i++)
            {
                viewModelParaBPDuplicado.NumberIdentification = casasFrente + i.ToString();
                _asset = BPAtivoComMesmaSiglaChapa(viewModelParaBPDuplicado);
                if (_asset != null)
                    break;
            }

            if (_asset != null)
            {
                ModelState.AddModelError("NumberIdentification", "A Sigla: " + _asset.RelatedInitial.Name + " - " + _asset.RelatedInitial.Description + " e a Chapa: " + _asset.NumberIdentification + _asset.DiferenciacaoChapa + " já estão cadastrados na UGE: " + _asset.RelatedManagerUnit.Code + " - " + _asset.RelatedManagerUnit.Description);
                return true;
            }
            else {
                return false;
            }
        }

        private bool VerificaSeDuplicaraBPs(AssetViewModel viewmodel) {
            Asset _assetRetornoItem = null;
            bool pesquisaSemBPDaUGEOrigem = false;

            if (viewmodel.MovementTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                viewmodel.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado)
            {
                if (viewmodel.AceiteManual == true)
                    pesquisaSemBPDaUGEOrigem = false;
                else {
                    pesquisaSemBPDaUGEOrigem = true;
                }
            }
            else {
                if (viewmodel.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado)
                    pesquisaSemBPDaUGEOrigem = true;
                else
                    pesquisaSemBPDaUGEOrigem = false;

            }

            if (pesquisaSemBPDaUGEOrigem)
            {
                _assetRetornoItem = (from a in db.Assets
                                     join m in db.AssetMovements on a.Id equals m.AssetId
                                     join ma in db.ManagerUnits on a.ManagerUnitId equals ma.Id
                                     where a.NumberIdentification == viewmodel.NumberIdentification
                                        && a.DiferenciacaoChapa == viewmodel.DiferenciacaoChapa
                                        && a.InitialId == viewmodel.InitialId
                                        && !a.flagVerificado.HasValue
                                        && a.flagDepreciaAcumulada == 1
                                        && a.Status == true
                                        && m.InstitutionId == viewmodel.InstitutionId
                                        && a.Id != viewmodel.AssetsIdTransferencia
                                     select a).FirstOrDefault();
            }
            else
            {
                _assetRetornoItem = BPAtivoComMesmaSiglaChapa(viewmodel);
            }

            if (_assetRetornoItem != null)
            {
                ModelState.AddModelError("NumberIdentification", "A Sigla: " + _assetRetornoItem.InitialName + " e a Chapa: " + _assetRetornoItem.NumberIdentification + _assetRetornoItem.DiferenciacaoChapa + " já estão cadastrados na UGE: " + _assetRetornoItem.RelatedManagerUnit.Code + " - " + _assetRetornoItem.RelatedManagerUnit.Description);
                return true;
            }
            else {
                return false;
            }

        }

        private Asset BPAtivoComMesmaSiglaChapa(AssetViewModel viewmodel) {
            return (from a in db.Assets
                    join m in db.AssetMovements on a.Id equals m.AssetId
                    join ma in db.ManagerUnits on a.ManagerUnitId equals ma.Id
                    where a.NumberIdentification == viewmodel.NumberIdentification
                       && a.DiferenciacaoChapa == viewmodel.DiferenciacaoChapa
                       && a.InitialId == viewmodel.InitialId
                       && !a.flagVerificado.HasValue
                       && a.flagDepreciaAcumulada == 1
                       && a.Status == true
                       && m.InstitutionId == viewmodel.InstitutionId
                    select a).FirstOrDefault();
        }

        private AssetViewModel PreencheDados(AssetViewModel viewmodel) {

            viewmodel = PreencheDadosDescricaoResumida(viewmodel);
            switch (viewmodel.MovementTypeId)
            {
                case (int)EnumMovimentType.IncorporacaoDeInventarioInicial:
                    if (!viewmodel.checkFlagAcervo && !viewmodel.checkFlagTerceiro)
                    {
                        viewmodel = PreencheDadosDepreciacaoPorGrupo(viewmodel);
                    }
                    break;
                case (int)EnumMovimentType.IncorpDoacaoIntraNoEstado:
                case (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado:
                case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao:
                case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia:
                    if (viewmodel.AssetsIdTransferencia != null && viewmodel.AssetsIdTransferencia > 0)
                    {
                        viewmodel = PreencheDadosPorBPUGEOrigem(viewmodel);
                    }
                    else {
                        viewmodel = PreencheDadosDepreciacaoPorGrupo(viewmodel);
                    }
                    break;
                case (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado:
                    viewmodel = PreencheDadosPorBPUGEOrigem(viewmodel);
                    break;
                default:

                    if (!viewmodel.checkDepreciacao && !viewmodel.checkFlagAcervo && !viewmodel.checkFlagTerceiro)
                    {
                        viewmodel = PreencheDadosDepreciacaoPorGrupo(viewmodel);
                    }

                    break;
            }

            ValidaEPreencheAnimalNaoServico(viewmodel);

            return viewmodel;
        }

        private AssetViewModel PreencheDadosDescricaoResumida(AssetViewModel viewmodel) {
            string buscaNoBanco = viewmodel.ShortDescription;
            var _ShortDescription = (from a in db.ShortDescriptionItens where a.Description == buscaNoBanco select a)
                                     .AsNoTracking().FirstOrDefault();

            if (_ShortDescription.IsNotNull())
            {
                viewmodel.ShortDescriptionItemId = _ShortDescription.Id;
            }
            else
            {
                ShortDescriptionItem _shortDescriptionItem = new ShortDescriptionItem();
                _shortDescriptionItem.Description = viewmodel.ShortDescription;

                db.Entry(_shortDescriptionItem).State = EntityState.Added;
                db.SaveChanges();

                viewmodel.ShortDescriptionItemId = _shortDescriptionItem.Id;
            }

            return viewmodel;
        }

        private void PreencheValorAquisicaoDeDecreto(Asset asset) {
                if (asset.flagDecreto == true)
                {
                    HistoricoValoresDecreto historico = new HistoricoValoresDecreto();
                    historico.AssetId = asset.Id;
                    historico.ValorAquisicao = asset.ValueAcquisition;
                    historico.ValorRevalorizacao = Convert.ToDecimal(asset.ValueUpdate);
                    historico.DataAlteracao = asset.MovimentDate;
                    historico.LoginId = UserCommon.CurrentUser().Id;
                    db.Entry(historico).State = EntityState.Added;
                    db.SaveChanges();

                    asset.ValueAcquisition = Convert.ToDecimal(asset.ValueUpdate);
                    db.Entry(asset).State = EntityState.Modified;
                    db.SaveChanges();
            }
            
        }

        private void CopiaValorAquisicaoDeDecreto(Asset antigo, Asset novo)
        {
            if (antigo.flagDecreto == true)
            {

                var lista = db.HistoricoValoresDecretos
                              .Where(h => h.AssetId == antigo.Id)
                              .ToList();

                HistoricoValoresDecreto salvar;
                foreach (HistoricoValoresDecreto historico in lista){
                    salvar = new HistoricoValoresDecreto();
                    salvar.AssetId = novo.Id;
                    salvar.ValorAquisicao = historico.ValorAquisicao;
                    salvar.ValorRevalorizacao = historico.ValorRevalorizacao;
                    salvar.DataAlteracao = historico.DataAlteracao;
                    salvar.LoginId = UserCommon.CurrentUser().Id;
                    db.Entry(salvar).State = EntityState.Added;
                }
                db.SaveChanges();
            }

        }

        private void CopiaValorDepreciacaoDeAceite(Asset antigo, int ManagerUnitId)
        {
            if (antigo.AssetStartId != null)
            {
                MonthlyDepreciation antigaDepreciacao = (db.MonthlyDepreciations.Where(x => x.AssetStartId == antigo.AssetStartId).OrderByDescending(x => x.Id)).Take(1).FirstOrDefault();
                if (antigaDepreciacao != null) {
                    var contador = 0;

                    if (antigaDepreciacao.LifeCycle < antigaDepreciacao.CurrentMonth)
                    {
                        contador =
                               (db.MonthlyDepreciations
                               .Where(x => x.AssetStartId == antigo.AssetStartId
                                        && x.ManagerUnitId == ManagerUnitId
                                        && x.CurrentDate.Month == antigaDepreciacao.CurrentDate.Month
                                        && x.CurrentDate.Year == antigaDepreciacao.CurrentDate.Year
                                        && x.LifeCycle < x.CurrentMonth)
                               .Count());
                    }
                    else {
                        contador =
                               (db.MonthlyDepreciations
                               .Where(x => x.AssetStartId == antigo.AssetStartId
                                        && x.ManagerUnitId == ManagerUnitId
                                        && x.CurrentDate.Month == antigaDepreciacao.CurrentDate.Month
                                        && x.CurrentDate.Year == antigaDepreciacao.CurrentDate.Year)
                               .Count());
                    }

                     MonthlyDepreciation novaDepreciacao = antigaDepreciacao.Clone();
                     novaDepreciacao.ManagerUnitId = ManagerUnitId;
                     novaDepreciacao.MesAbertoDoAceite = true;
                     novaDepreciacao.QtdLinhaRepetida = contador;
                     db.Entry(novaDepreciacao).State = EntityState.Added;
                     db.SaveChanges();

                }
            }
        }

        private AssetViewModel PreencheDadosDepreciacaoPorGrupo(AssetViewModel viewmodel) {
            var materialgroup = (from a in db.MaterialGroups where a.Code == viewmodel.MaterialGroupCode select a).FirstOrDefault();
            viewmodel.LifeCycle = materialgroup.LifeCycle;
            viewmodel.RateDepreciationMonthly = materialgroup.RateDepreciationMonthly;
            viewmodel.ResidualValue = materialgroup.ResidualValue;
            return viewmodel;
        }

        private void ValidaEPreencheAnimalNaoServico(AssetViewModel viewmodel) {
            //Se BP necessita de aceite, o sistema pega o valor do BP de origem
            if (viewmodel.AssetsIdTransferencia == null || viewmodel.AssetsIdTransferencia == 0) {
                if (viewmodel.MaterialGroupCode == 88)
                {
                    viewmodel.flagAnimalNaoServico = !viewmodel.checkflagAnimalAServico;
                }
                else {
                    viewmodel.flagAnimalNaoServico = false;
                }
            }
        }

        private AssetViewModel PreencheDadosPorBPUGEOrigem(AssetViewModel viewmodel)
        {
            var AssetTransferido = (from a in db.Assets where a.Id == viewmodel.AssetsIdTransferencia select a).FirstOrDefault();
            viewmodel.LifeCycle = AssetTransferido.LifeCycle;
            viewmodel.RateDepreciationMonthly = AssetTransferido.RateDepreciationMonthly;
            viewmodel.ResidualValue = AssetTransferido.ResidualValue;
            viewmodel.AssetStartId = AssetTransferido.AssetStartId;
            viewmodel.flagAnimalNaoServico = AssetTransferido.flagAnimalNaoServico;

            return viewmodel;
        }

        #endregion

        #region DadosSIAFEM
        [HttpPost]
        public ActionResult Prosseguir(string auditorias, string LoginSiafem, string SenhaSiafem)
        {
            string msgInformeAoUsuario = "Retorno vazio";
            using (db = new SAMContext())
            {
                MovementType tipoMovPatrimonial = null;
                int numeroNLs = 1;
                string nlLiquidacao = null;
                string nlDepreciacao = null;
                string nlReclassificacao = null;
                IList<Tuple<string, string>> listaNLsGeradas = null;
                string msgErroSIAFEM = null;
                string msgNLGerada = null;
                Tuple<string, string, string, string, bool> envioComandoGeracaoNL = null;
                TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido;
                string cpfUsuarioSessaoLogada = null;
                bool exibeBotaoAbortar = false;
                bool exibeBotaoGerarPendencia = false;
                int contadorNL = 0;
                bool ehEstorno = false;
                AuditoriaIntegracao registroAuditoriaIntegracao = null;
                List<int> IdsAuditorias = null;




                if (auditorias[auditorias.Length - 1] == ',')
                    IdsAuditorias = auditorias.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
                else
                    IdsAuditorias = auditorias.Split(',').Select(Int32.Parse).ToList();

                var ListaMovimentacoes = (from r in db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                          join am in db.AssetMovements on r.AssetMovementsId equals am.Id
                                          where IdsAuditorias.Contains(r.AuditoriaIntegracaoId)
                                          select am).ToList();

                var movementTypeId = ListaMovimentacoes.FirstOrDefault().MovementTypeId;
                var NumberDoc = ListaMovimentacoes.FirstOrDefault().NumberDoc;


                tipoMovPatrimonial = db.MovementTypes.Where(movPatrimonial => movPatrimonial.Id == movementTypeId).FirstOrDefault();
                IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracao => auditoriaIntegracao != 0).ToList();
                if(IdsAuditorias.HasElements())
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
                        registroAuditoriaIntegracao = registroAuditoriaIntegracao = obterEntidadeAuditoriaIntegracao(auditoriaIntegracaoId);
                        ehEstorno = (registroAuditoriaIntegracao.NLEstorno.ToUpperInvariant() == "S");

                        #region NL Liquidacao
                        //NL Liquidacao
                        if (tipoMovPatrimonial.ContraPartidaContabilLiquida() && tipoNotaSIAFEM == TipoNotaSIAF.NL_Liquidacao)
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
                                msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                break; //interrompe o loop
                            }
                        }
                        #endregion NL Liquidacao


                        #region NL Depreciacao
                        //NL Depreciacao
                        ++contadorNL;
                        var tipoMovimentacaoNaoFazLiquidacao = (String.IsNullOrWhiteSpace(nlLiquidacao) && !tipoMovPatrimonial.ContraPartidaContabilLiquida());
                        var temNL_Liquidacao = !String.IsNullOrWhiteSpace(nlLiquidacao);

                        if ((((numeroNLs > 1) && temNL_Liquidacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                            tipoMovPatrimonial.ContraPartidaContabilDeprecia()) //...e deprecia?
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
                                msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                break; //interrompe o loop
                            }
                        }
                        #endregion NL Depreciacao


                        #region NL Reclassificacao
                        //NL Reclassificacao
                        ++contadorNL;
                        var temNL_Depreciacao = !String.IsNullOrWhiteSpace(nlDepreciacao);
                        if ((((numeroNLs > 1) && temNL_Depreciacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                            tipoMovPatrimonial.ContraPartidaContabilReclassifica()) //...e reclassifica?
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
                                msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                break; //interrompe o loop
                            }
                        }
                        #endregion NL Reclassificacao
                    }
                }

                //formatacao mensagem de erro
                msgErroSIAFEM = ((!String.IsNullOrWhiteSpace(msgErroSIAFEM)) ? String.Format("Erro retornado pela integração SAM/Contabiliza-SP: {0}", msgErroSIAFEM) : null);

                //formatacao mensagem de exibicao de NL's
                if (listaNLsGeradas.Count() == 1)
                {
                    msgNLGerada = String.Format("NL retornada pela integração SAM/Contabiliza-SP: {0} {1}", listaNLsGeradas[0].Item2, listaNLsGeradas[0].Item1);
                }
                else if (listaNLsGeradas.Count() > 1)
                {
                    bool jahTemNL = false;
                    foreach (var nLGerada in listaNLsGeradas)
                    {
                        jahTemNL = !String.IsNullOrWhiteSpace(msgNLGerada);
                        msgNLGerada += String.Format("{0}{1} {2}", (jahTemNL? " e " : null), nLGerada.Item2, nLGerada.Item1);
                    }

                    msgNLGerada = String.Format("NLs retornadas pela integração SAM/Contabiliza-SP: {0}", msgNLGerada);
                }

                DadosSIAFEMValidacaoMovimentacoes praTela = new DadosSIAFEMValidacaoMovimentacoes();

                //sucesso OU erro?
                msgInformeAoUsuario = (!String.IsNullOrWhiteSpace(msgNLGerada) ? msgNLGerada : msgErroSIAFEM);
                //sucesso E erro?
                if (!String.IsNullOrWhiteSpace(msgNLGerada) && !String.IsNullOrWhiteSpace(msgErroSIAFEM))
                    msgInformeAoUsuario = String.Format("Retorno de processamento:\n1) {0}\n2) {1}", msgNLGerada, msgErroSIAFEM);

                //praTela.mensagem = (!String.IsNullOrWhiteSpace(msgNLGerada) ? msgNLGerada : msgErroSIAFEM);
                praTela.mensagem = msgInformeAoUsuario;
                praTela.mensagemDeSuccesso = (!String.IsNullOrWhiteSpace(msgNLGerada) ? true : false);

                exibeBotaoAbortar = (contadorNL < numeroNLs); //se na primeira jah parar (isso tendo gerado dois XMLs)
                exibeBotaoGerarPendencia = (contadorNL == numeroNLs); //se a ultima NL das possiveis nao for gerada
                praTela.primeiraNLGeraPendecia = exibeBotaoAbortar;


                return PartialView("_RetornoMovimentoSIAFEM", praTela);
            }
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
                    case "SAÍDA":           { tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao; } break;
                    case "DEPRECIACAO":
                    case "DEPRECIAÇÃO":     { tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao; } break;
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
                string name = "AssetsController.obterMovimentacoesPatrimoniaisVinculadasRegistroAuditoria";
                GravaLogErro(messageErro, stackTrace, name);
                throw excErroRuntime;
            }

            return listaMovPatrimoniais;
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

        [HttpPost]
        public void GerarPendenciaSIAFEM(string auditorias, string LoginSiafem, string SenhaSiafem)
        {
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            List<int> IdsAuditorias = null;


            if (auditorias[auditorias.Length - 1] == ',')
                IdsAuditorias = auditorias.RemoveUltimoCaracter().Split(',').Select(Int32.Parse).ToList();
            else
                IdsAuditorias = auditorias.Split(',').Select(Int32.Parse).ToList();

            IdsAuditorias = IdsAuditorias.Where(auditoriaIntegracaoId => auditoriaIntegracaoId != 0).ToList();
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

        #region Item Material Edicao
      
        public JsonResult BuscaItemMaterialParaAtualizacao(int numero, string item) {
            try
            {
                int itemComoInteiro;
                if (!int.TryParse(item, out itemComoInteiro))
                {
                    return Json(new { erro = true, mensagem = "Por favor, realize a procura do item material com um número" }, JsonRequestBehavior.AllowGet);
                }

                MaterialItemViewModel materialItemViewModel;
                Asset objAsset = null;

                using (db = new SAMContext())
                {
                    objAsset = db.Assets
                                 .Where(a => a.Id == numero && a.Status)
                                 .Select(a => new
                                 {
                                     MaterialGroupCode = a.MaterialGroupCode,
                                     MaterialItemCode = a.MaterialItemCode,
                                     MaterialItemDescription = a.MaterialItemDescription,
                                     flagAcervo = a.flagAcervo,
                                     flagTerceiro = a.flagTerceiro
                                 })
                                 .ToList()
                                 .Select(a => new Asset
                                 {
                                     MaterialGroupCode = a.MaterialGroupCode,
                                     MaterialItemCode = a.MaterialItemCode,
                                     MaterialItemDescription = a.MaterialItemDescription,
                                     flagAcervo = a.flagAcervo,
                                     flagTerceiro = a.flagTerceiro
                                 }).FirstOrDefault();

                    if (objAsset == null)
                        return Json(new { erro = true, mensagem = "BP ativo não foi encontrado." }, JsonRequestBehavior.AllowGet);

                    if (itemComoInteiro == objAsset.MaterialItemCode)
                        return Json(new { codigo = objAsset.MaterialItemCode, descricao = objAsset.MaterialItemDescription }, JsonRequestBehavior.AllowGet);

                    //Acervos e terceiros já possuem item material fixo, não podendo serem alterados
                    if (objAsset.flagAcervo == true || objAsset.flagTerceiro == true)
                        return Json(new { erro = true, mensagem = "BP do tipo acervo e terceiro tem código de item material fixo, conforme novas regras contábeis" }, JsonRequestBehavior.AllowGet);

                    BuscaItensMateriais buscaItensMateriais = new BuscaItensMateriais();
                    string msgErroSIAFISICO = buscaItensMateriais.BuscaMensagemValidaCodigoItemMaterial(item);

                    if (!string.IsNullOrEmpty(msgErroSIAFISICO) && !string.IsNullOrWhiteSpace(msgErroSIAFISICO))
                    {
                        materialItemViewModel = buscaItensMateriais.GetItemMaterial(item, ref msgErroSIAFISICO);

                        if (materialItemViewModel == null)
                            return Json(new { erro = true, mensagem = "Item Material não encontrado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        materialItemViewModel = buscaItensMateriais.GetItemMaterialBD(item);
                    }

                    if (materialItemViewModel.MaterialGroupCode != objAsset.MaterialGroupCode)
                        return Json(new { erro = true, mensagem = "Item Material não pertence ao mesmo grupo de material" }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { codigo = materialItemViewModel.MaterialItemCode, descricao = materialItemViewModel.Description }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool ValidaAlteracaoDeItemMaterial(Asset objAsset, int itemMaterial)
        {
            if (!PermitidoAlterarItemMaterial(objAsset))
                return false;

            MaterialItemViewModel materialItemViewModel;
            string item = itemMaterial.ToString();
            BuscaItensMateriais buscaItensMateriais = new BuscaItensMateriais();
            string msgErroSIAFISICO = buscaItensMateriais.BuscaMensagemValidaCodigoItemMaterial(item);


            if (!string.IsNullOrEmpty(msgErroSIAFISICO) && !string.IsNullOrWhiteSpace(msgErroSIAFISICO))
            {
                materialItemViewModel = buscaItensMateriais.GetItemMaterial(item, ref msgErroSIAFISICO);

                if (materialItemViewModel == null)
                    return false;
            }
            else
            {
                materialItemViewModel = buscaItensMateriais.GetItemMaterialBD(item);
            }


            return materialItemViewModel.MaterialGroupCode == objAsset.MaterialGroupCode;
        }

        private bool PermitidoAlterarItemMaterial(Asset objAsset)
        {
            return objAsset.Status && objAsset.flagAcervo != true && objAsset.flagTerceiro != true && BPNaoSeTratarDeAceite(objAsset.Id);
        }

        private void AlteraItemMaterialDaDepreciacaoDoBP(int? AssetStartId, int MaterialItemCode)
        {
            if (AssetStartId != null)
            {
                db.Database.ExecuteSqlCommand("update MonthlyDepreciation set MaterialItemCode = {0} where AssetStartId = {1}", MaterialItemCode, AssetStartId);
                db.SalvaSemNecessiadeDeGravarLog();
            }
        }
        #endregion
    }
}