
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Models.Compare;

namespace Cincy_Biz_Advisor.Services.Compare;

public sealed class AnalyzeCompareDataProvider : ICompareDataProvider
{
    private readonly INeighborhoodAnalysisService _analysis;

    public AnalyzeCompareDataProvider(
        INeighborhoodAnalysisService analysis)
    {
        _analysis = analysis;
    }

    public async Task<CompareProviderData> GetComparisonDataAsync(
        ComparisonRequest request,
        CancellationToken cancellationToken = default)
    {
        var neighborhoodA = CompareCatalog.Neighborhoods
            .FirstOrDefault(n => n.Id == request.NeighborhoodAId)
            ?? throw new ArgumentException("Invalid first neighborhood.");

        var neighborhoodB = CompareCatalog.Neighborhoods
            .FirstOrDefault(n => n.Id == request.NeighborhoodBId)
            ?? throw new ArgumentException("Invalid second neighborhood.");

        var businessType = request.BusinessTypeId switch
        {
            "restaurant" => BusinessType.Restaurant,
            "coffee-shop" => BusinessType.CoffeeShop,
            "retail" => BusinessType.Retail,
            _ => throw new ArgumentException("Invalid business type.")
        };

        // Both neighborhoods use the same complete calendar year.
        var start = new DateOnly(2025, 1, 1);
        var end = new DateOnly(2025, 12, 31);

        var first = await _analysis.AnalyzeAsync(
            neighborhoodA.Name, businessType, start, end,
            cancellationToken);

        var second = await _analysis.AnalyzeAsync(
            neighborhoodB.Name, businessType, start, end,
            cancellationToken);

        var period = new ReportingPeriod(
            "January–December 2025", start, end);

        return new CompareProviderData(
            new ComparisonMeasure(
                ComparisonMeasureType.Crime,
                first.CrimeCount.ToString(),
                second.CrimeCount.ToString(),
                ComparisonAvailability.Available,
                period,
                "Cincinnati Open Data",
                "Reported crime incidents"),

            new ComparisonMeasure(
                ComparisonMeasureType.BuildingPermitActivity,
                first.PermitCount.ToString(),
                second.PermitCount.ToString(),
                ComparisonAvailability.Available,
                period,
                "Cincinnati Open Data",
                "Building permit records"),

            new ComparisonMeasure(
                ComparisonMeasureType.LocationSuitability,
                first.SuitabilityScore.ToString(),
                second.SuitabilityScore.ToString(),
                ComparisonAvailability.Available,
                period,
                "Calculated from crime and permit data",
                "Business-type-specific suitability score")
        );
    }
}
