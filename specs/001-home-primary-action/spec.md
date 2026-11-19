# Feature Specification: Home Page Primary Action

**Feature Branch**: `home-neighborhood-search-awarera`
**Created**: 2026-11-06
**Status**: Draft
**Input**: User description: Build the MetroBiz Advisor Home page primary action so users can choose a business type and neighborhood, search neighborhoods by name, and analyze the selected combination.

## Clarifications

### Session 2026-11-06

- Q: How should neighborhood search match names and handle spaces? → A: Use case-insensitive substring matching after trimming leading and trailing spaces; for example, `oak` finds `Oakley`.
- Q: What should happen when the user edits search text after selecting a neighborhood? → A: Clear the selection and require the user to select a result again.
- Q: How should the out-of-scope restrictions apply to future teammate work? → A: They restrict this feature's implementation only; teammates may add those components later.
- Q: How should SC-002 measure successful pairing and submission? → A: Verify that each of the four supported business types can be paired with an available neighborhood and submitted successfully.
- Q: What viewport and zoom define the desktop layout check? → A: 1366 × 768 at 100% browser zoom, with the primary form visible without scrolling.
- Q: What visual direction should guide the Home page primary action? → A: Use a compact dark navy header, a white primary action panel, and a red Analyze button; preserve technical styling details for the Plan stage.
- Q: What smaller viewport behavior is required? → A: Support viewports down to 320 CSS pixels without horizontal page scrolling; keep the desktop check at 1366 × 768 and 100% zoom.
- Q: How should empty and whitespace-only searches behave? → A: Show all available neighborhoods; a non-empty unmatched search shows zero results and `No neighborhoods found`.
- Q: What accessible interaction pattern should the neighborhood picker use? → A: Use a combobox/listbox pattern where Tab moves between form controls, Up/Down arrows navigate results, Enter selects, Escape closes results, Space remains available for typing, and keyboard focus is visible.
- Q: What measurable accessibility outcomes apply? → A: Require text contrast of at least 4.5:1 for normal text and 3:1 for large text, applicable control boundaries and state indicators of at least 3:1, visible focus, keyboard completion without a trap, and programmatically associated labels, validation, and status feedback.
- Q: How should duplicate seeded neighborhood names be handled? → A: Neighborhood names are unique in the initial seeded dataset; no duplicate-name disambiguation UI is required.
- Q: How is the Analyze destination owned? → A: `/Analyze` is teammate-owned, accepts `neighborhood` and `businessType` query parameters, and is not created or modified by this feature.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Analyze a Business Opportunity (Priority: P1)

As a prospective business owner, I want to choose a business type and neighborhood so that I can begin an analysis for that combination.

**Why this priority**: Starting an analysis is the primary purpose of the Home page and the entry point to the product experience.

**Independent Test**: Select one listed business type and one neighborhood, activate Analyze, and verify that the application opens the analysis destination with both selected values.

**Acceptance Scenarios**:

1. **Given** a business type and neighborhood are selected, **When** I click Analyze, **Then** I am sent to `/Analyze?neighborhood=<name>&businessType=<type>` with the selected neighborhood name and business type.
2. **Given** no neighborhood is selected, **When** I click Analyze, **Then** I am prompted to select a neighborhood and no analysis is started.

---

### User Story 2 - Find a Neighborhood (Priority: P1)

As a prospective business owner, I want to search neighborhoods by name so that I can quickly find the area I want to analyze.

**Why this priority**: Neighborhood selection is required before an analysis can start, and search makes the selection usable as the neighborhood list grows.

**Independent Test**: Enter exact, case-varied, and unmatched neighborhood names and verify the result list and empty-state message.

**Acceptance Scenarios**:

1. **Given** neighborhood data is available, **When** I search `Over-the-Rhine`, **Then** I see a result containing `Over-the-Rhine`.
2. **Given** neighborhood data is available, **When** I search `oakley` in any casing, **Then** I see `Oakley`.
3. **Given** neighborhood data is available, **When** I search `oak`, **Then** I see `Oakley` because matching is case-insensitive and uses a substring.
4. **Given** no neighborhood name matches `abcdefgh123`, **When** I search for it, **Then** I see zero results and the message `No neighborhoods found`.
5. **Given** the search is empty or contains only whitespace, **When** I search, **Then** I see all available neighborhoods.

