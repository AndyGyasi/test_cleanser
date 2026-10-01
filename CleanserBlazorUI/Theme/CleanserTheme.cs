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
            Primary = "#157A47",           // teal-emerald brand accent -- buttons, links, active nav, chart primary series
            Secondary = "#2FBF71",         // cyan, for secondary actions/highlights -- distinct enough from Primary to read separately
            Info = "#9CCBF2",
            Success = "#2FBF71",           // kept apart from Primary's teal-emerald so "success" stays legible as its own signal, not brand color
            Warning = "#F2B85C",
            Error = "#F2A18C",

            Background = "#071A1F",
            Surface = "#0D262C",
            AppbarBackground = "#0A2228",
            AppbarText = "#E6F0EE",
            DrawerBackground = "#0A2228",
            DrawerText = "#E6F0EE",
            DrawerIcon = "#8AA5A1",

            TextPrimary = "#E6F0EE",
            TextSecondary = "#B0C4C1",
            TextDisabled = "#5F7773",

            ActionDefault = "#8AA5A1",
            ActionDisabled = "#2A4A51",

            Divider = "#18363D",
            LinesDefault = "#18363D",
            TableLines = "#18363D",
            TableStriped = "#0A2025",
        },
        // Same brand hues as PaletteDark (teal-emerald primary, cyan
        // secondary) carried onto a clean off-white surface, so switching
        // themes changes the backdrop, not the app's identity.
        PaletteLight = new PaletteLight
        {
            Primary = "#157A47",           // slightly deepened from PaletteDark's #1D9E75 for AA contrast on white
            Secondary = "#126B3E",         // deepened from #4FD1C5 for the same reason -- the dark palette's cyan is too pale to read on light backgrounds
            Info = "#1F5A8A",
            Success = "#1E9E5E",
            Warning = "#8A5A0A",
            Error = "#A23A22",

            Background = "#F6F8F7",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#0E2A2F",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#0E2A2F",
            DrawerIcon = "#5F7773",

            TextPrimary = "#0E2A2F",
            TextSecondary = "#47625E",
            TextDisabled = "#A8B5B1",

            ActionDefault = "#5F7773",
            ActionDisabled = "#C3D4CF",

            Divider = "#DCE6E3",
            LinesDefault = "#DCE6E3",
            TableLines = "#DCE6E3",
            TableStriped = "#F3F7F5",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = new[] { "IBM Plex Sans", "Segoe UI", "system-ui", "sans-serif" } }
        }
    };
}
