# Implementation Plan: Home Page Primary Action

**Branch**: `home-neighborhood-search-awarera` | **Date**: 2026-11-06 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [specs/001-home-primary-action/spec.md](spec.md)

## Summary

Build the MetroBiz Advisor Home page primary action in the existing .NET 10 Razor Pages application. The Home page will load supported business types and repository-backed neighborhoods, filter neighborhood results with trimmed case-insensitive substring matching, clear a prior selection when search text changes, validate both selections on the server, and redirect valid submissions to `/Analyze` with URL-encoded `neighborhood` and `businessType` query values.

The implementation will introduce a Neighborhood model and repository interface with a seeded in-memory implementation registered through dependency injection. Tests must be written first for repository behavior, PageModel validation/navigation, rendered search/selection behavior, measurable accessibility outcomes, 320 CSS pixel behavior, and the 1366 × 768 visual requirement. The Analyze page will remain unimplemented and teammate-owned.

## Technical Context

**Language/Version**: C# on .NET 10 (`net10.0`), nullable reference types and implicit usings enabled.

**Primary Dependencies**: ASP.NET Core Razor Pages, Bootstrap assets already present in `wwwroot/lib`, jQuery assets already present in `wwwroot/lib`, HTML, CSS, and the existing static asset pipeline. Add only test dependencies required for xUnit, ASP.NET Core integration testing, and Playwright browser checks.

**Storage**: Seeded in-memory neighborhood repository for this feature; repository abstraction keeps a future database implementation swappable.

**Testing**: New xUnit test project; unit tests for repository and PageModel rules; ASP.NET Core integration tests using `WebApplicationFactory`; Microsoft Playwright checks for browser interaction, keyboard/focus/status behavior, contrast states, and the desktop and narrow viewports. Candidate package targets are `Microsoft.NET.Test.Sdk` 17.14.x, `xunit` 2.9.x, `xunit.runner.visualstudio` 3.1.x, `Microsoft.AspNetCore.Mvc.Testing` 10.0.x, and `Microsoft.Playwright` 1.55.x; these are planning targets only and are not compatibility-verified until restore/build succeeds.

**Target Platform**: Existing ASP.NET Core web application hosted by the project's .NET 10 development/production profiles; desktop acceptance viewport is 1366 × 768 at 100% browser zoom, with responsive behavior supported down to 320 CSS pixels without horizontal page scrolling.

**Project Type**: Single-project server-rendered Razor Pages web application with a separate automated test project to be added during implementation.

**Performance Goals**: Neighborhood filtering should update within one user interaction without a network request; valid Home page submission should complete through the normal form request/redirect flow. No new performance target beyond the clarified acceptance scenarios is required.

**Test host setup**: The sibling `net10.0` test project will reference the production project and expose the top-level `Program` type to the test assembly through the standard Razor Pages test seam (for example, an internal visibility declaration or equivalent supported arrangement). `WebApplicationFactory` requests will disable automatic redirect following and inspect the response `Location` header. The test will parse the `/Analyze` query string and verify that both decoded values equal the selected neighborhood display name and business type; it will not require the teammate-owned `/Analyze` page to exist. Playwright will launch the application under the test fixture using the project’s development launch path or an explicitly started local process, wait for the Home page to become available, and close the process after the run. The implementation setup must install Playwright’s browser binaries with the generated Playwright install script after package restore; browser installation is not performed during planning.

**Verification gate**: The first Implement task must restore and build the solution and test project with the selected package versions, then install the Playwright browser required by the test fixture. Planning documents do not claim package or tooling compatibility until those restore/build steps succeed.

**Constraints**: Do not change the target framework; keep shared layout changes minimal; do not implement or modify teammate-owned `/Analyze`; validate submitted business type and neighborhood on the server; use the specified combobox/listbox keyboard interaction; support 320 CSS pixels without horizontal scrolling; preserve measurable accessibility outcomes of 4.5:1 normal-text contrast, 3:1 large-text contrast, 3:1 applicable control/state contrast, visible focus, keyboard completion without a trap, and programmatically associated labels/validation/status feedback; keep the specified dark navy/white/red visual direction local to this feature where possible.

**Scale/Scope**: One Home page form, four business types, a seeded neighborhood collection, one repository boundary, and one teammate-owned Analyze destination. Dashboard cards, charts, maps, global search, profile controls, crime/permit data, and other pages are outside this feature's implementation.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Constitution source**: `.specify/memory/constitution.md` is present but contains placeholder principle names, descriptions, sections, and governance values rather than ratified project rules; the shared constitution is managed by Priya on `main` and is not modified here.
- **Constitution status**: Unresolved. No constitution compliance checks are claimed as passed because the local file does not contain ratified rules.
- **Feature planning gates**: The plan applies the user's explicit test-first requirement, limits changes to the Home page flow and its repository/data boundary, retains the existing web stack plus planned test tooling, and includes the approved accessibility and responsive acceptance outcomes. These feature-planning checks do not replace constitution review.

