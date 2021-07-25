using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ManagerMap : EntityTypeConfiguration<Manager>
    {
        public ManagerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(120);

            this.Property(t => t.ShortName)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Telephone)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Manager");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ShortName).HasColumnName("ShortName");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.Telephone).HasColumnName("Telephone");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedAddress)
                .WithMany(t => t.RelatedManagers)
                .HasForeignKey(d => d.AddressId);
        }
    }
}
