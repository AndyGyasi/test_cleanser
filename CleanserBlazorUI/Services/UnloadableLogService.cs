using CleanserBlazorUI.Entities;

namespace CleanserBlazorUI.Services
{
    /// <summary>
    /// Builds the unloadable log workbook after a cleaning run: the section-1
    /// header row, a Table 1 breakdown by exact error message, and a Table 2
    /// rollup of those same messages into Demographic / Financial /
    /// Facility &amp; Submission categories.
    ///
    /// Scope note: generates a single-file workbook for the file just
    /// processed. It does not append to an existing master log workbook --
    /// if you're tracking a running log across files, copy this row in for
    /// now.
    /// </summary>
    public static class UnloadableLogService
    {
        // ── Message → category mapping ────────────────────────────────────────
        // Driven by the UnloadableErrorCatalogEntries table (see the
        // Unloadable Error Catalog page) instead of a hardcoded list, so new
        // error codes/categories can be added without a code deployment.
        // Anything unmatched falls into "Uncategorized" rather than silently
        // disappearing from Table 2.
        private static readonly (string Category, string SubCategory)[] UncategorizedTarget =
        {
            ("Uncategorized", "Uncategorized")
        };

        /// <summary>
        /// Returns the (Category, SubCategory) this message counts toward,
        /// per the first catalog entry whose DescriptionOfErrors pattern
        /// matches. Falls back to "Uncategorized" rather than silently
        /// dropping an unmapped message.
        /// </summary>
        public static (string Category, string SubCategory)[] CategorizeMessage(
            string message, IReadOnlyList<UnloadableErrorCatalogEntry> catalog)
        {
            if (string.IsNullOrWhiteSpace(message)) return UncategorizedTarget;
            if (catalog != null)
            {
                foreach (var entry in catalog)
                {
                    if (MatchesCatalogPattern(message, entry.DescriptionOfErrors))
                    {
                        return new[] { (entry.TopLevelCategory, entry.SubCategory) };
                    }
                }
            }
            return UncategorizedTarget;
        }

        /// <summary>
        /// A catalog pattern may contain literal wording plus {placeholder}
        /// segments standing in for real per-record values (a real ID, a real
        /// date) that the actual message already has substituted in -- so we
        /// never match those segments literally. We strip them out and
        /// require every remaining literal fragment (longer than a couple
        /// characters, so stray punctuation doesn't count) to appear in the
        /// message. A message documenting two distinct variants should be two
        /// separate catalog rows rather than one row joined by "or" -- "or" is
        /// too common a plain-English word in these messages to safely treat
        /// as a phrasing separator.
        /// </summary>
        private static bool MatchesCatalogPattern(string message, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return false;

            var fragments = System.Text.RegularExpressions.Regex
                .Split(pattern, @"\{[^}]*\}")
                .Select(f => f.Trim(' ', '-', ':', ';', '—', '(', ')'))
                .Where(f => f.Length > 2)
                .ToList();

            return fragments.Count > 0 && fragments.All(f => message.Contains(f, StringComparison.OrdinalIgnoreCase));
        }

        public class MessageRejectionSummary
        {
            public string ErrorMessage { get; set; } = string.Empty;
            public int Count { get; set; }
            public double Percentage { get; set; }
            public string Category { get; set; } = string.Empty;
        }

        public class CategoryRejectionSummary
        {
            public string SubCategory { get; set; } = string.Empty;
            public string DescriptionOfErrors { get; set; } = string.Empty;
            public int VolumeAffected { get; set; }
            public double Percentage { get; set; }
        }

        public class UnloadableLogHeader
        {
            public int SerialNo { get; set; }
            public string SubscriberName { get; set; } = string.Empty;
            public string Subcode { get; set; } = string.Empty;
            public string InstitutionType { get; set; } = string.Empty;
            public string Associate { get; set; } = string.Empty;
            public string Filename { get; set; } = string.Empty;
            public int NumberOfRecords { get; set; }
            public string ReportingPeriod { get; set; } = string.Empty;
            public string ReportingYear { get; set; } = string.Empty;
            public string UnloadableReason { get; set; } = string.Empty;
            public string DateEmailed { get; set; } = string.Empty;
            public string DateFixed { get; set; } = string.Empty;
            public string Comments { get; set; } = string.Empty;
            public string DataType { get; set; } = string.Empty;
            public string Months { get; set; } = string.Empty;
            public string LogYear { get; set; } = string.Empty;
        }

