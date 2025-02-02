using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceGetGameTests : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;

    public GameServiceGetGameTests(MainFixture fixture)
    {
        // Mocking IDataService with data from Fixture
        var mockDataService = new Mock<IDataService>();
        mockDataService.Setup(ds => ds.PlayerEntities).Returns(fixture.PlayerEntities);
        mockDataService.Setup(ds => ds.RatingEntities).Returns(fixture.RatingEntities);
        mockDataService.Setup(ds => ds.TransactionEntities).Returns(fixture.TransactionEntities);
        mockDataService.Setup(ds => ds.GameEntities).Returns(fixture.GameEntities);
        mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(fixture.PlayerAtGameEntities);

        // Create GameService with mocked IDataService
        _gameService = new GameService(mockDataService.Object);
    }
    
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
}