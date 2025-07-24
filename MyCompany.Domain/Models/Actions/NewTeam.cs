namespace MyCompany.Domain.Models.Actions;

/// <summary>
/// Domain model for a new team request
/// </summary>
public sealed record NewTeam
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? League { get; init; }
    public required string Country { get; init; }
}