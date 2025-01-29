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
            Assert.Equal("1", result.PlayerId);
            Assert.Equal("G1", result.GameId);
            Assert.Equal(3, result.Amount);
            Assert.Equal("ollie-watkins-20-01-2024-18-30-00", result.UrlSegment);
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
            Assert.Null(result.GameId); // Transaction T2 does not have a Game
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
            Assert.Equal("ollie-watkins-20-01-2024-18-30-00", result.UrlSegment);
        }
        
        [Fact]
        public void GetTransaction_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            const string transactionId = "invalid123";

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _transactionService.GetTransaction(transactionId));
        }

        #endregion
        
        #region GetTransactionByUrlSegment
        
        [Fact]
        public void GetTransactionByUrlSegment_WithValidUrlSegment_ReturnsTransaction()
        {
            // Arrange
            const string transactionUrlSegment = "ollie-watkins-20-01-2024-18-30-00";

            // Act
            var result = _transactionService.GetTransactionByUrlSegment(transactionUrlSegment);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionUrlSegment, result.UrlSegment);
            Assert.Equal("T1", result.Id);
            Assert.Equal("1", result.PlayerId);
            Assert.Equal("G1", result.GameId);
            Assert.Equal(3, result.Amount);
        }
        
        [Fact]
        public void GetTransactionByUrlSegment_WithInvalidUrlSegment_ThrowsException()
        {
            // Arrange
            const string transactionUrlSegment = "invalid123";

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _transactionService.GetTransactionByUrlSegment(transactionUrlSegment));
        }
        
        #endregion
        
        #region GetTransactions
        
        [Fact]
        public void GetTransactions_NoSkipOrTake_ReturnsAllTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, null, null);

            // Assert
            Assert.Equal(10, transactions.Count); // All transactions should be returned
        }

        [Fact]
        public void GetTransactions_WithSkip_ReturnsSkippedTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(5, null, null);

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
        
        [Fact]
        public void GetTransactions_WithPlayerId_ReturnsFilteredTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, null, "4");

            // Assert
            Assert.Equal(4, transactions.Count); // Only transactions for 4 should be returned
            Assert.All(transactions, t => Assert.Equal("4", t.PlayerId)); // Ensure all transactions belong to Jacob Ramsey (ID4)
        }

        [Fact]
        public void GetTransactions_WithInvalidPlayerId_ReturnsEmptyList()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, null, "invalidPlayer");

            // Assert
            Assert.Empty(transactions); // No transactions should be returned
        }

        [Fact]
        public void GetTransactions_WithPlayerIdAndSkip_ReturnsSkippedTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(2, null, "4");

            // Assert
            Assert.Equal(2, transactions.Count); // Only 2 transactions should be returned after skipping 2
            Assert.All(transactions, t => Assert.Equal("4", t.PlayerId)); // Ensure all transactions belong to Jacob Ramsey (ID4)
            Assert.Equal("T9", transactions[0].Id); // Verify the first returned transaction
        }

        [Fact]
        public void GetTransactions_WithPlayerIdAndTake_ReturnsLimitedTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, 3, "4");

            // Assert
            Assert.Equal(3, transactions.Count); // Only 3 transactions should be returned
            Assert.All(transactions, t => Assert.Equal("4", t.PlayerId)); // Ensure all transactions belong to Jacob Ramsey (ID4)
            Assert.Equal("T7", transactions[0].Id); // Verify the first returned transaction
        }

        [Fact]
        public void GetTransactions_WithPlayerIdSkipAndTake_ReturnsCorrectSubset()
        {
            // Act
            var transactions = _transactionService.GetTransactions(1, 2, "4");

            // Assert
            Assert.Equal(2, transactions.Count); // Only 4 transactions after skipping 3
            Assert.All(transactions, t => Assert.Equal("4", t.PlayerId)); // Ensure all transactions belong to Jacob Ramsey (ID4)
            Assert.Equal("T8", transactions[0].Id); // Verify the first returned transaction
        }

        [Fact]
        public void GetTransactions_WithPlayerIdAndSkipExceedsTotal_ReturnsEmptyList()
        {
            // Act
            var transactions = _transactionService.GetTransactions(15, null, "4");

            // Assert
            Assert.Empty(transactions); // No transactions should be returned
        }

        [Fact]
        public void GetTransactions_WithPlayerIdAndTakeExceedsTotal_ReturnsAllTransactions()
        {
            // Act
            var transactions = _transactionService.GetTransactions(null, 20, "4");

            // Assert
            Assert.Equal(4, transactions.Count); // All transactions should be returned
            Assert.All(transactions, t => Assert.Equal("4", t.PlayerId)); // Ensure all transactions belong to Jacob Ramsey (ID4)
        }
        
        #endregion

        #region DeleteTransaction

        [Fact]
        public async Task DeleteTransactionAsync_ShouldDeleteTransactionEntity()
        {
            // Arrange
            var transactionId = "1";

            // Act
            await _transactionService.DeleteTransactionAsync(transactionId);

            // Assert
            _mockDataService.Verify(ds => ds.DeleteEntityAsync<TransactionEntity>(transactionId), Times.Once);
        }

        #endregion
        
        #region UpsertTransactionAsync
        
        [Fact]
        public async Task UpsertTransactionAsync_ShouldCallUpsertEntityAsyncWithCorrectEntity()
        {
            // Arrange
            var expectedTransaction = _fixture.UpdatedJacobRamseyT10;

            // Act
            await _transactionService.UpsertTransactionAsync(expectedTransaction);

            // Assert
            // Verify that UpsertEntityAsync was called with the correct transformed TransactionEntity
            _mockDataService.Verify(ds => ds.UpsertEntityAsync(It.Is<TransactionEntity>(te => te.RowKey == "T10" && te.Amount == expectedTransaction.Amount && te.Notes == expectedTransaction.Notes)), Times.Once);
        }
        
        [Fact]
        public async Task UpsertTransaction_WhichIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Transaction nullTransaction = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _transactionService.UpsertTransactionAsync(nullTransaction));
        }
        
        [Fact]
        public async Task UpsertTransactionAsync_WhenAmountIsZero_ShouldNotCallUpsertEntityAsync()
        {
            // Arrange
            var transaction = new Transaction { Amount = 0 };

            var mockDataService = new Mock<IDataService>();
            var service = new TransactionService(mockDataService.Object);
        
            // Act
            await service.UpsertTransactionAsync(transaction);

            // Assert
            _mockDataService.Verify(ds => ds.UpsertEntityAsync(It.IsAny<TransactionEntity>()), Times.Never);
        }
        
        #endregion
        
    }
    
}
