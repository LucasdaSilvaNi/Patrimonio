using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SAM.Web.Models.Mapping
{
    public class SiafemCalendarMap : EntityTypeConfiguration<SiafemCalendar>
    {
        public SiafemCalendarMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.FiscalYear)
                .IsRequired();
            //.HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("SiafemCalendar");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FiscalYear).HasColumnName("FiscalYear");
            this.Property(t => t.ReferenceMonth).HasColumnName("ReferenceMonth");
            this.Property(t => t.DateClosing).HasColumnName("DateClosing");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.DataParaOperadores).HasColumnName("DataParaOperadores");
        }
    }
}