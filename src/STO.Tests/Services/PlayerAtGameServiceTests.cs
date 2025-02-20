using STO.Tests.Fixtures;

namespace STO.Tests.Services;

public class PlayerAtGameServiceTests : IClassFixture<MainFixture>
{
    private readonly Mock<IDataService> _mockDataService;
    private readonly Mock<IPlayerService> _mockPlayerService;
    private readonly Mock<IGameService> _mockGameService;
    private readonly Mock<ITransactionService> _mockTransactionService;
    private readonly PlayerAtGameService _service;
    
    public PlayerAtGameServiceTests(MainFixture fixture)
    {
        // Mocking IDataService with data from Fixture
        _mockDataService = new Mock<IDataService>();
        _mockPlayerService = new Mock<IPlayerService>();
        _mockGameService = new Mock<IGameService>();
        _mockTransactionService = new Mock<ITransactionService>();
        _mockDataService.Setup(ds => ds.PlayerEntities).Returns(fixture.PlayerEntities);
        _mockDataService.Setup(ds => ds.RatingEntities).Returns(fixture.RatingEntities);
        _mockDataService.Setup(ds => ds.TransactionEntities).Returns(fixture.TransactionEntities);
        _mockDataService.Setup(ds => ds.GameEntities).Returns(fixture.GameEntities);
        _mockDataService.Setup(ds => ds.PlayerAtGameEntities).Returns(fixture.PlayerAtGameEntities);

        // Create GameService with mocked IDataService
        _service = new PlayerAtGameService(_mockDataService.Object, _mockGameService.Object, _mockPlayerService.Object, _mockTransactionService.Object);
    }
    
    [Fact]
    public void GetPag_WhenExists_ReturnsPag()
    {
        // Act
        var result = _service.GetPag("PAG1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PAG1", result.Id);
        Assert.Equal("P1", result.PlayerId);
        Assert.Equal("G1", result.GameId);
        Assert.Equal(Enums.Team.A, result.Team);
    }
}