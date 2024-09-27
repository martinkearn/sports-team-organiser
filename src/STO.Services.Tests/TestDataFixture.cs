namespace STO.Services.Tests;

public class TestDataFixture
{
    public readonly List<PlayerEntity> MockPlayerEntities = [];
    public readonly List<RatingEntity> MockRatingEntities = [];
    public readonly List<TransactionEntity> MockTransactionEntities = [];
    public readonly List<PlayerAtGameEntity> MockPlayerAtGameEntities = [];
    public readonly List<GameEntity> MockGameEntities = [];

    public TestDataFixture()
    {
        // Initialize mock data for Ollie Watkins
        MockPlayerEntities.Add(
            new PlayerEntity
            {
                RowKey = "1",
                Name = "Ollie Watkins",
                Tags = "",
                Position = Enums.PlayerPosition.Forward,
                DefaultRate = 3,
                AdminRating = 5
            }
        );
        // Average 4.25
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 4 });
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 5 });
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 3 });
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "1", Rating = 5 });
        // Total £3
        MockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "1", Amount = 3 });
        MockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "1", Amount = -3 });
        MockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "1", Amount = 3 });
        
        // Initialize mock data for Morgan Rogers
        MockPlayerEntities.Add(
            new PlayerEntity
            {
                RowKey = "2",
                Name = "Morgan Rogers",
                Tags = "",
                Position = Enums.PlayerPosition.BoxToBox,
                DefaultRate = 3,
                AdminRating = 4
            }
        );
        // Average 3.5
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 4 });
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 4 });
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 3 });
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "2", Rating = 3 });
        // Total £0
        MockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "2", Amount = 3 });
        MockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "2", Amount = -3 });
            
        // Initialize mock data for Leon Baily
        MockPlayerEntities.Add(
            new PlayerEntity
            {
                RowKey = "3",
                Name = "Leon Bailey",
                Tags = "",
                Position = Enums.PlayerPosition.Forward,
                DefaultRate = 3,
                AdminRating = 4
            }
        );
        // Average 3.5
        MockRatingEntities.Add(new RatingEntity { PlayerRowKey = "3", Rating = 5 });
        // Total £0
        MockTransactionEntities.Add(new TransactionEntity { PlayerRowKey = "3", Amount = 3 });
            
        // Initialise mock Game data
        MockGameEntities.Add(new GameEntity()
        {
            RowKey = "G1",
            Date = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero),
            Notes = "Test game",
            TeamAGoals = 2,
            TeamBGoals = 1,
            Title = "Foo",
            UrlSegment = "01-01-2024"
        });
            
        // Initialise mock PLayerAtGame data
        MockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
        {
            GameRowKey = "G1",
            PlayerRowKey = "1",
            Forecast = Enums.PlayingStatus.Yes,
            Team = "A",
            Played = true,
            UrlSegment = "ollie-watkins-01-01-2024"
        });
        MockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
        {
            GameRowKey = "G1",
            PlayerRowKey = "2",
            Forecast = Enums.PlayingStatus.Yes,
            Team = "B",
            Played = false,
            UrlSegment = "morgan-rogers-01-01-2024"
        });
        // Leon Bailey not in Game
    }
}