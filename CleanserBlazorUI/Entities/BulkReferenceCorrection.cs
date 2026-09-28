namespace CleanserBlazorUI.Entities;

// One row of the admin bulk-correction import (see ReferenceDataBulkUpdate.razor).
// Matches an existing IndividualsData/BusinessesData row by the same
// (SubscriberCode, CustomerID, CreditFacilityAccNum, DisbursementDate) key
// used everywhere else in the app, then overwrites only the fields supplied.
public class BulkReferenceCorrectionRow
{
    public string EntityType { get; set; } = string.Empty; // "Individual" or "Business"
    public string SubscriberCode { get; set; } = string.Empty;
    public string CustomerID { get; set; } = string.Empty;
    public string CreditFacilityAccNum { get; set; } = string.Empty;
    public string DisbursementDate { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string NatIDNum { get; set; } = string.Empty;
    public string VotersIDNum { get; set; } = string.Empty;
    public string DriverLicNum { get; set; } = string.Empty;
    public string PassportNum { get; set; } = string.Empty;
    public string SSNum { get; set; } = string.Empty;
    public string EzwichNum { get; set; } = string.Empty;
    public string OtherIDNum { get; set; } = string.Empty;
    public string Busregnum { get; set; } = string.Empty;
    public string Tinum { get; set; } = string.Empty;
}

public class BulkReferenceCorrectionResult
{
    public int Applied { get; set; }
    public int NotFound { get; set; }
    public List<string> Errors { get; set; } = new();
}
