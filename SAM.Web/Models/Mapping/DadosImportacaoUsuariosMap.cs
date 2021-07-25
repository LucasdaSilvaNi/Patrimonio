using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class DadosImportacaoUsuarioMap : EntityTypeConfiguration<DadosImportacaoUsuario>
    {
        public DadosImportacaoUsuarioMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("DadosImportacaoUsuario");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Data_Importacao).HasColumnName("Data_Importacao");
            this.Property(t => t.CARGA_SEQ).HasColumnName("CARGA_SEQ");
            this.Property(t => t.PERFIL).HasColumnName("PERFIL");
            this.Property(t => t.USUARIO_CPF).HasColumnName("USUARIO_CPF");
            this.Property(t => t.USUARIO_NOME_USUARIO).HasColumnName("USUARIO_NOME_USUARIO");
            this.Property(t => t.ORGAO_CODIGO).HasColumnName("ORGAO_CODIGO");
            this.Property(t => t.UO_CODIGO).HasColumnName("UO_CODIGO");

            //Relationships
            this.HasRequired(t => t.ImportacaoPlanilha)
                .WithMany(t => t.DadosImportacaoUsuarios)
                .HasForeignKey(d => d.ImportacaoPlanilhaId);
        }
    }
}
