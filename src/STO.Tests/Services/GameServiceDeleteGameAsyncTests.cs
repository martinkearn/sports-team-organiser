using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceDeleteGameAsyncTests : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;
    private readonly Mock<IDataService> _mockDataService;

    public GameServiceDeleteGameAsyncTests(MainFixture fixture)
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
    
}