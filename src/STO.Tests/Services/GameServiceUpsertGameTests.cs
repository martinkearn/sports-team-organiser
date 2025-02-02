using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class GameServiceUpsertGameTests : IClassFixture<MainFixture>
{
    private readonly GameService _gameService;
    private readonly Mock<IDataService> _mockDataService;

    public GameServiceUpsertGameTests(MainFixture fixture)
    {
        // Mocking IDataService with data from Fixture
        _mockDataService = new Mock<IDataService>();
        _mockDataService.Setup(ds => ds.PlayerEntities).Returns(fixture.PlayerEntities);
        _mockDataService.Setup(ds => ds.RatingEntities).Returns(fixture.RatingEntities);
        _mockDataService.Setup(ds => ds.TransactionEntities).Returns(fixture.TransactionEntities);
        _mockDataService.Setup(ds => ds.GameEntities).Returns(fixture.GameEntities);
        _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(fixture.PlayerAtGameEntities);

        // Create GameService with mocked IDataService
        _gameService = new GameService(_mockDataService.Object);
    }
    
    [Fact]
    public async Task UpsertGameAsync_ShouldCallUpsertEntityAsync_WithCorrectGameEntity()
    {
        // Arrange
        var dt = new DateTime(2024, 12, 25);
        var game = new Game { Id = "G5", DateTime = dt, TeamAGoals = 2, TeamBGoals = 3, Title = "Game 5", Notes = "Fifth Game", UrlSegment = "25-12-2024" };

        // Act
        await _gameService.UpsertGameAsync(game);

        // Assert
        _mockDataService.Verify(ds => ds.UpsertEntityAsync(It.Is<GameEntity>(g => g.RowKey == "G5")), Times.Once);
    }
    
}