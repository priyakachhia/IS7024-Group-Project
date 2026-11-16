using System.Net;
using Cincy_Biz_Advisor.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Cincy_Biz_Advisor.Tests.Acceptance;

public sealed class HomePageAcceptanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HomePageAcceptanceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("Restaurant")]
    [InlineData("Coffee Shop")]
    [InlineData("Retail Store")]
    [InlineData("Fitness Center")]
    public async Task ValidBusinessTypesRedirectToAnalyzeWithDecodedSelectedValues(string businessType)
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await PostFormAsync(client, businessType);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var location = new Uri(new Uri("http://localhost"), response.Headers.Location!.OriginalString);
        Assert.Equal("/Analyze", location.AbsolutePath);

        var query = QueryHelpers.ParseQuery(location.Query);
        Assert.Equal("Over-the-Rhine", query["neighborhood"]);
        Assert.Equal(businessType, query["businessType"]);
    }

    [Fact]
    public async Task RedirectLocationContainsEncodedNeighborhoodAndBusinessTypeValues()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await PostFormAsync(client, "Coffee Shop");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = new Uri(new Uri("http://localhost"), response.Headers.Location!.OriginalString);
        Assert.Contains("businessType=Coffee%20Shop", location.OriginalString);

        var query = QueryHelpers.ParseQuery(location.Query);
        Assert.Equal("Over-the-Rhine", query["neighborhood"]);
        Assert.Equal("Coffee Shop", query["businessType"]);
    }

    [Theory]
    [InlineData("Over-the-Rhine", "Over-the-Rhine")]
    [InlineData("oakley", "Oakley")]
    [InlineData("OAKLEY", "Oakley")]
    [InlineData("oak", "Oakley")]
    [InlineData("  oak  ", "Oakley")]
    public async Task NeighborhoodSearchMatchesCaseInsensitiveSubstringsAndTrimsInput(string searchText, string expectedName)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(GetTestHostUrl());

        await page.Locator("#neighborhood-search").FillAsync(searchText);

        var results = page.Locator("[data-neighborhood-result]:visible");
        Assert.Equal(1, await results.CountAsync());
        Assert.Contains(expectedName, await results.First.InnerTextAsync());
    }

    [Fact]
    public async Task NeighborhoodSearchShowsAllForEmptyInputAndNoMatchStatusForUnknownInput()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(GetTestHostUrl());

        var results = page.Locator("[data-neighborhood-result]:visible");
        Assert.Equal(2, await results.CountAsync());
        Assert.Contains("Over-the-Rhine", await page.Locator("#neighborhood-results").InnerTextAsync());
        Assert.Contains("Oakley", await page.Locator("#neighborhood-results").InnerTextAsync());

        await page.Locator("#neighborhood-search").FillAsync("   ");
        Assert.Equal(2, await results.CountAsync());

        await page.Locator("#neighborhood-search").FillAsync("abcdefgh123");
        Assert.Equal(0, await results.CountAsync());
        Assert.Equal("No neighborhoods found", await page.Locator("#neighborhood-status").InnerTextAsync());
    }

    [Fact]
    public async Task EditingNeighborhoodSearchClearsSelectedIdAndRequiresReselection()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(GetTestHostUrl());

        await page.Locator("[data-neighborhood-result]").Filter(new LocatorFilterOptions { HasText = "Oakley" }).ClickAsync();
        Assert.Equal("2", await page.Locator("#neighborhood-id").InputValueAsync());

        await page.Locator("#neighborhood-search").FillAsync("oakley");
        Assert.Equal("", await page.Locator("#neighborhood-id").InputValueAsync());

        await page.Locator("#BusinessType").SelectOptionAsync("Restaurant");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Analyze" }).ClickAsync();
        Assert.Matches(new Regex("/$"), page.Url);
        Assert.Contains("Neighborhood is required.", await page.Locator("#NeighborhoodId-validation").InnerTextAsync());
    }

    [Fact]
    public async Task NeighborhoodPickerSupportsKeyboardSelectionEscapeAndVisibleFocus()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(GetTestHostUrl());

        var search = page.Locator("#neighborhood-search");
        await search.FocusAsync();
        Assert.True(await HasVisibleFocusAsync(page, "#neighborhood-search"));

        await page.Keyboard.TypeAsync("o");
        await page.Keyboard.PressAsync("ArrowDown");
        Assert.Equal("neighborhood-result-1", await search.GetAttributeAsync("aria-activedescendant"));

        await page.Keyboard.PressAsync("Enter");
        Assert.Equal("1", await page.Locator("#neighborhood-id").InputValueAsync());
        Assert.Equal("Over-the-Rhine", await search.InputValueAsync());

        await search.FocusAsync();
        await page.Keyboard.PressAsync("Escape");
        Assert.Equal("false", await search.GetAttributeAsync("aria-expanded"));

        await search.FocusAsync();
        await page.Keyboard.TypeAsync(" ");
        Assert.Contains(" ", await search.InputValueAsync());

        await page.Locator("#BusinessType").FocusAsync();
        await page.Keyboard.PressAsync("Tab");
        Assert.Equal("neighborhood-search", await page.EvaluateAsync<string>("document.activeElement.id"));
    }

    [Fact]
    public async Task HomeControlsExposeLabelsAssociationsStatusAndContrast()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(GetTestHostUrl());

        Assert.Equal("BusinessType", await page.GetByLabel("Business type").GetAttributeAsync("id"));
        Assert.Equal("neighborhood-search", await page.GetByRole(AriaRole.Combobox, new PageGetByRoleOptions { Name = "Neighborhood" }).GetAttributeAsync("id"));
        Assert.Equal("combobox", await page.Locator("#neighborhood-search").GetAttributeAsync("role"));
        Assert.Equal("neighborhood-results", await page.Locator("#neighborhood-search").GetAttributeAsync("aria-controls"));
        Assert.Equal("listbox", await page.Locator("#neighborhood-results").GetAttributeAsync("role"));
        Assert.Equal("polite", await page.Locator("#neighborhood-status").GetAttributeAsync("aria-live"));
        Assert.Contains("NeighborhoodId-validation", await page.Locator("#neighborhood-search").GetAttributeAsync("aria-describedby") ?? string.Empty);

        await page.Locator("#BusinessType").SelectOptionAsync("Restaurant");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Analyze" }).ClickAsync();
        await page.Locator("#neighborhood-search").WaitForAsync();
        Assert.Contains("Neighborhood is required.", await page.Locator("#NeighborhoodId-validation").InnerTextAsync());
        Assert.Equal("NeighborhoodId", await page.Locator("#NeighborhoodId-validation").GetAttributeAsync("data-valmsg-for"));

        await page.Locator("[data-neighborhood-result]").Filter(new LocatorFilterOptions { HasText = "Oakley" }).ClickAsync();
        Assert.True(await ContrastRatioAsync(page, "#home-primary-form") >= 4.5);
        Assert.True(await ContrastRatioAsync(page, ".home-header") >= 3);
        Assert.True(await ContrastRatioAsync(page, ".home-submit") >= 3);
        Assert.True(await ContrastRatioAsync(page, "[data-neighborhood-result][aria-selected='true']") >= 3);
    }

    [Fact]
    public async Task HomeLayoutMatchesDesktopAndMobileAcceptanceBounds()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync(new BrowserNewPageOptions { ViewportSize = new ViewportSize { Width = 1366, Height = 768 } });
        await page.GotoAsync(GetTestHostUrl());

        Assert.Equal("rgb(10, 31, 68)", await page.Locator(".home-header").EvaluateAsync<string>("element => getComputedStyle(element).backgroundColor"));
        Assert.Equal("rgb(198, 40, 40)", await page.Locator(".home-submit").EvaluateAsync<string>("element => getComputedStyle(element).backgroundColor"));
        var desktopForm = await page.Locator("#home-primary-form").BoundingBoxAsync();
        Assert.NotNull(desktopForm);
        Assert.True(desktopForm!.Y + desktopForm.Height <= 768);
        Assert.True(await page.EvaluateAsync<int>("document.documentElement.scrollHeight") <= 768);

        await page.SetViewportSizeAsync(320, 768);
        Assert.True(await page.EvaluateAsync<int>("document.documentElement.scrollWidth") <= await page.EvaluateAsync<int>("document.documentElement.clientWidth"));
        Assert.True(await page.EvaluateAsync<int>("document.body.scrollWidth") <= await page.EvaluateAsync<int>("document.body.clientWidth"));
    }

    private static async Task<bool> HasVisibleFocusAsync(IPage page, string selector)
    {
        return await page.Locator(selector).EvaluateAsync<bool>("element => { const style = getComputedStyle(element); return style.outlineStyle !== 'none' || style.boxShadow !== 'none'; }");
    }

    private static async Task<double> ContrastRatioAsync(IPage page, string selector)
    {
        return await page.Locator(selector).EvaluateAsync<double>("element => { const parse = color => color.match(/[\\d.]+/g).map(Number).slice(0, 3).map(value => value / 255).map(value => value <= 0.03928 ? value / 12.92 : Math.pow((value + 0.055) / 1.055, 2.4)); const luminance = color => { const channels = parse(color); return channels[0] * 0.2126 + channels[1] * 0.7152 + channels[2] * 0.0722; }; const style = getComputedStyle(element); const foreground = luminance(style.color); const background = luminance(style.backgroundColor); return (Math.max(foreground, background) + 0.05) / (Math.min(foreground, background) + 0.05); }");
    }

    private static string GetTestHostUrl()
    {
        return Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_HOST") ?? "http://127.0.0.1:5000";
    }

    private static async Task<HttpResponseMessage> PostFormAsync(HttpClient client, string businessType)
    {
        using var homeResponse = await client.GetAsync("/");
        var homeMarkup = await homeResponse.Content.ReadAsStringAsync();
        var tokenMatch = Regex.Match(homeMarkup, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");

        Assert.True(tokenMatch.Success, "The Home page did not render an antiforgery token.");

        return await client.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = tokenMatch.Groups[1].Value,
            ["BusinessType"] = businessType,
            ["NeighborhoodId"] = "1"
        }));
    }
}