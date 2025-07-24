using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyCompany.Api.Middleware;

/// <summary>
/// This type is returned when any error happens in the API together with the status code
/// </summary>
public record ErrorDetails
{
    /// <summary>
    /// The returned status code
    /// </summary>
    [JsonPropertyName("status_code")]
    public required int StatusCode { get; init; }
    
    
    /// <summary>
    /// The error details
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
    
    /// <summary>
    /// The trace id from the context
    /// </summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; init; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
};