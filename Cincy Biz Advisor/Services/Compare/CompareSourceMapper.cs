using Cincy_Biz_Advisor.Models.Compare;

namespace Cincy_Biz_Advisor.Services.Compare;

public sealed class CompareSourceMapper
{
    public CompareProviderData Map(CompareSourceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return new CompareProviderData(
            MapMeasure(ComparisonMeasureType.Crime, snapshot.Crime),
            MapMeasure(ComparisonMeasureType.BuildingPermitActivity, snapshot.BuildingPermitActivity),
            MapMeasure(ComparisonMeasureType.LocationSuitability, snapshot.LocationSuitability));
    }

    private static ComparisonMeasure MapMeasure(
        ComparisonMeasureType expectedType,
        CompareSourceMeasure sourceMeasure)
    {
        ArgumentNullException.ThrowIfNull(sourceMeasure);

        if (sourceMeasure.MeasureType != expectedType)
        {
            throw new ArgumentException(
                $"The source measure type must be {expectedType}.",
                nameof(sourceMeasure));
        }

        return new ComparisonMeasure(
            expectedType,
            sourceMeasure.NeighborhoodAValue,
            sourceMeasure.NeighborhoodBValue,
            sourceMeasure.Availability,
            sourceMeasure.ReportingPeriod,
            sourceMeasure.SourceLabel,
            sourceMeasure.Meaning,
            sourceMeasure.Message);
    }
}
