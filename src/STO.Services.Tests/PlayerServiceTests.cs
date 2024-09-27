namespace STO.Services.Tests
{
    public class PlayerServiceTests : IClassFixture<TestDataFixture>
    {
        private readonly PlayerService _playerService;
        private readonly Mock<IDataService> _mockDataService;

        public PlayerServiceTests(TestDataFixture fixture)
        {
            // Mocking IDataService with data from Fixture
            _mockDataService = new Mock<IDataService>();
            _mockDataService.Setup(ds => ds.PlayerEntities).Returns(fixture.MockPlayerEntities);
            _mockDataService.Setup(ds => ds.RatingEntities).Returns(fixture.MockRatingEntities);
            _mockDataService.Setup(ds => ds.TransactionEntities).Returns(fixture.MockTransactionEntities);
            _mockDataService.Setup(ds => ds.GameEntities).Returns(fixture.MockGameEntities);
            _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(fixture.MockPlayerAtGameEntities);

            // Create PlayerService with mocked IDataService
            _playerService = new PlayerService(_mockDataService.Object);
        }

        [Fact]
        public void GetPlayer_ShouldReturnPlayerWithCorrectDetails()
        {
            // Arrange
            const string playerId = "1";

            // Act
            var result = _playerService.GetPlayer(playerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(playerId, result.Id);
            Assert.Equal("Ollie Watkins", result.Name);
            Assert.Empty(result.Tags);
            Assert.Equal(Enums.PlayerPosition.Forward, result.Position);
            Assert.Equal(3, result.DefaultRate);
            Assert.Equal(5, result.AdminRating);
            Assert.Equal("Ollie Watkins", result.Label);
            Assert.Equal(4.25, result.Rating); // Average of 4,5,3,5
            Assert.Equal("ollie-watkins", result.UrlSegment);
            Assert.Equal(3, result.Balance); // +3 -3, +3
            Assert.Equal(1, result.GamesCount);
        }
        
        [Fact]
        public void GetPlayer_ShouldThrowExceptionIfPlayerNotFound()
        {
            // Arrange
            const string nonExistentPlayerId = "999";

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _playerService.GetPlayer(nonExistentPlayerId));
        }
        
        [Fact]
        public void GetPlayer_ShouldReturnCorrectBalanceEvenWithNoTransactions()
        {
            // Arrange
            const string playerId = "1";
            _mockDataService.Setup(ds => ds.TransactionEntities)
                .Returns([]);  // Override fixture to have no transactions

            // Act
            var result = _playerService.GetPlayer(playerId);

            // Assert
            Assert.Equal(0, result.Balance);  // Balance should be 0 without transactions
        }
        
        [Fact]
        public void GetPlayer_ShouldReturnZeroRatingIfNoRatingsExist()
        {
            // Arrange
            const string playerId = "1";
            _mockDataService.Setup(ds => ds.RatingEntities)
                .Returns([]);  // Override fixture to have no ratings

            // Act
            var result = _playerService.GetPlayer(playerId);

            // Assert
            Assert.Equal(0, result.Rating);  // Rating should be 0 without ratings
        }
        
        [Fact]
        public void GetPlayers_ShouldReturnEmptyList_WhenNoPlayersExist()
        {
            // Arrange
            _mockDataService.Setup(ds => ds.PlayerEntities)
                .Returns([]); // No PLayers
        
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void GetPlayers_ShouldReturnPlayersList_WhenPlayersExist()
        {
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.Id == "1");
            Assert.Contains(result, p => p.Id == "2");
            Assert.Contains(result, p => p.Id == "3");
        }
        
        [Fact]
        public void GetPlayers_ShouldReturnPlayersListOrderedByName_WhenPlayersExist()
        {
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("3", result[0].Id); // Leon Bailey (ID 3), then Morgan Rogers (ID 2), then Ollie Watkins (ID 1)
            Assert.Equal("2", result[1].Id); 
            Assert.Equal("1", result[2].Id);
        }

        [Fact]
        public void GetPlayers_ShouldReturnFullPlayerObject_ForEachEntity()
        {
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.Equal("Ollie Watkins", result.Single(p => p.Id == "1").Name);
            Assert.Equal("Morgan Rogers", result.Single(p => p.Id == "2").Name);
            Assert.Equal("Leon Bailey", result.Single(p => p.Id == "3").Name);
        }
        
        [Fact]
        public void GetPlayers_WithGameId_ShouldReturnOnlyPLayersInGame()
        {
            // Act
            var result = _playerService.GetPlayers("G1");

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Id == "1");
            Assert.Contains(result, p => p.Id == "2");
            Assert.DoesNotContain(result, p => p.Id == "3");
        }
        
        [Fact]
        public void GetPlayers_WithGameId_ShouldReturnSortedPlayersList()
        {
            // Act
            var result = _playerService.GetPlayers("G1");

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(result.OrderBy(p => p.Name).ToList(), result);
        }

        [Fact]
        public void GetPlayers_WithGameId_ShouldReturnEmptyList_WhenNoEntitiesExist()
        {
            // Act
            var result = _playerService.GetPlayers("G2");

            // Assert
            Assert.Empty(result);
        }
    }
    
}