        private static List<PropertyInfoAndType> GetCellProps<T>()
        {
            return typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(CellDataAndStatus))
                .Select(p => new PropertyInfoAndType(p))
                .ToList();
        }

        private sealed class PropertyInfoAndType
        {
            public System.Reflection.PropertyInfo Prop { get; }
            public PropertyInfoAndType(System.Reflection.PropertyInfo prop) { Prop = prop; }
        }

        /// <summary>
        /// Table 1: distinct error-message breakdown across the UNL batch,
        /// via reflection over CellDataAndStatus properties -- works for any
        /// T shaped that way (Individual or Business). Counts DISTINCT
        /// RECORDS affected by each message, not raw occurrences -- a message
        /// landing on two different fields of the same record (e.g. a joint
        /// validation) still only counts that record once.
        ///
        /// Duplicates is added as its own row afterward if duplicateCount > 0
        /// -- it isn't discoverable by scanning UNL records at all, since
        /// duplicate records live in a separate list entirely. Its percentage
        /// is measured against totalRecordsInFile, not the UNL count, since
        /// it's not a UNL-population statistic.
        /// </summary>
        public static List<MessageRejectionSummary> SummarizeByMessage<T>(
            List<T> unlRecords, IReadOnlyList<UnloadableErrorCatalogEntry> catalog, int duplicateCount, int totalRecordsInFile)
        {
            var results = new List<MessageRejectionSummary>();
            int unlTotal = unlRecords?.Count ?? 0;

            if (unlRecords != null && unlTotal > 0)
            {
                var props = GetCellProps<T>();
                var messageToRecords = new Dictionary<string, HashSet<object>>();

                foreach (var record in unlRecords)
                {
                    if (record == null) continue;
                    foreach (var p in props)
                    {
                        var cell = p.Prop.GetValue(record) as CellDataAndStatus;
                        if (cell == null || cell.Passed || cell.Errors == null) continue;
                        foreach (var e in cell.Errors)
                        {
                            if (string.IsNullOrWhiteSpace(e)) continue;
                            var trimmed = e.Trim();
                            if (!messageToRecords.TryGetValue(trimmed, out var set))
                            {
                                set = new HashSet<object>();
                                messageToRecords[trimmed] = set;
                            }
                            set.Add(record);
                        }
                    }
                }

                foreach (var kvp in messageToRecords)
                {
                    var targets = CategorizeMessage(kvp.Key, catalog);
                    results.Add(new MessageRejectionSummary
                    {
                        ErrorMessage = kvp.Key,
                        Count = kvp.Value.Count,
                        Percentage = unlTotal > 0 ? (double)kvp.Value.Count / unlTotal : 0,
                        Category = string.Join(" + ", targets.Select(t => t.SubCategory))
                    });
                }
            }

            if (duplicateCount > 0)
            {
                results.Add(new MessageRejectionSummary
                {
                    ErrorMessage = "DUPLICATE RECORD (same account/customer/date submitted more than once)",
                    Count = duplicateCount,
                    Percentage = totalRecordsInFile > 0 ? (double)duplicateCount / totalRecordsInFile : 0,
                    Category = "Duplicates *"
                });
            }

            return results.OrderByDescending(r => r.Count).ToList();
        }

