namespace CleanserBlazorUI.Entities;

// A DOB mismatch found against reference history during a register-check
// (Clean Only unchecked) is never auto-resolved by overwriting -- the existing
// DOB is left alone and a row is recorded here instead, so an admin can decide
// which value is actually correct via the Reference Data Conflicts page.
public class ReferenceDataConflict
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty; // "Individual" or "Business"
    public string SubscriberCode { get; set; } = string.Empty;
    public string CustomerID { get; set; } = string.Empty;
    public string CreditFacilityAccNum { get; set; } = string.Empty;
    public string DisbursementDate { get; set; } = string.Empty;
    public string ExistingDOB { get; set; } = string.Empty;
    public string IncomingDOB { get; set; } = string.Empty;
    public string SourceFileName { get; set; } = string.Empty;
    public DateTime DetectedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }
}
