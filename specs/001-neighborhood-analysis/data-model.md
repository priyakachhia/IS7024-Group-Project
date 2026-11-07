# Data Model: Neighborhood Analysis

## Configuration

### CityDataOptions

Application configuration bound from `CityData`.

| Property | Meaning | Initial value |
|---|---|---|
| `CrimeDatasetId` | Socrata crime dataset identifier | `7aqy-xrv9` |
| `CrimeNeighborhoodField` | Crime neighborhood field | `cpd_neighborhood` |
| `CrimeDateField` | Crime date field | `date_reported` |
| `PermitDatasetId` | Socrata permit dataset identifier | `uhjb-xac9` |
| `PermitNeighborhoodField` | Permit neighborhood field | `neighborhood` |
| `PermitDateField` | Permit date field | `issueddate` |
| `TimeoutSeconds` | Upstream request timeout | 10 seconds |

## Request values

### NeighborhoodAnalysisRequest

- `Neighborhood`: required user-selected Cincinnati neighborhood.
- `BusinessType`: required value: `Restaurant`, `Coffee Shop`, or `Retail`.
- `From`: required inclusive date.
- `To`: required inclusive date.

## Result values

### NeighborhoodAnalysisResult

- `Neighborhood` and `BusinessType` copied from the validated request.
- `From` and `To` copied from the validated request.
- `CrimeCount`: number of matching reported crime incidents.
- `PermitCount`: number of matching building permits.
- `Safety`: `max(0, 100 - CrimeCount / 8)`.
- `Growth`: `min(100, PermitCount)`.
- `Score`: rounded value of `Safety * weight + Growth * (1 - weight)`.

### BusinessTypeWeights

| Business type | Safety weight |
|---|---:|
| Restaurant | 0.40 |
| Coffee Shop | 0.55 |
| Retail | 0.70 |

## Validation rules

- Neighborhood is required.
- Business type must be one of the three supported values.
- From must be on or before To.
- Both data counts are required before a score is produced.
- Score output is constrained to 0 through 100 by the Safety and Growth caps.

## Upstream count response

Socrata responses are expected to contain one row with a `total` value from `$select=count(*) AS total`. The typed client parses that value as a non-negative integer and rejects malformed or unsuccessful responses as unavailable data.
