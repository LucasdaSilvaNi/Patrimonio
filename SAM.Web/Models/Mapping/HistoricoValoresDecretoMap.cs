using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class HistoricoValoresDecretoMap: EntityTypeConfiguration<HistoricoValoresDecreto>
    {
        public HistoricoValoresDecretoMap() {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("HistoricoValoresDecreto");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AssetId).HasColumnName("AssetId");
            this.Property(t => t.ValorAquisicao).HasColumnName("ValorAquisicao");
            this.Property(t => t.ValorRevalorizacao).HasColumnName("ValorRevalorizacao");
            this.Property(t => t.DataAlteracao).HasColumnName("DataAlteracao");
            this.Property(t => t.LoginId).HasColumnName("LoginId");

            // Relacionamento
            this.HasRequired(t => t.RelatedAsset)
                .WithMany(t => t.HistoricoValoresDecretos)
                .HasForeignKey(d => d.AssetId);

            // Relacionamento
            this.HasRequired(t => t.RelatedUser)
                .WithMany(t => t.HistoricoValoresDecretos)
                .HasForeignKey(d => d.LoginId);
        }
    }
}