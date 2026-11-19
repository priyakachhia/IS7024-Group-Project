# Data Model: Home Page Primary Action

## Neighborhood

Represents a selectable neighborhood available for analysis.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `Id` | `int` | Yes | Positive identifier; unique within the repository. |
| `Name` | `string` | Yes | Non-empty display name; unique in the initial seeded dataset and used in the Analyze query value. |

Seeded records MUST include at least:

- `Over-the-Rhine`
- `Oakley`

The repository owns the available-record collection. The Home page does not create, update, or delete neighborhoods.

Duplicate-name disambiguation is not required for this feature because the initial seeded neighborhood names are unique and the destination contract uses the neighborhood name.

## Business Type

A supported selection represented as one of these exact values:

- `Restaurant`
- `Coffee Shop`
- `Retail Store`
- `Fitness Center`

The supported list is application-owned and is not user-editable.

## Home Page Selection

Transient form state used only while completing the Home page action.

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `BusinessType` | `string` | Yes | Must match one supported business type on the server. |
| `NeighborhoodId` | `int?` | Yes on submit | Must identify a currently available repository record on the server. |
| `NeighborhoodSearch` | `string` | No | Trimmed for matching; empty or whitespace-only input shows all available neighborhoods; non-empty input uses case-insensitive substring filtering; editing it clears `NeighborhoodId`. |

The selected neighborhood's repository `Name` is used for the `neighborhood` query parameter. The submitted `NeighborhoodId` is not exposed as the Analyze query parameter.

## Repository Boundary

The implementation plan requires a repository abstraction that supports the Home page's needs without exposing storage details:

- Load all available neighborhoods for initial rendering.
- Retrieve a neighborhood by identifier for authoritative submission validation.

The initial implementation uses seeded in-memory records registered through dependency injection. A later persistent implementation can replace the registration without changing the PageModel's user-facing behavior or the Home page contract.

## State Transitions

1. Initial page load: available neighborhoods are rendered; no selection is active.
2. Search input changes: input is trimmed for matching, empty or whitespace-only input shows all results, non-empty input filters by case-insensitive substring, and any prior selection is cleared.
3. Result activation: the chosen neighborhood identifier is stored as the active selection and its display name remains visible.
4. Invalid submit: model state contains a field-level message; the page remains on Home and no redirect occurs.
5. Valid submit: the selected business type and repository-resolved neighborhood name are passed to the `/Analyze` destination.
