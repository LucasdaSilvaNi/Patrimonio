using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class BPsARealizaremReclassificacaoContabilMap: EntityTypeConfiguration<BPsARealizaremReclassificacaoContabil>
    {
        public BPsARealizaremReclassificacaoContabilMap()
        {
            this.HasKey(bp => bp.Id);
            this.ToTable("BPsARealizaremReclassificacaoContabil");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AssetId).HasColumnName("AssetId");
            this.Property(t => t.GrupoMaterial).HasColumnName("GrupoMaterial");
            this.Property(t => t.IdUGE).HasColumnName("IdUGE");
        }
    }
}