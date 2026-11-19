# Tasks: Neighborhood Analysis

**Input**: Design documents from `/specs/001-neighborhood-analysis/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/neighborhood-analysis.openapi.yaml`, `quickstart.md`

## Phase 1: Setup and Confirming API Field Names

**Purpose**: Establish the test project and verify the configured Socrata schema before implementation.

- [ ] T001 Create the `Cincy Biz Advisor.Tests` xUnit project in `Cincy Biz Advisor.Tests/Cincy Biz Advisor.Tests.csproj` and reference the web project.
- [ ] T002 Add the test project to `Cincy Biz Advisor.slnx`.
- [ ] T003 [P] Add shared test packages and nullable/implicit-using settings to `Cincy Biz Advisor.Tests/Cincy Biz Advisor.Tests.csproj`.
- [ ] T004 [P] Confirm the crime dataset fields `cpd_neighborhood` and `date_reported` against the Socrata metadata or a permitted sample request, recording the result in `specs/001-neighborhood-analysis/research.md`.
- [ ] T005 [P] Confirm the permit dataset fields `neighborhood` and `issueddate` against the Socrata metadata or a permitted sample request, recording the result in `specs/001-neighborhood-analysis/research.md`.
- [ ] T006 Update `Cincy Biz Advisor/appsettings.json` with the confirmed `CityData` dataset IDs, field names, and timeout values.

## Phase 2: Models and Scoring — Write Tests First

**Purpose**: Define the request/result/configuration models and lock down the pure scoring behavior before implementation.

- [ ] T007 Create the analysis request, result, business-type, and count-response models in `Cincy Biz Advisor/Services/AnalysisModels.cs`.
- [ ] T008 Create `CityDataOptions` with dataset IDs, field names, and timeout properties in `Cincy Biz Advisor/Services/CityDataOptions.cs`.
- [ ] T009 Write failing scoring tests for Safety and Growth bounds in `Cincy Biz Advisor.Tests/SuitabilityCalculatorTests.cs`.
- [ ] T010 Write failing scoring tests for Restaurant, Coffee Shop, and Retail weights in `Cincy Biz Advisor.Tests/SuitabilityCalculatorTests.cs`.
- [ ] T011 Write failing scoring tests for nearest-integer rounding and the 0–100 score range in `Cincy Biz Advisor.Tests/SuitabilityCalculatorTests.cs`.
- [ ] T012 Write failing reference-case tests for 412 crimes/87 permits = 72 and 138 crimes/41 permits = 64 in `Cincy Biz Advisor.Tests/SuitabilityCalculatorTests.cs`.
- [ ] T013 Implement the static pure calculation methods in `Cincy Biz Advisor/Services/SuitabilityCalculator.cs` until the scoring tests pass.

## Phase 3: City Data Service

**Purpose**: Encapsulate Socrata count queries and upstream failure translation.

- [ ] T014 Define `ICityDataService` count methods in `Cincy Biz Advisor/Services/ICityDataService.cs`.
- [ ] T015 Define `CityDataUnavailableException` and its safe crime-data message in `Cincy Biz Advisor/Services/CityDataUnavailableException.cs`.
- [ ] T016 Write failing fake-handler tests for crime and permit dataset URLs in `Cincy Biz Advisor.Tests/CityDataServiceTests.cs`.
- [ ] T017 Write failing fake-handler tests for `$select=count(*) AS total` and URL-encoded `$where` clauses in `Cincy Biz Advisor.Tests/CityDataServiceTests.cs`.
- [ ] T018 Write failing fake-handler tests for uppercase neighborhood matching and the inclusive-start/exclusive-next-day date bounds in `Cincy Biz Advisor.Tests/CityDataServiceTests.cs`.
- [ ] T019 Write failing fake-handler tests for malformed responses, non-success responses, and request timeouts in `Cincy Biz Advisor.Tests/CityDataServiceTests.cs`.
- [ ] T020 Implement `CityDataService` as a typed `HttpClient` using `CityDataOptions` in `Cincy Biz Advisor/Services/CityDataService.cs`.
- [ ] T021 Make `CityDataService` parse the single `total` response and translate upstream failures to `CityDataUnavailableException` in `Cincy Biz Advisor/Services/CityDataService.cs`.

