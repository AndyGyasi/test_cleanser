namespace CleanserBlazorUI.Entities;

// Entities for the Data Logging verification feature: before a file is
// cleaned, confirm it was logged in XDSDataLogDB's Transact.ReceivedTrans and
// that the logged-in user is the associate it's actually assigned to (see
// RunCleanser's ownership gate in Home.razor). Distinct from the Unloadable
// Log feature (see UnloadableLogEntities.cs) -- this is about verifying who
// may clean a file, not about categorizing why a record was rejected.

// Read-only mappings onto existing tables in the separate XDSDataLogDB
// database (owned/migrated by that external system) -- queried via
// XdsDataLogDbContext. Never add an EF migration against these; the tables
// already exist and this app must not alter their schema.

// Transact.ReceivedTrans: one row per file logged in for a subscriber.
// RenamedFile is matched against the uploaded file's name; AssignTo is the
// Ring.Users.UserID of the associate that file is assigned to.
public class ReceivedTrans
{
    public string? RenamedFile { get; set; }
    public string? AssignTo { get; set; }
}

// Ring.Users: staff directory. UserID corresponds to
// ApplicationUser.ReceivedTransUserID and Transact.ReceivedTrans.AssignTo.
public class RingUser
{
    public string? UserID { get; set; }
    public string? Email { get; set; }
}

// Subscriber.Subscribers: the authoritative subscriber/Data Provider table.
// Matched by ShortName (same short code derived from filenames everywhere
// else -- see GetFileShortCodeFromFileName). Only the columns this app
// actually reads are mapped; the real table has many more (contacts,
// address, etc.) that nothing here needs yet.
public class Subscriber
{
    public string? ShortName { get; set; }
    public string? SubName { get; set; }
    public string? SubCode { get; set; }
    public string? SubXDSCode { get; set; }
    public string? SubCategoryCode { get; set; }
}

// Subscriber.SubscriberCategory: resolves Subscriber.SubCategoryCode into its
// description (e.g. "09" -> "OTHERS" / "Others").
public class SubscriberCategory
{
    public string? SubCategoryCode { get; set; }
    public string? CatShortName { get; set; }
    public string? CatDescription { get; set; }
}

/// <summary>
/// Projection of Subscriber.Subscribers joined with Subscriber.SubscriberCategory,
/// for populating a Data Provider dropdown (see GetAllDataProvidersAsync). Not a
/// mapped table -- just a view model.
/// </summary>
public class DataProviderOption
{
    public string ShortName { get; set; } = string.Empty;
    public string SubName { get; set; } = string.Empty;
    public string? CategoryDescription { get; set; }

    public string DisplayLabel => !string.IsNullOrWhiteSpace(CategoryDescription)
        ? $"{SubName} ({ShortName}) — {CategoryDescription}"
        : $"{SubName} ({ShortName})";
}

// Editable wording for the Data Logging ownership gate. Singleton row
// (Id = 1) in the app's own database -- not part of XDSDataLogDB.
// {filename} and {assignedName} are substituted at runtime.
public class DataLoggingGateMessages
{
    public int Id { get; set; }
    public string NotLoggedMessageTemplate { get; set; } = "{filename}: not logged (no Transact.ReceivedTrans entry).";
    public string AssignedToOtherMessageTemplate { get; set; } = "{filename}: assigned to {assignedName}.";
}

/// <summary>
/// One row per distinct filename that someone has tried to run through
/// Run Cleanser (Clean Only unchecked) while it had no Transact.ReceivedTrans
/// entry -- there's no bypass or access-request path for this case (that's
/// what Clean Only is for), so this exists purely so admin can see which
/// files are still waiting to be logged, and by whom. Upserted per filename
/// (not one row per attempt) so repeated retries on the same file don't
/// spam the list.
/// </summary>
public class DataLoggingUnloggedAttempt
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string LastAttemptedByEmail { get; set; } = string.Empty;
    public DateTime FirstAttemptedDate { get; set; }
    public DateTime LastAttemptedDate { get; set; }
    public int AttemptCount { get; set; }

    // Spooled from Subscriber.Subscribers (see GetDataProviderInfoForFilenameAsync)
    // alongside the display name, so these rows can be joined/filtered by
    // subscriber without re-deriving the short code from the filename every time.
    // Both codes are kept: SubXDSCode is the cross-system identifier (needed
    // for a future additional server/database that won't have SubCode), while
    // SubCode is what Transact.ReceivedTrans itself keys off, so joining
    // straight back to it doesn't need to go through Subscriber.Subscribers.
    public string DataProvider { get; set; } = string.Empty;
    public string? SubCode { get; set; }
    public string? SubXDSCode { get; set; }
    public string? SubCategoryCode { get; set; }
}

