using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SAM.Web.Models.Mapping;
using System;
using SAM.Web.Common;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public partial class SAMContext : DbContext
    {
        public SAMContext(int timeout)
            : base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.Database.CommandTimeout = timeout;
        }

        public SAMContext()
            : base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.Database.CommandTimeout = 360;
        }

        public void SalvaSemNecessiadeDeGravarLog() {
            base.SaveChanges();
        }

        public void SalvaEdicao(int assetId, int assetMovementsId)
        {              
            string valorOriginalSalvoNoBanco, valorAtual, nomeDoCampo;
            int IdUsuarioLogado = UserCommon.CurrentUser().Id;
            List<LogAlteracaoDadosBP> listaDeAlteracoes = null;
            System.Reflection.PropertyInfo buscaPropiedade;
            DisplayAttribute[] buscaDataAnnotationDisplay;
            DateTime agora = DateTime.Now;

            var entidades = this.ChangeTracker.Entries();

            foreach (DbEntityEntry entidade in entidades)
            {
                if (entidade.State == EntityState.Modified)
                {
                    foreach (var nomePropiedade in entidade.CurrentValues.PropertyNames)
                    {

                        valorOriginalSalvoNoBanco = entidade.OriginalValues[nomePropiedade] == null ? "" : entidade.OriginalValues[nomePropiedade].ToString();
                        valorAtual = entidade.CurrentValues[nomePropiedade] == null ? "" : entidade.CurrentValues[nomePropiedade].ToString();

                        if (valorOriginalSalvoNoBanco != valorAtual)
                        {
                            if (listaDeAlteracoes == null)
                                listaDeAlteracoes = new List<LogAlteracaoDadosBP>();

                            buscaPropiedade = entidade.Entity.GetType().GetProperty(nomePropiedade);
                            buscaDataAnnotationDisplay = (DisplayAttribute[])buscaPropiedade.GetCustomAttributes(typeof(DisplayAttribute), false);

                            if (buscaDataAnnotationDisplay.Length == 0)
                                nomeDoCampo = nomePropiedade;
                            else
                                nomeDoCampo = buscaDataAnnotationDisplay[0].Name;

                            listaDeAlteracoes.Add(new LogAlteracaoDadosBP
                            {
                                AssetId = assetId,
                                AssetMovementId = assetMovementsId,
                                Campo = nomeDoCampo,
                                ValorAntigo = valorOriginalSalvoNoBanco,
                                ValorNovo = valorAtual,
                                UserId = IdUsuarioLogado,
                                DataHora = agora
                            });
                        }
                    }
                }

                if (listaDeAlteracoes != null && listaDeAlteracoes.Count > 0)
                    this.LogAlteracaoDadosBPs.AddRange(listaDeAlteracoes);
            }

            this.SaveChanges();
        }

        private int _savechanges;
        public override int SaveChanges()
        {
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Database.Connection.ConnectionString);
            System.Data.SqlClient.SqlCommand cmd;
            _savechanges = 0;

            try
            {
                Historico _historico = new Historico();
                HistoricoCampo _historicoCampo = new HistoricoCampo();
                //_savechanges = base.SaveChanges();

                var _Entidades = (base.ChangeTracker.Entries());
                foreach (var _entidade in _Entidades)
                {
                    var nomeTabela = _entidade.Entity.GetType().BaseType.Name;
                    if(nomeTabela == "Object")
                        nomeTabela = _entidade.Entity.GetType().Name;

                    // Trata excecoes de gravacao de log
                    if (Excecoes().Contains(nomeTabela))
                    {
                        _savechanges += base.SaveChanges();
                        return _savechanges;
                    }

                    if (_entidade.State != EntityState.Unchanged)
                    {
                        if (_entidade.State == EntityState.Modified)
                        {
                            var _id = _entidade.Entity.GetType().GetProperty("Id");
                            int id = int.Parse(_id.GetValue(_entidade.Entity, null).ToString());

                            cmd = new System.Data.SqlClient.SqlCommand("select * from [" + nomeTabela + "] where Id = " + id, con);

                            con.Open();

                            System.Data.DataTable retorno = new System.Data.DataTable();
                            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
                            da.Fill(retorno);
                            con.Close();

                            _historico = AdicionarHistorico(_entidade, nomeTabela);

                            foreach (System.Data.DataRow item in retorno.Rows)
                            {
                                foreach (System.Data.DataColumn dc in retorno.Columns)
                                {
                                    _historicoCampo = new HistoricoCampo();
                                    string nomeCampo = item.Table.Columns[dc.Caption].Caption;
                                    var ValorCampoAntigo = item[dc].ToString();
                                    var _CampoNovo = _entidade.Entity.GetType().GetProperty(nomeCampo);
                                    var ValorCampoNovo = _CampoNovo.GetValue(_entidade.Entity, null) == null ? "" : _CampoNovo.GetValue(_entidade.Entity, null).ToString();

                                    if (ValorCampoAntigo != ValorCampoNovo && !ValorCampoNovo.Contains("System."))
                                    {
                                        _historicoCampo.HistoricoId = _historico.Id;
                                        _historicoCampo.Campo = nomeCampo;
                                        _historicoCampo.ValorAntigo = ValorCampoAntigo;
                                        _historicoCampo.ValorNovo = ValorCampoNovo;

                                        HistoricoCampos.Add(_historicoCampo);
                                        base.SaveChanges();
                                    }
                                }
                            }

                            con.Close();
                        }
                        else
                        {
                            _historico = AdicionarHistorico(_entidade, nomeTabela);

                            foreach (var _propriedade in _entidade.Entity.GetType().GetProperties())
                            {
                                if (_propriedade != null)
                                {
                                    var ValorCampoNovo = _propriedade.GetValue(_entidade.Entity, null) == null ? null : _propriedade.GetValue(_entidade.Entity, null).ToString();
                                    if (ValorCampoNovo == null || !ValorCampoNovo.Contains("System."))
                                    {
                                        _historicoCampo = new HistoricoCampo();

                                        _historicoCampo.HistoricoId = _historico.Id;
                                        _historicoCampo.Campo = _propriedade.Name;
                                        _historicoCampo.ValorNovo = ValorCampoNovo;
                                        _historicoCampo.ValorAntigo = null;
                                        HistoricoCampos.Add(_historicoCampo);
                                        base.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
                return _savechanges;
            }
            catch (Exception ex)
            {
                con.Close();
                _savechanges += base.SaveChanges();
                return _savechanges;
            }
        }
        private List<string> Excecoes()
        {
            List<string> _excecoes = new List<string>();
            _excecoes.Add("ErroLog");
            _excecoes.Add("Object");
            _excecoes.Add("AssetMovements");
            _excecoes.Add("Closing");
            _excecoes.Add("MonthlyDepreciation");
            _excecoes.Add("Historico");
            _excecoes.Add("HistoricoCampo");
            _excecoes.Add("ELMAH_Error");
            _excecoes.Add("RelationshipTransactionProfile");
            _excecoes.Add("ItemInventario");
            _excecoes.Add("AuditoriaIntegracao");
            _excecoes.Add("LogAlteracaoDadosBP");
            _excecoes.Add("HouveAlteracaoContabil");
            _excecoes.Add("BPsARealizaremReclassificacaoContabil");

            return _excecoes;
        }

        private Historico AdicionarHistorico(DbEntityEntry _entidade, string nomeTabela)
        {
            Historico _historico = new Historico();
            var PropriedadeId = _entidade.Entity.GetType().GetProperty("Id");

            _historico.Acao = _entidade.State.ToString();
            _historico.Tabela = nomeTabela;
            _savechanges = base.SaveChanges(); // Salvo para gerar o Id ta tabela que foi adicionada

            var _HistoricoId = PropriedadeId.GetValue(_entidade.Entity, null);
            _historico.TabelaId = _HistoricoId == null ? 0 : int.Parse(_HistoricoId.ToString());

            _historico.Data = DateTime.Now;
            _historico.Login = UserCommon.CurrentUser().CPF;

            Historicos.Add(_historico);
            base.SaveChanges();
            return _historico;
        }

        public DbSet<AccountingClosing> AccountingClosings { get; set; }
        public DbSet<AccountingClosingExcluidos> AccountingClosingExcluidos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Addressee> Addressees { get; set; }
        public DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetMovements> AssetMovements { get; set; }
        public DbSet<AuxiliaryAccount> AuxiliaryAccounts { get; set; }
        public DbSet<BPsExcluidos> BPsExcluidos { get; set; }
        public DbSet<BPsARealizaremReclassificacaoContabil> BPsARealizaremReclassificacaoContabeis { get; set; }
        public DbSet<BudgetUnit> BudgetUnits { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<DepreciationAccount> DepreciationAccounts { get; set; }
        public DbSet<DepreciationAccountingClosing> DepreciationAccountingClosings { get; set; }
        public DbSet<HouveAlteracaoContabil> HouveAlteracaoContabeis { get; set; }
        public DbSet<Initial> Initials { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<LogAlteracaoDadosBP> LogAlteracaoDadosBPs { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<ManagerUnit> ManagerUnits { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialClass> MaterialClasses { get; set; }
        public DbSet<MaterialGroup> MaterialGroups { get; set; }
        public DbSet<MaterialItem> MaterialItems { get; set; }
        public DbSet<MaterialSubItem> MaterialSubItems { get; set; }
        public DbSet<MonthlyDepreciation> MonthlyDepreciations { get; set; }
        public DbSet<MovementType> MovementTypes { get; set; }
        public DbSet<OutSourced> OutSourceds { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<RelationshipProfileLevel> RelationshipProfileLevels { get; set; }
        public DbSet<RelationshipUserProfile> RelationshipUserProfiles { get; set; }
        public DbSet<RelationshipUserProfileInstitution> RelationshipUserProfileInstitutions { get; set; }
        public DbSet<RelationshipAuxiliaryAccountItemGroup> RelationshipAuxiliaryAccountItemGroups { get; set; }
        public DbSet<RelationshipAuxiliaryAccountMovementType> RelationshipAuxiliaryAccountMovementTypes { get; set; }
        public DbSet<SaldoContabilAtualUGEContaDepreciacao> SaldoContabilAtualUGEContaDepreciacoes { get; set; }
        public DbSet<SiafemCalendar> SiafemCalendars { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<SpendingOrigin> SpendingOrigins { get; set; }
        public DbSet<StateConservation> StateConservations { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplyUnit> SupplyUnits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AssetNumberIdentification> AssetNumberIdentifications { get; set; }
        public DbSet<Repair> Repair { get; set; }
        public DbSet<Mobile> Mobiles { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<ItemInventario> ItemInventarios { get; set; }
        public DbSet<EstadoItemInventario> EstadoItemInventarios { get; set; }
        public DbSet<Exchange> Exchange { get; set; }
        public DbSet<ShortDescriptionItem> ShortDescriptionItens { get; set; }
        public DbSet<ErroLog> ErroLogs { get; set; }
        public DbSet<Responsible> Responsibles { get; set; }
        public DbSet<GroupMoviment> GroupMoviments { get; set; }
        public DbSet<Historico> Historicos { get; set; }
        public DbSet<HistoricoCampo> HistoricoCampos { get; set; }
        public DbSet<ItemSiafisico> ItemSiafisicos { get; set; }
        public DbSet<AuditoriaIntegracao> AuditoriaIntegracoes { get; set; }
        public DbSet<UGEDepreciaramAbrilDoisMilVinte> UGEDepreciaramAbrilDoisMilVintes { get; set; }


        public DbSet<EventServiceContabilizaSP> EventServicesContabilizaSPs { get; set; }
        public DbSet<MetaDataTypeServiceContabilizaSP> MetaDataTypesServicesContabilizaSPs { get; set; }
        public DbSet<NotaLancamentoPendenteSIAFEM> NotaLancamentoPendenteSIAFEMs { get; set; }
        public DbSet<HistoricoValoresDecreto> HistoricoValoresDecretos { get; set; }
        public DbSet<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao> Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos { get; set; }
        public DbSet<DepreciationMaterialItem> DepreciationMaterialItems { get; set; }

        // Comentar para poder gerar as Views. Problema com o Update 4 do Visual Studio. 
        // E compilar antes de gerar, senão apresenta o mesmo erro.
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AccountingClosingMap());
            modelBuilder.Configurations.Add(new AccountingClosingExcluidosMap());
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new AddresseeMap());
            modelBuilder.Configurations.Add(new AdministrativeUnitMap());
            modelBuilder.Configurations.Add(new AssetMap());
            modelBuilder.Configurations.Add(new AssetMovementsMap());
            modelBuilder.Configurations.Add(new AuxiliaryAccountMap());
            modelBuilder.Configurations.Add(new BudgetUnitMap());
            modelBuilder.Configurations.Add(new BPsExcluidosMap());
            modelBuilder.Configurations.Add(new BPsARealizaremReclassificacaoContabilMap());
            modelBuilder.Configurations.Add(new ClosingMap());
            modelBuilder.Configurations.Add(new ConfigurationMap());
            modelBuilder.Configurations.Add(new CostCenterMap());
            modelBuilder.Configurations.Add(new DepreciationAccountMap());
            modelBuilder.Configurations.Add(new DepreciationAccountingClosingMap());
            modelBuilder.Configurations.Add(new DepreciationMaterialItemMap());
            modelBuilder.Configurations.Add(new HouveAlteracaoContabilMap());
            modelBuilder.Configurations.Add(new HistoricoValoresDecretoMap());
            modelBuilder.Configurations.Add(new InitialMap());
            modelBuilder.Configurations.Add(new InstitutionMap());
            modelBuilder.Configurations.Add(new LevelMap());
            modelBuilder.Configurations.Add(new LogAlteracaoDadosBPMap());
            modelBuilder.Configurations.Add(new ManagedSystemMap());
            modelBuilder.Configurations.Add(new ManagerMap());
            modelBuilder.Configurations.Add(new ManagerUnitMap());
            modelBuilder.Configurations.Add(new MaterialMap());
            modelBuilder.Configurations.Add(new MaterialClassMap());
            modelBuilder.Configurations.Add(new MaterialGroupMap());
            modelBuilder.Configurations.Add(new MaterialItemMap());
            modelBuilder.Configurations.Add(new MaterialSubItemMap());
            modelBuilder.Configurations.Add(new MonthlyDepreciationMap());
            modelBuilder.Configurations.Add(new MovementTypeMap());
            modelBuilder.Configurations.Add(new OutSourcedMap());
            modelBuilder.Configurations.Add(new ProfileMap());
            modelBuilder.Configurations.Add(new RelationshipAuxiliaryAccountItemGroupMap());
            modelBuilder.Configurations.Add(new RelationshipProfileLevelMap());
            modelBuilder.Configurations.Add(new RelationshipUserProfileMap());
            modelBuilder.Configurations.Add(new RelationshipUserProfileInstitutionMap());
            modelBuilder.Configurations.Add(new RelationshipAuxiliaryAccountMovementTypeMap());
            modelBuilder.Configurations.Add(new ResponsibleMap());
            modelBuilder.Configurations.Add(new SaldoContabilAtualUGEContaDepreciacaoMap());
            modelBuilder.Configurations.Add(new SiafemCalendarMap());
            modelBuilder.Configurations.Add(new SectionMap());
            modelBuilder.Configurations.Add(new SignatureMap());
            modelBuilder.Configurations.Add(new SpendingOriginMap());
            modelBuilder.Configurations.Add(new StateConservationMap());
            modelBuilder.Configurations.Add(new SupplierMap());
            modelBuilder.Configurations.Add(new SupplyUnitMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RelationshipProfileManagedSystemMap());
            modelBuilder.Configurations.Add(new AssetNumberIdentificationMap());
            modelBuilder.Configurations.Add(new RepairMap());
            modelBuilder.Configurations.Add(new ExchangeMap());
            modelBuilder.Configurations.Add(new MobileMap());
            modelBuilder.Configurations.Add(new InventarioMap());
            modelBuilder.Configurations.Add(new ItemInventarioMap());
            modelBuilder.Configurations.Add(new EstadoItemInventarioMap());
            modelBuilder.Configurations.Add(new ShortDescriptionItemMap());
            modelBuilder.Configurations.Add(new ErroLogMap());
            modelBuilder.Configurations.Add(new GroupMovimentMap());
            modelBuilder.Configurations.Add(new HistoricoMap());
            modelBuilder.Configurations.Add(new HistoricoCampoMap());
            modelBuilder.Configurations.Add(new SupportMap());
            modelBuilder.Configurations.Add(new HistoricSupportMap());
            modelBuilder.Configurations.Add(new ItemSiafisicoMap());
            modelBuilder.Configurations.Add(new AuditoriaIntegracaoMap());
            modelBuilder.Configurations.Add(new UGEDepreciaramAbrilDoisMilVinteMap());


            modelBuilder.Configurations.Add(new EventServiceContabilizaSPMap());
            modelBuilder.Configurations.Add(new MetaDataTypeServiceContabilizaSPMap());
            modelBuilder.Configurations.Add(new NotaLancamentoPendenteSIAFEMMap());
        }
    }
}
