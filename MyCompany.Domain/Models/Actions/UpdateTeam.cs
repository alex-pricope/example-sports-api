namespace MyCompany.Domain.Models.Actions;

/// <summary>
/// Domain model for an update team request
/// </summary>
public sealed record UpdateTeam
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? League { get; init; }
    public string? Country { get; init; }
}