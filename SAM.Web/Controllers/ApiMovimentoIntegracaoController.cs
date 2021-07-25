using PatrimonioBusiness.integracao;
using PatrimonioBusiness.integracao.entidades;
using PatrimonioBusiness.integracao.interfaces;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using ServicoOnlineServer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;

namespace SAM.Web.Controllers
{
    [Authorize]
    public class ApiMovimentoIntegracaoController : WebApiBaseController
    {
        private SAMContext contextPatrimonio = null;
        private const string CONTA_AUXILIAR_INTEGRACAO = "111111111";
        private int tipoMovimentacaoIntegracao_SAMEstoque_SAMPatrimonio = (int)EnumMovimentType.IncorpIntegracaoSAMEstoque_SAMPatrimonio;


        [HttpGet]
        public string Get()
        {
            return "Teste";
        }

        [HttpPost]
        [Route("api/ApiMovimentoIntegracao/GetParaMontarGrid")]
        public async Task<List<AssetEAssetMovimentViewModel>> GetParaMontarGrid([FromBody] DataTablesResponseViewModel model)
        {

            string filtro = model.Search.Value;
            int ordernar = model.Order[0].Column;
            string ordernarDirecao = model.Order[0].Dir;

            int _draw = model.Draw;
            int startRec = model.Start;
            int pageSize = model.Length;

            base.isolationLevel = IsolationLevel.ReadUncommitted;

            base.movimentoIntegracaoAbstract = IntegracaoFactory.GetInstancia().CreateMovimentoIntegracao(base.isolationLevel);
            var resultado = await base.movimentoIntegracaoAbstract.Gets(startRec, filtro, pageSize);

            return null;
        }

