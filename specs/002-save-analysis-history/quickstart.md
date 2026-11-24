# Quickstart: Save Analysis History

## Prerequisites

- .NET 10 SDK/runtime
- No separate database server

## First run

1. Start the existing Razor Pages application.
2. The application creates the SQLite database file in its data folder automatically.
3. Open Analyze and run an analysis.
4. Select Save Analysis and confirm the accessible status message.
5. Open History from the top menu.
6. Verify the saved entry appears newest first.

## API smoke checks

Create a valid entry:

```powershell
$body = @{
  neighborhood = 'Over-the-Rhine'
  businessType = 'Restaurant'
  periodStart = '2025-01-01'
  periodEnd = '2025-12-31'
  crimeCount = 42
  permitCount = 18
  safetyScore = 82.5
  growthScore = 74
  suitabilityScore = 79
} | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri 'https://localhost:5001/api/history' -ContentType 'application/json' -Body $body
Invoke-RestMethod -Method Get -Uri 'https://localhost:5001/api/history'
```

Submit an invalid body, such as a negative `crimeCount` or a `periodEnd` before `periodStart`, and verify that the response is `400` with validation details and that the subsequent GET list is unchanged.

## Tests

Run the complete solution test suite. The new service tests use SQLite in-memory, while controller tests use a fake `IHistoryService` to verify validation and HTTP status codes without requiring a persistent database file.
