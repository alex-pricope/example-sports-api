using AutoMapper;
using MyCompany.Domain.Entities;
using MyCompany.Domain.Models;
using MyCompany.Domain.Models.Actions;
using MyCompany.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MyCompany.Services.Tests;

[TestFixture]
public class PlayerManagementServiceTests
{
    private Mock<IPlayerRepository> _playerRepositoryMock;
    private Mock<ITeamRepository> _teamRepositoryMock;
    private Mock<ILogger<PlayerManagementService>> _loggerMock;
    private Mock<IMapper> _mapperMock;
    private PlayerManagementService _service;

    [SetUp]
    public void SetUp()
    {
        _playerRepositoryMock = new Mock<IPlayerRepository>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _loggerMock = new Mock<ILogger<PlayerManagementService>>();
        _mapperMock = new Mock<IMapper>();
        _service = new PlayerManagementService(
            _loggerMock.Object,
            _playerRepositoryMock.Object,
            _teamRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    #region AddNewPlayer Tests

    [Test]
    public async Task AddNewPlayer_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        // Arrange
        var teamId = 1;
        var newPlayer = new NewPlayer
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 1, 1),
        };

        _teamRepositoryMock.Setup(repo => repo.GetByIdAsync(teamId, false)).ReturnsAsync((TeamEntity?)null);

        // Act
        var result = await _service.AddNewPlayer(teamId, newPlayer);

        // Assert
        result.Should().BeNull();
        _playerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PlayerEntity>()), Times.Never);
    }

    [Test]
    public async Task AddNewPlayer_ShouldAddPlayer_WhenTeamExists()
    {
        // Arrange
        var teamId = 1;
        var teamEntity = new TeamEntity { Id = teamId, Name = "Team A" };
        var newPlayer = new NewPlayer
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 1, 1),
        };
        var playerEntity = new PlayerEntity
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 1, 1),
            TeamId = teamId
        };
        var addedPlayerEntity = new PlayerEntity { Id = 1, TeamId = teamId };
        var mappedPlayer = new Player
        {
            Id = 1,
            TeamId = teamId,
            FirstName = "JOhn",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 1, 1),
            Position = "Goalkeeper"
        };

        _teamRepositoryMock.Setup(repo => repo.GetByIdAsync(teamId, false)).ReturnsAsync(teamEntity);
        _mapperMock.Setup(m => m.Map<PlayerEntity>(newPlayer)).Returns(playerEntity);
        _playerRepositoryMock.Setup(repo => repo.AddAsync(playerEntity)).ReturnsAsync(addedPlayerEntity);
        _mapperMock.Setup(m => m.Map<Player>(addedPlayerEntity)).Returns(mappedPlayer);

        // Act
        var result = await _service.AddNewPlayer(teamId, newPlayer);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedPlayer);
        _playerRepositoryMock.Verify(x => x.AddAsync(playerEntity), Times.Once);
    }

    #endregion

    #region GetPlayerAsync Tests

    [Test]
    public async Task GetPlayerAsync_ShouldThrowArgumentOutOfRangeException_WhenPlayerIdIsInvalid()
    {
        // Act
        Func<Task> act = async () => await _service.GetPlayerAsync(0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("Player ID must be greater than 0.*");
    }

    [Test]
    public async Task GetPlayerAsync_ShouldReturnNull_WhenPlayerDoesNotExist()
    {
        // Arrange
        var playerId = 1;
        _playerRepositoryMock.Setup(repo => repo.GetByIdAsync(playerId)).ReturnsAsync((PlayerEntity?)null);

        // Act
        var result = await _service.GetPlayerAsync(playerId);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetPlayerAsync_ShouldReturnPlayer_WhenPlayerExists()
    {
        // Arrange
        const int playerId = 1;
        var playerEntity = new PlayerEntity { Id = playerId, FirstName = "John", LastName = "Doe" };
        var mappedPlayer = new Player
        {
            Id = playerId,
            FirstName = "John",
            LastName = "Doe",
            TeamId = 1,
            DateOfBirth = new DateOnly(2000, 1, 1),
        };

        _playerRepositoryMock.Setup(repo => repo.GetByIdAsync(playerId)).ReturnsAsync(playerEntity);
        _mapperMock.Setup(m => m.Map<Player>(playerEntity)).Returns(mappedPlayer);

        // Act
        var result = await _service.GetPlayerAsync(playerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedPlayer);
    }

    #endregion

    #region UpdatePlayerAsync Tests

    [Test]
    public async Task UpdatePlayerAsync_ShouldReturnNull_WhenPlayerDoesNotExist()
    {
        // Arrange
        var playerId = 1;
        var updatePlayer = new UpdatePlayer { FirstName = "Updated" };

        _playerRepositoryMock.Setup(repo => repo.GetByIdAsync(playerId)).ReturnsAsync((PlayerEntity?)null);

        // Act
        var result = await _service.UpdatePlayerAsync(playerId, updatePlayer);

        // Assert
        result.Should().BeNull();
        _playerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<PlayerEntity>()), Times.Never);
    }

    [Test]
    public async Task UpdatePlayerAsync_ShouldUpdatePlayer_WhenPlayerExists()
    {
        // Arrange
        var playerId = 1;
        var existingPlayerEntity = new PlayerEntity { Id = playerId, FirstName = "Old" };
        var updatePlayer = new UpdatePlayer { FirstName = "Updated" };
        var updatedPlayerEntity = new PlayerEntity { Id = playerId, FirstName = "Updated" };
        var mappedPlayer = new Player
        {
            Id = playerId,
            FirstName = "Updated",
            TeamId = 0,
            LastName = "",
            DateOfBirth = new DateOnly(2000, 1, 1),
        };

        _playerRepositoryMock.Setup(repo => repo.GetByIdAsync(playerId)).ReturnsAsync(existingPlayerEntity);
        _mapperMock.Setup(m => m.Map(updatePlayer, existingPlayerEntity));
        _playerRepositoryMock.Setup(repo => repo.UpdateAsync(existingPlayerEntity)).ReturnsAsync(updatedPlayerEntity);
        _mapperMock.Setup(m => m.Map<Player>(updatedPlayerEntity)).Returns(mappedPlayer);

        // Act
        var result = await _service.UpdatePlayerAsync(playerId, updatePlayer);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedPlayer);
    }

    #endregion

    #region DeletePlayerAsync Tests

    [Test]
    public async Task DeletePlayerAsync_ShouldDeletePlayer()
    {
        // Arrange
        var playerId = 1;

        // Act
        await _service.DeletePlayerAsync(playerId);

        // Assert
        _playerRepositoryMock.Verify(x => x.DeleteAsync(playerId), Times.Once);
    }

    #endregion
}