using System.ComponentModel.DataAnnotations;

namespace Cincy_Biz_Advisor.Models;

/// <summary>
/// The kinds of business a user can analyze a neighborhood for.
/// Each type weighs safety and growth differently (see SuitabilityCalculator).
/// </summary>
public enum BusinessType
{
    [Display(Name = "Restaurant")]
    Restaurant,

    [Display(Name = "Coffee Shop")]
    CoffeeShop,

    [Display(Name = "Retail")]
    Retail
}

public static class BusinessTypeExtensions
{
    /// <summary>Returns the friendly name shown to users, for example "Coffee Shop".</summary>
    public static string ToDisplayName(this BusinessType businessType) => businessType switch
    {
        BusinessType.CoffeeShop => "Coffee Shop",
        _ => businessType.ToString()
    };
}
