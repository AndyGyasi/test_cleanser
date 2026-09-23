using Microsoft.AspNetCore.Components.Forms;

namespace CleanserBlazorUI.Entities;
public class SpreadsheetFileAndChipColor
{
    public IBrowserFile? SpreadsheetFile { get; set; }
    public MudBlazor.Color SpreadsheetFileColor { get; set; }
    public bool? IsReferenceAvailable { get; set; }
    public bool IsPassed { get; set; }
    // Populated by the plain-HTTP upload endpoint (bypasses the SignalR circuit
    // size limit that killed large-file reads). Null means the upload endpoint
    // didn't run for this file, so callers fall back to browserFile.OpenReadStream.
    public string? TempFilePath { get; set; }
}