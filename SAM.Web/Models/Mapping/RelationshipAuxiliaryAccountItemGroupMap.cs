using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class RelationshipAuxiliaryAccountItemGroupMap : EntityTypeConfiguration<RelationshipAuxiliaryAccountItemGroup>
    {
        public RelationshipAuxiliaryAccountItemGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RelationshipAuxiliaryAccountItemGroup");

            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.MaterialGroupId).HasColumnName("MaterialGroupId");
            this.Property(t => t.AuxiliaryAccountId).HasColumnName("AuxiliaryAccountId");

            // Relationships
            this.HasRequired(t => t.RelatedMaterialGroup);

            this.HasRequired(t => t.RelatedAuxiliaryAccount);

        }
    }
}