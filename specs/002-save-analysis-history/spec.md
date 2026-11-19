# Feature Specification: Save Analysis History

**Feature Branch**: `002-save-analysis-history`

**Created**: 2026-11-19

**Status**: Draft

**Input**: User description: "Add a Save Analysis and History feature to MetroBiz Advisor (supports Requirement 103)."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Save a completed analysis (Priority: P1)

As a business advisor, I want to save the results of a completed neighborhood analysis so that I can refer to the decision later.

**Why this priority**: Persisting an analysis is the core value of the feature and enables every other history experience.

**Independent Test**: Complete an analysis, select Save Analysis, and verify that a confirmation appears and a new saved record is available in history.

**Acceptance Scenarios**:

1. **Given** an analysis displays results, **When** the user selects Save Analysis, **Then** the system saves the neighborhood, business type, date range, crime count, permit count, safety score, growth score, suitability score, and save date and time.
2. **Given** an analysis has just been saved, **When** the save completes, **Then** the user sees a clear confirmation announced to assistive technology.
3. **Given** an analysis has been saved once, **When** the user selects Save Analysis again, **Then** the system creates another saved entry rather than replacing the first.

---

### User Story 2 - Review saved analyses (Priority: P2)

As a business advisor, I want to open a History page and review my saved analyses so that I can compare prior decisions over time.

**Why this priority**: A readable history makes saved analyses useful after the original analysis session.

**Independent Test**: Save multiple analyses, open History from the top menu, and verify that all entries appear newest first with the required summary fields.

**Acceptance Scenarios**:

1. **Given** saved analyses exist, **When** the user opens History, **Then** a real table shows neighborhood, business type, period, suitability score, and saved date in newest-first order.
2. **Given** no saved analyses exist, **When** the user opens History, **Then** the page shows "No saved analyses found" and announces the empty state to screen readers.

---

### User Story 3 - Dependable local history (Priority: P3)

As a user, I want saved analyses to remain available after restarting the application so that my history is dependable without requiring a separate database server.

**Why this priority**: Persistence and clear validation protect the trustworthiness of saved decision records.

**Independent Test**: Save a valid record, restart the application, and request History; then submit invalid data and verify an error response with no new record.

**Acceptance Scenarios**:

1. **Given** a valid saved analysis exists, **When** the application restarts, **Then** the analysis remains available in History.
2. **Given** invalid analysis data is submitted, **When** the save request is processed, **Then** the system returns a clear validation error and does not save the invalid record.

---

### Edge Cases

- An analysis cannot be saved before results are available; the Save Analysis action is not shown until results are displayed.
- Missing, malformed, out-of-range, or otherwise invalid analysis fields are rejected as one request and do not create a partial record.
- Repeated valid save requests for the same analysis create distinct entries with their own save timestamps.
- History with a single entry still renders table headers and the complete row.
- A save or history request failure presents a clear user-facing error without claiming that the operation succeeded.
- The save action, form controls, status messages, and history table can all be operated and understood using keyboard navigation and assistive technology.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The Analyze page MUST show a Save Analysis button only after an analysis has displayed results.
- **FR-002**: Saving an analysis MUST record the neighborhood, business type, date range, crime count, permit count, safety score, growth score, suitability score, and the date and time of saving.
- **FR-003**: Each valid activation of Save Analysis MUST create a new saved analysis entry, including when its values match an existing entry.
- **FR-004**: The save operation MUST be available through `POST /api/history` and return a clear success or validation/error response.
- **FR-005**: The history operation MUST be available through `GET /api/history` and return saved analyses as JSON ordered from newest to oldest.
- **FR-006**: The system MUST reject missing, malformed, or invalid analysis data with a clear error and MUST NOT persist the rejected entry.
- **FR-007**: Saved analyses MUST be stored locally and remain available after the application restarts without requiring a separate database server.
- **FR-008**: A History page MUST be reachable from the top navigation menu.
- **FR-009**: The History page MUST display a real table with headers for neighborhood, business type, period, suitability score, and saved date.
- **FR-010**: When no entries exist, the History page MUST display exactly "No saved analyses found".
- **FR-011**: The save confirmation and empty-history message MUST be announced to screen readers, and controls MUST have clear labels and keyboard support.
- **FR-012**: The feature MUST NOT provide editing, deleting, user accounts, or Compare page functionality.

### Key Entities *(include if feature involves data)*

- **Saved Analysis**: A completed neighborhood analysis captured for later review, including the analysis inputs, counts, scores, and save timestamp.
- **Analysis Period**: The date range covered by the analysis, shown as a period in history.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In acceptance testing, 100% of valid save attempts create one and only one new history entry.
- **SC-002**: In acceptance testing, 100% of saved entries retain all nine required analysis values plus the save date and time after an application restart.
- **SC-003**: In acceptance testing, 100% of history responses list entries newest first and expose all required summary columns.
- **SC-004**: In acceptance testing, 100% of invalid save attempts return a clear error and leave the saved-entry count unchanged.
- **SC-005**: Users can reach History from the top menu and identify the empty state or a saved row using keyboard-only navigation.

## Assumptions

- The feature is for the existing MetroBiz Advisor user experience and does not distinguish records by user account.
- A saved date and time use the application’s standard local date/time presentation; ordering uses the full save timestamp.
- The existing analysis result values are authoritative; this feature does not recalculate scores.
- Local storage is available to the running application and is initialized automatically without manual server setup.
- The History page is read-only for this feature; editing and deleting are explicitly out of scope.

