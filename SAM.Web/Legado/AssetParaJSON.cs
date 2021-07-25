using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Legado
{
    public class AssetParaJSON
    {
        public int Id { get; set; }

        public int InitialId { get; set; }

        public string NumberIdentification { get; set; }

        public string PartNumberIdentification { get; set; }

        public int InstitutionId { get; set; }

        public int BudgetUnitId { get; set; }

        public int ManagerUnitId { get; set; }

        public int AdministrativeUnitId { get; set; }

        public int SectionId { get; set; }

        public int ResponsibleId { get; set; }

        public Nullable<int> AuxiliaryAccountId { get; set; }

        public Nullable<int> OutSourcedId { get; set; }
        public int IncorporationTypeId { get; set; }
        public int StateConservationId { get; set; }

        public int MovementTypeId { get; set; }

        public Nullable<int> SupplierId { get; set; }

        public Nullable<DateTime> InclusionDate { get; set; }

        public virtual Nullable<DateTime> AcquisitionDate { get; set; }
        public Nullable<DateTime> ManufactureDate { get; set; }

        public Nullable<DateTime> DateGuarantee { get; set; }

        public Nullable<DateTime> ReceiptTermDate { get; set; }
        public Nullable<DateTime> InventoryDate { get; set; }

        public Nullable<DateTime> AssigningAmountDate { get; set; }

        public Nullable<DateTime> BagDate { get; set; }

        public Nullable<DateTime> OutDate { get; set; }

        public string NoteGranting { get; set; }

        public bool Status { get; set; }

        public virtual Nullable<decimal> ValueAcquisition { get; set; }

        public virtual Nullable<decimal> ValueUpdate { get; set; }
        public string NumberPurchaseProcess { get; set; }
        public string NGPB { get; set; }

        public string Invoice { get; set; }

        public string SerialNumber { get; set; }
        public string ChassiNumber { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }
        public string NumberPlate { get; set; }

        public string AdditionalDescription { get; set; }

        public Nullable<int> HistoricId { get; set; }

        public bool Condition { get; set; }

        public bool Rating { get; set; }
        public Nullable<int> OldInitial { get; set; }
        public string OldNumberIdentification { get; set; }

        public string OldPartNumberIdentification { get; set; }

        public virtual Nullable<decimal> ValueOut { get; set; }

        public int? LifeCycle { get; set; }

        public decimal? RateDepreciationMonthly { get; set; }

        public decimal? ResidualValue { get; set; }

        public bool AceleratedDepreciation { get; set; }

        public int MaterialItemCode { get; set; }

        public string MaterialItemDescription { get; set; }

        public int MaterialGroupCode { get; set; }

        public string Empenho { get; set; }
        public string Gerador_Descricao { get; set; }

        public AuxiliarAcountJSON RelatedAuxiliaryAccount { get; set; }
        public SectionJSON RelatedSection { get; set; }
        public AdministrativeUnitJSON RelatedAdministrativeUnit { get; set; }
        public ManagerUnitJSON RelatedManagementUnit { get; set; }
        public BudgetUnitJSON RelatedBudgetUnit { get; set; }
        public InstitutionJson RelatedInstitution { get; set; }

    }

    public class AuxiliarAcountJSON
    {

        public int Id { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }


        public Nullable<int> BookAccount { get; set; }

        public bool Status { get; set; }
    }

    public class SectionJSON
    {
        public int Id { get; set; }

        public int AdministrativeUnitId { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }
        public Nullable<int> AddressId { get; set; }

        public string Telephone { get; set; }

        public bool Status { get; set; }

        public int? ResponsibleId { get; set; }

    }

    public class ManagerUnitJSON
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }


    }

    public class BudgetUnitJSON
    {
        public int Id { get; set; }

        public int InstitutionId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }
    }

    public partial class AdministrativeUnitJSON
    {
       

        public int Id { get; set; }

        public int ManagerUnitId { get; set; }

        public int Code { get; set; }


        public string Description { get; set; }


        public Nullable<int> RelationshipAdministrativeUnitId { get; set; }

        public bool Status { get; set; }

    }

    public partial class InstitutionJson
    {
        public int Id { get; set; }

        public String Code { get; set; }
    }

    public partial class ResponsibleJson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public bool Status { get; set; }
        public int AdministrativeUnitId { get; set; }

        public AdministrativeUnitJSON RelatedAdministrativeUnit { get; set; }
        public ManagerUnitJSON RelatedManagementUnit { get; set; }
        public BudgetUnitJSON RelatedBudgetUnit { get; set; }
        public InstitutionJson RelatedInstitution { get; set; }
    }
    public partial class OutSourcedJson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CPFCNPJ { get; set; }
        public Nullable<int> AddressId { get; set; }
        public string Telephone { get; set; }
        public bool Status { get; set; }
        public int InstitutionId { get; set; }
        public int? BudgetUnitId { get; set; }

        public AdministrativeUnitJSON RelatedAdministrativeUnit { get; set; }
        public ManagerUnitJSON RelatedManagementUnit { get; set; }
        public BudgetUnitJSON RelatedBudgetUnit { get; set; }
        public InstitutionJson RelatedInstitution { get; set; }
        public AddressJson RelatedAddressJson { get; set; }
    }

    public partial class InitialJson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BarCode { get; set; }
        public int InstitutionId { get; set; }
        public int BudgetUnitId { get; set; }
        public bool Status { get; set; }

        public AdministrativeUnitJSON RelatedAdministrativeUnit { get; set; }
        public ManagerUnitJSON RelatedManagementUnit { get; set; }
        public BudgetUnitJSON RelatedBudgetUnit { get; set; }
        public InstitutionJson RelatedInstitution { get; set; }
    }

    public partial class IncorporationTypeJson
    {
        
        public int Id { get; set; }
        public string Description { get; set; }
        public int InstitutionId { get; set; }
        public int BudgetUnitId { get; set; }
        public bool Status { get; set; }

        public AdministrativeUnitJSON RelatedAdministrativeUnit { get; set; }
        public ManagerUnitJSON RelatedManagementUnit { get; set; }
        public BudgetUnitJSON RelatedBudgetUnit { get; set; }
        public InstitutionJson RelatedInstitution { get; set; }
    }
    public partial class AddressJson
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string ComplementAddress { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

    public partial class ChaveBemPatrimonialJson
    {
        public String Sigla { get; set; }
        public String Chapa { get; set; }
        public String Desdobro { get; set; }
        public String Mensagem { get; set; }
        public string Orgao { get; set; }
        public string Uo { get; set; }
        public string Uge { get; set; }
        public string Ua { get; set; }
        public bool BemCadastrado { get; set; }

    }

}