---

description: "Task list for the Home page primary action"
---

# Tasks: Home Page Primary Action

**Input**: Design documents from `specs/001-home-primary-action/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/home-page.md`, `quickstart.md`

**Scope**: Implement only the MetroBiz Advisor Home page primary action. `/Analyze` is teammate-owned and MUST NOT be created or modified by these tasks.

**Requirements-review distinction**: `checklists/home-action.md` and `checklists/requirements.md` are requirements-quality review artifacts. Their checkbox states do not replace the implementation tests below and must not be treated as runtime test results.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish test-first infrastructure without changing the existing .NET 10 production target.

- [x] T001 Create the sibling test project `Cincy Biz Advisor.Tests/Cincy Biz Advisor.Tests.csproj` targeting `net10.0` with candidate package targets from `plan.md`, the selected xUnit test SDK, and a production-project reference.
- [x] T002 Add `Cincy Biz Advisor.Tests/Cincy Biz Advisor.Tests.csproj` to `Cincy Biz Advisor.slnx` without changing the existing `Cincy Biz Advisor/Cincy Biz Advisor.csproj` target framework.
- [x] T003 Add candidate ASP.NET Core integration-test and Microsoft Playwright dependencies to `Cincy Biz Advisor.Tests/Cincy Biz Advisor.Tests.csproj`; retain existing Bootstrap, jQuery, HTML, and CSS assets in the production project.
- [x] T004 Restore and build the solution and test project, record any package/framework incompatibility, and stop setup until both succeed; do not claim compatibility from planning alone.
- [x] T005 Configure the standard top-level `Program` test seam for `WebApplicationFactory`, configure requests to disable automatic redirect following, and establish the local application-host fixture used by browser tests.
- [x] T006 Install Playwright browser binaries with the generated install script and document the fixture startup/shutdown convention; do not install browsers before the restore/build gate succeeds.
- [x] T007 [P] Create the test folders `Cincy Biz Advisor.Tests/Services/`, `Cincy Biz Advisor.Tests/Pages/`, and `Cincy Biz Advisor.Tests/Acceptance/` and establish the test fixture conventions used by later tasks.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define and verify the replaceable neighborhood data boundary before user-story implementation.

**CRITICAL**: Tasks in later phases depend on this phase.

### Tests first — must fail before implementation

- [x] T008 [P] Write failing repository tests in `Cincy Biz Advisor.Tests/Services/InMemoryNeighborhoodRepositoryTests.cs` for seeded `Over-the-Rhine` and `Oakley` records, positive unique IDs, unique initial names, lookup by ID, and the repository's available-neighborhood collection contract; do not test browser filtering here.
- [x] T009 [P] Write a failing dependency-injection registration test in `Cincy Biz Advisor.Tests/Pages/IndexPageModelTests.cs` or a dedicated composition test proving the Home PageModel can receive the repository through the application service container.

### Implementation

- [x] T010 Create `Cincy Biz Advisor/Models/Neighborhood.cs` with only the required `Id` and `Name` properties and the documented initial seeded-name uniqueness rule.
- [x] T011 Create `Cincy Biz Advisor/Services/INeighborhoodRepository.cs` with operations to load available neighborhoods and retrieve a neighborhood by ID.
- [x] T012 Create `Cincy Biz Advisor/Services/InMemoryNeighborhoodRepository.cs` with deterministic seeded records including `Over-the-Rhine` and `Oakley`, unique IDs, unique names, and lookup behavior.
- [x] T013 Register the repository implementation in `Cincy Biz Advisor/Program.cs` through dependency injection without adding database access or unrelated services.
- [x] T014 Run the foundational repository and DI tests and confirm they pass after the preceding red tests have been observed failing.

**Checkpoint**: The repository boundary, seeded data, and DI registration are independently tested and ready for the Home page.

---

## Phase 3: User Story 1 - Analyze a Business Opportunity (Priority: P1) 🎯 MVP

**Goal**: Allow a user to select a supported business type and neighborhood, receive server-side validation, and redirect to the teammate-owned Analyze destination with encoded values.

**Independent Test**: Submit valid and invalid Home page form selections and inspect the PageModel result, validation state, and redirect location without requiring the Analyze page implementation.

