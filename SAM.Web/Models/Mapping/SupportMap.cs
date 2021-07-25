using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class SupportMap : EntityTypeConfiguration<Support>
    {
        public SupportMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("Support");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InstitutionId).HasColumnName("InstitutionId");
            this.Property(t => t.BudgetUnitId).HasColumnName("BudgetUnitId");
            this.Property(t => t.ManagerUnitId).HasColumnName("ManagerUnitId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.SupportStatusProdespId).HasColumnName("SupportStatusProdespId");
            this.Property(t => t.InclusionDate).HasColumnName("InclusionDate");
            this.Property(t => t.CloseDate).HasColumnName("CloseDate");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.SupportStatusUserId).HasColumnName("SupportStatusUserId");
            this.Property(t => t.Responsavel).HasColumnName("Responsavel");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.Login).HasColumnName("Login");
            this.Property(t => t.DataLogin).HasColumnName("DataLogin");
            this.Property(t => t.Observation).HasColumnName("Observation");
            this.Property(t => t.Status).HasColumnName("Status");


            // Relationships

            this.HasRequired(t => t.RelatedInstituation)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.InstitutionId);

            this.HasRequired(t => t.RelatedBudgetUnit)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.BudgetUnitId);

            this.HasRequired(t => t.RelatedManagerUnit)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.ManagerUnitId);

            this.HasRequired(t => t.RelatedUser)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.UserId);

            this.HasRequired(t => t.RelatedSupportStatusProdesp)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.SupportStatusProdespId);

            this.HasRequired(t => t.RelatedSupportStatusUser)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.SupportStatusUserId);

            this.HasRequired(t => t.RelatedModule)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.ModuleId);

            this.HasRequired(t => t.RelatedSupportType)
                .WithMany(t => t.Supports)
                .HasForeignKey(d => d.SupportTypeId);
        }
    }
}
