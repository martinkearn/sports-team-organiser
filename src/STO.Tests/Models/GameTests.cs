namespace STO.Tests.Models;

public class GameTests
{
    [Fact]
    public void DateTime_ShouldSetUrlSegmentAndLabel_WhenValueIsSet()
    {
        // Arrange
        var game = new Game { Title = "Championship" };
        var date = new DateTime(2024, 5, 10);

        // Act
        game.DateTime = date;

        // Assert
        Assert.Equal("10-05-2024", game.UrlSegment);
        Assert.Equal("10 May Championship", game.Label);
    }

    [Fact]
    public void DateTime_ShouldSetOnlyDateLabel_WhenTitleIsNull()
    {
        // Arrange
        var game = new Game();
        var date = new DateTime(2024, 6, 15);

        // Act
        game.DateTime = date;

        // Assert
        Assert.Equal("15-06-2024", game.UrlSegment);
        Assert.Equal("15 Jun", game.Label);
    }

    [Fact]
    public void TeamAGoals_ShouldDefaultToZero()
    {
        // Arrange & Act
        var game = new Game();

        // Assert
        Assert.Equal(0, game.TeamAGoals);
    }

    [Fact]
    public void TeamBGoals_ShouldDefaultToZero()
    {
        // Arrange & Act
        var game = new Game();

        // Assert
        Assert.Equal(0, game.TeamBGoals);
    }

    [Fact]
    public void Id_ShouldNotBeNull()
    {
        // Arrange & Act
        var game = new Game { Id = "12345" };

        // Assert
        Assert.NotNull(game.Id);
        Assert.Equal("12345", game.Id);
    }

    [Fact]
    public void Notes_ShouldBeSetCorrectly()
    {
        // Arrange
        var game = new Game { Notes = "This is a test note." };

        // Act & Assert
        Assert.Equal("This is a test note.", game.Notes);
    }
}