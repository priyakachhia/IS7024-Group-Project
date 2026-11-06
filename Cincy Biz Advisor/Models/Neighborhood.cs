namespace Cincy_Biz_Advisor.Models;

/// <summary>
/// A Cincinnati neighborhood shown in the dropdown.
/// </summary>
/// <param name="DisplayName">The name users see, for example "Over-the-Rhine".</param>
/// <param name="DatasetName">
/// Optional: the exact name used in the city datasets, if it is spelled differently
/// from the display name. Leave null when the names match (case does not matter).
/// </param>
public sealed record Neighborhood(string DisplayName, string? DatasetName = null)
{
    /// <summary>The value sent to the city API. Compared in uppercase, so case does not matter.</summary>
    public string QueryValue => (DatasetName ?? DisplayName).ToUpperInvariant();
}

/// <summary>
/// The neighborhoods users can pick from.
/// If a neighborhood always returns 0 crimes and 0 permits, its spelling probably
/// differs in the city data. Check the exact name (see docs/STEP_BY_STEP.md) and
/// set DatasetName.
/// </summary>
public static class CincinnatiNeighborhoods
{
    public static IReadOnlyList<Neighborhood> All { get; } = new List<Neighborhood>
    {
        new("Avondale"),
        new("Clifton"),
        new("College Hill"),
        new("East End"),
        new("East Price Hill"),
        new("Evanston"),
        new("Hyde Park"),
        new("Linwood"),
        new("Madisonville"),
        new("Mount Adams", "Mt. Adams"),
        new("Mount Auburn", "Mt. Auburn"),
        new("Mount Lookout", "Mt. Lookout"),
        new("Northside"),
        new("Oakley"),
        new("Over-the-Rhine"),
        new("Pendleton"),
        new("Walnut Hills"),
        new("West End"),
        new("Westwood")
    };

    /// <summary>Finds a neighborhood by its display name, ignoring case. Returns null if not found.</summary>
    public static Neighborhood? Find(string? displayName) =>
        string.IsNullOrWhiteSpace(displayName)
            ? null
            : All.FirstOrDefault(n => string.Equals(n.DisplayName, displayName.Trim(), StringComparison.OrdinalIgnoreCase));
}
