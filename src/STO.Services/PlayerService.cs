using System.Globalization;
using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class PlayerService : IPlayerService
{
    private readonly IDataService _dataService;

    public PlayerService(IDataService dataService)
    {
        _dataService = dataService;
    }

    /// <inheritdoc cref="IPlayerService" />
    private IEnumerable<PlayerEntity> GetPlayerEntities()
    {
        return [.. _dataService.PlayerEntities.OrderBy(o => o.Name)];
    }

    private Player ConstructPlayer(string playerId)
    {
        // PlayerEntity
        var playerEntity = GetPlayerEntities().First(pe => pe.RowKey == playerId);
        
        // RatingEntities
        var playerRatingEntities = _dataService.RatingEntities.Where(r => r.PlayerRowKey == playerId);
        var rating = playerRatingEntities.Average(r => r.Rating);
        
        // TransactionEntities
        var playerTransactionEntities = _dataService.TransactionEntities.Where(t => t.PlayerRowKey == playerId);
        var balance = playerTransactionEntities.Sum(o => o.Amount);
        
        // Label
        var textInfo = new CultureInfo("en-GB", false).TextInfo;
        var label = textInfo.ToTitleCase(playerEntity.Name);
        
        // Url Segment
        var urlSegment = playerEntity.Name.Replace(" ", "-").ToLowerInvariant();
        
        // Construct
        var player = new Player()
        {
            Id = playerEntity.RowKey,
            Name = playerEntity.Name,
            Tags = playerEntity.Tags,
            Position = playerEntity.Position,
            DefaultRate = playerEntity.DefaultRate,
            AdminRating = playerEntity.AdminRating,
            Rating = rating,
            Balance = balance,
            Label = label,
            UrlSegment= urlSegment
        };

        return player;
    }

    public Player GetPlayer(string id) => ConstructPlayer(id);
}