## Phase 4: Neighborhood Analysis Service

**Purpose**: Combine both counts and calculate a result without producing partial scores.

- [ ] T022 Define `INeighborhoodAnalysisService` and its request/result contract in `Cincy Biz Advisor/Services/INeighborhoodAnalysisService.cs`.
- [ ] T023 Write failing service tests for valid concurrent crime and permit requests in `Cincy Biz Advisor.Tests/NeighborhoodAnalysisServiceTests.cs`.
- [ ] T024 Write failing service tests proving both requests start before the combined result completes in `Cincy Biz Advisor.Tests/NeighborhoodAnalysisServiceTests.cs`.
- [ ] T025 Write failing service tests for invalid dates, unsupported business types, and unavailable crime data in `Cincy Biz Advisor.Tests/NeighborhoodAnalysisServiceTests.cs`.
- [ ] T026 Implement `NeighborhoodAnalysisService` with `Task.WhenAll` and `SuitabilityCalculator` in `Cincy Biz Advisor/Services/NeighborhoodAnalysisService.cs`.
- [ ] T027 Ensure `NeighborhoodAnalysisService` returns no score when either required data request fails in `Cincy Biz Advisor/Services/NeighborhoodAnalysisService.cs`.

## Phase 5: Program.cs Wiring and Optional JSON Endpoint

**Purpose**: Register configuration/services and expose the optional shared analysis endpoint.

- [ ] T028 Bind and validate the `CityData` section in `Cincy Biz Advisor/Program.cs`.
- [ ] T029 Register the typed `ICityDataService` client with the configured timeout in `Cincy Biz Advisor/Program.cs`.
- [ ] T030 Register `INeighborhoodAnalysisService` and `SuitabilityCalculator` dependencies in `Cincy Biz Advisor/Program.cs`.
- [ ] T031 Write failing endpoint tests for 200, 400, 404, and 503 responses in `Cincy Biz Advisor.Tests/AnalysisEndpointTests.cs`.
- [ ] T032 Map `GET /api/neighborhoods/{name}/analysis` and its query parameters in `Cincy Biz Advisor/Program.cs`.
- [ ] T033 Implement endpoint response mapping against `contracts/neighborhood-analysis.openapi.yaml` in `Cincy Biz Advisor/Program.cs`.

## Phase 6: Razor Page and CSS

**Purpose**: Deliver the accessible POST form and same-page analysis results.

- [ ] T034 Create `AnalyzeModel` with supported neighborhoods, business types, date defaults, validation state, and result state in `Cincy Biz Advisor/Pages/Analyze.cshtml.cs`.
- [ ] T035 Write failing PageModel tests for January 1–December 31, 2025 defaults in `Cincy Biz Advisor.Tests/AnalyzePageTests.cs`.
- [ ] T036 Write failing PageModel tests for missing-neighborhood validation and no service call in `Cincy Biz Advisor.Tests/AnalyzePageTests.cs`.
- [ ] T037 Write failing PageModel tests for successful results and the crime-unavailable message with no score in `Cincy Biz Advisor.Tests/AnalyzePageTests.cs`.
- [ ] T038 Implement the GET and POST handlers in `Cincy Biz Advisor/Pages/Analyze.cshtml.cs`.
- [ ] T039 Create the labeled neighborhood, business type, From, and To controls and Analyze form in `Cincy Biz Advisor/Pages/Analyze.cshtml`.
- [ ] T040 Add `role="alert"` error rendering and `aria-live` result rendering in `Cincy Biz Advisor/Pages/Analyze.cshtml`.
- [ ] T041 Create feature-specific layout and result styles in `Cincy Biz Advisor/wwwroot/css/analyze.css` without inline CSS.
- [ ] T042 Reference `analyze.css` from the Razor Page and verify keyboard-accessible form controls in `Cincy Biz Advisor/Pages/Analyze.cshtml`.

## Phase 7: Debugging and Verification

