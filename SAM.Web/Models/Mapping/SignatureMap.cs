using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class SignatureMap : EntityTypeConfiguration<Signature>
    {
        public SignatureMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.PositionCamex)
                .HasMaxLength(100);

            this.Property(t => t.Post)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Signature");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PositionCamex).HasColumnName("PositionCamex");
            this.Property(t => t.Post).HasColumnName("Post");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
