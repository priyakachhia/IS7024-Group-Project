# Tasks: Save Analysis History

**Input**: Design documents from `/specs/002-save-analysis-history/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/history-api.md`, `quickstart.md`

## Phase 1: Setup

**Purpose**: Add the requested SQLite dependencies and repository exclusions without changing runtime behavior.

- [ ] T001 Add `Microsoft.EntityFrameworkCore.Sqlite` to `Cincy Biz Advisor/Cincy Biz Advisor.csproj`.
- [ ] T002 [P] Add `Microsoft.EntityFrameworkCore.Sqlite` to `Cincy_Biz_Advisor.Tests/Cincy_Biz_Advisor.Tests.csproj` for SQLite in-memory tests.
- [ ] T003 [P] Add the application data directory and SQLite database filename pattern to `.gitignore`.

## Phase 2: Foundational Persistence and Contracts

**Purpose**: Establish the entity, validated DTO contract, EF Core context, service abstraction, and application wiring required by all user stories.

- [ ] T004 Create `HistoryEntry` with the analysis fields and generated save timestamp in `Cincy Biz Advisor/Models/HistoryEntry.cs`.
- [ ] T005 Create validated request and response DTOs in `Cincy Biz Advisor/Models/HistoryDtos.cs`, including required neighborhood, supported business type, score ranges, non-negative counts, and period ordering validation.
- [ ] T006 Create `HistoryDbContext` and configure the `HistoryEntry` schema in `Cincy Biz Advisor/Data/HistoryDbContext.cs`.
- [ ] T007 Define `IHistoryService` with `AddAsync` and newest-first `GetAllAsync` in `Cincy Biz Advisor/Services/IHistoryService.cs`.
- [ ] T008 Register the SQLite DbContext, `IHistoryService`, controllers, and Razor Pages in `Cincy Biz Advisor/Program.cs`.
- [ ] T009 Initialize the SQLite database automatically on first application run in `Cincy Biz Advisor/Program.cs`, using the application data folder and `EnsureCreatedAsync`.

## Phase 3: User Story 1 - Save a Completed Analysis (Priority: P1)

**Goal**: Save every completed analysis through the validated API and provide accessible feedback from Analyze.

**Independent Test**: POST a valid analysis, receive `201 Created` with the saved entry, repeat the POST to confirm two entries exist, then use the Analyze page Save Analysis button and verify the success message.

### Tests for User Story 1

- [ ] T010 [P] [US1] Write SQLite in-memory service tests for adding a complete entry and returning its generated save timestamp in `Cincy_Biz_Advisor.Tests/HistoryServiceTests.cs`.
- [ ] T011 [P] [US1] Write SQLite in-memory service tests proving `GetAllAsync` returns entries newest first and duplicate valid entries are preserved in `Cincy_Biz_Advisor.Tests/HistoryServiceTests.cs`.
- [ ] T012 [P] [US1] Write controller tests for valid POST `201 Created`, invalid model `400 Bad Request`, and no service call when validation fails in `Cincy_Biz_Advisor.Tests/HistoryControllerTests.cs`.

### Implementation for User Story 1

- [ ] T013 [US1] Implement `HistoryService.AddAsync` and `GetAllAsync` with EF Core persistence and descending save timestamp ordering in `Cincy Biz Advisor/Services/HistoryService.cs`.
- [ ] T014 [US1] Create `HistoryController` at `/api/history` with validated POST and newest-first GET endpoints in `Cincy Biz Advisor/Controllers/HistoryController.cs`.
- [ ] T015 [US1] Add the result-only Save Analysis button, JSON result payload, and `aria-live` confirmation/error region to `Cincy Biz Advisor/Pages/Analyze.cshtml`.
- [ ] T016 [US1] Implement Analyze save handling with `fetch` to POST `/api/history`, clear success/error messaging, and keyboard-accessible behavior in `Cincy Biz Advisor/wwwroot/js/history.js`.
- [ ] T017 [US1] Add Analyze save presentation styles using only `history-` prefixed classes in `Cincy Biz Advisor/wwwroot/css/history.css` and reference the assets from the page.

## Phase 4: User Story 2 - Review Saved Analyses (Priority: P2)

**Goal**: Provide a navigable History page that loads JSON and displays a real accessible table or the required empty message.

**Independent Test**: With zero records, open History and verify the announced empty message; with multiple records, verify the table headers and newest-first rows after a GET request.

