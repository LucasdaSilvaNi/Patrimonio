using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ImportacaoPlanilhaMap : EntityTypeConfiguration<ImportacaoPlanilha>
    {
        public ImportacaoPlanilhaMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("ImportacaoPlanilha");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.NomeArquivo).HasColumnName("NomeArquivo");
            this.Property(t => t.Processado).HasColumnName("Processado");
            this.Property(t => t.Login_Importacao).HasColumnName("Login_Importacao");
            this.Property(t => t.Data_Importacao).HasColumnName("Data_Importacao");
            this.Property(t => t.Login_Processamento).HasColumnName("Login_Processamento");
            this.Property(t => t.Data_Processamento).HasColumnName("Data_Processamento");

            //// Relationships
            //this.HasRequired(t => t.RelatedMovementType)
            //    .WithMany(t => t.Assets)
            //    .HasForeignKey(d => d.MovementTypeId);

        }
    }
}
