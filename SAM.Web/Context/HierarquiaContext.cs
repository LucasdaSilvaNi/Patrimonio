using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using SAM.Web.Models;
using SAM.Web.Models.Mapping;

namespace SAM.Web.Context
{
    public partial class HierarquiaContext : DbContext
    {
        public HierarquiaContext(): base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.Database.CommandTimeout = 180;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        public DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<BudgetUnit> BudgetUnits { get; set; }
        public DbSet<ManagerUnit> ManagerUnits { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Section> Sections { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AdministrativeUnitMap());
            modelBuilder.Configurations.Add(new InstitutionMap());
            modelBuilder.Configurations.Add(new BudgetUnitMap());
            modelBuilder.Configurations.Add(new ManagerUnitMap());
            modelBuilder.Configurations.Add(new ProfileMap());
            modelBuilder.Configurations.Add(new SectionMap());
        }
    }
}