- [ ] T018 [P] [US2] Write controller GET tests for `200 OK`, an empty JSON list, and newest-first response data in `Cincy_Biz_Advisor.Tests/HistoryControllerTests.cs`.
- [ ] T019 [US2] Create the Razor Pages shell with page title, `aria-live` empty/error region, and real table header structure in `Cincy Biz Advisor/Pages/History.cshtml`.
- [ ] T020 [US2] Implement History GET rendering, empty-state handling, date/period formatting, and keyboard/screen-reader labels in `Cincy Biz Advisor/wwwroot/js/history.js`.
- [ ] T021 [US2] Add the History navigation link and active-state handling in `Cincy Biz Advisor/Pages/Shared/_Layout.cshtml`.
- [ ] T022 [US2] Add table, empty-state, and error styles using only `history-` prefixed classes in `Cincy Biz Advisor/wwwroot/css/history.css`.

## Phase 5: User Story 3 - Dependable Local History (Priority: P3)

**Goal**: Verify that valid entries survive application lifecycle boundaries and invalid requests never persist data.

**Independent Test**: Save a valid entry to a file-backed SQLite database, recreate the service/context, and verify the entry remains; submit invalid values and verify the entry count is unchanged.

- [ ] T023 [P] [US3] Add a SQLite persistence test that recreates the context against the same temporary database and verifies saved history remains in `Cincy_Biz_Advisor.Tests/HistoryServiceTests.cs`.
- [ ] T024 [P] [US3] Add controller validation tests for negative counts, scores outside 0–100, unsupported business type, missing neighborhood, and period end before period start in `Cincy_Biz_Advisor.Tests/HistoryControllerTests.cs`.
- [ ] T025 [US3] Verify database initialization and data-folder path behavior in the application startup configuration in `Cincy Biz Advisor/Program.cs`.

## Phase 6: Polish and Cross-Cutting Validation

**Purpose**: Validate the complete feature, accessibility requirements, and existing behavior.

- [ ] T026 [P] Add or update focused page/client markup tests or manual acceptance checks for Save Analysis visibility, `aria-live` announcements, real table headers, and keyboard navigation.
- [ ] T027 [P] Verify all new CSS selectors in `Cincy Biz Advisor/wwwroot/css/history.css` begin with `history-` and no unrelated page styles are changed.
- [ ] T028 Run the complete xUnit test suite and resolve regressions in existing tests.
- [ ] T029 Run a solution build and verify the generated SQLite database is excluded from source control.
- [ ] T030 Validate the scenarios in `specs/002-save-analysis-history/quickstart.md` against the running Razor Pages application.

---

## Dependencies and Execution Order

### Phase Dependencies

- **Phase 1**: No dependencies; can start immediately.
- **Phase 2**: Depends on Phase 1; blocks all user stories.
- **Phase 3 (US1)**: Depends on Phase 2; delivers the MVP save flow.
- **Phase 4 (US2)**: Depends on Phase 2 and the API from US1; adds the History UI.
- **Phase 5 (US3)**: Depends on Phase 2 and persistence implementation from US1; verifies restart durability and validation coverage.
- **Phase 6**: Depends on all desired user stories.

### User Story Dependencies

- **US1 (P1)**: Starts after Phase 2 and is the MVP. Its API is required by the client integrations.
- **US2 (P2)**: Starts after Phase 2; depends on the shared GET API from US1 for runtime integration.
- **US3 (P3)**: Starts after Phase 2; depends on the service and database wiring from US1.

### Parallel Opportunities

- T002 and T003 can run in parallel with T001.
- T004, T005, T006, and T007 can be developed in parallel before wiring.
- T010, T011, and T012 can be written in parallel before their implementations.
- T018 can be written in parallel with T019.
- T023 and T024 can be written in parallel.
- T026 and T027 can run in parallel after UI implementation.

## Implementation Strategy

### MVP First

1. Complete Phases 1 and 2.
2. Complete US1 tests and implementation.
3. Run US1 tests and verify POST/save behavior independently.
4. Add US2 for history browsing.
5. Add US3 persistence and expanded validation coverage.
6. Run the full suite, build, and quickstart validation.

### Test-First Guidance

For each story, write the listed tests before the corresponding implementation tasks. Keep service tests on SQLite in-memory, use fake `IHistoryService` instances for controller tests, and avoid requiring the application’s persistent database during unit tests.

### Notes

- `[P]` indicates tasks that can proceed in parallel without editing the same file.
- `[US1]`, `[US2]`, and `[US3]` provide traceability to the prioritized specification stories.
- The feature intentionally excludes edit, delete, accounts, and Compare functionality.
