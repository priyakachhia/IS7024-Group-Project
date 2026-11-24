using Cincy_Biz_Advisor.Models.Compare;

namespace Cincy_Biz_Advisor.Services.Compare;

public sealed class NeighborhoodComparisonService
{
    private readonly ICompareDataProvider _dataProvider;

    public NeighborhoodComparisonService(ICompareDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public async Task<NeighborhoodComparisonResult> CompareAsync(
        ComparisonRequest request,
        CancellationToken cancellationToken = default)
    {
        BusinessTypeOption businessType = FindBusinessType(request.BusinessTypeId)
            ?? throw new ArgumentException("The selected business type is not available.", nameof(request));
        NeighborhoodOption neighborhoodA = FindNeighborhood(request.NeighborhoodAId)
            ?? throw new ArgumentException("The first neighborhood is not available.", nameof(request));
        NeighborhoodOption neighborhoodB = FindNeighborhood(request.NeighborhoodBId)
            ?? throw new ArgumentException("The second neighborhood is not available.", nameof(request));

        if (neighborhoodA.Id == neighborhoodB.Id)
        {
            throw new ArgumentException("Select two different neighborhoods.", nameof(request));
        }

        CompareProviderData data = await _dataProvider.GetComparisonDataAsync(request, cancellationToken);
        IReadOnlyList<ComparisonMeasure> measures =
        [data.Crime, data.BuildingPermitActivity, data.LocationSuitability];
        bool hasAvailable = measures.Any(measure => measure.Availability == ComparisonAvailability.Available);
        bool hasUnavailable = measures.Any(measure => measure.Availability != ComparisonAvailability.Available);

        return new NeighborhoodComparisonResult(
            businessType,
            neighborhoodA,
            neighborhoodB,
            measures,
            hasAvailable && hasUnavailable,
            hasUnavailable ? "Some comparison measures are unavailable." : null);
    }

    private static BusinessTypeOption? FindBusinessType(string id) =>
        CompareCatalog.BusinessTypes.FirstOrDefault(option =>
            string.Equals(option.Id, id, StringComparison.OrdinalIgnoreCase));

    private static NeighborhoodOption? FindNeighborhood(string id) =>
        CompareCatalog.Neighborhoods.FirstOrDefault(option =>
            string.Equals(option.Id, id, StringComparison.OrdinalIgnoreCase));
}
