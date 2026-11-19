using Cincy_Biz_Advisor.Models.Compare;
using Cincy_Biz_Advisor.Services.Compare;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public sealed class CompareProviderIntegrationTests
{
    [Fact]
    public async Task CompareAsync_PreservesPartialProviderResult()
    {
        var provider = new ControlledCompareDataProvider(new CompareProviderData(
            Measure(ComparisonMeasureType.Crime, "0", "4", ComparisonAvailability.Available),
            Measure(ComparisonMeasureType.BuildingPermitActivity, "N/A", "9", ComparisonAvailability.Unavailable),
            Measure(ComparisonMeasureType.LocationSuitability, "81", "N/A", ComparisonAvailability.SourceError)));
        var service = new NeighborhoodComparisonService(provider);

        NeighborhoodComparisonResult result = await service.CompareAsync(
            new ComparisonRequest("restaurant", "oakley", "clifton"));

        Assert.True(result.HasPartialData);
        Assert.Equal("0", result.Measures[0].NeighborhoodAValue);
        Assert.Equal(ComparisonAvailability.Available, result.Measures[0].Availability);
        Assert.Equal(ComparisonAvailability.Unavailable, result.Measures[1].Availability);
        Assert.Equal(ComparisonAvailability.SourceError, result.Measures[2].Availability);
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public async Task CompareAsync_RejectsInvalidBusinessTypeBeforeProviderCall()
    {
        var provider = new ControlledCompareDataProvider(EmptyData());
        var service = new NeighborhoodComparisonService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CompareAsync(
            new ComparisonRequest("not-a-business", "oakley", "clifton")));

        Assert.Equal(0, provider.CallCount);
    }

    [Fact]
    public async Task CompareAsync_RejectsDuplicateNeighborhoodsBeforeProviderCall()
    {
        var provider = new ControlledCompareDataProvider(EmptyData());
        var service = new NeighborhoodComparisonService(provider);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CompareAsync(
            new ComparisonRequest("restaurant", "oakley", "oakley")));

        Assert.Equal(0, provider.CallCount);
    }

    private static CompareProviderData EmptyData() => new(
        Measure(ComparisonMeasureType.Crime, "N/A", "N/A", ComparisonAvailability.Unavailable),
        Measure(ComparisonMeasureType.BuildingPermitActivity, "N/A", "N/A", ComparisonAvailability.Unavailable),
        Measure(ComparisonMeasureType.LocationSuitability, "N/A", "N/A", ComparisonAvailability.Unavailable));

    private static ComparisonMeasure Measure(
        ComparisonMeasureType type,
        string neighborhoodAValue,
        string neighborhoodBValue,
        ComparisonAvailability availability) =>
        new(type, neighborhoodAValue, neighborhoodBValue, availability);

    private sealed class ControlledCompareDataProvider : ICompareDataProvider
    {
        private readonly CompareProviderData _data;

        public ControlledCompareDataProvider(CompareProviderData data)
        {
            _data = data;
        }

        public int CallCount { get; private set; }

        public Task<CompareProviderData> GetComparisonDataAsync(
            ComparisonRequest request,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(_data);
        }
    }
}
