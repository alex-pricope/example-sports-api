using AutoMapper;
using MyCompany.Domain.Entities;
using MyCompany.Domain.Exceptions;
using MyCompany.Domain.Models;
using MyCompany.Domain.Models.Actions;
using MyCompany.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MyCompany.Services.Tests;

[TestFixture]
public class TeamManagementServiceTests
{
    private Mock<ITeamRepository> _teamRepositoryMock;
    private Mock<ILogger<TeamManagementService>> _loggerMock;
    private Mock<IMapper> _mapperMock;
    private TeamManagementService _service;

    [SetUp]
    public void SetUp()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _loggerMock = new Mock<ILogger<TeamManagementService>>();
        _mapperMock = new Mock<IMapper>();
        _service = new TeamManagementService(
            _loggerMock.Object,
            _teamRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    #region CreateTeamAsync Tests

    [Test]
    public async Task CreateTeamAsync_ShouldThrow_WhenTeamAlreadyExists()
    {
        // Arrange
        var newTeam = new NewTeam { Name = "Team A", Country = "Country A" };
        var existingTeamEntity = new TeamEntity { Id = 1, Name = "Team A" };

        _teamRepositoryMock.Setup(repo => repo.GetByNameAsync(newTeam.Name)).ReturnsAsync(existingTeamEntity);

        // Act
        Func<Task> act = async () => await _service.CreateTeamAsync(newTeam);

        // Assert
        await act.Should().ThrowAsync<EntityAlreadyExistsException>()
            .WithMessage("A team with the same name already exists.");
        _teamRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TeamEntity>()), Times.Never);
    }

    [Test]
    public async Task CreateTeamAsync_ShouldAddTeam_WhenTeamDoesNotExist()
    {
        // Arrange
        var newTeam = new NewTeam { Name = "Team A", Country = "Country A" };
        var teamEntity = new TeamEntity { Name = "Team A", Country = "Country A" };
        var addedTeamEntity = new TeamEntity { Id = 1, Name = "Team A", Country = "Country A" };
        var mappedTeam = new Team { Id = 1, Name = "Team A", Country = "Country A" };

        _teamRepositoryMock.Setup(repo => repo.GetByNameAsync(newTeam.Name)).ReturnsAsync((TeamEntity?)null);
        _mapperMock.Setup(m => m.Map<TeamEntity>(newTeam)).Returns(teamEntity);
        _teamRepositoryMock.Setup(repo => repo.AddAsync(teamEntity)).ReturnsAsync(addedTeamEntity);
        _mapperMock.Setup(m => m.Map<Team>(addedTeamEntity)).Returns(mappedTeam);

        // Act
        var result = await _service.CreateTeamAsync(newTeam);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedTeam);
        _teamRepositoryMock.Verify(x => x.AddAsync(teamEntity), Times.Once);
    }

    #endregion

    #region GetTeamDetailsByIdAsync Tests

    [Test]
    public async Task GetTeamDetailsByIdAsync_ShouldThrow_WhenTeamIdIsInvalid()
    {
        // Act
        Func<Task> act = async () => await _service.GetTeamDetailsByIdAsync(0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("Team ID must be greater than 0.*");
    }

    [Test]
    public async Task GetTeamDetailsByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        // Arrange
        var teamId = 1;
        _teamRepositoryMock.Setup(repo => repo.GetByIdAsync(teamId, false)).ReturnsAsync((TeamEntity?)null);

        // Act
        var result = await _service.GetTeamDetailsByIdAsync(teamId);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetTeamDetailsByIdAsync_ShouldReturnTeam_WhenTeamExists()
    {
        // Arrange
        var teamId = 1;
        var teamEntity = new TeamEntity { Id = teamId, Name = "Team A" };
        var mappedTeam = new Team
        {
            Id = teamId,
            Name = "Team A",
            Country = "Some country"
        };

        _teamRepositoryMock.Setup(repo => repo.GetByIdAsync(teamId, false)).ReturnsAsync(teamEntity);
        _mapperMock.Setup(m => m.Map<Team>(teamEntity)).Returns(mappedTeam);

        // Act
        var result = await _service.GetTeamDetailsByIdAsync(teamId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedTeam);
    }

    #endregion

    #region GetAllTeamsDetailsAsync Tests

    [Test]
    public async Task GetAllTeamsDetailsAsync_ShouldReturnNull_WhenNoTeamsExist()
    {
        // Arrange
        _teamRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<TeamEntity>());

        // Act
        var result = await _service.GetAllTeamsDetailsAsync();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllTeamsDetailsAsync_ShouldReturnTeams_WhenTeamsExist()
    {
        // Arrange
        var teamEntities = new List<TeamEntity>
        {
            new TeamEntity { Id = 1, Name = "Team A" },
            new TeamEntity { Id = 2, Name = "Team B" }
        };
        var mappedTeams = new List<Team>
        {
            new Team
            {
                Id = 1,
                Name = "Team A",
                Country = "Some country A"
            },
            new Team
            {
                Id = 2,
                Name = "Team B",
                Country = "Some country B"
            }
        };

        _teamRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(teamEntities);
        _mapperMock.Setup(m => m.Map<List<Team>>(teamEntities)).Returns(mappedTeams);

        // Act
        var result = await _service.GetAllTeamsDetailsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedTeams);
    }
    
    [Test]
    public async Task GetTeamDetailsByIdAsync_ShouldReturnTeamWithPlayers_WhenIncludePlayersIsTrue()
    {
        // Arrange
        var teamId = 1;
        var teamEntity = new TeamEntity
        {
            Id = teamId,
            Name = "Team A",
            Country = "Country A",
            Players = new List<PlayerEntity>
            {
                new PlayerEntity { Id = 1, FirstName = "John", LastName = "Doe" },
                new PlayerEntity { Id = 2, FirstName = "Jane", LastName = "Smith" }
            }
        };
        var mappedTeam = new Team
        {
            Id = teamId,
            Name = "Team A",
            Country = "Country A",
            Players = new List<Player>
            {
                new Player
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    TeamId = teamId,
                    DateOfBirth = new DateOnly(2000, 1, 1),
                },
                new Player
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    TeamId = teamId,
                    DateOfBirth = new DateOnly(2000, 1, 1),
                }
            }
        };

        _teamRepositoryMock.Setup(repo => repo.GetByIdAsync(teamId, true)).ReturnsAsync(teamEntity);
        _mapperMock.Setup(m => m.Map<Team>(teamEntity)).Returns(mappedTeam);

        // Act
        var result = await _service.GetTeamDetailsByIdAsync(teamId, true);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedTeam);
        result.Players.Should().HaveCount(2); // Ensure players are included
        result.Players.Should().ContainSingle(p => p.FirstName == "John" && p.LastName == "Doe");
        result.Players.Should().ContainSingle(p => p.FirstName == "Jane" && p.LastName == "Smith");
    }

    #endregion

    #region UpdateTeamAsync Tests

    [Test]
    public async Task UpdateTeamAsync_ShouldThrow_WhenTeamIdIsInvalid()
    {
        // Act
        Func<Task> act = async () => await _service.UpdateTeamAsync(0, new UpdateTeam());

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("Team ID must be greater than 0.*");
    }

    [Test]
    public async Task UpdateTeamAsync_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        // Arrange
        var teamId = 1;
        var updateTeam = new UpdateTeam { Name = "Updated Team A" };

        _teamRepositoryMock.Setup(repo => repo.GetByIdAsync(teamId, false)).ReturnsAsync((TeamEntity?)null);

        // Act
        var result = await _service.UpdateTeamAsync(teamId, updateTeam);

        // Assert
        result.Should().BeNull();
        _teamRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TeamEntity>()), Times.Never);
    }

    [Test]
    public async Task UpdateTeamAsync_ShouldUpdateTeam_WhenTeamExists()
    {
        // Arrange
        var teamId = 1;
        var existingTeamEntity = new TeamEntity { Id = teamId, Name = "Old Team A" };
        var updateTeam = new UpdateTeam { Name = "Updated Team A" };
        var updatedTeamEntity = new TeamEntity { Id = teamId, Name = "Updated Team A" };
        var mappedTeam = new Team
        {
            Id = teamId,
            Name = "Updated Team A",
            Country = "Some country"
        };

        _teamRepositoryMock.Setup(repo => repo.GetByIdAsync(teamId, false)).ReturnsAsync(existingTeamEntity);
        _mapperMock.Setup(m => m.Map(updateTeam, existingTeamEntity));
        _teamRepositoryMock.Setup(repo => repo.UpdateAsync(existingTeamEntity)).ReturnsAsync(updatedTeamEntity);
        _mapperMock.Setup(m => m.Map<Team>(updatedTeamEntity)).Returns(mappedTeam);

        // Act
        var result = await _service.UpdateTeamAsync(teamId, updateTeam);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedTeam);
    }

    #endregion
    
    [Test]
    public async Task DeleteTeamAsync_ShouldDeleteTeam()
    {
        // Arrange
        var teamId = 1;

        // Act
        await _service.DeleteTeamAsync(teamId);

        // Assert
        _teamRepositoryMock.Verify(x => x.DeleteAsync(teamId), Times.Once);
    }
}