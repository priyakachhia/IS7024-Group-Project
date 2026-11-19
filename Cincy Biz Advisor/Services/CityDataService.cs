using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Cincy_Biz_Advisor.Models;
using Microsoft.Extensions.Options;

namespace Cincy_Biz_Advisor.Services;

/// <summary>
/// Calls the Cincinnati Open Data (Socrata) API.
/// Counts and groupings are done by the city's server, so we download a few small
/// rows instead of thousands of records.
///
/// Example count request:
///   resource/t9kw-3jfx.json?$select=count(*) AS total
///     &$where=upper(snaneighborhood) = 'OVER-THE-RHINE'
///             AND reportdate >= '2025-01-01T00:00:00'
///             AND reportdate &lt; '2026-01-01T00:00:00'
/// Example response: [{"total":"412"}]
/// </summary>
public sealed partial class CityDataService : ICityDataService
{
    public const string CrimeUnavailableMessage = "Crime data is currently unavailable. Please try again later.";
    public const string PermitUnavailableMessage = "Building permit data is currently unavailable. Please try again later.";

    // The crime dataset column that holds the crime type, e.g. "THEFT".
    private const string CrimeCategoryField = "starscategory";

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
        GetAsync("crime", BuildCountQuery(_options.CrimeDatasetId, _options.CrimeNeighborhoodField, _options.CrimeDateField,
            neighborhoodQueryValue, start, end), CrimeUnavailableMessage, ParseCount, cancellationToken);

    public Task<int> GetPermitCountAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
        GetAsync("building permit", BuildCountQuery(_options.PermitDatasetId, _options.PermitNeighborhoodField, _options.PermitDateField,
            neighborhoodQueryValue, start, end), PermitUnavailableMessage, ParseCount, cancellationToken);

    public Task<IReadOnlyList<MonthlyCount>> GetCrimeTrendAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
        GetAsync("crime", BuildMonthlyQuery(_options.CrimeDatasetId, _options.CrimeNeighborhoodField, _options.CrimeDateField,
            neighborhoodQueryValue, start, end), CrimeUnavailableMessage, ParseMonthlyCounts, cancellationToken);

    public Task<IReadOnlyList<MonthlyCount>> GetPermitTrendAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
        GetAsync("building permit", BuildMonthlyQuery(_options.PermitDatasetId, _options.PermitNeighborhoodField, _options.PermitDateField,
            neighborhoodQueryValue, start, end), PermitUnavailableMessage, ParseMonthlyCounts, cancellationToken);

    public Task<IReadOnlyList<CategoryCount>> GetCrimeByTypeAsync(string neighborhoodQueryValue, DateOnly start, DateOnly end, CancellationToken cancellationToken = default) =>
        GetAsync("crime", BuildCategoryQuery(_options.CrimeDatasetId, _options.CrimeNeighborhoodField, _options.CrimeDateField,
            neighborhoodQueryValue, start, end), CrimeUnavailableMessage, ParseCategoryCounts, cancellationToken);

    /// <summary>Sends one request and turns any failure into a CityDataUnavailableException.</summary>
    private async Task<T> GetAsync<T>(
        string datasetName,
        string requestUri,
        string userMessage,
        Func<string, T> parse,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requesting {Dataset} data: {RequestUri}", datasetName, requestUri);

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

            return parse(body);
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
        string where = BuildWhere(datasetId, neighborhoodField, dateField, neighborhoodQueryValue, start, end);
        string select = "count(*) AS total";

        return $"resource/{datasetId}.json?$select={Uri.EscapeDataString(select)}&$where={Uri.EscapeDataString(where)}";
    }

    /// <summary>Builds a query that counts rows per month: [{"month":"2025-03-01T00:00:00.000","total":"41"}, ...]</summary>
    internal static string BuildMonthlyQuery(
        string datasetId,
        string neighborhoodField,
        string dateField,
        string neighborhoodQueryValue,
        DateOnly start,
        DateOnly end)
    {
        string where = BuildWhere(datasetId, neighborhoodField, dateField, neighborhoodQueryValue, start, end);
        string select = $"date_trunc_ym({dateField}) AS month, count(*) AS total";

        return $"resource/{datasetId}.json?$select={Uri.EscapeDataString(select)}&$where={Uri.EscapeDataString(where)}&$group=month&$order=month&$limit=1000";
    }

    /// <summary>Builds a query that counts crimes per category, largest first.</summary>
    internal static string BuildCategoryQuery(
        string datasetId,
        string neighborhoodField,
        string dateField,
        string neighborhoodQueryValue,
        DateOnly start,
        DateOnly end)
    {
        string where = BuildWhere(datasetId, neighborhoodField, dateField, neighborhoodQueryValue, start, end);
        string select = $"{CrimeCategoryField}, count(*) AS total";

        return $"resource/{datasetId}.json?$select={Uri.EscapeDataString(select)}&$where={Uri.EscapeDataString(where)}" +
               $"&$group={CrimeCategoryField}&$order={Uri.EscapeDataString("total DESC")}&$limit=50";
    }

    // Shared by every query: checks the inputs and builds the WHERE part.
    private static string BuildWhere(
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

        return $"upper({neighborhoodField}) = '{safeName}' AND {dateField} >= '{from}' AND {dateField} < '{toExclusive}'";
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

        return ReadTotal(root[0]);
    }

    /// <summary>Reads [{"month":"2025-03-01T00:00:00.000","total":"41"}, ...].</summary>
    internal static IReadOnlyList<MonthlyCount> ParseMonthlyCounts(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException("Expected a JSON array from the monthly query.");
        }

        var months = new List<MonthlyCount>();
        foreach (JsonElement row in root.EnumerateArray())
        {
            if (!row.TryGetProperty("month", out JsonElement month) || month.ValueKind != JsonValueKind.String)
            {
                throw new FormatException("A monthly row has no 'month' field.");
            }

            string text = month.GetString()!;
            if (text.Length < 10)
            {
                throw new FormatException("A monthly row has an invalid 'month' value.");
            }

            DateOnly date = DateOnly.ParseExact(text[..10], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            months.Add(new MonthlyCount(date, ReadTotal(row)));
        }

        return months;
    }

    /// <summary>Reads [{"starscategory":"THEFT","total":"120"}, ...].</summary>
    internal static IReadOnlyList<CategoryCount> ParseCategoryCounts(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException("Expected a JSON array from the category query.");
        }

        var categories = new List<CategoryCount>();
        foreach (JsonElement row in root.EnumerateArray())
        {
            string name = row.TryGetProperty(CrimeCategoryField, out JsonElement category) && category.ValueKind == JsonValueKind.String
                ? category.GetString()!
                : "UNKNOWN";

            categories.Add(new CategoryCount(name, ReadTotal(row)));
        }

        return categories;
    }

    // Reads the "total" field, which Socrata sends as a string ("412") or a number.
    private static int ReadTotal(JsonElement row)
    {
        if (!row.TryGetProperty("total", out JsonElement total))
        {
            throw new FormatException("The query response has no 'total' field.");
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