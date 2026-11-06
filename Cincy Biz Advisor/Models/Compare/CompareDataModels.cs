namespace Cincy_Biz_Advisor.Models.Compare;

public enum ComparisonMeasureType
{
    Crime,
    BuildingPermitActivity,
    LocationSuitability
}

public enum ComparisonAvailability
{
    Available,
    Unavailable,
    SourceError
}

public sealed record BusinessTypeOption(string Id, string Name);

public sealed record NeighborhoodOption(string Id, string Name, string QueryValue);

public sealed record ComparisonRequest(
    string BusinessTypeId,
    string NeighborhoodAId,
    string NeighborhoodBId);

public sealed record ReportingPeriod(string DisplayLabel, DateOnly? Start = null, DateOnly? End = null);

public sealed record ComparisonMeasure(
    ComparisonMeasureType MeasureType,
    string NeighborhoodAValue,
    string NeighborhoodBValue,
    ComparisonAvailability Availability,
    ReportingPeriod? ReportingPeriod = null,
    string? SourceLabel = null,
    string? Meaning = null,
    string? Message = null);

public sealed record NeighborhoodComparisonResult(
    BusinessTypeOption BusinessType,
    NeighborhoodOption NeighborhoodA,
    NeighborhoodOption NeighborhoodB,
    IReadOnlyList<ComparisonMeasure> Measures,
    bool HasPartialData,
    string? Message = null);

public static class CompareCatalog
{
    public static IReadOnlyList<BusinessTypeOption> BusinessTypes { get; } =
    [
        new("restaurant", "Restaurant"),
        new("coffee-shop", "Coffee Shop"),
        new("retail", "Retail")
    ];

    public static IReadOnlyList<NeighborhoodOption> Neighborhoods { get; } =
    [
        new("avondale", "Avondale", "AVONDALE"),
        new("clifton", "Clifton", "CLIFTON"),
        new("college-hill", "College Hill", "COLLEGE HILL"),
        new("east-end", "East End", "EAST END"),
        new("east-price-hill", "East Price Hill", "EAST PRICE HILL"),
        new("evanston", "Evanston", "EVANSTON"),
        new("hyde-park", "Hyde Park", "HYDE PARK"),
        new("linwood", "Linwood", "LINWOOD"),
        new("madisonville", "Madisonville", "MADISONVILLE"),
        new("mount-adams", "Mount Adams", "MT. ADAMS"),
        new("mount-auburn", "Mount Auburn", "MT. AUBURN"),
        new("mount-lookout", "Mount Lookout", "MT. LOOKOUT"),
        new("northside", "Northside", "NORTHSIDE"),
        new("oakley", "Oakley", "OAKLEY"),
        new("over-the-rhine", "Over-the-Rhine", "OVER-THE-RHINE"),
        new("pendleton", "Pendleton", "PENDLETON"),
        new("walnut-hills", "Walnut Hills", "WALNUT HILLS"),
        new("west-end", "West End", "WEST END"),
        new("westwood", "Westwood", "WESTWOOD")
    ];
}
