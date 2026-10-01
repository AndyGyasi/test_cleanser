using MudBlazor;

namespace CleanserBlazorUI.Theme;

// Single source of truth for the app's visual identity, so every MudBlazor
// component (buttons, cards, data grids, charts) inherits the same palette
// instead of only the app bar/background being manually colored while
// everything else ran on MudBlazor's plain default theme.
//
// Direction: dark, dense "operations console" -- refined from the existing
// #001D23 / LightGreen.Accent3 combination already informally in use, but
// swapping the neon-lime accent (#76FF03) for a grounded teal-emerald.
// Neon-on-near-black reads as a generic/templated default; this keeps the
// same "dark + green" identity while making it feel like a deliberate
// choice for a credit bureau's data-operations tool.
//
// Both PaletteDark and PaletteLight are real, designed palettes (not one
// real + one MudBlazor default) -- see MainLayout.razor for the toggle that
// switches between them, persisted per-browser via localStorage.
public static class CleanserTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#1D9E75",           // teal-emerald brand accent -- buttons, links, active nav, chart primary series
            Secondary = "#4FD1C5",         // cyan, for secondary actions/highlights -- distinct enough from Primary to read separately
            Info = "#4FD1C5",
            Success = "#4CAF50",           // kept apart from Primary's teal-emerald so "success" stays legible as its own signal, not brand color
            Warning = "#E0A94E",
            Error = "#E0684E",

            Background = "#0D1B1D",
            Surface = "#132A2D",
            AppbarBackground = "#16232A",
            AppbarText = "#E7EDEA",
            DrawerBackground = "#132A2D",
            DrawerText = "#E7EDEA",
            DrawerIcon = "#93A29C",

            TextPrimary = "#E7EDEA",
            TextSecondary = "#93A29C",
            TextDisabled = "#5B6B67",

            ActionDefault = "#93A29C",
            ActionDisabled = "#3A4C48",

            Divider = "#22383B",
            LinesDefault = "#22383B",
            TableLines = "#22383B",
            TableStriped = "#12262A",
        },
        // Same brand hues as PaletteDark (teal-emerald primary, cyan
        // secondary) carried onto a clean off-white surface, so switching
        // themes changes the backdrop, not the app's identity.
        PaletteLight = new PaletteLight
        {
            Primary = "#1C8A66",           // slightly deepened from PaletteDark's #1D9E75 for AA contrast on white
            Secondary = "#1B8F86",         // deepened from #4FD1C5 for the same reason -- the dark palette's cyan is too pale to read on light backgrounds
            Info = "#1B8F86",
            Success = "#3F8F42",
            Warning = "#B5790F",
            Error = "#C2492F",

            Background = "#F6F9F8",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#1A2322",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#1A2322",
            DrawerIcon = "#5B6B67",

            TextPrimary = "#1A2322",
            TextSecondary = "#5B6B67",
            TextDisabled = "#A8B5B1",

            ActionDefault = "#5B6B67",
            ActionDisabled = "#C7D1CE",

            Divider = "#E2E8E6",
            LinesDefault = "#E2E8E6",
            TableLines = "#E2E8E6",
            TableStriped = "#F2F6F5",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = new[] { "Segoe UI", "system-ui", "sans-serif" } }
        }
    };
}
