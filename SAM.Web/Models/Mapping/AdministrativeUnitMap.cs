using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AdministrativeUnitMap : EntityTypeConfiguration<AdministrativeUnit>
    {
        public AdministrativeUnitMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(120);

            // Table & Column Mappings
            this.ToTable("AdministrativeUnit");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.RelationshipAdministrativeUnitId).HasColumnName("RelationshipAdministrativeUnitId");
            this.Property(t => t.Status).HasColumnName("Status");

            this.HasRequired(t => t.RelatedManagerUnit)
                .WithMany(t => t.AdministrativeUnits)
                .HasForeignKey(d => d.ManagerUnitId);
        }
    }
}
