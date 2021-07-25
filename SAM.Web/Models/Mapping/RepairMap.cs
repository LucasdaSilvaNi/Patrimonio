using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SAM.Web.Models.Mapping
{
    public class RepairMap : EntityTypeConfiguration<Repair>
    {
        public RepairMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Properties
            this.Property(t => t.AssetId)
            .IsRequired();
            this.Property(t => t.InitialId)
            .IsRequired();
            this.Property(t => t.NumberIdentification)
            .IsRequired();
            this.Property(t => t.PartNumberIdentification)
                .IsRequired();
         

            // Table & Column Mappings
            this.ToTable("Repair");
            this.Property(t => t.Id)
                .HasColumnName("Id");
            this.Property(t => t.AssetId)
                .HasColumnName("AssetId");
            this.Property(t => t.InitialId)
                .HasColumnName("InitialId");
            this.Property(t => t.NumberIdentification)
                .HasColumnName("NumberIdentification");
            this.Property(t => t.PartNumberIdentification)
                .HasColumnName("PartNumberIdentification");
            this.Property(t => t.Reaseon)
              .HasColumnName("Reaseon")
              .HasMaxLength(30);
            this.Property(t => t.Destination)
                .HasColumnName("Destination")
                .HasMaxLength(20);
            this.Property(t => t.DateExpectedReturn)
                .HasColumnName("DateExpectedReturn");
            this.Property(t => t.DateOut)
                .HasColumnName("DateOut");
            this.Property(t => t.EstimatedCost)
                .HasColumnName("EstimatedCost");
            this.Property(t => t.FinalCost)
                .HasColumnName("FinalCost");
            this.Property(t => t.ReturnDate)
                .HasColumnName("ReturnDate");

            // Relationships
            this.HasRequired(t => t.RelatedAssets)
                .WithMany(t => t.Repairs)
                .HasForeignKey(d => d.AssetId);
            this.HasRequired(t => t.RelatedInitials)
               .WithMany(t => t.Concerts)
               .HasForeignKey(d => d.InitialId);
        }
    }
}