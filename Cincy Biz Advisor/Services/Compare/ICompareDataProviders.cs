using Cincy_Biz_Advisor.Models.Compare;

namespace Cincy_Biz_Advisor.Services.Compare;

public interface ICompareDataProvider
{
    Task<CompareProviderData> GetComparisonDataAsync(
        ComparisonRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record CompareProviderData(
    ComparisonMeasure Crime,
    ComparisonMeasure BuildingPermitActivity,
    ComparisonMeasure LocationSuitability);

public interface ICompareCatalogProvider
{
    IReadOnlyList<BusinessTypeOption> GetBusinessTypes();
    IReadOnlyList<NeighborhoodOption> GetNeighborhoods();
}
