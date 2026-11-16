using Cincy_Biz_Advisor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Cincy_Biz_Advisor.Services;

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

        public IndexModel(INeighborhoodRepository neighborhoodRepository)
        {
            _neighborhoodRepository = neighborhoodRepository;
        }

        public IReadOnlyList<string> SupportedBusinessTypes => SupportedTypes;

        public IReadOnlyList<Neighborhood> AvailableNeighborhoods { get; private set; } = [];

        [BindProperty]
        public string? BusinessType { get; set; }

        [BindProperty]
        public int? NeighborhoodId { get; set; }

        public void OnGet()
        {
            LoadAvailableNeighborhoods();
        }

        public IActionResult OnPost()
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
                return Page();
            }

            var queryString = QueryString.Create(new Dictionary<string, string?>
            {
                ["neighborhood"] = neighborhood!.Name,
                ["businessType"] = BusinessType
            });

            return Redirect($"/Analyze{queryString}");
        }

        private void LoadAvailableNeighborhoods()
        {
            AvailableNeighborhoods = _neighborhoodRepository.GetAvailableNeighborhoods();
        }
    }
}
