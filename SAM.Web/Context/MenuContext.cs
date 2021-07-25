using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using SAM.Web.Models;
using SAM.Web.Models.Mapping;

namespace SAM.Web.Context
{
    public partial class MenuContext : DbContext
    {
        public MenuContext(): base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.Database.CommandTimeout = 120;
        }

        public DbSet<ManagedSystem> ManagedSystems { get; set; }
        public DbSet<RelationshipProfileManagedSystem> RelationshipProfileManagedSystems { get; set; }
        public DbSet<RelationshipModuleProfile> RelationshipModuleProfiles { get; set; }
        public DbSet<Module> Modules { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RelationshipProfileManagedSystemMap());
            modelBuilder.Configurations.Add(new RelationshipModuleProfileMap());
            modelBuilder.Configurations.Add(new ManagedSystemMap());
            modelBuilder.Configurations.Add(new ModuleMap());
        }
    }
}