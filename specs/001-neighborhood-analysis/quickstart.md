# Quickstart: Neighborhood Analysis

## Configuration

Add a `CityData` section to `Cincy Biz Advisor/appsettings.json` with the two dataset IDs, configured field names, and timeout. Keep the crime defaults `cpd_neighborhood` and `date_reported`; permits use `neighborhood` and `issueddate`.

## Registration

Register Razor Pages, bind `CityDataOptions`, and register the typed `HttpClient` for `ICityDataService` plus `INeighborhoodAnalysisService`. The typed client timeout must be derived from `CityDataOptions.TimeoutSeconds`.

## Razor Page flow

1. `GET /Analyze` initializes the supported neighborhood/business-type choices and defaults dates to January 1 through December 31, 2025.
2. The POST form validates the model and returns the same page with validation errors when required input is invalid.
3. A valid POST calls `INeighborhoodAnalysisService`, which starts crime and permit count requests concurrently.
4. A successful response renders counts, Safety, Growth, and Score in an `aria-live` results region.
5. A `CityDataUnavailableException` renders its safe message in a `role="alert"` region and leaves the score absent.

## Manual verification

- Analyze Over-the-Rhine / Restaurant / 2025-01-01 to 2025-12-31 and verify 412 crimes, 87 permits, score 72.
- Analyze Oakley / Coffee Shop / the same dates and verify 138 crimes, 41 permits, score 64.
- Submit without a neighborhood and verify the required validation message.
- Simulate an unavailable crime request with a fake handler and verify the required message and no score.
- If enabled, request `/api/neighborhoods/Over-the-Rhine/analysis?businessType=Restaurant&from=2025-01-01&to=2025-12-31` and verify the documented status behavior.
