using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class PlayerAtGameServiceTests : IClassFixture<MainFixture>
{
    private readonly Mock<IDataService> _mockDataService;
    private readonly PlayerAtGameService _service;
    private readonly PlayerService _playerService;
    private readonly GameService _gameService;
    private readonly TransactionService _transactionService;
    private readonly MainFixture _fixture;
    
    public PlayerAtGameServiceTests(MainFixture fixture)
    {
        _fixture = fixture;
        
        // Mocking IDataService with data from Fixture
        _mockDataService = new Mock<IDataService>();
        _mockDataService.Setup(ds => ds.PlayerEntities).Returns(_fixture.PlayerEntities);
        _mockDataService.Setup(ds => ds.RatingEntities).Returns(_fixture.RatingEntities);
        _mockDataService.Setup(ds => ds.TransactionEntities).Returns(_fixture.TransactionEntities);
        _mockDataService.Setup(ds => ds.GameEntities).Returns(_fixture.GameEntities);
        _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(_fixture.PlayerAtGameEntities);

        // Create GameService with mocked IDataService and real other services
        _playerService = new PlayerService(_mockDataService.Object);
        _gameService = new GameService(_mockDataService.Object);
        _transactionService = new TransactionService(_mockDataService.Object);
        _service = new PlayerAtGameService(_mockDataService.Object, _gameService, _playerService, _transactionService);
    }
    
    #region GetPag
    
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
    
    [Fact]
    public void GetPag_WhenNotExists_ThrowsException()
    {
        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => _service.GetPag("999"));
        Assert.Contains("The PlayerAtGame with Id 999 was not found.", ex.Message);
    }
    
    [Fact]
    public async Task GetPag_WhenPagHasInvalidPlayerId_ThrowsException()
    {
        // Arrange
        var pag = new PlayerAtGameEntity()
        {
            RowKey = "PAG5",
            GameRowKey = "G1",
            PlayerRowKey = "999",
            Forecast = Enums.PlayingStatus.Yes,
            Played = false,
            Team = "A"
        };
        var thisPlayerAtGameEntities = _fixture.PlayerAtGameEntities.ToList();
        thisPlayerAtGameEntities.Add(pag);
        var thisMockDataService = new Mock<IDataService>();
        thisMockDataService.Setup(ds => ds.PlayerEntities).Returns(_fixture.PlayerEntities);
        thisMockDataService.Setup(ds => ds.RatingEntities).Returns(_fixture.RatingEntities);
        thisMockDataService.Setup(ds => ds.TransactionEntities).Returns(_fixture.TransactionEntities);
        thisMockDataService.Setup(ds => ds.GameEntities).Returns(_fixture.GameEntities);
        thisMockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(thisPlayerAtGameEntities);
        var thisService = new PlayerAtGameService(thisMockDataService.Object, _gameService, _playerService, _transactionService);
        
        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => thisService.GetPag("PAG5"));
        Assert.Contains("The PlayerEntity with ID 999 was not found.", ex.Message);
    }
    
    [Fact]
    public void GetPag_WhenPagHasInvalidGameId_ThrowsException()
    {
        // Arrange
        var pag = new PlayerAtGameEntity()
        {
            RowKey = "PAG5",
            GameRowKey = "999",
            PlayerRowKey = "1",
            Forecast = Enums.PlayingStatus.Yes,
            Played = false,
            Team = "A"
        };
        var thisPlayerAtGameEntities = _fixture.PlayerAtGameEntities.ToList();
        thisPlayerAtGameEntities.Add(pag);
        var thisMockDataService = new Mock<IDataService>();
        thisMockDataService.Setup(ds => ds.PlayerEntities).Returns(_fixture.PlayerEntities);
        thisMockDataService.Setup(ds => ds.RatingEntities).Returns(_fixture.RatingEntities);
        thisMockDataService.Setup(ds => ds.TransactionEntities).Returns(_fixture.TransactionEntities);
        thisMockDataService.Setup(ds => ds.GameEntities).Returns(_fixture.GameEntities);
        thisMockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(thisPlayerAtGameEntities);
        var thisService = new PlayerAtGameService(thisMockDataService.Object, _gameService, _playerService, _transactionService);
        
        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => thisService.GetPag("PAG5"));
        Assert.Contains("The Game with Id 999 was not found.", ex.Message);
    }
    
    #endregion
    
    #region GetPagByUrlSegment
    
    [Fact]
    public void GetPagByUrlSegment_WhenExists_ReturnsPag()
    {
        // Act
        var result = _service.GetPagByUrlSegment("ollie-watkins-10-06-2024");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PAG1", result.Id);
    }
    
    [Fact]
    public void GetPagByUrlSegment_IsCaseInsensitive()
    {
        // Act
        var result = _service.GetPagByUrlSegment("oLlIe-WaTkInS-10-06-2024");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PAG1", result.Id);
    }
    
    [Fact]
    public void GetPagByUrlSegment_WhenNotExists_ThrowsException()
    {
        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => _service.GetPagByUrlSegment("unknown-segment"));
        Assert.Contains("The PlayerAtGame with UrlSegment unknown-segment was not found.", ex.Message);
    }
    
    [Fact]
    public void GetPagByUrlSegment_WhenUrlSegmentIsNullOrEmpty_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetPagByUrlSegment(null));
    }

    [Fact]
    public void GetPagByUrlSegment_HandlesWhitespace()
    {
        // Act
        var result = _service.GetPagByUrlSegment(" ollie-watkins-10-06-2024 ");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PAG1", result.Id);
    }
    
    #endregion
    
    #region GetPags

    [Fact]
    public void GetPags_ReturnsAllPags_WhenNoSkipOrTakeProvided()
    {
        // Act
        var result = _service.GetPags(null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void GetPags_ReturnsCorrectNumberOfPags_WhenTakeIsProvided()
    {
        // Act
        var result = _service.GetPags(null, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetPags_SkipsCorrectNumberOfPags_WhenSkipIsProvided()
    {
        // Arrange
        var allPags = _service.GetPags(null, null);
        var expectedPag = allPags[1]; // The second item

        // Act
        var result = _service.GetPags(1, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(allPags.Count - 1, result.Count);
        Assert.Equal(expectedPag.Id, result[0].Id);
    }

    [Fact]
    public void GetPags_SkipsAndTakesCorrectly()
    {
        // Act
        var result = _service.GetPags(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetPags_ReturnsEmptyList_WhenSkipExceedsListSize()
    {
        // Arrange
        var totalPags = _service.GetPags(null, null).Count;

        // Act
        var result = _service.GetPags(totalPags + 1, null);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetPags_ReturnsEmptyList_WhenTakeIsZero()
    {
        // Act
        var result = _service.GetPags(null, 0);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}