# Research: Neighborhood Analysis

## Decision: Use configured Socrata count queries

- **Decision**: `CityDataService` will use the two dataset resource URLs and issue Socrata `$select=count(*) AS total` queries with URL-encoded `$where` clauses.
- **Rationale**: Counting at the source minimizes payload size and makes the service contract focused on the two required totals.
- **Alternatives considered**: Downloading individual records and counting in-process was rejected because it transfers more data and duplicates filtering logic.

## Decision: Make dataset fields configurable

- **Decision**: Bind dataset IDs, neighborhood fields, date fields, and timeout from the `CityData` configuration section. Use crime defaults `cpd_neighborhood` and `date_reported`; use permits `neighborhood` and `issueddate`.
- **Rationale**: The requested fields are dataset-specific and may change independently of application code. Configuration also supports verification and correction without changing the query builder.
- **Alternatives considered**: Hard-coding field names in the service was rejected because it makes schema changes require a code deployment.

## Decision: Normalize neighborhood comparisons in Socrata

- **Decision**: Build the required predicate as `upper(<neighborhood field>) = '<UPPERCASE NAME>'` and use an inclusive start plus exclusive next-day end date.
- **Rationale**: Case normalization provides stable matching for user-facing names, while the exclusive end boundary avoids time-of-day ambiguity.
- **Alternatives considered**: Client-side filtering and inclusive end-of-day timestamps were rejected because they require larger responses and can mishandle date-only fields.

## Decision: Keep suitability calculation pure

- **Decision**: Implement `SuitabilityCalculator` as static pure functions over counts, business type, and configured constants.
- **Rationale**: The formula is deterministic, independently testable, and should not depend on HTTP, UI, or configuration state.
- **Alternatives considered**: Putting the formula in the PageModel or HTTP service was rejected because it would make unit testing and reuse harder.

## Decision: Run the two data requests concurrently

- **Decision**: `NeighborhoodAnalysisService` starts crime and permit requests together with `Task.WhenAll`, then calculates the result only after both succeed.
- **Rationale**: The requests are independent and concurrency reduces total wait time while preserving the all-data requirement for a score.
- **Alternatives considered**: Sequential requests were rejected because they add avoidable latency.

## Decision: Use typed HTTP client and explicit unavailable exception

- **Decision**: Register a typed `HttpClient` for `ICityDataService`, configure its timeout from `CityDataOptions`, and translate unavailable/invalid upstream responses into `CityDataUnavailableException` with the required user-facing message.
- **Rationale**: Centralized HTTP policy keeps upstream behavior out of the PageModel and provides a stable error boundary for both the Razor Page and optional endpoint.
- **Alternatives considered**: Raw `HttpClient` construction in request handlers was rejected because it prevents consistent lifetime, timeout, and test-handler injection.

## Decision: Confirm crime schema through configuration and contract tests

- **Decision**: Ship the requested crime defaults `cpd_neighborhood` and `date_reported`; verify them through fake-handler request assertions and a live smoke check when the public endpoint permits access.
- **Rationale**: The supplied defaults are the authoritative planning input, while contract tests protect the exact query shape. Direct live verification may be blocked by upstream access controls, so runtime field names remain configurable.
- **Alternatives considered**: Blocking the plan until live metadata access was available was rejected because it would prevent design work and the required configuration already provides a controlled correction path.
