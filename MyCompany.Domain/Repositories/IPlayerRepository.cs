using MyCompany.Domain.Entities;

namespace MyCompany.Domain.Repositories;
/// <summary>
/// Player Repository
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Add new <see cref="PlayerEntity"/> to the store
    /// </summary>
    /// <param name="player">The new <see cref="PlayerEntity"/></param>
    /// <returns>Returns <see cref="PlayerEntity"/></returns>
    Task<PlayerEntity> AddAsync(PlayerEntity player);
    
    /// <summary>
    /// Gets a <see cref="PlayerEntity"/> by ID
    /// </summary>
    /// <param name="playerId">The player ID</param>
    /// <returns>Nullable <see cref="PlayerEntity"/>. Returns null when player does not exist</returns>
    Task<PlayerEntity?> GetByIdAsync(long playerId);
    
    /// <summary>
    /// Updates an existing <see cref="PlayerEntity"/>
    /// </summary>
    /// <param name="player">The input</param>
    /// <returns>Returns <see cref="PlayerEntity"/></returns>
    Task<PlayerEntity> UpdateAsync(PlayerEntity player);
    
    /// <summary>
    /// Deletes a player
    /// </summary>
    /// <param name="playerId">The input player ID</param>
    Task DeleteAsync(long playerId);
}