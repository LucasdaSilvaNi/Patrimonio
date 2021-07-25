using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class HistoricSupportMap : EntityTypeConfiguration<HistoricSupport>
    {
        public HistoricSupportMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("HistoricSupport");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SupportId).HasColumnName("SupportId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.InclusionDate).HasColumnName("InclusionDate");
            this.Property(t => t.Observation).HasColumnName("Observation");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedSupport)
                .WithMany(t => t.HistoricSupports)
                .HasForeignKey(d => d.SupportId);

            this.HasRequired(t => t.RelatedUser)
                .WithMany(t => t.HistoricSupports)
                .HasForeignKey(d => d.UserId);
        }
    }
}
