using STO.Tests.Fixtures;

namespace STO.Tests.Services
{
    public class TransactionServiceTests : IClassFixture<MainFixture>
    {
        private readonly TransactionService _transactionService;
        private readonly Mock<IDataService> _mockDataService;
        private readonly MainFixture _fixture;

        public TransactionServiceTests(MainFixture fixture)
        {
            _fixture = fixture;
            
            // Mocking IDataService with data from Fixture
            _mockDataService = new Mock<IDataService>();
            _mockDataService.Setup(ds => ds.PlayerEntities).Returns(_fixture.PlayerEntities);
            _mockDataService.Setup(ds => ds.RatingEntities).Returns(_fixture.RatingEntities);
            _mockDataService.Setup(ds => ds.TransactionEntities).Returns(_fixture.TransactionEntities);
            _mockDataService.Setup(ds => ds.GameEntities).Returns(_fixture.GameEntities);
            _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(_fixture.PlayerAtGameEntities);

            // Create PlayerService with mocked IDataService
            _transactionService = new TransactionService(_mockDataService.Object);
        }
        
        #region GetTransaction

        [Fact]
        public void GetTransaction_ShouldReturnTransactionWithCorrectDetails()
        {
            // Arrange
            const string transactionId = "T1";

            // Act
            var result = _transactionService.GetTransaction(transactionId);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
            Assert.Equal("1", result.PlayerEntity.RowKey);
            Assert.NotNull(result.GameEntity); // Transaction T1 does have a GameRowKey set which is G1
            Assert.Equal("G1", result.GameEntity.RowKey);
            Assert.Equal(3, result.Amount);
            Assert.Equal("ollie-watkins-3-20-01-2024-18-30-00", result.UrlSegment);
        }
        
        [Fact]
        public void GetTransaction_WithNoGame_ShouldReturnTransactionWithCorrectDetails()
        {
            // Arrange
            const string transactionId = "T2";

            // Act
            var result = _transactionService.GetTransaction(transactionId);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
            Assert.Equal("1", result.PlayerEntity.RowKey);
            Assert.Null(result.GameEntity); // Transaction T2 does now have a GameEntity
            Assert.Equal(-3, result.Amount); // Transaction T2 has a negative amount of -£3
            Assert.Equal("ollie-watkins--3-20-01-2024-18-30-00", result.UrlSegment);
        }

        #endregion
        
    }
    
}
