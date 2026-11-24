using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public class NeighborhoodAnalysisServiceTests
{
    private static readonly DateOnly Start = new(2025, 1, 1);
    private static readonly DateOnly End = new(2025, 12, 31);

    [Fact]
    public async Task AnalyzeAsync_OverTheRhineRestaurant_ReturnsReadmeExample()   // 101.1
    {
        var service = new NeighborhoodAnalysisService(new FakeCityData(crimes: 412, permits: 87));

        NeighborhoodAnalysis result = await service.AnalyzeAsync("Over-the-Rhine", BusinessType.Restaurant, Start, End);

        Assert.Equal("Over-the-Rhine", result.Neighborhood);
        Assert.Equal("Restaurant", result.BusinessTypeName);
        Assert.Equal(412, result.CrimeCount);
        Assert.Equal(87, result.PermitCount);
        Assert.Equal(72, result.SuitabilityScore);
    }

    [Fact]
    public async Task AnalyzeAsync_OakleyCoffeeShop_ReturnsReadmeExample()   // 101.2
    {
        var service = new NeighborhoodAnalysisService(new FakeCityData(crimes: 138, permits: 41));

        NeighborhoodAnalysis result = await service.AnalyzeAsync("oakley", BusinessType.CoffeeShop, Start, End);

        Assert.Equal("Oakley", result.Neighborhood);
        Assert.Equal("Coffee Shop", result.BusinessTypeName);
        Assert.Equal(64, result.SuitabilityScore);
    }

    [Fact]
    public async Task AnalyzeAsync_SendsUppercaseNameToCityData()
    {
        var cityData = new FakeCityData(crimes: 1, permits: 1);
        var service = new NeighborhoodAnalysisService(cityData);

        await service.AnalyzeAsync("Over-the-Rhine", BusinessType.Retail, Start, End);

        Assert.Equal("OVER-THE-RHINE", cityData.LastNeighborhood);
    }

    [Fact]
    public async Task AnalyzeAsync_UnknownNeighborhood_Throws()
    {
        var service = new NeighborhoodAnalysisService(new FakeCityData(crimes: 0, permits: 0));

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AnalyzeAsync("abcdefgh123", BusinessType.Restaurant, Start, End));
    }

    [Fact]
    public async Task AnalyzeAsync_CrimeDataDown_Throws()   // 101.4
    {
        var service = new NeighborhoodAnalysisService(new FakeCityData(crimes: 0, permits: 0, crimeFails: true));

        var ex = await Assert.ThrowsAsync<CityDataUnavailableException>(
            () => service.AnalyzeAsync("Over-the-Rhine", BusinessType.Restaurant, Start, End));

        Assert.Equal("Crime data is currently unavailable. Please try again later.", ex.UserMessage);
    }

    private sealed class FakeCityData : ICityDataService
    {
        private readonly int _crimes;
        private readonly int _permits;
        private readonly bool _crimeFails;

        public string? LastNeighborhood { get; private set; }

        public FakeCityData(int crimes, int permits, bool crimeFails = false)
        {
            _crimes = crimes;
            _permits = permits;
            _crimeFails = crimeFails;
        }

        public Task<int> GetCrimeCountAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default)
        {
            LastNeighborhood = neighborhoodQueryValue;
            if (_crimeFails)
            {
                throw new CityDataUnavailableException("crime", CityDataService.CrimeUnavailableMessage);
            }

            return Task.FromResult(_crimes);
        }

        public Task<int> GetPermitCountAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
            Task.FromResult(_permits);
    }
}
