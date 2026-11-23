using System.ComponentModel.DataAnnotations;
using Cincy_Biz_Advisor.Data;
using Cincy_Biz_Advisor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cincy_Biz_Advisor.Pages;

public class CommunityModel : PageModel
{
    private readonly CommunityDbContext? _db;

    public CommunityModel(CommunityDbContext? db = null)
    {
        _db = db;
    }

    public IReadOnlyList<string> BusinessTypes { get; } =
        ["Restaurant", "Coffee Shop", "Retail Store", "Fitness Center"];

    public IReadOnlyList<string> Categories { get; } =
        ["Discussion", "Seeking Partner", "Seeking Co-founder"];

    public IReadOnlyList<CommunityPost> Posts { get; private set; } = [];

    public bool CanPublish => _db is not null;

    [BindProperty]
    public PostDraft Draft { get; set; } = new();

    public bool ShowPreview { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadPostsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostPreviewAsync(CancellationToken cancellationToken)
    {
        ValidateDraft();
        ShowPreview = ModelState.IsValid;

        await LoadPostsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostPublishAsync(CancellationToken cancellationToken)
    {
        if (_db is null)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        var neighborhood = ValidateDraft();
        if (neighborhood is null || !ModelState.IsValid)
        {
            await LoadPostsAsync(cancellationToken);
            return Page();
        }

        var post = new CommunityPost
        {
            Title = Draft.Title!.Trim(),
            Neighborhood = neighborhood.DisplayName,
            BusinessType = Draft.BusinessType!,
            Category = Draft.Category!,
            Message = Draft.Message!.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.CommunityPosts.Add(post);
        await _db.SaveChangesAsync(cancellationToken);

        return RedirectToPage();
    }

    private Neighborhood? ValidateDraft()
    {
        var neighborhood = CincinnatiNeighborhoods.Find(Draft.Neighborhood);
        if (neighborhood is null)
        {
            ModelState.AddModelError("Draft.Neighborhood", "Select a neighborhood.");
        }

        if (Draft.BusinessType is null ||
            !BusinessTypes.Contains(Draft.BusinessType, StringComparer.Ordinal))
        {
            ModelState.AddModelError("Draft.BusinessType", "Select a business type.");
        }

        if (Draft.Category is null ||
            !Categories.Contains(Draft.Category, StringComparer.Ordinal))
        {
            ModelState.AddModelError("Draft.Category", "Select a category.");
        }

        return neighborhood;
    }

    private async Task LoadPostsAsync(CancellationToken cancellationToken)
    {
        if (_db is null)
        {
            return;
        }

        Posts = await _db.CommunityPosts
            .AsNoTracking()
            .OrderByDescending(post => post.CreatedAtUtc)
            .ThenByDescending(post => post.Id)
            .ToListAsync(cancellationToken);
    }

    public class PostDraft
    {
        [Required(ErrorMessage = "Enter a title.")]
        [StringLength(120)]
        public string? Title { get; set; }

        public string? Neighborhood { get; set; }
        public string? BusinessType { get; set; }
        public string? Category { get; set; }

        [Required(ErrorMessage = "Enter a message.")]
        [StringLength(2000)]
        public string? Message { get; set; }
    }
}