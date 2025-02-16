using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class PlayerAtGameService : IPlayerAtGameService
{
    private readonly IEnumerable<PlayerAtGameEntity> _pagEntities;
    private readonly IDataService _dataService;
    private readonly IPlayerService _playerService;
    private readonly IGameService _gameService;

    public PlayerAtGameService(IDataService dataService, IGameService gameService, IPlayerService playerService)
    {
        _pagEntities = dataService.PlayerAtGameEntities.OrderByDescending(o => o.UrlSegment);
        _dataService = dataService;
        _playerService = playerService;
        _gameService = gameService;
    }

    private PlayerAtGame ConstructPag(string pagId)
    {
        // Verify Pag
        var pagEntity = VerifyPagEntityExists(pagId);
        
        // Get associated objects
        var player = _playerService.GetPlayer(pagEntity.PlayerRowKey);
        var game = _gameService.GetGame(pagEntity.GameRowKey);

        // Construct
        var pag = new PlayerAtGame()
        {
            Id = pagEntity.RowKey,
            PlayerId = pagEntity.PlayerRowKey,
            GameId = pagEntity.GameRowKey,
            Forecast = pagEntity.Forecast,
            Played = pagEntity.Played,
            Team = pagEntity.Team,
            PlayerLabel = player.Label,
            PlayerBalance = player.Balance,
            PlayerName = player.Name,
            PlayerPosition = player.Position,
            PlayerRating = player.Rating,
            PlayerAdminRating = player.AdminRating,
            PlayerDefaultRate = player.DefaultRate,
            PlayerGamesCount = player.GamesCount,
            PlayerUrlSegment = player.UrlSegment,
            GameLabel = game.Label,
            GameDateTime = game.DateTime,
            GameUrlSegment = game.UrlSegment,
            UrlSegment = $"{player.UrlSegment}-{game.UrlSegment}"
            //Label is filled in the PlayerAtGame class
        };
        
        return pag;
    }
    
    private PlayerAtGameEntity VerifyPagEntityExists(string pagId)
    {
        var pagEntity = _pagEntities.FirstOrDefault(page => page.RowKey == pagId);
        if (pagEntity == null)
        {
            throw new KeyNotFoundException($"The PlayerAtGame with Id {pagId} was not found.");
        }
        return pagEntity;
    }

    private PlayerAtGameEntity DeconstructPag(PlayerAtGame pag)
    {
        var pe = new PlayerAtGameEntity()
        {
            RowKey = pag.Id,
            PlayerRowKey = pag.PlayerId,
            GameRowKey = pag.GameId,
            Forecast = pag.Forecast,
            Played = pag.Played,
            Team = pag.Team,
            UrlSegment = pag.UrlSegment,
        };

        return pe;
    }

    public List<PlayerAtGame> GetPags(int? skip, int? take)
    {
        var pagEntities = _pagEntities;
        
        // Apply Skip and Take only if values are provided
        pagEntities = skip.HasValue ? pagEntities.Skip(skip.Value) : pagEntities;
        pagEntities = take.HasValue ? pagEntities.Take(take.Value) : pagEntities;

        var pags = pagEntities.Select(page => ConstructPag(page.RowKey)).ToList();

        return pags;
    }

    public List<PlayerAtGame> GetPagsForGame(string gameId)
    {
        var pags = _pagEntities
            .Where(page => page.GameRowKey == gameId)
            .Select(page => ConstructPag(page.RowKey)).ToList();
        return pags;
    }

    public List<PlayerAtGame> OrganiseTeams(List<PlayerAtGame> pags)
    {
        throw new NotImplementedException();
    }

    public async Task ResetTeamsAsync(List<PlayerAtGame> pags)
    {
        // Using a For loop because we are modifying the collection as we go
        var index = 0;
        for (; index < pags.Count; index++)
        {
            var pag = pags[index];
            pag.Team = string.Empty;
            await UpsertPagAsync(pag);
        }
    }

    public async Task TogglePagPlayedAsync(string pagId, bool played)
    {
        throw new NotImplementedException();
    }

    public PlayerAtGame GetPag(string id)
    {
        return ConstructPag(id);
    }

    public PlayerAtGame GetPagByUrlSegment(string urlSegment)
    {
        // Get PlayerAtGameEntity for this UrlSegment
        var page = _pagEntities.FirstOrDefault(page => page.UrlSegment.ToLowerInvariant() == urlSegment.ToLowerInvariant());

        if (page == null)
        {
            throw new KeyNotFoundException($"The PlayerAtGame with UrlSegment {urlSegment} was not found.");
        }
        
        return ConstructPag(page.RowKey);
    }

    public async Task DeletePagAsync(string id)
    {
        await _dataService.DeleteEntityAsync<PlayerAtGameEntity>(id);
    }

    public async Task UpsertPagAsync(PlayerAtGame pag)
    {
        if (pag.PlayerId is null) return;
        if (pag.GameId is null) return;
        
        //Deconstruct
        var pagEntity = DeconstructPag(pag);
        
        //Set UrlSegment
        pagEntity.UrlSegment = $"{pag.PlayerUrlSegment}-{pag.GameUrlSegment}";
        
        //Check if Pag exists with same GameId and PlayerID.
        //We could have Pags with different Id but same Player and Game if a Pag has been already added to Game
        var matchingPagEntity = _pagEntities.FirstOrDefault(page => page.UrlSegment == pagEntity.UrlSegment);
        if (matchingPagEntity is not null)
        {
            //Overwrite the PagEntity we created with a new one based on existing PagEntity details with new properties from incoming Pag
            pagEntity = matchingPagEntity;
            pagEntity.Team = pag.Team;
            pagEntity.Forecast = pag.Forecast;
            pagEntity.Played = pag.Played;
        }
        
        // Upsert pag
        await _dataService.UpsertEntityAsync(pagEntity);
    }
}