        [HttpGet]
        [Route("api/ApiMovimentoIntegracao/GetBensPatrimoniais")]
        public async Task<IHttpActionResult> GetBensPatrimoniais()
        {

            var assets = await CreateIntegracao("",null,null);
            return Json(assets, base.jsonSerializerSettings);
           
        }
        public async Task<IHttpActionResult> CreateIntegracao(string usuario, int? codigoUgeEstoque, string codigoUGEPatrimonio)
        {
            if (codigoUgeEstoque.HasValue)
            {
                base.isolationLevel = IsolationLevel.ReadUncommitted;
                contextPatrimonio = new SAMContext();
                contextPatrimonio.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
                contextPatrimonio.Configuration.AutoDetectChangesEnabled = false;
                contextPatrimonio.Configuration.ProxyCreationEnabled = false;
                contextPatrimonio.Configuration.LazyLoadingEnabled = false;

                base.movimentoIntegracaoAbstract = IntegracaoFactory.GetInstancia().CreateMovimentoIntegracao(base.isolationLevel);
                var mesReferencia = contextPatrimonio.ManagerUnits.Where(x => x.Code.Equals(codigoUGEPatrimonio)).FirstOrDefaultAsync().Result.ManagmentUnit_YearMonthReference;


                var resultado = await base.movimentoIntegracaoAbstract.GetsAsync(codigoUgeEstoque.Value, mesReferencia);
                if (resultado != null && resultado.Count > 0)
                {
                    await GetAssetsAsync(resultado, usuario);
                    return Ok();
                }
            }

            return NotFound();
        }
        private async Task GetAssetsAsync(IList<IMovimentoIntegracao> movimentoIntegracoes, string usuario)
        {

            contextPatrimonio = new SAMContext();
            contextPatrimonio.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            contextPatrimonio.Configuration.AutoDetectChangesEnabled = false;
            contextPatrimonio.Configuration.ProxyCreationEnabled = false;
            contextPatrimonio.Configuration.LazyLoadingEnabled = false;

            int managerUnitId;
            int managerUnitId_AlmoxarifadoSAM;
            int ugeUaMovimentacao = 0;
            int administrativeUnitId;
            int budgetUnitId;
            int institutionId;
            int initialId;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                DateTime dataIncorporacao = DateTime.Now;
                DateTime dataAquisicao = DateTime.Now;
                int assetStartId = 0;
                int codigoDoGrupo = 0;
                int materialItemCode = 0;
                IList<IIntegracaoDepreciacao> parametrosDepreciacoes = new List<IIntegracaoDepreciacao>();

                foreach (var item in movimentoIntegracoes)
                {
                    IIntegracaoDepreciacao parametrosDepreciacao = new IntegracaoDepreciacao();

                    if (contextPatrimonio.Database.CurrentTransaction == null)
                        contextPatrimonio.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

                    administrativeUnitId = BuscaIdDaUA(item.CodigoUa, item.CodigoUge);
                    managerUnitId = contextPatrimonio.AdministrativeUnits.Where(x => x.Id == administrativeUnitId).FirstOrDefaultAsync().Result.ManagerUnitId;
                    //TODO MARCADOR INTEGRACAO SAM-ESTOQUE COM XML
                    ugeUaMovimentacao = Int32.Parse(contextPatrimonio.ManagerUnits.Where(x => x.Id == managerUnitId).FirstOrDefaultAsync().Result.Code);
                    managerUnitId_AlmoxarifadoSAM = contextPatrimonio.ManagerUnits.Where(x => x.Code == item.CodigoUge.ToString()).FirstOrDefaultAsync().Result.Id;
                    budgetUnitId = contextPatrimonio.ManagerUnits.Where(x => x.Id == managerUnitId).FirstOrDefaultAsync().Result.BudgetUnitId;
                    institutionId = contextPatrimonio.BudgetUnits.Where(x => x.Id == budgetUnitId).FirstOrDefaultAsync().Result.InstitutionId;

                    var sigla = await contextPatrimonio.Initials.Where(x => x.InstitutionId == institutionId && x.BudgetUnitId == budgetUnitId).FirstOrDefaultAsync();
                    if (sigla == null)
                        sigla = await contextPatrimonio.Initials.Where(x => x.InstitutionId == institutionId).FirstOrDefaultAsync();

                    initialId = sigla.Id;

                    var stateConservation = await contextPatrimonio.StateConservations.Where(x => x.Description.ToUpper().Equals("NOVO", StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
                    var shortDescription = await contextPatrimonio.ShortDescriptionItens.Where(x => x.Description == item.ItemMaterialDescricao).FirstOrDefaultAsync();

                    MaterialItemsController materialItemsController = new MaterialItemsController();
                    var material = materialItemsController.GetItemMaterialBD(item.ItemMaterialCode.ToString());
                    MaterialItemViewModel materialItemViewModel = null;
                    try
                    {
                        materialItemViewModel = serializer.Deserialize<MaterialItemViewModel>(material.Data.ToString());
                        if (materialItemViewModel == null)
                        {
                            material = materialItemsController.GetItemMaterial(item.ItemMaterialCode.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        material = materialItemsController.GetItemMaterial(item.ItemMaterialCode.ToString());
                    }
                    if (material == null)
                        material = materialItemsController.GetItemMaterial(item.ItemMaterialCode.ToString());


                    materialItemViewModel = serializer.Deserialize<MaterialItemViewModel>(material.Data.ToString());

                    if (shortDescription == null)
                    {
                        shortDescription = new ShortDescriptionItem();
                        shortDescription.Description = item.ItemMaterialDescricao;
                        contextPatrimonio.ShortDescriptionItens.Add(shortDescription);
                        await contextPatrimonio.SaveChangesAsync();
                    }

                    var contaContabil = contextPatrimonio.AuxiliaryAccounts.Where(x => x.BookAccount.ToString() == CONTA_AUXILIAR_INTEGRACAO).AsNoTracking().FirstOrDefault();
                    for (int i = 1; i <= Convert.ToInt32(item.QuantidadeMovimentada); i++)
                    {
                        Asset asset = new Asset
                        {
                            MovimentDate = item.DataOperacao.Value,
                            NumberDoc = item.NumeroDocumento,
                            Status = true,
                            ValueAcquisition = item.ValorUnitario,
                            //MovementTypeId = (int)EnumMovimentType.IncorporacaoDeInventarioInicial,
                            MovementTypeId = tipoMovimentacaoIntegracao_SAMEstoque_SAMPatrimonio,
                            flagVerificado = (int)EnumPendenciasDadosBP.PendenteChapa,
                            flagDepreciaAcumulada = null,
                            NumberIdentification = "99999999999",
                            MaterialItemCode = materialItemViewModel.Code,
                            MaterialGroupCode = materialItemViewModel.MaterialGroupCode,
                            LifeCycle = (materialItemViewModel.LifeCycle.HasValue ? materialItemViewModel.LifeCycle.Value : 0),
                            ResidualValue = (materialItemViewModel.ResidualValue.HasValue ? materialItemViewModel.ResidualValue.Value : 0),
                            RateDepreciationMonthly = (materialItemViewModel.RateDepreciationMonthly.HasValue ? materialItemViewModel.RateDepreciationMonthly.Value : 0),
                            InitialId = initialId,
                            MaterialItemDescription = shortDescription.Description,
                            ManagerUnitId = managerUnitId,
                            ShortDescriptionItemId = shortDescription.Id,
                            AcquisitionDate = item.DataMovimento,
                            flagVindoDoEstoque = true,
                            IndicadorOrigemInventarioInicial = (byte)EnumOrigemInventarioInicial.IntegracaoComEstoque,
                            DiferenciacaoChapa = "",
                            DiferenciacaoChapaAntiga = ""
                        };
                        contextPatrimonio.Assets.Add(asset);
                        await contextPatrimonio.SaveChangesAsync();
                        asset.AssetStartId = asset.Id;
                        contextPatrimonio.Entry(asset).State = EntityState.Modified;
                        await contextPatrimonio.SaveChangesAsync();

                        AssetMovements assetMovements = new AssetMovements
                        {
                            AssetId = asset.Id,
                            ManagerUnitId = managerUnitId,
                            AdministrativeUnitId = administrativeUnitId,
                            InstitutionId = institutionId,
                            BudgetUnitId = budgetUnitId,
                            Observation = item.Obeservacoes,
                            Status = true,
                            MovimentDate = item.DataMovimento,
                            //MovementTypeId = (int)EnumMovimentType.IncorporacaoDeInventarioInicial,
                            MovementTypeId = tipoMovimentacaoIntegracao_SAMEstoque_SAMPatrimonio,
                            NumberDoc = item.NumeroDocumento,
                            Login = usuario,
                            DataLogin = DateTime.Now,
                            StateConservationId = stateConservation.Id,
                            AuxiliaryAccountId = contaContabil.Id,
                            //TODO MARCADOR INTEGRACAO SAM-ESTOQUE COM XML
                            SourceDestiny_ManagerUnitId = managerUnitId_AlmoxarifadoSAM,
                            AuxiliaryMovementTypeId = obtemTipoMovimentacaoAuxiliar(ugeUaMovimentacao, item.CodigoUge)
                        };

                        contextPatrimonio.AssetMovements.Add(assetMovements);
                        await contextPatrimonio.SaveChangesAsync();
                        parametrosDepreciacao.AssetStartId = asset.AssetStartId.Value;
                        parametrosDepreciacao.CodigoDaUge = int.Parse(contextPatrimonio.ManagerUnits.Where(x => x.Id == managerUnitId).FirstOrDefault().Code);
                        parametrosDepreciacao.DataFinal = DateTime.Now.AddMonths(asset.LifeCycle);

                        dataAquisicao = asset.AcquisitionDate;
                        dataIncorporacao = asset.MovimentDate;
                        assetStartId = asset.AssetStartId.Value;
                        codigoDoGrupo = asset.MaterialGroupCode;
                        materialItemCode = asset.MaterialItemCode;

                        assetMovements = null;
                        asset = null;
                    }
                    var resultado = await base.movimentoIntegracaoAbstract.ConfirmarIntegracao(item.Id);
                    if (resultado)
                    {
                        contextPatrimonio.Database.CurrentTransaction.Commit();           
                    }
                    else
                        contextPatrimonio.Database.CurrentTransaction.Rollback();

                }
            }
            catch (Exception ex)
            {
                contextPatrimonio.Database.CurrentTransaction.Rollback();
                throw ex;
            }

        }

        private int BuscaIdDaUA(int? codigoUA, int codigoUGE)
        {

            int administrativeUnitId = -1;
            string codigoUGEstring = codigoUGE.ToString();

            administrativeUnitId = (from a in contextPatrimonio.AdministrativeUnits
                                    join m in contextPatrimonio.ManagerUnits on a.ManagerUnitId equals m.Id
                                    where a.Code == codigoUA
                                    && m.Code == codigoUGEstring
                                    && a.Status == true
                                    select a.Id).FirstOrDefault();

            if (administrativeUnitId > 0) return administrativeUnitId;

            administrativeUnitId = (from a in contextPatrimonio.AdministrativeUnits
                                    join m in contextPatrimonio.ManagerUnits on a.ManagerUnitId equals m.Id
                                    where a.Code == codigoUA
                                    && m.Code == codigoUGEstring
                                    select a.Id).FirstOrDefault();

            if (administrativeUnitId > 0) return administrativeUnitId;

            administrativeUnitId = contextPatrimonio.AdministrativeUnits.Where(x => x.Code == codigoUA && x.Status).FirstOrDefaultAsync().Result.Id;

            if (administrativeUnitId > 0) return administrativeUnitId;

            administrativeUnitId = contextPatrimonio.AdministrativeUnits.Where(x => x.Code == codigoUA).FirstOrDefaultAsync().Result.Id;

            return administrativeUnitId;
        }

        //TODO MARCADOR INTEGRACAO SAM-ESTOQUE COM XML
        private int obtemTipoMovimentacaoAuxiliar(int codigoUgeUA_RequisicaoMaterial, int codigoUGE_Patrimonio)
        {
            int tipoMovimentacaoAuxiliar = 0;

            if ((codigoUgeUA_RequisicaoMaterial > 0) && (codigoUGE_Patrimonio > 0))
            {

                if (codigoUgeUA_RequisicaoMaterial != codigoUGE_Patrimonio)
                    tipoMovimentacaoAuxiliar = EnumMovimentType.IncorpIntegracaoSAMEstoque_SAMPatrimonio_OutraUGE.GetHashCode();
                else
                    tipoMovimentacaoAuxiliar = EnumMovimentType.ReclassificacaoIntegracaoSAMEstoque_SAMPatrimonio_MesmaUGE.GetHashCode();
            }

            return tipoMovimentacaoAuxiliar;
        }
    }
}
