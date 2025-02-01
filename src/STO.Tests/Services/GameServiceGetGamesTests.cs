using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceGetGamesTests : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;
    private readonly Mock<IDataService> _mockDataService;
    private readonly MainFixture _fixture;
    
    public GameServiceGetGamesTests(MainFixture fixture)
    {
        _fixture = fixture;
            
        // Mocking IDataService with data from Fixture
        _mockDataService = new Mock<IDataService>();
        _mockDataService.Setup(ds => ds.PlayerEntities).Returns(_fixture.PlayerEntities);
        _mockDataService.Setup(ds => ds.RatingEntities).Returns(_fixture.RatingEntities);
        _mockDataService.Setup(ds => ds.TransactionEntities).Returns(_fixture.TransactionEntities);
        _mockDataService.Setup(ds => ds.GameEntities).Returns(_fixture.GameEntities);
        _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(_fixture.PlayerAtGameEntities);

        // Create GameService with mocked IDataService
        _gameService = new GameService(_mockDataService.Object);
    }
    
    // Initial tests from GPT
    /*[Fact]
    public void GetGames_ShouldReturnAllGames_WhenNoSkipOrTakeProvided()
    {
        // Act
        var result = _gameService.GetGames(null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
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
        Assert.Equal(2, result.Count);
        Assert.Equal("Game 2", result[0].Title);
    }

    [Fact]
    public void GetGames_ShouldReturnSkippedAndLimitedGames_WhenSkipAndTakeAreProvided()
    {
        // Act
        var result = _gameService.GetGames(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Game 2", result[0].Title);
    }*/
}