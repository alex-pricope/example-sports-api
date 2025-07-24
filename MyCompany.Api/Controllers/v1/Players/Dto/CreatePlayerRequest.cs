using System.Text.Json.Serialization;

namespace MyCompany.Api.Controllers.v1.Players.Dto;

public sealed record CreatePlayerRequest : PlayerDetails
{
    // To avoid a lot of duplication, overwrite these 2 and hide them from create request
    [JsonIgnore]
    public override long Id { get; init; }

    [JsonIgnore]
    public override long TeamId { get; init; }
}
