using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using SAM.Web.Models;
using SAM.Web.Models.Mapping;

namespace SAM.Web.Context
{
    public class ItemMaterialContext : DbContext
    {
        public ItemMaterialContext(): base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.Database.CommandTimeout = 180;
        }

        public DbSet<ItemSiafisico> ItemSiafisicos { get; set; }
        public DbSet<MaterialGroup> MaterialGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ItemSiafisicoMap());
            modelBuilder.Configurations.Add(new MaterialGroupMap());
        }
    }
}