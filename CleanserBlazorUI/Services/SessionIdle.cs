using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace CleanserBlazorUI.Services;

/// <summary>
/// Enforces the admin-configured idle timeout on the sign-in cookie. Every
/// authenticated request stamps "last activity" into the cookie (re-issued at
/// most once a minute); a request that arrives after the idle window has
/// passed is rejected and the user is signed out. A session with a running job
/// (SessionActivityRegistry) counts as active until the job ends. The cookie's
/// own expiry is only a long backstop -- this check is what ends sessions, and
/// it reads the current setting on every request, so a change by an admin
/// applies at once.
/// </summary>
public static class SessionIdle
{
    private const string LastActivityKey = "cx.last";
    private static readonly TimeSpan RestampEvery = TimeSpan.FromMinutes(1);

    /// <summary>Start the idle clock at sign-in and give the session its id.</summary>
    public static void StampSignIn(CookieSigningInContext context)
    {
        context.Properties.SetString(LastActivityKey, DateTimeOffset.UtcNow.UtcTicks.ToString());

        var identity = context.Principal?.Identities.FirstOrDefault();
        if (identity is not null && !identity.HasClaim(c => c.Type == SessionKey.ClaimType))
        {
            identity.AddClaim(new Claim(SessionKey.ClaimType, Guid.NewGuid().ToString("N")));
        }
    }

    public static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        var services = context.HttpContext.RequestServices;
        var idle = TimeSpan.FromMinutes(await services.GetRequiredService<SessionSettingsService>().GetIdleMinutesAsync());
        var now = DateTimeOffset.UtcNow;

        DateTimeOffset? last = null;
        if (long.TryParse(context.Properties.GetString(LastActivityKey), out var ticks))
        {
            last = new DateTimeOffset(ticks, TimeSpan.Zero);
        }

        // A running job (or one that just finished) is activity too.
        var key = SessionKey.For(context.Principal);
        var jobAt = key is null ? null : services.GetRequiredService<SessionActivityRegistry>().LastActivityUtc(key);
        if (jobAt is { } j && (last is null || j > last.Value.UtcDateTime))
        {
            last = new DateTimeOffset(DateTime.SpecifyKind(j, DateTimeKind.Utc));
        }

        if (last is { } l)
        {
            if (now - l > idle)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return;
            }

            // The page's "am I still signed in?" probe must never extend the session itself.
            if (context.Request.Path.StartsWithSegments("/session/check")) return;

            if (now - l < RestampEvery) return;
        }

        context.Properties.SetString(LastActivityKey, now.UtcTicks.ToString());
        context.ShouldRenew = true;
    }
}
