using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ConfigurationMap : EntityTypeConfiguration<Configuration>
    {
        public ConfigurationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Configuration");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InitialYearMonthAsset).HasColumnName("InitialYearMonthAsset");
            this.Property(t => t.ReferenceYearMonthAsset).HasColumnName("ReferenceYearMonthAsset");

        }
    }
}
