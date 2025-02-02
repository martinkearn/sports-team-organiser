using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceTests : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;
    private readonly Mock<IDataService> _mockDataService;

    public GameServiceTests(MainFixture fixture)
    {
        // Mocking IDataService with data from Fixture
        _mockDataService = new Mock<IDataService>();
        _mockDataService.Setup(ds => ds.PlayerEntities).Returns(fixture.PlayerEntities);
        _mockDataService.Setup(ds => ds.RatingEntities).Returns(fixture.RatingEntities);
        _mockDataService.Setup(ds => ds.TransactionEntities).Returns(fixture.TransactionEntities);
        _mockDataService.Setup(ds => ds.GameEntities).Returns(fixture.GameEntities);
        _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(fixture.PlayerAtGameEntities);

        // Create GameService with mocked IDataService
        _gameService = new GameService(_mockDataService.Object);
    }
    
    #region GetGames
    [Fact]
    public void GetGames_ShouldReturnAllGames_WhenNoSkipOrTakeProvided()
    {
        // Act
        var result = _gameService.GetGames(null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void GetGames_ShouldReturnLimitedGames_WhenTakeIsProvided()
    {
        // Act
        var result = _gameService.GetGames(null, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetGames_ShouldReturnSkippedGames_WhenSkipIsProvided()
    {
        // Act
        var result = _gameService.GetGames(1, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("G2", result[0].Id);
    }

    [Fact]
    public void GetGames_ShouldReturnSkippedAndLimitedGames_WhenSkipAndTakeAreProvided()
    {
        // Act
        var result = _gameService.GetGames(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("G2", result[0].Id);
    }
    #endregion
    
    #region GetGame
    [Fact]
    public void GetGame_ShouldReturnGame_WhenValidIdIsProvided()
    {
        // Act
        var result = _gameService.GetGame("G1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("G1", result.Id);
        Assert.Equal(new DateTimeOffset(2024, 6, 10, 18, 30, 0, TimeSpan.Zero).DateTime, result.DateTime);
        Assert.Equal("Test game G1", result.Notes);
        Assert.Equal(2, result.TeamAGoals);
        Assert.Equal(1, result.TeamBGoals);
        Assert.Equal("Foo", result.Title);
        Assert.Equal("10-06-2024", result.UrlSegment);
    }

    [Fact]
    public void GetGame_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        // Arrange
        var gameId = "notvalidgameid";
        
        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => _gameService.GetGame(gameId));
        Assert.Contains($"The Game with Id {gameId} was not found.", exception.Message);
    }
    #endregion
    
    #region GetGameByUrlSegment
    [Fact]
    public void GetGameByUrlSegment_ShouldReturnGame_WhenValidUrlSegmentIsProvided()
    {
        // Act
        var result = _gameService.GetGameByUrlSegment("10-06-2024");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("G1", result.Id);
    }

    [Fact]
    public void GetGameByUrlSegment_ShouldBeCaseInsensitive()
    {
        // Act
        var result = _gameService.GetGameByUrlSegment("10-05-2024");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("G2", result.Id);
    }

    [Fact]
    public void GetGameByUrlSegment_ShouldThrowKeyNotFoundException_WhenInvalidUrlSegmentIsProvided()
    {
        // Arrange
        var gameUrlSegment = "notvalidgameurlsegment";
            
        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => _gameService.GetGameByUrlSegment(gameUrlSegment));
        Assert.Contains($"The game with UrlSegment {gameUrlSegment} was not found.", exception.Message);
    }
    #endregion
    
    #region GetNextGame
    [Fact]
    public void GetNextGame_ShouldReturnLatestGame_WhenGamesExist()
    {
        // Act
        var result = _gameService.GetNextGame();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("G1", result.Id);
    }

    [Fact]
    public void GetNextGame_ShouldThrowKeyNotFoundException_WhenNoGamesExist()
    {
        // Arrange
        _mockDataService.Setup(ds => ds.GameEntities).Returns(new List<GameEntity>());
        var emptyGameService = new GameService(_mockDataService.Object);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => emptyGameService.GetNextGame());
        Assert.Equal("No next game found.", exception.Message);
    }
    #endregion
    
    #region DeleteGameAsync
    [Fact]
    public async Task DeleteGameAsync_ShouldDeleteGameAndRelatedEntities()
    {
        // Act
        await _gameService.DeleteGameAsync("G1");

        // Assert
        _mockDataService.Verify(ds => ds.DeleteEntityAsync<RatingEntity>("R1"), Times.Once);
        _mockDataService.Verify(ds => ds.DeleteEntityAsync<RatingEntity>("R2"), Times.Once);
        _mockDataService.Verify(ds => ds.DeleteEntityAsync<PlayerAtGameEntity>("PAG1"), Times.Once);
        _mockDataService.Verify(ds => ds.DeleteEntityAsync<PlayerAtGameEntity>("PAG2"), Times.Once);
        _mockDataService.Verify(ds => ds.DeleteEntityAsync<GameEntity>("G1"), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_ShouldNotFail_WhenGameHasNoRelatedEntities()
    {
        // Act
        await _gameService.DeleteGameAsync("G4");

        // Assert
        _mockDataService.Verify(ds => ds.DeleteEntityAsync<GameEntity>("G4"), Times.Once);
    }
    
    [Fact]
    public async Task DeleteGameAsync_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        // Arrange
        var gameId = "notvalidgameid";
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.DeleteGameAsync(gameId));
        Assert.Contains($"The Game with Id {gameId} was not found.", exception.Message);
    }
    #endregion
    
    #region UpdateGameAsync
    [Fact]
    public async Task UpsertGameAsync_ShouldCallUpsertEntityAsync_WithCorrectGameEntity()
    {
        // Arrange
        var dt = new DateTime(2024, 12, 25);
        var game = new Game { Id = "G5", DateTime = dt, TeamAGoals = 2, TeamBGoals = 3, Title = "Game 5", Notes = "Fifth Game", UrlSegment = "25-12-2024" };

        // Act
        await _gameService.UpsertGameAsync(game);

        // Assert
        _mockDataService.Verify(ds => ds.UpsertEntityAsync(It.Is<GameEntity>(g => g.RowKey == "G5")), Times.Once);
    }
    #endregion
}