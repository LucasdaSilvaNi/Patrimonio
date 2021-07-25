using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class InventarioMap : EntityTypeConfiguration<Inventario>
    {
        public InventarioMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Descricao)
                .HasMaxLength(120);

            this.Property(t => t.Usuario)
            .HasMaxLength(11);

            // Table & Column Mappings
            this.ToTable("Inventario");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Descricao).HasColumnName("Descricao");
            this.Property(t => t.DataInventario).HasColumnName("DataInventario");
            this.Property(t => t.UaId).HasColumnName("UaId");
            this.Property(t => t.DivisaoId).HasColumnName("DivisaoId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.ResponsavelId).HasColumnName("ResponsavelId");
            this.Property(t => t.Usuario).HasColumnName("Usuario");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedSection)
                .WithMany(t => t.Inventarios)
                .HasForeignKey(d => d.DivisaoId);

            this.HasRequired(t => t.RelatedAdministrativeUnit)
            .WithMany(t => t.Inventarios)
            .HasForeignKey(d => d.UaId);

            this.HasRequired(t => t.RelatedResponsible)
            .WithMany(t => t.Inventarios)
            .HasForeignKey(d => d.ResponsavelId);

            this.HasRequired(t => t.RelatedUser)
            .WithMany(t => t.Inventarios)
            .HasForeignKey(d => d.UserId);
        }
    }
}