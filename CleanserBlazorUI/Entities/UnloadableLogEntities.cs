using System.ComponentModel.DataAnnotations.Schema;

namespace CleanserBlazorUI.Entities;

/// <summary>
/// One row per Unloadable Log run -- the persisted, editable counterpart to
/// the header row in UnloadableLogService.GenerateWorkbook. The .xlsx is
/// still generated and downloaded exactly as before; this is written
/// alongside it so the log becomes a queryable running history across all
/// 200+ subscribers instead of a folder of one-off spreadsheets.
///
/// Keeps only the subscriber's short code; the name and institution type come live from XDSDataLogDB
/// (Subscriber.Subscribers / Subscriber.SubscriberCategory), so a correction there shows here at once.
///
/// SerialNo from the old in-memory _unlLogSerialCounter is intentionally
/// dropped: that counter reset on every app restart and was never a stable
/// identifier. Id is now the real, DB-generated ordinal.
/// </summary>
public class UnloadableLogHeader
{
    public int Id { get; set; }

    // The subscriber's short code (the part of the file name before the date, e.g. "LEA"). Name and
    // institution type are NOT stored: they are read live from Subscriber.Subscribers / SubscriberCategory
    // in XDSDataLogDB whenever the log is shown.
    public string SubscriberCode { get; set; } = string.Empty;
    [NotMapped] public string SubscriberName { get; set; } = string.Empty;
    [NotMapped] public string InstitutionType { get; set; } = string.Empty;

    public string Associate { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public int NumberOfRecords { get; set; }
    public string ReportingPeriod { get; set; } = string.Empty;
    public string ReportingYear { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Months { get; set; } = string.Empty;
    public string LogYear { get; set; } = string.Empty;

    // ── Editable after generation, via the log grid (not set at insert time) ──
    public DateTime? DateEmailed { get; set; }
    public DateTime? DateFixed { get; set; }
    public string? Comments { get; set; }

    // ── Immutable: when the run actually happened ──────────────────────────
    public DateTime CreatedDate { get; set; }

    public List<UnloadableLogMessageDetail> MessageDetails { get; set; } = new();
    public List<UnloadableLogCategoryDetail> CategoryDetails { get; set; } = new();
}

/// <summary>
/// Table 1 from the workbook (error-message breakdown), one row per distinct
/// message per run.
/// </summary>
public class UnloadableLogMessageDetail
{
    public int Id { get; set; }

    public int UnloadableLogHeaderId { get; set; }
    [ForeignKey(nameof(UnloadableLogHeaderId))]
    public UnloadableLogHeader? UnloadableLogHeader { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Table 2 from the workbook (category rollup), one row per subcategory per
/// run. TopLevelCategory is "Demographic" / "Financial" / "FacilitySubmission"
/// matching UnloadableLogService's MessageCategoryRules.
/// </summary>
public class UnloadableLogCategoryDetail
{
    public int Id { get; set; }

    public int UnloadableLogHeaderId { get; set; }
    [ForeignKey(nameof(UnloadableLogHeaderId))]
    public UnloadableLogHeader? UnloadableLogHeader { get; set; }

    public string TopLevelCategory { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string DescriptionOfErrors { get; set; } = string.Empty;
    public int VolumeAffected { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Generic, subscriber-agnostic reference table: given the fixed wording of
/// an error message (placeholders like real IDs/dates stripped out), says
/// which TopLevelCategory/SubCategory it belongs to. Drives
/// UnloadableLogService.CategorizeMessage -- editable via the
/// Unloadable Error Catalog page instead of requiring a code change
/// whenever an existing check's message needs a new or different label.
/// </summary>
public class UnloadableErrorCatalogEntry
{
    public int Id { get; set; }
    public string TopLevelCategory { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string DescriptionOfErrors { get; set; } = string.Empty;
    public DateTime LastUpdatedDate { get; set; }
}

/// <summary>
/// One enriched row per UnloadableLogHeader for the Unloadable Log Report page
/// (see GetUnloadableLogReportDataAsync) -- the header's own fields plus its
/// Data Provider identity resolved fresh from Subscriber.Subscribers (via the
/// filename's short code), since UnloadableLogHeader only links to the local,
/// legacy SubscriberProfile (no category). Not a mapped table -- a view model.
/// </summary>
public class UnloadableLogReportRow
{
    public int HeaderId { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string DataProvider { get; set; } = string.Empty;
    public string? SubCode { get; set; }
    public string? SubXDSCode { get; set; }
    public string? SubCategoryCode { get; set; }
    public string? SubCategoryDescription { get; set; }
    public string Associate { get; set; } = string.Empty;
    public int NumberOfRecords { get; set; }
    public string ReportingPeriod { get; set; } = string.Empty;
    public string ReportingYear { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Months { get; set; } = string.Empty;
    public string LogYear { get; set; } = string.Empty;
    public DateTime? DateEmailed { get; set; }
    public DateTime? DateFixed { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<UnloadableLogMessageDetail> MessageDetails { get; set; } = new();
    public List<UnloadableLogCategoryDetail> CategoryDetails { get; set; } = new();
}
