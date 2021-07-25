using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SAM.Web.Models.Mapping
{
    public class AttachmentSupportMap : EntityTypeConfiguration<AttachmentSupport>
    {
        public AttachmentSupportMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("AttachmentSupport");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SupportId).HasColumnName("SupportId");
            this.Property(t => t.File).HasColumnName("File");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Extension).HasColumnName("Extension");
            this.Property(t => t.ContentType).HasColumnName("ContentType");
            this.Property(t => t.InclusionDate).HasColumnName("InclusionDate");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedSupport)
                .WithMany(t => t.AttachmentSupports)
                .HasForeignKey(d => d.SupportId);
        }
    }
}