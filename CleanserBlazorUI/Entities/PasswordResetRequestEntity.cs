namespace CleanserBlazorUI.Entities;

/// <summary>
/// Someone who cannot sign in asked (from the Forgot password page) for an administrator to give
/// them a temporary password. Shown to admins in Manage users; closed when the admin resets the
/// password or dismisses the request. The app sends no email, so this is the "notice to the admin".
/// </summary>
public class PasswordResetRequest
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime RequestedUtc { get; set; }

    // Null while the request is open.
    public DateTime? ResolvedUtc { get; set; }
    public string? ResolvedBy { get; set; }
    public string? Outcome { get; set; }   // "Reset", "Dismissed" or "Account deleted"
}
