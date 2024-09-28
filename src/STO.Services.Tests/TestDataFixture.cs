namespace STO.Services.Tests;

public class TestDataFixture
{
    public readonly List<PlayerEntity> MockPlayerEntities = [];
    public readonly List<RatingEntity> MockRatingEntities = [];
    public readonly List<TransactionEntity> MockTransactionEntities = [];
    public readonly List<PlayerAtGameEntity> MockPlayerAtGameEntities = [];
    public readonly List<GameEntity> MockGameEntities = [];
    public readonly Player PlayerWollyWatkins;

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
        MockRatingEntities.Add(new RatingEntity { RowKey = "R1", PlayerRowKey = "1", Rating = 4 });
        MockRatingEntities.Add(new RatingEntity { RowKey = "R2", PlayerRowKey = "1", Rating = 5 });
        MockRatingEntities.Add(new RatingEntity { RowKey = "R3", PlayerRowKey = "1", Rating = 3 });
        MockRatingEntities.Add(new RatingEntity { RowKey = "R4", PlayerRowKey = "1", Rating = 5 });// Average 4.25
        MockTransactionEntities.Add(new TransactionEntity { RowKey = "T1", PlayerRowKey = "1", Amount = 3 });
        MockTransactionEntities.Add(new TransactionEntity { RowKey = "T2", PlayerRowKey = "1", Amount = -3 });
        MockTransactionEntities.Add(new TransactionEntity { RowKey = "T3", PlayerRowKey = "1", Amount = 3 });// Total £3
        
        // Initialise mock data for updated Ollie Watkins to test update methods
        PlayerWollyWatkins = new Player()
        {
            Id = "1",
            Name = "Wolly Watkins",
            Tags = "",
            Position = Enums.PlayerPosition.Defender,
            DefaultRate = 3,
            AdminRating = 2,
            Label = "Wolly Watkins",
            Rating = 2,
            UrlSegment = "wolly-watkins",
            Balance = 3,
            GamesCount = 1
        };
        
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
        MockRatingEntities.Add(new RatingEntity { RowKey = "R5", PlayerRowKey = "2", Rating = 4 });
        MockRatingEntities.Add(new RatingEntity { RowKey = "R6", PlayerRowKey = "2", Rating = 4 });
        MockRatingEntities.Add(new RatingEntity { RowKey = "R7", PlayerRowKey = "2", Rating = 3 });
        MockRatingEntities.Add(new RatingEntity { RowKey = "R8", PlayerRowKey = "2", Rating = 3 });// Average 3.5
        MockTransactionEntities.Add(new TransactionEntity { RowKey = "T4", PlayerRowKey = "2", Amount = 3 });
        MockTransactionEntities.Add(new TransactionEntity { RowKey = "T5", PlayerRowKey = "2", Amount = -3 });// Total £0
            
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
        MockRatingEntities.Add(new RatingEntity { RowKey = "R9", PlayerRowKey = "3", Rating = 5 });// Average 3.5
        MockTransactionEntities.Add(new TransactionEntity { RowKey = "T6", PlayerRowKey = "3", Amount = 3 });// Total £0
            
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
            RowKey = "PAG1",
            GameRowKey = "G1",
            PlayerRowKey = "1",
            Forecast = Enums.PlayingStatus.Yes,
            Team = "A",
            Played = true,
            UrlSegment = "ollie-watkins-01-01-2024"
        });
        MockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
        {
            RowKey = "PAG2",
            GameRowKey = "G1",
            PlayerRowKey = "2",
            Forecast = Enums.PlayingStatus.Yes,
            Team = "B",
            Played = false,
            UrlSegment = "morgan-rogers-01-01-2024"
        });
        // Leon Bailey not in Game so not PlayerAtGame
    }
}