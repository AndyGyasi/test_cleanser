# Cleanser App (`clnsr_prjct`)

**AB3 Credit Bureau — Data Cleanser Tool**

A C# / ASP.NET Blazor (.NET 9) application that validates, normalises, and prepares financial and credit records submitted by subscriber institutions before loading into the AB3 credit bureau system.

---

## What It Does

- Accepts individual (IND) and commercial (BUS) subscriber data files
- Applies field-level cleaning rules (dates, phone numbers, ID numbers, account numbers, names, nationality codes, facility terms, etc.)
- Detects and routes duplicate records
- Matches submitted records against a reference database to identify new and updated records
- Outputs a multi-sheet Excel workbook: **CLEAN**, **UNLOADABLE**, and **DUPLICATE** sheets
- Generates an on-demand **Unloadable Log** (header row + Demographic/Financial rejection breakdown) after cleaning an IND file
- Flags cross-record identity conflicts, including same `CustomerID` linked to different GHA (Ghana Card) numbers across any of the 7 ID columns

---

## Recent Changes

See [`CHANGELOG.md`](./CHANGELOG.md) for the full detail. Most recent:

- **2026-07-28** — GHA card cross-record conflict check; on-demand Unloadable Log
  (new `SubscriberProfile` table, requires a migration — see below); clarified
  the manual-rescue registration workflow; fixed overdraft disbursement-date
  false-UNL routing.

---

## Project Structure

