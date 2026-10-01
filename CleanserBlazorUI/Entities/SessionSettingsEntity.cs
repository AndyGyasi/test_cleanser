namespace CleanserBlazorUI.Entities;

/// <summary>
/// Single-row table holding the security setting that applies to every user:
/// how many minutes of inactivity end a session. Admin-editable from
/// /session-settings; read through SessionSettingsService (cached).
/// </summary>
public class SessionSettings
{
    public int Id { get; set; }
    public int IdleTimeoutMinutes { get; set; } = 30;
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
