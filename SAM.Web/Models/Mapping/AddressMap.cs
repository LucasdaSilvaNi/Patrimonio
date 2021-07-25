using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Street)
                .HasMaxLength(200);

            this.Property(t => t.Number)
                .HasMaxLength(10);

            this.Property(t => t.ComplementAddress)
                .HasMaxLength(30);

            this.Property(t => t.District)
                .HasMaxLength(50);

            this.Property(t => t.City)
                .HasMaxLength(50);

            this.Property(t => t.State)
                .HasMaxLength(2);

            this.Property(t => t.PostalCode)
                .IsFixedLength()
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("Address");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Street).HasColumnName("Street");
            this.Property(t => t.Number).HasColumnName("Number");
            this.Property(t => t.ComplementAddress).HasColumnName("ComplementAddress");
            this.Property(t => t.District).HasColumnName("District");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
        }
    }
}
