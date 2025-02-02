using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceGetNextGame : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;
    private readonly Mock<IDataService> _mockDataService;

    public GameServiceGetNextGame(MainFixture fixture)
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
}