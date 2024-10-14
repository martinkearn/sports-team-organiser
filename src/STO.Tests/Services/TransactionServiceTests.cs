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
            Assert.Equal("1", result.PlayerRowKey);
            Assert.Equal("G1", result.GameRowKey);
            Assert.Equal(3, result.Amount);
            Assert.Equal("ollie-watkins-01-20-2024-18-30-00", result.UrlSegment);
        }

        #endregion
        
    }
    
}
