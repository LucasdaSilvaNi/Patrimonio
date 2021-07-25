using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class OutSourcedMap : EntityTypeConfiguration<OutSourced>
    {
        public OutSourcedMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(60);

            this.Property(t => t.CPFCNPJ)
                .HasMaxLength(14);

            this.Property(t => t.Telephone)
                .HasMaxLength(11);

            // Table & Column Mappings
            this.ToTable("OutSourced");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CPFCNPJ).HasColumnName("CPFCNPJ");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.Telephone).HasColumnName("Telephone");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.InstitutionId).HasColumnName("InstitutionId");
            this.Property(t => t.BudgetUnitId).HasColumnName("BudgetUnitId");

            // Relationships
            this.HasOptional(t => t.RelatedAddress)
                .WithMany(t => t.RelatedOutSourced)
                .HasForeignKey(d => d.AddressId);

            // Relationships
            this.HasRequired(t => t.RelatedInstitution)
                .WithMany(t => t.OutSourceds)
                .HasForeignKey(d => d.InstitutionId);

        }
    }
}
