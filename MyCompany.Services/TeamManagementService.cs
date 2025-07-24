using AutoMapper;
using MyCompany.Domain.Entities;
using MyCompany.Domain.Exceptions;
using MyCompany.Domain.Interfaces;
using MyCompany.Domain.Models;
using MyCompany.Domain.Models.Actions;
using MyCompany.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace MyCompany.Services;

/// <summary>
/// <inheritdoc cref="ITeamManagementService"/>
/// </summary>
public sealed class TeamManagementService : ITeamManagementService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public TeamManagementService(ILogger<TeamManagementService> logger, ITeamRepository teamRepository,
        IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(teamRepository, nameof(teamRepository));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        _logger = logger;
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    public async Task<Team> CreateTeamAsync(NewTeam team)
    {
        ArgumentNullException.ThrowIfNull(team, nameof(team));
        
        // Map
        var teamEntity = _mapper.Map<TeamEntity>(team);
        
        // If a team exists with the same name then throw EntityAlreadyExistsException
        var existingTeam = await _teamRepository.GetByNameAsync(team.Name);
        if (existingTeam != null)
        {
            _logger.LogWarning("[team_service]: Team already exists");
            throw new EntityAlreadyExistsException("A team with the same name already exists.");
        }
        
        var added = await _teamRepository.AddAsync(teamEntity);
        return _mapper.Map<Team>(added);
    }

    public async Task<Team?> GetTeamDetailsByIdAsync(long teamId, bool includePlayers = false)
    {
        if (teamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(teamId), "Team ID must be greater than 0.");

        // Fetch team entity from repository
        var teamEntity = await _teamRepository.GetByIdAsync(teamId, includePlayers);

        // Return null if no team was found
        if (teamEntity == null)
        {
            _logger.LogWarning("team_service: Team with ID {team_id} not found.", teamId);
            return null;
        }

        // Map the persistence model to the domain model
        return _mapper.Map<Team>(teamEntity);
    }

    public async Task<List<Team>?> GetAllTeamsDetailsAsync()
    {
        // Fetch all teams from repository
        var teamEntities = await _teamRepository.GetAllAsync();

        // Return null if no teams exist
        if (teamEntities is { Count: 0 })
        {
            _logger.LogWarning("team_service: No teams found.");
            return null;
        }

        // Map the list of persistence models to the domain models
        return _mapper.Map<List<Team>>(teamEntities);
    }

    public async Task<Team?> UpdateTeamAsync(long teamId, UpdateTeam updateTeam)
    {
        if (teamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(teamId), "Team ID must be greater than 0.");

        if (updateTeam == null)
            throw new ArgumentNullException(nameof(updateTeam));

        // Fetch the existing team from the repository
        var existingTeamEntity = await _teamRepository.GetByIdAsync(teamId);

        // Log and return null if the team doesn't exist
        if (existingTeamEntity == null)
        {
            _logger.LogWarning("team_service: Team with ID {team_id} not found for update.", teamId);
            return null;
        }

        // Map the updated properties from the domain model to the persistence model
        _mapper.Map(updateTeam, existingTeamEntity);

        // Update the team in the repository
        var updatedTeamEntity = await _teamRepository.UpdateAsync(existingTeamEntity);

        // Map the updated persistence model back to the domain model
        return _mapper.Map<Team>(updatedTeamEntity);
    }

    public async Task DeleteTeamAsync(long teamId)
    {
        if (teamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(teamId), "Team ID must be greater than 0.");
        
        await _teamRepository.DeleteAsync(teamId);
    }
}