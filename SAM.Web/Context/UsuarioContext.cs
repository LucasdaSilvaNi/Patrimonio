using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System;
using SAM.Web.Common;
using System.Collections.Generic;
using System.Data.Common;
using SAM.Web.Models;
using SAM.Web.Models.Mapping;

namespace SAM.Web.Context
{
    public class UsuarioContext : DbContext
    {
        public UsuarioContext(): base(new SqlConnection(ConfigurationManager.ConnectionStrings["SAMContext"].ConnectionString), false)
        {
            this.Database.CommandTimeout = 180;
        }

        public DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }
        public DbSet<BudgetUnit> BudgetUnits { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<ManagerUnit> ManagerUnits { get; set; }
        public DbSet<RelationshipUserProfile> RelationshipUserProfiles { get; set; }
        public DbSet<RelationshipUserProfileInstitution> RelationshipUserProfileInstitutions { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AdministrativeUnitMap());
            modelBuilder.Configurations.Add(new BudgetUnitMap());
            modelBuilder.Configurations.Add(new InstitutionMap());
            modelBuilder.Configurations.Add(new ManagerUnitMap());
            modelBuilder.Configurations.Add(new RelationshipUserProfileMap());
            modelBuilder.Configurations.Add(new RelationshipUserProfileInstitutionMap());
            modelBuilder.Configurations.Add(new ProfileMap());
            modelBuilder.Configurations.Add(new SectionMap());
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}