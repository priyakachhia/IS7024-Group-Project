# Compare Neighborhoods Data Model

## Scope

This model describes the read-only data needed by the Compare page. It does not create a persistence schema, alter AnalysisHistory, or define the suitability calculation. Field names for source records are conceptual until the actual Cincinnati Open Data schemas are verified during implementation.

## Entities

### BusinessTypeOption

A canonical business type available for selection.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `Id` | canonical identifier | Yes | Must identify one available business type. |
| `Name` | text | Yes | Display name; must be non-empty. |
| `IsAvailable` | boolean | Yes | Only available values are selectable. |

### NeighborhoodOption

A canonical Cincinnati neighborhood available for selection.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `Id` | canonical identifier | Yes | Must be stable across source joins. |
| `Name` | text | Yes | Display name; must be non-empty. |
| `IsAvailable` | boolean | Yes | Only available values are selectable. |

### ComparisonRequest

The validated user input used to retrieve one comparison.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `BusinessTypeId` | canonical identifier | Yes | Must exist in the available business-type options. |
| `NeighborhoodAId` | canonical identifier | Yes | Must exist in the available neighborhood options. |
| `NeighborhoodBId` | canonical identifier | Yes | Must exist in the available neighborhood options and differ from `NeighborhoodAId`. |

### ReportingPeriod

The source period used to contextualize a metric.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `Start` | date/time or source period value | Conditional | Required when supplied by the source. |
| `End` | date/time or source period value | Conditional | Required when supplied by the source; must not precede `Start`. |
| `DisplayLabel` | text | Yes for display | Identifies the period without implying unsupported precision. |
| `IsComplete` | boolean | Yes | Only complete periods are eligible for crime/permit comparison. |

### CrimeComparisonMetric

The source-provided total reported-crime count for one neighborhood and the selected period.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `NeighborhoodId` | canonical identifier | Yes | Must match one request neighborhood. |
| `TotalCount` | non-negative number | Conditional | Present only when the source provides a valid total; never replaced with zero when absent. |
| `ReportingPeriod` | ReportingPeriod | Conditional | Identifies the latest complete common period when available. |
| `Availability` | availability state | Yes | Uses `Available`, `Unavailable`, or `SourceError`. |
| `SourceLabel` | text | Yes when available | Identifies the reported crime source. |

### PermitComparisonMetric

The source-provided total building-permit count for one neighborhood and the selected period.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `NeighborhoodId` | canonical identifier | Yes | Must match one request neighborhood. |
| `TotalCount` | non-negative number | Conditional | Present only when the source provides a valid total; never replaced with zero when absent. |
| `ReportingPeriod` | ReportingPeriod | Conditional | Identifies the latest complete common period when available. |
| `Availability` | availability state | Yes | Uses `Available`, `Unavailable`, or `SourceError`. |
| `SourceLabel` | text | Yes when available | Identifies the building permits source. |

### SuitabilityComparisonMetric

The existing suitability score or result for one canonical business type and neighborhood.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `BusinessTypeId` | canonical identifier | Yes | Must match the request business type. |
| `NeighborhoodId` | canonical identifier | Yes | Must match one request neighborhood. |
| `ScoreOrDisplayValue` | source-defined value | Conditional | Present only when an existing result is available. |
| `Availability` | availability state | Yes | Missing existing results are `Unavailable`; no calculation is performed. |
| `SourceLabel` | text | Yes when available | Identifies the existing suitability result source. |
| `Meaning` | text | Conditional | Describes the source-defined score meaning when available. |
| `ReportingPeriod` | ReportingPeriod | Conditional | Preserved when supplied by the suitability source. |

### ComparisonMeasure

