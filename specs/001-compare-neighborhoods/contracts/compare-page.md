# Compare Page Contract

## Purpose

Define the user-facing contract for the Compare Neighborhoods Razor Page. This contract assigns chart markup to the Razor page and does not prescribe a third-party charting library or source implementation.

## Current Implementation Status

The initial committed page implements the `/Compare` route, canonical selection form, duplicate-neighborhood validation, explicit unavailable/N/A states, three comparison result panels, a comparison table, navigation, and responsive styling. The panels are an initial presentation of the three measures; they are not yet the final source-backed server-rendered SVG charts required by this contract. The final chart, source-period, accessibility, and integrated behavior requirements remain unchanged.

## Page

- **Route**: `/Compare`
- **Page type**: Read-only comparison page with a form for selecting inputs.
- **Navigation**: The page may be linked from the existing site navigation, but it must not alter Analyze page navigation or behavior.
- **Product identity**: The final page title, navigation label, visible branding, and documentation use “Metro Biz Advisor.” Current Cincy paths and namespaces are temporary migration facts until the coordinated project rename is executed.

## Selection Form

| Control | Required | Contract |
|---------|----------|----------|
| Business type | Yes | Dropdown containing only canonical available business-type options. |
| Neighborhood A | Yes | Dropdown containing only canonical available neighborhood options. |
| Neighborhood B | Yes | Dropdown containing only canonical available neighborhood options. |
| Compare action | Yes | Submits the selected business type and two neighborhood identifiers. |

### Form Rules

- The two neighborhood controls must not accept the same canonical identifier.
- Missing selections show validation feedback without a comparison result.
- Unsupported business types or neighborhoods cannot be submitted as valid values.
- The selected values remain visible after validation failure and after a comparison is displayed.

## Result Heading

A successful result identifies:

- Selected business type
- Neighborhood A name
- Neighborhood B name
- The comparison period when a source period is available

## Chart Contract

The page renders exactly three measure-specific comparison charts in the mockup’s side-by-side comparison arrangement. Server-rendered SVG in `Compare.cshtml` owns chart markup and presentation data; client JavaScript must not introduce a second chart-rendering or metric-calculation path.

1. **Crime** — source-provided total reported-crime count for both neighborhoods.
2. **Building Permit Activity** — source-provided total permit count for both neighborhoods.
3. **Location Suitability** — existing suitability score/result for both neighborhoods; no score is calculated by Compare.

Every chart must:

- Identify the measure.
- Identify both neighborhood names.
- Display the applicable source period when available.
- Display the source label/context when available.
- Display `N/A` for a missing metric.
- Display a user-friendly unavailable/source-error message without substituting a value.
- Preserve the same metric meaning as the results table.

## Results Table Contract

The table appears with the comparison results and contains:

| Column | Content |
|--------|---------|
| Measure | Crime, Building Permit Activity, or Location Suitability |
| Neighborhood A | Source-backed value or `N/A` |
| Neighborhood B | Source-backed value or `N/A` |
| Period / source context | Applicable period and source label when available |

The table contains exactly one row for each of the three requested measures. It is the authoritative text equivalent for the charts, must remain readable for zero and large values, and must not silently round values into a misleading comparison.

## Validation and Empty States

- **Initial state**: Show the selection form and no comparison result.
- **Missing business type**: Identify the business type field as required.
- **Missing neighborhood**: Identify the missing neighborhood field as required.
- **Same neighborhood twice**: Explain that two different neighborhoods are required.
- **Unavailable source metric**: Render the chart/table metric as `N/A` and retain other available measures.
- **Source failure**: Render affected metrics as `N/A` and explain that the source is unavailable; do not expose raw source errors to users.
- **No valid result**: Do not render misleading empty charts or a comparison table with fabricated values.

## Analyze Isolation Boundary

The Compare page contract does not include Analyze-only controls, trend series, calculation inputs, scoring formulas, or Analyze page route/file changes. Compare consumes an existing suitability result through the data contract but does not calculate or persist it.

Any navigation, shared layout, or shared stylesheet change must be limited to the Metro Biz Advisor branding and the Compare link/presentation rules. It must be reviewed against Analyze routes, selectors, styles, and behavior; no Analyze-specific markup or CSS may be changed as part of Compare.

## Accessibility and Responsive Behavior

- Every dropdown has a visible label and an associated validation message.
- Charts must have a text-equivalent summary through labels, table values, or an accessible fallback.
- Server-rendered SVG must expose an accessible name/summary and use the same presentation values as the table; client JavaScript must not become a second rendering or calculation path.
- Color must not be the only distinction between neighborhoods.
- The side-by-side layout may stack on narrow screens while preserving the order and identity of both neighborhoods.