        /// <summary>
        /// Table 2: rolls Table 1's message-level data up into
        /// Demographic / Financial / Facility &amp; Submission sub-category
        /// buckets, for one top-level category at a time. Duplicates never
        /// appears here -- it isn't a field-level defect the way everything
        /// else in this table is, and it already has its place in Table 1.
        /// Facility &amp; Submission's Duplicates line item, when it needs to
        /// show up, is added directly by the caller from the same
        /// duplicateCount used in Table 1, not rediscovered here.
        /// </summary>
        public static List<CategoryRejectionSummary> SummarizeByCategory<T>(
            List<T> unlRecords, IReadOnlyList<UnloadableErrorCatalogEntry> catalog, string topLevelCategory, int duplicateCountForFacilitySubmission = 0)
        {
            var results = new List<CategoryRejectionSummary>();
            int total = unlRecords?.Count ?? 0;

            if (unlRecords != null && total > 0)
            {
                var props = GetCellProps<T>();
                var subcatToRecords = new Dictionary<string, HashSet<object>>();
                var subcatToMessages = new Dictionary<string, HashSet<string>>();

                foreach (var record in unlRecords)
                {
                    if (record == null) continue;
                    foreach (var p in props)
                    {
                        var cell = p.Prop.GetValue(record) as CellDataAndStatus;
                        if (cell == null || cell.Passed || cell.Errors == null) continue;
                        foreach (var e in cell.Errors)
                        {
                            if (string.IsNullOrWhiteSpace(e)) continue;
                            var trimmed = e.Trim();
                            var targets = CategorizeMessage(trimmed, catalog);
                            foreach (var (cat, subcat) in targets)
                            {
                                if (cat != topLevelCategory) continue;
                                if (!subcatToRecords.TryGetValue(subcat, out var recSet))
                                {
                                    recSet = new HashSet<object>();
                                    subcatToRecords[subcat] = recSet;
                                }
                                recSet.Add(record);

                                if (!subcatToMessages.TryGetValue(subcat, out var msgSet))
                                {
                                    msgSet = new HashSet<string>();
                                    subcatToMessages[subcat] = msgSet;
                                }
                                msgSet.Add(trimmed);
                            }
                        }
                    }
                }

                foreach (var subcat in subcatToRecords.Keys)
                {
                    results.Add(new CategoryRejectionSummary
                    {
                        SubCategory = subcat,
                        DescriptionOfErrors = string.Join("; ", subcatToMessages[subcat].OrderBy(m => m)),
                        VolumeAffected = subcatToRecords[subcat].Count,
                        Percentage = total > 0 ? (double)subcatToRecords[subcat].Count / total : 0
                    });
                }
            }

            if (topLevelCategory == "FacilitySubmission" && duplicateCountForFacilitySubmission > 0)
            {
                results.Add(new CategoryRejectionSummary
                {
                    SubCategory = "Duplicates",
                    DescriptionOfErrors = "DUPLICATE RECORD (same account/customer/date submitted more than once)",
                    VolumeAffected = duplicateCountForFacilitySubmission,
                    Percentage = 0 // see Table 1 for this figure -- percentage-of-what differs from every other row here
                });
            }

            return results.OrderByDescending(r => r.VolumeAffected).ToList();
        }

        /// <summary>
        /// One consolidated "Unloadable Reason" string for the header row --
        /// distinct error messages across the whole batch, alphabetized.
        /// </summary>
        public static string BuildUnloadableReasonSummary<T>(List<T> unlRecords)
        {
            if (unlRecords == null || unlRecords.Count == 0) return string.Empty;

            var props = GetCellProps<T>();
            var reasons = new HashSet<string>();
            foreach (var record in unlRecords)
            {
                if (record == null) continue;
                foreach (var p in props)
                {
                    var cell = p.Prop.GetValue(record) as CellDataAndStatus;
                    if (cell != null && !cell.Passed && cell.Errors != null)
                    {
                        foreach (var e in cell.Errors)
                        {
                            if (!string.IsNullOrWhiteSpace(e)) reasons.Add(e.Trim());
                        }
                    }
                }
            }
            return string.Join("; ", reasons.OrderBy(r => r));
        }

        public static byte[] GenerateWorkbook<T>(
            List<T> unlRecords, IReadOnlyList<UnloadableErrorCatalogEntry> catalog, UnloadableLogHeader header, int duplicateCount = 0, int totalRecordsInFile = 0)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Unloadable Log");

            int row = 1;

            // ── Section 1: header row ──────────────────────────────────────
            string[] headerCols =
            {
                "Serial No", "Subscriber Name", "Subcode", "Institution type", "Associate",
                "Filename", "Number Of Records", "Reporting Period", "ReportingYear",
                "Unloadable Reason", "Date Emailed", "Date Fixed", "Comments", "DataType",
                "Months", "LogYear"
            };
            for (int c = 0; c < headerCols.Length; c++)
            {
                var cell = ws.Cell(row, c + 1);
                cell.Value = headerCols[c];
                cell.Style.Font.Bold = true;
            }
            row++;

            ws.Cell(row, 1).Value = header.SerialNo;
            ws.Cell(row, 2).Value = header.SubscriberName;
            ws.Cell(row, 3).Value = header.Subcode;
            ws.Cell(row, 4).Value = header.InstitutionType;
            ws.Cell(row, 5).Value = header.Associate;
            ws.Cell(row, 6).Value = header.Filename;
            ws.Cell(row, 7).Value = header.NumberOfRecords;
            ws.Cell(row, 8).Value = header.ReportingPeriod;
            ws.Cell(row, 9).Value = header.ReportingYear;
            ws.Cell(row, 10).Value = header.UnloadableReason;
            ws.Cell(row, 11).Value = header.DateEmailed;
            ws.Cell(row, 12).Value = header.DateFixed;
            ws.Cell(row, 13).Value = header.Comments;
            ws.Cell(row, 14).Value = header.DataType;
            ws.Cell(row, 15).Value = header.Months;
            ws.Cell(row, 16).Value = header.LogYear;
            row += 2;

