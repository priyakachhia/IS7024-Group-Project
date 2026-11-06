# Compare Neighborhoods Research

## Repository Findings

- The application is a single .NET 10 Razor Pages web project in `Cincy Biz Advisor/Cincy Biz Advisor.csproj`.
- `Program.cs` currently registers only Razor Pages and the standard request pipeline.
- `appsettings.json` contains logging and allowed-host settings only; no database connection string or external data endpoint is configured.
- No Entity Framework package, DbContext, data-access service, dataset file, chart library, Compare page, or Analyze page is currently present in the project.
- Bootstrap, jQuery, and jQuery validation are available in `wwwroot/lib` and can support page layout and form validation. The repository does not currently include a charting library.
- The Compare mockup is `compare.png` (676 by 384 pixels). The README describes its intended content as two neighborhoods compared side by side with crime statistics, permit activity, and location suitability scores.

## Data Sources and Metric Decisions

### Decision: Use verified Cincinnati public-data source mappings behind a comparison data boundary

**Rationale**: The specification requires actual available data and forbids invented metrics. The README identifies the Cincinnati Open Data Portal reported crime dataset and building permits dataset, but the repository does not contain their files or schemas. A source boundary allows the implementation to map the actual published fields once the source access method is supplied without making the Razor Page responsible for source-specific parsing.

Before adapter implementation, the data owner must provide or confirm for each source: endpoint or access method, response schema, neighborhood join key and normalization rule, source reporting-period field, source-provided total-count field, completeness rule, refresh behavior, authentication/configuration requirements, and representative success/missing/error responses. The mapping record must identify the exact input field and output contract field; it must not introduce a derived metric or fallback constant. The latest complete common period is selected only after both source period coverage and completeness are verified.

**Alternatives considered**:

- Hard-code example values: rejected because the specification requires source-backed values and explicit N/A states.
- Add a new local dataset during Compare implementation: rejected because creating or fabricating datasets is out of scope.
- Couple the Razor Page directly to source-specific records: rejected because it would mix presentation with data-source mapping and make N/A behavior inconsistent.

### Decision: Use canonical option providers for business types and neighborhoods

**Rationale**: Clarification answers require both selectors to use canonical available values. The same canonical neighborhood identity must be used to match crime, permit, and suitability records consistently. Unsupported free-text business types must not be accepted.

**Alternatives considered**:

- Free-text inputs: rejected because spelling and identity mismatches could produce incorrect comparisons.
- A Compare-only hard-coded list: rejected because it could diverge from available project data.

### Decision: Compare source-provided totals for the latest complete common reporting period

**Rationale**: The clarified specification requires the latest complete common reporting period and source-provided total crime and permit counts. The implementation should carry the selected period in the result and display it in the chart/table context.

**Alternatives considered**:

- Compare arbitrary latest records: rejected because the neighborhoods could use different time windows.
- Add trend charts: rejected because Compare requires three measure-specific comparison charts, not Analyze's trend workflow.
- Define a new aggregation formula: rejected because the source-provided totals are the authoritative metrics for this feature.

### Decision: Reuse the existing suitability result without calculation

**Rationale**: The repository README mentions a location score and an AnalysisHistory suitability score, but no calculation source or formula is present. Compare must consume an existing result for the selected canonical business type and neighborhood. If no result exists, the chart/table displays N/A and the source context explains why.

The suitability provider owner must confirm the read-only contract, canonical business-type and neighborhood keys, score/result meaning, source metadata, missing-result representation, and failure behavior. Compare may map and display that existing result only; it may not access suitability inputs, weights, formulas, persistence, or Analyze implementation details.

**Alternatives considered**:

- Derive a new score from crime and permits: rejected because it would conflict with the Analyze page ownership and violate the specification.
- Treat a missing score as zero: rejected because zero is a data value, not an unavailable state.
- Block all results when suitability is missing: rejected by the clarification decision to show available measures and mark only missing measures unavailable.

## Architecture Decision

### Decision: Dedicated Compare Razor Page with read-only comparison services and presentation contracts

The planned boundary is a dedicated `Pages/Compare.cshtml` and `Pages/Compare.cshtml.cs` pair. The PageModel will validate canonical selections and coordinate a read-only comparison service. Source adapters/providers will supply option lists and source-backed metrics; the PageModel will map the result to chart and table presentation models. The page will display three charts—crime, building permit activity, and suitability—and one comparison table, with a consistent N/A availability state.

The design will not modify Analyze page files or reuse Analyze-specific calculation code. Shared domain types may be introduced only when they represent source-backed data contracts required by both features; suitability calculation remains outside this feature.

**Alternatives considered**:

- Put comparison logic in the Razor markup: rejected because source mapping, validation, and N/A handling would be difficult to test and maintain.
- Build Compare as a separate application: rejected because the repository is a single Razor Pages project and the mockup belongs in its existing navigation/layout.
- Add database persistence first: rejected because no database configuration or schema is present and the feature is read-only; persistence is not required by the clarified specification.

## Project Naming and Migration Decision

The final product name is **Metro Biz Advisor**. The rename is project-wide, not limited to visible text: it includes the solution and project names, application and test folders, C# namespaces, project references, build commands/configuration, README and documentation, Spec Kit artifacts, navigation, page titles, and branding.

The repository currently uses Cincy Biz Advisor paths and identifiers. This planning pass must not rename files or folders. A coordinated migration must first inventory references, update the solution/project relationship and namespaces in dependency order, validate build and tests, update GitHub/repository references, and provide branch migration instructions for teammates. Existing feature branches must be rebased or recreated only after owners coordinate the rename; no branch should silently retain stale project references.

## Visualization Decision

### Decision: Use server-rendered SVG as the single chart-rendering owner

The project has no chart dependency. Server-rendered SVG in `Pages/Compare.cshtml` is the smallest approved boundary for this plan. `Compare.cshtml.cs` supplies presentation-ready source-backed values and states; the Razor page owns SVG markup and accessible labels; the comparison table is the text equivalent. `wwwroot/js/compare.js` is not a chart owner and must not calculate metrics. Charts must render source-backed values, labels, reporting period, and N/A state without a second metric calculation layer.

**Alternatives considered**:

- Add a large chart framework immediately: deferred because the repository has no established charting standard and the planning phase should not add an unnecessary dependency before implementation.
- Render chart-like HTML only: insufficient for the requested three comparison charts unless the team explicitly accepts a non-visual fallback.

## Constitution Check Input

The repository constitution is still a placeholder template: it contains no ratified principles, concrete gates, or governance rules. The plan therefore applies the explicit feature constraints as provisional gates: source-backed data only, no fabricated metrics, read-only behavior, testable validation, and strict isolation from Analyze. No constitution violation can be identified from the current file.

## Risks and Implementation Follow-ups

- The actual Cincinnati Open Data field names, endpoint URLs, refresh behavior, and neighborhood join key are not in the repository. Implementation must verify them before writing source adapters; the plan must not invent field names.
- The project has no configured persistence. If the existing suitability result becomes available through a teammate-owned service or database later, Compare should consume its read-only contract rather than create a parallel scoring system.
- The mockup is a visual reference only; responsive behavior and accessible chart alternatives must be validated during implementation.
- The source schema, join-key, completeness, and suitability contract are external blockers until verified and documented by the responsible data/provider owners.
