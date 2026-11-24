using Cincy_Biz_Advisor.Models;

namespace Cincy_Biz_Advisor.Services;

/// <summary>
/// Analyzes a neighborhood: gets crime and permit counts from the city,
/// then calculates the scores. Compare (Req 102) can call this once per neighborhood.
/// </summary>
public interface INeighborhoodAnalysisService
{
    Task<NeighborhoodAnalysis> AnalyzeAsync(
        string neighborhoodName,
        BusinessType businessType,
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default);
}

public sealed class NeighborhoodAnalysisService : INeighborhoodAnalysisService
{
    private readonly ICityDataService _cityDataService;

    public NeighborhoodAnalysisService(ICityDataService cityDataService)
    {
        _cityDataService = cityDataService;
    }

    public async Task<NeighborhoodAnalysis> AnalyzeAsync(
        string neighborhoodName,
        BusinessType businessType,
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default)
    {
        Neighborhood neighborhood = CincinnatiNeighborhoods.Find(neighborhoodName)
            ?? throw new ArgumentException($"Unknown neighborhood: '{neighborhoodName}'.", nameof(neighborhoodName));

        if (end < start)
        {
            throw new ArgumentException("The end date must be on or after the start date.", nameof(end));
        }

        // Ask for both counts at the same time instead of one after the other.
        Task<int> crimeTask = _cityDataService.GetCrimeCountAsync(neighborhood.QueryValue, start, end, cancellationToken);
        Task<int> permitTask = _cityDataService.GetPermitCountAsync(neighborhood.QueryValue, start, end, cancellationToken);
        await Task.WhenAll(crimeTask, permitTask);

        int crimeCount = await crimeTask;
        int permitCount = await permitTask;

        return new NeighborhoodAnalysis
        {
            Neighborhood = neighborhood.DisplayName,
            BusinessType = businessType,
            PeriodStart = start,
            PeriodEnd = end,
            CrimeCount = crimeCount,
            PermitCount = permitCount,
            SafetyScore = SuitabilityCalculator.CalculateSafety(crimeCount),
            GrowthScore = SuitabilityCalculator.CalculateGrowth(permitCount),
            SuitabilityScore = SuitabilityCalculator.CalculateScore(crimeCount, permitCount, businessType)
        };
    }
}
