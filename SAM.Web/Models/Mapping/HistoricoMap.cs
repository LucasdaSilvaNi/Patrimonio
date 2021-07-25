using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class HistoricoMap : EntityTypeConfiguration<Historico>
    {
        public HistoricoMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("Historico");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Acao).HasColumnName("Acao");
            this.Property(t => t.Tabela).HasColumnName("Tabela");
            this.Property(t => t.TabelaId).HasColumnName("TabelaId");
            this.Property(t => t.Data).HasColumnName("Data");
            this.Property(t => t.Login).HasColumnName("Login");
        }
    }
}