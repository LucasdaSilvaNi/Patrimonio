using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class InstitutionMap : EntityTypeConfiguration<Institution>
    {
        public InstitutionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(120);
            this.Property(t => t.NameManagerReduced)
                .IsOptional()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Institution");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ManagerCode).HasColumnName("ManagerCode");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.NameManagerReduced).HasColumnName("NameManagerReduced");
            this.Property(t => t.flagImplantado).HasColumnName("flagImplantado");
            this.Property(t => t.flagIntegracaoSiafem).HasColumnName("flagIntegracaoSiafem");
        }
    }
}
