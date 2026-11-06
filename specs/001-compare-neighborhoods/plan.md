# Implementation Plan: Compare Neighborhoods

**Branch**: `feature/compare-neighborhoods-kachhipr` | **Date**: 2026-11-06 | **Spec**: [spec.md](spec.md)

## Summary

Build a dedicated read-only Compare Neighborhoods Razor Page for one canonical business type and two distinct canonical neighborhoods. Retrieve source-backed crime totals, building permit totals, and an existing suitability result; render exactly three comparison charts and a results table. Missing or failed source values render as `N/A`; Compare never calculates suitability or changes Analyze.

## Technical Context

**Language/Version**: C# with .NET 10 (`net10.0`)
**Primary Dependencies**: ASP.NET Core Razor Pages; existing Bootstrap, jQuery, and unobtrusive validation; no chart library currently exists.
**Storage**: Read-only source providers. No connection string, DbContext, persistence package, or dataset file exists; Compare adds no persistence.
**Testing**: `dotnet build`, focused validation/mapping tests, Razor Page/integration tests when a test project exists, and the scenarios in [quickstart.md](quickstart.md).
**Target Platform**: ASP.NET Core web application on .NET 10.
**Project Type**: Server-rendered Razor Pages web application.
**Performance Goals**: Charts and table visible within 5 seconds for valid selections with available data.
**Constraints**: Canonical identities; latest complete common period; source-provided totals; `N/A` for unavailable values; no fabricated metrics; reuse-only suitability; Analyze isolation; complete coordinated rename to Metro Biz Advisor before Compare implementation.
**Scale/Scope**: One read-only page, three measure groups, two neighborhoods per request, and one table; no history persistence or new dataset creation.

## Constitution Check

The repository constitution is an unfilled template with no approved principles, ratified version, governance rules, or concrete gates. Constitutional compliance cannot be evaluated or marked as passed. The following items are feature constraints and design safeguards, not approved constitution checks; they should be re-evaluated after the constitution is completed.

| Gate | Status |
|------|--------|
| Source-backed values only | NOT EVALUABLE — feature safeguard only |
| Explicit unavailable state | NOT EVALUABLE — feature safeguard only |
| No new suitability calculation | NOT EVALUABLE — feature safeguard only |
| Compare/Analyze isolation | NOT EVALUABLE — feature safeguard only |
| Read-only behavior | NOT EVALUABLE — feature safeguard only |

## Project Identity and Migration Gate

The final project identity is **Metro Biz Advisor** everywhere: application branding, solution and project names, application/test folders, C# namespaces, project references, build configuration and commands, README/documentation, Spec Kit artifacts, navigation, page titles, and GitHub/repository references.

The current repository remains physically and logically named Cincy Biz Advisor during this planning-only pass. No application files or folders are renamed now. The rename must be executed as a separately reviewed migration before Compare implementation:

1. Inventory solution, project, source, test, namespace, documentation, CI, launch, and script references.
2. Coordinate the target repository URL/name and GitHub redirects or remote updates with repository owners.
3. Update solution/project/folder names and project references in dependency order, then update namespaces and generated/build configuration.
4. Update README, Spec Kit artifacts, navigation, page titles, branding, CI/CD, launch profiles, and documentation.
5. Build and test from the renamed solution, verify no stale Cincy identifiers remain except explicitly historical migration notes, and document rollback points.
6. Coordinate teammate branches: pause conflicting rebases, communicate the migration commit/merge base, rebase or recreate branches with owners, resolve project-reference conflicts, and require each branch to build before Compare work resumes.

The migration owner, repository owner, and owners of Analyze and other active feature branches must review this sequence. Compare implementation is blocked until the rename review gate is approved.

## Project Structure

```text
Current planning path: Cincy Biz Advisor/ (temporary; replace after Phase 0)
├── Pages/Compare.cshtml and Compare.cshtml.cs
├── Models/Compare/
├── Services/Compare/
├── Data/Sources/
└── wwwroot/css/site.css and wwwroot/js/compare.js
```

**Structure Decision**: Use the existing single Razor Pages project. After Phase 0, apply the same structure under the approved Metro Biz Advisor project path and namespace. Keep the page in `Pages/Compare*`, models in `Models/Compare`, and source access behind `Services/Compare` and `Data/Sources`. Use an isolated server-rendered SVG chart presentation boundary; do not create a second project, database schema, shared Analyze service, or new API surface.

## Implementation Phases

### Phase 0: Coordinated Project Rename (Prerequisite)

1. Complete the rename inventory and identify all team-owned references.
2. Obtain team approval for repository naming, branch migration, and rollback strategy.
3. Execute the project/solution/folder/namespace/documentation/branding migration in dependency order.
4. Validate project references, build configuration, GitHub remotes/links, CI, and all active teammate branches.
5. Record the migration result and approve the gate before creating Compare application files.

### Phase 1: Data Access Readiness

1. Obtain data-owner confirmation of the crime and building-permit endpoints/access methods, response schemas, neighborhood join key/normalization, period fields, completeness rules, source-provided total fields, refresh behavior, and representative missing/error responses.
2. Verify canonical business-type and neighborhood option providers and their stable identifiers.
3. Obtain suitability-provider owner confirmation of the read-only result contract, canonical keys, score meaning, source metadata, missing-result behavior, and failure behavior.
4. Record the verified field-to-contract mapping in the source readiness artifact before writing adapters.
5. Configure source access without secrets or invented local metric data; this phase blocks adapter implementation and integration testing.

### Phase 2: Compare Domain and Services

1. Add models from [data-model.md](data-model.md).
2. Add read-only source-provider interfaces from the verified readiness contracts.
3. Add adapters only after Phase 1, mapping verified source fields and provider results to the comparison contract.
4. Validate identifiers, select the latest complete common period using the documented completeness rule, preserve source metadata/meaning, and produce `Available`, `Unavailable`, or `SourceError` states.
5. Test duplicate neighborhoods, unsupported options, missing records, source failures, complete/partial results, and suitability non-recalculation.

### Phase 3: Razor Page and Presentation

1. Add canonical dropdowns and validation.
2. Add mockup-aligned side-by-side results with exactly three measure-specific chart regions.
3. Add the results table as the accessible text equivalent.
4. Add server-rendered SVG in `Compare.cshtml` as the sole chart owner for source values, labels, periods, and `N/A`; do not add a competing chart-rendering path in JavaScript.
5. Add responsive/accessibility styling without changing Analyze behavior.

### Phase 4: Integration and Validation

1. Register only Compare-specific services in `Program.cs` after Phase 1 contracts and the Phase 0 rename gate are verified; preserve Analyze registrations.
2. Add navigation only if needed, using Metro Biz Advisor branding and preserving Analyze behavior; review shared layout/style changes separately.
3. Run a timed configured-source scenario and record p95 or equivalent observed duration against the five-second goal, including the test environment and source response conditions.
4. Run build, focused tests, and [quickstart.md](quickstart.md) from the renamed solution/project paths.
5. Search for stale identifiers and review the final diff to confirm no Analyze modifications, trend calculations, suitability formulas, persistence, or fabricated data.

## Design Artifact References

- [Research](research.md)
- [Data model](data-model.md)
- [Compare page contract](contracts/compare-page.md)
- [Compare data contract](contracts/compare-data.md)
- [Quickstart](quickstart.md)
- [Specification](spec.md)

## Complexity Tracking

Constitution violations cannot be assessed because the constitution is incomplete. The source-provider boundary is the minimum separation needed for source mapping, N/A handling, and suitability ownership; it remains subject to review once approved project principles exist.
