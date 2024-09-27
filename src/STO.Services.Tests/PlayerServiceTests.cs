using NuGet.Frameworks;

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

            // Mocking IDataService properties
            _mockDataService.Setup(ds => ds.PlayerEntities).Returns(mockPlayerEntities);
            _mockDataService.Setup(ds => ds.RatingEntities).Returns(mockRatingEntities);
            _mockDataService.Setup(ds => ds.TransactionEntities).Returns(mockTransactionEntities);

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
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Id == "1");
            Assert.Contains(result, p => p.Id == "2");
        }
        
        [Fact]
        public void GetPlayers_ShouldReturnPlayersListOrderedByName_WhenPlayersExist()
        {
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("2", result[0].Id); // Morgan Rogers (ID 2) before Ollie Watkins (ID 1)
            Assert.Equal("1", result[1].Id); 
        }

        [Fact]
        public void GetPlayers_ShouldReturnFullPlayerObject_ForEachEntity()
        {
            // Act
            var result = _playerService.GetPlayers();

            // Assert
            Assert.Equal("Ollie Watkins", result.Single(p => p.Id == "1").Name);
            Assert.Equal("Morgan Rogers", result.Single(p => p.Id == "2").Name);
        }
    }
    
}
