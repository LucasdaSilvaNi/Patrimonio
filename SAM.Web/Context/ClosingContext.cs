using System.Data.Entity;
using SAM.Web.Models.Mapping;
using System.Data.SqlClient;
using System.Configuration;
using SAM.Web.Models;

namespace SAM.Web.Context
{
    public class ClosingContext : DbContext
    {
        public ClosingContext() : base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.DesabilitarAutoDetectChanges();
            this.DesabilitarLazyLoading();
            this.DesabilitarProxyCreation();
        }

        public DbSet<AccountingClosing> AccountingClosings { get; set; }
        public DbSet<AccountingClosingExcluidos> AccountingClosingExcluidos { get; set; }
        public DbSet<ManagerUnit> ManagerUnits { get; set; }

        public void DesabilitarAutoDetectChanges()
        {
            this.Configuration.AutoDetectChangesEnabled = false;
        }
        public void HabilitarAutoDetectChanges()
        {
            this.Configuration.AutoDetectChangesEnabled = true;
        }
        public void HabilitarProxyCreation()
        {
            this.Configuration.ProxyCreationEnabled = true;
        }
        public void DesabilitarProxyCreation()
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public void DesabilitarLazyLoading()
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
        public void HabilitarLazyLoading()
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Configurations.Add(new AccountingClosingMap());
            modelBuilder.Configurations.Add(new AccountingClosingExcluidosMap());
            modelBuilder.Configurations.Add(new ManagerUnitMap());
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
       
    
}