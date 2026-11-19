using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<INeighborhoodRepository, InMemoryNeighborhoodRepository>();

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

if (!app.Environment.IsDevelopment())
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