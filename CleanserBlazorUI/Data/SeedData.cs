using CleanserBlazorUI.Data;
using CleanserBlazorUI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "admin", "manager", "superuser", "user", "MustChangePassword", "notallowed", "registrar" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create admin user
        var adminEmail = "noreply.XDSmonitor@XDSdatagh.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "AdminPassword123!");
            await userManager.AddToRoleAsync(adminUser, "admin");
        }

        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedUnloadableErrorCatalogAsync(dbContext);
        await SeedDataLoggingGateMessagesAsync(dbContext);
        await SeedDataLoggingAccessRequestReasonsAsync(dbContext);
        await SeedDataLoggingCleaningPurposeReasonsAsync(dbContext);
        await SeedNavigationAsync(dbContext);
    }

    // Initial sidebar layout -- every existing page assigned to the section
    // it belongs to. Purely a starting point: admin can rename sections and
    // drag items between them afterward (NavigationSettings.razor), and this
    // never re-runs once a NavSection exists.
    private static async Task SeedNavigationAsync(ApplicationDbContext context)
    {
        if (await context.NavSections.AnyAsync()) return;

        var accounts = new NavSection { Title = "Accounts", DisplayOrder = 1 };
        var dataCleaning = new NavSection { Title = "Data Cleaning", DisplayOrder = 2 };
        var unloadables = new NavSection { Title = "Unloadables", DisplayOrder = 3 };
        var referenceRegister = new NavSection { Title = "Reference Register", DisplayOrder = 4 };
        var dataLogging = new NavSection { Title = "Data Logging", DisplayOrder = 5 };

        context.NavSections.AddRange(accounts, dataCleaning, unloadables, referenceRegister, dataLogging);
        await context.SaveChangesAsync(); // need the sections' real Ids before attaching items

        (NavSection Section, string Label, string Href, string IconKey, string? Roles)[] items =
        {
            (accounts, "My Account", "Account/Manage", "Person", null),
            (accounts, "Register User", "Account/Register", "PersonAdd", "admin,superuser"),
            (accounts, "Manage Users", "Manageroles", "Group", "admin"),

            (dataCleaning, "Data Cleaning", "", "CleaningServices", null),
            (dataCleaning, "Add Business Names", "Businessnames", "Business", "admin"),
            (dataCleaning, "Business Name Mapping", "Business-names-normalizer", "Rule", "admin"),
            (dataCleaning, "Schedule Automatic Cleanup", "schedule-automatic-cleanup", "Schedule", "admin"),

            (unloadables, "Unloadable Log Report", "unloadable-log-report", "ReportProblem", null),
            (unloadables, "Unloadable Log History", "unloadable-log-history", "History", "admin"),
            (unloadables, "Unloadable Error Catalog", "unloadable-error-catalog", "MenuBook", "admin"),

            (referenceRegister, "Reference Register", "refdata", "Badge", "admin,registrar"),
            (referenceRegister, "Reference Data Conflicts", "reference-data-conflicts", "Rule", "admin"),
            (referenceRegister, "Bulk Update Reference Data", "reference-data-bulk-update", "UploadFile", "admin"),

            (dataLogging, "My Access Requests", "my-access-requests", "FactCheck", null),
            (dataLogging, "Access Requests", "data-logging-access-requests", "FactCheck", "admin"),
            (dataLogging, "Gate Messages", "data-logging-gate-messages", "Message", "admin"),
            (dataLogging, "Files Not Yet Logged", "data-logging-unlogged-attempts", "ErrorOutline", "admin"),
            (dataLogging, "Cleaning Purpose Reasons", "data-logging-cleaning-purposes", "ListAlt", "admin"),
        };

        var orderBySection = new Dictionary<int, int>();
        foreach (var (section, label, href, iconKey, roles) in items)
        {
            orderBySection.TryGetValue(section.Id, out var order);
            order++;
            orderBySection[section.Id] = order;

            context.NavItems.Add(new NavItem
            {
                NavSectionId = section.Id,
                Label = label,
                Href = href,
                IconKey = iconKey,
                RequiredRoles = roles,
                DisplayOrder = order
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedDataLoggingCleaningPurposeReasonsAsync(ApplicationDbContext context)
    {
        if (await context.DataLoggingCleaningPurposeReasons.AnyAsync()) return;

        context.DataLoggingCleaningPurposeReasons.AddRange(
            new DataLoggingCleaningPurposeReason { Reason = "Reclean" },
            new DataLoggingCleaningPurposeReason { Reason = "CorrectedRecords" });
        await context.SaveChangesAsync();
    }

    private static async Task SeedDataLoggingGateMessagesAsync(ApplicationDbContext context)
    {
        if (await context.DataLoggingGateMessages.AnyAsync()) return;

        context.DataLoggingGateMessages.Add(new DataLoggingGateMessages());
        await context.SaveChangesAsync();
    }

    private static async Task SeedDataLoggingAccessRequestReasonsAsync(ApplicationDbContext context)
    {
        if (await context.DataLoggingAccessRequestReasons.AnyAsync()) return;

        context.DataLoggingAccessRequestReasons.AddRange(
            new DataLoggingAccessRequestReason { Reason = "Assigned staff is on leave" },
            new DataLoggingAccessRequestReason { Reason = "Supervisor/Manager's instruction" });
        await context.SaveChangesAsync();
    }

    // Initial load of the error-classification catalog (see
    // UnloadableLogService.CategorizeMessage). Only runs once -- after this,
    // the catalog is expected to be maintained through the Unloadable Error
    // Catalog page, not by re-running this seed.
    private static async Task SeedUnloadableErrorCatalogAsync(ApplicationDbContext context)
    {
        if (await context.UnloadableErrorCatalogEntries.AnyAsync()) return;

        (string TopLevelCategory, string SubCategory, string DescriptionOfErrors)[] rows =
        {
            ("Demographic", "Business Registration Number / TIN", "BUSINESSNAME: INVALID CHARACTERS"),
            ("Demographic", "Business Registration Number / TIN", "BUSREGNUM APPEARS TO BE A PLACEHOLDER OR CATEGORY LABEL, NOT AN ACTUAL REGISTRATION NUMBER — PROVIDE A VALID REGISTRATION NUMBER (TINUM ALSO NOT A USABLE ALTERNATE ID)"),
            ("Demographic", "Business Registration Number / TIN", "Busregnum or Tinum: cannot be a Ghana card or contains 6 or more consecutive repeating characters"),
            ("Demographic", "Business Registration Number / TIN", "BUSREGNUM: ALL-NUMERIC REGISTRATION NUMBER — PROVIDE A VALID REGISTRATION NUMBER (TINUM ALSO NOT A USABLE ALTERNATE ID)"),
            ("Financial", "Business Registration Number / TIN", "BUSREGNUM: CONTAIN INVALID CHARACTERS"),
            ("Financial", "Current Balance / Arrears", "AMOUNT IN ARREARS AND CURRENT BALANCE SHOULD NOT BE ZERO"),
            ("Financial", "Current Balance / Arrears", "AMOUNT IN ARREARS SHOULD NOT BE ZERO OR LESS THAN CURRENT BALANCE; CURRENT BALANCE SHOULD NOT BE ZERO"),
            ("Financial", "Current Balance / Arrears", "Amount: Not VALID"),
            ("Financial", "Current Balance / Arrears", "CURRENT BALANCE SHOULD NOT BE 0 OR EMPTY FOR ACTIVE LOANS"),
            ("Financial", "Current Balance / Arrears", "CURRENT BALANCE SHOULD NOT BE EMPTY"),
            ("Financial", "Current Balance / Arrears", "CURRENT BALANCE SHOULD NOT BE EMPTY FOR ACTIVE LOANS"),
            ("Financial", "Current Balance / Arrears", "CURRENT BALANCE SHOULD NOT BE ZERO; AMOUNT IN ARREARS, WRITTEN OF AMOUNT AND NDIA'S SHOULD BE ZERO"),
            ("Financial", "Current Balance / Arrears", "CURRENTBALANCE SHOULD NOT BE EMPTY"),
            ("Financial", "Current Balance / Arrears", "CURRENTBALANCE,AMTINARREARS,WRITTENOFFAMOUNT AND NDIA SHOULD BE 0 FOR CLOSED LOANS"),
            ("Financial", "Current Balance / Arrears", "CURRENTBALANCE,AMTINARREARS,WRITTENOFFAMOUNT AND NDIA SHOULD BE 0 FOR EARLY SETTLED LOANS"),
            ("Financial", "Current Balance / Arrears", "CURRENTBALANCE,AMTINARREARS,WRITTENOFFAMOUNT AND NDIA SHOULD BE 0 FOR PAID UP LOANS"),
            ("Demographic", "Customer ID", "CUSTOMER ID CONFLICT: SAME CUSTOMER ID IS LINKED TO DIFFERENT DATES OF BIRTH"),
            ("Demographic", "Customer ID", "CUSTOMER ID CONFLICT: SAME CUSTOMER ID IS LINKED TO DIFFERENT GHANA CARD (GHA) NUMBERS"),
            // Split from one source row that joined two message variants with " or " --
            // kept as two rows since "or" is too common a word elsewhere to treat as a separator (see MatchesCatalogPattern).
            ("Demographic", "Customer ID", "CUSTOMER ID CONFLICT{customer ID}: NAME PARTIALLY MATCHES — POSSIBLE MISSPELLING — MANUAL REVIEW REQUIRED"),
            ("Demographic", "Customer ID", "CUSTOMER ID CONFLICT{customer ID}: SAME CUSTOMER ID IS LINKED TO DIFFERENT NAMES ACROSS RECORDS{details} — VERIFY WHICH IS CORRECT"),
            ("Financial", "Customer ID", "CUSTOMERID: CONTAIN INVALID CHARACTERS"),
            ("Financial", "Customer ID", "CUSTOMERID: CUSTOMERID CANNOT BE THE SAME AS FACILITY ACCOUNT NUMBER"),
            ("Financial", "Customer ID", "CUSTOMERID: INVALID CUSTOMER ID - EXPONENTIATED VALUE DETECTED"),
            ("Financial", "Customer ID", "CUSTOMERID: NOT VALID"),
            ("Financial", "Customer ID", "CUSTOMERID: NOT VALID OR EMPTY"),
            ("Demographic", "Customer Name", "CROSS-RECORD CONFLICT: DUPLICATE RECORD HAS A DIFFERENT NAME — REVIEW REQUIRED"),
            ("Demographic", "Customer Name", "Invalid Business Name"),
            ("Demographic", "Customer Name", "INVALID NAME: BUSINESS KEYWORDS DETECTED IN INDIVIDUAL RECORD — USE COMMERCIAL FILE TYPE"),
            ("Demographic", "Customer Name", "INVALID NAME: MULTIPLE INDIVIDUALS DETECTED — A FACILITY RECORD MUST BE ASSIGNED TO A SINGLE PERSON"),
            ("Demographic", "Customer Name", "INVALID SURNAME AND FIRSTNAME"),
            ("Demographic", "Customer Name", "NAMES CANNOT CONTAIN SEPECIAL CHARACTER(S)"),
            ("Demographic", "Customer Name", "SURNAME AND FIRSTNAME ARE MANDATORY AND SHOULD CONTAIN VALID NAMES"),
            ("Demographic", "Customer Name", "Tradingname: INVALID CHARACTERS"),
            // Trimmed of the "(LAST CONFIRMED: {period})" tail -- that clause is only
            // appended by the real code when a previous reporting period is on file, so
            // requiring it in the match would miss messages generated without it.
            ("Demographic", "Date of Birth", "DATE OF BIRTH DOES NOT MATCH PREVIOUS SUBMISSION — WAS {previous DOB}"),
            ("Demographic", "Date of Birth", "DATEOFBIRTH: INVALID DATE"),
            ("Demographic", "Date of Birth", "DUPLICATE WITH DIFFERENT DATE OF BIRTH"),
            ("Demographic", "Date of Birth", "DUPLICATE WITH SAME OR DIFFERENT DATE OF BIRTH"),
            ("Demographic", "Date of Birth", "Empty or Invalid Busregnum or Tinum"),
            ("Financial", "Disbursement Date", "DISBURSEMENT DATE CANNOT BE GREATER THAN MATURITY DATE"),
            ("Financial", "Disbursement Date", "DISBURSEMENT DATE CANNOT BE GREATER THAN SUBMISSION DATE"),
            ("Financial", "Disbursement Date", "DISBURSEMENT DATE YEAR ({year}) IS IMPLAUSIBLY EARLY — CHECK FOR A DATA ENTRY ERROR"),
            ("Financial", "Disbursement Date", "DISBURSEMENTDATE: INVALID DATE"),
            ("Financial", "Disbursement Date", "DUPLICATE WITH DIFFERENT DISBURSEMENT DATE"),
            ("Demographic", "Disbursement Date", "DUPLICATE WITH SAME OR DIFFERENT DATE OF DISBURSEMENTDATE"),
            ("Demographic", "Disbursement Date", "TINUM APPEARS TO BE A PLACEHOLDER OR CATEGORY LABEL, NOT AN ACTUAL TIN — PROVIDE A VALID TIN (BUSREGNUM ALSO NOT A USABLE ALTERNATE ID)"),
            ("Financial", "Facility Account Number", "{account field}: FACILITY ACCOUNT NUMBER CANNOT BE THE SAME AS CUSTOMERID"),
            ("Financial", "Facility Account Number", "{account field}: INVALID ACCOUNT NUMBER - EXPONENTIATED VALUE DETECTED"),
            ("Financial", "Facility Account Number", "{account field}: MISSING FACILITY ACCOUNT NUMBER"),
            ("Financial", "Facility Account Number", "CREDITFACILITYACCNUM: CONTAIN INVALID CHARACTERS"),
            ("Financial", "Facility Account Number", "CREDITFACILITYACCNUM: NOT VALID"),
            ("Financial", "Facility Account Number", "FACILITYACCNUM: CONTAIN INVALID CHARACTERS"),
            ("Financial", "Facility Account Number", "Facilityaccnum: NOT VALID"),
            ("Demographic", "Business Registration Number / TIN", "TINUM: ALL-NUMERIC TIN — PROVIDE A VALID TIN (BUSREGNUM ALSO NOT A USABLE ALTERNATE ID)"),
            ("Financial", "Facility Status", "Invalid FacilityStatusCode"),
            ("Demographic", "ID Number", "CROSS-RECORD CONFLICT: {ID field} IS SHARED ACROSS MULTIPLE CUSTOMER IDS WITH SAME DOB BUT SIMILAR NAME(S) — POSSIBLE MISSPELLING"),
            ("Demographic", "ID Number", "ID APPEARS TO BE A CATEGORY LABEL (e.g. STAFF, GHANACARD, STUDENT), NOT AN ACTUAL ID NUMBER — PROVIDE A VALID ID (NO OTHER COLUMN HAS A USABLE ID EITHER)"),
            ("Demographic", "ID Number", "ID APPEARS TO BE A SEQUENTIAL PLACEHOLDER VALUE (e.g. 123456789) — PROVIDE A VALID ID"),
            ("Demographic", "ID Number", "INVALID NATIONAL ID FORMAT — EXPECTED 3-LETTER COUNTRY CODE (GHA/NGA/CIV) FOLLOWED BY 10 ALPHANUMERIC CHARACTERS"),
            ("Demographic", "ID Number", "NATIONAL ID CONFLICT: {ID field} — NAMES PARTIALLY MATCH, POSSIBLE MISSPELLING — MANUAL REVIEW REQUIRED"),
            ("Demographic", "ID Number", "NATIONAL ID LINKED TO DIFFERENT DATES OF BIRTH — SAME {ID field} CANNOT BELONG TO TWO DIFFERENT PEOPLE"),
            ("Financial", "Business Registration Number / TIN", "TINUM: CONTAIN INVALID CHARACTERS"),
            ("Demographic", "ID Number", "WRONG OR EMPTY IDS. PROVIDE VALID IDS"),
            ("Financial", "Loan/Disbursement Amount", "FACILITYAMOUNT, DISBURSEMENTAMT CANNOT BE EMPTY OR ZERO"),
            ("Financial", "Maturity Date", "MATURITY DATE YEAR ({year}) IS IMPLAUSIBLY EARLY — CHECK FOR A DATA ENTRY ERROR"),
            ("Financial", "Maturity Date", "MATURITYDATE: INVALID DATE"),
            ("Financial", "Written Off Facility", "WRITTENOFFAMOUNT AND AMTINARREARS SHOULD BE THE SAME"),
            ("Financial", "Written Off Facility", "WRITTENOFFAMOUNT AND AMTINARREARS SHOULD BE THE SAME FOR WRITTENOFFAMOUNT"),
        };

        var now = DateTime.Now;
        context.UnloadableErrorCatalogEntries.AddRange(rows.Select(r => new UnloadableErrorCatalogEntry
        {
            TopLevelCategory = r.TopLevelCategory,
            SubCategory = r.SubCategory,
            DescriptionOfErrors = r.DescriptionOfErrors,
            LastUpdatedDate = now
        }));
        await context.SaveChangesAsync();
    }
}