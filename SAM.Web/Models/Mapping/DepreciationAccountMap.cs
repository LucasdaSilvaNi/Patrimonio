using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class DepreciationAccountMap : EntityTypeConfiguration<DepreciationAccount>
    {
        public DepreciationAccountMap()
        {
            this.HasKey(t => t.Id);

            this.ToTable("DepreciationAccount");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.DescricaoContaDepreciacaoContabilizaSP).HasColumnName("DescricaoContaDepreciacaoContabilizaSP");
        }

    }
}