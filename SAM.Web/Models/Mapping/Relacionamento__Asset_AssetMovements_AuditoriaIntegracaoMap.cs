using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class Relacionamento__Asset_AssetMovements_AuditoriaIntegracaoMap : EntityTypeConfiguration<Relacionamento__Asset_AssetMovements_AuditoriaIntegracao>
    {
        public Relacionamento__Asset_AssetMovements_AuditoriaIntegracaoMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Relacionamento__Asset_AssetMovements_AuditoriaIntegracao");

            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AssetId).HasColumnName("AssetId");
            this.Property(t => t.AssetMovementsId).HasColumnName("AssetMovementsId");
            this.Property(t => t.AuditoriaIntegracaoId).HasColumnName("AuditoriaIntegracaoId");
        }
    }
}