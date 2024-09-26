namespace STO.Wasm.Services;

public class PlayerService(IDataService dataService) : IPlayerService
{
    /// <inheritdoc />
    private List<PlayerEntity> GetPlayerEntities()
    {
        return [.. dataService.PlayerEntities.OrderBy(o => o.Name)];
    }

    private Player ConstructPlayer(string rowkey)
    {
        // PlayerEntity
        var playerEntity = GetPlayerEntities().First(pe => pe.RowKey == rowkey);
        
        // Ratings
        var playerRatings = dataService.RatingEntities.Where(r => r.PlayerRowKey == rowkey);
        var rating = playerRatings.Average(r => r.Rating);
        
        // Transactions
        
        // Construct
        var player = new Player()
        {
            Name = playerEntity.Name,
            Tags = playerEntity.Tags,
            Position = playerEntity.Position,
            DefaultRate = playerEntity.DefaultRate,
            AdminRating = playerEntity.AdminRating,
            Rating = (int)rating
        };

        return player;
    }

    public Player GetPlayer(string rowKey)
    {
        throw new NotImplementedException();
    }
}