using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class PlayerAtGameServiceTests : IClassFixture<MainFixture>
{
    private readonly PlayerAtGameService _service;
    
    public PlayerAtGameServiceTests(MainFixture fixture)
    {
        // Mocking IDataService with data from Fixture
        var mockDataService = new Mock<IDataService>();
        mockDataService.Setup(ds => ds.PlayerEntities).Returns(fixture.PlayerEntities);
        mockDataService.Setup(ds => ds.RatingEntities).Returns(fixture.RatingEntities);
        mockDataService.Setup(ds => ds.TransactionEntities).Returns(fixture.TransactionEntities);
        mockDataService.Setup(ds => ds.GameEntities).Returns(fixture.GameEntities);
        mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(fixture.PlayerAtGameEntities);

        // Create GameService with mocked IDataService and real other services
        var playerService = new PlayerService(mockDataService.Object);
        var gameService = new GameService(mockDataService.Object);
        var transactionService = new TransactionService(mockDataService.Object);
        _service = new PlayerAtGameService(mockDataService.Object, gameService, playerService, transactionService);
    }
    
    [Fact]
    public void GetPag_WhenExists_ReturnsPag()
    {
        // Act
        var result = _service.GetPag("PAG1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PAG1", result.Id);
        Assert.Equal("1", result.PlayerId);
        Assert.Equal("G1", result.GameId);
        Assert.Equal(Enums.PlayingStatus.Yes, result.Forecast);
        Assert.Equal("10 Jun Foo", result.GameLabel);
        Assert.Equal("10-06-2024", result.GameUrlSegment);
        Assert.Equal("Ollie Watkins at 10 Jun Foo", result.Label);
        Assert.True(result.Played);
        Assert.Equal(5, result.PlayerAdminRating);
        Assert.Equal(3, result.PlayerBalance);
        Assert.Equal(3, result.PlayerDefaultRate);
        Assert.Equal(1, result.PlayerGamesCount);
        Assert.Equal("Ollie Watkins", result.PlayerLabel);
        Assert.Equal("Ollie Watkins", result.PlayerName);
        Assert.Equal(Enums.PlayerPosition.Forward, result.PlayerPosition);
        Assert.Equal(4.25, result.PlayerRating);
        Assert.Equal("ollie-watkins", result.PlayerUrlSegment);
        Assert.Equal(Enums.Team.A, result.Team);
        Assert.Equal("ollie-watkins-10-06-2024", result.UrlSegment);
    }
}