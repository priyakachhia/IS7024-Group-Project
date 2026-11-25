using Cincy_Biz_Advisor.Data;
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSingleton<INeighborhoodRepository, InMemoryNeighborhoodRepository>();

var sqlConnection = builder.Configuration.GetConnectionString("MetroBizAdvisor");
var useSqlServer = !string.IsNullOrWhiteSpace(sqlConnection);
if (useSqlServer)
{
    builder.Services.AddDbContext<HistoryDbContext>(options => options.UseSqlServer(sqlConnection));
    builder.Services.AddDbContext<CommunityDbContext>(options => options.UseSqlServer(sqlConnection));
}
else
{
    if (!string.IsNullOrWhiteSpace(builder.Configuration["WEBSITE_SITE_NAME"]))
    {
        throw new InvalidOperationException(
            "Set the MetroBizAdvisor connection string in Azure App Service before starting the app.");
    }

    var historyDbPath = Path.Combine(builder.Environment.ContentRootPath, "metrobiz-history.db");
    builder.Services.AddDbContext<HistoryDbContext>(options =>
        options.UseSqlite($"Data Source={historyDbPath}"));

    if (builder.Environment.IsDevelopment())
    {
        var communityDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MetroBizAdvisor");

        Directory.CreateDirectory(communityDataFolder);
        var communityDatabasePath = Path.Combine(communityDataFolder, "community.db");
        builder.Services.AddDbContext<CommunityDbContext>(options =>
            options.UseSqlite($"Data Source={communityDatabasePath}"));
    }
}

builder.Services.AddScoped<IHistoryService, HistoryService>();

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

// Azure SQL tables are provisioned by deploy/azure-sql-schema.sql.
if (!useSqlServer)
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<HistoryDbContext>().Database.EnsureCreated();
    if (app.Environment.IsDevelopment())
    {
        scope.ServiceProvider.GetRequiredService<CommunityDbContext>().Database.EnsureCreated();
    }
}

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
app.MapControllers();

app.Run();

public partial class Program;
