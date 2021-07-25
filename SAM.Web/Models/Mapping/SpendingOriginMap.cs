using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class SpendingOriginMap : EntityTypeConfiguration<SpendingOrigin>
    {
        public SpendingOriginMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SpendingOrigin");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ActivityIndicator).HasColumnName("ActivityIndicator");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
