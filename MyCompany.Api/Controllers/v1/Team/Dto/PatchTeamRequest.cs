using System.Text.Json.Serialization;
using MyCompany.Api.Controllers.Validators;

namespace MyCompany.Api.Controllers.v1.Team.Dto;

public sealed record PatchTeamRequest
{
    /// <summary>
    /// The new name
    /// </summary>
    [JsonPropertyName("name")]
    [NotEmptyIfPresent(ErrorMessage = "The Name field cannot be null or empty if provided.")]
    public string? Name { get; init; }
    
    /// <summary>
    /// The new description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    /// <summary>
    /// The new league
    /// </summary>
    [JsonPropertyName("league")]
    public string? League { get; init; }
    
    /// <summary>
    /// The new country value
    /// </summary>
    [JsonPropertyName("country")]
    [NotEmptyIfPresent(ErrorMessage = "The Country field cannot be null or empty if provided.")]
    public string? Country { get; init; }
}