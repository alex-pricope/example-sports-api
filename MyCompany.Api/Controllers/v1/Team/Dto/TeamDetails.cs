using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyCompany.Api.Controllers.v1.Team.Dto;

/// <summary>
/// Team details DTO
/// </summary>
public record TeamDetails
{
    /// <summary>
    /// The team ID
    /// </summary>
    [Description("The team ID")]
    [JsonPropertyName("id")]
    public virtual long Id { get; init; }

    /// <summary>
    /// The team name
    /// </summary>
    [Description("Team name")]
    [Required(ErrorMessage = "team name is required", AllowEmptyStrings = false)]
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// (Optional) team description
    /// </summary>
    [Description("(optional) Team description")]
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// (Optional) team league
    /// </summary>
    [Description("(optional) Team league")]
    [JsonPropertyName("league")] 
    public string? League { get; init; }

    /// <summary>
    /// Team country
    /// </summary>
    [Description("Team country")]
    [Required(ErrorMessage = "team country is required", AllowEmptyStrings = false)]
    [JsonPropertyName("country")] 
    public required string Country { get; init; }
}