### Tests first — must fail before implementation

- [x] T015 [P] [US1] Write failing PageModel tests in `Cincy Biz Advisor.Tests/Pages/IndexPageModelTests.cs` for the exact four supported business types and required business-type validation.
- [x] T016 [P] [US1] Write failing PageModel tests in `Cincy Biz Advisor.Tests/Pages/IndexPageModelTests.cs` for missing, unknown, and stale `NeighborhoodId` values, including field-associated validation and no redirect.
- [x] T017 [P] [US1] Write failing integration tests in `Cincy Biz Advisor.Tests/Acceptance/HomePageAcceptanceTests.cs` for each supported business type paired with an available neighborhood; disable automatic redirect following and inspect the `/Analyze` `Location` header without requiring the destination page.
- [x] T018 [P] [US1] Write a failing integration test in `Cincy Biz Advisor.Tests/Acceptance/HomePageAcceptanceTests.cs` proving the `Location` header contains URL-encoded neighborhood and business type values that decode to the selected names.

### Implementation

- [x] T019 [US1] Add Home PageModel properties and GET loading in `Cincy Biz Advisor/Pages/Index.cshtml.cs` for the supported business types, available neighborhoods, selected `BusinessType`, and selected `NeighborhoodId`.
- [x] T020 [US1] Implement authoritative POST validation in `Cincy Biz Advisor/Pages/Index.cshtml.cs` for missing and unsupported business types and neighborhood IDs using the repository; preserve model-state errors and remain on `/` for invalid submissions.
- [x] T021 [US1] Implement valid redirect generation in `Cincy Biz Advisor/Pages/Index.cshtml.cs` to `/Analyze` with validated neighborhood `Name` and business type query values encoded by framework URL generation; do not create or modify any Analyze page.
- [x] T022 [US1] Run the US1 tests and confirm valid submissions return the expected redirect `Location` while missing, unsupported, or tampered selections do not redirect.

**Checkpoint**: User Story 1 is independently testable as the Home page form submission and redirect contract; `/Analyze` remains an external teammate-owned dependency.

---

## Phase 4: User Story 2 - Find a Neighborhood (Priority: P1)

**Goal**: Provide a searchable neighborhood picker with trimmed case-insensitive substring matching, empty-state behavior, result selection, and selection clearing after search edits.

**Independent Test**: Use the Home page picker with the specified search terms, select a result, edit the search, and inspect results, status text, and selection state without relying on the Analyze page.

### Tests first — must fail before implementation

- [x] T023 [P] [US2] Write failing browser acceptance tests in `Cincy Biz Advisor.Tests/Acceptance/HomePageAcceptanceTests.cs` for `Over-the-Rhine`, `oakley`, mixed casing, substring search `oak`, and trimmed search `  oak  ` returning `Oakley`.
- [x] T024 [P] [US2] Write failing browser acceptance tests for empty and whitespace-only search showing all available neighborhoods, rendered seeded neighborhood names, and non-empty unmatched `abcdefgh123` showing zero results plus `No neighborhoods found`.
- [x] T025 [P] [US2] Write a failing browser acceptance test proving that selecting a displayed neighborhood name submits the matching ID, then editing the search clears the selection and requires a new result selection before submission.

### Implementation

- [x] T026 [US2] Add the Home page form, neighborhood search control, accessible result list, selection state, hidden/posted neighborhood ID, and result/status markup in `Cincy Biz Advisor/Pages/Index.cshtml` without adding dashboard, map, global-search, profile, or other-page UI.
- [x] T027 [US2] Implement trimmed empty/whitespace and case-insensitive substring filtering, result rendering, selection assignment, and selection clearing in `Cincy Biz Advisor/wwwroot/js/site.js` using the existing jQuery asset.
- [x] T028 [US2] Add the exact empty-state and no-match status messages and ensure each selected displayed neighborhood name remains consistent with its submitted ID in `Cincy Biz Advisor/Pages/Index.cshtml` and `Cincy Biz Advisor/wwwroot/js/site.js`.
- [x] T029 [US2] Run the US2 browser tests and confirm every specified search, rendered-name, display/ID, no-results, selection, and selection-clearing scenario passes.

