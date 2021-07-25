using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ItemInventarioMap : EntityTypeConfiguration<ItemInventario>
    {
        public ItemInventarioMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Code)
                .HasMaxLength(250);

            this.Property(t => t.Item)
            .HasMaxLength(120);

            // Table & Column Mappings
            this.ToTable("ItemInventario");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InventarioId).HasColumnName("InventarioId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Item).HasColumnName("Item");
            this.Property(t => t.Estado).HasColumnName("Estado");
            this.Property(t => t.InitialName).HasColumnName("InitialName");
            this.Property(t => t.AssetId).HasColumnName("AssetId");
            this.Property(t => t.AssetMovementsIdOriginal).HasColumnName("AssetMovementsIdOriginal");

            // Relationships
            this.HasRequired(t => t.RelatedInventario)
                .WithMany(t => t.ItemInventarios)
                .HasForeignKey(d => d.InventarioId);

            // Relationships
            this.HasRequired(t => t.RelatedEstadoItemInventario)
                .WithMany(t => t.ItemInventarios)
                .HasForeignKey(d => d.Estado);

            //Propriedade de navegação entre ItemInventario/Asset
            //Relationships
            this.HasOptional(t => t.RelatedAsset)
                .WithMany(t => t.RelatedItemInventarios)
                .HasForeignKey(d => d.AssetId);

            this.HasOptional(t => t.RelatedAssetMovement)
                .WithMany(t => t.RelatedItemInventarios)
                .HasForeignKey(d => d.AssetMovementsIdOriginal);
        }
    }
}