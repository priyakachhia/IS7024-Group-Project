using Cincy_Biz_Advisor.Data;
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.Configure<CityDataOptions>(
    builder.Configuration.GetSection(CityDataOptions.SectionName));

builder.Services.AddHttpClient<ICityDataService, CityDataService>((serviceProvider, client) =>
{
    CityDataOptions options = serviceProvider.GetRequiredService<IOptions<CityDataOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

builder.Services.AddScoped<INeighborhoodAnalysisService, NeighborhoodAnalysisService>();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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