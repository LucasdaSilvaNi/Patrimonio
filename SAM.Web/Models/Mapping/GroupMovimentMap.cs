using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class GroupMovimentMap : EntityTypeConfiguration<GroupMoviment>
    {
        public GroupMovimentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Code)
                .IsRequired();

            this.Property(t => t.Description)
                .IsOptional()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("GroupMoviment");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
        }
    }
}
