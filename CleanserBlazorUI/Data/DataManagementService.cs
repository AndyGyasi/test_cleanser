using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using CleanserBlazorUI.Services;

namespace CleanserBlazorUI.Data;
public class DataManagementService
{
    private string text_value_seperator { get; set; } = "&&&___&&&";
    private readonly ApplicationDbContext _context;
    private readonly XdsDataLogDbContext _xdsDataLogDbContext;
    public DataManagementService(ApplicationDbContext context, XdsDataLogDbContext xdsDataLogDbContext)
    {
        _context = context;
        _xdsDataLogDbContext = xdsDataLogDbContext;
    }
    // Returns all reference records for this subscriber.
    // The reference always holds the cumulative known state — every unique
    // (AccNum, CustomerID, DisbursementDate) ever seen for this subscriber.
    public async Task<List<IndividualRef>> GETReferenceData_IND(string fileShortName)
    {
        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        return await _context.IndividualsData
            .Where(p => p.SubscriberCode == subscriber)
            .ToListAsync();
    }
    public async Task<List<BusinessRef>> GETReferenceData_BUS(string fileShortName)
    {
        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        return await _context.BusinessesData
            .Where(p => p.SubscriberCode == subscriber)
            .ToListAsync();
    }
    // Individual Records Mobile -- same schema as Individual, separate table
    // (IndividualsMobileData) so a CustomerID appearing in both channels never
    // cross-contaminates.
    public async Task<List<IndividualMobileRef>> GETReferenceData_INDMobile(string fileShortName)
    {
        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        return await _context.IndividualsMobileData
            .Where(p => p.SubscriberCode == subscriber)
            .ToListAsync();
    }

    // Maps IndividualMobileRef rows into IndividualRef-shaped objects so the
    // existing IND matcher/transformer and cross-record-check logic (which take
    // List<IndividualRef>) can be reused as-is for the Mobile channel, instead
    // of duplicating that large, intricate logic a second time. Only used for
    // reading/comparing -- writes always go through SaveExcelDataToDatabaseIndMobile,
    // which targets IndividualsMobileData directly.
    public static List<IndividualRef> MapMobileToIndividualRefShape(List<IndividualMobileRef> mobileRows) =>
        mobileRows.Select(m => new IndividualRef
        {
            Id = m.Id, CurrenVersion = m.CurrenVersion, SubscriberCode = m.SubscriberCode,
            CreditFacilityAccNum = m.CreditFacilityAccNum, CustomerID = m.CustomerID,
            DateOfBirth = m.DateOfBirth, DisbursementDate = m.DisbursementDate,
            CreatedDate = m.CreatedDate, LastUpdatedDate = m.LastUpdatedDate,
            LastConfirmedReportingPeriod = m.LastConfirmedReportingPeriod,
            NatIDNum = m.NatIDNum, VotersIDNum = m.VotersIDNum, DriverLicNum = m.DriverLicNum,
            PassportNum = m.PassportNum, SSNum = m.SSNum, EzwichNum = m.EzwichNum, OtherIDNum = m.OtherIDNum,
            Surname = m.Surname, FirstName = m.FirstName, MiddleNames = m.MiddleNames,
            FacilityStatusCode = m.FacilityStatusCode
        }).ToList();
    
    
    
    // ── IND Upsert ────────────────────────────────────────────────────────────
    // Upsert key: (SubscriberCode, AccNum, CustomerID, DisbursementDate)
    // - New records    → INSERT
    // - Existing rows  → enrich ID fields where DB is empty + file has value
    // - Absent records → left as-is (historical, never deleted)
    // Returns changelog: every (AccNum, CustomerID, DisbDate, FieldName) enriched this run.
    public async Task<List<(string AccNum, string CustomerID, string DisbDate, string FieldAdded)>>
        SaveExcelDataToDatabaseInd(IEnumerable<DBIndividualContext> dataFromExcel, string fileShortName)
    {
        // ApplicationDbContext is registered Scoped, which in Blazor Server means
        // one instance per browser circuit -- NOT per upload. Without clearing,
        // entities tracked from an earlier upload in the same session can still be
        // tracked here, and EF Core throws when a "new" query tries to track an
        // entity with a key that's already tracked by a different, stale instance.
        // This was almost certainly the cause of a later upload in the same
        // session silently producing no output (previously hidden by an empty
        // catch block, now fixed alongside this).
        _context.ChangeTracker.Clear();

        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        var now        = DateTime.Now;
        var reportingPeriod = GetReportingPeriodLabel(fileShortName);
        var changelog  = new List<(string, string, string, string)>();

        var existing = await _context.IndividualsData
            .Where(r => r.SubscriberCode == subscriber)
            .ToListAsync();

        // The reference table can genuinely have duplicate (AccNum, CustomerID,
        // DisbursementDate) rows -- the old Register tool used to blind-insert a
        // fresh row on every run instead of matching against what's already there
        // (see Initialize_SaveExcelDataToDatabaseInd). When duplicates exist, pick
        // ONE canonical row per key via PickCanonicalIndividualRef rather than just
        // whatever the database happened to return last -- the same helper is used
        // by BuildReferenceVirtualRecordsInd's cross-check, so both agree on which
        // row is authoritative.
        var existingIndex = existing
            .GroupBy(r => (Norm(r.CreditFacilityAccNum), Norm(r.CustomerID), Norm(r.DisbursementDate)))
            .ToDictionary(g => g.Key, g => PickCanonicalIndividualRef(g));

        var toInsert = new List<IndividualRef>();
        var toUpdate = new List<IndividualRef>();

        foreach (var item in dataFromExcel)
        {
            var key = (Norm(item.CreditFacilityAccNum),
                       Norm(item.CustomerID),
                       Norm(item.DisbursementDate));

            if (existingIndex.TryGetValue(key, out var dbRow))
            {
                // EF Core entity properties can't be passed as ref directly.
                // Copy to locals, enrich, write back.
                string? natID = dbRow.NatIDNum, votersID = dbRow.VotersIDNum,
                        driverLic = dbRow.DriverLicNum, passport = dbRow.PassportNum,
                        ssNum = dbRow.SSNum, ezwich = dbRow.EzwichNum, otherID = dbRow.OtherIDNum;
                string? surname = dbRow.Surname, firstName = dbRow.FirstName, middleNames = dbRow.MiddleNames;

                bool changed = false;
                changed |= EnrichField(ref natID,     item.NatIDNum,     "NatIDNum",     key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref votersID,  item.VotersIDNum,  "VotersIDNum",  key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref driverLic, item.DriverLicNum, "DriverLicNum", key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref passport,  item.PassportNum,  "PassportNum",  key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref ssNum,     item.SSNum,        "SSNum",        key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref ezwich,    item.EzwichNum,    "EzwichNum",    key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref otherID,   item.OtherIDNum,   "OtherIDNum",   key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref surname,     item.Surname,     "Surname",     key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref firstName,   item.FirstName,   "FirstName",   key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref middleNames, item.MiddleNames, "MiddleNames", key.Item1, key.Item2, key.Item3, changelog);

                if (changed)
                {
                    dbRow.NatIDNum = natID; dbRow.VotersIDNum = votersID;
                    dbRow.DriverLicNum = driverLic; dbRow.PassportNum = passport;
                    dbRow.SSNum = ssNum; dbRow.EzwichNum = ezwich; dbRow.OtherIDNum = otherID;
                    dbRow.Surname = surname; dbRow.FirstName = firstName; dbRow.MiddleNames = middleNames;
                    dbRow.LastUpdatedDate = now;
                    toUpdate.Add(dbRow);
                }
                // Previously set on the tracked entity but never added to
                // toUpdate unless another field also changed -- BulkUpdateAsync
                // bypasses the change tracker entirely, so a DOB-only update
                // with nothing else newly enriched was silently never
                // persisted. Now explicitly tracked whenever DOB actually changes.
                if (!string.IsNullOrWhiteSpace(item.DateOfBirth) && string.IsNullOrWhiteSpace(dbRow.DateOfBirth))
                {
                    // Nothing on file yet -- this is enrichment, not a conflict.
                    dbRow.DateOfBirth = item.DateOfBirth;
                    dbRow.LastUpdatedDate = now;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }
                else if (!string.IsNullOrWhiteSpace(item.DateOfBirth) && dbRow.DateOfBirth != item.DateOfBirth)
                {
                    // A different DOB for the same customer is a genuine conflict --
                    // flag it for manual review instead of silently trusting whichever
                    // file came in last.
                    // Use dbRow's own (unnormalized) field values as the conflict's key,
                    // not the Norm()'d matching key -- ResolveReferenceDataConflictAsync
                    // looks the row back up by exact equality against these later, and
                    // the row itself was stored with its original casing/whitespace.
                    await RecordOrRefreshDobConflictAsync("Individual", subscriber, dbRow.CustomerID, dbRow.CreditFacilityAccNum, dbRow.DisbursementDate,
                        dbRow.DateOfBirth, item.DateOfBirth, fileShortName);
                }

                // Status is a mutable real-world state (open/closed), not enriched-once
                // like names/IDs -- always refresh to whatever was just submitted.
                if (!string.IsNullOrWhiteSpace(item.FacilityStatusCode) && dbRow.FacilityStatusCode != item.FacilityStatusCode)
                {
                    dbRow.FacilityStatusCode = item.FacilityStatusCode;
                    dbRow.LastUpdatedDate = now;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }

                // Option A: "last confirmed" tracking -- update on every match,
                // regardless of whether any field's value actually changed.
                if (dbRow.LastConfirmedReportingPeriod != reportingPeriod)
                {
                    dbRow.LastConfirmedReportingPeriod = reportingPeriod;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }
            }
            else
            {
                toInsert.Add(new IndividualRef
                {
                    SubscriberCode       = subscriber,
                    CreditFacilityAccNum = item.CreditFacilityAccNum        ?? string.Empty,
                    CustomerID           = item.CustomerID                  ?? string.Empty,
                    DisbursementDate     = item.DisbursementDate             ?? string.Empty,
                    DateOfBirth          = item.DateOfBirth                  ?? string.Empty,
                    NatIDNum             = item.NatIDNum                    ?? string.Empty,
                    VotersIDNum          = item.VotersIDNum                 ?? string.Empty,
                    DriverLicNum         = item.DriverLicNum                ?? string.Empty,
                    PassportNum          = item.PassportNum                 ?? string.Empty,
                    SSNum                = item.SSNum                       ?? string.Empty,
                    EzwichNum            = item.EzwichNum                   ?? string.Empty,
                    OtherIDNum           = item.OtherIDNum                  ?? string.Empty,
                    Surname              = item.Surname                    ?? string.Empty,
                    FirstName            = item.FirstName                  ?? string.Empty,
                    MiddleNames          = item.MiddleNames                ?? string.Empty,
                    FacilityStatusCode   = item.FacilityStatusCode         ?? string.Empty,
                    LastConfirmedReportingPeriod = reportingPeriod,
                    CurrenVersion        = 1,
                    CreatedDate          = now,
                    LastUpdatedDate      = now
                });
            }
        }

        const int batchSize = 10_000;
        for (int i = 0; i < toInsert.Count; i += batchSize)
            await BulkInsertBatchIND(toInsert.Skip(i).Take(batchSize).ToList());
        if (toUpdate.Count > 0)
            await _context.BulkUpdateAsync(toUpdate, new BulkConfig { BatchSize = 4000 });

        return changelog;
    }


    // ── Overload for CLEAN path — accepts IndividualContext (has .Data properties) ─
    public async Task<List<(string AccNum, string CustomerID, string DisbDate, string FieldAdded)>>
        SaveExcelDataToDatabaseInd(IEnumerable<IndividualContext> dataFromExcel, string fileShortName)
    {
        var mapped = dataFromExcel.Select(r => new DBIndividualContext
        {
            CreditFacilityAccNum = r.CreditFacilityAccNum?.Data ?? string.Empty,
            CustomerID           = r.CustomerID?.Data           ?? string.Empty,
            DisbursementDate     = r.DisbursementDate?.Data     ?? string.Empty,
            DateOfBirth          = r.DateOfBirth?.Data          ?? string.Empty,
            NatIDNum             = r.NatIDNum?.Data             ?? string.Empty,
            VotersIDNum          = r.VotersIDNum?.Data          ?? string.Empty,
            DriverLicNum         = r.DriverLicNum?.Data         ?? string.Empty,
            PassportNum          = r.PassportNum?.Data          ?? string.Empty,
            SSNum                = r.SSNum?.Data                ?? string.Empty,
            EzwichNum            = r.EzwichNum?.Data            ?? string.Empty,
            OtherIDNum           = r.OtherIDNum?.Data           ?? string.Empty,
            Surname              = r.Surname?.Data              ?? string.Empty,
            FirstName            = r.FirstName?.Data            ?? string.Empty,
            MiddleNames          = r.MiddleNames?.Data          ?? string.Empty,
            FacilityStatusCode   = r.FacilityStatusCode?.Data   ?? string.Empty,
        });
        return await SaveExcelDataToDatabaseInd(mapped, fileShortName);
    }

