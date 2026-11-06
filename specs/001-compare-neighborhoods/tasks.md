# Tasks: Compare Neighborhoods

**Input**: Design documents from `/specs/001-compare-neighborhoods/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, and `quickstart.md`

**Implementation boundary**: All application tasks are limited to the Compare page and its dedicated models/services/data-source adapters after the approved Metro Biz Advisor rename. Do not modify Analyze page files, Analyze calculations, trend workflows, or suitability ownership.

**Implementation gate**: Do not create Compare application files, folders, project references, or feature code until the coordinated project-wide rename tasks T000-T000.6 are reviewed and completed. During the current planning pass, no application files or folders are renamed.

## Phase 0: Coordinated Project-Wide Rename (Blocking Prerequisite)

**Purpose**: Safely migrate the current Cincy Biz Advisor identity to Metro Biz Advisor before Compare implementation, with explicit team coordination and rollback points.

- [ ] T000 Inventory every Cincy Biz Advisor identifier across the solution, projects, folders, C# namespaces, project references, build/launch/CI configuration, README, documentation, Spec Kit artifacts, navigation, page titles, branding, GitHub links, and remotes; record current paths without renaming them.
- [ ] T000.1 [TEAM] Approve the target Metro Biz Advisor solution/project/folder/namespace naming, GitHub repository name/URL strategy, redirect/remote changes, migration owner, rollback point, and validation evidence with repository owners.
- [ ] T000.2 Rename the solution, projects, application/test folders, and project references in dependency order; update build configuration, launch profiles, CI/CD, scripts, and generated project metadata without changing Analyze behavior.
- [ ] T000.3 Rename C# namespaces and update all code references, documentation links, README content, Spec Kit artifacts, navigation labels, page titles, and application branding to Metro Biz Advisor.
- [ ] T000.4 [TEAM] Coordinate GitHub repository references, remotes, links, branch protection, CI secrets/settings, and any external documentation with repository owners; preserve redirects or a documented rollback path.
- [ ] T000.5 [TEAM] Coordinate existing teammate feature branches by announcing the migration commit/merge base, pausing conflicting work as needed, rebasing or recreating branches with each owner, resolving project-reference conflicts, and requiring a successful build before work resumes.
- [ ] T000.6 Validate the renamed solution and all active branches, search for stale Cincy identifiers except explicitly labeled historical migration notes, and obtain the rename review approval that unlocks Compare implementation.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the Compare-specific implementation and test locations without adding application behavior.

The Cincy paths in the Phase 1-6 task descriptions are current repository paths recorded for planning only. After Phase 0, they must be mechanically replaced by the approved Metro Biz Advisor paths and namespaces before execution.

- [ ] T001 Create the Compare source directories `Cincy Biz Advisor/Models/Compare/`, `Cincy Biz Advisor/Services/Compare/`, and `Cincy Biz Advisor/Data/Sources/` according to `specs/001-compare-neighborhoods/plan.md`.
- [ ] T002 [P] Create the Compare test project directory using the approved Metro Biz Advisor test path and configure it to reference the renamed Metro Biz Advisor application project.
- [ ] T003 [P] Add the Compare-specific test project to the renamed Metro Biz Advisor solution without adding or changing Analyze project references.
- [ ] T004 Record the actual Cincinnati crime and building-permit access methods, endpoint responses, canonical neighborhood join key/normalization, reporting periods, completeness rules, source-provided total fields, and schema mappings after verification with the data owner; do not add invented fields or metric values. This task blocks source adapters and integration testing.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implement shared source contracts, canonical option handling, availability states, and configuration required by all user stories.

**CRITICAL**: Complete this phase before user-story implementation.

- [ ] T005 Define canonical option, reporting-period, availability-state, and source-metric contracts in `Cincy Biz Advisor/Models/Compare/CompareDataModels.cs`, preserving the data-model rules that identifiers are canonical, required selections are non-empty, and metric totals are never replaced with zero when absent.
- [ ] T006 Define read-only source-provider interfaces for canonical business types, canonical neighborhoods, crime totals, permit totals, and existing suitability results in `Cincy Biz Advisor/Services/Compare/ICompareDataProviders.cs`.
- [ ] T007 Implement source configuration binding in `Cincy Biz Advisor/Services/Compare/CompareDataOptions.cs` and `Cincy Biz Advisor/appsettings.json` without committing secrets, database credentials, or fabricated dataset records.
- [ ] T008 Implement source adapter error mapping in `Cincy Biz Advisor/Data/Sources/CompareSourceAvailability.cs` so missing records become `Unavailable`, source read/payload failures become `SourceError`, and raw source errors are not sent to users.
- [ ] T009 [P] Add foundational contract tests for availability-state mapping and canonical identifier rules in `Cincy Biz Advisor.Tests/CompareDataContractTests.cs`.
- [ ] T010 [P] Add foundational service tests for canonical option filtering and distinct-neighborhood validation in `Cincy Biz Advisor.Tests/CompareSelectionValidationTests.cs`.
- [ ] T010.1 Register the verified Compare-specific options, providers, and service in the renamed Metro Biz Advisor `Program.cs` only after source readiness and the rename gate are complete; do not alter Analyze registrations.

**Checkpoint**: Source contracts, canonical selectors, error states, and test scaffolding are ready; no user story should query raw source data directly.

---

## Phase 3: User Story 1 - Compare Two Neighborhoods (Priority: P1) MVP

**Goal**: Allow a user to select one supported business type and two different canonical neighborhoods and obtain a source-backed comparison context.

**Independent Test**: Select a canonical business type and two different canonical neighborhoods, submit Compare, and verify that the result identifies the business type and both neighborhoods without accepting duplicate or unsupported selections.

### Tests for User Story 1

- [ ] T011 [P] [US1] Add Razor Page request tests for valid selections, missing business type, missing neighborhoods, unsupported identifiers, and identical neighborhoods in `Cincy Biz Advisor.Tests/ComparePageSelectionTests.cs`.
- [ ] T012 [P] [US1] Add comparison-service tests for canonical option matching and result context creation in `Cincy Biz Advisor.Tests/NeighborhoodComparisonServiceTests.cs`.

### Implementation for User Story 1

- [ ] T013 [P] [US1] Implement canonical business-type and neighborhood option providers in `Cincy Biz Advisor/Services/Compare/CompareOptionService.cs` using only available canonical values.
- [ ] T014 [US1] Implement comparison request validation and read-only orchestration in `Cincy Biz Advisor/Services/Compare/NeighborhoodComparisonService.cs`, requiring two different neighborhood identifiers before querying source metrics.
- [ ] T015 [US1] Implement the Compare PageModel in `Cincy Biz Advisor/Pages/Compare.cshtml.cs` with GET option loading and POST validation/orchestration, keeping all Analyze files untouched.
- [ ] T016 [US1] Implement the Compare selection form in `Cincy Biz Advisor/Pages/Compare.cshtml` with canonical business-type, Neighborhood A, and Neighborhood B dropdowns, a Compare action, preserved selections, and field-level validation messages.
- [ ] T017 [US1] Add the Compare route to the existing navigation only if needed in `Cincy Biz Advisor/Pages/Shared/_Layout.cshtml`, without changing Analyze navigation or behavior.

**Checkpoint**: A user can independently select valid inputs, receive clear validation for invalid inputs, and obtain a comparison context labeled with the selected business type and two neighborhoods.

---

## Phase 4: User Story 2 - Review Comparison Charts (Priority: P1)

**Goal**: Display exactly three source-backed comparison charts for crime, building permit activity, and existing location suitability.

**Independent Test**: Run a valid comparison and verify that exactly one chart exists for each measure, both neighborhoods are represented, the applicable period/source context is shown, and missing values render as `N/A`.

### Tests for User Story 2

- [ ] T018 [P] [US2] Add source-mapping tests for latest complete common-period crime totals and permit totals in `Cincy Biz Advisor.Tests/CompareMetricMappingTests.cs`.
- [ ] T019 [P] [US2] Add suitability reuse tests proving Compare displays an existing suitability result and does not calculate a replacement in `Cincy Biz Advisor.Tests/SuitabilityReuseTests.cs`.
- [ ] T020 [P] [US2] Add chart presentation tests for three measure types, both neighborhood labels, period/source context, and `N/A` states in `Cincy Biz Advisor.Tests/CompareChartPresentationTests.cs`.

### Implementation for User Story 2

- [ ] T021 [US2] Implement the verified Cincinnati reported-crime source adapter in `Cincy Biz Advisor/Data/Sources/ReportedCrimeSource.cs`, mapping actual source fields to source-provided total counts for the latest complete common reporting period.
- [ ] T022 [US2] Implement the verified Cincinnati building-permit source adapter in `Cincy Biz Advisor/Data/Sources/BuildingPermitSource.cs`, mapping actual source fields to source-provided total counts for the latest complete common reporting period.
- [ ] T023 [US2] Implement the existing suitability-result provider adapter in `Cincy Biz Advisor/Data/Sources/ExistingSuitabilitySource.cs`, returning the existing result and meaning or `Unavailable` without recalculation.
- [ ] T024 [US2] Extend `Cincy Biz Advisor/Services/Compare/NeighborhoodComparisonService.cs` to compose exactly three `ComparisonMeasure` records and preserve reporting periods, source labels, partial-data state, and user-facing availability messages.
- [ ] T025 [US2] Add isolated server-rendered SVG chart presentation in `Cincy Biz Advisor/Pages/Compare.cshtml`, rendering crime, building permit activity, and location suitability without domain metric calculations; keep `compare.js` out of chart ownership unless a separately approved design requires it.
- [ ] T026 [US2] Add responsive and accessible chart styling in `Cincy Biz Advisor/wwwroot/css/site.css`, ensuring neighborhood identity is not conveyed by color alone and chart information has a text-equivalent.

**Checkpoint**: A valid comparison displays exactly three charts backed by actual source values or explicit `N/A` states, with no new suitability formula and no Analyze coupling.

---

## Phase 5: User Story 3 - Inspect Comparison Details (Priority: P2)

**Goal**: Present a precise comparison table beneath the charts with one row per requested measure.

**Independent Test**: Run a valid comparison and verify that the table contains exactly one row for crime, one for building permit activity, and one for location suitability, with values or `N/A` for both neighborhoods and period/source context where available.

### Tests for User Story 3

- [ ] T027 [P] [US3] Add results-table rendering tests for the three required measure rows, both neighborhood columns, period/source context, zero values, large values, and `N/A` states in `Cincy Biz Advisor.Tests/CompareResultsTableTests.cs`.
- [ ] T028 [P] [US3] Add partial-result integration tests proving available measures remain visible when another source metric is missing or fails in `Cincy Biz Advisor.Tests/ComparePartialDataTests.cs`.

### Implementation for User Story 3

- [ ] T029 [US3] Implement the comparison results table in `Cincy Biz Advisor/Pages/Compare.cshtml` with Measure, Neighborhood A, Neighborhood B, and Period/source-context columns.
- [ ] T030 [US3] Implement `N/A` and source-error table messaging in `Cincy Biz Advisor/Pages/Compare.cshtml.cs` and `Cincy Biz Advisor/Pages/Compare.cshtml`, retaining available measures and never substituting zero or estimates.
- [ ] T031 [US3] Verify the table is the accessible text-equivalent for all three charts and remains readable for zero and large values in `Cincy Biz Advisor/Pages/Compare.cshtml` and `Cincy Biz Advisor/wwwroot/css/site.css`.

**Checkpoint**: User Stories 1, 2, and 3 work together while the table remains independently verifiable and all unavailable metrics are explicit.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate the complete feature, preserve Analyze isolation, and document operational readiness.

- [ ] T032 [P] Add end-to-end Compare workflow tests covering valid comparison, duplicate neighborhoods, missing selections, source failure, partial data, and suitability reuse in `Cincy Biz Advisor.Tests/CompareWorkflowTests.cs`.
- [ ] T033 [P] Add accessibility and responsive checks for labeled dropdowns, keyboard navigation, stacked narrow-screen layout, chart text alternatives, and non-color distinctions in `Cincy Biz Advisor.Tests/CompareAccessibilityTests.cs`.
- [ ] T034 Run `dotnet build ".\Cincy Biz Advisor\Cincy Biz Advisor.csproj"` and the Compare test project; resolve only Compare-related failures in the planned files.
- [ ] T035 Run all scenarios in `specs/001-compare-neighborhoods/quickstart.md` against configured actual sources, recording `N/A` for missing source metrics instead of inventing expected values.
- [ ] T036 Review the final change set to confirm no files, routes, calculations, trend workflows, or suitability ownership in Analyze were changed and no persistence or fabricated dataset was added.
- [ ] T037 Update `specs/001-compare-neighborhoods/quickstart.md` with verified source setup and observed validation commands without adding metric fixtures or application code to the planning documents.
- [ ] T038 [P] Add a timed validation for the five-second valid-comparison goal using configured actual sources, recording environment and result without inventing metric fixtures.
- [ ] T039 Review shared `_Layout.cshtml` and `site.css` changes for Analyze isolation, documenting any affected selectors, navigation, branding, or responsive rules before approval.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Rename (Phase 0)**: Must be reviewed and completed before any Compare application files, folders, project references, or implementation tasks; requires repository and teammate coordination.
- **Setup (Phase 1)**: Depends on the approved rename gate; establishes directories, tests, and source verification.
- **Foundational (Phase 2)**: Depends on Setup and verified source readiness; blocks all user stories because every story needs canonical options and source availability states.
- **User Story 1 (Phase 3)**: Depends on Foundational; provides the valid Compare request and page form.
- **User Story 2 (Phase 4)**: Depends on User Story 1’s request/page boundary and Foundational source contracts; provides the three charts.
- **User Story 3 (Phase 5)**: Depends on User Story 2’s composed comparison result; provides the table and partial-data details.
- **Polish (Phase 6)**: Depends on all desired user stories; performs end-to-end, accessibility, build, and Analyze-isolation validation.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational; MVP input and comparison-context flow.
- **User Story 2 (P1)**: Starts after Foundational and integrates with the US1 result boundary; independently verifies chart output.
- **User Story 3 (P2)**: Starts after US2’s measure model; independently verifies the table and partial-data behavior.

### Parallel Opportunities

- T000 inventory and review preparation can proceed in parallel, but rename execution, branch migration, and validation are ordered and coordinated.
- T002–T003 can run in parallel after the rename gate; T004 must complete before source adapters, source configuration, or integration testing.
- T005–T008 and T010.1 are ordered: define contracts, verify/configure source access, map availability states, then register services; T010.1 cannot run before T004 and the rename gate.
- T009–T010 can run in parallel after foundational contracts are defined.
- T011–T012 can run in parallel before US1 implementation.
- T013 and T017 can run in parallel after the page boundary is agreed; T014–T016 depend on the foundational contracts.
- T018–T020 can run in parallel before US2 implementation.
- T021–T023 can run in parallel after source schemas and provider contracts are verified.
- T027–T028 can run in parallel before US3 implementation.
- T032–T033 can run in parallel after the user-story implementations are integrated.

## Parallel Example: User Story 1

```text
Task: T011 [US1] Request validation tests in Cincy Biz Advisor.Tests/ComparePageSelectionTests.cs
Task: T012 [US1] Comparison-service tests in Cincy Biz Advisor.Tests/NeighborhoodComparisonServiceTests.cs
Task: T013 [US1] Canonical option providers in Cincy Biz Advisor/Services/Compare/CompareOptionService.cs
Task: T017 [US1] Optional Compare navigation in Cincy Biz Advisor/Pages/Shared/_Layout.cshtml
```

## Parallel Example: User Story 2

```text
Task: T018 [US2] Crime/permit mapping tests in Cincy Biz Advisor.Tests/CompareMetricMappingTests.cs
Task: T019 [US2] Suitability reuse tests in Cincy Biz Advisor.Tests/SuitabilityReuseTests.cs
Task: T021 [US2] Crime adapter in Cincy Biz Advisor/Data/Sources/ReportedCrimeSource.cs
Task: T022 [US2] Permit adapter in Cincy Biz Advisor/Data/Sources/BuildingPermitSource.cs
Task: T023 [US2] Existing suitability adapter in Cincy Biz Advisor/Data/Sources/ExistingSuitabilitySource.cs

```

## Implementation Strategy

### MVP First

1. Complete and obtain approval for the Phase 0 Metro Biz Advisor rename and branch-migration gate.
2. Complete Setup and Foundational phases using the renamed project paths and namespaces.
3. Complete User Story 1 for canonical selections, validation, and comparison context.
4. Complete User Story 2 for the three source-backed charts.
5. Stop and validate the primary Compare journey using actual source data and explicit `N/A` states.

### Incremental Delivery

1. Deliver the approved Metro Biz Advisor identity migration and coordinated branch baseline.
2. Deliver canonical selection and validation.
3. Add crime, permit, and suitability source-backed chart output.
4. Add the comparison table and partial-data behavior.
5. Complete accessibility, responsive, build, end-to-end, timed-performance, and Analyze-isolation checks.

### Notes

- Every application task uses the required checklist format: checkbox, sequential ID, optional `[P]`, required story label for story phases, and an exact file path. Phase 0 coordination tasks identify their owner and migration deliverable instead of inventing a future path.
- Test tasks are included because the user explicitly requested testing and the specification defines independent test criteria.
- Do not implement a new suitability formula, add invented data, add persistence, or modify the Analyze page.
