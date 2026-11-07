using Cincy_Biz_Advisor.Models.Compare;

namespace Cincy_Biz_Advisor.Services.Compare;

public sealed record CompareSourceSnapshot(
    CompareSourceMeasure Crime,
    CompareSourceMeasure BuildingPermitActivity,
    CompareSourceMeasure LocationSuitability);

public sealed record CompareSourceMeasure(
    ComparisonMeasureType MeasureType,
    string NeighborhoodAValue,
    string NeighborhoodBValue,
    ComparisonAvailability Availability,
    ReportingPeriod? ReportingPeriod = null,
    string? SourceLabel = null,
    string? Meaning = null,
    string? Message = null);
