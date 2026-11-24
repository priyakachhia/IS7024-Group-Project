using System.ComponentModel.DataAnnotations;

namespace Cincy_Biz_Advisor.Models;

public class SaveHistoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Neighborhood { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string BusinessType { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int ReportedCrimes { get; set; }

    [Range(0, int.MaxValue)]
    public int BuildingPermits { get; set; }

    [Range(0, 100)]
    public double SafetyScore { get; set; }

    [Range(0, 100)]
    public double GrowthScore { get; set; }

    [Range(0, 100)]
    public int OverallScore { get; set; }
}