            // ── Table 1: by exact error message ──────────────────────────────
            row = WriteMessageSection(ws, row, "2. Error Message Breakdown",
                SummarizeByMessage(unlRecords, catalog, duplicateCount, totalRecordsInFile));
            row += 1;

            // ── Table 2: rolled up into categories ───────────────────────────
            row = WriteCategorySection(ws, row, "3. Demographic Information", "3.1 Rejections",
                SummarizeByCategory(unlRecords, catalog, "Demographic"));
            row += 1;
            row = WriteCategorySection(ws, row, "4. Financial Information", "4.1 Rejections",
                SummarizeByCategory(unlRecords, catalog, "Financial"));
            row += 1;
            row = WriteCategorySection(ws, row, "5. Facility & Submission Information", "5.1 Rejections",
                SummarizeByCategory(unlRecords, catalog, "FacilitySubmission", duplicateCount));

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        /// <summary>
        /// Combined export for the Unloadable Log Report page -- unlike
        /// GenerateWorkbook (one run per file), this covers an arbitrary,
        /// already-filtered set of saved runs across many files/providers/
        /// periods. Three sheets: one row per run, then the error-message and
        /// category breakdowns flattened across all of them, each row tagged
        /// with which run it came from (Filename/Data Provider/Period) since
        /// there's no longer a single header these numbers belong to.
        /// </summary>
        public static byte[] GenerateCombinedWorkbook(List<UnloadableLogReportRow> rows)
        {
            using var workbook = new XLWorkbook();

            var runsWs = workbook.Worksheets.Add("Runs");
            string[] runCols =
            {
                "Filename", "Data Provider", "SubCode", "SubXDSCode", "Category", "Associate",
                "Reporting Period", "Reporting Year", "DataType", "Number Of Records",
                "Date Emailed", "Date Fixed", "Comments", "Created Date"
            };
            WriteHeaderRow(runsWs, runCols);
            int runRow = 2;
            foreach (var r in rows)
            {
                runsWs.Cell(runRow, 1).Value = r.Filename;
                runsWs.Cell(runRow, 2).Value = r.DataProvider;
                runsWs.Cell(runRow, 3).Value = r.SubCode;
                runsWs.Cell(runRow, 4).Value = r.SubXDSCode;
                runsWs.Cell(runRow, 5).Value = r.SubCategoryDescription;
                runsWs.Cell(runRow, 6).Value = r.Associate;
                runsWs.Cell(runRow, 7).Value = r.ReportingPeriod;
                runsWs.Cell(runRow, 8).Value = r.ReportingYear;
                runsWs.Cell(runRow, 9).Value = r.DataType;
                runsWs.Cell(runRow, 10).Value = r.NumberOfRecords;
                runsWs.Cell(runRow, 11).Value = r.DateEmailed;
                runsWs.Cell(runRow, 12).Value = r.DateFixed;
                runsWs.Cell(runRow, 13).Value = r.Comments;
                runsWs.Cell(runRow, 14).Value = r.CreatedDate;
                runRow++;
            }
            runsWs.Columns().AdjustToContents();

            var messagesWs = workbook.Worksheets.Add("Error Message Breakdown");
            string[] msgCols = { "Filename", "Data Provider", "Reporting Period", "Error Message", "Count", "Percentage", "Category" };
            WriteHeaderRow(messagesWs, msgCols);
            int msgRow = 2;
            foreach (var r in rows)
            {
                foreach (var m in r.MessageDetails)
                {
                    messagesWs.Cell(msgRow, 1).Value = r.Filename;
                    messagesWs.Cell(msgRow, 2).Value = r.DataProvider;
                    messagesWs.Cell(msgRow, 3).Value = r.ReportingPeriod;
                    messagesWs.Cell(msgRow, 4).Value = m.ErrorMessage;
                    messagesWs.Cell(msgRow, 5).Value = m.Count;
                    var pctCell = messagesWs.Cell(msgRow, 6);
                    pctCell.Value = m.Percentage;
                    pctCell.Style.NumberFormat.Format = "0.0%";
                    messagesWs.Cell(msgRow, 7).Value = m.Category;
                    msgRow++;
                }
            }
            messagesWs.Columns().AdjustToContents();

            var categoriesWs = workbook.Worksheets.Add("Category Breakdown");
            string[] catCols = { "Filename", "Data Provider", "Reporting Period", "Top Level Category", "Sub Category", "Description Of Errors", "Volume Affected", "Percentage" };
            WriteHeaderRow(categoriesWs, catCols);
            int catRow = 2;
            foreach (var r in rows)
            {
                foreach (var c in r.CategoryDetails)
                {
                    categoriesWs.Cell(catRow, 1).Value = r.Filename;
                    categoriesWs.Cell(catRow, 2).Value = r.DataProvider;
                    categoriesWs.Cell(catRow, 3).Value = r.ReportingPeriod;
                    categoriesWs.Cell(catRow, 4).Value = c.TopLevelCategory;
                    categoriesWs.Cell(catRow, 5).Value = c.SubCategory;
                    categoriesWs.Cell(catRow, 6).Value = c.DescriptionOfErrors;
                    categoriesWs.Cell(catRow, 7).Value = c.VolumeAffected;
                    var catPctCell = categoriesWs.Cell(catRow, 8);
                    catPctCell.Value = c.Percentage;
                    catPctCell.Style.NumberFormat.Format = "0.0%";
                    catRow++;
                }
            }
            categoriesWs.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void WriteHeaderRow(IXLWorksheet ws, string[] cols)
        {
            for (int c = 0; c < cols.Length; c++)
            {
                var cell = ws.Cell(1, c + 1);
                cell.Value = cols[c];
                cell.Style.Font.Bold = true;
            }
        }

        private static int WriteMessageSection(IXLWorksheet ws, int row, string sectionTitle,
            List<MessageRejectionSummary> items)
        {
            ws.Cell(row, 1).Value = sectionTitle;
            ws.Cell(row, 1).Style.Font.Bold = true;
            row++;

            string[] cols = { "Error Message", "Count", "Percentage", "Category" };
            for (int c = 0; c < cols.Length; c++)
            {
                var cell = ws.Cell(row, c + 1);
                cell.Value = cols[c];
                cell.Style.Font.Bold = true;
            }
            row++;

            foreach (var item in items)
            {
                ws.Cell(row, 1).Value = item.ErrorMessage;
                ws.Cell(row, 2).Value = item.Count;

                var pctCell = ws.Cell(row, 3);
                pctCell.Value = item.Percentage;
                pctCell.Style.NumberFormat.Format = "0.0%";

                ws.Cell(row, 4).Value = item.Category;
                row++;
            }

            if (items.Count == 0)
            {
                ws.Cell(row, 1).Value = "(no unloadable records in this run)";
                row++;
            }
            else if (items.Any(i => i.Category == "Duplicates *"))
            {
                ws.Cell(row, 1).Value = "* Duplicates' percentage is of total records in the file — every other row's percentage is of UNL records only.";
                ws.Cell(row, 1).Style.Font.Italic = true;
                row++;
            }

            return row;
        }

        private static int WriteCategorySection(IXLWorksheet ws, int row, string sectionTitle,
            string subTitle, List<CategoryRejectionSummary> items)
        {
            ws.Cell(row, 1).Value = sectionTitle;
            ws.Cell(row, 1).Style.Font.Bold = true;
            row++;

            ws.Cell(row, 1).Value = subTitle;
            row++;

            string[] cols = { "Category", "Description of Errors", "Volume of Records Affected", "Percentage" };
            for (int c = 0; c < cols.Length; c++)
            {
                var cell = ws.Cell(row, c + 1);
                cell.Value = cols[c];
                cell.Style.Font.Bold = true;
            }
            row++;

            foreach (var item in items)
            {
                ws.Cell(row, 1).Value = item.SubCategory;
                ws.Cell(row, 2).Value = item.DescriptionOfErrors;
                ws.Cell(row, 3).Value = item.VolumeAffected;

                var pctCell = ws.Cell(row, 4);
                if (item.SubCategory == "Duplicates")
                {
                    ws.Cell(row, 4).Value = "see Table 1";
                }
                else
                {
                    pctCell.Value = item.Percentage;
                    pctCell.Style.NumberFormat.Format = "0.0%";
                }

                row++;
            }

            if (items.Count == 0)
            {
                ws.Cell(row, 1).Value = "(no rejections in this section)";
                row++;
            }

            return row;
        }
    }
}
