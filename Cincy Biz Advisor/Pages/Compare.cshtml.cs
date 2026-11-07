using System.ComponentModel.DataAnnotations;
using Cincy_Biz_Advisor.Models.Compare;
using Cincy_Biz_Advisor.Services.Compare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cincy_Biz_Advisor.Pages;

public sealed class CompareModel : PageModel
{
    private readonly ICompareCatalogProvider _catalogProvider;
    private readonly NeighborhoodComparisonService _comparisonService;

    public CompareModel(
        ICompareCatalogProvider catalogProvider,
        NeighborhoodComparisonService comparisonService)
    {
        _catalogProvider = catalogProvider;
        _comparisonService = comparisonService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Select a business type.")]
    public string? BusinessTypeId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Select the first neighborhood.")]
    public string? NeighborhoodAId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Select the second neighborhood.")]
    public string? NeighborhoodBId { get; set; }

    public IReadOnlyList<BusinessTypeOption> BusinessTypes => _catalogProvider.GetBusinessTypes();
    public IReadOnlyList<NeighborhoodOption> Neighborhoods => _catalogProvider.GetNeighborhoods();
    public NeighborhoodComparisonResult? Result { get; private set; }
    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(NeighborhoodAId) &&
            string.Equals(NeighborhoodAId, NeighborhoodBId, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(NeighborhoodBId), "Select two different neighborhoods.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            Result = await _comparisonService.CompareAsync(
                new ComparisonRequest(BusinessTypeId!, NeighborhoodAId!, NeighborhoodBId!),
                cancellationToken);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return Page();
    }
}
