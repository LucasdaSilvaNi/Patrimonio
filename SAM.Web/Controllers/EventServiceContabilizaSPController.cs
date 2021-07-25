using AutoMapper;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Transactions;
using Sam.Common.Util;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using Sam.Common.Enums;
using Sam.Integracao.SIAF.Mensagem.Interface;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using Sam.Integracao.SIAF.Mensagem.Enum;

namespace SAM.Web.Controllers
{
    public class EventServiceContabilizaSPController : BaseController
    {
        private SAMContext db = new SAMContext();
        //private RelationshipUserProfile rup;
        //private Hierarquia hierarquia = new Hierarquia();

        //private int _institutionId;
        //private int? _budgetUnitId;
        //private int? _managerUnitId;

        //public void getHierarquiaPerfil()
        //{
        //    User u = UserCommon.CurrentUser();
        //    rup = UserCommon.CurrentRelationshipUsersProfile(u.Id);
        //    var perflLogado = UserCommon.CurrentProfileLogin(rup.Id);
        //    _institutionId = perflLogado.InstitutionId;
        //    _budgetUnitId = perflLogado.BudgetUnitId;
        //    _managerUnitId = perflLogado.ManagerUnitId;
        //}

        #region Actions Methods
        #region Index Actions

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.TemPermissao = PerfilAdmGeral();

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(EventServiceContabilizaSP eventServiceContabilizaSP)
        {
            string draw = Request.Form["draw"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            int codigoTipoMovimento = -1;
            int codigoTipoMovimentacao = -1;
            MovementType tipoMovPatrimonial_SAM = null;
            MovementType tipoMovPatrimonial_ContabilizaSP = null;
            IQueryable<EventServiceContabilizaSP> qryRetorno = null;
            int[] agrupamentosTiposMovimentacao = null;
            int[] tiposMovimentacaoContabilizaSP = null;




            try
            {

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    qryRetorno = db.EventServicesContabilizaSPs
                                   .Include("RelatedMetaDataType_StockDestination")
                                   .Include("RelatedMetaDataType_StockSource")
                                   .Include("RelatedMetaDataType_AccountingValueField")
                                   .Include("RelatedMetaDataType_DateField")
                                   .Where(_eventServiceContabilizaSP => _eventServiceContabilizaSP.Status == true)
                                   .Where(_eventServiceContabilizaSP => _eventServiceContabilizaSP.RelatedMovementType.Code.ToString().Contains(currentFilter) ||
                                                                        _eventServiceContabilizaSP.RelatedMovementType.Description.Contains(currentFilter) ||
                                                                        _eventServiceContabilizaSP.RelatedMetaDataType_StockDestination.ToString().Contains(currentFilter) ||
                                                                        _eventServiceContabilizaSP.RelatedMetaDataType_StockSource.ToString().Contains(currentFilter) ||
                                                                        _eventServiceContabilizaSP.RelatedMetaDataType_AccountingValueField.ToString().Contains(currentFilter) ||
                                                                        _eventServiceContabilizaSP.RelatedMetaDataType_DateField.ToString().Contains(currentFilter)
                                         )
                                   .AsNoTracking();
                else
                    qryRetorno = db.EventServicesContabilizaSPs
                                   .Include("RelatedMovementType")
                                   .Where(_eventServiceContabilizaSP => _eventServiceContabilizaSP.Status == true)
                                   .AsNoTracking();

                int totalRegistros = await qryRetorno.CountAsync();
                var result = await qryRetorno.OrderBy(s => new { s.TipoMovimento_SamPatrimonio })
                                             .ThenBy(s => new { s.AccountEntryTypeId })
                                             .Skip(startRec).Take(length)
                                             .ToListAsync();


                agrupamentosTiposMovimentacao = new int[] { (int)EnumGroupMoviment.Incorporacao, (int)EnumGroupMoviment.Movimentacao, (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };
                tiposMovimentacaoContabilizaSP = new int[] { (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };
                result.ToList().ForEach(_eventServiceContabilizaSP => {
                                                                        _eventServiceContabilizaSP.MovimentacaoPatrimonialDepreciaOuReclassifica = _eventServiceContabilizaSP.RelatedMovementType.ContraPartidaContabilDepreciaOuReclassifica();
                                                                        _eventServiceContabilizaSP.AccountEntryType                              = (AccountEntryType.FromValue(_eventServiceContabilizaSP.AccountEntryTypeId)).Name;

                                                                        _eventServiceContabilizaSP.MetaDataType_AccountingValueFieldDescription  = (MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_AccountingValueField)).Name;
                                                                        _eventServiceContabilizaSP.MetaDataType_DateFieldDescription             = (MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_DateField)).Name;

                                                                        //_eventServiceContabilizaSP.MetaDataType_StockDestinationDescription     = ((_eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull()) ? (MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name : String.Empty);
                                                                        if (_eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull())
                                                                         if (_eventServiceContabilizaSP.MetaDataType_StockDestination == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                                                                            _eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = _eventServiceContabilizaSP.StockDestination;
                                                                         //else if (_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                                                                         //   _eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                                                                         else
                                                                              _eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name;

                                                                        if (_eventServiceContabilizaSP.MetaDataType_StockSource.IsNotNull())
                                                                         if (_eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                                                                            _eventServiceContabilizaSP.MetaDataType_StockSourceDescription = _eventServiceContabilizaSP.StockSource;
                                                                         //else if (_eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                                                                         //   _eventServiceContabilizaSP.MetaDataType_StockSourceDescription = MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_StockSource.Value).Name;
                                                                         else
                                                                            _eventServiceContabilizaSP.MetaDataType_StockSourceDescription = MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_StockSource.Value).Name;


                                                                        tipoMovPatrimonial_SAM                                                  = (Int32.TryParse(_eventServiceContabilizaSP.TipoMovimento_SamPatrimonio, out codigoTipoMovimento) ? (db.MovementTypes.Where(tipoMovimentacaoPatrimonial => agrupamentosTiposMovimentacao.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId) && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimento).FirstOrDefault()) : null);
                                                                        tipoMovPatrimonial_ContabilizaSP                                        = (Int32.TryParse(_eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP, out codigoTipoMovimentacao) ? (db.MovementTypes.Where(tipoMovimentacaoPatrimonial => tipoMovimentacaoPatrimonial.GroupMovimentId == _eventServiceContabilizaSP.AccountEntryTypeId && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimentacao).FirstOrDefault()) : null);

                                                                        _eventServiceContabilizaSP.DescricaoTipoMovimento_SamPatrimonio         = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                                                                        _eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP      = (tipoMovPatrimonial_ContabilizaSP.IsNotNull() ? tipoMovPatrimonial_ContabilizaSP.Description : "");
                                                                        _eventServiceContabilizaSP.InputOutputReclassificationDepreciationType  = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");

                                                                        //_eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP      = (tipoMovPatrimonial_ContabilizaSP.IsNotNull() ? tipoMovPatrimonial_ContabilizaSP.Description : (_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.IsNotNull() ? (MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value)).Name : String.Empty));
                                                                        //if (tipoMovPatrimonial_SAM.IsNotNull() && tiposMovimentacaoContabilizaSP.Contains(tipoMovPatrimonial_SAM.GroupMovimentId))
                                                                        if (tipoMovPatrimonial_SAM.IsNotNull() && tiposMovimentacaoContabilizaSP.Contains(_eventServiceContabilizaSP.RelatedMovementType.GroupMovimentId))
                                                                        {
                                                                            _eventServiceContabilizaSP.InputOutputReclassificationDepreciationType = (_eventServiceContabilizaSP.RelatedMovementType.IsNotNull() ? _eventServiceContabilizaSP.RelatedMovementType.Description : "");
                                                                        //    _eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = "";
                                                                        }
                                                                        //else if (tipoMovPatrimonial_ContabilizaSP.IsNull())
                                                                        //else if (tipoMovPatrimonial_SAM.IsNotNull() && tipoMovPatrimonial_ContabilizaSP.IsNull())
                                                                        if (tipoMovPatrimonial_SAM.IsNotNull() && tipoMovPatrimonial_ContabilizaSP.IsNull())
                                                                            if (_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.IsNotNull())
                                                                        {
                                                                            if (_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                                                                                _eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = _eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP;
                                                                            else if (_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.Sam_MovementTypeId.Value)
                                                                                _eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                                                                                else
                                                                                _eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                                                                        }

                                                                        codigoTipoMovimentacao = -1;
                                                                        codigoTipoMovimento = -1;
                                                                        tipoMovPatrimonial_SAM = null;
                                                                        tipoMovPatrimonial_ContabilizaSP = null;
                });

                var eventServiceContabilizaSPResult = result.ConvertAll(new Converter<EventServiceContabilizaSP, EventServiceContabilizaSPViewModel>(new EventServiceContabilizaSPViewModel().Create));
                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = eventServiceContabilizaSPResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Details Actions

        public ActionResult Details(int? id)
        {
            

            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                EventServiceContabilizaSP eventServiceContabilizaSP = db.EventServicesContabilizaSPs
                                                                        .Include("RelatedMovementType")
                                                                        .Where(_eventServiceContabilizaSP => _eventServiceContabilizaSP.Id == id)
                                                                        .FirstOrDefault();
                if (eventServiceContabilizaSP.IsNotNull())
                {
                    int codigoTipoMovimento = -1;
                    int codigoTipoMovimentacao = -1;
                    MovementType tipoMovPatrimonial_SAM = null;
                    MovementType tipoMovPatrimonial_ContabilizaSP = null;
                    int[] agrupamentosTiposMovimentacao = new int[] { (int)EnumGroupMoviment.Incorporacao, (int)EnumGroupMoviment.Movimentacao, (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };
                    int[] tiposMovimentacaoContabilizaSP = new int[] { (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };

                    {
                        eventServiceContabilizaSP.MovimentacaoPatrimonialDepreciaOuReclassifica = eventServiceContabilizaSP.RelatedMovementType.ContraPartidaContabilDepreciaOuReclassifica();
                        eventServiceContabilizaSP.AccountEntryType = (AccountEntryType.FromValue(eventServiceContabilizaSP.AccountEntryTypeId)).Name;

                        eventServiceContabilizaSP.MetaDataType_AccountingValueFieldDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_AccountingValueField)).Name;
                        eventServiceContabilizaSP.MetaDataType_DateFieldDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_DateField)).Name;

                        //eventServiceContabilizaSP.MetaDataType_StockDestinationDescription     = ((eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull()) ? (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name : String.Empty);
                        if (eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull())
                            if (eventServiceContabilizaSP.MetaDataType_StockDestination == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                                eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = eventServiceContabilizaSP.StockDestination;
                            else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                                eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                            else
                                eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name;

                        if (eventServiceContabilizaSP.MetaDataType_StockSource.IsNotNull())
                            if (eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                                eventServiceContabilizaSP.MetaDataType_StockSourceDescription = eventServiceContabilizaSP.StockSource;
                            else if (eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                                eventServiceContabilizaSP.MetaDataType_StockSourceDescription = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockSource.Value).Name;



                        tipoMovPatrimonial_SAM = (Int32.TryParse(eventServiceContabilizaSP.TipoMovimento_SamPatrimonio, out codigoTipoMovimento) ? (db.MovementTypes.Where(tipoMovimentacaoPatrimonial => agrupamentosTiposMovimentacao.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId) && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimento).FirstOrDefault()) : null);
                        tipoMovPatrimonial_ContabilizaSP = (Int32.TryParse(eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP, out codigoTipoMovimentacao) ? (db.MovementTypes.Where(tipoMovimentacaoPatrimonial => tipoMovimentacaoPatrimonial.GroupMovimentId == eventServiceContabilizaSP.AccountEntryTypeId && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimentacao).FirstOrDefault()) : null);

                        eventServiceContabilizaSP.DescricaoTipoMovimento_SamPatrimonio = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                        eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_ContabilizaSP.IsNotNull() ? tipoMovPatrimonial_ContabilizaSP.Description : "");
                        eventServiceContabilizaSP.InputOutputReclassificationDepreciationType = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");

                        if (tipoMovPatrimonial_SAM.IsNotNull() && tiposMovimentacaoContabilizaSP.Contains(eventServiceContabilizaSP.RelatedMovementType.GroupMovimentId))
                        {
                            eventServiceContabilizaSP.InputOutputReclassificationDepreciationType = (eventServiceContabilizaSP.RelatedMovementType.IsNotNull() ? eventServiceContabilizaSP.RelatedMovementType.Description : "");
                            eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = "";
                        }
                        else if (tipoMovPatrimonial_SAM.IsNotNull())// && tipoMovPatrimonial_ContabilizaSP.IsNull())
                        {
                            if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                                eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP;
                            else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.Sam_MovementTypeId.Value)
                                eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                            else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                                eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                        }
                    }
                }
                else if (eventServiceContabilizaSP.IsNull())
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                //if (rup.RelatedProfile.flagPerfilMaster == true)
                return View(eventServiceContabilizaSP);

                //if (UserCommon.ValidarRequisicao(administrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId, administrativeUnit.RelatedManagerUnit.BudgetUnitId, administrativeUnit.ManagerUnitId, administrativeUnit.Id, null))
                //    return View(administrativeUnit);
                //else
                //    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions

        public ActionResult Create()
        {
            try
            {
                CarregaCombos(null);
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventServiceContabilizaSPViewModel model)
        {
            try
            {
                ModelState.Clear();

                //Tipo Agrupamento (MetaDado)
                if (model.AccountEntryTypeId == 0)
                    ModelState.AddModelError("AccountEntryTypeId", "O campo Tipo de Agrupamento é obrigatório");

                //Tipo Movimentação Contabiliza
                if (model.CodigoTipoMovimento_ContabilizaSP == 0)
                    ModelState.AddModelError("CodigoTipoMovimento_ContabilizaSP", "O campo Tipo Movimento (Contabiliza) é obrigatório");
                else
                {
                    if (model.CodigoTipoMovimento_ContabilizaSP == 9
                    && (string.IsNullOrEmpty(model.DescricaoTipoMovimentacao_ContabilizaSP) || (string.IsNullOrWhiteSpace(model.DescricaoTipoMovimentacao_ContabilizaSP)))
                   ) ModelState.AddModelError("DescricaoTipoMovimentacao_ContabilizaSP", "A informação textual do Tipo Movimento (Contabiliza) é obrigatório");
                }

                //Estoque Origem
                if (model.CodigoEstoque_Origem == 9
                    && (string.IsNullOrEmpty(model.MetaDataType_StockSourceDescription) || (string.IsNullOrWhiteSpace(model.MetaDataType_StockSourceDescription)))
                   ) ModelState.AddModelError("MetaDataType_StockSourceDescription", "A informação textual do Estoque Origem (MetaDado) é obrigatório");

                //Estoque Destino
                if (model.CodigoEstoque_Destino == 9
                    && (string.IsNullOrEmpty(model.MetaDataType_StockDestinationDescription) || (string.IsNullOrWhiteSpace(model.MetaDataType_StockDestinationDescription)))
                   ) ModelState.AddModelError("MetaDataType_StockDestinationDescription", "A informação textual do Estoque Destino (MetaDado) é obrigatório");


                //Data Movimentação (MetaDado)
                if (model.MetaDataType_DateField == 0)
                    ModelState.AddModelError("MetaDataType_DateField", "O campo Data de Movimentação (MetaDado) é obrigatório");

                //Valor Movimentação (MetaDado)
                if (model.MetaDataType_AccountingValueField == 0)
                    ModelState.AddModelError("MetaDataType_AccountingValueField", "O campo Valor de Movimentação (MetaDado) é obrigatório");

                //Controle Específico
                if (model.CodigoCE == 9
                    && (string.IsNullOrEmpty(model.SpecificControl) || (string.IsNullOrWhiteSpace(model.SpecificControl)))
                   ) ModelState.AddModelError("SpecificControl", "A informação textual do Controle Específico (MetaDado) é obrigatório");


                //Controle Específico Entrada
                if (model.CodigoCE_Entrada == 9
                    && (string.IsNullOrEmpty(model.SpecificInputControl) || (string.IsNullOrWhiteSpace(model.SpecificInputControl)))
                   ) ModelState.AddModelError("SpecificInputControl", "A informação textual do Controle Específico de entrada (MetaDado) é obrigatório");


                //Controle Específico Saída
                if (model.CodigoCE_Saida == 9
                    && (string.IsNullOrEmpty(model.SpecificOutputControl) || (string.IsNullOrWhiteSpace(model.SpecificOutputControl)))
                   ) ModelState.AddModelError("SpecificOutputControl", "A informação textual do Controle Específico de saída (MetaDado) é obrigatório");


                //Tipo Movimentação Patrimonio
                if (model.CodigoTipoMovimento_SamPatrimonio == 0)
                    ModelState.AddModelError("CodigoTipoMovimento_SamPatrimonio", "O campo Tipo Movimento (SAM) é obrigatório");
                else
                {
                    if (!(from m in db.MovementTypes where m.Id == model.CodigoTipoMovimento_SamPatrimonio select m.Id).Any())
                        ModelState.AddModelError("CodigoTipoMovimento_SamPatrimonio", "Campo com valor inválido");
                }

                //Tipo Entrada/ Saída / Reclassificação / Depreciação (ContabilizaSP)
                if (model.InputOutputReclassificationDepreciationTypeCode == 0)
                    ModelState.AddModelError("InputOutputReclassificationDepreciationTypeCode", "O campo Tipo de Movimento (SAM) é obrigatório");
                else
                {
                    if (!(from m in db.MovementTypes where m.Id == model.InputOutputReclassificationDepreciationTypeCode select m.Id).Any())
                        ModelState.AddModelError("InputOutputReclassificationDepreciationTypeCode", "Campo com valor inválido");
                }

                if (ModelState.IsValid) {
                    EventServiceContabilizaSP eventService = new EventServiceContabilizaSP();

                    //valores padrão
                    eventService.StockDescription = "ESTOQUE LONGO PRAZO";
                    eventService.StockType = "PERMANENTE";
                    eventService.MaterialType = "MaterialPermanente";
                    eventService.Status = true;

                    //Tipo Movimento (SAM)
                    eventService.TipoMovimento_SamPatrimonio = model.CodigoTipoMovimento_SamPatrimonio.ToString();

                    //Tipo Grupo
                    eventService.AccountEntryTypeId = model.AccountEntryTypeId;

                    //Tipo Movimento (Contabiliza)
                    eventService.MetaDataType_MovementTypeContabilizaSP = model.CodigoTipoMovimento_ContabilizaSP;

                    if (model.CodigoTipoMovimento_ContabilizaSP == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                        eventService.TipoMovimentacao_ContabilizaSP = model.DescricaoTipoMovimentacao_ContabilizaSP;
                    else
                        eventService.TipoMovimentacao_ContabilizaSP = string.Empty;

                    //Tipo Entrada/Saída/Reclassificação/Depreciação
                    eventService.InputOutputReclassificationDepreciationTypeCode = model.InputOutputReclassificationDepreciationTypeCode;

                    //Estoque Origem
                    eventService.StockSource = null;
                    if (model.CodigoEstoque_Origem != 0)
                    {
                        eventService.MetaDataType_StockSource = model.CodigoEstoque_Origem;
                        if (model.CodigoEstoque_Origem == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.StockSource = model.MetaDataType_StockSourceDescription;
                    }
                    else
                        eventService.MetaDataType_StockSource = null;

                    //Estoque Destino
                    eventService.StockDestination = null;
                    if (model.CodigoEstoque_Destino != 0)
                    {
                        eventService.MetaDataType_StockDestination = model.CodigoEstoque_Destino;
                        if (model.CodigoEstoque_Destino == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.StockDestination = model.MetaDataType_StockDestinationDescription;
                    }
                    else
                        eventService.MetaDataType_StockDestination = null;

                    //Data Movimentação (Metadado)
                    eventService.MetaDataType_DateField = model.MetaDataType_DateField;

                    //Valor Movimentação (Metadado)
                    eventService.MetaDataType_AccountingValueField = model.MetaDataType_AccountingValueField;

                    //Controle Específico
                    eventService.SpecificControl = string.Empty;
                    if (model.CodigoCE != 0)
                    {
                        eventService.MetaDataType_SpecificControl = model.CodigoCE;
                        if (model.CodigoCE == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.SpecificControl = model.SpecificControl;
                    }
                    else
                        eventService.MetaDataType_SpecificControl = null;

                    //Controle Específico de Entrada
                    eventService.SpecificInputControl = string.Empty;
                    if (model.CodigoCE_Entrada != 0)
                    {
                        eventService.MetaDataType_SpecificInputControl = model.CodigoCE_Entrada;
                        if (model.CodigoCE_Entrada == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.SpecificInputControl = model.SpecificInputControl;
                    }
                    else
                        eventService.MetaDataType_SpecificInputControl = null;


                    //Controle Específico de Saída
                    eventService.SpecificOutputControl = string.Empty;
                    if (model.CodigoCE_Saida != 0)
                    {
                        eventService.MetaDataType_SpecificOutputControl = model.CodigoCE_Saida;
                        if (model.CodigoCE_Saida == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.SpecificOutputControl = model.SpecificOutputControl;
                    }
                    else
                        eventService.MetaDataType_SpecificOutputControl = null;

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        eventService.DataAtivacaoTipoMovimentacao = DateTime.Now;
                        db.Entry(eventService).State = EntityState.Added;
                        db.SaveChanges();
                        transaction.Complete();
                    }

                    return RedirectToAction("Index");
                }


                CarregaCombos(model);
                return View(model);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions

        public ActionResult Edit(int? id)
        {
            try
            {
                if (PerfilAdmGeral())
                {
                    if (id == null)
                        return MensagemErro(CommonMensagens.IdentificadorNulo);

                    EventServiceContabilizaSP eventServiceContabilizaSP = db.EventServicesContabilizaSPs
                                                                            .Where(_eventServiceContabilizaSP => _eventServiceContabilizaSP.Id == id)
                                                                            .FirstOrDefault();

                    if (eventServiceContabilizaSP == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    //Model para ViewModel
                    eventServiceContabilizaSP.MovimentacaoPatrimonialDepreciaOuReclassifica = eventServiceContabilizaSP.RelatedMovementType.ContraPartidaContabilDepreciaOuReclassifica();

                    if (eventServiceContabilizaSP.MetaDataType_StockSource.IsNotNull())
                        if (eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventServiceContabilizaSP.MetaDataType_StockSourceDescription = eventServiceContabilizaSP.StockSource;
                        else if (eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                            eventServiceContabilizaSP.MetaDataType_StockSourceDescription = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockSource.Value).Name;

                    EventServiceContabilizaSPViewModel model = new EventServiceContabilizaSPViewModel();
                    model = model.Create(eventServiceContabilizaSP);
                    model.CodigoTipoMovimento_SamPatrimonio = Convert.ToInt32(eventServiceContabilizaSP.TipoMovimento_SamPatrimonio);
                    

                    int codigoTipoMovimento;
                    int[] agrupamentosTiposMovimentacao = new int[] { (int)EnumGroupMoviment.Incorporacao, (int)EnumGroupMoviment.Movimentacao, (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };
                    MovementType tipoMovPatrimonial_SAM = (Int32.TryParse(eventServiceContabilizaSP.TipoMovimento_SamPatrimonio, out codigoTipoMovimento) ? (db.MovementTypes.Where(tipoMovimentacaoPatrimonial => agrupamentosTiposMovimentacao.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId) && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimento).FirstOrDefault()) : null);

                    if (eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull())
                        if (eventServiceContabilizaSP.MetaDataType_StockDestination == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            model.MetaDataType_StockDestinationDescription = eventServiceContabilizaSP.StockDestination;
                        else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                            model.MetaDataType_StockDestinationDescription = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                        else
                            model.MetaDataType_StockDestinationDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name;

                    if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                        model.DescricaoTipoMovimentacao_ContabilizaSP = eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP;
                    else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.Sam_MovementTypeId.Value)
                        model.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                    else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                        model.DescricaoTipoMovimentacao_ContabilizaSP = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;


                    CarregaCombos(model);

                    return View(model);
                }
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventServiceContabilizaSPViewModel model)
        {
            try
            {
                ModelState.Clear();

                //Tipo Movimentação Contabiliza
                if (model.CodigoTipoMovimento_ContabilizaSP == 0)
                    ModelState.AddModelError("CodigoTipoMovimento_ContabilizaSP", "O campo Tipo Movimento (Contabiliza) é obrigatório");
                else
                {
                    if (model.CodigoTipoMovimento_ContabilizaSP == 9
                    && (string.IsNullOrEmpty(model.DescricaoTipoMovimentacao_ContabilizaSP) || (string.IsNullOrWhiteSpace(model.DescricaoTipoMovimentacao_ContabilizaSP)))
                   ) ModelState.AddModelError("DescricaoTipoMovimentacao_ContabilizaSP", "A informação textual do Tipo Movimento (Contabiliza) é obrigatório");
                }

                //Estoque Origem
                if (model.CodigoEstoque_Origem == 9
                    && (string.IsNullOrEmpty(model.MetaDataType_StockSourceDescription) || (string.IsNullOrWhiteSpace(model.MetaDataType_StockSourceDescription)))
                   ) ModelState.AddModelError("MetaDataType_StockSourceDescription", "A informação textual do Estoque Origem (MetaDado) é obrigatório");

                //Estoque Destino
                if (model.CodigoEstoque_Destino == 9
                    && (string.IsNullOrEmpty(model.MetaDataType_StockDestinationDescription) || (string.IsNullOrWhiteSpace(model.MetaDataType_StockDestinationDescription)))
                   ) ModelState.AddModelError("MetaDataType_StockDestinationDescription", "A informação textual do Estoque Destino (MetaDado) é obrigatório");


                //Data Movimentação (MetaDado)
                if (model.MetaDataType_DateField == 0)
                    ModelState.AddModelError("MetaDataType_DateField", "O campo Data de Movimentação (MetaDado) é obrigatório");

                //Valor Movimentação (MetaDado)
                if (model.MetaDataType_AccountingValueField == 0)
                    ModelState.AddModelError("MetaDataType_AccountingValueField", "O campo Valor de Movimentação (MetaDado) é obrigatório");

                //Controle Específico
                if (model.CodigoCE == 9
                    && (string.IsNullOrEmpty(model.SpecificControl) || (string.IsNullOrWhiteSpace(model.SpecificControl)))
                   ) ModelState.AddModelError("SpecificControl", "A informação textual do Controle Específico (MetaDado) é obrigatório");


                //Controle Específico Entrada
                if (model.CodigoCE_Entrada == 9
                    && (string.IsNullOrEmpty(model.SpecificInputControl) || (string.IsNullOrWhiteSpace(model.SpecificInputControl)))
                   ) ModelState.AddModelError("SpecificInputControl", "A informação textual do Controle Específico de entrada (MetaDado) é obrigatório");


                //Controle Específico Saída
                if (model.CodigoCE_Saida == 9
                    && (string.IsNullOrEmpty(model.SpecificOutputControl) || (string.IsNullOrWhiteSpace(model.SpecificOutputControl)))
                   ) ModelState.AddModelError("SpecificOutputControl", "A informação textual do Controle Específico de saída (MetaDado) é obrigatório");

                if (ModelState.IsValid) {

                    EventServiceContabilizaSP eventService = db.EventServicesContabilizaSPs.Find(model.Id);

                    if (eventService == null) 
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    //Tipo Movimento (Contabiliza)
                    eventService.MetaDataType_MovementTypeContabilizaSP = model.CodigoTipoMovimento_ContabilizaSP;

                    if (model.CodigoTipoMovimento_ContabilizaSP == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                        eventService.TipoMovimentacao_ContabilizaSP = model.DescricaoTipoMovimentacao_ContabilizaSP;
                    else
                        eventService.TipoMovimentacao_ContabilizaSP = string.Empty;

                    //Estoque Origem
                    eventService.StockSource = null;
                    if (model.CodigoEstoque_Origem != 0)
                    {
                        eventService.MetaDataType_StockSource = model.CodigoEstoque_Origem;
                        if (model.CodigoEstoque_Origem == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.StockSource = model.MetaDataType_StockSourceDescription;
                    } else
                        eventService.MetaDataType_StockSource = null;

                    //Estoque Destino
                    eventService.StockDestination = null;
                    if (model.CodigoEstoque_Destino != 0)
                    {
                        eventService.MetaDataType_StockDestination = model.CodigoEstoque_Destino;
                        if (model.CodigoEstoque_Destino == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.StockDestination = model.MetaDataType_StockDestinationDescription;
                    }
                    else
                        eventService.MetaDataType_StockDestination = null;

                    //Data Movimentação (Metadado)
                    eventService.MetaDataType_DateField = model.MetaDataType_DateField;

                    //Valor Movimentação (Metadado)
                    eventService.MetaDataType_DateField = model.MetaDataType_DateField;

                    //Controle Específico
                    eventService.SpecificControl = string.Empty;
                    if (model.CodigoCE != 0)
                    {
                        eventService.MetaDataType_SpecificControl = model.CodigoCE;
                        if (model.CodigoCE == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.SpecificControl = model.SpecificControl;
                    }
                    else
                        eventService.MetaDataType_SpecificControl = null;

                    //Controle Específico de Entrada
                    eventService.SpecificInputControl = string.Empty;
                    if (model.CodigoCE_Entrada != 0)
                    {
                        eventService.MetaDataType_SpecificInputControl = model.CodigoCE_Entrada;
                        if (model.CodigoCE_Entrada == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.SpecificInputControl = model.SpecificInputControl;
                    }
                    else
                        eventService.MetaDataType_SpecificInputControl = null;

                    //Controle Específico de Saída
                    eventService.SpecificOutputControl = string.Empty;
                    if (model.CodigoCE_Saida != 0)
                    {
                        eventService.MetaDataType_SpecificOutputControl = model.CodigoCE_Saida;
                        if (model.CodigoCE_Saida == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                            eventService.SpecificOutputControl = model.SpecificOutputControl;
                    }
                    else
                        eventService.MetaDataType_SpecificOutputControl = null;

                    eventService.TipoMovimentacao_ContabilizaSP = eventService.TipoMovimentacao_ContabilizaSP != null ? eventService.TipoMovimentacao_ContabilizaSP: string.Empty;

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        db.Entry(eventService).State = EntityState.Modified;
                        db.SaveChanges();
                        transaction.Complete();
                    }

                    return RedirectToAction("Index");
                }

                CarregaCombos(model);
                return View(model);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion
        #region Delete Actions

        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                EventServiceContabilizaSP eventServiceContabilizaSP =  db.EventServicesContabilizaSPs.Find(id);

                if (eventServiceContabilizaSP.IsNull())
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                int codigoTipoMovimento = -1;
                int codigoTipoMovimentacao = -1;
                MovementType tipoMovPatrimonial_SAM = null;
                MovementType tipoMovPatrimonial_ContabilizaSP = null;
                int[] agrupamentosTiposMovimentacao = new int[] { (int)EnumGroupMoviment.Incorporacao, (int)EnumGroupMoviment.Movimentacao, (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };
                int[] tiposMovimentacaoContabilizaSP = new int[] { (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };

                eventServiceContabilizaSP.MovimentacaoPatrimonialDepreciaOuReclassifica = eventServiceContabilizaSP.RelatedMovementType.ContraPartidaContabilDepreciaOuReclassifica();
                eventServiceContabilizaSP.AccountEntryType = (AccountEntryType.FromValue(eventServiceContabilizaSP.AccountEntryTypeId)).Name;

                eventServiceContabilizaSP.MetaDataType_AccountingValueFieldDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_AccountingValueField)).Name;
                eventServiceContabilizaSP.MetaDataType_DateFieldDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_DateField)).Name;

                //eventServiceContabilizaSP.MetaDataType_StockDestinationDescription     = ((eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull()) ? (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name : String.Empty);
                if (eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull())
                    if (eventServiceContabilizaSP.MetaDataType_StockDestination == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                        eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = eventServiceContabilizaSP.StockDestination;
                    else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                        eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                    else
                        eventServiceContabilizaSP.MetaDataType_StockDestinationDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name;

                if (eventServiceContabilizaSP.MetaDataType_StockSource.IsNotNull())
                    if (eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                        eventServiceContabilizaSP.MetaDataType_StockSourceDescription = eventServiceContabilizaSP.StockSource;
                    else if (eventServiceContabilizaSP.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                        eventServiceContabilizaSP.MetaDataType_StockSourceDescription = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_StockSource.Value).Name;



                tipoMovPatrimonial_SAM = (Int32.TryParse(eventServiceContabilizaSP.TipoMovimento_SamPatrimonio, out codigoTipoMovimento) ? (db.MovementTypes.Where(tipoMovimentacaoPatrimonial => agrupamentosTiposMovimentacao.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId) && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimento).FirstOrDefault()) : null);
                tipoMovPatrimonial_ContabilizaSP = (Int32.TryParse(eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP, out codigoTipoMovimentacao) ? (db.MovementTypes.Where(tipoMovimentacaoPatrimonial => tipoMovimentacaoPatrimonial.GroupMovimentId == eventServiceContabilizaSP.AccountEntryTypeId && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimentacao).FirstOrDefault()) : null);

                eventServiceContabilizaSP.DescricaoTipoMovimento_SamPatrimonio = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_ContabilizaSP.IsNotNull() ? tipoMovPatrimonial_ContabilizaSP.Description : "");
                eventServiceContabilizaSP.InputOutputReclassificationDepreciationType = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");

                if (tipoMovPatrimonial_SAM.IsNotNull() && tiposMovimentacaoContabilizaSP.Contains(eventServiceContabilizaSP.RelatedMovementType.GroupMovimentId))
                {
                    eventServiceContabilizaSP.InputOutputReclassificationDepreciationType = (eventServiceContabilizaSP.RelatedMovementType.IsNotNull() ? eventServiceContabilizaSP.RelatedMovementType.Description : "");
                    eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = "";
                }
                else if (tipoMovPatrimonial_SAM.IsNotNull())// && tipoMovPatrimonial_ContabilizaSP.IsNull())
                {
                    if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                        eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = eventServiceContabilizaSP.TipoMovimentacao_ContabilizaSP;
                    else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.Sam_MovementTypeId.Value)
                        eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                    else if (eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                        eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP = MetaDataTypeServiceContabilizaSPType.FromValue(eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                }
                


                return View(eventServiceContabilizaSP);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                EventServiceContabilizaSP eventServiceContabilizaSP = db.EventServicesContabilizaSPs.Find(id);
                eventServiceContabilizaSP.Status = false;

                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    db.Entry(eventServiceContabilizaSP).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Complete();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #endregion

        #region View Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Filtra os dados pela expressão informada
        /// </summary>
        /// <param name="searchString">Expressão informada</param>
        /// <param name="result">Modelo que será filtrado</param>
        //private IQueryable<EventServiceContabilizaSP> SearchByFilter(string searchString, IQueryable<EventServiceContabilizaSP> result)
        //{
        //    if (!String.IsNullOrEmpty(searchString))
        //        result = result.Where(registroParaConsulta =>  registroParaConsulta.Status == true 
        //                                                   && (registroParaConsulta.RelatedMovementType.Code.ToString().Contains(searchString) ||
        //                                                       registroParaConsulta.RelatedMovementType.Description.Contains(searchString) ||
        //                                                       registroParaConsulta.RelatedMetaDataType_StockDestination.ToString().Contains(searchString) ||
        //                                                       registroParaConsulta.RelatedMetaDataType_StockSource.ToString().Contains(searchString) ||
        //                                                       registroParaConsulta.RelatedMetaDataType_AccountingValueField.ToString().Contains(searchString) ||
        //                                                       registroParaConsulta.RelatedMetaDataType_DateField.ToString().Contains(searchString)));
        //    return result;
        //}

        ///// <summary>
        ///// Ordena os dados pelo parametro informado
        ///// </summary>
        ///// <param name="sortOrder">parametro de Ordenação</param>
        ///// <param name="result">Modelo que será ordenado</param>
        //private IQueryable<EventServiceContabilizaSP> SortingByFilter(string sortOrder, IQueryable<EventServiceContabilizaSP> result)
        //{
        //    ViewBag.CodeSortParm = string.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
        //    ViewBag.DescriptionSortParm = sortOrder == "description_desc" ? "description_desc" : "description_desc";

        //    switch (sortOrder)
        //    {
        //        case "description":
        //            result = result.OrderBy(m => m.Description);
        //            break;
        //        case "description_desc":
        //            result = result.OrderByDescending(m => m.Description);
        //            break;
        //        case "code":
        //            result = result.OrderBy(m => m.Code);
        //            break;
        //        case "code_desc":
        //            result = result.OrderByDescending(m => m.Code);
        //            break;
        //        default:
        //            result = result.OrderBy(m => m.Description);
        //            break;
        //    }

        //    return result;
        //}
        #endregion

        #region Métodos privados

        private void CarregaCombos(EventServiceContabilizaSPViewModel eventServiceContabilizaSP)
        {
            MetaDataTypeServiceContabilizaSPType[] lista = null;

            if (eventServiceContabilizaSP == null)
            {
                //Tipo Movimentação (SAM) 
                ViewBag.TiposMovimentos_SamPatrimonio = new SelectList(db.MovementTypes.Where(movPatrimonial => movPatrimonial.Status == true).AsEnumerable(), "Id", "Description");

                //Tipo Agrupamento (ContabilizaSP)
                ViewBag.AccountEntryTypes = new SelectList(AccountEntryType.GetEnumValues(), "Value", "Name");

                //Tipo Entrada/ Saída / Reclassificação / Depreciação (ContabilizaSP)
                ViewBag.InputOutputReclassificationDepreciationTypeCodes = new SelectList(db.MovementTypes.Where(movPatrimonial => movPatrimonial.Status == true).AsEnumerable(), "Id", "Description");

                lista = new MetaDataTypeServiceContabilizaSPType[4] {
                    MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.DepreciationAuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.Sam_MovementTypeId,
                    MetaDataTypeServiceContabilizaSPType.TextualInformation
                };

                //Tipo Movimentação (ContabilizaSP)
                ViewBag.TiposMovimentos_Contabiliza = new SelectList(lista, "Value", "Name");

                lista = new MetaDataTypeServiceContabilizaSPType[2] {
                    MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.TextualInformation
                };

                //Estoque Origem (MetaDado)
                ViewBag.EstoqueOrigens = new SelectList(lista, "Value", "Name");

                lista = new MetaDataTypeServiceContabilizaSPType[3] {
                    MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.DepreciationAuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.TextualInformation
                };

                //Estoque Destino (MetaDado)
                ViewBag.EstoqueDestino = new SelectList(lista, "Value", "Name");

                lista = new MetaDataTypeServiceContabilizaSPType[2] { MetaDataTypeServiceContabilizaSPType.IncorporationDate, MetaDataTypeServiceContabilizaSPType.MovimentDate };

                //Data Movimentação (MetaDado)
                ViewBag.DatasMovimentacao = new SelectList(lista, "Value", "Name");

                lista = new MetaDataTypeServiceContabilizaSPType[3] { MetaDataTypeServiceContabilizaSPType.AcquisitionValue,
                                                                      MetaDataTypeServiceContabilizaSPType.CurrentValue,
                                                                      MetaDataTypeServiceContabilizaSPType.AccumulatedDepreciationValue };

                //Valor Movimentação (MetaDado)
                ViewBag.ValoresMovimentacao = new SelectList(lista, "Value", "Name");

                lista = new MetaDataTypeServiceContabilizaSPType[3] {
                    MetaDataTypeServiceContabilizaSPType.TextualInformation,
                    MetaDataTypeServiceContabilizaSPType.StandartSpecificControl,
                    MetaDataTypeServiceContabilizaSPType.SpecificControlByMovementType
                };

                //Controle específico
                ViewBag.ControleEspecifico = new SelectList(lista, "Value", "Name");

                //Controle específico de entrada
                ViewBag.CEEntrada = new SelectList(lista, "Value", "Name");

                //Controle específico de saída
                ViewBag.CESaida = new SelectList(lista, "Value", "Name");
            }
            else {
                //Tipo Movimentação (SAM) 
                ViewBag.TiposMovimentos_SamPatrimonio = new SelectList(db.MovementTypes.Where(movPatrimonial => movPatrimonial.Status == true).AsEnumerable(), "Id", "Description", Convert.ToInt32(eventServiceContabilizaSP.TipoMovimento_SamPatrimonio));

                //Tipo Agrupamento (ContabilizaSP)
                ViewBag.AccountEntryTypes = new SelectList(AccountEntryType.GetEnumValues(), "Value", "Name", eventServiceContabilizaSP.AccountEntryTypeId);

                //Tipo Entrada/ Saída / Reclassificação / Depreciação (ContabilizaSP)
                ViewBag.InputOutputReclassificationDepreciationTypeCodes = new SelectList(db.MovementTypes.Where(movPatrimonial => movPatrimonial.Status == true).AsEnumerable(), "Id", "Description", eventServiceContabilizaSP.InputOutputReclassificationDepreciationTypeCode);

                lista = new MetaDataTypeServiceContabilizaSPType[4] {
                    MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.DepreciationAuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.Sam_MovementTypeId,
                    MetaDataTypeServiceContabilizaSPType.TextualInformation
                };

                //Tipo Movimentação (ContabilizaSP)
                ViewBag.TiposMovimentos_Contabiliza = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.CodigoTipoMovimento_ContabilizaSP);

                lista = new MetaDataTypeServiceContabilizaSPType[2] {
                    MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.TextualInformation
                };

                //Estoque Origem (MetaDado)
                ViewBag.EstoqueOrigens = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.CodigoEstoque_Origem);

                lista = new MetaDataTypeServiceContabilizaSPType[3] {
                    MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.DepreciationAuxiliaryAccountAsset,
                    MetaDataTypeServiceContabilizaSPType.TextualInformation
                };


                //Estoque Destino (MetaDado)
                ViewBag.EstoqueDestino = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.MetaDataType_StockDestination ?? 0);

                lista = new MetaDataTypeServiceContabilizaSPType[2] { MetaDataTypeServiceContabilizaSPType.IncorporationDate, MetaDataTypeServiceContabilizaSPType.MovimentDate };

                //Data Movimentação (MetaDado)
                ViewBag.DatasMovimentacao = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.MetaDataType_DateField);

                lista = new MetaDataTypeServiceContabilizaSPType[3] { MetaDataTypeServiceContabilizaSPType.AcquisitionValue,
                                                                      MetaDataTypeServiceContabilizaSPType.CurrentValue,
                                                                      MetaDataTypeServiceContabilizaSPType.AccumulatedDepreciationValue };

                //Valor Movimentação (MetaDado)
                ViewBag.ValoresMovimentacao = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.MetaDataType_AccountingValueField);

                lista = new MetaDataTypeServiceContabilizaSPType[3] {
                    MetaDataTypeServiceContabilizaSPType.TextualInformation,
                    MetaDataTypeServiceContabilizaSPType.StandartSpecificControl,
                    MetaDataTypeServiceContabilizaSPType.SpecificControlByMovementType
                };

                //Controle específico
                ViewBag.ControleEspecifico = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.CodigoCE);

                //Controle específico de entrada
                ViewBag.CEEntrada = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.CodigoCE_Entrada);

                //Controle específico de saída
                ViewBag.CESaida = new SelectList(lista, "Value", "Name", eventServiceContabilizaSP.CodigoCE_Saida);
            }
        }

        #endregion

        #region Métodos estáticos

        static internal EventServiceContabilizaSP _instanciadorDTOEventServiceContabilizaSP(EventServiceContabilizaSP rowTabela)
        {
            int codigoTipoMovimento = -1;
            int codigoTipoMovimentacao = -1;
            MovementType tipoMovPatrimonial_SAM = null;
            MovementType tipoMovPatrimonial_ContabilizaSP = null;
            int[] agrupamentosTiposMovimentacao = null;
            int[] tiposMovimentacaoContabilizaSP = null;
            EventServiceContabilizaSP dtoRetorno = null;
            SAMContext contextoCamadaDados = new SAMContext();



            agrupamentosTiposMovimentacao = new int[] { (int)EnumGroupMoviment.Incorporacao, (int)EnumGroupMoviment.Movimentacao, (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };
            tiposMovimentacaoContabilizaSP = new int[] { (int)EnumGroupMoviment.Depreciacao, (int)EnumGroupMoviment.Reclassificacao };


            dtoRetorno = new EventServiceContabilizaSP();

            dtoRetorno.MovimentacaoPatrimonialDepreciaOuReclassifica = rowTabela.RelatedMovementType.ContraPartidaContabilDepreciaOuReclassifica();
            dtoRetorno.AccountEntryType = (AccountEntryType.FromValue(rowTabela.AccountEntryTypeId)).Name;

            dtoRetorno.MetaDataType_AccountingValueFieldDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_AccountingValueField)).Name;
            dtoRetorno.MetaDataType_DateFieldDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_DateField)).Name;


            //_eventServiceContabilizaSP.MetaDataType_StockDestinationDescription     = ((_eventServiceContabilizaSP.MetaDataType_StockDestination.IsNotNull()) ? (MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_StockDestination.Value)).Name : String.Empty);
            if (rowTabela.MetaDataType_StockDestination.IsNotNull())
                if (rowTabela.MetaDataType_StockDestination == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                    dtoRetorno.MetaDataType_StockDestinationDescription = rowTabela.StockDestination;
                else if (rowTabela.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                    dtoRetorno.MetaDataType_StockDestinationDescription = MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_MovementTypeContabilizaSP.Value).Name;
                else
                    dtoRetorno.MetaDataType_StockDestinationDescription = (MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_StockDestination.Value)).Name;

            if (rowTabela.MetaDataType_StockSource.IsNotNull())
                if (rowTabela.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                    dtoRetorno.MetaDataType_StockSourceDescription = rowTabela.StockSource;
                else if (rowTabela.MetaDataType_StockSource == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                    dtoRetorno.MetaDataType_StockSourceDescription = MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_StockSource.Value).Name;


            //CONTROLE ESPECIFICO
            if (rowTabela.MetaDataType_SpecificControl.IsNotNull())
                if (rowTabela.MetaDataType_SpecificControl == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                    dtoRetorno.MetaDataType_SpecificControlDescription = rowTabela.SpecificControl;
                else if (rowTabela.MetaDataType_SpecificControl == MetaDataTypeServiceContabilizaSPType.SpecificControlByMovementType.Value)
                    dtoRetorno.MetaDataType_SpecificControlDescription = MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_SpecificControl.Value).Name;

            //CONTROLE ESPECIFICO ENTRADA
            if (rowTabela.MetaDataType_SpecificInputControl.IsNotNull())
                if (rowTabela.MetaDataType_SpecificInputControl == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                    dtoRetorno.MetaDataType_SpecificInputControlDescription = rowTabela.SpecificInputControl;
                else if (rowTabela.MetaDataType_SpecificInputControl == MetaDataTypeServiceContabilizaSPType.SpecificControlByMovementType.Value)
                    dtoRetorno.MetaDataType_SpecificInputControlDescription = MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_SpecificInputControl.Value).Name;

            //CONTROLE ESPECIFICO SAIDA
            if (rowTabela.MetaDataType_SpecificOutputControl.IsNotNull())
                if (rowTabela.MetaDataType_SpecificOutputControl == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                    dtoRetorno.MetaDataType_SpecificOutputControlDescription = rowTabela.SpecificOutputControl;
                else if (rowTabela.MetaDataType_SpecificOutputControl == MetaDataTypeServiceContabilizaSPType.SpecificControlByMovementType.Value)
                    dtoRetorno.MetaDataType_SpecificOutputControlDescription = MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_SpecificOutputControl.Value).Name;

            tipoMovPatrimonial_SAM = (Int32.TryParse(rowTabela.TipoMovimento_SamPatrimonio, out codigoTipoMovimento) ? (contextoCamadaDados.MovementTypes.Where(tipoMovimentacaoPatrimonial => agrupamentosTiposMovimentacao.Contains(tipoMovimentacaoPatrimonial.GroupMovimentId) && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimento).FirstOrDefault()) : null);
            tipoMovPatrimonial_ContabilizaSP = (Int32.TryParse(rowTabela.TipoMovimentacao_ContabilizaSP, out codigoTipoMovimentacao) ? (contextoCamadaDados.MovementTypes.Where(tipoMovimentacaoPatrimonial => tipoMovimentacaoPatrimonial.GroupMovimentId == rowTabela.AccountEntryTypeId && tipoMovimentacaoPatrimonial.Code == codigoTipoMovimentacao).FirstOrDefault()) : null);

            dtoRetorno.DescricaoTipoMovimento_SamPatrimonio = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
            dtoRetorno.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_ContabilizaSP.IsNotNull() ? tipoMovPatrimonial_ContabilizaSP.Description : "");
            dtoRetorno.InputOutputReclassificationDepreciationType = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");

            //_eventServiceContabilizaSP.DescricaoTipoMovimentacao_ContabilizaSP      = (tipoMovPatrimonial_ContabilizaSP.IsNotNull() ? tipoMovPatrimonial_ContabilizaSP.Description : (_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.IsNotNull() ? (MetaDataTypeServiceContabilizaSPType.FromValue(_eventServiceContabilizaSP.MetaDataType_MovementTypeContabilizaSP.Value)).Name : String.Empty));
            //if (tipoMovPatrimonial_SAM.IsNotNull() && tiposMovimentacaoContabilizaSP.Contains(tipoMovPatrimonial_SAM.GroupMovimentId))
            if (tipoMovPatrimonial_SAM.IsNotNull() && tiposMovimentacaoContabilizaSP.Contains(rowTabela.RelatedMovementType.GroupMovimentId))
            {
                dtoRetorno.InputOutputReclassificationDepreciationType = (rowTabela.RelatedMovementType.IsNotNull() ? rowTabela.RelatedMovementType.Description : "");
                dtoRetorno.DescricaoTipoMovimentacao_ContabilizaSP = "";
            }
            //else if (tipoMovPatrimonial_ContabilizaSP.IsNull())
            else if (tipoMovPatrimonial_SAM.IsNotNull())// && tipoMovPatrimonial_ContabilizaSP.IsNull())
            {
                if (rowTabela.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.TextualInformation.Value)
                    dtoRetorno.DescricaoTipoMovimentacao_ContabilizaSP = rowTabela.TipoMovimentacao_ContabilizaSP;
                else if (rowTabela.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.Sam_MovementTypeId.Value)
                    dtoRetorno.DescricaoTipoMovimentacao_ContabilizaSP = (tipoMovPatrimonial_SAM.IsNotNull() ? tipoMovPatrimonial_SAM.Description : "");
                else if (rowTabela.MetaDataType_MovementTypeContabilizaSP == MetaDataTypeServiceContabilizaSPType.AuxiliaryAccountAsset.Value)
                    dtoRetorno.DescricaoTipoMovimentacao_ContabilizaSP = MetaDataTypeServiceContabilizaSPType.FromValue(rowTabela.MetaDataType_MovementTypeContabilizaSP.Value).Name;
            }

            codigoTipoMovimentacao = -1;
            codigoTipoMovimento = -1;
            tipoMovPatrimonial_SAM = null;
            tipoMovPatrimonial_ContabilizaSP = null;

            return dtoRetorno;
        }

        #endregion
    }
}
