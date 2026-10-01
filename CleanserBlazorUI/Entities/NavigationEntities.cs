using System.ComponentModel.DataAnnotations.Schema;

namespace CleanserBlazorUI.Entities;

/// <summary>
/// A sidebar section header (e.g. "Data Cleaning", "Unloadables"). Admin can
/// rename these and drag NavItems between them -- see NavigationSettings.razor.
/// The sidebar itself (NavMenu.razor) renders sections in DisplayOrder, each
/// with only the items the current user's role is allowed to see; a section
/// with zero visible items for that user is skipped entirely.
/// </summary>
public class NavSection
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public List<NavItem> Items { get; set; } = new();
}

/// <summary>
/// One sidebar link. RequiredRoles is a comma-separated list of role names
/// (e.g. "admin,registrar") checked against the logged-in user -- empty/null
/// means visible to any authenticated user. IconKey is a short lookup name
/// (see NavMenu.razor's icon map), not a literal icon string, so this stays
/// readable in the database and in the management UI.
/// </summary>
public class NavItem
{
    public int Id { get; set; }

    public int NavSectionId { get; set; }
    [ForeignKey(nameof(NavSectionId))]
    public NavSection? NavSection { get; set; }

    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string? IconKey { get; set; }
    public string? RequiredRoles { get; set; }
    public int DisplayOrder { get; set; }
}
