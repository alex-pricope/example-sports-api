namespace MyCompany.Domain.Models.Actions;

/// <summary>
/// Domain model for a new player
/// </summary>
public sealed record NewPlayer
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public double? Height { get; init; }
    public string? Citizenship { get; init; }
    public string? PlaceOfBirth { get; init; }
    public string? Position { get; init; }
    public required DateOnly DateOfBirth { get; init; }
}

