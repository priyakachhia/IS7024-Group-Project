# Compare Data Contract

## Purpose

Define the read-only boundary between the Compare page and source-backed data providers. The contract uses canonical identifiers and availability states so the page does not invent metrics or perform suitability calculations.

## Option Data

### Business Type Options

```text
BusinessTypeOption
- id: canonical identifier
- name: display text
- isAvailable: boolean
```

Only records with `isAvailable = true` are presented as selectable options.

### Neighborhood Options

```text
NeighborhoodOption
- id: canonical identifier
- name: display text
- isAvailable: boolean
```

Only records with `isAvailable = true` are presented as selectable options. The identifier must be usable as the join identity for all source metrics.

## Comparison Request

```text
ComparisonRequest
- businessTypeId: canonical identifier
- neighborhoodAId: canonical identifier
- neighborhoodBId: canonical identifier
```

The request is valid only when all identifiers exist in the canonical option sets and the two neighborhood identifiers differ.

## Comparison Response

```text
NeighborhoodComparisonResult
- businessType: BusinessTypeOption
- neighborhoodA: NeighborhoodOption
- neighborhoodB: NeighborhoodOption
- measures: exactly three ComparisonMeasure records
- hasPartialData: boolean
- message: optional user-facing availability message
```

Each `ComparisonMeasure` contains:

```text
ComparisonMeasure
- measureType: Crime | BuildingPermitActivity | LocationSuitability
- neighborhoodAValue: source-backed display value or N/A
- neighborhoodBValue: source-backed display value or N/A
- availability: Available | Unavailable | SourceError
- reportingPeriod: optional source period
- sourceLabel: optional source label
- meaning: optional source-defined meaning
- sourceFieldMapping: verified source field or provider contract reference
```

## Source Mapping Rules

- `Crime` maps to the source-provided total reported-crime count for the latest complete common reporting period.
- `BuildingPermitActivity` maps to the source-provided total permit count for the latest complete common reporting period.
- `LocationSuitability` maps to the existing suitability result for the selected canonical business type and neighborhood.
- Crime and permit adapters must document the verified endpoint/access method, response field, neighborhood join key, period field, completeness rule, and source-provided total field before implementation.
- The common period is the latest period that is complete and comparable for both requested neighborhoods and both crime/permit sources; if no common complete period exists for a measure, that measure is `Unavailable` with source context.
- The suitability adapter must consume the provider-owned result and meaning using canonical identifiers and must not read or reproduce scoring inputs, formulas, or persistence.
- No response value may be generated from a new formula, estimate, or fallback constant.
- Missing values map to `Unavailable` and render as `N/A`.
- A source read or source payload failure maps to `SourceError` and renders as `N/A` with a user-friendly explanation.
- Partial responses are valid when at least one measure is available.

## Error Contract

| Condition | Page behavior |
|-----------|---------------|
| Missing selection | Field-level validation; do not query comparison sources. |
| Identical neighborhoods | Validation message requiring two different neighborhoods; do not query comparison sources. |
| Unsupported canonical identifier | Validation message; do not produce a result. |
| Missing source record | Return the affected measure as `Unavailable`; retain other measures. |
| Source unavailable or invalid | Return the affected measure as `SourceError`; retain other measures. |
| No usable source result | Show a user-friendly unavailable message and no fabricated result values. |

## Ownership Boundary

The Compare data contract is read-only. It does not expose suitability inputs, weights, formulas, trend calculations, or persistence operations owned by Analyze or History. If a teammate-owned suitability provider exists, Compare consumes only its existing result and source metadata.

## Source Readiness and Coordination

Actual source endpoints, schemas, neighborhood join keys, and suitability-provider ownership must be verified before adapters are implemented. The implementation must not infer or invent these values. Any teammate-owned suitability contract or shared source configuration requires coordination before Compare service registration or integration testing.

## Current Implementation Status

The initial committed implementation provides the Compare model contracts, canonical catalog, read-only provider boundary, orchestration service, and an unavailable provider. The unavailable provider returns `N/A`/unavailable states for crime, building permits, and suitability rather than fabricated values. Verified source adapters, common-period mapping, source-error mapping, and reuse of the existing suitability provider remain pending and are still governed by the contract above.
