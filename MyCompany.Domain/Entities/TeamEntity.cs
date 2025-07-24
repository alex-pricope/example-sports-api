namespace MyCompany.Domain.Entities;

public sealed class TeamEntity : Entity
{
    public override long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? League { get; set; }
    public string Country { get; set; } = string.Empty;
    
    public ICollection<PlayerEntity> Players { get; init; } = new List<PlayerEntity>();
}