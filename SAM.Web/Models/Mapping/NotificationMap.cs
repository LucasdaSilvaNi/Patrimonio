using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class NotificationMap : EntityTypeConfiguration<Notification>
    {
        public NotificationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Titulo)
                .IsRequired()
                .HasMaxLength(255);


            // Table & Column Mappings
            this.ToTable("Notification");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Titulo).HasColumnName("Titulo");
            this.Property(t => t.CorpoMensagem).HasColumnName("CorpoMensagem");
            this.Property(t => t.DataCriacao).HasColumnName("DataCriacao");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}

