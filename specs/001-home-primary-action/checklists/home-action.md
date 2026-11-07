# Requirements Quality Checklist: Home Page Primary Action

**Purpose**: Review the clarity, completeness, consistency, and acceptance-readiness of search, selection, validation, navigation, accessibility, and scope requirements.
**Created**: 2026-11-06
**Feature**: [../spec.md](../spec.md)

**Note**: This custom checklist is a reviewer-owned requirements-quality artifact. It evaluates the written requirements in `spec.md` and their alignment with `plan.md`; it does not verify implementation behavior.
**Review Ownership**: Mark an item `[x]` only when the reviewer determines that the requirements-quality criterion is satisfied.
**Marker Semantics**: `[x]` means the criterion has been reviewed and satisfied for requirements quality. It does not mean implementation work is complete.

## Search and Selection Requirements

- [ ] CHK001 Are the search input normalization rules explicitly defined for leading/trailing spaces, case differences, and substring matching? [Clarity, Spec §FR-004]
- [ ] CHK002 Are the required search examples (`Over-the-Rhine`, `oakley`, `oak`, and `abcdefgh123`) consistently represented across scenarios, requirements, contract, and plan? [Consistency, Spec §User Story 2; Plan §Acceptance Test Coverage]
- [x] CHK003 Is the empty-search behavior distinguishable from the no-match behavior, including what result state is presented? [Completeness, Spec §Edge Cases]
- [ ] CHK004 Is the selection model clear about how a result becomes the submitted neighborhood and how the neighborhood identifier relates to its displayed name? [Clarity, Spec §FR-003; Data Model §Home Page Selection]
- [ ] CHK005 Is it unambiguous that any edit to the search text clears the previous selection and requires a new result selection? [Consistency, Spec §User Story 3; Contract §Rendered interaction contract]
- [x] CHK006 Are keyboard and pointer selection expectations defined for every neighborhood result without prescribing an implementation algorithm? [Accessibility, Spec §FR-019; Contract §Rendered interaction contract]
- [x] CHK007 Are duplicate neighborhood names and the relationship between identifier uniqueness and display-name uniqueness defined sufficiently for an unambiguous selection? [Completeness, Data Model §Neighborhood]

## Validation and Navigation Requirements

- [ ] CHK008 Are both required selections identified consistently as business type and neighborhood, including the behavior when either is missing? [Consistency, Spec §FR-008–FR-010; Contract §Invalid submission]
- [ ] CHK009 Does the specification distinguish missing values from unsupported or tampered submitted values for both fields? [Completeness, Spec §FR-009–FR-010; Plan §Acceptance Test Coverage]
- [ ] CHK010 Are the four supported business types listed exactly once as the canonical set and used consistently by the specification, data model, contract, and plan? [Consistency, Spec §FR-002; Data Model §Business Type]
- [ ] CHK011 Are validation outcomes sufficiently specific to establish which field needs correction without requiring an implementation-specific message or control? [Clarity, Spec §FR-010; Contract §Invalid submission]
- [ ] CHK012 Is the valid destination path, parameter naming, selected neighborhood value, selected business type value, and URL-encoding requirement stated consistently? [Consistency, Spec §FR-011; Contract §Valid submission]
- [ ] CHK013 Is the requirement explicit that invalid submissions remain on the Home page and never reach the teammate-owned Analyze destination? [Completeness, Spec §FR-010 and §FR-017; Contract §Invalid submission]
- [ ] CHK014 Does the plan preserve the specification’s boundary that the Analyze page is a destination contract only and is not part of this feature? [Consistency, Spec §FR-017; Plan §Summary and §Constraints]

## Accessibility and Visual Requirements

