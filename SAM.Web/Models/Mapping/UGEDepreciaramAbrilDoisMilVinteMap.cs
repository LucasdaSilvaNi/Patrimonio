using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class UGEDepreciaramAbrilDoisMilVinteMap: EntityTypeConfiguration<UGEDepreciaramAbrilDoisMilVinte>
    {
        public UGEDepreciaramAbrilDoisMilVinteMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.ToTable("UGEDepreciaramAbrilDoisMilVinte");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
        }
    }
}