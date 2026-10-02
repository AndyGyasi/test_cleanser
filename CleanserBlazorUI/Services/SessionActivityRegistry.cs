using System.Collections.Concurrent;
using System.Security.Claims;

namespace CleanserBlazorUI.Services;

/// <summary>
/// Remembers, per signed-in session, which long jobs (cleaning, referencing,
/// registering, bulk updates...) are running and when the last one finished. A
/// session with a running job is never idle; once its last job ends, the idle
/// clock starts from that moment. Held in memory on the server, so it does not
/// depend on the browser page staying awake -- SessionIdle consults it on every
/// request.
/// </summary>
public class SessionActivityRegistry
{
    // A job that "runs" longer than this is assumed stuck and stops holding the session open.
    private static readonly TimeSpan MaxJobAge = TimeSpan.FromHours(6);
    private static readonly TimeSpan KeepFinished = TimeSpan.FromDays(1);

    private sealed class Entry
    {
        public readonly Dictionary<Guid, DateTime> Active = new();
        public DateTime LastEndUtc = DateTime.MinValue;
    }

    private readonly ConcurrentDictionary<string, Entry> _entries = new();

    public Guid JobStarted(string sessionKey)
    {
        var id = Guid.NewGuid();
        var entry = _entries.GetOrAdd(sessionKey, _ => new Entry());
        lock (entry) entry.Active[id] = DateTime.UtcNow;
        return id;
    }

    public void JobEnded(string sessionKey, Guid jobId)
    {
        if (_entries.TryGetValue(sessionKey, out var entry))
        {
            lock (entry)
            {
                entry.Active.Remove(jobId);
                entry.LastEndUtc = DateTime.UtcNow;
            }
        }
        Prune();
    }

    /// <summary>
    /// "Now" while a job is running, otherwise when the last job finished, or null
    /// if this session never ran one.
    /// </summary>
    public DateTime? LastActivityUtc(string sessionKey)
    {
        if (!_entries.TryGetValue(sessionKey, out var entry)) return null;

        lock (entry)
        {
            var now = DateTime.UtcNow;
            if (entry.Active.Values.Any(started => now - started < MaxJobAge)) return now;
            return entry.LastEndUtc == DateTime.MinValue ? null : entry.LastEndUtc;
        }
    }

    private void Prune()
    {
        var cutoff = DateTime.UtcNow - KeepFinished;
        foreach (var (key, entry) in _entries)
        {
            bool stale;
            lock (entry) stale = entry.Active.Count == 0 && entry.LastEndUtc < cutoff;
            if (stale) _entries.TryRemove(key, out _);
        }
    }
}

/// <summary>Identifies one signed-in session: a per-sign-in id stamped into the cookie.</summary>
public static class SessionKey
{
    public const string ClaimType = "cx.sid";

    public static string? For(ClaimsPrincipal? principal) =>
        principal?.FindFirst(ClaimType)?.Value ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
