using Cincy_Biz_Advisor.Models.Compare;

namespace Cincy_Biz_Advisor.Services.Compare;

public sealed class UnavailableCompareDataProvider : ICompareDataProvider
{
    public Task<CompareProviderData> GetComparisonDataAsync(
        ComparisonRequest request,
        CancellationToken cancellationToken = default)
    {
        const string message = "This measure is unavailable until its approved source is configured.";

        return Task.FromResult(new CompareProviderData(
            CreateUnavailable(ComparisonMeasureType.Crime, "Crime", message),
            CreateUnavailable(ComparisonMeasureType.BuildingPermitActivity, "Building permits", message),
            CreateUnavailable(ComparisonMeasureType.LocationSuitability, "Location suitability", "An existing suitability result is not available.")));
    }

    private static ComparisonMeasure CreateUnavailable(
        ComparisonMeasureType measureType,
        string sourceLabel,
        string message) =>
        new(
            measureType,
            "N/A",
            "N/A",
            ComparisonAvailability.Unavailable,
            SourceLabel: sourceLabel,
            Message: message);
}
