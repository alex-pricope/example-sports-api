using MyCompany.Domain.Exceptions;
using MyCompany.Domain.Models;
using MyCompany.Domain.Models.Actions;

namespace MyCompany.Domain.Interfaces;

/// <summary>
/// The manager service for players
/// </summary>
public interface IPlayerManagementService
{
    /// <summary>
    /// Adds a new player. The player is always associated with a team. 
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="player">The new player </param>
    /// <exception cref="EntityAlreadyExistsException">When either ID or name already exists</exception>
    /// <returns>The domain <see cref="Player"/></returns>
    Task<Player?> AddNewPlayer(long teamId, NewPlayer player);
    
    /// <summary>
    /// Get the player via ID
    /// </summary>
    /// <param name="playerId">The player ID</param>
    /// <returns>The domain nullable <see cref="Player"/>Existing player or null (if missing)</returns>
    Task<Player?> GetPlayerAsync(long playerId);

    /// <summary>
    /// Update an existing player
    /// </summary>
    /// <param name="playerId">The player ID</param>
    /// <param name="updatePlayer">The updated entity</param>
    /// <returns>The domain nullable <see cref="Player"/>Existing updated player or null (if missing)</returns>
    Task<Player?> UpdatePlayerAsync(long playerId, UpdatePlayer updatePlayer);
    
    /// <summary>
    /// Deletes an existing player
    /// </summary>
    /// <param name="playerId">The player ID</param>
    Task DeletePlayerAsync(long playerId);
}