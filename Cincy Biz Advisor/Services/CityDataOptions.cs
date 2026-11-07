namespace Cincy_Biz_Advisor.Services;

/// <summary>
/// Settings for the Cincinnati Open Data (Socrata) API, read from the "CityData"
/// section of appsettings.json. Field names live here (not in code) so they can be
/// fixed without recompiling if the city renames a column.
/// </summary>
public sealed class CityDataOptions
{
    public const string SectionName = "CityData";

    public string BaseUrl { get; set; } = "https://data.cincinnati-oh.gov/";

    // Reported Crime (STARS Category Offenses) on or after 6/3/2024
    public string CrimeDatasetId { get; set; } = "7aqy-xrv9";
    public string CrimeNeighborhoodField { get; set; } = "cpd_neighborhood";
    public string CrimeDateField { get; set; } = "date_reported";

    // Cincinnati Building Permits
    public string PermitDatasetId { get; set; } = "uhjb-xac9";
    public string PermitNeighborhoodField { get; set; } = "neighborhood";
    public string PermitDateField { get; set; } = "issueddate";

    public int TimeoutSeconds { get; set; } = 15;
}
