using System.Text.Json.Serialization;

namespace MyCompany.Api.Controllers.v1.Team.Dto;

/// <summary>
/// <inheritdoc cref="TeamDetails"/>
/// </summary>
public sealed record CreateTeamRequest : TeamDetails
{
    // To avoid a lot of duplication, overwrite and hide the ID from create request (autoincrement)
    [JsonIgnore]
    public override long Id { get; init; }
}
