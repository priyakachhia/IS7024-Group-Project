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

    [Fact]
    public async Task AnalyzeAsync_ReturnsChartData()
    {
        var crimeTrend = new List<MonthlyCount> { new(new DateOnly(2025, 1, 1), 30), new(new DateOnly(2025, 2, 1), 25) };
        var permitTrend = new List<MonthlyCount> { new(new DateOnly(2025, 1, 1), 7) };
        var byType = new List<CategoryCount> { new("THEFT", 40), new("ASSAULT", 15) };
        var service = new NeighborhoodAnalysisService(
            new FakeCityData(crimes: 55, permits: 7, crimeTrend: crimeTrend, permitTrend: permitTrend, crimeByType: byType));

        NeighborhoodAnalysis result = await service.AnalyzeAsync("Over-the-Rhine", BusinessType.Restaurant, Start, End);

        Assert.Equal(crimeTrend, result.CrimeTrend);
        Assert.Equal(permitTrend, result.PermitTrend);
        Assert.Equal(byType, result.CrimeByType);
    }

    [Fact]
    public async Task AnalyzeAsync_MoreThanFiveCrimeTypes_GroupsTheRestAsOther()
    {
        var byType = new List<CategoryCount>
        {
            new("A", 50), new("B", 40), new("C", 30), new("D", 20), new("E", 10), new("F", 5), new("G", 3)
        };
        var service = new NeighborhoodAnalysisService(new FakeCityData(crimes: 158, permits: 1, crimeByType: byType));

        NeighborhoodAnalysis result = await service.AnalyzeAsync("Over-the-Rhine", BusinessType.Restaurant, Start, End);

        Assert.Equal(6, result.CrimeByType.Count);
        Assert.Equal(new CategoryCount("OTHER", 8), result.CrimeByType[^1]);
        Assert.Equal(new CategoryCount("A", 50), result.CrimeByType[0]);
    }

    private sealed class FakeCityData : ICityDataService
    {
        private readonly int _crimes;
        private readonly int _permits;
        private readonly bool _crimeFails;
        private readonly IReadOnlyList<MonthlyCount> _crimeTrend;
        private readonly IReadOnlyList<MonthlyCount> _permitTrend;
        private readonly IReadOnlyList<CategoryCount> _crimeByType;

        public string? LastNeighborhood { get; private set; }

        public FakeCityData(
            int crimes,
            int permits,
            bool crimeFails = false,
            IReadOnlyList<MonthlyCount>? crimeTrend = null,
            IReadOnlyList<MonthlyCount>? permitTrend = null,
            IReadOnlyList<CategoryCount>? crimeByType = null)
        {
            _crimes = crimes;
            _permits = permits;
            _crimeFails = crimeFails;
            _crimeTrend = crimeTrend ?? [];
            _permitTrend = permitTrend ?? [];
            _crimeByType = crimeByType ?? [];
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

        public Task<IReadOnlyList<MonthlyCount>> GetCrimeTrendAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
            Task.FromResult(_crimeTrend);

        public Task<IReadOnlyList<MonthlyCount>> GetPermitTrendAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
            Task.FromResult(_permitTrend);

        public Task<IReadOnlyList<CategoryCount>> GetCrimeByTypeAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
            Task.FromResult(_crimeByType);
    }
}