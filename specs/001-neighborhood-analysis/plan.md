# Implementation Plan: Neighborhood Analysis

**Branch**: `001-neighborhood-analysis` | **Date**: 2026-11-05 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-neighborhood-analysis/spec.md`

## Summary

Implement Requirement 101 as a Razor Pages analysis flow. `Pages/Analyze.cshtml` and `AnalyzeModel` will accept a neighborhood, business type, and inclusive date range through a POST form, call a concurrent application service for crime and permit counts, calculate a bounded suitability score with a pure calculator, and render results or accessible errors on the same page. A typed Socrata client will construct count queries from `CityDataOptions`, with optional JSON endpoint support sharing the same application service.

## Technical Context

**Language/Version**: C# with .NET 10 / ASP.NET Core Razor Pages

**Primary Dependencies**: ASP.NET Core Razor Pages, `HttpClientFactory`, options binding, xUnit

**Storage**: N/A; results are calculated per request and not persisted

**Testing**: xUnit unit, service, Razor Page, and optional endpoint tests with a fake `HttpMessageHandler`

**Target Platform**: .NET 10 web application hosting the existing Razor Pages project

**Project Type**: Single-project web application with an optional JSON API surface

**Performance Goals**: Issue the two independent upstream count requests concurrently; honor the configured timeout and avoid downloading individual records

**Constraints**: No inline CSS; accessible labels and status regions; score only when both counts succeed; exclusive upper date bound is the day after the selected To date; no database persistence

**Scale/Scope**: One analysis page, three supported business types, configurable public Socrata datasets, and one optional analysis endpoint

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

The repository constitution is still the generated placeholder and defines no enforceable project-specific principles. No gate violations are identified. The design nevertheless keeps the calculation pure, isolates external I/O behind interfaces, and uses automated tests for the HTTP contract. Post-design re-check: the requested typed client, pure calculator, concurrent service, accessibility requirements, and xUnit coverage are all represented without adding persistence or unrelated scope.

## Project Structure

### Documentation (this feature)

```text
specs/001-neighborhood-analysis/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/neighborhood-analysis.openapi.yaml
└── tasks.md              # Phase 2 output; not created by this plan
```

### Source Code (repository root)

```text
Cincy Biz Advisor/
├── Pages/
│   ├── Analyze.cshtml
│   └── Analyze.cshtml.cs
├── Services/
│   ├── CityDataOptions.cs
│   ├── ICityDataService.cs
│   ├── CityDataService.cs
│   ├── INeighborhoodAnalysisService.cs
│   ├── NeighborhoodAnalysisService.cs
│   ├── SuitabilityCalculator.cs
│   └── CityDataUnavailableException.cs
├── wwwroot/css/analyze.css
├── appsettings.json
└── Program.cs

Cincy Biz Advisor.Tests/
├── Cincy Biz Advisor.Tests.csproj
├── CityDataServiceTests.cs
├── SuitabilityCalculatorTests.cs
├── NeighborhoodAnalysisServiceTests.cs
├── AnalyzePageTests.cs
└── AnalysisEndpointTests.cs
```

**Structure Decision**: Keep the feature in the existing single Razor Pages project. Put external-data access and domain orchestration behind `Services/`; keep form and presentation state in `Pages/Analyze`; place feature-specific styling in `wwwroot/css/analyze.css`; add a sibling xUnit project for isolated tests. `Program.cs` owns dependency injection, options binding, and the optional endpoint mapping.

## Architecture

```mermaid
flowchart LR
    Browser[Analyze Razor Page] -->|POST form| Page[AnalyzeModel]
    Browser -->|optional GET| Endpoint[/api/neighborhoods/{name}/analysis]
    Page --> Analysis[INeighborhoodAnalysisService]
    Endpoint --> Analysis
    Analysis -->|Task.WhenAll| City[ICityDataService]
    City --> Crime[(Socrata crime dataset)]
    City --> Permits[(Socrata permits dataset)]
    Analysis --> Calculator[SuitabilityCalculator]
    Calculator --> Result[NeighborhoodAnalysisResult]
    Result --> Page
    Result --> Endpoint
