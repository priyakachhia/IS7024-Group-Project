namespace Cincy_Biz_Advisor.Models;

/// <summary>
/// A Cincinnati neighborhood used by the Home picker and the city data service.
/// </summary>
/// <param name="DisplayName">The neighborhood name shown to users.</param>
/// <param name="DatasetName">
/// The exact name used by the city datasets when it differs from the display name.
/// </param>
public sealed record Neighborhood(string DisplayName, string? DatasetName = null)
{
    public Neighborhood() : this(string.Empty) { }

    public int Id { get; init; }

    public string Name
    {
        get => DisplayName;
        init => DisplayName = value;
    }

    public string QueryValue => (DatasetName ?? DisplayName).ToUpperInvariant();
}

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

    public static Neighborhood? Find(string? displayName) =>
        string.IsNullOrWhiteSpace(displayName)
            ? null
            : All.FirstOrDefault(n =>
                string.Equals(n.DisplayName, displayName.Trim(), StringComparison.OrdinalIgnoreCase));
}