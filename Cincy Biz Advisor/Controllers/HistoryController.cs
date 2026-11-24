using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cincy_Biz_Advisor.Controllers;

[ApiController]
[Route("api/history")]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<HistoryEntry>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<HistoryEntry>>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<HistoryEntry> entries = await _historyService.GetAllAsync(cancellationToken);
        return Ok(entries);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HistoryEntry), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HistoryEntry>> Create(
        [FromBody] SaveHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var entry = new HistoryEntry
        {
            Neighborhood = request.Neighborhood.Trim(),
            BusinessType = request.BusinessType.Trim(),
            ReportedCrimes = request.ReportedCrimes,
            BuildingPermits = request.BuildingPermits,
            SafetyScore = request.SafetyScore,
            GrowthScore = request.GrowthScore,
            OverallScore = request.OverallScore
        };

        HistoryEntry saved = await _historyService.AddAsync(entry, cancellationToken);
        return Created($"/api/history/{saved.Id}", saved);
    }
}