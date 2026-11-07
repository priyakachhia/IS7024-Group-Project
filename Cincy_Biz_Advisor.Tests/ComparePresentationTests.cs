using Cincy_Biz_Advisor.Models.Compare;
using Cincy_Biz_Advisor.Services.Compare;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public sealed class ComparePresentationTests
{
    [Fact]
    public async Task CompareAsync_ProvidesThreeMeasuresForChartAndTableBinding()
    {
        var service = new NeighborhoodComparisonService(new UnavailableCompareDataProvider());

        NeighborhoodComparisonResult result = await service.CompareAsync(
            new ComparisonRequest("restaurant", "oakley", "clifton"));

        Assert.Equal(
            [
                ComparisonMeasureType.Crime,
                ComparisonMeasureType.BuildingPermitActivity,
                ComparisonMeasureType.LocationSuitability
            ],
            result.Measures.Select(measure => measure.MeasureType));

        Assert.Equal(3, result.Measures.Count);
        Assert.All(result.Measures, measure =>
        {
            Assert.Equal("N/A", measure.NeighborhoodAValue);
            Assert.Equal("N/A", measure.NeighborhoodBValue);
            Assert.Equal(ComparisonAvailability.Unavailable, measure.Availability);
        });
    }

    [Fact]
    public async Task CompareAsync_RejectsUnsupportedBusinessType()
    {
        var service = new NeighborhoodComparisonService(new UnavailableCompareDataProvider());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CompareAsync(
            new ComparisonRequest("not-a-business", "oakley", "clifton")));
    }
}
