using Cincy_Biz_Advisor.Services;
using Cincy_Biz_Advisor.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Cincy_Biz_Advisor.Tests.Pages;

public sealed class IndexPageModelTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IndexPageModelTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void HomePageModelCanReceiveNeighborhoodRepositoryThroughServiceContainer()
    {
        using var scope = _factory.Services.CreateScope();

        var pageModel = ActivatorUtilities.CreateInstance<IndexModel>(scope.ServiceProvider);

        Assert.NotNull(pageModel);
    }

    [Fact]
    public void OnGetExposesExactlyTheSupportedBusinessTypesAndNeighborhoods()
    {
        var pageModel = CreatePageModel();

        pageModel.OnGet();

        Assert.Equal(
            ["Restaurant", "Coffee Shop", "Retail Store", "Fitness Center"],
            pageModel.SupportedBusinessTypes);
        Assert.Equal(["Over-the-Rhine", "Oakley"], pageModel.AvailableNeighborhoods.Select(neighborhood => neighborhood.Name));
    }

    [Fact]
    public void OnPostRequiresBusinessType()
    {
        var pageModel = CreatePageModel();
        pageModel.NeighborhoodId = 1;

        var result = pageModel.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.NotEmpty(pageModel.ModelState[nameof(pageModel.BusinessType)]!.Errors);
    }

    [Fact]
    public void OnPostRequiresNeighborhood()
    {
        var pageModel = CreatePageModel();
        pageModel.BusinessType = "Restaurant";

        var result = pageModel.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.NotEmpty(pageModel.ModelState[nameof(pageModel.NeighborhoodId)]!.Errors);
    }

    [Theory]
    [InlineData("Unsupported Business", 1)]
    [InlineData("Restaurant", 999)]
    public void OnPostRejectsUnsupportedSubmittedValues(string businessType, int neighborhoodId)
    {
        var pageModel = CreatePageModel();
        pageModel.BusinessType = businessType;
        pageModel.NeighborhoodId = neighborhoodId;

        var result = pageModel.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(result is RedirectToPageResult);
        Assert.NotEmpty(pageModel.ModelState.Values.SelectMany(entry => entry.Errors));
    }

    private static IndexModel CreatePageModel()
    {
        return new IndexModel(new InMemoryNeighborhoodRepository());
    }
}
