using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceGetGamesTests : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;

    public GameServiceGetGamesTests(MainFixture fixture)
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
    
    //TODO: Test for no games in collection
}