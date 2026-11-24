namespace Cincy_Biz_Advisor.Services;

/// <summary>
/// Reads counts from the Cincinnati Open Data portal.
/// An interface so pages and tests can use a fake version.
/// </summary>
public interface ICityDataService
{
    /// <summary>Number of reported crimes in the neighborhood between start and end (inclusive).</summary>
    Task<int> GetCrimeCountAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);

    /// <summary>Number of building permits issued in the neighborhood between start and end (inclusive).</summary>
    Task<int> GetPermitCountAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);
}
