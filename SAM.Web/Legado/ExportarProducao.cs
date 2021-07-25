using AutoMapper;
using SAM.Web.Common;
using SAM.Web.Controllers;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SAM.Web.Legado
{
    public class ExportarProducao
    {
        private SAMContext contexto = null;
        private Asset BemPatrimonial = null;
        private Responsible Responsavel = null;
        private AuxiliaryAccount ContaAuxiliar = null;
        private OutSourced Terceiro = null;
        private Initial Sigla = null;


        private IDictionary<string, IList<RetornoImportacao>> mensagensDeRetorno;

        private ExportarProducao() {
            contexto = new SAMContext();
        }

        public static ExportarProducao createInstance()
        {
            return new ExportarProducao();
        }

        public void exportarContasAuxiliares(IList<AuxiliaryAccount> ContasAuxiliares) {
            AuxiliaryAccountsController auxiliaryAccountsController = new AuxiliaryAccountsController();

            foreach(AuxiliaryAccount conta in ContasAuxiliares)
            {
                auxiliaryAccountsController.Create(conta);
            }

        }


        public void exportarResponsaveis(Responsible responsavel)
        {
            ResponsiblesController responsiblesController = new ResponsiblesController();
            responsiblesController.Create();
        }
        public void exportarResponsaveis(IList<Responsible> Responsaveis)
        {
            ResponsiblesController responsiblesController = new ResponsiblesController();
            foreach (Responsible responsavel in Responsaveis)
            {
                responsiblesController.Create();
            }

        }

        public void exportarTerceiros(IList<OutSourced> Terceiros)
        {
            OutSourcedsController outSourcedsController = new OutSourcedsController();
      
            foreach (OutSourced terceiro in Terceiros)
            {
                outSourcedsController.Create(terceiro);
            }


        }

        public void exportarSiglas(IList<Initial> Siglas)
        {
            InitialsController initialsController = new InitialsController();

            foreach (Initial sigla in Siglas)
            {
                initialsController.Create(sigla);
            }


        }

       
        // TODO: COMENTADO JULIO
        //public void exportarBensPatrimoniais(IList<Asset> bensPatrimoniais)
        //{
        //    AssetsController assetsController = new AssetsController();

        //    foreach (Asset bemPatrimonial in bensPatrimoniais)
        //    {
        //        AssetViewModel model = new AssetViewModel();
        //        model.Id = bemPatrimonial.Id;
        //        model.InitialId = contexto.Initials.Where(x => x.InstitutionId == bemPatrimonial.InstitutionId 
        //                                                    && x.BudgetUnitId == bemPatrimonial.BudgetUnitId).Select(x => x.Id).FirstOrDefault();
        //        model.NumberIdentification = bemPatrimonial.NumberIdentification;
        //        model.PartNumberIdentification = bemPatrimonial.PartNumberIdentification;
        //        model.EndNumberIdentification = 1;
        //        model.InstitutionId = bemPatrimonial.InstitutionId;
        //        model.AdministrativeUnitId = contexto.AdministrativeUnits.Where(x => x.ManagerUnitId == bemPatrimonial.ManagerUnitId
        //                                                                        && x.RelatedManagerUnit.BudgetUnitId == bemPatrimonial.BudgetUnitId
        //                                                                        && x.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId == bemPatrimonial.InstitutionId
        //                                                                        && x.Code == bemPatrimonial.RelatedAdministrativeUnit.Code).Select(x => x.Id).FirstOrDefault();  //bemPatrimonial.AdministrativeUnitId;

        //        model.BudgetUnitId = contexto.BudgetUnits.Where(x => x.InstitutionId == bemPatrimonial.InstitutionId 
        //                                                        && x.Code == bemPatrimonial.RelatedBudgetUnit.Code).Select(x => x.Id).FirstOrDefault(); //bemPatrimonial.BudgetUnitId;

        //        model.ManagerUnitId = contexto.ManagerUnits.Where(x => x.RelatedBudgetUnit.Id == bemPatrimonial.BudgetUnitId
        //                                                           && x.RelatedBudgetUnit.InstitutionId == bemPatrimonial.InstitutionId
        //                                                           && x.Code == bemPatrimonial.RelatedManagementUnit.Code).Select(x => x.Id).FirstOrDefault();  //bemPatrimonial.ManagerUnitId;

        //        model.SectionId = contexto.Sections.Where(x => x.RelatedAdministrativeUnit.Id == bemPatrimonial.AdministrativeUnitId
        //                                                    && x.RelatedAdministrativeUnit.RelatedManagerUnit.Id == bemPatrimonial.ManagerUnitId
        //                                                    && x.RelatedAdministrativeUnit.RelatedManagerUnit.BudgetUnitId == bemPatrimonial.BudgetUnitId
        //                                                    && x.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId == bemPatrimonial.InstitutionId
        //                                                    && x.Code == bemPatrimonial.RelatedSection.Code).Select(x => x.Id).FirstOrDefault(); //bemPatrimonial.SectionId;

        //        model.ResponsibleId = contexto.Responsibles.Where(x => x.RelatedAdministrativeUnits.Id == bemPatrimonial.AdministrativeUnitId
        //                                                    && x.RelatedAdministrativeUnits.RelatedManagerUnit.Id == bemPatrimonial.ManagerUnitId
        //                                                    && x.RelatedAdministrativeUnits.RelatedManagerUnit.BudgetUnitId == bemPatrimonial.BudgetUnitId
        //                                                    && x.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId == bemPatrimonial.InstitutionId).Select(x => x.Id).FirstOrDefault(); //bemPatrimonial.ResponsibleId;

        //        model.AuxiliaryAccountId = contexto.Assets.Where(x => x.RelatedAuxiliaryAccount.Id == bemPatrimonial.AuxiliaryAccountId
        //                                                           && x.RelatedAuxiliaryAccount.Code == bemPatrimonial.RelatedAuxiliaryAccount.Code).Select(x => x.RelatedAuxiliaryAccount.Id).FirstOrDefault(); //bemPatrimonial.AuxiliaryAccountId;

        //        model.OutSourcedId = contexto.OutSourceds.Where(x => x.BudgetUnitId == bemPatrimonial.BudgetUnitId
        //                                                         && x.InstitutionId == bemPatrimonial.InstitutionId).Select(x => x.Id).FirstOrDefault(); //bemPatrimonial.OutSourcedId;

        //        model.IncorporationTypeId = contexto.IncorporationTypes.Where(x => x.BudgetUnitId == bemPatrimonial.BudgetUnitId
        //                                                         && x.InstitutionId == bemPatrimonial.InstitutionId).Select(x => x.Id).FirstOrDefault();

        //        model.StateConservationId = bemPatrimonial.StateConservationId;
        //        model.MovementTypeId = bemPatrimonial.MovementTypeId;

        //        model.SupplierId = contexto.Suppliers.Where(x => x.CPFCNPJ == bemPatrimonial.RelatedSupplier.CPFCNPJ).Select(x => x.Id).FirstOrDefault();  //bemPatrimonial.SupplierId;

        //        model.AcquisitionDate = bemPatrimonial.AcquisitionDate;
        //        model.ValueAcquisition = bemPatrimonial.ValueAcquisition;
        //        model.ValueUpdate = bemPatrimonial.ValueUpdate;
        //        model.NumberPurchaseProcess = bemPatrimonial.NumberPurchaseProcess;
        //        model.NGPB = bemPatrimonial.NGPB;
        //        model.Invoice = bemPatrimonial.Invoice;
        //        model.SerialNumber = bemPatrimonial.SerialNumber;
        //        model.ManufactureDate = bemPatrimonial.ManufactureDate;
        //        model.DateGuarantee = bemPatrimonial.DateGuarantee;
        //        model.ChassiNumber = bemPatrimonial.ChassiNumber;
        //        model.Brand = bemPatrimonial.Brand;
        //        model.Model = bemPatrimonial.Model;
        //        model.NumberPlate = bemPatrimonial.NumberPlate;
        //        model.AdditionalDescription = bemPatrimonial.AdditionalDescription;
        //        model.HistoricId = bemPatrimonial.HistoricId;
        //        model.Condition = bemPatrimonial.Condition;
        //        model.Rating = bemPatrimonial.Rating;
        //        model.BagDate = bemPatrimonial.BagDate;
        //        model.OldInitial = bemPatrimonial.OldInitial;
        //        model.OldNumberIdentification = bemPatrimonial.OldNumberIdentification;
        //        model.OldPartNumberIdentification = bemPatrimonial.OldPartNumberIdentification;
        //        model.ReceiptTermDate = bemPatrimonial.ReceiptTermDate;
        //        model.InventoryDate = bemPatrimonial.InventoryDate;
        //        model.NoteGranting = bemPatrimonial.NoteGranting;
        //        model.AssigningAmountDate = bemPatrimonial.AssigningAmountDate;
        //        model.Status = bemPatrimonial.Status;
        //        model.LifeCycle = bemPatrimonial.LifeCycle;
        //        model.RateDepreciationMonthly = bemPatrimonial.RateDepreciationMonthly;
        //        model.ResidualValue = bemPatrimonial.ResidualValue;
        //        model.MaterialItemCode = bemPatrimonial.MaterialItemCode;
        //        model.MaterialItemDescription = bemPatrimonial.MaterialItemDescription;
        //        model.MaterialGroupCode = bemPatrimonial.MaterialGroupCode;
        //        model.Empenho = bemPatrimonial.Empenho;
        //        model.Gerador_Descricao = bemPatrimonial.Gerador_Descricao;

        //        assetsController.Create(model);
        //        model = null;
        //    }


        //}

        // TODO: COMENTADO JULIO
        //internal IList<AssetParaJSON> getBensPatrimoniasDoOrgao(int institutionId)
        //{

        //    contexto.Configuration.LazyLoadingEnabled = false;
        //    contexto.Configuration.AutoDetectChangesEnabled = false;
        //    IList<AssetParaJSON> Assets = new List<AssetParaJSON>();

        //    IQueryable<Asset> query = (from bem in contexto.Assets
        //                               where bem.InstitutionId == institutionId
        //                               select bem)
        //                               .Include(x=> x.RelatedAdministrativeUnit)
        //                               .Include(x=> x.RelatedBudgetUnit)
        //                               .Include(x=> x.RelatedManagementUnit)
        //                               .Include(x=> x.RelatedSection)
        //                               .Include(x=> x.RelatedAuxiliaryAccount)
        //                               .Include(x=> x.RelatedInstitution)
        //                               .Distinct().AsQueryable();

        //    var retorno = query.ToList();


        //    foreach (Asset asset in retorno)
        //    {
        //        AssetParaJSON assetParaJSON = converterAssetParaAssetJson(asset);
        //        AuxiliarAcountJSON auxiliarAcountJSON = converterAuxiliarAcountParaAuxiliarAcountJson(asset.RelatedAuxiliaryAccount);
        //        SectionJSON sectionJSON = converterSectionParaSectionJson(asset.RelatedSection);
        //        ManagerUnitJSON managerUnitJSON = converterManagerUnitParaManagerUnitJson(asset.RelatedManagementUnit);
        //        BudgetUnitJSON budgetUnitJSON = converterBudgetUnitParaBudgetUnitJson(asset.RelatedBudgetUnit);
        //        AdministrativeUnitJSON administrativeUnitJSON = converterAdministrativeUnitParaAdministrativeUnitJson(asset.RelatedAdministrativeUnit);
        //        InstitutionJson institutionJson = converterInstitutionParaInstitutionJson(asset.RelatedInstitution);


        //        assetParaJSON.RelatedAdministrativeUnit = administrativeUnitJSON;
        //        assetParaJSON.RelatedAuxiliaryAccount = auxiliarAcountJSON;
        //        assetParaJSON.RelatedBudgetUnit = budgetUnitJSON;
        //        assetParaJSON.RelatedManagementUnit = managerUnitJSON;
        //        assetParaJSON.RelatedSection = sectionJSON;

        //        Assets.Add(assetParaJSON);

        //        assetParaJSON = null;
        //        sectionJSON = null;
        //        managerUnitJSON = null;
        //        budgetUnitJSON = null;
        //        auxiliarAcountJSON = null;
        //        administrativeUnitJSON = null;
        //    }

        //    //IList<ManagerUnit> managerUnits =  retorno.Select(x => x.RelatedManagementUnit).ToList();
        //    //IList<BudgetUnit> budgetUnits = retorno.Select(x => x.RelatedManagementUnit).ToList().Select(x => x.RelatedBudgetUnit).ToList();
        //    //IList<AdministrativeUnit> administrativeUnits = retorno.Select(x => x.RelatedAdministrativeUnit).ToList();
        //    //IList<AuxiliaryAccount> auxiliaryAccounts = retorno.Select(x => x.RelatedAuxiliaryAccount).ToList();

        //    //Mapper.CreateMap<IList<ManagerUnit>, List<ManagerUnit>>();

        //    //Mapper.CreateMap<IList<BudgetUnit>, List<BudgetUnit>>();
        //    //Mapper.CreateMap<IList<AdministrativeUnit>, List<AdministrativeUnit>>();
        //    //Mapper.CreateMap<IList<AuxiliaryAccount>, List<AuxiliaryAccount>>();

        //    //foreach(Asset asset in retorno)
        //    //{
        //    //    foreach(ManagerUnit managerUnit in asset.AdministrativeUni)
        //    //}


        //    return Assets;
        //}
        public AddressJson converterAddressJsonParaAddressJsonJson(Address endereco)
        {
            AddressJson _endereco = new AddressJson();
            _endereco.City = endereco.City;
            _endereco.ComplementAddress = endereco.ComplementAddress;
            _endereco.District = endereco.District;
            _endereco.Id  = endereco.Id;
            _endereco.Number  = endereco.Number;
            _endereco.PostalCode = endereco.PostalCode ;
            _endereco.State = endereco.State;
            _endereco.Street  = endereco.Street;

            return _endereco;
        }

        public ResponsibleJson converterResponsibleParaResponsibleJson(Responsible responsavel)
        {
            ResponsibleJson _responsavel = new ResponsibleJson();
            _responsavel.AdministrativeUnitId = responsavel.AdministrativeUnitId;
            _responsavel.Id  = responsavel.Id;
            _responsavel.Name  = responsavel.Name;
            _responsavel.Position  = responsavel.Position;
            _responsavel.RelatedAdministrativeUnit  = converterAdministrativeUnitParaAdministrativeUnitJson(responsavel.RelatedAdministrativeUnits);
            _responsavel.RelatedManagementUnit = converterManagerUnitParaManagerUnitJson(responsavel.RelatedAdministrativeUnits.RelatedManagerUnit);
            _responsavel.RelatedBudgetUnit = converterBudgetUnitParaBudgetUnitJson(responsavel.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit);
            _responsavel.RelatedInstitution = converterInstitutionParaInstitutionJson(responsavel.RelatedAdministrativeUnits.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution);


            return _responsavel;
        }
        public OutSourcedJson converterOutSourcedParaOutSourcedJson(OutSourced terceiro)
        {
            OutSourcedJson _terceiro = new OutSourcedJson();

            _terceiro.AddressId = terceiro.AddressId;
            _terceiro.BudgetUnitId = terceiro.BudgetUnitId;
            _terceiro.CPFCNPJ = terceiro.CPFCNPJ;
            _terceiro.Id = terceiro.Id;
            _terceiro.InstitutionId = terceiro.InstitutionId;
            _terceiro.Name = terceiro.Name;
            _terceiro.RelatedAddressJson = converterAddressJsonParaAddressJsonJson(terceiro.RelatedAddress);
            _terceiro.RelatedBudgetUnit = converterBudgetUnitParaBudgetUnitJson(terceiro.RelatedBudgetUnit);
            _terceiro.RelatedInstitution = converterInstitutionParaInstitutionJson(terceiro.RelatedInstitution);


            return _terceiro;
        }

        public InitialJson converterInitialParaInitialJson(Initial sigla)
        {
            InitialJson _sigla = new InitialJson();

            _sigla.BarCode = sigla.BarCode;
            _sigla.BudgetUnitId  = sigla.BudgetUnitId.Value;
            _sigla.Description  = sigla.Description;
            _sigla.Id  = sigla.Id;
            _sigla.InstitutionId  = sigla.InstitutionId;
            _sigla.RelatedBudgetUnit = converterBudgetUnitParaBudgetUnitJson(sigla.RelatedBudgetUnit);
            _sigla.RelatedInstitution = converterInstitutionParaInstitutionJson(sigla.RelatedInstitution);

            return _sigla;
        }

       

        public InstitutionJson converterInstitutionParaInstitutionJson(Institution institution)
        {
            InstitutionJson institutionJson = new InstitutionJson();
            institutionJson.Code = institution.Code;
            institutionJson.Id = institution.Id;
          
            return institutionJson;
        }
        public AdministrativeUnitJSON converterAdministrativeUnitParaAdministrativeUnitJson(AdministrativeUnit administrativeUnit)
        {
            AdministrativeUnitJSON administrativeUnitJSON = new AdministrativeUnitJSON();
            administrativeUnitJSON.Code = administrativeUnit.Code;
            administrativeUnitJSON.Description = administrativeUnit.Description;
            administrativeUnitJSON.Id = administrativeUnit.Id;
            administrativeUnitJSON.ManagerUnitId = administrativeUnit.ManagerUnitId;
            administrativeUnitJSON.RelationshipAdministrativeUnitId = administrativeUnit.RelationshipAdministrativeUnitId;
            administrativeUnitJSON.Status = administrativeUnit.Status;

            return administrativeUnitJSON;
        }
        public BudgetUnitJSON converterBudgetUnitParaBudgetUnitJson(BudgetUnit budgetUnit)
        {
            BudgetUnitJSON budgetUnitJSON = new Legado.BudgetUnitJSON();
            budgetUnitJSON.Code = budgetUnit.Code;
            budgetUnitJSON.Description = budgetUnit.Description;
            budgetUnitJSON.Id = budgetUnit.Id;
            budgetUnitJSON.InstitutionId = budgetUnit.InstitutionId;
            budgetUnitJSON.Status = budgetUnit.Status;

            return budgetUnitJSON;
        }

        public ManagerUnitJSON converterManagerUnitParaManagerUnitJson(ManagerUnit managerUnit)
        {
            ManagerUnitJSON managerUnitJSON = new ManagerUnitJSON();
            managerUnitJSON.Code = managerUnit.Code;
            managerUnitJSON.Description = managerUnit.Description;
            managerUnitJSON.Id = managerUnit.Id;
            managerUnitJSON.Status = managerUnit.Status;

            return managerUnitJSON;
        }
        public SectionJSON converterSectionParaSectionJson(Section section)
        {
            SectionJSON sectionJSON = new Legado.SectionJSON();
            sectionJSON.AddressId = section.AddressId;
            sectionJSON.AdministrativeUnitId = section.AdministrativeUnitId;
            sectionJSON.Code = section.Code;
            sectionJSON.Description = section.Description;
            sectionJSON.Id = section.Id;
            sectionJSON.ResponsibleId = section.ResponsibleId;
            sectionJSON.Status = section.Status;
            sectionJSON.Telephone = section.Telephone;

            return sectionJSON;
        }

        public AuxiliarAcountJSON converterAuxiliarAcountParaAuxiliarAcountJson(AuxiliaryAccount auxiliaryAccount)
        {
            AuxiliarAcountJSON auxiliarAcountJSON = new AuxiliarAcountJSON();

            auxiliarAcountJSON.BookAccount = auxiliaryAccount.BookAccount;
            auxiliarAcountJSON.Code = auxiliaryAccount.Code;
            auxiliarAcountJSON.Description = auxiliaryAccount.Description;
            auxiliarAcountJSON.Id = auxiliaryAccount.Id;
            auxiliarAcountJSON.Status = auxiliaryAccount.Status;

            return auxiliarAcountJSON;
        }

        // TODO: COMENTADO JULIO
        //public AssetParaJSON converterAssetParaAssetJson(Asset asset)
        //{
        //    AssetParaJSON assetParaJSON = new Legado.AssetParaJSON();
        //    assetParaJSON.Id = asset.Id;
        //    assetParaJSON.InitialId = asset.InitialId;
        //    assetParaJSON.NumberIdentification = asset.NumberIdentification;
        //    assetParaJSON.PartNumberIdentification = asset.PartNumberIdentification;
        //    assetParaJSON.InstitutionId = asset.InstitutionId;
        //    assetParaJSON.BudgetUnitId = asset.BudgetUnitId;
        //    assetParaJSON.ManagerUnitId = asset.ManagerUnitId;
        //    assetParaJSON.AdministrativeUnitId = asset.AdministrativeUnitId;
        //    assetParaJSON.SectionId = asset.SectionId;
        //    assetParaJSON.ResponsibleId = asset.ResponsibleId;
        //    assetParaJSON.AuxiliaryAccountId = asset.AuxiliaryAccountId;
        //    assetParaJSON.OutSourcedId = asset.OutSourcedId;
        //    assetParaJSON.IncorporationTypeId = asset.IncorporationTypeId;
        //    assetParaJSON.StateConservationId = asset.StateConservationId;
        //    assetParaJSON.MovementTypeId = asset.MovementTypeId;
        //    assetParaJSON.SupplierId = asset.SupplierId;
        //    assetParaJSON.InclusionDate = asset.InclusionDate;
        //    assetParaJSON.AcquisitionDate = asset.AcquisitionDate;
        //    assetParaJSON.ManufactureDate = asset.ManufactureDate;
        //    assetParaJSON.DateGuarantee = asset.DateGuarantee;
        //    assetParaJSON.ReceiptTermDate = asset.ReceiptTermDate;
        //    assetParaJSON.InventoryDate = asset.InventoryDate;
        //    assetParaJSON.AssigningAmountDate = asset.AssigningAmountDate;
        //    assetParaJSON.BagDate = asset.BagDate;
        //    assetParaJSON.OutDate = asset.OutDate;
        //    assetParaJSON.NoteGranting = asset.NoteGranting;
        //    assetParaJSON.Status = asset.Status;
        //    assetParaJSON.ValueAcquisition = asset.ValueAcquisition;
        //    assetParaJSON.ValueUpdate = asset.ValueUpdate;
        //    assetParaJSON.NumberPurchaseProcess = asset.NumberPurchaseProcess;
        //    assetParaJSON.NGPB = asset.NGPB;
        //    assetParaJSON.Invoice = asset.Invoice;
        //    assetParaJSON.SerialNumber = asset.SerialNumber;
        //    assetParaJSON.ChassiNumber = asset.ChassiNumber;
        //    assetParaJSON.Brand = asset.Brand;
        //    assetParaJSON.Model = asset.Model;
        //    assetParaJSON.NumberPlate = asset.NumberPlate;
        //    assetParaJSON.AdditionalDescription = asset.AdditionalDescription;
        //    assetParaJSON.HistoricId = asset.HistoricId;
        //    assetParaJSON.Condition = asset.Condition;
        //    assetParaJSON.Rating = asset.Rating;
        //    assetParaJSON.OldInitial = asset.OldInitial;
        //    assetParaJSON.OldNumberIdentification = asset.OldNumberIdentification;
        //    assetParaJSON.OldPartNumberIdentification = asset.OldPartNumberIdentification;
        //    assetParaJSON.ValueOut = asset.ValueOut;
        //    assetParaJSON.LifeCycle = asset.LifeCycle;
        //    assetParaJSON.RateDepreciationMonthly = asset.RateDepreciationMonthly;
        //    assetParaJSON.ResidualValue = asset.ResidualValue;
        //    assetParaJSON.AceleratedDepreciation = asset.AceleratedDepreciation;
        //    assetParaJSON.MaterialItemCode = asset.MaterialItemCode;
        //    assetParaJSON.MaterialItemDescription = asset.MaterialItemDescription;
        //    assetParaJSON.MaterialGroupCode = asset.MaterialGroupCode;
        //    assetParaJSON.Empenho = asset.Empenho;
        //    assetParaJSON.Gerador_Descricao = asset.Gerador_Descricao;

        //    return assetParaJSON;
        //}

        private IList<ResponsibleJson> getResponsaveisDoOrgao(int institutionId)
        {
            IList<ResponsibleJson> responsaveisJson = null;

            contexto.Configuration.LazyLoadingEnabled = false;
            contexto.Configuration.AutoDetectChangesEnabled = false;

            IQueryable<Responsible> query = (from res in contexto.Responsibles
                                             join ua in contexto.AdministrativeUnits on res.AdministrativeUnitId equals ua.Id
                                             join uge in contexto.ManagerUnits on ua.ManagerUnitId equals uge.Id
                                             join uo in contexto.BudgetUnits on uge.BudgetUnitId equals uo.Id
                                             join orgao in contexto.Institutions on uo.InstitutionId equals orgao.Id
                                             where orgao.Id == institutionId
                                             select res)
                                             .Distinct().AsQueryable();

            IList<Responsible> Responsaveis = query.ToList();
            foreach(Responsible responsavel in Responsaveis)
            {

            }

            return responsaveisJson;
        }

        // TODO: COMENTADO JULIO
        //private IList<AuxiliaryAccount> getContaAuxiliares(int institutionId)
        //{
        //    contexto.Configuration.LazyLoadingEnabled = false;
        //    contexto.Configuration.AutoDetectChangesEnabled = false;

        //    IQueryable<AuxiliaryAccount> query = (from aux in contexto.AuxiliaryAccounts
        //                                          join asse in contexto.Assets on aux.Id equals asse.AuxiliaryAccountId
        //                                          join orgao in contexto.Institutions on asse.InstitutionId equals orgao.Id
        //                                          join uo in contexto.BudgetUnits on orgao.Id equals uo.InstitutionId
        //                                          join uge in contexto.ManagerUnits on uo.Id equals uge.BudgetUnitId
        //                                          join ua in contexto.AdministrativeUnits on uge.Id equals ua.ManagerUnitId
        //                                          where orgao.Id == institutionId
        //                                          select aux).Distinct().AsQueryable();



        //    return query.ToList();
        //}

        private IList<OutSourced> getTerceirosDoOrgao(int institutionId)
        {
            contexto.Configuration.LazyLoadingEnabled = false;
            contexto.Configuration.AutoDetectChangesEnabled = false;

            IQueryable<OutSourced> query = (from ter in contexto.OutSourceds
                                            join orgao in contexto.Institutions on ter.InstitutionId equals orgao.Id
                                            join uo in contexto.BudgetUnits on orgao.Id equals uo.InstitutionId
                                            where ter.InstitutionId == institutionId
                                              && ter.BudgetUnitId == uo.Id
                                            select ter).Distinct().AsQueryable();



            return query.ToList();
        }

        private IList<Initial> getSiglasDoOrgao(int institutionId)
        {
            contexto.Configuration.LazyLoadingEnabled = false;
            contexto.Configuration.AutoDetectChangesEnabled = false;

            IQueryable<Initial> query = (from sig in contexto.Initials
                                         join orgao in contexto.Institutions on sig.InstitutionId equals orgao.Id
                                         join uo in contexto.BudgetUnits on orgao.Id equals uo.InstitutionId
                                         where sig.InstitutionId == institutionId
                                           && sig.BudgetUnitId == uo.Id 
                                         select sig).Distinct().AsQueryable();



            return query.ToList();
        }
        //private IList<IncorporationType> getTipoDeIncorporacoesDoOrgao(int institutionId)
        //{
        //    contexto.Configuration.LazyLoadingEnabled = false;
        //    contexto.Configuration.AutoDetectChangesEnabled = false;

        //    IQueryable<IncorporationType> query = (from tip in contexto.IncorporationTypes
        //                                           join orgao in contexto.Institutions on tip.InstitutionId equals orgao.Id
        //                                           join uo in contexto.BudgetUnits on orgao.Id equals uo.InstitutionId
        //                                           where tip.InstitutionId == institutionId
        //                                             && tip.BudgetUnitId == uo.Id 
        //                                           select tip).Distinct().AsQueryable();



        //    return query.ToList();
        //}

      
        public String converterBensPatrimoniasParaJson(IList<AssetParaJSON> assets)
        {
            String retorno = Configuracao.converterObjetoParaJSON<IList<AssetParaJSON>>(assets);

            return retorno;
        }

        // TODO: COMENTADO JULIO
        //public IDictionary<string, IList<RetornoImportacao>> createJsonDosDadosAserExportados(int orgaoId,string caminhoDoArquivoJson)
        //{
        //    mensagensDeRetorno = new Dictionary<string, IList<RetornoImportacao>>();
        //    RetornoImportacao mensagemDeRetorno = new RetornoImportacao();
        //    IList<RetornoImportacao> Retornos = new List<RetornoImportacao>();


        //    //Responsavel

        //    var Responsaveis = getResponsaveisDoOrgao(orgaoId);
        //    var jsonResponsaveis = Configuracao.converterObjetoParaJSON<IList<Responsible>>(null);
        //    System.IO.File.WriteAllText(caminhoDoArquivoJson + "\\Responsavel.json", jsonResponsaveis);
        //    jsonResponsaveis = null;

        //    mensagemDeRetorno = new RetornoImportacao();
        //    mensagemDeRetorno.caminhoDoArquivoDownload = string.Empty;
        //    mensagemDeRetorno.mensagemImportacao = "Responsavel";
        //    mensagemDeRetorno.quantidadeDeRegistros = Responsaveis.Count();
        //    Retornos.Add(mensagemDeRetorno);
        //    mensagemDeRetorno = null;

        //    //Conta Auxiliar
        //    var contasAuxiliares = getContaAuxiliares(orgaoId);
        //    var jsonContasAuxiliares = Configuracao.converterObjetoParaJSON<IList<AuxiliaryAccount>>(contasAuxiliares);
        //    System.IO.File.WriteAllText(caminhoDoArquivoJson + "\\ContaAuxiliar.json", jsonContasAuxiliares);
        //    jsonContasAuxiliares = null;

        //    mensagemDeRetorno = new RetornoImportacao();
        //    mensagemDeRetorno.caminhoDoArquivoDownload = string.Empty;
        //    mensagemDeRetorno.mensagemImportacao = "Conta Auxiliar";
        //    mensagemDeRetorno.quantidadeDeRegistros = contasAuxiliares.Count();
        //    Retornos.Add(mensagemDeRetorno);
        //    mensagemDeRetorno = null;

        //    //Terceiro
        //    var Terceiros = getTerceirosDoOrgao(orgaoId);
        //    var jsonTerceiros = Configuracao.converterObjetoParaJSON<IList<OutSourced>>(Terceiros);
        //    System.IO.File.WriteAllText(caminhoDoArquivoJson + "\\Terceiro.json", jsonTerceiros);
        //    jsonTerceiros = null;

        //    mensagemDeRetorno = new RetornoImportacao();
        //    mensagemDeRetorno.caminhoDoArquivoDownload = string.Empty;
        //    mensagemDeRetorno.mensagemImportacao = "Terceiro";
        //    mensagemDeRetorno.quantidadeDeRegistros = Terceiros.Count();
        //    Retornos.Add(mensagemDeRetorno);
        //    mensagemDeRetorno = null;

        //    //Sigla
        //    var Siglas = getSiglasDoOrgao(orgaoId);
        //    var jsonSiglas = Configuracao.converterObjetoParaJSON<IList<Initial>>(Siglas);
        //    System.IO.File.WriteAllText(caminhoDoArquivoJson + "\\Sigla.json", jsonSiglas);
        //    jsonSiglas = null;

        //    mensagemDeRetorno = new RetornoImportacao();
        //    mensagemDeRetorno.caminhoDoArquivoDownload = string.Empty;
        //    mensagemDeRetorno.mensagemImportacao = "Sigla";
        //    mensagemDeRetorno.quantidadeDeRegistros = Siglas.Count();
        //    Retornos.Add(mensagemDeRetorno);
        //    mensagemDeRetorno = null;

        //    //Tipo de Incorporação
        //    var tiposDeIncorporacoes = getTipoDeIncorporacoesDoOrgao(orgaoId);
        //    var jsonTiposDeIncorporacoes = Configuracao.converterObjetoParaJSON<IList<IncorporationType>>(tiposDeIncorporacoes);
        //    System.IO.File.WriteAllText(caminhoDoArquivoJson + "\\TipoDeIncorporacao.json", jsonTiposDeIncorporacoes);
        //    jsonTiposDeIncorporacoes = null;

        //    mensagemDeRetorno = new RetornoImportacao();
        //    mensagemDeRetorno.caminhoDoArquivoDownload = string.Empty;
        //    mensagemDeRetorno.mensagemImportacao = "Tipo de Incorporação";
        //    mensagemDeRetorno.quantidadeDeRegistros = tiposDeIncorporacoes.Count();
        //    Retornos.Add(mensagemDeRetorno);
        //    mensagemDeRetorno = null;


        //    //Bem Patrimonial
        //    var bensPatrimoniais = getBensPatrimoniasDoOrgao(orgaoId);
        //    var jsonBensPatrimoniais = Configuracao.converterObjetoParaJSON<IList<AssetParaJSON>>(bensPatrimoniais);
        //    System.IO.File.WriteAllText(caminhoDoArquivoJson + "\\BemPatrimonial.json", jsonBensPatrimoniais);
        //    jsonBensPatrimoniais = null;

        //    mensagemDeRetorno = new RetornoImportacao();
        //    mensagemDeRetorno.caminhoDoArquivoDownload = string.Empty;
        //    mensagemDeRetorno.mensagemImportacao = "Bem Patrimonial";
        //    mensagemDeRetorno.quantidadeDeRegistros = bensPatrimoniais.Count();
        //    Retornos.Add(mensagemDeRetorno);
        //    mensagemDeRetorno = null;

        //    mensagensDeRetorno.Add("Exportacao", Retornos);


        //    //var json = converterBensPatrimoniasParaJson(bensPatrimoniais);

        //    //System.IO.File.WriteAllText(caminhoDoArquivoJson, json);

        //    //string jsonReturno = System.IO.File.ReadAllText(caminhoDoArquivoJson);

        //    //IList<AssetParaJSON> list = Configuracao.converterJSONparaObjeto<IList<AssetParaJSON>>(jsonReturno);

        //    return mensagensDeRetorno;
        //}

        // TODO: COMENTADO JULIO
        //public void createExportacao(IList<Responsible> responsaveis,IList<AuxiliaryAccount> contasAuxiliares,IList<OutSourced> terceiros,IList<Initial> siglas,IList<IncorporationType> tiposDeIncorporacoes,IList<AssetParaJSON> bensPatrimoniais)
        //{
        //    foreach(Responsible responsavel in responsaveis)
        //    {
        //        int administrativeUnitId = (from be in contexto.Assets
        //                                    join ad in bensPatrimoniais.Select(x => x.RelatedAdministrativeUnit) on be.RelatedAdministrativeUnit.Code equals ad.Code
        //                                    join ma in bensPatrimoniais.Select(x => x.RelatedManagementUnit) on be.RelatedManagementUnit.Code equals ma.Code
        //                                    join bu in bensPatrimoniais.Select(x => x.RelatedBudgetUnit) on be.RelatedBudgetUnit.Code equals bu.Code
        //                                    join og in bensPatrimoniais.Select(x => x.RelatedInstitution) on be.RelatedInstitution.Code equals og.Code
        //                                    where ad.Code == responsavel.RelatedAdministrativeUnits.Code 
        //                                    select ad.Id
        //                                    ).AsQueryable().FirstOrDefault();

        //        responsavel.AdministrativeUnitId = administrativeUnitId;


        //    } 

        //    foreach(AuxiliaryAccount contaAuxiliar in contasAuxiliares)
        //    {

        //    }

        //    foreach(OutSourced terceiro in terceiros)
        //    {
        //        int intitutionId = (from be in contexto.Assets
        //                            join og in bensPatrimoniais.Select(x => x.RelatedInstitution) on be.RelatedInstitution.Code equals og.Code
        //                            where terceiro.CPFCNPJ == be.RelatedOutSourced.CPFCNPJ
        //                            select og.Id
        //                                 ).AsQueryable().FirstOrDefault();

        //        int budgeUnitId = (from be in contexto.Assets
        //                            join bu in bensPatrimoniais.Select(x => x.RelatedBudgetUnit) on be.RelatedBudgetUnit.Code equals bu.Code
        //                            join og in bensPatrimoniais.Select(x => x.RelatedInstitution) on be.RelatedInstitution.Code equals og.Code
        //                            where terceiro.CPFCNPJ == be.RelatedOutSourced.CPFCNPJ
        //                            select bu.Id
        //                                 ).AsQueryable().FirstOrDefault();

        //        terceiro.BudgetUnitId = budgeUnitId;
        //        terceiro.InstitutionId = intitutionId;
        //    }

        //    foreach(Initial sigla in siglas)
        //    {

        //        int intitutionId = (from be in contexto.Assets
        //                            join og in bensPatrimoniais.Select(x => x.RelatedInstitution) on be.RelatedInstitution.Code equals og.Code
        //                            where sigla.RelatedInstitution.Code == og.Code 
        //                            select og.Id
        //                                 ).AsQueryable().FirstOrDefault();

        //        int budgeUnitId = (from be in contexto.Assets
        //                           join bu in bensPatrimoniais.Select(x => x.RelatedBudgetUnit) on be.RelatedBudgetUnit.Code equals bu.Code
        //                           join og in bensPatrimoniais.Select(x => x.RelatedInstitution) on be.RelatedInstitution.Code equals og.Code
        //                           where sigla.RelatedInstitution.Code == be.RelatedOutSourced.CPFCNPJ
        //                           select bu.Id
        //                                 ).AsQueryable().FirstOrDefault();
        //    }
        //}

    }
}