---

### User Story 3 - Complete the Home Page Form Accessibly (Priority: P2)

As a Home page user, I want clear controls and validation messages so that I can complete the form and correct missing selections without confusion.

**Why this priority**: Clear feedback and accessible controls help all users successfully complete the primary task.

**Independent Test**: Navigate through the form using a keyboard and assistive technology, omit the required neighborhood, and verify that controls and validation feedback are understandable and discoverable.

**Acceptance Scenarios**:

1. **Given** the Home page is displayed, **When** I inspect the primary form controls, **Then** the business type, neighborhood search/selection, and Analyze action have visible labels and appropriate accessible names.
2. **Given** a required neighborhood is missing, **When** I submit the form, **Then** the validation message is associated with the neighborhood control and is perceivable by users of assistive technology.
3. **Given** the Home page is displayed, **When** I use only the keyboard, **Then** I can reach, operate, and understand the business type selector, neighborhood search/selection, and Analyze action.
4. **Given** a neighborhood is selected, **When** I edit the search text, **Then** the neighborhood selection is cleared and I must select a result again before analyzing.

### Edge Cases

- When the neighborhood search is empty, the user can see the available neighborhood choices rather than an unexplained error.
- When the neighborhood search contains only whitespace, it is treated as empty and shows all available neighborhoods.
- When the search text contains leading or trailing spaces, matching trims those spaces before applying case-insensitive substring matching.
- When a search has no matches, the result area shows zero results and `No neighborhoods found` without suggesting an invalid selection.
- When the user changes the search after selecting a neighborhood, the selected neighborhood is cleared and must be selected again.
- When the user submits without selecting a business type, the form prompts for the missing business type and does not start an analysis.
- Neighborhood names containing punctuation, including `Over-the-Rhine`, remain searchable by their displayed name.
- Neighborhood names are unique in the initial seeded dataset; duplicate-name disambiguation is not part of this feature.
- The neighborhood picker uses a combobox/listbox interaction: Tab moves between form controls, Up/Down arrows move through results, Enter selects a result, Escape closes the results, Space remains available for typing, and focus is visible.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The Home page MUST present a primary action form with a business type selector, neighborhood search and selection control, and Analyze action.
- **FR-002**: The business type selector MUST offer exactly these choices: Restaurant, Coffee Shop, Retail Store, and Fitness Center.
- **FR-003**: The neighborhood selector MUST provide available neighborhoods using each neighborhood's displayed name.
- **FR-004**: The neighborhood search MUST trim leading and trailing spaces from the entered text and match the remaining text as a case-insensitive substring of the neighborhood name.
- **FR-005**: The neighborhood search MUST show a result containing `Over-the-Rhine` when the user searches for `Over-the-Rhine`, assuming that neighborhood is available.
- **FR-006**: The neighborhood search MUST show `Oakley` when the user searches for `oakley`, regardless of the casing used.
- **FR-007**: When no neighborhood matches the search, the Home page MUST show zero results and the message `No neighborhoods found`.
- **FR-008**: The user MUST be required to select a neighborhood before an analysis can begin.
- **FR-009**: The user MUST be required to select a business type before an analysis can begin.
- **FR-010**: If the user activates Analyze without selecting a required value, the Home page MUST show a clear validation message for each missing value and MUST NOT start an analysis.
- **FR-011**: When both required values are selected and the user activates Analyze, the Home page MUST navigate to `/Analyze` with query parameters named `neighborhood` and `businessType` containing the selected neighborhood name and business type.
- **FR-012**: The Home page header MUST use a reduced height so the primary action form is prominent without requiring unnecessary scrolling.
- **FR-013**: The Home page MUST present a compact dark navy header, a white primary action panel, and a red Analyze button as the visual direction for this feature.
- **FR-014**: Form controls, search results, selection state, validation messages, and the empty-results message MUST have clear labels or accessible names, keyboard-operable behavior, and programmatically associated status or validation information.
- **FR-015**: The neighborhood data source MUST represent neighborhoods with an identifier and name and MUST be replaceable by a persistent data source without changing the Home page's user-facing behavior.
- **FR-016**: The initial neighborhood data MUST be available through a seeded in-memory source for this feature, including the neighborhoods needed to demonstrate `Over-the-Rhine` and `Oakley` searches.
- **FR-017**: The implementation of this feature MUST not include statistics cards, recommendations, recent analyses, crime data, permit data, or the Analyze page; teammates may add those components in later work.
- **FR-018**: The Home page MUST remain usable at viewport widths down to 320 CSS pixels without horizontal page scrolling.
- **FR-019**: The neighborhood picker MUST use a consistent combobox/listbox interaction in which Tab moves between form controls, Up/Down arrows navigate results, Enter selects a result, Escape closes the results, Space remains available for typing neighborhood names, and keyboard focus is visible.
- **FR-020**: The Home page MUST meet these measurable accessibility outcomes: at least 4.5:1 contrast for normal text, at least 3:1 contrast for large text, at least 3:1 contrast for applicable control boundaries and state indicators, visible focus, keyboard completion without a trap, and programmatically associated labels, validation, and status feedback.

