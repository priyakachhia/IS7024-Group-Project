# History API Contract

Base path: `/api/history`

## POST /api/history

Creates one saved analysis entry.

### Request body

```json
{
  "neighborhood": "Over-the-Rhine",
  "businessType": "Restaurant",
  "periodStart": "2025-01-01",
  "periodEnd": "2025-12-31",
  "crimeCount": 42,
  "permitCount": 18,
  "safetyScore": 82.5,
  "growthScore": 74.0,
  "suitabilityScore": 79
}
```

### Success response: `201 Created`

The response contains the persisted entry, including its generated `id` and `savedAt`, and includes a `Location` pointing to `/api/history` because individual retrieval is out of scope.

```json
{
  "id": 1,
  "neighborhood": "Over-the-Rhine",
  "businessType": "Restaurant",
  "periodStart": "2025-01-01",
  "periodEnd": "2025-12-31",
  "crimeCount": 42,
  "permitCount": 18,
  "safetyScore": 82.5,
  "growthScore": 74.0,
  "suitabilityScore": 79,
  "savedAt": "2026-11-19T14:30:00Z"
}
```

### Validation response: `400 Bad Request`

The response uses ASP.NET Core validation-problem details and includes field-specific errors. Examples include missing/blank neighborhood, unsupported business type, negative counts, scores outside 0–100, and a period end before its start. The database is unchanged.

## GET /api/history

Returns every saved entry ordered newest first.

### Success response: `200 OK`

```json
[
  {
	"id": 1,
	"neighborhood": "Over-the-Rhine",
	"businessType": "Restaurant",
	"periodStart": "2025-01-01",
	"periodEnd": "2025-12-31",
	"crimeCount": 42,
	"permitCount": 18,
	"safetyScore": 82.5,
	"growthScore": 74.0,
	"suitabilityScore": 79,
	"savedAt": "2026-11-19T14:30:00Z"
  }
]
```

An empty database returns `200 OK` with `[]`.
