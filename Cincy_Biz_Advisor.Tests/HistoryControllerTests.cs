using System.ComponentModel.DataAnnotations;
using Cincy_Biz_Advisor.Controllers;
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public class HistoryControllerTests
{
    private sealed class FakeHistoryService : IHistoryService
    {
        public List<HistoryEntry> Saved { get; } = [];

        public Task<HistoryEntry> AddAsync(HistoryEntry entry, CancellationToken cancellationToken = default)
        {
            entry.Id = Saved.Count + 1;
            Saved.Add(entry);
            return Task.FromResult(entry);
        }

        public Task<IReadOnlyList<HistoryEntry>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<HistoryEntry> newestFirst = Enumerable.Reverse(Saved).ToList();
            return Task.FromResult(newestFirst);
        }
    }

    private static SaveHistoryRequest ValidRequest() => new()
    {
        Neighborhood = "  Madisonville ",
        BusinessType = "Restaurant",
        ReportedCrimes = 396,
        BuildingPermits = 462,
        SafetyScore = 50.5,
        GrowthScore = 100,
        OverallScore = 80
    };

    private static bool IsValid(SaveHistoryRequest request)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201AndSavesTrimmedEntry()
    {
        var service = new FakeHistoryService();
        var controller = new HistoryController(service);

        ActionResult<HistoryEntry> result = await controller.Create(ValidRequest(), CancellationToken.None);

        CreatedResult created = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
        HistoryEntry entry = Assert.IsType<HistoryEntry>(created.Value);
        Assert.Equal("Madisonville", entry.Neighborhood);
        Assert.Single(service.Saved);
    }

    [Fact]
    public async Task GetAll_Returns200WithSavedEntries()
    {
        var service = new FakeHistoryService();
        var controller = new HistoryController(service);
        await controller.Create(ValidRequest(), CancellationToken.None);

        ActionResult<IReadOnlyList<HistoryEntry>> result = await controller.GetAll(CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
        IReadOnlyList<HistoryEntry> entries = Assert.IsAssignableFrom<IReadOnlyList<HistoryEntry>>(ok.Value);
        Assert.Single(entries);
    }

    [Fact]
    public void Request_WithValidValues_PassesValidation()
    {
        Assert.True(IsValid(ValidRequest()));
    }

    [Fact]
    public void Request_WithMissingNeighborhood_FailsValidation()
    {
        SaveHistoryRequest request = ValidRequest();
        request.Neighborhood = "";

        Assert.False(IsValid(request));
    }

    [Fact]
    public void Request_WithScoreAbove100_FailsValidation()
    {
        SaveHistoryRequest request = ValidRequest();
        request.OverallScore = 101;

        Assert.False(IsValid(request));
    }

    [Fact]
    public void Request_WithNegativeCrimeCount_FailsValidation()
    {
        SaveHistoryRequest request = ValidRequest();
        request.ReportedCrimes = -1;

        Assert.False(IsValid(request));
    }
}