/// <summary>
/// Admin-editable list of purposes a user can cite when requesting access to
/// clean a file assigned to someone else (e.g. "Assigned staff is on leave").
/// Purely a pick-list for the request form -- DataLoggingAccessRequest stores
/// the chosen reason as its own text, so editing/removing a reason here never
/// changes the wording on a request already submitted.
/// </summary>
public class DataLoggingAccessRequestReason
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public enum DataLoggingAccessRequestStatus
{
    Pending = 0,
    Approved = 1,
    Denied = 2
}

/// <summary>
/// A user's request to override the "assigned to someone else" gate for one
/// specific file (never the "not logged at all" gate -- that one has no
/// override path). One row per file requested, even when submitted together
/// as part of a batch-cleaning selection.
/// </summary>
public class DataLoggingAccessRequest
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string DataProvider { get; set; } = string.Empty;

    // Spooled alongside DataProvider (see GetDataProviderInfoForFilenameAsync)
    // so requests can be queried/joined by subscriber code directly instead
    // of only by the display name. SubCode is what Transact.ReceivedTrans
    // itself keys off; SubXDSCode is the cross-system identifier for a
    // future additional server/database that won't have SubCode.
    public string? SubCode { get; set; }
    public string? SubXDSCode { get; set; }
    public string? SubCategoryCode { get; set; }

    public string RequestedByEmail { get; set; } = string.Empty;
    public string AssignedToEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DataLoggingAccessRequestStatus Status { get; set; } = DataLoggingAccessRequestStatus.Pending;
    public string? ReviewedByEmail { get; set; }
    public DateTime? ReviewedDate { get; set; }
}

/// <summary>
/// Admin-editable list of purposes a user can cite when cleaning a file
/// they're the true owner of (or that they're otherwise cleared to clean --
/// admin, or an approved access request), e.g. "Reclean", "CorrectedRecords".
/// Purely a pick-list for the popup shown before Run Cleanser proceeds --
/// DataLoggingCleaningPurposeLog stores the chosen text as its own copy.
/// </summary>
public class DataLoggingCleaningPurposeReason
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// One row per file actually cleaned (Clean Only unchecked, file not
/// blocked), recording which purpose the user selected in the popup before
/// the run proceeded. Purely for admin visibility -- nothing reads this back
/// to affect cleaning behavior.
/// </summary>
public class DataLoggingCleaningPurposeLog
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string DataProvider { get; set; } = string.Empty;
    public string? SubCode { get; set; }
    public string? SubXDSCode { get; set; }
    public string? SubCategoryCode { get; set; }
    public string PerformedByEmail { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime PerformedDate { get; set; }
}

/// <summary>
/// One row per file cleaned with "Clean only" ticked (no reference check). Written when the file
/// is actually cleaned, never for a skipped file. FileInReceivedTrans is true when the file already
/// exists in Transact.ReceivedTrans, which makes the run a re-clean: only the file's owner, an admin,
/// or someone with admin-approved access may do that. AccessBasis says which of those applied.
/// </summary>
public class DataLoggingCleanOnlyLog
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string DataProvider { get; set; } = string.Empty;
    public string? SubCode { get; set; }
    public string? SubXDSCode { get; set; }
    public string? SubCategoryCode { get; set; }
    public string PerformedByEmail { get; set; } = string.Empty;
    public DateTime PerformedDate { get; set; }
    public bool FileInReceivedTrans { get; set; }
    public string? AssignedToEmail { get; set; }
    // "Not in ReceivedTrans", "Owner", "Admin" or "Approved access"
    public string AccessBasis { get; set; } = string.Empty;
}

/// <summary>One line of the dashboard's "Recent cleaning runs" list.</summary>
public class CleaningRunRow
{
    // "Clean only", "Cleaned with reference check" or "Re-cleaned"
    public string Kind { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string DataProvider { get; set; } = string.Empty;
    public string PerformedByEmail { get; set; } = string.Empty;
    public DateTime PerformedDate { get; set; }
    public string Detail { get; set; } = string.Empty;
}
