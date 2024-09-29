namespace STO.Tests.Models;

public class PlayerTests
{
    [Fact]
    public void SettingName_ShouldUpdateUrlSegmentAndLabel()
    {
        // Arrange
        var player = new Player
        {
            // Act
            Name = "John Doe"
        };

        // Assert
        Assert.Equal("john-doe", player.UrlSegment); // Check if UrlSegment is set correctly
        Assert.Equal("John Doe", player.Label);      // Check if Label is set correctly
    }

    [Fact]
    public void Name_ShouldReturnCorrectValue()
    {
        // Arrange
        var player = new Player { Name = "John Doe" };

        // Act
        var name = player.Name;

        // Assert
        Assert.Equal("John Doe", name);              // Check if Name is returned correctly
    }

    [Fact]
    public void SettingId_ShouldUpdateId()
    {
        // Arrange
        var player = new Player
        {
            // Act
            Id = "123"
        };

        // Assert
        Assert.Equal("123", player.Id);              // Check if Id is set correctly
    }

    [Fact]
    public void SettingTags_ShouldUpdateTags()
    {
        // Arrange
        var player = new Player
        {
            // Act
            Tags = "Tag1,Tag2"
        };

        // Assert
        Assert.Equal("Tag1,Tag2", player.Tags);      // Check if Tags are set correctly
    }

    [Fact]
    public void SettingPosition_ShouldUpdatePosition()
    {
        // Arrange
        var player = new Player
        {
            // Act
            Position = Enums.PlayerPosition.Forward
        };

        // Assert
        Assert.Equal(Enums.PlayerPosition.Forward, player.Position);  // Check if Position is set correctly
    }

    [Fact]
    public void SettingDefaultRate_ShouldUpdateDefaultRate()
    {
        // Arrange
        var player = new Player
        {
            // Act
            DefaultRate = 9.5
        };

        // Assert
        Assert.Equal(9.5, player.DefaultRate);        // Check if DefaultRate is set correctly
    }

    [Fact]
    public void SettingAdminRating_ShouldUpdateAdminRating()
    {
        // Arrange
        var player = new Player
        {
            // Act
            AdminRating = 4
        };

        // Assert
        Assert.Equal(4, player.AdminRating);         // Check if AdminRating is set correctly
    }

    [Fact]
    public void SettingBalance_ShouldUpdateBalance()
    {
        // Arrange
        var player = new Player
        {
            // Act
            Balance = 100.50
        };

        // Assert
        Assert.Equal(100.50, player.Balance);        // Check if Balance is set correctly
    }

    [Fact]
    public void SettingGamesCount_ShouldUpdateGamesCount()
    {
        // Arrange
        var player = new Player
        {
            // Act
            GamesCount = 15
        };

        // Assert
        Assert.Equal(15, player.GamesCount);         // Check if GamesCount is set correctly
    }
}