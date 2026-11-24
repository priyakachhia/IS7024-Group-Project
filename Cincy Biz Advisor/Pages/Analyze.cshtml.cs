using System.ComponentModel.DataAnnotations;
using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cincy_Biz_Advisor.Pages;

/// <summary>
/// Requirement 101.0: Analyze a Neighborhood.
/// GET shows the empty form; POST runs the analysis and shows the results on the same page.
/// </summary>
public class AnalyzeModel : PageModel
{
    public const string SelectNeighborhoodMessage = "Please select a neighborhood";

    private readonly INeighborhoodAnalysisService _analysisService;
    private readonly ILogger<AnalyzeModel> _logger;

    public AnalyzeModel(INeighborhoodAnalysisService analysisService, ILogger<AnalyzeModel> logger)
    {
        _analysisService = analysisService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = SelectNeighborhoodMessage)]
    [Display(Name = "Neighborhood")]
    public string? Neighborhood { get; set; }

    [BindProperty]
    [Display(Name = "Business type")]
    public BusinessType BusinessType { get; set; } = BusinessType.Restaurant;

    [BindProperty]
    [DataType(DataType.Date)]
    [Display(Name = "From")]
    public DateTime From { get; set; }

    [BindProperty]
    [DataType(DataType.Date)]
    [Display(Name = "To")]
    public DateTime To { get; set; }

    public IReadOnlyList<Neighborhood> Neighborhoods => CincinnatiNeighborhoods.All;

    /// <summary>The analysis to show. Null until the user runs one successfully.</summary>
    public NeighborhoodAnalysis? Result { get; private set; }

    /// <summary>A friendly error to show when city data cannot be loaded.</summary>
    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
        // Default period: the last full calendar year (in 2026 that is January 1 to December 31, 2025).
        int lastYear = DateTime.Today.Year - 1;
        From = new DateTime(lastYear, 1, 1);
        To = new DateTime(lastYear, 12, 31);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(Neighborhood) && CincinnatiNeighborhoods.Find(Neighborhood) is null)
        {
            ModelState.AddModelError(nameof(Neighborhood), SelectNeighborhoodMessage);
        }

        if (From.Date > To.Date)
        {
            ModelState.AddModelError(nameof(To), "The end date must be on or after the start date");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            Result = await _analysisService.AnalyzeAsync(
                Neighborhood!,
                BusinessType,
                DateOnly.FromDateTime(From),
                DateOnly.FromDateTime(To),
                cancellationToken);
        }
        catch (CityDataUnavailableException ex)
        {
            _logger.LogWarning(ex, "Analysis failed for {Neighborhood} because {Dataset} data was unavailable.",
                Neighborhood, ex.DatasetName);
            ErrorMessage = ex.UserMessage;
        }

        return Page();
    }
}
