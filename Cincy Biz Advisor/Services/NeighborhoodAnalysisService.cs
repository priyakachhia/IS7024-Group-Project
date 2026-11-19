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
    // The donut chart shows the biggest crime types; everything else is grouped as "OTHER".
    private const int MaxCrimeTypes = 5;

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

        // Ask for everything at the same time instead of one after the other.
        Task<int> crimeTask = _cityDataService.GetCrimeCountAsync(neighborhood.QueryValue, start, end, cancellationToken);
        Task<int> permitTask = _cityDataService.GetPermitCountAsync(neighborhood.QueryValue, start, end, cancellationToken);
        Task<IReadOnlyList<MonthlyCount>> crimeTrendTask = _cityDataService.GetCrimeTrendAsync(neighborhood.QueryValue, start, end, cancellationToken);
        Task<IReadOnlyList<MonthlyCount>> permitTrendTask = _cityDataService.GetPermitTrendAsync(neighborhood.QueryValue, start, end, cancellationToken);
        Task<IReadOnlyList<CategoryCount>> crimeByTypeTask = _cityDataService.GetCrimeByTypeAsync(neighborhood.QueryValue, start, end, cancellationToken);
        await Task.WhenAll(crimeTask, permitTask, crimeTrendTask, permitTrendTask, crimeByTypeTask);

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
            SuitabilityScore = SuitabilityCalculator.CalculateScore(crimeCount, permitCount, businessType),
            CrimeTrend = await crimeTrendTask,
            PermitTrend = await permitTrendTask,
            CrimeByType = KeepLargestCategories(await crimeByTypeTask)
        };
    }

    // Keeps the biggest crime types and adds the rest together as "OTHER".
    private static IReadOnlyList<CategoryCount> KeepLargestCategories(IReadOnlyList<CategoryCount> categories)
    {
        if (categories.Count <= MaxCrimeTypes)
        {
            return categories;
        }

        List<CategoryCount> ordered = categories.OrderByDescending(c => c.Count).ToList();
        List<CategoryCount> result = ordered.Take(MaxCrimeTypes).ToList();
        result.Add(new CategoryCount("OTHER", ordered.Skip(MaxCrimeTypes).Sum(c => c.Count)));
        return result;
    }
}