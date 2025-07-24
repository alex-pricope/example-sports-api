using MyCompany.Domain.Exceptions;
using MyCompany.Domain.Models;
using MyCompany.Domain.Models.Actions;

namespace MyCompany.Domain.Interfaces;

/// <summary>
/// The manager service for teams
/// </summary>
public interface ITeamManagementService
{
    /// <summary>
    /// Adds a team to the service
    /// </summary>
    /// <param name="team">The new team</param>
    /// <exception cref="EntityAlreadyExistsException">When either ID or name already exists</exception>
    /// <returns>The domain <see cref="Team"/></returns>
    Task<Team> CreateTeamAsync(NewTeam team);
    
    /// <summary>
    /// <para>Gets the team details by input ID.</para>
    /// <para>Include optional bool to return the player info as well</para>
    /// </summary>
    /// <param name="teamId">The ID of the team</param>
    /// <param name="includePlayers">Optional flag to return the players as well</param>
    /// <returns>The domain nullable <see cref="Team"/>Existing team or null (if missing)</returns>
    Task<Team?> GetTeamDetailsByIdAsync(long teamId, bool includePlayers = false);
    
    /// <summary>
    /// Return all team details
    /// </summary>
    /// <returns>List of <see cref="Team"/></returns>
    Task<List<Team>?> GetAllTeamsDetailsAsync();

    /// <summary>
    /// Update the team details
    /// </summary>
    /// <param name="teamId">The ID of the team</param>
    /// <param name="updateTeam">Input <see cref="UpdateTeam"/></param>
    /// <returns>The domain nullable <see cref="Team"/>Existing updated team or null (if missing)</returns>
    Task<Team?> UpdateTeamAsync(long teamId, UpdateTeam updateTeam);
    
    /// <summary>
    /// Deletes the team (and associated players)
    /// </summary>
    /// <param name="teamId">The ID of the team</param>
    Task DeleteTeamAsync(long teamId);
}