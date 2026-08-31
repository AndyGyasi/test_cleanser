namespace CleanserBlazorUI.Entities;

// Now an EF-tracked entity in ApplicationDbContext (previously read via raw
// ADO.NET from a separate, unmigrated database -- see
// DataManagementService.GetShortCodeFromSubscribeIDAsync). Every existing
// consumer only ever reads ShortName/SubCategoryCode, so the fields below
// are purely additive -- expanded to match the original legacy Subscriber
// table's schema, not yet wired into any behavior beyond that lookup.
public class SubscribeContext
{
    public int Id { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string SubCategoryCode { get; set; } = string.Empty;

    // ── Expanded to match the original Subscriber table ──────────────────────
    public string? SubCode { get; set; }
    public string? SubXDSCode { get; set; }
    public string? SubBoGCode { get; set; }
    public string? SubName { get; set; }
    public string? ContactName1 { get; set; }
    public string? Position1 { get; set; }
    public string? ContactPhone1 { get; set; }
    public string? ContactEmail1 { get; set; }
    public string? ContactName2 { get; set; }
    public string? Position2 { get; set; }
    public string? ContactPhone2 { get; set; }
    public string? ContactEmail2 { get; set; }
    public string? ContactName3 { get; set; }
    public string? Position3 { get; set; }
    public string? ContactPhone3 { get; set; }
    public string? ContactEmail3 { get; set; }
    public string? PostAdd { get; set; }
    public string? Town { get; set; }
    public string? DistrictCode { get; set; }
    public string? RegionCode { get; set; }
    public string? Email { get; set; }
    public string? SectorCode { get; set; }
    public string? Remarks { get; set; }
    public string? SubStatus { get; set; }
    public string? DeactivatedReason { get; set; }
    public DateTime? DateSubscribe { get; set; }
    public DateTime? TransDate { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? EntryDate { get; set; }
    public string? UserID { get; set; }
}


