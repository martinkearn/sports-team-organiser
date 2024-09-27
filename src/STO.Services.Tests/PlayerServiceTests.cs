namespace STO.Services.Tests
{
    public class PlayerServiceTests
    {
        private readonly PlayerService _playerService;

        public PlayerServiceTests()
        {
            var mockDataService = new Mock<IDataService>();

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
            mockDataService.Setup(ds => ds.PlayerEntities).Returns(mockPlayerEntities);
            mockDataService.Setup(ds => ds.RatingEntities).Returns(mockRatingEntities);
            mockDataService.Setup(ds => ds.TransactionEntities).Returns(mockTransactionEntities);

            _playerService = new PlayerService(mockDataService.Object);
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
    }
    
}
