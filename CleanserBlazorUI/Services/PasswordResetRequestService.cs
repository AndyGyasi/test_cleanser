using System.Collections.Concurrent;
using CleanserBlazorUI.Data;
using CleanserBlazorUI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanserBlazorUI.Services;

/// <summary>
/// Password-reset requests from the Forgot password page. The page always answers the same way
/// whether or not the email belongs to an account, so nothing here may be observable by the requester.
/// </summary>
public class PasswordResetRequestService(ApplicationDbContext db, UserManager<ApplicationUser> users, ILogger<PasswordResetRequestService> logger)
{
    private const string ProtectedAccount = "noreply.XDSmonitor@XDSdatagh.com";

    // A computer may file at most this many requests per hour, so the page can't be used to flood admins.
    private const int MaxPerHourPerAddress = 10;
    private static readonly ConcurrentDictionary<string, List<DateTime>> Recent = new();

    /// <summary>Records a request if the email belongs to an account. Silent otherwise.</summary>
    public async Task RecordAsync(string email, string? fromAddress)
    {
        if (!WithinLimit(fromAddress ?? "unknown")) return;

        var user = await users.FindByEmailAsync(email);
        if (user is null || string.Equals(user.Email, ProtectedAccount, StringComparison.OrdinalIgnoreCase)) return;

        var open = await db.PasswordResetRequests.FirstOrDefaultAsync(r => r.UserId == user.Id && r.ResolvedUtc == null);
        if (open is null)
        {
            db.PasswordResetRequests.Add(new PasswordResetRequest { UserId = user.Id, Email = user.Email ?? email, RequestedUtc = DateTime.UtcNow });
        }
        else
        {
            open.RequestedUtc = DateTime.UtcNow;   // one open request per account: just bump it
        }
        await db.SaveChangesAsync();
        logger.LogInformation("Password reset requested for '{Email}'.", user.Email);
    }

    public Task<List<PasswordResetRequest>> OpenAsync() =>
        db.PasswordResetRequests.AsNoTracking().Where(r => r.ResolvedUtc == null).OrderByDescending(r => r.RequestedUtc).ToListAsync();

    public Task<int> CountOpenAsync() => db.PasswordResetRequests.CountAsync(r => r.ResolvedUtc == null);

    public async Task ResolveAsync(string userId, string resolvedBy, string outcome)
    {
        var open = await db.PasswordResetRequests.Where(r => r.UserId == userId && r.ResolvedUtc == null).ToListAsync();
        if (open.Count == 0) return;
        foreach (var r in open)
        {
            r.ResolvedUtc = DateTime.UtcNow;
            r.ResolvedBy = resolvedBy;
            r.Outcome = outcome;
        }
        await db.SaveChangesAsync();
    }

    private static bool WithinLimit(string address)
    {
        var list = Recent.GetOrAdd(address, _ => new List<DateTime>());
        lock (list)
        {
            var cutoff = DateTime.UtcNow.AddHours(-1);
            list.RemoveAll(t => t < cutoff);
            if (list.Count >= MaxPerHourPerAddress) return false;
            list.Add(DateTime.UtcNow);
            return true;
        }
    }
}