- [x] CHK015 Are visible labels, accessible names, keyboard operation, focus states, validation associations, and result/status feedback specified for all interactive and feedback elements? [Completeness, Spec §FR-014 and §FR-020; Quickstart §Accessibility and visual verification]
- [x] CHK016 Is the expected relationship between search results, the active selection, and status messaging clear enough to support assistive-technology users? [Clarity, Spec §FR-014 and §FR-019; Contract §Rendered interaction contract]
- [x] CHK017 Is WCAG 2.1 AA treated as an outcome with enough measurable acceptance detail to avoid relying only on the phrase “consistent with” the standard? [Measurability, Spec §FR-020 and §SC-005]
- [ ] CHK018 Are the compact dark navy header, white primary action panel, and red Analyze button consistently described without expanding the requirement into unrelated page styling? [Consistency, Spec §FR-013; Plan §Constraints]
- [ ] CHK019 Is the desktop layout acceptance condition objectively defined as 1366 × 768 at 100% browser zoom with the primary form visible without scrolling? [Measurability, Spec §SC-006; Plan §Target Platform]
- [x] CHK020 Does the specification identify any required responsive or non-desktop behavior, or explicitly establish that the defined desktop check is the only viewport-specific acceptance condition? [Completeness, Spec §FR-018 and §SC-007]

## Scope and Cross-Artifact Consistency

- [ ] CHK021 Are the implementation exclusions—dashboard cards, charts, maps, global search, profile controls, crime/permit data, and other pages—listed consistently without preventing teammates from adding those areas later? [Consistency, Spec §FR-017; Plan §Scale/Scope]
- [ ] CHK022 Do the plan’s proposed production paths and design artifacts cover every requirement boundary without introducing unrequested Home page features? [Scope, Plan §Project Structure; Spec §FR-001 and §FR-017]
- [ ] CHK023 Are the repository interface, seeded in-memory records, `Id`/`Name` entity shape, and dependency-injection requirement consistently represented between the specification and plan without becoming a user-facing scope expansion? [Consistency, Spec §FR-015–FR-016; Plan §Summary and §Technical Context]
- [ ] CHK024 Does the test-first plan trace every primary, alternate, exception, accessibility, and visual acceptance scenario back to a requirement or contract statement? [Traceability, Spec §User Scenarios & Testing; Plan §Implementation Sequence and §Acceptance Test Coverage]
- [ ] CHK025 Does the plan retain the specification’s minimal shared-layout boundary rather than implying a global navigation or theme redesign? [Scope, Spec §FR-013 and §FR-017; Plan §Constraints and §Project Structure]
- [ ] CHK026 Is the distinction between requirements-quality review and implementation/test execution preserved in the checklist, plan, and feature scope? [Consistency, Checklist Purpose; Plan §Implementation Sequence]

## Gaps and Assumptions Requiring Reviewer Attention

- [x] CHK027 Is the behavior for a search input containing only whitespace explicitly defined as equivalent to an empty search? [Gap, Spec §Edge Cases and §FR-004]
- [x] CHK028 Are the exact keyboard interaction expectations for moving among results and activating a result specified sufficiently for consistent accessibility review? [Gap, Spec §FR-019; Contract §Rendered interaction contract]
- [x] CHK029 Are the visual colors and focus treatment constrained by an explicit contrast/outcome requirement rather than only color names? [Gap, Spec §FR-020 and §SC-005]
- [x] CHK030 Are the assumptions about the availability and ownership of the `/Analyze` destination documented as a dependency without implying that this feature implements that page? [Assumption, Spec §Assumptions; Contract §Analyze dependency]
- [ ] CHK031 Is the local constitution limitation retained as an explicit unresolved governance note, without claiming that constitution checks passed? [Governance, Plan §Constitution Check]

## Notes

- Re-evaluation result: 11 of 31 checklist items are now resolved and marked `[x]` based on the user's approved decisions; remaining items still require reviewer assessment.
- These items assess requirements quality, not application behavior or test execution.
- The shared constitution is managed by Priya on `main` and was not recreated or modified.
- CHK031 remains unchecked because the local constitution file contains placeholders; no constitution compliance is claimed.
- `tasks.md` is intentionally not created; this work stops at Checklist.
