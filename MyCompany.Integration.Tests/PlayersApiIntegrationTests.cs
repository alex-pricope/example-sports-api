using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyCompany.Api.Controllers.v1.Players.Dto;
using MyCompany.Api.Controllers.v1.Team.Dto;
using NUnit.Framework;

namespace MyCompany.Integration.Tests;

[TestFixture]
public class PlayerControllerIntegrationTests : TestingBase
{
    private const string TeamsRoute = "/api/v1/teams";
    private const string PlayersRoute = "/api/v1/players";

    private List<long> _createdTeamIds;
    private List<long> _createdPlayerIds;

    [SetUp]
    public void SetUp()
    {
        _createdTeamIds = [];
        _createdPlayerIds = [];
    }

    [TearDown]
    public async Task TearDown()
    {
        // Cleanup players
        foreach (var playerId in _createdPlayerIds)
            await DoDeleteRequest($"{PlayersRoute}/{playerId}");

        // Cleanup teams
        foreach (var teamId in _createdTeamIds)
            await DoDeleteRequest($"{TeamsRoute}/{teamId}");
    }

    #region POST Tests

    [Test]
    public async Task Post_ShouldCreatePlayer_WhenValidRequest()
    {
        // Arrange: Create a team
        var teamRequest = new CreateTeamRequest { Name = "Team A", Country = "Country A" };
        var postResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        var playerRequest = new CreatePlayerRequest
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Position = "Forward"
        };

        // Act
        var playerResponse = await DoPostRequest($"{TeamsRoute}/{teamId}/players", playerRequest);
        var playerData = await playerResponse.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        if (playerData != null)
        {
            _createdPlayerIds.Add(playerData.Id);
        }

