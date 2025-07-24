using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyCompany.Api.Controllers.v1.Players.Dto;
using MyCompany.Api.Controllers.v1.Team.Dto;
using NUnit.Framework;

namespace MyCompany.Integration.Tests;

[TestFixture]
public class TeamControllerIntegrationTests : TestingBase
{
    private const string TeamsRoute = "/api/v1/teams";
    private List<long> _createdTeamIds;

    [SetUp]
    public void SetUp()
    {
        _createdTeamIds = [];
    }

    [TearDown]
    public async Task TearDown()
    {
        // Clean up all created teams after each test
        foreach (var teamId in _createdTeamIds)
            await DoDeleteRequest($"{TeamsRoute}/{teamId}");
    }

    #region POST Tests

    [Test]
    public async Task Post_ShouldCreateTeam_WhenValidRequest()
    {
        // Arrange
        var request = new CreateTeamRequest { Name = "Team A", Country = "Country A" };

        // Act
        var response = await DoPostRequest(TeamsRoute, request);
        var responseData = await response.Content.ReadFromJsonAsync<CreateTeamResponse>();

        // Track created team for cleanup
        if (responseData != null)
        {
            _createdTeamIds.Add(responseData.Id);
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        responseData.Should().NotBeNull();
        responseData!.Name.Should().Be("Team A");
        responseData.Country.Should().Be("Country A");
    }

    [Test]
    public async Task Post_ShouldReturnConflict_WhenTeamAlreadyExists()
    {
        // Arrange
        var request = new CreateTeamRequest { Name = "Duplicate Team", Country = "Country B" };
        var postResponse = await DoPostRequest(TeamsRoute, request); // Create the team first
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();

        if (createdTeam != null)
        {
            _createdTeamIds.Add(createdTeam.Id);
        }

        // Act
        var response = await DoPostRequest(TeamsRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task Post_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        // Arrange
        var invalidRequest = new CreateTeamRequest
        {
            Country = "Country C",
            Name = null
        };

        // Act
        var response = await DoPostRequest(TeamsRoute, invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GET Tests

    [Test]
    public async Task Get_ShouldReturnTeam_WhenTeamExists()
    {
        // Arrange
        var request = new CreateTeamRequest { Name = "Existing Team", Country = "Country D" };
        var postResponse = await DoPostRequest(TeamsRoute, request);
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        // Act
        var response = await DoGetRequest($"{TeamsRoute}/{teamId}");
        var responseData = await response.Content.ReadFromJsonAsync<GetTeamDetailsResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData!.Name.Should().Be("Existing Team");
        responseData.Country.Should().Be("Country D");
    }

    [Test]
    public async Task Get_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Act
        var response = await DoGetRequest($"{TeamsRoute}/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task GetWithPlayers_ShouldReturnTeamWithPlayers_WhenPlayersExist()
    {
        // Arrange: Create a team
        var teamRequest = new CreateTeamRequest { Name = "Team With Players", Country = "Country A" };
        var postResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId);

        // Arrange: Add players to the created team
        var player1 = new CreatePlayerRequest
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Position = "Forward"
        };
        var player2 = new CreatePlayerRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1992, 5, 15),
            Position = "Midfielder"
        };

        var player1Response = await DoPostRequest($"{TeamsRoute}/{teamId}/players", player1);
        var player1Data = await player1Response.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        var player2Response = await DoPostRequest($"{TeamsRoute}/{teamId}/players", player2);
        var player2Data = await player2Response.Content.ReadFromJsonAsync<CreatePlayerResponse>();
        
        // Validate the POST responses for adding players
        player1Response.StatusCode.Should().Be(HttpStatusCode.Created);
        player1Data.Should().NotBeNull();
        player1Data!.FirstName.Should().Be("John");
        player1Data.LastName.Should().Be("Doe");
        player1Data.Position.Should().Be("Forward");

        player2Response.StatusCode.Should().Be(HttpStatusCode.Created);
        player2Data.Should().NotBeNull();
        player2Data!.FirstName.Should().Be("Jane");
        player2Data.LastName.Should().Be("Smith");
        player2Data.Position.Should().Be("Midfielder");

        // Act: Get team details with players
        var response = await DoGetRequest($"{TeamsRoute}/{teamId}/players");
        var responseData = await response.Content.ReadFromJsonAsync<GetTeamDetailsWithPlayers>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData!.Players.Should().NotBeNull().And.HaveCount(2);
        responseData.Players.Should().Contain(p => p.FirstName == "John" && p.LastName == "Doe");
        responseData.Players.Should().Contain(p => p.FirstName == "Jane" && p.LastName == "Smith");
    }

    #endregion

    #region GET All Tests

    [Test]
    public async Task GetAll_ShouldReturnAllTeams_WhenTeamsExist()
    {
        // Arrange
        var team1 = new CreateTeamRequest { Name = "Team E", Country = "Country E" };
        var team2 = new CreateTeamRequest { Name = "Team F", Country = "Country F" };
        var postResponse1 = await DoPostRequest(TeamsRoute, team1);
        var postResponse2 = await DoPostRequest(TeamsRoute, team2);
        _createdTeamIds.AddRange(new[]
        {
            (await postResponse1.Content.ReadFromJsonAsync<CreateTeamResponse>())!.Id,
            (await postResponse2.Content.ReadFromJsonAsync<CreateTeamResponse>())!.Id
        });

        // Act
        var response = await DoGetRequest(TeamsRoute);
        var responseData = await response.Content.ReadFromJsonAsync<List<GetTeamDetailsResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().Contain(t => t.Name == "Team E").And.Contain(t => t.Name == "Team F");
    }

    [Test]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoTeamsExist()
    {
        // Act
        var response = await DoGetRequest(TeamsRoute);
        var responseData = await response.Content.ReadFromJsonAsync<List<GetTeamDetailsResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().BeEmpty();
    }

    #endregion

    #region DELETE Tests

    [Test]
    public async Task DeleteById_ShouldReturnNoContent_WhenTeamExists()
    {
        // Arrange
        var request = new CreateTeamRequest { Name = "Team to Delete", Country = "Country X" };
        var postResponse = await DoPostRequest(TeamsRoute, request);
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId); // Track for cleanup

        // Act
        var response = await DoDeleteRequest($"{TeamsRoute}/{teamId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _createdTeamIds.Remove(teamId); // Already deleted
    }

    [Test]
    public async Task DeleteById_ShouldBeIdempotent_WhenTeamDoesNotExist()
    {
        // Act
        var response = await DoDeleteRequest($"{TeamsRoute}/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent); // Idempotency ensures no error
    }

    #endregion

    #region PATCH Tests

    [Test]
    public async Task Patch_ShouldUpdateTeam_WhenValidRequest()
    {
        // Arrange
        var teamRequest = new CreateTeamRequest { Name = "Team to Patch", Country = "Country H" };
        var postResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId); // Track for cleanup

        var patchRequest = new PatchTeamRequest { Name = "Updated Team" };

        // Act
        var response = await DoPatchRequest($"{TeamsRoute}/{teamId}", patchRequest);
        var responseData = await response.Content.ReadFromJsonAsync<GetTeamDetailsResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData!.Name.Should().Be("Updated Team");
    }

    [Test]
    public async Task Patch_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        var patchRequest = new PatchTeamRequest { Name = "Non-Existent Team" , Country = "Some country"};

        // Act
        var response = await DoPatchRequest($"{TeamsRoute}/99999", patchRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Patch_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var teamRequest = new CreateTeamRequest { Name = "Team Valid", Country = "Country Valid" };
        var postResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId); // Track for cleanup

        var invalidPatchRequest = new PatchTeamRequest
        {
            Name = "",
            Country = "Updated Country"
        };

        // Act
        var response = await DoPatchRequest($"{TeamsRoute}/{teamId}", invalidPatchRequest);
        var responseData = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseData.Should().Contain("The Name field cannot be null or empty if provided.");
    }

    [Test]
    public async Task Patch_ShouldReturnBadRequest_WhenCountryIsEmpty()
    {
        // Arrange
        var teamRequest = new CreateTeamRequest { Name = "Team Valid", Country = "Country Valid" };
        var postResponse = await DoPostRequest(TeamsRoute, teamRequest);
        var createdTeam = await postResponse.Content.ReadFromJsonAsync<CreateTeamResponse>();
        var teamId = createdTeam!.Id;
        _createdTeamIds.Add(teamId); // Track for cleanup

        var invalidPatchRequest = new PatchTeamRequest
        {
            Name = "Updated Name",
            Country = ""
        };

        // Act
        var response = await DoPatchRequest($"{TeamsRoute}/{teamId}", invalidPatchRequest);
        var responseData = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseData.Should().Contain("The Country field cannot be null or empty if provided.");
    }
    
    #endregion
}