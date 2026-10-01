using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace CleanserBlazorUI.Services;

/// <summary>
/// Enforces the admin-configured idle timeout on the sign-in cookie. Every
/// authenticated request stamps "last activity" into the cookie (re-issued at
/// most once a minute); a request that arrives after the idle window has
/// passed is rejected and the user is signed out. The cookie's own expiry is
/// only a long backstop -- this check is what ends sessions, and it reads the
/// current setting on every request, so a change by an admin applies at once.
/// </summary>
public static class SessionIdle
{
    private const string LastActivityKey = "cx.last";
    private static readonly TimeSpan RestampEvery = TimeSpan.FromMinutes(1);

    /// <summary>Start the idle clock at sign-in, so a cookie that is never used still expires.</summary>
    public static void StampSignIn(CookieSigningInContext context) =>
        context.Properties.SetString(LastActivityKey, DateTimeOffset.UtcNow.UtcTicks.ToString());

    public static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        var settings = context.HttpContext.RequestServices.GetRequiredService<SessionSettingsService>();
        var idle = TimeSpan.FromMinutes(await settings.GetIdleMinutesAsync());
        var now = DateTimeOffset.UtcNow;

        if (long.TryParse(context.Properties.GetString(LastActivityKey), out var ticks))
        {
            var last = new DateTimeOffset(ticks, TimeSpan.Zero);
            if (now - last > idle)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return;
            }

            if (now - last < RestampEvery) return;
        }

        context.Properties.SetString(LastActivityKey, now.UtcTicks.ToString());
        context.ShouldRenew = true;
    }
}
