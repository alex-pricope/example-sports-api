using System.Text.Json.Serialization;
using MyCompany.Api.Controllers.v1.Players.Dto;

namespace MyCompany.Api.Controllers.v1.Team.Dto;

/// <summary>
/// <inheritdoc cref="TeamDetails"/>
/// </summary>
public sealed record GetTeamDetailsWithPlayers : TeamDetails
{
    /// <summary>
    /// The list of the teams assigned players
    /// </summary>
    [JsonPropertyName("players")]
    public List<PlayerDetails>? Players { get; init; }
}