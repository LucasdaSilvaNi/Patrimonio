using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class ErroLogMap : EntityTypeConfiguration<ErroLog>
    {
        public ErroLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            //this.Property(t => t.exMessage)
            //    .IsRequired()
            //    .HasMaxLength(1000);
            //this.Property(t => t.exStackTrace)
            //    .IsRequired()
            //    .HasMaxLength(500);
            //this.Property(t => t.exTargetSite)
            //    .IsRequired()
            //    .HasMaxLength(500);
            this.Property(t => t.Usuario)
                //.IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("ErroLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.exMessage).HasColumnName("exMessage");
            this.Property(t => t.exStackTrace).HasColumnName("exStackTrace");
            this.Property(t => t.exTargetSite).HasColumnName("exTargetSite");
            this.Property(t => t.DataOcorrido).HasColumnName("DataOcorrido");
            this.Property(t => t.Usuario).HasColumnName("Usuario");
        }
    }
}