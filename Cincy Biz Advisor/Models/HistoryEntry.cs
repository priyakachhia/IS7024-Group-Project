namespace Cincy_Biz_Advisor.Models;

public class HistoryEntry
{
    public int Id { get; set; }

    public string Neighborhood { get; set; } = string.Empty;

    public string BusinessType { get; set; } = string.Empty;

    public int ReportedCrimes { get; set; }

    public int BuildingPermits { get; set; }

    public int SafetyScore { get; set; }

    public int GrowthScore { get; set; }

    public int OverallScore { get; set; }

    public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
}