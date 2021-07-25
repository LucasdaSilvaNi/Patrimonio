using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.CPF)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            this.Property(t => t.Password)
                .HasMaxLength(255);

            this.Property(t => t.Phrase)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("User");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CPF).HasColumnName("CPF");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Phrase).HasColumnName("Phrase");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.InvalidAttempts).HasColumnName("InvalidAttempts");
            //this.Property(t => t.Blocked).HasColumnName("Blocked");
            this.Property(t => t.ChangePassword).HasColumnName("ChangePassword");
            this.Property(t => t.DataUltimoTreinamento).HasColumnName("DataUltimoTreinamento");
        }
    }
}
