namespace STO.Services.Tests
{
    public class PlayerServiceTests
    {
        private readonly Mock<IDataService> _mockDataService;
        private readonly PlayerService _playerService;

        public PlayerServiceTests()
        {
            _mockDataService = new Mock<IDataService>();
            List<PlayerEntity> mockPlayerEntities = [];
            List<RatingEntity> mockRatingEntities = [];
            List<TransactionEntity> mockTransactionEntities = [];
            List<PlayerAtGameEntity> mockPlayerAtGameEntities = [];
            List<GameEntity> mockGameEntities = [];

            // Initialize mock data for Ollie Watkins
            mockPlayerEntities.Add(
                new PlayerEntity
                {
                    RowKey = "1",
                    Name = "Ollie Watkins",
                    Tags = "",
                    Position = Enums.PlayerPosition.Forward,
                    DefaultRate = 3,
                    AdminRating = 5
                }
            );
            // Average 4.25
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 4 });
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 5 });
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 3 });
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 5 });
            // Total £3
            mockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "1", Amount = 3 });
            mockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "1", Amount = -3 });
            mockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "1", Amount = 3 });
            
            // Initialize mock data for Morgan Rogers
            mockPlayerEntities.Add(
                new PlayerEntity
                {
                    RowKey = "2",
                    Name = "Morgan Rogers",
                    Tags = "",
                    Position = Enums.PlayerPosition.BoxToBox,
                    DefaultRate = 3,
                    AdminRating = 4
                }
            );
            // Average 3.5
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 4 });
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 4 });
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 3 });
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 3 });
            // Total £0
            mockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "2", Amount = 3 });
            mockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "2", Amount = -3 });
            
            // Initialize mock data for Leon Baily
            mockPlayerEntities.Add(
                new PlayerEntity
                {
                    RowKey = "3",
                    Name = "Leon Bailey",
                    Tags = "",
                    Position = Enums.PlayerPosition.Forward,
                    DefaultRate = 3,
                    AdminRating = 4
                }
            );
            // Average 3.5
            mockRatingEntities.Add(new RatingEntity { PlayerRowKey = "3", Rating = 5 });
            // Total £0
            mockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "3", Amount = 3 });
            
            // Initialise mock Game data
            mockGameEntities.Add(new GameEntity()
            {
                RowKey = "G1",
                Date = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero),
                Notes = "Test game",
                TeamAGoals = 2,
                TeamBGoals = 1,
                Title = "Foo",
                UrlSegment = "01-01-2024"
            });
            
            // Initialise mock PLayerAtGame data
            mockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
            {
                GameRowKey = "G1",
                PlayerRowKey = "1",
                Forecast = Enums.PlayingStatus.Yes,
                Team = "A",
                Played = true,
                UrlSegment = "ollie-watkins-01-01-2024"
            });
            mockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
            {
                GameRowKey = "G1",
                PlayerRowKey = "2",
                Forecast = Enums.PlayingStatus.Yes,
                Team = "B",
                Played = false,
                UrlSegment = "morgan-rogers-01-01-2024"
            });
            // Leon Bailey not in Game

            // Mocking IDataService properties
            _mockDataService.Setup(ds => ds.PlayerEntities).Returns(mockPlayerEntities);
            _mockDataService.Setup(ds => ds.RatingEntities).Returns(mockRatingEntities);
            _mockDataService.Setup(ds => ds.TransactionEntities).Returns(mockTransactionEntities);
            _mockDataService.Setup(ds => ds.GameEntities).Returns(mockGameEntities);
            _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(mockPlayerAtGameEntities);

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
                .Returns([]);  // No transactions

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
                .Returns([]);  // No ratings

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
