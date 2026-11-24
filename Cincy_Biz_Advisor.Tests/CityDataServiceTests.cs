using System.Net;
using Cincy_Biz_Advisor.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public class CityDataServiceTests
{
    private static readonly DateOnly Start = new(2025, 1, 1);
    private static readonly DateOnly End = new(2025, 12, 31);

    [Fact]
    public async Task GetCrimeCountAsync_ReadsTotalFromResponse()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "[{\"total\":\"412\"}]");
        CityDataService service = CreateService(handler);

        int count = await service.GetCrimeCountAsync("OVER-THE-RHINE", Start, End);

        Assert.Equal(412, count);
    }

    [Fact]
    public async Task GetCrimeCountAsync_SendsNeighborhoodAndDatesToCrimeDataset()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "[{\"total\":\"412\"}]");
        CityDataService service = CreateService(handler);

        await service.GetCrimeCountAsync("OVER-THE-RHINE", Start, End);

        string url = Uri.UnescapeDataString(handler.LastRequestUri!.ToString());
        Assert.Contains("resource/7aqy-xrv9.json", url);
        Assert.Contains("upper(cpd_neighborhood) = 'OVER-THE-RHINE'", url);
        Assert.Contains("date_reported >= '2025-01-01T00:00:00'", url);
        Assert.Contains("date_reported < '2026-01-01T00:00:00'", url);
    }

    [Fact]
    public async Task GetPermitCountAsync_UsesPermitDataset()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "[{\"total\":\"87\"}]");
        CityDataService service = CreateService(handler);

        int count = await service.GetPermitCountAsync("OVER-THE-RHINE", Start, End);

        Assert.Equal(87, count);
        string url = Uri.UnescapeDataString(handler.LastRequestUri!.ToString());
        Assert.Contains("resource/uhjb-xac9.json", url);
        Assert.Contains("issueddate >= '2025-01-01T00:00:00'", url);
    }

    [Fact]
    public async Task GetCrimeCountAsync_ServerError_ThrowsWithFriendlyMessage()   // 101.4
    {
        var handler = new FakeHandler(HttpStatusCode.InternalServerError, "oops");
        CityDataService service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<CityDataUnavailableException>(
            () => service.GetCrimeCountAsync("OVER-THE-RHINE", Start, End));

        Assert.Equal("Crime data is currently unavailable. Please try again later.", ex.UserMessage);
    }

    [Fact]
    public async Task GetCrimeCountAsync_NetworkFailure_ThrowsWithFriendlyMessage()   // 101.4
    {
        var handler = new FakeHandler(new HttpRequestException("No connection"));
        CityDataService service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<CityDataUnavailableException>(
            () => service.GetCrimeCountAsync("OVER-THE-RHINE", Start, End));

        Assert.Equal("crime", ex.DatasetName);
    }

    [Fact]
    public void BuildCountQuery_EscapesQuotesInNames()
    {
        string query = CityDataService.BuildCountQuery("7aqy-xrv9", "cpd_neighborhood", "date_reported", "O'Bryonville", Start, End);

        Assert.Contains("'O''BRYONVILLE'", Uri.UnescapeDataString(query));
    }

    [Fact]
    public void BuildCountQuery_RejectsUnsafeFieldNames()
    {
        Assert.Throws<InvalidOperationException>(() =>
            CityDataService.BuildCountQuery("7aqy-xrv9", "x) OR (1=1", "date_reported", "OAKLEY", Start, End));
    }

    [Theory]
    [InlineData("[{\"total\":\"412\"}]", 412)]
    [InlineData("[{\"total\":54}]", 54)]
    [InlineData("[]", 0)]
    public void ParseCount_ReadsDifferentShapes(string json, int expected)
    {
        Assert.Equal(expected, CityDataService.ParseCount(json));
    }

    private static CityDataService CreateService(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://data.cincinnati-oh.gov/") };
        return new CityDataService(client, Options.Create(new CityDataOptions()), NullLogger<CityDataService>.Instance);
    }

    /// <summary>Pretends to be the city API so tests never touch the network.</summary>
    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _body = "";
        private readonly Exception? _exception;

        public Uri? LastRequestUri { get; private set; }

        public FakeHandler(HttpStatusCode statusCode, string body)
        {
            _statusCode = statusCode;
            _body = body;
        }

        public FakeHandler(Exception exception)
        {
            _exception = exception;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            if (_exception is not null)
            {
                throw _exception;
            }

            return Task.FromResult(new HttpResponseMessage(_statusCode) { Content = new StringContent(_body) });
        }
    }
}
