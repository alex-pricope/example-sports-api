using AutoMapper;
using MyCompany.Domain.Entities;
using MyCompany.Domain.Interfaces;
using MyCompany.Domain.Models;
using MyCompany.Domain.Models.Actions;
using MyCompany.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace MyCompany.Services;

/// <summary>
/// <inheritdoc cref="IPlayerManagementService"/>
/// </summary>
public sealed class PlayerManagementService : IPlayerManagementService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly ILogger<PlayerManagementService> _logger;
    private readonly IMapper _mapper;

    public PlayerManagementService(ILogger<PlayerManagementService> logger, IPlayerRepository playerRepository,
        ITeamRepository teamRepository, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(playerRepository, nameof(playerRepository));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(teamRepository, nameof(teamRepository));

        _playerRepository = playerRepository;
        _mapper = mapper;
        _logger = logger;
        _teamRepository = teamRepository;
    }

    public async Task<Player?> AddNewPlayer(long teamId, NewPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player, nameof(player));
        if (teamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(teamId), "Team ID must be greater than 0.");
        
        // Check if the team exists in the repository
        var existingTeam = await _teamRepository.GetByIdAsync(teamId);
        if (existingTeam == null)
        {
            _logger.LogWarning("player_service [create]: Team with ID {teamId} does not exist. Cannot add player with {player_name}",
                teamId, $"{player.FirstName} {player.LastName}");
            return null;
        }

        var playerEntity = _mapper.Map<PlayerEntity>(player);
        playerEntity.TeamId = teamId;
        var added = await _playerRepository.AddAsync(playerEntity);

        _logger.LogInformation("player_service [create]: Player {player_name} added successfully to team {team_id}",
            $"{playerEntity.FirstName} {playerEntity.LastName}", teamId);
        
        return _mapper.Map<Player>(added);
    }

    public async Task<Player?> GetPlayerAsync(long playerId)
    {
        if (playerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(playerId), "Player ID must be greater than 0.");
        
        var existingPlayerEntity = await _playerRepository.GetByIdAsync(playerId);
        if (existingPlayerEntity == null)
        {
            _logger.LogWarning("player_service [get]: Player with ID {player_id} not found", playerId);
            return null;
        }
        
        return _mapper.Map<Player>(existingPlayerEntity);
    }

    public async Task<Player?> UpdatePlayerAsync(long playerId, UpdatePlayer updatePlayer)
    {
        if (playerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(playerId), "Player ID must be greater than 0.");

        var existingPlayerEntity = await _playerRepository.GetByIdAsync(playerId);
        if (existingPlayerEntity == null)
        {
            _logger.LogWarning("player_service [update]: Player with ID {player_id} not found", playerId);
            return null;
        }
        
        // Map updated properties from DTO to the existing player entity
        _mapper.Map(updatePlayer, existingPlayerEntity);

        // Update the player in the repository
        var updatedPlayerEntity = await _playerRepository.UpdateAsync(existingPlayerEntity);
        
        return _mapper.Map<Player>(updatedPlayerEntity);
    }

    public async Task DeletePlayerAsync(long playerId)
    {
        if (playerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(playerId), "Player ID must be greater than 0.");

        await _playerRepository.DeleteAsync(playerId);
    }
}