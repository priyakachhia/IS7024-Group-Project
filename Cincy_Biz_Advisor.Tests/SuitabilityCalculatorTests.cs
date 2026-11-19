using Cincy_Biz_Advisor.Models;
using Cincy_Biz_Advisor.Services;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

/// <summary>Unit tests built directly from the Given/When/Then examples in the README.</summary>
public class SuitabilityCalculatorTests
{
    [Theory]
    [InlineData(412, 87, BusinessType.Restaurant, 72)]   // 101.1 Over-the-Rhine, Restaurant
    [InlineData(138, 41, BusinessType.CoffeeShop, 64)]   // 101.2 Oakley, Coffee Shop
    [InlineData(138, 41, BusinessType.Restaurant, 58)]   // 102.1 Oakley, Restaurant
    [InlineData(5000, 500, BusinessType.Retail, 30)]     // Edge: safety bottoms out at 0, growth caps at 100
    [InlineData(0, 0, BusinessType.Retail, 70)]          // Edge: no crime, no permits
    public void CalculateScore_MatchesReadmeExamples(int crimes, int permits, BusinessType businessType, int expectedScore)
    {
        int score = SuitabilityCalculator.CalculateScore(crimes, permits, businessType);

        Assert.Equal(expectedScore, score);
    }

    [Fact]
    public void CalculateSafety_NeverGoesBelowZero()
    {
        Assert.Equal(0m, SuitabilityCalculator.CalculateSafety(5000));
        Assert.Equal(0m, SuitabilityCalculator.CalculateSafety(800));   // 800 / 8 = 100 -> exactly 0
    }

    [Fact]
    public void CalculateGrowth_NeverGoesAboveOneHundred()
    {
        Assert.Equal(100m, SuitabilityCalculator.CalculateGrowth(250));
    }

    [Fact]
    public void CalculateScore_NegativeCounts_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SuitabilityCalculator.CalculateScore(-1, 10, BusinessType.Retail));
        Assert.Throws<ArgumentOutOfRangeException>(() => SuitabilityCalculator.CalculateScore(10, -1, BusinessType.Retail));
    }
}
