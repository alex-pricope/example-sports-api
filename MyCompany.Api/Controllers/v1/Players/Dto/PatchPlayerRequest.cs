using System.Text.Json.Serialization;
using MyCompany.Api.Controllers.Validators;

namespace MyCompany.Api.Controllers.v1.Players.Dto;

public sealed record PatchPlayerRequest
{
    /// <summary>
    /// Updated first name
    /// </summary>
    [JsonPropertyName("first_name")]
    [NotEmptyIfPresent(ErrorMessage = "The FirstName field cannot be empty if provided.")]
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Updated last name
    /// </summary>
    [JsonPropertyName("last_name")]
    [NotEmptyIfPresent(ErrorMessage = "The LastName field cannot be empty if provided.")]
    public string? LastName { get; set; }
    
    /// <summary>
    /// Updated date of birth
    /// </summary>
    [JsonPropertyName("dob")]
    [NotEmptyIfPresent(ErrorMessage = "The DateOfBirth field cannot empty if provided.")]
    public DateOnly? DateOfBirth { get; set; }
    
    /// <summary>
    /// Updated height
    /// </summary>
    [JsonPropertyName("height")]
    public double? Height { get; init; }
    
    /// <summary>
    /// Updated citizenship
    /// </summary>
    [JsonPropertyName("citizenship")]
    public string? Citizenship { get; init; }
    
    /// <summary>
    /// Updated place of birth
    /// </summary>
    [JsonPropertyName("place_of_birth")]
    public string? PlaceOfBirth { get; init; }
}