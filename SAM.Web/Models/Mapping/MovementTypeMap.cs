using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class MovementTypeMap : EntityTypeConfiguration<MovementType>
    {
        public MovementTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            //this.Property(t => t.Id)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("MovementType");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.GroupMovimentId).HasColumnName("GroupMovimentId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.IncorporacaoEmLote).HasColumnName("IncorporacaoEmLote");
            this.Property(t => t.IncorporacaoSimples).HasColumnName("IncorporacaoSimples");

            this.HasRequired(t => t.RelatedGroupMoviment)
                .WithMany(t => t.MovementTypes)
                .HasForeignKey(d => d.GroupMovimentId);

            this.HasOptional(t => t.RelatedMovementType)
                .WithMany(t => t.MovementTypes)
                .HasForeignKey(d => d.TipoMovimentacaoRelacionada);
        }
    }
}

