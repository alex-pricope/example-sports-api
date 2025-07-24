using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using AutoMapper;
using MyCompany.Domain.Interfaces;
using MyCompany.Domain.Models.Actions;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Api.Controllers.v1.Players.Dto;
using MyCompany.Api.Middleware;

namespace MyCompany.Api.Controllers.v1.Players;

[Route("api/v1")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly IPlayerManagementService _playerManagementService;
    private readonly IMapper _mapper;

    public PlayerController(ILogger<PlayerController> logger, IPlayerManagementService playerManagementService, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(playerManagementService);
        ArgumentNullException.ThrowIfNull(mapper);

        _logger = logger;
        _playerManagementService = playerManagementService;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Add a player to a team (create a new player)
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="playerCreateRequest">Input type: <see cref="CreatePlayerRequest"/></param>
    /// <returns>Return type: <see cref="CreatePlayerResponse"/></returns>
    /// <response code="201">Returns the newly created player details together with the team: <see cref="CreatePlayerResponse"/></response>
    /// <response code="400">If request is invalid</response>
    /// <response code="404">The team was not found</response>
    /// <response code="409">The player already exists (same name)</response>
    /// <response code="500">Unexpected error</response>
    [HttpPost("teams/{teamId:long}/players")]
    [Produces(contentType: MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatePlayerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<CreatePlayerResponse>> Post(
        [Required] long teamId,
        [FromBody] CreatePlayerRequest playerCreateRequest)
    {
        var newPlayer = _mapper.Map<NewPlayer>(playerCreateRequest);
        var createdPlayer = await _playerManagementService.AddNewPlayer(teamId, newPlayer);

        if (createdPlayer is null)
        {
            _logger.LogWarning("Team with teamId: {team_id} does not exist", teamId);
            return NotFound($"team with teamId: {teamId} not found");
        }
        
        var responseObj = _mapper.Map<CreatePlayerResponse>(createdPlayer);
        return CreatedAtAction(nameof(Post), new { id = responseObj.Id }, responseObj);
    }

    /// <summary>
    /// Retrieves player details via ID.
    /// </summary>
    /// <returns>The player details: <see cref="GetPlayerResponse"/></returns>
    /// <response code="200">Returns the player details</response>
    /// <response code="400">If the input playerId is empty</response>
    /// <response code="404">If the player does not exist</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("players/{playerId:long}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPlayerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<GetPlayerResponse>> Get([Required] long playerId)
    {
        var existingPlayer = await _playerManagementService.GetPlayerAsync(playerId);
        if (existingPlayer is not null)
        {
            var responseObj = _mapper.Map<GetPlayerResponse>(existingPlayer);
            return Ok(responseObj);
        }

        _logger.LogWarning("Player with playerId: {player_id} does not exist", playerId);
        return NotFound($"player with playerId: {playerId} not found");
    }

    /// <summary>
    /// Update player details
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <param name="patchPlayerRequest"><see cref="PatchPlayerRequest"/></param>
    /// <returns>The updated team details: <see cref="GetPlayerResponse"/></returns>
    [HttpPatch("players/{playerId:long}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPlayerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetPlayerResponse>> Patch([Required] long playerId,
        [FromBody] PatchPlayerRequest patchPlayerRequest)
    {
        var updatedPlayerData = _mapper.Map<UpdatePlayer>(patchPlayerRequest);
        var updatedPlayer = await _playerManagementService.UpdatePlayerAsync(playerId, updatedPlayerData);

        if (updatedPlayer is not null)
        {
            var responseObj = _mapper.Map<GetPlayerResponse>(updatedPlayer);
            return Ok(responseObj);
        }

        _logger.LogWarning("Player with playerId: {player_id} does not exist", playerId);
        return NotFound($"player with playerId: {playerId} not found");
    }

    /// <summary>
    /// Deletes a player
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="400">If the input id is empty</response>
    /// <response code="500">Unexpected error</response>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    [HttpDelete("players/{id:long}")]
    public async Task<ActionResult> DeleteById(long id)
    {
        // idempotent: call this many times, does not matter
        await _playerManagementService.DeletePlayerAsync(id);
        
        return NoContent();
    }
}