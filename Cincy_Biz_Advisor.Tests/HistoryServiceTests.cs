using Cincy_Biz_Advisor.Data;
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public sealed class HistoryServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<HistoryDbContext> _options;

    public HistoryServiceTests()
    {
        // An in-memory SQLite database lives only as long as this connection stays open.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<HistoryDbContext>().UseSqlite(_connection).Options;

        using var context = new HistoryDbContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private HistoryService CreateService() => new(new HistoryDbContext(_options));

    private static HistoryEntry NewEntry(string neighborhood) => new()
    {
        Neighborhood = neighborhood,
        BusinessType = "Restaurant",
        ReportedCrimes = 100,
        BuildingPermits = 50,
        SafetyScore = 87.5,
        GrowthScore = 50,
        OverallScore = 72
    };

    [Fact]
    public async Task AddAsync_SavesEntryAndAssignsId()
    {
        HistoryService service = CreateService();

        HistoryEntry saved = await service.AddAsync(NewEntry("Madisonville"));

        Assert.True(saved.Id > 0);
        IReadOnlyList<HistoryEntry> all = await CreateService().GetAllAsync();
        HistoryEntry stored = Assert.Single(all);
        Assert.Equal("Madisonville", stored.Neighborhood);
        Assert.Equal(87.5, stored.SafetyScore);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNewestFirst()
    {
        HistoryService service = CreateService();
        await service.AddAsync(NewEntry("Oakley"));
        await service.AddAsync(NewEntry("Hyde Park"));
        await service.AddAsync(NewEntry("Downtown"));

        IReadOnlyList<HistoryEntry> all = await service.GetAllAsync();

        Assert.Equal(new[] { "Downtown", "Hyde Park", "Oakley" }, all.Select(e => e.Neighborhood));
    }

    [Fact]
    public async Task GetAllAsync_WhenNothingSaved_ReturnsEmptyList()
    {
        IReadOnlyList<HistoryEntry> all = await CreateService().GetAllAsync();

        Assert.Empty(all);
    }
}