**Purpose**: Validate the complete feature against the specification and configured public data behavior.

- [ ] T043 Run the complete xUnit suite and fix compilation or test failures in the affected files.
- [ ] T044 Run the application and verify the default form, labels, alerts, and live results on `/Analyze`.
- [ ] T045 Verify acceptance scenario 101.1 with controlled responses: Over-the-Rhine, Restaurant, 2025 dates, 412 crimes, 87 permits, score 72.
- [ ] T046 Verify acceptance scenario 101.2 with controlled responses: Oakley, Coffee Shop, 2025 dates, 138 crimes, 41 permits, score 64.
- [ ] T047 Verify acceptance scenarios 101.3 and 101.4 for missing neighborhood and unavailable crime data.
- [ ] T048 Run a permitted live-data smoke test to confirm the configured crime and permit fields and query boundaries.
- [ ] T049 Run `dotnet build` for the solution and resolve all feature-related errors and warnings.
- [ ] T050 Run the quickstart steps in `specs/001-neighborhood-analysis/quickstart.md` and update the documentation for any discovered differences.

## Phase 8: Pull Request

**Purpose**: Prepare a reviewable, traceable change.

- [ ] T051 Review the diff for scope compliance with `spec.md`, including out-of-scope charts, comparison, and persistence.
- [ ] T052 Review accessibility, error-message text, score suppression, and configuration defaults before commit.
- [ ] T053 Commit the implementation and tests with a focused Neighborhood Analysis message.
- [ ] T054 Create the pull request with links to `spec.md`, `plan.md`, `tasks.md`, test results, and any live API verification notes.
- [ ] T055 Respond to review feedback and rerun the affected tests before merge.

## Dependencies and Execution Order

### Phase Dependencies

- **Phase 1** has no dependencies and establishes the test project, schema confirmation, and configuration.
- **Phase 2** depends on Phase 1; its models and failing-first scoring tests block service work.
- **Phase 3** depends on Phase 2 models/options and blocks the analysis service.
- **Phase 4** depends on Phase 3 and blocks the page and endpoint integration.
- **Phase 5** depends on Phase 4 for service registration and endpoint behavior.
- **Phase 6** depends on Phase 4 and can proceed in parallel with Phase 5 after the shared service contracts exist.
- **Phase 7** depends on Phases 5 and 6.
- **Phase 8** depends on Phase 7 passing.

### Story Traceability

- **User Story 1 (P1)**: T007–T027, T034–T042, T045–T046
- **User Story 2 (P2)**: T034–T035, T038–T042
- **User Story 3 (P1)**: T015, T019, T025, T027, T036–T037, T047

### Parallel Opportunities

- T003–T005 can run in parallel after the test project exists.
- T009–T012 can run in parallel because they target independent scoring cases in the same test file and should all fail before T013.
- T016–T019 can be divided among contributors after T014–T015 define the contracts.
- T023–T025 can run in parallel after T022 defines the analysis contract.
- T028–T030 can be prepared in parallel, then reviewed together in `Program.cs`.
- T035–T037 can run in parallel before T038 implements the PageModel.
- T044–T048 are separate verification activities after the application builds.

## Implementation Strategy

### MVP First

1. Complete Phases 1–4 to establish models, scoring, Socrata access, and analysis orchestration.
2. Complete Phase 6 for the Razor Page and accessible result flow.
3. Stop and validate scenarios 101.1–101.4 before adding the optional endpoint.
4. Complete Phase 5 endpoint work if the optional JSON surface is required for the release.

### Incremental Delivery

1. Deliver the scoring and fake-handler tests first.
2. Add the typed data client and concurrent analysis service.
3. Add the Razor Page and CSS as the primary user-facing slice.
4. Add and verify the optional JSON endpoint.
5. Finish debugging, documentation, and pull request review.

### Notes

- Every task is intentionally small and references its target path.
- Tasks marked `[P]` can be performed in parallel when they do not modify the same code region.
- Tests marked failing-first must be observed failing before the corresponding implementation task is completed.
- The crime field confirmation is an explicit prerequisite because the public API was not reachable during planning.
