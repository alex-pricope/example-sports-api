using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCompany.Domain.Entities;
using MyCompany.Domain.Repositories;

namespace MyCompany.Data;

/// <summary>
/// <inheritdoc cref="IPlayerRepository"/>
/// </summary>
public class PlayerRepository(ILogger<TeamRepository> logger, ApplicationDbContext dbContext)
    : RepositoryBase(logger, dbContext), IPlayerRepository
{
    public async Task<PlayerEntity> AddAsync(PlayerEntity player)
    {
        var addedEntity = await Context.Set<PlayerEntity>().AddAsync(player);
        await Context.SaveChangesAsync();

        Logger.LogInformation("player_repository: Added player with id: {player_id}", player.Id);
        return addedEntity.Entity;
    }

    public async Task<PlayerEntity?> GetByIdAsync(long playerId)
    {
        var player = await Context.Set<PlayerEntity>()
            .Include(p => p.Team)
            .FirstOrDefaultAsync(p => p.Id == playerId);
        if (player == null)
            return null;

        Logger.LogInformation("player_repository: Retrieved player with id: {player_id}", player.Id);
        return player;
    }

    public async Task<PlayerEntity> UpdateAsync(PlayerEntity player)
    {
        Context.Set<PlayerEntity>().Entry(player).CurrentValues.SetValues(player);
        await Context.SaveChangesAsync();

        Logger.LogInformation("player_repository: Updated player with id: {player_id}", player.Id);
        return player;
    }

    public async Task DeleteAsync(long playerId)
    {
        var player = await Context.Set<PlayerEntity>().FindAsync(playerId);
        if (player != null)
        {
            Context.Set<PlayerEntity>().Remove(player);
            await Context.SaveChangesAsync();
            Logger.LogWarning("player_repository: Deleted player with id: {player_id}", playerId);
        }
    }
}