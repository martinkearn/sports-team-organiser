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
        public void GetTransaction_WithValidId_ReturnsTransaction()
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
            Assert.True(result.LastUpdated > DateTime.UtcNow.Subtract(new TimeSpan(0,0,1,0))); // was update more recently than now minus 1 minute 
        }
        
        [Fact]
        public void GetTransaction_WithMissingGameEntity_ReturnsTransactionWithoutGameEntity()
        {
            // Arrange
            const string transactionId = "T2";

            // Act
            var result = _transactionService.GetTransaction(transactionId);
            
            // Assert
            Assert.NotNull(result);
            Assert.Null(result.GameEntity); // Transaction T2 does not have a GameEntity
        }
        
        [Fact]
        public void GetTransaction_WithNegativeAmount_ReturnsTransaction()
        {
            // Arrange
            const string transactionId = "T2"; // Amount for T2 is -£3

            // Act
            var result = _transactionService.GetTransaction(transactionId);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(-3, result.Amount);
            Assert.Equal("ollie-watkins--3-20-01-2024-18-30-00", result.UrlSegment);
        }
        
        [Fact]
        public void GetTransaction_WithInvalidId_ThrowsException()
        {
            // Arrange
            const string transactionId = "invalid123";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _transactionService.GetTransaction(transactionId));
        }

        #endregion
        
        #region GetTransactions
        
        [Fact]
        public void GetTransactions_NoSkipOrTake_ReturnsAllTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, null);

            // Assert
            Assert.Equal(10, transactions.Count); // All transactions should be returned
        }

        [Fact]
        public void GetTransactions_WithSkip_ReturnsSkippedTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(5, null);

            // Assert
            Assert.Equal(5, transactions.Count); // 5 transactions after skipping 5
            Assert.Equal("T6", transactions[0].Id); // Verify the first returned transaction
        }

        [Fact]
        public void GetTransactions_WithTake_ReturnsLimitedTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, 3);

            // Assert
            Assert.Equal(3, transactions.Count); // Only 3 transactions should be returned
            Assert.Equal("T1", transactions[0].Id); // Verify the first returned transaction
        }

        [Fact]
        public void GetTransactions_WithSkipAndTake_ReturnsCorrectSubset()
        {
            // Act
            var transactions = _transactionService.GetTransactions(3, 4);

            // Assert
            Assert.Equal(4, transactions.Count); // 4 transactions after skipping 3
            Assert.Equal("T4", transactions[0].Id); // Verify the first returned transaction
        }

        [Fact]
        public void GetTransactions_SkipExceedsTotal_ReturnsEmptyList()
        {
            // Act
            var transactions = _transactionService.GetTransactions(15, null);

            // Assert
            Assert.Empty(transactions); // No transactions should be returned
        }

        [Fact]
        public void GetTransactions_TakeExceedsTotal_ReturnsAllTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, 20);

            // Assert
            Assert.Equal(10, transactions.Count); // All transactions should be returned
        }

        [Fact]
        public void GetTransactions_SkipAndTakeExceedTotal_ReturnsEmptyList()
        {
            // Act
            var transactions = _transactionService.GetTransactions(15, 20);

            // Assert
            Assert.Empty(transactions); // No transactions should be returned
        }
        
        #endregion
        
    }
    
}