```
clnsr_prjct/
├── CleanserBlazorUI/
│   ├── Components/
│   │   └── Pages/
│   │       └── Home.razor          # Main page — orchestrates all cleaning & output
│   ├── Converters/
│   │   ├── IndividualDataTransformer.cs    # Cell-level cleaning — IND records
│   │   ├── IndividualDataTransformerDud.cs # Cleaning — IND unloadable path
│   │   ├── IndividualDataTransformerRef.cs # Cleaning — IND reference path
│   │   ├── BusinessDataTransformer.cs      # Cell-level cleaning — BUS records
│   │   ├── BusinessDataTransformerDud.cs   # Cleaning — BUS unloadable path
│   │   ├── BusinessDataTransformerRef.cs   # Cleaning — BUS reference path
│   │   └── ExcelProcessorService.cs        # Excel read/write helpers
│   ├── Data/
│   │   └── DataManagementService.cs        # SQL Server — reference DB access
│   ├── Services/
│   │   └── UnloadableLogService.cs         # Unloadable log aggregation + xlsx generation
│   ├── Entities/
│   │   ├── CellDataAndStatus.cs            # Core result object per field
│   │   ├── IndividualContext.cs            # Row-level context — IND
│   │   ├── BusinessContext.cs              # Row-level context — BUS
│   │   └── ...
│   ├── Helpers/
│   │   └── StringHelper.cs                 # Shared utility functions (~4800 lines)
│   ├── Repository/
│   │   └── DataCleaningType.cs
│   └── Constants/
└── tracker_rows.txt                        # Legacy issue notes (superseded by issues tracker)
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Blazor Server (.NET 9) |
| UI Components | MudBlazor |
| Excel I/O | ClosedXML / EPPlus |
| Database | SQL Server (EF Core + BulkExtensions) |
| Language | C# 13 |

---

## Branch & Commit Convention

| Branch | Purpose |
|---|---|
| `main` | Stable, tested baseline |
| `sprint/N-short-description` | Active sprint work |

Commit message format:
```
[#ISSUE_ID] Short description of change

- Detail 1
- Detail 2
```

Example:
```
[#7] Fix FacilityTerm round-up in IND and BUS transformers

- Changed (int)(value / 30.44) to (int)Math.Ceiling(value / 30.44)
- Applied to both IndividualDataTransformer and BusinessDataTransformer
```

---

## Issue Tracker

All known issues, severity ratings, priority levels, and resolution status are maintained in the formal issues tracker document (`Cleanser_Issues_Tracker_v2.xlsx`). See the accompanying Word report (`Cleanser_Issues_Report.docx`) for the full executive summary and issue descriptions.

> **Note:** the sprint plan previously listed here (Sprints 1–6, issues #1–#19) is stale as of 2026-07-28 — several of those issues have since shipped (see `CHANGELOG.md`), and the tracker document is the source of truth for current status. This README won't try to keep a duplicate sprint table in sync going forward; check the tracker directly.

---

## Setting Up on a New Machine

### Prerequisites

- **.NET 9 SDK**
- **SQL Server** — local instance or a reachable server (Trusted/Windows auth or SQL auth both work; see connection string below)
- **`dotnet-ef`** global tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
  (skip if already installed — check with `dotnet ef --version`)

### Steps

```bash
# 1. Clone
git clone https://github.com/XAb3d/clnsr_prjct.git
cd clnsr_prjct

# 2. Restore dependencies (restores every project in CleanserProject.sln)
dotnet restore

# 3. Configure your connection string
cp CleanserBlazorUI/appsettings.template.json CleanserBlazorUI/appsettings.json
#    Edit CleanserBlazorUI/appsettings.json -- set Server, Database, and auth
#    to match your SQL Server instance. Trusted_Connection=True (Windows auth)
#    is the default in the template; for SQL auth, replace it with
#    User Id=...;Password=...

# 4. Apply all database migrations
dotnet ef database update --project CleanserBlazorUI/CleanserBlazorUI.csproj

# 5. Run
dotnet run --project CleanserBlazorUI/CleanserBlazorUI.csproj
```

> **`appsettings.json` is excluded from Git** (it holds your connection string). Always start from `appsettings.template.json` on a new machine -- never commit your own `appsettings.json`.

#### What step 4 actually does

`dotnet ef database update` applies **every** migration in `CleanserBlazorUI/Migrations/`, in order, against whatever database your connection string points to. EF Core tracks which migrations have already been applied (in an `__EFMigrationsHistory` table it creates), so this one command works identically whether you're pointing at:
- a **brand-new, empty database** (it creates every table from scratch), or
- an **existing database from an older checkout** (it applies only the migrations that are still missing).

There's only one `DbContext` in the project (`ApplicationDbContext`), and it's the same `DefaultConnection` the running app reads from -- so there's no separate "reference DB" migration step to worry about; one connection string, one `dotnet ef database update`, done.

For awareness, here's what's accumulated in `Migrations/` so far, oldest to newest:

| When | Migration | What it added |
|---|---|---|
| Apr 2025 | `init4`, `clean1`, `bus1` | Initial schema |
| Jul 2026 | `sprint5_ref_id_fields`, `sprint5_ref_id_fields_v2`, `sprint10_ref_name_fields` | Reference-matching ID/name fields |
| Jul 28, 2026 | `AddSubscriberProfile` | `SubscriberProfile` table (Unloadable Log header dedup) |
| Jul 29, 2026 | `AddIndividualRefMiddleNames` | Middle-name field on the individual reference-matching path |
| Jul 30, 2026 | `AddFacilityStatusAndBusinessIdentity` | Facility status field; business identity fields |
| Aug 2, 2026 | `AddBusinessRefTinumAndBusregnumFlag` | TIN/BusRegNum flag on the business reference-matching path |
| Aug 5, 2026 | `AddUnloadableLogTables` | Unloadable Log tables |
| Aug 9, 2026 | `MoveSubscriberShortCodesIntoAppDb` | Moved `SubscriberShortCodes` off the old legacy DB and into this app's own `DefaultConnection` |
| Aug 10, 2026 | `AddLastConfirmedReportingPeriod` | Reporting-period tracking |
| Aug 31, 2026 | `ExpandSubscriberShortCodes` | Expanded `SubscriberShortCodes` with the legacy `Subscriber` schema's remaining columns (contacts, addresses, sector code, etc.) |

You don't need to run these individually or in a particular order beyond what step 4 already does automatically -- this table is just so you know what's in the DB after setup, and why.

#### Verifying it worked

After step 5, upload a small test file through any of the tabs (e.g. "3. INDIVIDUAL RECORDS DUD") and confirm it processes and downloads without a connection error. If it fails immediately with a SQL error, double check the `Server`/`Database` values and that the SQL Server instance is reachable from your machine (firewall, VPN, etc.).

#### Optional: Aspire dashboard / telemetry

`CleanserMonitor.AppHost` is a separate .NET Aspire host for observability (telemetry, health checks) -- it's **not required** to run `CleanserBlazorUI` day-to-day. If you want the dashboard:
```bash
dotnet run --project CleanserMonitor/CleanserMonitor.AppHost
```

---

*Maintained by AB3 Data Operations.*
