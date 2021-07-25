using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SAM.Web.Models.Mapping
{
    public class ExchangeMap : EntityTypeConfiguration<Exchange>
    {
        public ExchangeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Properties
            this.Property(t => t.AssetId)
            .IsRequired();
            this.Property(t => t.DateRequisition)
            .IsRequired();
            this.Property(t => t.Status)
                .IsRequired();


            // Table & Column Mappings
            this.ToTable("Exchange");
            this.Property(t => t.Id)
                .HasColumnName("Id");
            this.Property(t => t.AssetId)
                .HasColumnName("AssetId");
            this.Property(t => t.DateRequisition)
                .HasColumnName("DateRequisition");
            this.Property(t => t.DateService)
                .HasColumnName("DateService");
            this.Property(t => t.Status)
                .HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.RelatedAsset)
                .WithMany(t => t.Exchanges)
                .HasForeignKey(d => d.AssetId);

        }
    }
}