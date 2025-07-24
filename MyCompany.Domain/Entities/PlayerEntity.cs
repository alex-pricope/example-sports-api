namespace MyCompany.Domain.Entities;

public sealed class PlayerEntity : Entity
{
    public override long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Citizenship { get; set; }
    public string? PlaceOfBirth { get; set; }
    public double? Height { get; set; }
    public string? Position { get; set; }
    
    // Relationship with Team
    public long TeamId { get; set; }
    public TeamEntity Team { get; set; } = null!;
}