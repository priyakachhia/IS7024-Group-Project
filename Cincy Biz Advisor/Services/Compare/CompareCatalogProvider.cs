using Cincy_Biz_Advisor.Models.Compare;

namespace Cincy_Biz_Advisor.Services.Compare;

public sealed class CompareCatalogProvider : ICompareCatalogProvider
{
    public IReadOnlyList<BusinessTypeOption> GetBusinessTypes() => CompareCatalog.BusinessTypes;

    public IReadOnlyList<NeighborhoodOption> GetNeighborhoods() => CompareCatalog.Neighborhoods;
}
