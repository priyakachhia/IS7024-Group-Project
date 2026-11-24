namespace Cincy_Biz_Advisor.Models;

/// <summary>
/// The result of analyzing one neighborhood for one business type over a time period.
/// </summary>
public sealed record NeighborhoodAnalysis
{
    public required string Neighborhood { get; init; }
    public required BusinessType BusinessType { get; init; }
    public required DateOnly PeriodStart { get; init; }
    public required DateOnly PeriodEnd { get; init; }

    /// <summary>Number of reported crimes in the period.</summary>
    public required int CrimeCount { get; init; }

    /// <summary>Number of building permits issued in the period.</summary>
    public required int PermitCount { get; init; }

    /// <summary>0–100. Higher means fewer reported crimes.</summary>
    public required decimal SafetyScore { get; init; }

    /// <summary>0–100. Higher means more building activity.</summary>
    public required decimal GrowthScore { get; init; }

    /// <summary>0–100. Safety and growth combined using the business type's weights.</summary>
    public required int SuitabilityScore { get; init; }

    public string BusinessTypeName => BusinessType.ToDisplayName();
}
