using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using SAM.Web.Common;
using SAM.Web.Models;
using SAM.Web.Models.Mapping;

namespace SAM.Web.Context
{
    public class ContaContabilContext : DbContext
    {
        public ContaContabilContext() : base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
        }

        public DbSet<AssetMovements> AssetMovements { get; set; }
        public DbSet<AuxiliaryAccount> AuxiliaryAccounts { get; set; }
        public DbSet<DepreciationAccount> DepreciationAccounts { get; set; }
        public DbSet<HistoricoCampo> HistoricoCampos { get; set; }
        public DbSet<Historico> Historicos { get; set; }
        public DbSet<MaterialGroup> MaterialGroups { get; set; }
        public DbSet<RelationshipAuxiliaryAccountItemGroup> RelationshipAuxiliaryAccountItemGroups { get; set; }

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
                    if (nomeTabela == "Object")
                        nomeTabela = _entidade.Entity.GetType().Name;

                    // Trata excecoes de gravacao de log
                    if (Excecoes().Contains(nomeTabela))
                    {
                        _savechanges = base.SaveChanges();
                        return _savechanges;
                    }

                    if (_entidade.State == EntityState.Modified)
                    {
                        var _id = _entidade.Entity.GetType().GetProperty("Id");
                        int id = int.Parse(_id.GetValue(_entidade.Entity, null).ToString());

                        //var tabelaType = Type.GetType("SAM.Web.Models."+ nomeTabela);
                        //var f = base.GetType().GetProperty(nomeTabela + "s");


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
                return _savechanges;
            }
            catch (Exception ex)
            {
                con.Close();
                _savechanges = base.SaveChanges();
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
            _excecoes.Add("Historico");
            _excecoes.Add("HistoricoCampo");
            _excecoes.Add("ELMAH_Error");
            _excecoes.Add("RelationshipTransactionProfile");
            _excecoes.Add("ItemInventario");
            _excecoes.Add("AuditoriaIntegracao");

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AssetMovementsMap());
            modelBuilder.Configurations.Add(new AuxiliaryAccountMap());
            modelBuilder.Configurations.Add(new DepreciationAccountMap());
            modelBuilder.Configurations.Add(new HistoricoCampoMap());
            modelBuilder.Configurations.Add(new HistoricoMap());
            modelBuilder.Configurations.Add(new MaterialGroupMap());
            modelBuilder.Configurations.Add(new RelationshipAuxiliaryAccountItemGroupMap());
        }
    }
}