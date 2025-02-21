using STO.Tests.Fixtures;
using Moq;

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

    private PlayerAtGameService CreateIsolatedService(List<PlayerAtGameEntity>? pags)
    {
        var isolatedFixture = new MainFixture();
        var isolatedMockDataService = new Mock<IDataService>();
        isolatedMockDataService.Setup(ds => ds.PlayerEntities).Returns(isolatedFixture.PlayerEntities);
        isolatedMockDataService.Setup(ds => ds.RatingEntities).Returns(isolatedFixture.RatingEntities);
        isolatedMockDataService.Setup(ds => ds.TransactionEntities).Returns(isolatedFixture.TransactionEntities);
        isolatedMockDataService.Setup(ds => ds.GameEntities).Returns(isolatedFixture.GameEntities);
        if (pags != null)
        {
            isolatedMockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(pags);
        }
        else
        {
            isolatedMockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(isolatedFixture.PlayerAtGameEntities);
        }
        var isolatedPlayerService = new PlayerService(isolatedMockDataService.Object);
        var isolatedGameService = new GameService(isolatedMockDataService.Object);
        var isolatedTransactionService = new TransactionService(isolatedMockDataService.Object);
        var isolatedService = new PlayerAtGameService(isolatedMockDataService.Object, isolatedGameService, isolatedPlayerService, isolatedTransactionService);
        return isolatedService;
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
        var isolatedService = CreateIsolatedService(thisPlayerAtGameEntities);
        
        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => isolatedService.GetPag("PAG5"));
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
    
    #region GetPagsForGame

    [Fact]
    public void GetPagsForGame_WhenGameExists_ReturnsPags()
    {
        // Arrange
        string gameId = "G1";

        // Act
        var result = _service.GetPagsForGame(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, pag => Assert.Equal(gameId, pag.GameId));
    }

    [Fact]
    public void GetPagsForGame_WhenGameHasNoPags_ReturnsEmptyList()
    {
        // Arrange
        string gameId = "G999"; // Non-existent game

        // Act
        var result = _service.GetPagsForGame(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetPagsForGame_WhenGameIdIsNullOrEmpty_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetPagsForGame(null));
        Assert.Throws<ArgumentNullException>(() => _service.GetPagsForGame(string.Empty));
    }

    [Fact]
    public void GetPagsForGame_ReturnsCorrectNumberOfPags()
    {
        // Arrange
        string gameId = "G1";
        var expectedCount = _fixture.PlayerAtGameEntities.Count(p => p.GameRowKey == gameId);

        // Act
        var result = _service.GetPagsForGame(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCount, result.Count);
    }

    #endregion
    
    #region OrganiseTeams
    
        [Fact]
    public async Task OrganiseTeams_DistributesTeamsEvenlyBasedOnAdminRating()
    {
        // Arrange
        var pags = new List<PlayerAtGame>
        {
            new PlayerAtGame { Id = "1", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 5, PlayerPosition = Enums.PlayerPosition.Forward },
            new PlayerAtGame { Id = "2", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 5, PlayerPosition = Enums.PlayerPosition.Forward },
            new PlayerAtGame { Id = "3", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 1, PlayerPosition = Enums.PlayerPosition.Forward },
            new PlayerAtGame { Id = "4", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 1, PlayerPosition = Enums.PlayerPosition.Forward },
            new PlayerAtGame { Id = "5", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 5, PlayerPosition = Enums.PlayerPosition.Defender },
            new PlayerAtGame { Id = "6", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 5, PlayerPosition = Enums.PlayerPosition.Defender },
            new PlayerAtGame { Id = "7", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 1, PlayerPosition = Enums.PlayerPosition.Defender },
            new PlayerAtGame { Id = "8", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 1, PlayerPosition = Enums.PlayerPosition.Defender },
        };

        // Act
        var result = await _service.OrganiseTeams(pags);

        // Assert
        var teamARating = result.Where(p => p.Team == Enums.Team.A).Sum(p => p.PlayerAdminRating);
        var teamBRating = result.Where(p => p.Team == Enums.Team.B).Sum(p => p.PlayerAdminRating);
        Assert.Equal(12, teamARating);
        Assert.Equal(12, teamBRating);
    }

    [Fact]
    public async Task OrganiseTeams_IgnoresPlayersNotMarkedAsYes()
    {
        // Arrange
        var pags = new List<PlayerAtGame>
        {
            new PlayerAtGame { Id = "1", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 8, PlayerPosition = Enums.PlayerPosition.Forward },
            new PlayerAtGame { Id = "2", Forecast = Enums.PlayingStatus.No, PlayerAdminRating = 5, PlayerPosition = Enums.PlayerPosition.Forward }, // Should be ignored
            new PlayerAtGame { Id = "3", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 7, PlayerPosition = Enums.PlayerPosition.Defender },
        };

        // Act
        var result = await _service.OrganiseTeams(pags);

        // Assert
        Assert.Equal(2, result.Count); // Only two players should be assigned
        Assert.All(result, pag => Assert.Equal(Enums.PlayingStatus.Yes, pag.Forecast));
    }

    [Fact]
    public async Task OrganiseTeams_EnsuresPositionBalance()
    {
        // Arrange
        var pags = new List<PlayerAtGame>
        {
            new PlayerAtGame { Id = "1", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 9, PlayerPosition = Enums.PlayerPosition.Midfielder },
            new PlayerAtGame { Id = "2", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 6, PlayerPosition = Enums.PlayerPosition.Midfielder },
            new PlayerAtGame { Id = "3", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 7, PlayerPosition = Enums.PlayerPosition.Defender },
            new PlayerAtGame { Id = "4", Forecast = Enums.PlayingStatus.Yes, PlayerAdminRating = 5, PlayerPosition = Enums.PlayerPosition.Defender },
        };

        // Act
        var result = await _service.OrganiseTeams(pags);

        // Assert
        var teamA = result.Where(p => p.Team == Enums.Team.A).ToList();
        var teamB = result.Where(p => p.Team == Enums.Team.B).ToList();

        Assert.Equal(2, teamA.Count);
        Assert.Equal(2, teamB.Count);

        Assert.Equal(1, teamA.Count(p => p.PlayerPosition == Enums.PlayerPosition.Midfielder));
        Assert.Equal(1, teamA.Count(p => p.PlayerPosition == Enums.PlayerPosition.Defender));
        Assert.Equal(1, teamB.Count(p => p.PlayerPosition == Enums.PlayerPosition.Midfielder));
        Assert.Equal(1, teamB.Count(p => p.PlayerPosition == Enums.PlayerPosition.Defender));
    }

    [Fact]
    public async Task OrganiseTeams_UpsertsPagForEachPlayer()
    {
        // Arrange
        var thisService = new PlayerAtGameService(_mockDataService.Object, _gameService, _playerService, _transactionService);
        var playersAtGame = thisService.GetPags(null, null);
        
        // Act
        var result = await thisService.OrganiseTeams(playersAtGame);
        
        // Assert
        var teamNone = result.Where(p => p.Team == Enums.Team.None).ToList();
        
        Assert.Empty(teamNone); // Started with two Pags with Team set to None
    }

    [Fact]
    public async Task OrganiseTeams_ReturnsEmptyList_WhenNoPlayersAreAvailable()
    {
        // Arrange
        var pags = new List<PlayerAtGame>(); // No players

        // Act
        var result = await _service.OrganiseTeams(pags);

        // Assert
        Assert.Empty(result);
    }
    
    #endregion
    
    #region ResetTeams
    
    [Fact]
    public async Task ResetTeamsAsync_SetsAllPlayersToNone()
    {
        // Arrange
        var pags = new List<PlayerAtGame>
        {
            new PlayerAtGame { Id = "1", Team = Enums.Team.A },
            new PlayerAtGame { Id = "2", Team = Enums.Team.B },
            new PlayerAtGame { Id = "3", Team = Enums.Team.A }
        };

        // Act
        await _service.ResetTeamsAsync(pags);

        // Assert
        Assert.All(pags, pag => Assert.Equal(Enums.Team.None, pag.Team));
    }

    [Fact]
    public async Task ResetTeamsAsync_DoesNotFailOnEmptyList()
    {
        // Arrange
        var pags = new List<PlayerAtGame>();

        // Act
        await _service.ResetTeamsAsync(pags);

        // Assert
        Assert.Empty(pags);
    }
    
    #endregion
    
    #region TogglepagPlayedAsync
    
    
    [Fact]
    public async Task TogglePagPlayedAsync_TogglesPlayedStatus()
    {
        // Arrange
        var isolatedService = CreateIsolatedService(null);
        
        // Act
        await isolatedService.TogglePagPlayedAsync("PAG1");
        var updatedPag = isolatedService.GetPag("PAG1");

        // Assert
        Assert.False(updatedPag.Played); // Should now be false
    }
    
    //ToDo: Tests for transactions when Played is toggled. Need to look at whether CreateIsolatedService keeps returning the initial TransactionEntities list.
    
    [Fact]
    public async Task TogglePagPlayedAsync_ThrowsKeyNotFoundException_WhenPagDoesNotExist()
    {
        // Arrange
        var isolatedService = CreateIsolatedService(null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => isolatedService.TogglePagPlayedAsync("InvalidId"));
    }
    
    #endregion
}