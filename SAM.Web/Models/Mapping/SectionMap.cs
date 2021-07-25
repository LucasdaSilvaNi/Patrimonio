using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class SectionMap : EntityTypeConfiguration<Section>
    {
        public SectionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(120);

            this.Property(t => t.Telephone)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Section");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AdministrativeUnitId).HasColumnName("AdministrativeUnitId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.ResponsibleId).HasColumnName("ResponsibleId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.Telephone).HasColumnName("Telephone");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedAddress)
                .WithMany(t => t.RelatedSections)
                .HasForeignKey(d => d.AddressId);
            this.HasRequired(t => t.RelatedResponsible)
                .WithMany(t => t.Sections)
                .HasForeignKey(d => d.ResponsibleId);
            this.HasRequired(t => t.RelatedAdministrativeUnit)
                .WithMany(t => t.Sections)
                .HasForeignKey(d => d.AdministrativeUnitId);

        }
    }
}
