using System.Data.Entity.ModelConfiguration;
namespace SAM.Web.Models.Mapping
{
    public class RelationshipAuxiliaryAccountMovementTypeMap : EntityTypeConfiguration<RelationshipAuxiliaryAccountMovementType>
    {
        public RelationshipAuxiliaryAccountMovementTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RelationshipAuxiliaryAccountMovementType");

            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AuxiliaryAccountId).HasColumnName("AuxiliaryAccountId");
            this.Property(t => t.MovementTypeId).HasColumnName("MovementTypeId");

            // Relationships
            this.HasRequired(t => t.RelatedAuxiliaryAccount);

            this.HasRequired(t => t.RelatedMovementType);
        }
    }
}