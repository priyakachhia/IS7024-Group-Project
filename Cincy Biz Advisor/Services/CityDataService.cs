using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Cincy_Biz_Advisor.Services;

/// <summary>
/// Calls the Cincinnati Open Data (Socrata) API and asks it to COUNT matching rows,
/// so we download one small number instead of thousands of records.
///
/// Example request it builds:
///   resource/7aqy-xrv9.json?$select=count(*) AS total
///     &$where=upper(cpd_neighborhood) = 'OVER-THE-RHINE'
///             AND date_reported >= '2025-01-01T00:00:00'
///             AND date_reported &lt; '2026-01-01T00:00:00'
/// Example response: [{"total":"412"}]
/// </summary>
public sealed partial class CityDataService : ICityDataService
{
    public const string CrimeUnavailableMessage = "Crime data is currently unavailable. Please try again later.";
    public const string PermitUnavailableMessage = "Building permit data is currently unavailable. Please try again later.";

    private readonly HttpClient _httpClient;
    private readonly CityDataOptions _options;
    private readonly ILogger<CityDataService> _logger;

    public CityDataService(HttpClient httpClient, IOptions<CityDataOptions> options, ILogger<CityDataService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<int> GetCrimeCountAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
        GetCountAsync("crime", _options.CrimeDatasetId, _options.CrimeNeighborhoodField, _options.CrimeDateField,
            neighborhoodQueryValue, start, end, CrimeUnavailableMessage, cancellationToken);

    public Task<int> GetPermitCountAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
        GetCountAsync("building permit", _options.PermitDatasetId, _options.PermitNeighborhoodField, _options.PermitDateField,
            neighborhoodQueryValue, start, end, PermitUnavailableMessage, cancellationToken);

    private async Task<int> GetCountAsync(
        string datasetName,
        string datasetId,
        string neighborhoodField,
        string dateField,
        string neighborhoodQueryValue,
        DateOnly start,
        DateOnly end,
        string userMessage,
        CancellationToken cancellationToken)
    {
        string requestUri = BuildCountQuery(datasetId, neighborhoodField, dateField, neighborhoodQueryValue, start, end);
        _logger.LogInformation("Requesting {Dataset} count: {RequestUri}", datasetName, requestUri);

        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(requestUri, cancellationToken);
            string body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // Socrata explains bad requests in the body, e.g. "No such column: cpd_neighborhood".
                _logger.LogWarning("The {Dataset} API returned {StatusCode}: {Body}",
                    datasetName, (int)response.StatusCode, Truncate(body, 500));
                throw new CityDataUnavailableException(datasetName, userMessage);
            }

            return ParseCount(body);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not reach the {Dataset} API.", datasetName);
            throw new CityDataUnavailableException(datasetName, userMessage, ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // The HttpClient timed out (the user did not cancel).
            _logger.LogWarning(ex, "The {Dataset} API timed out.", datasetName);
            throw new CityDataUnavailableException(datasetName, userMessage, ex);
        }
        catch (Exception ex) when (ex is JsonException or FormatException or OverflowException)
        {
            _logger.LogWarning(ex, "The {Dataset} API returned data we could not read.", datasetName);
            throw new CityDataUnavailableException(datasetName, userMessage, ex);
        }
    }

    /// <summary>Builds the relative URL for a Socrata count query. Internal so unit tests can check it.</summary>
    internal static string BuildCountQuery(
        string datasetId,
        string neighborhoodField,
        string dateField,
        string neighborhoodQueryValue,
        DateOnly start,
        DateOnly end)
    {
        EnsureSafeIdentifier(datasetId, nameof(datasetId));
        EnsureSafeIdentifier(neighborhoodField, nameof(neighborhoodField));
        EnsureSafeIdentifier(dateField, nameof(dateField));

        if (string.IsNullOrWhiteSpace(neighborhoodQueryValue))
        {
            throw new ArgumentException("A neighborhood is required.", nameof(neighborhoodQueryValue));
        }

        if (end < start)
        {
            throw new ArgumentException("The end date must be on or after the start date.", nameof(end));
        }

        // SoQL strings use single quotes; a quote inside a name is written twice (O'Bryonville -> O''BRYONVILLE).
        string safeName = neighborhoodQueryValue.Trim().ToUpperInvariant().Replace("'", "''");
        string from = start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T00:00:00";
        string toExclusive = end.AddDays(1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T00:00:00";

        string select = "count(*) AS total";
        string where = $"upper({neighborhoodField}) = '{safeName}' AND {dateField} >= '{from}' AND {dateField} < '{toExclusive}'";

        return $"resource/{datasetId}.json?$select={Uri.EscapeDataString(select)}&$where={Uri.EscapeDataString(where)}";
    }

    /// <summary>Reads [{"total":"412"}] and returns 412. Internal so unit tests can check it.</summary>
    internal static int ParseCount(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException("Expected a JSON array from the count query.");
        }

        if (root.GetArrayLength() == 0)
        {
            return 0;
        }

        if (!root[0].TryGetProperty("total", out JsonElement total))
        {
            throw new FormatException("The count query response has no 'total' field.");
        }

        return total.ValueKind switch
        {
            JsonValueKind.String => int.Parse(total.GetString()!, NumberStyles.Integer, CultureInfo.InvariantCulture),
            JsonValueKind.Number => total.GetInt32(),
            _ => throw new FormatException("The 'total' field is not a number.")
        };
    }

    // Field names and dataset ids come from appsettings.json and are put straight into the query,
    // so only allow simple names like "cpd_neighborhood" or "7aqy-xrv9".
    private static void EnsureSafeIdentifier(string value, string settingName)
    {
        if (string.IsNullOrWhiteSpace(value) || !SafeIdentifierRegex().IsMatch(value))
        {
            throw new InvalidOperationException($"The CityData setting '{settingName}' has an invalid value: '{value}'.");
        }
    }

    private static string Truncate(string text, int maxLength) =>
        text.Length <= maxLength ? text : text[..maxLength] + "…";

    [GeneratedRegex("^[A-Za-z0-9_-]+$")]
    private static partial Regex SafeIdentifierRegex();
}
