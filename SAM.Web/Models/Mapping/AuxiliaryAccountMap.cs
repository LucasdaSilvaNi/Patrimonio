using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AuxiliaryAccountMap : EntityTypeConfiguration<AuxiliaryAccount>
    {
        public AuxiliaryAccountMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.DepreciationAccountId)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("AuxiliaryAccount");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.BookAccount).HasColumnName("BookAccount");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.DepreciationAccountId).HasColumnName("DepreciationAccountId");
            this.Property(t => t.RelacionadoBP).HasColumnName("RelacionadoBP");
            this.Property(t => t.ContaContabilApresentacao).HasColumnName("ContaContabilApresentacao");
            this.Property(t => t.ControleEspecificoResumido).HasColumnName("ControleEspecificoResumido");
            this.Property(t => t.TipoMovimentacaoContabilizaSP).HasColumnName("TipoMovimentacaoContabilizaSP");

            // Relacionamento
            this.HasRequired(t => t.RelatedDepreciationAccount)
                .WithMany(t => t.AuxiliaryAccount)
                .HasForeignKey(d => d.DepreciationAccountId);
        }
    }
}
