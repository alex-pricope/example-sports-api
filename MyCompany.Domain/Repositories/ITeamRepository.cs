using MyCompany.Domain.Entities;

namespace MyCompany.Domain.Repositories;

/// <summary>
/// Team repository
/// </summary>
public interface ITeamRepository
{
    /// <summary>
    /// Add new <see cref="TeamEntity"/> to the store
    /// </summary>
    /// <param name="team">The new <see cref="TeamEntity"/></param>
    /// <returns>Returns <see cref="TeamEntity"/></returns>
    Task<TeamEntity> AddAsync(TeamEntity team);
    
    /// <summary>
    /// <para>Gets a <see cref="TeamEntity"/> by ID </para>
    /// <para> Include optional bool to include players in the response </para>
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="includePlayers">Optional flag</param>
    /// <returns>Nullable <see cref="TeamEntity"/>. Returns null when team does not exist</returns>
    Task<TeamEntity?> GetByIdAsync(long teamId, bool includePlayers = false);

    /// <summary>
    /// <para>Gets a <see cref="TeamEntity"/> by name </para>
    /// </summary>
    /// <param name="name">The team name</param>
    /// <returns>Nullable <see cref="TeamEntity"/>. Returns null when team does not exist</returns>
    Task<TeamEntity?> GetByNameAsync(string name);
    
    /// <summary>
    /// Returns all teams
    /// </summary>
    /// <returns>Nullable collection of <see cref="TeamEntity"/></returns>
    Task<List<TeamEntity>?> GetAllAsync();
    
    /// <summary>
    /// Updates an existing <see cref="TeamEntity"/>
    /// </summary>
    /// <param name="team">The input</param>
    /// <returns>Returns <see cref="TeamEntity"/></returns>
    Task<TeamEntity> UpdateAsync(TeamEntity team);
    
    /// <summary>
    /// Deletes a team (and associated players)
    /// </summary>
    /// <param name="teamId">The input tea, ID</param>
    Task DeleteAsync(long teamId);
}