namespace STO.Tests.Services;

public class PlayerServiceFixture
{
    public readonly List<PlayerEntity> MockPlayerEntities = [];
    public readonly List<RatingEntity> MockRatingEntities = [];
    public readonly List<TransactionEntity> MockTransactionEntities = [];
    public readonly List<PlayerAtGameEntity> MockPlayerAtGameEntities = [];
    public readonly List<GameEntity> MockGameEntities = [];
    public readonly Player PlayerWollyWatkins;

    public PlayerServiceFixture()
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
           
        // Initialize mock data for Jacob Ramsey
        MockPlayerEntities.Add(
            new PlayerEntity
            {
                RowKey = "4",
                Name = "Jacob Ramsey",
                Tags = "",
                Position = Enums.PlayerPosition.BoxToBox,
                DefaultRate = 3,
                AdminRating = 3
            }
        );
        
        // Initialise mock Game data
        MockGameEntities.Add(new GameEntity()
        {
            RowKey = "G1",
            Date = new DateTimeOffset(2024, 6, 10, 18, 30, 0, TimeSpan.Zero),
            Notes = "Test game G1",
            TeamAGoals = 2,
            TeamBGoals = 1,
            Title = "Foo",
            UrlSegment = "10-06-2024"
        });
        MockGameEntities.Add(new GameEntity()
        {
            RowKey = "G2",
            Date = new DateTimeOffset(2024, 5, 10, 18, 30, 0, TimeSpan.Zero),
            Notes = "Test game G2",
            Title = "",
            UrlSegment = "10-05-2024"
        });
        MockGameEntities.Add(new GameEntity()
        {
            RowKey = "G3",
            Date = new DateTimeOffset(2024, 4, 10, 18, 30, 0, TimeSpan.Zero),
            Notes = "Test game G3",
            TeamAGoals = 0,
            TeamBGoals = 0,
            Title = "",
            UrlSegment = "10-04-2024"
        });
        MockGameEntities.Add(new GameEntity()
        {
            RowKey = "G4",
            Date = new DateTimeOffset(2024, 3, 10, 18, 30, 0, TimeSpan.Zero),
            Notes = "Test game G4",
            TeamAGoals = 0,
            TeamBGoals = 0,
            Title = "",
            UrlSegment = "10-03-2024"
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
            UrlSegment = "ollie-watkins-10-06-2024"
        });
        MockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
        {
            RowKey = "PAG2",
            GameRowKey = "G1",
            PlayerRowKey = "2",
            Forecast = Enums.PlayingStatus.Yes,
            Team = "B",
            Played = false,
            UrlSegment = "morgan-rogers-10-06-2024"
        });        
        MockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
        {
            RowKey = "PAG3",
            GameRowKey = "G2",
            PlayerRowKey = "3",
            Forecast = Enums.PlayingStatus.Yes,
            Played = false,
            UrlSegment = "leon-bailey-10-05-2024"
        });     
        MockPlayerAtGameEntities.Add(new PlayerAtGameEntity()
        {
            RowKey = "PAG4",
            GameRowKey = "G3",
            PlayerRowKey = "4",
            Forecast = Enums.PlayingStatus.Yes,
            Played = false,
            UrlSegment = "jacob-ramsey-10-04-2024"
        });
    }
}