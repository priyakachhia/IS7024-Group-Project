using Microsoft.AspNetCore.Mvc.Testing;

namespace Cincy_Biz_Advisor.Tests;

public sealed class WebApplicationFactorySmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WebApplicationFactorySmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePageStartsThroughProgramTestSeam()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        using var response = await client.GetAsync("/");

        Assert.True(response.IsSuccessStatusCode);
    }
}
