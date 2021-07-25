using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MaterialSubItemMap : EntityTypeConfiguration<MaterialSubItem>
    {
        public MaterialSubItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(120);

            this.Property(t => t.BarCode)
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("MaterialSubItem");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.MaterialItemId).HasColumnName("MaterialItemId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.BarCode).HasColumnName("BarCode");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.ActivityIndicator).HasColumnName("ActivityIndicator");
            this.Property(t => t.ManagerId).HasColumnName("ManagerId");
            this.Property(t => t.SpendingOriginId).HasColumnName("SpendingOriginId");
            this.Property(t => t.AuxiliaryAccountId).HasColumnName("AuxiliaryAccountId");
            this.Property(t => t.SupplyUnitId).HasColumnName("SupplyUnitId");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedAuxiliaryAccount)
                .WithMany(t => t.MaterialSubItems)
                .HasForeignKey(d => d.AuxiliaryAccountId);
            this.HasRequired(t => t.RelatedManager)
                .WithMany(t => t.MaterialSubItems)
                .HasForeignKey(d => d.ManagerId);
            this.HasRequired(t => t.RelatedMaterialItem)
                .WithMany(t => t.MaterialSubItems)
                .HasForeignKey(d => d.MaterialItemId);
            this.HasRequired(t => t.RelatedSpendingOrigin)
                .WithMany(t => t.MaterialSubItems)
                .HasForeignKey(d => d.SpendingOriginId);
            this.HasRequired(t => t.RelatedSupplyUnit)
                .WithMany(t => t.MaterialSubItems)
                .HasForeignKey(d => d.SupplyUnitId);

        }
    }
}