**Checkpoint**: User Story 2 is independently testable as a neighborhood search and selection experience.

---

## Phase 5: User Story 3 - Complete the Home Page Form Accessibly (Priority: P2)

**Goal**: Make the Home page primary action keyboard-completable, accessible, responsive, and visually scoped to the requested compact design.

**Independent Test**: Complete the Home page form with keyboard-only interaction at the required desktop and narrow viewport sizes, inspect focus/status/validation associations and contrast, and confirm no horizontal scrolling.

### Tests first — must fail before implementation

- [x] T030 [P] [US3] Write failing browser accessibility tests in `Cincy Biz Advisor.Tests/Acceptance/HomePageAcceptanceTests.cs` for the combobox/listbox pattern: Tab moves between form controls, Up/Down arrows navigate results, Enter selects, Escape closes results, Space remains usable while typing, and focus is visible during a keyboard-only walkthrough.
- [x] T031 [P] [US3] Write failing browser accessibility tests for visible labels, accessible names, programmatically associated validation and status feedback, keyboard completion without a trap, and computed contrast thresholds of 4.5:1 normal text, 3:1 large text, and 3:1 applicable control boundaries/state indicators in default, focused, selected, and validation states.
- [x] T032 [P] [US3] Write failing browser layout tests for the compact dark navy header, white primary action panel, red Analyze button, 1366 × 768 at 100% zoom with the complete form bounding rectangle inside the initial viewport, and widths down to 320 CSS pixels with `scrollWidth <= clientWidth`.

### Implementation

- [x] T033 [US3] Refine accessible combobox/listbox semantics, labels, focus management, result state, validation associations, and status announcements in `Cincy Biz Advisor/Pages/Index.cshtml` and `Cincy Biz Advisor/wwwroot/js/site.js` without introducing a keyboard trap.
- [x] T034 [US3] Add scoped Home page styling in `Cincy Biz Advisor/wwwroot/css/site.css` for the compact dark navy header, white primary action panel, red Analyze button, visible focus, contrast thresholds, and responsive layout from 320 CSS pixels upward.
- [x] T035 [US3] Keep changes to `Cincy Biz Advisor/Pages/Shared/_Layout.cshtml` minimal and only when required for shared assets or non-conflicting page structure; do not redesign global navigation or other pages.
- [x] T036 [US3] Run the US3 browser accessibility and layout tests and confirm keyboard completion, focus/status behavior, computed contrast in all specified states, desktop initial-viewport fit, narrow no-horizontal-scroll, and scoped visual requirements.

**Checkpoint**: All three user stories are independently testable, with the Home page primary action complete and `/Analyze` untouched.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final debugging, traceability, walkthrough, and acceptance verification across the completed Home page feature.

- [x] T037 [P] Run the complete test suite for `Cincy Biz Advisor.Tests` and investigate failures only within the Home page primary-action scope.
- [x] T038 [P] Run the `specs/001-home-primary-action/quickstart.md` acceptance walkthrough and record results for all search, selection, validation, non-followed redirect-header, accessibility, responsive, and visual checks.
- [x] T039 Debug and fix remaining Home page failures across `Cincy Biz Advisor/Pages/Index.cshtml`, `Cincy Biz Advisor/Pages/Index.cshtml.cs`, `Cincy Biz Advisor/wwwroot/js/site.js`, `Cincy Biz Advisor/wwwroot/css/site.css`, `Cincy Biz Advisor/Models/Neighborhood.cs`, `Cincy Biz Advisor/Services/INeighborhoodRepository.cs`, `Cincy Biz Advisor/Services/InMemoryNeighborhoodRepository.cs`, and `Cincy Biz Advisor/Program.cs`; do not modify `/Analyze`.
- [x] T040 Perform a requirements walkthrough in the implementer's own words, explaining how the repository, browser filtering, display/ID matching, selection clearing, server validation, non-followed URL-encoded redirect, accessibility measurements, responsive layout, and scoped styling satisfy `spec.md` and `contracts/home-page.md`.
- [x] T041 Re-run every acceptance scenario in `specs/001-home-primary-action/spec.md` and verify the four-business-type submission matrix, exact no-results message, decoded `Location` query values, 320 CSS pixel overflow condition, and 1366 × 768 initial-viewport form condition.
- [x] T042 Review `specs/001-home-primary-action/checklists/home-action.md` and `checklists/requirements.md` as requirements-review artifacts only; distinguish their checkbox state from runtime test results and do not claim the unresolved constitution item is passed.
- [x] T043 Confirm `/Analyze` was neither created nor modified, unrelated UI remains out of scope, the shared constitution remains unchanged, and no feature work expanded beyond the Home page primary action.

