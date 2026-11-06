# Feature Specification: Compare Neighborhoods

**Feature Branch**: `feature/compare-neighborhoods-kachhipr`
**Created**: 2026-11-06
**Status**: Draft
**Input**: User description: "Create a feature specification for Compare Neighborhoods in Metro Biz Advisor. The feature should let users select a business type and two different neighborhoods, compare crime metrics, building permit activity, and location suitability, and view three comparison charts plus a comparison table. Follow the existing Compare page mockup. Use available datasets rather than invented metrics, and keep the feature separate from the Analyze page being developed by another teammate."

## Clarifications

### Session 2026-11-06

- Q: How should users choose neighborhoods for comparison? → A: Select from the available canonical neighborhood list.
- Q: How should the Compare page determine which business types users can select? → A: Select from the available canonical business-type list.
- Q: If one requested source dataset has no matching data for a selected neighborhood or period, should the Compare page still show the other available measures? → A: Show available measures and mark only missing measures as unavailable.
- Q: How should crime and building permit activity be summarized when the source datasets contain multiple records or reporting periods? → A: Use the latest complete common reporting period and source-provided total counts.
- Q: How should location suitability be determined for the comparison? → A: Reuse the existing available suitability score or result and do not recalculate it.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Compare Two Neighborhoods (Priority: P1)

As a prospective business owner, I want to select a business type and two different Cincinnati neighborhoods so that I can evaluate the locations side by side before choosing where to open my business.

**Why this priority**: Selecting valid comparison inputs and seeing a useful comparison is the core value of the Compare page.

**Independent Test**: Select a supported business type, select two different neighborhoods, choose Compare, and verify that both neighborhoods appear in a side-by-side comparison with the requested measures.

**Acceptance Scenarios**:

1. **Given** neighborhood and business type options are available, **When** I select a business type and two different neighborhoods and choose Compare, **Then** I see a comparison labeled with the selected business type and both neighborhood names.
2. **Given** I have selected two different neighborhoods, **When** the comparison is displayed, **Then** the results are presented side by side in the same overall arrangement as the existing Compare page mockup.
3. **Given** I select the same neighborhood in both neighborhood fields, **When** I try to compare, **Then** I am told to select two different neighborhoods and no misleading comparison is shown.

---

### User Story 2 - Review Comparison Charts (Priority: P1)

As a prospective business owner, I want visual comparisons of crime, building permit activity, and location suitability so that meaningful differences are easy to recognize.

**Why this priority**: The three requested visual comparisons are the primary way users interpret differences between locations.

**Independent Test**: Run a valid comparison and verify that exactly one chart is available for each requested measure, with both selected neighborhoods represented and labels that identify the measure and period where applicable.

**Acceptance Scenarios**:

1. **Given** a valid neighborhood comparison has completed, **When** I review the results, **Then** I see a crime comparison chart using reported crime data for both neighborhoods.
2. **Given** a valid neighborhood comparison has completed, **When** I review the results, **Then** I see a building permit activity comparison chart using building permit data for both neighborhoods.
3. **Given** a valid neighborhood comparison has completed, **When** I review the results, **Then** I see a location suitability comparison chart using the available suitability score for both neighborhoods.
4. **Given** a requested dataset does not contain a value for one selected neighborhood or period, **When** I review the affected chart, **Then** the missing value is clearly identified as unavailable rather than replaced with an invented metric.

---

### User Story 3 - Inspect Comparison Details (Priority: P2)

As a prospective business owner, I want a comparison table beneath the charts so that I can inspect the underlying values and understand which neighborhood is higher or lower for each measure.

**Why this priority**: A table provides precise details and supports users who need to verify what the visual summaries represent.

**Independent Test**: Run a valid comparison and verify that the table contains one row for crime, one row for building permit activity, and one row for location suitability, with values or unavailable indicators for both neighborhoods.

**Acceptance Scenarios**:

1. **Given** a valid comparison has completed, **When** I review the comparison table, **Then** it contains the selected neighborhoods as comparison columns and rows for crime metrics, building permit activity, and location suitability.
2. **Given** the source data identifies a reporting period, **When** I review a table row or chart, **Then** the applicable period is shown so that the values are not presented without context.
3. **Given** one or more requested source values are unavailable, **When** I submit the comparison, **Then** I see the available measures, an explicit unavailable state for each missing measure, and a user-friendly explanation of how to continue without fabricated results.

### Edge Cases

