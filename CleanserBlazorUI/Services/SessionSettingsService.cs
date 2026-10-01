using CleanserBlazorUI.Data;
using CleanserBlazorUI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanserBlazorUI.Services;

/// <summary>
/// The idle-timeout (minutes) that applies to every signed-in user. Singleton:
/// it is read on every authenticated request by the cookie validator, so the
/// value is cached for a minute and refreshed immediately when an admin saves.
/// Each lookup uses its own DI scope, so it never shares a DbContext with the
/// page that triggered it.
/// </summary>
public class SessionSettingsService(IServiceScopeFactory scopeFactory, ILogger<SessionSettingsService> logger)
{
    public const int DefaultMinutes = 30;
    public const int MinMinutes = 5;
    public const int MaxMinutes = 480; // 8 hours

    private static readonly TimeSpan CacheFor = TimeSpan.FromSeconds(60);
    private int _minutes = DefaultMinutes;
    private DateTime _loadedAtUtc = DateTime.MinValue;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<int> GetIdleMinutesAsync()
    {
        if (DateTime.UtcNow - _loadedAtUtc < CacheFor) return _minutes;

        await _gate.WaitAsync();
        try
        {
            if (DateTime.UtcNow - _loadedAtUtc < CacheFor) return _minutes;

            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var row = await db.SessionSettings.AsNoTracking().FirstOrDefaultAsync();
                _minutes = Clamp(row?.IdleTimeoutMinutes ?? DefaultMinutes);
            }
            catch (Exception ex)
            {
                // Never lock everybody out because the lookup failed: keep the last known value.
                logger.LogError(ex, "Could not read session settings; keeping {Minutes} minutes.", _minutes);
            }

            _loadedAtUtc = DateTime.UtcNow;
            return _minutes;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<SessionSettings?> GetRowAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.SessionSettings.AsNoTracking().FirstOrDefaultAsync();
    }

    /// <summary>Returns an error message, or null on success.</summary>
    public async Task<string?> SaveAsync(int minutes, string updatedBy)
    {
        if (minutes < MinMinutes || minutes > MaxMinutes)
        {
            return $"Enter a value between {MinMinutes} and {MaxMinutes} minutes.";
        }

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var row = await db.SessionSettings.FirstOrDefaultAsync();
        if (row is null)
        {
            row = new SessionSettings();
            db.SessionSettings.Add(row);
        }
        row.IdleTimeoutMinutes = minutes;
        row.UpdatedBy = updatedBy;
        row.UpdatedUtc = DateTime.UtcNow;
        await db.SaveChangesAsync();

        _minutes = minutes;
        _loadedAtUtc = DateTime.UtcNow;
        logger.LogInformation("Session idle timeout set to {Minutes} minutes by {User}.", minutes, updatedBy);
        return null;
    }

    private static int Clamp(int minutes) => Math.Clamp(minutes, MinMinutes, MaxMinutes);
}
