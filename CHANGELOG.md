# Changelog

All notable changes to the Cleanser app are recorded here, newest first.
This complements the git history — it's the "what changed and why" for
anyone (including a future contributor) who doesn't want to reconstruct
intent from commit messages alone.

---

## 2026-07-28

### Added — GHA (Ghana Card) cross-record conflict check
If the same `CustomerID` is linked to two different **GHA-format** national ID
values across records, this is now flagged as an identity conflict — regardless
of which of the 7 ID columns (`NatIDNum`, `VotersIDNum`, `DriverLicNum`,
`PassportNum`, `SSNum`, `EzwichNum`, `OtherIDNum`) each value happens to be filed
under. Only GHA-prefixed IDs trigger this (not other 3-letter country codes like
NGA/CIV), since the Ghana Card is the one ID type guaranteed unique per person.

Both the shared `CustomerID` cell and each record's own conflicting ID field are
flagged, routing the record to UNL with a clear message.

*Files:* `Home.razor` (`RunAllCrossRecordChecksIND`, new check #15),
`IndividualContext.cs` (new `SameCustomerIDDifferentGhanaCard` flag).

### Added — On-demand Unloadable Log (Individual/IND only)
After cleaning an IND file, an associate can now generate the unloadable log's
header row plus Demographic/Financial rejection breakdown tables for that run,
via a new panel on the CLEANING view (appears only when the run produced UNL
records).

- **New `SubscriberProfile` table** (EF Core entity + migration) — a one-time
  Subscriber Name / Institution Type lookup keyed by subscriber code, since the
  external `[Subscriber].[Subscribers]` table doesn't carry those fields. Filled
  in once per subscriber, reused automatically after that.
- **New `Services/UnloadableLogService.cs`** — reflection-based aggregation over
  any record type shaped with `CellDataAndStatus` fields (works for Individual
  today; Business would reuse the same service). Builds the xlsx via ClosedXML.
- Percentage in the rejection tables is **% of UNL records**, not % of the total
  file.
- `Date Emailed` / `Date Fixed` are left blank for manual entry — the automated
  email/reporting framework isn't built into this codebase yet.
- The Demographic/Financial field split is a **best-guess from field order**,
  not yet verified against the BoG field-mapping document from earlier work.
  Correct `DemographicFieldNames` in `UnloadableLogService.cs` if anything's
  misclassified.
- **Known limitation:** generates a single-file workbook for the file just
  processed. Does not read-modify-write an existing master log — copy the row
  in manually for now if you're tracking a running log across files. Storing
  this as a proper database table (instead of a one-off xlsx per run) is
  planned as a future improvement.

*Files:* `ApplicationDbContext.cs`, `DataManagementService.cs`,
`SubScribersDBContext.cs` (new `SubscriberProfile` entity),
`Services/UnloadableLogService.cs` (new), `Home.razor`.

### Clarified — manual-rescue registration workflow
The existing **REFERENCE A FILE** tab (renamed **REFERENCE / REGISTER RECORDS**)
already did what a planned "register manually rescued records" feature was
scoped to do — no new logic was needed, just clarity. Verified the tab's
`DBIndividualContext` read path uses the exact same column layout as the
CLEAN-sheet write path (`AccNum` col 3, `CustomerID` col 4, `CreditFacilityType`
col 69, `DisbursementDate` col 75), so re-uploading a corrected CLEAN sheet —
including manually rescued records an associate added by hand — registers those
records into `IndividualsData` via the existing idempotent upsert.

Added an in-app info alert explaining both use cases (initial reference load +
manual-rescue registration) and flagging the one real constraint: **the
uploaded file's name prefix (before the first `_`) must stay intact**, since
`GetFileShortCodeFromFileName` derives the subscriber code from it.

*Known gap, not addressed:* `IndividualRef` has no provenance field, so a
record registered this way is indistinguishable from one the cleanser matched
automatically. Worth a follow-up if BoG audit trail ever requires knowing *why*
a record is trusted.

*Files:* `Home.razor` (markup/labels only — no logic change).

### Fixed — overdraft disbursement-date false-UNL routing
Two related bugs were causing legitimate overdraft redraws to be wrongly routed
to UNL:

1. **Cross-submission mismatch.** In `REF_MainMatcherTransformerInd_Clean_IND`,
   the reference lookup (`databaseDict`) was keyed by `(AccNum, CustomerID)`
   only — dropping `DisbursementDate` — so a facility's full disbursement-date
   history collapsed to whichever historical row happened to be last inserted.
   A new draw that matched an *earlier* historical date (just not the
   last-surviving one) was wrongly flagged as a mismatch. Fixed by keeping the
   full list of historical dates per facility and checking the incoming date
   against the whole set.
2. **Same-submission duplicates.** `AssignDuplicateDoBIssues` had no
   overdraft-type awareness at all (unlike the sibling duplicate-check block
   right above it), so multiple legitimate overdraft draws in the same file
   were blanket-flagged as `DisbursementDateMisMatch = "UNL"`. Added the same
   `CreditFacilityType == "V"` exemption used elsewhere.

Both fixes are scoped to overdraft facilities specifically — term-loan
mismatches and genuine duplicates are still correctly flagged (verified via a
regression test alongside the fix).

*Files:* `Home.razor` (`REF_MainMatcherTransformerInd_Clean_IND`,
`AssignDuplicateDoBIssues`).

---

## Backlog / Future Work

Not scheduled yet — noted here so it isn't lost, not because it's next.

- **Full UI redesign.** Parked deliberately for a later version, once the core
  logic work (Phases 1-6, this file's main body) is settled. Direction agreed
  so far: stay on Blazor Server + MudBlazor rather than switching stacks (the
  app's core work — Excel parsing, EF Core, direct DB/file access — is
  inherently server-side; a JS frontend would mean building and maintaining a
  full API layer on top of everything, for a UI reskin). Full redesign in
  scope (not just the daily-use screens), theme explicitly NOT XDS branding —
  to be specified later. Plan when we get there: agree on information
  architecture first, extract business logic out of `Home.razor` into
  services (valuable independent of the visual redesign too), then rebuild
  screen by screen with a proper design system.

- **Offline, whole-codebase understanding agent/tool.** Wanted: something
  that understands the entire codebase (not just what fits in a token-limited
  context window) and can point to exactly where in the code something lives,
  what it means, and how a change would ripple through it — running locally/
  offline, not a cloud/token-metered service. Noted for further discussion,
  not scoped yet.

- **Unloadable Log should source from the UNL sheet, not the DUPLICATE sheet.**
  The current Unloadable Log feature (`UnloadableLogService`) is wired to the
  duplicates list (`_duplicatesInd`), but the UNL sheet is what actually holds
  the genuinely-unloadable records — duplicates are a different category.
  Flagged directly by the person doing the day-to-day work, deferred
  intentionally rather than rushed alongside other in-flight fixes.

---

## Earlier history

Changes prior to 2026-07-28 are tracked in git history and
`Cleanser_Issues_Tracker_v2.xlsx` / `Cleanser_Issues_Report.docx` (see README),
not retroactively backfilled into this file.
