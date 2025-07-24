namespace MyCompany.Domain.Models;

/// <summary>
/// Domain type for player
/// </summary>
public sealed record Player
{
    public required long Id { get; init; }
    public required long TeamId { get; init; }
    
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    
    public double? Height { get; init; }
    public string? Citizenship { get; init; }
    public string? PlaceOfBirth { get; init; }
    public string? Position { get; init; }
    
    public Team? Team { get; init; }
}