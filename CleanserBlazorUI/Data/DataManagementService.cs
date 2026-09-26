using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using CleanserBlazorUI.Services;

namespace CleanserBlazorUI.Data;
public class DataManagementService
{
    private string text_value_seperator { get; set; } = "&&&___&&&";
    private readonly ApplicationDbContext _context;
    public DataManagementService(ApplicationDbContext context)
    {
        _context = context;
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

        // .ToDictionary() throws "An item with the same key has already been
        // added" if the reference table has duplicate (AccNum, CustomerID,
        // DisbursementDate) rows -- which can genuinely happen in the DB.
        // Build the index manually instead so a duplicate just overwrites
        // (last one wins) rather than crashing the whole cleaning run.
        var existingIndex = new Dictionary<(string, string, string), IndividualRef>();
        foreach (var r in existing)
        {
            existingIndex[(Norm(r.CreditFacilityAccNum), Norm(r.CustomerID), Norm(r.DisbursementDate))] = r;
        }

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
                if (!string.IsNullOrWhiteSpace(item.DateOfBirth) && dbRow.DateOfBirth != item.DateOfBirth)
                {
                    dbRow.DateOfBirth = item.DateOfBirth;
                    dbRow.LastUpdatedDate = now;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
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
                if (!string.IsNullOrWhiteSpace(item.DateOfBirth) && dbRow.DateOfBirth != item.DateOfBirth)
                {
                    dbRow.DateOfBirth = item.DateOfBirth;
                    dbRow.LastUpdatedDate = now;
                    if (!toUpdate.Contains(dbRow)) toUpdate.Add(dbRow);
                }

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
        var facilityDate = stringHelper.GetFacilityDateFromFileName(fileShortName, true);
        if (facilityDate.IsValid &&
            DateTime.TryParseExact(facilityDate.LastDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        }
        return string.Empty;
    }



    //INSERT REFERENCE BUSINESS DATA
    public async Task Initialize_SaveExcelDataToDatabaseInd(IEnumerable<DBIndividualContext> dataFromExcel, string fileShortName)
    {
        //var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        const int batchSize = 10_000;
        var currentBatch = new List<DBIndividualContext>(batchSize);
        var currentBatchDB = new List<IndividualRef>();

        var _createdDate = DateTime.Now;
        var currentVersion = await GetIndividualMaxversion();
        foreach (var item in dataFromExcel)
        {
            currentBatchDB.Add(new IndividualRef
            {
                CreditFacilityAccNum = item.CreditFacilityAccNum,
                CustomerID = item.CustomerID,
                DisbursementDate = item.DisbursementDate,
                DateOfBirth = item.DateOfBirth,
                SubscriberCode = subscriber,
                CurrenVersion = currentVersion,
                CreatedDate = _createdDate
            });

            if (currentBatchDB.Count >= batchSize)
            {
                await BulkInsertBatchIND(currentBatchDB);
                currentBatch.Clear();
                currentBatchDB.Clear();
            }
        }

        // Insert remaining records
        if (currentBatchDB.Count > 0)
        {
            await BulkInsertBatchIND(currentBatchDB);
        }
    }
    public async Task Initialize_SaveExcelDataToDatabaseBus(IEnumerable<DBBusinessContext> dataFromExcel, string fileShortName)
    {
        //var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        var subscriber = await GetFileShortCodeFromFileName(fileShortName);
        const int batchSize = 10_000;
        var currentBatch = new List<DBBusinessContext>(batchSize);
        var currentBatchDB = new List<BusinessRef>();

        var _createdDate = DateTime.Now;
        var currentVersion = await GetBusinessMaxversion();
        foreach (var item in dataFromExcel)
        {
            currentBatchDB.Add(new BusinessRef
            {
                CreditFacilityAccNum = item.Facilityaccnum,
                CustomerID = item.CustomerID,
                DisbursementDate = item.DisbursementDate,
                DateOfBirth = item.DateOfBirth,
                SubscriberCode = subscriber,
                CurrenVersion = currentVersion,
                CreatedDate = _createdDate
            });

            if (currentBatchDB.Count >= batchSize)
            {
                await BulkInsertBatchBUS(currentBatchDB);
                currentBatch.Clear();
                currentBatchDB.Clear();
            }
        }

        // Insert remaining records
        if (currentBatchDB.Count > 0)
        {
            await BulkInsertBatchBUS(currentBatchDB);
        }
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
        DateTime date = new DateTime();
        if (existingItem == null)
        {
            _item.DateCreated = date.Date;
            _item.DateModified = date.Date;
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
}
