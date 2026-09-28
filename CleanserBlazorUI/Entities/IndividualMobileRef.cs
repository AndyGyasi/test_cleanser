namespace CleanserBlazorUI.Entities;

// Reference table for the "Individual Records Mobile" data type -- a separate
// channel from regular Individual (IND) records, kept in its own table rather
// than IndividualsData so a CustomerID appearing in both channels never
// cross-contaminates. Mirrors IndividualRef exactly (same headers, same
// fields) since Mobile's file schema is identical to Individual's.
public interface IIndividualReferenceRow
{
    string? DateOfBirth { get; set; }
    string? Surname { get; set; }
    string? FirstName { get; set; }
    string? MiddleNames { get; set; }
    DateTime CreatedDate { get; set; }
    DateTime LastUpdatedDate { get; set; }
}

public class IndividualMobileRef : IIndividualReferenceRow
{
    public int Id { get; set; }
    public int CurrenVersion { get; set; }
    public string SubscriberCode { get; set; }
    public string? CreditFacilityAccNum { get; set; } = string.Empty;
    public string? CustomerID { get; set; } = string.Empty;
    public string? DateOfBirth { get; set; } = string.Empty;
    public string? DisbursementDate { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public string? LastConfirmedReportingPeriod { get; set; } = string.Empty;
    public string? NatIDNum     { get; set; } = string.Empty;
    public string? VotersIDNum  { get; set; } = string.Empty;
    public string? DriverLicNum { get; set; } = string.Empty;
    public string? PassportNum  { get; set; } = string.Empty;
    public string? SSNum        { get; set; } = string.Empty;
    public string? EzwichNum    { get; set; } = string.Empty;
    public string? OtherIDNum   { get; set; } = string.Empty;
    public string? Surname    { get; set; } = string.Empty;
    public string? FirstName  { get; set; } = string.Empty;
    public string? MiddleNames { get; set; } = string.Empty;
    public string? FacilityStatusCode { get; set; } = string.Empty;
}