A presentation-neutral measure group used to render one chart and one table row.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `MeasureType` | enum-like value | Yes | Exactly one of `Crime`, `BuildingPermitActivity`, or `LocationSuitability`. |
| `NeighborhoodAValue` | source-backed display value | Conditional | Value or N/A state for neighborhood A. |
| `NeighborhoodBValue` | source-backed display value | Conditional | Value or N/A state for neighborhood B. |
| `Availability` | availability state | Yes | Must reflect the underlying source values; `Available` requires both requested neighborhood values, while `Unavailable` or `SourceError` identifies the affected measure. |
| `ReportingPeriod` | ReportingPeriod | Conditional | Displayed when supplied. |
| `SourceLabel` | text | Conditional | Displayed when supplied. |
| `Meaning` | text | Conditional | Preserves source-defined meaning, especially for suitability. |

### NeighborhoodComparisonResult

The complete read-only result for a validated request.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `Request` | ComparisonRequest | Yes | Contains one business type and two distinct neighborhoods. |
| `BusinessType` | BusinessTypeOption | Yes | The selected canonical type. |
| `NeighborhoodA` | NeighborhoodOption | Yes | First selected neighborhood. |
| `NeighborhoodB` | NeighborhoodOption | Yes | Second selected neighborhood; differs from A. |
| `Measures` | collection of ComparisonMeasure | Yes | Contains exactly three measure groups, one for each requested chart/table row. |
| `HasPartialData` | boolean | Yes | True only when at least one measure is available and at least one other measure is unavailable or has a source error. |
| `Message` | text | Conditional | User-facing source or availability explanation when needed. |

## Relationships

- One `BusinessTypeOption` can be associated with many `SuitabilityComparisonMetric` records.
- One `NeighborhoodOption` can be associated with many crime, permit, and suitability metric records.
- One `ComparisonRequest` references exactly one business type and exactly two distinct neighborhoods.
- One `NeighborhoodComparisonResult` contains exactly three `ComparisonMeasure` groups.
- Each `ComparisonMeasure` maps to the two requested neighborhoods and one source metric type.
- A comparison result is read-only and has no persistence relationship to AnalysisHistory.

## Availability States

| State | Meaning | Display behavior |
|-------|---------|------------------|
| `Available` | A valid source-backed value exists. | Display the value, source label, and period when supplied. |
| `Unavailable` | No matching source value exists for the selected identity or period. | Display `N/A` and explain which measure is unavailable. Do not use zero or an estimate. |
| `SourceError` | The source could not be read or returned an invalid response. | Display `N/A` with a user-friendly source-unavailable message; retain other available measures. |
| `InvalidRequest` | Required selection is missing, unsupported, or neighborhoods are identical. | Do not produce a result; display validation feedback beside the relevant selection. |

## Validation Rules

1. Business type must be selected from the canonical available business-type options.
2. Both neighborhoods must be selected from the canonical available neighborhood options.
3. Neighborhood A and neighborhood B must have different canonical identifiers.
4. Crime and permit values must use source-provided totals from the latest complete common reporting period.
5. A missing metric is represented as `Unavailable`, not as zero, an estimate, or a derived value.
6. Existing suitability values are read-only source results for the selected business type and neighborhood; no Compare calculation is permitted.
7. A partial result is valid when at least one requested measure is available; missing measures remain visible as `N/A`.
8. The result always includes the three requested measure groups, even when one or more groups are unavailable.
9. Source period and source label are preserved whenever the source provides them.
10. The model contains no Analyze-only trend series, scoring inputs, or calculation fields.

## State Flow

```text
NoRequest
  -> InvalidRequest (missing/unsupported selection or duplicate neighborhoods)
  -> ValidatedRequest
  -> Loading
	 -> CompleteResult (all three measures available)
   -> PartialResult (at least one measure available; another is unavailable or source-error)
   -> SourceFailure (no usable source response; represented to the user as N/A/source unavailable)
```

`CompleteResult`, `PartialResult`, and `SourceFailure` are mutually exclusive display outcomes from `Loading`. `SourceFailure` is not a transition from `PartialResult`. The Compare page never transitions a result into a saved or calculated suitability state.

`InvalidRequest` is a validation outcome before source access. `SourceFailure` is used only when no usable response is available for the comparison; if at least one measure is available, the result is `PartialResult` and each failed measure retains `SourceError`. Every displayed available measure must retain its source label and reporting period when supplied; no state may replace missing values with zero, estimates, or recalculated suitability.
