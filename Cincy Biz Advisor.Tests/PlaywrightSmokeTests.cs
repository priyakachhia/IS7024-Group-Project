using Microsoft.Playwright;

namespace Cincy_Biz_Advisor.Tests;

public sealed class PlaywrightSmokeTests
{
    [Fact]
    public async Task ChromiumLaunchesAndReachesTestHost()
    {
        var baseUrl = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_HOST") ?? "http://127.0.0.1:5000";
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();
        var response = await page.GotoAsync($"{baseUrl.TrimEnd('/')}/");

        Assert.NotNull(response);
        Assert.Equal(200, response.Status);
    }
}