using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cincy_Biz_Advisor.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly IReadOnlyList<string> SupportedTypes =
        [
            "Restaurant",
            "Coffee Shop",
            "Retail Store",
            "Fitness Center"
        ];

        private readonly INeighborhoodRepository _neighborhoodRepository;
        private readonly ICityDataService _cityDataService;

        public IndexModel(
            INeighborhoodRepository neighborhoodRepository,
            ICityDataService cityDataService)
        {
            _neighborhoodRepository = neighborhoodRepository;
            _cityDataService = cityDataService;
        }

        public IReadOnlyList<string> SupportedBusinessTypes => SupportedTypes;

        public IReadOnlyList<Neighborhood> AvailableNeighborhoods { get; private set; } = [];

        public IReadOnlyList<NeighborhoodDashboardData> NeighborhoodDashboard { get; private set; } = [];

        public int ReportingYear { get; private set; }

        public int? TotalCrimeCount =>
            SumComplete(NeighborhoodDashboard.Select(item => item.CrimeCount));

        public int? TotalPermitCount =>
            SumComplete(NeighborhoodDashboard.Select(item => item.PermitCount));

        [BindProperty]
        public string? BusinessType { get; set; }

        [BindProperty]
        public int? NeighborhoodId { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            LoadAvailableNeighborhoods();
            await LoadDashboardAsync(cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            LoadAvailableNeighborhoods();

            if (string.IsNullOrWhiteSpace(BusinessType))
            {
                ModelState.AddModelError(nameof(BusinessType), "Business type is required.");
            }
            else if (!SupportedTypes.Contains(BusinessType, StringComparer.Ordinal))
            {
                ModelState.AddModelError(nameof(BusinessType), "Select a supported business type.");
            }

            Neighborhood? neighborhood = null;

            if (!NeighborhoodId.HasValue)
            {
                ModelState.AddModelError(nameof(NeighborhoodId), "Neighborhood is required.");
            }
            else
            {
                neighborhood = _neighborhoodRepository.GetById(NeighborhoodId.Value);
                if (neighborhood is null)
                {
                    ModelState.AddModelError(nameof(NeighborhoodId), "Select an available neighborhood.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadDashboardAsync(cancellationToken);
                return Page();
            }

            var queryString = QueryString.Create(new Dictionary<string, string?>
            {
                ["neighborhood"] = neighborhood!.Name,
                ["businessType"] = BusinessType
            });

            return Redirect($"/Analyze{queryString}");
        }

        private async Task LoadDashboardAsync(CancellationToken cancellationToken)
        {
            ReportingYear = DateTime.Today.Year - 1;
            var start = new DateOnly(ReportingYear, 1, 1);
            var end = new DateOnly(ReportingYear, 12, 31);
            var neighborhoods = CincinnatiNeighborhoods.All;
            var results = new NeighborhoodDashboardData[neighborhoods.Count];

            await Parallel.ForEachAsync(
                Enumerable.Range(0, neighborhoods.Count),
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = 4,
                    CancellationToken = cancellationToken
                },
                async (index, token) =>
                {
                    var neighborhood = neighborhoods[index];

                    var crimes = LoadCountAsync(() =>
                        _cityDataService.GetCrimeCountAsync(
                            neighborhood.QueryValue, start, end, token));

                    var permits = LoadCountAsync(() =>
                        _cityDataService.GetPermitCountAsync(
                            neighborhood.QueryValue, start, end, token));

                    await Task.WhenAll(crimes, permits);

                    results[index] = new NeighborhoodDashboardData(
                        neighborhood.DisplayName, await crimes, await permits);
                });

            NeighborhoodDashboard = results;
        }

        private static async Task<int?> LoadCountAsync(Func<Task<int>> loadCount)
        {
            try
            {
                return await loadCount();
            }
            catch (CityDataUnavailableException)
            {
                return null;
            }
        }

        private static int? SumComplete(IEnumerable<int?> counts)
        {
            var values = counts.ToArray();
            return values.Length > 0 && values.All(value => value.HasValue)
                ? values.Sum()
                : null;
        }

        private void LoadAvailableNeighborhoods()
        {
            AvailableNeighborhoods = _neighborhoodRepository.GetAvailableNeighborhoods();
        }

        public sealed record NeighborhoodDashboardData(
            string Name, int? CrimeCount, int? PermitCount);
    }
}