**Final checkpoint**: All implementation tests and acceptance checks pass, the walkthrough is explainable, scope is preserved, and the local constitution limitation remains explicitly unresolved.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies; establishes the test project and test tooling.
- **Phase 2 Foundational**: Depends on Phase 1; blocks all user stories because they require the model/repository/DI boundary.
- **Phase 3 User Story 1 (P1 MVP)**: Depends on Phase 2; establishes valid/invalid server submission and redirect behavior.
- **Phase 4 User Story 2 (P1)**: Depends on Phase 2 and integrates with the Home PageModel from Phase 3; its tests must fail before picker implementation.
- **Phase 5 User Story 3 (P2)**: Depends on Phases 2–4 because accessibility and styling apply to the completed form and picker.
- **Phase 6 Polish**: Depends on all desired user stories and their checkpoints.

### User Story Dependencies

- **US1**: Depends on Foundational; no dependency on the Analyze page implementation.
- **US2**: Depends on Foundational and the Home PageModel contract from US1; independently validates search and selection behavior.
- **US3**: Depends on the rendered controls from US1/US2; independently validates accessibility, responsive layout, and scoped styling.

### Parallel Opportunities

- T007 can run in parallel with T003 after the test project exists; T004 blocks test writing and browser setup.
- T005 and T006 can run in parallel after T004 succeeds and before foundational implementation.
- T015–T018 can run in parallel because they target separate test cases/files and all precede US1 implementation.
- T023–T025 can run in parallel because they cover independent browser search, rendered data/ID, and selection-clearing scenarios.
- T030–T032 can run in parallel because they cover keyboard/focus, computed contrast/association, and layout concerns.
- T037 and T038 can run in parallel after the story checkpoints; T042 and T043 can run in parallel during final review.

---

## Parallel Example: User Story 1

```text
T015: PageModel tests for supported business types and required business-type validation
T016: PageModel tests for missing/unsupported neighborhood validation
T017: Integration tests for the four supported business-type/neighborhood submissions
T018: Integration test for URL-encoded Analyze redirect values
```

## Parallel Example: User Story 2

```text
T023: Search result acceptance tests for exact, case-varied, substring, and trimmed searches
T024: Empty/whitespace, rendered-name, and unmatched-search acceptance tests
T025: Display/ID matching and selection-clearing acceptance test
```

## Parallel Example: User Story 3

```text
T030: Combobox/listbox keyboard and focus acceptance tests
T031: Accessibility association, computed contrast-state, and keyboard-trap acceptance tests
T032: Responsive and visual layout acceptance tests
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational and confirm repository/DI tests pass.
3. Complete Phase 3 User Story 1 and validate server-side submission plus redirect without implementing `/Analyze`.
4. Stop for an MVP walkthrough of the Home page primary action.

### Incremental Delivery

1. Add US1 server submission and redirect behavior.
2. Add US2 search, selection, and selection-clearing behavior.
3. Add US3 accessibility, responsive layout, and scoped styling.
4. Complete Phase 6 debugging, walkthrough, requirements-review distinction, and full acceptance verification.

### Notes

- `[P]` tasks can run in parallel only when they use different files or independent test cases and have no unmet dependency.
- Every test task explicitly precedes its corresponding implementation task and must be observed failing before implementation begins.
- Requirements-review checklist items are not implementation tests and do not replace the test suite or acceptance walkthrough.
- The local constitution is placeholder content managed by Priya on `main`; tasks carry the limitation forward and do not modify or claim compliance with it.
- No task creates or modifies `/Analyze`.