        // Assert
        playerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        playerData.Should().NotBeNull();
        playerData!.FirstName.Should().Be("John");
        playerData.LastName.Should().Be("Doe");
        playerData.Position.Should().Be("Forward");
    }

    [Test]
    public async Task Post_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        var invalidTeamId = 99999;
        var playerRequest = new CreatePlayerRequest
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Position = "Forward"
        };

        // Act
        var response = await DoPostRequest($"{TeamsRoute}/{invalidTeamId}/players", playerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET Tests

    [Test]
    public async Task Get_ShouldReturnPlayer_WhenPlayerExists()
    {
        // Arrange: Create a team and a player
        var teamRequest = new CreateTeamRequest { Name = "Team B", Country = "Country B" };
        var teamResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await teamResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        var playerRequest = new CreatePlayerRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1992, 5, 15),
            Position = "Midfielder"
        };
        var playerResponse = await DoPostRequest($"{TeamsRoute}/{teamId}/players", playerRequest);
        var playerData = await playerResponse.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        var playerId = playerData!.Id;
        _createdPlayerIds.Add(playerId);

        // Act
        var response = await DoGetRequest($"{PlayersRoute}/{playerId}");
        var responseData = await response.Content.ReadFromJsonAsync<GetPlayerResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData!.FirstName.Should().Be("Jane");
        responseData.LastName.Should().Be("Smith");
    }

    [Test]
    public async Task Get_ShouldReturnNotFound_WhenPlayerDoesNotExist()
    {
        // Act
        var response = await DoGetRequest($"{PlayersRoute}/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region PATCH Tests

    [Test]
    public async Task Patch_ShouldUpdatePlayer_WhenValidRequest()
    {
        // Arrange: Create a team and a player
        var teamRequest = new CreateTeamRequest { Name = "Team C", Country = "Country C" };
        var teamResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await teamResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        var playerRequest = new CreatePlayerRequest
        {
            FirstName = "Mike",
            LastName = "Jordan",
            DateOfBirth = new DateOnly(1990, 7, 23),
            Position = "Guard"
        };
        var playerResponse = await DoPostRequest($"{TeamsRoute}/{teamId}/players", playerRequest);
        var playerData = await playerResponse.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        var playerId = playerData!.Id;
        _createdPlayerIds.Add(playerId);

        var patchRequest = new PatchPlayerRequest
        {
            FirstName = "Michael",
            LastName = "Jordan"
        };

        // Act
        var patchResponse = await DoPatchRequest($"{PlayersRoute}/{playerId}", patchRequest);
        var responseData = await patchResponse.Content.ReadFromJsonAsync<GetPlayerResponse>();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData!.FirstName.Should().Be("Michael");
        responseData.LastName.Should().Be("Jordan");
    }

    [Test]
    public async Task Patch_ShouldReturnNotFound_WhenPlayerDoesNotExist()
    {
        // Arrange
        var patchRequest = new PatchPlayerRequest { FirstName = "Updated Name" };

        // Act
        var response = await DoPatchRequest($"{PlayersRoute}/99999", patchRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task Patch_ShouldReturnBadRequest_WhenDateOfBirthIsDefault()
    {
        // Arrange: Create a team and a player
        var teamRequest = new CreateTeamRequest { Name = "Team E", Country = "Country E" };
        var teamResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await teamResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        var playerRequest = new CreatePlayerRequest
        {
            FirstName = "Mike",
            LastName = "Brown",
            DateOfBirth = new DateOnly(1985, 5, 20),
            Position = "Defender"
        };
        var playerResponse = await DoPostRequest($"{TeamsRoute}/{teamId}/players", playerRequest);
        var playerData = await playerResponse.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        var playerId = playerData!.Id;
        _createdPlayerIds.Add(playerId);

        var invalidPatchRequest = new PatchPlayerRequest
        {
            DateOfBirth = new DateOnly()
        };

        // Act
        var patchResponse = await DoPatchRequest($"{PlayersRoute}/{playerId}", invalidPatchRequest);
        var responseContent = await patchResponse.Content.ReadAsStringAsync();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().Contain("The DateOfBirth field cannot empty");
    }
    
    [Test]
    public async Task Patch_ShouldReturnBadRequest_WhenFirstNameIsEmpty()
    {
        // Arrange: Create a team and a player
        var teamRequest = new CreateTeamRequest { Name = "Team F", Country = "Country F" };
        var teamResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await teamResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        var playerRequest = new CreatePlayerRequest
        {
            FirstName = "Sara",
            LastName = "Connor",
            DateOfBirth = new DateOnly(1980, 7, 15),
            Position = "Striker"
        };
        var playerResponse = await DoPostRequest($"{TeamsRoute}/{teamId}/players", playerRequest);
        var playerData = await playerResponse.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        var playerId = playerData!.Id;
        _createdPlayerIds.Add(playerId);

        var invalidPatchRequest = new PatchPlayerRequest
        {
            FirstName = "",
            LastName = "Connor"
        };

        // Act
        var patchResponse = await DoPatchRequest($"{PlayersRoute}/{playerId}", invalidPatchRequest);
        var responseContent = await patchResponse.Content.ReadAsStringAsync();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().Contain("The FirstName field cannot be empty");
    }
    
    #endregion

    #region DELETE Tests

    [Test]
    public async Task DeleteById_ShouldReturnNoContent_WhenPlayerExists()
    {
        // Arrange: Create a team and a player
        var teamRequest = new CreateTeamRequest { Name = "Team D", Country = "Country D" };
        var teamResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await teamResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        var playerRequest = new CreatePlayerRequest
        {
            FirstName = "Leo",
            LastName = "Messi",
            DateOfBirth = new DateOnly(1987, 6, 24),
            Position = "Forward"
        };
        var playerResponse = await DoPostRequest($"{TeamsRoute}/{teamId}/players", playerRequest);
        var playerData = await playerResponse.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        var playerId = playerData!.Id;
        _createdPlayerIds.Add(playerId);

        // Act
        var deleteResponse = await DoDeleteRequest($"{PlayersRoute}/{playerId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _createdPlayerIds.Remove(playerId); // Already deleted
    }

    [Test]
    public async Task DeleteById_ShouldReturnNotFound_WhenPlayerDoesNotExist()
    {
        // Act
        var response = await DoDeleteRequest($"{PlayersRoute}/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent); // Idempotency ensures no error
    }

    #endregion
}