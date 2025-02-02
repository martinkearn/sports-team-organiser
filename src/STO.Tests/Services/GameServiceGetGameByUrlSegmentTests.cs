using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceGetGameByUrlSegmentTests : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;

    public GameServiceGetGameByUrlSegmentTests(MainFixture fixture)
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
}