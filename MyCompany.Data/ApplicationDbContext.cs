using MyCompany.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyCompany.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TeamEntity> Teams { get; init; }
    public DbSet<PlayerEntity> Players { get; init; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure TeamEntity
        modelBuilder.Entity<TeamEntity>()
            .HasKey(t => t.Id);

        // Enable auto-increment for TeamEntity.Id
        modelBuilder.Entity<TeamEntity>()
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        // Ensure Name is non-null + limit
        modelBuilder.Entity<TeamEntity>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Ensure Country is non-null + limit
        modelBuilder.Entity<TeamEntity>()
            .Property(t => t.Country)
            .IsRequired()
            .HasMaxLength(100);

        // Limit for description
        modelBuilder.Entity<TeamEntity>()
            .Property(t => t.Description)
            .HasMaxLength(500);

        // Limit for league
        modelBuilder.Entity<TeamEntity>()
            .Property(t => t.League)
            .HasMaxLength(100);

        // Configure PlayerEntity
        modelBuilder.Entity<PlayerEntity>()
            .HasKey(p => p.Id);

        // Auto-generated ID
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        // Ensure FirstName is not-null + limit
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        // Ensure LastName is not-null + limit
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Ensure DateOfBirth is non-null
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.DateOfBirth)
            .IsRequired();

        // Citizenship limit
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.Citizenship)
            .HasMaxLength(100);

        // Place of birth limit
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.PlaceOfBirth)
            .HasMaxLength(100);
        
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.Position)
            .HasMaxLength(100);

        // Ensure no player has no team
        modelBuilder.Entity<PlayerEntity>()
            .Property(p => p.TeamId)
            .IsRequired();

        // Define the relationship between Team and Player
        modelBuilder.Entity<TeamEntity>()
            .HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .IsRequired();

        // Add an index for TeamId in PlayerEntity
        modelBuilder.Entity<PlayerEntity>()
            .HasIndex(p => p.TeamId);

        base.OnModelCreating(modelBuilder);
    }
}