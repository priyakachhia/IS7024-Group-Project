using Cincy_Biz_Advisor.Models.Compare;
using Cincy_Biz_Advisor.Services.Compare;
using Xunit;

namespace Cincy_Biz_Advisor.Tests;

public sealed class CompareSourceMapperTests
{
    private static readonly ReportingPeriod CommonPeriod =
        new("January 1, 2025 – December 31, 2025", new DateOnly(2025, 1, 1), new DateOnly(2025, 12, 31));

    [Fact]
    public void Map_PreservesAvailableValuesAndMetadata()
    {
        var mapper = new CompareSourceMapper();
        CompareSourceSnapshot snapshot = CreateSnapshot(
            CreateMeasure(ComparisonMeasureType.Crime, "12", "7", ComparisonAvailability.Available, "Cincinnati Crime", "Reported incidents"),
            CreateMeasure(ComparisonMeasureType.BuildingPermitActivity, "4", "9", ComparisonAvailability.Available, "Cincinnati Permits", "Issued permits"),
            CreateMeasure(ComparisonMeasureType.LocationSuitability, "81", "74", ComparisonAvailability.Available, "Analyze suitability", "Existing suitability result"));

        CompareProviderData result = mapper.Map(snapshot);

        Assert.Equal("12", result.Crime.NeighborhoodAValue);
        Assert.Equal("7", result.Crime.NeighborhoodBValue);
        Assert.Equal(ComparisonAvailability.Available, result.Crime.Availability);
        Assert.Equal("Cincinnati Crime", result.Crime.SourceLabel);
        Assert.Equal("Reported incidents", result.Crime.Meaning);
        Assert.Equal(CommonPeriod, result.Crime.ReportingPeriod);
        Assert.Equal("81", result.LocationSuitability.NeighborhoodAValue);
        Assert.Equal("74", result.LocationSuitability.NeighborhoodBValue);
    }

    [Fact]
    public void Map_PreservesVerifiedZeroAsAvailable()
    {
        var mapper = new CompareSourceMapper();
        CompareSourceSnapshot snapshot = CreateSnapshot(
            CreateMeasure(ComparisonMeasureType.Crime, "0", "0", ComparisonAvailability.Available),
            CreateMeasure(ComparisonMeasureType.BuildingPermitActivity, "0", "3", ComparisonAvailability.Available),
            CreateMeasure(ComparisonMeasureType.LocationSuitability, "0", "0", ComparisonAvailability.Available));

        CompareProviderData result = mapper.Map(snapshot);

        Assert.Equal("0", result.Crime.NeighborhoodAValue);
        Assert.Equal("0", result.Crime.NeighborhoodBValue);
        Assert.Equal(ComparisonAvailability.Available, result.Crime.Availability);
        Assert.Equal(ComparisonAvailability.Available, result.LocationSuitability.Availability);
    }

    [Fact]
    public void Map_PreservesUnavailableAndSourceErrorStatesForPartialResults()
    {
        var mapper = new CompareSourceMapper();
        CompareSourceSnapshot snapshot = CreateSnapshot(
            CreateMeasure(ComparisonMeasureType.Crime, "12", "N/A", ComparisonAvailability.Unavailable, message: "One neighborhood has no crime record."),
            CreateMeasure(ComparisonMeasureType.BuildingPermitActivity, "4", "9", ComparisonAvailability.Available),
            CreateMeasure(ComparisonMeasureType.LocationSuitability, "N/A", "N/A", ComparisonAvailability.SourceError, message: "Suitability source failed."));

        CompareProviderData result = mapper.Map(snapshot);

        Assert.Equal(ComparisonAvailability.Unavailable, result.Crime.Availability);
        Assert.Equal("One neighborhood has no crime record.", result.Crime.Message);
        Assert.Equal(ComparisonAvailability.Available, result.BuildingPermitActivity.Availability);
        Assert.Equal(ComparisonAvailability.SourceError, result.LocationSuitability.Availability);
    }

    [Fact]
    public void Map_RejectsMeasureInUnexpectedPosition()
    {
        var mapper = new CompareSourceMapper();
        CompareSourceSnapshot snapshot = CreateSnapshot(
            CreateMeasure(ComparisonMeasureType.BuildingPermitActivity, "1", "2", ComparisonAvailability.Available),
            CreateMeasure(ComparisonMeasureType.Crime, "3", "4", ComparisonAvailability.Available),
            CreateMeasure(ComparisonMeasureType.LocationSuitability, "5", "6", ComparisonAvailability.Available));

        Assert.Throws<ArgumentException>(() => mapper.Map(snapshot));
    }

    private static CompareSourceSnapshot CreateSnapshot(
        CompareSourceMeasure crime,
        CompareSourceMeasure permits,
        CompareSourceMeasure suitability) =>
        new(crime, permits, suitability);

    private static CompareSourceMeasure CreateMeasure(
        ComparisonMeasureType type,
        string neighborhoodAValue,
        string neighborhoodBValue,
        ComparisonAvailability availability,
        string? sourceLabel = null,
        string? meaning = null,
        string? message = null) =>
        new(
            type,
            neighborhoodAValue,
            neighborhoodBValue,
            availability,
            CommonPeriod,
            sourceLabel,
            meaning,
            message);
}
