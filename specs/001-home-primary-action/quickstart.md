# Quickstart: Home Page Primary Action

This guide describes how implementation should be verified after the test project and feature code are created. It is planning guidance only; this workflow does not implement the application or create `tasks.md`.

## Test-first setup

1. Add the sibling `Cincy Biz Advisor.Tests` project and reference the production Razor Pages project.
2. Add the candidate xUnit, ASP.NET Core integration-test, and Microsoft Playwright dependencies documented in `plan.md`.
3. Confirm the test project targets `net10.0` and does not change the production target framework.
4. Restore and build the solution and test project. Treat package/tooling compatibility as unverified until both operations succeed; resolve version or host errors before writing implementation code.
5. Configure the standard top-level `Program` test seam for `WebApplicationFactory`, disable automatic redirect following, and install Playwright browser binaries with the generated install script.
6. Write the failing tests listed below before adding production feature code.

## Required automated checks

### Repository and PageModel checks

- The seeded repository returns `Over-the-Rhine` and `Oakley`.
- Repository tests cover seeded `Over-the-Rhine` and `Oakley` records, unique IDs/names, the available-neighborhood collection, and lookup by identifier. They do not test browser filtering.
- Neighborhood lookup by identifier returns the selected record and rejects an unknown identifier.
- The PageModel exposes exactly four supported business types.
- Missing business type and missing neighborhood each produce a field-level validation error.
- Unsupported business type and unsupported neighborhood identifier are rejected on the server.
- A valid submission returns a redirect without following it; the `/Analyze` `Location` header is parsed and both decoded query values equal the selected neighborhood name and business type.
- An invalid submission does not redirect.

### Browser acceptance checks

- Render the Home page and verify labeled business type, search, result, selection, and Analyze controls.
- Search `Over-the-Rhine`; verify the matching result is visible.
- Search `oakley`, mixed casing, and `oak`; verify `Oakley` is visible.
- Search `  oak  `; verify `Oakley` is visible after trimming.
- Verify the rendered result list includes the seeded display names and that selecting a displayed name posts its matching neighborhood ID.
- Search `abcdefgh123`; verify zero results and the exact `No neighborhoods found` status message.
- Select a neighborhood, edit the search text, and verify the selection is cleared and a result must be selected again.
- Submit with no neighborhood and with no business type; verify actionable validation and no navigation to `/Analyze`.
- Submit every supported business type with an available neighborhood; verify the redirect and query values.
- Submit tampered unsupported values; verify server validation prevents navigation.

## Accessibility and visual verification

- Navigate the complete form using keyboard input only.
- Verify the combobox/listbox pattern: Tab moves between form controls, Up/Down arrows navigate results, Enter selects, Escape closes results, Space remains available for typing, and focus is visible.
- Verify visible focus states for the business type selector, search control, results, and Analyze button.
- Verify labels, error associations, and result/status announcements are programmatically connected to their controls.
- Verify normal-text contrast of at least 4.5:1, large-text contrast of at least 3:1, and applicable control/state contrast of at least 3:1 by measuring computed foreground/background colors in default, focused, selected, and validation states. Complete a keyboard-only walkthrough and verify visible focus on each control/result state without a trap.
- Verify the compact dark navy header, white primary action panel, and red Analyze button.
- Set the browser viewport to 1366 × 768 at 100% zoom and verify the complete primary form bounding rectangle is within the initial viewport and the document does not require vertical scrolling to see it.
- Set the browser viewport to 320 CSS pixels wide and verify `scrollWidth <= clientWidth` for the document and root page container.
- Verify shared layout changes are limited to what is necessary for the Home page and do not add dashboard, map, global search, profile, or other-page UI.

## Completion gate

The implementation is ready for review only when the test-first suite passes, all required acceptance scenarios pass, accessibility checks pass, and the feature-planning scope and dependency checks remain satisfied. The local constitution limitation remains unresolved; no constitution compliance is claimed until the shared constitution is available from its owner on `main`.
