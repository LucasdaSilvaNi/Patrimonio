using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class SaldoContabilAtualUGEContaDepreciacaoMap : EntityTypeConfiguration<SaldoContabilAtualUGEContaDepreciacao>
    {
        public SaldoContabilAtualUGEContaDepreciacaoMap() {
            this.HasKey(t => t.Id);

            this.ToTable("SaldoContabilAtualUGEContaDepreciacao");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IdOrgao).HasColumnName("IdOrgao");
            this.Property(t => t.IdUO).HasColumnName("IdUO");
            this.Property(t => t.IdUGE).HasColumnName("IdUGE");
            this.Property(t => t.CodigoOrgao).HasColumnName("CodigoOrgao");
            this.Property(t => t.CodigoUO).HasColumnName("CodigoUO");
            this.Property(t => t.CodigoUGE).HasColumnName("CodigoUGE");
            this.Property(t => t.DepreciationAccountId).HasColumnName("DepreciationAccountId");
            this.Property(t => t.DepreciacaoAcumulada).HasColumnName("DepreciacaoAcumulada");
        }
    }
}