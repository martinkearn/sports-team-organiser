namespace STO.Services.Tests
{
    public class PlayerServiceTests : IClassFixture<PlayerServiceFixture>
    {
        private readonly PlayerService _playerService;
        private readonly Mock<IDataService> _mockDataService;
        private readonly PlayerServiceFixture _fixture;

        public PlayerServiceTests(PlayerServiceFixture fixture)
        {
            _fixture = fixture;
            
            // Mocking IDataService with data from Fixture
            _mockDataService = new Mock<IDataService>();
            _mockDataService.Setup(ds => ds.PlayerEntities).Returns(_fixture.MockPlayerEntities);
            _mockDataService.Setup(ds => ds.RatingEntities).Returns(_fixture.MockRatingEntities);
            _mockDataService.Setup(ds => ds.TransactionEntities).Returns(_fixture.MockTransactionEntities);
            _mockDataService.Setup(ds => ds.GameEntities).Returns(_fixture.MockGameEntities);
            _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(_fixture.MockPlayerAtGameEntities);

            // Create PlayerService with mocked IDataService
            _playerService = new PlayerService(_mockDataService.Object);
        }
        
        #region GetPlayer

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
            // Player 999 does not exist in the fixture
            const string nonExistentPlayerId = "999";

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _playerService.GetPlayer(nonExistentPlayerId));
        }
        
        [Fact]
        public void GetPlayer_ShouldReturnCorrectBalanceEvenWithNoTransactions()
        {
            // Arrange
            const string playerId = "1";
            _mockDataService.Setup(ds => ds.TransactionEntities).Returns([]);  // Override fixture to have no transactions

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
            _mockDataService.Setup(ds => ds.RatingEntities).Returns([]);  // Override fixture to have no ratings

            // Act
            var result = _playerService.GetPlayer(playerId);

            // Assert
            Assert.Equal(0, result.Rating);  // Rating should be 0 without ratings
        }
        
        #endregion
        
        #region GetPlayers
        
        [Fact]
        public void GetPlayers_ShouldReturnEmptyList_WhenNoPlayersExist()
        {
            // Arrange
            _mockDataService.Setup(ds => ds.PlayerEntities).Returns([]); // Override fixture to have no PLayers
        
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
            Assert.Equal(4, result.Count);
            Assert.Contains(result, p => p.Id == "1");
            Assert.Contains(result, p => p.Id == "2");
            Assert.Contains(result, p => p.Id == "3");
            Assert.Contains(result, p => p.Id == "4");
        }
        
        [Fact]
        public void GetPlayers_ShouldReturnPlayersListOrderedByName_WhenPlayersExist()
        {
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("4", result[0].Id); // Jacob Ramsey (ID 4)
            Assert.Equal("3", result[1].Id); // Leon Bailey (ID 3)
            Assert.Equal("2", result[2].Id); // Morgan Rogers (ID 2)
            Assert.Equal("1", result[3].Id); // Ollie Watkins (ID 1)
        }

        [Fact]
        public void GetPlayers_ShouldReturnFullPlayerObject_ForEachEntity()
        {
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("Ollie Watkins", result.Single(p => p.Id == "1").Name);
            Assert.Equal("Morgan Rogers", result.Single(p => p.Id == "2").Name);
            Assert.Equal("Leon Bailey", result.Single(p => p.Id == "3").Name);
            Assert.Equal("Jacob Ramsey", result.Single(p => p.Id == "4").Name);
        }
        
        #endregion
        
        #region GetPlayers_WithGameId
        
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
            // Game G999 does not exist in fixture
            var result = _playerService.GetPlayers("G999");

            // Assert
            Assert.Empty(result);
        }
        
        #endregion
        
        #region GetPlayer_WithDate
        
        [Fact]
        public void GetPlayers_WithDate_ShouldReturnPlayersWithinDateRange()
        {
            // Arrange
            // Set range to include games G1 (10-06-2024) and G2 (10-05-2024) in the fixture
            // This will include Ollie Watkins & Morgans Rogers (G1) and Leon bailey (G2)
            var dateRangeStart = new DateTime(2024, 5, 1);
            var dateRangeEnd = new DateTime(2024, 6, 20);

            // Act
            var result = _playerService.GetPlayers(dateRangeStart, dateRangeEnd);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.Id == "1");
            Assert.Contains(result, p => p.Id == "2");
            Assert.Contains(result, p => p.Id == "3");
            Assert.DoesNotContain(result, p => p.Id == "4");
        }
        
        #endregion
        
        #region DeletePlayerAsync
        
        [Fact]
        public async Task DeletePlayerAsync_ShouldDeletePlayerRatings()
        {
            // Act
            await _playerService.DeletePlayerAsync("1");

            // Assert
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<RatingEntity>("R1"), Times.Once);
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<RatingEntity>("R2"), Times.Once);
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<RatingEntity>("R3"), Times.Once);
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<RatingEntity>("R4"), Times.Once);
        }
        
        [Fact]
        public async Task DeletePlayerAsync_ShouldDeletePlayerTransactions()
        {
            // Act
            await _playerService.DeletePlayerAsync("1");

            // Assert
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<TransactionEntity>("T1"), Times.Once);
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<TransactionEntity>("T2"), Times.Once);
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<TransactionEntity>("T3"), Times.Once);
        }
        
        [Fact]
        public async Task DeletePlayerAsync_ShouldDeletePlayerAtGameEntities()
        {
            // Act
            await _playerService.DeletePlayerAsync("1");

            // Assert
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<PlayerAtGameEntity>("PAG1"), Times.Once);
        } 
        
        [Fact]
        public async Task DeletePlayerAsync_ShouldDeletePlayerEntity()
        {
            // Arrange
            var playerId = "1";

            // Act
            await _playerService.DeletePlayerAsync(playerId);

            // Assert
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<PlayerEntity>(playerId), Times.Once);
        }
        
        [Fact]
        public async Task DeletePlayerAsync_ShouldHandleNoEntitiesToDelete()
        {
            // Arrange
            const string playerId = "1";

            // Mock all entity collections to be empty
            _mockDataService.Setup(ds => ds.RatingEntities).Returns([]);
            _mockDataService.Setup(ds => ds.TransactionEntities).Returns([]);
            _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns([]);

            // Act
            await _playerService.DeletePlayerAsync(playerId);

            // Assert
            // Verify that no DeleteEntityAsync calls are made for non-existent entities
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<RatingEntity>(It.IsAny<string>()), Times.Never);
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<TransactionEntity>(It.IsAny<string>()), Times.Never);
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<PlayerAtGameEntity>(It.IsAny<string>()), Times.Never);

            // Verify that DeleteEntityAsync was still called for PlayerEntity
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<PlayerEntity>(playerId), Times.Once);
        }

        [Fact]
        public async Task DeletePlayerAsync_ShouldThrowExceptionIfPlayerNotFound()
        {
            // Arrange
            const string nonExistentPlayerId = "999";

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _playerService.DeletePlayerAsync(nonExistentPlayerId));
        }


        #endregion
        
        #region UpsertPlayerAsync
        
        [Fact]
        public async Task UpsertPlayerAsync_ShouldCallUpsertEntityAsyncWithCorrectEntity()
        {
            // Arrange
            var expectedPlayer = _fixture.PlayerWollyWatkins;

            // Act
            await _playerService.UpsertPlayerAsync(expectedPlayer);

            // Assert
            // Verify that UpsertEntityAsync was called with the correct transformed PlayerEntity
            _mockDataService.Verify(ds => ds.UpsertEntityAsync(It.Is<PlayerEntity>(pe => pe.RowKey == "1" && pe.Name == "Wolly Watkins")), Times.Once);
        }
        
        #endregion

    }
    
}
