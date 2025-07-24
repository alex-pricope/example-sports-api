using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using AutoMapper;
using MyCompany.Domain.Interfaces;
using MyCompany.Domain.Models.Actions;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Api.Controllers.v1.Team.Dto;
using MyCompany.Api.Middleware;

namespace MyCompany.Api.Controllers.v1.Team;


[Route("api/v1/teams")]
[ApiController]
public class TeamController : ControllerBase
{
    private readonly ILogger<TeamController> _logger;
    private readonly ITeamManagementService _teamManagementService;
    private readonly IMapper _mapper;

    public TeamController(ILogger<TeamController> logger, ITeamManagementService teamManagementService, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(teamManagementService);
        ArgumentNullException.ThrowIfNull(mapper);

        _logger = logger;
        _teamManagementService = teamManagementService;
        _mapper = mapper;
    }

    
    /// <param name="teamCreateRequest">Input type: <see cref="CreateTeamRequest"/></param>
    /// <returns>Return type: <see cref="CreateTeamResponse"/></returns>
    /// <response code="201">Returns the newly created team details: <see cref="CreateTeamResponse"/></response>
    /// <response code="400">If request is invalid</response>
    /// <response code="409">The team already exists (same name)</response>
    /// <response code="500">Unexpected error</response>
    [HttpPost]
    [Produces(contentType: MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateTeamResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<CreateTeamResponse>> Post([FromBody] CreateTeamRequest teamCreateRequest)
    {
        // no need to try catch, the error middleware will do that
        var newTeam = _mapper.Map<NewTeam>(teamCreateRequest);
        var createdTeam = await _teamManagementService.CreateTeamAsync(newTeam);

        var responseObj = _mapper.Map<CreateTeamResponse>(createdTeam);
        return CreatedAtAction(nameof(Post), new { id = responseObj.Id }, responseObj);
    }

    /// <summary>
    /// Retrieves team details via ID.
    /// </summary>
    /// <returns>The team details: <see cref="GetTeamDetailsResponse"/></returns>
    /// <response code="200">Returns the team details</response>
    /// <response code="400">If the input teamId is empty</response>
    /// <response code="404">If the team does not exist</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{teamId:long}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTeamDetailsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<GetTeamDetailsResponse>> Get([Required] long teamId)
    {
        // if the result is null, return 404
        var existingTeam = await _teamManagementService.GetTeamDetailsByIdAsync(teamId);
        if (existingTeam is not null)
        {
            var responseObj = _mapper.Map<GetTeamDetailsResponse>(existingTeam);
            return Ok(responseObj);
        }
        
        _logger.LogWarning("Team with teamId: {team_id} does not exist", teamId);
        return NotFound($"team with id: {teamId} not found");
    }

    /// <summary>
    /// Retrieves all teams details.
    /// </summary>
    /// <returns>A list team details: <see cref="GetTeamDetailsResponse"/></returns>
    /// <response code="200">Returns the team details</response>
    /// <response code="400">If the input teamId is empty</response>
    /// <response code="404">If the team does not exist</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet()]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetTeamDetailsResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<List<GetTeamDetailsResponse>>> GetAll()
    {
        var existingTeams = await _teamManagementService.GetAllTeamsDetailsAsync();
        if (existingTeams == null || existingTeams.Count == 0)
        {
            _logger.LogWarning("No teams found");
            return Ok(new List<GetTeamDetailsResponse>());
        }

        var returnObj = existingTeams.Select(team => _mapper
            .Map<GetTeamDetailsResponse>(team)).ToList();
        return Ok(returnObj);
    }

    /// <summary>
    /// Retrieves team details with players via ID.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <returns>The team details with players: <see cref="GetTeamDetailsWithPlayers"/></returns>
    /// <response code="200">Returns the team details with players</response>
    /// <response code="400">If the input teamId is empty</response>
    /// <response code="404">If the team does not exist</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{teamId:long}/players")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTeamDetailsWithPlayers))]
    [ProducesResponseType(StatusCodes.Status400BadRequest,  Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<GetTeamDetailsWithPlayers>> GetWithPlayers([Required] long teamId)
    {
        var existingTeam = await _teamManagementService.GetTeamDetailsByIdAsync(teamId, includePlayers: true);
        if (existingTeam is not null)
        {
            var responseObj = _mapper.Map<GetTeamDetailsWithPlayers>(existingTeam);
            return Ok(responseObj);
        }

        _logger.LogWarning("Team with teamId: {team_id} does not exist", teamId);
        return NotFound($"team with id: {teamId} not found");
    }

    /// <summary>
    /// Update team details
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="patchTeamRequest"><see cref="PatchTeamRequest"/></param>
    /// <response code="200"></response>
    /// <response code="400">If request is invalid</response>
    /// <response code="404">The team was not found</response>
    /// <response code="500">Unexpected error</response>
    /// <returns>The updated team details: <see cref="GetTeamDetailsResponse"/></returns>
    [HttpPatch("{teamId:long}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTeamDetailsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<GetTeamDetailsResponse>> Patch(
        [Required] long teamId,
        [FromBody] PatchTeamRequest patchTeamRequest)
    {
        var updatedTeamData = _mapper.Map<UpdateTeam>(patchTeamRequest);
        var updatedTeam = await _teamManagementService.UpdateTeamAsync(teamId, updatedTeamData);

        if (updatedTeam is null)
        {
            _logger.LogWarning("Team with teamId: {team_id} does not exist", teamId);
            return NotFound($"team with id: {teamId} not found");
        }
        
        var responseObj = _mapper.Map<GetTeamDetailsResponse>(updatedTeam);
        return Ok(responseObj);
    }
    
    /// <summary>
    /// Deletes a team (together with it's players)
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="400">If the input id is empty</response>
    /// <response code="500">Unexpected error</response>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteById(long id)
    {
        // idempotent: call this many times, does not matter
        await _teamManagementService.DeleteTeamAsync(id);
        
        return NoContent();
    }
}