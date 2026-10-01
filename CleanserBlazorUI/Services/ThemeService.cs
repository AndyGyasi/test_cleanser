using Microsoft.JSInterop;

namespace CleanserBlazorUI.Services;

// Scoped so one instance is shared by every component on a circuit. Backed by
// a cookie rather than localStorage because static-rendered pages (Login and
// the other pre-auth Account pages) have no live circuit for JS interop, but
// they do have the request's cookies -- SetFromCookieValue lets them (and
// MainLayout, on the first static pass before the circuit connects) read the
// preference synchronously. ToggleAsync (JS-interop, only works once a
// circuit is live) keeps it updated for the rest of an interactive session.
public class ThemeService
{
    public const string CookieName = "cleanser-theme";

    public bool IsDark { get; private set; } = true;
    public event Action? Changed;

    public void SetFromCookieValue(string? raw)
    {
        if (raw == "light") IsDark = false;
        else if (raw == "dark") IsDark = true;
    }

    public async Task ToggleAsync(IJSRuntime js)
    {
        IsDark = !IsDark;
        Changed?.Invoke();

        try
        {
            await js.InvokeVoidAsync("cleanserTheme.set", IsDark ? "dark" : "light");
        }
        catch
        {
            // Not fatal -- the toggle still works for this circuit even if saving fails.
        }
    }
}
