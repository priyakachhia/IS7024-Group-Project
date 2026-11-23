using Cincy_Biz_Advisor.Data;
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<INeighborhoodRepository, InMemoryNeighborhoodRepository>();

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

builder.Services.Configure<CityDataOptions>(
    builder.Configuration.GetSection(CityDataOptions.SectionName));

builder.Services.AddHttpClient<ICityDataService, CityDataService>((serviceProvider, client) =>
{
    CityDataOptions options = serviceProvider.GetRequiredService<IOptions<CityDataOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

builder.Services.AddScoped<INeighborhoodAnalysisService, NeighborhoodAnalysisService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var communityDb = scope.ServiceProvider.GetRequiredService<CommunityDbContext>();
    communityDb.Database.EnsureCreated();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

public partial class Program;