### Key Entities *(include if feature involves data)*

- **Neighborhood**: A selectable geographic area available for analysis, identified by an Id and a display Name.
- **Business Type**: A supported business category that can be paired with a neighborhood for analysis.
- **Analysis Selection**: The user's selected business type and neighborhood name that are passed to the analysis destination.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In acceptance testing, 100% of the specified searches return the expected neighborhood result or the specified zero-result message.
- **SC-002**: Each of the four supported business types can be paired with an available neighborhood and submitted successfully in a repeatable acceptance check.
- **SC-003**: 100% of submitted forms missing a required selection display a clear, actionable validation message and do not start an analysis.
- **SC-004**: 100% of valid submissions open the analysis destination with both selected values preserved in the destination address.
- **SC-005**: In accessibility checks, all primary form controls and validation/empty-state feedback are keyboard operable and meet these measurable WCAG 2.1 AA baseline outcomes: text contrast is at least 4.5:1 for normal text and 3:1 for large text; applicable control boundaries and state indicators are at least 3:1 in default, focused, selected, and validation states; focus is visible during keyboard completion without a trap; and labels, validation, and status feedback are programmatically associated.
- **SC-006**: At 1366 × 768 viewport dimensions and 100% browser zoom, the primary form is visible without scrolling and users can identify and begin the primary analysis flow.
- **SC-007**: At viewport widths from 320 CSS pixels through the desktop acceptance width, the Home page does not require horizontal page scrolling.

## Assumptions

- The feature is for unauthenticated users unless the existing application establishes another access rule.
- The initial neighborhood source is seeded in memory and exposed behind a replaceable repository boundary; the source may later be changed to a database without changing the user-facing requirements.
- The neighborhood data includes at least `Over-the-Rhine` and `Oakley` for the specified acceptance scenarios.
- Neighborhood search trims leading and trailing spaces, then performs case-insensitive substring matching against the neighborhood name.
- Empty or whitespace-only search shows all available neighborhoods; a non-empty unmatched search shows zero results and `No neighborhoods found`.
- Editing search text after a neighborhood is selected clears the selection; the user must select a result again before submitting.
- Neighborhood names are unique in the initial seeded dataset because the destination contract uses neighborhood names; duplicate-name disambiguation UI is out of scope.
- Validation messages use plain, actionable language and appear near the relevant control.
- The neighborhood picker follows the specified combobox/listbox keyboard interaction and visible-focus behavior.
- The Home page meets the specified contrast, keyboard completion, focus, and programmatic association outcomes.
- The Home page follows the visual direction of a compact dark navy header, a white primary action panel, and a red Analyze button; technical styling details are deferred to the Plan stage.
- Statistics cards, recommendations, recent analyses, the Analyze page, crime data, and permit data are excluded from this feature's implementation but may be added by teammates in later work.
- `/Analyze` is teammate-owned and accepts the neighborhood name and business type through the specified query parameter names; this feature does not create or modify that page.