## Project Structure

### Documentation (this feature)

```text
specs/001-home-primary-action/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── home-page.md
├── checklists/
│   ├── requirements.md
│   └── home-action.md
└── tasks.md              # Not created by this Plan workflow
```

### Source Code (repository root)

```text
Cincy Biz Advisor/
├── Program.cs                         # Register the repository implementation
├── Pages/
│   ├── Index.cshtml                    # Home page form and accessible result/status markup
│   ├── Index.cshtml.cs                 # PageModel loading, server validation, redirect
│   └── Shared/_Layout.cshtml           # Change only if required; keep shared edits minimal
├── Models/
│   └── Neighborhood.cs                 # Id and Name entity
├── Services/
│   ├── INeighborhoodRepository.cs      # Replaceable repository contract
│   └── InMemoryNeighborhoodRepository.cs
└── wwwroot/
    ├── css/site.css                    # Home-specific visual/focus/layout rules
    └── js/site.js                      # Search filtering and selection clearing

Cincy Biz Advisor.Tests/
├── Cincy Biz Advisor.Tests.csproj
├── Services/
│   └── InMemoryNeighborhoodRepositoryTests.cs
├── Pages/
│   └── IndexPageModelTests.cs
└── Acceptance/
    └── HomePageAcceptanceTests.cs
```

**Structure Decision**: Keep production code in the existing single Razor Pages project, using its existing `Models`, `Services`, `Pages`, and `wwwroot` locations. Add a sibling xUnit test project because no test project currently exists. Do not create an Analyze page or unrelated feature directories.

## Implementation Sequence (test-first)

1. Add the test project and test dependencies without changing the production target framework.
2. Write failing repository tests for seeded `Over-the-Rhine`/`Oakley` data, unique seeded neighborhood names, the available-neighborhood collection, and lookup by identifier; do not place browser filtering behavior in the repository contract.
3. Write failing PageModel/integration tests for supported business types, required selections, unsupported submitted values, valid redirect values, and URL encoding.
4. Write failing browser acceptance tests for browser-owned filtering including `  oak  ` → `Oakley`, rendering seeded neighborhood names, matching a selected displayed name to its posted ID, selection clearing after search edits, empty/whitespace and no-results status, the specified combobox/listbox keyboard behavior, visible focus walkthrough, default/focused/selected/validation contrast states, 320 CSS pixel no-horizontal-scroll behavior, visual direction, and the 1366 × 768 full-form initial-viewport check.
5. Implement the Neighborhood model, repository interface, seeded repository, and DI registration.
6. Implement Home PageModel loading and authoritative server validation/redirect behavior.
7. Implement Home page markup, result selection semantics, and minimal local styling/script behavior.
8. Run the full test suite and accessibility/visual acceptance checks; fix only feature-scope failures.

## Acceptance Test Coverage

- Search `Over-the-Rhine`: matching result is rendered.
- Search `oakley`, mixed casing, and `oak`: `Oakley` is rendered.
- Search `abcdefgh123`: zero results and `No neighborhoods found` status are rendered.
- Select a neighborhood, edit search text, and verify selection is cleared.
- Submit without a neighborhood: field-level prompt, no redirect.
- Submit without a business type: field-level prompt, no redirect.
- Submit unsupported business type or neighborhood identifier: server rejects it, no redirect.
- Submit each of the four supported business types with an available neighborhood: redirect to `/Analyze` with URL-encoded query values.
- Verify the combobox/listbox keyboard pattern through a keyboard-only walkthrough, labels, visible focus in each control/result state, associated validation/status feedback, 4.5:1 normal-text contrast, 3:1 large-text contrast, and 3:1 applicable control/state contrast measured in default, focused, selected, and validation states; also verify keyboard completion without a trap, the compact dark navy header, white panel, red Analyze button, `scrollWidth <= clientWidth` at 320 CSS pixels, and the complete form bounding rectangle within the initial 1366 × 768 viewport.

## Complexity Tracking

| Addition | Why Needed | Simpler Alternative Rejected Because |
|----------|------------|--------------------------------------|
| Replaceable repository interface plus seeded implementation | Required by the specification so storage can be swapped later and submitted neighborhoods can be validated authoritatively | Direct collection access would couple the PageModel to storage and weaken test seams |
| Separate xUnit and browser acceptance test project | Required to apply the explicit test-first constraint to a repository with no existing tests and to verify client-side accessibility behavior | Manual-only checks would not reliably protect validation, navigation, or search-selection behavior |
