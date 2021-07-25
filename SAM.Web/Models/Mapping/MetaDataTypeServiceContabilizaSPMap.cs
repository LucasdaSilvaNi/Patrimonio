using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MetaDataTypeServiceContabilizaSPMap : EntityTypeConfiguration<MetaDataTypeServiceContabilizaSP>
    {
        public MetaDataTypeServiceContabilizaSPMap() {
            this.HasKey(t => t.Id);

            this.Property(t => t.Name).IsRequired();

            this.ToTable("MetaDataTypeServiceContabilizaSP");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}