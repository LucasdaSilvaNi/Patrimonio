using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class HouveAlteracaoContabilMap : EntityTypeConfiguration<HouveAlteracaoContabil>
    {
        public HouveAlteracaoContabilMap() {
            this.ToTable("HouveAlteracaoContabil");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IdOrgao).HasColumnName("IdOrgao");
            this.Property(t => t.IdUO).HasColumnName("IdUO");
            this.Property(t => t.IdUGE).HasColumnName("IdUGE");
            this.Property(t => t.IdContaContabil).HasColumnName("IdContaContabil");
            this.Property(t => t.HouveAlteracao).HasColumnName("HouveAlteracao");
        }
    }
}