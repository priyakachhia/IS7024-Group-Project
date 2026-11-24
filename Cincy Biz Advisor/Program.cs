using Cincy_Biz_Advisor.Data;
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSingleton<INeighborhoodRepository, InMemoryNeighborhoodRepository>();

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


// Local SQLite database for saved analysis history (created automatically).
string historyDbPath = Path.Combine(builder.Environment.ContentRootPath, "metrobiz-history.db");
builder.Services.AddDbContext<HistoryDbContext>(options =>
    options.UseSqlite($"Data Source={historyDbPath}"));
builder.Services.AddScoped<IHistoryService, HistoryService>();

var app = builder.Build();

// Create the history database and table on startup if they do not exist.
using (IServiceScope scope = app.Services.CreateScope())
{
    HistoryDbContext db = scope.ServiceProvider.GetRequiredService<HistoryDbContext>();
    db.Database.EnsureCreated();
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
