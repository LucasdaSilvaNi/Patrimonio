using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ResponsibleMap : EntityTypeConfiguration<Responsible>
    {
        public ResponsibleMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Position)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("Responsible");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Position).HasColumnName("Position");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.AdministrativeUnitId).HasColumnName("AdministrativeUnitId");
            this.Property(t => t.CPF).HasColumnName("CPF");
            this.Property(t => t.Email).HasColumnName("Email");

            // Relationships
            this.HasRequired(t => t.RelatedAdministrativeUnits)
                .WithMany(t => t.Responsibles)
                .HasForeignKey(d => d.AdministrativeUnitId);

        }
    }
}
