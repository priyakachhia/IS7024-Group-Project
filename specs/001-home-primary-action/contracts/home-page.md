# Contract: Home Page Primary Action

## GET `/`

The Home page loads the supported business types and all available neighborhoods needed for initial display and client-side filtering.

### Rendered interaction contract

- The business type control exposes exactly four supported options.
- The neighborhood search control is labeled and exposes a status region for result counts and the `No neighborhoods found` message.
- Search matching trims leading and trailing spaces; empty or whitespace-only input shows all available neighborhoods, while non-empty input performs case-insensitive substring matching.
- Filtering is performed in the browser over the repository-loaded list; the repository contract is limited to loading available neighborhoods and looking up a submitted ID.
- The rendered result list exposes the seeded neighborhood display names, and selecting a displayed name posts the ID belonging to that same record.
- The neighborhood picker uses a combobox/listbox interaction: Tab moves between form controls, Up/Down arrows navigate results, Enter selects, Escape closes results, Space remains available when typing neighborhood names, and keyboard focus is visible.
- Each result exposes an accessible name and can be selected with keyboard or pointer input.
- Editing search text clears any prior neighborhood selection and requires a new result selection.
- The Analyze action remains keyboard operable and has a visible focus state.
- Text contrast is at least 4.5:1 for normal text and 3:1 for large text; applicable control boundaries and state indicators are at least 3:1 in default, focused, selected, and validation states. Contrast is measured from the computed foreground/background colors for representative rendered elements in each state.
- At 1366 × 768 and 100% browser zoom, the complete primary form is within the initial viewport, verified by its bounding rectangle and document scroll height. At 320 CSS pixels wide, the document has no horizontal overflow (`scrollWidth` does not exceed `clientWidth`).
- The complete task is possible with a keyboard without a trap, and labels, validation messages, and status feedback are programmatically associated.

## POST `/`

The Home page submits the selected values to its PageModel for authoritative validation.

### Request fields

| Field | Required | Validation |
|-------|----------|------------|
| `BusinessType` | Yes | Must be one of `Restaurant`, `Coffee Shop`, `Retail Store`, `Fitness Center`. |
| `NeighborhoodId` | Yes | Must resolve to an available neighborhood in the repository. |

Search text is interaction state and is not trusted as the submitted neighborhood selection.

### Invalid submission

- Stay on `/`.
- Show a clear, field-associated validation message for each missing or unsupported value.
- Preserve enough form state to let the user correct the input.
- Do not redirect to `/Analyze`.

### Valid submission

Redirect to:

```text
/Analyze?neighborhood=<url-encoded-neighborhood-name>&businessType=<url-encoded-business-type>
```

The redirect MUST be produced from validated values and MUST URL-encode both query parameter values. The Analyze page is not part of this feature and must not be created by this implementation.

## Required acceptance tests

1. Search `Over-the-Rhine` and verify a matching result.
2. Search `oakley`, `OAKLEY`, and `oak`; verify `Oakley` is shown.
3. Search `abcdefgh123`; verify zero results and `No neighborhoods found`.
4. Select a neighborhood, edit the search text, and verify the selection is cleared.
5. Submit with no neighborhood; verify validation and no redirect.
6. Submit with no business type; verify validation and no redirect.
7. Submit unsupported business type or neighborhood identifier; verify server validation and no redirect.
8. Submit each of the four business types with an available neighborhood; disable automatic redirect following, inspect the `/Analyze` `Location` header, and decode both query values to verify the selected names.
9. Verify empty and whitespace-only searches show all available neighborhoods, `  oak  ` shows `Oakley`, rendered results include the seeded display names, selecting a displayed name posts its matching ID, and non-empty unmatched searches show zero results and `No neighborhoods found`.
10. Verify the combobox/listbox keyboard pattern with a keyboard-only walkthrough, including visible focus on each control and result state, labels, validation/status associations, keyboard completion without a trap, contrast thresholds in default/focused/selected/validation states, 320 CSS pixel no-horizontal-scroll behavior, and the 1366 × 768 at 100% full-form initial-viewport check.

## Analyze dependency

`/Analyze` is teammate-owned and is expected to accept the `neighborhood` and `businessType` query parameters. This feature does not create or modify that page.