    // ── Individual Records Mobile Upsert ────────────────────────────────────────
    // Exact mirror of SaveExcelDataToDatabaseInd above, targeting IndividualsMobileData
    // instead of IndividualsData. Kept as a genuine duplicate rather than a generic
    // repository, since EF Core's DbSet<T> access doesn't generalize cleanly here and
    // this method's enrichment rules are complex enough that a shared/generic version
    // would obscure more than it'd save.
    public async Task<List<(string AccNum, string CustomerID, string DisbDate, string FieldAdded)>>
        SaveExcelDataToDatabaseIndMobile(IEnumerable<DBIndividualContext> dataFromExcel, string fileShortName)
    {
        _context.ChangeTracker.Clear();

        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        var now        = DateTime.Now;
        var reportingPeriod = GetReportingPeriodLabel(fileShortName);
        var changelog  = new List<(string, string, string, string)>();

        var existing = await _context.IndividualsMobileData
            .Where(r => r.SubscriberCode == subscriber)
            .ToListAsync();

        var existingIndex = existing
            .GroupBy(r => (Norm(r.CreditFacilityAccNum), Norm(r.CustomerID), Norm(r.DisbursementDate)))
            .ToDictionary(g => g.Key, g => PickCanonicalIndividualRef(g));

        var toInsert = new List<IndividualMobileRef>();
        var toUpdate = new List<IndividualMobileRef>();

        foreach (var item in dataFromExcel)
        {
            var key = (Norm(item.CreditFacilityAccNum),
                       Norm(item.CustomerID),
                       Norm(item.DisbursementDate));

            if (existingIndex.TryGetValue(key, out var dbRow))
            {
                string? natID = dbRow.NatIDNum, votersID = dbRow.VotersIDNum,
                        driverLic = dbRow.DriverLicNum, passport = dbRow.PassportNum,
                        ssNum = dbRow.SSNum, ezwich = dbRow.EzwichNum, otherID = dbRow.OtherIDNum;
                string? surname = dbRow.Surname, firstName = dbRow.FirstName, middleNames = dbRow.MiddleNames;

                bool changed = false;
                changed |= EnrichField(ref natID,     item.NatIDNum,     "NatIDNum",     key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref votersID,  item.VotersIDNum,  "VotersIDNum",  key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref driverLic, item.DriverLicNum, "DriverLicNum", key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref passport,  item.PassportNum,  "PassportNum",  key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref ssNum,     item.SSNum,        "SSNum",        key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref ezwich,    item.EzwichNum,    "EzwichNum",    key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref otherID,   item.OtherIDNum,   "OtherIDNum",   key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref surname,     item.Surname,     "Surname",     key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref firstName,   item.FirstName,   "FirstName",   key.Item1, key.Item2, key.Item3, changelog);
                changed |= EnrichField(ref middleNames, item.MiddleNames, "MiddleNames", key.Item1, key.Item2, key.Item3, changelog);

                if (changed)
                {
                    dbRow.NatIDNum = natID; dbRow.VotersIDNum = votersID;
                    dbRow.DriverLicNum = driverLic; dbRow.PassportNum = passport;
                    dbRow.SSNum = ssNum; dbRow.EzwichNum = ezwich; dbRow.OtherIDNum = otherID;
                    dbRow.Surname = surname; dbRow.FirstName = firstName; dbRow.MiddleNames = middleNames;
                    dbRow.LastUpdatedDate = now;
                    toUpdate.Add(dbRow);
                }
                if (!string.IsNullOrWhiteSpace(item.DateOfBirth) && string.IsNullOrWhiteSpace(dbRow.DateOfBirth))
                {
                    dbRow.DateOfBirth = item.DateOfBirth;
                    dbRow.LastUpdatedDate = now;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }
                else if (!string.IsNullOrWhiteSpace(item.DateOfBirth) && dbRow.DateOfBirth != item.DateOfBirth)
                {
                    await RecordOrRefreshDobConflictAsync("IndividualMobile", subscriber, dbRow.CustomerID, dbRow.CreditFacilityAccNum, dbRow.DisbursementDate,
                        dbRow.DateOfBirth, item.DateOfBirth, fileShortName);
                }

                if (!string.IsNullOrWhiteSpace(item.FacilityStatusCode) && dbRow.FacilityStatusCode != item.FacilityStatusCode)
                {
                    dbRow.FacilityStatusCode = item.FacilityStatusCode;
                    dbRow.LastUpdatedDate = now;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }

                if (dbRow.LastConfirmedReportingPeriod != reportingPeriod)
                {
                    dbRow.LastConfirmedReportingPeriod = reportingPeriod;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }
            }
            else
            {
                toInsert.Add(new IndividualMobileRef
                {
                    SubscriberCode       = subscriber,
                    CreditFacilityAccNum = item.CreditFacilityAccNum        ?? string.Empty,
                    CustomerID           = item.CustomerID                  ?? string.Empty,
                    DisbursementDate     = item.DisbursementDate             ?? string.Empty,
                    DateOfBirth          = item.DateOfBirth                  ?? string.Empty,
                    NatIDNum             = item.NatIDNum                    ?? string.Empty,
                    VotersIDNum          = item.VotersIDNum                 ?? string.Empty,
                    DriverLicNum         = item.DriverLicNum                ?? string.Empty,
                    PassportNum          = item.PassportNum                 ?? string.Empty,
                    SSNum                = item.SSNum                       ?? string.Empty,
                    EzwichNum            = item.EzwichNum                   ?? string.Empty,
                    OtherIDNum           = item.OtherIDNum                  ?? string.Empty,
                    Surname              = item.Surname                    ?? string.Empty,
                    FirstName            = item.FirstName                  ?? string.Empty,
                    MiddleNames          = item.MiddleNames                ?? string.Empty,
                    FacilityStatusCode   = item.FacilityStatusCode         ?? string.Empty,
                    LastConfirmedReportingPeriod = reportingPeriod,
                    CurrenVersion        = 1,
                    CreatedDate          = now,
                    LastUpdatedDate      = now
                });
            }
        }

        const int batchSize = 10_000;
        for (int i = 0; i < toInsert.Count; i += batchSize)
            await BulkInsertBatchINDMobile(toInsert.Skip(i).Take(batchSize).ToList());
        if (toUpdate.Count > 0)
            await _context.BulkUpdateAsync(toUpdate, new BulkConfig { BatchSize = 4000 });

        return changelog;
    }

    public async Task<List<(string AccNum, string CustomerID, string DisbDate, string FieldAdded)>>
        SaveExcelDataToDatabaseIndMobile(IEnumerable<IndividualContext> dataFromExcel, string fileShortName)
    {
        var mapped = dataFromExcel.Select(r => new DBIndividualContext
        {
            CreditFacilityAccNum = r.CreditFacilityAccNum?.Data ?? string.Empty,
            CustomerID           = r.CustomerID?.Data           ?? string.Empty,
            DisbursementDate     = r.DisbursementDate?.Data     ?? string.Empty,
            DateOfBirth          = r.DateOfBirth?.Data          ?? string.Empty,
            NatIDNum             = r.NatIDNum?.Data             ?? string.Empty,
            VotersIDNum          = r.VotersIDNum?.Data          ?? string.Empty,
            DriverLicNum         = r.DriverLicNum?.Data         ?? string.Empty,
            PassportNum          = r.PassportNum?.Data          ?? string.Empty,
            SSNum                = r.SSNum?.Data                ?? string.Empty,
            EzwichNum            = r.EzwichNum?.Data            ?? string.Empty,
            OtherIDNum           = r.OtherIDNum?.Data           ?? string.Empty,
            Surname              = r.Surname?.Data              ?? string.Empty,
            FirstName            = r.FirstName?.Data            ?? string.Empty,
            MiddleNames          = r.MiddleNames?.Data          ?? string.Empty,
            FacilityStatusCode   = r.FacilityStatusCode?.Data   ?? string.Empty,
        });
        return await SaveExcelDataToDatabaseIndMobile(mapped, fileShortName);
    }

    // ── BUS Upsert ────────────────────────────────────────────────────────────
    public async Task SaveExcelDataToDatabaseBus(IEnumerable<DBBusinessContext> dataFromExcel, string fileShortName)
    {
        // Same reasoning as the IND overload above -- see that comment.
        _context.ChangeTracker.Clear();

        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        var now        = DateTime.Now;
        var reportingPeriod = GetReportingPeriodLabel(fileShortName);

        var existing = await _context.BusinessesData
            .Where(r => r.SubscriberCode == subscriber)
            .ToListAsync();

        // See matching fix on the Individual overload above -- avoids a
        // crash on duplicate reference rows.
        var existingIndex = new Dictionary<(string, string, string), BusinessRef>();
        foreach (var r in existing)
        {
            existingIndex[(Norm(r.CreditFacilityAccNum), Norm(r.CustomerID), Norm(r.DisbursementDate))] = r;
        }

        var toInsert = new List<BusinessRef>();
        var toUpdate = new List<BusinessRef>();
        var busChangelog = new List<(string, string, string, string)>();

        foreach (var item in dataFromExcel)
        {
            var key = (Norm(item.Facilityaccnum),
                       Norm(item.CustomerID),
                       Norm(item.DisbursementDate));

            if (existingIndex.TryGetValue(key, out var dbRow))
            {
                // Business records carry no real DOB -- the uploaded file has no such
                // column at all (see SpreadSheetHeadersData.Business). DateOfBirth on
                // BusinessRef is left untouched here; don't confuse it with the real
                // Registrationdate/Commencementdate fields, which are handled below.

                string? businessName = dbRow.Businessname, busRegNum = dbRow.Busregnum, tinNum = dbRow.Tinum;
                bool changed = false;
                changed |= EnrichField(ref businessName, item.Businessname, "Businessname", key.Item1, key.Item2, key.Item3, busChangelog);
                changed |= EnrichField(ref busRegNum,     item.Busregnum,   "Busregnum",     key.Item1, key.Item2, key.Item3, busChangelog);
                changed |= EnrichField(ref tinNum,        item.Tinum,       "Tinum",         key.Item1, key.Item2, key.Item3, busChangelog);
                if (changed)
                {
                    dbRow.Businessname = businessName;
                    dbRow.Busregnum = busRegNum;
                    dbRow.Tinum = tinNum;
                    dbRow.LastUpdatedDate = now;
                    toUpdate.Add(dbRow);
                }

                // Status is mutable -- always refresh to the latest submitted value,
                // same policy as the IND side.
                if (!string.IsNullOrWhiteSpace(item.FacilityStatusCode) && dbRow.FacilityStatusCode != item.FacilityStatusCode)
                {
                    dbRow.FacilityStatusCode = item.FacilityStatusCode;
                    dbRow.LastUpdatedDate = now;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }

                // Option A: "last confirmed" tracking -- see IND overload above.
                if (dbRow.LastConfirmedReportingPeriod != reportingPeriod)
                {
                    dbRow.LastConfirmedReportingPeriod = reportingPeriod;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }
            }
            else
            {
                toInsert.Add(new BusinessRef
                {
                    SubscriberCode       = subscriber,
                    CreditFacilityAccNum = item.Facilityaccnum  ?? string.Empty,
                    CustomerID           = item.CustomerID       ?? string.Empty,
                    DisbursementDate     = item.DisbursementDate ?? string.Empty,
                    DateOfBirth          = item.DateOfBirth      ?? string.Empty,
                    Businessname         = item.Businessname     ?? string.Empty,
                    Busregnum            = item.Busregnum        ?? string.Empty,
                    Tinum                = item.Tinum            ?? string.Empty,
                    FacilityStatusCode   = item.FacilityStatusCode ?? string.Empty,
                    LastConfirmedReportingPeriod = reportingPeriod,
                    CurrenVersion        = 1,
                    CreatedDate          = now,
                    LastUpdatedDate      = now
                });
            }
        }

        const int batchSize = 10_000;
        for (int i = 0; i < toInsert.Count; i += batchSize)
            await BulkInsertBatchBUS(toInsert.Skip(i).Take(batchSize).ToList());
        if (toUpdate.Count > 0)
            await _context.BulkUpdateAsync(toUpdate, new BulkConfig { BatchSize = 4000 });
    }


