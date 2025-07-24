using System.Text.Json.Serialization;
using MyCompany.Api.Controllers.v1.Team.Dto;

namespace MyCompany.Api.Controllers.v1.Players.Dto;

public sealed record GetPlayerResponse : PlayerDetails
{
    [JsonPropertyName("team")]
    public required TeamDetails Team { get; init; }
}