- A user submits without selecting a business type.
- A user submits with one or both neighborhoods missing.
- A user selects the same neighborhood twice.
- A neighborhood exists in one source dataset but has no records in another source dataset.
- A source dataset contains no records for the selected reporting period.
- The selected neighborhoods have different available reporting periods.
- The source data is temporarily unavailable or cannot be read.
- A business type is valid for selection but no existing location suitability result is available for it; suitability is marked unavailable rather than newly calculated.
- Very large or zero values must remain readable and must not be silently rounded into misleading comparisons.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The Compare page MUST allow a user to select one business type from the available canonical business-type list and MUST NOT accept unsupported free-text business types.
- **FR-002**: The Compare page MUST allow a user to select two neighborhoods from the available canonical neighborhood list so that each selection can be matched consistently across the source datasets.
- **FR-003**: The Compare page MUST require the two selected neighborhoods to be different before producing results.
- **FR-004**: The Compare page MUST preserve the selected business type and both neighborhood names in the comparison context and result heading.
- **FR-005**: The Compare page MUST present a crime comparison chart containing the source-provided total reported-crime count for both selected neighborhoods for the latest complete common reporting period.
- **FR-006**: The Compare page MUST present a building permit activity comparison chart containing the source-provided total permit count for both selected neighborhoods for the latest complete common reporting period.
- **FR-007**: The Compare page MUST present a location suitability comparison chart using the existing available location suitability score or result for both selected neighborhoods and MUST NOT recalculate, invent, or imply a new scoring formula in this feature.
- **FR-008**: The Compare page MUST present a comparison table containing the available crime, building permit activity, and location suitability values for both selected neighborhoods.
- **FR-009**: Charts and table values MUST identify the applicable reporting period or source context when that information is available in the source data.
- **FR-010**: The Compare page MUST identify unavailable, incomplete, or stale source values explicitly and MUST NOT substitute invented values, estimates, or unsupported metrics.
- **FR-011**: The Compare page MUST provide clear validation feedback when a required selection is missing or the same neighborhood is selected twice.
- **FR-012**: The comparison MUST use the available Cincinnati Open Data Portal reported crime dataset for crime measures and the available Cincinnati Open Data Portal building permits dataset for permit activity.
- **FR-013**: The comparison MUST use the existing available neighborhood/location suitability result or score for the selected business type as its suitability measure, MUST keep the source and meaning of that result visible to users, and MUST show it as unavailable when no existing result exists.
- **FR-014**: This feature MUST be scoped to the Compare page and its comparison results. It MUST NOT change, duplicate, or redefine the Analyze page workflow, calculations, trend visualizations, or ownership assigned to the teammate developing Analyze.
- **FR-015**: The comparison MUST remain usable when one source has no matching records by showing the available measures and marking only the affected measure as unavailable; missing data MUST NOT block the entire comparison or be replaced with fabricated values.

### Key Entities *(include if feature involves data)*

- **Business Type**: A type selected from the available canonical business-type list, used to provide comparison context and, where available, select the relevant suitability result.
- **Neighborhood**: A Cincinnati neighborhood selected from the canonical neighborhood list for one side of the comparison, identified by its available name and source identity.
- **Crime Measure**: The source-provided total reported-crime count for a neighborhood in the latest complete common reporting period.
- **Building Permit Activity**: The source-provided total building-permit count for a neighborhood in the latest complete common reporting period.
- **Location Suitability Result**: The existing suitability score or result for a canonical business type and neighborhood, including its source context and applicable period; Compare does not calculate this result.
- **Neighborhood Comparison**: A user-requested comparison containing one business type, two different neighborhoods, the three requested measures, their applicable periods, and availability status.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In usability testing, at least 90% of users can select a business type, select two different neighborhoods, and reach a valid comparison on their first attempt.
- **SC-002**: For a valid selection with available data, users see all three comparison charts and the comparison table within 5 seconds of submitting the comparison under normal operating conditions.
- **SC-003**: Every valid comparison displays exactly three measure-specific charts: crime, building permit activity, and location suitability.
- **SC-004**: Every valid comparison displays both selected neighborhoods in the comparison table and includes rows for all three requested measures, with unavailable values explicitly marked when necessary.
- **SC-005**: 100% of displayed comparison values can be traced to an available source dataset or existing suitability result, with no invented metrics or silently substituted values.
- **SC-006**: 100% of attempts to compare a neighborhood with itself receive validation feedback instead of a comparison result.
- **SC-007**: In review, the Compare page contains no Analyze-page-only controls, calculations, or trend workflows, and the Analyze page remains unchanged by this feature specification.

## Assumptions

- The initial audience is users evaluating potential Cincinnati business locations; no special account or role is required to view a comparison.
- Supported business types and neighborhood names come from the available project data rather than being hard-coded in the feature specification; both are selected using their canonical available values.
- Crime information comes from the Cincinnati Open Data Portal reported crime dataset, and permit information comes from the Cincinnati Open Data Portal building permits dataset described in the project documentation.
- Location suitability is the existing available result or score for the selected canonical business type and neighborhood; this feature does not define or run a new suitability algorithm or invent component metrics.
- The comparison uses the latest complete common reporting period available for both selected neighborhoods in each source; the source-provided total crime and permit counts are used without introducing new aggregation rules. If a common period cannot be established, the periods and limitation are shown to the user.
- The existing Compare page mockup is the visual reference for the selection controls, side-by-side result organization, three charts, and comparison table; exact presentation details may be refined during design without changing the user outcomes in this specification.
- The coordinated project-wide rename from Cincy Biz Advisor to Metro Biz Advisor is a prerequisite for implementation and applies to application branding, solution/project names, folders, namespaces, documentation, navigation, page titles, and repository references; it must not change the Compare feature behavior or requirements.
- Existing Cincy Biz Advisor paths, project references, and namespaces remain authoritative until the rename migration is reviewed and coordinated across the repository and teammate branches.
- Saving comparisons, editing the Analyze page, creating new datasets, and adding unrelated dashboard or history behavior are outside this feature's scope.
- The comparison is read-only and does not change source data.

## Out of Scope

- Building or modifying the Analyze page being developed by another teammate.
- Defining a new location suitability formula or adding metrics not present in the available datasets.
- Creating, cleaning, or fabricating crime, permit, neighborhood, or suitability data.
- Saving comparison results to analysis history.
- Providing recommendations beyond showing the requested side-by-side measures.
