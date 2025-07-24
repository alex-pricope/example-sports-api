using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Api.Controllers.Validators;
using MyCompany.Api.ModelBinder;

namespace MyCompany.Api.Controllers.v1.Players.Dto;

/// <summary>
/// Player Details DTO
/// </summary>
public record PlayerDetails
{
    /// <summary>
    /// The player ID
    /// </summary>
    [Description("The player ID")]
    [JsonPropertyName("id")]
    public virtual long Id { get; init; }

    /// <summary>
    /// The team ID
    /// </summary>
    [JsonPropertyName("team_id")]
    public virtual long TeamId { get; init; }

    /// <summary>
    /// First Name
    /// </summary>
    [JsonPropertyName("first_name")]
    [Required(ErrorMessage = "First name is required", AllowEmptyStrings = false)]
    public required string FirstName { get; init; }

    /// <summary>
    /// Last name
    /// </summary>
    [JsonPropertyName("last_name")]
    [Required(ErrorMessage = "Last name is required", AllowEmptyStrings = false)]
    public required string LastName { get; init; }

    /// <summary>
    /// Date of birth
    /// </summary>
    [ModelBinder(BinderType = typeof(DateOnlyModelBinder))]
    [ValidDateOnlyValidator(minAge: 10)]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Date of birth is required")]
    [JsonPropertyName("dob")]
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Height in meters
    /// </summary>
    [JsonPropertyName("height")]
    public double? Height { get; init; }

    /// <summary>
    /// Citizenship
    /// </summary>
    [JsonPropertyName("citizenship")]
    public string? Citizenship { get; init; }

    /// <summary>
    /// Place of birth
    /// </summary>
    [JsonPropertyName("place_of_birth")]
    public string? PlaceOfBirth { get; init; }
    
    /// <summary>
    /// Position on the team 
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; init; }
}