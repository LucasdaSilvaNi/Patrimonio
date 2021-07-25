using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class HistoricoCampoMap : EntityTypeConfiguration<HistoricoCampo>
    {
        public HistoricoCampoMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("HistoricoCampo");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.HistoricoId).HasColumnName("HistoricoId");
            this.Property(t => t.Campo).HasColumnName("Campo");
            this.Property(t => t.ValorAntigo).HasColumnName("ValorAntigo");
            this.Property(t => t.ValorNovo).HasColumnName("ValorNovo");

            // Relationships
            this.HasRequired(t => t.RelatedHistorico)
                .WithMany(t => t.HistoricoCampos)
                .HasForeignKey(d => d.HistoricoId);
        }
    }
}