    // ── BUS overload for CLEAN path — accepts BusinessContext (has .Data properties) ─
    public async Task SaveExcelDataToDatabaseBus(IEnumerable<BusinessContext> dataFromExcel, string fileShortName)
    {
        var mapped = dataFromExcel.Select(r => new DBBusinessContext
        {
            Facilityaccnum   = r.Facilityaccnum?.Data   ?? string.Empty,
            CustomerID       = r.CustomerID?.Data       ?? string.Empty,
            DisbursementDate = r.DisbursementDate?.Data ?? string.Empty,
            DateOfBirth      = r.DateOfBirth            ?? string.Empty,
            Businessname       = r.Businessname?.Data       ?? string.Empty,
            Busregnum          = r.Busregnum?.Data          ?? string.Empty,
            Tinum              = r.Tinum?.Data              ?? string.Empty,
            FacilityStatusCode = r.FacilityStatusCode?.Data ?? string.Empty,
        });
        await SaveExcelDataToDatabaseBus(mapped, fileShortName);
    }

    private bool EnrichField(
        ref string? dbField, string? fileValue,
        string fieldName, string accNum, string custId, string disbDate,
        List<(string, string, string, string)> changelog)
    {
        if (!string.IsNullOrWhiteSpace(fileValue) && string.IsNullOrWhiteSpace(dbField))
        {
            dbField = fileValue;
            changelog.Add((accNum, custId, disbDate, fieldName));
            return true;
        }
        return false;
    }

    private string Norm(string? v) =>
        string.IsNullOrWhiteSpace(v) ? string.Empty : v.Trim().ToUpperInvariant();

    // ── Canonical-row selection among duplicate reference rows ────────────────
    // Legacy duplicates (same AccNum/CustomerID/DisbursementDate, multiple rows --
    // see the old Register tool bug) are never deleted here, just not all treated
    // as equally authoritative. A row that already carries real DOB/name data is
    // preferred over a blank one, however old; only when every duplicate is
    // completely blank do we fall back to the most recently created one. Used by
    // both the save/enrich path (SaveExcelDataToDatabaseInd) and the reference
    // cross-check (BuildReferenceVirtualRecordsInd) so they never disagree on
    // which row is "the" reference for a given key.
    public static bool IsBlankIndividualIdentity(IIndividualReferenceRow r) =>
        string.IsNullOrWhiteSpace(r.DateOfBirth) &&
        string.IsNullOrWhiteSpace(r.Surname) &&
        string.IsNullOrWhiteSpace(r.FirstName) &&
        string.IsNullOrWhiteSpace(r.MiddleNames);

    // Generic over IIndividualReferenceRow so IndividualRef (regular Individual
    // channel) and IndividualMobileRef (Mobile channel) share this exact same
    // rule without duplicating it.
    public static T PickCanonicalIndividualRef<T>(IEnumerable<T> duplicates) where T : IIndividualReferenceRow
    {
        var rows = duplicates.ToList();
        var withData = rows.Where(r => !IsBlankIndividualIdentity(r)).ToList();
        // LastUpdatedDate is tied (usually at its unset default) for every legacy
        // duplicate that's never actually been enriched/updated -- without a
        // second key, OrderBy's stable sort just keeps whatever order the
        // database happened to return, not the most recently created row.
        return withData.Count > 0
            ? withData.OrderByDescending(r => r.LastUpdatedDate).ThenByDescending(r => r.CreatedDate).First()
            : rows.OrderByDescending(r => r.CreatedDate).First();
    }

    // Same rule as PickCanonicalIndividualRef, for Business duplicates (identity
    // field is Businessname rather than DOB/Surname/FirstName/MiddleNames).
    public static bool IsBlankBusinessIdentity(BusinessRef r) => string.IsNullOrWhiteSpace(r.Businessname);

    public static BusinessRef PickCanonicalBusinessRef(IEnumerable<BusinessRef> duplicates)
    {
        var rows = duplicates.ToList();
        var withData = rows.Where(r => !IsBlankBusinessIdentity(r)).ToList();
        // Same reasoning as PickCanonicalIndividualRef -- see comment there.
        return withData.Count > 0
            ? withData.OrderByDescending(r => r.LastUpdatedDate).ThenByDescending(r => r.CreatedDate).First()
            : rows.OrderByDescending(r => r.CreatedDate).First();
    }

    // ── Option A: "last confirmed" reporting-period tracking ──────────────────
    // Derives a human-readable "MMMM yyyy" label from the filename's facility
    // date, same convention already used for the Unloadable Log header
    // (_lastUnlReportingPeriod in Home.razor). Not a full history -- just
    // "as of the most recent time we processed this subscriber's file, this
    // reference row was confirmed." Falls back to empty string if the
    // filename's date can't be parsed, rather than throwing.
    private string GetReportingPeriodLabel(string fileShortName)
    {
        var stringHelper = new StringHelper();
        // false -- the true/15-day-grace variant is for comparing disbursement
        // dates against the reporting period, not for labeling which month the
        // file itself represents. LEA0126_IND -> January 2026 (last day of that
        // month, no shift), not February from adding 15 days past Jan 31.
        var facilityDate = stringHelper.GetFacilityDateFromFileName(fileShortName, false);
        if (facilityDate.IsValid &&
            DateTime.TryParseExact(facilityDate.LastDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        }
        return string.Empty;
    }

    // ── DOB conflict recording ─────────────────────────────────────────────
    // A different DOB for the same (AccNum, CustomerID, DisbursementDate) reference
    // row is a genuine identity conflict, not something either save path should
    // silently resolve by trusting whichever file came in last. Instead of
    // overwriting, record/refresh a pending entry here so an admin can decide
    // which value is correct via the Reference Data Conflicts page. Dedupes on
    // the same key while a conflict is still unresolved, so re-processing the
    // same file repeatedly doesn't spam duplicate queue entries.
    public async Task RecordOrRefreshDobConflictAsync(
        string entityType, string subscriber, string custId, string accNum, string disbDate,
        string existingDob, string incomingDob, string fileShortName)
    {
        var pending = await _context.ReferenceDataConflicts.FirstOrDefaultAsync(c =>
            c.ResolvedDate == null &&
            c.EntityType == entityType &&
            c.SubscriberCode == subscriber &&
            c.CustomerID == custId &&
            c.CreditFacilityAccNum == accNum &&
            c.DisbursementDate == disbDate);

        if (pending != null)
        {
            pending.IncomingDOB = incomingDob;
            pending.SourceFileName = fileShortName;
            pending.DetectedDate = DateTime.Now;
        }
        else
        {
            _context.ReferenceDataConflicts.Add(new ReferenceDataConflict
            {
                EntityType = entityType,
                SubscriberCode = subscriber,
                CustomerID = custId,
                CreditFacilityAccNum = accNum,
                DisbursementDate = disbDate,
                ExistingDOB = existingDob,
                IncomingDOB = incomingDob,
                SourceFileName = fileShortName,
                DetectedDate = DateTime.Now
            });
        }
        await _context.SaveChangesAsync();
    }

    // "Register / Insert reference(s)" used to blindly bulk-insert every row as
    // a brand new IndividualRef with no lookup against what's already there --
    // never capturing Surname/FirstName/MiddleNames/ID fields or LastUpdatedDate
    // at all, and creating a fresh duplicate row on every re-run against the
    // same (AccNum, CustomerID, DisbursementDate) key instead of enriching the
    // one that already exists. SaveExcelDataToDatabaseInd already does exactly
    // the match-then-enrich-or-insert logic this needs (plus the DOB-conflict
    // flagging above), so delegate to it instead of maintaining a second,
    // divergent write path into the same table.
    public async Task Initialize_SaveExcelDataToDatabaseInd(IEnumerable<DBIndividualContext> dataFromExcel, string fileShortName)
    {
        await SaveExcelDataToDatabaseInd(dataFromExcel, fileShortName);
    }
    // See Initialize_SaveExcelDataToDatabaseInd above -- same fix, same reason.
    public async Task Initialize_SaveExcelDataToDatabaseBus(IEnumerable<DBBusinessContext> dataFromExcel, string fileShortName)
    {
        await SaveExcelDataToDatabaseBus(dataFromExcel, fileShortName);
    }
    
    
    
    public async Task<string> GetFileShortCodeFromFileNameDB(string fileShortName)
    {
        var _filename = fileShortName.Split('_')[0];
        _filename = GetFirstPartExcludeLastPart(_filename, 4);
        var subscriber = await GetSubscriberUserInfos(_filename);
        return subscriber;
    }
    public async Task<string> GetSubscribeX_or_Y(string fileShortName)
    {
        var _filename = fileShortName.Split('_')[0];
        _filename = GetFirstPartExcludeLastPart(_filename, 4);
        List<SubscribeContext> all_subscriberCategoryCode = await GetShortCodeFromSubscribeIDAsync();

        // Use null-conditional operator and null-coalescing operator to handle potential null values
        string scc = all_subscriberCategoryCode
            .FirstOrDefault(sub => sub.ShortName == _filename)?.SubCategoryCode ?? string.Empty;

        if (scc == "01")
        {
            scc = "X";
        }
        else
        {
            scc = "W";
        }
        return scc;
    }
    //Return only Shortname of a file
    public async Task<string> GetFileShortCodeFromFileName(string fileShortName)
    {
        var _filename = fileShortName.Split('_')[0];
        _filename = GetFirstPartExcludeLastPart(_filename, 4);
        return _filename;
    }
    public async Task<string> GetShortCodefromReferencing(string _filename, string type_Bus_or_ind)
    {
        var _shortcode = await GetFileShortCodeFromFileName(_filename);
        // Project only SubscriberCode instead of materializing the full entity --
        // a NULL in any other non-nullable column on a matching row (e.g.
        // DateOfBirth) would otherwise throw here and silently drop the file's
        // chip, since this call sits inside a per-file try/catch in REF_FileUpload.
        if (type_Bus_or_ind == "ind")
        {
            return await _context.IndividualsData
                .Where(s => s.SubscriberCode == _shortcode)
                .Select(s => s.SubscriberCode)
                .FirstOrDefaultAsync() ?? string.Empty;
        }
        else if (type_Bus_or_ind == "bus")
        {
            return await _context.BusinessesData
                .Where(s => s.SubscriberCode == _shortcode)
                .Select(s => s.SubscriberCode)
                .FirstOrDefaultAsync() ?? string.Empty;
        }
        else
        {
            return string.Empty;
        }
    }



    //SHARED FUNCTION INDIVIDUAL
    private async Task BulkInsertBatchIND(List<IndividualRef> batch)
    {
        await _context.BulkInsertAsync(batch, new BulkConfig
        {
            BatchSize = 4000,
            EnableStreaming = true,
            BulkCopyTimeout = 3600 // 1 hour
        });
    }
    private async Task BulkInsertBatchINDMobile(List<IndividualMobileRef> batch)
    {
        await _context.BulkInsertAsync(batch, new BulkConfig
        {
            BatchSize = 4000,
            EnableStreaming = true,
            BulkCopyTimeout = 3600 // 1 hour
        });
    }
    //SHARED FUNCTION BUSINESS
    private async Task BulkInsertBatchBUS(List<BusinessRef> batch)
    {
        await _context.BulkInsertAsync(batch, new BulkConfig
        {
            BatchSize = 4000,
            EnableStreaming = true,
            BulkCopyTimeout = 3600 // 1 hour
        });
    }
    public async Task<int> GetIndividualMaxversion()
    {
        int maxId = await _context.IndividualsData
            .MaxAsync(s => (int?)s.CurrenVersion) ?? 0; // Handle null case

        return maxId + 1;
    }
    public async Task<int> GetBusinessMaxversion()
    {
        int maxId = await _context.BusinessesData
            .MaxAsync(s => (int?)s.CurrenVersion) ?? 0; // Handle null case

        return maxId + 1;
    }

    //GET EXTETNAL DATABASE TABLE (SUBSCRIBERS)
    public async Task<string> GetSubscriberUserInfos(string _shortname)
    {
        List<SubscribeContext> contexts = await GetShortCodeFromSubscribeIDAsync();
        List<string> shortCodes = contexts.Select(context => context.ShortName).ToList();
        string specificShortCode = shortCodes.FirstOrDefault(s => s == _shortname) ?? string.Empty;
        return specificShortCode;
    }
    // Reads from the same EF-managed database as everything else now
    // (see ApplicationDbContext.SubscriberShortCodes) -- previously raw
    // ADO.NET against a separate, unmigrated "blazor-CleanserAppDB".
    // Method name/signature/return shape unchanged so every caller
    // (GetSubscriberUserInfosShotCode, GetSubscriberShortCode, etc.)
    // keeps working without modification.
    public async Task<List<SubscribeContext>> GetShortCodeFromSubscribeIDAsync()
    {
        try
        {
            return await _context.SubscriberShortCodes.ToListAsync();
        }
        catch (Exception)
        {
            // Matches the original method's behavior: swallow and return
            // empty rather than surface an exception to callers that were
            // never written expecting one from this method.
            return new List<SubscribeContext>();
        }
    }
    public async Task<List<string>> GetSubscriberUserInfosShotCode()
    {
        //return await GetShortCodeFromSubscribeIDAsync();
        // Assuming SubscribeContext has a 'ShortCode' property
        List<SubscribeContext> contexts = await GetShortCodeFromSubscribeIDAsync();
        return contexts.Select(context => context.ShortName).ToList();
    }





    //*****SettingsClass*******************
    public async Task AddSettings(SettingsClass _item, SettingsDataType settingsDataType)
    {
        var item = _item;
        item.DataType = settingsDataType;
        await _context.Settings.AddAsync(item);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteSetting(SettingsClass item, SettingsDataType settingsDataType)
    {
        var itemToRemove = await _context.Settings.FirstOrDefaultAsync(s => s.Value == item.Value && s.DataType == settingsDataType);
        _context.Settings.Remove(itemToRemove);
        await _context.SaveChangesAsync();
    }
    /// <summary>Changes the text of one saved name in place (by Id).</summary>
    public async Task UpdateSetting(int id, string newValue, SettingsDataType settingsDataType)
    {
        var item = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id && s.DataType == settingsDataType);
        if (item == null) return;
        item.Value = newValue;
        await _context.SaveChangesAsync();
    }
    public async Task<List<SettingsClass>> GetAllSettingsAsync(SettingsDataType settingsDataType)
    {
        return await _context.Settings.Where(s => s.DataType == settingsDataType).OrderBy(s => s.Value).ToListAsync();
    }
    public async Task<bool> GetSettingOne(string item, SettingsDataType settingsDataType)
    {
        return await _context.Settings.AnyAsync(s => s.Value == item && s.DataType == settingsDataType);
    }
    public async Task AddSettingsBulk(List<SettingsClass> item)
    {
        if (item.Count <= 0)
            return;
        await _context.Settings.AddRangeAsync(item);
        await _context.SaveChangesAsync();
    }
    private readonly string[] businessKeywords = {
      "CHURCH", "EMBASSY", "COMPANY", "BANK", "SCHOOL", "LTD", "LIMITED", "HOSPITAL",
        "CENTER", "CENTRE", "COMMUNICATION", "ENTERPRISE", "WORKS", "INSTITUTE", "DEV'T",
        "BUSINESS", "BUSINESSES", "INFORMATION", "SERVICE","SERVICES", "FIRM", "TRAINING", "HOTEL","AUTO","AUTOS","SCH","KIDS",
        "& CO", "& SONS", "VENTURES", "PRINTING", "OFFICE","INTERNATIONAL","GPRTU","TUC","MINISTRY","GROUP","WITH","GRP"," INT ",
        "CONSTRUCTION","SMART","LOAN","ENTRPRISE","SCHEME","AUTHORITHY","ASSOCIATION", "LOANS", "STAFF", "CONTROLLER", "EMPLOYEE",
        "COLLEGE", "ACCOUNT", "GHANA", "MOTORS", "AGENCY", "ENVIRONMENTAL", "UNIVERSITY", "PERSONAL", "UNION", "CO-OPERATIVE",
        "CO OPERATIVE", "COOPERATIVE",
        // Sprint 8 — business keywords found in IND name fields
        "SOLUTION", "SOLUTIONS", "LEGACY", "INVESTMENTS", "INVESTMENT", "ASSOCIATES",
        "PREPARATORY", "PRIDE", "HOLDINGS", "CONSULT", "CONSULTING", "CONSULTANCY",
        "TRADING", "TRADERS", "LOGISTICS", "MANAGEMENT", "PROPERTIES", "PROPERTY",
        "DEVELOPERS", "DEVELOPMENT", "FOUNDATION", "INDUSTRIES", "INDUSTRY",
        "RESOURCES", "TECHNOLOGIES", "TECHNOLOGY", "ENTERPRISES", "GLOBAL",
        "INTEGRATED", "CONCEPTS", "CONCEPT", "SYSTEMS", "NETWORK", "NETWORKS",
        "MICROFINANCE", "FINANCE", "FINANCIAL", "CAPITAL", "INSURANCE", "SAVINGS"
    };
    public async Task<List<string>> InitializeSettingsDBAsync()
    {
        var BusinessNamesSettings = await GetAllSettingsAsync(SettingsDataType.BusinessName);
        if (BusinessNamesSettings.Count <= 0)
        {
            foreach (var item in businessKeywords)
            {
               var settingsClass = new SettingsClass() { Value = item, DataType = SettingsDataType.BusinessName };
                BusinessNamesSettings.Add(settingsClass);
            }
            await AddSettingsBulk(BusinessNamesSettings);
        }
        var settings = await GetAllSettingsAsync(SettingsDataType.BusinessName);
        List<string> settingAsAnArray = new();
        foreach (var item in settings)
        {
            settingAsAnArray.Add(item.Value ?? string.Empty);
        }
        return settingAsAnArray;
    }
  
    public string GetFirstPartExcludeLastPart(string input, int n)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= n)
            return string.Empty;

