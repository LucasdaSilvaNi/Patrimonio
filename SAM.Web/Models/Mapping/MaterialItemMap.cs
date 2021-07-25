using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MaterialItemMap : EntityTypeConfiguration<MaterialItem>
    {
        public MaterialItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(120);

            // Table & Column Mappings
            this.ToTable("MaterialItem");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");

            // Relationships
            this.HasRequired(t => t.RelatedMaterial)
                .WithMany(t => t.MaterialItems)
                .HasForeignKey(d => d.MaterialId);

        }
    }
}
