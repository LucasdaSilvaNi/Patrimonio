using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using SAM.Web.Models;
using SAM.Web.Models.Mapping;

namespace SAM.Web.Context
{
    public class StructureContext : DbContext
    {
        public StructureContext(): base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.Database.CommandTimeout = 180;
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<TypeTransaction> TypeTransactions { get; set; }
        public DbSet<ManagedSystem> ManagedSystems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ModuleMap());
            modelBuilder.Configurations.Add(new TypeTransactionMap());
            modelBuilder.Configurations.Add(new ManagedSystemMap());
            modelBuilder.Configurations.Add(new TransactionMap());
        }
    }
}