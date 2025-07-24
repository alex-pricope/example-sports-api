using MyCompany.Domain.Entities;
using MyCompany.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyCompany.Data;

/// <summary>
/// <inheritdoc cref="ITeamRepository"/>
/// </summary>
public class TeamRepository(ILogger<TeamRepository> logger, ApplicationDbContext dbContext)
    : RepositoryBase(logger, dbContext), ITeamRepository
{
    public async Task<TeamEntity> AddAsync(TeamEntity team)
    {
        var addedEntity = await Context.Set<TeamEntity>().AddAsync(team);
        await Context.SaveChangesAsync();
        
        Logger.LogInformation("team_repository: Added team with id: {team_id}", team.Id);
        return addedEntity.Entity;
    }

    public async Task<TeamEntity?> GetByIdAsync(long teamId, bool includePlayers = false)
    {
        IQueryable<TeamEntity> query = Context.Set<TeamEntity>();

        if (includePlayers)
            query = query.Include(t => t.Players);

        Logger.LogInformation("team_repository: Got team with id: {team_id}", teamId);
        return await query.FirstOrDefaultAsync(t => t.Id == teamId);
    }

    public async Task<TeamEntity?> GetByNameAsync(string name)
    {
        return await Context.Set<TeamEntity>()
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<List<TeamEntity>?> GetAllAsync()
    {
        var teams = await Context.Set<TeamEntity>().ToListAsync();
        Logger.LogInformation("team_repository: Getting {team_count} teams", teams.Count);
        return teams;
    }

    public async Task<TeamEntity> UpdateAsync(TeamEntity team)
    {
        Context.Set<TeamEntity>().Entry(team).CurrentValues.SetValues(team);
        await Context.SaveChangesAsync();
        
        Logger.LogInformation("team_repository: Updated team with id: {team_id}", team.Id);
        return team;
    }

    public async Task DeleteAsync(long teamId)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(); // Begin the transaction
        try
        {
            // Find the team
            var team = await Context.Set<TeamEntity>().FindAsync(teamId);
            if (team != null)
            {
                // Delete associated players
                var players = Context.Set<PlayerEntity>().Where(p => p.TeamId == teamId);
                var playerCount = await players.CountAsync();

                if (playerCount > 0)
                    Logger.LogWarning(
                        "team_repository: Deleting team with ID {team_id} will also delete {player_count} players.",
                        teamId, playerCount);
                else
                    Logger.LogWarning(
                        "team_repository: Team with ID {team_id} has no associated players to delete.", teamId);

                Context.Set<PlayerEntity>().RemoveRange(players);

                // Delete the team
                Context.Set<TeamEntity>().Remove(team);

                // Save changes
                await Context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); // Rollback the transaction if an error occurs
            Logger.LogError(ex, "team_repository: Error deleting team with id: {team_id}", teamId);
            throw;
        }
    }
}