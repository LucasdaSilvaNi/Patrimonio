using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class DepreciationAccountingClosingMap : EntityTypeConfiguration<DepreciationAccountingClosing>
    {
        public DepreciationAccountingClosingMap() {
            this.ToTable("DepreciationAccountingClosing");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AccountingClosingId).HasColumnName("AccountingClosingId");
            this.Property(t => t.BookAccount).HasColumnName("BookAccount");
            this.Property(t => t.AccountingValue).HasColumnName("AccountingValue");
            this.Property(t => t.DepreciationMonth).HasColumnName("DepreciationMonth");
            this.Property(t => t.AccumulatedDepreciation).HasColumnName("AccumulatedDepreciation");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.AccountingDescription).HasColumnName("AccountingDescription");
            this.Property(t => t.ReferenceMonth).HasColumnName("ReferenceMonth");
            this.Property(t => t.ManagerUnitCode).HasColumnName("ManagerUnitCode");
            this.Property(t => t.ManagerDescription).HasColumnName("ManagerDescription");
        }
    }
}