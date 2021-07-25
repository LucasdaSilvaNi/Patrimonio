using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class DepreciationMaterialItemMap : EntityTypeConfiguration<DepreciationMaterialItem>
    {
        public DepreciationMaterialItemMap()
        {
            this.HasKey(t => t.DepreciationAccount);
            // Properties
            this.Property(t => t.DepreciationAccount).IsRequired();
            this.Property(t => t.MaterialItemCode).IsRequired();

            // Table & Column Mappings
            this.ToTable("DepreciationMaterialItem");
            this.Property(t => t.DepreciationAccount).HasColumnName("DepreciationAccount");
            this.Property(t => t.MaterialItemCode).HasColumnName("MaterialItemCode");
        }
    }
}
