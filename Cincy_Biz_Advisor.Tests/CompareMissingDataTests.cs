using Cincy_Biz_Advisor.Models.Compare;
using Cincy_Biz_Advisor.Services.Compare;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public sealed class CompareMissingDataTests
{
    [Fact]
    public async Task CompareAsync_ReportsUnavailableMeasuresWithoutInventedValues()
    {
        var service = new NeighborhoodComparisonService(new UnavailableCompareDataProvider());

        NeighborhoodComparisonResult result = await service.CompareAsync(
            new ComparisonRequest("restaurant", "oakley", "clifton"));

        Assert.Equal(3, result.Measures.Count);
        Assert.All(result.Measures, measure =>
        {
            Assert.Equal(ComparisonAvailability.Unavailable, measure.Availability);
            Assert.Equal("N/A", measure.NeighborhoodAValue);
            Assert.Equal("N/A", measure.NeighborhoodBValue);
        });
        Assert.False(result.HasPartialData);
    }
}
