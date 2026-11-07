using Cincy_Biz_Advisor.Models.Compare;
using Cincy_Biz_Advisor.Services.Compare;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public sealed class CompareNeighborhoodValidationTests
{
    [Fact]
    public async Task CompareAsync_RejectsIdenticalNeighborhoods()
    {
        var service = new NeighborhoodComparisonService(new UnavailableCompareDataProvider());

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CompareAsync(
            new ComparisonRequest("restaurant", "oakley", "oakley")));

        Assert.Contains("different neighborhoods", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CompareAsync_RejectsUnsupportedNeighborhood()
    {
        var service = new NeighborhoodComparisonService(new UnavailableCompareDataProvider());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CompareAsync(
            new ComparisonRequest("restaurant", "not-a-neighborhood", "oakley")));
    }
}