        return input[..^n]; // Using range operator to remove last n characters
    }

    public async Task BUS_NUM_AddSettings(BusSettNormalizer _item)
    {
        if (_item == null) return;
        var existingItem = await _context.BusinessClassNormalizer.FirstOrDefaultAsync(s => s.ShortValue == _item.ShortValue);
        DateTime date = DateTime.Now;
        if (existingItem == null)
        {
            _item.DateCreated = date;
            _item.DateModified = date;
            await _context.BusinessClassNormalizer.AddAsync(_item);
        }
        await _context.SaveChangesAsync();
    }

    //*****BUS_NORM_SettingsClass*******************

    public async Task BUS_NUM_DeleteSetting(BusSettNormalizer item)
    {
        if (item == null) return;
        var itemToRemove = await _context.BusinessClassNormalizer.FirstOrDefaultAsync(s => s.ShortValue == item.ShortValue);
        _context.BusinessClassNormalizer.Remove(itemToRemove);
        await _context.SaveChangesAsync();
    }
    //public async Task BUS_NUM_DeleteSetting(BusSettNormalizer item)
    //{
    //    if (item == null) return;
    //    var itemToRemove = await _context.Settings.FirstOrDefaultAsync(s => s.Id == item.Id);
    //    _context.Settings.Remove(itemToRemove);
    //    await _context.SaveChangesAsync();
    //}
    /// <summary>Changes one saved short-name to full-name mapping in place (by Id) and stamps the modified date.</summary>
    public async Task BUS_NUM_UpdateSetting(int id, string shortValue, string longValue)
    {
        var item = await _context.BusinessClassNormalizer.FirstOrDefaultAsync(s => s.Id == id);
        if (item == null) return;
        item.ShortValue = shortValue;
        item.LongValue = longValue;
        item.DateModified = DateTime.Now;
        await _context.SaveChangesAsync();
    }
    public async Task<List<BusSettNormalizer>> BUS_NUM_GetAllSettingsAsync()
    {
        return await _context.BusinessClassNormalizer.OrderBy(s => s.ShortValue).ToListAsync();
    }
    public async Task<bool> BUS_NUM_GetSettingOne(string shortvalue)
    {
        return await _context.BusinessClassNormalizer.AnyAsync(s => s.ShortValue == shortvalue);
    }
    public async Task BUS_NUM_AddSettingsBulk(List<BusSettNormalizer> item)
    {
        if (item.Count <= 0)
            return;
        await _context.BusinessClassNormalizer.AddRangeAsync(item);
        await _context.SaveChangesAsync();
    }

    private readonly Dictionary<string, string> businesssShortForms = new()
        {
            { "A/C", "ACCOUNT" },
            { "ACCT", "ACCOUNT" },
            { "ASSOC", "ASSOCIATION" },
            { "ASSO", "ASSOCIATION" },
            { "CO", "COMPANY" },
            { "COM", "COMPANY" },
            { "CONST", "CONSTRUCTION" },
            { "CON", "CONSULT" },
            { "CRDT", "CREDIT" },
            { "DEVT", "DEVELOPMENT" },
            { "DEV’T", "DEVELOPMENT" },
            { "ENG", "ENGINEERING" },
            { "GH", "GHANA" },
            { "(GH)", "GHANA" },
            { "GOV", "GOVERNMENT" },
            { "GOV’T", "GOVERNMENT" },
            { "GRP", "GROUP" },
            { "INT", "INTERNATIONAL" },
            { "INV", "INVESTMENT" },
            { "LT", "LIMITED" },
            { "LTD", "LIMITED" },
            { "MKTG", "MARKETING" },
            { "ND", "AND" },
            { "SCH", "SCHOOL" },
            { "SER", "SERVICE" },
            { "SERV", "SERVICE" },
            { "SYS", "SYSTEM" },
            { "TRAD", "TRADING" },
            { "WKS", "WORKS" },
            { "WK", "WORKS" },
            { "ENT", "ENTERPRISE" },
            { "HOSP", "HOSPITAL" }
        };
    public async Task<List<BusSettNormalizer>> BUS_NUM_InitializeSettingsDBAsyncNormalizer()
    {
        var BusinessNamesSettingsNormal = await BUS_NUM_GetAllSettingsAsync();
        if (BusinessNamesSettingsNormal.Count <= 0)
        {
            foreach (var item in businesssShortForms)
            {
                var settingsClassNormal = new BusSettNormalizer() { ShortValue = item.Key, LongValue = item.Value,DateCreated = DateTime.Now, DateModified = DateTime.Now, DataType = SettingsDataType.BusinessNamenormalizer };
                BusinessNamesSettingsNormal.Add(settingsClassNormal);
            }
            await BUS_NUM_AddSettingsBulk(BusinessNamesSettingsNormal);
        }
        var settingsNormal = await BUS_NUM_GetAllSettingsAsync();
        List<BusSettNormalizer> settingAsAnArray = new();

        foreach (var item in settingsNormal)
        {
            settingAsAnArray.Add(item);
        }
        return settingAsAnArray;
    }
    public async Task<Dictionary<string, string>> BUS_NUM_InitializeSettingsDBAsyncNormalizer_Home()
    {
        var BusinessNamesSettingsNormal = await BUS_NUM_GetAllSettingsAsync();
        if (BusinessNamesSettingsNormal.Count <= 0)
        {
            foreach (var item in businesssShortForms)
            {
                var settingsClassNormal = new BusSettNormalizer() { ShortValue = item.Key, LongValue = item.Value, DateCreated = DateTime.Now, DateModified = DateTime.Now, DataType = SettingsDataType.BusinessNamenormalizer };
                BusinessNamesSettingsNormal.Add(settingsClassNormal);
            }
            await BUS_NUM_AddSettingsBulk(BusinessNamesSettingsNormal);
        }
        var settingsNormal = await BUS_NUM_GetAllSettingsAsync();
        Dictionary<string, string> settingAsAnArray = new();

        foreach (var item in settingsNormal)
        {
            if (item.ShortValue != null && item.LongValue != null)
            {
                settingAsAnArray.Add(item.ShortValue, item.LongValue);
            }

        }
        return settingAsAnArray;
    }

    // ── Unloadable Log: subscriber profile lookup (Name + Institution Type) ──
    // Filled in once per subscriber via the log-generation dialog, reused
    // automatically on every subsequent log for that same subscriber code.
    public async Task<SubscriberProfile?> GetSubscriberProfileAsync(string subscriberCode)
    {
        if (string.IsNullOrWhiteSpace(subscriberCode)) return null;
        return await _context.SubscriberProfiles
            .FirstOrDefaultAsync(p => p.SubscriberCode == subscriberCode);
    }

    public async Task SaveSubscriberProfileAsync(string subscriberCode, string subscriberName, string institutionType)
    {
        if (string.IsNullOrWhiteSpace(subscriberCode)) return;

        var existing = await _context.SubscriberProfiles
            .FirstOrDefaultAsync(p => p.SubscriberCode == subscriberCode);

        if (existing != null)
        {
            existing.SubscriberName = subscriberName;
            existing.InstitutionType = institutionType;
            existing.LastUpdatedDate = DateTime.Now;
        }
        else
        {
            _context.SubscriberProfiles.Add(new SubscriberProfile
            {
                SubscriberCode = subscriberCode,
                SubscriberName = subscriberName,
                InstitutionType = institutionType,
                LastUpdatedDate = DateTime.Now
            });
        }
        await _context.SaveChangesAsync();
    }

    // ── Unloadable Log: persist header + message/category detail ────────────
    // Runs alongside GenerateWorkbook, not instead of it -- the .xlsx download
    // is unchanged. This is what turns each one-off log into a queryable row
    // in the running history across all subscribers.
    //
    // Guarantees a SubscriberProfile row exists (FK requires it) regardless of
    // whether the "save subscriber info" checkbox was checked -- that checkbox
    // only governs whether typed name/institution type *overwrite* an existing
    // profile (handled separately by SaveSubscriberProfileAsync). Here we only
    // create a minimal profile if one is missing; we never overwrite one that
    // already exists, since that's not this method's job.
    public async Task<int> SaveUnloadableLogAsync(
        string subscriberCode,
        string subscriberName,
        string institutionType,
        UnloadableLogService.UnloadableLogHeader header,
        List<UnloadableLogService.MessageRejectionSummary> messageSummaries,
        List<(string TopLevelCategory, List<UnloadableLogService.CategoryRejectionSummary> Items)> categorySummaries)
    {
        if (string.IsNullOrWhiteSpace(subscriberCode))
            throw new ArgumentException("Subscriber code is required to save the Unloadable Log.", nameof(subscriberCode));

        var profile = await _context.SubscriberProfiles
            .FirstOrDefaultAsync(p => p.SubscriberCode == subscriberCode);

        if (profile == null)
        {
            profile = new SubscriberProfile
            {
                SubscriberCode = subscriberCode,
                SubscriberName = subscriberName,
                InstitutionType = institutionType,
                LastUpdatedDate = DateTime.Now
            };
            _context.SubscriberProfiles.Add(profile);
            await _context.SaveChangesAsync(); // need profile.Id before the header can reference it
        }

        var logHeader = new UnloadableLogHeader
        {
            SubscriberProfileId = profile.Id,
            Associate = header.Associate,
            Filename = header.Filename,
            NumberOfRecords = header.NumberOfRecords,
            ReportingPeriod = header.ReportingPeriod,
            ReportingYear = header.ReportingYear,
            DataType = header.DataType,
            Months = header.Months,
            LogYear = header.LogYear,
            Comments = string.IsNullOrWhiteSpace(header.Comments) ? null : header.Comments,
            CreatedDate = DateTime.Now
        };

        foreach (var m in messageSummaries)
        {
            logHeader.MessageDetails.Add(new UnloadableLogMessageDetail
            {
                ErrorMessage = m.ErrorMessage,
                Count = m.Count,
                Percentage = m.Percentage,
                Category = m.Category
            });
        }

        foreach (var (topLevelCategory, items) in categorySummaries)
        {
            foreach (var c in items)
            {
                logHeader.CategoryDetails.Add(new UnloadableLogCategoryDetail
                {
                    TopLevelCategory = topLevelCategory,
                    SubCategory = c.SubCategory,
                    DescriptionOfErrors = c.DescriptionOfErrors,
                    VolumeAffected = c.VolumeAffected,
                    Percentage = c.Percentage
                });
            }
        }

        _context.UnloadableLogHeaders.Add(logHeader);
        await _context.SaveChangesAsync();
        return logHeader.Id;
    }

    // ── Unloadable Log: browse/filter for the history grid ──────────────────
    // ChangeTracker.Clear() up front: this page's filters can re-run this
    // query multiple times in the same Blazor Server circuit (same scoped
    // DbContext), and without clearing, re-fetching a header already tracked
    // from a prior search throws "instance ... already being tracked" --
    // the same class of bug already hit once in the upload flow.
    public async Task<List<UnloadableLogHeader>> GetUnloadableLogHeadersAsync(string? logYear = null, string? dataType = null, int? subscriberProfileId = null)
    {
        _context.ChangeTracker.Clear();

        var query = _context.UnloadableLogHeaders
            .Include(h => h.SubscriberProfile)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(logYear))
            query = query.Where(h => h.LogYear == logYear);
        if (!string.IsNullOrWhiteSpace(dataType))
            query = query.Where(h => h.DataType == dataType);
        if (subscriberProfileId.HasValue)
            query = query.Where(h => h.SubscriberProfileId == subscriberProfileId.Value);

        return await query.OrderByDescending(h => h.CreatedDate).ToListAsync();
    }

    public async Task<List<SubscriberProfile>> GetAllSubscriberProfilesAsync()
    {
        return await _context.SubscriberProfiles.OrderBy(p => p.SubscriberName).ToListAsync();
    }

    // Looks up which associate a given uploaded filename was logged/assigned
    // to, via XDSDataLogDB's Transact.ReceivedTrans. Found=false means the
    // file isn't logged there at all; Found=true with a null/empty AssignTo
    // means it's logged but unassigned.
    public async Task<(bool Found, string? AssignTo)> GetFileAssignmentAsync(string filename)
    {
        var row = await _xdsDataLogDbContext.ReceivedTrans
            .Where(r => r.RenamedFile == filename)
            .Select(r => new { r.AssignTo })
            .FirstOrDefaultAsync();

        return row == null ? (false, null) : (true, row.AssignTo);
    }

    // Resolves a Ring.Users/Transact.ReceivedTrans.AssignTo value (stored on
    // ApplicationUser.ReceivedTransUserID) to that associate's email, for
    // display (e.g. "this file is assigned to X") or for the Unloadable Log's
    // Associate column.
    public async Task<string?> GetAssociateEmailForReceivedTransUserIdAsync(string receivedTransUserId)
    {
        if (string.IsNullOrWhiteSpace(receivedTransUserId)) return null;

        return await _context.Users
            .Where(u => u.ReceivedTransUserID == receivedTransUserId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();
    }

    // Auto-populates ReceivedTransUserID for a newly-registered account by
    // matching its email against Ring.Users.Email. Returns the matched
    // Ring.Users.UserID, or null if no Ring.Users row matched that email
    // (the column is then left for manual entry, same as a legacy account).
    public async Task<string?> AutoPopulateReceivedTransUserIdAsync(string applicationUserId, string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        var matchedUserId = await _xdsDataLogDbContext.RingUsers
            .Where(r => r.Email == email)
            .Select(r => r.UserID)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(matchedUserId)) return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == applicationUserId);
        if (user == null) return null;

        user.ReceivedTransUserID = matchedUserId;
        await _context.SaveChangesAsync();
        return matchedUserId;
    }

    // This table holds exactly one settings row (seeded once in SeedData,
    // never multiplied elsewhere) -- fetched by position, not by a pinned Id,
    // since the identity value it lands on after seeding isn't guaranteed to
    // be 1 (e.g. if the row is ever manually deleted and reseeded).
    public async Task<DataLoggingGateMessages> GetDataLoggingGateMessagesAsync()
    {
        return await _context.DataLoggingGateMessages.FirstAsync();
    }

    public async Task UpdateDataLoggingGateMessagesAsync(string notLoggedMessageTemplate, string assignedToOtherMessageTemplate)
    {
        var existing = await _context.DataLoggingGateMessages.FirstAsync();
        existing.NotLoggedMessageTemplate = notLoggedMessageTemplate;
        existing.AssignedToOtherMessageTemplate = assignedToOtherMessageTemplate;
        await _context.SaveChangesAsync();
    }

    // Records that someone tried to run a file with no Transact.ReceivedTrans
    // entry through Run Cleanser -- there's no bypass for this (that's what
    // Clean Only is for), so this exists purely for admin visibility into
    // which files are still waiting to be logged. Upserted per filename so
    // repeated retries on the same file update one row instead of piling up.
    public async Task RecordUnloggedFileAttemptAsync(string filename, string attemptedByEmail)
    {
        var existing = await _context.DataLoggingUnloggedAttempts.FirstOrDefaultAsync(a => a.Filename == filename);
        var now = DateTime.Now;
        var (dataProvider, subCode, subXDSCode, subCategoryCode) = await GetDataProviderInfoForFilenameAsync(filename);

        if (existing == null)
        {
            _context.DataLoggingUnloggedAttempts.Add(new DataLoggingUnloggedAttempt
            {
                Filename = filename,
                LastAttemptedByEmail = attemptedByEmail,
                FirstAttemptedDate = now,
                LastAttemptedDate = now,
                AttemptCount = 1,
                DataProvider = dataProvider,
                SubCode = subCode,
                SubXDSCode = subXDSCode,
                SubCategoryCode = subCategoryCode
            });
        }
        else
        {
            existing.LastAttemptedByEmail = attemptedByEmail;
            existing.LastAttemptedDate = now;
            existing.AttemptCount += 1;
            existing.DataProvider = dataProvider;
            existing.SubCode = subCode;
            existing.SubXDSCode = subXDSCode;
            existing.SubCategoryCode = subCategoryCode;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<DataLoggingUnloggedAttempt>> GetUnloggedFileAttemptsAsync()
    {
        return await _context.DataLoggingUnloggedAttempts
            .OrderByDescending(a => a.LastAttemptedDate)
            .ToListAsync();
    }

    // Resolves a filename to its Data Provider display name plus SubCode/
    // SubXDSCode/SubCategoryCode, via the same short-code extraction used
    // everywhere else (GetFileShortCodeFromFileName) against the real
    // Subscriber.Subscribers table in XDSDataLogDB (same connection as
    // Transact.ReceivedTrans/Ring.Users -- see XdsDataLogDbContext).
    // DataProvider falls back to the short code itself if there's no
    // matching subscriber row, so callers never end up with a blank display
    // name; the codes stay null in that case since there's nothing to spool.
    // Both codes are kept rather than just one: SubCode is what
    // Transact.ReceivedTrans itself keys off (so joining straight back to it
    // is direct), while SubXDSCode is the cross-system identifier needed for
    // a future additional server/database that won't have SubCode.
    public async Task<(string DataProvider, string? SubCode, string? SubXDSCode, string? SubCategoryCode)> GetDataProviderInfoForFilenameAsync(string filename)
    {
        var shortCode = await GetFileShortCodeFromFileName(filename);
        var row = await _xdsDataLogDbContext.Subscribers
            .Where(s => s.ShortName == shortCode)
            .Select(s => new { s.SubName, s.SubCode, s.SubXDSCode, s.SubCategoryCode })
            .FirstOrDefaultAsync();

        var dataProvider = !string.IsNullOrWhiteSpace(row?.SubName) ? row!.SubName! : shortCode;
        return (dataProvider, row?.SubCode, row?.SubXDSCode, row?.SubCategoryCode);
    }

    // SubCategoryCode -> description (e.g. "09" -> "Others"), from the real
    // Subscriber.SubscriberCategory table in XDSDataLogDB. Used to resolve
    // the raw code stored on DataLoggingAccessRequest/DataLoggingUnloggedAttempt
    // into something readable on an admin grid. Loaded whole and joined
    // in-memory by callers rather than queried per row.
    public async Task<Dictionary<string, string>> GetSubscriberCategoryLookupAsync()
    {
        return await _xdsDataLogDbContext.SubscriberCategories
            .Where(c => c.SubCategoryCode != null)
            .ToDictionaryAsync(c => c.SubCategoryCode!, c => c.CatDescription ?? string.Empty);
    }

    // Full Data Provider list for dropdowns -- real Subscriber.Subscribers in
    // XDSDataLogDB, joined in-memory with Subscriber.SubscriberCategory so
    // each option can show its category description alongside the name.
    public async Task<List<DataProviderOption>> GetAllDataProvidersAsync()
    {
        var categoryLookup = await GetSubscriberCategoryLookupAsync();
        var subscribers = await _xdsDataLogDbContext.Subscribers
            .Where(s => s.ShortName != null)
            .OrderBy(s => s.SubName)
            .ToListAsync();

        return subscribers.Select(s => new DataProviderOption
        {
            ShortName = s.ShortName!,
            SubName = s.SubName ?? string.Empty,
            CategoryDescription = !string.IsNullOrWhiteSpace(s.SubCategoryCode) && categoryLookup.TryGetValue(s.SubCategoryCode, out var desc)
                ? desc
                : null
        }).ToList();
    }

    // Every UnloadableLogHeader the caller is allowed to see, enriched with
    // its Data Provider identity (resolved fresh from Subscriber.Subscribers
    // via the filename's short code -- UnloadableLogHeader's own
    // SubscriberProfile link is the local, legacy table and carries no
    // category). Admin sees every row; a regular user sees only rows whose
    // filename is currently assigned to them in Transact.ReceivedTrans --
    // the same check RunCleanser's gate uses, reused here for visibility.
    // Loads everything into memory in one pass rather than re-querying per
    // filter change -- the report page filters/aggregates this same list
    // client-side as the user adjusts filters, so charts update instantly.
    // The set of filenames currently assigned to a user in Transact.ReceivedTrans
    // -- shared by GetUnloadableLogReportDataAsync and GetUnloadableRecordsCountAsync
    // so both apply the exact same visibility rule.
    private async Task<HashSet<string>> GetAssignedFilenamesAsync(string receivedTransUserId)
    {
        var assignedFilenames = await _xdsDataLogDbContext.ReceivedTrans
            .Where(r => r.AssignTo == receivedTransUserId && r.RenamedFile != null)
            .Select(r => r.RenamedFile!)
            .ToListAsync();
        return new HashSet<string>(assignedFilenames, StringComparer.OrdinalIgnoreCase);
    }

    // Lightweight count for the sidebar's "Unloadable log" badge -- same
    // visibility rule as GetUnloadableLogReportDataAsync, but without
    // loading MessageDetails/CategoryDetails, since all this needs is a sum.
    public async Task<int> GetUnloadableRecordsCountAsync(bool isAdmin, string? currentUserReceivedTransId)
    {
        var headers = await _context.UnloadableLogHeaders
            .Select(h => new { h.Filename, h.NumberOfRecords })
            .ToListAsync();

        if (isAdmin) return headers.Sum(h => h.NumberOfRecords);

        if (string.IsNullOrWhiteSpace(currentUserReceivedTransId)) return 0;

        var assignedSet = await GetAssignedFilenamesAsync(currentUserReceivedTransId);
        return headers.Where(h => assignedSet.Contains(h.Filename)).Sum(h => h.NumberOfRecords);
    }

    // Lightweight counts for the sidebar badges (open items an admin needs to act on).
    public Task<int> GetPendingReferenceDataConflictsCountAsync() =>
        _context.ReferenceDataConflicts.CountAsync(c => c.ResolvedDate == null);

    public Task<int> GetPendingAccessRequestsCountAsync() =>
        _context.DataLoggingAccessRequests.CountAsync(r => r.Status == DataLoggingAccessRequestStatus.Pending);

    // ── Dynamic sidebar navigation ──────────────────────────────────────────
    public async Task<List<NavSection>> GetNavSectionsAsync()
    {
        return await _context.NavSections
            .Include(s => s.Items)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task UpdateNavSectionTitleAsync(int sectionId, string title)
    {
        var section = await _context.NavSections.FirstOrDefaultAsync(s => s.Id == sectionId);
        if (section == null) return;

        section.Title = title;
        await _context.SaveChangesAsync();
    }

    public async Task<NavSection> AddNavSectionAsync(string title)
    {
        var maxOrder = await _context.NavSections.MaxAsync(s => (int?)s.DisplayOrder) ?? 0;
        var section = new NavSection { Title = title, DisplayOrder = maxOrder + 1 };
        _context.NavSections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }

    // Cascades to the section's items (see OnModelCreating) -- the admin UI
    // confirms with the user before calling this, since it's not reversible.
    public async Task DeleteNavSectionAsync(int sectionId)
    {
        var section = await _context.NavSections.FirstOrDefaultAsync(s => s.Id == sectionId);
        if (section == null) return;

        _context.NavSections.Remove(section);
        await _context.SaveChangesAsync();
    }

    public async Task AddNavItemAsync(int sectionId, string label, string href, string? iconKey, string? requiredRoles)
    {
        var maxOrder = await _context.NavItems
            .Where(i => i.NavSectionId == sectionId)
            .MaxAsync(i => (int?)i.DisplayOrder) ?? 0;

        _context.NavItems.Add(new NavItem
        {
            NavSectionId = sectionId,
            Label = label,
            Href = href,
            IconKey = iconKey,
            RequiredRoles = requiredRoles,
            DisplayOrder = maxOrder + 1
        });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateNavItemAsync(int itemId, string label, string href, string? iconKey, string? requiredRoles)
    {
        var item = await _context.NavItems.FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return;

        item.Label = label;
        item.Href = href;
        item.IconKey = iconKey;
        item.RequiredRoles = requiredRoles;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNavItemAsync(int itemId)
    {
        var item = await _context.NavItems.FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return;

        _context.NavItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    // Drag-and-drop reassignment: moves an item to a (possibly different)
    // section, placed last within it.
    public async Task MoveNavItemToSectionAsync(int itemId, int newSectionId)
    {
        var item = await _context.NavItems.FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return;

        var maxOrder = await _context.NavItems
            .Where(i => i.NavSectionId == newSectionId)
            .MaxAsync(i => (int?)i.DisplayOrder) ?? 0;

        item.NavSectionId = newSectionId;
        item.DisplayOrder = maxOrder + 1;
        await _context.SaveChangesAsync();
    }

    public async Task<List<UnloadableLogReportRow>> GetUnloadableLogReportDataAsync(bool isAdmin, string? currentUserReceivedTransId)
    {
        var headers = await _context.UnloadableLogHeaders
            .Include(h => h.MessageDetails)
            .Include(h => h.CategoryDetails)
            .ToListAsync();

        if (!isAdmin)
        {
            if (string.IsNullOrWhiteSpace(currentUserReceivedTransId))
            {
                return new List<UnloadableLogReportRow>();
            }

            var assignedSet = await GetAssignedFilenamesAsync(currentUserReceivedTransId);
            headers = headers.Where(h => assignedSet.Contains(h.Filename)).ToList();
        }

        // Resolve every header's short code once, then batch-query
        // Subscriber.Subscribers for all of them in a single round trip
        // instead of one query per header.
        var shortCodeByHeaderId = new Dictionary<int, string>();
        foreach (var h in headers)
        {
            shortCodeByHeaderId[h.Id] = await GetFileShortCodeFromFileName(h.Filename);
        }

        var distinctShortCodes = shortCodeByHeaderId.Values.Distinct().ToList();
        var subscriberRows = await _xdsDataLogDbContext.Subscribers
            .Where(s => s.ShortName != null && distinctShortCodes.Contains(s.ShortName))
            .ToListAsync();
        var subscribersByShortCode = subscriberRows
            .Where(s => s.ShortName != null)
            .GroupBy(s => s.ShortName!)
            .ToDictionary(g => g.Key, g => g.First());

        var categoryLookup = await GetSubscriberCategoryLookupAsync();

        var result = new List<UnloadableLogReportRow>();
        foreach (var h in headers)
        {
            var shortCode = shortCodeByHeaderId[h.Id];
            subscribersByShortCode.TryGetValue(shortCode, out var subscriber);
            var categoryDescription = !string.IsNullOrWhiteSpace(subscriber?.SubCategoryCode) &&
                categoryLookup.TryGetValue(subscriber.SubCategoryCode, out var desc) ? desc : null;

            result.Add(new UnloadableLogReportRow
            {
                HeaderId = h.Id,
                Filename = h.Filename,
                DataProvider = !string.IsNullOrWhiteSpace(subscriber?.SubName) ? subscriber!.SubName! : shortCode,
                SubCode = subscriber?.SubCode,
                SubXDSCode = subscriber?.SubXDSCode,
                SubCategoryCode = subscriber?.SubCategoryCode,
                SubCategoryDescription = categoryDescription,
                Associate = h.Associate,
                NumberOfRecords = h.NumberOfRecords,
                ReportingPeriod = h.ReportingPeriod,
                ReportingYear = h.ReportingYear,
                DataType = h.DataType,
                Months = h.Months,
                LogYear = h.LogYear,
                DateEmailed = h.DateEmailed,
                DateFixed = h.DateFixed,
                Comments = h.Comments,
                CreatedDate = h.CreatedDate,
                MessageDetails = h.MessageDetails,
                CategoryDetails = h.CategoryDetails
            });
        }

        return result;
    }

    public async Task<List<DataLoggingAccessRequestReason>> GetAccessRequestReasonsAsync()
    {
        return await _context.DataLoggingAccessRequestReasons.OrderBy(r => r.Reason).ToListAsync();
    }

    public async Task AddAccessRequestReasonAsync(string reason)
    {
        _context.DataLoggingAccessRequestReasons.Add(new DataLoggingAccessRequestReason { Reason = reason });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAccessRequestReasonAsync(int id, string reason)
    {
        var existing = await _context.DataLoggingAccessRequestReasons.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return;

        existing.Reason = reason;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAccessRequestReasonAsync(int id)
    {
        var existing = await _context.DataLoggingAccessRequestReasons.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return;

        _context.DataLoggingAccessRequestReasons.Remove(existing);
        await _context.SaveChangesAsync();
    }

    // One row per selected file, all sharing the reason/requester/timestamp --
    // covers the batch-cleaning case where several blocked files are
    // requested together in one submission.
    public async Task SubmitAccessRequestsAsync(
        List<(string Filename, string DataProvider, string? SubCode, string? SubXDSCode, string? SubCategoryCode, string AssignedToEmail)> files,
        string requestedByEmail, string reason)
    {
        var now = DateTime.Now;
        foreach (var file in files)
        {
            _context.DataLoggingAccessRequests.Add(new DataLoggingAccessRequest
            {
                Filename = file.Filename,
                DataProvider = file.DataProvider,
                SubCode = file.SubCode,
                SubXDSCode = file.SubXDSCode,
                SubCategoryCode = file.SubCategoryCode,
                RequestedByEmail = requestedByEmail,
                AssignedToEmail = file.AssignedToEmail,
                Reason = reason,
                RequestDate = now,
                Status = DataLoggingAccessRequestStatus.Pending
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<DataLoggingAccessRequest>> GetAllAccessRequestsAsync()
    {
        return await _context.DataLoggingAccessRequests
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();
    }

    public async Task<List<DataLoggingAccessRequest>> GetAccessRequestsForUserAsync(string requestedByEmail)
    {
        return await _context.DataLoggingAccessRequests
            .Where(r => r.RequestedByEmail == requestedByEmail)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();
    }

    public async Task ReviewAccessRequestAsync(int requestId, bool approve, string reviewedByEmail)
    {
        var existing = await _context.DataLoggingAccessRequests.FirstOrDefaultAsync(r => r.Id == requestId);
        if (existing == null) return;

        existing.Status = approve ? DataLoggingAccessRequestStatus.Approved : DataLoggingAccessRequestStatus.Denied;
        existing.ReviewedByEmail = reviewedByEmail;
        existing.ReviewedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    // Gate check: has this user got a standing approval to clean this
    // specific file despite it being assigned to someone else? Checked fresh
    // on every Run Cleanser attempt, not cached -- admin can still see/revoke
    // by denying a previously-approved request if needed.
    public async Task<bool> HasApprovedAccessAsync(string filename, string requestedByEmail)
    {
        return await _context.DataLoggingAccessRequests.AnyAsync(r =>
            r.Filename == filename &&
            r.RequestedByEmail == requestedByEmail &&
            r.Status == DataLoggingAccessRequestStatus.Approved);
    }

    public async Task<List<DataLoggingCleaningPurposeReason>> GetCleaningPurposeReasonsAsync()
    {
        return await _context.DataLoggingCleaningPurposeReasons.OrderBy(r => r.Reason).ToListAsync();
    }

    public async Task AddCleaningPurposeReasonAsync(string reason)
    {
        _context.DataLoggingCleaningPurposeReasons.Add(new DataLoggingCleaningPurposeReason { Reason = reason });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCleaningPurposeReasonAsync(int id, string reason)
    {
        var existing = await _context.DataLoggingCleaningPurposeReasons.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return;

        existing.Reason = reason;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCleaningPurposeReasonAsync(int id)
    {
        var existing = await _context.DataLoggingCleaningPurposeReasons.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return;

        _context.DataLoggingCleaningPurposeReasons.Remove(existing);
        await _context.SaveChangesAsync();
    }

    // Records which purpose was selected for a file that actually proceeded
    // to cleaning (owner match, admin, or approved access -- never a blocked
    // file). Purely for admin visibility; nothing reads this back.
    public async Task RecordCleaningPurposeAsync(string filename, string performedByEmail, string purpose)
    {
        var (dataProvider, subCode, subXDSCode, subCategoryCode) = await GetDataProviderInfoForFilenameAsync(filename);

        _context.DataLoggingCleaningPurposeLogs.Add(new DataLoggingCleaningPurposeLog
        {
            Filename = filename,
            DataProvider = dataProvider,
            SubCode = subCode,
            SubXDSCode = subXDSCode,
            SubCategoryCode = subCategoryCode,
            PerformedByEmail = performedByEmail,
            Purpose = purpose,
            PerformedDate = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    // Who may clean a file that is already in Transact.ReceivedTrans, with "Clean only" ticked:
    // its owner, an admin, or someone with admin-approved access. Returns the basis, or null when none applies.
    public async Task<string?> GetReCleanAccessBasisAsync(string assignedUserId, string? currentUserReceivedTransId, bool isAdmin, string? email, string filename)
    {
        if (string.Equals(assignedUserId, currentUserReceivedTransId, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(currentUserReceivedTransId)) return "Owner";
        if (isAdmin) return "Admin";
        if (!string.IsNullOrWhiteSpace(email) && await HasApprovedAccessAsync(filename, email)) return "Approved access";
        return null;
    }

    // Records one file cleaned with "Clean only" ticked. Purely for admin visibility.
    public async Task RecordCleanOnlyRunAsync(string filename, string performedByEmail, bool fileInReceivedTrans, string? assignedToEmail, string accessBasis, string? purpose = null)
    {
        var (dataProvider, subCode, subXDSCode, subCategoryCode) = await GetDataProviderInfoForFilenameAsync(filename);

        _context.DataLoggingCleanOnlyLogs.Add(new DataLoggingCleanOnlyLog
        {
            Filename = filename,
            DataProvider = dataProvider,
            SubCode = subCode,
            SubXDSCode = subXDSCode,
            SubCategoryCode = subCategoryCode,
            PerformedByEmail = performedByEmail,
            PerformedDate = DateTime.Now,
            FileInReceivedTrans = fileInReceivedTrans,
            AssignedToEmail = assignedToEmail,
            AccessBasis = accessBasis,
            Purpose = purpose
        });
        await _context.SaveChangesAsync();
    }

    // The dashboard's "Recent cleaning runs": the two logs side by side, newest first. Pass an email to see
    // only that person's runs (non-admins), or null for everyone's (admins).
    public async Task<List<CleaningRunRow>> GetRecentCleaningRunsAsync(string? onlyEmail, string? dataProvider = null, string? categoryCode = null,
        DateTime? from = null, DateTime? to = null, string? associate = null, int take = 200)
    {
        var withRef = _context.DataLoggingCleaningPurposeLogs.AsNoTracking().AsQueryable();
        var cleanOnly = _context.DataLoggingCleanOnlyLogs.AsNoTracking().AsQueryable();
        // an ordinary user is always limited to their own runs; the Associate filter only applies to admins
        var who = onlyEmail ?? (string.IsNullOrWhiteSpace(associate) ? null : associate);
        if (who != null)
        {
            withRef = withRef.Where(l => l.PerformedByEmail == who);
            cleanOnly = cleanOnly.Where(l => l.PerformedByEmail == who);
        }
        if (!string.IsNullOrWhiteSpace(dataProvider))
        {
            withRef = withRef.Where(l => l.DataProvider == dataProvider);
            cleanOnly = cleanOnly.Where(l => l.DataProvider == dataProvider);
        }
        if (!string.IsNullOrWhiteSpace(categoryCode))
        {
            withRef = withRef.Where(l => l.SubCategoryCode == categoryCode);
            cleanOnly = cleanOnly.Where(l => l.SubCategoryCode == categoryCode);
        }
        if (from != null)
        {
            var f = from.Value.Date;
            withRef = withRef.Where(l => l.PerformedDate >= f);
            cleanOnly = cleanOnly.Where(l => l.PerformedDate >= f);
        }
        if (to != null)
        {
            var t = to.Value.Date.AddDays(1);   // the chosen day is included
            withRef = withRef.Where(l => l.PerformedDate < t);
            cleanOnly = cleanOnly.Where(l => l.PerformedDate < t);
        }

        var a = (await withRef.OrderByDescending(l => l.PerformedDate).Take(take).ToListAsync())
            .Select(l => new CleaningRunRow { Kind = "Cleaned with reference check", Filename = l.Filename, DataProvider = l.DataProvider, SubCategoryCode = l.SubCategoryCode, PerformedByEmail = l.PerformedByEmail, PerformedDate = l.PerformedDate, Detail = l.Purpose });
        var b = (await cleanOnly.OrderByDescending(l => l.PerformedDate).Take(take).ToListAsync())
            .Select(l => new CleaningRunRow
            {
                Kind = l.FileInReceivedTrans ? "Re-cleaned" : "Clean only",
                Filename = l.Filename, DataProvider = l.DataProvider, SubCategoryCode = l.SubCategoryCode, PerformedByEmail = l.PerformedByEmail, PerformedDate = l.PerformedDate,
                Detail = l.FileInReceivedTrans
                    ? (string.IsNullOrWhiteSpace(l.Purpose) ? l.AccessBasis : $"{l.Purpose} ({l.AccessBasis})")
                    : "Not in ReceivedTrans"
            });
        return a.Concat(b).OrderByDescending(r => r.PerformedDate).Take(take).ToList();
    }

    // Values for the list's filter drop-downs, from the runs this person may see (null email = everyone's).
    public async Task<CleaningRunFilterOptions> GetCleaningRunFilterOptionsAsync(string? onlyEmail)
    {
        var withRef = _context.DataLoggingCleaningPurposeLogs.AsNoTracking().AsQueryable();
        var cleanOnly = _context.DataLoggingCleanOnlyLogs.AsNoTracking().AsQueryable();
        if (onlyEmail != null)
        {
            withRef = withRef.Where(l => l.PerformedByEmail == onlyEmail);
            cleanOnly = cleanOnly.Where(l => l.PerformedByEmail == onlyEmail);
        }
        var p1 = await withRef.Select(l => new { l.DataProvider, l.SubCategoryCode, l.PerformedByEmail }).Distinct().ToListAsync();
        var p2 = await cleanOnly.Select(l => new { l.DataProvider, l.SubCategoryCode, l.PerformedByEmail }).Distinct().ToListAsync();
        var all = p1.Concat(p2).ToList();
        return new CleaningRunFilterOptions
        {
            DataProviders = all.Select(x => x.DataProvider).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList(),
            CategoryCodes = all.Select(x => x.SubCategoryCode ?? string.Empty).Where(x => x.Length > 0).Distinct().OrderBy(x => x).ToList(),
            Associates = all.Select(x => x.PerformedByEmail).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList()
        };
    }

    public async Task<List<DataLoggingCleaningPurposeLog>> GetCleaningPurposeLogAsync()
    {
        return await _context.DataLoggingCleaningPurposeLogs
            .OrderByDescending(l => l.PerformedDate)
            .ToListAsync();
    }

    public async Task<List<UnloadableErrorCatalogEntry>> GetUnloadableErrorCatalogAsync()
    {
        return await _context.UnloadableErrorCatalogEntries
            .OrderBy(e => e.TopLevelCategory).ThenBy(e => e.SubCategory)
            .ToListAsync();
    }

    public async Task AddUnloadableErrorCatalogEntryAsync(string topLevelCategory, string subCategory, string descriptionOfErrors)
    {
        _context.UnloadableErrorCatalogEntries.Add(new UnloadableErrorCatalogEntry
        {
            TopLevelCategory = topLevelCategory,
            SubCategory = subCategory,
            DescriptionOfErrors = descriptionOfErrors,
            LastUpdatedDate = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUnloadableErrorCatalogEntryAsync(int id, string topLevelCategory, string subCategory, string descriptionOfErrors)
    {
        var existing = await _context.UnloadableErrorCatalogEntries.FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return;

        existing.TopLevelCategory = topLevelCategory;
        existing.SubCategory = subCategory;
        existing.DescriptionOfErrors = descriptionOfErrors;
        existing.LastUpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUnloadableErrorCatalogEntryAsync(int id)
    {
        var existing = await _context.UnloadableErrorCatalogEntries.FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return;

        _context.UnloadableErrorCatalogEntries.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetUnloadableLogYearsAsync()
    {
        return await _context.UnloadableLogHeaders
            .Select(h => h.LogYear)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();
    }

    // Only DateEmailed/DateFixed/Comments are ever updated here -- everything
    // else on the header is a snapshot of what happened at generation time
    // and isn't meant to change after the fact.
    public async Task UpdateUnloadableLogAsync(UnloadableLogHeader updated)
    {
        var existing = await _context.UnloadableLogHeaders
            .FirstOrDefaultAsync(h => h.Id == updated.Id);
        if (existing == null) return;

        existing.DateEmailed = updated.DateEmailed;
        existing.DateFixed = updated.DateFixed;
        existing.Comments = string.IsNullOrWhiteSpace(updated.Comments) ? null : updated.Comments;
        await _context.SaveChangesAsync();
    }

    // ── Unloadable Log: dashboard aggregates ─────────────────────────────────
    public async Task<(int TotalRuns, int TotalUnlRecords, int AwaitingEmailCount, int EmailedNotFixedCount)>
        GetUnloadableLogDashboardSummaryAsync(string? logYear = null)
    {
        var query = _context.UnloadableLogHeaders.AsQueryable();
        if (!string.IsNullOrWhiteSpace(logYear))
            query = query.Where(h => h.LogYear == logYear);

        int totalRuns = await query.CountAsync();
        int totalUnlRecords = await query.SumAsync(h => (int?)h.NumberOfRecords) ?? 0;
        int awaitingEmail = await query.CountAsync(h => h.DateEmailed == null);
        int emailedNotFixed = await query.CountAsync(h => h.DateEmailed != null && h.DateFixed == null);

        return (totalRuns, totalUnlRecords, awaitingEmail, emailedNotFixed);
    }

    // Cross-subscriber "what's actually driving rejections" -- Section 3.5
    // in CleanserDB_Scripts.sql, rendered instead of hand-run.
    public async Task<List<(string SubCategory, int TotalVolume)>> GetTopCategoriesAsync(string? logYear = null, int take = 8)
    {
        var query = _context.UnloadableLogCategoryDetails
            .Include(c => c.UnloadableLogHeader)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(logYear))
            query = query.Where(c => c.UnloadableLogHeader!.LogYear == logYear);

        var grouped = await query
            .GroupBy(c => c.SubCategory)
            .Select(g => new { SubCategory = g.Key, TotalVolume = g.Sum(x => x.VolumeAffected) })
            .OrderByDescending(g => g.TotalVolume)
            .Take(take)
            .ToListAsync();

        return grouped.Select(g => (g.SubCategory, g.TotalVolume)).ToList();
    }

    // Monthly UNL volume trend. Months is stored as a name ("January"), not
    // chronologically sortable in SQL -- grouped in SQL, then ordered in
    // memory using the actual month number.
    public async Task<List<(string Label, int TotalRecords)>> GetMonthlyTrendAsync(string? logYear = null)
    {
        var query = _context.UnloadableLogHeaders.AsQueryable();
        if (!string.IsNullOrWhiteSpace(logYear))
            query = query.Where(h => h.LogYear == logYear);

        var raw = await query
            .GroupBy(h => new { h.LogYear, h.Months })
            .Select(g => new { g.Key.LogYear, g.Key.Months, TotalRecords = g.Sum(x => x.NumberOfRecords) })
            .ToListAsync();

        return raw
            .Select(r => new { r.LogYear, r.Months, r.TotalRecords, SortKey = MonthSortKey(r.LogYear, r.Months) })
            .OrderBy(r => r.SortKey)
            .Select(r => ($"{r.Months} {r.LogYear}", r.TotalRecords))
            .ToList();
    }

    private static DateTime MonthSortKey(string year, string monthName)
    {
        if (int.TryParse(year, out int y) &&
            DateTime.TryParseExact(monthName, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return new DateTime(y, parsed.Month, 1);
        }
        return DateTime.MinValue;
    }

    // Most common individual error messages across every subscriber --
    // Section 3.7 in CleanserDB_Scripts.sql.
    public async Task<List<(string ErrorMessage, string Category, int TotalCount)>> GetTopErrorMessagesAsync(string? logYear = null, int take = 10)
    {
        var query = _context.UnloadableLogMessageDetails
            .Include(m => m.UnloadableLogHeader)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(logYear))
            query = query.Where(m => m.UnloadableLogHeader!.LogYear == logYear);

        var grouped = await query
            .GroupBy(m => new { m.ErrorMessage, m.Category })
            .Select(g => new { g.Key.ErrorMessage, g.Key.Category, TotalCount = g.Sum(x => x.Count) })
            .OrderByDescending(g => g.TotalCount)
            .Take(take)
            .ToListAsync();

        return grouped.Select(g => (g.ErrorMessage, g.Category, g.TotalCount)).ToList();
    }

    // ── Reference Data Conflicts: admin review/correction ──────────────────────
    // (see ReferenceDataConflicts.razor, admin-only)

    public async Task<List<ReferenceDataConflict>> GetPendingReferenceDataConflictsAsync()
    {
        return await _context.ReferenceDataConflicts
            .Where(c => c.ResolvedDate == null)
            .OrderByDescending(c => c.DetectedDate)
            .ToListAsync();
    }

    // Writes the admin-chosen DOB onto the actual reference row and marks the
    // conflict resolved. Returns false if the conflict or its target row can't
    // be found (e.g. the row was deleted since the conflict was recorded).
    public async Task<bool> ResolveReferenceDataConflictAsync(int conflictId, string correctedDob, string resolvedBy, string? notes)
    {
        var conflict = await _context.ReferenceDataConflicts.FindAsync(conflictId);
        if (conflict == null || conflict.ResolvedDate != null) return false;

        bool applied;
        if (conflict.EntityType == "Individual")
        {
            var matches = await _context.IndividualsData.Where(r =>
                r.SubscriberCode == conflict.SubscriberCode && r.CustomerID == conflict.CustomerID &&
                r.CreditFacilityAccNum == conflict.CreditFacilityAccNum && r.DisbursementDate == conflict.DisbursementDate)
                .ToListAsync();
            // Duplicates for this key resolve to the same canonical row the
            // save/enrich path and matcher already agree on, not just whichever
            // one the database returns first.
            var row = matches.Count > 0 ? PickCanonicalIndividualRef(matches) : null;
            applied = row != null;
            if (row != null) { row.DateOfBirth = correctedDob; row.LastUpdatedDate = DateTime.Now; }
        }
        else if (conflict.EntityType == "IndividualMobile")
        {
            var matches = await _context.IndividualsMobileData.Where(r =>
                r.SubscriberCode == conflict.SubscriberCode && r.CustomerID == conflict.CustomerID &&
                r.CreditFacilityAccNum == conflict.CreditFacilityAccNum && r.DisbursementDate == conflict.DisbursementDate)
                .ToListAsync();
            var row = matches.Count > 0 ? PickCanonicalIndividualRef(matches) : null;
            applied = row != null;
            if (row != null) { row.DateOfBirth = correctedDob; row.LastUpdatedDate = DateTime.Now; }
        }
        else
        {
            var matches = await _context.BusinessesData.Where(r =>
                r.SubscriberCode == conflict.SubscriberCode && r.CustomerID == conflict.CustomerID &&
                r.CreditFacilityAccNum == conflict.CreditFacilityAccNum && r.DisbursementDate == conflict.DisbursementDate)
                .ToListAsync();
            var row = matches.Count > 0 ? PickCanonicalBusinessRef(matches) : null;
            applied = row != null;
            if (row != null) { row.DateOfBirth = correctedDob; row.LastUpdatedDate = DateTime.Now; }
        }

        conflict.ResolvedDate = DateTime.Now;
        conflict.ResolvedBy = resolvedBy;
        conflict.ResolutionNotes = notes;
        await _context.SaveChangesAsync();
        return applied;
    }

    // ── Direct search + manual correction (outside the conflict queue) ─────────
    // For fixing a record nobody's file ever conflicted against but that's
    // still known to be wrong (e.g. reported by a subscriber).

    // CustomerID and accountNo are each optional, but at least one of the two
    // is expected by the caller -- whichever is supplied narrows the search;
    // supplying both narrows by both (AND), not either/or.
    public async Task<List<IndividualRef>> SearchIndividualRefAsync(string subscriberCode, string? customerId, string? accountNo = null)
    {
        var query = _context.IndividualsData.Where(r => r.SubscriberCode == subscriberCode);
        if (!string.IsNullOrWhiteSpace(customerId)) query = query.Where(r => r.CustomerID == customerId);
        if (!string.IsNullOrWhiteSpace(accountNo)) query = query.Where(r => r.CreditFacilityAccNum == accountNo);
        return await query.ToListAsync();
    }

    public async Task<List<IndividualMobileRef>> SearchIndividualMobileRefAsync(string subscriberCode, string? customerId, string? accountNo = null)
    {
        var query = _context.IndividualsMobileData.Where(r => r.SubscriberCode == subscriberCode);
        if (!string.IsNullOrWhiteSpace(customerId)) query = query.Where(r => r.CustomerID == customerId);
        if (!string.IsNullOrWhiteSpace(accountNo)) query = query.Where(r => r.CreditFacilityAccNum == accountNo);
        return await query.ToListAsync();
    }

    public async Task<List<BusinessRef>> SearchBusinessRefAsync(string subscriberCode, string? customerId, string? accountNo = null)
    {
        var query = _context.BusinessesData.Where(r => r.SubscriberCode == subscriberCode);
        if (!string.IsNullOrWhiteSpace(customerId)) query = query.Where(r => r.CustomerID == customerId);
        if (!string.IsNullOrWhiteSpace(accountNo)) query = query.Where(r => r.CreditFacilityAccNum == accountNo);
        return await query.ToListAsync();
    }

    public async Task UpdateIndividualRefIdentityAsync(int id, string dob, string natId, string votersId,
        string driverLic, string passport, string ssNum, string ezwich, string otherId)
    {
        var row = await _context.IndividualsData.FindAsync(id);
        if (row == null) return;
        row.DateOfBirth = dob;
        row.NatIDNum = natId; row.VotersIDNum = votersId; row.DriverLicNum = driverLic;
        row.PassportNum = passport; row.SSNum = ssNum; row.EzwichNum = ezwich; row.OtherIDNum = otherId;
        row.LastUpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateIndividualMobileRefIdentityAsync(int id, string dob, string natId, string votersId,
        string driverLic, string passport, string ssNum, string ezwich, string otherId)
    {
        var row = await _context.IndividualsMobileData.FindAsync(id);
        if (row == null) return;
        row.DateOfBirth = dob;
        row.NatIDNum = natId; row.VotersIDNum = votersId; row.DriverLicNum = driverLic;
        row.PassportNum = passport; row.SSNum = ssNum; row.EzwichNum = ezwich; row.OtherIDNum = otherId;
        row.LastUpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBusinessRefIdentityAsync(int id, string busRegNum, string tinNum)
    {
        var row = await _context.BusinessesData.FindAsync(id);
        if (row == null) return;
        row.Busregnum = busRegNum; row.Tinum = tinNum;
        row.LastUpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    // ── Bulk correction import (admin-only, see ReferenceDataBulkUpdate.razor) ──
    // Only overwrites fields actually supplied in a row -- a blank cell in the
    // import leaves that field untouched rather than clearing it. Also
    // auto-resolves any pending conflict for the same key, since a bulk
    // correction is itself the manual review the conflict was waiting on.
    public async Task<BulkReferenceCorrectionResult> ApplyBulkReferenceCorrectionsAsync(List<BulkReferenceCorrectionRow> rows)
    {
        var result = new BulkReferenceCorrectionResult();

        async Task ResolveMatchingConflict(string entityType, string subscriberCode, string customerId, string accNum, string disbDate)
        {
            var pending = await _context.ReferenceDataConflicts.FirstOrDefaultAsync(c =>
                c.ResolvedDate == null && c.EntityType == entityType &&
                c.SubscriberCode == subscriberCode && c.CustomerID == customerId &&
                c.CreditFacilityAccNum == accNum && c.DisbursementDate == disbDate);
            if (pending != null)
            {
                pending.ResolvedDate = DateTime.Now;
                pending.ResolvedBy = "Bulk Import";
            }
        }

        foreach (var row in rows)
        {
            try
            {
                if (string.Equals(row.EntityType, "Individual", StringComparison.OrdinalIgnoreCase))
                {
                    var matches = await _context.IndividualsData.Where(r =>
                        r.SubscriberCode == row.SubscriberCode && r.CustomerID == row.CustomerID &&
                        r.CreditFacilityAccNum == row.CreditFacilityAccNum && r.DisbursementDate == row.DisbursementDate)
                        .ToListAsync();
                    // Same canonical-row rule as everywhere else -- duplicates for
                    // this key resolve to one agreed row, not whichever the
                    // database returns first.
                    var dbRow = matches.Count > 0 ? PickCanonicalIndividualRef(matches) : null;
                    if (dbRow == null) { result.NotFound++; continue; }

                    if (!string.IsNullOrWhiteSpace(row.DateOfBirth)) dbRow.DateOfBirth = row.DateOfBirth;
                    if (!string.IsNullOrWhiteSpace(row.NatIDNum)) dbRow.NatIDNum = row.NatIDNum;
                    if (!string.IsNullOrWhiteSpace(row.VotersIDNum)) dbRow.VotersIDNum = row.VotersIDNum;
                    if (!string.IsNullOrWhiteSpace(row.DriverLicNum)) dbRow.DriverLicNum = row.DriverLicNum;
                    if (!string.IsNullOrWhiteSpace(row.PassportNum)) dbRow.PassportNum = row.PassportNum;
                    if (!string.IsNullOrWhiteSpace(row.SSNum)) dbRow.SSNum = row.SSNum;
                    if (!string.IsNullOrWhiteSpace(row.EzwichNum)) dbRow.EzwichNum = row.EzwichNum;
                    if (!string.IsNullOrWhiteSpace(row.OtherIDNum)) dbRow.OtherIDNum = row.OtherIDNum;
                    dbRow.LastUpdatedDate = DateTime.Now;

                    await ResolveMatchingConflict("Individual", row.SubscriberCode, row.CustomerID, row.CreditFacilityAccNum, row.DisbursementDate);
                    result.Applied++;
                }
                else if (string.Equals(row.EntityType, "Business", StringComparison.OrdinalIgnoreCase))
                {
                    var matches = await _context.BusinessesData.Where(r =>
                        r.SubscriberCode == row.SubscriberCode && r.CustomerID == row.CustomerID &&
                        r.CreditFacilityAccNum == row.CreditFacilityAccNum && r.DisbursementDate == row.DisbursementDate)
                        .ToListAsync();
                    var dbRow = matches.Count > 0 ? PickCanonicalBusinessRef(matches) : null;
                    if (dbRow == null) { result.NotFound++; continue; }

                    // No DateOfBirth here -- Business records have no such column in
                    // the uploaded file (see Registrationdate/Commencementdate instead).
                    if (!string.IsNullOrWhiteSpace(row.Busregnum)) dbRow.Busregnum = row.Busregnum;
                    if (!string.IsNullOrWhiteSpace(row.Tinum)) dbRow.Tinum = row.Tinum;
                    dbRow.LastUpdatedDate = DateTime.Now;

                    await ResolveMatchingConflict("Business", row.SubscriberCode, row.CustomerID, row.CreditFacilityAccNum, row.DisbursementDate);
                    result.Applied++;
                }
                else if (string.Equals(row.EntityType, "IndividualMobile", StringComparison.OrdinalIgnoreCase))
                {
                    var matches = await _context.IndividualsMobileData.Where(r =>
                        r.SubscriberCode == row.SubscriberCode && r.CustomerID == row.CustomerID &&
                        r.CreditFacilityAccNum == row.CreditFacilityAccNum && r.DisbursementDate == row.DisbursementDate)
                        .ToListAsync();
                    var dbRow = matches.Count > 0 ? PickCanonicalIndividualRef(matches) : null;
                    if (dbRow == null) { result.NotFound++; continue; }

                    if (!string.IsNullOrWhiteSpace(row.DateOfBirth)) dbRow.DateOfBirth = row.DateOfBirth;
                    if (!string.IsNullOrWhiteSpace(row.NatIDNum)) dbRow.NatIDNum = row.NatIDNum;
                    if (!string.IsNullOrWhiteSpace(row.VotersIDNum)) dbRow.VotersIDNum = row.VotersIDNum;
                    if (!string.IsNullOrWhiteSpace(row.DriverLicNum)) dbRow.DriverLicNum = row.DriverLicNum;
                    if (!string.IsNullOrWhiteSpace(row.PassportNum)) dbRow.PassportNum = row.PassportNum;
                    if (!string.IsNullOrWhiteSpace(row.SSNum)) dbRow.SSNum = row.SSNum;
                    if (!string.IsNullOrWhiteSpace(row.EzwichNum)) dbRow.EzwichNum = row.EzwichNum;
                    if (!string.IsNullOrWhiteSpace(row.OtherIDNum)) dbRow.OtherIDNum = row.OtherIDNum;
                    dbRow.LastUpdatedDate = DateTime.Now;

                    await ResolveMatchingConflict("IndividualMobile", row.SubscriberCode, row.CustomerID, row.CreditFacilityAccNum, row.DisbursementDate);
                    result.Applied++;
                }
                else
                {
                    result.Errors.Add($"Row for CustomerID '{row.CustomerID}': unrecognized EntityType '{row.EntityType}' (expected Individual, Business, or IndividualMobile)");
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Row for CustomerID '{row.CustomerID}': {ex.Message}");
            }
        }

        await _context.SaveChangesAsync();
        return result;
    }
}
