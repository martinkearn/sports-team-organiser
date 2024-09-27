namespace STO.Services.Tests
{
    public class PlayerServiceTests
    {
        private readonly Mock<IDataService> _mockDataService;
        private readonly PlayerService _playerService;

        public PlayerServiceTests()
        {
            _mockDataService = new Mock<IDataService>();

            // Initialize mock data
            List<PlayerEntity> mockPlayerEntities = [
                new PlayerEntity
                {
                    RowKey = "1",
                    Name = "Ollie Watkins",
                    Tags = "",
                    Position = Enums.PlayerPosition.Forward,
                    DefaultRate = 3,
                    AdminRating = 5
                }
            ];

            List<RatingEntity> mockRatingEntities = [
                new RatingEntity { PlayerRowKey = "1", Rating = 4 },
                new RatingEntity { PlayerRowKey = "1", Rating = 5 },
                new RatingEntity { PlayerRowKey = "1", Rating = 3 },
                new RatingEntity { PlayerRowKey = "1", Rating = 5 },
            ];

            List<TransactionEntity> mockTransactionEntities = [
                new TransactionEntity { PlayerRowKey = "1", Amount = 3 },
                new TransactionEntity { PlayerRowKey = "1", Amount = -3 },
                new TransactionEntity { PlayerRowKey = "1", Amount = 3 },
            ];

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
            var nonExistentPlayerId = "999";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _playerService.GetPlayer(nonExistentPlayerId));
        }
        
        [Fact]
        public void GetPlayer_ShouldReturnCorrectBalanceEvenWithNoTransactions()
        {
            // Arrange
            var playerId = "1";
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
            var playerId = "1";
            _mockDataService.Setup(ds => ds.RatingEntities)
                .Returns([]);  // No ratings

            // Act
            var result = _playerService.GetPlayer(playerId);

            // Assert
            Assert.Equal(0, result.Rating);  // Rating should be 0 without ratings
        }
    }
    
}
