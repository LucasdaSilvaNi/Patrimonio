using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MaterialClassMap : EntityTypeConfiguration<MaterialClass>
    {
        public MaterialClassMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(120);

            // Table & Column Mappings
            this.ToTable("MaterialClass");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.MaterialGroupId).HasColumnName("MaterialGroupId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedMaterialGroup)
                .WithMany(t => t.MaterialClasses)
                .HasForeignKey(d => d.MaterialGroupId);

        }
    }
}
