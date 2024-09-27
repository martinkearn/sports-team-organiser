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
        var playerEntity = GetPlayerEntities().FirstOrDefault(pe => pe.RowKey == playerId);

        if (playerEntity == default)
        {
            throw new KeyNotFoundException($"The object with ID {playerId} was not found.");
        }

        // RatingEntities
        var playerRatingEntities = _dataService.RatingEntities.Where(r => r.PlayerRowKey == playerId);
        var playerRatingEntitiesList = playerRatingEntities.ToList();
        var rating = (playerRatingEntitiesList.Any()) ? playerRatingEntitiesList.Average(r => r.Rating) : 0;
        
        // TransactionEntities
        var playerTransactionEntities = _dataService.TransactionEntities.Where(t => t.PlayerRowKey == playerId);
        var balance = playerTransactionEntities.Sum(o => o.Amount);
        
        // PlayerAtGameEntities
        var pagEntities = _dataService.PlayerAtGameEntities
            .Where(p => p.PlayerRowKey == playerId)
            .Where(p => p.Played == true);
        var pagCount = pagEntities.Count();
        
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
            UrlSegment= urlSegment,
            GamesCount = pagCount
        };

        return player;
    }

    private PlayerEntity DeconstructPlayer(Player player)
    {
        var pe = new PlayerEntity
        {
            DefaultRate = player.DefaultRate,
            AdminRating = player.AdminRating,
            Position = player.Position,
            Name = player.Name,
            RowKey = player.Id,
            Tags = player.Tags,
            UrlSegment = player.UrlSegment
        };
        return pe;
    }

    public List<Player> GetPlayers()
    {
        var playerEntities = GetPlayerEntities();
        var players = playerEntities.Select(pe => ConstructPlayer(pe.RowKey)).ToList();
        return players.OrderBy(p => p.Name).ToList();
    }

    public List<Player> GetPlayers(string gameId)
    {
        var pagEntities = _dataService.PlayerAtGameEntities;
        var pags = pagEntities
            .Where(page => page.GameRowKey == gameId)
            .Select(page => ConstructPlayer(page.PlayerRowKey)).ToList();
        return pags.OrderBy(p => p.Name).ToList();
    }

    public Player GetPlayer(string id)
    {
        return ConstructPlayer(id);
    }

    public async Task DeletePlayerAsync(string id)
    {
        // Delete Ratings
        var allRatingEntities = await _dataService.QueryEntitiesAsync<RatingEntity>();
        var ratingsForPlayer = allRatingEntities.Where(o => o.PlayerRowKey == id).ToList();
        foreach (var rating in ratingsForPlayer)
        {
            await _dataService.DeleteEntityAsync<RatingEntity>(rating.RowKey);
        }

        // Delete TransactionEntity
        var transactionsResult = await _dataService.QueryEntitiesAsync<TransactionEntity>();
        var transactions = transactionsResult.Where(t => t.PlayerRowKey == id);
        foreach (var transaction in transactions)
        {
            await _dataService.DeleteEntityAsync<TransactionEntity>(transaction.RowKey);
        }

        // Delete PlayerAtGameEntity
        var pagsResult = await _dataService.QueryEntitiesAsync<PlayerAtGameEntity>();
        var pags = pagsResult.Where(pag => pag.PlayerRowKey == id);
        foreach (var pag in pags)
        {
            await _dataService.DeleteEntityAsync<PlayerAtGameEntity>(pag.RowKey);
        }

        // Delete PlayerEntity
        await _dataService.DeleteEntityAsync<PlayerEntity>(id);
    }

    public async Task UpsertPlayerAsync(Player player)
    {
        var pe = DeconstructPlayer(player);
        await _dataService.UpsertEntityAsync(pe);
    }
}