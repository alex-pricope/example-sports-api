namespace MyCompany.Domain.Models.Actions;

/// <summary>
/// Domain model for a new team
/// </summary>
public sealed record UpdatePlayer
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public double? Height { get; init; }
    public string? Citizenship { get; init; }
    public string? PlaceOfBirth { get; init; }
}