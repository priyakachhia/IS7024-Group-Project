using Cincy_Biz_Advisor.Models;

namespace Cincy_Biz_Advisor.Services;

/// <summary>
/// Turns crime and permit counts into a 0–100 suitability score.
///
///   Safety = 100 − (crimes ÷ 8), minimum 0
///   Growth = permits, maximum 100
///   Score  = Safety × safetyWeight + Growth × (1 − safetyWeight), rounded to a whole number
///
/// Safety weights: Restaurant 0.4, Coffee Shop 0.55, Retail 0.7 (growth gets the rest).
/// These values reproduce the scores in the README examples 101.1, 101.2 and 102.1.
/// </summary>
public static class SuitabilityCalculator
{
    public static decimal CalculateSafety(int crimeCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(crimeCount);
        return Math.Max(0m, 100m - crimeCount / 8m);
    }

    public static decimal CalculateGrowth(int permitCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(permitCount);
        return Math.Min(100m, permitCount);
    }

    public static decimal GetSafetyWeight(BusinessType businessType) => businessType switch
    {
        BusinessType.Restaurant => 0.4m,
        BusinessType.CoffeeShop => 0.55m,
        BusinessType.Retail => 0.7m,
        _ => throw new ArgumentOutOfRangeException(nameof(businessType), businessType, "Unknown business type.")
    };

    public static int CalculateScore(int crimeCount, int permitCount, BusinessType businessType)
    {
        decimal safety = CalculateSafety(crimeCount);
        decimal growth = CalculateGrowth(permitCount);
        decimal safetyWeight = GetSafetyWeight(businessType);

        decimal score = safety * safetyWeight + growth * (1m - safetyWeight);
        return (int)Math.Round(score, MidpointRounding.AwayFromZero);
    }
}
