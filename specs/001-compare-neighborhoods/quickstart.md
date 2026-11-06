# Compare Neighborhoods Validation Guide

## Purpose

Validate the planned Compare Neighborhoods feature end to end without relying on fabricated crime, permit, or suitability values. The scenarios use the actual configured source providers and canonical option data available to the implementation environment.

## Prerequisites

- .NET 10 SDK installed.
- Repository root is `C:\Users\priya\source\repos\IS7024-Group-Project`.
- The project has been configured with access to the actual Cincinnati reported crime and building permits datasets.
- The existing suitability-result provider is available for at least one supported canonical business type and two neighborhoods, or the test environment can return an explicit unavailable state.
- No test scenario should seed invented metric values. If a source is unavailable, the expected result is `N/A`.
- The current Cincy paths in the commands below are temporary repository paths. After the approved rename migration, replace them with the renamed Metro Biz Advisor solution/project paths and verify the old identifiers are absent from build output and configuration.
- The source-readiness record from T004 must identify the verified endpoint/access method, response schema, join key, completeness rule, period field, total field, and test responses before Scenario 1 or the timed validation is run.

## Build Validation

From the repository root:

```powershell
dotnet build ".\Cincy Biz Advisor\Cincy Biz Advisor.csproj"
```

Expected result: the project builds without errors.

## Run Validation

From the repository root:

```powershell
dotnet run --project ".\Cincy Biz Advisor\Cincy Biz Advisor.csproj"
```

Open the reported HTTPS URL and navigate to `/Compare`.

## Scenario 1: Valid Complete Comparison

1. Open `/Compare`.
2. Select one business type from the canonical business-type dropdown.
3. Select two different neighborhoods from the canonical neighborhood dropdowns.
4. Submit Compare.
5. Verify the result heading identifies the business type and both neighborhood names.
6. Verify exactly three charts are displayed: Crime, Building Permit Activity, and Location Suitability.
7. Verify each chart identifies both neighborhoods and the applicable source period/context.
8. Verify the results table contains exactly one row for each of the three measures.
9. Verify displayed values match the actual source-provided totals or existing suitability result; do not compare against invented expected numbers.
10. Record the submit-to-complete-result duration and test environment for the five-second performance criterion when all requested data is available.

Expected result: a complete side-by-side comparison with all source-backed measures visible.

Performance result: record the observed duration and whether the result met the five-second target under the documented normal operating conditions; do not claim compliance from an unmeasured run.

## Scenario 2: Same Neighborhood Validation

1. Select any supported business type.
2. Select the same canonical neighborhood in both neighborhood controls.
3. Submit Compare.

Expected result: validation explains that two different neighborhoods are required. No comparison charts or result table are shown, and no comparison source request is made.

## Scenario 3: Missing Required Selection

Run each case separately:

- Submit with no business type.
- Submit with no first neighborhood.
- Submit with no second neighborhood.

Expected result: the relevant field is identified as required, selections remain available for correction, and no comparison result is shown.

## Scenario 4: Missing Source Metric

1. Select valid canonical inputs for which the configured source has no record for at least one requested measure, or use a source-backed test response that explicitly represents a missing record.
2. Submit Compare.

Expected result:

- The affected chart and table cell show `N/A`.
- The available measures remain visible.
- The page explains which measure is unavailable.
- No zero, estimate, or newly calculated substitute appears.

## Scenario 5: Source Failure

1. Use the approved test configuration to make one source unavailable or return an invalid response.
2. Submit a valid comparison.

Expected result: the affected measure shows `N/A` with a user-friendly source-unavailable message, other available measures remain visible, and raw source errors are not displayed.

## Scenario 6: Suitability Is Reused, Not Recalculated

1. Select a business type and two neighborhoods with existing suitability results.
2. Record the suitability values and source meaning from the approved suitability provider.
3. Submit Compare.
4. Compare the displayed suitability values with the provider results.

Expected result: Compare displays the existing results and source meaning without applying a new formula, weights, or crime/permit-derived calculation.

Repeat with a valid business type and neighborhood pair for which no existing suitability result is available.

Expected result: suitability displays as `N/A`; the page does not calculate a replacement and does not block available crime or permit measures.

Record the provider contract version or test-response identity used for the reuse check.

## Scenario 7: Responsive and Accessibility Review

1. Review the page at desktop width matching the mockup’s side-by-side arrangement.
2. Review the page at a narrow viewport.
3. Navigate the form using the keyboard only.
4. Inspect the chart/table text alternative.

Expected result: the two neighborhoods remain identifiable when the layout stacks, each control has a visible label and validation message, and chart information remains available through text or the comparison table without relying on color alone.

## Scenario 8: Analyze Isolation Regression Check

1. Record the Analyze page route and files present before Compare implementation.
2. Build and run the application after Compare changes.
3. Open the Analyze page and execute its existing scenarios, if the page exists in the implementation branch.
4. Review the change set for modifications to Analyze routes, PageModels, calculations, trend contracts, or suitability ownership.

Expected result: Compare works through its own page/data boundary, the Analyze workflow is unchanged, and no Analyze-only trend or scoring calculation has been added to Compare.

## Scenario 9: Rename Migration Validation

1. Confirm the approved migration inventory covers solution/project names, folders, namespaces, project references, build configuration, CI/CD, GitHub links/remotes, README, Spec Kit artifacts, navigation, page titles, and branding.
2. Build and test from the renamed Metro Biz Advisor solution using the updated project references.
3. Search tracked files and build configuration for stale Cincy Biz Advisor identifiers, allowing only explicitly labeled historical migration notes.
4. Confirm each active teammate branch has been rebased or recreated from the migration result and builds successfully before Compare implementation begins.

Expected result: the final project consistently uses Metro Biz Advisor and the Compare implementation gate is approved by the migration and affected feature owners.

## Scenario 10: Verified Dataset Mapping

1. Compare the source-readiness record with the crime and permit adapter mappings.
2. Confirm each adapter uses the documented endpoint/access method, response field, neighborhood join key, reporting-period field, completeness rule, and source-provided total field.
3. Use a recorded source response for an available value, a missing neighborhood/period, and an invalid or unavailable response.
4. Verify available responses map to source totals, missing responses map to `Unavailable`, and invalid/source failures map to `SourceError` without zero, estimates, or fabricated values.
5. Confirm the selected period is the latest complete common period documented by the source-readiness record.

Expected result: every displayed crime and permit value is traceable to a verified source field and the mapping behavior is reproducible from the recorded response examples.

## References

- [Feature specification](spec.md)
- [Data model](data-model.md)
- [Compare page contract](contracts/compare-page.md)
- [Compare data contract](contracts/compare-data.md)
- [Research decisions](research.md)
