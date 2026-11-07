namespace Cincy_Biz_Advisor.Services;

/// <summary>
/// Thrown when a city dataset cannot be reached or returns something we cannot read.
/// UserMessage is safe to show on the page; the inner exception has the technical details for the log.
/// </summary>
public sealed class CityDataUnavailableException : Exception
{
    public string DatasetName { get; }
    public string UserMessage { get; }

    public CityDataUnavailableException(string datasetName, string userMessage, Exception? innerException = null)
        : base($"The {datasetName} dataset is unavailable.", innerException)
    {
        DatasetName = datasetName;
        UserMessage = userMessage;
    }
}