```

## Implementation Details

1. **Configuration and registration**
   - Add `CityDataOptions` with crime/permit dataset IDs, neighborhood fields, date fields, and timeout.
   - Add the `CityData` section to `Cincy Biz Advisor/appsettings.json` using crime defaults `cpd_neighborhood`/`date_reported` and permit fields `neighborhood`/`issueddate`.
   - Register options validation, the typed `ICityDataService` client, and `INeighborhoodAnalysisService` in `Program.cs`.
2. **Socrata data service**
   - Build `$select=count(*) AS total` and `$where` query parameters using URL encoding.
   - Use `upper(<configured neighborhood field>) = '<UPPERCASE NAME>'` and `>= start` with `< To + 1 day` for each configured date field.
   - Parse the single-row `total` response, reject malformed/non-success responses, and throw `CityDataUnavailableException` with the required crime message when the crime request is unavailable.
3. **Domain service and calculator**
   - Validate the request boundary and run crime and permit calls concurrently with `Task.WhenAll`.
   - Keep `SuitabilityCalculator` static and pure; encode the three weights, Safety floor, Growth cap, and nearest-integer rounding there.
   - Return a result only when both counts are available.
4. **Razor Page and optional endpoint**
   - Add `Analyze.cshtml`/`Analyze.cshtml.cs` with POST binding, defaults, validation, supported selections, and same-page result rendering.
   - Use labels and validation attributes, `role="alert"` for errors, and `aria-live` for results; reference `wwwroot/css/analyze.css` rather than inline styles.
   - Optionally map `GET /api/neighborhoods/{name}/analysis` to the shared service with 200/404/400/503 responses as defined in [contracts/neighborhood-analysis.openapi.yaml](./contracts/neighborhood-analysis.openapi.yaml).

## Error Handling

| Condition | Boundary | User/API behavior | Score? |
|---|---|---|---|
| Missing neighborhood | Razor Page validation / endpoint validation | Page: `Please select a neighborhood`; API: 404 for missing/unrecognized route neighborhood | No |
| Unsupported business type or invalid date range | Request validation | Page validation message; API 400 | No |
| Crime Socrata timeout, non-success, or malformed count | `CityDataService` → `CityDataUnavailableException` | Page: `Crime data is currently unavailable. Please try again later.`; API 503 | No |
| Permit Socrata timeout, non-success, or malformed count | `CityDataService` → unavailable exception | Page/API safe data-unavailable response; API 503 | No |
| Both data calls succeed | Analysis service | Render/return counts, Safety, Growth, and rounded score | Yes |

## File List

### Existing files to modify

- `Cincy Biz Advisor/Program.cs` — register options, typed client, analysis service, and optional endpoint.
- `Cincy Biz Advisor/appsettings.json` — add the `CityData` configuration section.

### New application files

- `Cincy Biz Advisor/Pages/Analyze.cshtml` — accessible form and same-page results.
- `Cincy Biz Advisor/Pages/Analyze.cshtml.cs` — `AnalyzeModel`, POST workflow, validation, and view state.
- `Cincy Biz Advisor/Services/CityDataOptions.cs` — typed configuration.
- `Cincy Biz Advisor/Services/ICityDataService.cs` and `CityDataService.cs` — Socrata count-query client.
- `Cincy Biz Advisor/Services/INeighborhoodAnalysisService.cs` and `NeighborhoodAnalysisService.cs` — concurrent orchestration.
- `Cincy Biz Advisor/Services/SuitabilityCalculator.cs` — pure score math.
- `Cincy Biz Advisor/Services/CityDataUnavailableException.cs` — safe upstream failure boundary.
- `Cincy Biz Advisor/wwwroot/css/analyze.css` — feature-specific styling.

### New test files

- `Cincy Biz Advisor.Tests/Cincy Biz Advisor.Tests.csproj` — xUnit test project reference.
- `Cincy Biz Advisor.Tests/CityDataServiceTests.cs` — fake-handler query construction and response parsing.
- `Cincy Biz Advisor.Tests/SuitabilityCalculatorTests.cs` — formula, caps, weights, rounding, and reference scores.
- `Cincy Biz Advisor.Tests/NeighborhoodAnalysisServiceTests.cs` — concurrent orchestration and failure behavior.
- `Cincy Biz Advisor.Tests/AnalyzePageTests.cs` — defaults, POST validation, result rendering, and accessible errors.
- `Cincy Biz Advisor.Tests/AnalysisEndpointTests.cs` — optional endpoint status and JSON contract tests.

## Testing Plan

- **Calculator unit tests**: verify Safety `max(0, 100 - crimes/8)`, Growth `min(100, permits)`, all weights, 0–100 output, and nearest rounding. Include 101.1 (72) and 101.2 (64).
- **HTTP contract tests**: inject a fake `HttpMessageHandler`, assert both dataset IDs and URL-decoded `$select`/`$where` values, including uppercase neighborhood, inclusive start, exclusive next-day end, and configured fields.
- **Service tests**: return fake crime and permit counts, verify both requests are initiated, both are required for a result, and upstream failures become `CityDataUnavailableException`.
- **Razor Page tests**: verify 2025 defaults, required neighborhood message, valid same-page result, crime-unavailable alert, no score on failure, labels, `role="alert"`, and `aria-live` result markup.
- **Endpoint tests**: verify 200 result JSON, 400 invalid inputs, 404 missing/unrecognized neighborhood, and 503 data unavailability when the optional endpoint is enabled.
- **Regression verification**: run the full xUnit suite and build the solution after implementation; manually execute both reference scenarios against configured test responses before live-data smoke testing.

## Risks

- **Upstream schema drift**: field names may change or differ by environment. Mitigation: keep fields configurable, assert query construction in tests, and perform a live smoke check before release.
- **Socrata access limits or outages**: public endpoints can reject requests or time out. Mitigation: typed client timeout, explicit exception translation, no partial score, and user-safe/API-safe errors.
- **Neighborhood spelling mismatch**: data may contain inconsistent casing or whitespace. Mitigation: uppercase query matching and a single canonical selection list; avoid broad fuzzy matching.
- **Date type differences**: a source may expose date-only or timestamp values. Mitigation: use the exclusive next-day boundary and verify each configured field with a representative response.
- **Optional endpoint scope growth**: endpoint and page could diverge. Mitigation: share `INeighborhoodAnalysisService` and keep the OpenAPI contract limited to the requested 200/404/400/503 behavior.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|---|---|---|
| None | N/A | The typed client, service interfaces, and optional endpoint are explicitly required by the feature and do not introduce an unnecessary architectural layer. |
