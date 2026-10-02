using Microsoft.AspNetCore.Components.Authorization;

namespace CleanserBlazorUI.Services;

/// <summary>
/// Marks a long-running job so the signed-in session is not timed out while it runs.
/// Use at the top of the method that does the work:
///
///     await using var job = await JobTracker.BeginAsync();
///
/// The session is released when the method ends, however it ends (finished, failed
/// or cancelled), and the idle clock restarts from that moment.
/// </summary>
public sealed class JobTracker(AuthenticationStateProvider authenticationState, SessionActivityRegistry registry)
{
    public async Task<IAsyncDisposable> BeginAsync()
    {
        var state = await authenticationState.GetAuthenticationStateAsync();
        var key = SessionKey.For(state.User);
        return key is null ? NoJob.Instance : new Job(registry, key, registry.JobStarted(key));
    }

    private sealed class Job(SessionActivityRegistry registry, string key, Guid id) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            registry.JobEnded(key, id);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class NoJob : IAsyncDisposable
    {
        public static readonly NoJob Instance = new();
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
