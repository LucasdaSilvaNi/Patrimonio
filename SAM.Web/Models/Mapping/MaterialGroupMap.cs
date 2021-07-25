using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MaterialGroupMap : EntityTypeConfiguration<MaterialGroup>
    {
        public MaterialGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(120);

            // Table & Column Mappings
            this.ToTable("MaterialGroup");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.LifeCycle).HasColumnName("LifeCycle");
            this.Property(t => t.RateDepreciationMonthly).HasColumnName("RateDepreciationMonthly");
            this.Property(t => t.ResidualValue).HasColumnName("ResidualValue");
        }
    }
}
