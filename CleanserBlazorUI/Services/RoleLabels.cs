namespace CleanserBlazorUI.Services;

/// <summary>Friendly names for the role strings stored in the database.</summary>
public static class RoleLabels
{
    // Most-privileged first, so a user with several roles is shown by their strongest one.
    private static readonly (string Role, string Label)[] Known =
    {
        ("admin", "Administrator"), ("superuser", "Superuser"), ("manager", "Manager"),
        ("registrar", "Registrar"), ("user", "User"),
    };

    public static string For(string role) =>
        Known.FirstOrDefault(k => string.Equals(k.Role, role, StringComparison.OrdinalIgnoreCase)).Label ?? role;

    /// <summary>The label of the user's strongest known role, else their first role, else "User".</summary>
    public static string ForUser(IEnumerable<string> roles)
    {
        var set = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
        var known = Known.FirstOrDefault(k => set.Contains(k.Role)).Label;
        return known ?? set.FirstOrDefault() ?? "User";
    }
}
