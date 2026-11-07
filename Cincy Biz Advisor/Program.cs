
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Compare Neighborhoods services
builder.Services.AddSingleton<
    Cincy_Biz_Advisor.Services.Compare.ICompareCatalogProvider,
    Cincy_Biz_Advisor.Services.Compare.CompareCatalogProvider>();

builder.Services.AddScoped<
    Cincy_Biz_Advisor.Services.Compare.ICompareDataProvider,
    Cincy_Biz_Advisor.Services.Compare.AnalyzeCompareDataProvider>();

builder.Services.AddScoped<
    Cincy_Biz_Advisor.Services.Compare.NeighborhoodComparisonService>();

// Analyze Neighborhood services
builder.Services.Configure<CityDataOptions>(
    builder.Configuration.GetSection(CityDataOptions.SectionName));

builder.Services.AddHttpClient<ICityDataService, CityDataService>(
    (serviceProvider, client) =>
    {
        CityDataOptions options = serviceProvider
            .GetRequiredService<IOptions<CityDataOptions>>().Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    });

builder.Services.AddScoped<
    INeighborhoodAnalysisService,
    NeighborhoodAnalysisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
