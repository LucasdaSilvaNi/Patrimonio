using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ItemSiafisicoMap : EntityTypeConfiguration<ItemSiafisico>
    {
        public ItemSiafisicoMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("Item_Siafisico");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Cod_Grupo).HasColumnName("Cod_Grupo");
            this.Property(t => t.Nome_Grupo).HasColumnName("Nome_Grupo");
            this.Property(t => t.Cod_Material).HasColumnName("Cod_Material");
            this.Property(t => t.Nome_Material).HasColumnName("Nome_Material");
            this.Property(t => t.Cod_Item_Mat).HasColumnName("Cod_Item_Mat");
            this.Property(t => t.Nome_Item_Mat).HasColumnName("Nome_Item_Mat");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.BEC).HasColumnName("BEC");
        }
    }
}
