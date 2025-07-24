namespace MyCompany.Domain.Models;

/// <summary>
/// Domain type for Team
/// </summary>
public sealed record Team
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? League { get; init; }
    public required string Country { get; init; }
    public List<Player>? Players { get; init; }
}