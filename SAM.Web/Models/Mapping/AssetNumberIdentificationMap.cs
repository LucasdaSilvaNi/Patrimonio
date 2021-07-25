using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class AssetNumberIdentificationMap : EntityTypeConfiguration<AssetNumberIdentification>
    {
        public AssetNumberIdentificationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // Properties
            this.ToTable("AssetNumberIdentification");
            this.Property(t => t.NumberIdentification).HasColumnName("NumberIdentification").IsRequired();
            this.Property(p => p.Status).HasColumnName("Status");
            this.Property(p => p.AssetId).HasColumnName("AssetId");
            this.Property(p => p.Login).HasColumnName("Login");
            this.Property(p => p.InclusionDate).HasColumnName("InclusionDate");
            this.Property(p => p.InitialId).HasColumnName("InitialId");
            this.Property(p => p.DiferenciacaoChapa).HasColumnName("DiferenciacaoChapa").IsRequired();
            this.Property(p => p.ChapaCompleta).HasColumnName("ChapaCompleta");

            this.HasRequired(t => t.RelatedAsset)
               .WithMany(t => t.AssetNumberIdentifications)
                .HasForeignKey(d => d.AssetId);

            this.HasRequired(t => t.RelatedInitial)
               .WithMany(t => t.AssetNumberIdentifications)
                .HasForeignKey(d => d.InitialId);
        }
    }
}