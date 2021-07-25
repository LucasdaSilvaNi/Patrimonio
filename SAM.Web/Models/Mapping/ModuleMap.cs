using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
public class ModuleMap : EntityTypeConfiguration<Module>
    {
        public ModuleMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.Path)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Module");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ManagedSystemId).HasColumnName("ManagedSystemId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.MenuName).HasColumnName("MenuName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.ParentId).HasColumnName("ParentId");
            this.Property(t => t.Sequence).HasColumnName("Sequence");

            // Relationships
            this.HasOptional(t => t.RelatedModule)
                .WithMany(t => t.Modules)
                .HasForeignKey(d => d.ParentId);
            this.HasRequired(t => t.RelatedManagedSystem)
                .WithMany(t => t.Modules)
                .HasForeignKey(d => d.ManagedSystemId);

        }
    }
}
