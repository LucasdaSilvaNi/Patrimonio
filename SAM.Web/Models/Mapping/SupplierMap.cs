using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class SupplierMap : EntityTypeConfiguration<Supplier>
    {
        public SupplierMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.CPFCNPJ)
                .IsRequired()
                .HasMaxLength(14);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.Telephone)
                .HasMaxLength(20);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.AdditionalData)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Supplier");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CPFCNPJ).HasColumnName("CPFCNPJ");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.Telephone).HasColumnName("Telephone");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.AdditionalData).HasColumnName("AdditionalData");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasOptional(t => t.RelatedAddress)
                .WithMany(t => t.RelatedSuppliers)
                .HasForeignKey(d => d.AddressId);

        }
    }
}
