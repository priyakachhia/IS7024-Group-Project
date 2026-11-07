# Feature Specification: Neighborhood Analysis

**Feature Branch**: `001-neighborhood-analysis`

**Created**: 2026-11-05

**Status**: Draft

**Input**: User description: Create a feature spec for "Neighborhood Analysis" (Requirement 101) in MetroBiz Advisor.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Analyze Neighborhood Suitability (Priority: P1)

A user selects a Cincinnati neighborhood, business type, and date range to understand the neighborhood's safety and growth conditions before deciding whether it is suitable for a new business.

**Why this priority**: This is the core value of Requirement 101: provide a single, understandable suitability assessment from current public safety and development activity.

**Independent Test**: Select a supported neighborhood, business type, and date range, submit the analysis, and verify that crime incidents, building permits, and a suitability score are displayed.

**Acceptance Scenarios**:

1. **101.1 — Given** the user selects Over-the-Rhine, Restaurant, and January 1 through December 31, 2025, **When** the user clicks Analyze, **Then** the page shows 412 reported crime incidents, 87 building permits, and a suitability score of 72.
2. **101.2 — Given** the user selects Oakley, Coffee Shop, and January 1 through December 31, 2025, **When** the user clicks Analyze, **Then** the page shows 138 reported crime incidents, 41 building permits, and a suitability score of 64.

---

### User Story 2 - Use Default Analysis Period (Priority: P2)

A user can begin an analysis without manually entering dates because the date range defaults to the last full calendar year: January 1 through December 31, 2025.

**Why this priority**: A useful default reduces friction and makes the primary analysis flow immediately usable while preserving the ability to choose another period.

**Independent Test**: Open the analysis page and verify that the From date is January 1, 2025 and the To date is December 31, 2025 before any user changes are made.

**Acceptance Scenarios**:

1. **Given** the user opens the analysis page, **When** the form is displayed, **Then** the From date defaults to January 1, 2025 and the To date defaults to December 31, 2025.
2. **Given** the user selects a neighborhood, business type, and valid custom date range, **When** the user clicks Analyze, **Then** the results use the selected date range.

---

### User Story 3 - Receive Clear Validation and Availability Feedback (Priority: P1)

A user receives a clear message when required input is missing or crime data cannot be retrieved, rather than seeing an incomplete or misleading suitability result.

**Why this priority**: Preventing an unsupported score is essential to user trust in the analysis.

**Independent Test**: Submit the form without a neighborhood, then repeat with the crime data source unavailable, and verify the specified messages and score behavior.

**Acceptance Scenarios**:

1. **101.3 — Given** no neighborhood is selected, **When** the user clicks Analyze, **Then** the page prompts the user with "Please select a neighborhood".
2. **101.4 — Given** the crime data source is unreachable, **When** the user clicks Analyze with otherwise valid selections, **Then** the page displays "Crime data is currently unavailable. Please try again later." and does not display a suitability score.

---

### Edge Cases

- If the selected date range has no matching records, the page displays zero crime incidents and zero building permits and calculates the score using those values.
- If the From date is later than the To date, the analysis is not performed and the user is told to provide a valid date range.
- If crime or permit counts exceed the scoring limits, Safety cannot be less than 0 and Growth cannot exceed 100.
- If the crime data source is unavailable, no score is shown even if permit data is available.
- If permit data is unavailable, the page must not present an unqualified score; the user receives a data availability message.
- Business type selection is limited to Restaurant, Coffee Shop, and Retail.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST allow the user to select one Cincinnati neighborhood.
- **FR-002**: The system MUST allow the user to select one business type from Restaurant, Coffee Shop, and Retail.
- **FR-003**: The system MUST provide From and To dates for the analysis period.
- **FR-004**: The system MUST default the analysis period to January 1, 2025 through December 31, 2025, representing the last full calendar year for this release.
- **FR-005**: When the user submits a valid selection, the system MUST count reported crime incidents for the selected neighborhood and date range.
- **FR-006**: When the user submits a valid selection, the system MUST count building permits for the selected neighborhood and date range.
- **FR-007**: The system MUST display the reported crime count and building permit count for the selected neighborhood and period.
- **FR-008**: The system MUST calculate Safety as 100 minus the crime count divided by 8, with a minimum value of 0.
- **FR-009**: The system MUST calculate Growth as the building permit count, with a maximum value of 100.
- **FR-010**: The system MUST calculate the suitability score as Safety multiplied by the business-type weight plus Growth multiplied by one minus the business-type weight, then round the result to the nearest whole number.
- **FR-011**: The system MUST use a Safety weight of 0.4 for Restaurant, 0.55 for Coffee Shop, and 0.7 for Retail.
- **FR-012**: The system MUST display the suitability score on a 0-to-100 scale when both required data sets are successfully obtained.
- **FR-013**: If no neighborhood is selected, the system MUST display "Please select a neighborhood" and must not perform the analysis.
- **FR-014**: If the crime data source cannot be reached, the system MUST display "Crime data is currently unavailable. Please try again later." and must not display a suitability score.
- **FR-015**: The system MUST obtain crime data from the public Cincinnati crime dataset at `https://data.cincinnati-oh.gov/resource/7aqy-xrv9.json`.
- **FR-016**: The system MUST obtain building permit data from the public Cincinnati building permits dataset at `https://data.cincinnati-oh.gov/resource/uhjb-xac9.json`.
- **FR-017**: The system MUST exclude charts, neighborhood comparison, and saving analysis results to the database from this feature.

### Key Entities *(include if feature involves data)*

- **Neighborhood**: A Cincinnati neighborhood available for selection, identified by its display name.
- **Business Type**: The selected business category—Restaurant, Coffee Shop, or Retail—and its corresponding Safety weight.
- **Analysis Period**: The inclusive From and To dates used to filter both public datasets.
- **Crime Record**: A reported crime incident associated with a neighborhood and date.
- **Building Permit**: A permit record associated with a neighborhood and date.
- **Neighborhood Analysis**: The result for one neighborhood, business type, and period, including crime count, permit count, Safety, Growth, and rounded suitability score.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: For the two defined reference scenarios, the displayed crime count, permit count, and rounded score exactly match the specified acceptance results: Over-the-Rhine/Restaurant = 412/87/72 and Oakley/Coffee Shop = 138/41/64.
- **SC-002**: At least 95% of valid analysis submissions display both source counts and a suitability score without requiring the user to resubmit.
- **SC-003**: 100% of submissions without a neighborhood display "Please select a neighborhood" and do not produce an analysis result.
- **SC-004**: 100% of submissions made while the crime source is unavailable display the specified availability message and no suitability score.
- **SC-005**: A user can complete a valid neighborhood analysis in no more than three actions after opening the page: choose the required selections, optionally adjust dates, and click Analyze.

## Assumptions

- Users understand that reported crime incidents and building permits are indicators, not a guarantee of business success.
- Neighborhood names in both public datasets can be matched to the selectable Cincinnati neighborhood names.
- Date filtering is inclusive of both the From and To dates.
- The public Cincinnati Socrata datasets remain available without an authentication key and provide the fields needed to identify neighborhood and date.
- The last full calendar year for the initial release is January 1, 2025 through December 31, 2025.
- The analysis is calculated for the current request and is not saved to the database.
- Charts and comparison workflows are out of scope for this release.
