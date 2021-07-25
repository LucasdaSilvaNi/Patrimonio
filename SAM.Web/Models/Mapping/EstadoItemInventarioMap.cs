using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class EstadoItemInventarioMap : EntityTypeConfiguration<EstadoItemInventario>
    {
        public EstadoItemInventarioMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Descricao)
                .HasMaxLength(120);

            // Table & Column Mappings
            this.ToTable("EstadoItemInventario");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Descricao).HasColumnName("Descricao");
        }
    }
}